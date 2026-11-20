using System;

namespace IRtekHLAPI
{
    internal static class ThermalDataParser
    {
        public const int RawHdrSize = 48;
        public const int GlobalSectionSize = 20;
        public const int MarkerEntrySize = 92;
        public const int TdatHdrSize = 20;
        public const uint TdatMagic = 0x54414454u; // "TDAT"
        public const uint DataTypeRaw = 0x01;
        public const uint DataTypeGlobal = 0x02;
        public const uint DataTypeMarker = 0x04;
        public const uint DataTypeMask = DataTypeRaw | DataTypeGlobal | DataTypeMarker;
        public const uint MaxLutLen = 64;
        public const ushort MaxMarkers = 16;

        public sealed class ParsedThermalData
        {
            public uint DataTypes;
            public ulong TimestampUs;
            public bool HasRaw;
            public ushort Width;
            public ushort Height;
            public ushort RangeId;
            public uint PointsCount;
            public uint LutLen;
            public uint LutEnvTemp;
            public uint SensorTemp;
            public uint ShutterState;
            public uint FrameNo;
            public ulong Pts;
            public uint RawType;
            public uint ShutterAdv;
            public ushort[] Pixels;
            public float[] LutTemp;
            public ushort[] LutAd;
            public NetSdkNative.NetTempPointInfo Global;
            public NetSdkNative.NetMarkerTempInfo[] Markers;
            public ushort MarkersCount;
        }

        public static bool TryParseTdat(byte[] buf, out ParsedThermalData result)
        {
            int consumed;
            if (!TryParseTdatSection(buf, 0, buf != null ? buf.Length : 0, out result, out consumed))
                return false;
            return consumed == buf.Length;
        }

        public static bool TryParsePackedThermalContext(byte[] buf, int offset, int available, out ParsedThermalData result, out int consumed)
        {
            if (TryParseTdatSection(buf, offset, available, out result, out consumed))
                return true;

            result = null;
            consumed = 0;
            if (buf == null || offset < 0 || available <= 0 || offset + available > buf.Length)
                return false;

            var parsed = new ParsedThermalData
            {
                Markers = new NetSdkNative.NetMarkerTempInfo[NetSdkNative.NET_MAX_MARKERS]
            };
            int cursor = offset;
            int left = available;
            bool any = false;

            int rawUsed;
            if (left >= RawHdrSize && TryParseRaw(buf, cursor, left, parsed, out rawUsed) && rawUsed > 0)
            {
                parsed.DataTypes |= DataTypeRaw;
                parsed.TimestampUs = parsed.Pts;
                cursor += rawUsed;
                left -= rawUsed;
                any = true;
            }

            if (left >= GlobalSectionSize)
            {
                NetSdkNative.NetTempPointInfo global;
                int globalUsed;
                if (TryParseGlobal(buf, cursor, left, out global, out globalUsed))
                {
                    parsed.DataTypes |= DataTypeGlobal;
                    parsed.Global = global;
                    cursor += globalUsed;
                    left -= globalUsed;
                    any = true;
                }
            }

            if (left >= 2)
            {
                ushort count;
                int markerUsed;
                if (TryParseMarkers(buf, cursor, left, parsed.Markers, out count, out markerUsed)
                    && (count == 0 || LooksLikePackedName(buf, cursor + 2 + 8)))
                {
                    parsed.DataTypes |= DataTypeMarker;
                    parsed.MarkersCount = count;
                    cursor += markerUsed;
                    any = true;
                }
            }

            if (!any)
                return false;

            result = parsed;
            consumed = cursor - offset;
            return true;
        }

        public static bool TryParseTdatSection(byte[] buf, int offset, int available, out ParsedThermalData result, out int consumed)
        {
            result = null;
            consumed = 0;
            if (buf == null || offset < 0 || available < TdatHdrSize || offset + available > buf.Length)
                return false;

            uint magic = ReadU32(buf, offset);
            if (magic != TdatMagic)
                return false;

            uint dataTypes = ReadU32(buf, offset + 4) & DataTypeMask;
            uint rawLen = ReadU32(buf, offset + 8);
            uint globalLen = ReadU32(buf, offset + 12);
            uint markerLen = ReadU32(buf, offset + 16);

            long total = (long)TdatHdrSize + rawLen + globalLen + markerLen;
            if (total > available)
                return false;

            if ((rawLen > 0) != ((dataTypes & DataTypeRaw) != 0))
                dataTypes = rawLen > 0 ? (dataTypes | DataTypeRaw) : (dataTypes & ~DataTypeRaw);
            if ((globalLen > 0) != ((dataTypes & DataTypeGlobal) != 0))
                dataTypes = globalLen > 0 ? (dataTypes | DataTypeGlobal) : (dataTypes & ~DataTypeGlobal);
            if ((markerLen > 0) != ((dataTypes & DataTypeMarker) != 0))
                dataTypes = markerLen > 0 ? (dataTypes | DataTypeMarker) : (dataTypes & ~DataTypeMarker);

            var parsed = new ParsedThermalData
            {
                DataTypes = dataTypes,
                Markers = new NetSdkNative.NetMarkerTempInfo[NetSdkNative.NET_MAX_MARKERS]
            };

            int cursor = offset + TdatHdrSize;
            if (rawLen > 0)
            {
                int used;
                if (!TryParseRaw(buf, cursor, (int)rawLen, parsed, out used) || used != rawLen)
                    return false;
                parsed.TimestampUs = parsed.Pts;
                cursor += (int)rawLen;
            }

            if (globalLen > 0)
            {
                if (globalLen != GlobalSectionSize)
                    return false;
                int used;
                NetSdkNative.NetTempPointInfo global;
                if (!TryParseGlobal(buf, cursor, (int)globalLen, out global, out used) || used != globalLen)
                    return false;
                parsed.Global = global;
                cursor += (int)globalLen;
            }

            if (markerLen > 0)
            {
                int used;
                ushort count;
                if (!TryParseMarkers(buf, cursor, (int)markerLen, parsed.Markers, out count, out used) || used != markerLen)
                    return false;
                parsed.MarkersCount = count;
            }

            result = parsed;
            consumed = (int)total;
            return true;
        }

        private static bool TryParseRaw(byte[] data, int offset, int len, ParsedThermalData outData, out int consumed)
        {
            consumed = 0;
            if (len < RawHdrSize)
                return false;

            ushort width = ReadU16(data, offset + 0);
            ushort height = ReadU16(data, offset + 2);
            ushort rangeId = ReadU16(data, offset + 4);
            uint pointsCount = ReadU32(data, offset + 8);
            uint lutLen = ReadU32(data, offset + 12);
            if (width < 16 || height < 16 || width > 2048 || height > 2048 || lutLen > MaxLutLen)
                return false;

            int pixelCount = width * height;
            int pixelBytes = pixelCount * 2;
            int lutTempBytes = (int)lutLen * 4;
            int lutAdBytes = (int)lutLen * 2;
            int needed = RawHdrSize + pixelBytes + lutTempBytes + lutAdBytes;
            if (len < needed)
                return false;

            outData.HasRaw = true;
            outData.Width = width;
            outData.Height = height;
            outData.RangeId = rangeId;
            outData.PointsCount = pointsCount;
            outData.LutLen = lutLen;
            outData.LutEnvTemp = ReadU32(data, offset + 16);
            outData.SensorTemp = ReadU32(data, offset + 20);
            outData.ShutterState = ReadU32(data, offset + 24);
            outData.FrameNo = ReadU32(data, offset + 28);
            outData.Pts = ReadU64(data, offset + 32);
            outData.RawType = ReadU32(data, offset + 40);
            outData.ShutterAdv = ReadU32(data, offset + 44);

            outData.Pixels = new ushort[pixelCount];
            int pixelPtr = offset + RawHdrSize;
            for (int i = 0; i < pixelCount; i++)
                outData.Pixels[i] = ReadU16(data, pixelPtr + i * 2);

            outData.LutTemp = new float[lutLen];
            int lutTempPtr = pixelPtr + pixelBytes;
            for (int i = 0; i < lutLen; i++)
                outData.LutTemp[i] = ReadF32(data, lutTempPtr + i * 4);

            outData.LutAd = new ushort[lutLen];
            int lutAdPtr = lutTempPtr + lutTempBytes;
            for (int i = 0; i < lutLen; i++)
                outData.LutAd[i] = ReadU16(data, lutAdPtr + i * 2);

            consumed = needed;
            return true;
        }

        private static bool TryParseGlobal(byte[] data, int offset, int len, out NetSdkNative.NetTempPointInfo info, out int consumed)
        {
            info = default(NetSdkNative.NetTempPointInfo);
            consumed = 0;
            if (len < GlobalSectionSize)
                return false;

            info.max.temperature = ReadF32(data, offset + 0);
            info.max.point.x = ReadU16(data, offset + 4);
            info.max.point.y = ReadU16(data, offset + 6);
            info.min.temperature = ReadF32(data, offset + 8);
            info.min.point.x = ReadU16(data, offset + 12);
            info.min.point.y = ReadU16(data, offset + 14);
            info.avg_temp = ReadF32(data, offset + 16);
            consumed = GlobalSectionSize;
            return true;
        }

        private static bool TryParseMarkers(byte[] data, int offset, int len, NetSdkNative.NetMarkerTempInfo[] dest, out ushort count, out int consumed)
        {
            count = 0;
            consumed = 0;
            if (len < 2)
                return false;

            ushort n = ReadU16(data, offset);
            if (n > MaxMarkers)
                return false;

            int needed = 2 + n * MarkerEntrySize;
            if (len < needed)
                return false;

            if (n > 0 && !LooksLikePackedName(data, offset + 2 + 8))
                return false;

            int entry = offset + 2;
            for (int i = 0; i < n; i++)
            {
                var m = new NetSdkNative.NetMarkerTempInfo();
                m.marker_id = ReadU32(data, entry + 0);
                m.type = (NetSdkNative.NetMarkerType)ReadU32(data, entry + 4);
                m.marker_name = new byte[NetSdkNative.NET_LEN_64];
                Buffer.BlockCopy(data, entry + 8, m.marker_name, 0, NetSdkNative.NET_LEN_64);
                m.temp_point_info.max.temperature = ReadF32(data, entry + 72);
                m.temp_point_info.max.point.x = ReadU16(data, entry + 76);
                m.temp_point_info.max.point.y = ReadU16(data, entry + 78);
                m.temp_point_info.min.temperature = ReadF32(data, entry + 80);
                m.temp_point_info.min.point.x = ReadU16(data, entry + 84);
                m.temp_point_info.min.point.y = ReadU16(data, entry + 86);
                m.temp_point_info.avg_temp = ReadF32(data, entry + 88);
                dest[i] = m;
                entry += MarkerEntrySize;
            }

            count = n;
            consumed = needed;
            return true;
        }

        public static bool LooksLikePackedName(byte[] data, int offset)
        {
            if (data == null || offset < 0 || offset + NetSdkNative.NET_LEN_64 > data.Length)
                return false;

            int n = 0;
            for (int i = 0; i < NetSdkNative.NET_LEN_64; i++)
            {
                byte b = data[offset + i];
                if (b == 0)
                {
                    for (int j = i; j < NetSdkNative.NET_LEN_64; j++)
                    {
                        if (data[offset + j] != 0)
                            return false;
                    }
                    return n > 0 && n <= 32;
                }

                bool ok = (b >= (byte)'A' && b <= (byte)'Z')
                    || (b >= (byte)'a' && b <= (byte)'z')
                    || (b >= (byte)'0' && b <= (byte)'9')
                    || b == (byte)'_' || b == (byte)'-' || b == (byte)' ';
                if (!ok)
                    return false;
                n++;
            }
            return n > 0;
        }

        public static void ApplyToPublic(ParsedThermalData parsed, HlApiSession session, ref NetSdkNative.NetThermalData realdata)
        {
            realdata = default(NetSdkNative.NetThermalData);
            realdata.data_types = (NetSdkNative.NetThermalDataType)parsed.DataTypes;
            realdata.timestamp_us = parsed.TimestampUs;
            realdata.global_temp_info = parsed.Global;
            realdata.marker_temps = new NetSdkNative.NetMarkerTempInfo[NetSdkNative.NET_MAX_MARKERS];
            realdata.markers_count = parsed.MarkersCount;
            if (parsed.Markers != null && parsed.MarkersCount > 0)
                Array.Copy(parsed.Markers, realdata.marker_temps, parsed.MarkersCount);

            if (!parsed.HasRaw)
                return;

            session.ClearThermalPins();

            realdata.raw_frame.width = parsed.Width;
            realdata.raw_frame.height = parsed.Height;
            realdata.raw_frame.range_id = parsed.RangeId;
            realdata.raw_frame.points_count = parsed.PointsCount;
            realdata.raw_frame.lut_len = parsed.LutLen;
            realdata.raw_frame.lut_env_temp = parsed.LutEnvTemp;
            realdata.raw_frame.sensor_temp = parsed.SensorTemp;
            realdata.raw_frame.shutter_state = parsed.ShutterState;
            realdata.raw_frame.frame_no = parsed.FrameNo;
            realdata.raw_frame.pts = parsed.Pts;
            realdata.raw_frame.raw_type = parsed.RawType;
            realdata.raw_frame.shutter_adv = parsed.ShutterAdv;

            if (parsed.Pixels != null && parsed.Pixels.Length > 0)
            {
                session.ThermalPixelBytes = new byte[parsed.Pixels.Length * 2];
                Buffer.BlockCopy(parsed.Pixels, 0, session.ThermalPixelBytes, 0, session.ThermalPixelBytes.Length);
                session.ThermalPixelPin = new GCHandleHolder(session.ThermalPixelBytes);
                realdata.raw_frame.data = session.ThermalPixelPin.AddrOfPinnedObject();
            }

            if (parsed.LutTemp != null && parsed.LutTemp.Length > 0)
            {
                session.ThermalLutTempBytes = new byte[parsed.LutTemp.Length * 4];
                Buffer.BlockCopy(parsed.LutTemp, 0, session.ThermalLutTempBytes, 0, session.ThermalLutTempBytes.Length);
                session.ThermalLutTempPin = new GCHandleHolder(session.ThermalLutTempBytes);
                realdata.raw_frame.lut_temp_value = session.ThermalLutTempPin.AddrOfPinnedObject();
            }

            if (parsed.LutAd != null && parsed.LutAd.Length > 0)
            {
                session.ThermalLutAdBytes = new byte[parsed.LutAd.Length * 2];
                Buffer.BlockCopy(parsed.LutAd, 0, session.ThermalLutAdBytes, 0, session.ThermalLutAdBytes.Length);
                session.ThermalLutAdPin = new GCHandleHolder(session.ThermalLutAdBytes);
                realdata.raw_frame.lut_ad_value = session.ThermalLutAdPin.AddrOfPinnedObject();
            }
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

        private static float ReadF32(byte[] b, int o)
        {
            return BitConverter.ToSingle(b, o);
        }
    }
}
