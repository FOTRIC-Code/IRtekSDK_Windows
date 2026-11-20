using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IRtekNetSDK
{
    public static class NetSdkNative
    {
        private const string DllName = "IRtekNetSDK.dll";
        private const CallingConvention CC = CallingConvention.StdCall;

        #region Constants

        public const int NET_LEN_16 = 16;
        public const int NET_LEN_32 = 32;
        public const int NET_LEN_64 = 64;
        public const int NET_LEN_128 = 128;
        public const int NET_LEN_256 = 256;

        public const int NET_MAX_RANGES = 4;
        public const int NET_MAX_NTP_SERVERS = 4;
        public const int NET_MAX_DNS = 4;
        public const int NET_MAX_GATEWAYS = 4;
        public const int NET_MAX_LUT_POINTS = 64;
        public const int NET_MAX_PALETTE_COLORS = 256;
        public const int NET_MAX_MARKER_POINTS = 10;
        public const int NET_MAX_MARKERS = 16;
        public const int NET_MAX_REGIONS = 3;
        public const int NET_MAX_ACTIVE_ALARMS = 16;
        public const int NET_MAX_SCHEDULE_DAYS = 7;
        public const int NET_MAX_SCHEDULE_PERIODS = 8;
        public const int NET_MAX_WEBHOOKS = 3;
        public const int NET_DISK_ALL = 0xFF;

        #endregion

        #region Enums

        public enum NetErrorCode
        {
            OK = 0,
            Fail = -1,
            Memory = -2,
            NullPointer = -3,
            InvalidParam = -4,
            NotSupported = -5,
            NotInitialized = -6,
            HttpError = -7,
            AuthFailed = -8,
            Timeout = -9
        }

        public enum NetShutterMode
        {
            Automatic = 0,
            Manual = 1
        }

        public enum NetImageFlipMode
        {
            None = 0,
            UpDown = 1,
            LeftRight = 2,
            Center = 3
        }

        public enum NetTimeMode
        {
            Local = 0,
            NTP = 1
        }

        public enum NetUserLevel
        {
            Root = 0,
            Operator = 1,
            Viewer = 2
        }

        public enum NetOverlayAlign
        {
            TopLeft = 0,
            TopRight = 1,
            BottomLeft = 2,
            BottomRight = 3
        }

        public enum NetOverlayDisplayMode
        {
            Light = 0,
            Blink = 1
        }

        public enum NetExceptionType
        {
            NetworkDisconnected = 1,
            NetworkResumed = 2,
            VideoLost = 3,
            AlarmLost = 4
        }

        public enum NetThermalTempAlarmType
        {
            HighestGreater = 1,
            HighestLess = 2,
            LowestGreater = 3,
            LowestLess = 4,
            AverageGreater = 5,
            AverageLess = 6
        }

        public enum NetAlarmType
        {
            ThermalTemp = 1,
            Intrusion = 2,
            HighTemp = 3,
            TempDetect = 4,
            AcousticSource = 5,
            AcousticDb = 6,
            AcousticFreq = 7
        }

        public enum NetAcousticCaptureMode
        {
            PureImage = 0,
            IrtekImage = 1
        }

        public enum NetAcousticDbScaleMode
        {
            AutoMono = 1,
            AutoMulti = 2,
            Fixed = 3,
            AutoHolo = 4
        }

        [Flags]
        public enum NetAcousticDataType : uint
        {
            Visual = 0x01,
            Spl = 0x02,
            Spectrum = 0x04,
            Global = 0x08
        }

        public enum NetStreamType
        {
            H264 = 1,
            RGBA = 2,
            YUV422 = 3,
            BGRA = 4
        }

        public enum NetThermalCaptureMode
        {
            PureImage = 0,
            IrtekImage = 1
        }

        public enum NetMarkerType
        {
            Spot = 1,
            Polygon = 5,
            Polyline = 6
        }

        public enum NetThermalDataType : uint
        {
            Raw = 0x01,
            Global = 0x02,
            Marker = 0x04
        }

        public enum NetThermalTempAlarmLevel
        {
            None = 0,
            Alert = 1,
            Warning = 2,
            Alarm = 3
        }

        public enum NetColorDistType
        {
            TWB = 1,
            TempLinear = 2,
            ADLinear = 3
        }

        public enum NetGainMode
        {
            Fixed = 1,
            Auto = 2
        }

        public enum NetTempUnit
        {
            Centigrade = 0,
            Kelvin = 1,
            Fahrenheit = 2
        }

        public enum NetDistanceUnit
        {
            Meter = 0,
            Foot = 1
        }

        public enum NetSrMode
        {
            None = 0,
            SR = 1
        }

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct NetLoginInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] ip;
            public ushort port_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] user_name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] password;
            public uint timeout_ms;

            public static NetLoginInfo Create(string ip, ushort port, string userName, string password, uint timeoutMs = 5000)
            {
                return new NetLoginInfo
                {
                    ip = StringToFixedBytes(ip, NET_LEN_32),
                    port_no = port,
                    user_name = StringToFixedBytes(userName, NET_LEN_32),
                    password = StringToFixedBytes(password, NET_LEN_32),
                    timeout_ms = timeoutMs
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetSdkVersion
        {
            public ushort major;
            public ushort minor;
            public ushort patch;
            public ushort build;

            public override string ToString() => $"{major}.{minor}.{patch}.{build}";
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetTempRange
        {
            public float min;
            public float max;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetTempRangeMap
        {
            public ushort id;
            public NetTempRange range;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetSize
        {
            public ushort width;
            public ushort height;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetDeviceInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] model;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] spec;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_16)]
            public byte[] serial_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] brand;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] company;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] firmware_name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] firmware_version;

            public static NetDeviceInfo Create()
            {
                return new NetDeviceInfo
                {
                    name = new byte[NET_LEN_128],
                    model = new byte[NET_LEN_128],
                    spec = new byte[NET_LEN_128],
                    serial_no = new byte[NET_LEN_16],
                    id = new byte[NET_LEN_32],
                    brand = new byte[NET_LEN_32],
                    company = new byte[NET_LEN_64],
                    firmware_name = new byte[NET_LEN_128],
                    firmware_version = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetLensInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] name;
            public float focal;
            public float vfov;
            public float hfov;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] band;
            public ushort ranges_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_RANGES)]
            public NetTempRangeMap[] range_map;

            public static NetLensInfo Create()
            {
                return new NetLensInfo
                {
                    name = new byte[NET_LEN_64],
                    band = new byte[NET_LEN_32],
                    range_map = new NetTempRangeMap[NET_MAX_RANGES]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetTimeInfo
        {
            public NetTimeMode time_mode;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] local_time;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] time_zone;

            public static NetTimeInfo Create()
            {
                return new NetTimeInfo
                {
                    local_time = new byte[NET_LEN_32],
                    time_zone = new byte[NET_LEN_32]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetNtpInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] host_name;
            public ushort port_no;
            public uint syn_interval;

            public static NetNtpInfo Create()
            {
                return new NetNtpInfo
                {
                    host_name = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetNetworkInfo
        {
            public uint id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] name;
            public byte dhcp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] ip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] netmask;
            public ushort dns_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_DNS * NET_LEN_64)]
            public byte[] dns;
            public ushort gateway_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_GATEWAYS * NET_LEN_64)]
            public byte[] gateway;

            public static NetNetworkInfo Create()
            {
                return new NetNetworkInfo
                {
                    name = new byte[NET_LEN_32],
                    ip = new byte[NET_LEN_64],
                    netmask = new byte[NET_LEN_64],
                    dns = new byte[NET_MAX_DNS * NET_LEN_64],
                    gateway = new byte[NET_MAX_GATEWAYS * NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetFtpInfo
        {
            public byte use_ssl;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] ip;
            public ushort port_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] user_name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] pwd;
            public byte enable_annoy;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] path_template;

            public static NetFtpInfo Create()
            {
                return new NetFtpInfo
                {
                    ip = new byte[NET_LEN_64],
                    user_name = new byte[NET_LEN_32],
                    pwd = new byte[NET_LEN_64],
                    path_template = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetWebhookInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_256)]
            public byte[] url;

            public static NetWebhookInfo Create()
            {
                return new NetWebhookInfo
                {
                    url = new byte[NET_LEN_256]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetPortInfo
        {
            public ushort http_port_no;
            public ushort rtsp_port_no;
            public ushort rtp_port_no;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetUpgradeInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] path;

            public static NetUpgradeInfo FromFilePath(string filePath)
            {
                return new NetUpgradeInfo
                {
                    path = StringToFixedBytes(filePath, NET_LEN_128)
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetUserInfo
        {
            public uint id;
            public NetUserLevel user_level;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] user_name;

            public static NetUserInfo Create()
            {
                return new NetUserInfo
                {
                    user_name = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetUserPwdInfo
        {
            public uint id;
            public NetUserLevel user_level;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] user_name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] login_pwd;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] pwd;

            public static NetUserPwdInfo Create()
            {
                return new NetUserPwdInfo
                {
                    user_name = new byte[NET_LEN_64],
                    login_pwd = new byte[NET_LEN_64],
                    pwd = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetStreamInfo
        {
            public uint id;
            public uint bit_rate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_16)]
            public byte[] codec_type;
            public byte disable_skip;
            public byte enable_rc;
            public uint gop_size;
            public uint h264_profile;
            public uint initial_qp;
            public uint max_frame_rate;
            public uint maximum_qp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_16)]
            public byte[] quality_control_type;
            public ushort resolution_width;
            public ushort resolution_height;

            public static NetStreamInfo Create()
            {
                return new NetStreamInfo
                {
                    codec_type = new byte[NET_LEN_16],
                    quality_control_type = new byte[NET_LEN_16]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetStreamStatus
        {
            public uint id;
            public uint connection_count;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetImageParams
        {
            public ushort contrast;
            public ushort brightness;
            public byte dde_enable;
            public ushort dde_value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetChannelInfo
        {
            public uint id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] name;

            public static NetChannelInfo Create()
            {
                return new NetChannelInfo
                {
                    name = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetOverlayBlock
        {
            public NetOverlayAlign align;
            public byte enabled;
            public NetOverlayDisplayMode mode;
            public uint x;
            public uint y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetOverlayCustomBlock
        {
            public NetOverlayAlign align;
            public byte enabled;
            public NetOverlayDisplayMode mode;
            public uint x;
            public uint y;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] text;

            public static NetOverlayCustomBlock Create()
            {
                return new NetOverlayCustomBlock
                {
                    text = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetOverlayInfo
        {
            public NetOverlayBlock channel_name_overlay;
            public NetOverlayBlock time_overlay;
            public NetOverlayCustomBlock custom_overlay;

            public static NetOverlayInfo Create()
            {
                return new NetOverlayInfo
                {
                    custom_overlay = NetOverlayCustomBlock.Create()
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetLutTable
        {
            public ushort range_id;
            public ushort length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_LUT_POINTS)]
            public ushort[] ad;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_LUT_POINTS)]
            public float[] t;

            public static NetLutTable Create()
            {
                return new NetLutTable
                {
                    ad = new ushort[NET_MAX_LUT_POINTS],
                    t = new float[NET_MAX_LUT_POINTS]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetThermalTempAlarm
        {
            // 标识名称：报警配置通过名称和测温对象匹配 / Marker name: matching marker alarm config with thermal objects
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] marker_name;
            public byte enabled;
            public NetThermalTempAlarmType alarm_type;
            public float alarm_temp;
            public uint alarm_delay_time;
            public float warning_temp;
            public uint warning_delay_time;
            public float alert_temp;
            public uint alert_delay_time;

            public static NetThermalTempAlarm Create()
            {
                return new NetThermalTempAlarm
                {
                    marker_name = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetThermalTempAlarmInterval
        {
            public ushort interval;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetRegion
        {
            public uint id;
            public ushort points_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKER_POINTS)]
            public NetPoint[] points;

            public static NetRegion Create()
            {
                return new NetRegion
                {
                    points = new NetPoint[NET_MAX_MARKER_POINTS]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetIntrusionAlarm
        {
            public byte enabled;
            public uint no_repeat_minutes;
            public uint sensitivity;
            public uint record_duration_sec;
            public ushort regions_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_REGIONS)]
            public NetRegion[] regions;

            public static NetIntrusionAlarm Create()
            {
                var alarm = new NetIntrusionAlarm
                {
                    regions = new NetRegion[NET_MAX_REGIONS]
                };
                for (int i = 0; i < NET_MAX_REGIONS; i++)
                    alarm.regions[i] = NetRegion.Create();
                return alarm;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetHighTempAlarm
        {
            public byte enabled;
            public uint no_repeat_minutes;
            public float alarm_temp;
            public uint record_duration_sec;
            public ushort regions_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_REGIONS)]
            public NetRegion[] regions;

            public static NetHighTempAlarm Create()
            {
                var alarm = new NetHighTempAlarm
                {
                    regions = new NetRegion[NET_MAX_REGIONS]
                };
                for (int i = 0; i < NET_MAX_REGIONS; i++)
                    alarm.regions[i] = NetRegion.Create();
                return alarm;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetTempDetectAlarm
        {
            public byte enabled;
            public uint sensitivity;
            public ushort regions_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_REGIONS)]
            public NetRegion[] regions;

            public static NetTempDetectAlarm Create()
            {
                var alarm = new NetTempDetectAlarm
                {
                    regions = new NetRegion[NET_MAX_REGIONS]
                };
                for (int i = 0; i < NET_MAX_REGIONS; i++)
                    alarm.regions[i] = NetRegion.Create();
                return alarm;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetThermalParams
        {
            public float emissivity;
            public float distance;
            public float refl_temp;
            public float atm_temp;
            public float rel_humidity;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetPaletteName
        {
            public uint ref_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] name;

            public static NetPaletteName Create()
            {
                return new NetPaletteName
                {
                    name = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetPalette
        {
            public uint ref_no;
            public ushort color_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_PALETTE_COLORS)]
            public uint[] colors;
            public byte inverted;

            public static NetPalette Create()
            {
                return new NetPalette
                {
                    colors = new uint[NET_MAX_PALETTE_COLORS]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetGain
        {
            public NetGainMode mode;
            public float max;
            public float min;
            public byte fixedSpan;
            public float span;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetOverlayFollowBlock
        {
            public NetOverlayAlign align;
            public byte enabled;
            public byte follow;
            public byte max_enabled;
            public byte min_enabled;
            public byte avg_enabled;
            public byte emissivity_enabled;
            public uint x;
            public uint y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetExceptionInfo
        {
            public NetExceptionType exception_type;
            public uint stream_id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetStreamData
        {
            public uint stream_id;
            public NetStreamType stream_type;
            public ushort width;
            public ushort height;
            public uint data_length;
            public IntPtr data;
            public ulong timestamp_us;
            public byte key_frame;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetThermalOverlayInfo
        {
            public NetOverlayBlock unit_overlay;
            public NetOverlayBlock plt_overlay;
            public NetOverlayBlock thermal_params_overlay;
            public NetOverlayFollowBlock global_overlay;
            public NetOverlayFollowBlock marker_overlay;
            public NetOverlayBlock alarm_overlay;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetPoint
        {
            public ushort x;
            public ushort y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetTempPoint
        {
            public float temperature;
            public NetPoint point;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetTempPointInfo
        {
            public NetTempPoint max;
            public NetTempPoint min;
            public float avg_temp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetMarkerInfo
        {
            public uint id;
            public NetMarkerType type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] name;
            public uint local_params;
            public float refl_temp;
            public float distance;
            public float emissivity;
            public ushort points_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKER_POINTS)]
            public NetPoint[] points;

            public static NetMarkerInfo Create()
            {
                return new NetMarkerInfo
                {
                    name = new byte[NET_LEN_64],
                    points = new NetPoint[NET_MAX_MARKER_POINTS]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetUnitInfo
        {
            public NetTempUnit temp_unit;
            public NetDistanceUnit distance_unit;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetMarkerAlarmInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] marker_name;
            public NetThermalTempAlarmLevel alarm_level;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetMarkerTempInfo
        {
            public uint marker_id;
            public NetMarkerType type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] marker_name;
            public NetTempPointInfo temp_point_info;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetDiscoveredDevice
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] ip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] netmask;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] model;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_16)]
            public byte[] serial_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_32)]
            public byte[] id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] firmware_version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] firmware_name;
            public byte wandering;

            public static NetDiscoveredDevice Create()
            {
                return new NetDiscoveredDevice
                {
                    ip = new byte[NET_LEN_64],
                    netmask = new byte[NET_LEN_64],
                    name = new byte[NET_LEN_128],
                    model = new byte[NET_LEN_128],
                    serial_no = new byte[NET_LEN_16],
                    id = new byte[NET_LEN_32],
                    firmware_version = new byte[NET_LEN_128],
                    firmware_name = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetCaptureFile
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] local_file;

            public static NetCaptureFile Create()
            {
                return new NetCaptureFile
                {
                    local_file = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetRawFrame
        {
            public ushort width;
            public ushort height;
            public ushort range_id;
            public uint points_count;
            public IntPtr data;
            public uint lut_len;
            public IntPtr lut_temp_value;
            public IntPtr lut_ad_value;
            public uint lut_env_temp;
            public uint sensor_temp;
            public uint shutter_state;
            public uint frame_no;
            public ulong pts;
            public uint raw_type;
            public uint shutter_adv;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetThermalData
        {
            public NetThermalDataType data_types;
            public ulong timestamp_us;
            public NetRawFrame raw_frame;
            public NetTempPointInfo global_temp_info;
            public ushort markers_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKERS)]
            public NetMarkerTempInfo[] marker_temps;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetThermalDataConfig
        {
            public NetThermalDataType data_types;
            public uint sample_interval;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetDpcParams
        {
            public ushort count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public NetPoint[] points;

            public static NetDpcParams Create()
            {
                return new NetDpcParams
                {
                    points = new NetPoint[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmTrigger
        {
            public byte trigger_ftp_enabled;
            public byte trigger_webhook_enabled;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmSchedulePeriod
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_16)]
            public byte[] start_time;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_16)]
            public byte[] end_time;

            public static NetAlarmSchedulePeriod Create()
            {
                return new NetAlarmSchedulePeriod
                {
                    start_time = new byte[NET_LEN_16],
                    end_time = new byte[NET_LEN_16]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmScheduleDay
        {
            public byte weekday;
            public ushort periods_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_SCHEDULE_PERIODS)]
            public NetAlarmSchedulePeriod[] periods;

            public static NetAlarmScheduleDay Create()
            {
                var day = new NetAlarmScheduleDay
                {
                    periods = new NetAlarmSchedulePeriod[NET_MAX_SCHEDULE_PERIODS]
                };
                for (int i = 0; i < day.periods.Length; i++)
                    day.periods[i] = NetAlarmSchedulePeriod.Create();
                return day;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmSchedule
        {
            public ushort days_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_SCHEDULE_DAYS)]
            public NetAlarmScheduleDay[] days;

            public static NetAlarmSchedule Create()
            {
                var schedule = new NetAlarmSchedule
                {
                    days = new NetAlarmScheduleDay[NET_MAX_SCHEDULE_DAYS]
                };
                for (int i = 0; i < schedule.days.Length; i++)
                    schedule.days[i] = NetAlarmScheduleDay.Create();
                return schedule;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmDeviceInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] ip;
            public ushort port_no;
            public uint channel_id;

            public static NetAlarmDeviceInfo Create()
            {
                return new NetAlarmDeviceInfo
                {
                    ip = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmThermalTempInfo
        {
            public ulong timestamp_us;
            public NetAlarmDeviceInfo dev_info;
            public NetRawFrame raw_frame;
            public NetTempPointInfo global_temp_info;
            public ushort marker_alarms_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKERS)]
            public NetMarkerAlarmInfo[] marker_alarms;
            public ushort markers_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKERS)]
            public NetMarkerTempInfo[] marker_temps;
            public uint message_length;
            public IntPtr message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmIntrusionInfo
        {
            public ulong timestamp_us;
            public NetAlarmDeviceInfo dev_info;
            public int motion_level;
            public uint message_length;
            public IntPtr message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmHighTempInfo
        {
            public ulong timestamp_us;
            public NetAlarmDeviceInfo dev_info;
            public float temperature;
            public float threshold;
            public uint message_length;
            public IntPtr message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmTempDetectInfo
        {
            public ulong timestamp_us;
            public NetAlarmDeviceInfo dev_info;
            public float temperature;
            public float baseline;
            public float slope;
            public uint message_length;
            public IntPtr message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmer
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] serial_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] device_name;
            public ushort link_port;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] device_ip;

            public static NetAlarmer Create()
            {
                return new NetAlarmer
                {
                    serial_no = new byte[NET_LEN_64],
                    device_name = new byte[NET_LEN_128],
                    device_ip = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmInfo
        {
            public NetAlarmType type;
            public NetAlarmer alarmer;
            public uint alarm_info_length;
            public IntPtr alarm_info;
            public uint alarm_id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticSensorInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] firmware_version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] serial_no;
            public uint sample_rate;
            public uint mic_count;
            public uint ref_mic_index;
            public float amplitude;
            public NetAcousticFreqRange freq_range;

            public static NetAcousticSensorInfo Create()
            {
                return new NetAcousticSensorInfo
                {
                    firmware_version = new byte[NET_LEN_128],
                    serial_no = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticFreqRange
        {
            public float min;
            public float max;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticDbScale
        {
            public NetAcousticDbScaleMode mode;
            public float max;
            public float min;
            public byte fixed_span;
            public float span;
            public float sensitivity;
            public uint source_size;
            public float limit;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetOverlayAcousticBlock
        {
            public NetOverlayAlign align;
            public byte enabled;
            public byte max_db_enabled;
            public byte max_freq_enabled;
            public uint x;
            public uint y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticOverlayInfo
        {
            public NetOverlayBlock palette_overlay;
            public NetOverlayBlock spectrum_overlay;
            public NetOverlayAcousticBlock spl_overlay;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticSourceAlarm
        {
            public byte enabled;
            public uint duration;
            public uint tolerance_pixels;
            public uint no_repeat_minutes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticDbAlarm
        {
            public byte enabled;
            public float db_threshold;
            public uint no_repeat_minutes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticFreqAlarm
        {
            public byte enabled;
            public float frequency;
            public float db_threshold;
            public uint no_repeat_minutes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticSplPoint
        {
            public float db;
            public NetPoint point;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticSplInfo
        {
            public NetAcousticSplPoint max;
            public NetAcousticSplPoint min;
            public float avg_db;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticSplFrame
        {
            public ushort width;
            public ushort height;
            public uint points_count;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticSpectrumFrame
        {
            public uint bin_count;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetRawVisualFrame
        {
            public ushort width;
            public ushort height;
            public int type;
            public uint points_count;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticData
        {
            public NetAcousticDataType data_types;
            public ulong timestamp_us;
            public NetRawVisualFrame visual_frame;
            public NetAcousticSplFrame spl_frame;
            public NetAcousticSpectrumFrame spectrum_frame;
            public NetAcousticSplInfo global_spl_info;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAcousticDataConfig
        {
            public NetAcousticDataType data_types;
            public uint sample_interval;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmAcousticSourceInfo
        {
            public ulong timestamp_us;
            public NetAlarmDeviceInfo dev_info;
            public uint duration;
            public uint tolerance_pixels;
            public NetPoint point;
            public uint message_length;
            public IntPtr message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmAcousticDbInfo
        {
            public ulong timestamp_us;
            public NetAlarmDeviceInfo dev_info;
            public float db;
            public float db_threshold;
            public uint message_length;
            public IntPtr message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmAcousticFreqInfo
        {
            public ulong timestamp_us;
            public NetAlarmDeviceInfo dev_info;
            public float frequency;
            public float db;
            public float db_threshold;
            public uint message_length;
            public IntPtr message;
        }

        public enum NetStorageStatus
        {
            Normal = 0,
            Unformatted = 1,
            Error = 2
        }

        public enum NetStorageAttr
        {
            ReadWrite = 0,
            ReadOnly = 1
        }

        public enum NetFormatStatus
        {
            InProgress = 0,
            Finished = 1,
            Error = 2,
            Interrupted = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetStorageInfo
        {
            public uint id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public byte[] name;
            public uint capacity_mb;
            public uint free_mb;
            public NetStorageStatus status;
            public NetStorageAttr attr;
            public byte recycling;

            public static NetStorageInfo Create()
            {
                return new NetStorageInfo
                {
                    name = new byte[NET_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetSearchAlarmParam
        {
            public uint alarm_type;
            public uint channel_id;
            public ulong start_timestamp_us;
            public ulong stop_timestamp_us;
            public uint page_no;
            public uint page_size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetSearchPage
        {
            public uint total;
            public uint page_no;
            public uint count;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetSearchAlarmData
        {
            public uint alarm_id;
            public uint channel_id;
            public ulong timestamp_us;
            public uint alarm_type;
            public uint alarm_info_length;
            public IntPtr alarm_info;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] record_path;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] snapshot_path;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_128)]
            public byte[] data_path;

            public static NetSearchAlarmData Create()
            {
                return new NetSearchAlarmData
                {
                    record_path = new byte[NET_LEN_128],
                    snapshot_path = new byte[NET_LEN_128],
                    data_path = new byte[NET_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetSearchAlarmResult
        {
            public ushort length;
            public IntPtr datas;
            public IntPtr page;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetFormatProgress
        {
            public int disk_number;
            public int progress;
            public NetFormatStatus status;
        }

        #endregion

        #region Callbacks

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void OnNetExceptionCallback(ref NetExceptionInfo exceptionInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void OnNetAlarmCallback(IntPtr alarmInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void OnNetStreamReceivedCallback(ref NetStreamData streamData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void OnNetThermalDataStreamReceivedCallback(IntPtr pThermalData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void OnNetAcousticDataStreamReceivedCallback(IntPtr frame);

        #endregion

        #region SDK Lifecycle

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_sdk_init();

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_sdk_cleanup();

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_enable_log(byte enable);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_login(ref NetLoginInfo info, out IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_logout(ref IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_sdk_version(ref NetSdkVersion version);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_exception_callback(IntPtr handle, OnNetExceptionCallback cb);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_start_alarm_listener(IntPtr handle, OnNetAlarmCallback cb);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_stop_alarm_listener(IntPtr alarm_listener_handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_alarm_trigger(IntPtr handle, ref NetAlarmType type, ref NetAlarmTrigger trigger);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_alarm_trigger(IntPtr handle, ref NetAlarmType type, ref NetAlarmTrigger trigger);

        [DllImport(DllName, CallingConvention = CC, EntryPoint = "net_get_alarm_trigger")]
        public static extern int net_get_alarm_trigger_ptr(IntPtr handle, NetAlarmType type, IntPtr trigger);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_alarm_schedule(IntPtr handle, ref NetAlarmType type, ref NetAlarmSchedule schedule);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_alarm_schedule(IntPtr handle, ref NetAlarmType type, ref NetAlarmSchedule schedule);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_search_devices(
            [In, Out] NetDiscoveredDevice[] devices,
            ushort length,
            out ushort out_length,
            uint timeout_ms);

        #endregion

        #region Device Management

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_device_info(IntPtr handle, ref NetDeviceInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_device_info(IntPtr handle, ref NetDeviceInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_device_restart(IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_device_reset(IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_device_upgrade(IntPtr handle, ref NetUpgradeInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_time(IntPtr handle, ref NetTimeInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_time(IntPtr handle, ref NetTimeInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_ntp_server(IntPtr handle, ref NetNtpInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_ntp_server(IntPtr handle, ref NetNtpInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_networks(
            IntPtr handle,
            [In, Out] NetNetworkInfo[] networks,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_network(IntPtr handle, uint id, ref NetNetworkInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_network(IntPtr handle, uint id, ref NetNetworkInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_ftp(IntPtr handle, ref NetFtpInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_ftp(IntPtr handle, ref NetFtpInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_test_ftp(IntPtr handle, ref NetFtpInfo info);

        /// <summary>
        /// 获取最近一次写入列表的 Webhook 地址。尚未设置时 url 为空。
        /// 该记录与列表保存在一起，默认取最新的一条数据。
        /// </summary>
        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_webhook(IntPtr handle, ref NetWebhookInfo info);

        /// <summary>
        /// 设置单条 Webhook 地址。地址已在列表中则直接成功，不更新最近写入记录。
        /// </summary>
        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_webhook(IntPtr handle, ref NetWebhookInfo info);

        /// <summary>
        /// 获取 Webhook 地址列表，最多 NET_MAX_WEBHOOKS 条。
        /// </summary>
        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_webhooks(
            IntPtr handle,
            [In, Out] NetWebhookInfo[] list,
            ushort length,
            out ushort out_length);

        /// <summary>
        /// 设置 Webhook 地址列表。length 为 0 时清空列表，并清空最近写入记录。
        /// </summary>
        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_webhooks(
            IntPtr handle,
            [In] NetWebhookInfo[] list,
            ushort length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_port(IntPtr handle, ref NetPortInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_port(IntPtr handle, ref NetPortInfo info);

        #endregion

        #region User Management

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_users(
            IntPtr handle,
            [In, Out] NetUserInfo[] users,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_user(IntPtr handle, uint id, ref NetUserInfo user);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_update_user(IntPtr handle, ref NetUserPwdInfo user);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_delete_user(IntPtr handle, uint id);

        #endregion

        #region Streaming

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_streams(
            IntPtr handle,
            [In, Out] NetStreamInfo[] streams,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_stream(IntPtr handle, uint stream_id, ref NetStreamInfo stream);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_stream_status(IntPtr handle, uint stream_id, ref NetStreamStatus status);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_start_stream_play(
            IntPtr handle,
            ref NetStreamInfo stream,
            NetStreamType stream_type,
            OnNetStreamReceivedCallback cb);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_stop_stream_play(IntPtr stream_handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_stream_callback(IntPtr stream_handle, OnNetStreamReceivedCallback cb);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_start_stream_record(IntPtr stream_handle, byte[] local_file, ushort length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_stop_stream_record(IntPtr stream_handle);

        #endregion

        #region Display

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_channels(
            IntPtr handle,
            [In, Out] NetChannelInfo[] channel,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_image_params(IntPtr handle, uint channel_id, ref NetImageParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_image_params(IntPtr handle, uint channel_id, ref NetImageParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_image_flip(IntPtr handle, uint channel_id, out NetImageFlipMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_image_flip(IntPtr handle, uint channel_id, ref NetImageFlipMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_channel(IntPtr handle, uint channel_id, ref NetChannelInfo channel);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_channel(IntPtr handle, uint channel_id, ref NetChannelInfo channel);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_overlay(IntPtr handle, uint channel_id, ref NetOverlayInfo overlay);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_overlay(IntPtr handle, uint channel_id, ref NetOverlayInfo overlay);

        #endregion

        #region Thermal

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_lens_info(IntPtr handle, ref NetLensInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_thermal_calibrate(IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_shutter_mode(IntPtr handle, out NetShutterMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_shutter_mode(IntPtr handle, ref NetShutterMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_shutter_time(IntPtr handle, out ushort minutes);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_shutter_time(IntPtr handle, ref ushort minutes);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_resolution(IntPtr handle, ref NetSize size);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_range(IntPtr handle, out ushort range_id);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_range(IntPtr handle, ref ushort range_id);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_lut(IntPtr handle, ref NetLutTable lut);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_params(IntPtr handle, ref NetThermalParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_params(IntPtr handle, ref NetThermalParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_preset_palettes(
            IntPtr handle,
            [In, Out] NetPaletteName[] palettes,
            ushort length,
            out ushort out_length);

        public static int net_get_thermal_palettes(
            IntPtr handle,
            NetPaletteName[] palettes,
            ushort length,
            out ushort out_length)
        {
            return net_get_thermal_preset_palettes(handle, palettes, length, out out_length);
        }

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_palette(IntPtr handle, ref NetPalette palette);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_palette(IntPtr handle, ref NetPalette palette);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_overlay(IntPtr handle, ref NetThermalOverlayInfo overlay);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_overlay(IntPtr handle, ref NetThermalOverlayInfo overlay);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_unit(IntPtr handle, ref NetUnitInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_unit(IntPtr handle, ref NetUnitInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_capture(IntPtr handle, NetThermalCaptureMode mode, ref NetCaptureFile capture_file);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_markers(
            IntPtr handle,
            [In, Out] NetMarkerInfo[] markers,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_markers(
            IntPtr handle,
            [In] NetMarkerInfo[] markers,
            ushort length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_temp_alarms(
            IntPtr handle,
            [In, Out] NetThermalTempAlarm[] alarms,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_temp_alarms(
            IntPtr handle,
            [In] NetThermalTempAlarm[] alarms,
            ushort length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_temp_alarm_interval(IntPtr handle, ref NetThermalTempAlarmInterval interval);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_temp_alarm_interval(IntPtr handle, ref NetThermalTempAlarmInterval interval);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_intrusion_alarm(IntPtr handle, ref NetIntrusionAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_intrusion_alarm(IntPtr handle, ref NetIntrusionAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_high_temp_alarm(IntPtr handle, ref NetHighTempAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_high_temp_alarm(IntPtr handle, ref NetHighTempAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_temp_detect_alarm(IntPtr handle, ref NetTempDetectAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_temp_detect_alarm(IntPtr handle, ref NetTempDetectAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_dpc(IntPtr handle, ref NetDpcParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_color_dist(IntPtr handle, out NetColorDistType type);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_color_dist(IntPtr handle, ref NetColorDistType type);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_gain(IntPtr handle, ref NetGain gain);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_thermal_gain(IntPtr handle, ref NetGain gain);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_start_thermal_data_stream(
            IntPtr handle,
            ref NetThermalDataConfig config,
            OnNetThermalDataStreamReceivedCallback cb);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_stop_thermal_data_stream(IntPtr thermal_data_handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_thermal_data(IntPtr handle, uint data_types, ref NetThermalData realdata);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_sensor_info(IntPtr handle, ref NetAcousticSensorInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_freq_range(IntPtr handle, ref NetAcousticFreqRange range);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_acoustic_freq_range(IntPtr handle, ref NetAcousticFreqRange range);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_db_scale(IntPtr handle, ref NetAcousticDbScale scale);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_acoustic_db_scale(IntPtr handle, ref NetAcousticDbScale scale);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_overlay(IntPtr handle, ref NetAcousticOverlayInfo overlay);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_acoustic_overlay(IntPtr handle, ref NetAcousticOverlayInfo overlay);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_capture(IntPtr handle, NetAcousticCaptureMode mode, ref NetCaptureFile captureFile);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_source_alarm(IntPtr handle, ref NetAcousticSourceAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_acoustic_source_alarm(IntPtr handle, ref NetAcousticSourceAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_db_alarm(IntPtr handle, ref NetAcousticDbAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_acoustic_db_alarm(IntPtr handle, ref NetAcousticDbAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_freq_alarm(IntPtr handle, ref NetAcousticFreqAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_acoustic_freq_alarm(IntPtr handle, ref NetAcousticFreqAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_acoustic_data(IntPtr handle, uint dataTypes, ref NetAcousticData data);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_start_acoustic_data_stream(
            IntPtr handle,
            ref NetAcousticDataConfig config,
            OnNetAcousticDataStreamReceivedCallback cb);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_stop_acoustic_data_stream(IntPtr acousticDataHandle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_storages(
            IntPtr handle,
            [In, Out] NetStorageInfo[] storages,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_set_storage(IntPtr handle, uint id, ref NetStorageInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_find_alarm_datas(IntPtr handle, ref NetSearchAlarmParam param, ref NetSearchAlarmResult result);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_format_disk(IntPtr handle, uint disk_number, out IntPtr format_handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_get_format_progress(IntPtr format_handle, ref NetFormatProgress progress);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int net_close_format_handle(IntPtr format_handle);

        #endregion

        #region Helpers

        public static bool IsOk(int errorCode)
        {
            return errorCode == (int)NetErrorCode.OK;
        }

        public static bool IsSuccess(int ret)
        {
            return IsOk(ret) || ret > 0;
        }

        public static string FormatReturnStatus(int ret)
        {
            if (IsOk(ret))
                return "OK";

            if (ret > 0)
                return "OK handle=" + ret;

            return ((NetErrorCode)ret).ToString();
        }

        public static bool TryGetSdkHandle(int ret, out IntPtr handle)
        {
            if (ret > 0)
            {
                handle = new IntPtr(ret);
                return true;
            }

            handle = IntPtr.Zero;
            return false;
        }

        public static bool TryReadAlarmInfo(IntPtr alarmInfoPtr, out NetAlarmInfo alarmInfo)
        {
            alarmInfo = default(NetAlarmInfo);
            if (alarmInfoPtr == IntPtr.Zero)
                return false;

            alarmInfo = (NetAlarmInfo)Marshal.PtrToStructure(alarmInfoPtr, typeof(NetAlarmInfo));
            if (alarmInfo.alarmer.serial_no == null)
                alarmInfo.alarmer = NetAlarmer.Create();
            return true;
        }

        public static NetAlarmThermalTempInfo ParseAlarmThermalTempInfo(IntPtr thermalAlarmPtr)
        {
            if (thermalAlarmPtr == IntPtr.Zero)
                return default(NetAlarmThermalTempInfo);

            var info = (NetAlarmThermalTempInfo)Marshal.PtrToStructure(thermalAlarmPtr, typeof(NetAlarmThermalTempInfo));
            if (info.dev_info.ip == null)
                info.dev_info.ip = new byte[NET_LEN_64];
            if (info.marker_alarms == null)
                info.marker_alarms = new NetMarkerAlarmInfo[NET_MAX_MARKERS];
            if (info.marker_temps == null)
                info.marker_temps = new NetMarkerTempInfo[NET_MAX_MARKERS];
            return info;
        }

        public static string BytesToString(byte[] bytes)
        {
            if (bytes == null) return string.Empty;
            int length = Array.IndexOf(bytes, (byte)0);
            if (length < 0) length = bytes.Length;
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        public static string ReadUtf8(IntPtr ptr, uint length)
        {
            if (ptr == IntPtr.Zero || length == 0)
                return string.Empty;

            int len = (int)Math.Min(length, 4096u);
            byte[] bytes = new byte[len];
            Marshal.Copy(ptr, bytes, 0, len);
            int zero = Array.IndexOf(bytes, (byte)0);
            if (zero >= 0)
                len = zero;
            return Encoding.UTF8.GetString(bytes, 0, len);
        }

        public static byte[] StringToFixedBytes(string value, int length)
        {
            var bytes = new byte[length];
            if (string.IsNullOrEmpty(value)) return bytes;
            var source = Encoding.UTF8.GetBytes(value);
            Buffer.BlockCopy(source, 0, bytes, 0, Math.Min(source.Length, length - 1));
            return bytes;
        }

        #endregion
    }
}
