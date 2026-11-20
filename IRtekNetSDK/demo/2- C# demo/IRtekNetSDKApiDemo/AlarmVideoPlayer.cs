using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IRtekNetSDKApiDemo
{
    internal sealed partial class AlarmVideoPlayer : System.Windows.Forms.UserControl
    {
        MediaElement _media;
        System.Windows.Forms.Timer _clock;
        string _path = "";
        string _pendingPath = "";
        int _publishedSecond = -1;
        bool _forceZero;
        bool _wantPlay;
        bool _opened;

        public event Action<string> StatusChanged;
        public event Action<int> SecondChanged;

        public bool IsActive
        {
            get { return _media != null && _media.Source != null && !string.IsNullOrEmpty(_path); }
        }

        public int CurrentSecond
        {
            get
            {
                if (_forceZero)
                    return 0;
                try
                {
                    if (!_opened || _media == null || _media.Source == null)
                        return 0;
                    int second = (int)Math.Floor(_media.Position.TotalSeconds);
                    return second < 0 ? 0 : second;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public AlarmVideoPlayer()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            _media = new MediaElement();
            _media.LoadedBehavior = MediaState.Play;
            _media.UnloadedBehavior = MediaState.Manual;
            _media.Stretch = Stretch.Uniform;
            _media.ScrubbingEnabled = false;
            _media.MediaOpened += (sender, e) =>
            {
                _opened = true;
                if (!_wantPlay)
                    return;
                _media.Dispatcher.BeginInvoke(new Action(BeginPlayback));
            };
            _media.Loaded += (sender, e) =>
            {
                if (_wantPlay && _opened)
                    BeginPlayback();
            };
            _media.MediaEnded += (sender, e) =>
            {
                if (!_wantPlay)
                    return;
                _forceZero = true;
                _publishedSecond = -1;
                try
                {
                    _media.Position = TimeSpan.Zero;
                    _media.Play();
                }
                catch (Exception)
                {
                }
                PublishSecond();
            };
            _clock = new System.Windows.Forms.Timer { Interval = 200 };
            _clock.Tick += (sender, e) => PublishSecond();
            Disposed += (sender, e) => StopClock();
            _media.MediaFailed += (sender, e) =>
            {
                _wantPlay = false;
                _opened = false;
                StopClock();
                string reason = e.ErrorException != null ? e.ErrorException.Message : "未知错误";
                SetStatus("播放失败：" + reason);
            };
            _host.Child = _media;
            Resize += (sender, e) => OpenPending();
            _host.Resize += (sender, e) => OpenPending();
        }

        void OpenPending()
        {
            if (string.IsNullOrEmpty(_pendingPath) || _host == null || _host.IsDisposed)
                return;
            if (_host.ClientSize.Width < 2 || _host.ClientSize.Height < 2)
                return;
            OpenSource(new Uri(Path.GetFullPath(_pendingPath)));
        }

        public void SetMuted(bool muted)
        {
            if (_media == null)
                return;
            try
            {
                _media.IsMuted = muted;
            }
            catch (Exception)
            {
            }
        }

        public bool PlayFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return false;

            _wantPlay = true;
            _publishedSecond = -1;
            _forceZero = false;
            StopClock();
            _path = path;
            _pendingPath = path;
            try
            {
                OpenPending();
                if (!string.IsNullOrEmpty(_pendingPath))
                    SetStatus("正在打开 " + Path.GetFileName(path));
                return true;
            }
            catch (Exception ex)
            {
                _wantPlay = false;
                _pendingPath = "";
                SetStatus("播放失败：" + ex.Message);
                return false;
            }
        }

        void OpenSource(Uri uri)
        {
            if (_media == null || uri == null)
                return;
            if (_host == null || _host.ClientSize.Width < 2 || _host.ClientSize.Height < 2)
                return;
            _pendingPath = "";
            if (SameSource(uri))
            {
                _opened = true;
                try
                {
                    _media.Position = TimeSpan.Zero;
                }
                catch (Exception)
                {
                }
                BeginPlayback();
                return;
            }
            _opened = false;
            _media.Source = uri;
            SetStatus("正在打开 " + Path.GetFileName(_path));
        }

        void BeginPlayback()
        {
            if (!_wantPlay || _media == null || _media.Source == null)
                return;
            try
            {
                _media.Play();
            }
            catch (Exception ex)
            {
                SetStatus("播放失败：" + ex.Message);
                return;
            }
            StartClock();
            SetStatus("正在播放 " + Path.GetFileName(_path));
        }

        bool SameSource(Uri uri)
        {
            if (_media == null || _media.Source == null || uri == null)
                return false;
            try
            {
                return string.Equals(_media.Source.LocalPath, uri.LocalPath, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void StopPlay()
        {
            if (_media == null)
                return;
            try
            {
                _media.Stop();
                _media.Source = null;
            }
            catch (Exception)
            {
            }
            _wantPlay = false;
            _opened = false;
            _pendingPath = "";
            StopClock();
            _path = "";
            _publishedSecond = -1;
            _forceZero = false;
        }

        void StartClock()
        {
            if (_clock == null)
                return;
            _clock.Start();
            PublishSecond();
        }

        void StopClock()
        {
            if (_clock == null)
                return;
            _clock.Stop();
        }

        void PublishSecond()
        {
            int second;
            if (_forceZero)
            {
                _forceZero = false;
                second = 0;
            }
            else
            {
                try
                {
                    if (!_opened || _media == null || _media.Source == null)
                        return;
                    second = (int)Math.Floor(_media.Position.TotalSeconds);
                    if (second < 0)
                        second = 0;
                }
                catch (Exception)
                {
                    return;
                }
            }

            if (second == _publishedSecond)
                return;
            _publishedSecond = second;
            Action<int> handler = SecondChanged;
            if (handler != null)
                handler(second);
        }

        internal string DebugState()
        {
            if (_media == null)
                return "no-media";
            try
            {
                string duration = _media.NaturalDuration.HasTimeSpan
                    ? _media.NaturalDuration.TimeSpan.TotalSeconds.ToString("0.00")
                    : "none";
                return "opened=" + _opened
                    + " want=" + _wantPlay
                    + " loaded=" + _media.IsLoaded
                    + " handle=" + IsHandleCreated
                    + " host=" + (_host == null ? "null" : _host.ClientSize.Width + "x" + _host.ClientSize.Height)
                    + " pos=" + _media.Position.TotalSeconds.ToString("0.00")
                    + " box=" + _media.ActualWidth.ToString("0") + "x" + _media.ActualHeight.ToString("0")
                    + " video=" + _media.NaturalVideoWidth + "x" + _media.NaturalVideoHeight
                    + " dur=" + duration;
            }
            catch (Exception ex)
            {
                return ex.GetType().Name + " " + ex.Message;
            }
        }

        void SetStatus(string text)
        {
            Action<string> handler = StatusChanged;
            if (handler != null)
                handler(text);
        }
    }
}
