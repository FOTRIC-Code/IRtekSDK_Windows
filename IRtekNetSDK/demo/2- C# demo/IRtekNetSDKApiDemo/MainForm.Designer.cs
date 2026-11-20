using System.Drawing;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._devices = new System.Windows.Forms.ListView();
            this._deviceIp = new System.Windows.Forms.ColumnHeader();
            this._deviceName = new System.Windows.Forms.ColumnHeader();
            this._deviceModel = new System.Windows.Forms.ColumnHeader();
            this._deviceSerial = new System.Windows.Forms.ColumnHeader();
            this._deviceFirmware = new System.Windows.Forms.ColumnHeader();
            this._deviceConflict = new System.Windows.Forms.ColumnHeader();
            this._port = new System.Windows.Forms.TextBox();
            this._user = new System.Windows.Forms.TextBox();
            this._password = new System.Windows.Forms.TextBox();
            this._manualIp = new System.Windows.Forms.TextBox();
            this._timeout = new System.Windows.Forms.NumericUpDown();
            this._search = new System.Windows.Forms.Button();
            this._connect = new System.Windows.Forms.Button();
            this._disconnect = new System.Windows.Forms.Button();
            this._watch = new System.Windows.Forms.Button();
            this._upgrade = new System.Windows.Forms.Button();
            this._apiTest = new System.Windows.Forms.Button();
            this._status = new System.Windows.Forms.Label();
            this._log = new System.Windows.Forms.TextBox();
            this._alarms = new System.Windows.Forms.ListView();
            this._alarmTime = new System.Windows.Forms.ColumnHeader();
            this._alarmType = new System.Windows.Forms.ColumnHeader();
            this._alarmDetail = new System.Windows.Forms.ColumnHeader();
            this._alarmStatus = new System.Windows.Forms.Label();
            this._clearAlarms = new System.Windows.Forms.Button();
            this._restartListener = new System.Windows.Forms.Button();
            this._exceptionBanner = new System.Windows.Forms.Label();
            this._alarmQuery = new IRtekNetSDKApiDemo.AlarmQueryPanel();
            this._tabs = new System.Windows.Forms.TabControl();
            this._devicePage = new System.Windows.Forms.TabPage();
            this._alarmPage = new System.Windows.Forms.TabPage();
            this._preview = new IRtekNetSDKApiDemo.LivePreviewPanel();
            this._searchBar = new System.Windows.Forms.FlowLayoutPanel();
            this._lblTimeout = new System.Windows.Forms.Label();
            this._fields = new System.Windows.Forms.TableLayoutPanel();
            this._lblPort = new System.Windows.Forms.Label();
            this._lblUser = new System.Windows.Forms.Label();
            this._lblPassword = new System.Windows.Forms.Label();
            this._lblManualIp = new System.Windows.Forms.Label();
            this._lblHint = new System.Windows.Forms.Label();
            this._actions = new System.Windows.Forms.FlowLayoutPanel();
            this._side = new System.Windows.Forms.Panel();
            this._alarmBar = new System.Windows.Forms.FlowLayoutPanel();
            this._lblAlarmTitle = new System.Windows.Forms.Label();
            this._alarmHost = new System.Windows.Forms.Panel();
            this._logHost = new System.Windows.Forms.Panel();
            this._lblLog = new System.Windows.Forms.Label();
            this._bottom = new System.Windows.Forms.TableLayoutPanel();
            this._body = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._timeout)).BeginInit();
            this._tabs.SuspendLayout();
            this._devicePage.SuspendLayout();
            this._alarmPage.SuspendLayout();
            this._searchBar.SuspendLayout();
            this._fields.SuspendLayout();
            this._actions.SuspendLayout();
            this._side.SuspendLayout();
            this._alarmBar.SuspendLayout();
            this._alarmHost.SuspendLayout();
            this._logHost.SuspendLayout();
            this._bottom.SuspendLayout();
            this._body.SuspendLayout();
            this.SuspendLayout();
            // 
            // _devices
            // 
            this._devices.Dock = System.Windows.Forms.DockStyle.Fill;
            this._devices.FullRowSelect = true;
            this._devices.GridLines = true;
            this._devices.HideSelection = false;
            this._devices.Location = new System.Drawing.Point(0, 0);
            this._devices.Name = "_devices";
            this._devices.Size = new System.Drawing.Size(904, 545);
            this._devices.TabIndex = 0;
            this._devices.UseCompatibleStateImageBehavior = false;
            this._devices.View = System.Windows.Forms.View.Details;
            this._deviceIp.Text = "IP";
            this._deviceIp.Width = 140;
            this._deviceName.Text = "名称";
            this._deviceName.Width = 140;
            this._deviceModel.Text = "型号";
            this._deviceModel.Width = 140;
            this._deviceSerial.Text = "序列号";
            this._deviceSerial.Width = 120;
            this._deviceFirmware.Text = "固件";
            this._deviceFirmware.Width = 160;
            this._deviceConflict.Text = "地址冲突";
            this._deviceConflict.Width = 80;
            this._devices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._deviceIp,
            this._deviceName,
            this._deviceModel,
            this._deviceSerial,
            this._deviceFirmware,
            this._deviceConflict});
            // 
            // _port
            // 
            this._port.Dock = System.Windows.Forms.DockStyle.Top;
            this._port.Location = new System.Drawing.Point(80, 12);
            this._port.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this._port.Name = "_port";
            this._port.Size = new System.Drawing.Size(264, 23);
            this._port.TabIndex = 1;
            this._port.Text = "80";
            // 
            // _user
            // 
            this._user.Dock = System.Windows.Forms.DockStyle.Top;
            this._user.Location = new System.Drawing.Point(80, 32);
            this._user.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(264, 23);
            this._user.TabIndex = 3;
            // 
            // _password
            // 
            this._password.Dock = System.Windows.Forms.DockStyle.Top;
            this._password.Location = new System.Drawing.Point(80, 52);
            this._password.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this._password.Name = "_password";
            this._password.Size = new System.Drawing.Size(264, 23);
            this._password.TabIndex = 5;
            this._password.UseSystemPasswordChar = true;
            // 
            // _manualIp
            // 
            this._manualIp.Dock = System.Windows.Forms.DockStyle.Top;
            this._manualIp.Location = new System.Drawing.Point(80, 72);
            this._manualIp.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this._manualIp.Name = "_manualIp";
            this._manualIp.Size = new System.Drawing.Size(264, 23);
            this._manualIp.TabIndex = 7;
            // 
            // _timeout
            // 
            this._timeout.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this._timeout.Location = new System.Drawing.Point(164, 8);
            this._timeout.Margin = new System.Windows.Forms.Padding(0);
            this._timeout.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            this._timeout.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this._timeout.Name = "_timeout";
            this._timeout.Size = new System.Drawing.Size(120, 23);
            this._timeout.TabIndex = 2;
            this._timeout.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // _search
            // 
            this._search.AutoSize = true;
            this._search.Location = new System.Drawing.Point(11, 11);
            this._search.Name = "_search";
            this._search.Size = new System.Drawing.Size(75, 27);
            this._search.TabIndex = 0;
            this._search.Text = "搜索设备";
            // 
            // _connect
            // 
            this._connect.AutoSize = true;
            this._connect.Location = new System.Drawing.Point(3, 3);
            this._connect.Name = "_connect";
            this._connect.Size = new System.Drawing.Size(75, 27);
            this._connect.TabIndex = 0;
            this._connect.Text = "连接";
            // 
            // _disconnect
            // 
            this._disconnect.AutoSize = true;
            this._disconnect.Enabled = false;
            this._disconnect.Location = new System.Drawing.Point(84, 3);
            this._disconnect.Name = "_disconnect";
            this._disconnect.Size = new System.Drawing.Size(75, 27);
            this._disconnect.TabIndex = 1;
            this._disconnect.Text = "断开";
            // 
            // _watch
            // 
            this._watch.AutoSize = true;
            this._watch.Enabled = false;
            this._watch.Location = new System.Drawing.Point(165, 3);
            this._watch.Name = "_watch";
            this._watch.Size = new System.Drawing.Size(78, 27);
            this._watch.TabIndex = 2;
            this._watch.Text = "原始视频流";
            // 
            // _upgrade
            // 
            this._upgrade.AutoSize = true;
            this._upgrade.Enabled = false;
            this._upgrade.Location = new System.Drawing.Point(249, 3);
            this._upgrade.Name = "_upgrade";
            this._upgrade.Size = new System.Drawing.Size(75, 27);
            this._upgrade.TabIndex = 3;
            this._upgrade.Text = "升级";
            // 
            // _apiTest
            // 
            this._apiTest.AutoSize = true;
            this._apiTest.Enabled = false;
            this._apiTest.Location = new System.Drawing.Point(3, 36);
            this._apiTest.Name = "_apiTest";
            this._apiTest.Size = new System.Drawing.Size(75, 27);
            this._apiTest.TabIndex = 4;
            this._apiTest.Text = "接口测试";
            // 
            // _status
            // 
            this._status.AutoSize = true;
            this._fields.SetColumnSpan(this._status, 2);
            this._status.Location = new System.Drawing.Point(11, 128);
            this._status.Name = "_status";
            this._status.Size = new System.Drawing.Size(44, 17);
            this._status.TabIndex = 10;
            this._status.Text = "未连接";
            // 
            // _log
            // 
            this._log.Dock = System.Windows.Forms.DockStyle.Fill;
            this._log.Location = new System.Drawing.Point(0, 22);
            this._log.Multiline = true;
            this._log.Name = "_log";
            this._log.ReadOnly = true;
            this._log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._log.Size = new System.Drawing.Size(525, 204);
            this._log.TabIndex = 0;
            // 
            // _alarms
            // 
            this._alarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this._alarms.FullRowSelect = true;
            this._alarms.GridLines = true;
            this._alarms.HideSelection = false;
            this._alarms.Location = new System.Drawing.Point(0, 73);
            this._alarms.Name = "_alarms";
            this._alarms.ShowItemToolTips = true;
            this._alarms.Size = new System.Drawing.Size(719, 153);
            this._alarms.TabIndex = 0;
            this._alarms.UseCompatibleStateImageBehavior = false;
            this._alarms.View = System.Windows.Forms.View.Details;
            this._alarmTime.Text = "时间";
            this._alarmTime.Width = 80;
            this._alarmType.Text = "类型";
            this._alarmType.Width = 90;
            this._alarmDetail.Text = "内容";
            this._alarmDetail.Width = 1100;
            this._alarms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._alarmTime,
            this._alarmType,
            this._alarmDetail});
            // 
            // _alarmStatus
            // 
            this._alarmStatus.AutoSize = true;
            this._alarmStatus.Location = new System.Drawing.Point(102, 0);
            this._alarmStatus.Name = "_alarmStatus";
            this._alarmStatus.Padding = new System.Windows.Forms.Padding(8, 4, 0, 0);
            this._alarmStatus.Size = new System.Drawing.Size(52, 21);
            this._alarmStatus.TabIndex = 1;
            this._alarmStatus.Text = "未监听";
            // 
            // _clearAlarms
            // 
            this._clearAlarms.AutoSize = true;
            this._clearAlarms.Location = new System.Drawing.Point(241, 3);
            this._clearAlarms.Name = "_clearAlarms";
            this._clearAlarms.Size = new System.Drawing.Size(75, 27);
            this._clearAlarms.TabIndex = 3;
            this._clearAlarms.Text = "清空";
            // 
            // _restartListener
            // 
            this._restartListener.AutoSize = true;
            this._restartListener.Enabled = false;
            this._restartListener.Location = new System.Drawing.Point(160, 3);
            this._restartListener.Name = "_restartListener";
            this._restartListener.Size = new System.Drawing.Size(75, 27);
            this._restartListener.TabIndex = 2;
            this._restartListener.Text = "重新监听";
            // 
            // _exceptionBanner
            // 
            this._exceptionBanner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(243)))), ((int)(((byte)(205)))));
            this._exceptionBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this._exceptionBanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(60)))), ((int)(((byte)(0)))));
            this._exceptionBanner.Location = new System.Drawing.Point(0, 0);
            this._exceptionBanner.Name = "_exceptionBanner";
            this._exceptionBanner.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this._exceptionBanner.Size = new System.Drawing.Size(719, 36);
            this._exceptionBanner.TabIndex = 2;
            this._exceptionBanner.Text = "连接后会监听报警。报警回调或视频流丢失后会持续重连。";
            this._exceptionBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _alarmQuery
            // 
            this._alarmQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this._alarmQuery.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this._alarmQuery.Location = new System.Drawing.Point(4, 4);
            this._alarmQuery.Name = "_alarmQuery";
            this._alarmQuery.Size = new System.Drawing.Size(896, 537);
            this._alarmQuery.TabIndex = 0;
            // 
            // _tabs
            // 
            this._tabs.Controls.Add(this._devicePage);
            this._tabs.Controls.Add(this._alarmPage);
            this._tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabs.Location = new System.Drawing.Point(8, 0);
            this._tabs.Name = "_tabs";
            this._tabs.SelectedIndex = 0;
            this._tabs.Size = new System.Drawing.Size(912, 575);
            this._tabs.TabIndex = 0;
            // 
            // _devicePage
            // 
            this._devicePage.Controls.Add(this._devices);
            this._devicePage.Location = new System.Drawing.Point(4, 26);
            this._devicePage.Name = "_devicePage";
            this._devicePage.Size = new System.Drawing.Size(904, 545);
            this._devicePage.TabIndex = 0;
            this._devicePage.Text = "设备";
            // 
            // _alarmPage
            // 
            this._alarmPage.Controls.Add(this._alarmQuery);
            this._alarmPage.Location = new System.Drawing.Point(4, 26);
            this._alarmPage.Name = "_alarmPage";
            this._alarmPage.Padding = new System.Windows.Forms.Padding(4);
            this._alarmPage.Size = new System.Drawing.Size(904, 545);
            this._alarmPage.TabIndex = 1;
            this._alarmPage.Text = "报警查询与录像";
            // 
            // _preview
            // 
            this._preview.Dock = System.Windows.Forms.DockStyle.Fill;
            this._preview.Location = new System.Drawing.Point(0, 156);
            this._preview.Name = "_preview";
            this._preview.Size = new System.Drawing.Size(352, 411);
            this._preview.TabIndex = 0;
            // 
            // _searchBar
            // 
            this._searchBar.AutoSize = true;
            this._searchBar.Controls.Add(this._search);
            this._searchBar.Controls.Add(this._lblTimeout);
            this._searchBar.Controls.Add(this._timeout);
            this._searchBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._searchBar.Location = new System.Drawing.Point(0, 0);
            this._searchBar.Name = "_searchBar";
            this._searchBar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this._searchBar.Size = new System.Drawing.Size(1280, 45);
            this._searchBar.TabIndex = 3;
            this._searchBar.WrapContents = false;
            // 
            // _lblTimeout
            // 
            this._lblTimeout.AutoSize = true;
            this._lblTimeout.Location = new System.Drawing.Point(92, 8);
            this._lblTimeout.Name = "_lblTimeout";
            this._lblTimeout.Padding = new System.Windows.Forms.Padding(12, 6, 0, 0);
            this._lblTimeout.Size = new System.Drawing.Size(69, 23);
            this._lblTimeout.TabIndex = 1;
            this._lblTimeout.Text = "超时(ms)";
            // 
            // _fields
            // 
            this._fields.AutoSize = true;
            this._fields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._fields.ColumnCount = 2;
            this._fields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this._fields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._fields.Controls.Add(this._lblPort, 0, 0);
            this._fields.Controls.Add(this._port, 1, 0);
            this._fields.Controls.Add(this._lblUser, 0, 1);
            this._fields.Controls.Add(this._user, 1, 1);
            this._fields.Controls.Add(this._lblPassword, 0, 2);
            this._fields.Controls.Add(this._password, 1, 2);
            this._fields.Controls.Add(this._lblManualIp, 0, 3);
            this._fields.Controls.Add(this._manualIp, 1, 3);
            this._fields.Controls.Add(this._lblHint, 0, 4);
            this._fields.Controls.Add(this._actions, 0, 5);
            this._fields.Controls.Add(this._status, 0, 6);
            this._fields.Dock = System.Windows.Forms.DockStyle.Top;
            this._fields.Location = new System.Drawing.Point(0, 0);
            this._fields.Name = "_fields";
            this._fields.Padding = new System.Windows.Forms.Padding(8);
            this._fields.RowCount = 7;
            this._fields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._fields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._fields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._fields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._fields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._fields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._fields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._fields.Size = new System.Drawing.Size(352, 156);
            this._fields.TabIndex = 1;
            // 
            // _lblPort
            // 
            this._lblPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblPort.AutoSize = true;
            this._lblPort.Location = new System.Drawing.Point(8, 16);
            this._lblPort.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this._lblPort.Name = "_lblPort";
            this._lblPort.Size = new System.Drawing.Size(66, 12);
            this._lblPort.TabIndex = 0;
            this._lblPort.Text = "HTTP 端口";
            // 
            // _lblUser
            // 
            this._lblUser.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblUser.AutoSize = true;
            this._lblUser.Location = new System.Drawing.Point(8, 36);
            this._lblUser.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this._lblUser.Name = "_lblUser";
            this._lblUser.Size = new System.Drawing.Size(44, 12);
            this._lblUser.TabIndex = 2;
            this._lblUser.Text = "用户名";
            // 
            // _lblPassword
            // 
            this._lblPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblPassword.AutoSize = true;
            this._lblPassword.Location = new System.Drawing.Point(8, 56);
            this._lblPassword.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this._lblPassword.Name = "_lblPassword";
            this._lblPassword.Size = new System.Drawing.Size(32, 12);
            this._lblPassword.TabIndex = 4;
            this._lblPassword.Text = "密码";
            // 
            // _lblManualIp
            // 
            this._lblManualIp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblManualIp.AutoSize = true;
            this._lblManualIp.Location = new System.Drawing.Point(8, 76);
            this._lblManualIp.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this._lblManualIp.Name = "_lblManualIp";
            this._lblManualIp.Size = new System.Drawing.Size(47, 12);
            this._lblManualIp.TabIndex = 6;
            this._lblManualIp.Text = "手动 IP";
            // 
            // _lblHint
            // 
            this._lblHint.AutoSize = true;
            this._fields.SetColumnSpan(this._lblHint, 2);
            this._lblHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this._lblHint.Location = new System.Drawing.Point(8, 92);
            this._lblHint.Margin = new System.Windows.Forms.Padding(0, 4, 0, 8);
            this._lblHint.Name = "_lblHint";
            this._lblHint.Size = new System.Drawing.Size(258, 8);
            this._lblHint.TabIndex = 8;
            this._lblHint.Text = "Parkia 默认端口 80，用户名和密码可以留空。";
            // 
            // _actions
            // 
            this._actions.AutoSize = true;
            this._actions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._fields.SetColumnSpan(this._actions, 2);
            this._actions.Controls.Add(this._connect);
            this._actions.Controls.Add(this._disconnect);
            this._actions.Controls.Add(this._watch);
            this._actions.Controls.Add(this._upgrade);
            this._actions.Controls.Add(this._apiTest);
            this._actions.Dock = System.Windows.Forms.DockStyle.Top;
            this._actions.Location = new System.Drawing.Point(11, 111);
            this._actions.Name = "_actions";
            this._actions.Size = new System.Drawing.Size(330, 14);
            this._actions.TabIndex = 9;
            // 
            // _side
            // 
            this._side.Controls.Add(this._preview);
            this._side.Controls.Add(this._fields);
            this._side.Dock = System.Windows.Forms.DockStyle.Right;
            this._side.Location = new System.Drawing.Point(920, 45);
            this._side.Name = "_side";
            this._side.Padding = new System.Windows.Forms.Padding(0, 0, 8, 8);
            this._side.Size = new System.Drawing.Size(360, 575);
            this._side.TabIndex = 1;
            // 
            // _alarmBar
            // 
            this._alarmBar.AutoSize = true;
            this._alarmBar.Controls.Add(this._lblAlarmTitle);
            this._alarmBar.Controls.Add(this._alarmStatus);
            this._alarmBar.Controls.Add(this._restartListener);
            this._alarmBar.Controls.Add(this._clearAlarms);
            this._alarmBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._alarmBar.Location = new System.Drawing.Point(0, 36);
            this._alarmBar.Name = "_alarmBar";
            this._alarmBar.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this._alarmBar.Size = new System.Drawing.Size(719, 37);
            this._alarmBar.TabIndex = 1;
            this._alarmBar.WrapContents = false;
            // 
            // _lblAlarmTitle
            // 
            this._lblAlarmTitle.AutoSize = true;
            this._lblAlarmTitle.Location = new System.Drawing.Point(3, 0);
            this._lblAlarmTitle.Name = "_lblAlarmTitle";
            this._lblAlarmTitle.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this._lblAlarmTitle.Size = new System.Drawing.Size(93, 21);
            this._lblAlarmTitle.TabIndex = 0;
            this._lblAlarmTitle.Text = "实时报警 / 异常";
            // 
            // _alarmHost
            // 
            this._alarmHost.Controls.Add(this._alarms);
            this._alarmHost.Controls.Add(this._alarmBar);
            this._alarmHost.Controls.Add(this._exceptionBanner);
            this._alarmHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this._alarmHost.Location = new System.Drawing.Point(11, 3);
            this._alarmHost.Name = "_alarmHost";
            this._alarmHost.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this._alarmHost.Size = new System.Drawing.Size(727, 226);
            this._alarmHost.TabIndex = 0;
            // 
            // _logHost
            // 
            this._logHost.Controls.Add(this._log);
            this._logHost.Controls.Add(this._lblLog);
            this._logHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logHost.Location = new System.Drawing.Point(744, 3);
            this._logHost.Name = "_logHost";
            this._logHost.Size = new System.Drawing.Size(525, 226);
            this._logHost.TabIndex = 1;
            // 
            // _lblLog
            // 
            this._lblLog.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblLog.Location = new System.Drawing.Point(0, 0);
            this._lblLog.Name = "_lblLog";
            this._lblLog.Size = new System.Drawing.Size(525, 22);
            this._lblLog.TabIndex = 1;
            this._lblLog.Text = "日志";
            // 
            // _bottom
            // 
            this._bottom.ColumnCount = 2;
            this._bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this._bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            this._bottom.Controls.Add(this._alarmHost, 0, 0);
            this._bottom.Controls.Add(this._logHost, 1, 0);
            this._bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._bottom.Location = new System.Drawing.Point(0, 620);
            this._bottom.Name = "_bottom";
            this._bottom.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this._bottom.RowCount = 1;
            this._bottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._bottom.Size = new System.Drawing.Size(1280, 240);
            this._bottom.TabIndex = 2;
            // 
            // _body
            // 
            this._body.Controls.Add(this._tabs);
            this._body.Dock = System.Windows.Forms.DockStyle.Fill;
            this._body.Location = new System.Drawing.Point(0, 45);
            this._body.Name = "_body";
            this._body.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this._body.Size = new System.Drawing.Size(920, 575);
            this._body.TabIndex = 0;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1280, 860);
            this.Controls.Add(this._body);
            this.Controls.Add(this._side);
            this.Controls.Add(this._bottom);
            this.Controls.Add(this._searchBar);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 720);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IRtekNetSDK API Demo";
            ((System.ComponentModel.ISupportInitialize)(this._timeout)).EndInit();
            this._tabs.ResumeLayout(false);
            this._devicePage.ResumeLayout(false);
            this._alarmPage.ResumeLayout(false);
            this._searchBar.ResumeLayout(false);
            this._searchBar.PerformLayout();
            this._fields.ResumeLayout(false);
            this._fields.PerformLayout();
            this._actions.ResumeLayout(false);
            this._actions.PerformLayout();
            this._side.ResumeLayout(false);
            this._side.PerformLayout();
            this._alarmBar.ResumeLayout(false);
            this._alarmBar.PerformLayout();
            this._alarmHost.ResumeLayout(false);
            this._alarmHost.PerformLayout();
            this._logHost.ResumeLayout(false);
            this._logHost.PerformLayout();
            this._bottom.ResumeLayout(false);
            this._body.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ListView _devices;
        private ColumnHeader _deviceIp;
        private ColumnHeader _deviceName;
        private ColumnHeader _deviceModel;
        private ColumnHeader _deviceSerial;
        private ColumnHeader _deviceFirmware;
        private ColumnHeader _deviceConflict;
        private TextBox _port;
        private TextBox _user;
        private TextBox _password;
        private TextBox _manualIp;
        private NumericUpDown _timeout;
        private Button _search;
        private Button _connect;
        private Button _disconnect;
        private Button _watch;
        private Button _upgrade;
        private Button _apiTest;
        private Label _status;
        private TextBox _log;
        private ListView _alarms;
        private ColumnHeader _alarmTime;
        private ColumnHeader _alarmType;
        private ColumnHeader _alarmDetail;
        private Label _alarmStatus;
        private Button _clearAlarms;
        private Button _restartListener;
        private Label _exceptionBanner;
        private AlarmQueryPanel _alarmQuery;
        private TabControl _tabs;
        private TabPage _devicePage;
        private TabPage _alarmPage;
        private LivePreviewPanel _preview;
        private FlowLayoutPanel _searchBar;
        private Label _lblTimeout;
        private TableLayoutPanel _fields;
        private Label _lblPort;
        private Label _lblUser;
        private Label _lblPassword;
        private Label _lblManualIp;
        private Label _lblHint;
        private FlowLayoutPanel _actions;
        private Panel _side;
        private FlowLayoutPanel _alarmBar;
        private Label _lblAlarmTitle;
        private Panel _alarmHost;
        private Panel _logHost;
        private Label _lblLog;
        private TableLayoutPanel _bottom;
        private Panel _body;
    }
}
