using System;
using System.Collections.Generic;
using System.IO;

namespace IRtekHLAPI
{
    public static partial class NetSdkNative
    {
        #region Thermal

        public static int net_get_thermal_lens_info(IntPtr handle, ref NetLensInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.LensInfo, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapLensInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_thermal_calibrate(IntPtr handle)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.Thermal.Calibrate, HlApiJson.NewObject()));
        }

        public static int net_get_thermal_shutter_mode(IntPtr handle, out NetShutterMode mode)
        {
            mode = NetShutterMode.Automatic;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.ShutterMode, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var d = HlApiJson.AsDict(data);
            if (d != null)
                mode = (NetShutterMode)HlApiJson.ToInt(HlApiJson.Get(d, "mode"));
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_shutter_mode(IntPtr handle, ref NetShutterMode mode)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var body = HlApiJson.NewObject();
            body["mode"] = (int)mode;
            return OkOr(session.PutJson(HlApiPaths.Thermal.ShutterMode, body));
        }

        public static int net_get_thermal_shutter_time(IntPtr handle, out ushort minutes)
        {
            minutes = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.ShutterTime, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var d = HlApiJson.AsDict(data);
            if (d != null)
                minutes = HlApiJson.ToUShort(HlApiJson.Get(d, "interval"));
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_shutter_time(IntPtr handle, ref ushort minutes)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var body = HlApiJson.NewObject();
            body["interval"] = minutes;
            return OkOr(session.PutJson(HlApiPaths.Thermal.ShutterTime, body));
        }

        public static int net_get_thermal_resolution(IntPtr handle, ref NetSize size)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Resolution, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var d = HlApiJson.AsDict(data);
            if (d != null)
            {
                size.width = HlApiJson.ToUShort(HlApiJson.Get(d, "width"));
                size.height = HlApiJson.ToUShort(HlApiJson.Get(d, "height"));
            }
            return (int)NetErrorCode.OK;
        }

        public static int net_get_thermal_range(IntPtr handle, out ushort range_id)
        {
            range_id = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Range, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var d = HlApiJson.AsDict(data);
            if (d != null)
                range_id = HlApiJson.ToUShort(HlApiJson.Get(d, "range_id"));
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_range(IntPtr handle, ref ushort range_id)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var body = HlApiJson.NewObject();
            body["range_id"] = range_id;
            return OkOr(session.PutJson(HlApiPaths.Thermal.Range, body));
        }

        public static int net_get_thermal_lut(IntPtr handle, ref NetLutTable lut)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Lut, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapLutTable(data, ref lut);
            return (int)NetErrorCode.OK;
        }

        public static int net_get_thermal_params(IntPtr handle, ref NetThermalParams param)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Params, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapThermalParams(data, ref param);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_params(IntPtr handle, ref NetThermalParams param)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.Thermal.Params, HlApiMapper.ToThermalParamsBody(ref param)));
        }

        public static int net_get_thermal_preset_palettes(
            IntPtr handle,
            NetPaletteName[] palettes,
            ushort length,
            out ushort out_length)
        {
            out_length = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Palettes, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var arr = HlApiJson.AsArray(data);
            if (arr == null) return (int)NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(arr.Count, (int)length), palettes != null ? palettes.Length : 0);
            for (int i = 0; i < n; i++)
            {
                if (palettes[i].name == null) palettes[i] = NetPaletteName.Create();
                HlApiMapper.MapPaletteName(arr[i], ref palettes[i]);
            }
            out_length = n;
            return (int)NetErrorCode.OK;
        }

        public static int net_get_thermal_palettes(
            IntPtr handle,
            NetPaletteName[] palettes,
            ushort length,
            out ushort out_length)
        {
            return net_get_thermal_preset_palettes(handle, palettes, length, out out_length);
        }

        public static int net_get_thermal_palette(IntPtr handle, ref NetPalette palette)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Palette, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapPalette(data, ref palette);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_palette(IntPtr handle, ref NetPalette palette)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.Thermal.Palette, HlApiMapper.ToPaletteBody(ref palette)));
        }

        public static int net_get_thermal_overlay(IntPtr handle, ref NetThermalOverlayInfo overlay)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Overlay, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapThermalOverlay(data, ref overlay);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_overlay(IntPtr handle, ref NetThermalOverlayInfo overlay)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.Thermal.Overlay, HlApiMapper.ToThermalOverlayBody(ref overlay)));
        }

        public static int net_get_thermal_unit(IntPtr handle, ref NetUnitInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Unit, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapUnitInfo(data, ref info);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_unit(IntPtr handle, ref NetUnitInfo info)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.Thermal.Unit, HlApiMapper.ToUnitInfoBody(ref info)));
        }

        public static int net_get_thermal_capture(IntPtr handle, NetThermalCaptureMode mode, ref NetCaptureFile capture_file)
        {
            string localPath = BytesToString(capture_file.local_file);
            if (string.IsNullOrWhiteSpace(localPath))
                return (int)NetErrorCode.InvalidParam;

            byte[] bytes;
            int rc = net_get_thermal_capture_bytes(handle, mode, out bytes);
            if (rc != (int)NetErrorCode.OK)
                return rc;

            try
            {
                string dir = Path.GetDirectoryName(localPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllBytes(localPath, bytes);
                return (int)NetErrorCode.OK;
            }
            catch
            {
                return (int)NetErrorCode.Fail;
            }
        }

        /// <summary>
        /// Download thermal capture into memory. PureImage (0) returns raw JPEG bytes.
        /// </summary>
        public static int net_get_thermal_capture_bytes(IntPtr handle, NetThermalCaptureMode mode, out byte[] data)
        {
            data = null;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;

            // Pure image: /thermal/capture/0 (same as NetSDK capture_pure_image).
            // Mode 1 would be TCAP; HLAPI demo only uses PureImage.
            string path = mode == NetThermalCaptureMode.PureImage
                ? HlApiPaths.Thermal.Capture + "/0"
                : HlApiPaths.Thermal.Capture + "/" + ((int)mode).ToString();

            var ec = session.GetBinary(path, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            if (data == null || data.Length == 0)
                return (int)NetErrorCode.HttpError;

            // Pure JPEG must start with SOI marker; reject JSON/HTML error bodies saved as .jpg.
            if (mode == NetThermalCaptureMode.PureImage)
            {
                if (data.Length < 2 || data[0] != 0xFF || data[1] != 0xD8)
                    return (int)NetErrorCode.HttpError;
            }

            return (int)NetErrorCode.OK;
        }

        public static int net_get_thermal_markers(
            IntPtr handle,
            NetMarkerInfo[] markers,
            ushort length,
            out ushort out_length)
        {
            out_length = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Marker, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var arr = HlApiJson.AsArray(data);
            if (arr == null) return (int)NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(arr.Count, (int)length), markers != null ? markers.Length : 0);
            for (int i = 0; i < n; i++)
            {
                if (markers[i].name == null) markers[i] = NetMarkerInfo.Create();
                HlApiMapper.MapMarkerInfo(arr[i], ref markers[i]);
            }
            out_length = n;
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_markers(
            IntPtr handle,
            NetMarkerInfo[] markers,
            ushort length)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var arr = HlApiJson.NewArray();
            int n = Math.Min((int)length, markers != null ? markers.Length : 0);
            for (int i = 0; i < n; i++)
                arr.Add(HlApiMapper.ToMarkerInfoBody(ref markers[i]));
            return OkOr(session.PutJson(HlApiPaths.Thermal.Marker, arr));
        }

        public static int net_get_thermal_temp_alarms(
            IntPtr handle,
            NetThermalTempAlarm[] alarms,
            ushort length,
            out ushort out_length)
        {
            out_length = 0;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Alarm, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var arr = HlApiJson.AsArray(data);
            if (arr == null) return (int)NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(arr.Count, (int)length), alarms != null ? alarms.Length : 0);
            for (int i = 0; i < n; i++)
            {
                if (alarms[i].marker_name == null) alarms[i] = NetThermalTempAlarm.Create();
                HlApiMapper.MapTempAlarm(arr[i], ref alarms[i]);
            }
            out_length = n;
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_temp_alarms(
            IntPtr handle,
            NetThermalTempAlarm[] alarms,
            ushort length)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var arr = HlApiJson.NewArray();
            int n = Math.Min((int)length, alarms != null ? alarms.Length : 0);
            for (int i = 0; i < n; i++)
                arr.Add(HlApiMapper.ToTempAlarmBody(ref alarms[i]));
            return OkOr(session.PutJson(HlApiPaths.Thermal.Alarm, arr));
        }

        public static int net_get_thermal_temp_alarm_interval(IntPtr handle, ref NetThermalTempAlarmInterval interval)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.AlarmInterval, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var d = HlApiJson.AsDict(data);
            if (d != null)
                interval.interval = HlApiJson.ToUShort(HlApiJson.Get(d, "interval"));
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_temp_alarm_interval(IntPtr handle, ref NetThermalTempAlarmInterval interval)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var body = HlApiJson.NewObject();
            body["interval"] = interval.interval;
            return OkOr(session.PutJson(HlApiPaths.Thermal.AlarmInterval, body));
        }

        public static int net_set_thermal_dpc(IntPtr handle, ref NetDpcParams param)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            Dictionary<string, object> body;
            HlApiMapper.MapDpcParams(ref param, out body);
            return OkOr(session.PutJson(HlApiPaths.Thermal.Dpc, body));
        }

        public static int net_get_thermal_color_dist(IntPtr handle, out NetColorDistType type)
        {
            type = NetColorDistType.TWB;
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.ColorDist, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            var d = HlApiJson.AsDict(data);
            if (d != null)
                type = (NetColorDistType)HlApiJson.ToInt(HlApiJson.Get(d, "type"));
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_color_dist(IntPtr handle, ref NetColorDistType type)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            var body = HlApiJson.NewObject();
            body["type"] = (int)type;
            return OkOr(session.PutJson(HlApiPaths.Thermal.ColorDist, body));
        }

        public static int net_get_thermal_gain(IntPtr handle, ref NetGain gain)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            object data;
            var ec = session.GetJson(HlApiPaths.Thermal.Gain, out data);
            if (ec != NetErrorCode.OK) return (int)ec;
            HlApiMapper.MapGain(data, ref gain);
            return (int)NetErrorCode.OK;
        }

        public static int net_set_thermal_gain(IntPtr handle, ref NetGain gain)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;
            return OkOr(session.PutJson(HlApiPaths.Thermal.Gain, HlApiMapper.ToGainBody(ref gain)));
        }

        public static int net_get_thermal_data(IntPtr handle, uint data_types, ref NetThermalData realdata)
        {
            HlApiSession session;
            int rc = RequireSession(handle, out session);
            if (rc != (int)NetErrorCode.OK) return rc;

            data_types &= ThermalDataParser.DataTypeMask;
            if (data_types == 0)
                return (int)NetErrorCode.InvalidParam;

            string path = HlApiPaths.WithDataTypes(HlApiPaths.Thermal.Data, data_types);
            byte[] bytes;
            var ec = session.GetBinary(path, out bytes);
            if (ec != NetErrorCode.OK) return (int)ec;

            ThermalDataParser.ParsedThermalData parsed;
            if (!ThermalDataParser.TryParseTdat(bytes, out parsed))
                return (int)NetErrorCode.HttpError;

            ThermalDataParser.ApplyToPublic(parsed, session, ref realdata);
            return (int)NetErrorCode.OK;
        }

        #endregion
    }
}
