using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace IRtekHLAPIDemo
{
    /// <summary>
    /// Managed FFmpeg RTSP player: opens an RTSP URL, decodes video to BGRA, and
    /// invokes a callback with a copied frame buffer on a background thread.
    /// </summary>
    internal sealed class RtspPlayer : IDisposable
    {
        private const string AvFormat = "avformat-59.dll";
        private const string AvCodec = "avcodec-59.dll";
        private const string AvUtil = "avutil-57.dll";
        private const string SwScale = "swscale-6.dll";

        private const int AVMEDIA_TYPE_VIDEO = 0;
        private const int AV_PIX_FMT_BGRA = 28;
        private const int SWS_BILINEAR = 2;

        // FFmpeg 5.1.2 (lavf 59 / lavc 59 / lavu 57). Offsets come from Sequential
        // layouts so x86 and x64 both match the MinGW-w64 / MSVC ABI.
        private static readonly int FmtNbStreamsOffset =
            (int)Marshal.OffsetOf(typeof(AVFormatContextHead), "nb_streams");
        private static readonly int FmtStreamsOffset =
            (int)Marshal.OffsetOf(typeof(AVFormatContextHead), "streams");
        private static readonly int StreamCodecparOffset =
            (int)Marshal.OffsetOf(typeof(AVStreamLayout), "codecpar");
        private static readonly int PacketStreamIndexOffset =
            (int)Marshal.OffsetOf(typeof(AVPacketLayout), "stream_index");

        // AVFrame (FFmpeg 5.x): data[8], linesize[8], extended_data, width, height, nb_samples, format
        private static readonly int FrameDataOffset = 0;
        private static readonly int FrameLinesizeOffset = 8 * IntPtr.Size;
        private static readonly int FrameExtendedDataOffset = FrameLinesizeOffset + 8 * sizeof(int);
        private static readonly int FrameWidthOffset = FrameExtendedDataOffset + IntPtr.Size;
        private static readonly int FrameHeightOffset = FrameWidthOffset + sizeof(int);
        private static readonly int FrameNbSamplesOffset = FrameHeightOffset + sizeof(int);
        private static readonly int FrameFormatOffset = FrameNbSamplesOffset + sizeof(int);

        [StructLayout(LayoutKind.Sequential)]
        private struct AVRationalLayout
        {
            public int num;
            public int den;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AVFormatContextHead
        {
            public IntPtr av_class;
            public IntPtr iformat;
            public IntPtr oformat;
            public IntPtr priv_data;
            public IntPtr pb;
            public int ctx_flags;
            public uint nb_streams;
            public IntPtr streams;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AVPacketLayout
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
            public AVRationalLayout time_base;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AVStreamLayout
        {
            public int index;
            public int id;
            public IntPtr priv_data;
            public AVRationalLayout time_base;
            public long start_time;
            public long duration;
            public long nb_frames;
            public int disposition;
            public int discard;
            public AVRationalLayout sample_aspect_ratio;
            public IntPtr metadata;
            public AVRationalLayout avg_frame_rate;
            public AVPacketLayout attached_pic;
            public IntPtr side_data;
            public int nb_side_data;
            public int event_flags;
            public AVRationalLayout r_frame_rate;
            public IntPtr codecpar;
            public int pts_wrap_bits;
        }

        private static int _networkInited;

        private readonly object _sync = new object();
        private Thread _worker;
        private volatile bool _stop;
        private volatile bool _running;
        private bool _disposed;
        private Action<byte[], int, int> _onFrame;

        public bool IsRunning
        {
            get { return _running; }
        }

        #region P/Invoke avformat

        [DllImport(AvFormat, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avformat_network_init();

        [DllImport(AvFormat, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avformat_open_input(
            ref IntPtr ps, [MarshalAs(UnmanagedType.LPStr)] string url, IntPtr fmt, ref IntPtr options);

        [DllImport(AvFormat, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avformat_find_stream_info(IntPtr ic, IntPtr options);

        [DllImport(AvFormat, CallingConvention = CallingConvention.Cdecl)]
        private static extern int av_find_best_stream(
            IntPtr ic, int type, int wantedStreamNb, int relatedStream, IntPtr decoderRet, int flags);

        [DllImport(AvFormat, CallingConvention = CallingConvention.Cdecl)]
        private static extern int av_read_frame(IntPtr s, IntPtr pkt);

        [DllImport(AvFormat, CallingConvention = CallingConvention.Cdecl)]
        private static extern void avformat_close_input(ref IntPtr s);

        #endregion

        #region P/Invoke avcodec

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr avcodec_alloc_context3(IntPtr codec);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avcodec_parameters_to_context(IntPtr codecCtx, IntPtr codecpar);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avcodec_open2(IntPtr ctx, IntPtr codec, IntPtr options);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avcodec_send_packet(IntPtr ctx, IntPtr pkt);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern int avcodec_receive_frame(IntPtr ctx, IntPtr frame);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern void avcodec_free_context(ref IntPtr ctx);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr av_packet_alloc();

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern void av_packet_unref(IntPtr pkt);

        [DllImport(AvCodec, CallingConvention = CallingConvention.Cdecl)]
        private static extern void av_packet_free(ref IntPtr pkt);

        #endregion

        #region P/Invoke avutil

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern int av_dict_set(ref IntPtr pm, [MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] string value, int flags);

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern void av_dict_free(ref IntPtr pm);

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr av_frame_alloc();

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern void av_frame_free(ref IntPtr frame);

        [DllImport(AvUtil, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr av_malloc(UIntPtr size);

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

        public bool Start(string rtspUrl, Action<byte[], int, int> onFrame)
        {
            if (string.IsNullOrEmpty(rtspUrl) || onFrame == null)
                return false;

            lock (_sync)
            {
                if (_disposed)
                    return false;
                StopLocked();

                EnsureNetworkInit();
                _onFrame = onFrame;
                _stop = false;
                _running = true;
                _worker = new Thread(PlayLoop)
                {
                    IsBackground = true,
                    Name = "RtspPlayer"
                };
                _worker.Start(rtspUrl);
                return true;
            }
        }

        public void Stop()
        {
            lock (_sync)
            {
                StopLocked();
            }
        }

        private void StopLocked()
        {
            _stop = true;
            Thread worker = _worker;
            _worker = null;
            if (worker != null && worker.IsAlive)
            {
                try { worker.Join(); } catch { }
            }
            _running = false;
            _onFrame = null;
        }

        public void Dispose()
        {
            lock (_sync)
            {
                if (_disposed) return;
                _disposed = true;
                StopLocked();
            }
        }

        private static void EnsureNetworkInit()
        {
            if (Interlocked.CompareExchange(ref _networkInited, 1, 0) == 0)
                avformat_network_init();
        }

        private void PlayLoop(object state)
        {
            string url = (string)state;
            IntPtr fmtCtx = IntPtr.Zero;
            IntPtr codecCtx = IntPtr.Zero;
            IntPtr frame = IntPtr.Zero;
            IntPtr packet = IntPtr.Zero;
            IntPtr swsCtx = IntPtr.Zero;
            IntPtr bgraBuf = IntPtr.Zero;
            int bgraBufSize = 0;
            int swsW = 0, swsH = 0, swsFmt = -1;

            try
            {
                IntPtr opts = IntPtr.Zero;
                av_dict_set(ref opts, "rtsp_transport", "tcp", 0);
                av_dict_set(ref opts, "stimeout", "5000000", 0);
                av_dict_set(ref opts, "max_delay", "500000", 0);

                int ret = avformat_open_input(ref fmtCtx, url, IntPtr.Zero, ref opts);
                if (opts != IntPtr.Zero)
                    av_dict_free(ref opts);
                if (ret < 0 || fmtCtx == IntPtr.Zero)
                    return;

                ret = avformat_find_stream_info(fmtCtx, IntPtr.Zero);
                if (ret < 0)
                    return;

                IntPtr decoderHolder = Marshal.AllocHGlobal(IntPtr.Size);
                IntPtr decoder;
                int videoIdx;
                try
                {
                    Marshal.WriteIntPtr(decoderHolder, IntPtr.Zero);
                    videoIdx = av_find_best_stream(fmtCtx, AVMEDIA_TYPE_VIDEO, -1, -1, decoderHolder, 0);
                    decoder = Marshal.ReadIntPtr(decoderHolder);
                }
                finally
                {
                    Marshal.FreeHGlobal(decoderHolder);
                }

                if (videoIdx < 0 || decoder == IntPtr.Zero)
                    return;

                int nbStreams = Marshal.ReadInt32(fmtCtx, FmtNbStreamsOffset);
                if (videoIdx >= nbStreams)
                    return;

                IntPtr streamsPtr = Marshal.ReadIntPtr(fmtCtx, FmtStreamsOffset);
                if (streamsPtr == IntPtr.Zero)
                    return;
                IntPtr streamPtr = Marshal.ReadIntPtr(streamsPtr, videoIdx * IntPtr.Size);
                if (streamPtr == IntPtr.Zero)
                    return;
                IntPtr codecpar = Marshal.ReadIntPtr(streamPtr, StreamCodecparOffset);
                if (codecpar == IntPtr.Zero)
                    return;

                codecCtx = avcodec_alloc_context3(decoder);
                if (codecCtx == IntPtr.Zero)
                    return;
                if (avcodec_parameters_to_context(codecCtx, codecpar) < 0)
                    return;
                if (avcodec_open2(codecCtx, decoder, IntPtr.Zero) < 0)
                    return;

                frame = av_frame_alloc();
                packet = av_packet_alloc();
                if (frame == IntPtr.Zero || packet == IntPtr.Zero)
                    return;

                IntPtr dstDataArray = Marshal.AllocHGlobal(IntPtr.Size * 4);
                IntPtr dstStrideArray = Marshal.AllocHGlobal(sizeof(int) * 4);
                try
                {
                    while (!_stop)
                    {
                        ret = av_read_frame(fmtCtx, packet);
                        if (ret < 0)
                            break;

                        int streamIndex = Marshal.ReadInt32(packet, PacketStreamIndexOffset);
                        if (streamIndex != videoIdx)
                        {
                            av_packet_unref(packet);
                            continue;
                        }

                        ret = avcodec_send_packet(codecCtx, packet);
                        av_packet_unref(packet);
                        if (ret < 0)
                            continue;

                        while (!_stop)
                        {
                            ret = avcodec_receive_frame(codecCtx, frame);
                            if (ret != 0)
                                break;

                            int width = Marshal.ReadInt32(frame, FrameWidthOffset);
                            int height = Marshal.ReadInt32(frame, FrameHeightOffset);
                            int format = Marshal.ReadInt32(frame, FrameFormatOffset);
                            if (width <= 0 || height <= 0)
                                continue;

                            if (swsCtx == IntPtr.Zero || swsW != width || swsH != height || swsFmt != format)
                            {
                                if (swsCtx != IntPtr.Zero)
                                {
                                    sws_freeContext(swsCtx);
                                    swsCtx = IntPtr.Zero;
                                }
                                swsCtx = sws_getContext(
                                    width, height, format,
                                    width, height, AV_PIX_FMT_BGRA,
                                    SWS_BILINEAR, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                                swsW = width;
                                swsH = height;
                                swsFmt = format;
                                if (swsCtx == IntPtr.Zero)
                                    break;

                                int needed = width * height * 4;
                                if (bgraBuf != IntPtr.Zero && bgraBufSize < needed)
                                {
                                    av_free(bgraBuf);
                                    bgraBuf = IntPtr.Zero;
                                    bgraBufSize = 0;
                                }
                                if (bgraBuf == IntPtr.Zero)
                                {
                                    bgraBuf = av_malloc(new UIntPtr((uint)(needed + 64)));
                                    bgraBufSize = needed;
                                }
                                if (bgraBuf == IntPtr.Zero)
                                    break;
                            }

                            Marshal.WriteIntPtr(dstDataArray, 0, bgraBuf);
                            Marshal.WriteIntPtr(dstDataArray, IntPtr.Size, IntPtr.Zero);
                            Marshal.WriteIntPtr(dstDataArray, IntPtr.Size * 2, IntPtr.Zero);
                            Marshal.WriteIntPtr(dstDataArray, IntPtr.Size * 3, IntPtr.Zero);

                            int dstStride = width * 4;
                            Marshal.WriteInt32(dstStrideArray, 0, dstStride);
                            Marshal.WriteInt32(dstStrideArray, sizeof(int), 0);
                            Marshal.WriteInt32(dstStrideArray, sizeof(int) * 2, 0);
                            Marshal.WriteInt32(dstStrideArray, sizeof(int) * 3, 0);

                            IntPtr srcData = frame + FrameDataOffset;
                            IntPtr srcLinesize = frame + FrameLinesizeOffset;
                            sws_scale(swsCtx, srcData, srcLinesize, 0, height, dstDataArray, dstStrideArray);

                            int bytes = width * height * 4;
                            byte[] copy = new byte[bytes];
                            Marshal.Copy(bgraBuf, copy, 0, bytes);

                            Action<byte[], int, int> cb = _onFrame;
                            if (cb != null)
                            {
                                try { cb(copy, width, height); }
                                catch { }
                            }
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(dstDataArray);
                    Marshal.FreeHGlobal(dstStrideArray);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RtspPlayer error: " + ex.Message);
            }
            finally
            {
                if (swsCtx != IntPtr.Zero)
                    sws_freeContext(swsCtx);
                if (bgraBuf != IntPtr.Zero)
                    av_free(bgraBuf);
                if (packet != IntPtr.Zero)
                    av_packet_free(ref packet);
                if (frame != IntPtr.Zero)
                    av_frame_free(ref frame);
                if (codecCtx != IntPtr.Zero)
                    avcodec_free_context(ref codecCtx);
                if (fmtCtx != IntPtr.Zero)
                    avformat_close_input(ref fmtCtx);
                _running = false;
            }
        }
    }
}
