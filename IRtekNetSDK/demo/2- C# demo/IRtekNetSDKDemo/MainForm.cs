using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IRtekNetSDK;

namespace IRtekNetSDKDemo
{
    public partial class MainForm : Form
    {
        public enum MarkerType
        {
            None,
            Spot,
            Line,
            Rectangle,
            Ellipse,
            Polygon,
            Delta,
        }

        private enum TrackerHitType
        {
            hitNothing = -1,
            hitTopLeft = 0,
            hitTopRight = 1,
            hitBottomRight = 2,
            hitBottomLeft = 3,
            hitTop = 4,
            hitRight = 5,
            hitBottom = 6,
            hitLeft = 7,
            hitMiddle = 8,
            hitEdge = 9,
            hitStart = 10,
            hitEnd = 11
        };

        private class MarkerTracker
        {
            public MarkerTracker()
            {
                Reset();
            }

            public void Reset()
            {
                IsTracking = false;
                Name = string.Empty;
                MarkerType = MarkerType.None;
                HitType = TrackerHitType.hitNothing;
                PrevPos = new Point(0, 0);
                HitPos = new Point(0, 0);
            }

            public bool IsTracking { get; set; }
            public string Name { get; set; }
            public MarkerType MarkerType { get; set; }
            public TrackerHitType HitType { get; set; }
            public Point PrevPos { get; set; }
            public Point HitPos { get; set; }
        }

        private bool IsGUIRefreshing { get; set; }
        private MarkerTracker Tracker { get; set; }
        private bool Adding { get; set; }
        private bool Deleting { get; set; }
        private MarkerType CurrentMarkerType { get; set; }

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr CreateFileMapping(
            IntPtr hFile, IntPtr lpAttributes, uint flProtect,
            uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject, uint dwDesiredAccess,
            uint dwFileOffsetHigh, uint dwFileOffsetLow, UIntPtr dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        private const uint PAGE_READWRITE = 0x04;
        private const uint FILE_MAP_READ = 0x0004;
        private const uint WAIT_OBJECT_0 = 0;
        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

        private bool _sdkLogConsoleAttached;
        private Thread _sdkLogOdsThread;
        private volatile bool _sdkLogOdsRunning;
        private IntPtr _dbWinBufferMapping = IntPtr.Zero;
        private IntPtr _dbWinBufferView = IntPtr.Zero;
        private IntPtr _dbWinBufferReady = IntPtr.Zero;
        private IntPtr _dbWinDataReady = IntPtr.Zero;

        // NetSDK fields
        private IntPtr _netHandle = IntPtr.Zero;
        private bool _netConnected;
        private bool _loadingTimeUi;
        private uint _displayChannelId;
        private NetSdkNative.OnNetStreamReceivedCallback _streamCb;
        private NetSdkNative.OnNetExceptionCallback _exceptionCb;
        private NetSdkNative.OnNetAlarmCallback _alarmCb;
        private IntPtr _streamPlayHandle = IntPtr.Zero;
        private IntPtr _thermalDataStreamHandle = IntPtr.Zero;
        private IntPtr _alarmListenerHandle = IntPtr.Zero;
        private int _reconnecting;
        private readonly List<Delegate> _staleCallbacks = new List<Delegate>();
        private NetSdkNative.NetStreamType _currentStreamType = NetSdkNative.NetStreamType.BGRA;
        private bool _updatingStreamTypeCombo;
        private uint _currentStreamId;
        private bool _updatingStreamIdCombo;
        private volatile int _pendingStreamDataLogCount;
        private bool _streamStatsOnlyMode;
        private int _streamStatsFrameCount;
        private bool _updatingStreamCallbackToggle;
        private readonly object _h264DecoderLock = new object();
        private FFmpegDecoder _h264Decoder;

        // Marker state
        private NetSdkNative.NetMarkerInfo[] _markers = new NetSdkNative.NetMarkerInfo[NetSdkNative.NET_MAX_MARKERS];
        private ushort _markerCount;
        private NetSdkNative.NetMarkerTempInfo[] _markerTemps = new NetSdkNative.NetMarkerTempInfo[NetSdkNative.NET_MAX_MARKERS];
        private ushort _markerTempsCount;
        private NetSdkNative.NetTempPointInfo _globalTempInfo;
        private bool _hasGlobalTemp;
        private readonly object _markerLock = new object();
        private const string GlobalMarkerListName = "global";
        private const int GlobalMarkerListTag = -1;
        // Device typically allows 10 markers; SDK buffer is NET_MAX_MARKERS (16).
        private const int DeviceMaxMarkers = 10;
        // 1 global + 10 markers, no vertical scroll needed for the usual full set
        private const int MarkerListVisibleRowCount = 11;
        private int _streamWidth = 640;
        private int _streamHeight = 480;
        private int _spotCounter;
        private int _areaCounter;
        private int _lineCounter;
        private readonly List<Point> _pendingPolygonPoints = new List<Point>();
        private Point _pendingPolygonCursor;
        private string _lastMarkerListHitName;
        private NetSdkNative.NetTempUnit _activeTempUnit = NetSdkNative.NetTempUnit.Centigrade;
        private NetSdkNative.NetDistanceUnit _activeDistUnit = NetSdkNative.NetDistanceUnit.Meter;
        private NetSdkNative.OnNetThermalDataStreamReceivedCallback _thermalDataCb;
        private NetSdkNative.NetMarkerAlarmInfo[] _markerAlarms = new NetSdkNative.NetMarkerAlarmInfo[NetSdkNative.NET_MAX_MARKERS];
        private const uint ThermalStreamSampleIntervalMs = 100;
        private const int ThermalStreamStartupLogFrameCount = 2;
        private int _thermalStartupLogRemaining;
        private bool _updatingThermalStreamConfigCombo;
        private ThermalStreamConfigPreset _selectedThermalStreamPreset = ThermalStreamConfigPreset.DefaultGlobalMarker;

        // Alarm state
        private NetSdkNative.NetThermalTempAlarm[] _alarms = new NetSdkNative.NetThermalTempAlarm[NetSdkNative.NET_MAX_MARKERS];
        private ushort _alarmCount;
        private bool _updatingAlarmUi;

        // DPC state
        private readonly List<NetSdkNative.NetPoint> _dpcPoints = new List<NetSdkNative.NetPoint>();
        private int _dpcSubmittedCount;

        // Webhook / alarm listener UI (created in InitializeExtendedUi)
        private TextBox tbWebhookLatest;
        private TextBox[] tbWebhookUrls;
        private Button btnWebhookGet;
        private Button btnWebhookSet;
        private Button btnWebhooksGet;
        private Button btnWebhooksSet;
        private Button btnAlarmListenerStart;
        private Button btnAlarmListenerStop;
        private CheckBox cbAlarmWebhookEnabled;

        public struct PresetPaletteItem
        {
            public uint RefNo { get; set; }
            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                if (!(obj is PresetPaletteItem))
                {
                    return false;
                }

                return RefNo == ((PresetPaletteItem)obj).RefNo;
            }

            public override int GetHashCode()
            {
                return RefNo.GetHashCode();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private sealed class StreamIdItem
        {
            public StreamIdItem(uint id, ushort width, ushort height)
            {
                Id = id;
                Display = string.Format("{0} ({1}x{2})", id, width, height);
            }

            public uint Id { get; }
            public string Display { get; }

            public override string ToString()
            {
                return Display;
            }
        }

        private enum ThermalStreamConfigPreset
        {
            DefaultGlobalMarker = 0,
            AllTypes = 1
        }

        private sealed class ThermalStreamConfigItem
        {
            public ThermalStreamConfigItem(ThermalStreamConfigPreset preset, string display)
            {
                Preset = preset;
                Display = display;
            }

            public ThermalStreamConfigPreset Preset { get; }
            public string Display { get; }

            public override string ToString()
            {
                return Display;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            thermalOsdPanel.GetClicked += (s, e) => LoadThermalOsdSettings();
            thermalOsdPanel.SetClicked += (s, e) => SaveThermalOsdSettings();

            var insetRenderer = new InsetToolStripRenderer();
            tsConnection.Renderer = insetRenderer;
            tsMainForm.Renderer = insetRenderer;

            SetStreamTypeCombo(_currentStreamType);

            // Hide controls not supported by NetSDK thermal_params

            // Range list will be loaded from device after connection

            // Palette list will be loaded from device after connection

            tscbColorDistributionType.Items.Add(NetSdkNative.NetColorDistType.TWB);
            tscbColorDistributionType.Items.Add(NetSdkNative.NetColorDistType.TempLinear);
            tscbColorDistributionType.Items.Add(NetSdkNative.NetColorDistType.ADLinear);
            tscbColorDistributionType.SelectedIndex = 0;
            if (tscbImageFlip.Items.Count > 0)
                tscbImageFlip.SelectedIndex = 0;

            foreach (NetSdkNative.NetThermalTempAlarmType alarmType in Enum.GetValues(typeof(NetSdkNative.NetThermalTempAlarmType)))
                cbAlarmType.Items.Add(alarmType);

            for (int i = 0; i < _alarms.Length; i++)
                _alarms[i] = NetSdkNative.NetThermalTempAlarm.Create();

            for (int i = 0; i < _markerAlarms.Length; i++)
            {
                _markerAlarms[i].marker_name = new byte[NetSdkNative.NET_LEN_64];
                _markerAlarms[i].alarm_level = NetSdkNative.NetThermalTempAlarmLevel.None;
            }

            _updatingThermalStreamConfigCombo = true;
            try
            {
                tscbThermalStreamConfig.Items.Add(new ThermalStreamConfigItem(
                    ThermalStreamConfigPreset.DefaultGlobalMarker, "dt=6 (G|M)"));
                tscbThermalStreamConfig.Items.Add(new ThermalStreamConfigItem(
                    ThermalStreamConfigPreset.AllTypes, "dt=7 (Raw|G|M)"));
                tscbThermalStreamConfig.SelectedIndex = 0;
            }
            finally
            {
                _updatingThermalStreamConfigCombo = false;
            }

            InitializeExtendedUi();
            AssignMarkerToolIcons();

            IsGUIRefreshing = false;
            Tracker = new MarkerTracker();
            Adding = false;
            Deleting = false;
            CurrentMarkerType = MarkerType.None;

            var sdkVer = new NetSdkNative.NetSdkVersion();
            NetSdkNative.net_get_sdk_version(ref sdkVer);
            Text = $"IRtekNetSDKDemo v{sdkVer.major}.{sdkVer.minor}.{sdkVer.patch}.{sdkVer.build}";

            UpdateStreamCallbackButtonUi();
            EnsureAcousticUi();
            Load += MainForm_LoadAdjustMarkerListHeight;
        }

        private void MainForm_LoadAdjustMarkerListHeight(object sender, EventArgs e)
        {
            Load -= MainForm_LoadAdjustMarkerListHeight;
            AdjustMarkerListHeightForVisibleRows(MarkerListVisibleRowCount);
        }

        /// <summary>
        /// Size mlvMarkers to fit exactly <paramref name="rowCount"/> detail rows (plus header),
        /// shrinking the thermal preview area by the same delta.
        /// </summary>
        private void AdjustMarkerListHeightForVisibleRows(int rowCount)
        {
            if (rowCount <= 0 || !mlvMarkers.IsHandleCreated)
                return;

            bool addedTemp = false;
            if (mlvMarkers.Items.Count == 0)
            {
                mlvMarkers.Items.Add(string.Empty);
                addedTemp = true;
            }

            try
            {
                Rectangle itemRect = mlvMarkers.GetItemRect(0);
                int headerHeight = Math.Max(0, itemRect.Top);
                int itemHeight = itemRect.Height;
                if (itemHeight <= 0)
                    return;

                int border = mlvMarkers.Height - mlvMarkers.ClientSize.Height;
                int columnsWidth = 0;
                foreach (ColumnHeader column in mlvMarkers.Columns)
                    columnsWidth += column.Width;
                int hScroll = columnsWidth > mlvMarkers.ClientSize.Width
                    ? SystemInformation.HorizontalScrollBarHeight
                    : 0;

                int targetHeight = headerHeight + itemHeight * rowCount + border + hScroll;
                int delta = targetHeight - mlvMarkers.Height;
                if (delta == 0)
                    return;

                const int minPreviewHeight = 200;
                if (pbThermalImage.Height - delta < minPreviewHeight)
                    delta = pbThermalImage.Height - minPreviewHeight;
                if (delta == 0)
                    return;

                SuspendLayout();
                try
                {
                    // Bottom-anchored: keep bottom edge, grow/shrink upward
                    mlvMarkers.Top -= delta;
                    mlvMarkers.Height += delta;

                    pbThermalImage.Height -= delta;
                    pbPalleteImage.Height -= delta;
                    nudMin.Top -= delta;
                    lblMinUnit.Top -= delta;
                    btnAutoAdjust.Top -= delta;
                }
                finally
                {
                    ResumeLayout(true);
                }
            }
            finally
            {
                if (addedTemp)
                    mlvMarkers.Items.Clear();
            }
        }

        private void InitializeExtendedUi()
        {
            tpNetwork.AutoScroll = true;
            tpAlarm.AutoScroll = true;
            tpDpc.AutoScroll = true;

            tbWebhookUrls = new TextBox[NetSdkNative.NET_MAX_WEBHOOKS];
            int webhookTop = 500;
            var lblWebhookLatest = new Label
            {
                Text = "Latest Webhook",
                AutoSize = true,
                Location = new Point(15, webhookTop)
            };
            tbWebhookLatest = new TextBox
            {
                Location = new Point(110, webhookTop - 3),
                Size = new Size(360, 21)
            };
            tpNetwork.Controls.Add(lblWebhookLatest);
            tpNetwork.Controls.Add(tbWebhookLatest);
            webhookTop += 28;
            btnWebhookGet = new Button
            {
                Text = "Get Webhook",
                Location = new Point(110, webhookTop),
                Size = new Size(110, 25)
            };
            btnWebhookGet.Click += btnWebhookGet_Click;
            btnWebhookSet = new Button
            {
                Text = "Set Webhook",
                Location = new Point(230, webhookTop),
                Size = new Size(110, 25)
            };
            btnWebhookSet.Click += btnWebhookSet_Click;
            tpNetwork.Controls.Add(btnWebhookGet);
            tpNetwork.Controls.Add(btnWebhookSet);
            webhookTop += 32;
            for (int i = 0; i < tbWebhookUrls.Length; i++)
            {
                var lblWebhook = new Label
                {
                    Text = "Webhook " + (i + 1),
                    AutoSize = true,
                    Location = new Point(15, webhookTop)
                };
                tbWebhookUrls[i] = new TextBox
                {
                    Location = new Point(110, webhookTop - 3),
                    Size = new Size(360, 21)
                };
                tpNetwork.Controls.Add(lblWebhook);
                tpNetwork.Controls.Add(tbWebhookUrls[i]);
                webhookTop += 28;
            }
            btnWebhooksGet = new Button
            {
                Text = "Get Webhooks",
                Location = new Point(110, webhookTop),
                Size = new Size(110, 25)
            };
            btnWebhooksGet.Click += btnWebhooksGet_Click;
            btnWebhooksSet = new Button
            {
                Text = "Set Webhooks",
                Location = new Point(230, webhookTop),
                Size = new Size(110, 25)
            };
            btnWebhooksSet.Click += btnWebhooksSet_Click;
            tpNetwork.Controls.Add(btnWebhooksGet);
            tpNetwork.Controls.Add(btnWebhooksSet);

            cbAlarmWebhookEnabled = new CheckBox
            {
                Text = "Webhook Trigger",
                AutoSize = true,
                Location = new Point(120, 420)
            };

            var lblAlarmListenerHint = new Label
            {
                Text = "Alarm listener uses the current login session.",
                AutoSize = true,
                Location = new Point(9, 480)
            };
            btnAlarmListenerStart = new Button
            {
                Text = "Start Listener",
                Location = new Point(9, 505),
                Size = new Size(100, 25)
            };
            btnAlarmListenerStart.Click += btnAlarmListenerStart_Click;
            btnAlarmListenerStop = new Button
            {
                Text = "Stop Listener",
                Location = new Point(115, 505),
                Size = new Size(100, 25),
                Enabled = false
            };
            btnAlarmListenerStop.Click += btnAlarmListenerStop_Click;

            btnAlarmGet.Location = new Point(6, 445);
            btnAlarmSet.Location = new Point(96, 445);
            btnAlarmGet.Text = "Get Rules";
            btnAlarmSet.Text = "Set Rules";

            tpAlarm.Controls.Add(cbAlarmWebhookEnabled);
            tpAlarm.Controls.Add(lblAlarmListenerHint);
            tpAlarm.Controls.Add(btnAlarmListenerStart);
            tpAlarm.Controls.Add(btnAlarmListenerStop);
        }

        private void btnWebhookGet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadWebhook(true);
        }

        private void btnWebhookSet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            string error;
            NetSdkNative.NetWebhookInfo info;
            if (!TryBuildWebhook(tbWebhookLatest.Text, out info, out error))
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ret = NetSdkNative.net_set_webhook(_netHandle, ref info);
            LogApi("net_set_webhook", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("Webhook updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Set webhook failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnWebhooksGet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadWebhooks(true);
        }

        private void btnWebhooksSet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            var urls = new string[tbWebhookUrls.Length];
            for (int i = 0; i < tbWebhookUrls.Length; i++)
                urls[i] = tbWebhookUrls[i].Text;

            string error;
            NetSdkNative.NetWebhookInfo[] list;
            if (!TryBuildWebhookList(urls, out list, out error))
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ushort length = (ushort)(list == null ? 0 : list.Length);
            int ret = NetSdkNative.net_set_webhooks(_netHandle, list, length);
            LogApi("net_set_webhooks", ret, "count=" + length);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show(length == 0 ? "Webhook list cleared" : "Webhook list updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Set webhooks failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void LoadWebhook(bool notify)
        {
            var latest = NetSdkNative.NetWebhookInfo.Create();
            int ret = NetSdkNative.net_get_webhook(_netHandle, ref latest);
            string url = NetSdkNative.IsOk(ret) ? NetSdkNative.BytesToString(latest.url) : string.Empty;
            LogApi("net_get_webhook", ret, string.IsNullOrEmpty(url) ? "url empty" : url);
            if (NetSdkNative.IsOk(ret))
                tbWebhookLatest.Text = url;
            else if (notify)
                MessageBox.Show("Get webhook failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void LoadWebhooks(bool notify)
        {
            var list = new NetSdkNative.NetWebhookInfo[NetSdkNative.NET_MAX_WEBHOOKS];
            for (int i = 0; i < list.Length; i++)
                list[i] = NetSdkNative.NetWebhookInfo.Create();

            ushort outLen;
            int ret = NetSdkNative.net_get_webhooks(_netHandle, list, (ushort)list.Length, out outLen);
            LogApi("net_get_webhooks", ret, "count=" + outLen);
            if (NetSdkNative.IsOk(ret))
            {
                int count = Math.Min(outLen, tbWebhookUrls.Length);
                for (int i = 0; i < tbWebhookUrls.Length; i++)
                    tbWebhookUrls[i].Text = i < count ? NetSdkNative.BytesToString(list[i].url) : string.Empty;
            }
            else if (notify)
            {
                MessageBox.Show("Get webhooks failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static bool TryBuildWebhookList(string[] urls, out NetSdkNative.NetWebhookInfo[] list, out string error)
        {
            var items = new List<NetSdkNative.NetWebhookInfo>();
            list = null;
            error = null;
            if (urls == null)
                return true;

            foreach (string raw in urls)
            {
                string url = (raw ?? string.Empty).Trim();
                if (url.Length == 0)
                    continue;

                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    error = "Webhook URL must start with http:// or https://: " + url;
                    return false;
                }

                int byteLength = Encoding.UTF8.GetByteCount(url);
                if (byteLength >= NetSdkNative.NET_LEN_256)
                {
                    error = "Webhook URL exceeds " + (NetSdkNative.NET_LEN_256 - 1) + " bytes: " + url;
                    return false;
                }

                var info = NetSdkNative.NetWebhookInfo.Create();
                info.url = NetSdkNative.StringToFixedBytes(url, NetSdkNative.NET_LEN_256);
                items.Add(info);
            }

            list = items.Count == 0 ? null : items.ToArray();
            return true;
        }

        private static bool TryBuildWebhook(string raw, out NetSdkNative.NetWebhookInfo info, out string error)
        {
            info = NetSdkNative.NetWebhookInfo.Create();
            error = null;
            string url = (raw ?? string.Empty).Trim();
            if (url.Length == 0)
            {
                error = "Webhook URL is empty";
                return false;
            }

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                error = "Webhook URL must start with http:// or https://: " + url;
                return false;
            }

            int byteLength = Encoding.UTF8.GetByteCount(url);
            if (byteLength >= NetSdkNative.NET_LEN_256)
            {
                error = "Webhook URL exceeds " + (NetSdkNative.NET_LEN_256 - 1) + " bytes: " + url;
                return false;
            }

            info.url = NetSdkNative.StringToFixedBytes(url, NetSdkNative.NET_LEN_256);
            return true;
        }

        private void btnAlarmListenerStart_Click(object sender, EventArgs e)
        {
            if (!_netConnected || _netHandle == IntPtr.Zero)
            {
                MessageBox.Show("请先登录设备后再启动报警监听。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_alarmListenerHandle != IntPtr.Zero)
            {
                MessageBox.Show("Alarm listener is already running.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _alarmCb = OnNetAlarmReceived;
            int ret = NetSdkNative.net_start_alarm_listener(_netHandle, _alarmCb);
            LogApi("net_start_alarm_listener", ret);
            if (!NetSdkNative.TryGetSdkHandle(ret, out _alarmListenerHandle))
            {
                _alarmCb = null;
                string err = ret + " (" + (NetSdkNative.NetErrorCode)ret + ")";
                MessageBox.Show(
                    "启动报警监听失败: " + err + Environment.NewLine + Environment.NewLine
                    + "请确认设备已登录且当前账号具备报警监听权限。" + Environment.NewLine
                    + "临时方案：可在 Network 页配置 Webhook 接收报警。",
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SyncAlarmListenerButtons();
            LogApiCall("Alarm listener started (handle=" + _alarmListenerHandle.ToInt32()
                + "). Waiting for device alarm push; Alarm column updates only when callback fires.");
            LogApiCall("Tip: enable toolbar Logging to see native [alarm decode] lines if no [Alarm] appears.");
        }

        private void btnAlarmListenerStop_Click(object sender, EventArgs e)
        {
            StopAlarmListener();
        }

        private void StopAlarmListener()
        {
            if (_alarmListenerHandle == IntPtr.Zero)
                return;

            int ret = NetSdkNative.net_stop_alarm_listener(_alarmListenerHandle);
            LogApi("net_stop_alarm_listener", ret);
            _alarmListenerHandle = IntPtr.Zero;
            _alarmCb = null;
            SyncAlarmListenerButtons();
        }

        private void SyncAlarmListenerButtons()
        {
            bool running = _alarmListenerHandle != IntPtr.Zero;
            btnAlarmListenerStart.Enabled = !running;
            btnAlarmListenerStop.Enabled = running;
            if (btnAcAlarmListenerStart == null)
                return;
            btnAcAlarmListenerStart.Enabled = !running;
            btnAcAlarmListenerStop.Enabled = running;
        }

        private void OnNetAlarmReceived(IntPtr alarmInfoPtr)
        {
            try
            {
                if (!NetSdkNative.TryReadAlarmInfo(alarmInfoPtr, out var alarmInfo))
                {
                    PostAlarmLog("[Alarm] callback with null alarm_info pointer");
                    return;
                }

                PostAlarmLog("[Alarm] callback type=" + alarmInfo.type
                    + " id=" + alarmInfo.alarm_id
                    + " device=" + NetSdkNative.BytesToString(alarmInfo.alarmer.device_ip)
                    + ":" + alarmInfo.alarmer.link_port
                    + " sn=" + NetSdkNative.BytesToString(alarmInfo.alarmer.serial_no)
                    + " info_len=" + alarmInfo.alarm_info_length
                    + " info_ptr=" + alarmInfo.alarm_info);

                if (alarmInfo.type == NetSdkNative.NetAlarmType.AcousticSource
                || alarmInfo.type == NetSdkNative.NetAlarmType.AcousticDb
                || alarmInfo.type == NetSdkNative.NetAlarmType.AcousticFreq)
            {
                HandleAcousticAlarm(alarmInfo);
                return;
            }

            if (alarmInfo.type == NetSdkNative.NetAlarmType.Intrusion
                || alarmInfo.type == NetSdkNative.NetAlarmType.HighTemp
                || alarmInfo.type == NetSdkNative.NetAlarmType.TempDetect)
            {
                PostAlarmLog("[Alarm] " + FormatExtendedAlarm(alarmInfo));
                return;
            }

            if (alarmInfo.type != NetSdkNative.NetAlarmType.ThermalTemp)
                    return;

                if (alarmInfo.alarm_info == IntPtr.Zero)
                {
                    PostAlarmLog("[Alarm] thermal payload pointer is null");
                    return;
                }

                var thermalAlarm = NetSdkNative.ParseAlarmThermalTempInfo(alarmInfo.alarm_info);
                ApplyMarkerAlarmsFromListener(thermalAlarm);

                string message = NetSdkNative.ReadUtf8(thermalAlarm.message, thermalAlarm.message_length);

                string summary = BuildAlarmListenerSummary(thermalAlarm, message);
                PostAlarmLog("[Alarm] " + summary);
            }
            catch (Exception ex)
            {
                PostAlarmLog("[Alarm] callback exception: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private static string FormatExtendedAlarm(NetSdkNative.NetAlarmInfo alarmInfo)
        {
            if (alarmInfo.alarm_info == IntPtr.Zero)
                return alarmInfo.type + " id=" + alarmInfo.alarm_id;

            if (alarmInfo.type == NetSdkNative.NetAlarmType.Intrusion)
            {
                var info = (NetSdkNative.NetAlarmIntrusionInfo)Marshal.PtrToStructure(
                    alarmInfo.alarm_info, typeof(NetSdkNative.NetAlarmIntrusionInfo));
                return "Intrusion id=" + alarmInfo.alarm_id
                    + " motion=" + info.motion_level
                    + " " + NetSdkNative.ReadUtf8(info.message, info.message_length);
            }

            if (alarmInfo.type == NetSdkNative.NetAlarmType.HighTemp)
            {
                var info = (NetSdkNative.NetAlarmHighTempInfo)Marshal.PtrToStructure(
                    alarmInfo.alarm_info, typeof(NetSdkNative.NetAlarmHighTempInfo));
                return "HighTemp id=" + alarmInfo.alarm_id
                    + " temp=" + info.temperature.ToString("F1")
                    + " threshold=" + info.threshold.ToString("F1")
                    + " " + NetSdkNative.ReadUtf8(info.message, info.message_length);
            }

            var body = (NetSdkNative.NetAlarmTempDetectInfo)Marshal.PtrToStructure(
                alarmInfo.alarm_info, typeof(NetSdkNative.NetAlarmTempDetectInfo));
            return "TempDetect id=" + alarmInfo.alarm_id
                + " temp=" + body.temperature.ToString("F1")
                + " baseline=" + body.baseline.ToString("F1")
                + " slope=" + body.slope.ToString("F2")
                + " " + NetSdkNative.ReadUtf8(body.message, body.message_length);
        }

        private void PostAlarmLog(string text)
        {
            try
            {
                if (IsDisposed || !IsHandleCreated)
                    return;

                BeginInvoke(new Action(() => LogApiCall(text)));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Alarm] PostAlarmLog failed: " + ex.Message);
            }
        }

        private void ApplyMarkerAlarmsFromListener(NetSdkNative.NetAlarmThermalTempInfo thermalAlarm)
        {
            lock (_markerLock)
            {
                for (int i = 0; i < _markerAlarms.Length; i++)
                {
                    if (_markerAlarms[i].marker_name == null)
                        _markerAlarms[i].marker_name = new byte[NetSdkNative.NET_LEN_64];
                    else
                        Array.Clear(_markerAlarms[i].marker_name, 0, _markerAlarms[i].marker_name.Length);
                    _markerAlarms[i].alarm_level = NetSdkNative.NetThermalTempAlarmLevel.None;
                }

                if (thermalAlarm.marker_alarms != null)
                {
                    int count = Math.Min((int)thermalAlarm.marker_alarms_count, NetSdkNative.NET_MAX_MARKERS);
                    for (int i = 0; i < count; i++)
                    {
                        var src = thermalAlarm.marker_alarms[i];
                        _markerAlarms[i].alarm_level = src.alarm_level;
                        if (src.marker_name != null)
                        {
                            Buffer.BlockCopy(
                                src.marker_name, 0,
                                _markerAlarms[i].marker_name, 0,
                                Math.Min(src.marker_name.Length, _markerAlarms[i].marker_name.Length));
                        }
                    }
                }

                if (thermalAlarm.marker_temps != null && thermalAlarm.markers_count > 0)
                {
                    _markerTempsCount = thermalAlarm.markers_count;
                    int count = Math.Min((int)thermalAlarm.markers_count, NetSdkNative.NET_MAX_MARKERS);
                    for (int i = 0; i < count; i++)
                        _markerTemps[i] = thermalAlarm.marker_temps[i];
                }
            }

            try
            {
                if (IsDisposed || !IsHandleCreated)
                    return;

                BeginInvoke(new Action(() =>
                {
                    if (!_acousticActive && chExInfo.Width < 80)
                        chExInfo.Width = 80;
                    RefreshMarkerListView();
                }));
            }
            catch
            {
            }
        }

        private string BuildAlarmListenerSummary(NetSdkNative.NetAlarmThermalTempInfo thermalAlarm, string message)
        {
            var parts = new List<string>();
            parts.Add("ts=" + thermalAlarm.timestamp_us);

            string deviceIp = NetSdkNative.BytesToString(thermalAlarm.dev_info.ip);
            if (!string.IsNullOrEmpty(deviceIp) || thermalAlarm.dev_info.port_no != 0 || thermalAlarm.dev_info.channel_id != 0)
                parts.Add(string.Format("dev={0}:{1}/ch{2}", deviceIp, thermalAlarm.dev_info.port_no, thermalAlarm.dev_info.channel_id));

            if (!string.IsNullOrEmpty(message))
                parts.Add("msg=" + message);

            if (thermalAlarm.marker_alarms != null)
            {
                var activeAlarms = new List<string>();
                int count = Math.Min((int)thermalAlarm.marker_alarms_count, NetSdkNative.NET_MAX_MARKERS);
                for (int i = 0; i < count; i++)
                {
                    var alarm = thermalAlarm.marker_alarms[i];
                    if (alarm.alarm_level == NetSdkNative.NetThermalTempAlarmLevel.None)
                        continue;

                    string markerName = NetSdkNative.BytesToString(alarm.marker_name);
                    if (string.IsNullOrEmpty(markerName))
                        markerName = "marker#" + (i + 1);
                    activeAlarms.Add(markerName + ":" + FormatThermalAlarmLevel(alarm.alarm_level));
                }

                parts.Add("alarms=" + (activeAlarms.Count > 0 ? string.Join(",", activeAlarms.ToArray()) : "none")
                    + "(" + thermalAlarm.marker_alarms_count + ")");
            }

            return string.Join(" ", parts.ToArray());
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void DoSave()
        {
            if (!_netConnected)
                return;

            if (_acousticActive)
            {
                DoAcousticCapture();
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "IRTEK Thermal Image (*.jpg)|*.jpg|JPEG Image (*.jpg)|*.jpg";
                sfd.DefaultExt = "jpg";
                sfd.FilterIndex = 1;
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    Cursor = Cursors.WaitCursor;

                    var captureFile = NetSdkNative.NetCaptureFile.Create();
                    byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(sfd.FileName);
                    Array.Copy(pathBytes, captureFile.local_file,
                        Math.Min(pathBytes.Length, NetSdkNative.NET_LEN_128 - 1));

                    var captureMode = sfd.FilterIndex == 1
                        ? NetSdkNative.NetThermalCaptureMode.IrtekImage
                        : NetSdkNative.NetThermalCaptureMode.PureImage;

                    int ret = NetSdkNative.net_get_thermal_capture(
                        _netHandle,
                        captureMode,
                        ref captureFile);
                    LogApi("net_get_thermal_capture", ret);

                    if (NetSdkNative.IsOk(ret))
                    {
                        MessageBox.Show($"Saved: {sfd.FileName}", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Capture failed: {ret} ({(NetSdkNative.NetErrorCode)ret})",
                            Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void tsbGetThermalData_Click(object sender, EventArgs e)
        {
            DoGetThermalData();
        }

        private void tsbGetThermalLut_Click(object sender, EventArgs e)
        {
            DoGetThermalLut();
        }

        private void DoGetThermalLut()
        {
            if (!_netConnected)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var lut = NetSdkNative.NetLutTable.Create();
                int ret = NetSdkNative.net_get_thermal_lut(_netHandle, ref lut);
                var snapshot = ThermalLutSnapshot.Capture(lut);
                LogApi("net_get_thermal_lut", ret, snapshot.BuildSummary());

                if (!NetSdkNative.IsOk(ret))
                {
                    MessageBox.Show("Get thermal LUT failed: " + ret + " (" + (NetSdkNative.NetErrorCode)ret + ")",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ExportThermalLutSnapshot(snapshot);
            }
            catch (Exception ex)
            {
                AppendApiLog("thermal_lut export error: " + ex.Message);
                MessageBox.Show("Export thermal LUT failed: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ExportThermalLutSnapshot(ThermalLutSnapshot snapshot)
        {
            foreach (string line in snapshot.BuildLogLines())
                AppendApiLog(line);

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON files (*.json)|*.json";
                sfd.DefaultExt = "json";
                sfd.FileName = "thermal_lut_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                snapshot.WriteJson(sfd.FileName);
                AppendApiLog("thermal_lut exported: " + sfd.FileName);
                MessageBox.Show("Thermal LUT exported:\n" + sfd.FileName,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DoGetThermalData()
        {
            if (!_netConnected)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var thermalData = CreateThermalDataBuffer();
                var dataTypes = GetAllThermalDataTypes();
                int ret = NetSdkNative.net_get_thermal_data(_netHandle, (uint)dataTypes, ref thermalData);
                LogApi(
                    "net_get_thermal_data",
                    ret,
                    "request=" + FormatThermalDataTypes((NetSdkNative.NetThermalDataType)dataTypes)
                        + " " + BuildThermalDataSummary(thermalData));

                if (!NetSdkNative.IsOk(ret))
                {
                    MessageBox.Show("Get thermal data failed: " + ret + " (" + (NetSdkNative.NetErrorCode)ret + ")",
                        Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LogRawFrameProbe(thermalData);
                ApplyThermalData(thermalData);
                ExportThermalDataSnapshot(thermalData, (NetSdkNative.NetThermalDataType)dataTypes);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ExportThermalDataSnapshot(NetSdkNative.NetThermalData thermalData, NetSdkNative.NetThermalDataType requestedTypes)
        {
            var snapshot = ThermalDataSnapshot.Capture(thermalData, requestedTypes);

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON files (*.json)|*.json";
                sfd.DefaultExt = "json";
                sfd.FileName = "thermal_data_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                snapshot.WriteJson(sfd.FileName);
                AppendApiLog("thermal_data exported: " + sfd.FileName);
                MessageBox.Show("Thermal data exported:\n" + sfd.FileName,
                    Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LogRawFrameProbe(NetSdkNative.NetThermalData thermalData)
        {
            var raw = thermalData.raw_frame;
            AppendApiLog("raw_frame probe: response_types=" + FormatThermalDataTypes(thermalData.data_types)
                + " width=" + raw.width
                + " height=" + raw.height
                + " points_count=" + raw.points_count
                + " lut_len=" + raw.lut_len
                + " data_ptr=" + raw.data.ToInt64()
                + " lut_temp_ptr=" + raw.lut_temp_value.ToInt64()
                + " lut_ad_ptr=" + raw.lut_ad_value.ToInt64());
        }

        private const int PolygonMinPoints = 3;
        private const int PolygonMaxPoints = 6;

        private void tsbSelect_Click(object sender, EventArgs e)
        {
            SetMarkerTool(MarkerType.None, false, false);
        }

        private void tsbSpot_Click(object sender, EventArgs e)
        {
            SetMarkerTool(MarkerType.Spot, true, false);
        }

        private void tsbRectangle_Click(object sender, EventArgs e)
        {
            SetMarkerTool(MarkerType.Rectangle, true, false);
        }

        private void tsbLine_Click(object sender, EventArgs e)
        {
            SetMarkerTool(MarkerType.Line, true, false);
        }

        private void tsbPolygon_Click(object sender, EventArgs e)
        {
            SetMarkerTool(MarkerType.Polygon, true, false);
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            SetMarkerTool(MarkerType.None, false, true);
        }

        private void SetMarkerTool(MarkerType type, bool adding, bool deleting)
        {
            CancelPendingPolygon();
            CurrentMarkerType = type;
            Adding = adding;
            Deleting = deleting;
            tsbSelect.Checked = type == MarkerType.None && !deleting;
            tsbSpot.Checked = type == MarkerType.Spot;
            tsbRectangle.Checked = type == MarkerType.Rectangle;
            tsbLine.Checked = type == MarkerType.Line;
            tsbPolygon.Checked = type == MarkerType.Polygon;
            tsbDelete.Checked = deleting;
            if (!adding && !deleting)
                Cursor = Cursors.Default;
        }

        private void AssignMarkerToolIcons()
        {
            tsbPolygon.Image = CreateMarkerToolIcon(g =>
            {
                var pts = new[]
                {
                    new Point(12, 2),
                    new Point(21, 9),
                    new Point(17, 21),
                    new Point(7, 21),
                    new Point(3, 9)
                };
                g.DrawPolygon(Pens.Black, pts);
            });
        }

        private static Image CreateMarkerToolIcon(Action<Graphics> draw)
        {
            var bmp = new Bitmap(24, 24, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                draw(g);
            }
            return bmp;
        }

        private int _markerResW = 320;
        private int _markerResH = 240;

        private bool TryGetImageDisplayRect(out double displayW, out double displayH, out double offsetX, out double offsetY)
        {
            displayW = displayH = offsetX = offsetY = 0;
            Rectangle clientRect = pbThermalImage.ClientRectangle;
            if (clientRect.Width == 0 || clientRect.Height == 0 || _markerResW <= 0 || _markerResH <= 0)
                return false;

            double imgAspect = (double)_markerResW / _markerResH;
            double ctrlAspect = (double)clientRect.Width / clientRect.Height;
            if (ctrlAspect > imgAspect)
            {
                displayH = clientRect.Height;
                displayW = displayH * imgAspect;
                offsetX = (clientRect.Width - displayW) / 2.0;
                offsetY = 0;
            }
            else
            {
                displayW = clientRect.Width;
                displayH = displayW / imgAspect;
                offsetX = 0;
                offsetY = (clientRect.Height - displayH) / 2.0;
            }
            return displayW > 0 && displayH > 0;
        }

        private Point ScreenToImage(Point screenPt)
        {
            if (!TryGetImageDisplayRect(out double displayW, out double displayH, out double offsetX, out double offsetY))
                return screenPt;

            int x = (int)((screenPt.X - offsetX) * _markerResW / displayW + 0.5);
            int y = (int)((screenPt.Y - offsetY) * _markerResH / displayH + 0.5);
            return new Point(
                Math.Max(0, Math.Min(_markerResW - 1, x)),
                Math.Max(0, Math.Min(_markerResH - 1, y)));
        }

        private Point ImageToScreen(Point imagePt)
        {
            if (!TryGetImageDisplayRect(out double displayW, out double displayH, out double offsetX, out double offsetY))
                return imagePt;

            int x = (int)(imagePt.X * displayW / _markerResW + offsetX + 0.5);
            int y = (int)(imagePt.Y * displayH / _markerResH + offsetY + 0.5);
            return new Point(x, y);
        }

        private void pbThermalImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_netConnected) return;

            Point point = ScreenToImage(e.Location);
            string name = string.Empty;
            MarkerType markerType = MarkerType.None;
            TrackerHitType hitType = TrackerHitType.hitNothing;

            if (IsDrawingVertices)
            {
                HandleVertexMouseDown(e.Button, point);
                return;
            }

            if (Deleting)
            {
                if (NetHitTest(point, ref name, ref markerType, ref hitType))
                {
                    lock (_markerLock)
                    {
                        int idx = FindMarkerByName(name);
                        if (idx >= 0)
                        {
                            for (int i = idx; i < _markerCount - 1; i++)
                                _markers[i] = _markers[i + 1];
                            _markerCount--;
                        }
                    }
                    PushMarkers();
                    RefreshMarkerListView();
                }

                SetMarkerTool(MarkerType.None, false, false);
                return;
            }

            if (Adding)
            {
                ushort markerCount;
                lock (_markerLock)
                    markerCount = _markerCount;
                if (markerCount >= DeviceMaxMarkers)
                {
                    MessageBox.Show("Maximum markers reached (" + DeviceMaxMarkers + ").", "Warning");
                    SetMarkerTool(MarkerType.None, false, false);
                    return;
                }

                NetSdkNative.NetPoint np = new NetSdkNative.NetPoint { x = (ushort)point.X, y = (ushort)point.Y };
                NetSdkNative.NetMarkerInfo newMarker;

                switch (CurrentMarkerType)
                {
                    case MarkerType.Spot:
                        newMarker = CreateMarkerInfo(NetSdkNative.NetMarkerType.Spot, NextSpotName(),
                            new NetSdkNative.NetPoint[] { np }, 1);
                        break;
                    case MarkerType.Rectangle:
                        newMarker = CreateMarkerInfo(NetSdkNative.NetMarkerType.Polygon, NextAreaName(),
                            CreateRectPolygonPoints(point, 30, 30), 4);
                        break;
                    case MarkerType.Line:
                        {
                            var np2 = new NetSdkNative.NetPoint
                            {
                                x = (ushort)Math.Min(_markerResW - 1, point.X + 40),
                                y = (ushort)Math.Min(_markerResH - 1, point.Y + 40)
                            };
                            newMarker = CreateMarkerInfo(NetSdkNative.NetMarkerType.Polyline, NextLineName(),
                                new NetSdkNative.NetPoint[] { np, np2 }, 2);
                            break;
                        }
                    default:
                        return;
                }

                lock (_markerLock)
                {
                    _markers[_markerCount] = newMarker;
                    _markerCount++;
                }
                PushMarkers();
                RefreshMarkerListView();
                SetMarkerTool(MarkerType.None, false, false);
                return;
            }

            if (NetHitTest(point, ref name, ref markerType, ref hitType))
            {
                Tracker.IsTracking = true;
                Tracker.Name = name;
                Tracker.MarkerType = markerType;
                Tracker.HitType = hitType;
                Tracker.PrevPos = point;
                Tracker.HitPos = point;
            }
        }

        private void pbThermalImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (Tracker.IsTracking)
            {
                PushMarkers();
                RefreshMarkerListView();
            }
            Tracker.Reset();
        }

        private void pbThermalImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_netConnected) return;

            Point point = ScreenToImage(e.Location);

            if (IsDrawingVertices)
            {
                Cursor = Cursors.Cross;
                _pendingPolygonCursor = point;
                pbThermalImage.Invalidate();
                return;
            }

            if (Deleting)
            {
                Cursor = Cursors.Cross;
            }
            else
            {
                if (!Tracker.IsTracking)
                {
                    string name = string.Empty;
                    MarkerType markerType = MarkerType.None;
                    TrackerHitType hitType = TrackerHitType.hitNothing;
                    NetHitTest(point, ref name, ref markerType, ref hitType);
                }
                else
                {
                    NetTrackSymbol(point);
                }
            }
        }

        private void pbThermalImage_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void pbThermalImage_Paint(object sender, PaintEventArgs e)
        {
            if (_pendingPolygonPoints.Count == 0)
                return;

            var screenPts = new Point[_pendingPolygonPoints.Count];
            for (int i = 0; i < _pendingPolygonPoints.Count; i++)
                screenPts[i] = ImageToScreen(_pendingPolygonPoints[i]);

            using (var pen = new Pen(Color.Lime, 1))
            using (var brush = new SolidBrush(Color.Lime))
            {
                if (screenPts.Length >= 2)
                    e.Graphics.DrawLines(pen, screenPts);

                Point cursor = ImageToScreen(_pendingPolygonCursor);
                e.Graphics.DrawLine(pen, screenPts[screenPts.Length - 1], cursor);
                if (CurrentMarkerType == MarkerType.Polygon && screenPts.Length >= 2)
                    e.Graphics.DrawLine(Pens.Yellow, cursor, screenPts[0]);

                for (int i = 0; i < screenPts.Length; i++)
                    e.Graphics.FillEllipse(brush, screenPts[i].X - 3, screenPts[i].Y - 3, 6, 6);
            }
        }

        private bool IsDrawingVertices
        {
            get { return Adding && CurrentMarkerType == MarkerType.Polygon; }
        }

        private int PendingVertexMinCount
        {
            get { return PolygonMinPoints; }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && IsDrawingVertices)
            {
                SetMarkerTool(MarkerType.None, false, false);
                e.Handled = true;
            }
        }

        private void HandleVertexMouseDown(MouseButtons button, Point point)
        {
            if (button == MouseButtons.Right)
            {
                if (_pendingPolygonPoints.Count >= PendingVertexMinCount)
                    CommitPendingVertices();
                else
                    SetMarkerTool(MarkerType.None, false, false);
                return;
            }

            if (button != MouseButtons.Left)
                return;

            if (_pendingPolygonPoints.Count == 0)
            {
                ushort markerCount;
                lock (_markerLock)
                    markerCount = _markerCount;
                if (markerCount >= DeviceMaxMarkers)
                {
                    MessageBox.Show("Maximum markers reached (" + DeviceMaxMarkers + ").", "Warning");
                    SetMarkerTool(MarkerType.None, false, false);
                    return;
                }
            }

            _pendingPolygonPoints.Add(point);
            _pendingPolygonCursor = point;
            pbThermalImage.Invalidate();

            if (_pendingPolygonPoints.Count >= PolygonMaxPoints)
                CommitPendingVertices();
        }

        private void CommitPendingVertices()
        {
            if (_pendingPolygonPoints.Count < PendingVertexMinCount)
            {
                CancelPendingPolygon();
                return;
            }

            var pts = new NetSdkNative.NetPoint[_pendingPolygonPoints.Count];
            for (int i = 0; i < _pendingPolygonPoints.Count; i++)
            {
                pts[i] = new NetSdkNative.NetPoint
                {
                    x = (ushort)_pendingPolygonPoints[i].X,
                    y = (ushort)_pendingPolygonPoints[i].Y
                };
            }

            var newMarker = CreateMarkerInfo(
                NetSdkNative.NetMarkerType.Polygon,
                NextAreaName(),
                pts,
                (ushort)pts.Length);

            lock (_markerLock)
            {
                _markers[_markerCount] = newMarker;
                _markerCount++;
            }
            PushMarkers();
            RefreshMarkerListView();
            SetMarkerTool(MarkerType.None, false, false);
        }

        private NetSdkNative.NetPoint[] CreateRectPolygonPoints(Point origin, int width, int height)
        {
            ushort x0 = (ushort)origin.X;
            ushort y0 = (ushort)origin.Y;
            ushort x1 = (ushort)Math.Min(_markerResW - 1, origin.X + width);
            ushort y1 = (ushort)Math.Min(_markerResH - 1, origin.Y + height);
            return new[]
            {
                new NetSdkNative.NetPoint { x = x0, y = y0 },
                new NetSdkNative.NetPoint { x = x1, y = y0 },
                new NetSdkNative.NetPoint { x = x1, y = y1 },
                new NetSdkNative.NetPoint { x = x0, y = y1 }
            };
        }

        private void CancelPendingPolygon()
        {
            if (_pendingPolygonPoints.Count == 0)
                return;
            _pendingPolygonPoints.Clear();
            if (pbThermalImage.IsHandleCreated)
                pbThermalImage.Invalidate();
        }

        private void cbFixedSpan_CheckedChanged(object sender, EventArgs e)
        {
            if (_acousticActive)
            {
                if (IsGUIRefreshing || !_netConnected)
                    return;
                UpdateAcousticScaleEditors();
                WriteAcousticScale(false);
                return;
            }
            nudSpan.Enabled = cbFixedSpan.Checked;
            if (IsGUIRefreshing || !_netConnected) return;
            ApplyGain();
        }

        private void nudMax_ValueChanged(object sender, EventArgs e)
        {
            if (_acousticActive)
            {
                if (IsGUIRefreshing || !_netConnected)
                    return;
                WriteAcousticScale(false);
                return;
            }
            if (IsGUIRefreshing || !_netConnected) return;
            ApplyGain();
        }

        private void nudMin_ValueChanged(object sender, EventArgs e)
        {
            if (_acousticActive)
            {
                if (IsGUIRefreshing || !_netConnected)
                    return;
                WriteAcousticScale(false);
                return;
            }
            if (IsGUIRefreshing || !_netConnected) return;
            ApplyGain();
        }

        private void nudSpan_ValueChanged(object sender, EventArgs e)
        {
            if (_acousticActive)
            {
                if (IsGUIRefreshing || !_netConnected)
                    return;
                WriteAcousticScale(false);
                return;
            }
            if (IsGUIRefreshing || !_netConnected) return;
            ApplyGain();
        }

        private void btnAutoAdjust_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (_acousticActive)
            {
                CycleAcousticMode();
                return;
            }

            var gain = new NetSdkNative.NetGain();
            int ret = NetSdkNative.net_get_thermal_gain(_netHandle, ref gain);
            LogApi("net_get_thermal_gain", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            gain.mode = gain.mode == NetSdkNative.NetGainMode.Auto
                ? NetSdkNative.NetGainMode.Fixed
                : NetSdkNative.NetGainMode.Auto;

            ret = NetSdkNative.net_set_thermal_gain(_netHandle, ref gain);
            LogApi("net_set_thermal_gain", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            LoadGain();
        }

        private void LoadGain()
        {
            var gain = new NetSdkNative.NetGain();
            int ret = NetSdkNative.net_get_thermal_gain(_netHandle, ref gain);
            LogApi("net_get_thermal_gain", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            LogGainValues(gain, "[Gain] loaded");

            IsGUIRefreshing = true;
            nudMax.Value = Clamp((decimal)gain.max, nudMax.Minimum, nudMax.Maximum);
            nudMin.Value = Clamp((decimal)gain.min, nudMin.Minimum, nudMin.Maximum);
            cbFixedSpan.Checked = gain.fixedSpan != 0;
            nudSpan.Value = Clamp((decimal)gain.span, nudSpan.Minimum, nudSpan.Maximum);
            nudSpan.Enabled = cbFixedSpan.Checked;
            btnAutoAdjust.Text = gain.mode == NetSdkNative.NetGainMode.Auto ? "Auto" : "Fixed";
            IsGUIRefreshing = false;
        }

        private void LogGainValues(NetSdkNative.NetGain gain, string prefix)
        {
            LogApiCall(string.Format(
                "{0} mode={1} max={2:F1} min={3:F1} fixedSpan={4} span={5:F1}",
                prefix,
                gain.mode,
                gain.max,
                gain.min,
                gain.fixedSpan,
                gain.span));
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void ApplyGain()
        {
            var gain = new NetSdkNative.NetGain();
            int ret = NetSdkNative.net_get_thermal_gain(_netHandle, ref gain);
            LogApi("net_get_thermal_gain", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            gain.max = (float)nudMax.Value;
            gain.min = (float)nudMin.Value;
            gain.fixedSpan = (byte)(cbFixedSpan.Checked ? 1 : 0);
            gain.span = (float)nudSpan.Value;

            ret = NetSdkNative.net_set_thermal_gain(_netHandle, ref gain);
            LogApi("net_set_thermal_gain", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            LoadGain();
        }

        private void tscbPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing || !_netConnected)
                return;

            if (tscbPalette.SelectedItem == null)
                return;

            var item = (PresetPaletteItem)tscbPalette.SelectedItem;
            var palette = NetSdkNative.NetPalette.Create();
            palette.ref_no = item.RefNo;
            palette.inverted = (byte)(tsbInverted.Checked ? 1 : 0);
            int ret = NetSdkNative.net_set_thermal_palette(_netHandle, ref palette);
            LogApi("net_set_thermal_palette", ret);

            var updated = NetSdkNative.NetPalette.Create();
            ret = NetSdkNative.net_get_thermal_palette(_netHandle, ref updated);
            LogApi("net_get_thermal_palette", ret);
            if (NetSdkNative.IsOk(ret))
            {
                UpdatePaletteImage(updated);
            }
        }

        private void tscbImageFlip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing || !_netConnected) return;
            if (tscbImageFlip.SelectedIndex < 0) return;

            uint channelId;
            if (!TryResolveDisplayChannelId(out channelId)) return;

            var mode = (NetSdkNative.NetImageFlipMode)tscbImageFlip.SelectedIndex;
            int retFlip = NetSdkNative.net_set_image_flip(_netHandle, channelId, ref mode);
            LogApi("net_set_image_flip", retFlip, $"channel_id={channelId}");
        }

        private void tsbCalibrate_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            int ret = NetSdkNative.net_thermal_calibrate(_netHandle);
            LogApi("net_thermal_calibrate", ret);
            if (!NetSdkNative.IsOk(ret))
                MessageBox.Show($"Calibrate failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LoadImageFlip()
        {
            uint channelId;
            if (!TryResolveDisplayChannelId(out channelId)) return;

            IsGUIRefreshing = true;
            try
            {
                NetSdkNative.NetImageFlipMode mode;
                int ret = NetSdkNative.net_get_image_flip(_netHandle, channelId, out mode);
                LogApi("net_get_image_flip", ret, $"channel_id={channelId}");
                if (NetSdkNative.IsOk(ret))
                {
                    int idx = (int)mode;
                    if (idx >= 0 && idx < tscbImageFlip.Items.Count)
                        tscbImageFlip.SelectedIndex = idx;
                }
                else if (tscbImageFlip.SelectedIndex < 0 && tscbImageFlip.Items.Count > 0)
                {
                    tscbImageFlip.SelectedIndex = 0;
                }
            }
            finally
            {
                IsGUIRefreshing = false;
            }
        }

        private void LoadColorDistType()
        {
            IsGUIRefreshing = true;
            try
            {
                NetSdkNative.NetColorDistType type;
                int ret = NetSdkNative.net_get_thermal_color_dist(_netHandle, out type);
                LogApi("net_get_thermal_color_dist", ret);
                if (NetSdkNative.IsOk(ret))
                {
                    for (int i = 0; i < tscbColorDistributionType.Items.Count; i++)
                    {
                        if ((NetSdkNative.NetColorDistType)tscbColorDistributionType.Items[i] == type)
                        {
                            tscbColorDistributionType.SelectedIndex = i;
                            return;
                        }
                    }
                }

                if (tscbColorDistributionType.SelectedIndex < 0 && tscbColorDistributionType.Items.Count > 0)
                    tscbColorDistributionType.SelectedIndex = 0;
            }
            finally
            {
                IsGUIRefreshing = false;
            }
        }

        /// <summary>
        /// 先通过 net_get_channels 获取设备通道 id，再用于后续 Display 相关接口。
        /// </summary>
        private bool TryResolveDisplayChannelId(out uint channelId)
        {
            channelId = 0;
            if (_netHandle == IntPtr.Zero) return false;

            var channels = new NetSdkNative.NetChannelInfo[4];
            for (int i = 0; i < channels.Length; i++)
                channels[i] = NetSdkNative.NetChannelInfo.Create();

            ushort count;
            int ret = NetSdkNative.net_get_channels(_netHandle, channels, (ushort)channels.Length, out count);
            LogApi("net_get_channels", ret, $"count={count}");
            if (!NetSdkNative.IsOk(ret) || count == 0)
                return false;

            channelId = channels[0].id;
            _displayChannelId = channelId;
            return true;
        }

        private void tsbInverted_CheckedChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing || !_netConnected)
                return;

            var palette = NetSdkNative.NetPalette.Create();
            int ret = NetSdkNative.net_get_thermal_palette(_netHandle, ref palette);
            LogApi("net_get_thermal_palette", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            palette.inverted = (byte)(tsbInverted.Checked ? 1 : 0);
            ret = NetSdkNative.net_set_thermal_palette(_netHandle, ref palette);
            LogApi("net_set_thermal_palette", ret);

            var updated = NetSdkNative.NetPalette.Create();
            if (NetSdkNative.IsOk(NetSdkNative.net_get_thermal_palette(_netHandle, ref updated)))
                UpdatePaletteImage(updated);
        }

        private void SetPaletteInverted(bool inverted)
        {
            var palette = NetSdkNative.NetPalette.Create();
            int ret = NetSdkNative.net_get_thermal_palette(_netHandle, ref palette);
            LogApi("net_get_thermal_palette", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            palette.inverted = (byte)(inverted ? 1 : 0);
            ret = NetSdkNative.net_set_thermal_palette(_netHandle, ref palette);
            LogApi("net_set_thermal_palette", ret);
        }

        private void tscbColorDistributionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing || !_netConnected)
                return;
            if (tscbColorDistributionType.SelectedItem == null)
                return;

            var type = (NetSdkNative.NetColorDistType)tscbColorDistributionType.SelectedItem;
            int ret = NetSdkNative.net_set_thermal_color_dist(_netHandle, ref type);
            LogApi("net_set_thermal_color_dist", ret);
            if (ret != 0)
                MessageBox.Show("Set color distribution failed: " + ret, "Error");
        }

        private static bool IsInsideRect(Rectangle rect, Point pt)
        {
            bool isInside = false;

            if (pt.X >= rect.X &&
                pt.X < (rect.X + rect.Width) &&
                pt.Y >= rect.Y &&
                pt.Y < (rect.Y + rect.Height))
            {
                isInside = true;
            }

            return isInside;
        }

        private static Rectangle PadHitRect(Point pt)
        {
            return new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
        }

        private static float GetDistanceFromLine(Point start, Point end, Point pt)
        {
            if (start.X != end.X)
            {
                float m = (float)(end.Y - start.Y) / (end.X - start.X);
                float b = -1.0F;
                float c = start.Y - m * start.X;

                float x = m * pt.X + b * pt.Y + c;
                float y = (float)(Math.Sqrt(m * m + b * b));

                return x / y;
            }
            else
            {
                return (float)(pt.X - start.X);
            }
        }

        #region NetSDK Connection

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_netConnected) NetDisconnect();
            if (_sdkLogConsoleAttached || tsbLogging.Checked)
                DisableSdkLogging();
            base.OnFormClosing(e);
        }

        private void tsbConnect_Click(object sender, EventArgs e)
        {
            if (_netConnected)
            {
                NetDisconnect();
            }
            else
            {
                NetConnect();
            }
        }

        private void NetConnect()
        {
            int ret = NetSdkNative.net_sdk_init();
            LogApi("net_sdk_init", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("SDK init failed: " + ret, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ushort port;
            if (!ushort.TryParse(tstbPort.Text.Trim(), out port))
                port = 80;

            var loginInfo = NetSdkNative.NetLoginInfo.Create(
                tstbIP.Text.Trim(),
                port,
                tstbUser.Text.Trim(),
                tstbPassword.Text.Trim(),
                5000
            );

            ret = NetSdkNative.net_login(ref loginInfo, out _netHandle);
            LogApi("net_login", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show($"Connect failed, error: {ret} ({(NetSdkNative.NetErrorCode)ret})",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NetSdkNative.net_sdk_cleanup();
                return;
            }

            _netConnected = true;
            tsbConnect.Text = "Disconnect";
            tsbConnect.BackColor = System.Drawing.Color.LightGreen;
            tstbIP.Enabled = false;
            tstbPort.Enabled = false;
            tstbUser.Enabled = false;
            tstbPassword.Enabled = false;

            Text = $"IRtekNetSDKDemo - Connected {tstbIP.Text}";

            _exceptionCb = OnNetExceptionReceived;
            int evtRet = NetSdkNative.net_set_exception_callback(_netHandle, _exceptionCb);
            LogApi("net_set_exception_callback", evtRet);

            if (TryActivateAcousticDevice())
            {
                StartVideoStream();
                return;
            }

            LoadThermalParams();
            _activeTempUnit = GetSelectedTempUnit();
            _activeDistUnit = GetSelectedDistanceUnit();
            LoadRanges();
            LoadPalettes();
            LoadProperties();
            LoadDeviceTime();
            LoadNetworkInfo();
            LoadUsers();
            LoadImageFlip();
            LoadColorDistType();
            LoadGain();
            LoadMarkers();
            StartThermalDataStream();
            StartVideoStream();
        }

        private void NetDisconnect()
        {
            SetMarkerTool(MarkerType.None, false, false);
            HideAcousticChrome();

            if (_exceptionCb != null)
            {
                NetSdkNative.net_set_exception_callback(_netHandle, null);
                LogApiCall("net_set_exception_callback (clear)");
                _exceptionCb = null;
            }

            StopAlarmListener();
            StopThermalDataStream();
            StopVideoStream();
            DisposeH264Decoder();
            lock (_markerLock)
            {
                _markerCount = 0;
                _markerTempsCount = 0;
                _hasGlobalTemp = false;
            }
            _spotCounter = 0;
            _areaCounter = 0;
            _lineCounter = 0;

            if (_netHandle != IntPtr.Zero)
            {
                NetSdkNative.net_logout(ref _netHandle);
                LogApiCall("net_logout");
                _netHandle = IntPtr.Zero;
            }
            NetSdkNative.net_sdk_cleanup();
            LogApiCall("net_sdk_cleanup");

            _netConnected = false;
            _displayChannelId = 0;
            _currentStreamId = 0;
            _pendingStreamDataLogCount = 0;
            _updatingStreamIdCombo = true;
            try
            {
                tscbStreamId.Items.Clear();
            }
            finally
            {
                _updatingStreamIdCombo = false;
            }
            tsbConnect.Text = "Connect";
            tsbConnect.BackColor = System.Drawing.SystemColors.Control;
            tstbIP.Enabled = true;
            tstbPort.Enabled = true;
            tstbUser.Enabled = true;
            tstbPassword.Enabled = true;

            Text = "IRtekNetSDKDemo";
            UpdateTimeUiState();
            ResetStreamCallbackMode();
            UpdateStreamCallbackButtonUi();
        }

        #region Marker Management

        private void LoadMarkers()
        {
            var size = new NetSdkNative.NetSize();
            int retRes = NetSdkNative.net_get_thermal_resolution(_netHandle, ref size);
            LogApi("net_get_thermal_resolution", retRes);
            if (NetSdkNative.IsOk(retRes))
            {
                _markerResW = size.width;
                _markerResH = size.height;
            }

            var buf = new NetSdkNative.NetMarkerInfo[NetSdkNative.NET_MAX_MARKERS];
            for (int i = 0; i < buf.Length; i++)
                buf[i] = NetSdkNative.NetMarkerInfo.Create();

            ushort outLen;
            int ret = NetSdkNative.net_get_thermal_markers(_netHandle, buf, (ushort)buf.Length, out outLen);
            LogApi("net_get_thermal_markers", ret);
            if (NetSdkNative.IsOk(ret))
            {
                lock (_markerLock)
                {
                    for (int i = 0; i < outLen; i++)
                    {
                        // Header has no type=2; normalize legacy device Line values to Polyline.
                        if ((int)buf[i].type == 2)
                            buf[i].type = NetSdkNative.NetMarkerType.Polyline;
                    }
                    _markers = buf;
                    _markerCount = outLen;
                }
                UpdateMarkerCounters();
                RefreshMarkerListView();
            }
        }

        private void PushMarkers()
        {
            NetSdkNative.NetMarkerInfo[] copy;
            ushort count;
            lock (_markerLock)
            {
                copy = (NetSdkNative.NetMarkerInfo[])_markers.Clone();
                count = _markerCount;
                for (int i = 0; i < count; i++)
                {
                    if ((int)copy[i].type == 2)
                        copy[i].type = NetSdkNative.NetMarkerType.Polyline;
                }
            }
            int ret = NetSdkNative.net_set_thermal_markers(_netHandle, copy, count);
            LogApi("net_set_thermal_markers", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                LoadMarkers();
                Action showError = () =>
                    MessageBox.Show("Set markers failed: " + ret, "Error");
                if (InvokeRequired)
                    BeginInvoke(showError);
                else
                    showError();
            }
        }

        private string NextSpotName()
        {
            _spotCounter++;
            return "S" + _spotCounter.ToString("D2");
        }

        private string NextAreaName()
        {
            _areaCounter++;
            return "A" + _areaCounter.ToString("D2");
        }

        private string NextLineName()
        {
            _lineCounter++;
            return "L" + _lineCounter.ToString("D2");
        }

        private void UpdateMarkerCounters()
        {
            _spotCounter = 0;
            _areaCounter = 0;
            _lineCounter = 0;
            for (int i = 0; i < _markerCount; i++)
            {
                switch (_markers[i].type)
                {
                    case NetSdkNative.NetMarkerType.Spot:
                        _spotCounter++;
                        break;
                    case NetSdkNative.NetMarkerType.Polygon:
                        _areaCounter++;
                        break;
                    case NetSdkNative.NetMarkerType.Polyline:
                        _lineCounter++;
                        break;
                }
            }
        }

        private void RefreshMarkerListView()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshMarkerListView));
                return;
            }

            string selectedName = null;
            if (mlvMarkers.SelectedItems.Count == 1)
                selectedName = mlvMarkers.SelectedItems[0].Text;
            else if (!string.IsNullOrEmpty(_lastMarkerListHitName))
                selectedName = _lastMarkerListHitName;

            mlvMarkers.BeginUpdate();
            try
            {
                mlvMarkers.Items.Clear();
                lock (_markerLock)
                {
                    AppendGlobalMarkerListItem(selectedName);

                    for (int i = 0; i < _markerCount; i++)
                    {
                        var m = _markers[i];
                        string name = GetMarkerNameString(m);
                        string typeStr = GetMarkerTypeDisplayName(m.type) + " id=" + m.id;
                        var item = new System.Windows.Forms.ListViewItem(name);
                        item.SubItems.Add(typeStr);
                        item.Tag = i;

                        NetSdkNative.NetTempPointInfo? tempInfo = null;
                        for (int t = 0; t < _markerTempsCount; t++)
                        {
                            string tempName = GetTempMarkerName(_markerTemps[t]);
                            if (tempName == name)
                            {
                                tempInfo = _markerTemps[t].temp_point_info;
                                break;
                            }
                        }

                        if (tempInfo.HasValue)
                            FillTempInfoSubItems(item, tempInfo.Value);
                        else
                            FillEmptyTempSubItems(item);

                        item.SubItems.Add("pts=" + m.points_count + " p0=(" + m.points[0].x + "," + m.points[0].y + ")");
                        if (m.points_count >= 2)
                            item.SubItems[item.SubItems.Count - 1].Text += " p1=(" + m.points[1].x + "," + m.points[1].y + ")";

                        item.SubItems.Add(FormatLocalParamsText(m));
                        item.SubItems.Add(GetMarkerRuntimeAlarmLevelText(name));

                        mlvMarkers.Items.Add(item);
                        if (selectedName != null && name == selectedName)
                            item.Selected = true;
                    }
                }

                if (mlvMarkers.SelectedItems.Count == 1)
                    mlvMarkers.SelectedItems[0].EnsureVisible();
            }
            finally
            {
                mlvMarkers.EndUpdate();
            }
        }

        private void AppendGlobalMarkerListItem(string selectedName)
        {
            var item = new System.Windows.Forms.ListViewItem(GlobalMarkerListName);
            item.SubItems.Add("Global");
            item.Tag = GlobalMarkerListTag;

            if (_hasGlobalTemp)
                FillTempInfoSubItems(item, _globalTempInfo);
            else
                FillEmptyTempSubItems(item);

            item.SubItems.Add("");
            item.SubItems.Add("");
            // marker_alarms may include a "global" entry; match by name (not marker index).
            item.SubItems.Add(GetMarkerRuntimeAlarmLevelText(GlobalMarkerListName));

            mlvMarkers.Items.Add(item);
            if (selectedName != null && selectedName == GlobalMarkerListName)
                item.Selected = true;
        }

        private void FillTempInfoSubItems(System.Windows.Forms.ListViewItem item, NetSdkNative.NetTempPointInfo tempInfo)
        {
            item.SubItems.Add(FormatMarkerTemperature(tempInfo.min.temperature) +
                "(" + tempInfo.min.point.x + "," + tempInfo.min.point.y + ")");
            item.SubItems.Add(FormatMarkerTemperature(tempInfo.max.temperature) +
                "(" + tempInfo.max.point.x + "," + tempInfo.max.point.y + ")");
            item.SubItems.Add(FormatMarkerTemperature(tempInfo.avg_temp));
        }

        private static void FillEmptyTempSubItems(System.Windows.Forms.ListViewItem item)
        {
            item.SubItems.Add("-");
            item.SubItems.Add("-");
            item.SubItems.Add("-");
        }

        private string FormatLocalParamsText(NetSdkNative.NetMarkerInfo m)
        {
            if (m.local_params == 0)
                return "OFF";

            return string.Format("ON e={0:F2} d={1:F2}{2} r={3:F1}{4}",
                m.emissivity,
                m.distance,
                GetDistanceUnitLabel(GetSelectedDistanceUnit()),
                m.refl_temp,
                GetTempUnitLabel(GetSelectedTempUnit()));
        }

        private void mlvMarkers_MouseDown(object sender, MouseEventArgs e)
        {
            var hit = mlvMarkers.HitTest(e.Location);
            if (hit.Item == null)
            {
                _lastMarkerListHitName = null;
                return;
            }

            _lastMarkerListHitName = hit.Item.Text;
            foreach (ListViewItem item in mlvMarkers.Items)
                item.Selected = false;
            hit.Item.Selected = true;
            hit.Item.Focused = true;
        }

        private void mlvMarkers_DoubleClick(object sender, EventArgs e)
        {
            if (_acousticActive)
                return;
            EditSelectedMarkerLocalParams();
        }

        private void tsmiLocalParameters_Click(object sender, EventArgs e)
        {
            EditSelectedMarkerLocalParams();
        }

        private int ResolveSelectedMarkerIndex()
        {
            if (mlvMarkers.SelectedItems.Count == 1 && mlvMarkers.SelectedItems[0].Tag is int)
                return (int)mlvMarkers.SelectedItems[0].Tag;

            if (mlvMarkers.FocusedItem != null && mlvMarkers.FocusedItem.Tag is int)
                return (int)mlvMarkers.FocusedItem.Tag;

            if (!string.IsNullOrEmpty(_lastMarkerListHitName))
            {
                lock (_markerLock)
                {
                    return FindMarkerByName(_lastMarkerListHitName);
                }
            }

            return -1;
        }

        private void EditSelectedMarkerLocalParams()
        {
            if (!_netConnected)
            {
                MessageBox.Show("Please connect to a device first.", "Local Parameters");
                return;
            }

            int idx = ResolveSelectedMarkerIndex();
            if (idx < 0)
            {
                MessageBox.Show("Please select one marker.", "Local Parameters");
                return;
            }

            NetSdkNative.NetMarkerInfo marker;
            string markerName;
            lock (_markerLock)
            {
                if (idx < 0 || idx >= _markerCount)
                    return;
                marker = _markers[idx];
                markerName = GetMarkerNameString(marker);
            }

            bool enabled = marker.local_params != 0;
            float emissivity = marker.emissivity;
            float distance = marker.distance;
            float reflTemp = marker.refl_temp;

            // Seed defaults from global thermal params when local values look unset.
            if (!enabled && emissivity <= 0f && distance <= 0f && reflTemp == 0f)
            {
                emissivity = (float)nudEmissivity.Value;
                distance = (float)nudDistance.Value;
                reflTemp = (float)nudReflTemp.Value;
            }
            else
            {
                if (emissivity <= 0f) emissivity = (float)nudEmissivity.Value;
                if (distance < 0f) distance = (float)nudDistance.Value;
            }

            using (var dlg = new LocalParamsDialog(
                markerName,
                enabled,
                emissivity,
                distance,
                reflTemp,
                GetTempUnitLabel(GetSelectedTempUnit()),
                GetDistanceUnitLabel(GetSelectedDistanceUnit())))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                lock (_markerLock)
                {
                    idx = FindMarkerByName(markerName);
                    if (idx < 0)
                        return;
                    _markers[idx].local_params = dlg.LocalParamsEnabled ? 1u : 0u;
                    _markers[idx].emissivity = dlg.Emissivity;
                    _markers[idx].distance = dlg.Distance;
                    _markers[idx].refl_temp = dlg.ReflTemp;
                }
            }

            _lastMarkerListHitName = markerName;
            PushMarkers();
            RefreshMarkerListView();
        }

        private NetSdkNative.NetMarkerInfo CreateMarkerInfo(NetSdkNative.NetMarkerType type, string name,
            NetSdkNative.NetPoint[] points, ushort pointsCount)
        {
            var info = NetSdkNative.NetMarkerInfo.Create();
            info.type = type;
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(name);
            System.Array.Copy(nameBytes, info.name, System.Math.Min(nameBytes.Length, NetSdkNative.NET_LEN_64 - 1));
            for (int i = 0; i < pointsCount && i < NetSdkNative.NET_MAX_MARKER_POINTS; i++)
                info.points[i] = points[i];
            info.points_count = pointsCount;
            info.local_params = 0;
            info.emissivity = (float)nudEmissivity.Value;
            info.distance = (float)nudDistance.Value;
            info.refl_temp = (float)nudReflTemp.Value;
            return info;
        }

        private int FindMarkerByName(string name)
        {
            for (int i = 0; i < _markerCount; i++)
            {
                if (GetMarkerNameString(_markers[i]) == name)
                    return i;
            }
            return -1;
        }

        private static string GetMarkerNameString(NetSdkNative.NetMarkerInfo m)
        {
            if (m.name == null) return "";
            int len = 0;
            while (len < m.name.Length && m.name[len] != 0) len++;
            return System.Text.Encoding.UTF8.GetString(m.name, 0, len);
        }

        private static string GetTempMarkerName(NetSdkNative.NetMarkerTempInfo t)
        {
            if (t.marker_name == null) return "";
            int len = 0;
            while (len < t.marker_name.Length && t.marker_name[len] != 0) len++;
            return System.Text.Encoding.UTF8.GetString(t.marker_name, 0, len);
        }

        private void StartThermalDataStream()
        {
            RestartThermalDataStream();
        }

        private int StartThermalDataStreamWithConfig(NetSdkNative.NetThermalDataConfig config)
        {
            _thermalDataCb = OnThermalDataReceived;
            int ret = NetSdkNative.net_start_thermal_data_stream(_netHandle, ref config, _thermalDataCb);
            if (!NetSdkNative.TryGetSdkHandle(ret, out _thermalDataStreamHandle))
            {
                _thermalDataCb = null;
                _thermalDataStreamHandle = IntPtr.Zero;
            }

            return ret;
        }

        private void RestartThermalDataStream()
        {
            StopThermalDataStream();
            ApplyThermalStreamConfig(GetSelectedThermalStreamPreset());
        }

        private void tscbThermalStreamConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingThermalStreamConfigCombo)
                return;

            SyncSelectedThermalStreamPresetFromUi();

            if (!_netConnected)
                return;

            ApplyThermalStreamConfig(_selectedThermalStreamPreset);
        }

        private void SyncSelectedThermalStreamPresetFromUi()
        {
            var item = tscbThermalStreamConfig.SelectedItem as ThermalStreamConfigItem;
            _selectedThermalStreamPreset = item != null
                ? item.Preset
                : ThermalStreamConfigPreset.DefaultGlobalMarker;
        }

        private ThermalStreamConfigPreset GetSelectedThermalStreamPreset()
        {
            if (!InvokeRequired)
                SyncSelectedThermalStreamPresetFromUi();

            return _selectedThermalStreamPreset;
        }

        private static NetSdkNative.NetThermalDataConfig CreateThermalStreamConfig(ThermalStreamConfigPreset preset)
        {
            var config = new NetSdkNative.NetThermalDataConfig
            {
                sample_interval = ThermalStreamSampleIntervalMs
            };

            switch (preset)
            {
                case ThermalStreamConfigPreset.AllTypes:
                    config.data_types = GetAllThermalDataTypes();
                    break;
                default:
                    config.data_types = GetDemoThermalDataTypes();
                    break;
            }

            return config;
        }

        private static string GetThermalStreamPresetName(ThermalStreamConfigPreset preset)
        {
            switch (preset)
            {
                case ThermalStreamConfigPreset.AllTypes:
                    return "AllTypes";
                default:
                    return "DefaultGlobalMarker";
            }
        }

        private int ApplyThermalStreamConfig(ThermalStreamConfigPreset preset)
        {
            if (_netHandle == IntPtr.Zero)
                return -1;

            var config = CreateThermalStreamConfig(preset);
            StopThermalDataStream();
            int ret = StartThermalDataStreamWithConfig(config);
            LogApi(
                "net_start_thermal_data_stream",
                ret,
                "preset=" + GetThermalStreamPresetName(preset)
                    + " data_types=" + config.data_types
                    + " interval_ms=" + config.sample_interval);

            if (NetSdkNative.IsSuccess(ret))
                _thermalStartupLogRemaining = ThermalStreamStartupLogFrameCount;

            return ret;
        }

        private static NetSdkNative.NetThermalDataType GetDemoThermalDataTypes()
        {
            return NetSdkNative.NetThermalDataType.Global
                | NetSdkNative.NetThermalDataType.Marker;
        }

        private static NetSdkNative.NetThermalDataType GetAllThermalDataTypes()
        {
            return NetSdkNative.NetThermalDataType.Raw
                | NetSdkNative.NetThermalDataType.Global
                | NetSdkNative.NetThermalDataType.Marker;
        }

        private void StopThermalDataStream()
        {
            if (_thermalDataStreamHandle != IntPtr.Zero)
            {
                int ret = NetSdkNative.net_stop_thermal_data_stream(_thermalDataStreamHandle);
                LogApi("net_stop_thermal_data_stream", ret);
                _thermalDataStreamHandle = IntPtr.Zero;
            }

            _thermalDataCb = null;
            _thermalStartupLogRemaining = 0;
        }

        private void OnNetExceptionReceived(ref NetSdkNative.NetExceptionInfo exceptionInfo)
        {
            var evtType = exceptionInfo.exception_type;
            uint streamId = exceptionInfo.stream_id;
            string msg = NetSdkNative.BytesToString(exceptionInfo.message);
            string desc = $"[Exception] {evtType} stream={streamId} msg={msg}";

            BeginInvoke(new Action(() => AppendApiLog(desc)));

            switch (evtType)
            {
                case NetSdkNative.NetExceptionType.NetworkDisconnected:
                    BeginInvoke(new Action(() =>
                    {
                        Text = $"IRtekNetSDKDemo - DISCONNECTED";
                        tsbConnect.BackColor = System.Drawing.Color.Salmon;
                    }));
                    break;

                case NetSdkNative.NetExceptionType.NetworkResumed:
                    if (Interlocked.CompareExchange(ref _reconnecting, 1, 0) != 0)
                    {
                        LogSafe("[Reconnect] Reconnect already running, skip duplicate event");
                        break;
                    }
                    BeginInvoke(new Action(() =>
                    {
                        Text = $"IRtekNetSDKDemo - Reconnecting...";
                        tsbConnect.BackColor = System.Drawing.Color.Gold;
                        string ip = tstbIP.Text.Trim();
                        string port = tstbPort.Text.Trim();
                        string user = tstbUser.Text.Trim();
                        string pwd = tstbPassword.Text.Trim();
                        ThreadPool.QueueUserWorkItem(_ => ReconnectFull(ip, port, user, pwd));
                    }));
                    break;

                case NetSdkNative.NetExceptionType.VideoLost:
                    BeginInvoke(new Action(() =>
                        AppendApiLog($"[Exception] Video lost on stream {streamId}")));
                    break;

                case NetSdkNative.NetExceptionType.AlarmLost:
                    BeginInvoke(new Action(() =>
                        AppendApiLog($"[Exception] Alarm lost on stream {streamId}")));
                    break;
            }
        }

        private void OnThermalDataReceived(IntPtr pThermalData)
        {
            if (pThermalData == IntPtr.Zero) return;

            var data = (NetSdkNative.NetThermalData)Marshal.PtrToStructure(
                pThermalData, typeof(NetSdkNative.NetThermalData));

            if (_thermalStartupLogRemaining > 0)
            {
                int index = ThermalStreamStartupLogFrameCount - _thermalStartupLogRemaining + 1;
                LogThermalDataStartupSummary(data, index, ThermalStreamStartupLogFrameCount);
                _thermalStartupLogRemaining--;
            }

            ApplyThermalData(data);
        }

        private static NetSdkNative.NetThermalData CreateThermalDataBuffer()
        {
            var data = new NetSdkNative.NetThermalData
            {
                marker_temps = new NetSdkNative.NetMarkerTempInfo[NetSdkNative.NET_MAX_MARKERS]
            };

            for (int i = 0; i < data.marker_temps.Length; i++)
                data.marker_temps[i].marker_name = new byte[NetSdkNative.NET_LEN_64];

            return data;
        }

        private void ApplyThermalData(NetSdkNative.NetThermalData data)
        {
            bool hasGlobal = (data.data_types & NetSdkNative.NetThermalDataType.Global) != 0;
            bool hasMarker = (data.data_types & NetSdkNative.NetThermalDataType.Marker) != 0;

            lock (_markerLock)
            {
                if (hasGlobal)
                {
                    _globalTempInfo = data.global_temp_info;
                    _hasGlobalTemp = true;
                }

                if (hasMarker && data.marker_temps != null)
                {
                    _markerTempsCount = data.markers_count;
                    int count = Math.Min((int)data.markers_count, NetSdkNative.NET_MAX_MARKERS);
                    for (int i = 0; i < count; i++)
                        _markerTemps[i] = data.marker_temps[i];
                }
            }

            Action refresh = delegate
            {
                RefreshMarkerListView();
            };

            if (InvokeRequired)
                BeginInvoke(refresh);
            else
                refresh();
        }

        private string BuildThermalDataSummary(NetSdkNative.NetThermalData data)
        {
            var parts = new List<string>();
            parts.Add("types=" + FormatThermalDataTypes(data.data_types));
            parts.Add("ts=" + data.timestamp_us);

            if ((data.data_types & NetSdkNative.NetThermalDataType.Global) != 0)
            {
                parts.Add(string.Format("global min={0} max={1} avg={2}",
                    FormatMarkerTemperature(data.global_temp_info.min.temperature),
                    FormatMarkerTemperature(data.global_temp_info.max.temperature),
                    FormatMarkerTemperature(data.global_temp_info.avg_temp)));
            }

            if ((data.data_types & NetSdkNative.NetThermalDataType.Marker) != 0)
                AppendMarkerTempSummaryParts(parts, data);

            return string.Join(" ", parts.ToArray());
        }

        private void AppendMarkerTempSummaryParts(List<string> parts, NetSdkNative.NetThermalData data)
        {
            if (data.marker_temps == null)
            {
                parts.Add("markers=" + data.markers_count);
                return;
            }

            int count = Math.Min((int)data.markers_count, NetSdkNative.NET_MAX_MARKERS);
            if (count <= 0)
            {
                parts.Add("markers=0");
                return;
            }

            var markerParts = new List<string>();
            for (int i = 0; i < count; i++)
            {
                var marker = data.marker_temps[i];
                string name = GetTempMarkerName(marker);
                if (string.IsNullOrEmpty(name))
                    name = "id=" + marker.marker_id;

                var info = marker.temp_point_info;
                markerParts.Add(string.Format("{0} min={1} max={2} avg={3}",
                    name,
                    FormatMarkerTemperature(info.min.temperature),
                    FormatMarkerTemperature(info.max.temperature),
                    FormatMarkerTemperature(info.avg_temp)));
            }

            parts.Add("markers=" + data.markers_count + " [" + string.Join("; ", markerParts.ToArray()) + "]");
        }

        private void LogThermalDataStartupSummary(NetSdkNative.NetThermalData data, int index, int total)
        {
            if (cbInterfaceNotify == null || !cbInterfaceNotify.Checked)
                return;

            uint expected = (uint)CreateThermalStreamConfig(_selectedThermalStreamPreset).data_types;
            uint actual = (uint)data.data_types;
            var raw = data.raw_frame;

            var parts = new List<string>();
            parts.Add("net_thermal_data [startup_frame_" + index + "/" + total + "]");
            parts.Add(FormatThermalDataTypeMaskDiff(expected, actual));
            parts.Add("ts=" + data.timestamp_us);
            parts.Add("markers_count=" + data.markers_count);

            if ((data.data_types & NetSdkNative.NetThermalDataType.Global) != 0)
            {
                parts.Add(string.Format("global min={0} max={1} avg={2}",
                    FormatMarkerTemperature(data.global_temp_info.min.temperature),
                    FormatMarkerTemperature(data.global_temp_info.max.temperature),
                    FormatMarkerTemperature(data.global_temp_info.avg_temp)));
            }

            if ((data.data_types & NetSdkNative.NetThermalDataType.Marker) != 0)
                AppendMarkerTempSummaryParts(parts, data);

            parts.Add("raw=" + FormatRawFrameProbeSummary(data));
            parts.Add("raw_meta=width=" + raw.width
                + " height=" + raw.height
                + " range_id=" + raw.range_id
                + " frame_no=" + raw.frame_no
                + " sensor_temp=" + raw.sensor_temp
                + " shutter_state=" + raw.shutter_state
                + " pts=" + raw.pts
                + " raw_type=" + raw.raw_type
                + " shutter_adv=" + raw.shutter_adv
                + " lut_len=" + raw.lut_len
                + " lut_env_temp=" + raw.lut_env_temp);
            parts.Add("data_preview=" + FormatRawUshortPreview(raw.data, (int)raw.points_count, raw.width, raw.height, 3));
            parts.Add("lut_temp_preview=" + FormatFloatPtrPreview(raw.lut_temp_value, (int)raw.lut_len, 3));
            parts.Add("lut_ad_preview=" + FormatRawUshortPreview(raw.lut_ad_value, (int)raw.lut_len, 0, 0, 3));
            AppendApiLog(string.Join(" ", parts.ToArray()));
        }

        private static string FormatRawUshortPreview(IntPtr dataPtr, int pointCount, ushort width, ushort height, int previewCount)
        {
            if (pointCount <= 0 && width > 0 && height > 0)
                pointCount = width * height;
            if (dataPtr == IntPtr.Zero || pointCount <= 0)
                return "null";

            int count = Math.Min(previewCount, pointCount);
            var shorts = new short[count];
            Marshal.Copy(dataPtr, shorts, 0, count);
            var values = new string[count];
            for (int i = 0; i < count; i++)
                values[i] = ((ushort)shorts[i]).ToString(System.Globalization.CultureInfo.InvariantCulture);
            return "[" + string.Join(",", values) + "]";
        }

        private static string FormatFloatPtrPreview(IntPtr dataPtr, int length, int previewCount)
        {
            if (dataPtr == IntPtr.Zero || length <= 0)
                return "null";

            int count = Math.Min(previewCount, length);
            var values = new float[count];
            Marshal.Copy(dataPtr, values, 0, count);
            var parts = new string[count];
            for (int i = 0; i < count; i++)
                parts[i] = values[i].ToString("G6", System.Globalization.CultureInfo.InvariantCulture);
            return "[" + string.Join(",", parts) + "]";
        }

        private void LogThermalDataPeriodicSummary(NetSdkNative.NetThermalData data, int index, int total)
        {
            if (cbInterfaceNotify == null || !cbInterfaceNotify.Checked)
                return;

            uint expected = (uint)CreateThermalStreamConfig(_selectedThermalStreamPreset).data_types;
            uint actual = (uint)data.data_types;

            var parts = new List<string>();
            parts.Add("net_thermal_data [periodic_sample_" + index + "/" + total + "]");
            parts.Add(FormatThermalDataTypeMaskDiff(expected, actual));
            parts.Add("ts=" + data.timestamp_us);

            if ((data.data_types & NetSdkNative.NetThermalDataType.Global) != 0)
            {
                parts.Add(string.Format("global min={0} max={1} avg={2}",
                    FormatMarkerTemperature(data.global_temp_info.min.temperature),
                    FormatMarkerTemperature(data.global_temp_info.max.temperature),
                    FormatMarkerTemperature(data.global_temp_info.avg_temp)));
            }

            if ((data.data_types & NetSdkNative.NetThermalDataType.Marker) != 0)
                AppendMarkerTempSummaryParts(parts, data);

            parts.Add("raw=" + FormatRawFrameProbeSummary(data));
            AppendApiLog(string.Join(" ", parts.ToArray()));
        }

        private static string FormatThermalDataTypeMask(uint mask)
        {
            return mask + "(" + FormatThermalDataTypes((NetSdkNative.NetThermalDataType)mask) + ")";
        }

        private static string FormatThermalDataTypeMaskDiff(uint expected, uint actual)
        {
            uint extra = actual & ~expected;
            uint missing = expected & ~actual;

            var parts = new List<string>();
            parts.Add("expected=" + FormatThermalDataTypeMask(expected));
            parts.Add("actual=" + FormatThermalDataTypeMask(actual));
            if (extra != 0)
                parts.Add("extra=" + FormatThermalDataTypeMask(extra));
            if (missing != 0)
                parts.Add("missing=" + FormatThermalDataTypeMask(missing));
            if (extra == 0 && missing == 0)
                parts.Add("match=ok");

            return string.Join(" ", parts.ToArray());
        }

        private static string FormatRawFrameProbeSummary(NetSdkNative.NetThermalData data)
        {
            var raw = data.raw_frame;
            bool hasRawFlag = (data.data_types & NetSdkNative.NetThermalDataType.Raw) != 0;
            bool hasPayload = raw.data != IntPtr.Zero && raw.points_count > 0;

            if (hasPayload)
            {
                return "ok(" + raw.width + "x" + raw.height
                    + " pts=" + raw.points_count
                    + " ptr=0x" + raw.data.ToInt64().ToString("X") + ")";
            }

            if (hasRawFlag)
            {
                return "flag_only(width=" + raw.width
                    + " height=" + raw.height
                    + " ptr=" + raw.data.ToInt64() + ")";
            }

            return "empty(ptr=" + raw.data.ToInt64() + ")";
        }

        private void LogThermalDataFull(NetSdkNative.NetThermalData data, string tag)
        {
            if (cbInterfaceNotify == null || !cbInterfaceNotify.Checked)
                return;

            AppendApiLog("net_thermal_data [" + tag + "] types=" + FormatThermalDataTypes(data.data_types)
                + " ts=" + data.timestamp_us
                + " markers_count=" + data.markers_count);

            if ((data.data_types & NetSdkNative.NetThermalDataType.Global) != 0)
            {
                AppendApiLog("  global_temp_info: " + FormatTempPointInfoSummary(data.global_temp_info));
            }

            if ((data.data_types & NetSdkNative.NetThermalDataType.Marker) != 0 && data.marker_temps != null)
            {
                int count = Math.Min((int)data.markers_count, NetSdkNative.NET_MAX_MARKERS);
                for (int i = 0; i < count; i++)
                {
                    var marker = data.marker_temps[i];
                    AppendApiLog("  marker_temps[" + i + "]: id=" + marker.marker_id
                        + " type=" + marker.type
                        + " name=" + GetTempMarkerName(marker)
                        + " " + FormatTempPointInfoSummary(marker.temp_point_info));
                }
            }

            RawFrameSnapshot rawSnapshot;
            string rawError;
            if (RawFrameSnapshot.TryCapture(data, tag, out rawSnapshot, out rawError))
            {
                foreach (string line in rawSnapshot.BuildLogLines())
                    AppendApiLog("  " + line);
            }
            else
            {
                var raw = data.raw_frame;
                AppendApiLog("  raw_frame: " + rawError
                    + " width=" + raw.width
                    + " height=" + raw.height
                    + " range_id=" + raw.range_id
                    + " points_count=" + raw.points_count
                    + " lut_len=" + raw.lut_len
                    + " data_ptr=" + raw.data.ToInt64()
                    + " lut_temp_ptr=" + raw.lut_temp_value.ToInt64()
                    + " lut_ad_ptr=" + raw.lut_ad_value.ToInt64());
            }
        }

        private static string FormatTempPointInfoSummary(NetSdkNative.NetTempPointInfo info)
        {
            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "min={0}@({1},{2}) max={3}@({4},{5}) avg={6}",
                info.min.temperature,
                info.min.point.x,
                info.min.point.y,
                info.max.temperature,
                info.max.point.x,
                info.max.point.y,
                info.avg_temp);
        }

        private static string FormatThermalDataTypes(NetSdkNative.NetThermalDataType types)
        {
            if (types == 0)
                return "None";

            var names = new List<string>();
            if ((types & NetSdkNative.NetThermalDataType.Raw) != 0) names.Add("Raw");
            if ((types & NetSdkNative.NetThermalDataType.Global) != 0) names.Add("Global");
            if ((types & NetSdkNative.NetThermalDataType.Marker) != 0) names.Add("Marker");
            return string.Join("|", names.ToArray());
        }

        private static string FormatThermalAlarmLevel(NetSdkNative.NetThermalTempAlarmLevel level)
        {
            switch (level)
            {
                case NetSdkNative.NetThermalTempAlarmLevel.Alert:
                    return "Alert";
                case NetSdkNative.NetThermalTempAlarmLevel.Warning:
                    return "Warning";
                case NetSdkNative.NetThermalTempAlarmLevel.Alarm:
                    return "Alarm";
                default:
                    return "None";
            }
        }

        private string GetMarkerRuntimeAlarmLevelText(string markerName)
        {
            lock (_markerLock)
            {
                // Prefer name match: marker_alarms may include "global" and is not aligned to marker list index.
                for (int i = 0; i < NetSdkNative.NET_MAX_MARKERS; i++)
                {
                    string name = NetSdkNative.BytesToString(_markerAlarms[i].marker_name);
                    if (string.IsNullOrEmpty(name))
                        continue;
                    if (!string.Equals(name, markerName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (_markerAlarms[i].alarm_level == NetSdkNative.NetThermalTempAlarmLevel.None)
                        return "-";

                    return FormatThermalAlarmLevel(_markerAlarms[i].alarm_level);
                }

                // Fallback for ordinary markers only (global is not in _markers).
                if (!string.Equals(markerName, GlobalMarkerListName, StringComparison.OrdinalIgnoreCase))
                {
                    int markerIndex = FindMarkerByName(markerName);
                    if (markerIndex >= 0 && markerIndex < NetSdkNative.NET_MAX_MARKERS)
                    {
                        var level = _markerAlarms[markerIndex].alarm_level;
                        if (level != NetSdkNative.NetThermalTempAlarmLevel.None)
                            return FormatThermalAlarmLevel(level);
                    }
                }
            }

            return "-";
        }

        #endregion

        #region NetSDK Marker HitTest and Tracking

        private bool NetHitTest(Point point, ref string name, ref MarkerType markerType, ref TrackerHitType hitType)
        {
            lock (_markerLock)
            {
                for (int i = 0; i < _markerCount; i++)
                {
                    var m = _markers[i];
                    string mName = GetMarkerNameString(m);

                    switch (m.type)
                    {
                        case NetSdkNative.NetMarkerType.Spot:
                            {
                                int sx = m.points[0].x, sy = m.points[0].y;
                                if (Math.Abs(point.X - sx) <= 8 && Math.Abs(point.Y - sy) <= 8)
                                {
                                    Cursor = Cursors.SizeAll;
                                    name = mName;
                                    markerType = MarkerType.Spot;
                                    hitType = TrackerHitType.hitMiddle;
                                    return true;
                                }
                                break;
                            }
                        case NetSdkNative.NetMarkerType.Polygon:
                            {
                                if (m.points_count < 3) break;
                                int minX = int.MaxValue, minY = int.MaxValue;
                                int maxX = int.MinValue, maxY = int.MinValue;
                                for (int v = 0; v < m.points_count; v++)
                                {
                                    if (m.points[v].x < minX) minX = m.points[v].x;
                                    if (m.points[v].y < minY) minY = m.points[v].y;
                                    if (m.points[v].x > maxX) maxX = m.points[v].x;
                                    if (m.points[v].y > maxY) maxY = m.points[v].y;
                                }
                                for (int v = 0; v < m.points_count; v++)
                                {
                                    if (Math.Abs(point.X - m.points[v].x) <= 5 && Math.Abs(point.Y - m.points[v].y) <= 5)
                                    {
                                        Cursor = Cursors.SizeAll;
                                        name = mName;
                                        markerType = MarkerType.Polygon;
                                        hitType = (TrackerHitType)(TrackerHitType.hitTopLeft + v);
                                        return true;
                                    }
                                }
                                Rectangle polyBounds = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                                if (PointInPolygon(point, m.points, m.points_count) ||
                                    HitTestRect(polyBounds, point) == TrackerHitType.hitMiddle)
                                {
                                    Cursor = Cursors.SizeAll;
                                    name = mName;
                                    markerType = MarkerType.Polygon;
                                    hitType = TrackerHitType.hitMiddle;
                                    return true;
                                }
                                break;
                            }
                        case NetSdkNative.NetMarkerType.Polyline:
                            {
                                if (m.points_count < 2) break;
                                for (int v = 0; v < m.points_count; v++)
                                {
                                    if (Math.Abs(point.X - m.points[v].x) <= 5 && Math.Abs(point.Y - m.points[v].y) <= 5)
                                    {
                                        Cursor = Cursors.SizeAll;
                                        name = mName;
                                        markerType = MarkerType.Line;
                                        if (v == 0)
                                            hitType = TrackerHitType.hitStart;
                                        else if (v == m.points_count - 1)
                                            hitType = TrackerHitType.hitEnd;
                                        else
                                            hitType = (TrackerHitType)(TrackerHitType.hitTopLeft + v);
                                        return true;
                                    }
                                }
                                for (int v = 1; v < m.points_count; v++)
                                {
                                    Point p0 = new Point(m.points[v - 1].x, m.points[v - 1].y);
                                    Point p1 = new Point(m.points[v].x, m.points[v].y);
                                    if (Math.Abs(GetDistanceFromLine(p0, p1, point)) < 5.0f)
                                    {
                                        Cursor = Cursors.SizeAll;
                                        name = mName;
                                        markerType = MarkerType.Line;
                                        hitType = TrackerHitType.hitEdge;
                                        return true;
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            Cursor = Cursors.Default;
            return false;
        }

        private static Rectangle BoundsFromOppositePoints(NetSdkNative.NetPoint a, NetSdkNative.NetPoint b)
        {
            int x = Math.Min(a.x, b.x);
            int y = Math.Min(a.y, b.y);
            return new Rectangle(x, y, Math.Abs(a.x - b.x), Math.Abs(a.y - b.y));
        }

        private TrackerHitType HitTestRect(Rectangle rect, Point point)
        {
            if (PadHitRect(rect.Location).Contains(point)) { Cursor = Cursors.SizeNWSE; return TrackerHitType.hitTopLeft; }
            if (PadHitRect(new Point(rect.X + rect.Width / 2, rect.Y)).Contains(point)) { Cursor = Cursors.SizeNS; return TrackerHitType.hitTop; }
            if (PadHitRect(new Point(rect.Right, rect.Y)).Contains(point)) { Cursor = Cursors.SizeNESW; return TrackerHitType.hitTopRight; }
            if (PadHitRect(new Point(rect.Right, rect.Y + rect.Height / 2)).Contains(point)) { Cursor = Cursors.SizeWE; return TrackerHitType.hitRight; }
            if (PadHitRect(new Point(rect.Right, rect.Bottom)).Contains(point)) { Cursor = Cursors.SizeNWSE; return TrackerHitType.hitBottomRight; }
            if (PadHitRect(new Point(rect.X + rect.Width / 2, rect.Bottom)).Contains(point)) { Cursor = Cursors.SizeNS; return TrackerHitType.hitBottom; }
            if (PadHitRect(new Point(rect.X, rect.Bottom)).Contains(point)) { Cursor = Cursors.SizeNESW; return TrackerHitType.hitBottomLeft; }
            if (PadHitRect(new Point(rect.X, rect.Y + rect.Height / 2)).Contains(point)) { Cursor = Cursors.SizeWE; return TrackerHitType.hitLeft; }
            if (rect.Contains(point)) { Cursor = Cursors.SizeAll; return TrackerHitType.hitMiddle; }
            return TrackerHitType.hitNothing;
        }

        private bool PointInPolygon(Point test, NetSdkNative.NetPoint[] pts, ushort count)
        {
            bool inside = false;
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                int xi = pts[i].x, yi = pts[i].y;
                int xj = pts[j].x, yj = pts[j].y;
                if (((yi > test.Y) != (yj > test.Y)) &&
                    (test.X < (xj - xi) * (test.Y - yi) / (double)(yj - yi) + xi))
                    inside = !inside;
            }
            return inside;
        }

        private void NetTrackSymbol(Point point)
        {
            if (Tracker.PrevPos == point) return;

            int dx = point.X - Tracker.PrevPos.X;
            int dy = point.Y - Tracker.PrevPos.Y;
            Tracker.PrevPos = point;

            lock (_markerLock)
            {
                int idx = FindMarkerByName(Tracker.Name);
                if (idx < 0) return;

                ref NetSdkNative.NetMarkerInfo m = ref _markers[idx];

                if (Tracker.MarkerType == MarkerType.Spot)
                {
                    m.points[0].x = (ushort)Math.Max(0, Math.Min(_markerResW - 1, m.points[0].x + dx));
                    m.points[0].y = (ushort)Math.Max(0, Math.Min(_markerResH - 1, m.points[0].y + dy));
                }
                else if (Tracker.MarkerType == MarkerType.Line)
                {
                    int vtx = -1;
                    if (Tracker.HitType == TrackerHitType.hitStart)
                        vtx = 0;
                    else if (Tracker.HitType == TrackerHitType.hitEnd)
                        vtx = m.points_count - 1;
                    else if (Tracker.HitType != TrackerHitType.hitEdge)
                        vtx = (int)Tracker.HitType - (int)TrackerHitType.hitTopLeft;

                    if (vtx >= 0 && vtx < m.points_count)
                    {
                        m.points[vtx].x = (ushort)Math.Max(0, Math.Min(_markerResW - 1, m.points[vtx].x + dx));
                        m.points[vtx].y = (ushort)Math.Max(0, Math.Min(_markerResH - 1, m.points[vtx].y + dy));
                    }
                    else
                    {
                        for (int p = 0; p < m.points_count; p++)
                        {
                            m.points[p].x = (ushort)Math.Max(0, Math.Min(_markerResW - 1, m.points[p].x + dx));
                            m.points[p].y = (ushort)Math.Max(0, Math.Min(_markerResH - 1, m.points[p].y + dy));
                        }
                    }
                }
                else if (Tracker.MarkerType == MarkerType.Rectangle
                    || Tracker.MarkerType == MarkerType.Ellipse
                    || Tracker.MarkerType == MarkerType.Polygon)
                {
                    if (m.type == NetSdkNative.NetMarkerType.Polygon)
                    {
                        if (Tracker.HitType == TrackerHitType.hitMiddle)
                        {
                            for (int p = 0; p < m.points_count; p++)
                            {
                                m.points[p].x = (ushort)Math.Max(0, Math.Min(_markerResW - 1, m.points[p].x + dx));
                                m.points[p].y = (ushort)Math.Max(0, Math.Min(_markerResH - 1, m.points[p].y + dy));
                            }
                        }
                        else
                        {
                            int vtx = (int)Tracker.HitType - (int)TrackerHitType.hitTopLeft;
                            if (vtx >= 0 && vtx < m.points_count)
                            {
                                m.points[vtx].x = (ushort)Math.Max(0, Math.Min(_markerResW - 1, m.points[vtx].x + dx));
                                m.points[vtx].y = (ushort)Math.Max(0, Math.Min(_markerResH - 1, m.points[vtx].y + dy));
                            }
                        }
                    }
                    else
                    {
                        int x0 = m.points[0].x, y0 = m.points[0].y;
                        int x1 = m.points[1].x, y1 = m.points[1].y;

                        switch (Tracker.HitType)
                        {
                            case TrackerHitType.hitMiddle:
                                x0 += dx; y0 += dy; x1 += dx; y1 += dy;
                                break;
                            case TrackerHitType.hitTopLeft:
                                x0 += dx; y0 += dy;
                                break;
                            case TrackerHitType.hitTop:
                                y0 += dy;
                                break;
                            case TrackerHitType.hitTopRight:
                                x1 += dx; y0 += dy;
                                break;
                            case TrackerHitType.hitRight:
                                x1 += dx;
                                break;
                            case TrackerHitType.hitBottomRight:
                                x1 += dx; y1 += dy;
                                break;
                            case TrackerHitType.hitBottom:
                                y1 += dy;
                                break;
                            case TrackerHitType.hitBottomLeft:
                                x0 += dx; y1 += dy;
                                break;
                            case TrackerHitType.hitLeft:
                                x0 += dx;
                                break;
                        }

                        if (x0 > x1) { int tmp = x0; x0 = x1; x1 = tmp; }
                        if (y0 > y1) { int tmp = y0; y0 = y1; y1 = tmp; }

                        m.points[0].x = (ushort)Math.Max(0, Math.Min(_markerResW - 1, x0));
                        m.points[0].y = (ushort)Math.Max(0, Math.Min(_markerResH - 1, y0));
                        m.points[1].x = (ushort)Math.Max(0, Math.Min(_markerResW - 1, x1));
                        m.points[1].y = (ushort)Math.Max(0, Math.Min(_markerResH - 1, y1));
                    }
                }
            }
        }

        #endregion

        private void LoadProperties()
        {
            lvProperties.BeginUpdate();
            try
            {
            lvProperties.Items.Clear();

            var devInfo = NetSdkNative.NetDeviceInfo.Create();
            int ret = NetSdkNative.net_get_device_info(_netHandle, ref devInfo);
            LogApi("net_get_device_info", ret);
            if (NetSdkNative.IsOk(ret))
            {
                tbDeviceName.Text = NetSdkNative.BytesToString(devInfo.name);
                var grp = lvProperties.Groups["lvgCameraInfo"];
                lvProperties.Items.Add(new ListViewItem(new[] { "Name", NetSdkNative.BytesToString(devInfo.name) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Model", NetSdkNative.BytesToString(devInfo.model) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Spec", NetSdkNative.BytesToString(devInfo.spec) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Serial No.", NetSdkNative.BytesToString(devInfo.serial_no) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Brand", NetSdkNative.BytesToString(devInfo.brand) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Company", NetSdkNative.BytesToString(devInfo.company) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Firmware", NetSdkNative.BytesToString(devInfo.firmware_name) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "FW Version", NetSdkNative.BytesToString(devInfo.firmware_version) }, grp));
            }

            var lensInfo = NetSdkNative.NetLensInfo.Create();
            ret = NetSdkNative.net_get_thermal_lens_info(_netHandle, ref lensInfo);
            LogApi("net_get_thermal_lens_info", ret);
            if (NetSdkNative.IsOk(ret))
            {
                var grp = lvProperties.Groups["lvgLensInfo"];
                lvProperties.Items.Add(new ListViewItem(new[] { "Name", NetSdkNative.BytesToString(lensInfo.name) }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Focal", lensInfo.focal.ToString("F1") + "mm" }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "HFov", lensInfo.hfov.ToString("F1") + "°" }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "VFov", lensInfo.vfov.ToString("F1") + "°" }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Band", NetSdkNative.BytesToString(lensInfo.band) }, grp));

                var rgrp = lvProperties.Groups["lvgTempRange"];
                for (int i = 0; i < lensInfo.ranges_count; i++)
                {
                    var r = lensInfo.range_map[i];
                    lvProperties.Items.Add(new ListViewItem(new[] {
                        $"Range {r.id}", FormatTemperatureRange(r.range.min, r.range.max) }, rgrp));
                }
            }

            var thermalSize = new NetSdkNative.NetSize();
            ret = NetSdkNative.net_get_thermal_resolution(_netHandle, ref thermalSize);
            LogApi("net_get_thermal_resolution", ret);
            if (NetSdkNative.IsOk(ret))
            {
                var grp = lvProperties.Groups["lvgSize"];
                lvProperties.Items.Add(new ListViewItem(new[] {
                    "Thermal Resolution", $"{thermalSize.width}x{thermalSize.height}"
                }, grp));
            }

            AppendStorageRows();

            ushort streamCount;
            NetSdkNative.NetStreamInfo[] streams;
            bool hasStreams = TryGetAvailableStreams(out streams, out streamCount);
            NetSdkNative.NetStreamInfo primaryStream = NetSdkNative.NetStreamInfo.Create();
            bool hasPrimaryStream = false;
            if (hasStreams)
            {
                var videoGrp = lvProperties.Groups["lvgSize"];
                for (int i = 0; i < streamCount; i++)
                {
                    uint streamId = streams[i].id;
                    var stream = NetSdkNative.NetStreamInfo.Create();
                    int streamRet = NetSdkNative.net_get_stream(_netHandle, streamId, ref stream);
                    LogApi("net_get_stream", streamRet, "stream_id=" + streamId);
                    if (!NetSdkNative.IsOk(streamRet))
                        stream = streams[i];
                    if (!hasPrimaryStream)
                    {
                        primaryStream = stream;
                        hasPrimaryStream = true;
                    }

                    var status = new NetSdkNative.NetStreamStatus();
                    uint connectionCount = 0;
                    int statusRet = NetSdkNative.net_get_stream_status(_netHandle, streamId, ref status);
                    LogApi("net_get_stream_status", statusRet,
                        string.Format("stream_id={0} connections={1}", streamId, status.connection_count));
                    if (NetSdkNative.IsOk(statusRet))
                        connectionCount = status.connection_count;

                    AppendStreamInfoProperties(videoGrp, stream, connectionCount);
                }
            }

            AppendAcousticPropertyGroups(primaryStream, hasPrimaryStream);
            }
            finally
            {
                lvProperties.EndUpdate();
                lvProperties.ShowGroups = false;
                lvProperties.ShowGroups = true;
            }
        }

        private static string FormatBoolByte(byte value)
        {
            return value != 0 ? "1 (true)" : "0 (false)";
        }

        private void AppendStreamInfoProperties(ListViewGroup grp, NetSdkNative.NetStreamInfo stream, uint connectionCount)
        {
            string header = string.Format("Stream {0}", stream.id);
            lvProperties.Items.Add(new ListViewItem(new[] { header, string.Empty }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] { "  id", stream.id.ToString() }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  bit_rate", string.Format("{0} ({1} kbps)", stream.bit_rate, stream.bit_rate / 1024)
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  codec_type", NetSdkNative.BytesToString(stream.codec_type)
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  disable_skip", FormatBoolByte(stream.disable_skip)
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  enable_rc", FormatBoolByte(stream.enable_rc)
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] { "  gop_size", stream.gop_size.ToString() }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] { "  h264_profile", stream.h264_profile.ToString() }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] { "  initial_qp", stream.initial_qp.ToString() }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  max_frame_rate", stream.max_frame_rate + " fps"
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] { "  maximum_qp", stream.maximum_qp.ToString() }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  quality_control_type", NetSdkNative.BytesToString(stream.quality_control_type)
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  resolution_width", stream.resolution_width.ToString()
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  resolution_height", stream.resolution_height.ToString()
            }, grp));
            lvProperties.Items.Add(new ListViewItem(new[] {
                "  connection_count", connectionCount.ToString()
            }, grp));
        }

        private void AppendStorageRows()
        {
            var list = new NetSdkNative.NetStorageInfo[8];
            for (int i = 0; i < list.Length; i++)
                list[i] = NetSdkNative.NetStorageInfo.Create();

            ushort count;
            int ret = NetSdkNative.net_get_storages(_netHandle, list, (ushort)list.Length, out count);
            LogApi("net_get_storages", ret, "count=" + count);
            if (!NetSdkNative.IsOk(ret))
                return;

            var grp = lvProperties.Groups["lvgCameraInfo"];
            int shown = Math.Min(count, list.Length);
            for (int i = 0; i < shown; i++)
            {
                var storage = list[i];
                string recycling = storage.recycling != 0 ? ", recycle" : string.Empty;
                lvProperties.Items.Add(new ListViewItem(new[]
                {
                    "Storage " + storage.id,
                    NetSdkNative.BytesToString(storage.name)
                        + " " + storage.free_mb + "/" + storage.capacity_mb + " MB, "
                        + storage.status + ", " + storage.attr + recycling
                }, grp));
            }
        }

        private void btnGetTime_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadDeviceTime();
        }

        private void btnSyncTime_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (cbTimeMode.SelectedIndex != (int)NetSdkNative.NetTimeMode.Local)
                return;

            tbLocalTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ApplyTimeSettings();
        }

        private void btnApplyTime_Click(object sender, EventArgs e)
        {
            ApplyTimeSettings();
        }

        private void cbTimeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingTimeUi) return;
            UpdateTimeUiState();
        }

        private void btnNtpApply_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            var ntp = NetSdkNative.NetNtpInfo.Create();
            int ret = NetSdkNative.net_get_ntp_server(_netHandle, ref ntp);
            LogApi("net_get_ntp_server", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show($"Failed to read current NTP settings: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ntp.host_name = NetSdkNative.StringToFixedBytes(tbNtpHost.Text.Trim(), NetSdkNative.NET_LEN_128);
            ntp.port_no = (ushort)nudNtpPort.Value;
            ntp.syn_interval = (uint)nudNtpInterval.Value;

            ret = NetSdkNative.net_set_ntp_server(_netHandle, ref ntp);
            LogApi("net_set_ntp_server", ret);
            if (NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("NTP server updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadNtpServer();
            }
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LoadDeviceTime()
        {
            _loadingTimeUi = true;
            try
            {
                var timeInfo = NetSdkNative.NetTimeInfo.Create();
                int ret = NetSdkNative.net_get_time(_netHandle, ref timeInfo);
                LogApi("net_get_time", ret);
                if (NetSdkNative.IsOk(ret))
                {
                    cbTimeMode.SelectedIndex = (int)timeInfo.time_mode;
                    tbLocalTime.Text = NetSdkNative.BytesToString(timeInfo.local_time);

                    string tz = NetSdkNative.BytesToString(timeInfo.time_zone);
                    int tzIndex = -1;
                    for (int i = 0; i < cbTimeZone.Items.Count; i++)
                    {
                        if (cbTimeZone.Items[i].ToString() == tz)
                        {
                            tzIndex = i;
                            break;
                        }
                    }
                    if (tzIndex >= 0)
                        cbTimeZone.SelectedIndex = tzIndex;
                }

                UpdateTimeUiState();
            }
            finally
            {
                _loadingTimeUi = false;
            }

            LoadNtpServer();
        }

        private void UpdateTimeUiState()
        {
            bool isLocal = cbTimeMode.SelectedIndex == (int)NetSdkNative.NetTimeMode.Local;
            bool isNtp = cbTimeMode.SelectedIndex == (int)NetSdkNative.NetTimeMode.NTP;

            tbLocalTime.ReadOnly = !isLocal;
            btnSyncTime.Enabled = isLocal && _netConnected;
            btnApplyTime.Enabled = _netConnected;

            tbNtpHost.Enabled = isNtp;
            nudNtpPort.Enabled = isNtp;
            nudNtpInterval.Enabled = isNtp;
            btnNtpApply.Enabled = isNtp && _netConnected;
        }

        private bool TryBuildTimeInfoFromUi(out NetSdkNative.NetTimeInfo timeInfo, out string error)
        {
            timeInfo = NetSdkNative.NetTimeInfo.Create();
            error = null;

            if (cbTimeMode.SelectedIndex < 0)
            {
                error = "Please select time mode.";
                return false;
            }

            timeInfo.time_mode = (NetSdkNative.NetTimeMode)cbTimeMode.SelectedIndex;

            string tz = cbTimeZone.SelectedItem != null
                ? cbTimeZone.SelectedItem.ToString()
                : "UTC+08:00";
            timeInfo.time_zone = NetSdkNative.StringToFixedBytes(tz, NetSdkNative.NET_LEN_32);

            string time = tbLocalTime.Text.Trim();
            if (timeInfo.time_mode == NetSdkNative.NetTimeMode.Local)
            {
                if (!DateTime.TryParseExact(time, "yyyy-MM-dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out _))
                {
                    error = "Invalid device time. Use format yyyy-MM-dd HH:mm:ss.";
                    return false;
                }
                timeInfo.local_time = NetSdkNative.StringToFixedBytes(time, NetSdkNative.NET_LEN_32);
            }
            else
            {
                // NTP 模式下不回写本地时间，交由设备从 NTP 服务器自行校时
                timeInfo.local_time = new byte[NetSdkNative.NET_LEN_32];
            }

            return true;
        }

        private void ApplyTimeSettings()
        {
            if (!_netConnected) return;

            if (!TryBuildTimeInfoFromUi(out var uiInfo, out string error))
            {
                MessageBox.Show(error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var timeInfo = NetSdkNative.NetTimeInfo.Create();
            int ret = NetSdkNative.net_get_time(_netHandle, ref timeInfo);
            LogApi("net_get_time", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show($"Failed to read current time settings: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            timeInfo.time_mode = uiInfo.time_mode;
            timeInfo.time_zone = uiInfo.time_zone;
            if (uiInfo.time_mode == NetSdkNative.NetTimeMode.Local)
            {
                // 仅 Local 模式下写入手动设置的时间
                timeInfo.local_time = uiInfo.local_time;
            }
            else
            {
                // NTP 模式清空 local_time，避免把 Local 阶段的残留时间写回设备
                timeInfo.local_time = new byte[NetSdkNative.NET_LEN_32];
            }

            ret = NetSdkNative.net_set_time(_netHandle, ref timeInfo);
            LogApi("net_set_time", ret);
            if (NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("Time settings updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDeviceTime();
            }
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LoadNtpServer()
        {
            var ntp = NetSdkNative.NetNtpInfo.Create();
            int ret = NetSdkNative.net_get_ntp_server(_netHandle, ref ntp);
            LogApi("net_get_ntp_server", ret);
            if (NetSdkNative.IsOk(ret))
            {
                tbNtpHost.Text = NetSdkNative.BytesToString(ntp.host_name);
                nudNtpPort.Value = ntp.port_no;
                nudNtpInterval.Value = ntp.syn_interval;
            }
        }

        private uint _netIfId;

        private void btnUpgradeBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Firmware Files|*.bin;*.fw;*.img|All Files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                    tbUpgradePath.Text = ofd.FileName;
            }
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (string.IsNullOrEmpty(tbUpgradePath.Text))
            {
                MessageBox.Show("Select a firmware file first", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Upgrade device firmware?\nDo NOT power off during upgrade.", Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var info = NetSdkNative.NetUpgradeInfo.FromFilePath(tbUpgradePath.Text);
            int ret = NetSdkNative.net_device_upgrade(_netHandle, ref info);
            LogApi("net_device_upgrade", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("Upgrade started", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Upgrade failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnDeviceRestart_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (MessageBox.Show("Restart device?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int ret = NetSdkNative.net_device_restart(_netHandle);
            LogApi("net_device_restart", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("Device restarting", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Restart failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnDeviceReset_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (MessageBox.Show("Reset device to factory defaults?\nAll settings will be lost.", Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            int ret = NetSdkNative.net_device_reset(_netHandle);
            LogApi("net_device_reset", ret);
            if (NetSdkNative.IsOk(ret))
            {
                LoadRanges();
                MessageBox.Show("Device reset", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show($"Reset failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnNetRefresh_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadNetworkInfo();
        }

        private void LoadNetworkInfo()
        {
            ushort outLen;
            var networks = new NetSdkNative.NetNetworkInfo[4];
            for (int i = 0; i < networks.Length; i++)
                networks[i] = NetSdkNative.NetNetworkInfo.Create();

            int ret = NetSdkNative.net_get_networks(_netHandle, networks, (ushort)networks.Length, out outLen);
            LogApi("net_get_networks", ret);
            if (NetSdkNative.IsOk(ret) && outLen > 0)
            {
                var n = networks[0];
                _netIfId = n.id;
                cbNetDhcp.Checked = n.dhcp != 0;
                tbNetIp.Text = NetSdkNative.BytesToString(n.ip);
                tbNetMask.Text = NetSdkNative.BytesToString(n.netmask);
                if (n.gateway_count > 0)
                {
                    byte[] gw = new byte[NetSdkNative.NET_LEN_64];
                    Array.Copy(n.gateway, 0, gw, 0, NetSdkNative.NET_LEN_64);
                    tbNetGateway.Text = NetSdkNative.BytesToString(gw);
                }
                if (n.dns_count > 0)
                {
                    byte[] dn = new byte[NetSdkNative.NET_LEN_64];
                    Array.Copy(n.dns, 0, dn, 0, NetSdkNative.NET_LEN_64);
                    tbNetDns.Text = NetSdkNative.BytesToString(dn);
                }
            }

            var ftp = NetSdkNative.NetFtpInfo.Create();
            ret = NetSdkNative.net_get_ftp(_netHandle, ref ftp);
            LogApi("net_get_ftp", ret);
            if (NetSdkNative.IsOk(ret))
            {
                tbFtpServer.Text = NetSdkNative.BytesToString(ftp.ip);
                nudFtpPort.Value = ftp.port_no;
                tbFtpUser.Text = NetSdkNative.BytesToString(ftp.user_name);
                tbFtpPwd.Text = NetSdkNative.BytesToString(ftp.pwd);
                cbFtpSsl.Checked = ftp.use_ssl != 0;
                cbFtpAnonymous.Checked = ftp.enable_annoy != 0;
                UpdateFtpCredentialFieldsEnabled();
                tbFtpPath.Text = NetSdkNative.BytesToString(ftp.path_template);
            }

            var port = new NetSdkNative.NetPortInfo();
            ret = NetSdkNative.net_get_port(_netHandle, ref port);
            LogApi("net_get_port", ret);
            if (NetSdkNative.IsOk(ret))
            {
                nudHttpPort.Value = port.http_port_no;
                nudRtspPort.Value = port.rtsp_port_no;
                nudRtpPort.Value = port.rtp_port_no;
            }

            if (tbWebhookUrls != null)
            {
                LoadWebhook(false);
                LoadWebhooks(false);
            }
        }

        private void btnNetApply_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            var info = NetSdkNative.NetNetworkInfo.Create();
            int ret = NetSdkNative.net_get_network(_netHandle, _netIfId, ref info);
            LogApi("net_get_network", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show($"Failed to read current network info: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            info.dhcp = (byte)(cbNetDhcp.Checked ? 1 : 0);

            byte[] ipBytes = System.Text.Encoding.UTF8.GetBytes(tbNetIp.Text.Trim());
            Array.Clear(info.ip, 0, info.ip.Length);
            Array.Copy(ipBytes, info.ip, Math.Min(ipBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            byte[] maskBytes = System.Text.Encoding.UTF8.GetBytes(tbNetMask.Text.Trim());
            Array.Clear(info.netmask, 0, info.netmask.Length);
            Array.Copy(maskBytes, info.netmask, Math.Min(maskBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            string gw = tbNetGateway.Text.Trim();
            Array.Clear(info.gateway, 0, info.gateway.Length);
            if (!string.IsNullOrEmpty(gw))
            {
                info.gateway_count = 1;
                byte[] gwBytes = System.Text.Encoding.UTF8.GetBytes(gw);
                Array.Copy(gwBytes, 0, info.gateway, 0, Math.Min(gwBytes.Length, NetSdkNative.NET_LEN_64 - 1));
            }
            else
            {
                info.gateway_count = 0;
            }

            string dns = tbNetDns.Text.Trim();
            Array.Clear(info.dns, 0, info.dns.Length);
            if (!string.IsNullOrEmpty(dns))
            {
                info.dns_count = 1;
                byte[] dnsBytes = System.Text.Encoding.UTF8.GetBytes(dns);
                Array.Copy(dnsBytes, 0, info.dns, 0, Math.Min(dnsBytes.Length, NetSdkNative.NET_LEN_64 - 1));
            }
            else
            {
                info.dns_count = 0;
            }

            ret = NetSdkNative.net_set_network(_netHandle, info.id, ref info);
            LogApi("net_set_network", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("Network updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateFtpCredentialFieldsEnabled()
        {
            bool enabled = !cbFtpAnonymous.Checked;
            tbFtpUser.Enabled = enabled;
            tbFtpPwd.Enabled = enabled;
        }

        private void cbFtpAnonymous_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFtpCredentialFieldsEnabled();
        }

        private NetSdkNative.NetFtpInfo BuildFtpInfoFromUi()
        {
            var ftp = NetSdkNative.NetFtpInfo.Create();
            ftp.use_ssl = (byte)(cbFtpSsl.Checked ? 1 : 0);
            ftp.enable_annoy = (byte)(cbFtpAnonymous.Checked ? 1 : 0);
            ftp.port_no = (ushort)nudFtpPort.Value;

            byte[] ipBytes = System.Text.Encoding.UTF8.GetBytes(tbFtpServer.Text.Trim());
            Array.Copy(ipBytes, ftp.ip, Math.Min(ipBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            byte[] userBytes = System.Text.Encoding.UTF8.GetBytes(tbFtpUser.Text.Trim());
            Array.Copy(userBytes, ftp.user_name, Math.Min(userBytes.Length, NetSdkNative.NET_LEN_32 - 1));

            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(tbFtpPwd.Text);
            Array.Copy(pwdBytes, ftp.pwd, Math.Min(pwdBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(tbFtpPath.Text.Trim());
            Array.Copy(pathBytes, ftp.path_template, Math.Min(pathBytes.Length, NetSdkNative.NET_LEN_128 - 1));

            return ftp;
        }

        private void btnFtpApply_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            var ftp = BuildFtpInfoFromUi();
            int ret = NetSdkNative.net_set_ftp(_netHandle, ref ftp);
            LogApi("net_set_ftp", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("FTP updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnPortApply_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            var port = new NetSdkNative.NetPortInfo();
            port.http_port_no = (ushort)nudHttpPort.Value;
            port.rtsp_port_no = (ushort)nudRtspPort.Value;
            port.rtp_port_no = (ushort)nudRtpPort.Value;

            int ret = NetSdkNative.net_set_port(_netHandle, ref port);
            LogApi("net_set_port", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("Ports updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnUserRefresh_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadUsers();
        }

        private void LoadUsers()
        {
            lvUsers.Items.Clear();

            var users = new NetSdkNative.NetUserInfo[16];
            for (int i = 0; i < users.Length; i++)
                users[i] = NetSdkNative.NetUserInfo.Create();

            ushort outLen;
            int ret = NetSdkNative.net_get_users(_netHandle, users, (ushort)users.Length, out outLen);
            LogApi("net_get_users", ret);
            if (!NetSdkNative.IsOk(ret)) return;

            for (int i = 0; i < outLen; i++)
            {
                var u = users[i];
                string levelStr;
                switch (u.user_level)
                {
                    case NetSdkNative.NetUserLevel.Root: levelStr = "Root"; break;
                    case NetSdkNative.NetUserLevel.Operator: levelStr = "Operator"; break;
                    case NetSdkNative.NetUserLevel.Viewer: levelStr = "Viewer"; break;
                    default: levelStr = u.user_level.ToString(); break;
                }
                var item = new ListViewItem(new[] { u.id.ToString(), NetSdkNative.BytesToString(u.user_name), levelStr });
                item.Tag = u.id;
                lvUsers.Items.Add(item);
            }
        }

        private void lvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUsers.SelectedItems.Count == 0) return;
            var item = lvUsers.SelectedItems[0];
            tbUserName.Text = item.SubItems[1].Text;
            string level = item.SubItems[2].Text;
            for (int i = 0; i < cbUserLevel.Items.Count; i++)
            {
                if (cbUserLevel.Items[i].ToString() == level)
                {
                    cbUserLevel.SelectedIndex = i;
                    break;
                }
            }
            tbUserLoginPwd.Text = "";
            tbUserPwd.Text = "";
        }

        private void btnUserAdd_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (string.IsNullOrEmpty(tbUserName.Text.Trim()))
            {
                MessageBox.Show("Enter a user name", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var info = NetSdkNative.NetUserPwdInfo.Create();
            info.id = 0;
            info.user_level = (NetSdkNative.NetUserLevel)(cbUserLevel.SelectedIndex >= 0 ? cbUserLevel.SelectedIndex : 2);

            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(tbUserName.Text.Trim());
            Array.Copy(nameBytes, info.user_name, Math.Min(nameBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(tbUserPwd.Text);
            Array.Copy(pwdBytes, info.pwd, Math.Min(pwdBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            int ret = NetSdkNative.net_update_user(_netHandle, ref info);
            LogApi("net_update_user (add)", ret);
            if (NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("User added", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnUserUpdate_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (lvUsers.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a user first", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            uint userId = (uint)lvUsers.SelectedItems[0].Tag;
            var info = NetSdkNative.NetUserPwdInfo.Create();
            info.id = userId;
            info.user_level = (NetSdkNative.NetUserLevel)cbUserLevel.SelectedIndex;

            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(tbUserName.Text.Trim());
            Array.Copy(nameBytes, info.user_name, Math.Min(nameBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            byte[] loginPwdBytes = System.Text.Encoding.UTF8.GetBytes(tbUserLoginPwd.Text);
            Array.Copy(loginPwdBytes, info.login_pwd, Math.Min(loginPwdBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(tbUserPwd.Text);
            Array.Copy(pwdBytes, info.pwd, Math.Min(pwdBytes.Length, NetSdkNative.NET_LEN_64 - 1));

            int ret = NetSdkNative.net_update_user(_netHandle, ref info);
            LogApi("net_update_user (update)", ret);
            if (NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("User updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnUserDelete_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (lvUsers.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a user first", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            uint userId = (uint)lvUsers.SelectedItems[0].Tag;
            string userName = lvUsers.SelectedItems[0].SubItems[1].Text;

            if (MessageBox.Show($"Delete user '{userName}'?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int ret = NetSdkNative.net_delete_user(_netHandle, userId);
            LogApi("net_delete_user", ret);
            if (NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("User deleted", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
            else
                MessageBox.Show($"Failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void LoadThermalParams()
        {
            IsGUIRefreshing = true;
            if (!ReloadThermalParamValues())
            {
                IsGUIRefreshing = false;
                return;
            }

            NetSdkNative.NetShutterMode shutterMode;
            int retSM = NetSdkNative.net_get_thermal_shutter_mode(_netHandle, out shutterMode);
            LogApi("net_get_thermal_shutter_mode", retSM);
            if (NetSdkNative.IsOk(retSM))
                cbShutterMode.SelectedIndex = (int)shutterMode;

            ushort shutterMin;
            int retST = NetSdkNative.net_get_thermal_shutter_time(_netHandle, out shutterMin);
            LogApi("net_get_thermal_shutter_time", retST);
            if (NetSdkNative.IsOk(retST))
                nudShutterTime.Value = shutterMin;

            var unitInfo = new NetSdkNative.NetUnitInfo();
            int retUnit = NetSdkNative.net_get_thermal_unit(_netHandle, ref unitInfo);
            LogApi("net_get_thermal_unit", retUnit);
            if (NetSdkNative.IsOk(retUnit))
            {
                cbTempUnit.SelectedIndex = (int)unitInfo.temp_unit;
                cbDistUnit.SelectedIndex = (int)unitInfo.distance_unit;
                _activeTempUnit = unitInfo.temp_unit;
                _activeDistUnit = unitInfo.distance_unit;
            }

            UpdateThermalParamUnitLabels();
            IsGUIRefreshing = false;
        }

        private bool ReloadThermalParamValues()
        {
            var p = new NetSdkNative.NetThermalParams();
            int ret = NetSdkNative.net_get_thermal_params(_netHandle, ref p);
            LogApi("net_get_thermal_params", ret);
            if (!NetSdkNative.IsOk(ret)) return false;

            nudEmissivity.Value = Clamp((decimal)p.emissivity, nudEmissivity.Minimum, nudEmissivity.Maximum);
            nudReflTemp.Value = Clamp((decimal)p.refl_temp, nudReflTemp.Minimum, nudReflTemp.Maximum);
            nudDistance.Value = Clamp((decimal)p.distance, nudDistance.Minimum, nudDistance.Maximum);
            nudAtmTemp.Value = Clamp((decimal)p.atm_temp, nudAtmTemp.Minimum, nudAtmTemp.Maximum);
            nudRelHumidity.Value = Clamp((decimal)(p.rel_humidity * 100f), nudRelHumidity.Minimum, nudRelHumidity.Maximum);
            return true;
        }

        private void UpdateThermalParamUnitLabels()
        {
            string tempUnit = GetTempUnitLabel(GetSelectedTempUnit());
            lblReflTempUnit.Text = tempUnit;
            lblAtmTempUnit.Text = tempUnit;
            lblDistanceUnit.Text = GetDistanceUnitLabel(GetSelectedDistanceUnit());
            lblMaxUnit.Text = tempUnit;
            lblMinUnit.Text = tempUnit;
            lblAlarmTempUnit.Text = tempUnit;
            lblAlarmThreshTempUnit.Text = tempUnit;
            lblAlarmTrigTempUnit.Text = tempUnit;
        }

        private NetSdkNative.NetTempUnit GetSelectedTempUnit()
        {
            return cbTempUnit.SelectedIndex >= 0
                ? (NetSdkNative.NetTempUnit)cbTempUnit.SelectedIndex
                : NetSdkNative.NetTempUnit.Centigrade;
        }

        private NetSdkNative.NetDistanceUnit GetSelectedDistanceUnit()
        {
            return cbDistUnit.SelectedIndex >= 0
                ? (NetSdkNative.NetDistanceUnit)cbDistUnit.SelectedIndex
                : NetSdkNative.NetDistanceUnit.Meter;
        }

        private static string GetTempUnitLabel(NetSdkNative.NetTempUnit unit)
        {
            switch (unit)
            {
                case NetSdkNative.NetTempUnit.Kelvin:
                    return "K";
                case NetSdkNative.NetTempUnit.Fahrenheit:
                    return "°F";
                default:
                    return "°C";
            }
        }

        private string FormatMarkerTemperature(float temperature)
        {
            return temperature.ToString("F1") + GetTempUnitLabel(GetSelectedTempUnit());
        }

        private static string GetMarkerTypeDisplayName(NetSdkNative.NetMarkerType type)
        {
            switch (type)
            {
                case NetSdkNative.NetMarkerType.Spot:
                    return "Spot(1)";
                case NetSdkNative.NetMarkerType.Polygon:
                    return "Polygon(5)";
                case NetSdkNative.NetMarkerType.Polyline:
                    return "Polyline(6)";
                default:
                    return type.ToString() + "(" + (int)type + ")";
            }
        }

        private static string GetDistanceUnitLabel(NetSdkNative.NetDistanceUnit unit)
        {
            return unit == NetSdkNative.NetDistanceUnit.Foot ? "ft" : "m";
        }

        private string FormatTemperatureRange(float min, float max)
        {
            return $"{min:F0}~{max:F0}{GetTempUnitLabel(GetSelectedTempUnit())}";
        }

        private void cbTempUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing) return;
            UpdateThermalParamUnitLabels();
        }

        private void cbDistUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing) return;
            UpdateThermalParamUnitLabels();
        }

        private struct LocalParamsSnapshot
        {
            public string Name;
            public uint LocalParams;
            public float Emissivity;
            public float Distance;
            public float ReflTemp;
        }

        private System.Collections.Generic.List<LocalParamsSnapshot> SnapshotEnabledLocalParams()
        {
            var list = new System.Collections.Generic.List<LocalParamsSnapshot>();
            lock (_markerLock)
            {
                for (int i = 0; i < _markerCount; i++)
                {
                    var m = _markers[i];
                    if (m.local_params == 0)
                        continue;
                    list.Add(new LocalParamsSnapshot
                    {
                        Name = GetMarkerNameString(m),
                        LocalParams = m.local_params,
                        Emissivity = m.emissivity,
                        Distance = m.distance,
                        ReflTemp = m.refl_temp
                    });
                }
            }
            return list;
        }

        private void RestoreLocalParamsAfterUnitChange(
            System.Collections.Generic.List<LocalParamsSnapshot> snapshot,
            NetSdkNative.NetTempUnit oldTempUnit,
            NetSdkNative.NetDistanceUnit oldDistUnit,
            NetSdkNative.NetTempUnit newTempUnit,
            NetSdkNative.NetDistanceUnit newDistUnit)
        {
            if (snapshot == null || snapshot.Count == 0)
                return;

            bool changed = false;
            lock (_markerLock)
            {
                for (int i = 0; i < _markerCount; i++)
                {
                    string name = GetMarkerNameString(_markers[i]);
                    LocalParamsSnapshot? matched = null;
                    for (int s = 0; s < snapshot.Count; s++)
                    {
                        if (snapshot[s].Name == name)
                        {
                            matched = snapshot[s];
                            break;
                        }
                    }
                    if (!matched.HasValue)
                        continue;

                    var snap = matched.Value;
                    _markers[i].local_params = snap.LocalParams != 0 ? 1u : 0u;
                    _markers[i].emissivity = snap.Emissivity;
                    _markers[i].distance = ConvertDistance(snap.Distance, oldDistUnit, newDistUnit);
                    _markers[i].refl_temp = ConvertTemperature(snap.ReflTemp, oldTempUnit, newTempUnit);
                    changed = true;
                }
            }

            if (!changed)
                return;

            PushMarkers();
            RefreshMarkerListView();
        }

        private static float ConvertTemperature(float value,
            NetSdkNative.NetTempUnit from, NetSdkNative.NetTempUnit to)
        {
            if (from == to)
                return value;

            float kelvin;
            switch (from)
            {
                case NetSdkNative.NetTempUnit.Kelvin:
                    kelvin = value;
                    break;
                case NetSdkNative.NetTempUnit.Fahrenheit:
                    kelvin = (value - 32f) * 5f / 9f + 273.15f;
                    break;
                default:
                    kelvin = value + 273.15f;
                    break;
            }

            switch (to)
            {
                case NetSdkNative.NetTempUnit.Kelvin:
                    return kelvin;
                case NetSdkNative.NetTempUnit.Fahrenheit:
                    return (kelvin - 273.15f) * 9f / 5f + 32f;
                default:
                    return kelvin - 273.15f;
            }
        }

        private static float ConvertDistance(float value,
            NetSdkNative.NetDistanceUnit from, NetSdkNative.NetDistanceUnit to)
        {
            if (from == to)
                return value;

            float meters = from == NetSdkNative.NetDistanceUnit.Foot
                ? value * 0.3048f
                : value;
            return to == NetSdkNative.NetDistanceUnit.Foot
                ? meters / 0.3048f
                : meters;
        }

        private void LoadRanges()
        {
            tscbFileFormat.Items.Clear();
            tscbFileFormat.SelectedIndexChanged -= tscbFileFormat_SelectedIndexChanged;
            try
            {
                var lensInfo = NetSdkNative.NetLensInfo.Create();
                int ret = NetSdkNative.net_get_thermal_lens_info(_netHandle, ref lensInfo);
                LogApi("net_get_thermal_lens_info", ret);
                if (!NetSdkNative.IsOk(ret) || lensInfo.ranges_count == 0)
                    return;

                for (int i = 0; i < lensInfo.ranges_count; i++)
                {
                    var r = lensInfo.range_map[i];
                    string label = FormatTemperatureRange(r.range.min, r.range.max);
                    tscbFileFormat.Items.Add(new RangeItem { Id = r.id, Label = label });
                }

                ushort currentRange;
                ret = NetSdkNative.net_get_thermal_range(_netHandle, out currentRange);
                LogApi("net_get_thermal_range", ret);
                if (NetSdkNative.IsOk(ret))
                {
                    for (int i = 0; i < tscbFileFormat.Items.Count; i++)
                    {
                        if (((RangeItem)tscbFileFormat.Items[i]).Id == currentRange)
                        {
                            tscbFileFormat.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            finally
            {
                tscbFileFormat.SelectedIndexChanged += tscbFileFormat_SelectedIndexChanged;
            }
        }

        private void tscbFileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_netConnected || tscbFileFormat.SelectedItem == null)
                return;

            var item = (RangeItem)tscbFileFormat.SelectedItem;
            ushort rangeId = item.Id;
            int retRange = NetSdkNative.net_set_thermal_range(_netHandle, ref rangeId);
            LogApi("net_set_thermal_range", retRange);
        }

        private struct RangeItem
        {
            public ushort Id;
            public string Label;
            public override string ToString() { return Label; }
        }

        private void LoadPalettes()
        {
            bool wasRefreshing = IsGUIRefreshing;
            IsGUIRefreshing = true;
            try
            {
                tscbPalette.Items.Clear();

                ushort outLen;
                var names = new NetSdkNative.NetPaletteName[32];
                for (int i = 0; i < names.Length; i++)
                    names[i] = NetSdkNative.NetPaletteName.Create();

                int ret = NetSdkNative.net_get_thermal_palettes(_netHandle, names, (ushort)names.Length, out outLen);
                LogApi("net_get_thermal_palettes", ret);
                if (!NetSdkNative.IsOk(ret) || outLen == 0)
                    return;

                for (int i = 0; i < outLen; i++)
                {
                    string name = NetSdkNative.BytesToString(names[i].name);
                    tscbPalette.Items.Add(new PresetPaletteItem { RefNo = names[i].ref_no, Name = name });
                }

                var current = NetSdkNative.NetPalette.Create();
                ret = NetSdkNative.net_get_thermal_palette(_netHandle, ref current);
                LogApi("net_get_thermal_palette", ret);
                if (NetSdkNative.IsOk(ret))
                {
                    for (int i = 0; i < tscbPalette.Items.Count; i++)
                    {
                        if (((PresetPaletteItem)tscbPalette.Items[i]).RefNo == current.ref_no)
                        {
                            tscbPalette.SelectedIndex = i;
                            break;
                        }
                    }
                    tsbInverted.Checked = current.inverted != 0;
                    UpdatePaletteImage(current);
                }
            }
            finally
            {
                IsGUIRefreshing = wasRefreshing;
            }
        }

        private void UpdatePaletteImage(NetSdkNative.NetPalette palette)
        {
            int colorCount = palette.color_count > 0 ? palette.color_count : 256;

            // Update the vertical color bar (high temp at top, low temp at bottom)
            int w = pbPalleteImage.Width;
            int h = pbPalleteImage.Height;
            if (w > 0 && h > 0)
            {
                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                {
                    for (int y = 0; y < h; y++)
                    {
                        int idx = (h - 1 - y) * colorCount / h;
                        if (idx >= colorCount) idx = colorCount - 1;
                        uint argb = palette.colors[idx] | 0xFF000000;
                        Color c = Color.FromArgb((int)argb);
                        using (var pen = new Pen(c))
                        {
                            g.DrawLine(pen, 0, y, w, y);
                        }
                    }
                }
                var old = pbPalleteImage.Image;
                pbPalleteImage.Image = bmp;
                old?.Dispose();
            }

            // Update the toolbar palette thumbnail (high temp at top, low temp at bottom)
            int tw = tsbPalette.Width > 0 ? tsbPalette.Width : 28;
            int th = tsbPalette.Height > 0 ? tsbPalette.Height : 28;
            Bitmap thumb = new Bitmap(tw, th, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(thumb))
            {
                for (int y = 0; y < th; y++)
                {
                    int idx = (th - 1 - y) * colorCount / th;
                    if (idx >= colorCount) idx = colorCount - 1;
                    uint argb = palette.colors[idx] | 0xFF000000;
                    Color c = Color.FromArgb((int)argb);
                    using (var pen = new Pen(c))
                    {
                        g.DrawLine(pen, 0, y, tw, y);
                    }
                }
            }
            tsbPalette.Image = thumb;
        }

        private NetSdkNative.NetStreamInfo _currentStream;

        private bool TryGetAvailableStreams(out NetSdkNative.NetStreamInfo[] streams, out ushort streamCount)
        {
            streams = new NetSdkNative.NetStreamInfo[4];
            for (int i = 0; i < streams.Length; i++)
                streams[i] = NetSdkNative.NetStreamInfo.Create();

            int ret = NetSdkNative.net_get_streams(_netHandle, streams, (ushort)streams.Length, out streamCount);
            LogApi("net_get_streams", ret);
            return NetSdkNative.IsOk(ret) && streamCount > 0;
        }

        private static bool StreamIdExists(uint streamId, NetSdkNative.NetStreamInfo[] streams, ushort streamCount)
        {
            for (int i = 0; i < streamCount; i++)
            {
                if (streams[i].id == streamId)
                    return true;
            }
            return false;
        }

        private bool TryLoadCurrentStream(uint streamId)
        {
            _currentStream = NetSdkNative.NetStreamInfo.Create();
            int ret = NetSdkNative.net_get_stream(_netHandle, streamId, ref _currentStream);
            LogApi("net_get_stream", ret, "stream_id=" + streamId);
            return NetSdkNative.IsOk(ret);
        }

        private void RefreshStreamIdCombo(ushort streamCount, NetSdkNative.NetStreamInfo[] streams)
        {
            _updatingStreamIdCombo = true;
            try
            {
                tscbStreamId.Items.Clear();
                for (int i = 0; i < streamCount; i++)
                {
                    var stream = streams[i];
                    tscbStreamId.Items.Add(new StreamIdItem(stream.id, stream.resolution_width, stream.resolution_height));
                }
            }
            finally
            {
                _updatingStreamIdCombo = false;
            }

            if (tscbStreamId.Items.Count == 0)
                return;

            if (_currentStreamId == 0 || !StreamIdExists(_currentStreamId, streams, streamCount))
                _currentStreamId = ((StreamIdItem)tscbStreamId.Items[0]).Id;

            SetStreamIdCombo(_currentStreamId);
        }

        private void SetStreamIdCombo(uint streamId)
        {
            _updatingStreamIdCombo = true;
            try
            {
                for (int i = 0; i < tscbStreamId.Items.Count; i++)
                {
                    var item = tscbStreamId.Items[i] as StreamIdItem;
                    if (item != null && item.Id == streamId)
                    {
                        if (tscbStreamId.SelectedIndex != i)
                            tscbStreamId.SelectedIndex = i;
                        return;
                    }
                }
            }
            finally
            {
                _updatingStreamIdCombo = false;
            }
        }

        private StreamIdItem GetSelectedStreamIdItem()
        {
            return tscbStreamId.SelectedItem as StreamIdItem;
        }

        private void RequestStreamDataLog()
        {
            _pendingStreamDataLogCount = 2;
        }

        private static bool IsValidStreamFrame(ref NetSdkNative.NetStreamData data)
        {
            if (data.data == IntPtr.Zero || data.data_length == 0)
                return false;

            if (data.stream_type != NetSdkNative.NetStreamType.H264 && (data.width == 0 || data.height == 0))
                return false;

            return true;
        }

        private static string BuildStreamDataSummary(ref NetSdkNative.NetStreamData data)
        {
            return string.Format(
                "stream_id={0} stream_type={1} width={2} height={3} data=0x{4:X} data_length={5} timestamp_us={6} key_frame={7} ({8})",
                data.stream_id,
                data.stream_type,
                data.width,
                data.height,
                data.data.ToInt64(),
                data.data_length,
                data.timestamp_us,
                data.key_frame,
                data.key_frame != 0 ? "true" : "false");
        }

        private void TryLogStreamDataSample(ref NetSdkNative.NetStreamData data)
        {
            if (_pendingStreamDataLogCount <= 0 || !IsValidStreamFrame(ref data))
                return;

            int frameIndex = 3 - _pendingStreamDataLogCount;
            _pendingStreamDataLogCount--;
            string label = frameIndex == 1 ? "1st" : "2nd";
            LogApiCall("net_stream_data (" + label + ") " + BuildStreamDataSummary(ref data));
        }

        private NetSdkNative.NetStreamType GetSelectedStreamType()
        {
            switch (tscbStreamType.SelectedItem as string)
            {
                case "RGBA":
                    return NetSdkNative.NetStreamType.RGBA;
                case "YUV422":
                    return NetSdkNative.NetStreamType.YUV422;
                case "H264":
                    return NetSdkNative.NetStreamType.H264;
                case "BGRA":
                default:
                    return NetSdkNative.NetStreamType.BGRA;
            }
        }

        private static int GetStreamTypeIndex(NetSdkNative.NetStreamType streamType)
        {
            switch (streamType)
            {
                case NetSdkNative.NetStreamType.RGBA:
                    return 1;
                case NetSdkNative.NetStreamType.YUV422:
                    return 2;
                case NetSdkNative.NetStreamType.H264:
                    return 3;
                case NetSdkNative.NetStreamType.BGRA:
                default:
                    return 0;
            }
        }

        private void SetStreamTypeCombo(NetSdkNative.NetStreamType streamType)
        {
            _updatingStreamTypeCombo = true;
            try
            {
                int index = GetStreamTypeIndex(streamType);
                if (tscbStreamType.SelectedIndex != index)
                    tscbStreamType.SelectedIndex = index;
            }
            finally
            {
                _updatingStreamTypeCombo = false;
            }
        }

        private void StartVideoStream()
        {
            ushort streamCount;
            NetSdkNative.NetStreamInfo[] streams;
            if (!TryGetAvailableStreams(out streams, out streamCount))
            {
                MessageBox.Show("Get streams failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_acousticActive)
            {
                for (int i = 0; i < streamCount; i++)
                {
                    string codec = NetSdkNative.BytesToString(streams[i].codec_type);
                    if (codec.IndexOf("264", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _currentStreamType = NetSdkNative.NetStreamType.H264;
                        break;
                    }
                }
            }

            RefreshStreamIdCombo(streamCount, streams);

            if (!TryLoadCurrentStream(_currentStreamId))
            {
                MessageBox.Show("Get stream failed: " + _currentStreamId, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetStreamTypeCombo(_currentStreamType);

            if (StartVideoStreamCallback(_currentStreamType, true))
                RequestStreamDataLog();
        }

        private bool StartVideoStreamCallback(NetSdkNative.NetStreamType streamType, bool showError)
        {
            if (streamType == NetSdkNative.NetStreamType.H264 && !EnsureH264Decoder(showError))
                return false;

            StopVideoStream(streamType);

            _streamCb = OnStreamDataReceived;
            int ret = NetSdkNative.net_start_stream_play(_netHandle, ref _currentStream, streamType, _streamCb);
            LogApi("net_start_stream_play", ret, $"stream_id={_currentStream.id} type={streamType}");
            if (!NetSdkNative.TryGetSdkHandle(ret, out _streamPlayHandle))
            {
                if (showError)
                    MessageBox.Show("Start stream play failed: " + ret, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _streamCb = null;
                _streamPlayHandle = IntPtr.Zero;
                return false;
            }

            ResetStreamCallbackMode();
            UpdateStreamCallbackButtonUi();
            return true;
        }

        private void ReconnectFull(string ip, string portStr, string user, string pwd)
        {
            try
            {
                Thread.Sleep(2000);

                const int maxRetries = 3;
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        LogSafe($"[Reconnect] Attempt {attempt}/{maxRetries} — login new handle...");

                        ushort port;
                        if (!ushort.TryParse(portStr, out port)) port = 80;
                        var loginInfo = NetSdkNative.NetLoginInfo.Create(ip, port, user, pwd, 5000);

                        IntPtr newHandle;
                        int ret = NetSdkNative.net_login(ref loginInfo, out newHandle);
                        LogSafe($"[Reconnect] net_login(new) → {ret}");
                        if (!NetSdkNative.IsOk(ret))
                        {
                            Thread.Sleep(2000);
                            continue;
                        }

                        KeepCallbacksAlive();
                        _netHandle = newHandle;

                        _exceptionCb = OnNetExceptionReceived;
                        ret = NetSdkNative.net_set_exception_callback(_netHandle, _exceptionCb);
                        LogSafe($"[Reconnect] net_set_exception_callback → {ret}");

                        if (_acousticActive)
                        {
                            ret = RestartAcousticDataStreamForReconnect();
                            LogSafe($"[Reconnect] net_start_acoustic_data_stream → {ret}");
                        }
                        else
                        {
                            ret = RestartThermalDataStreamForReconnect();
                            LogSafe($"[Reconnect] net_start_thermal_data_stream → {ret}");
                        }

                        ushort outLen;
                        NetSdkNative.NetStreamInfo[] streams;
                        if (!TryGetAvailableStreams(out streams, out outLen))
                        {
                            Thread.Sleep(2000);
                            continue;
                        }

                        if (!TryLoadCurrentStream(_currentStreamId))
                        {
                            _currentStreamId = streams[0].id;
                            if (!TryLoadCurrentStream(_currentStreamId))
                            {
                                Thread.Sleep(2000);
                                continue;
                            }
                        }

                        if (_currentStreamType == NetSdkNative.NetStreamType.H264 && !EnsureH264Decoder(false))
                        {
                            LogSafe("[Reconnect] H264 decoder unavailable");
                            Thread.Sleep(2000);
                            continue;
                        }

                        _streamCb = OnStreamDataReceived;
                        ret = NetSdkNative.net_start_stream_play(
                            _netHandle, ref _currentStream, _currentStreamType, _streamCb);
                        LogSafe($"[Reconnect] net_start_stream_play stream_id={_currentStreamId} type={_currentStreamType} → {ret}");
                        if (!NetSdkNative.TryGetSdkHandle(ret, out _streamPlayHandle))
                            ret = -1;

                        if (ret > 0 && _streamPlayHandle != IntPtr.Zero)
                        {
                            _netConnected = true;
                            ResetStreamCallbackMode();
                            RequestStreamDataLog();
                            BeginInvoke(new Action(() =>
                            {
                                Text = $"IRtekNetSDKDemo - Connected {ip}";
                                tsbConnect.BackColor = System.Drawing.Color.LightGreen;
                                RefreshStreamIdCombo(outLen, streams);
                                SetStreamTypeCombo(_currentStreamType);
                                UpdateStreamCallbackButtonUi();
                            }));
                            LogSafe("[Reconnect] Reconnected with new handle successfully");
                            return;
                        }

                        KeepCallbacksAlive();
                        Thread.Sleep(2000);
                    }
                    catch (Exception ex)
                    {
                        LogSafe($"[Reconnect] Attempt {attempt} error: {ex.Message}");
                        Thread.Sleep(2000);
                    }
                }

                BeginInvoke(new Action(() =>
                {
                    Text = $"IRtekNetSDKDemo - Reconnect failed";
                    tsbConnect.BackColor = System.Drawing.Color.Salmon;
                }));
                LogSafe("[Reconnect] All attempts failed — please reconnect manually");
            }
            finally
            {
                Interlocked.Exchange(ref _reconnecting, 0);
            }
        }

        private int RestartThermalDataStreamForReconnect()
        {
            StopThermalDataStream();
            return ApplyThermalStreamConfig(GetSelectedThermalStreamPreset());
        }

        private void KeepCallbacksAlive()
        {
            lock (_staleCallbacks)
            {
                if (_exceptionCb != null) _staleCallbacks.Add(_exceptionCb);
                if (_alarmCb != null) _staleCallbacks.Add(_alarmCb);
                if (_streamCb != null) _staleCallbacks.Add(_streamCb);
                if (_thermalDataCb != null) _staleCallbacks.Add(_thermalDataCb);
                if (_acousticDataCb != null) _staleCallbacks.Add(_acousticDataCb);
            }
        }

        private void LogSafe(string message)
        {
            BeginInvoke(new Action(() => AppendApiLog(message)));
        }

        private void StopVideoStream()
        {
            StopVideoStream(_currentStreamType);
        }

        private void StopVideoStream(NetSdkNative.NetStreamType streamType)
        {
            if (_streamPlayHandle != IntPtr.Zero)
            {
                int ret = NetSdkNative.net_stop_stream_play(_streamPlayHandle);
                LogApi("net_stop_stream_play", ret, $"stop stream_id={_currentStream.id} type={streamType}");
                _streamPlayHandle = IntPtr.Zero;
            }
            _streamCb = null;
            ResetStreamCallbackMode();
            UpdateStreamCallbackButtonUi();
        }

        private void DisposeH264Decoder()
        {
            lock (_h264DecoderLock)
            {
                if (_h264Decoder != null)
                {
                    _h264Decoder.Dispose();
                    _h264Decoder = null;
                }
            }
        }

        private void OnStreamDataReceived(ref NetSdkNative.NetStreamData data)
        {
            try
            {
                TryLogStreamDataSample(ref data);

                int width = data.width;
                int height = data.height;
                if (data.data == IntPtr.Zero || data.data_length == 0)
                    return;

                if (_acousticActive && !_acousticDeviceVideoPreview)
                    return;

                if (data.stream_type != NetSdkNative.NetStreamType.H264 && (width == 0 || height == 0))
                    return;

                Bitmap bmp = CreateBitmapFromStreamData(ref data, ref width, ref height);
                if (bmp == null)
                    return;

                _streamWidth = width;
                _streamHeight = height;

                if (pbThermalImage.IsHandleCreated)
                {
                    pbThermalImage.BeginInvoke(new Action(() =>
                    {
                        if (IsDisposed || (_acousticActive && !_acousticDeviceVideoPreview))
                        {
                            bmp.Dispose();
                            return;
                        }
                        var old = pbThermalImage.Image;
                        pbThermalImage.Image = bmp;
                        old?.Dispose();
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Stream data error: " + ex.Message);
            }
        }

        private void OnStreamDataStatsOnly(ref NetSdkNative.NetStreamData data)
        {
            try
            {
                if (!IsValidStreamFrame(ref data))
                    return;

                int frameNo = System.Threading.Interlocked.Increment(ref _streamStatsFrameCount);
                if (frameNo > 2 && frameNo % 30 != 0)
                    return;

                string summary = BuildStreamDataSummary(ref data);
                if (pbThermalImage.IsHandleCreated)
                {
                    pbThermalImage.BeginInvoke(new Action(() =>
                        AppendApiLog("net_stream_data [stats] #" + frameNo + " " + summary)));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Stream stats error: " + ex.Message);
            }
        }

        private void ResetStreamCallbackMode()
        {
            _streamStatsOnlyMode = false;
            _streamStatsFrameCount = 0;
        }

        private void UpdateStreamCallbackButtonUi()
        {
            if (tsbStreamCallbackToggle == null)
                return;

            bool canToggle = _netConnected && _streamPlayHandle != IntPtr.Zero;
            tsbStreamCallbackToggle.Enabled = canToggle;

            _updatingStreamCallbackToggle = true;
            try
            {
                tsbStreamCallbackToggle.Checked = canToggle && _streamStatsOnlyMode;
            }
            finally
            {
                _updatingStreamCallbackToggle = false;
            }

            tsbStreamCallbackToggle.BackColor = _streamStatsOnlyMode
                ? System.Drawing.Color.LightGoldenrodYellow
                : System.Drawing.SystemColors.Control;
        }

        private bool ApplyStreamCallback(bool statsOnly, bool showError)
        {
            if (_streamPlayHandle == IntPtr.Zero)
                return false;

            bool previousMode = _streamStatsOnlyMode;
            KeepCallbacksAlive();
            _streamStatsOnlyMode = statsOnly;
            _streamStatsFrameCount = 0;
            if (statsOnly)
                _streamCb = OnStreamDataStatsOnly;
            else
                _streamCb = OnStreamDataReceived;

            int ret = NetSdkNative.net_set_stream_callback(_streamPlayHandle, _streamCb);
            LogApi("net_set_stream_callback", ret, statsOnly ? "mode=stats" : "mode=display");
            if (!NetSdkNative.IsOk(ret))
            {
                _streamStatsOnlyMode = previousMode;
                if (previousMode)
                    _streamCb = OnStreamDataStatsOnly;
                else
                    _streamCb = OnStreamDataReceived;
                if (showError)
                {
                    MessageBox.Show(
                        "Set stream callback failed: " + ret + " (" + (NetSdkNative.NetErrorCode)ret + ")",
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                return false;
            }

            UpdateStreamCallbackButtonUi();
            return true;
        }

        private void tsbStreamCallbackToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (_updatingStreamCallbackToggle)
                return;

            if (!_netConnected || _streamPlayHandle == IntPtr.Zero)
            {
                UpdateStreamCallbackButtonUi();
                return;
            }

            bool wantStats = tsbStreamCallbackToggle.Checked;
            if (wantStats == _streamStatsOnlyMode)
                return;

            if (!ApplyStreamCallback(wantStats, true))
                UpdateStreamCallbackButtonUi();
        }

        private Bitmap CreateBitmapFromStreamData(ref NetSdkNative.NetStreamData data, ref int width, ref int height)
        {
            int pixelCount = width * height;
            int bgraLength = pixelCount * 4;

            switch (data.stream_type)
            {
                case NetSdkNative.NetStreamType.BGRA:
                    if (data.data_length < bgraLength)
                        return null;
                    return CreateBitmapFromBgra(data.data, width, height, width * 4);

                case NetSdkNative.NetStreamType.RGBA:
                    if (data.data_length < bgraLength)
                        return null;
                    return CreateBitmapFromBgra(data.data, width, height, width * 4);

                case NetSdkNative.NetStreamType.YUV422:
                    if (data.data_length < pixelCount * 2)
                        return null;
                    return CreateBitmapFromBgra(ConvertYuv422ToBgra(data.data, width, height), width, height);

                case NetSdkNative.NetStreamType.H264:
                    return CreateBitmapFromH264(data.data, (int)data.data_length, ref width, ref height);

                default:
                    return null;
            }
        }

        private Bitmap CreateBitmapFromBgra(IntPtr bgraData, int width, int height, int stride)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                if (stride == width * 4)
                {
                    CopyMemory(bmpData.Scan0, bgraData, (uint)(height * stride));
                }
                else
                {
                    for (int y = 0; y < height; y++)
                    {
                        IntPtr src = IntPtr.Add(bgraData, y * stride);
                        IntPtr dst = IntPtr.Add(bmpData.Scan0, y * bmpData.Stride);
                        CopyMemory(dst, src, (uint)(width * 4));
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }

            return bmp;
        }

        private Bitmap CreateBitmapFromBgra(byte[] bgraData, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                Marshal.Copy(bgraData, 0, bmpData.Scan0, bgraData.Length);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }

            return bmp;
        }

        private byte[] ConvertYuv422ToBgra(IntPtr yuvData, int width, int height)
        {
            int pixelCount = width * height;
            byte[] yuv = new byte[pixelCount * 2];
            Marshal.Copy(yuvData, yuv, 0, yuv.Length);
            byte[] bgra = new byte[pixelCount * 4];

            for (int src = 0, dst = 0; src + 3 < yuv.Length && dst + 7 < bgra.Length; src += 4, dst += 8)
            {
                byte y0 = yuv[src];
                byte u = yuv[src + 1];
                byte y1 = yuv[src + 2];
                byte v = yuv[src + 3];

                WriteYuvPixelAsBgra(y0, u, v, bgra, dst);
                WriteYuvPixelAsBgra(y1, u, v, bgra, dst + 4);
            }

            return bgra;
        }

        private static void WriteYuvPixelAsBgra(byte y, byte u, byte v, byte[] bgra, int offset)
        {
            int c = y - 16;
            int d = u - 128;
            int e = v - 128;
            if (c < 0) c = 0;

            bgra[offset] = ClipToByte((298 * c + 516 * d + 128) >> 8);
            bgra[offset + 1] = ClipToByte((298 * c - 100 * d - 208 * e + 128) >> 8);
            bgra[offset + 2] = ClipToByte((298 * c + 409 * e + 128) >> 8);
            bgra[offset + 3] = 255;
        }

        private static byte ClipToByte(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return (byte)value;
        }

        private Bitmap CreateBitmapFromH264(IntPtr h264Data, int dataLength, ref int width, ref int height)
        {
            lock (_h264DecoderLock)
            {
                if (!EnsureH264Decoder(false))
                    return null;

                IntPtr bgraData;
                int decodedWidth;
                int decodedHeight;
                int stride;
                if (!_h264Decoder.Decode(h264Data, dataLength, out bgraData, out decodedWidth, out decodedHeight, out stride))
                    return null;

                width = decodedWidth;
                height = decodedHeight;
                return CreateBitmapFromBgra(bgraData, decodedWidth, decodedHeight, stride);
            }
        }

        private bool EnsureH264Decoder(bool showError)
        {
            lock (_h264DecoderLock)
            {
                if (_h264Decoder != null)
                    return true;

                try
                {
                    var decoder = new FFmpegDecoder();
                    if (decoder.Initialize())
                    {
                        _h264Decoder = decoder;
                        return true;
                    }

                    decoder.Dispose();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("H264 decoder init error: " + ex.Message);
                }

                if (showError)
                    MessageBox.Show("H264 decoder initialization failed.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        #endregion

        #region Device Name, Record, Display

        private void btnSetDeviceName_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            var info = NetSdkNative.NetDeviceInfo.Create();
            int ret = NetSdkNative.net_get_device_info(_netHandle, ref info);
            LogApi("net_get_device_info", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show($"Get device info failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(tbDeviceName.Text);
            Array.Clear(info.name, 0, info.name.Length);
            Array.Copy(nameBytes, info.name, Math.Min(nameBytes.Length, info.name.Length - 1));
            ret = NetSdkNative.net_set_device_info(_netHandle, ref info);
            LogApi("net_set_device_info", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("Device name updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Set device info failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool _recording;

        private void tsbRecord_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            if (!_recording)
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select folder for recording";
                    if (fbd.ShowDialog() != DialogResult.OK) return;
                    string path = System.IO.Path.Combine(fbd.SelectedPath,
                        "record_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".mp4");
                    byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(path);
                    if (_streamPlayHandle == IntPtr.Zero)
                    {
                        MessageBox.Show("Start video stream before recording.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int ret = NetSdkNative.net_start_stream_record(_streamPlayHandle, pathBytes, (ushort)pathBytes.Length);
                    LogApi("net_start_stream_record", ret);
                    if (NetSdkNative.IsOk(ret))
                    {
                        _recording = true;
                        tsbRecord.Text = "Stop Rec";
                        tsbRecord.BackColor = System.Drawing.Color.LightCoral;
                    }
                    else
                        MessageBox.Show($"Start record failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                int ret = NetSdkNative.net_stop_stream_record(_streamPlayHandle);
                LogApi("net_stop_stream_record", ret);
                _recording = false;
                tsbRecord.Text = "Record";
                tsbRecord.BackColor = System.Drawing.SystemColors.Control;
                if (!NetSdkNative.IsOk(ret))
                    MessageBox.Show($"Stop record failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisplayGet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadDisplaySettings();
        }

        private void LoadDisplaySettings()
        {
            uint channelId;
            if (!TryResolveDisplayChannelId(out channelId)) return;

            var size = new NetSdkNative.NetSize();
            int retSize = NetSdkNative.net_get_thermal_resolution(_netHandle, ref size);
            LogApi("net_get_thermal_resolution", retSize);
            if (NetSdkNative.IsOk(retSize))
            {
                tbChNameX.Maximum = size.width;
                tbChNameY.Maximum = size.height;
                tbTimeX.Maximum = size.width;
                tbTimeY.Maximum = size.height;
                tbCustomX.Maximum = size.width;
                tbCustomY.Maximum = size.height;
            }

            var ch = NetSdkNative.NetChannelInfo.Create();
            int retCh = NetSdkNative.net_get_channel(_netHandle, channelId, ref ch);
            LogApi("net_get_channel", retCh, $"channel_id={channelId}");
            if (NetSdkNative.IsOk(retCh))
            {
                tbChannelName.Text = System.Text.Encoding.UTF8.GetString(ch.name).TrimEnd('\0');
            }

            var ovl = NetSdkNative.NetOverlayInfo.Create();
            int retOvl = NetSdkNative.net_get_overlay(_netHandle, channelId, ref ovl);
            LogApi("net_get_overlay", retOvl, $"channel_id={channelId}");
            if (NetSdkNative.IsOk(retOvl))
            {
                cbChNameEnabled.Checked = ovl.channel_name_overlay.enabled != 0;
                cbChNameAlign.SelectedIndex = (int)ovl.channel_name_overlay.align;
                cbChNameMode.SelectedIndex = (int)ovl.channel_name_overlay.mode;
                tbChNameX.Value = Math.Min((int)ovl.channel_name_overlay.x, tbChNameX.Maximum);
                tbChNameY.Value = Math.Min((int)ovl.channel_name_overlay.y, tbChNameY.Maximum);
                lblChNameXVal.Text = "x:" + ovl.channel_name_overlay.x;
                lblChNameYVal.Text = "y:" + ovl.channel_name_overlay.y;

                cbTimeEnabled.Checked = ovl.time_overlay.enabled != 0;
                cbTimeAlign.SelectedIndex = (int)ovl.time_overlay.align;
                cbTimeOvlMode.SelectedIndex = (int)ovl.time_overlay.mode;
                tbTimeX.Value = Math.Min((int)ovl.time_overlay.x, tbTimeX.Maximum);
                tbTimeY.Value = Math.Min((int)ovl.time_overlay.y, tbTimeY.Maximum);
                lblTimeXVal.Text = "x:" + ovl.time_overlay.x;
                lblTimeYVal.Text = "y:" + ovl.time_overlay.y;

                cbCustomEnabled.Checked = ovl.custom_overlay.enabled != 0;
                cbCustomAlign.SelectedIndex = (int)ovl.custom_overlay.align;
                cbCustomMode.SelectedIndex = (int)ovl.custom_overlay.mode;
                tbCustomText.Text = NetSdkNative.BytesToString(ovl.custom_overlay.text);
                tbCustomX.Value = Math.Min((int)ovl.custom_overlay.x, tbCustomX.Maximum);
                tbCustomY.Value = Math.Min((int)ovl.custom_overlay.y, tbCustomY.Maximum);
                lblCustomXVal.Text = "x:" + ovl.custom_overlay.x;
                lblCustomYVal.Text = "y:" + ovl.custom_overlay.y;
            }

            var imgp = new NetSdkNative.NetImageParams();
            int retImg = NetSdkNative.net_get_image_params(_netHandle, channelId, ref imgp);
            LogApi("net_get_image_params", retImg, $"channel_id={channelId}");
            if (NetSdkNative.IsOk(retImg))
            {
                nudContrast.Value = imgp.contrast;
                nudBrightness.Value = imgp.brightness;
                cbDdeEnable.Checked = imgp.dde_enable != 0;
                nudDdeValue.Value = imgp.dde_value;
            }

        }

        private void btnDisplaySet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            uint channelId;
            if (!TryResolveDisplayChannelId(out channelId)) return;

            var ovl = NetSdkNative.NetOverlayInfo.Create();
            int retGo = NetSdkNative.net_get_overlay(_netHandle, channelId, ref ovl);
            LogApi("net_get_overlay", retGo, $"channel_id={channelId}");
            ovl.channel_name_overlay.enabled = (byte)(cbChNameEnabled.Checked ? 1 : 0);
            if (cbChNameAlign.SelectedIndex >= 0)
                ovl.channel_name_overlay.align = (NetSdkNative.NetOverlayAlign)cbChNameAlign.SelectedIndex;
            if (cbChNameMode.SelectedIndex >= 0)
                ovl.channel_name_overlay.mode = (NetSdkNative.NetOverlayDisplayMode)cbChNameMode.SelectedIndex;
            ovl.channel_name_overlay.x = (uint)tbChNameX.Value;
            ovl.channel_name_overlay.y = (uint)tbChNameY.Value;
            ovl.time_overlay.enabled = (byte)(cbTimeEnabled.Checked ? 1 : 0);
            if (cbTimeAlign.SelectedIndex >= 0)
                ovl.time_overlay.align = (NetSdkNative.NetOverlayAlign)cbTimeAlign.SelectedIndex;
            if (cbTimeOvlMode.SelectedIndex >= 0)
                ovl.time_overlay.mode = (NetSdkNative.NetOverlayDisplayMode)cbTimeOvlMode.SelectedIndex;
            ovl.time_overlay.x = (uint)tbTimeX.Value;
            ovl.time_overlay.y = (uint)tbTimeY.Value;

            ovl.custom_overlay.enabled = (byte)(cbCustomEnabled.Checked ? 1 : 0);
            if (cbCustomAlign.SelectedIndex >= 0)
                ovl.custom_overlay.align = (NetSdkNative.NetOverlayAlign)cbCustomAlign.SelectedIndex;
            if (cbCustomMode.SelectedIndex >= 0)
                ovl.custom_overlay.mode = (NetSdkNative.NetOverlayDisplayMode)cbCustomMode.SelectedIndex;
            ovl.custom_overlay.x = (uint)tbCustomX.Value;
            ovl.custom_overlay.y = (uint)tbCustomY.Value;
            ovl.custom_overlay.text = NetSdkNative.StringToFixedBytes(tbCustomText.Text, NetSdkNative.NET_LEN_128);

            int retSo = NetSdkNative.net_set_overlay(_netHandle, channelId, ref ovl);
            LogApi("net_set_overlay", retSo, $"channel_id={channelId}");

            var imgp = new NetSdkNative.NetImageParams();
            imgp.contrast = (ushort)nudContrast.Value;
            imgp.brightness = (ushort)nudBrightness.Value;
            imgp.dde_enable = (byte)(cbDdeEnable.Checked ? 1 : 0);
            imgp.dde_value = (ushort)nudDdeValue.Value;
            int riImg = NetSdkNative.net_set_image_params(_netHandle, channelId, ref imgp);
            LogApi("net_set_image_params", riImg, $"channel_id={channelId}");

            MessageBox.Show("Display settings applied", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tbChNameX_Scroll(object sender, EventArgs e) { lblChNameXVal.Text = "x:" + tbChNameX.Value; }
        private void tbChNameY_Scroll(object sender, EventArgs e) { lblChNameYVal.Text = "y:" + tbChNameY.Value; }
        private void tbTimeX_Scroll(object sender, EventArgs e) { lblTimeXVal.Text = "x:" + tbTimeX.Value; }
        private void tbTimeY_Scroll(object sender, EventArgs e) { lblTimeYVal.Text = "y:" + tbTimeY.Value; }
        private void tbCustomX_Scroll(object sender, EventArgs e) { lblCustomXVal.Text = "x:" + tbCustomX.Value; }
        private void tbCustomY_Scroll(object sender, EventArgs e) { lblCustomYVal.Text = "y:" + tbCustomY.Value; }

        private void LoadThermalOsdSettings()
        {
            if (!_netConnected || _acousticActive) return;

            var size = new NetSdkNative.NetSize();
            int retSize = NetSdkNative.net_get_thermal_resolution(_netHandle, ref size);
            LogApi("net_get_thermal_resolution", retSize);
            if (NetSdkNative.IsOk(retSize))
                thermalOsdPanel.SetAxisLimits(size.width, size.height);

            var info = new NetSdkNative.NetThermalOverlayInfo();
            int ret = NetSdkNative.net_get_thermal_overlay(_netHandle, ref info);
            LogApi("net_get_thermal_overlay", ret, ThermalOsdPanel.FormatSummary(info));
            if (NetSdkNative.IsOk(ret))
            {
                thermalOsdPanel.LoadFrom(info);
            }
            else
            {
                MessageBox.Show("Get thermal overlay failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveThermalOsdSettings()
        {
            if (!_netConnected) return;

            var info = thermalOsdPanel.BuildStruct();
            int ret = NetSdkNative.net_set_thermal_overlay(_netHandle, ref info);
            LogApi("net_set_thermal_overlay", ret, ThermalOsdPanel.FormatSummary(info));
            if (NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("Thermal OSD settings applied", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Set thermal overlay failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSetChannelName_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            uint channelId;
            if (!TryResolveDisplayChannelId(out channelId)) return;

            var ch = NetSdkNative.NetChannelInfo.Create();
            ch.id = channelId;
            var nameBytes = System.Text.Encoding.UTF8.GetBytes(tbChannelName.Text);
            Array.Copy(nameBytes, ch.name, Math.Min(nameBytes.Length, NetSdkNative.NET_LEN_64 - 1));
            int ret = NetSdkNative.net_set_channel(_netHandle, channelId, ref ch);
            LogApi("net_set_channel", ret, $"channel_id={channelId}");
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("Channel name updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        #region Search & Alarm

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            int initRet = NetSdkNative.net_sdk_init();
            LogApi("net_sdk_init", initRet);

            var devices = new NetSdkNative.NetDiscoveredDevice[64];
            for (int i = 0; i < devices.Length; i++)
                devices[i] = NetSdkNative.NetDiscoveredDevice.Create();

            ushort outLen;
            int ret = NetSdkNative.net_search_devices(devices, (ushort)devices.Length, out outLen, 3000);
            LogApi("net_search_devices", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("Search failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dlg = new Form())
            {
                dlg.Text = "Discovered Devices";
                dlg.Size = new System.Drawing.Size(900, 350);
                dlg.StartPosition = FormStartPosition.CenterParent;

                var lv = new ListView();
                lv.Dock = DockStyle.Fill;
                lv.View = View.Details;
                lv.FullRowSelect = true;
                lv.GridLines = true;
                lv.Columns.Add("IP", 120);
                lv.Columns.Add("Netmask", 110);
                lv.Columns.Add("Name", 140);
                lv.Columns.Add("Model", 110);
                lv.Columns.Add("SN", 90);
                lv.Columns.Add("FW Name", 90);
                lv.Columns.Add("FW", 90);
                lv.Columns.Add("Wandering", 70);

                for (int i = 0; i < outLen; i++)
                {
                    string ip = System.Text.Encoding.UTF8.GetString(devices[i].ip).TrimEnd('\0');
                    string netmask = System.Text.Encoding.UTF8.GetString(devices[i].netmask).TrimEnd('\0');
                    string name = System.Text.Encoding.UTF8.GetString(devices[i].name).TrimEnd('\0');
                    string model = System.Text.Encoding.UTF8.GetString(devices[i].model).TrimEnd('\0');
                    string sn = System.Text.Encoding.UTF8.GetString(devices[i].serial_no).TrimEnd('\0');
                    string fwName = System.Text.Encoding.UTF8.GetString(devices[i].firmware_name).TrimEnd('\0');
                    string fw = System.Text.Encoding.UTF8.GetString(devices[i].firmware_version).TrimEnd('\0');
                    string wandering = devices[i].wandering != 0 ? "Yes" : "No";
                    var item = new ListViewItem(new[] { ip, netmask, name, model, sn, fwName, fw, wandering });
                    item.Tag = ip;
                    lv.Items.Add(item);
                }

                lv.DoubleClick += (s, ev) =>
                {
                    if (lv.SelectedItems.Count > 0)
                    {
                        tstbIP.Text = lv.SelectedItems[0].Tag as string;
                        dlg.DialogResult = DialogResult.OK;
                        dlg.Close();
                    }
                };

                dlg.Controls.Add(lv);
                dlg.ShowDialog(this);
            }
        }

        private void btnAlarmGet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            LoadMarkers();
            LoadThermalAlarms();
            LoadAlarmTrigger();
            LoadAlarmInterval();
        }

        private void btnAlarmSet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            // Keep selected row edits before pushing the full list.
            ApplySelectedAlarmFromUi(false);

            if (_alarmCount == 0)
            {
                MessageBox.Show("No alarm rules available. Click Get Rules first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int retAlarms = NetSdkNative.net_set_thermal_temp_alarms(_netHandle, _alarms, _alarmCount);
            LogApi("net_set_thermal_temp_alarms", retAlarms);
            if (!NetSdkNative.IsOk(retAlarms))
            {
                MessageBox.Show("Set thermal alarms failed: " + retAlarms, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var interval = new NetSdkNative.NetThermalTempAlarmInterval
            {
                interval = (ushort)nudAlarmInterval.Value
            };
            int retInterval = NetSdkNative.net_set_thermal_temp_alarm_interval(_netHandle, ref interval);
            LogApi("net_set_thermal_temp_alarm_interval", retInterval);
            if (!NetSdkNative.IsOk(retInterval))
            {
                MessageBox.Show("Set thermal alarm interval failed: " + retInterval, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var trigType = NetSdkNative.NetAlarmType.ThermalTemp;
            var trigger = new NetSdkNative.NetAlarmTrigger();
            int retGet = NetSdkNative.net_get_alarm_trigger(_netHandle, ref trigType, ref trigger);
            LogApi("net_get_alarm_trigger", retGet);
            if (!NetSdkNative.IsOk(retGet))
            {
                MessageBox.Show("Get alarm trigger failed before set: " + retGet, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            trigger.trigger_ftp_enabled = (byte)(cbAlarmFtpEnabled.Checked ? 1 : 0);
            trigger.trigger_webhook_enabled = (byte)(cbAlarmWebhookEnabled.Checked ? 1 : 0);
            int retTrig = NetSdkNative.net_set_alarm_trigger(_netHandle, ref trigType, ref trigger);
            LogApi("net_set_alarm_trigger", retTrig);
            if (NetSdkNative.IsOk(retTrig))
                MessageBox.Show("Thermal alarms, interval and trigger updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Set alarm trigger failed: " + retTrig, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnAlarmApply_Click(object sender, EventArgs e)
        {
            ApplySelectedAlarmFromUi(true);
        }

        private void lvAlarms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingAlarmUi) return;
            FillAlarmEditorFromSelection();
        }

        private void LoadThermalAlarms(bool showErrors = true)
        {
            if (_acousticActive)
                return;
            var deviceAlarms = new NetSdkNative.NetThermalTempAlarm[NetSdkNative.NET_MAX_MARKERS];
            for (int i = 0; i < deviceAlarms.Length; i++)
                deviceAlarms[i] = NetSdkNative.NetThermalTempAlarm.Create();

            ushort deviceAlarmCount;
            int ret = NetSdkNative.net_get_thermal_temp_alarms(_netHandle, deviceAlarms, (ushort)deviceAlarms.Length, out deviceAlarmCount);
            LogApi("net_get_thermal_temp_alarms", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                if (showErrors)
                    MessageBox.Show("Get alarms failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MergeAlarmsWithMarkers(deviceAlarms, deviceAlarmCount);
            RefreshAlarmListView();
        }

        private void MergeAlarmsWithMarkers(NetSdkNative.NetThermalTempAlarm[] deviceAlarms, ushort deviceAlarmCount)
        {
            var merged = new NetSdkNative.NetThermalTempAlarm[NetSdkNative.NET_MAX_MARKERS];
            for (int i = 0; i < merged.Length; i++)
                merged[i] = NetSdkNative.NetThermalTempAlarm.Create();

            ushort mergedCount = 0;

            // Always expose a "global" rule first: marker_alarms may include global,
            // and net_thermal_temp_alarm.marker_name can be "global".
            {
                int globalAlarmIndex = FindAlarmIndexByMarkerName(deviceAlarms, deviceAlarmCount, GlobalMarkerListName);
                merged[mergedCount] = globalAlarmIndex >= 0
                    ? deviceAlarms[globalAlarmIndex]
                    : CreateDefaultAlarmForMarker(GlobalMarkerListName);
                mergedCount++;
            }

            lock (_markerLock)
            {
                for (int i = 0; i < _markerCount && mergedCount < NetSdkNative.NET_MAX_MARKERS; i++)
                {
                    string markerName = GetMarkerNameString(_markers[i]);
                    if (string.IsNullOrEmpty(markerName))
                        continue;
                    if (string.Equals(markerName, GlobalMarkerListName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    int alarmIndex = FindAlarmIndexByMarkerName(deviceAlarms, deviceAlarmCount, markerName);
                    merged[mergedCount] = alarmIndex >= 0
                        ? deviceAlarms[alarmIndex]
                        : CreateDefaultAlarmForMarker(markerName);
                    mergedCount++;
                }
            }

            _alarms = merged;
            _alarmCount = mergedCount;
        }

        private static int FindAlarmIndexByMarkerName(NetSdkNative.NetThermalTempAlarm[] alarms, ushort count, string markerName)
        {
            for (int i = 0; i < count; i++)
            {
                if (string.Equals(NetSdkNative.BytesToString(alarms[i].marker_name), markerName, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        private static NetSdkNative.NetThermalTempAlarm CreateDefaultAlarmForMarker(string markerName)
        {
            var alarm = NetSdkNative.NetThermalTempAlarm.Create();
            alarm.marker_name = NetSdkNative.StringToFixedBytes(markerName, NetSdkNative.NET_LEN_64);
            alarm.enabled = 0;
            alarm.alarm_type = NetSdkNative.NetThermalTempAlarmType.HighestGreater;
            alarm.alarm_temp = 50f;
            alarm.alarm_delay_time = 1;
            alarm.warning_temp = 30f;
            alarm.warning_delay_time = 1;
            alarm.alert_temp = -50f;
            alarm.alert_delay_time = 1;
            return alarm;
        }

        private void LoadAlarmTrigger()
        {
            var trigger = new NetSdkNative.NetAlarmTrigger();
            var trigType = NetSdkNative.NetAlarmType.ThermalTemp;
            int retTrig = NetSdkNative.net_get_alarm_trigger(_netHandle, ref trigType, ref trigger);
            LogApi("net_get_alarm_trigger", retTrig);
            if (NetSdkNative.IsOk(retTrig))
            {
                cbAlarmFtpEnabled.Checked = trigger.trigger_ftp_enabled != 0;
                cbAlarmWebhookEnabled.Checked = trigger.trigger_webhook_enabled != 0;
            }
            else
                MessageBox.Show("Get alarm trigger failed: " + retTrig, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void LoadAlarmInterval(bool showErrors = true)
        {
            var interval = new NetSdkNative.NetThermalTempAlarmInterval();
            int ret = NetSdkNative.net_get_thermal_temp_alarm_interval(_netHandle, ref interval);
            LogApi("net_get_thermal_temp_alarm_interval", ret);
            if (NetSdkNative.IsOk(ret))
            {
                decimal value = interval.interval;
                if (value < nudAlarmInterval.Minimum) value = nudAlarmInterval.Minimum;
                if (value > nudAlarmInterval.Maximum) value = nudAlarmInterval.Maximum;
                nudAlarmInterval.Value = value;
            }
            else if (showErrors)
            {
                MessageBox.Show("Get alarm interval failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshAlarmListView(int selectIndex = 0)
        {
            _updatingAlarmUi = true;
            try
            {
                lvAlarms.BeginUpdate();
                lvAlarms.Items.Clear();
                for (int i = 0; i < _alarmCount; i++)
                {
                    var item = CreateAlarmListItem(i, _alarms[i]);
                    lvAlarms.Items.Add(item);
                }
                lvAlarms.EndUpdate();
            }
            finally
            {
                _updatingAlarmUi = false;
            }

            if (lvAlarms.Items.Count > 0)
            {
                if (selectIndex < 0 || selectIndex >= lvAlarms.Items.Count)
                    selectIndex = 0;
                lvAlarms.Items[selectIndex].Selected = true;
                lvAlarms.Items[selectIndex].Focused = true;
                lvAlarms.Items[selectIndex].EnsureVisible();
            }
            else
            {
                ClearAlarmEditor();
            }
        }

        private ListViewItem CreateAlarmListItem(int index, NetSdkNative.NetThermalTempAlarm alarm)
        {
            string markerName = NetSdkNative.BytesToString(alarm.marker_name);
            var item = new ListViewItem(new[]
            {
                markerName,
                alarm.enabled != 0 ? "Y" : "N",
                alarm.alarm_type.ToString(),
                alarm.alarm_temp.ToString("F1"),
                alarm.alarm_delay_time.ToString(),
                alarm.warning_temp.ToString("F1"),
                alarm.warning_delay_time.ToString(),
                alarm.alert_temp.ToString("F1"),
                alarm.alert_delay_time.ToString()
            });
            item.Tag = index;
            return item;
        }

        private void UpdateAlarmListItem(int index)
        {
            if (index < 0 || index >= lvAlarms.Items.Count)
                return;

            var alarm = _alarms[index];
            var item = lvAlarms.Items[index];
            item.SubItems[0].Text = NetSdkNative.BytesToString(alarm.marker_name);
            item.SubItems[1].Text = alarm.enabled != 0 ? "Y" : "N";
            item.SubItems[2].Text = alarm.alarm_type.ToString();
            item.SubItems[3].Text = alarm.alarm_temp.ToString("F1");
            item.SubItems[4].Text = alarm.alarm_delay_time.ToString();
            item.SubItems[5].Text = alarm.warning_temp.ToString("F1");
            item.SubItems[6].Text = alarm.warning_delay_time.ToString();
            item.SubItems[7].Text = alarm.alert_temp.ToString("F1");
            item.SubItems[8].Text = alarm.alert_delay_time.ToString();
            item.Tag = index;
        }

        private void FillAlarmEditorFromSelection()
        {
            if (lvAlarms.SelectedItems.Count == 0)
            {
                ClearAlarmEditor();
                return;
            }

            int index = (int)lvAlarms.SelectedItems[0].Tag;
            if (index < 0 || index >= _alarmCount)
            {
                ClearAlarmEditor();
                return;
            }

            var alarm = _alarms[index];
            _updatingAlarmUi = true;
            try
            {
                tbAlarmMarker.Text = NetSdkNative.BytesToString(alarm.marker_name);
                cbAlarmEnabled.Checked = alarm.enabled != 0;
                SelectAlarmType(alarm.alarm_type);
                SetNumericValue(nudAlarmTemp, (decimal)alarm.alarm_temp);
                SetNumericValue(nudAlarmDelay, alarm.alarm_delay_time);
                SetNumericValue(nudAlarmThreshTemp, (decimal)alarm.warning_temp);
                SetNumericValue(nudAlarmThreshDelay, alarm.warning_delay_time);
                SetNumericValue(nudAlarmTrigTemp, (decimal)alarm.alert_temp);
                SetNumericValue(nudAlarmTrigDelay, alarm.alert_delay_time);
            }
            finally
            {
                _updatingAlarmUi = false;
            }
        }

        private void ClearAlarmEditor()
        {
            _updatingAlarmUi = true;
            try
            {
                tbAlarmMarker.Text = string.Empty;
                cbAlarmEnabled.Checked = false;
                if (cbAlarmType.Items.Count > 0)
                    cbAlarmType.SelectedIndex = 0;
                SetNumericValue(nudAlarmTemp, 0);
                SetNumericValue(nudAlarmDelay, 0);
                SetNumericValue(nudAlarmThreshTemp, 0);
                SetNumericValue(nudAlarmThreshDelay, 0);
                SetNumericValue(nudAlarmTrigTemp, 0);
                SetNumericValue(nudAlarmTrigDelay, 0);
            }
            finally
            {
                _updatingAlarmUi = false;
            }
        }

        private void ApplySelectedAlarmFromUi(bool showMessage)
        {
            if (lvAlarms.SelectedItems.Count == 0)
            {
                if (showMessage)
                    MessageBox.Show("Please select an alarm rule first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = (int)lvAlarms.SelectedItems[0].Tag;
            if (index < 0 || index >= _alarmCount)
                return;

            var alarm = _alarms[index];
            alarm.enabled = (byte)(cbAlarmEnabled.Checked ? 1 : 0);
            if (cbAlarmType.SelectedItem is NetSdkNative.NetThermalTempAlarmType)
                alarm.alarm_type = (NetSdkNative.NetThermalTempAlarmType)cbAlarmType.SelectedItem;
            alarm.alarm_temp = (float)nudAlarmTemp.Value;
            alarm.alarm_delay_time = (uint)nudAlarmDelay.Value;
            alarm.warning_temp = (float)nudAlarmThreshTemp.Value;
            alarm.warning_delay_time = (uint)nudAlarmThreshDelay.Value;
            alarm.alert_temp = (float)nudAlarmTrigTemp.Value;
            alarm.alert_delay_time = (uint)nudAlarmTrigDelay.Value;
            _alarms[index] = alarm;
            UpdateAlarmListItem(index);

            if (showMessage)
                MessageBox.Show("Selected alarm updated locally. Click Set to write to device.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SelectAlarmType(NetSdkNative.NetThermalTempAlarmType type)
        {
            for (int i = 0; i < cbAlarmType.Items.Count; i++)
            {
                if ((NetSdkNative.NetThermalTempAlarmType)cbAlarmType.Items[i] == type)
                {
                    cbAlarmType.SelectedIndex = i;
                    return;
                }
            }

            if (cbAlarmType.Items.Count > 0)
                cbAlarmType.SelectedIndex = 0;
        }

        private static void SetNumericValue(NumericUpDown control, decimal value)
        {
            if (value < control.Minimum)
                value = control.Minimum;
            if (value > control.Maximum)
                value = control.Maximum;
            control.Value = value;
        }

        #endregion

        #region DPC

        private void RefreshDpcUi()
        {
            lblDpcResolution.Text = "Thermal Resolution: " + _markerResW + "x" + _markerResH;
            int maxX = Math.Max(0, _markerResW - 1);
            int maxY = Math.Max(0, _markerResH - 1);
            nudDpcX.Maximum = maxX;
            nudDpcY.Maximum = maxY;
            if (nudDpcX.Value > maxX) nudDpcX.Value = maxX;
            if (nudDpcY.Value > maxY) nudDpcY.Value = maxY;
            btnDpcSet.Enabled = _netConnected;
            RefreshDpcListView();
        }

        private void RefreshDpcListView()
        {
            lvDpcPoints.BeginUpdate();
            lvDpcPoints.Items.Clear();
            for (int i = 0; i < _dpcPoints.Count; i++)
            {
                var point = _dpcPoints[i];
                var item = new ListViewItem(new[]
                {
                    (i + 1).ToString(),
                    point.x.ToString(),
                    point.y.ToString()
                });
                item.Tag = i;
                if (i < _dpcSubmittedCount)
                    item.ForeColor = SystemColors.GrayText;
                lvDpcPoints.Items.Add(item);
            }
            lvDpcPoints.EndUpdate();
        }

        private void btnDpcAdd_Click(object sender, EventArgs e)
        {
            ushort x = (ushort)nudDpcX.Value;
            ushort y = (ushort)nudDpcY.Value;

            if (x >= _markerResW || y >= _markerResH)
            {
                MessageBox.Show("Coordinate is out of thermal image bounds.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var point in _dpcPoints)
            {
                if (point.x == x && point.y == y)
                {
                    MessageBox.Show("Duplicate coordinate (" + x + ", " + y + ").", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            _dpcPoints.Add(new NetSdkNative.NetPoint { x = x, y = y });
            RefreshDpcListView();
        }

        private void btnDpcRemove_Click(object sender, EventArgs e)
        {
            if (lvDpcPoints.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a point to remove.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int index = (int)lvDpcPoints.SelectedItems[0].Tag;
            if (index < 0 || index >= _dpcPoints.Count)
                return;

            if (index < _dpcSubmittedCount)
            {
                MessageBox.Show("Submitted points cannot be removed from the local list. They remain on the device.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _dpcPoints.RemoveAt(index);
            RefreshDpcListView();
        }

        private void btnDpcClear_Click(object sender, EventArgs e)
        {
            if (_dpcPoints.Count == 0)
                return;

            if (_dpcSubmittedCount > 0)
            {
                var result = MessageBox.Show(
                    "Clear pending points only? Submitted points already on the device cannot be removed via this API.",
                    Text,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                {
                    _dpcPoints.RemoveRange(_dpcSubmittedCount, _dpcPoints.Count - _dpcSubmittedCount);
                    RefreshDpcListView();
                    return;
                }
            }

            _dpcPoints.Clear();
            _dpcSubmittedCount = 0;
            RefreshDpcListView();
        }

        private void btnDpcSet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            int pendingCount = _dpcPoints.Count - _dpcSubmittedCount;
            if (pendingCount <= 0)
            {
                MessageBox.Show("No new points to submit.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (pendingCount > NetSdkNative.NET_LEN_64)
            {
                MessageBox.Show("At most " + NetSdkNative.NET_LEN_64 + " points can be submitted at once.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var param = NetSdkNative.NetDpcParams.Create();
            param.count = (ushort)pendingCount;
            for (int i = 0; i < pendingCount; i++)
                param.points[i] = _dpcPoints[_dpcSubmittedCount + i];

            int ret = NetSdkNative.net_set_thermal_dpc(_netHandle, ref param);
            LogApi("net_set_thermal_dpc", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("Set thermal DPC failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _dpcSubmittedCount = _dpcPoints.Count;
            RefreshDpcListView();
            MessageBox.Show("Submitted " + pendingCount + " dead pixel point(s).", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void tsbLogging_CheckedChanged(object sender, EventArgs e)
        {
            if (tsbLogging.Checked)
                EnableSdkLogging();
            else
                DisableSdkLogging();

            tsbLogging.Text = tsbLogging.Checked ? "\u2611 Logging" : "\u2610 Logging";
        }

        private void EnableSdkLogging()
        {
            AttachSdkLogConsole();
            StartOutputDebugStringListener();

            int retEn = NetSdkNative.net_enable_log(1);
            LogApi("net_enable_log(1)", retEn);

            try
            {
                Console.WriteLine("[IRtekNetSDKDemo] Logging enabled via net_enable_log(1).");
                Console.WriteLine("[IRtekNetSDKDemo] Capturing OutputDebugString (close DebugView if no logs appear).");
            }
            catch
            {
            }
        }

        private void DisableSdkLogging()
        {
            int ret = NetSdkNative.net_enable_log(0);
            LogApi("net_enable_log(0)", ret);
            StopOutputDebugStringListener();
            DetachSdkLogConsole();
        }

        private void StartOutputDebugStringListener()
        {
            StopOutputDebugStringListener();

            _dbWinBufferMapping = CreateFileMapping(
                InvalidHandleValue, IntPtr.Zero, PAGE_READWRITE, 0, 4096, "DBWIN_BUFFER");
            if (_dbWinBufferMapping == IntPtr.Zero)
            {
                try { Console.WriteLine("[IRtekNetSDKDemo] Failed to create DBWIN_BUFFER (Error " + Marshal.GetLastWin32Error() + ")."); }
                catch { }
                return;
            }

            _dbWinBufferView = MapViewOfFile(_dbWinBufferMapping, FILE_MAP_READ, 0, 0, UIntPtr.Zero);
            if (_dbWinBufferView == IntPtr.Zero)
            {
                try { Console.WriteLine("[IRtekNetSDKDemo] Failed to map DBWIN_BUFFER."); }
                catch { }
                CloseHandle(_dbWinBufferMapping);
                _dbWinBufferMapping = IntPtr.Zero;
                return;
            }

            _dbWinBufferReady = CreateEvent(IntPtr.Zero, false, false, "DBWIN_BUFFER_READY");
            _dbWinDataReady = CreateEvent(IntPtr.Zero, false, false, "DBWIN_DATA_READY");
            if (_dbWinBufferReady == IntPtr.Zero || _dbWinDataReady == IntPtr.Zero)
            {
                try { Console.WriteLine("[IRtekNetSDKDemo] Failed to create DBWIN events."); }
                catch { }
                StopOutputDebugStringListener();
                return;
            }

            int currentPid = System.Diagnostics.Process.GetCurrentProcess().Id;
            _sdkLogOdsRunning = true;
            _sdkLogOdsThread = new Thread(() => ListenOutputDebugString(currentPid))
            {
                IsBackground = true,
                Name = "IRtekNetSDK-ODS"
            };
            _sdkLogOdsThread.Start();
        }

        private void StopOutputDebugStringListener()
        {
            _sdkLogOdsRunning = false;
            Thread thread = _sdkLogOdsThread;
            _sdkLogOdsThread = null;
            if (thread != null && thread.IsAlive)
            {
                try { thread.Join(800); }
                catch { }
            }

            if (_dbWinBufferView != IntPtr.Zero)
            {
                UnmapViewOfFile(_dbWinBufferView);
                _dbWinBufferView = IntPtr.Zero;
            }
            if (_dbWinBufferMapping != IntPtr.Zero)
            {
                CloseHandle(_dbWinBufferMapping);
                _dbWinBufferMapping = IntPtr.Zero;
            }
            if (_dbWinBufferReady != IntPtr.Zero)
            {
                CloseHandle(_dbWinBufferReady);
                _dbWinBufferReady = IntPtr.Zero;
            }
            if (_dbWinDataReady != IntPtr.Zero)
            {
                CloseHandle(_dbWinDataReady);
                _dbWinDataReady = IntPtr.Zero;
            }
        }

        private void ListenOutputDebugString(int currentPid)
        {
            while (_sdkLogOdsRunning)
            {
                try
                {
                    SetEvent(_dbWinBufferReady);
                    uint wait = WaitForSingleObject(_dbWinDataReady, 200);
                    if (wait != WAIT_OBJECT_0 || !_sdkLogOdsRunning)
                        continue;

                    int pid = Marshal.ReadInt32(_dbWinBufferView);
                    if (pid != currentPid)
                        continue;

                    string message = Marshal.PtrToStringAnsi(IntPtr.Add(_dbWinBufferView, 4));
                    if (!string.IsNullOrEmpty(message))
                        Console.Write(message.EndsWith("\n") ? message : message + Environment.NewLine);
                }
                catch
                {
                }
            }
        }

        private void AttachSdkLogConsole()
        {
            if (!_sdkLogConsoleAttached)
            {
                AllocConsole();
                _sdkLogConsoleAttached = true;
            }

            SetConsoleTitle("IRtek NET SDK Log");

            try
            {
                var utf8 = new UTF8Encoding(false);
                Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), utf8) { AutoFlush = true });
                Console.SetError(new StreamWriter(Console.OpenStandardError(), utf8) { AutoFlush = true });
            }
            catch
            {
            }
        }

        private void DetachSdkLogConsole()
        {
            if (!_sdkLogConsoleAttached)
                return;

            FreeConsole();
            _sdkLogConsoleAttached = false;
        }

        private void btnPropertiesGet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadProperties();
        }

        private void tcThermalFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcThermalFile.SelectedTab == tpDpc)
            {
                RefreshDpcUi();
                return;
            }

            if (!_netConnected) return;
            RefreshSelectedConfigTab();
        }

        private void RefreshSelectedConfigTab()
        {
            if (tcThermalFile.SelectedTab == tpProperties)
                LoadProperties();
            else if (tcThermalFile.SelectedTab == tpThermalParams)
                LoadThermalParams();
            else if (tcThermalFile.SelectedTab == tpTime)
                LoadDeviceTime();
            else if (tcThermalFile.SelectedTab == tpNetwork)
                LoadNetworkInfo();
            else if (tcThermalFile.SelectedTab == tpUser)
                LoadUsers();
            else if (tcThermalFile.SelectedTab == tpDisplay)
                LoadDisplaySettings();
            else if (tcThermalFile.SelectedTab == tpThermalOsd)
            {
                if (_acousticActive)
                    LoadAcousticOverlay(false);
                else
                    LoadThermalOsdSettings();
            }
            else if (tcThermalFile.SelectedTab == tpAlarm)
            {
                if (_acousticActive)
                    LoadAcousticAlarms(false);
                else
                {
                    LoadMarkers();
                    LoadThermalAlarms();
                    LoadAlarmTrigger();
                    LoadAlarmInterval();
                }
            }
        }

        private void btnThermalParamsGet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;
            LoadThermalParams();
        }

        private void btnThermalParamsSet_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            var p = new NetSdkNative.NetThermalParams
            {
                emissivity = (float)nudEmissivity.Value,
                refl_temp = (float)nudReflTemp.Value,
                distance = (float)nudDistance.Value,
                atm_temp = (float)nudAtmTemp.Value,
                rel_humidity = (float)nudRelHumidity.Value / 100f
            };
            int retParams = NetSdkNative.net_set_thermal_params(_netHandle, ref p);
            LogApi("net_set_thermal_params", retParams);
            if (!NetSdkNative.IsOk(retParams))
            {
                MessageBox.Show("Set thermal params failed: " + retParams, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbShutterMode.SelectedIndex >= 0)
            {
                var mode = (NetSdkNative.NetShutterMode)cbShutterMode.SelectedIndex;
                int retMode = NetSdkNative.net_set_thermal_shutter_mode(_netHandle, ref mode);
                LogApi("net_set_thermal_shutter_mode", retMode);
                if (!NetSdkNative.IsOk(retMode))
                {
                    MessageBox.Show("Set shutter mode failed: " + retMode, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            ushort minutes = (ushort)nudShutterTime.Value;
            int retTime = NetSdkNative.net_set_thermal_shutter_time(_netHandle, ref minutes);
            LogApi("net_set_thermal_shutter_time", retTime);
            if (!NetSdkNative.IsOk(retTime))
            {
                MessageBox.Show("Set shutter time failed: " + retTime, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbTempUnit.SelectedIndex < 0 || cbDistUnit.SelectedIndex < 0)
            {
                MessageBox.Show("Thermal params updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            NetSdkNative.NetTempUnit oldTempUnit = _activeTempUnit;
            NetSdkNative.NetDistanceUnit oldDistUnit = _activeDistUnit;
            NetSdkNative.NetTempUnit newTempUnit = GetSelectedTempUnit();
            NetSdkNative.NetDistanceUnit newDistUnit = GetSelectedDistanceUnit();
            var localParamsSnapshot = SnapshotEnabledLocalParams();

            var info = new NetSdkNative.NetUnitInfo();
            info.temp_unit = newTempUnit;
            info.distance_unit = newDistUnit;
            int retUnit = NetSdkNative.net_set_thermal_unit(_netHandle, ref info);
            LogApi("net_set_thermal_unit", retUnit);
            if (!NetSdkNative.IsOk(retUnit))
            {
                MessageBox.Show("Set thermal unit failed: " + retUnit, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IsGUIRefreshing = true;
            ReloadThermalParamValues();
            UpdateThermalParamUnitLabels();
            IsGUIRefreshing = false;
            LoadRanges();
            LoadGain();
            LoadProperties();
            LoadMarkers();
            RestoreLocalParamsAfterUnitChange(localParamsSnapshot, oldTempUnit, oldDistUnit, newTempUnit, newDistUnit);
            _activeTempUnit = newTempUnit;
            _activeDistUnit = newDistUnit;
            if (_alarmCount > 0 || lvAlarms.Items.Count > 0)
                LoadThermalAlarms(false);

            MessageBox.Show("Thermal params updated", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tscbStreamId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingStreamIdCombo)
                return;

            StreamIdItem item = GetSelectedStreamIdItem();
            if (item == null || item.Id == _currentStreamId)
                return;

            uint previousId = _currentStreamId;
            uint selectedId = item.Id;

            if (!_netConnected)
            {
                _currentStreamId = selectedId;
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                StopVideoStream(_currentStreamType);
                if (_currentStreamType == NetSdkNative.NetStreamType.H264)
                    DisposeH264Decoder();

                if (!TryLoadCurrentStream(selectedId))
                {
                    MessageBox.Show("Get stream failed: " + selectedId, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStreamIdCombo(previousId);
                    StartVideoStreamCallback(_currentStreamType, true);
                    return;
                }

                _currentStreamId = selectedId;
                RequestStreamDataLog();
                if (!StartVideoStreamCallback(_currentStreamType, true))
                {
                    _currentStreamId = previousId;
                    TryLoadCurrentStream(previousId);
                    SetStreamIdCombo(previousId);
                    StartVideoStreamCallback(_currentStreamType, true);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void tscbStreamType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingStreamTypeCombo)
                return;

            NetSdkNative.NetStreamType selectedType = GetSelectedStreamType();
            if (selectedType == _currentStreamType)
                return;

            NetSdkNative.NetStreamType previousType = _currentStreamType;
            if (!_netConnected)
            {
                _currentStreamType = selectedType;
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                StopVideoStream(previousType);
                if (previousType == NetSdkNative.NetStreamType.H264 && selectedType != NetSdkNative.NetStreamType.H264)
                    DisposeH264Decoder();

                _currentStreamType = selectedType;
                RequestStreamDataLog();
                if (StartVideoStreamCallback(selectedType, true))
                {
                    SetStreamTypeCombo(selectedType);
                    return;
                }

                _currentStreamType = previousType;
                SetStreamTypeCombo(previousType);
                StartVideoStreamCallback(previousType, true);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnFtpTest_Click(object sender, EventArgs e)
        {
            if (!_netConnected) return;

            var ftp = BuildFtpInfoFromUi();
            int ret = NetSdkNative.net_test_ftp(_netHandle, ref ftp);
            LogApi("net_test_ftp", ret);
            if (NetSdkNative.IsOk(ret))
                MessageBox.Show("FTP test passed", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"FTP test failed: {ret} ({(NetSdkNative.NetErrorCode)ret})", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region Interface Notification

        private void cbInterfaceNotify_CheckedChanged(object sender, EventArgs e)
        {
            rtbInterfaceLog.Visible = cbInterfaceNotify.Checked;
            if (cbInterfaceNotify.Checked)
                AppendApiLog("Interface notification enabled");
        }

        private void AppendApiLog(string text)
        {
            if (rtbInterfaceLog == null || rtbInterfaceLog.IsDisposed) return;

            if (rtbInterfaceLog.InvokeRequired)
            {
                rtbInterfaceLog.BeginInvoke(new Action<string>(AppendApiLog), text);
                return;
            }

            string ts = DateTime.Now.ToString("HH:mm:ss.fff");
            rtbInterfaceLog.AppendText("[" + ts + "] " + text + "\r\n");
            rtbInterfaceLog.ScrollToCaret();
        }

        private void LogApi(string apiName, int ret)
        {
            if (cbInterfaceNotify == null || !cbInterfaceNotify.Checked) return;
            AppendApiLog(apiName + " → " + ret + " (" + NetSdkNative.FormatReturnStatus(ret) + ")");
        }

        private void LogApi(string apiName, int ret, string extra)
        {
            if (cbInterfaceNotify == null || !cbInterfaceNotify.Checked) return;
            AppendApiLog(apiName + " → " + ret + " (" + NetSdkNative.FormatReturnStatus(ret) + ") " + extra);
        }

        private void LogApiCall(string message)
        {
            if (cbInterfaceNotify == null || !cbInterfaceNotify.Checked) return;
            AppendApiLog(message);
        }

        #endregion
    }
}
