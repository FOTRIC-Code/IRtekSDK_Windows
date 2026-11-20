using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace IRtekHLAPI
{
    public static partial class NetSdkNative
    {
        private static int _initialized;
        private static int _nextSessionId = 1;
        private static int _nextAlarmListenerId = 1000;
        private static readonly ConcurrentDictionary<int, HlApiSession> Sessions = new ConcurrentDictionary<int, HlApiSession>();
        private static readonly ConcurrentDictionary<int, RtpAlarmListener> AlarmListeners = new ConcurrentDictionary<int, RtpAlarmListener>();

        internal static bool TryGetSession(IntPtr handle, out HlApiSession session)
        {
            session = null;
            if (handle == IntPtr.Zero)
                return false;
            return Sessions.TryGetValue(handle.ToInt32(), out session) && session != null;
        }

        private static int RequireSession(IntPtr handle, out HlApiSession session)
        {
            session = null;
            if (_initialized == 0)
                return (int)NetErrorCode.NotInitialized;
            if (!TryGetSession(handle, out session))
                return (int)NetErrorCode.InvalidParam;
            return (int)NetErrorCode.OK;
        }

        private static int OkOr(NetErrorCode code)
        {
            return (int)code;
        }

        #region SDK Lifecycle

        public static int net_sdk_init()
        {
            Interlocked.Exchange(ref _initialized, 1);
            return (int)NetErrorCode.OK;
        }

        public static int net_sdk_cleanup()
        {
            StopAllAlarmListeners();
            foreach (var kv in Sessions)
            {
                HlApiSession s;
                if (Sessions.TryRemove(kv.Key, out s) && s != null)
                    s.Dispose();
            }
            Interlocked.Exchange(ref _initialized, 0);
            return (int)NetErrorCode.OK;
        }

        public static int net_login(ref NetLoginInfo info, out IntPtr handle)
        {
            handle = IntPtr.Zero;
            if (_initialized == 0)
                return (int)NetErrorCode.NotInitialized;

            string ip = BytesToString(info.ip);
            if (string.IsNullOrWhiteSpace(ip))
                return (int)NetErrorCode.InvalidParam;

            HlApiSession session;
            try
            {
                session = HlApiSession.Create(
                    ip,
                    info.port_no,
                    BytesToString(info.user_name),
                    BytesToString(info.password),
                    info.timeout_ms);
            }
            catch
            {
                return (int)NetErrorCode.InvalidParam;
            }

            object data;
            var ec = session.GetJson(HlApiPaths.System.DeviceInfo, out data);
            if (ec != NetErrorCode.OK)
            {
                session.Dispose();
                return (int)ec;
            }

            int id = Interlocked.Increment(ref _nextSessionId);
            Sessions[id] = session;
            handle = new IntPtr(id);
            return (int)NetErrorCode.OK;
        }

        public static int net_logout(ref IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return (int)NetErrorCode.InvalidParam;

            int id = handle.ToInt32();
            StopAlarmListenersForSession(id);
            HlApiSession session;
            if (!Sessions.TryRemove(id, out session) || session == null)
                return (int)NetErrorCode.InvalidParam;

            session.Dispose();
            handle = IntPtr.Zero;
            return (int)NetErrorCode.OK;
        }

        public static int net_start_alarm_listener(IntPtr handle, OnNetAlarmCallback cb)
        {
            if (cb == null)
                return (int)NetErrorCode.InvalidParam;

            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;

            var portInfo = new NetPortInfo();
            rc = net_get_port(handle, ref portInfo);
            if (rc != (int)NetErrorCode.OK) return rc;

            string host = ExtractHostFromBaseUrl(session.BaseUrl);
            if (string.IsNullOrEmpty(host))
                return (int)NetErrorCode.InvalidParam;

            ushort loginPort = ExtractPortFromBaseUrl(session.BaseUrl);
            var candidates = new System.Collections.Generic.List<ushort>();
            AddUniquePort(candidates, portInfo.rtp_port_no);
            AddUniquePort(candidates, 8001);
            AddUniquePort(candidates, loginPort);
            if (candidates.Count == 0)
                return (int)NetErrorCode.InvalidParam;

            RtpAlarmListener listener = null;
            NetSdkNative.NetErrorCode startEc = NetSdkNative.NetErrorCode.InvalidParam;
            for (int i = 0; i < candidates.Count; i++)
            {
                if (listener != null)
                    listener.Dispose();
                listener = new RtpAlarmListener(handle.ToInt32(), session, host, candidates[i], cb);
                startEc = listener.Start();
                if (startEc == NetErrorCode.OK)
                    break;
            }
            if (startEc != NetErrorCode.OK)
            {
                if (listener != null)
                    listener.Dispose();
                return (int)startEc;
            }

            int id = Interlocked.Increment(ref _nextAlarmListenerId);
            AlarmListeners[id] = listener;
            return id;
        }

        public static int net_stop_alarm_listener(IntPtr alarm_listener_handle)
        {
            if (alarm_listener_handle == IntPtr.Zero)
                return (int)NetErrorCode.InvalidParam;

            int id = alarm_listener_handle.ToInt32();
            RtpAlarmListener listener;
            if (!AlarmListeners.TryRemove(id, out listener) || listener == null)
                return (int)NetErrorCode.InvalidParam;

            listener.Dispose();
            return (int)NetErrorCode.OK;
        }

        private static void StopAlarmListenersForSession(int sessionHandleId)
        {
            foreach (var kv in AlarmListeners)
            {
                if (kv.Value != null && kv.Value.SessionHandleId == sessionHandleId)
                {
                    RtpAlarmListener listener;
                    if (AlarmListeners.TryRemove(kv.Key, out listener) && listener != null)
                        listener.Dispose();
                }
            }
        }

        private static void StopAllAlarmListeners()
        {
            foreach (var kv in AlarmListeners)
            {
                RtpAlarmListener listener;
                if (AlarmListeners.TryRemove(kv.Key, out listener) && listener != null)
                    listener.Dispose();
            }
        }

        public static int net_get_sdk_version(ref NetSdkVersion version)
        {
            version.major = 1;
            version.minor = 0;
            version.patch = 0;
            version.build = 1;
            return (int)NetErrorCode.OK;
        }

        public static int net_set_alarm_trigger(IntPtr handle, ref NetAlarmType type, ref NetAlarmTrigger trigger)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            string path = HlApiPaths.WithType(HlApiPaths.Event.Trigger, (int)type);
            return OkOr(session.PutJson(path, HlApiMapper.ToAlarmTriggerBody(ref trigger)));
        }

        public static int net_get_alarm_trigger(IntPtr handle, ref NetAlarmType type, ref NetAlarmTrigger trigger)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            string path = HlApiPaths.WithType(HlApiPaths.Event.Trigger, (int)type);
            var ec = session.GetJson(path, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapAlarmTrigger(data, ref trigger);
            return (int)NetErrorCode.OK;
        }

        public static int net_get_alarm_trigger_ptr(IntPtr handle, NetAlarmType type, IntPtr trigger)
        {
            if (trigger == IntPtr.Zero)
                return (int)NetErrorCode.NullPointer;
            var t = type;
            var info = default(NetAlarmTrigger);
            int rc = net_get_alarm_trigger(handle, ref t, ref info);
            if (rc != (int)NetErrorCode.OK) return rc;
            System.Runtime.InteropServices.Marshal.StructureToPtr(info, trigger, false);
            return (int)NetErrorCode.OK;
        }

        #endregion

        #region Device Management

        public static int net_get_device_info(IntPtr handle, ref NetDeviceInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.System.DeviceInfo, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapDeviceInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_device_info(IntPtr handle, ref NetDeviceInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.DeviceInfo, HlApiMapper.ToDeviceInfoBody(ref info)));
        }

        public static int net_device_restart(IntPtr handle)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.DeviceRestart, HlApiJson.NewObject()));
        }

        public static int net_device_reset(IntPtr handle)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.DeviceReset, HlApiJson.NewObject()));
        }

        public static int net_device_upgrade(IntPtr handle, ref NetUpgradeInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            string path = BytesToString(info.path);
            return OkOr(session.UploadFile(HlApiPaths.System.DeviceUpgrade, path));
        }

        public static int net_get_time(IntPtr handle, ref NetTimeInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.System.Time, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapTimeInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_time(IntPtr handle, ref NetTimeInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.Time, HlApiMapper.ToTimeInfoBody(ref info)));
        }

        public static int net_get_ntp_server(IntPtr handle, ref NetNtpInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.System.NtpServer, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapNtpInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_ntp_server(IntPtr handle, ref NetNtpInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.NtpServer, HlApiMapper.ToNtpInfoBody(ref info)));
        }

        public static int net_get_networks(
            IntPtr handle,
            NetNetworkInfo[] networks,
            ushort length,
            out ushort out_length)
        {
            out_length = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.System.Networks, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var arr = HlApiJson.AsArray(data);
            if (arr == null) return (int)NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(arr.Count, (int)length), networks != null ? networks.Length : 0);
            for (int i = 0; i < n; i++)
            {
                if (networks[i].name == null) networks[i] = NetNetworkInfo.Create();
                HlApiMapper.MapNetworkInfo(arr[i], ref networks[i]);
            }
            out_length = n;
            return (int)NetErrorCode.OK;
        }

        public static int net_get_network(IntPtr handle, uint id, ref NetNetworkInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.System.Network, id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapNetworkInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_network(IntPtr handle, uint id, ref NetNetworkInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.WithId(HlApiPaths.System.Network, id), HlApiMapper.ToNetworkInfoBody(ref info)));
        }

        public static int net_get_ftp(IntPtr handle, ref NetFtpInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.System.Ftp, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapFtpInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_ftp(IntPtr handle, ref NetFtpInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.Ftp, HlApiMapper.ToFtpInfoBody(ref info)));
        }

        public static int net_test_ftp(IntPtr handle, ref NetFtpInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PostJson(HlApiPaths.System.FtpTest, HlApiMapper.ToFtpInfoBody(ref info)));
        }

        public static int net_get_webhook(IntPtr handle, ref NetWebhookInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.System.Webhook, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapWebhookInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_webhook(IntPtr handle, ref NetWebhookInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.Webhook, HlApiMapper.ToWebhookInfoBody(ref info)));
        }

        public static int net_get_port(IntPtr handle, ref NetPortInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.System.Port, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapPortInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_port(IntPtr handle, ref NetPortInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.System.Port, HlApiMapper.ToPortInfoBody(ref info)));
        }

        /// <summary>
        /// Build: rtsp://host:rtspPort/video/streamId
        /// Host is taken from the session BaseUrl; RTSP port prefers net_get_port.rtsp_port_no.
        /// </summary>
        public static string BuildRtspUrl(IntPtr handle, uint streamId, ushort rtspPortFallback = 554)
        {
            HlApiSession session;
            if (!TryGetSession(handle, out session) || session == null || string.IsNullOrEmpty(session.BaseUrl))
                return null;

            string host = ExtractHostFromBaseUrl(session.BaseUrl);
            if (string.IsNullOrEmpty(host))
                return null;

            ushort port = rtspPortFallback;
            var portInfo = new NetPortInfo();
            if (net_get_port(handle, ref portInfo) == (int)NetErrorCode.OK && portInfo.rtsp_port_no > 0)
                port = portInfo.rtsp_port_no;

            return "rtsp://" + host + ":" + port.ToString() + "/video/" + streamId.ToString();
        }

        private static string ExtractHostFromBaseUrl(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return string.Empty;

            string url = baseUrl.Trim();
            int scheme = url.IndexOf("://", StringComparison.Ordinal);
            if (scheme >= 0)
                url = url.Substring(scheme + 3);

            int colon = url.IndexOf(':');
            int slash = url.IndexOf('/');
            if (colon >= 0 && (slash < 0 || colon < slash))
                return url.Substring(0, colon);
            if (slash >= 0)
                return url.Substring(0, slash);
            return url;
        }

        private static ushort ExtractPortFromBaseUrl(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return 0;

            string url = baseUrl.Trim();
            int scheme = url.IndexOf("://", StringComparison.Ordinal);
            if (scheme >= 0)
                url = url.Substring(scheme + 3);

            int slash = url.IndexOf('/');
            if (slash >= 0)
                url = url.Substring(0, slash);

            int colon = url.LastIndexOf(':');
            if (colon < 0 || colon >= url.Length - 1)
                return 0;

            ushort port;
            return ushort.TryParse(url.Substring(colon + 1), out port) ? port : (ushort)0;
        }

        private static void AddUniquePort(System.Collections.Generic.IList<ushort> ports, ushort port)
        {
            if (port == 0 || ports == null)
                return;
            for (int i = 0; i < ports.Count; i++)
            {
                if (ports[i] == port)
                    return;
            }
            ports.Add(port);
        }

        #endregion

        #region User Management

        public static int net_get_users(
            IntPtr handle,
            NetUserInfo[] users,
            ushort length,
            out ushort out_length)
        {
            out_length = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Security.Users, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var arr = HlApiJson.AsArray(data);
            if (arr == null) return (int)NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(arr.Count, (int)length), users != null ? users.Length : 0);
            for (int i = 0; i < n; i++)
            {
                if (users[i].user_name == null) users[i] = NetUserInfo.Create();
                HlApiMapper.MapUserInfo(arr[i], ref users[i]);
            }
            out_length = n;
            return (int)NetErrorCode.OK;
        }

        public static int net_get_user(IntPtr handle, uint id, ref NetUserInfo user)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.Security.User, id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapUserInfo(data, ref user);
            return (int)NetErrorCode.OK;
        }

        public static int net_update_user(IntPtr handle, ref NetUserPwdInfo user)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.Security.Users, HlApiMapper.ToUserPwdBody(ref user)));
        }

        public static int net_delete_user(IntPtr handle, uint id)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.Delete(HlApiPaths.WithId(HlApiPaths.Security.User, id)));
        }

        #endregion

        #region Streaming metadata

        public static int net_get_streams(
            IntPtr handle,
            NetStreamInfo[] streams,
            ushort length,
            out ushort out_length)
        {
            out_length = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Streaming.Channels, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var arr = HlApiJson.AsArray(data);
            if (arr == null) return (int)NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(arr.Count, (int)length), streams != null ? streams.Length : 0);
            for (int i = 0; i < n; i++)
            {
                if (streams[i].codec_type == null) streams[i] = NetStreamInfo.Create();
                HlApiMapper.MapStreamInfo(arr[i], ref streams[i]);
            }
            out_length = n;
            return (int)NetErrorCode.OK;
        }

        public static int net_get_stream(IntPtr handle, uint stream_id, ref NetStreamInfo stream)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.Streaming.Channel, stream_id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapStreamInfo(data, ref stream);
            return (int)NetErrorCode.OK;
        }

        public static int net_get_stream_status(IntPtr handle, uint stream_id, ref NetStreamStatus status)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.Streaming.ChannelStatus, stream_id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapStreamStatus(data, ref status);
            return (int)NetErrorCode.OK;
        }

        #endregion

        #region Display

        public static int net_get_channels(
            IntPtr handle,
            NetChannelInfo[] channel,
            ushort length,
            out ushort out_length)
        {
            out_length = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Video.Channels, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var arr = HlApiJson.AsArray(data);
            if (arr == null) return (int)NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(arr.Count, (int)length), channel != null ? channel.Length : 0);
            for (int i = 0; i < n; i++)
            {
                if (channel[i].name == null) channel[i] = NetChannelInfo.Create();
                HlApiMapper.MapChannelInfo(arr[i], ref channel[i]);
            }
            out_length = n;
            return (int)NetErrorCode.OK;
        }

        public static int net_get_image_params(IntPtr handle, uint channel_id, ref NetImageParams param)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.Image.ImageParams, channel_id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapImageParams(data, ref param);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_image_params(IntPtr handle, uint channel_id, ref NetImageParams param)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.WithId(HlApiPaths.Image.ImageParams, channel_id), HlApiMapper.ToImageParamsBody(ref param)));
        }

        public static int net_get_image_flip(IntPtr handle, uint channel_id, out NetImageFlipMode mode)
        {
            mode = NetImageFlipMode.None;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.Image.ImageFlip, channel_id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var d = HlApiJson.AsDict(data);
            if (d != null)
                mode = (NetImageFlipMode)HlApiJson.ToInt(HlApiJson.Get(d, "mode"));
            return (int)NetErrorCode.OK;
        }

        public static int net_set_image_flip(IntPtr handle, uint channel_id, ref NetImageFlipMode mode)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var body = HlApiJson.NewObject();
            body["mode"] = (int)mode;
            return OkOr(session.PutJson(HlApiPaths.WithId(HlApiPaths.Image.ImageFlip, channel_id), body));
        }

        public static int net_get_channel(IntPtr handle, uint channel_id, ref NetChannelInfo channel)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.Video.Channel, channel_id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapChannelInfo(data, ref channel);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_channel(IntPtr handle, uint channel_id, ref NetChannelInfo channel)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.WithId(HlApiPaths.Video.Channel, channel_id), HlApiMapper.ToChannelInfoBody(ref channel)));
        }

        public static int net_get_overlay(IntPtr handle, uint channel_id, ref NetOverlayInfo overlay)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.WithId(HlApiPaths.Video.ChannelOverlay, channel_id), out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapOverlayInfo(data, ref overlay);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_overlay(IntPtr handle, uint channel_id, ref NetOverlayInfo overlay)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.WithId(HlApiPaths.Video.ChannelOverlay, channel_id), HlApiMapper.ToOverlayInfoBody(ref overlay)));
        }

        #endregion
    }
}
