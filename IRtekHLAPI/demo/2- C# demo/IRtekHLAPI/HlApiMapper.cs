using System;
using System.Collections;
using System.Collections.Generic;

namespace IRtekHLAPI
{
    internal static class HlApiMapper
    {
        public static void FillBytes(byte[] dest, string value)
        {
            if (dest == null)
                return;
            Array.Clear(dest, 0, dest.Length);
            if (string.IsNullOrEmpty(value))
                return;
            var src = System.Text.Encoding.UTF8.GetBytes(value);
            Buffer.BlockCopy(src, 0, dest, 0, Math.Min(src.Length, dest.Length - 1));
        }

        public static string FromBytes(byte[] bytes)
        {
            return NetSdkNative.BytesToString(bytes);
        }

        public static void CopyStringArray(byte[] dest, int slotLen, ArrayList values, out ushort count)
        {
            count = 0;
            if (dest == null)
                return;
            Array.Clear(dest, 0, dest.Length);
            if (values == null)
                return;
            int max = dest.Length / slotLen;
            for (int i = 0; i < values.Count && i < max; i++)
            {
                FillBytesSlice(dest, i * slotLen, slotLen, HlApiJson.ToString(values[i]));
                count++;
            }
        }

        public static ArrayList PackStringArray(byte[] src, int slotLen, ushort count)
        {
            var list = HlApiJson.NewArray();
            if (src == null || count == 0)
                return list;
            int max = src.Length / slotLen;
            int n = Math.Min((int)count, max);
            for (int i = 0; i < n; i++)
            {
                var slice = new byte[slotLen];
                Buffer.BlockCopy(src, i * slotLen, slice, 0, slotLen);
                string s = FromBytes(slice);
                if (!string.IsNullOrEmpty(s))
                    list.Add(s);
            }
            return list;
        }

        private static void FillBytesSlice(byte[] dest, int offset, int length, string value)
        {
            if (dest == null || length <= 0)
                return;
            int end = Math.Min(offset + length, dest.Length);
            for (int i = offset; i < end; i++)
                dest[i] = 0;
            if (string.IsNullOrEmpty(value))
                return;
            var src = System.Text.Encoding.UTF8.GetBytes(value);
            Buffer.BlockCopy(src, 0, dest, offset, Math.Min(src.Length, length - 1));
        }

        public static void MapDeviceInfo(object data, ref NetSdkNative.NetDeviceInfo info)
        {
            EnsureDevice(ref info);
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            FillBytes(info.name, HlApiJson.ToString(HlApiJson.Get(d, "name")));
            FillBytes(info.model, HlApiJson.ToString(HlApiJson.Get(d, "model")));
            FillBytes(info.spec, HlApiJson.ToString(HlApiJson.Get(d, "spec")));
            FillBytes(info.serial_no, HlApiJson.ToString(HlApiJson.Get(d, "serial_no")));
            FillBytes(info.id, HlApiJson.ToString(HlApiJson.Get(d, "id")));
            FillBytes(info.brand, HlApiJson.ToString(HlApiJson.Get(d, "brand")));
            FillBytes(info.company, HlApiJson.ToString(HlApiJson.Get(d, "company")));
            FillBytes(info.firmware_name, HlApiJson.ToString(HlApiJson.Get(d, "firmware_name")));
            FillBytes(info.firmware_version, HlApiJson.ToString(HlApiJson.Get(d, "firmware_version")));
        }

        public static Dictionary<string, object> ToDeviceInfoBody(ref NetSdkNative.NetDeviceInfo info)
        {
            var body = HlApiJson.NewObject();
            body["name"] = FromBytes(info.name);
            return body;
        }

        private static void EnsureDevice(ref NetSdkNative.NetDeviceInfo info)
        {
            if (info.name == null) info = NetSdkNative.NetDeviceInfo.Create();
        }

        public static void MapTimeInfo(object data, ref NetSdkNative.NetTimeInfo info)
        {
            if (info.local_time == null) info = NetSdkNative.NetTimeInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.time_mode = (NetSdkNative.NetTimeMode)HlApiJson.ToInt(HlApiJson.Get(d, "time_mode"));
            FillBytes(info.local_time, HlApiJson.ToString(HlApiJson.Get(d, "local_time")));
            FillBytes(info.time_zone, HlApiJson.ToString(HlApiJson.Get(d, "time_zone")));
        }

        public static Dictionary<string, object> ToTimeInfoBody(ref NetSdkNative.NetTimeInfo info)
        {
            var body = HlApiJson.NewObject();
            body["time_mode"] = (int)info.time_mode;
            body["local_time"] = FromBytes(info.local_time);
            body["time_zone"] = FromBytes(info.time_zone);
            return body;
        }

        public static void MapNtpInfo(object data, ref NetSdkNative.NetNtpInfo info)
        {
            if (info.host_name == null) info = NetSdkNative.NetNtpInfo.Create();
            var d = HlApiJson.AsDict(data);
            // Some firmwares return an array; take first element.
            if (d == null)
            {
                var arr = HlApiJson.AsArray(data);
                if (arr != null && arr.Count > 0)
                    d = HlApiJson.AsDict(arr[0]);
            }
            if (d == null) return;
            FillBytes(info.host_name, HlApiJson.ToString(HlApiJson.Get(d, "host_name")));
            info.port_no = HlApiJson.ToUShort(HlApiJson.Get(d, "port_no"));
            info.syn_interval = HlApiJson.ToUInt(HlApiJson.Get(d, "syn_interval"));
        }

        public static Dictionary<string, object> ToNtpInfoBody(ref NetSdkNative.NetNtpInfo info)
        {
            var body = HlApiJson.NewObject();
            body["host_name"] = FromBytes(info.host_name);
            body["port_no"] = info.port_no;
            body["syn_interval"] = info.syn_interval;
            return body;
        }

        public static void MapNetworkInfo(object data, ref NetSdkNative.NetNetworkInfo info)
        {
            if (info.name == null) info = NetSdkNative.NetNetworkInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.id = HlApiJson.ToUInt(HlApiJson.Get(d, "id"));
            FillBytes(info.name, HlApiJson.ToString(HlApiJson.Get(d, "name")));
            info.dhcp = HlApiJson.ToByteFlag(HlApiJson.Get(d, "dhcp"));
            FillBytes(info.ip, HlApiJson.ToString(HlApiJson.Get(d, "ip")));
            FillBytes(info.netmask, HlApiJson.ToString(HlApiJson.Get(d, "netmask")));
            ushort dnsCount;
            CopyStringArray(info.dns, NetSdkNative.NET_LEN_64, HlApiJson.AsArray(HlApiJson.Get(d, "dns")), out dnsCount);
            info.dns_count = dnsCount;
            ushort gwCount;
            CopyStringArray(info.gateway, NetSdkNative.NET_LEN_64, HlApiJson.AsArray(HlApiJson.Get(d, "gateway")), out gwCount);
            info.gateway_count = gwCount;
        }

        public static Dictionary<string, object> ToNetworkInfoBody(ref NetSdkNative.NetNetworkInfo info)
        {
            var body = HlApiJson.NewObject();
            body["id"] = info.id;
            body["name"] = FromBytes(info.name);
            body["dhcp"] = info.dhcp != 0;
            body["ip"] = FromBytes(info.ip);
            body["netmask"] = FromBytes(info.netmask);
            body["dns"] = PackStringArray(info.dns, NetSdkNative.NET_LEN_64, info.dns_count);
            body["gateway"] = PackStringArray(info.gateway, NetSdkNative.NET_LEN_64, info.gateway_count);
            return body;
        }

        public static void MapFtpInfo(object data, ref NetSdkNative.NetFtpInfo info)
        {
            if (info.ip == null) info = NetSdkNative.NetFtpInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.use_ssl = HlApiJson.ToByteFlag(HlApiJson.Get(d, "use_ssl"));
            FillBytes(info.ip, HlApiJson.ToString(HlApiJson.Get(d, "ip")));
            info.port_no = HlApiJson.ToUShort(HlApiJson.Get(d, "port_no"));
            FillBytes(info.user_name, HlApiJson.ToString(HlApiJson.Get(d, "user_name")));
            FillBytes(info.pwd, HlApiJson.ToString(HlApiJson.Get(d, "pwd")));
            info.enable_annoy = HlApiJson.ToByteFlag(HlApiJson.Get(d, "enable_annoy"));
            FillBytes(info.path_template, HlApiJson.ToString(HlApiJson.Get(d, "path_template")));
        }

        public static Dictionary<string, object> ToFtpInfoBody(ref NetSdkNative.NetFtpInfo info)
        {
            var body = HlApiJson.NewObject();
            body["use_ssl"] = info.use_ssl != 0;
            body["ip"] = FromBytes(info.ip);
            body["port_no"] = info.port_no;
            body["user_name"] = FromBytes(info.user_name);
            body["pwd"] = FromBytes(info.pwd);
            body["enable_annoy"] = info.enable_annoy != 0;
            body["path_template"] = FromBytes(info.path_template);
            return body;
        }

        public static void MapWebhookInfo(object data, ref NetSdkNative.NetWebhookInfo info)
        {
            if (info.url == null) info = NetSdkNative.NetWebhookInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            FillBytes(info.url, HlApiJson.ToString(HlApiJson.Get(d, "url")));
        }

        public static Dictionary<string, object> ToWebhookInfoBody(ref NetSdkNative.NetWebhookInfo info)
        {
            var body = HlApiJson.NewObject();
            body["url"] = FromBytes(info.url);
            return body;
        }

        public static void MapPortInfo(object data, ref NetSdkNative.NetPortInfo info)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.http_port_no = HlApiJson.ToUShort(HlApiJson.Get(d, "http_port_no"));
            info.rtsp_port_no = HlApiJson.ToUShort(HlApiJson.Get(d, "rtsp_port_no"));
            info.rtp_port_no = HlApiJson.ToUShort(HlApiJson.Get(d, "rtp_port_no"));
        }

        public static Dictionary<string, object> ToPortInfoBody(ref NetSdkNative.NetPortInfo info)
        {
            var body = HlApiJson.NewObject();
            body["http_port_no"] = info.http_port_no;
            body["rtsp_port_no"] = info.rtsp_port_no;
            body["rtp_port_no"] = info.rtp_port_no;
            return body;
        }

        public static void MapUserInfo(object data, ref NetSdkNative.NetUserInfo info)
        {
            if (info.user_name == null) info = NetSdkNative.NetUserInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.id = HlApiJson.ToUInt(HlApiJson.Get(d, "id"));
            info.user_level = (NetSdkNative.NetUserLevel)HlApiJson.ToInt(HlApiJson.Get(d, "user_level"));
            FillBytes(info.user_name, HlApiJson.ToString(HlApiJson.Get(d, "user_name")));
        }

        public static Dictionary<string, object> ToUserPwdBody(ref NetSdkNative.NetUserPwdInfo user)
        {
            var body = HlApiJson.NewObject();
            body["id"] = user.id;
            body["user_level"] = (int)user.user_level;
            body["user_name"] = FromBytes(user.user_name);
            body["login_pwd"] = FromBytes(user.login_pwd);
            body["pwd"] = FromBytes(user.pwd);
            return body;
        }

        public static void MapStreamInfo(object data, ref NetSdkNative.NetStreamInfo info)
        {
            if (info.codec_type == null) info = NetSdkNative.NetStreamInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.id = HlApiJson.ToUInt(HlApiJson.Get(d, "id"));
            info.bit_rate = HlApiJson.ToUInt(HlApiJson.Get(d, "bit_rate"));
            FillBytes(info.codec_type, HlApiJson.ToString(HlApiJson.Get(d, "codec_type")));
            info.disable_skip = HlApiJson.ToByteFlag(HlApiJson.Get(d, "disable_skip"));
            info.enable_rc = HlApiJson.ToByteFlag(HlApiJson.Get(d, "enable_rc"));
            info.gop_size = HlApiJson.ToUInt(HlApiJson.Get(d, "gop_size"));
            info.h264_profile = HlApiJson.ToUInt(HlApiJson.Get(d, "h264_profile"));
            info.initial_qp = HlApiJson.ToUInt(HlApiJson.Get(d, "initial_qp"));
            info.max_frame_rate = HlApiJson.ToUInt(HlApiJson.Get(d, "max_frame_rate"));
            info.maximum_qp = HlApiJson.ToUInt(HlApiJson.Get(d, "maximum_qp"));
            FillBytes(info.quality_control_type, HlApiJson.ToString(HlApiJson.Get(d, "quality_control_type")));
            info.resolution_width = HlApiJson.ToUShort(HlApiJson.Get(d, "resolution_width"));
            info.resolution_height = HlApiJson.ToUShort(HlApiJson.Get(d, "resolution_height"));
        }

        public static void MapStreamStatus(object data, ref NetSdkNative.NetStreamStatus status)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            status.id = HlApiJson.ToUInt(HlApiJson.Get(d, "id"));
            status.connection_count = HlApiJson.ToUInt(HlApiJson.Get(d, "connection_count"));
        }

        public static void MapChannelInfo(object data, ref NetSdkNative.NetChannelInfo info)
        {
            if (info.name == null) info = NetSdkNative.NetChannelInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.id = HlApiJson.ToUInt(HlApiJson.Get(d, "id"));
            FillBytes(info.name, HlApiJson.ToString(HlApiJson.Get(d, "name")));
        }

        public static Dictionary<string, object> ToChannelInfoBody(ref NetSdkNative.NetChannelInfo info)
        {
            var body = HlApiJson.NewObject();
            body["id"] = info.id;
            body["name"] = FromBytes(info.name);
            return body;
        }

        public static void MapImageParams(object data, ref NetSdkNative.NetImageParams param)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            param.contrast = HlApiJson.ToUShort(HlApiJson.Get(d, "contrast"));
            param.brightness = HlApiJson.ToUShort(HlApiJson.Get(d, "brightness"));
            param.dde_enable = HlApiJson.ToByteFlag(HlApiJson.Get(d, "dde_enable"));
            param.dde_value = HlApiJson.ToUShort(HlApiJson.Get(d, "dde_value"));
        }

        public static Dictionary<string, object> ToImageParamsBody(ref NetSdkNative.NetImageParams param)
        {
            var body = HlApiJson.NewObject();
            body["contrast"] = param.contrast;
            body["brightness"] = param.brightness;
            body["dde_enable"] = param.dde_enable != 0;
            body["dde_value"] = param.dde_value;
            return body;
        }

        public static void MapOverlayBlock(object data, ref NetSdkNative.NetOverlayBlock block)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            block.align = (NetSdkNative.NetOverlayAlign)HlApiJson.ToInt(HlApiJson.Get(d, "align"));
            block.enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "enabled"));
            block.mode = (NetSdkNative.NetOverlayDisplayMode)HlApiJson.ToInt(HlApiJson.Get(d, "mode"));
            block.x = HlApiJson.ToUInt(HlApiJson.Get(d, "x"));
            block.y = HlApiJson.ToUInt(HlApiJson.Get(d, "y"));
        }

        public static Dictionary<string, object> ToOverlayBlockBody(ref NetSdkNative.NetOverlayBlock block)
        {
            var body = HlApiJson.NewObject();
            body["align"] = (int)block.align;
            body["enabled"] = block.enabled != 0;
            body["mode"] = (int)block.mode;
            body["x"] = block.x;
            body["y"] = block.y;
            return body;
        }

        public static void MapOverlayCustom(object data, ref NetSdkNative.NetOverlayCustomBlock block)
        {
            if (block.text == null) block = NetSdkNative.NetOverlayCustomBlock.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            block.align = (NetSdkNative.NetOverlayAlign)HlApiJson.ToInt(HlApiJson.Get(d, "align"));
            block.enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "enabled"));
            block.mode = (NetSdkNative.NetOverlayDisplayMode)HlApiJson.ToInt(HlApiJson.Get(d, "mode"));
            block.x = HlApiJson.ToUInt(HlApiJson.Get(d, "x"));
            block.y = HlApiJson.ToUInt(HlApiJson.Get(d, "y"));
            FillBytes(block.text, HlApiJson.ToString(HlApiJson.Get(d, "text")));
        }

        public static Dictionary<string, object> ToOverlayCustomBody(ref NetSdkNative.NetOverlayCustomBlock block)
        {
            var body = HlApiJson.NewObject();
            body["align"] = (int)block.align;
            body["enabled"] = block.enabled != 0;
            body["mode"] = (int)block.mode;
            body["x"] = block.x;
            body["y"] = block.y;
            body["text"] = FromBytes(block.text);
            return body;
        }

        public static void MapOverlayInfo(object data, ref NetSdkNative.NetOverlayInfo overlay)
        {
            if (overlay.custom_overlay.text == null) overlay = NetSdkNative.NetOverlayInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            MapOverlayBlock(HlApiJson.Get(d, "channel_name_overlay"), ref overlay.channel_name_overlay);
            MapOverlayBlock(HlApiJson.Get(d, "time_overlay"), ref overlay.time_overlay);
            MapOverlayCustom(HlApiJson.Get(d, "custom_overlay"), ref overlay.custom_overlay);
        }

        public static Dictionary<string, object> ToOverlayInfoBody(ref NetSdkNative.NetOverlayInfo overlay)
        {
            var body = HlApiJson.NewObject();
            body["channel_name_overlay"] = ToOverlayBlockBody(ref overlay.channel_name_overlay);
            body["time_overlay"] = ToOverlayBlockBody(ref overlay.time_overlay);
            body["custom_overlay"] = ToOverlayCustomBody(ref overlay.custom_overlay);
            return body;
        }

        public static void MapFollowBlock(object data, ref NetSdkNative.NetOverlayFollowBlock block)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            block.align = (NetSdkNative.NetOverlayAlign)HlApiJson.ToInt(HlApiJson.Get(d, "align"));
            block.enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "enabled"));
            block.follow = HlApiJson.ToByteFlag(HlApiJson.Get(d, "follow"));
            block.max_enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "max_enabled"));
            block.min_enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "min_enabled"));
            block.avg_enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "avg_enabled"));
            block.emissivity_enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "emissivity_enabled"));
            block.x = HlApiJson.ToUInt(HlApiJson.Get(d, "x"));
            block.y = HlApiJson.ToUInt(HlApiJson.Get(d, "y"));
        }

        public static Dictionary<string, object> ToFollowBlockBody(ref NetSdkNative.NetOverlayFollowBlock block)
        {
            var body = HlApiJson.NewObject();
            body["align"] = (int)block.align;
            body["enabled"] = block.enabled != 0;
            body["follow"] = block.follow != 0;
            body["max_enabled"] = block.max_enabled != 0;
            body["min_enabled"] = block.min_enabled != 0;
            body["avg_enabled"] = block.avg_enabled != 0;
            body["emissivity_enabled"] = block.emissivity_enabled != 0;
            body["x"] = block.x;
            body["y"] = block.y;
            return body;
        }

        public static void MapThermalOverlay(object data, ref NetSdkNative.NetThermalOverlayInfo overlay)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            MapOverlayBlock(HlApiJson.Get(d, "unit_overlay"), ref overlay.unit_overlay);
            MapOverlayBlock(HlApiJson.Get(d, "plt_overlay"), ref overlay.plt_overlay);
            MapOverlayBlock(HlApiJson.Get(d, "thermal_params_overlay"), ref overlay.thermal_params_overlay);
            MapFollowBlock(HlApiJson.Get(d, "global_overlay"), ref overlay.global_overlay);
            MapFollowBlock(HlApiJson.Get(d, "marker_overlay"), ref overlay.marker_overlay);
            MapOverlayBlock(HlApiJson.Get(d, "alarm_overlay"), ref overlay.alarm_overlay);
        }

        public static Dictionary<string, object> ToThermalOverlayBody(ref NetSdkNative.NetThermalOverlayInfo overlay)
        {
            var body = HlApiJson.NewObject();
            body["unit_overlay"] = ToOverlayBlockBody(ref overlay.unit_overlay);
            body["plt_overlay"] = ToOverlayBlockBody(ref overlay.plt_overlay);
            body["thermal_params_overlay"] = ToOverlayBlockBody(ref overlay.thermal_params_overlay);
            body["global_overlay"] = ToFollowBlockBody(ref overlay.global_overlay);
            body["marker_overlay"] = ToFollowBlockBody(ref overlay.marker_overlay);
            body["alarm_overlay"] = ToOverlayBlockBody(ref overlay.alarm_overlay);
            return body;
        }

        public static void MapLensInfo(object data, ref NetSdkNative.NetLensInfo info)
        {
            if (info.name == null) info = NetSdkNative.NetLensInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            FillBytes(info.name, HlApiJson.ToString(HlApiJson.Get(d, "name")));
            info.focal = HlApiJson.ToFloat(HlApiJson.Get(d, "focal"));
            info.vfov = HlApiJson.ToFloat(HlApiJson.Get(d, "vfov"));
            info.hfov = HlApiJson.ToFloat(HlApiJson.Get(d, "hfov"));
            FillBytes(info.band, HlApiJson.ToString(HlApiJson.Get(d, "band")));
            var ranges = HlApiJson.AsArray(HlApiJson.Get(d, "range_map"));
            info.ranges_count = 0;
            if (ranges == null) return;
            for (int i = 0; i < ranges.Count && i < NetSdkNative.NET_MAX_RANGES; i++)
            {
                var item = HlApiJson.AsDict(ranges[i]);
                if (item == null) continue;
                info.range_map[i].id = HlApiJson.ToUShort(HlApiJson.Get(item, "id"));
                var range = HlApiJson.AsDict(HlApiJson.Get(item, "range"));
                if (range != null)
                {
                    info.range_map[i].range.min = HlApiJson.ToFloat(HlApiJson.Get(range, "min"));
                    info.range_map[i].range.max = HlApiJson.ToFloat(HlApiJson.Get(range, "max"));
                }
                info.ranges_count++;
            }
        }

        public static void MapLutTable(object data, ref NetSdkNative.NetLutTable lut)
        {
            if (lut.ad == null) lut = NetSdkNative.NetLutTable.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            lut.range_id = HlApiJson.ToUShort(HlApiJson.Get(d, "range_id"));
            lut.length = HlApiJson.ToUShort(HlApiJson.Get(d, "length"));
            var ad = HlApiJson.AsArray(HlApiJson.Get(d, "ad"));
            var t = HlApiJson.AsArray(HlApiJson.Get(d, "t"));
            int n = Math.Min((int)lut.length, NetSdkNative.NET_MAX_LUT_POINTS);
            if (ad != null)
            {
                n = Math.Min(n, ad.Count);
                for (int i = 0; i < n; i++)
                    lut.ad[i] = HlApiJson.ToUShort(ad[i]);
            }
            if (t != null)
            {
                int tn = Math.Min(n, t.Count);
                for (int i = 0; i < tn; i++)
                    lut.t[i] = HlApiJson.ToFloat(t[i]);
            }
            lut.length = (ushort)n;
        }

        public static void MapThermalParams(object data, ref NetSdkNative.NetThermalParams param)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            param.emissivity = HlApiJson.ToFloat(HlApiJson.Get(d, "emissivity"));
            param.distance = HlApiJson.ToFloat(HlApiJson.Get(d, "distance"));
            param.refl_temp = HlApiJson.ToFloat(HlApiJson.Get(d, "refl_temp"));
            param.atm_temp = HlApiJson.ToFloat(HlApiJson.Get(d, "atm_temp"));
            param.rel_humidity = HlApiJson.ToFloat(HlApiJson.Get(d, "rel_humidity"));
        }

        public static Dictionary<string, object> ToThermalParamsBody(ref NetSdkNative.NetThermalParams param)
        {
            var body = HlApiJson.NewObject();
            body["emissivity"] = param.emissivity;
            body["distance"] = param.distance;
            body["refl_temp"] = param.refl_temp;
            body["atm_temp"] = param.atm_temp;
            body["rel_humidity"] = param.rel_humidity;
            return body;
        }

        public static void MapPaletteName(object data, ref NetSdkNative.NetPaletteName palette)
        {
            if (palette.name == null) palette = NetSdkNative.NetPaletteName.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            palette.ref_no = HlApiJson.ToUInt(HlApiJson.Get(d, "ref_no"));
            FillBytes(palette.name, HlApiJson.ToString(HlApiJson.Get(d, "name")));
        }

        public static void MapPalette(object data, ref NetSdkNative.NetPalette palette)
        {
            if (palette.colors == null) palette = NetSdkNative.NetPalette.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            palette.ref_no = HlApiJson.ToUInt(HlApiJson.Get(d, "ref_no"));
            palette.inverted = HlApiJson.ToByteFlag(HlApiJson.Get(d, "inverted"));
            var colors = HlApiJson.AsArray(HlApiJson.Get(d, "colors"));
            palette.color_count = 0;
            if (colors == null) return;
            int n = Math.Min(colors.Count, NetSdkNative.NET_MAX_PALETTE_COLORS);
            for (int i = 0; i < n; i++)
                palette.colors[i] = HlApiJson.ToUInt(colors[i]);
            palette.color_count = (ushort)n;
            if (d.ContainsKey("color_count"))
                palette.color_count = Math.Min(palette.color_count, HlApiJson.ToUShort(HlApiJson.Get(d, "color_count")));
        }

        public static Dictionary<string, object> ToPaletteBody(ref NetSdkNative.NetPalette palette)
        {
            var body = HlApiJson.NewObject();
            body["ref_no"] = palette.ref_no;
            body["inverted"] = palette.inverted != 0;
            var colors = HlApiJson.NewArray();
            int n = Math.Min((int)palette.color_count, palette.colors != null ? palette.colors.Length : 0);
            for (int i = 0; i < n; i++)
                colors.Add(palette.colors[i]);
            body["colors"] = colors;
            body["color_count"] = n;
            return body;
        }

        public static void MapGain(object data, ref NetSdkNative.NetGain gain)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            gain.mode = (NetSdkNative.NetGainMode)HlApiJson.ToInt(HlApiJson.Get(d, "mode"));
            gain.max = HlApiJson.ToFloat(HlApiJson.Get(d, "max"));
            gain.min = HlApiJson.ToFloat(HlApiJson.Get(d, "min"));
            gain.fixedSpan = HlApiJson.ToByteFlag(HlApiJson.Get(d, "fixedSpan"));
            gain.span = HlApiJson.ToFloat(HlApiJson.Get(d, "span"));
        }

        public static Dictionary<string, object> ToGainBody(ref NetSdkNative.NetGain gain)
        {
            var body = HlApiJson.NewObject();
            body["mode"] = (int)gain.mode;
            body["max"] = gain.max;
            body["min"] = gain.min;
            body["fixedSpan"] = gain.fixedSpan != 0;
            body["span"] = gain.span;
            return body;
        }

        public static void MapUnitInfo(object data, ref NetSdkNative.NetUnitInfo info)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.temp_unit = (NetSdkNative.NetTempUnit)HlApiJson.ToInt(HlApiJson.Get(d, "temp_unit"));
            info.distance_unit = (NetSdkNative.NetDistanceUnit)HlApiJson.ToInt(HlApiJson.Get(d, "distance_unit"));
        }

        public static Dictionary<string, object> ToUnitInfoBody(ref NetSdkNative.NetUnitInfo info)
        {
            var body = HlApiJson.NewObject();
            body["temp_unit"] = (int)info.temp_unit;
            body["distance_unit"] = (int)info.distance_unit;
            return body;
        }

        public static void MapMarkerInfo(object data, ref NetSdkNative.NetMarkerInfo info)
        {
            if (info.name == null) info = NetSdkNative.NetMarkerInfo.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            info.id = HlApiJson.ToUInt(HlApiJson.Get(d, "id"));
            info.type = (NetSdkNative.NetMarkerType)HlApiJson.ToInt(HlApiJson.Get(d, "type"));
            FillBytes(info.name, HlApiJson.ToString(HlApiJson.Get(d, "name")));
            info.local_params = HlApiJson.ToBool(HlApiJson.Get(d, "local_params")) ? 1u : 0u;
            info.refl_temp = HlApiJson.ToFloat(HlApiJson.Get(d, "refl_temp"));
            info.distance = HlApiJson.ToFloat(HlApiJson.Get(d, "distance"));
            info.emissivity = HlApiJson.ToFloat(HlApiJson.Get(d, "emissivity"));
            var points = HlApiJson.AsArray(HlApiJson.Get(d, "points"));
            info.points_count = 0;
            if (points == null) return;
            for (int i = 0; i < points.Count && i < NetSdkNative.NET_MAX_MARKER_POINTS; i++)
            {
                var p = HlApiJson.AsDict(points[i]);
                if (p == null) continue;
                info.points[i].x = HlApiJson.ToUShort(HlApiJson.Get(p, "x"));
                info.points[i].y = HlApiJson.ToUShort(HlApiJson.Get(p, "y"));
                info.points_count++;
            }
        }

        public static Dictionary<string, object> ToMarkerInfoBody(ref NetSdkNative.NetMarkerInfo info)
        {
            var body = HlApiJson.NewObject();
            body["id"] = info.id;
            body["type"] = (int)info.type;
            body["name"] = FromBytes(info.name);
            body["local_params"] = info.local_params != 0;
            body["refl_temp"] = info.refl_temp;
            body["distance"] = info.distance;
            body["emissivity"] = info.emissivity;
            var points = HlApiJson.NewArray();
            int n = Math.Min((int)info.points_count, info.points != null ? info.points.Length : 0);
            for (int i = 0; i < n; i++)
            {
                var p = HlApiJson.NewObject();
                p["x"] = info.points[i].x;
                p["y"] = info.points[i].y;
                points.Add(p);
            }
            body["points"] = points;
            return body;
        }

        public static void MapTempAlarm(object data, ref NetSdkNative.NetThermalTempAlarm alarm)
        {
            if (alarm.marker_name == null) alarm = NetSdkNative.NetThermalTempAlarm.Create();
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            FillBytes(alarm.marker_name, HlApiJson.ToString(HlApiJson.Get(d, "marker_name")));
            alarm.enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "enabled"));
            alarm.alarm_type = (NetSdkNative.NetThermalTempAlarmType)HlApiJson.ToInt(HlApiJson.Get(d, "alarm_type"));
            alarm.alarm_temp = HlApiJson.ToFloat(HlApiJson.Get(d, "alarm_temp"));
            alarm.alarm_delay_time = HlApiJson.ToUInt(HlApiJson.Get(d, "alarm_delay_time"));
            alarm.warning_temp = HlApiJson.ToFloat(HlApiJson.Get(d, "warning_temp"));
            alarm.warning_delay_time = HlApiJson.ToUInt(HlApiJson.Get(d, "warning_delay_time"));
            alarm.alert_temp = HlApiJson.ToFloat(HlApiJson.Get(d, "alert_temp"));
            alarm.alert_delay_time = HlApiJson.ToUInt(HlApiJson.Get(d, "alert_delay_time"));
        }

        public static Dictionary<string, object> ToTempAlarmBody(ref NetSdkNative.NetThermalTempAlarm alarm)
        {
            var body = HlApiJson.NewObject();
            body["marker_name"] = FromBytes(alarm.marker_name);
            body["enabled"] = alarm.enabled != 0;
            body["alarm_type"] = (int)alarm.alarm_type;
            body["alarm_temp"] = alarm.alarm_temp;
            body["alarm_delay_time"] = alarm.alarm_delay_time;
            body["warning_temp"] = alarm.warning_temp;
            body["warning_delay_time"] = alarm.warning_delay_time;
            body["alert_temp"] = alarm.alert_temp;
            body["alert_delay_time"] = alarm.alert_delay_time;
            return body;
        }

        public static void MapDpcParams(ref NetSdkNative.NetDpcParams param, out Dictionary<string, object> body)
        {
            body = HlApiJson.NewObject();
            var points = HlApiJson.NewArray();
            int n = Math.Min((int)param.count, param.points != null ? param.points.Length : 0);
            for (int i = 0; i < n; i++)
            {
                var p = HlApiJson.NewObject();
                p["x"] = param.points[i].x;
                p["y"] = param.points[i].y;
                points.Add(p);
            }
            body["points"] = points;
        }

        public static void MapAlarmTrigger(object data, ref NetSdkNative.NetAlarmTrigger trigger)
        {
            var d = HlApiJson.AsDict(data);
            if (d == null) return;
            trigger.trigger_ftp_enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "trigger_ftp_enabled"));
            trigger.trigger_webhook_enabled = HlApiJson.ToByteFlag(HlApiJson.Get(d, "trigger_webhook_enabled"));
        }

        public static Dictionary<string, object> ToAlarmTriggerBody(ref NetSdkNative.NetAlarmTrigger trigger)
        {
            var body = HlApiJson.NewObject();
            body["trigger_ftp_enabled"] = trigger.trigger_ftp_enabled != 0;
            body["trigger_webhook_enabled"] = trigger.trigger_webhook_enabled != 0;
            return body;
        }

        public static int FillList<T>(ArrayList source, T[] dest, ushort length, out ushort outLength, Action<object, int> mapItem)
        {
            outLength = 0;
            if (dest == null)
                return (int)NetSdkNative.NetErrorCode.NullPointer;
            if (source == null)
                return (int)NetSdkNative.NetErrorCode.OK;
            ushort n = (ushort)Math.Min(Math.Min(source.Count, (int)length), dest.Length);
            for (int i = 0; i < n; i++)
                mapItem(source[i], i);
            outLength = n;
            return (int)NetSdkNative.NetErrorCode.OK;
        }
    }
}
