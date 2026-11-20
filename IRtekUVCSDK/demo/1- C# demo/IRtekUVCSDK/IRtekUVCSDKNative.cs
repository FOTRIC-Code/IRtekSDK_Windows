using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IRtekUVCSDK
{
    public static class UvcSdkNative
    {
        private const string DllName = "IRtekUVCSDK.dll";
        private const CallingConvention CC = CallingConvention.Cdecl;

        #region Constants

        public const int UVC_LEN_4 = 4;
        public const int UVC_LEN_16 = 16;
        public const int UVC_LEN_32 = 32;
        public const int UVC_LEN_64 = 64;
        public const int UVC_LEN_80 = 80;
        public const int UVC_LEN_128 = 128;
        public const int UVC_LEN_256 = 256;

        public const int UVC_MAX_RANGES = 4;
        public const int UVC_MAX_DPC_POINTS = 64;
        public const int UVC_MAX_LUT_POINTS = 64;
        public const int UVC_ACOUSTIC_EXTREME_POINT_COUNT = 3;

        #endregion

        #region Enums

        public enum UvcErrorCode
        {
            OK = 0,
            Fail = -1,
            Memory = -2,
            NullPointer = -3,
            InvalidParam = -4,
            NotSupported = -5,
            NotInitialized = -6
        }

        public enum UvcShutterMode : byte
        {
            Automatic = 0,
            Manual = 1
        }

        public enum UvcImageFlipMode : byte
        {
            None = 0,
            UpDown = 1,
            LeftRight = 2,
            Center = 3
        }

        public enum UvcSrMode : byte
        {
            None = 0,
            SR = 1
        }

        public enum UvcUpgradeState : byte
        {
            Idle = 0,
            Receiving = 1,
            Staged = 2,
            Flashing = 3,
            Done = 4,
            Failed = 5
        }

        public enum UvcAcousticDbScaleMode : byte
        {
            AutoMono = 1,
            AutoMulti = 2,
            Fixed = 3,
            AutoHolo = 4
        }

        public enum UvcThermalGainMode : byte
        {
            Fixed = 1,
            Auto = 2,
            Touch = 3
        }

        public enum UvcColorDistType : byte
        {
            TTwb = 1,
            TempLinear = 2,
            AdLinear = 3
        }

        public enum UvcColorSpaceType : byte
        {
            Bgr = 1,
            Rgba = 2
        }

        public enum UvcAlarmType : byte
        {
            Normal = 0,
            Below = 1,
            Above = 2,
            Between = 3,
            MixPalette = 4,
            Outside = 5
        }

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcSdkVersion
        {
            public ushort major;
            public ushort minor;
            public ushort patch;
            public ushort build;

            public override string ToString() => $"{major}.{minor}.{patch}.{build}";
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcDevice
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_64)]
            public byte[] path;
            public ushort vid;
            public ushort pid;

            public static UvcDevice Create()
            {
                return new UvcDevice
                {
                    path = new byte[UVC_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcFrameInfo
        {
            public ushort interface_num;
            public ushort alt_setting;
            public ushort frame_index;
            public ushort format_index;
            public ushort width;
            public ushort height;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_128)]
            public byte[] format;

            public static UvcFrameInfo Create()
            {
                return new UvcFrameInfo
                {
                    format = new byte[UVC_LEN_128]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcFrameData
        {
            public IntPtr data;
            public uint length;
            public ushort width;
            public ushort height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcDeviceInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_128)]
            public byte[] model;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_16)]
            public byte[] serial_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_32)]
            public byte[] id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_32)]
            public byte[] brand;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_64)]
            public byte[] company;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_128)]
            public byte[] firmware_name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_16)]
            public byte[] firmware_version;

            public static UvcDeviceInfo Create()
            {
                return new UvcDeviceInfo
                {
                    model = new byte[UVC_LEN_128],
                    serial_no = new byte[UVC_LEN_16],
                    id = new byte[UVC_LEN_32],
                    brand = new byte[UVC_LEN_32],
                    company = new byte[UVC_LEN_64],
                    firmware_name = new byte[UVC_LEN_128],
                    firmware_version = new byte[UVC_LEN_16]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcTempRange
        {
            public float min;
            public float max;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcTempRangeMap
        {
            public ushort id;
            public UvcTempRange range;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcLensInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_64)]
            public byte[] name;
            public float focal;
            public float vfov;
            public float hfov;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_32)]
            public byte[] band;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_MAX_RANGES)]
            public UvcTempRangeMap[] range_map;
            public ushort ranges_count;

            public static UvcLensInfo Create()
            {
                return new UvcLensInfo
                {
                    name = new byte[UVC_LEN_64],
                    band = new byte[UVC_LEN_32],
                    range_map = new UvcTempRangeMap[UVC_MAX_RANGES]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcSize
        {
            public ushort width;
            public ushort height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcLutTable
        {
            public ushort range_id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_MAX_LUT_POINTS)]
            public ushort[] ad;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_MAX_LUT_POINTS)]
            public float[] t;
            public ushort lut_count;

            public static UvcLutTable Create()
            {
                return new UvcLutTable
                {
                    ad = new ushort[UVC_MAX_LUT_POINTS],
                    t = new float[UVC_MAX_LUT_POINTS]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcThermalParams
        {
            public float emissivity;
            public float distance;
            public float refl_temp;
            public float atm_temp;
            public float rel_humidity;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcDpcPoint
        {
            public ushort y;
            public ushort x;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcDpcParams
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_MAX_DPC_POINTS)]
            public UvcDpcPoint[] points;
            public ushort count;

            public static UvcDpcParams Create()
            {
                return new UvcDpcParams
                {
                    points = new UvcDpcPoint[UVC_MAX_DPC_POINTS]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcImageParams
        {
            public ushort contrast;
            public ushort brightness;
            public byte dde_enable;
            public ushort dde_value;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcPalette
        {
            public byte ref_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_256)]
            public uint[] colors;
            public byte inverted;
            public byte alpha;

            public static UvcPalette Create()
            {
                return new UvcPalette { colors = new uint[UVC_LEN_256] };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcThermalGain
        {
            [MarshalAs(UnmanagedType.U1)]
            public UvcThermalGainMode mode;
            public ushort max;
            public ushort min;
            public byte fixed_span;
            public ushort span;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcThermalImageInfo
        {
            public uint size;
            public UvcThermalGain gain;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcAcousticPoint
        {
            public int x;
            public int y;
            public float value;
        }

        // Layout matches uvc_acoustic_summary: 5 points + count = 64 bytes.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcAcousticSummary
        {
            public UvcAcousticPoint max;
            public UvcAcousticPoint min;
            public UvcAcousticPoint extreme_point0;
            public UvcAcousticPoint extreme_point1;
            public UvcAcousticPoint extreme_point2;
            public int extreme_points_count;

            public UvcAcousticPoint ExtremePoint(int index)
            {
                switch (index)
                {
                    case 0: return extreme_point0;
                    case 1: return extreme_point1;
                    default: return extreme_point2;
                }
            }
        }

        // Layout matches uvc_acoustic_image_info: size + db_scale + summary = 95 bytes.
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcAcousticImageInfo
        {
            public uint size;
            public UvcAcousticDbScale db_scale;
            public UvcAcousticSummary summary;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcAlarm
        {
            [MarshalAs(UnmanagedType.U1)]
            public UvcAlarmType type;
            public byte custom;
            public uint color;
            public ushort max;
            public ushort min;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcPresetPalette
        {
            public byte ref_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_32)]
            public byte[] name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcPresetPalettes
        {
            public uint count;
            public IntPtr palettes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcAcousticSensorInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_128)]
            public byte[] firmware_version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = UVC_LEN_64)]
            public byte[] serial_no;
            public uint sample_rate;
            public ushort mic_count;
            public UvcAcousticFreqRange freq_range;

            public static UvcAcousticSensorInfo Create()
            {
                return new UvcAcousticSensorInfo
                {
                    firmware_version = new byte[UVC_LEN_128],
                    serial_no = new byte[UVC_LEN_64]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcAcousticFreqRange
        {
            public float min;
            public float max;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct UvcAcousticDbScale
        {
            [MarshalAs(UnmanagedType.U1)]
            public UvcAcousticDbScaleMode mode;
            public float max;
            public float min;
            public byte fixed_span;
            public float span;
            public float sensitivity;
            public int source_size;
            public float limit;
            public byte focus_enabled;
        }

        #endregion

        #region Callbacks

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void OnUvcFoundCallback(ref UvcDevice device, [MarshalAs(UnmanagedType.I1)] bool found);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void OnFrameReceivedCallback(ref UvcFrameData frame);

        #endregion

        #region SDK Lifecycle

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_sdk_version(ref UvcSdkVersion version);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_sdk_init();

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_enable_log([MarshalAs(UnmanagedType.I1)] bool enable);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_sdk_cleanup();

        #endregion

        #region Device Discovery

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_uvc_list(
            [In, Out] UvcDevice[] device_list,
            ushort length,
            out ushort out_length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_hotplug_callback(OnUvcFoundCallback cb);

        #endregion

        #region Login / Logout

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_login(ushort vid, ushort pid, out IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_login_by_path(byte[] path, ushort length, out IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_logout(ref IntPtr handle);

        #endregion

        #region Streaming

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_frame_list(
            IntPtr handle,
            [In, Out] UvcFrameInfo[] frames,
            ushort length,
            out ushort out_length);

        // Acoustic devices: pass frame = null to pull the FF/51 composite stream.
        // The callback returns the raw composite bytes (data/length) and the visible
        // resolution (width/height). Synthesize an image with uvc_get_acoustic_image_from_frame.
        // cb = null stops the current stream.
        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_stream_callback(IntPtr handle, ref UvcFrameInfo frame, OnFrameReceivedCallback cb);

        [DllImport(DllName, EntryPoint = "uvc_set_stream_callback", CallingConvention = CC)]
        public static extern int uvc_set_stream_callback(IntPtr handle, IntPtr frame, OnFrameReceivedCallback cb);

        #endregion

        #region Device Management

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_device_info(IntPtr handle, ref UvcDeviceInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_lens_info(IntPtr handle, ref UvcLensInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_device_restart(IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_device_reset(IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_device_upgrade(IntPtr handle, byte[] path, ushort length);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_upgrade_state(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] out UvcUpgradeState state);

        #endregion

        #region Thermal

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_thermal_calibrate(IntPtr handle);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_shutter_mode(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] out UvcShutterMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_shutter_mode(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] ref UvcShutterMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_shutter_time(IntPtr handle, out ushort minutes);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_shutter_time(IntPtr handle, ref ushort minutes);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_resolution(IntPtr handle, ref UvcSize size);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_range(IntPtr handle, out ushort range_id);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_range(IntPtr handle, ref ushort range_id);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_lut(IntPtr handle, ref UvcLutTable lut);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_params(IntPtr handle, ref UvcThermalParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_params(IntPtr handle, ref UvcThermalParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_dpc(IntPtr handle, ref UvcDpcParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_sr(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] out UvcSrMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_sr(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] ref UvcSrMode mode);

        #endregion

        #region Display

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_image_params(IntPtr handle, ref UvcImageParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_image_params(IntPtr handle, ref UvcImageParams param);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_image_flip(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] out UvcImageFlipMode mode);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_image_flip(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] ref UvcImageFlipMode mode);

        #endregion

        #region Thermal Imaging

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_preset_palettes(IntPtr handle, ref UvcPresetPalettes palettes);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_palette(IntPtr handle, ref UvcPalette palette);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_palette(IntPtr handle, ref UvcPalette palette);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_gain(IntPtr handle, ref UvcThermalGain gain);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_gain(IntPtr handle, ref UvcThermalGain gain);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_color_dist(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] out UvcColorDistType type);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_color_dist(
            IntPtr handle,
            [MarshalAs(UnmanagedType.U1)] ref UvcColorDistType type);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_alarm(IntPtr handle, ref UvcAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_thermal_alarm(IntPtr handle, ref UvcAlarm alarm);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_thermal_image_from_frame(
            IntPtr handle,
            ref UvcFrameData frame,
            [MarshalAs(UnmanagedType.U1)] ref UvcColorSpaceType type,
            ref UvcFrameData image,
            ref UvcThermalImageInfo info);

        #endregion

        #region Acoustic

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_acoustic_sensor_info(IntPtr handle, ref UvcAcousticSensorInfo info);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_acoustic_freq_range(IntPtr handle, ref UvcAcousticFreqRange range);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_acoustic_freq_range(IntPtr handle, ref UvcAcousticFreqRange range);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_acoustic_db_scale(IntPtr handle, ref UvcAcousticDbScale scale);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_acoustic_db_scale(IntPtr handle, ref UvcAcousticDbScale scale);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_acoustic_resolution(IntPtr handle, ref UvcSize size);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_acoustic_preset_palettes(IntPtr handle, ref UvcPresetPalettes palettes);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_acoustic_palette(IntPtr handle, ref UvcPalette palette);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_set_acoustic_palette(IntPtr handle, ref UvcPalette palette);

        [DllImport(DllName, CallingConvention = CC)]
        public static extern int uvc_get_acoustic_image_from_frame(
            IntPtr handle,
            ref UvcFrameData frameData,
            [MarshalAs(UnmanagedType.U1)] ref UvcColorSpaceType type,
            ref UvcFrameData image,
            ref UvcAcousticImageInfo info);

        #endregion

        #region Helpers

        public struct NamedPreset
        {
            public byte RefNo;
            public string Name;

            public override string ToString()
            {
                return Name ?? string.Empty;
            }
        }

        public static bool TryGetPresetPalettes(IntPtr handle, bool acoustic, out NamedPreset[] presets)
        {
            presets = null;
            var probe = new UvcPresetPalettes();
            int ret = acoustic
                ? uvc_get_acoustic_preset_palettes(handle, ref probe)
                : uvc_get_thermal_preset_palettes(handle, ref probe);
            if (!IsOk(ret) || probe.count == 0 || probe.count > 256)
                return false;

            int count = (int)probe.count;
            int stride = Marshal.SizeOf(typeof(UvcPresetPalette));
            IntPtr buffer = Marshal.AllocHGlobal(stride * count);
            try
            {
                for (int i = 0; i < stride * count; i++)
                    Marshal.WriteByte(buffer, i, 0);
                var request = new UvcPresetPalettes
                {
                    count = (uint)count,
                    palettes = buffer
                };
                ret = acoustic
                    ? uvc_get_acoustic_preset_palettes(handle, ref request)
                    : uvc_get_thermal_preset_palettes(handle, ref request);
                if (!IsOk(ret))
                    return false;

                int filled = (int)Math.Min(request.count, (uint)count);
                presets = new NamedPreset[filled];
                for (int i = 0; i < filled; i++)
                {
                    var item = (UvcPresetPalette)Marshal.PtrToStructure(
                        IntPtr.Add(buffer, i * stride), typeof(UvcPresetPalette));
                    presets[i] = new NamedPreset
                    {
                        RefNo = item.ref_no,
                        Name = BytesToString(item.name)
                    };
                }
                return presets.Length > 0;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static bool IsOk(int errorCode)
        {
            return errorCode == (int)UvcErrorCode.OK;
        }

        public static string BytesToString(byte[] bytes)
        {
            if (bytes == null) return string.Empty;
            int length = Array.IndexOf(bytes, (byte)0);
            if (length < 0) length = bytes.Length;
            if (length == 0) return string.Empty;
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        public static string BytesToPathString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return string.Empty;

            // Detect UTF-16LE: ASCII chars have 0x00 as the high byte
            if (bytes.Length >= 4 && bytes[0] != 0 && bytes[1] == 0 && bytes[2] != 0 && bytes[3] == 0)
            {
                string result = Encoding.Unicode.GetString(bytes);
                int idx = result.IndexOf('\0');
                return idx >= 0 ? result.Substring(0, idx) : result;
            }

            int length = Array.IndexOf(bytes, (byte)0);
            if (length < 0) length = bytes.Length;
            if (length == 0) return string.Empty;

            // Filter non-printable characters for safety
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                char c = (char)bytes[i];
                sb.Append(c >= 0x20 && c < 0x7F ? c : '?');
            }
            return sb.ToString();
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
