using System;
using System.Runtime.InteropServices;

namespace IRtekNetSDKDemo
{
    internal static class AcousticUtilsNative
    {
        private const string DllName = "AcousticUtils.dll";

        public const int PaletteSize = 256;
        public const int Argb = 0;

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct GridSpl
        {
            public int width;
            public int height;
            public IntPtr spl;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DbScaleV2
        {
            public int mode;
            public float max;
            public float min;
            public byte fixed_span;
            public float span;
            public float sensitivity;
            public int source_size;
            public float limit;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct FocusMode
        {
            public byte enabled;
            public float scope;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct Palette
        {
            public byte ref_no;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = PaletteSize)]
            public uint[] colors;
            public byte alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcousticPoint
        {
            public int x;
            public int y;
            public float value;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct AcousticSummary
        {
            public AcousticPoint max;
            public AcousticPoint min;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public AcousticPoint[] extreme_points;
            public int extreme_points_count;

            public static AcousticSummary Create()
            {
                return new AcousticSummary
                {
                    extreme_points = new AcousticPoint[3]
                };
            }
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "acoustic_get_spl_image_v2")]
        public static extern int acoustic_get_spl_image_v2(
            ref GridSpl spl,
            ref DbScaleV2 dbScale,
            ref FocusMode focus,
            ref Palette palette,
            int imageFormat,
            float scale,
            IntPtr data,
            ref AcousticSummary summary,
            ref byte isShow);
    }
}
