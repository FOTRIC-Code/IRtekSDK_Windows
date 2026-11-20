using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal interface ISdkFrameTarget
    {
        bool IsDisposed { get; }
        void PresentFrame(int width, int height, byte[] pixels);
        IAsyncResult BeginInvoke(Delegate method);
    }

    internal static class StreamHub
    {
        sealed class Slot
        {
            public ISdkFrameTarget Target;
            public uint StreamId;
            public byte[] Pixels;
            public int Width;
            public int Height;
            public int Pending;
        }

        static readonly object Gate = new object();
        static readonly List<Slot> Slots = new List<Slot>();

        public static readonly NetSdk.StreamCallback Callback = OnStream;

        public static void Attach(ISdkFrameTarget target, uint streamId)
        {
            lock (Gate)
            {
                DetachLocked(target);
                Slot slot = new Slot();
                slot.Target = target;
                slot.StreamId = streamId;
                Slots.Add(slot);
            }
        }

        public static void Detach(ISdkFrameTarget target)
        {
            lock (Gate)
                DetachLocked(target);
        }

        static void DetachLocked(ISdkFrameTarget target)
        {
            for (int i = Slots.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(Slots[i].Target, target))
                    Slots.RemoveAt(i);
            }
        }

        static void OnStream(IntPtr streamData)
        {
            if (streamData == IntPtr.Zero)
                return;

            NetStreamData frame;
            try
            {
                frame = Marshal.PtrToStructure<NetStreamData>(streamData);
            }
            catch (Exception)
            {
                return;
            }

            if (frame.Data == IntPtr.Zero || frame.DataLength == 0 || frame.Width == 0 || frame.Height == 0)
                return;

            int expected = frame.Width * frame.Height * 4;
            if (frame.DataLength < expected)
                return;

            byte[] pixels = new byte[expected];
            Marshal.Copy(frame.Data, pixels, 0, expected);

            List<Slot> ready = new List<Slot>();
            lock (Gate)
            {
                for (int i = 0; i < Slots.Count; i++)
                {
                    Slot slot = Slots[i];
                    if (slot.Target == null || slot.Target.IsDisposed || slot.StreamId != frame.StreamId)
                        continue;
                    slot.Pixels = pixels;
                    slot.Width = frame.Width;
                    slot.Height = frame.Height;
                    if (Interlocked.Exchange(ref slot.Pending, 1) == 0)
                        ready.Add(slot);
                }
            }

            for (int i = 0; i < ready.Count; i++)
                Post(ready[i]);
        }

        static void Post(Slot slot)
        {
            ISdkFrameTarget target = slot.Target;
            try
            {
                target.BeginInvoke(new Action(delegate
                {
                    byte[] latest;
                    int width;
                    int height;
                    lock (Gate)
                    {
                        latest = slot.Pixels;
                        width = slot.Width;
                        height = slot.Height;
                    }
                    Interlocked.Exchange(ref slot.Pending, 0);
                    if (latest != null && !target.IsDisposed)
                        target.PresentFrame(width, height, latest);
                }));
            }
            catch (Exception)
            {
                Interlocked.Exchange(ref slot.Pending, 0);
            }
        }
    }

    internal sealed class VideoPlayRetry
    {
        const int IntervalMs = 3000;

        readonly Control _ui;
        readonly object _gate = new object();
        System.Windows.Forms.Timer _timer;
        DeviceSession _session;
        ISdkFrameTarget _target;
        Action<string> _setStatus;
        Action _updateButtons;
        string _playingStatus = "";
        string _logName = "视频流";
        volatile int _generation;
        int _playId;
        uint _streamId;
        bool _busy;
        bool _lost;
        volatile bool _wantPlay;

        public VideoPlayRetry(Control ui)
        {
            _ui = ui;
            _ui.Disposed += delegate { Shutdown(); };
        }

        public bool WantPlay { get { return _wantPlay; } }

        public bool IsPlaying
        {
            get
            {
                lock (_gate)
                    return _playId > 0;
            }
        }

        public void Begin(DeviceSession session, uint streamId, ISdkFrameTarget target, string playingStatus, string logName, Action<string> setStatus, Action updateButtons)
        {
            lock (_gate)
            {
                _generation++;
                _wantPlay = true;
            }

            _session = session;
            _streamId = streamId;
            _target = target;
            _playingStatus = playingStatus;
            _logName = string.IsNullOrEmpty(logName) ? "视频流" : logName;
            _setStatus = setStatus;
            _updateButtons = updateButtons;
            _lost = false;
            ExceptionHub.Received -= OnException;
            ExceptionHub.Received += OnException;
            EnsureTimer();
            _timer.Start();
            StreamHub.Attach(target, streamId);
            if (_setStatus != null)
                _setStatus("正在连接视频流");
            if (_updateButtons != null)
                _updateButtons();
            Attempt();
        }

        public int? End()
        {
            if (_timer != null)
                _timer.Stop();
            ExceptionHub.Received -= OnException;
            int playId;
            lock (_gate)
            {
                _generation++;
                _wantPlay = false;
                playId = _playId;
                _playId = 0;
            }
            if (_target != null)
                StreamHub.Detach(_target);
            if (_updateButtons != null)
                _updateButtons();
            if (playId <= 0)
                return null;
            // net_stop_stream_play joins the decode thread. Doing that on the UI
            // thread freezes the message pump while av_read_frame is still blocked.
            if (_ui.InvokeRequired)
                return StopPlay(playId);
            int stoppedId = playId;
            string logName = _logName;
            Task.Run(() =>
            {
                int code = StopPlay(stoppedId);
                if (code != NetSdk.Ok)
                    SdkLifetime.Write(logName + " 停止失败：" + NetSdk.ErrorText(code));
            });
            return NetSdk.Ok;
        }

        public void PauseRetry()
        {
            if (_timer != null)
                _timer.Stop();
            ExceptionHub.Received -= OnException;
            lock (_gate)
            {
                _generation++;
                _wantPlay = false;
            }
            if (_updateButtons != null)
                _updateButtons();
        }

        void Shutdown()
        {
            End();
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        void EnsureTimer()
        {
            if (_timer != null)
                return;
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = IntervalMs;
            _timer.Tick += delegate { Attempt(); };
        }

        void OnException(ExceptionNotice notice)
        {
            if (notice.Type != NetSdk.ExceptionVideoLost || notice.StreamId != _streamId || !_wantPlay)
                return;
            if (_ui.IsDisposed || !_ui.IsHandleCreated)
                return;
            try
            {
                if (_ui.InvokeRequired)
                {
                    _ui.BeginInvoke(new Action(() => OnException(notice)));
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

            _lost = true;
            if (_setStatus != null)
                _setStatus("视频流已丢失，正在重连");
            SdkLifetime.Write(_logName + " 丢失，开始持续重连");
            if (_timer != null)
                _timer.Start();
            if (!_busy)
                Attempt();
        }

        void Attempt()
        {
            if (_busy || !_wantPlay || _session == null || _session.Handle == IntPtr.Zero)
                return;

            _busy = true;
            IntPtr handle = _session.Handle;
            uint streamId = _streamId;
            int generation;
            lock (_gate)
                generation = _generation;
            Task.Run(() => Open(handle, generation, streamId))
                .ContinueWith(task =>
                {
                    if (_ui.IsDisposed || !_ui.IsHandleCreated)
                        return;
                    try
                    {
                        _ui.BeginInvoke(new Action(() => Apply(generation, task)));
                    }
                    catch (Exception)
                    {
                    }
                });
        }

        (int Id, int Error) Open(IntPtr handle, int generation, uint streamId)
        {
            int previousId;
            lock (_gate)
            {
                if (generation != _generation || !_wantPlay || handle == IntPtr.Zero)
                    return (0, 0);
                previousId = _playId;
                _playId = 0;
            }
            if (previousId > 0)
                StopPlay(previousId);
            if (!StillCurrent(generation) || handle == IntPtr.Zero)
                return (0, 0);

            NetStreamInfo info = NetStreamInfo.CreateBlank();
            if (NetSdk.net_get_stream(handle, streamId, ref info) != NetSdk.Ok)
            {
                info = NetStreamInfo.CreateBlank();
                info.Id = streamId;
            }
            if (!StillCurrent(generation))
                return (0, 0);

            int id = NetSdk.net_start_stream_play(handle, ref info, NetSdk.StreamBgra, StreamHub.Callback);
            int abandon = 0;
            lock (_gate)
            {
                if (generation != _generation || !_wantPlay)
                    abandon = id > 0 ? id : 0;
                else if (id > 0)
                    _playId = id;
            }
            if (abandon > 0)
            {
                StopPlay(abandon);
                return (0, 0);
            }
            return (id, id > 0 ? NetSdk.Ok : id);
        }

        bool StillCurrent(int generation)
        {
            lock (_gate)
                return generation == _generation && _wantPlay;
        }

        void Apply(int generation, Task<(int Id, int Error)> task)
        {
            bool lost = _lost;
            _lost = false;
            _busy = false;
            if (_ui.IsDisposed || generation != _generation || !_wantPlay)
            {
                if (!_ui.IsDisposed && _wantPlay)
                    Attempt();
                return;
            }
            if (task.IsFaulted)
            {
                SdkLifetime.Write(_logName + " 重连异常：" + task.Exception.GetBaseException().Message + "，将继续重试");
                ShowRetrying();
                return;
            }
            if (task.Result.Id > 0 && !lost)
            {
                if (_timer != null)
                    _timer.Stop();
                if (_setStatus != null)
                    _setStatus(_playingStatus);
                if (_updateButtons != null)
                    _updateButtons();
                SdkLifetime.Write(_logName + " 已打开，流 " + _streamId);
                return;
            }
            if (task.Result.Id > 0 && lost)
            {
                ShowRetrying();
                return;
            }

            SdkLifetime.Write(_logName + " 重连失败：" + NetSdk.ErrorText(task.Result.Error) + "，将继续重试");
            ShowRetrying();
        }

        void ShowRetrying()
        {
            if (_setStatus != null)
                _setStatus("视频流重连失败，正在重试");
            if (_updateButtons != null)
                _updateButtons();
            if (_timer != null)
                _timer.Start();
        }

        int StopPlay(int playId)
        {
            if (playId <= 0)
                return NetSdk.Ok;
            try
            {
                return NetSdk.net_stop_stream_play(new IntPtr(playId));
            }
            catch (Exception ex)
            {
                SdkLifetime.Write(_logName + " 停止异常：" + ex.Message);
                return -1;
            }
        }
    }
}
