using System;
using System.Runtime.InteropServices;

namespace IRtekNetSDKDemo
{
    internal class FFmpegDecoder : IDisposable
    {
        private const string AvCodec = "avcodec-59.dll";
        private const string AvUtil = "avutil-57.dll";
        private const string SwScale = "swscale-6.dll";

        private const int AV_CODEC_ID_H264 = 27;
        private const int AV_PIX_FMT_YUV420P = 0;
        private const int AV_PIX_FMT_BGRA = 28;
        private const int SWS_BILINEAR = 2;

        #region AVPacket managed struct

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeAVPacket
        {
            public IntPtr buf;
            public long pts;
            public long dts;
            public IntPtr data;
            public int size;
            public int stream_index;
            public int flags;
            public IntPtr side_data;
            public int side_data_elems;
            public long duration;
            public long pos;
            public IntPtr opaque;
            public IntPtr opaque_ref;
            public int time_base_num;
            public int time_base_den;
        }

        #endregion

        #region P/Invoke avcodec

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr avcodec_find_decoder(int id);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr avcodec_alloc_context3(IntPtr codec);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avcodec_open2(IntPtr ctx, IntPtr codec, IntPtr options);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avcodec_send_packet(IntPtr ctx, IntPtr pkt);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avcodec_receive_frame(IntPtr ctx, IntPtr frame);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern void avcodec_free_context(ref IntPtr ctx);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern void avcodec_flush_buffers(IntPtr ctx);

        #endregion

        #region P/Invoke avutil

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr av_frame_alloc();

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern void av_frame_free(ref IntPtr frame);

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern void av_frame_unref(IntPtr frame);

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr av_malloc(int size);

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern void av_free(IntPtr ptr);

        #endregion

        #region P/Invoke swscale

        [DllImport(SwScale, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sws_getContext(
            int srcW, int srcH, int srcFormat,
            int dstW, int dstH, int dstFormat,
            int flags, IntPtr srcFilter, IntPtr dstFilter, IntPtr param);

        [DllImport(SwScale, CallingConvention = CallingConvention.Cdecl)]
        private static extern int sws_scale(
            IntPtr ctx,
            IntPtr srcSlice, IntPtr srcStride,
            int srcSliceY, int srcSliceH,
            IntPtr dstSlice, IntPtr dstStride);

        [DllImport(SwScale, CallingConvention = CallingConvention.Cdecl)]
        private static extern void sws_freeContext(IntPtr ctx);

        #endregion

        // AVFrame layout (FFmpeg 5.x):
        //   uint8_t *data[8]          -> 8 pointers
        //   int linesize[8]           -> 8 ints
        //   uint8_t **extended_data   -> 1 pointer
        //   int width                 -> int
        //   int height                -> int
        //   int nb_samples            -> int
        //   int format                -> int
        private static readonly int FrameDataOffset = 0;
        private static readonly int FrameLinesizeOffset = 8 * IntPtr.Size;
        private static readonly int FrameExtendedDataOffset = FrameLinesizeOffset + 8 * sizeof(int);
        private static readonly int FrameWidthOffset = FrameExtendedDataOffset + IntPtr.Size;
        private static readonly int FrameHeightOffset = FrameWidthOffset + sizeof(int);

        private IntPtr _codecCtx;
        private IntPtr _frame;
        private IntPtr _swsCtx;
        private int _swsWidth;
        private int _swsHeight;
        private IntPtr _bgraBuffer;
        private int _bgraBufferSize;
        private bool _disposed;

        public bool Initialize()
        {
            IntPtr codec = avcodec_find_decoder(AV_CODEC_ID_H264);
            if (codec == IntPtr.Zero)
                return false;

            _codecCtx = avcodec_alloc_context3(codec);
            if (_codecCtx == IntPtr.Zero)
                return false;

            int ret = avcodec_open2(_codecCtx, codec, IntPtr.Zero);
            if (ret < 0)
            {
                avcodec_free_context(ref _codecCtx);
                return false;
            }

            _frame = av_frame_alloc();
            if (_frame == IntPtr.Zero)
            {
                Dispose();
                return false;
            }

            return true;
        }

        public bool Decode(IntPtr h264Data, int dataLength, out IntPtr bgraOut, out int width, out int height, out int stride)
        {
            bgraOut = IntPtr.Zero;
            width = 0;
            height = 0;
            stride = 0;

            if (_codecCtx == IntPtr.Zero || _disposed)
                return false;

            var pkt = new NativeAVPacket();
            pkt.data = h264Data;
            pkt.size = dataLength;
            pkt.pts = long.MinValue; // AV_NOPTS_VALUE
            pkt.dts = long.MinValue;

            GCHandle pktHandle = GCHandle.Alloc(pkt, GCHandleType.Pinned);
            int sendRet;
            try
            {
                sendRet = avcodec_send_packet(_codecCtx, pktHandle.AddrOfPinnedObject());
            }
            finally
            {
                pktHandle.Free();
            }

            if (sendRet < 0)
                return false;

            av_frame_unref(_frame);
            int recvRet = avcodec_receive_frame(_codecCtx, _frame);
            if (recvRet < 0)
                return false;

            int frameWidth = Marshal.ReadInt32(_frame, FrameWidthOffset);
            int frameHeight = Marshal.ReadInt32(_frame, FrameHeightOffset);

            if (frameWidth <= 0 || frameHeight <= 0)
                return false;

            EnsureSwsContext(frameWidth, frameHeight);
            if (_swsCtx == IntPtr.Zero)
                return false;

            IntPtr srcData = _frame + FrameDataOffset;
            IntPtr srcLinesize = _frame + FrameLinesizeOffset;

            int dstStride = frameWidth * 4;

            IntPtr dstDataArray = Marshal.AllocHGlobal(IntPtr.Size * 4);
            IntPtr dstStrideArray = Marshal.AllocHGlobal(sizeof(int) * 4);
            try
            {
                Marshal.WriteIntPtr(dstDataArray, 0, _bgraBuffer);
                Marshal.WriteIntPtr(dstDataArray, IntPtr.Size, IntPtr.Zero);
                Marshal.WriteIntPtr(dstDataArray, IntPtr.Size * 2, IntPtr.Zero);
                Marshal.WriteIntPtr(dstDataArray, IntPtr.Size * 3, IntPtr.Zero);

                Marshal.WriteInt32(dstStrideArray, 0, dstStride);
                Marshal.WriteInt32(dstStrideArray, sizeof(int), 0);
                Marshal.WriteInt32(dstStrideArray, sizeof(int) * 2, 0);
                Marshal.WriteInt32(dstStrideArray, sizeof(int) * 3, 0);

                sws_scale(_swsCtx, srcData, srcLinesize, 0, frameHeight, dstDataArray, dstStrideArray);

                bgraOut = _bgraBuffer;
                width = frameWidth;
                height = frameHeight;
                stride = dstStride;
                return true;
            }
            finally
            {
                Marshal.FreeHGlobal(dstDataArray);
                Marshal.FreeHGlobal(dstStrideArray);
            }
        }

        private void EnsureSwsContext(int w, int h)
        {
            if (_swsCtx != IntPtr.Zero && _swsWidth == w && _swsHeight == h)
                return;

            if (_swsCtx != IntPtr.Zero)
                sws_freeContext(_swsCtx);

            _swsCtx = sws_getContext(
                w, h, AV_PIX_FMT_YUV420P,
                w, h, AV_PIX_FMT_BGRA,
                SWS_BILINEAR, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            _swsWidth = w;
            _swsHeight = h;

            int needed = w * h * 4;
            if (_bgraBuffer != IntPtr.Zero && _bgraBufferSize < needed)
            {
                av_free(_bgraBuffer);
                _bgraBuffer = IntPtr.Zero;
            }
            if (_bgraBuffer == IntPtr.Zero)
            {
                _bgraBuffer = av_malloc(needed + 64);
                _bgraBufferSize = needed;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_swsCtx != IntPtr.Zero)
            {
                sws_freeContext(_swsCtx);
                _swsCtx = IntPtr.Zero;
            }
            if (_bgraBuffer != IntPtr.Zero)
            {
                av_free(_bgraBuffer);
                _bgraBuffer = IntPtr.Zero;
            }
            if (_frame != IntPtr.Zero)
                av_frame_free(ref _frame);
            if (_codecCtx != IntPtr.Zero)
                avcodec_free_context(ref _codecCtx);
        }
    }
}
