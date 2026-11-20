using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal sealed partial class RtspForm : Form, ISdkFrameTarget
    {
        readonly DeviceSession _session;
        readonly VideoPlayRetry _video;
        int _nullCallbackSessionId;
        int _frames;
        DateTime _fpsStamp = DateTime.UtcNow;
        Bitmap _bitmap;

        public RtspForm()
        {
            InitializeComponent();
            _video = new VideoPlayRetry(this);
        }

        public RtspForm(DeviceSession session) : this()
        {
            _session = session;
            if (session == null)
                return;
            Text = "RTSP - " + session.Ip;
            foreach (StreamDescriptor stream in session.Streams)
                _streams.Items.Add(stream);
            StreamPicker.SelectDefault(_streams);
            _streams.SelectedIndexChanged += delegate
            {
                RefreshUrl();
                if (_video.WantPlay)
                {
                    StopPlay();
                    StartPlay();
                }
            };
            RefreshUrl();
            _play.Click += delegate { StartPlay(); };
            _stop.Click += delegate { StopPlay(); };
            _stopCallback.Click += delegate { StopByClearCallback(); };
            UpdatePlayButtons();
            Shown += delegate { StartPlay(); };
            FormClosing += delegate { StopPlay(); };
        }

        public void PresentFrame(int width, int height, byte[] pixels)
        {
            if (IsDisposed || width <= 0 || height <= 0)
                return;

            int stride = width * 4;
            if (pixels.Length < stride * height)
                return;

            if (_bitmap == null || _bitmap.Width != width || _bitmap.Height != height)
            {
                Bitmap previous = _bitmap;
                _bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                _view.Image = _bitmap;
                if (previous != null)
                    previous.Dispose();
            }

            BitmapData bits = _bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            try
            {
                if (bits.Stride == stride)
                    Marshal.Copy(pixels, 0, bits.Scan0, stride * height);
                else
                {
                    for (int y = 0; y < height; y++)
                        Marshal.Copy(pixels, y * stride, IntPtr.Add(bits.Scan0, y * bits.Stride), stride);
                }
            }
            finally
            {
                _bitmap.UnlockBits(bits);
            }

            _view.Invalidate();
            _frames++;
            double elapsed = (DateTime.UtcNow - _fpsStamp).TotalSeconds;
            if (elapsed >= 1)
            {
                double fps = _frames / elapsed;
                _frames = 0;
                _fpsStamp = DateTime.UtcNow;
                _status.Text = width + " x " + height + "    " + fps.ToString("0.0") + " fps";
            }
        }

        void RefreshUrl()
        {
            StreamDescriptor selected = _streams.SelectedItem as StreamDescriptor;
            if (selected != null)
                _url.Text = _session.BuildRtspUrl(selected.Id);
        }

        void StartPlay()
        {
            if (_video.WantPlay || _video.IsPlaying || _nullCallbackSessionId > 0 || _session.Handle == IntPtr.Zero)
                return;
            StreamDescriptor selected = _streams.SelectedItem as StreamDescriptor;
            if (selected == null)
                return;

            RefreshUrl();
            _frames = 0;
            _fpsStamp = DateTime.UtcNow;
            _video.Begin(_session, selected.Id, this, "正在通过 SDK 接收 RTSP", "RTSP",
                text => _status.Text = text, UpdatePlayButtons);
        }

        void StopPlay()
        {
            int? playCode = _video.End();
            int nullCode = ReleaseNullCallback();
            if (playCode == null && nullCode == NetSdk.Ok)
            {
                UpdatePlayButtons();
                return;
            }

            int code = playCode ?? nullCode;
            if (playCode != null && playCode.Value != NetSdk.Ok)
                code = playCode.Value;
            else if (nullCode != NetSdk.Ok)
                code = nullCode;
            _status.Text = code == NetSdk.Ok ? "已停止" : "停止失败：" + NetSdk.ErrorText(code);
            UpdatePlayButtons();
            SdkLifetime.Write(code == NetSdk.Ok ? "RTSP 已停止" : "停止 RTSP 失败：" + NetSdk.ErrorText(code));
        }

        void StopByClearCallback()
        {
            if (!_video.IsPlaying || _session.Handle == IntPtr.Zero)
                return;

            NetStreamInfo info = NetStreamInfo.CreateBlank();
            StreamDescriptor selected = _streams.SelectedItem as StreamDescriptor;
            if (selected != null && NetSdk.net_get_stream(_session.Handle, selected.Id, ref info) != NetSdk.Ok)
            {
                info = NetStreamInfo.CreateBlank();
                info.Id = selected.Id;
            }
            int sessionId = NetSdk.net_start_stream_play(_session.Handle, ref info, NetSdk.StreamBgra, null);
            if (sessionId <= 0)
            {
                _status.Text = "回调置空失败：" + NetSdk.ErrorText(sessionId);
                SdkLifetime.Write("RTSP 回调置空失败：" + NetSdk.ErrorText(sessionId));
                UpdatePlayButtons();
                return;
            }

            _video.PauseRetry();
            StreamHub.Detach(this);
            _nullCallbackSessionId = sessionId;
            _status.Text = "已停止（net_start_stream_play 回调为空）";
            UpdatePlayButtons();
            SdkLifetime.Write("RTSP 已用空回调调用 net_start_stream_play");
        }

        int ReleaseNullCallback()
        {
            if (_nullCallbackSessionId <= 0)
                return NetSdk.Ok;
            int id = _nullCallbackSessionId;
            _nullCallbackSessionId = 0;
            return NetSdk.net_stop_stream_play(new IntPtr(id));
        }

        void UpdatePlayButtons()
        {
            bool playing = _video.WantPlay || _video.IsPlaying || _nullCallbackSessionId > 0;
            _play.Enabled = !playing;
            _stop.Enabled = playing;
            _stopCallback.Enabled = _video.IsPlaying && _nullCallbackSessionId <= 0;
        }

        void ReleaseFrame()
        {
            if (_view == null)
                return;
            Image image = _view.Image;
            _view.Image = null;
            if (image != null)
                image.Dispose();
            _bitmap = null;
        }
    }
}
