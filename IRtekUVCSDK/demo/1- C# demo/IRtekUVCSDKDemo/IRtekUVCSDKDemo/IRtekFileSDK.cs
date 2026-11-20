using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IRtekUVCSDKDemo
{
    public static class IRtekFileSDK
    {
        private const string DllName = "IRtekFileSDK.dll";

        #region Constants

        public const int IR_FILE_EC_OK = 0;
        public const int IR_FILE_EC_FAIL = -1;

        public const int IR_FILE_THERMAL_PICTURE = 1;
        public const int IR_FILE_BGR = 1;
        public const int IR_FILE_RGBA = 2;

        public const int IR_FILE_ISP_NEMO = 0;
        public const int IR_FILE_ISP_HOLLY = 1;

        public const int IR_FILE_GAIN_FIXED = 1;
        public const int IR_FILE_GAIN_AUTO = 2;
        public const int IR_FILE_GAIN_TOUCH = 3;

        public const int IR_FILE_T_TWB = 1;
        public const int IR_FILE_TEMP_LINEAR = 2;
        public const int IR_FILE_AD_LINEAR = 3;

        public const int IR_FILE_SPOT = 1;
        public const int IR_FILE_RECTANGLE = 3;
        public const int IR_FILE_POLYLINE = 6;

        public const int NAME_LEN = 32;
        public const int BAND_LEN = 32;

        public const int IR_FILE_TEMP_MAX = 0;
        public const int IR_FILE_TEMP_MIN = 1;
        public const int IR_FILE_TEMP_AVG = 2;

        public const int IR_FILE_NORMAL = 0;

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFilePoint
        {
            public short x;
            public short y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct IrFileMarker
        {
            public int type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] name;
            public byte localParams;
            public float reflTemp;
            public float distance;
            public float emissivity;
            public uint pointCount;
            public IntPtr points;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] tempValues;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public IrFilePoint[] tempSpots;
            public float measuredValue;
            public IrFileBuffer exInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileSize
        {
            public short width;
            public short height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileTempRange
        {
            public float max;
            public float min;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileThermalParams
        {
            public float reflTemp;
            public float atmTemp;
            public float distance;
            public float relHumidity;
            public float emissivity;
            public float extOptTemp;
            public float extOptTrans;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileFrame
        {
            public ulong timeStamp;
            public uint length;
            public IntPtr buffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileImage
        {
            public uint offset;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileGain
        {
            public int mode;
            public float max;
            public float min;
            public byte fixedSpan;
            public float span;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileLut
        {
            public byte radMethod;
            public uint count;
            public IntPtr adArray;
            public IntPtr tempArray;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileBuffer
        {
            public uint length;
            public IntPtr buffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileCameraInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] model;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
            public byte[] serialNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] brand;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] company;
            public uint logoLength;
            public IntPtr logo;

            public static IrFileCameraInfo Create()
            {
                return new IrFileCameraInfo
                {
                    model = new byte[32],
                    serialNo = new byte[48],
                    brand = new byte[32],
                    company = new byte[64],
                    logo = IntPtr.Zero
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFilePalette
        {
            public byte refNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] colors;
            public byte inverted;

            public static IrFilePalette Create()
            {
                return new IrFilePalette
                {
                    colors = new uint[256]
                };
            }
        }

        #endregion

        #region Lens Info

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 8)]
        public struct IrFileLensInfoV2
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] name;
            public float focalLength;
            public float hfov;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BAND_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] band;

            public static IrFileLensInfoV2 Create()
            {
                return new IrFileLensInfoV2
                {
                    name = new byte[NAME_LEN],
                    band = new byte[BAND_LEN]
                };
            }
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ir_file_set_lens_info_v2(IntPtr h, ref IrFileLensInfoV2 value);

        #endregion

        #region SDK version

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_get_sdk_version(out uint ver, ref IrFileBuffer str);

        public static bool TryGetSdkVersion(out uint versionNumber, out string versionString)
        {
            versionNumber = 0;
            versionString = null;
            byte[] buf = new byte[512];
            GCHandle pin = GCHandle.Alloc(buf, GCHandleType.Pinned);
            try
            {
                var irBuf = new IrFileBuffer { length = (uint)buf.Length, buffer = pin.AddrOfPinnedObject() };
                int ret = ir_file_get_sdk_version(out versionNumber, ref irBuf);
                if (ret != IR_FILE_EC_OK)
                    return false;
                int n = 0;
                while (n < buf.Length && buf[n] != 0)
                    n++;
                versionString = n > 0 ? Encoding.UTF8.GetString(buf, 0, n) : string.Empty;
                return true;
            }
            finally
            {
                pin.Free();
            }
        }

        #endregion

        #region Core APIs

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ir_file_create(string file_name, int file_type, ref IntPtr h);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_close(IntPtr h);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_thermal_size(IntPtr h, ref IrFileSize value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_temp_range(IntPtr h, ref IrFileTempRange value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_thermal_params(IntPtr h, ref IrFileThermalParams value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_get_thermal_image_from_frame(IntPtr h, ref IrFileFrame frame, int colorSpaceType, ref IrFileImage value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_save(IntPtr h);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_append_frame(IntPtr h, ref IrFileFrame value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_camera_info(IntPtr h, ref IrFileCameraInfo value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_cover_image(IntPtr h, ref IrFileBuffer value);

        #endregion

        #region HOLLY Mode APIs

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_isp_type(IntPtr h, int value);

        #endregion

        #region Gain and Color distribution

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_gain(IntPtr h, ref IrFileGain value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_color_dist(IntPtr h, int value);

        #endregion

        #region Preset Palettes

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IrFilePresetPalette
        {
            public byte refNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFilePresetPalettes
        {
            public uint count;
            public IntPtr palettes;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_get_preset_palettes(ref IrFilePresetPalettes value);

        #endregion

        #region Alarm

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileAlarm
        {
            public int type;
            public byte custom;
            public uint color;
            public float max;
            public float min;
            public float rhLimit;
            public float indoorTemp;
            public float outdoorTemp;
            public float heatIndex;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct IrFileAlarmV2
        {
            public int type;
            public byte custom;
            public uint color;
            public float max;
            public float min;
            public float rhLimit;
            public float indoorTemp;
            public float outdoorTemp;
            public float heatIndex;
            public float threshold;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_alarm(IntPtr h, ref IrFileAlarm value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_get_alarm_v2(IntPtr h, ref IrFileAlarmV2 value);

        #endregion

        #region LUT and Palette APIs

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_lut(IntPtr h, ref IrFileLut value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_palette(IntPtr h, ref IrFilePalette value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_get_palette_image(IntPtr h, ref IrFileImage value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_get_preset_palette_image(byte refNo, ref IrFileImage value);

        #endregion

        #region Marker APIs

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_set_marker(IntPtr h, ref IrFileMarker value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_get_marker(IntPtr h, ref IrFileMarker value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ir_file_remove_marker(IntPtr h, byte[] name);

        public static byte[] StringToMarkerName(string s)
        {
            byte[] result = new byte[NAME_LEN];
            if (!string.IsNullOrEmpty(s))
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(s);
                int len = Math.Min(utf8.Length, NAME_LEN - 1);
                Array.Copy(utf8, result, len);
            }
            return result;
        }

        #endregion
    }
}
