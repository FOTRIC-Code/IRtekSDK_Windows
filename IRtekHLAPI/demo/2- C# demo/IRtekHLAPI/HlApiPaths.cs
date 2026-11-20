namespace IRtekHLAPI
{
    internal static class HlApiPaths
    {
        public static class System
        {
            public const string DeviceInfo = "/system/device_info";
            public const string DeviceRestart = "/system/device_restart";
            public const string DeviceReset = "/system/device_reset";
            public const string DeviceUpgrade = "/system/device_upgrade";
            public const string NtpServer = "/system/ntp_server";
            public const string Time = "/system/time";
            public const string Port = "/system/port";
            public const string Ftp = "/system/ftp";
            public const string FtpTest = "/system/ftp/test";
            public const string Webhook = "/system/webhook";
            public const string Networks = "/system/networks";
            public const string Network = "/system/networks/{id}";
        }

        public static class Security
        {
            public const string Users = "/security/users";
            public const string User = "/security/users/{id}";
        }

        public static class Streaming
        {
            public const string Channels = "/streaming/channels";
            public const string Channel = "/streaming/channels/{id}";
            public const string ChannelStatus = "/streaming/channels/{id}/status";
        }

        public static class Image
        {
            public const string ImageParams = "/image/channels/{id}/image_params";
            public const string ImageFlip = "/image/channels/{id}/image_flip";
        }

        public static class Video
        {
            public const string Channels = "/video/channels";
            public const string Channel = "/video/channels/{id}";
            public const string ChannelOverlay = "/video/channels/{id}/overlay";
        }

        public static class Thermal
        {
            public const string LensInfo = "/thermal/lens_info";
            public const string Calibrate = "/thermal/thermal_calibrate";
            public const string ShutterMode = "/thermal/thermal_shutter_mode";
            public const string ShutterTime = "/thermal/thermal_shutter_time";
            public const string Resolution = "/thermal/thermal_resolution";
            public const string Range = "/thermal/thermal_range";
            public const string Lut = "/thermal/thermal_lut";
            public const string Params = "/thermal/thermal_params";
            public const string ColorDist = "/thermal/color_dist";
            public const string Gain = "/thermal/gain";
            public const string Palettes = "/thermal/preset_palettes";
            public const string Palette = "/thermal/palette";
            public const string Overlay = "/thermal/thermal_overlay";
            public const string Unit = "/thermal/thermal_unit";
            public const string Capture = "/thermal/capture";
            public const string Marker = "/thermal/marker";
            public const string Alarm = "/thermal/temp/alarm";
            public const string AlarmInterval = "/thermal/temp/alarm/interval";
            public const string Dpc = "/thermal/dpc";
            public const string Data = "/thermal/data/{data_types}";
        }

        public static class Event
        {
            public const string Trigger = "/alarm/triggers/{type}";
        }

        public static class Session
        {
            public const string Alarm = "/session?role=alarm";
        }

        public static string WithId(string template, uint id)
        {
            return template.Replace("{id}", id.ToString());
        }

        public static string WithType(string template, int type)
        {
            return template.Replace("{type}", type.ToString());
        }

        public static string WithDataTypes(string template, uint dataTypes)
        {
            return template.Replace("{data_types}", dataTypes.ToString());
        }
    }
}
