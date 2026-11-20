using System;
using System.Runtime.InteropServices;

namespace IRtekUVCSDKDemo
{
    public static class IRtekACFileSDK
    {
        private const string DllName = "IRtekACFileSDK.dll";

        public const int AC_FILE_EC_OK = 0;
        public const int AC_FILE_PICTURE = 1;
        public const byte AC_FILE_CAPTURE_MODE_LEAK = 1;

        public const int MODEL_LEN = 32;
        public const int SERIAL_NO_LEN = 48;
        public const int BRAND_LEN = 32;
        public const int COMPANY_LEN = 64;
        public const int VERSION_LEN = 48;
        public const int COLORS_LEN = 256;

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileBuffer
        {
            public uint length;
            public IntPtr buffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileCameraInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MODEL_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] model;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SERIAL_NO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] serialNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BRAND_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] brand;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = COMPANY_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] company;
            public uint logoLength;
            public IntPtr logo;

            public static AcFileCameraInfo Create()
            {
                return new AcFileCameraInfo
                {
                    model = new byte[MODEL_LEN],
                    serialNo = new byte[SERIAL_NO_LEN],
                    brand = new byte[BRAND_LEN],
                    company = new byte[COMPANY_LEN]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileSensorInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = SERIAL_NO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] serialNo;
            public int channelCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = VERSION_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] version;
            public int refChannel;
            public int sampleRate;
            public float amplitude;

            public static AcFileSensorInfo Create()
            {
                return new AcFileSensorInfo
                {
                    serialNo = new byte[SERIAL_NO_LEN],
                    version = new byte[VERSION_LEN]
                };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileBand
        {
            public float max;
            public float min;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFilePalette
        {
            public byte refNo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = COLORS_LEN)]
            public uint[] colors;
            public byte alpha;

            public static AcFilePalette Create()
            {
                return new AcFilePalette { colors = new uint[COLORS_LEN] };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileSize
        {
            public short width;
            public short height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileDbScale
        {
            public int mode;
            public float max;
            public float min;
            public byte fixedSpan;
            public float span;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileDbScaleV2
        {
            public int mode;
            public float max;
            public float min;
            public byte fixedSpan;
            public float span;
            public float sensitivity;
            public int sourceSize;
            public float limit;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileFocusMode
        {
            public byte enabled;
            public float scope;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileRaw
        {
            public uint length;
            public IntPtr buffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileSpectrum
        {
            public AcFileRaw fft;
            public AcFileBand range;
            public float delta;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileSound
        {
            public AcFileRaw source;
            public AcFileRaw filter;
            public AcFileRaw downSample;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFilePrpd
        {
            public uint pointsCount;
            public IntPtr points;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileKvPair
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] key;
            public uint valueLength;
            public IntPtr valueBuffer;

            public static AcFileKvPair Create()
            {
                return new AcFileKvPair { key = new byte[16] };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcFileFrame
        {
            public ulong timeStamp;
            public AcFileRaw acousticImageSpl;
            public AcFileBuffer visualImage;
            public AcFileSound sound;
            public AcFileSpectrum spectrum;
            public AcFilePrpd prpd;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ac_file_create(string fileName, int fileType, ref IntPtr h);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_save(IntPtr h);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_close(IntPtr h);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_camera_info(IntPtr h, ref AcFileCameraInfo value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_sensor_info(IntPtr h, ref AcFileSensorInfo value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_frequency_range(IntPtr h, ref AcFileBand value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_palette(IntPtr h, ref AcFilePalette value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_get_preset_palette_image(byte refNo, ref AcFileBuffer value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_acoustic_size(IntPtr h, ref AcFileSize value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_db_scale(IntPtr h, ref AcFileDbScale value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_db_scale_v2(IntPtr h, ref AcFileDbScaleV2 value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_capture_mode(IntPtr h, byte value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_focus_mode(IntPtr h, ref AcFileFocusMode value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_distance(IntPtr h, float value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_frame_rate(IntPtr h, float value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_user_data(IntPtr h, ref AcFileKvPair value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_cover_image(IntPtr h, ref AcFileBuffer value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_append_frame(IntPtr h, ref AcFileFrame value);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ac_file_set_frame_index(IntPtr h, uint value);
    }
}
