using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IRtekHLAPI
{
    public static partial class NetSdkNative
    {

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
        public const int NET_MAX_ACTIVE_ALARMS = 16;

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
            VideoLost = 3
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
            ThermalTemp = 1
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
            Rectangle = 3,
            Ellipse = 4,
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_RANGES)]
            public NetTempRangeMap[] range_map;
            public ushort ranges_count;

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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_DNS * NET_LEN_64)]
            public byte[] dns;
            public ushort dns_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_GATEWAYS * NET_LEN_64)]
            public byte[] gateway;
            public ushort gateway_count;

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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_PALETTE_COLORS)]
            public uint[] colors;
            public ushort color_count;
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
            public IntPtr data;
            public uint data_length;
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKER_POINTS)]
            public NetPoint[] points;
            public ushort points_count;

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
            public IntPtr data;
            public uint points_count;
            public uint lut_len;
            public uint lut_env_temp;
            public uint sensor_temp;
            public uint shutter_state;
            public uint frame_no;
            public ulong pts;
            public uint raw_type;
            public IntPtr lut_temp_value;
            public IntPtr lut_ad_value;
            public uint shutter_adv;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetThermalData
        {
            public NetThermalDataType data_types;
            public ulong timestamp_us;
            public NetRawFrame raw_frame;
            public NetTempPointInfo global_temp_info;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKERS)]
            public NetMarkerTempInfo[] marker_temps;
            public ushort markers_count;
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_LEN_64)]
            public NetPoint[] points;
            public ushort count;

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

        public delegate void OnNetAlarmCallback(IntPtr alarmInfo);

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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKERS)]
            public NetMarkerAlarmInfo[] marker_alarms;
            public ushort marker_alarms_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NET_MAX_MARKERS)]
            public NetMarkerTempInfo[] marker_temps;
            public ushort markers_count;
            public uint message_length;
            public IntPtr message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NetAlarmInfo
        {
            public NetAlarmType type;
            public uint alarm_info_length;
            public IntPtr alarm_info;
        }

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
            return true;
        }

        public static NetAlarmThermalTempInfo ParseAlarmThermalTempInfo(IntPtr alarmInfoPtr)
        {
            if (alarmInfoPtr == IntPtr.Zero)
                return default(NetAlarmThermalTempInfo);

            var info = (NetAlarmThermalTempInfo)Marshal.PtrToStructure(alarmInfoPtr, typeof(NetAlarmThermalTempInfo));
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
