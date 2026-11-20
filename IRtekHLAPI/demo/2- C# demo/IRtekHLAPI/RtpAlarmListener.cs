using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace IRtekHLAPI
{
    /// <summary>
    /// RTP alarm listener for <c>net_start_alarm_listener</c>.
    /// Handshake is <c>GET /session?role=alarm</c> on the device RTP port (api.md local extension).
    /// </summary>
    internal sealed class RtpAlarmListener : IDisposable
    {
        private const byte FrameMagic = 0x24;
        private const int FrameHdrSize = 8;
        private const int PayloadHdrSize = 24;
        private const uint MsgAlarm = 2;
        private const int AlarmBodyHdrSize = 16;
        private const int DeviceInfoPackedSize = 70;
        private const int DeviceInfoAlignedSize = 72;
        private const int MarkerAlarmEntrySize = 68;
        private const int MaxPayloadBytes = 32 * 1024 * 1024;

        private readonly HlApiSession _session;
        private readonly string _host;
        private readonly ushort _rtpPort;
        private readonly NetSdkNative.OnNetAlarmCallback _callback;
        private readonly object _pinLock = new object();

        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _thread;
        private volatile bool _running;
        private bool _disposed;
        private readonly List<byte> _rx = new List<byte>(4096);

        private byte[] _pixelBytes;
        private byte[] _lutTempBytes;
        private byte[] _lutAdBytes;
        private byte[] _messageBytes;
        private GCHandleHolder _pixelPin;
        private GCHandleHolder _lutTempPin;
        private GCHandleHolder _lutAdPin;
        private GCHandleHolder _messagePin;

        public int SessionHandleId { get; private set; }

        public RtpAlarmListener(
            int sessionHandleId,
            HlApiSession session,
            string host,
            ushort rtpPort,
            NetSdkNative.OnNetAlarmCallback callback)
        {
            SessionHandleId = sessionHandleId;
            _session = session;
            _host = host;
            _rtpPort = rtpPort;
            _callback = callback;
        }

        public NetSdkNative.NetErrorCode Start()
        {
            if (_disposed)
                return NetSdkNative.NetErrorCode.NotInitialized;
            if (string.IsNullOrEmpty(_host) || _rtpPort == 0 || _callback == null)
                return NetSdkNative.NetErrorCode.InvalidParam;

            try
            {
                _client = new TcpClient();
                if (!Connect(_client, _host, _rtpPort, _session.TimeoutMs))
                    return NetSdkNative.NetErrorCode.Timeout;

                _stream = _client.GetStream();
                _stream.ReadTimeout = Timeout.Infinite;
                _stream.WriteTimeout = Timeout.Infinite;

                var hs = Handshake(_stream, _host, _rtpPort, _session);
                if (hs != NetSdkNative.NetErrorCode.OK)
                {
                    CloseSocket();
                    return hs;
                }

                _running = true;
                _thread = new Thread(ReceiveLoop)
                {
                    IsBackground = true,
                    Name = "RtpAlarmListener"
                };
                _thread.Start();
                return NetSdkNative.NetErrorCode.OK;
            }
            catch (SocketException)
            {
                CloseSocket();
                return NetSdkNative.NetErrorCode.HttpError;
            }
            catch (IOException)
            {
                CloseSocket();
                return NetSdkNative.NetErrorCode.HttpError;
            }
            catch
            {
                CloseSocket();
                return NetSdkNative.NetErrorCode.Fail;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _running = false;
            CloseSocket();
            if (_thread != null)
            {
                try
                {
                    if (!_thread.Join(2000))
                        _thread.Interrupt();
                }
                catch
                {
                }
                _thread = null;
            }
            ClearPins();
        }

        private void ReceiveLoop()
        {
            var tmp = new byte[4096];
            while (_running && _stream != null)
            {
                int n;
                try
                {
                    n = _stream.Read(tmp, 0, tmp.Length);
                }
                catch (IOException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch
                {
                    if (!_running)
                        break;
                    continue;
                }

                if (n <= 0)
                    break;

                for (int i = 0; i < n; i++)
                    _rx.Add(tmp[i]);
                PumpFrames();
            }
        }

        private void PumpFrames()
        {
            while (_rx.Count >= 4)
            {
                int magicAt = -1;
                for (int i = 0; i < _rx.Count; i++)
                {
                    if (_rx[i] == FrameMagic)
                    {
                        magicAt = i;
                        break;
                    }
                }
                if (magicAt < 0)
                {
                    _rx.Clear();
                    return;
                }
                if (magicAt > 0)
                    _rx.RemoveRange(0, magicAt);

                int hdrSize;
                int payloadSize;
                if (!TryPeekFrameSize(_rx, out hdrSize, out payloadSize))
                    return;

                if (payloadSize <= 0 || payloadSize > MaxPayloadBytes)
                {
                    _rx.RemoveAt(0);
                    continue;
                }

                int total = hdrSize + payloadSize;
                if (_rx.Count < total)
                    return;

                var payload = new byte[payloadSize];
                _rx.CopyTo(hdrSize, payload, 0, payloadSize);
                _rx.RemoveRange(0, total);
                TryDispatchPayload(payload);
            }
        }

        private static bool TryPeekFrameSize(List<byte> rx, out int hdrSize, out int payloadSize)
        {
            hdrSize = FrameHdrSize;
            payloadSize = 0;
            if (rx.Count < FrameHdrSize)
                return false;

            // TCP frame: 0x24 + 3 reserved bytes + big-endian payload length.
            int size32be = (rx[4] << 24) | (rx[5] << 16) | (rx[6] << 8) | rx[7];
            if (size32be > 0 && size32be <= MaxPayloadBytes)
            {
                hdrSize = FrameHdrSize;
                payloadSize = size32be;
                return true;
            }

            return false;
        }

        private void TryDispatchPayload(byte[] payload)
        {
            if (payload == null || payload.Length < PayloadHdrSize)
                return;

            uint msgType = ReadU32(payload, 8);
            uint bodyLen = ReadU32(payload, 12);
            if (msgType != MsgAlarm)
                return;
            if (bodyLen == 0 || (long)PayloadHdrSize + bodyLen > payload.Length)
                return;

            ulong frameTs = ReadU64(payload, 16);
            DecodeAndInvoke(payload, PayloadHdrSize, (int)bodyLen, frameTs);
        }

        private void DecodeAndInvoke(byte[] body, int offset, int length, ulong frameTs)
        {
            if (length < 4)
                return;

            var alarmType = (NetSdkNative.NetAlarmType)ReadU32(body, offset);
            if (alarmType != NetSdkNative.NetAlarmType.ThermalTemp && alarmType != 0)
            {
                // Keep going if the payload still looks like a thermal alarm body.
                if (IndexOfTdat(body, offset, offset + length) < 0 && length < AlarmBodyHdrSize)
                    return;
                alarmType = NetSdkNative.NetAlarmType.ThermalTemp;
            }
            else
            {
                alarmType = NetSdkNative.NetAlarmType.ThermalTemp;
            }

            ulong timestamp = length >= 12 ? ReadU64(body, offset + 4) : 0;
            if (timestamp == 0)
                timestamp = frameTs;

            int afterHdr = offset + Math.Min(AlarmBodyHdrSize, length);
            int end = offset + length;
            if (afterHdr > end)
                return;

            int thermalStart;
            NetSdkNative.NetAlarmDeviceInfo devInfo;
            ThermalDataParser.ParsedThermalData parsed;
            if (!TryLocateThermalPayload(body, afterHdr, end, out thermalStart, out devInfo, out parsed))
                return;
            if (parsed != null && parsed.TimestampUs != 0)
                timestamp = parsed.TimestampUs;

            var markerAlarms = CreateMarkerAlarmBuffer();
            ushort alarmCount;
            byte[] messageBytes;
            TryReadAlarmsAndMessage(body, thermalStart, end, parsed, markerAlarms, out alarmCount, out messageBytes);

            var thermal = new NetSdkNative.NetAlarmThermalTempInfo();
            thermal.timestamp_us = timestamp;
            thermal.dev_info = devInfo;
            thermal.marker_alarms = markerAlarms;
            thermal.marker_alarms_count = alarmCount;
            thermal.marker_temps = CreateMarkerTempBuffer();
            if (parsed != null)
            {
                thermal.global_temp_info = parsed.Global;
                thermal.markers_count = parsed.MarkersCount;
                if (parsed.Markers != null && parsed.MarkersCount > 0)
                    Array.Copy(parsed.Markers, thermal.marker_temps, parsed.MarkersCount);
            }

            EnsureMarkerArrays(ref thermal);

            lock (_pinLock)
            {
                ClearPins();
                if (parsed != null && parsed.HasRaw)
                    ApplyRawFrame(parsed, ref thermal);
                if (messageBytes != null && messageBytes.Length > 0)
                {
                    _messageBytes = messageBytes;
                    _messagePin = new GCHandleHolder(_messageBytes);
                    thermal.message_length = (uint)_messageBytes.Length;
                    thermal.message = _messagePin.AddrOfPinnedObject();
                }

                InvokeCallback(alarmType, ref thermal);
            }
        }

        private static NetSdkNative.NetMarkerAlarmInfo[] CreateMarkerAlarmBuffer()
        {
            var arr = new NetSdkNative.NetMarkerAlarmInfo[NetSdkNative.NET_MAX_MARKERS];
            for (int i = 0; i < arr.Length; i++)
                arr[i].marker_name = new byte[NetSdkNative.NET_LEN_64];
            return arr;
        }

        private static NetSdkNative.NetMarkerTempInfo[] CreateMarkerTempBuffer()
        {
            var arr = new NetSdkNative.NetMarkerTempInfo[NetSdkNative.NET_MAX_MARKERS];
            for (int i = 0; i < arr.Length; i++)
                arr[i].marker_name = new byte[NetSdkNative.NET_LEN_64];
            return arr;
        }

        private static void EnsureMarkerArrays(ref NetSdkNative.NetAlarmThermalTempInfo thermal)
        {
            if (thermal.marker_alarms == null)
                thermal.marker_alarms = CreateMarkerAlarmBuffer();
            if (thermal.marker_temps == null)
                thermal.marker_temps = CreateMarkerTempBuffer();
            for (int i = 0; i < NetSdkNative.NET_MAX_MARKERS; i++)
            {
                if (thermal.marker_alarms[i].marker_name == null)
                    thermal.marker_alarms[i].marker_name = new byte[NetSdkNative.NET_LEN_64];
                if (thermal.marker_temps[i].marker_name == null)
                    thermal.marker_temps[i].marker_name = new byte[NetSdkNative.NET_LEN_64];
            }
            if (thermal.dev_info.ip == null)
                thermal.dev_info.ip = new byte[NetSdkNative.NET_LEN_64];
        }

        private static int IndexOfTdat(byte[] body, int start, int end)
        {
            if (body == null || start < 0)
                return -1;
            int last = Math.Min(end, body.Length) - 4;
            for (int i = start; i <= last; i++)
            {
                if (ReadU32(body, i) == ThermalDataParser.TdatMagic)
                    return i;
            }
            return -1;
        }

        private static bool TryLocateThermalPayload(
            byte[] body,
            int afterHdr,
            int end,
            out int thermalStart,
            out NetSdkNative.NetAlarmDeviceInfo devInfo,
            out ThermalDataParser.ParsedThermalData parsed)
        {
            thermalStart = afterHdr;
            parsed = null;
            int remaining = end - afterHdr;
            int tdatAt = IndexOfTdat(body, afterHdr, end);
            int packedUsed;

            if (tdatAt >= afterHdr)
            {
                int gap = tdatAt - afterHdr;
                int devSize;
                if (gap < DeviceInfoPackedSize
                    || !TryReadDeviceInfo(body, afterHdr, gap, out devInfo, out devSize))
                {
                    devInfo = NetSdkNative.NetAlarmDeviceInfo.Create();
                    return false;
                }
                thermalStart = tdatAt;
                remaining = end - thermalStart;
                ThermalDataParser.TryParsePackedThermalContext(body, thermalStart, remaining, out parsed, out packedUsed);
                return true;
            }

            NetSdkNative.NetAlarmDeviceInfo packedInfo;
            NetSdkNative.NetAlarmDeviceInfo alignedInfo;
            bool packedOk = TryReadDeviceInfoAt(body, afterHdr, DeviceInfoPackedSize, remaining, out packedInfo);
            bool alignedOk = TryReadDeviceInfoAt(body, afterHdr, DeviceInfoAlignedSize, remaining, out alignedInfo);

            ThermalDataParser.ParsedThermalData packedParsed = null;
            ThermalDataParser.ParsedThermalData alignedParsed = null;
            int packedUsed70;
            int packedUsed72;
            bool packedRaw = packedOk
                && ThermalDataParser.TryParsePackedThermalContext(
                    body, afterHdr + DeviceInfoPackedSize, remaining - DeviceInfoPackedSize, out packedParsed, out packedUsed70)
                && packedParsed != null && packedParsed.HasRaw;
            bool alignedRaw = alignedOk
                && ThermalDataParser.TryParsePackedThermalContext(
                    body, afterHdr + DeviceInfoAlignedSize, remaining - DeviceInfoAlignedSize, out alignedParsed, out packedUsed72)
                && alignedParsed != null && alignedParsed.HasRaw;

            if (packedRaw)
            {
                devInfo = packedInfo;
                thermalStart = afterHdr + DeviceInfoPackedSize;
                parsed = packedParsed;
                return true;
            }
            if (alignedRaw)
            {
                devInfo = alignedInfo;
                thermalStart = afterHdr + DeviceInfoAlignedSize;
                parsed = alignedParsed;
                return true;
            }

            if (packedOk)
            {
                devInfo = packedInfo;
                thermalStart = afterHdr + DeviceInfoPackedSize;
                parsed = packedParsed;
                return true;
            }

            devInfo = NetSdkNative.NetAlarmDeviceInfo.Create();
            return false;
        }

        private static void TryReadAlarmsAndMessage(
            byte[] body,
            int thermalStart,
            int end,
            ThermalDataParser.ParsedThermalData parsed,
            NetSdkNative.NetMarkerAlarmInfo[] dest,
            out ushort alarmCount,
            out byte[] messageBytes)
        {
            alarmCount = 0;
            messageBytes = null;
            if (body == null || dest == null || thermalStart < 0 || end > body.Length || thermalStart >= end)
                return;

            int cursor = thermalStart;
            if (parsed != null)
            {
                int packedUsed;
                ThermalDataParser.ParsedThermalData ignored;
                if (ThermalDataParser.TryParsePackedThermalContext(body, thermalStart, end - thermalStart, out ignored, out packedUsed)
                    && packedUsed > 0)
                    cursor = thermalStart + packedUsed;
            }

            int used;
            if (TryReadMarkerAlarms(body, cursor, end - cursor, dest, out alarmCount, out used)
                && LooksLikeMarkerAlarms(body, cursor, used))
            {
                cursor += used;
                TryReadMessage(body, cursor, end, out messageBytes);
                if (alarmCount > 0)
                    return;
            }

            if (TryFindAlarmsBeforeMessage(body, thermalStart, end, dest, out alarmCount, out messageBytes)
                && alarmCount > 0)
                return;

            if (messageBytes == null)
                TryReadMessage(body, cursor, end, out messageBytes);
            if (alarmCount == 0 && messageBytes != null)
                TryParseAlarmsFromMessage(messageBytes, dest, out alarmCount);
        }

        private static bool TryFindAlarmsBeforeMessage(
            byte[] body,
            int start,
            int end,
            NetSdkNative.NetMarkerAlarmInfo[] dest,
            out ushort alarmCount,
            out byte[] messageBytes)
        {
            alarmCount = 0;
            messageBytes = null;
            int maxMsg = Math.Min(512, Math.Max(0, end - start - 4));
            for (int msgLen = 0; msgLen <= maxMsg; msgLen++)
            {
                int msgHdr = end - 4 - msgLen;
                if (msgHdr < start)
                    continue;
                if (ReadU32(body, msgHdr) != (uint)msgLen)
                    continue;
                if (msgLen > 0 && !LooksLikeMessageText(body, msgHdr + 4, msgLen))
                    continue;

                int regionStart = Math.Max(start, msgHdr - (2 + NetSdkNative.NET_MAX_MARKERS * MarkerAlarmEntrySize));
                for (int i = regionStart; i + 2 <= msgHdr; i++)
                {
                    int remaining = msgHdr - i;
                    int used;
                    ushort count;
                    if (!TryReadMarkerAlarms(body, i, remaining, dest, out count, out used))
                        continue;
                    if (used != remaining || count == 0 || !LooksLikeMarkerAlarms(body, i, used))
                        continue;

                    alarmCount = count;
                    if (msgLen > 0)
                    {
                        int copy = Math.Min(msgLen, 4096);
                        messageBytes = new byte[copy];
                        Buffer.BlockCopy(body, msgHdr + 4, messageBytes, 0, copy);
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool TryReadMessage(byte[] body, int offset, int end, out byte[] messageBytes)
        {
            messageBytes = null;
            if (offset < 0 || end - offset < 4)
                return false;
            uint messageLen = ReadU32(body, offset);
            int remaining = end - offset - 4;
            if (messageLen == 0 || messageLen > 4096 || remaining < messageLen)
                return false;
            if (!LooksLikeMessageText(body, offset + 4, (int)messageLen))
                return false;
            int copy = (int)messageLen;
            messageBytes = new byte[copy];
            Buffer.BlockCopy(body, offset + 4, messageBytes, 0, copy);
            return true;
        }

        private static bool LooksLikeMessageText(byte[] body, int offset, int length)
        {
            if (length <= 0 || offset < 0 || offset + length > body.Length)
                return false;
            int printable = 0;
            for (int i = 0; i < length; i++)
            {
                byte b = body[offset + i];
                if (b == 0)
                    break;
                if (b < 32 || b > 126)
                    return false;
                printable++;
            }
            return printable > 0;
        }

        private static bool TryParseAlarmsFromMessage(
            byte[] messageBytes,
            NetSdkNative.NetMarkerAlarmInfo[] dest,
            out ushort alarmCount)
        {
            alarmCount = 0;
            if (messageBytes == null || dest == null)
                return false;

            string text = Encoding.UTF8.GetString(messageBytes).Trim('\0', ' ', '\r', '\n');
            if (string.IsNullOrEmpty(text))
                return false;

            string[] parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ushort n = 0;
            for (int i = 0; i < parts.Length && n < dest.Length; i++)
            {
                string part = parts[i];
                int sep = part.LastIndexOf(':');
                if (sep <= 0 || sep >= part.Length - 1)
                    continue;

                string name = part.Substring(0, sep).Trim();
                string levelText = part.Substring(sep + 1).Trim();
                NetSdkNative.NetThermalTempAlarmLevel level;
                if (string.IsNullOrEmpty(name) || !TryParseAlarmLevel(levelText, out level))
                    continue;

                dest[n].marker_name = NetSdkNative.StringToFixedBytes(name, NetSdkNative.NET_LEN_64);
                dest[n].alarm_level = level;
                n++;
            }

            alarmCount = n;
            return n > 0;
        }

        private static bool TryParseAlarmLevel(string text, out NetSdkNative.NetThermalTempAlarmLevel level)
        {
            level = NetSdkNative.NetThermalTempAlarmLevel.None;
            if (string.IsNullOrEmpty(text))
                return false;
            if (text.Equals("Alert", StringComparison.OrdinalIgnoreCase))
            {
                level = NetSdkNative.NetThermalTempAlarmLevel.Alert;
                return true;
            }
            if (text.Equals("Warning", StringComparison.OrdinalIgnoreCase))
            {
                level = NetSdkNative.NetThermalTempAlarmLevel.Warning;
                return true;
            }
            if (text.Equals("Alarm", StringComparison.OrdinalIgnoreCase))
            {
                level = NetSdkNative.NetThermalTempAlarmLevel.Alarm;
                return true;
            }
            if (text.Equals("None", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private static bool LooksLikeMarkerAlarms(byte[] body, int offset, int remaining)
        {
            if (remaining < 2)
                return false;
            ushort n = ReadU16(body, offset);
            if (n == 0 || n > NetSdkNative.NET_MAX_MARKERS)
                return false;
            int needed = 2 + n * MarkerAlarmEntrySize;
            if (remaining < needed)
                return false;

            int entry = offset + 2;
            for (int i = 0; i < n; i++)
            {
                if (!ThermalDataParser.LooksLikePackedName(body, entry))
                    return false;
                uint level = ReadU32(body, entry + NetSdkNative.NET_LEN_64);
                if (level > (uint)NetSdkNative.NetThermalTempAlarmLevel.Alarm)
                    return false;
                entry += MarkerAlarmEntrySize;
            }
            return true;
        }

        private void ApplyRawFrame(ThermalDataParser.ParsedThermalData parsed, ref NetSdkNative.NetAlarmThermalTempInfo thermal)
        {
            thermal.raw_frame.width = parsed.Width;
            thermal.raw_frame.height = parsed.Height;
            thermal.raw_frame.range_id = parsed.RangeId;
            thermal.raw_frame.points_count = parsed.PointsCount;
            thermal.raw_frame.lut_len = parsed.LutLen;
            thermal.raw_frame.lut_env_temp = parsed.LutEnvTemp;
            thermal.raw_frame.sensor_temp = parsed.SensorTemp;
            thermal.raw_frame.shutter_state = parsed.ShutterState;
            thermal.raw_frame.frame_no = parsed.FrameNo;
            thermal.raw_frame.pts = parsed.Pts;
            thermal.raw_frame.raw_type = parsed.RawType;
            thermal.raw_frame.shutter_adv = parsed.ShutterAdv;

            if (parsed.Pixels != null && parsed.Pixels.Length > 0)
            {
                _pixelBytes = new byte[parsed.Pixels.Length * 2];
                Buffer.BlockCopy(parsed.Pixels, 0, _pixelBytes, 0, _pixelBytes.Length);
                _pixelPin = new GCHandleHolder(_pixelBytes);
                thermal.raw_frame.data = _pixelPin.AddrOfPinnedObject();
            }
            if (parsed.LutTemp != null && parsed.LutTemp.Length > 0)
            {
                _lutTempBytes = new byte[parsed.LutTemp.Length * 4];
                Buffer.BlockCopy(parsed.LutTemp, 0, _lutTempBytes, 0, _lutTempBytes.Length);
                _lutTempPin = new GCHandleHolder(_lutTempBytes);
                thermal.raw_frame.lut_temp_value = _lutTempPin.AddrOfPinnedObject();
            }
            if (parsed.LutAd != null && parsed.LutAd.Length > 0)
            {
                _lutAdBytes = new byte[parsed.LutAd.Length * 2];
                Buffer.BlockCopy(parsed.LutAd, 0, _lutAdBytes, 0, _lutAdBytes.Length);
                _lutAdPin = new GCHandleHolder(_lutAdBytes);
                thermal.raw_frame.lut_ad_value = _lutAdPin.AddrOfPinnedObject();
            }
        }

        private void InvokeCallback(NetSdkNative.NetAlarmType type, ref NetSdkNative.NetAlarmThermalTempInfo thermal)
        {
            int thermalSize = Marshal.SizeOf(typeof(NetSdkNative.NetAlarmThermalTempInfo));
            int alarmSize = Marshal.SizeOf(typeof(NetSdkNative.NetAlarmInfo));
            IntPtr thermalPtr = Marshal.AllocHGlobal(thermalSize);
            IntPtr alarmPtr = Marshal.AllocHGlobal(alarmSize);
            try
            {
                Marshal.StructureToPtr(thermal, thermalPtr, false);
                var alarm = new NetSdkNative.NetAlarmInfo
                {
                    type = type,
                    alarm_info_length = (uint)thermalSize,
                    alarm_info = thermalPtr
                };
                Marshal.StructureToPtr(alarm, alarmPtr, false);
                _callback(alarmPtr);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[RtpAlarmListener] callback marshal failed: " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(alarmPtr);
                Marshal.DestroyStructure(thermalPtr, typeof(NetSdkNative.NetAlarmThermalTempInfo));
                Marshal.FreeHGlobal(thermalPtr);
            }
        }

        private static bool TryReadDeviceInfoAt(
            byte[] body,
            int offset,
            int size,
            int remaining,
            out NetSdkNative.NetAlarmDeviceInfo info)
        {
            info = NetSdkNative.NetAlarmDeviceInfo.Create();
            if (body == null || size < DeviceInfoPackedSize || remaining < size || offset < 0 || offset + size > body.Length)
                return false;

            Buffer.BlockCopy(body, offset, info.ip, 0, NetSdkNative.NET_LEN_64);
            info.port_no = ReadU16(body, offset + NetSdkNative.NET_LEN_64);
            info.channel_id = ReadU32(body, offset + size - 4);
            return true;
        }

        private static bool TryReadDeviceInfo(
            byte[] body,
            int offset,
            int remaining,
            out NetSdkNative.NetAlarmDeviceInfo info,
            out int consumed)
        {
            info = NetSdkNative.NetAlarmDeviceInfo.Create();
            consumed = 0;
            if (remaining < DeviceInfoPackedSize)
                return false;

            Buffer.BlockCopy(body, offset, info.ip, 0, NetSdkNative.NET_LEN_64);
            info.port_no = ReadU16(body, offset + NetSdkNative.NET_LEN_64);

            int packedChannel = offset + DeviceInfoPackedSize - 4;
            int alignedChannel = offset + DeviceInfoAlignedSize - 4;
            bool packedTdat = HasTdatMagic(body, offset + DeviceInfoPackedSize, remaining - DeviceInfoPackedSize);
            bool alignedTdat = remaining >= DeviceInfoAlignedSize
                && HasTdatMagic(body, offset + DeviceInfoAlignedSize, remaining - DeviceInfoAlignedSize);

            if (alignedTdat && !packedTdat)
            {
                info.channel_id = ReadU32(body, alignedChannel);
                consumed = DeviceInfoAlignedSize;
            }
            else
            {
                info.channel_id = ReadU32(body, packedChannel);
                consumed = DeviceInfoPackedSize;
            }
            return true;
        }

        private static bool TryReadMarkerAlarms(
            byte[] body,
            int offset,
            int remaining,
            NetSdkNative.NetMarkerAlarmInfo[] dest,
            out ushort count,
            out int consumed)
        {
            count = 0;
            consumed = 0;
            if (remaining < 2)
                return false;

            ushort n = ReadU16(body, offset);
            if (n > NetSdkNative.NET_MAX_MARKERS)
                return false;

            int needed = 2 + n * MarkerAlarmEntrySize;
            if (remaining < needed)
                return false;

            int entry = offset + 2;
            for (int i = 0; i < n; i++)
            {
                var item = new NetSdkNative.NetMarkerAlarmInfo();
                item.marker_name = new byte[NetSdkNative.NET_LEN_64];
                Buffer.BlockCopy(body, entry, item.marker_name, 0, NetSdkNative.NET_LEN_64);
                item.alarm_level = (NetSdkNative.NetThermalTempAlarmLevel)ReadU32(body, entry + NetSdkNative.NET_LEN_64);
                dest[i] = item;
                entry += MarkerAlarmEntrySize;
            }

            count = n;
            consumed = needed;
            return true;
        }

        private static bool HasTdatMagic(byte[] body, int offset, int remaining)
        {
            if (remaining < 4 || offset < 0 || offset + 4 > body.Length)
                return false;
            return ReadU32(body, offset) == ThermalDataParser.TdatMagic;
        }

        private static NetSdkNative.NetErrorCode Handshake(
            NetworkStream stream,
            string host,
            ushort port,
            HlApiSession session)
        {
            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(session.Username + ":" + session.Password));
            var req = new StringBuilder();
            req.Append("GET ").Append(HlApiPaths.Session.Alarm).Append(" HTTP/1.1\r\n");
            req.Append("Host: ").Append(host).Append(':').Append(port).Append("\r\n");
            req.Append("Authorization: Basic ").Append(token).Append("\r\n");
            req.Append("Connection: keep-alive\r\n");
            req.Append("\r\n");
            byte[] bytes = Encoding.ASCII.GetBytes(req.ToString());
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();

            string headerText;
            if (!ReadHttpHeaders(stream, out headerText))
                return NetSdkNative.NetErrorCode.HttpError;

            int status = ParseHttpStatus(headerText);
            if (status == 401)
                return NetSdkNative.NetErrorCode.AuthFailed;
            if (status < 200 || status >= 300)
                return NetSdkNative.NetErrorCode.HttpError;

            string contentType = ParseHeaderValue(headerText, "Content-Type:");
            if (!string.IsNullOrEmpty(contentType)
                && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0)
                return NetSdkNative.NetErrorCode.HttpError;

            int contentLength = ParseContentLength(headerText);
            bool octetStream = !string.IsNullOrEmpty(contentType)
                && contentType.IndexOf("octet-stream", StringComparison.OrdinalIgnoreCase) >= 0;
            if (contentLength > 0 && !octetStream)
                return NetSdkNative.NetErrorCode.HttpError;

            return NetSdkNative.NetErrorCode.OK;
        }

        private static bool ReadHttpHeaders(NetworkStream stream, out string headerText)
        {
            headerText = null;
            var ms = new MemoryStream();
            var one = new byte[1];
            int matched = 0;
            byte[] end = { 13, 10, 13, 10 };
            while (ms.Length < 8192)
            {
                int n;
                try
                {
                    n = stream.Read(one, 0, 1);
                }
                catch (IOException)
                {
                    return false;
                }
                if (n <= 0)
                    return false;
                ms.WriteByte(one[0]);
                if (one[0] == end[matched])
                {
                    matched++;
                    if (matched == 4)
                    {
                        headerText = Encoding.ASCII.GetString(ms.ToArray());
                        return true;
                    }
                }
                else
                {
                    matched = one[0] == end[0] ? 1 : 0;
                }
            }
            return false;
        }

        private static int ParseHttpStatus(string headers)
        {
            if (string.IsNullOrEmpty(headers))
                return 0;
            int sp1 = headers.IndexOf(' ');
            if (sp1 < 0)
                return 0;
            int sp2 = headers.IndexOf(' ', sp1 + 1);
            string code = sp2 > sp1
                ? headers.Substring(sp1 + 1, sp2 - sp1 - 1)
                : headers.Substring(sp1 + 1);
            int status;
            return int.TryParse(code.Trim(), out status) ? status : 0;
        }

        private static int ParseContentLength(string headers)
        {
            int len;
            return int.TryParse(ParseHeaderValue(headers, "Content-Length:"), out len) ? len : 0;
        }

        private static string ParseHeaderValue(string headers, string key)
        {
            if (string.IsNullOrEmpty(headers) || string.IsNullOrEmpty(key))
                return string.Empty;
            int idx = headers.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                return string.Empty;
            int start = idx + key.Length;
            int end = headers.IndexOf("\r\n", start, StringComparison.Ordinal);
            if (end < 0)
                end = headers.Length;
            return headers.Substring(start, end - start).Trim();
        }

        private static bool Connect(TcpClient client, string host, ushort port, uint timeoutMs)
        {
            IAsyncResult ar = client.BeginConnect(host, port, null, null);
            int wait = timeoutMs > 0 ? (int)timeoutMs : 5000;
            if (!ar.AsyncWaitHandle.WaitOne(wait, false))
            {
                try { client.Close(); } catch { }
                return false;
            }
            try
            {
                client.EndConnect(ar);
                return client.Connected;
            }
            catch
            {
                return false;
            }
        }

        private static bool ReadExactStatic(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            int got = 0;
            while (got < count)
            {
                int n;
                try
                {
                    n = stream.Read(buffer, offset + got, count - got);
                }
                catch (IOException)
                {
                    return false;
                }
                if (n <= 0)
                    return false;
                got += n;
            }
            return got == count;
        }

        private void CloseSocket()
        {
            try { if (_stream != null) _stream.Close(); } catch { }
            try { if (_client != null) _client.Close(); } catch { }
            _stream = null;
            _client = null;
        }

        private void ClearPins()
        {
            if (_pixelPin != null) { _pixelPin.Dispose(); _pixelPin = null; }
            if (_lutTempPin != null) { _lutTempPin.Dispose(); _lutTempPin = null; }
            if (_lutAdPin != null) { _lutAdPin.Dispose(); _lutAdPin = null; }
            if (_messagePin != null) { _messagePin.Dispose(); _messagePin = null; }
            _pixelBytes = null;
            _lutTempBytes = null;
            _lutAdBytes = null;
            _messageBytes = null;
        }

        private static ushort ReadU16(byte[] b, int o)
        {
            return (ushort)(b[o] | (b[o + 1] << 8));
        }

        private static uint ReadU32(byte[] b, int o)
        {
            return (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24));
        }

        private static ulong ReadU64(byte[] b, int o)
        {
            return ((ulong)ReadU32(b, o)) | (((ulong)ReadU32(b, o + 4)) << 32);
        }
    }
}
