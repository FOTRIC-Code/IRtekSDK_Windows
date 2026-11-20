using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal sealed partial class LivePreviewPanel : UserControl, ISdkFrameTarget
    {
        readonly VideoPlayRetry _video;
        DeviceSession _session;
        int _frames;
        DateTime _fpsStamp = DateTime.UtcNow;
        Bitmap _bitmap;

        public LivePreviewPanel()
        {
            InitializeComponent();
            _video = new VideoPlayRetry(this);
            _streams.SelectedIndexChanged += delegate
            {
                RefreshUrl();
                if (_video.WantPlay)
                {
                    StopPlay();
                    StartPlay();
                }
            };
            _play.Click += delegate { StartPlay(); };
            _stop.Click += delegate { StopPlay(); };
        }

        public void Bind(DeviceSession session)
        {
            Unbind();
            _session = session;
            _streams.Items.Clear();
            if (session != null)
            {
                foreach (StreamDescriptor stream in session.Streams)
                    _streams.Items.Add(stream);
                StreamPicker.SelectDefault(_streams);
            }
            RefreshUrl();
            StartPlay();
        }

        public void Unbind()
        {
            StopPlay();
            _session = null;
            _streams.Items.Clear();
            _url.Text = "";
            _status.Text = "未连接";
            Image image = _view.Image;
            _view.Image = null;
            if (image != null)
                image.Dispose();
            _bitmap = null;
            UpdatePlayButtons();
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
            if (_session != null && selected != null)
                _url.Text = _session.BuildRtspUrl(selected.Id);
            else
                _url.Text = "";
        }

        void StartPlay()
        {
            if (_video.WantPlay || _session == null || _session.Handle == IntPtr.Zero)
                return;
            StreamDescriptor selected = _streams.SelectedItem as StreamDescriptor;
            if (selected == null)
            {
                _status.Text = "没有可播放的视频流";
                UpdatePlayButtons();
                return;
            }

            _url.Text = _session.BuildRtspUrl(selected.Id);
            _frames = 0;
            _fpsStamp = DateTime.UtcNow;
            _video.Begin(_session, selected.Id, this, "正在通过 SDK 接收 RTSP", "主界面 RTSP",
                text => _status.Text = text, UpdatePlayButtons);
        }

        void StopPlay()
        {
            int? code = _video.End();
            if (code == null)
            {
                UpdatePlayButtons();
                return;
            }

            _status.Text = code.Value == NetSdk.Ok ? "已停止" : "停止失败：" + NetSdk.ErrorText(code.Value);
            UpdatePlayButtons();
            SdkLifetime.Write(code.Value == NetSdk.Ok ? "主界面 RTSP 已停止" : "停止主界面 RTSP 失败：" + NetSdk.ErrorText(code.Value));
        }

        void UpdatePlayButtons()
        {
            bool connected = _session != null && _session.Handle != IntPtr.Zero;
            bool playing = _video.WantPlay || _video.IsPlaying;
            _streams.Enabled = connected;
            _play.Enabled = connected && !playing;
            _stop.Enabled = playing;
        }

        void ReleasePreview()
        {
            StopPlay();
            if (_view == null)
                return;
            Image image = _view.Image;
            _view.Image = null;
            if (image != null)
                image.Dispose();
            _bitmap = null;
        }
    }

    internal static class StreamPicker
    {
        public static void SelectDefault(ComboBox streams)
        {
            if (streams == null)
                return;
            for (int i = 0; i < streams.Items.Count; i++)
            {
                StreamDescriptor item = streams.Items[i] as StreamDescriptor;
                if (item != null && item.Id == ChannelIds.Default)
                {
                    streams.SelectedIndex = i;
                    return;
                }
            }
            if (streams.Items.Count > 0)
                streams.SelectedIndex = 0;
        }
    }
}
