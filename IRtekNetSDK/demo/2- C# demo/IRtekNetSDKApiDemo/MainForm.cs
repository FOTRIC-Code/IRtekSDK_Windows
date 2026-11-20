using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
internal sealed partial class MainForm : Form
{
    readonly List<NetDiscoveredDevice> _found = new List<NetDiscoveredDevice>();
    readonly List<Form> _videoForms = new List<Form>();
    ApiTestForm _apiForm;
    const int AlarmReconnectIntervalMs = 3000;

    readonly object _alarmGate = new object();
    DeviceSession _session;
    System.Windows.Forms.Timer _alarmRetry;
    int _alarmListenerId;
    int _alarmRetryGeneration;
    int _busy;
    int _connectGeneration;
    bool _alarmRetrying;
    bool _alarmAttemptBusy;
    bool _connectInFlight;
    string _pendingIp;
    ushort _pendingPort;

    public MainForm()
    {
        InitializeComponent();
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            return;

        _devices.SelectedIndexChanged += (sender, e) => ApplySelection();
        _devices.DoubleClick += (sender, e) => ConnectDoubleClicked();
        _search.Click += async (sender, e) => await SearchAsync();
        _connect.Click += (sender, e) => ConnectSelected(false);
        _disconnect.Click += (sender, e) => Disconnect();
        _watch.Click += (sender, e) => ShowRaw();
        _upgrade.Click += (sender, e) => PromptUpgrade();
        _apiTest.Click += (sender, e) => ShowApiTest();
        _clearAlarms.Click += (sender, e) => _alarms.Items.Clear();
        _restartListener.Click += (sender, e) => RestartAlarmListener();
        SdkLifetime.Log += AppendLog;
        AlarmHub.Received += OnAlarmReceived;
        ExceptionHub.Received += OnExceptionReceived;
        FormClosed += (sender, e) =>
        {
            AlarmHub.Received -= OnAlarmReceived;
            ExceptionHub.Received -= OnExceptionReceived;
            SdkLifetime.Log -= AppendLog;
            Disconnect();
            if (_alarmRetry != null)
            {
                _alarmRetry.Dispose();
                _alarmRetry = null;
            }
        };
    }

    async Task SearchAsync()
    {
        if (Interlocked.Exchange(ref _busy, 1) == 1)
            return;

        _search.Enabled = false;
        uint timeout = (uint)_timeout.Value;
        AppendLog($"开始搜索设备，超时 {timeout} ms");
        try
        {
            (int code, NetDiscoveredDevice[] devices, ushort count) = await Task.Run(() =>
            {
                var buffer = new NetDiscoveredDevice[32];
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = NetDiscoveredDevice.CreateBlank();
                int ec = NetSdk.net_search_devices(buffer, (ushort)buffer.Length, out ushort found, timeout);
                return (ec, buffer, found);
            });

            _devices.Items.Clear();
            _found.Clear();
            if (code != NetSdk.Ok)
            {
                AppendLog("搜索失败：" + NetSdk.ErrorText(code));
                return;
            }

            int shown = Math.Min(count, devices.Length);
            for (int i = 0; i < shown; i++)
            {
                NetDiscoveredDevice device = devices[i];
                _found.Add(device);
                string ip = NetSdk.CString(device.Ip);
                var item = new ListViewItem(ip);
                item.SubItems.Add(NetSdk.CString(device.Name));
                item.SubItems.Add(NetSdk.CString(device.Model));
                item.SubItems.Add(NetSdk.CString(device.SerialNo));
                item.SubItems.Add(NetSdk.CString(device.FirmwareVersion));
                item.SubItems.Add(device.Wandering != 0 ? "是" : "");
                item.Tag = i;
                _devices.Items.Add(item);
            }
            AppendLog($"搜索完成，发现 {shown} 台设备");
            if (_devices.Items.Count > 0)
                _devices.Items[0].Selected = true;
        }
        catch (Exception ex)
        {
            AppendLog("搜索异常：" + ex.Message);
        }
        finally
        {
            _search.Enabled = true;
            Interlocked.Exchange(ref _busy, 0);
        }
    }

    void ApplySelection()
    {
        if (_devices.SelectedItems.Count == 0)
            return;
        if (_devices.SelectedItems[0].Tag is int index && index >= 0 && index < _found.Count)
            _manualIp.Text = NetSdk.CString(_found[index].Ip);
    }

    void ConnectDoubleClicked()
    {
        ListViewHitTestInfo hit = _devices.HitTest(_devices.PointToClient(Cursor.Position));
        if (hit.Item == null)
            return;
        hit.Item.Selected = true;
        _manualIp.Text = hit.Item.Text.Trim();
        ConnectSelected(true);
    }

    void ConnectSelected(bool replaceExisting)
    {
        string ip = _manualIp.Text.Trim();
        if (string.IsNullOrWhiteSpace(ip) && _devices.SelectedItems.Count > 0 && _devices.SelectedItems[0].Tag is int index && index < _found.Count)
            ip = NetSdk.CString(_found[index].Ip);
        if (string.IsNullOrWhiteSpace(ip))
        {
            MessageBox.Show(this, "请先搜索并选择设备，或填写手动 IP。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!ushort.TryParse(_port.Text.Trim(), out ushort port) || port == 0)
        {
            MessageBox.Show(this, "HTTP 端口无效。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ReleaseForTarget(ip, port, replaceExisting))
            return;
        if (!replaceExisting && _connectInFlight && string.Equals(_pendingIp, ip, StringComparison.OrdinalIgnoreCase) && _pendingPort == port)
        {
            AppendLog("正在连接这台设备");
            return;
        }
        if (replaceExisting && _connectInFlight)
            AppendLog($"取消正在连接的 {_pendingIp}:{_pendingPort}，改为连接 {ip}:{port}");

        _connectGeneration++;
        int generation = _connectGeneration;
        _pendingIp = ip;
        _pendingPort = port;
        _connectInFlight = true;
        SetBusy(true);
        string user = _user.Text;
        string password = _password.Text;
        AppendLog($"正在连接 {ip}:{port}");
        Task.Run(() => Login(ip, port, user, password))
            .ContinueWith(task =>
            {
                DeviceSession leftoverSession = task.Status == TaskStatus.RanToCompletion ? task.Result.Session : null;
                if (IsDisposed)
                {
                    LogoutSession(leftoverSession);
                    return;
                }

                BeginInvoke(new Action(() =>
                {
                    if (generation != _connectGeneration)
                    {
                        LogoutSession(leftoverSession);
                        return;
                    }

                    _connectInFlight = false;
                    _pendingIp = null;
                    if (task.IsFaulted)
                    {
                        SetBusy(false);
                        AppendLog("连接异常：" + task.Exception?.GetBaseException().Message);
                        return;
                    }

                    int code = task.Result.Code;
                    DeviceSession session = task.Result.Session;
                    if (code != NetSdk.Ok || session == null)
                    {
                        SetBusy(false);
                        AppendLog("连接失败：" + NetSdk.ErrorText(code));
                        MessageBox.Show(this, "连接失败：" + NetSdk.ErrorText(code), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _session = session;
                    SetBusy(false);
                    _connect.Enabled = false;
                    _disconnect.Enabled = true;
                    _watch.Enabled = true;
                    _upgrade.Enabled = true;
                    _apiTest.Enabled = true;
                    _restartListener.Enabled = true;
                    _status.Text = $"已连接 {session.Ip}:{session.HttpPort}    RTSP {session.RtspPort}    流 {session.Streams.Count}";
                    AppendLog(_status.Text);
                    _alarmQuery.Bind(session);
                    StartExceptionWatch();
                    StartAlarmListener();
                    _preview.Bind(session);
                }));
            });
    }

    internal void UseSessionForCheck(DeviceSession session)
    {
        _session = session;
        _disconnect.Enabled = session != null;
        _connect.Enabled = session == null;
        _status.Text = session == null ? "未连接" : "已连接 " + session.Ip;
    }

    internal bool HasSessionForCheck
    {
        get { return _session != null; }
    }

    internal bool ReleaseForTarget(string ip, ushort port, bool replaceExisting)
    {
        if (_session == null)
            return true;
        bool same = string.Equals(_session.Ip, ip, StringComparison.OrdinalIgnoreCase) && _session.HttpPort == port;
        if (!replaceExisting)
        {
            AppendLog(same ? "已经连接这台设备" : "已经连接，请先断开");
            return false;
        }

        AppendLog($"断开 {_session.Ip}:{_session.HttpPort}，改连 {ip}:{port}");
        _alarms.Items.Clear();
        Disconnect();
        return true;
    }

    static void LogoutSession(DeviceSession session)
    {
        if (session == null || session.Handle == IntPtr.Zero)
            return;
        IntPtr handle = session.Handle;
        session.Handle = IntPtr.Zero;
        NetSdk.net_set_exception_callback(handle, null);
        NetSdk.net_logout(ref handle);
    }

    static (int Code, DeviceSession Session) Login(string ip, ushort port, string user, string password)
    {
        NetLoginInfo login = NetLoginInfo.Create(ip, port, user, password, 180000);
        int code = NetSdk.net_login(ref login, out IntPtr handle);
        if (code != NetSdk.Ok || handle == IntPtr.Zero)
            return (code, null);

        var session = new DeviceSession
        {
            Handle = handle,
            Ip = ip,
            HttpPort = port,
            UserName = user,
            Password = password
        };

        if (NetSdk.net_get_port(handle, out NetPortInfo ports) == NetSdk.Ok && ports.RtspPortNo != 0)
            session.RtspPort = ports.RtspPortNo;

        var streams = new NetStreamInfo[8];
        for (int i = 0; i < streams.Length; i++)
            streams[i] = NetStreamInfo.CreateBlank();
        if (NetSdk.net_get_streams(handle, streams, (ushort)streams.Length, out ushort count) == NetSdk.Ok)
        {
            int shown = Math.Min(count, streams.Length);
            for (int i = 0; i < shown; i++)
            {
                session.Streams.Add(new StreamDescriptor
                {
                    Id = streams[i].Id,
                    Codec = NetSdk.CString(streams[i].CodecType),
                    Width = streams[i].ResolutionWidth,
                    Height = streams[i].ResolutionHeight
                });
            }
        }
        if (session.Streams.Count == 0)
            session.Streams.Add(new StreamDescriptor { Id = ChannelIds.Default, Codec = "video" });

        return (NetSdk.Ok, session);
    }

    void ShowApiTest()
    {
        if (_session == null)
            return;
        if (_apiForm != null && !_apiForm.IsDisposed)
        {
            _apiForm.Activate();
            return;
        }

        _apiForm = new ApiTestForm(_session);
        _apiForm.DeviceInvalidated += Disconnect;
        _apiForm.FormClosed += (sender, e) =>
        {
            if (_apiForm != null)
                _apiForm.DeviceInvalidated -= Disconnect;
            _apiForm = null;
        };
        _apiForm.Show(this);
    }

    void PromptUpgrade()
    {
        if (_session == null)
            return;

        CloseVideoForms();
        _preview.Unbind();
        using (UpgradeForm dialog = new UpgradeForm(_session))
        {
            dialog.ShowDialog(this);
            if (dialog.DeviceRestarted)
            {
                Disconnect();
                return;
            }
        }
        if (_session != null)
            _preview.Bind(_session);
    }

    void ShowRaw()
    {
        if (_session == null)
            return;
        ShowVideoForm(new RawStreamForm(_session));
    }

    void ShowVideoForm(Form form)
    {
        int index = _videoForms.Count;
        form.Text = form.Text + "  #" + (index + 1);
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(Location.X + 36 + index * 28, Location.Y + 36 + index * 28);
        _videoForms.Add(form);
        form.FormClosed += (sender, e) => _videoForms.Remove(form);
        form.Show(this);
    }

    void CloseVideoForms()
    {
        Form[] forms = _videoForms.ToArray();
        foreach (Form form in forms)
            form.Close();
        _videoForms.Clear();
    }

    void StartExceptionWatch()
    {
        if (_session == null || _session.Handle == IntPtr.Zero)
            return;
        int code = NetSdk.net_set_exception_callback(_session.Handle, ExceptionHub.Callback);
        if (code != NetSdk.Ok)
        {
            AppendLog("设置异常回调失败：" + NetSdk.ErrorText(code));
            return;
        }
        AppendLog("已注册异常回调（含报警丢失）");
    }

    void StartAlarmListener()
    {
        if (_session == null || _session.Handle == IntPtr.Zero)
            return;

        int id;
        lock (_alarmGate)
        {
            if (_alarmListenerId > 0)
                return;
            id = NetSdk.net_start_alarm_listener(_session.Handle, AlarmHub.Callback);
            if (id > 0)
                _alarmListenerId = id;
        }
        if (id <= 0)
        {
            _alarmStatus.ForeColor = Color.Firebrick;
            _alarmStatus.Text = "监听失败：" + NetSdk.ErrorText(id);
            AppendLog("报警监听开启失败：" + NetSdk.ErrorText(id));
            return;
        }

        _alarmStatus.ForeColor = Color.ForestGreen;
        _alarmStatus.Text = "监听中";
        AppendLog("报警监听已开启");
    }

    void RestartAlarmListener()
    {
        if (_session == null || _session.Handle == IntPtr.Zero)
            return;
        AppendLog("手动重新监听报警");
        ShowAlarmReconnectPending("正在重新监听报警，失败会持续重试。");
        BeginAlarmReconnect(false);
    }

    void BeginAlarmReconnect(bool fromLoss)
    {
        if (_session == null || _session.Handle == IntPtr.Zero)
            return;

        bool already = _alarmRetrying;
        _alarmRetrying = true;
        if (_alarmRetry == null)
        {
            _alarmRetry = new System.Windows.Forms.Timer { Interval = AlarmReconnectIntervalMs };
            _alarmRetry.Tick += (sender, e) => AttemptAlarmReconnect();
        }
        _alarmRetry.Start();
        if (!already)
            AppendLog(fromLoss ? "报警回调丢失，开始持续重连" : "开始重新监听报警");
        AttemptAlarmReconnect();
    }

    void AttemptAlarmReconnect()
    {
        if (_alarmAttemptBusy || !_alarmRetrying || _session == null || _session.Handle == IntPtr.Zero)
            return;

        _alarmAttemptBusy = true;
        IntPtr handle = _session.Handle;
        int generation = _alarmRetryGeneration;
        Task.Run(() => ReconnectAlarmListener(handle, generation))
            .ContinueWith(task =>
            {
                if (IsDisposed || !IsHandleCreated)
                    return;
                try
                {
                    BeginInvoke(new Action(() => ApplyAlarmReconnect(generation, task)));
                }
                catch (Exception)
                {
                }
            });
    }

    (int Id, int Error) ReconnectAlarmListener(IntPtr handle, int generation)
    {
        lock (_alarmGate)
        {
            if (generation != _alarmRetryGeneration || handle == IntPtr.Zero)
                return (0, 0);
            if (_alarmListenerId > 0)
                StopAlarmListenerLocked();
            int id = NetSdk.net_start_alarm_listener(handle, AlarmHub.Callback);
            if (generation != _alarmRetryGeneration)
            {
                if (id > 0)
                    NetSdk.net_stop_alarm_listener(new IntPtr(id));
                return (0, 0);
            }
            if (id > 0)
                _alarmListenerId = id;
            return (id, id > 0 ? NetSdk.Ok : id);
        }
    }

    void ApplyAlarmReconnect(int generation, Task<(int Id, int Error)> task)
    {
        _alarmAttemptBusy = false;
        if (IsDisposed || generation != _alarmRetryGeneration || !_alarmRetrying)
            return;
        if (task.IsFaulted)
        {
            AppendLog("报警监听重连异常：" + task.Exception.GetBaseException().Message + "，将继续重试");
            ShowAlarmReconnectPending("报警回调丢失，重连失败，正在持续重试。");
            return;
        }
        if (task.Result.Id > 0)
        {
            _alarmRetrying = false;
            if (_alarmRetry != null)
                _alarmRetry.Stop();
            _alarmStatus.ForeColor = Color.ForestGreen;
            _alarmStatus.Text = "监听中";
            _exceptionBanner.BackColor = Color.Honeydew;
            _exceptionBanner.ForeColor = Color.DarkGreen;
            _exceptionBanner.Text = "报警监听已重连。";
            AppendLog("报警监听已重连");
            return;
        }

        AppendLog("报警监听重连失败：" + NetSdk.ErrorText(task.Result.Error) + "，将继续重试");
        ShowAlarmReconnectPending("报警回调丢失，重连失败，正在持续重试。");
    }

    void ShowAlarmReconnectPending(string text)
    {
        _alarmStatus.ForeColor = Color.Firebrick;
        _alarmStatus.Text = "重连中";
        _exceptionBanner.BackColor = Color.MistyRose;
        _exceptionBanner.ForeColor = Color.Firebrick;
        _exceptionBanner.Text = text;
    }

    void CancelAlarmReconnect()
    {
        if (_alarmRetry != null)
            _alarmRetry.Stop();
        int code = NetSdk.Ok;
        bool stopped = false;
        lock (_alarmGate)
        {
            _alarmRetryGeneration++;
            _alarmRetrying = false;
            if (_alarmListenerId > 0)
            {
                code = StopAlarmListenerLocked();
                stopped = true;
            }
        }
        if (stopped)
            ShowAlarmStopped(code);
    }

    void StopAlarmListener()
    {
        int code;
        lock (_alarmGate)
        {
            if (_alarmListenerId <= 0)
                return;
            code = StopAlarmListenerLocked();
        }
        ShowAlarmStopped(code);
    }

    int StopAlarmListenerLocked()
    {
        int id = _alarmListenerId;
        _alarmListenerId = 0;
        return NetSdk.net_stop_alarm_listener(new IntPtr(id));
    }

    void ShowAlarmStopped(int code)
    {
        _alarmStatus.ForeColor = SystemColors.ControlText;
        _alarmStatus.Text = "未监听";
        AppendLog(code == NetSdk.Ok ? "报警监听已停止" : "停止报警监听失败：" + NetSdk.ErrorText(code));
    }

    void OnAlarmReceived(AlarmNotice notice)
    {
        if (IsDisposed || !IsHandleCreated)
            return;

        try
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnAlarmReceived(notice)));
                return;
            }
        }
        catch (Exception)
        {
            return;
        }

        var item = new ListViewItem(notice.Time);
        item.SubItems.Add(notice.Type);
        item.SubItems.Add(notice.Detail);
        item.ToolTipText = notice.Detail;
        _alarms.Items.Insert(0, item);
        while (_alarms.Items.Count > 300)
            _alarms.Items.RemoveAt(_alarms.Items.Count - 1);
        AppendLog($"报警 {notice.Type}  {notice.Detail}");
    }

    void OnExceptionReceived(ExceptionNotice notice)
    {
        if (IsDisposed || !IsHandleCreated)
            return;

        try
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnExceptionReceived(notice)));
                return;
            }
        }
        catch (Exception)
        {
            return;
        }

        string detail = notice.Message;
        if (notice.StreamId != 0)
            detail = "stream " + notice.StreamId + "  " + detail;
        var item = new ListViewItem(notice.Time);
        item.SubItems.Add(notice.TypeName);
        item.SubItems.Add(detail);
        if (notice.IsAlarmLost)
        {
            item.BackColor = Color.MistyRose;
            item.ForeColor = Color.Firebrick;
            _alarmStatus.ForeColor = Color.Firebrick;
            _alarmStatus.Text = "报警丢失";
            _exceptionBanner.BackColor = Color.MistyRose;
            _exceptionBanner.ForeColor = Color.Firebrick;
            _exceptionBanner.Text = "报警回调已丢失，正在持续重连。";
            BeginAlarmReconnect(true);
        }
        else if (notice.Type == NetSdk.ExceptionVideoLost)
        {
            item.BackColor = Color.MistyRose;
            item.ForeColor = Color.Firebrick;
            if (!_alarmRetrying)
            {
                _exceptionBanner.BackColor = Color.MistyRose;
                _exceptionBanner.ForeColor = Color.Firebrick;
                _exceptionBanner.Text = "视频流已丢失，正在持续重连。";
            }
        }
        else if (notice.Type == NetSdk.ExceptionNetworkDisconnected)
        {
            item.BackColor = Color.FromArgb(255, 243, 205);
            if (!_alarmRetrying)
                _exceptionBanner.Text = "网络断开。若报警监听也中断，随后会自动重连。";
        }
        else if (notice.Type == NetSdk.ExceptionNetworkResumed)
        {
            item.BackColor = Color.Honeydew;
            if (_alarmRetrying)
            {
                _exceptionBanner.Text = "网络已恢复，正在持续重连报警监听。";
            }
            else
            {
                _exceptionBanner.BackColor = Color.Honeydew;
                _exceptionBanner.ForeColor = Color.DarkGreen;
                _exceptionBanner.Text = "网络已恢复。";
            }
        }
        _alarms.Items.Insert(0, item);
        while (_alarms.Items.Count > 300)
            _alarms.Items.RemoveAt(_alarms.Items.Count - 1);
        AppendLog("异常 " + notice.TypeName + "  " + detail);
    }

    void Disconnect()
    {
        _connectGeneration++;
        _connectInFlight = false;
        _pendingIp = null;
        _preview.Unbind();
        CloseVideoForms();
        if (_apiForm != null && !_apiForm.IsDisposed)
            _apiForm.Close();
        _apiForm = null;
        _alarmQuery.Unbind();
        CancelAlarmReconnect();

        if (_session == null)
        {
            SetBusy(false);
            return;
        }

        IntPtr handle = _session.Handle;
        _session = null;
        _disconnect.Enabled = false;
        _watch.Enabled = false;
        _upgrade.Enabled = false;
        _apiTest.Enabled = false;
        _restartListener.Enabled = false;
        _connect.Enabled = true;
        _status.Text = "未连接";
        _alarmStatus.ForeColor = SystemColors.ControlText;
        _alarmStatus.Text = "未监听";
        _exceptionBanner.BackColor = Color.FromArgb(255, 243, 205);
        _exceptionBanner.ForeColor = Color.FromArgb(102, 60, 0);
        _exceptionBanner.Text = "连接后会监听报警。报警回调或视频流丢失后会持续重连。";
        if (handle != IntPtr.Zero)
        {
            NetSdk.net_set_exception_callback(handle, null);
            int code = NetSdk.net_logout(ref handle);
            AppendLog(code == NetSdk.Ok ? "已断开" : "断开失败：" + NetSdk.ErrorText(code));
        }
        SetBusy(false);
    }

    void SetBusy(bool busy)
    {
        _search.Enabled = !busy;
        _connect.Enabled = !busy && _session == null;
        _watch.Enabled = !busy && _session != null;
        _upgrade.Enabled = !busy && _session != null;
        _apiTest.Enabled = !busy && _session != null;
        _restartListener.Enabled = !busy && _session != null;
        UseWaitCursor = busy;
    }

    void AppendLog(string message)
    {
        if (IsDisposed)
            return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => AppendLog(message)));
            return;
        }

        _log.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }
}
}
