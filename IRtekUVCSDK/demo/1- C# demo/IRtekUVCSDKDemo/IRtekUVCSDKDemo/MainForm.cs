using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IRtekUVCSDK;

namespace IRtekUVCSDKDemo
{
    public partial class MainForm : Form
    {
        private const int SettingsPanelWidth = 448;
        private const int SettingsLabelWidth = 128;
        private const int SettingsEditorWidth = 112;
        private const int SettingsUnitWidth = 36;
        private const int DisplayWidth = 320;
        private const int DisplayHeight = 240;
        private const ushort HollyVid = 0x0603;
        private const ushort HollyPid = 0x8612;
        private const ushort NemoVid = 0x03FD;
        private const ushort NemoPid = 0x0100;

        private enum DeviceMode
        {
            Unknown,
            Thermal,
            Acoustic
        }

        private IntPtr _uvcHandle = IntPtr.Zero;
        private bool _connected;
        private bool _streaming;
        private ushort _connectedVid;
        private ushort _connectedPid;
        private byte[] _connectedPath;
        private bool _deviceLinkDown;
        private bool _resumeStreamAfterReconnect;
        private UvcSdkNative.OnUvcFoundCallback _hotplugCb;
        private DeviceMode _deviceMode;
        private bool _isGuiRefreshing;
        private volatile bool _isDisposing;
        private readonly ManualResetEvent _loginLoadDone = new ManualResetEvent(true);
        private int _loginGeneration;
        private bool _loginUiLocked;
        private LoginUiFilter _loginUiFilter;
        private bool _settingsSectionsReady;
        private bool _settingsSectionsThermal;
        private bool _settingsSectionsAcoustic;
        private UvcSdkNative.OnFrameReceivedCallback _frameCb;
        private UvcSdkNative.UvcFrameInfo _currentFrame;
        private System.Collections.Generic.List<FrameItem> _frameList = new System.Collections.Generic.List<FrameItem>();
        private ThermalImageRenderer _renderer;
        private readonly object _renderLock = new object();

        private byte[] _acousticPixels;
        private byte[] _thermalPixels;
        private readonly object _acousticUiLock = new object();
        private Bitmap _acousticPending;
        private bool _acousticUiQueued;
        private int _acousticVisualW;
        private int _acousticVisualH;
        private volatile int _acousticRawW;
        private volatile int _acousticRawH;
        private readonly object _spectrumLock = new object();
        private int _spectrumFrame;
        private float[] _spectrumColumns;
        private float _spectrumPeak;
        private float _adoptedSpectrumMax;
        private float _pendingSpectrumMax;
        private DateTime _spectrumPendingSinceUtc;
        private SpectrumSnapshot _acousticSpectrumPending;
        private float _acousticPendingDbMin = float.NaN;
        private float _acousticPendingDbMax = float.NaN;
        private readonly AcousticSourcePoint[] _acousticPendingExtremes =
            new AcousticSourcePoint[UvcSdkNative.UVC_ACOUSTIC_EXTREME_POINT_COUNT];
        private int _acousticPendingExtremeCount;
        private readonly AcousticSourcePoint[] _acousticExtremes =
            new AcousticSourcePoint[UvcSdkNative.UVC_ACOUSTIC_EXTREME_POINT_COUNT];
        private int _acousticExtremeCount;
        private readonly Bitmap _acousticPointMarker;
        private float _acousticFreqMinHz = 5000f;
        private float _acousticFreqMaxHz = 100000f;
        private bool _acousticFreqReady;
        private const int SpectrumPanelWidth = 491;
        private const int SpectrumViewWidth = 680;
        private const int SpectrumViewHeight = 88;
        private const int SpectrumSidePadding = 36;
        private const int SpectrumBottomMargin = 8;
        private const int SpectrumColumnsPerKhz = 5;
        private const float SpectrumMinKhz = 2f;
        private const int AcousticDbOffset = 7680;
        private const float AcousticPointMarkerSize = 40f;
        private const float AcousticSpectrumMinHz = 2000f;
        private const float AcousticSpectrumMaxHz = 100000f;
        private const float AcousticSpectrumDeltaHz = 100f;
        private static readonly Color SpectrumBackColor = Color.FromArgb(18, 24, 38);
        private int _acousticFpsFrames;
        private int _acousticFpsWindowStart;
        private double _acousticFps;
        private int _acousticSizePosted;

        private byte[] _lastRawFrame;
        private int _lastRawW, _lastRawH;
        private byte[] _lastAcousticFrame;
        private int _lastAcousticFrameW;
        private int _lastAcousticFrameH;
        private ushort[] _lastLutAd;
        private float[] _lastLutTemp;
        private byte[] _thermalGrid;
        private int _thermalGridW;
        private int _thermalGridH;
        private ThermalAdLut _cursorLut;
        private ushort[] _cursorLutAd;
        private float[] _cursorLutTemp;
        private Point _cursorClient;
        private bool _cursorOverVideo;
        private string _cursorTempText;
        private readonly Font _cursorTempFont;
        private readonly object _frameLock = new object();

        private byte _paletteRefNo;
        private byte _paletteInverted;
        private byte _paletteAlpha = 60;
        private readonly ToolStripLabel _tslBlend;
        private readonly ToolStripControlHost _tsBlend;
        private readonly NumericUpDown _nudBlend;
        private readonly AcousticSettingsPanel _acousticSettings;
        private readonly PictureBox _pbAcousticSpectrum;
        private SpectrumSnapshot _lastSpectrumView;
        private readonly GroupBox _grpAcoustic;
        private readonly ThermalImagingPanel _thermalImaging;
        private readonly ColorScaleBar _colorScale;
        private readonly object _colorScaleLock = new object();
        private const float ColorScaleCelsiusOffset = 273.15f;
        private volatile bool _colorScaleFollowFrame;
        private bool _colorScaleFixedReady;
        private float _colorScaleFixedMinC = 20f;
        private float _colorScaleFixedMaxC = 40f;
        private float _colorScalePendingMinC = float.NaN;
        private float _colorScalePendingMaxC = float.NaN;
        private float _colorScaleLastMinC = float.NaN;
        private float _colorScaleLastMaxC = float.NaN;
        private bool _colorScaleUpdateQueued;
        private ThermalAdLut _colorScaleLut;
        private ushort[] _colorScaleLutAd;
        private float[] _colorScaleLutTemp;
        private readonly GroupBox _grpImaging;
        private readonly TabControl _tcSettings;
        private readonly Dictionary<Control, Control> _sectionButtonHosts = new Dictionary<Control, Control>();
        private Control _thermalParamButtons;
        private readonly Rectangle _thermalPreviewBounds;
        private readonly StatusStrip _ssMain;
        private readonly ToolStripStatusLabel _tsslConnection;
        private readonly ToolStripStatusLabel _tsslMode;
        private readonly ToolStripStatusLabel _tsslStats;
        private readonly System.Windows.Forms.Timer _statsTimer;
        private int _lastStatsFrames;
        private volatile int _previewFrameW;
        private volatile int _previewFrameH;
        public enum MarkerType { None, Spot, Rectangle, Line }

        private enum TrackerHitType
        {
            hitNothing = -1,
            hitTopLeft = 0,
            hitTopRight = 1,
            hitBottomRight = 2,
            hitBottomLeft = 3,
            hitMiddle = 8,
            hitStart = 10,
            hitEnd = 11,
            hitEdge = 9
        }

        private class MarkerTracker
        {
            public bool IsTracking { get; set; }
            public string Name { get; set; }
            public MarkerType MarkerType { get; set; }
            public TrackerHitType HitType { get; set; }
            public Point PrevPos { get; set; }

            public MarkerTracker() { Reset(); }

            public void Reset()
            {
                IsTracking = false;
                Name = string.Empty;
                MarkerType = MarkerType.None;
                HitType = TrackerHitType.hitNothing;
                PrevPos = Point.Empty;
            }
        }

        private bool Adding { get; set; }
        private bool Deleting { get; set; }
        private MarkerType CurrentMarkerType { get; set; }

        private const int MaxMarkers = 16;

        private struct LocalMarker
        {
            public MarkerType Type;
            public string Name;
            public Point[] Points; // Spot:1pt, Rect:2pts(topLeft,bottomRight), Line:2pts
        }

        private readonly LocalMarker[] _markers = new LocalMarker[MaxMarkers];
        private int _markerCount;
        private readonly object _markerLock = new object();
        private int _spotCounter;
        private int _areaCounter;
        private int _lineCounter;
        private int _frameCounter;
        private readonly MarkerTracker _tracker = new MarkerTracker();

        private struct DeviceItem
        {
            public UvcSdkNative.UvcDevice Device;
            public string Label;
            public override string ToString() { return Label; }
        }

        private struct RangeItem
        {
            public ushort Id;
            public string Label;
            public override string ToString() { return Label; }
        }

        private struct FrameItem
        {
            public int Index;
            public UvcSdkNative.UvcFrameInfo Frame;
            public string Label;
            public override string ToString() { return Label; }
        }

        private struct AcousticSourcePoint
        {
            public int X;
            public int Y;
            public float Value;
        }

        public struct PresetPaletteItem
        {
            public byte RefNo { get; set; }
            public string Name { get; set; }
            public override string ToString() { return Name ?? string.Empty; }
        }

        public MainForm()
        {
            InitializeComponent();
            _cursorTempFont = new Font(Font.FontFamily, Font.SizeInPoints + 2f, Font.Style, GraphicsUnit.Point);
            _acousticPointMarker = LoadAcousticPointMarker();
            pbThermalImage.Paint += pbThermalImage_Paint;
            PropertyInfo doubleBuffered = typeof(Control).GetProperty(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (doubleBuffered != null)
                doubleBuffered.SetValue(pbThermalImage, true, null);
            FixSettingsPanelWidth();

            _thermalPreviewBounds = pbThermalImage.Bounds;
            _acousticSettings = new AcousticSettingsPanel();
            _acousticSettings.FrequencyRangeChanged += AcousticFrequencyRangeChanged;
            _pbAcousticSpectrum = new SpectrumPictureBox
            {
                SizeMode = PictureBoxSizeMode.Normal,
                TabStop = false,
                Visible = false,
                Size = new Size(SpectrumViewWidth + SpectrumSidePadding * 2, SpectrumViewHeight)
            };
            scMain.Panel1.Controls.Add(_pbAcousticSpectrum);
            scMain.Panel1.Resize += AcousticPreview_Resize;
            _colorScale = new ColorScaleBar();
            _colorScale.BackColor = pbThermalImage.BackColor;
            _colorScale.Visible = false;
            _colorScale.ModeChanged += ColorScale_ModeChanged;
            _colorScale.RangeChanged += ColorScale_RangeChanged;
            scMain.Panel1.Controls.Add(_colorScale);
            scMain.Panel1.Resize += ThermalPreview_Resize;
            _grpAcoustic = new GroupBox
            {
                Text = "Acoustic Parameters",
                Padding = new Padding(3)
            };
            _grpAcoustic.Controls.Add(_acousticSettings);
            _thermalImaging = new ThermalImagingPanel();
            _thermalImaging.Applied += (sender, args) => UpdateRendererParams();
            _grpImaging = new GroupBox
            {
                Text = "Imaging",
                Padding = new Padding(3)
            };
            _grpImaging.Controls.Add(_thermalImaging);

            tlpSections.Visible = false;
            pnlSettings.AutoScroll = false;
            _tcSettings = new TabControl
            {
                Dock = DockStyle.Fill,
                Multiline = false
            };
            pnlSettings.Controls.Add(_tcSettings);
            AddThermalParamButtons();
            PinSectionButtons();

            _tsslConnection = new ToolStripStatusLabel("Not connected")
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _tsslMode = new ToolStripStatusLabel
            {
                BorderSides = ToolStripStatusLabelBorderSides.Left
            };
            _tsslStats = new ToolStripStatusLabel
            {
                BorderSides = ToolStripStatusLabelBorderSides.Left
            };
            _ssMain = new StatusStrip();
            _ssMain.Items.AddRange(new ToolStripItem[] { _tsslConnection, _tsslMode, _tsslStats });
            Controls.Add(_ssMain);

            _statsTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _statsTimer.Tick += StatsTimer_Tick;
            _statsTimer.Start();

            _nudBlend = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 100,
                Value = _paletteAlpha,
                Width = 48,
                TextAlign = HorizontalAlignment.Right
            };
            _nudBlend.ValueChanged += nudBlend_ValueChanged;
            _tslBlend = new ToolStripLabel("Blend:");
            _tsBlend = new ToolStripControlHost(_nudBlend)
            {
                AutoSize = false,
                Width = 56,
                Margin = new Padding(0, 2, 0, 2)
            };
            int blendAt = tsMainForm.Items.IndexOf(tsbInverted) + 1;
            tsMainForm.Items.Insert(blendAt, _tslBlend);
            tsMainForm.Items.Insert(blendAt + 1, _tsBlend);
            ConfigureForDeviceMode(DeviceMode.Unknown);

            int ret = UvcSdkNative.uvc_sdk_init();
            if (!UvcSdkNative.IsOk(ret))
                MessageBox.Show($"SDK init failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                _hotplugCb = OnHotplug;
                UvcSdkNative.uvc_set_hotplug_callback(_hotplugCb);
            }

            var ver = new UvcSdkNative.UvcSdkVersion();
            UvcSdkNative.uvc_get_sdk_version(ref ver);
            string title = $"IRtekUVCSDKDemo v{ver}";
            uint fileVer;
            string fileVerStr;
            if (IRtekFileSDK.TryGetSdkVersion(out fileVer, out fileVerStr) && !string.IsNullOrEmpty(fileVerStr))
                title += $" / FileSDK {fileVerStr}";
            Text = title;

            LoadPalettes();

            EnableControlBuffer(pnlSettings);
            EnableControlBuffer(_tcSettings);
            EnableControlBuffer(_thermalImaging);
            EnableControlBuffer(_acousticSettings);
            _loginUiFilter = new LoginUiFilter(this);
            Application.AddMessageFilter(_loginUiFilter);

            Adding = false;
            Deleting = false;
            CurrentMarkerType = MarkerType.None;
            Shown += RefreshDevicesOnStartup;
        }

        private void RefreshDevicesOnStartup(object sender, EventArgs e)
        {
            Shown -= RefreshDevicesOnStartup;
            tsbRefreshDevices_Click(tsbRefreshDevices, EventArgs.Empty);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _isDisposing = true;
            CancelDeviceBind();
            _statsTimer.Stop();

            if (_connected)
            {
                if (_streaming)
                    StopStream();
                Thread.Sleep(100);

                lock (_renderLock)
                {
                    if (_renderer != null)
                    {
                        _renderer.Dispose();
                        _renderer = null;
                    }
                }

                UvcSdkNative.uvc_logout(ref _uvcHandle);
                _uvcHandle = IntPtr.Zero;
                _connected = false;
            }

            UvcSdkNative.uvc_set_hotplug_callback(null);
            UvcSdkNative.uvc_sdk_cleanup();

            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_loginUiFilter != null)
                Application.RemoveMessageFilter(_loginUiFilter);
            _loginLoadDone.Dispose();
            _acousticPointMarker?.Dispose();
            base.OnFormClosed(e);
        }

        #region Device List

        private void tsbRefreshDevices_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var devices = new UvcSdkNative.UvcDevice[16];
                for (int i = 0; i < devices.Length; i++)
                    devices[i] = UvcSdkNative.UvcDevice.Create();

                ushort outLen;
                int ret = UvcSdkNative.uvc_get_uvc_list(devices, (ushort)devices.Length, out outLen);
                if (!UvcSdkNative.IsOk(ret))
                {
                    MessageBox.Show($"Get device list failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                tscbDeviceList.Items.Clear();
                for (int i = 0; i < outLen; i++)
                {
                    var dev = devices[i];
                    string path = FormatPathBytes(dev.path);
                    string label = $"0x{dev.vid:X4}:0x{dev.pid:X4} ({path})";
                    tscbDeviceList.Items.Add(new DeviceItem { Device = dev, Label = label });
                }

                if (outLen > 0)
                    tscbDeviceList.SelectedIndex = 0;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Login / Logout

        private void tsbLogin_Click(object sender, EventArgs e)
        {
            if (_connected)
            {
                MessageBox.Show("Already connected. Logout first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tscbDeviceList.SelectedIndex < 0 || !(tscbDeviceList.SelectedItem is DeviceItem devItem))
            {
                MessageBox.Show("Select a device first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool loading = false;
            try
            {
                Cursor = Cursors.WaitCursor;
                UseWaitCursor = true;

                int ret = UvcSdkNative.uvc_login(devItem.Device.vid, devItem.Device.pid, out _uvcHandle);
                if (!UvcSdkNative.IsOk(ret))
                {
                    MessageBox.Show($"Login failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _connected = true;
                _connectedVid = devItem.Device.vid;
                _connectedPid = devItem.Device.pid;
                _connectedPath = devItem.Device.path == null ? null : (byte[])devItem.Device.path.Clone();
                _deviceLinkDown = false;
                _resumeStreamAfterReconnect = false;
                _tsslConnection.Text = $"Connected  0x{devItem.Device.vid:X4}:0x{devItem.Device.pid:X4}  Loading...";
                _lastStatsFrames = 0;
                _tsslStats.Text = string.Empty;

                tscbDeviceList.Enabled = false;
                tsbRefreshDevices.Enabled = false;
                tsbLogin.Enabled = false;
                tsbLogout.Enabled = true;

                if (devItem.Device.vid == NemoVid && devItem.Device.pid == NemoPid)
                    _deviceMode = DeviceMode.Acoustic;
                else if (devItem.Device.vid == HollyVid && devItem.Device.pid == HollyPid)
                    _deviceMode = DeviceMode.Thermal;
                else
                    _deviceMode = DeviceMode.Unknown;

                if (_deviceMode == DeviceMode.Unknown)
                {
                    ConfigureForDeviceMode(_deviceMode);
                    MessageBox.Show("Unsupported device VID/PID.", Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                loading = true;
                BeginDeviceBind();
            }
            finally
            {
                if (!loading)
                    RestoreUiCursor();
            }
        }

        private void BeginDeviceBind()
        {
            int generation = ++_loginGeneration;
            IntPtr handle = _uvcHandle;
            DeviceMode mode = _deviceMode;
            _loginUiLocked = true;
            _isDisposing = false;
            DeviceReadCache.Begin(handle);
            _loginLoadDone.Reset();
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    if (mode == DeviceMode.Thermal)
                        DeviceReadCache.PrimeThermal(handle);
                    else if (mode == DeviceMode.Acoustic)
                        DeviceReadCache.PrimeAcoustic(handle);
                }
                catch
                {
                }
                finally
                {
                    _loginLoadDone.Set();
                }

                if (_isDisposing || generation != _loginGeneration)
                    return;

                try
                {
                    BeginInvoke(new Action(delegate
                    {
                        FinishDeviceBind(generation, handle);
                    }));
                }
                catch (InvalidOperationException)
                {
                    _loginUiLocked = false;
                    DeviceReadCache.End();
                }
            });
        }

        private void FinishDeviceBind(int generation, IntPtr handle)
        {
            if (IsDisposed || _isDisposing || generation != _loginGeneration || !_connected || _uvcHandle != handle)
            {
                _loginUiLocked = false;
                DeviceReadCache.End();
                if (!IsDisposed)
                    RestoreUiCursor();
                return;
            }

            try
            {
                RestoreUiCursor();
                SuspendLayout();
                tsMainForm.SuspendLayout();
                pnlSettings.SuspendLayout();
                _tcSettings.SuspendLayout();
                try
                {
                    _tsslConnection.Text = $"Connected  0x{_connectedVid:X4}:0x{_connectedPid:X4}";
                    ConfigureForDeviceMode(_deviceMode);
                    if (_deviceMode == DeviceMode.Acoustic)
                    {
                        LoadProperties();
                        LoadImageFlip();
                        LoadPalettesFromDevice();
                        _acousticSettings.LoadFromDevice(_uvcHandle);
                        LoadDisplayParams();
                        SyncAcousticColorScale();
                    }
                    else if (_deviceMode == DeviceMode.Thermal)
                    {
                        LoadProperties();
                        LoadThermalParams();
                        LoadFrameList();
                        LoadRanges();
                        LoadImageFlip();
                        LoadPalettesFromDevice();
                        _thermalImaging.LoadFromDevice(_uvcHandle);
                        SyncColorScaleFromDevice();
                        LoadDisplayParams();

                        try
                        {
                            _renderer = new ThermalImageRenderer(DisplayWidth, DisplayHeight);
                            UpdateRendererParams();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Init renderer failed: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                finally
                {
                    _tcSettings.ResumeLayout(false);
                    pnlSettings.ResumeLayout(false);
                    tsMainForm.ResumeLayout(false);
                    ResumeLayout(true);
                }

                BeginInvoke(new Action(delegate
                {
                    if (IsDisposed || generation != _loginGeneration || !_connected || _uvcHandle != handle)
                        return;
                    StartStream();
                }));
            }
            finally
            {
                _loginUiLocked = false;
                DeviceReadCache.End();
                RestoreUiCursor();
            }
        }

        private void RestoreUiCursor()
        {
            UseWaitCursor = false;
            Cursor = Cursors.Default;
            lvProperties.UseWaitCursor = false;
            lvProperties.Cursor = Cursors.Default;
            Cursor.Current = Cursors.Default;
        }

        private void CancelDeviceBind()
        {
            _loginGeneration++;
            DeviceReadCache.Cancel();
            _loginLoadDone.WaitOne();
            _loginUiLocked = false;
            DeviceReadCache.End();
        }

        private void tsbLogout_Click(object sender, EventArgs e)
        {
            if (!_connected) return;

            CancelDeviceBind();
            if (!_connected || IsDisposed) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                _isDisposing = true;
                _colorScaleFollowFrame = false;

                if (_streaming)
                    StopStream();
                Thread.Sleep(100);

                lock (_renderLock)
                {
                    if (_renderer != null)
                    {
                        _renderer.Dispose();
                        _renderer = null;
                    }
                }

                int ret = UvcSdkNative.uvc_logout(ref _uvcHandle);
                if (!UvcSdkNative.IsOk(ret))
                    MessageBox.Show($"Logout failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                _uvcHandle = IntPtr.Zero;
                _connected = false;
                _connectedVid = 0;
                _connectedPid = 0;
                _connectedPath = null;
                _deviceLinkDown = false;
                _resumeStreamAfterReconnect = false;
                _isDisposing = false;
                _tsslConnection.Text = "Not connected";
                _tsslStats.Text = string.Empty;
                _lastStatsFrames = 0;
                _frameCounter = 0;
                _acousticSettings.ClearDevice();
                _thermalImaging.ClearDevice();
                ResetColorScale();
                LoadPalettes();
                ConfigureForDeviceMode(DeviceMode.Unknown);

                tscbDeviceList.Enabled = true;
                tsbRefreshDevices.Enabled = true;
                tsbLogin.Enabled = true;
                tsbLogout.Enabled = false;
                tsbCalibrate.Enabled = false;

                lvProperties.Items.Clear();
                RefitSettingsSections();
                _acousticVisualW = 0;
                _acousticVisualH = 0;
                _acousticFps = 0;
                _acousticFpsFrames = 0;
                _acousticFpsWindowStart = 0;
                _frameList.Clear();
                tscbRange.Items.Clear();

                lock (_markerLock) { _markerCount = 0; }
                _spotCounter = 0;
                _areaCounter = 0;
                _lineCounter = 0;
                lock (_frameLock)
                {
                    _lastRawFrame = null;
                    _lastLutAd = null;
                    _lastLutTemp = null;
                    _thermalGrid = null;
                    _thermalGridW = 0;
                    _thermalGridH = 0;
                    _lastAcousticFrame = null;
                    _lastAcousticFrameW = 0;
                    _lastAcousticFrameH = 0;
                }
                lock (_acousticUiLock)
                {
                    _acousticPending?.Dispose();
                    _acousticPending = null;
                    _acousticSpectrumPending = null;
                    _acousticPendingExtremeCount = 0;
                }
                _acousticExtremeCount = 0;
                ResetAcousticSpectrum();
                _acousticRawW = 0;
                _acousticRawH = 0;
                _acousticFreqReady = false;
                _lastSpectrumView = null;
                ClearAcousticSpectrumImage();
                RefreshMarkerListView();

                HideCursorTemperature(false);
                var old = pbThermalImage.Image;
                pbThermalImage.Image = null;
                old?.Dispose();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Properties

        private void LoadProperties()
        {
            lvProperties.BeginUpdate();
            try
            {
                lvProperties.Items.Clear();
                if (_deviceMode == DeviceMode.Acoustic)
                    LoadAcousticProperties();
                else if (_deviceMode == DeviceMode.Thermal)
                    LoadThermalProperties();
            }
            finally
            {
                lvProperties.EndUpdate();
                lvProperties.ShowGroups = false;
                lvProperties.ShowGroups = true;
                lvProperties.Cursor = Cursors.Default;
            }
            RefitSettingsSections();
        }

        private void LoadThermalProperties()
        {
            var devInfo = UvcSdkNative.UvcDeviceInfo.Create();
            int ret = DeviceReadCache.GetDeviceInfo(_uvcHandle, ref devInfo);
            if (UvcSdkNative.IsOk(ret))
            {
                string model = UvcSdkNative.BytesToString(devInfo.model);
                string serialNo = UvcSdkNative.BytesToString(devInfo.serial_no);
                string id = UvcSdkNative.BytesToString(devInfo.id);
                string brand = UvcSdkNative.BytesToString(devInfo.brand);
                string company = UvcSdkNative.BytesToString(devInfo.company);
                string fwName = UvcSdkNative.BytesToString(devInfo.firmware_name);
                string fwVersion = UvcSdkNative.BytesToString(devInfo.firmware_version);

                var grp = lvProperties.Groups["lvgCameraInfo"];
                lvProperties.Items.Add(new ListViewItem(new[] { "Model", model }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Serial No.", serialNo }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "ID", id }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Brand", brand }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Company", company }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Firmware", fwName }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "FW Version", fwVersion }, grp));
            }

            var lensInfo = UvcSdkNative.UvcLensInfo.Create();
            ret = DeviceReadCache.GetLensInfo(_uvcHandle, ref lensInfo);
            if (UvcSdkNative.IsOk(ret))
            {
                string lensName = UvcSdkNative.BytesToString(lensInfo.name);
                string band = UvcSdkNative.BytesToString(lensInfo.band);

                var grp = lvProperties.Groups["lvgLensInfo"];
                lvProperties.Items.Add(new ListViewItem(new[] { "Name", lensName }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Focal", $"{lensInfo.focal:F1}mm" }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "HFov", $"{lensInfo.hfov:F1}°" }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "VFov", $"{lensInfo.vfov:F1}°" }, grp));
                lvProperties.Items.Add(new ListViewItem(new[] { "Band", band }, grp));

                var rgrp = lvProperties.Groups["lvgTempRange"];
                for (int i = 0; i < lensInfo.ranges_count && i < lensInfo.range_map.Length; i++)
                {
                    var r = lensInfo.range_map[i];
                    string rangeStr = $"{r.range.min:F0}~{r.range.max:F0}°C";
                    lvProperties.Items.Add(new ListViewItem(new[] { $"Range {r.id}", rangeStr }, rgrp));
                }
            }

            var thermalGrp = lvProperties.Groups["lvgThermal"];
            var thermalSize = new UvcSdkNative.UvcSize();
            ret = DeviceReadCache.GetThermalResolution(_uvcHandle, ref thermalSize);
            if (UvcSdkNative.IsOk(ret))
            {
                lvProperties.Items.Add(new ListViewItem(new[] {
                    "Resolution", $"{thermalSize.width}x{thermalSize.height}"
                }, thermalGrp));
            }

            UvcSdkNative.UvcSrMode srMode;
            ret = DeviceReadCache.GetThermalSr(_uvcHandle, out srMode);
            if (UvcSdkNative.IsOk(ret))
            {
                lvProperties.Items.Add(new ListViewItem(new[] {
                    "SR Mode", srMode.ToString()
                }, thermalGrp));
            }
        }

        private void LoadAcousticProperties()
        {
            var stale = lvProperties.Groups["lvgAcoustic"];
            if (stale != null)
                lvProperties.Groups.Remove(stale);
            chProperty.Width = 150;

            var camera = EnsurePropertyGroup("Camera Info");
            var devInfo = UvcSdkNative.UvcDeviceInfo.Create();
            int ret = DeviceReadCache.GetDeviceInfo(_uvcHandle, ref devInfo);
            if (UvcSdkNative.IsOk(ret))
            {
                AddPropertyRow(camera, "Model", ValueOrDash(UvcSdkNative.BytesToString(devInfo.model)));
                AddPropertyRow(camera, "Serial No.", ValueOrDash(UvcSdkNative.BytesToString(devInfo.serial_no)));
                AddPropertyRow(camera, "ID", ValueOrDash(UvcSdkNative.BytesToString(devInfo.id)));
                AddPropertyRow(camera, "Brand", ValueOrDash(UvcSdkNative.BytesToString(devInfo.brand)));
                AddPropertyRow(camera, "Company", ValueOrDash(UvcSdkNative.BytesToString(devInfo.company)));
                AddPropertyRow(camera, "Firmware", ValueOrDash(UvcSdkNative.BytesToString(devInfo.firmware_name)));
                AddPropertyRow(camera, "FW Version", ValueOrDash(UvcSdkNative.BytesToString(devInfo.firmware_version)));
            }
            else if (tscbDeviceList.SelectedItem is DeviceItem devItem)
            {
                AddPropertyRow(camera, "VID", "0x" + devItem.Device.vid.ToString("X4"));
                AddPropertyRow(camera, "PID", "0x" + devItem.Device.pid.ToString("X4"));
            }

            var sensorGrp = EnsurePropertyGroup("Sensor Info");
            var info = UvcSdkNative.UvcAcousticSensorInfo.Create();
            ret = DeviceReadCache.GetAcousticSensor(_uvcHandle, ref info);
            if (UvcSdkNative.IsOk(ret))
            {
                AddPropertyRow(sensorGrp, "Firmware", ValueOrDash(UvcSdkNative.BytesToString(info.firmware_version)));
                AddPropertyRow(sensorGrp, "Serial No.", ValueOrDash(UvcSdkNative.BytesToString(info.serial_no)));
                AddPropertyRow(sensorGrp, "Sample Rate", info.sample_rate + " Hz");
                AddPropertyRow(sensorGrp, "Mic Count", info.mic_count.ToString());
                AddPropertyRow(sensorGrp, "Frequency Range",
                    info.freq_range.min.ToString("F0") + "-" + info.freq_range.max.ToString("F0") + " Hz");
                if (info.freq_range.max > info.freq_range.min)
                {
                    _acousticFreqMinHz = info.freq_range.min;
                    _acousticFreqMaxHz = info.freq_range.max;
                    _acousticFreqReady = true;
                }
            }

            var sizeGrp = EnsurePropertyGroup("Size");
            var acousticSize = new UvcSdkNative.UvcSize();
            ret = DeviceReadCache.GetAcousticResolution(_uvcHandle, ref acousticSize);
            if (UvcSdkNative.IsOk(ret))
            {
                _acousticRawW = acousticSize.width;
                _acousticRawH = acousticSize.height;
            }
            string acousticSizeText = UvcSdkNative.IsOk(ret)
                ? FormatPixelSize(acousticSize.width, acousticSize.height)
                : "—";
            AddPropertyRow(sizeGrp, "Acoustic Size", acousticSizeText);
            AddPropertyRow(sizeGrp, "Visual Size", FormatPixelSize(_acousticVisualW, _acousticVisualH));
            AddPropertyRow(sizeGrp, "Frame Rate", _acousticFps > 0 ? _acousticFps.ToString("F1") + " fps" : "—");
        }

        private ListViewGroup EnsurePropertyGroup(string header)
        {
            foreach (ListViewGroup group in lvProperties.Groups)
            {
                if (group.Header == header)
                    return group;
            }

            var created = new ListViewGroup(header, HorizontalAlignment.Left);
            int insertAt = lvProperties.Groups.Count;
            bool inBlock = false;
            for (int i = 0; i < lvProperties.Groups.Count; i++)
            {
                string existing = lvProperties.Groups[i].Header;
                if (existing == "Camera Info" || existing == "Sensor Info" || existing == "Size")
                {
                    insertAt = i + 1;
                    inBlock = true;
                }
                else if (inBlock)
                {
                    break;
                }
            }
            lvProperties.Groups.Insert(insertAt, created);
            return created;
        }

        private void AddPropertyRow(ListViewGroup group, string key, string value)
        {
            var item = new ListViewItem(key);
            item.SubItems.Add(value ?? string.Empty);
            lvProperties.Items.Add(item);
            item.Group = group;
        }

        private static string FormatPixelSize(int width, int height)
        {
            return width > 0 && height > 0 ? width + "x" + height : "—";
        }

        private void NoteAcousticFrame(int width, int height)
        {
            bool sizeChanged = width != _acousticVisualW || height != _acousticVisualH;
            _acousticVisualW = width;
            _acousticVisualH = height;

            int now = Environment.TickCount;
            if (_acousticFpsWindowStart == 0)
                _acousticFpsWindowStart = now;
            _acousticFpsFrames++;
            int elapsed = unchecked(now - _acousticFpsWindowStart);
            bool fpsReady = elapsed >= 1000;
            if (fpsReady)
            {
                _acousticFps = _acousticFpsFrames * 1000.0 / elapsed;
                _acousticFpsFrames = 0;
                _acousticFpsWindowStart = now;
            }
            if ((!sizeChanged && !fpsReady) || !IsHandleCreated || IsDisposed)
                return;
            if (System.Threading.Interlocked.Exchange(ref _acousticSizePosted, 1) == 1)
                return;
            try
            {
                BeginInvoke(new Action(() =>
                {
                    _acousticSizePosted = 0;
                    if (_isDisposing || _deviceMode != DeviceMode.Acoustic)
                        return;
                    SetPropertyValue("Visual Size", FormatPixelSize(_acousticVisualW, _acousticVisualH));
                    SetPropertyValue("Frame Rate", _acousticFps > 0 ? _acousticFps.ToString("F1") + " fps" : "—");
                }));
            }
            catch (InvalidOperationException)
            {
                _acousticSizePosted = 0;
            }
        }

        private void SetPropertyValue(string key, string value)
        {
            foreach (ListViewItem item in lvProperties.Items)
            {
                if (item.Text == key && item.SubItems.Count > 1)
                {
                    item.SubItems[1].Text = value;
                    return;
                }
            }
        }

        #endregion

        #region Thermal Params

        private TableLayoutPanel _tlpShutter;

        private void LoadThermalParams()
        {
            _isGuiRefreshing = true;
            tlpThermalParams.SuspendLayout();
            _tlpShutter.SuspendLayout();
            try
            {
                ReadMeasureParams();
                ReadShutterParams();
            }
            finally
            {
                _tlpShutter.ResumeLayout(true);
                tlpThermalParams.ResumeLayout(true);
                _isGuiRefreshing = false;
            }
        }

        private void ReadMeasureParams()
        {
            var p = new UvcSdkNative.UvcThermalParams();
            int ret = DeviceReadCache.GetThermalParams(_uvcHandle, ref p);
            if (!UvcSdkNative.IsOk(ret))
                return;

            nudEmissivity.Value = Clamp((decimal)p.emissivity, nudEmissivity.Minimum, nudEmissivity.Maximum);
            nudReflTemp.Value = Clamp(KelvinToCelsius(p.refl_temp), nudReflTemp.Minimum, nudReflTemp.Maximum);
            nudDistance.Value = Clamp((decimal)p.distance, nudDistance.Minimum, nudDistance.Maximum);
            nudAtmTemp.Value = Clamp(KelvinToCelsius(p.atm_temp), nudAtmTemp.Minimum, nudAtmTemp.Maximum);
            nudRelHumidity.Value = Clamp((decimal)(p.rel_humidity * 100f), nudRelHumidity.Minimum, nudRelHumidity.Maximum);
        }

        private void ReadShutterParams()
        {
            UvcSdkNative.UvcShutterMode shutterMode;
            int ret = DeviceReadCache.GetShutterMode(_uvcHandle, out shutterMode);
            if (UvcSdkNative.IsOk(ret))
                cbShutterMode.SelectedIndex = (int)shutterMode;

            ushort shutterMin;
            ret = DeviceReadCache.GetShutterTime(_uvcHandle, out shutterMin);
            if (UvcSdkNative.IsOk(ret))
                nudShutterTime.Value = Clamp(shutterMin, nudShutterTime.Minimum, nudShutterTime.Maximum);
        }

        private void AddThermalParamButtons()
        {
            lblShutterTimeUnit.Anchor = AnchorStyles.Left;
            lblDistanceUnit.Anchor = AnchorStyles.Left;
            lblRelHumidityUnit.Anchor = AnchorStyles.Left;
            lblReflTempUnit.Anchor = AnchorStyles.Left;
            lblAtmTempUnit.Anchor = AnchorStyles.Left;
            UseCelsiusRange(nudReflTemp);
            UseCelsiusRange(nudAtmTemp);
            StackSettingsRows(tlpThermalParams, true,
                new Control[] { lblEmissivity, nudEmissivity },
                new Control[] { lblReflTemp, nudReflTemp, lblReflTempUnit },
                new Control[] { lblDistance, nudDistance, lblDistanceUnit },
                new Control[] { lblAtmTemp, nudAtmTemp, lblAtmTempUnit },
                new Control[] { lblRelHumidity, nudRelHumidity, lblRelHumidityUnit });

            _tlpShutter = new TableLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(8),
                Margin = Padding.Empty
            };
            StackSettingsRows(_tlpShutter, true,
                new Control[] { lblShutterMode, cbShutterMode },
                new Control[] { lblShutterTime, nudShutterTime, lblShutterTimeUnit });

            grpThermal.Controls.Remove(tlpThermalParams);
            var shutterGroup = SettingsGroup("Shutter", _tlpShutter);
            var parameterGroup = SettingsGroup("Parameters", tlpThermalParams);
            grpThermal.Controls.Add(shutterGroup);
            grpThermal.Controls.Add(parameterGroup);

            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = Padding.Empty
            };
            var get = SettingsButton("Get");
            var set = SettingsButton("Set");
            get.Click += (sender, args) =>
            {
                if (!_connected) return;
                LoadThermalParams();
            };
            set.Click += (sender, args) =>
            {
                if (!_connected) return;
                ApplyThermalParams();
                ApplyShutter();
            };
            buttons.Controls.Add(get);
            buttons.Controls.Add(set);
            _thermalParamButtons = buttons;
        }

        private static GroupBox SettingsGroup(string text, Control layout)
        {
            var group = new GroupBox
            {
                Text = text,
                Dock = DockStyle.Top,
                Padding = new Padding(3)
            };
            layout.Dock = DockStyle.Top;
            group.Controls.Add(layout);
            int chrome = group.Height - group.DisplayRectangle.Height;
            if (chrome < 18)
                chrome = 18;
            group.Height = Math.Max(1, layout.PreferredSize.Height + chrome + group.Padding.Vertical);
            return group;
        }

        private void PinSectionButtons()
        {
            tlpDisplay.Controls.Remove(btnDisplayGet);
            tlpDisplay.Controls.Remove(btnDisplaySet);
            StackSettingsRows(tlpDisplay, false,
                new Control[] { lblContrast, nudContrast },
                new Control[] { lblBrightness, nudBrightness },
                new Control[] { cbDdeEnable },
                new Control[] { lblDdeValue, nudDdeValue },
                new Control[] { cbSrEnable });
            tlpDisplay.SetColumnSpan(cbDdeEnable, 2);
            tlpDisplay.SetColumnSpan(cbSrEnable, 2);
            cbDdeEnable.CheckedChanged += (sender, args) => UpdateDdeValueVisible();
            UpdateDdeValueVisible();
            var displayButtons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = Padding.Empty
            };
            displayButtons.Controls.Add(btnDisplayGet);
            displayButtons.Controls.Add(btnDisplaySet);

            _sectionButtonHosts[grpThermal] = ButtonHost(_thermalParamButtons);
            _sectionButtonHosts[_grpImaging] = ButtonHost(_thermalImaging.CommandButtons);
            _sectionButtonHosts[grpDisplay] = ButtonHost(displayButtons);
            _sectionButtonHosts[_grpAcoustic] = ButtonHost(_acousticSettings.CommandButtons);
        }

        private static Button SettingsButton(string text)
        {
            return new Button
            {
                Text = text,
                AutoSize = false,
                Size = new Size(75, 23),
                MinimumSize = new Size(75, 23),
                MaximumSize = new Size(75, 23),
                Margin = new Padding(6, 0, 0, 0),
                UseVisualStyleBackColor = true
            };
        }

        private static void UniformSettingsButtons(Control root)
        {
            foreach (Control child in root.Controls)
            {
                var button = child as Button;
                if (button != null && (button.Text == "Get" || button.Text == "Set"))
                {
                    button.AutoSize = false;
                    button.Size = new Size(75, 23);
                    button.MinimumSize = button.Size;
                    button.MaximumSize = button.Size;
                    button.Margin = new Padding(6, 0, 0, 0);
                }
                else
                {
                    UniformSettingsButtons(child);
                }
            }
        }

        private static Control ButtonHost(Control buttons)
        {
            UniformSettingsButtons(buttons);
            var flow = buttons as FlowLayoutPanel;
            if (flow != null)
                flow.WrapContents = true;
            buttons.Margin = Padding.Empty;
            var host = new Panel { Dock = DockStyle.Bottom, Height = 36 };
            buttons.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            host.Controls.Add(buttons);
            host.Resize += (sender, args) => PlaceSettingsButtons(host, buttons);
            PlaceSettingsButtons(host, buttons);
            return host;
        }

        private static void PlaceSettingsButtons(Control host, Control buttons)
        {
            int width = Math.Max(1, host.ClientSize.Width - 8);
            buttons.MaximumSize = new Size(width, 0);
            Size preferred = buttons.GetPreferredSize(new Size(width, 0));
            int buttonWidth = Math.Min(width, Math.Max(1, preferred.Width));
            int buttonHeight = Math.Max(23, preferred.Height);
            var size = new Size(buttonWidth, buttonHeight);
            if (buttons.Size != size)
                buttons.Size = size;
            int hostHeight = buttonHeight + 12;
            if (host.Height != hostHeight)
                host.Height = hostHeight;
            var location = new Point(Math.Max(0, host.ClientSize.Width - buttons.Width - 8), 6);
            if (buttons.Location != location)
                buttons.Location = location;
        }

        private void FixSettingsPanelWidth()
        {
            scMain.Panel2MinSize = SettingsPanelWidth;
            scMain.FixedPanel = FixedPanel.Panel2;
            scMain.IsSplitterFixed = true;
            int distance = scMain.Width - scMain.SplitterWidth - SettingsPanelWidth;
            int minimum = scMain.Panel1MinSize;
            int maximum = scMain.Width - scMain.SplitterWidth - scMain.Panel2MinSize;
            if (distance < minimum)
                distance = minimum;
            if (distance > maximum)
                distance = maximum;
            scMain.SplitterDistance = distance;
            chProperty.Width = 110;
            chValue.Width = 280;
        }

        private static void StackSettingsRows(TableLayoutPanel layout, bool includeUnit, params Control[][] rows)
        {
            layout.SuspendLayout();
            layout.Controls.Clear();
            layout.ColumnStyles.Clear();
            layout.RowStyles.Clear();
            layout.ColumnCount = includeUnit ? 3 : 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsLabelWidth));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsEditorWidth));
            if (includeUnit)
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, SettingsUnitWidth));
            layout.RowCount = rows.Length;
            for (int row = 0; row < rows.Length; row++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                Control[] fields = rows[row];
                for (int column = 0; column < fields.Length; column++)
                {
                    Control field = fields[column];
                    if (field is NumericUpDown || field is ComboBox)
                        field.Width = SettingsEditorWidth;
                    field.Anchor = AnchorStyles.Left;
                    layout.Controls.Add(field, column, row);
                }
            }
            layout.ResumeLayout();
        }

        private static void UseCelsiusRange(NumericUpDown box)
        {
            box.Minimum = -273.15m;
            box.Maximum = 5476.85m;
            box.Value = 20m;
        }

        private static decimal KelvinToCelsius(float kelvin)
        {
            return (decimal)kelvin - 273.15m;
        }

        private static float CelsiusToKelvin(float celsius)
        {
            return celsius + ColorScaleCelsiusOffset;
        }

        private void ApplyThermalParams()
        {
            var p = new UvcSdkNative.UvcThermalParams
            {
                emissivity = (float)nudEmissivity.Value,
                refl_temp = CelsiusToKelvin((float)nudReflTemp.Value),
                distance = (float)nudDistance.Value,
                atm_temp = CelsiusToKelvin((float)nudAtmTemp.Value),
                rel_humidity = (float)nudRelHumidity.Value / 100f
            };

            int ret = UvcSdkNative.uvc_set_thermal_params(_uvcHandle, ref p);
            if (!UvcSdkNative.IsOk(ret))
                MessageBox.Show($"Set thermal params failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                UpdateRendererParams();
        }

        private void ApplyShutter()
        {
            if (cbShutterMode.SelectedIndex >= 0)
            {
                var mode = (UvcSdkNative.UvcShutterMode)(byte)cbShutterMode.SelectedIndex;
                int ret = UvcSdkNative.uvc_set_thermal_shutter_mode(_uvcHandle, ref mode);
                if (!UvcSdkNative.IsOk(ret))
                    MessageBox.Show($"Set shutter mode failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ushort minutes = (ushort)nudShutterTime.Value;
            int timeRet = UvcSdkNative.uvc_set_thermal_shutter_time(_uvcHandle, ref minutes);
            if (!UvcSdkNative.IsOk(timeRet))
                MessageBox.Show($"Set shutter time failed: {timeRet}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Stream

        private bool LoadFrameList()
        {
            _frameList.Clear();

            var frames = new UvcSdkNative.UvcFrameInfo[32];
            for (int i = 0; i < frames.Length; i++)
                frames[i] = UvcSdkNative.UvcFrameInfo.Create();

            ushort outLen;
            int ret = DeviceReadCache.GetFrameList(_uvcHandle, frames, (ushort)frames.Length, out outLen);
            if (!UvcSdkNative.IsOk(ret))
            {
                MessageBox.Show($"Get frame list failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int count = Math.Min(outLen, frames.Length);
            for (int i = 0; i < count; i++)
            {
                var f = frames[i];
                string format = UvcSdkNative.BytesToString(f.format);
                string label = $"{f.width}x{f.height} {format} (if={f.interface_num} alt={f.alt_setting} fi={f.frame_index} fo={f.format_index})";
                _frameList.Add(new FrameItem { Index = i, Frame = f, Label = label });
            }
            return true;
        }

        private bool SameConnectedDevice(UvcSdkNative.UvcDevice device)
        {
            if (!_connected || device.vid != _connectedVid || device.pid != _connectedPid)
                return false;
            if (_connectedPath == null || device.path == null)
                return false;

            int length = Math.Min(_connectedPath.Length, device.path.Length);
            for (int i = 0; i < length; i++)
            {
                if (_connectedPath[i] != device.path[i])
                    return false;
            }
            return true;
        }

        private void OnHotplug(ref UvcSdkNative.UvcDevice device, bool found)
        {
            if (_isDisposing || !SameConnectedDevice(device))
                return;

            bool arrived = found;
            try
            {
                if (IsHandleCreated && !IsDisposed)
                    BeginInvoke(new Action(() => HandleDeviceLink(arrived)));
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void HandleDeviceLink(bool arrived)
        {
            if (_isDisposing || !_connected)
                return;

            if (!arrived)
            {
                _deviceLinkDown = true;
                _resumeStreamAfterReconnect = _streaming;
                return;
            }

            if (!_deviceLinkDown || _deviceMode != DeviceMode.Thermal)
                return;

            _deviceLinkDown = false;
            if (!LoadFrameList())
                return;

            if (!_resumeStreamAfterReconnect)
                return;

            _resumeStreamAfterReconnect = false;
            _streaming = false;
            StartStream();
        }

        private void StartStream()
        {
            if (!_connected || _streaming) return;
            _frameCb = OnFrameReceived;

            int ret;
            if (_deviceMode == DeviceMode.Acoustic)
            {
                ret = UvcSdkNative.uvc_set_stream_callback(_uvcHandle, IntPtr.Zero, _frameCb);
            }
            else if (_deviceMode == DeviceMode.Thermal)
            {
                if (_frameList.Count == 0)
                {
                    _frameCb = null;
                    return;
                }
                _currentFrame = _frameList[0].Frame;
                ret = UvcSdkNative.uvc_set_stream_callback(_uvcHandle, ref _currentFrame, _frameCb);
            }
            else
            {
                _frameCb = null;
                return;
            }
            if (!UvcSdkNative.IsOk(ret))
            {
                _frameCb = null;
                MessageBox.Show($"Start stream failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _streaming = true;
        }

        private void StopStream()
        {
            if (!_streaming) return;

            int ret;
            if (_deviceMode == DeviceMode.Acoustic)
                ret = UvcSdkNative.uvc_set_stream_callback(_uvcHandle, IntPtr.Zero, null);
            else if (_deviceMode == DeviceMode.Thermal)
                ret = UvcSdkNative.uvc_set_stream_callback(_uvcHandle, ref _currentFrame, null);
            else
                ret = 0;
            if (!UvcSdkNative.IsOk(ret))
                MessageBox.Show($"Stop stream failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

            _streaming = false;
            _frameCb = null;
        }

        private void OnFrameReceived(ref UvcSdkNative.UvcFrameData frame)
        {
            if (_isDisposing) return;
            if (frame.data == IntPtr.Zero || frame.length == 0)
            {
                return;
            }

            if (_deviceMode == DeviceMode.Acoustic)
            {
                RenderAcousticFrame(ref frame);
                return;
            }

            ThermalImageRenderer renderer;
            lock (_renderLock)
            {
                if (_renderer == null)
                {
                    return;
                }
                renderer = _renderer;
            }

            try
            {
                int w = frame.width;
                int h = frame.height;
                if (w <= 0 || h <= 4)
                {
                    return;
                }
                long totalBytes = (long)w * h * 2;
                if (totalBytes <= 0 || totalBytes > int.MaxValue || frame.length < (uint)totalBytes)
                {
                    return;
                }

                byte[] fullFrame = new byte[totalBytes];
                Marshal.Copy(frame.data, fullFrame, 0, (int)totalBytes);

                int imageH = h - 4;
                int metaOffset = w * imageH * 2;
                if (metaOffset < 0 || metaOffset + 32 > fullFrame.Length) return;

                uint lutLength = BitConverter.ToUInt32(fullFrame, metaOffset);
                if (lutLength > 0 && lutLength <= 4096)
                {
                    int tempOffset = metaOffset + 32;
                    int adOffset = tempOffset + (int)lutLength * 4;
                    int lutEnd = adOffset + (int)lutLength * 2;
                    if (lutEnd <= fullFrame.Length)
                    {
                        float[] temps = new float[lutLength];
                        ushort[] ads = new ushort[lutLength];
                        Buffer.BlockCopy(fullFrame, tempOffset, temps, 0, (int)lutLength * 4);
                        Buffer.BlockCopy(fullFrame, adOffset, ads, 0, (int)lutLength * 2);
                        renderer.SetLut(ads, temps);
                        lock (_frameLock)
                        {
                            _lastLutAd = (ushort[])ads.Clone();
                            _lastLutTemp = (float[])temps.Clone();
                        }
                    }
                }

                int imageBytes = w * imageH * 2;
                byte[] nativeGrid = new byte[imageBytes];
                Buffer.BlockCopy(fullFrame, 0, nativeGrid, 0, imageBytes);
                byte[] dataToRender = (w == DisplayWidth && imageH == DisplayHeight)
                    ? (byte[])nativeGrid.Clone()
                    : ResizeRawData(nativeGrid, w, imageH, DisplayWidth, DisplayHeight);

                lock (_frameLock)
                {
                    _lastRawFrame = (byte[])dataToRender.Clone();
                    _lastRawW = DisplayWidth;
                    _lastRawH = DisplayHeight;
                    _thermalGrid = nativeGrid;
                    _thermalGridW = w;
                    _thermalGridH = imageH;
                }

                renderer.ProcessFrame(dataToRender);

                var bmp = RenderThermalFrame(ref frame);
                if (bmp != null)
                {
                    ApplyFlip(bmp);
                    DrawMarkerOverlay(bmp);
                    bmp = PresentThermalView(bmp);

                    _frameCounter++;
                    bool refreshList = (_frameCounter % 15 == 0) && _markerCount > 0;

                    BeginInvoke(new Action(() =>
                    {
                        if (_isDisposing)
                        {
                            bmp.Dispose();
                            return;
                        }
                        if (_cursorOverVideo)
                            UpdateCursorTemperature(_cursorClient, false);
                        var old = pbThermalImage.Image;
                        pbThermalImage.Image = bmp;
                        old?.Dispose();

                        if (refreshList)
                            RefreshMarkerListView();
                    }));
                }
            }
            catch
            {
            }
        }

        private Bitmap RenderThermalFrame(ref UvcSdkNative.UvcFrameData frame)
        {
            int width = frame.width;
            int height = frame.height - 4;
            // Super-resolution keeps the 2x image. Reserve that capacity; the call
            // writes the actual width, height, and byte count back into image.
            long bgrBytes = (long)width * height * 3 * 4;
            if (width <= 0 || height <= 0 || bgrBytes <= 0 || bgrBytes > int.MaxValue)
                return null;

            if (_thermalPixels == null || _thermalPixels.LongLength != bgrBytes)
                _thermalPixels = new byte[bgrBytes];

            GCHandle pin = GCHandle.Alloc(_thermalPixels, GCHandleType.Pinned);
            Bitmap bitmap = null;
            try
            {
                var image = new UvcSdkNative.UvcFrameData
                {
                    data = pin.AddrOfPinnedObject(),
                    length = (uint)_thermalPixels.Length
                };
                var colorSpace = UvcSdkNative.UvcColorSpaceType.Bgr;
                var info = new UvcSdkNative.UvcThermalImageInfo
                {
                    size = (uint)Marshal.SizeOf(typeof(UvcSdkNative.UvcThermalImageInfo))
                };
                int ret = UvcSdkNative.uvc_get_thermal_image_from_frame(
                    _uvcHandle, ref frame, ref colorSpace, ref image, ref info);
                int outWidth = image.width;
                int outHeight = image.height;
                long outBytes = (long)outWidth * outHeight * 3;
                bool dataMoved = image.data != pin.AddrOfPinnedObject();
                if (!UvcSdkNative.IsOk(ret) ||
                    dataMoved ||
                    outWidth <= 0 || outHeight <= 0 ||
                    outBytes <= 0 || outBytes > _thermalPixels.Length ||
                    image.length < (uint)outBytes)
                {
                    return null;
                }

                if (_colorScaleFollowFrame)
                    PublishAutoColorScale(info.gain);

                bitmap = new Bitmap(outWidth, outHeight, PixelFormat.Format24bppRgb);
                BitmapData data = bitmap.LockBits(
                    new Rectangle(0, 0, outWidth, outHeight),
                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                try
                {
                    int sourceStride = outWidth * 3;
                    for (int y = 0; y < outHeight; y++)
                    {
                        Marshal.Copy(_thermalPixels, y * sourceStride,
                            IntPtr.Add(data.Scan0, y * data.Stride), sourceStride);
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                Bitmap result = bitmap;
                bitmap = null;
                return result;
            }
            finally
            {
                bitmap?.Dispose();
                pin.Free();
            }
        }

        private void RenderAcousticFrame(ref UvcSdkNative.UvcFrameData frame)
        {
            // Acoustic callback returns the raw FF/51 composite frame.
            // width/height are the visible resolution; length is the composite byte count.
            int width = frame.width;
            int height = frame.height;
            long bgrBytes = (long)width * height * 3;
            if (width <= 0 || height <= 0 || bgrBytes <= 0 || bgrBytes > int.MaxValue ||
                frame.data == IntPtr.Zero || frame.length == 0)
                return;
            NoteAcousticFrame(width, height);
            if (frame.length <= int.MaxValue)
            {
                byte[] composite = new byte[frame.length];
                Marshal.Copy(frame.data, composite, 0, (int)frame.length);
                lock (_frameLock)
                {
                    _lastAcousticFrame = composite;
                    _lastAcousticFrameW = width;
                    _lastAcousticFrameH = height;
                }
            }

            if (_acousticPixels == null || _acousticPixels.LongLength != bgrBytes)
                _acousticPixels = new byte[bgrBytes];
            byte[] pixels = _acousticPixels;
            GCHandle pin = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            Bitmap bitmap = null;
            try
            {
                var image = new UvcSdkNative.UvcFrameData
                {
                    data = pin.AddrOfPinnedObject(),
                    length = (uint)pixels.Length
                };
                var colorSpace = UvcSdkNative.UvcColorSpaceType.Bgr;
                var info = new UvcSdkNative.UvcAcousticImageInfo
                {
                    size = (uint)Marshal.SizeOf(typeof(UvcSdkNative.UvcAcousticImageInfo))
                };
                int ret = UvcSdkNative.uvc_get_acoustic_image_from_frame(
                    _uvcHandle, ref frame, ref colorSpace, ref image, ref info);
                int outWidth = image.width;
                int outHeight = image.height;
                long outBytes = (long)outWidth * outHeight * 3;
                if (!UvcSdkNative.IsOk(ret) ||
                    image.data != pin.AddrOfPinnedObject() ||
                    outWidth <= 0 || outHeight <= 0 ||
                    outBytes <= 0 || outBytes > pixels.Length ||
                    image.length < (uint)outBytes)
                    return;

                bitmap = new Bitmap(outWidth, outHeight, PixelFormat.Format24bppRgb);
                var rect = new Rectangle(0, 0, outWidth, outHeight);
                BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                try
                {
                    int sourceStride = outWidth * 3;
                    if (data.Stride == sourceStride)
                    {
                        Marshal.Copy(pixels, 0, data.Scan0, (int)outBytes);
                    }
                    else
                    {
                        for (int y = 0; y < outHeight; y++)
                        {
                            Marshal.Copy(pixels, y * sourceStride,
                                IntPtr.Add(data.Scan0, y * data.Stride), sourceStride);
                        }
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                ApplyFlip(bitmap);
                Bitmap ready = bitmap;
                bitmap = null;
                float dbMin = info.db_scale.min;
                float dbMax = info.db_scale.max;
                int extremeCount = info.summary.extreme_points_count;
                if (extremeCount < 0)
                    extremeCount = 0;
                else if (extremeCount > UvcSdkNative.UVC_ACOUSTIC_EXTREME_POINT_COUNT)
                    extremeCount = UvcSdkNative.UVC_ACOUSTIC_EXTREME_POINT_COUNT;
                SpectrumSnapshot spectrum = AdvanceAcousticSpectrum(
                    ReadAcousticSpectrum(frame.data, frame.length, width, height));
                bool post = false;
                lock (_acousticUiLock)
                {
                    _acousticPending?.Dispose();
                    _acousticPending = ready;
                    _acousticSpectrumPending = spectrum;
                    _acousticPendingDbMin = dbMin;
                    _acousticPendingDbMax = dbMax;
                    _acousticPendingExtremeCount = extremeCount;
                    for (int i = 0; i < extremeCount; i++)
                    {
                        UvcSdkNative.UvcAcousticPoint point = info.summary.ExtremePoint(i);
                        _acousticPendingExtremes[i] = new AcousticSourcePoint
                        {
                            X = point.x,
                            Y = point.y,
                            Value = point.value
                        };
                    }
                    if (!_acousticUiQueued)
                    {
                        _acousticUiQueued = true;
                        post = true;
                    }
                }
                if (!post)
                    return;

                BeginInvoke(new Action(() =>
                {
                    Bitmap show;
                    SpectrumSnapshot view;
                    float scaleMin;
                    float scaleMax;
                    int sourceCount;
                    AcousticSourcePoint source0;
                    AcousticSourcePoint source1;
                    AcousticSourcePoint source2;
                    lock (_acousticUiLock)
                    {
                        show = _acousticPending;
                        view = _acousticSpectrumPending;
                        scaleMin = _acousticPendingDbMin;
                        scaleMax = _acousticPendingDbMax;
                        sourceCount = _acousticPendingExtremeCount;
                        source0 = _acousticPendingExtremes[0];
                        source1 = _acousticPendingExtremes[1];
                        source2 = _acousticPendingExtremes[2];
                        _acousticPending = null;
                        _acousticSpectrumPending = null;
                        _acousticUiQueued = false;
                    }
                    if (show == null)
                        return;
                    if (_isDisposing || _deviceMode != DeviceMode.Acoustic)
                    {
                        show.Dispose();
                        return;
                    }
                    _acousticExtremes[0] = source0;
                    _acousticExtremes[1] = source1;
                    _acousticExtremes[2] = source2;
                    _acousticExtremeCount = sourceCount;
                    if (_cursorOverVideo)
                        UpdateCursorTemperature(_cursorClient, false);
                    var old = pbThermalImage.Image;
                    pbThermalImage.Image = show;
                    old?.Dispose();
                    ShowAcousticSpectrum(view);
                    ApplyAcousticColorScale(scaleMin, scaleMax);
                }));
            }
            catch
            {
                bitmap?.Dispose();
            }
            finally
            {
                if (pin.IsAllocated)
                    pin.Free();
            }
        }

        // Visible BGR, then the raw grid (uvc_get_acoustic_resolution, 16-bit samples),
        // then little-endian 16-bit spectrum bins. Bin 0 is 2 kHz and the last bin is 100 kHz.
        private float[] ReadAcousticSpectrum(IntPtr data, uint length, int width, int height)
        {
            int rawW = _acousticRawW;
            int rawH = _acousticRawH;
            if (data == IntPtr.Zero || rawW <= 0 || rawH <= 0 || width <= 0 || height <= 0)
                return null;
            long offset = (long)width * height * 3 + (long)rawW * rawH * 2;
            if (offset < 0 || offset >= length || offset > int.MaxValue)
                return null;
            long bytes = length - offset;
            if (bytes < 4 || (bytes % 2) != 0 || bytes / 2 > 65536)
                return null;
            int bins = (int)(bytes / 2);
            byte[] raw = new byte[bins * 2];
            Marshal.Copy(IntPtr.Add(data, (int)offset), raw, 0, raw.Length);
            float[] values = new float[bins];
            for (int i = 0; i < bins; i++)
            {
                short sample = (short)(raw[i * 2] | (raw[i * 2 + 1] << 8));
                values[i] = AcousticSpectrumSampleToDb(sample);
            }
            return values;
        }

        private static float AcousticSpectrumSampleToDb(short sample)
        {
            return sample > 0 ? sample / 64f : 0f;
        }

        // AnalyzIR reads fft[frequency / delta]. Delta is 100 Hz, so index 20 is 2 kHz and index 1000 is 100 kHz.
        private static float[] BuildAcousticFileSpectrum(float[] bins)
        {
            int count = (int)(AcousticSpectrumMaxHz / AcousticSpectrumDeltaHz) + 1;
            float[] fft = new float[count];
            if (bins == null || bins.Length == 0)
                return fft;
            int first = (int)(AcousticSpectrumMinHz / AcousticSpectrumDeltaHz);
            if (bins.Length == 1)
            {
                for (int i = first; i < count; i++)
                    fft[i] = bins[0];
                return fft;
            }

            float span = AcousticSpectrumMaxHz - AcousticSpectrumMinHz;
            for (int i = first; i < count; i++)
            {
                float position = (i * AcousticSpectrumDeltaHz - AcousticSpectrumMinHz) / span * (bins.Length - 1);
                int left = (int)position;
                if (left >= bins.Length - 1)
                {
                    fft[i] = bins[bins.Length - 1];
                    continue;
                }
                fft[i] = bins[left] + (bins[left + 1] - bins[left]) * (position - left);
            }
            return fft;
        }

        private static float AcousticSampleToDb(int sample)
        {
            return (sample - AcousticDbOffset) / 64f;
        }

        private void AcousticFrequencyRangeChanged(object sender, EventArgs e)
        {
            float minHz = _acousticSettings.FrequencyMinHz;
            float maxHz = _acousticSettings.FrequencyMaxHz;
            if (maxHz <= minHz)
                return;
            _acousticFreqMinHz = minHz;
            _acousticFreqMaxHz = maxHz;
            _acousticFreqReady = true;
            if (_deviceMode == DeviceMode.Acoustic)
                ShowAcousticSpectrum(_lastSpectrumView);
        }

        private void ShowAcousticSpectrum(SpectrumSnapshot view)
        {
            if (_pbAcousticSpectrum == null || !_pbAcousticSpectrum.Visible || view == null)
                return;
            _lastSpectrumView = view;
            int width = Math.Max(1, _pbAcousticSpectrum.ClientSize.Width);
            int height = Math.Max(1, _pbAcousticSpectrum.ClientSize.Height);
            var panel = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            PaintAcousticSpectrum(panel, view);
            Image previous = _pbAcousticSpectrum.Image;
            _pbAcousticSpectrum.Image = panel;
            previous?.Dispose();
        }

        private void ClearAcousticSpectrumImage()
        {
            if (_pbAcousticSpectrum == null)
                return;
            Image old = _pbAcousticSpectrum.Image;
            _pbAcousticSpectrum.Image = null;
            old?.Dispose();
        }

        private void ResetAcousticSpectrum()
        {
            lock (_spectrumLock)
            {
                _spectrumFrame = 0;
                _spectrumColumns = null;
                _spectrumPeak = 0f;
                _adoptedSpectrumMax = 0f;
                _pendingSpectrumMax = 0f;
                _spectrumPendingSinceUtc = DateTime.UtcNow;
            }
        }

        private SpectrumSnapshot AdvanceAcousticSpectrum(float[] samples)
        {
            lock (_spectrumLock)
            {
                _spectrumFrame++;
                if ((_spectrumFrame % 2) == 1 || _spectrumColumns == null)
                {
                    if (_spectrumColumns == null || _spectrumColumns.Length != SpectrumPanelWidth)
                        _spectrumColumns = new float[SpectrumPanelWidth];
                    ResampleSpectrum(samples, _spectrumColumns, out _spectrumPeak);
                }
                if (_adoptedSpectrumMax <= 0f)
                    _adoptedSpectrumMax = _spectrumPeak;
                DateTime now = DateTime.UtcNow;
                if (Math.Abs(_spectrumPeak - _adoptedSpectrumMax) > 5f)
                {
                    if (Math.Abs(_spectrumPeak - _pendingSpectrumMax) > 0.5f)
                    {
                        _pendingSpectrumMax = _spectrumPeak;
                        _spectrumPendingSinceUtc = now;
                    }
                    else if (now - _spectrumPendingSinceUtc >= TimeSpan.FromSeconds(5))
                    {
                        _adoptedSpectrumMax = _pendingSpectrumMax;
                    }
                }
                else
                {
                    _pendingSpectrumMax = _adoptedSpectrumMax;
                    _spectrumPendingSinceUtc = now;
                }
                return new SpectrumSnapshot
                {
                    Columns = (float[])_spectrumColumns.Clone(),
                    Peak = _spectrumPeak,
                    Adopted = _adoptedSpectrumMax
                };
            }
        }

        private static void ResampleSpectrum(float[] values, float[] columns, out float peak)
        {
            int bins = values == null ? 0 : values.Length;
            peak = 0f;
            for (int i = 0; i < columns.Length; i++)
                columns[i] = 0f;
            if (bins == 0)
                return;
            for (int i = 0; i < bins; i++)
            {
                if (values[i] > peak)
                    peak = values[i];
            }
            if (bins <= columns.Length)
            {
                for (int x = 0; x < columns.Length; x++)
                {
                    float position = columns.Length == 1
                        ? 0f
                        : (float)x * (bins - 1) / (columns.Length - 1);
                    int left = (int)position;
                    int right = Math.Min(left + 1, bins - 1);
                    columns[x] = values[left] + (values[right] - values[left]) * (position - left);
                }
                return;
            }
            for (int x = 0; x < columns.Length; x++)
            {
                int begin = x * bins / columns.Length;
                int end = Math.Max(begin + 1, (x + 1) * bins / columns.Length);
                for (int i = begin; i < end; i++)
                {
                    float value = values[Math.Min(i, bins - 1)];
                    if (value > columns[x])
                        columns[x] = value;
                }
            }
        }

        private void LayoutAcousticSpectrum()
        {
            if (_pbAcousticSpectrum == null || _colorScale == null)
                return;
            int clientW = Math.Max(1, scMain.Panel1.ClientSize.Width);
            int clientH = Math.Max(1, scMain.Panel1.ClientSize.Height);
            int left = Math.Max(0, _thermalPreviewBounds.X);
            int top = Math.Max(0, _thermalPreviewBounds.Y);
            const int gap = 6;
            const int edge = 3;
            int fullWidth = Math.Max(1, clientW - left - edge);
            int barWidth = ColorScaleBar.BarWidth;
            int imageWidth = Math.Max(1, fullWidth - gap - barWidth);
            int spectrumHeight = mlvMarkers.Height;
            int spectrumTop = clientH - SpectrumBottomMargin - spectrumHeight;
            if (spectrumTop < top + 80)
                spectrumTop = top + 80;
            int imageHeight = Math.Max(1, spectrumTop - gap - top);

            pbThermalImage.Dock = DockStyle.None;
            pbThermalImage.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pbThermalImage.Bounds = new Rectangle(left, top, imageWidth, imageHeight);

            _colorScale.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _colorScale.Bounds = new Rectangle(left + imageWidth + gap, top, barWidth, imageHeight);
            _colorScale.BringToFront();

            var bounds = new Rectangle(left, spectrumTop, fullWidth, spectrumHeight);
            if (_pbAcousticSpectrum.Bounds != bounds)
            {
                _pbAcousticSpectrum.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                _pbAcousticSpectrum.Bounds = bounds;
            }
        }

        private void AcousticPreview_Resize(object sender, EventArgs e)
        {
            if (_deviceMode != DeviceMode.Acoustic || _pbAcousticSpectrum == null || !_pbAcousticSpectrum.Visible)
                return;
            LayoutAcousticSpectrum();
            ShowAcousticSpectrum(_lastSpectrumView);
        }

        private void PaintAcousticSpectrum(Bitmap image, SpectrumSnapshot view)
        {
            if (image == null || view == null || view.Columns == null ||
                image.PixelFormat != PixelFormat.Format32bppArgb)
                return;
            int width = image.Width;
            int height = image.Height;
            if (width < 2 || height < 2)
                return;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData locked = image.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                int stride = locked.Stride;
                byte[] pixels = new byte[stride * height];
                for (int row = 0; row < height; row++)
                {
                    int rowOffset = row * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int offset = rowOffset + x * 4;
                        pixels[offset] = SpectrumBackColor.B;
                        pixels[offset + 1] = SpectrumBackColor.G;
                        pixels[offset + 2] = SpectrumBackColor.R;
                        pixels[offset + 3] = 255;
                    }
                }
                int maxBarHeight = Math.Max(1, height * 75 / SpectrumViewHeight);
                float dbMax = Math.Max(1f, view.Adopted + 10f);
                for (int x = 0; x < SpectrumPanelWidth && x < view.Columns.Length; x++)
                {
                    int waveHeight = (int)(view.Columns[x] * maxBarHeight / dbMax);
                    if (waveHeight > maxBarHeight)
                        waveHeight = maxBarHeight;
                    if (waveHeight < 0)
                        waveHeight = 0;
                    int x0 = SpectrumColumnLeft(x, width);
                    int x1 = SpectrumColumnLeft(x + 1, width);
                    if (x1 <= x0)
                        x1 = x0 + 1;
                    for (int y = height - waveHeight; y < height; y++)
                    {
                        float rowDb = (height - y) * dbMax / maxBarHeight;
                        int colorIndex = view.Peak > 0f ? (int)(rowDb * 255f / view.Peak) : 0;
                        if (colorIndex < 0)
                            colorIndex = 0;
                        if (colorIndex > 255)
                            colorIndex = 255;
                        uint packed = RedBluePalette[colorIndex];
                        byte r = (byte)(packed >> 24);
                        byte g = (byte)(packed >> 16);
                        byte b = (byte)(packed >> 8);
                        for (int px = x0; px < x1; px++)
                        {
                            PutSpectrumPixel(pixels, stride, width, height, px, y, r, g, b);
                        }
                    }
                }
                if (_acousticFreqReady && _acousticFreqMaxHz > _acousticFreqMinHz)
                {
                    DrawSpectrumRuler(pixels, stride, width, height,
                        _acousticFreqMinHz, _acousticFreqMaxHz);
                }
                Marshal.Copy(pixels, 0, locked.Scan0, pixels.Length);
            }
            finally
            {
                image.UnlockBits(locked);
            }
            if (_acousticFreqReady && _acousticFreqMaxHz > _acousticFreqMinHz)
                DrawSpectrumLabels(image, _acousticFreqMinHz, _acousticFreqMaxHz, Font);
        }

        private static int SpectrumColumnLeft(int column, int viewWidth)
        {
            if (column < 0)
                column = 0;
            int plot = viewWidth - SpectrumSidePadding * 2;
            int origin = SpectrumSidePadding;
            if (plot < 1)
            {
                plot = viewWidth;
                origin = 0;
            }
            return origin + column * plot / SpectrumPanelWidth;
        }

        private static int SpectrumFrequencyX(float hz)
        {
            float khz = hz / 1000f;
            int offset = (int)Math.Round((khz - SpectrumMinKhz) * SpectrumColumnsPerKhz);
            if (offset < 0)
                offset = 0;
            if (offset > SpectrumPanelWidth - 1)
                offset = SpectrumPanelWidth - 1;
            return offset;
        }

        private static void DrawSpectrumRuler(
            byte[] pixels, int stride, int width, int height, float minHz, float maxHz)
        {
            int bottomY = height - 1;
            for (int khz = 2; khz <= 100; khz = khz == 2 ? 5 : khz + 5)
            {
                int logical = (khz - 2) * SpectrumColumnsPerKhz;
                int x0 = SpectrumColumnLeft(logical, width);
                int x1 = SpectrumColumnLeft(logical + 1, width);
                if (x1 <= x0)
                    x1 = x0 + 1;
                bool endpoint = khz == 2 || khz == 100;
                int tickHeight = endpoint ? 36 : (khz % 10 == 0 ? 12 : 8);
                int tickTop = bottomY - tickHeight + 1;
                for (int y = tickTop; y <= bottomY; y++)
                {
                    for (int x = x0; x < x1; x++)
                        PutSpectrumPixel(pixels, stride, width, height, x, y, 255, 255, 255);
                }
            }

            DrawSpectrumRangeLine(pixels, stride, width, height, SpectrumFrequencyX(minHz));
            DrawSpectrumRangeLine(pixels, stride, width, height, SpectrumFrequencyX(maxHz));
        }

        private static void DrawSpectrumLabels(Bitmap image, float minHz, float maxHz, Font font)
        {
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                int width = image.Width;
                int bottomY = image.Height - 1;
                float lineHeight = graphics.MeasureString("10k", font).Height;
                for (int khz = 2; khz <= 100; khz = khz == 2 ? 5 : khz + 5)
                {
                    if (khz != 2 && khz % 10 != 0)
                        continue;
                    int logical = (khz - 2) * SpectrumColumnsPerKhz;
                    int x0 = SpectrumColumnLeft(logical, width);
                    int tickHeight = khz == 2 || khz == 100 ? 36 : 12;
                    int tickTop = bottomY - tickHeight + 1;
                    DrawSpectrumLabel(graphics, font, khz + "k", x0, tickTop - 4, Color.White, width);
                }

                float amberBottom = bottomY - 36 - lineHeight - 2;
                DrawSpectrumLabel(graphics, font, SpectrumFrequencyText(minHz), SpectrumColumnLeft(SpectrumFrequencyX(minHz), width), amberBottom, Color.FromArgb(255, 191, 0), width);
                DrawSpectrumLabel(graphics, font, SpectrumFrequencyText(maxHz), SpectrumColumnLeft(SpectrumFrequencyX(maxHz), width), amberBottom, Color.FromArgb(255, 191, 0), width);
            }
        }

        private static string SpectrumFrequencyText(float hz)
        {
            float khz = hz / 1000f;
            return Math.Abs(khz - (float)Math.Round(khz)) < 0.05f
                ? ((int)Math.Round(khz)) + "k"
                : khz.ToString("0.0") + "k";
        }

        private static void DrawSpectrumLabel(Graphics graphics, Font font, string text, float centerX, float bottom, Color color, int canvasWidth)
        {
            SizeF size = graphics.MeasureString(text, font);
            float x = centerX - size.Width / 2f;
            float y = bottom - size.Height;
            if (x < 0)
                x = 0;
            if (x + size.Width > canvasWidth)
                x = Math.Max(0, canvasWidth - size.Width);
            if (y < 0)
                y = 0;
            using (var brush = new SolidBrush(color))
                graphics.DrawString(text, font, brush, x, y);
        }

        private static void DrawSpectrumRangeLine(
            byte[] pixels, int stride, int width, int height, int logicalX)
        {
            int start = logicalX >= SpectrumPanelWidth - 1 ? logicalX - 1 : logicalX;
            int x0 = SpectrumColumnLeft(start, width);
            int x1 = SpectrumColumnLeft(start + 2, width);
            if (x1 <= x0)
                x1 = x0 + 2;
            int bottomY = height - 1;
            for (int lineX = x0; lineX < x1; lineX++)
            {
                for (int y = bottomY - 35; y <= bottomY; y++)
                    PutSpectrumPixel(pixels, stride, width, height, lineX, y, 255, 191, 0);
            }
        }

        private static void PutSpectrumPixel(
            byte[] pixels, int stride, int width, int height, int x, int y, byte r, byte g, byte b)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;
            int offset = y * stride + x * 4;
            pixels[offset] = b;
            pixels[offset + 1] = g;
            pixels[offset + 2] = r;
            pixels[offset + 3] = 255;
        }

        private static readonly uint[] RedBluePalette =
        {
            2147548928, 2147548928, 2113994496, 2063662848, 2030108416, 1979776768,
            1946222336, 1895890688, 1862336256, 1812004608, 1778450176, 1728118528,
            1694564096, 1644232448, 1610678016, 1560346368, 1526791936, 1476460288,
            1442905856, 1392574208, 1359019776, 1308688128, 1275133696, 1224802048,
            1191247616, 1140915968, 1107361536, 1057029888, 1006698240, 973143808,
            922812160, 889257728, 838926080, 805371648, 755040000, 721485568,
            671153920, 637599488, 587267840, 553713408, 503381760, 469827328,
            419495680, 385941248, 335609600, 302055168, 251723520, 218169088,
            167837440, 134283008, 83951360, 50396928, 130816, 458496, 786176,
            1113856, 1441536, 1769216, 2096896, 2424576, 2752256, 3079936,
            3407616, 3735296, 4062976, 4390656, 4718336, 5046016, 5373696,
            5701376, 6029056, 6356736, 6684416, 7012096, 7339776, 7667456,
            7995136, 8322816, 8650496, 8978176, 9305856, 9633536, 9961216,
            10288896, 10616576, 10944256, 11271936, 11599616, 11927296, 12254976,
            12582656, 12910336, 13238016, 13565696, 13893376, 14221056, 14548736,
            14876416, 15204096, 15531776, 15859456, 16187136, 16514816, 16776192,
            16774912, 16773632, 16772352, 16771072, 16769792, 16768512, 16767232,
            16765952, 16764672, 16763392, 16762112, 16760832, 16759552, 16758272,
            16756992, 16755712, 16754432, 16753152, 16751872, 16750592, 16749312,
            16748032, 16746752, 16745472, 16744192, 16742912, 16741632, 16740352,
            16739072, 16737792, 16736512, 16735232, 16733952, 16732672, 16731392,
            16730112, 16728832, 16727552, 16726272, 16724992, 16723712, 16722432,
            16721152, 16719872, 16718592, 16717312, 16716032, 16714752, 16713472,
            16712192, 67043328, 150929408, 234815488, 318701568, 402587648,
            486473728, 570359808, 654245888, 738131968, 822018048, 905904128,
            989790208, 1073676288, 1157562368, 1241448448, 1325334528, 1409220608,
            1493106688, 1576992768, 1660878848, 1744764928, 1828651008, 1912537088,
            1996423168, 2080309248, 2164195328, 2248081408, 2331967488, 2415853568,
            2499739648, 2583625728, 2667511808, 2751397888, 2835283968, 2919170048,
            3003056128, 3086942208, 3170828288, 3254714368, 3338600448, 3422486528,
            3506372608, 3590258688, 3674144768, 3758030848, 3841916928, 3925803008,
            4009689088, 4093575168, 4177461248, 4261347328, 4294639616, 4294311936,
            4293984256, 4293656576, 4293328896, 4293001216, 4292673536, 4292345856,
            4292018176, 4291690496, 4291362816, 4291035136, 4290707456, 4290379776,
            4290052096, 4289724416, 4289396736, 4289069056, 4288741376, 4288413696,
            4288086016, 4287758336, 4287430656, 4287102976, 4286775296, 4286447616,
            4286119936, 4285792256, 4285464576, 4285136896, 4284809216, 4284481536,
            4284153856, 4283826176, 4283498496, 4283170816, 4282843136, 4282515456,
            4282187776, 4281860096, 4281532416, 4281204736, 4280877056, 4280549376,
            4280221696, 4279894016, 4279566336, 4279238656, 4278910976, 4278583296,
            4278190080
        };

        private sealed class SpectrumSnapshot
        {
            public float[] Columns;
            public float Peak;
            public float Adopted;
        }

        private sealed class SpectrumPictureBox : PictureBox
        {
            public SpectrumPictureBox()
            {
                BackColor = SpectrumBackColor;
            }
        }

        private byte[] ResizeRawData(byte[] rawData, int srcW, int srcH, int dstW, int dstH)
        {
            int dstPixelCount = dstW * dstH;
            byte[] result = new byte[dstPixelCount * 2];

            float scaleX = (float)srcW / dstW;
            float scaleY = (float)srcH / dstH;

            for (int y = 0; y < dstH; y++)
            {
                for (int x = 0; x < dstW; x++)
                {
                    int srcX = (int)(x * scaleX);
                    int srcY = (int)(y * scaleY);
                    if (srcX >= srcW) srcX = srcW - 1;
                    if (srcY >= srcH) srcY = srcH - 1;

                    int srcIndex = (srcY * srcW + srcX) * 2;
                    int dstIndex = (y * dstW + x) * 2;
                    result[dstIndex] = rawData[srcIndex];
                    result[dstIndex + 1] = rawData[srcIndex + 1];
                }
            }

            return result;
        }

        #endregion

        #region Range

        private void LoadRanges()
        {
            tscbRange.Items.Clear();

            var lensInfo = UvcSdkNative.UvcLensInfo.Create();
            int ret = DeviceReadCache.GetLensInfo(_uvcHandle, ref lensInfo);
            if (!UvcSdkNative.IsOk(ret) || lensInfo.ranges_count == 0)
                return;

            _isGuiRefreshing = true;
            try
            {
                for (int i = 0; i < lensInfo.ranges_count && i < lensInfo.range_map.Length; i++)
                {
                    var r = lensInfo.range_map[i];
                    string label = $"{r.range.min:F0}~{r.range.max:F0}°C";
                    tscbRange.Items.Add(new RangeItem { Id = r.id, Label = label });
                }

                ushort currentRange;
                ret = DeviceReadCache.GetThermalRange(_uvcHandle, out currentRange);
                if (UvcSdkNative.IsOk(ret))
                {
                    for (int i = 0; i < tscbRange.Items.Count; i++)
                    {
                        if (((RangeItem)tscbRange.Items[i]).Id == currentRange)
                        {
                            tscbRange.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            finally
            {
                _isGuiRefreshing = false;
            }
        }

        private void tscbRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isGuiRefreshing || !_connected || tscbRange.SelectedItem == null) return;

            var item = (RangeItem)tscbRange.SelectedItem;
            ushort rangeId = item.Id;
            int ret = UvcSdkNative.uvc_set_thermal_range(_uvcHandle, ref rangeId);
            if (!UvcSdkNative.IsOk(ret))
                MessageBox.Show($"Set range failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Image Flip

        private int _flipMode;

        private void LoadImageFlip()
        {
            if (!_connected) return;

            _isGuiRefreshing = true;
            try
            {
                if (_deviceMode == DeviceMode.Thermal)
                {
                    UvcSdkNative.UvcImageFlipMode mode;
                    int ret = DeviceReadCache.GetImageFlip(_uvcHandle, out mode);
                    if (UvcSdkNative.IsOk(ret))
                    {
                        int idx = (int)mode;
                        if (idx >= 0 && idx < tscbImageFlip.Items.Count)
                        {
                            tscbImageFlip.SelectedIndex = idx;
                            _flipMode = idx;
                            return;
                        }
                    }
                }

                if (tscbImageFlip.SelectedIndex < 0 && tscbImageFlip.Items.Count > 0)
                    tscbImageFlip.SelectedIndex = 0;
                _flipMode = tscbImageFlip.SelectedIndex >= 0 ? tscbImageFlip.SelectedIndex : 0;
            }
            finally
            {
                _isGuiRefreshing = false;
            }
        }

        private void tscbImageFlip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isGuiRefreshing) return;
            if (tscbImageFlip.SelectedIndex < 0) return;
            _flipMode = tscbImageFlip.SelectedIndex;
            if (!_connected || _deviceMode != DeviceMode.Thermal) return;

            var mode = (UvcSdkNative.UvcImageFlipMode)(byte)_flipMode;
            int ret = UvcSdkNative.uvc_set_image_flip(_uvcHandle, ref mode);
            if (!UvcSdkNative.IsOk(ret))
                MessageBox.Show($"Set image flip failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private Bitmap ApplyFlip(Bitmap bmp)
        {
            if (bmp == null || _flipMode == 0) return bmp;
            switch (_flipMode)
            {
                case 1: bmp.RotateFlip(RotateFlipType.RotateNoneFlipY); break;
                case 2: bmp.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
                case 3: bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY); break;
            }
            return bmp;
        }

        #endregion

        #region Calibrate

        private void tsbCalibrate_Click(object sender, EventArgs e)
        {
            if (!_connected) return;

            int ret = UvcSdkNative.uvc_thermal_calibrate(_uvcHandle);
            if (!UvcSdkNative.IsOk(ret))
                MessageBox.Show($"Calibrate failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Toolbar Stubs (Save, Record, Drawing Tools, Palette, Alarm, ColorDist)

        private void tsbCapture_Click(object sender, EventArgs e)
        {
            if (!_connected) return;

            if (_deviceMode == DeviceMode.Acoustic)
            {
                SaveAcousticSnapshot();
                return;
            }

            byte[] rawFrame;
            int rawW, rawH;
            lock (_frameLock)
            {
                if (_lastRawFrame == null) return;
                rawFrame = (byte[])_lastRawFrame.Clone();
                rawW = _lastRawW;
                rawH = _lastRawH;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "IRtek Thermal Image|*.jpg";
                sfd.DefaultExt = "jpg";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    SaveThermalImageAsIrFile(sfd.FileName, rawFrame, rawW, rawH);
                    MessageBox.Show("Image saved.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Save failed: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveAcousticSnapshot()
        {
            byte[] composite;
            int width;
            int height;
            lock (_frameLock)
            {
                if (_lastAcousticFrame == null) return;
                composite = (byte[])_lastAcousticFrame.Clone();
                width = _lastAcousticFrameW;
                height = _lastAcousticFrameH;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "IRtek Acoustic Image|*.jpg";
                sfd.DefaultExt = "jpg";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    SaveAcousticImageAsAcFile(sfd.FileName, composite, width, height);
                    MessageBox.Show("Image saved.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Save failed: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveAcousticImageAsAcFile(string filePath, byte[] composite, int width, int height)
        {
            int rawW;
            int rawH;
            ReadAcousticGridSize(out rawW, out rawH);
            int visibleBytes = width * height * 3;
            int gridBytes = rawW * rawH * 2;
            if (width <= 0 || height <= 0 || rawW <= 0 || rawH <= 0 ||
                visibleBytes < 0 || gridBytes < 0 ||
                (long)visibleBytes + gridBytes > composite.Length)
                throw new InvalidOperationException("Acoustic frame is incomplete.");

            IntPtr h = IntPtr.Zero;
            int ret = IRtekACFileSDK.ac_file_create(filePath, IRtekACFileSDK.AC_FILE_PICTURE, ref h);
            if (ret != IRtekACFileSDK.AC_FILE_EC_OK || h == IntPtr.Zero)
                throw new InvalidOperationException($"ac_file_create failed: {ret}");

            try
            {
                bool focusEnabled;
                SetAcousticFileInfo(h, rawW, rawH, out focusEnabled);
                float[] spl = AppendAcousticFrame(h, composite, width, height, rawW, rawH, visibleBytes);
                SetLeakUserData(h, AcousticMaxDb(spl, rawW, rawH, focusEnabled));
                EnsureAcFile(IRtekACFileSDK.ac_file_set_frame_index(h, 0), "ac_file_set_frame_index");
                SetAcousticCoverImage(h);
                EnsureAcFile(IRtekACFileSDK.ac_file_save(h), "ac_file_save");
            }
            finally
            {
                IRtekACFileSDK.ac_file_close(h);
            }
        }

        private void SetAcousticFileInfo(IntPtr h, int rawW, int rawH, out bool focusEnabled)
        {
            focusEnabled = false;
            var devInfo = UvcSdkNative.UvcDeviceInfo.Create();
            if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_device_info(_uvcHandle, ref devInfo)))
            {
                var camera = IRtekACFileSDK.AcFileCameraInfo.Create();
                CopyCString(devInfo.model, camera.model);
                CopyCString(devInfo.serial_no, camera.serialNo);
                CopyCString(devInfo.brand, camera.brand);
                CopyCString(devInfo.company, camera.company);
                EnsureAcFile(IRtekACFileSDK.ac_file_set_camera_info(h, ref camera), "ac_file_set_camera_info");
            }

            var sensorInfo = UvcSdkNative.UvcAcousticSensorInfo.Create();
            if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_acoustic_sensor_info(_uvcHandle, ref sensorInfo)))
            {
                var sensor = IRtekACFileSDK.AcFileSensorInfo.Create();
                CopyCString(sensorInfo.serial_no, sensor.serialNo);
                sensor.channelCount = sensorInfo.mic_count;
                CopyCString(sensorInfo.firmware_version, sensor.version);
                sensor.sampleRate = sensorInfo.sample_rate > int.MaxValue
                    ? int.MaxValue
                    : (int)sensorInfo.sample_rate;
                EnsureAcFile(IRtekACFileSDK.ac_file_set_sensor_info(h, ref sensor), "ac_file_set_sensor_info");
            }

            var band = new IRtekACFileSDK.AcFileBand
            {
                max = AcousticSpectrumMaxHz,
                min = AcousticSpectrumMinHz
            };
            EnsureAcFile(IRtekACFileSDK.ac_file_set_frequency_range(h, ref band), "ac_file_set_frequency_range");

            var devicePalette = UvcSdkNative.UvcPalette.Create();
            if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_acoustic_palette(_uvcHandle, ref devicePalette)))
            {
                var palette = IRtekACFileSDK.AcFilePalette.Create();
                // Preset numbers drop the color table on save. 0xFF keeps this reversed copy.
                palette.refNo = 0xFF;
                int colorCount = palette.colors.Length;
                for (int i = 0; i < colorCount; i++)
                    palette.colors[i] = devicePalette.colors[colorCount - 1 - i];
                palette.alpha = devicePalette.alpha;
                EnsureAcFile(IRtekACFileSDK.ac_file_set_palette(h, ref palette), "ac_file_set_palette");
            }

            var size = new IRtekACFileSDK.AcFileSize { width = (short)rawW, height = (short)rawH };
            EnsureAcFile(IRtekACFileSDK.ac_file_set_acoustic_size(h, ref size), "ac_file_set_acoustic_size");
            EnsureAcFile(IRtekACFileSDK.ac_file_set_capture_mode(h, IRtekACFileSDK.AC_FILE_CAPTURE_MODE_LEAK), "ac_file_set_capture_mode");
            EnsureAcFile(IRtekACFileSDK.ac_file_set_distance(h, 1f), "ac_file_set_distance");

            var scale = new UvcSdkNative.UvcAcousticDbScale();
            if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_acoustic_db_scale(_uvcHandle, ref scale)))
            {
                var dbScale = new IRtekACFileSDK.AcFileDbScale
                {
                    mode = (int)scale.mode,
                    max = scale.max,
                    min = scale.min,
                    fixedSpan = scale.fixed_span,
                    span = scale.span
                };
                EnsureAcFile(IRtekACFileSDK.ac_file_set_db_scale(h, ref dbScale), "ac_file_set_db_scale");

                var dbScaleV2 = new IRtekACFileSDK.AcFileDbScaleV2
                {
                    mode = (int)scale.mode,
                    max = scale.max,
                    min = scale.min,
                    fixedSpan = scale.fixed_span,
                    span = scale.span,
                    sensitivity = scale.sensitivity,
                    sourceSize = scale.source_size,
                    limit = scale.limit
                };
                EnsureAcFile(IRtekACFileSDK.ac_file_set_db_scale_v2(h, ref dbScaleV2), "ac_file_set_db_scale_v2");

                var focus = new IRtekACFileSDK.AcFileFocusMode
                {
                    enabled = scale.focus_enabled,
                    scope = 0.5f
                };
                focusEnabled = scale.focus_enabled != 0;
                EnsureAcFile(IRtekACFileSDK.ac_file_set_focus_mode(h, ref focus), "ac_file_set_focus_mode");
            }
        }

        private static float[] AppendAcousticFrame(IntPtr h, byte[] composite, int width, int height, int rawW, int rawH, int visibleBytes)
        {
            int gridCount = rawW * rawH;
            float[] spl = new float[gridCount];
            int gridOffset = visibleBytes;
            for (int i = 0; i < gridCount; i++)
            {
                int offset = gridOffset + i * 2;
                int sample = composite[offset] | (composite[offset + 1] << 8);
                spl[i] = AcousticSampleToDb(sample);
            }

            byte[] visible = new byte[visibleBytes];
            Buffer.BlockCopy(composite, 0, visible, 0, visibleBytes);
            byte[] visualJpeg = EncodeBgrJpeg(visible, width, height);

            int spectrumOffset = gridOffset + gridCount * 2;
            int spectrumBytes = composite.Length - spectrumOffset;
            if (spectrumBytes < 0 || (spectrumBytes % 2) != 0)
                throw new InvalidOperationException("Acoustic spectrum is incomplete.");
            int bins = spectrumBytes / 2;
            float[] source = new float[bins];
            for (int i = 0; i < bins; i++)
            {
                int offset = spectrumOffset + i * 2;
                short sample = (short)(composite[offset] | (composite[offset + 1] << 8));
                source[i] = AcousticSpectrumSampleToDb(sample);
            }
            float[] fft = BuildAcousticFileSpectrum(source);

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            ulong timeStamp = (ulong)(DateTime.UtcNow - epoch).TotalMilliseconds;
            GCHandle splPin = GCHandle.Alloc(spl, GCHandleType.Pinned);
            GCHandle visualPin = GCHandle.Alloc(visualJpeg, GCHandleType.Pinned);
            GCHandle fftPin = fft.Length > 0 ? GCHandle.Alloc(fft, GCHandleType.Pinned) : default(GCHandle);
            try
            {
                var frame = new IRtekACFileSDK.AcFileFrame
                {
                    timeStamp = timeStamp,
                    acousticImageSpl = new IRtekACFileSDK.AcFileRaw
                    {
                        length = (uint)spl.Length,
                        buffer = splPin.AddrOfPinnedObject()
                    },
                    visualImage = new IRtekACFileSDK.AcFileBuffer
                    {
                        length = (uint)visualJpeg.Length,
                        buffer = visualPin.AddrOfPinnedObject()
                    },
                    spectrum = new IRtekACFileSDK.AcFileSpectrum
                    {
                        fft = new IRtekACFileSDK.AcFileRaw
                        {
                            length = (uint)fft.Length,
                            buffer = fft.Length > 0 ? fftPin.AddrOfPinnedObject() : IntPtr.Zero
                        },
                        range = new IRtekACFileSDK.AcFileBand { max = AcousticSpectrumMaxHz, min = AcousticSpectrumMinHz },
                        delta = AcousticSpectrumDeltaHz
                    }
                };
                EnsureAcFile(IRtekACFileSDK.ac_file_append_frame(h, ref frame), "ac_file_append_frame");
            }
            finally
            {
                splPin.Free();
                visualPin.Free();
                if (fftPin.IsAllocated)
                    fftPin.Free();
            }
            return spl;
        }

        private static void SetLeakUserData(IntPtr h, float maxDb)
        {
            const float correction = 1f;
            float leakLevel = (float)Math.Round(Math.Max(0f, (maxDb - 34f) / 7f), 1);
            float leakRate = 0f;
            if (leakLevel > 0f)
                leakRate = (float)(0.165 * Math.Exp(0.0578 * maxDb) * correction);

            string json = string.Format(CultureInfo.InvariantCulture,
                "{{\"Version\":100,\"LeakUnit\":1,\"Currency\":\"\u00a5\",\"EnergyCost\":2.7,\"GasCost\":0.0,\"LeakCorrection\":1.0,\"SpecificPower\":7,\"OperatingHours\":0,\"LeakLevel\":{0},\"LeakRate\":{1}}}",
                leakLevel, leakRate);
            byte[] value = Encoding.UTF8.GetBytes(json);
            var pair = IRtekACFileSDK.AcFileKvPair.Create();
            byte[] key = Encoding.ASCII.GetBytes("LEAK");
            Array.Copy(key, pair.key, key.Length);
            GCHandle pin = GCHandle.Alloc(value, GCHandleType.Pinned);
            try
            {
                pair.valueLength = (uint)value.Length;
                pair.valueBuffer = pin.AddrOfPinnedObject();
                EnsureAcFile(IRtekACFileSDK.ac_file_set_user_data(h, ref pair), "ac_file_set_user_data");
            }
            finally
            {
                pin.Free();
            }
        }

        private static float AcousticMaxDb(float[] spl, int width, int height, bool focusEnabled)
        {
            if (spl == null || spl.Length == 0)
                return 0f;
            if (!focusEnabled || width <= 1 || height <= 1)
                return GridMax(spl, 0, 0, width, height, width);
            int dataWidth = width - 1;
            int dataHeight = height - 1;
            float diameter = 0.5f * Math.Min(dataWidth, dataHeight);
            int x = Convert.ToInt32(((dataWidth - diameter) / 2f).ToString("N0"));
            int y = Convert.ToInt32(((dataHeight - diameter) / 2f).ToString("N0"));
            int ellipseWidth = Convert.ToInt32(diameter.ToString("n0"));
            int ellipseHeight = Convert.ToInt32(diameter.ToString("n0"));
            if (ellipseWidth <= 0 || ellipseHeight <= 0)
                return GridMax(spl, 0, 0, width, height, width);
            int centreX = x + (ellipseWidth + 1) / 2;
            int centreY = y + (ellipseHeight + 1) / 2;
            int longRadius = ellipseWidth > ellipseHeight ? (ellipseWidth + 1) / 2 : (ellipseHeight + 1) / 2;
            int shortRadius = ellipseWidth > ellipseHeight ? (ellipseHeight + 1) / 2 : (ellipseWidth + 1) / 2;
            float max = float.NegativeInfinity;
            bool found = false;
            int y1 = Math.Min(height, y + ellipseHeight);
            int x1 = Math.Min(width, x + ellipseWidth);
            for (int row = Math.Max(0, y); row < y1; row++)
            {
                for (int col = Math.Max(0, x); col < x1; col++)
                {
                    double dx = col - centreX;
                    double dy = row - centreY;
                    if ((dx * dx) / (longRadius * longRadius) + (dy * dy) / (shortRadius * shortRadius) >= 1)
                        continue;
                    float value = spl[row * width + col];
                    if (value > max)
                        max = value;
                    found = true;
                }
            }
            return found ? max : 0f;
        }

        private static float GridMax(float[] spl, int x, int y, int width, int height, int stride)
        {
            float max = float.NegativeInfinity;
            for (int row = y; row < y + height; row++)
            {
                int offset = row * stride + x;
                for (int col = 0; col < width; col++)
                {
                    float value = spl[offset + col];
                    if (value > max)
                        max = value;
                }
            }
            return max;
        }

        private void SetAcousticCoverImage(IntPtr h)
        {
            if (pbThermalImage.Image == null) return;
            try
            {
                using (var snapshot = new Bitmap(pbThermalImage.Image))
                using (var cover = EnsureMinSide(snapshot, 240))
                using (var ms = new MemoryStream())
                {
                    cover.Save(ms, ImageFormat.Jpeg);
                    byte[] jpeg = ms.ToArray();
                    GCHandle pin = GCHandle.Alloc(jpeg, GCHandleType.Pinned);
                    try
                    {
                        var buffer = new IRtekACFileSDK.AcFileBuffer
                        {
                            length = (uint)jpeg.Length,
                            buffer = pin.AddrOfPinnedObject()
                        };
                        IRtekACFileSDK.ac_file_set_cover_image(h, ref buffer);
                    }
                    finally
                    {
                        pin.Free();
                    }
                }
            }
            catch { }
        }

        private void ReadAcousticGridSize(out int rawW, out int rawH)
        {
            rawW = _acousticRawW;
            rawH = _acousticRawH;
            if (rawW > 0 && rawH > 0) return;
            var size = new UvcSdkNative.UvcSize();
            if (!UvcSdkNative.IsOk(UvcSdkNative.uvc_get_acoustic_resolution(_uvcHandle, ref size)))
                return;
            rawW = size.width;
            rawH = size.height;
        }

        private static void EnsureAcFile(int ret, string api)
        {
            if (ret != IRtekACFileSDK.AC_FILE_EC_OK)
                throw new InvalidOperationException(api + " failed: " + ret);
        }

        private static void CopyCString(byte[] src, byte[] dst)
        {
            if (dst == null || dst.Length == 0) return;
            Array.Clear(dst, 0, dst.Length);
            if (src == null) return;
            int length = Math.Min(src.Length, dst.Length - 1);
            int end = 0;
            while (end < length && src[end] != 0)
                end++;
            if (end > 0)
                Array.Copy(src, dst, end);
        }

        private static Bitmap EnsureMinSide(Bitmap source, int minSide)
        {
            if (source.Width >= minSide && source.Height >= minSide)
                return (Bitmap)source.Clone();
            float scale = Math.Max(minSide / (float)source.Width, minSide / (float)source.Height);
            int width = Math.Max(minSide, (int)Math.Ceiling(source.Width * scale));
            int height = Math.Max(minSide, (int)Math.Ceiling(source.Height * scale));
            var scaled = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(scaled))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, 0, 0, width, height);
            }
            return scaled;
        }

        private static byte[] EncodeBgrJpeg(byte[] bgr, int width, int height)
        {
            using (var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
            {
                var rect = new Rectangle(0, 0, width, height);
                BitmapData data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                try
                {
                    int sourceStride = width * 3;
                    if (data.Stride == sourceStride)
                    {
                        Marshal.Copy(bgr, 0, data.Scan0, sourceStride * height);
                    }
                    else
                    {
                        for (int y = 0; y < height; y++)
                        {
                            Marshal.Copy(bgr, y * sourceStride,
                                IntPtr.Add(data.Scan0, y * data.Stride), sourceStride);
                        }
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
        }

        private void SaveThermalImageAsIrFile(string filePath, byte[] rawFrame, int width, int height)
        {
            IntPtr h = IntPtr.Zero;
            int ret = IRtekFileSDK.ir_file_create(filePath, IRtekFileSDK.IR_FILE_THERMAL_PICTURE, ref h);
            if (ret != IRtekFileSDK.IR_FILE_EC_OK || h == IntPtr.Zero)
                throw new InvalidOperationException($"ir_file_create failed: {ret}");

            try
            {
                var sz = new IRtekFileSDK.IrFileSize { width = (short)width, height = (short)height };
                ret = IRtekFileSDK.ir_file_set_thermal_size(h, ref sz);
                if (ret != IRtekFileSDK.IR_FILE_EC_OK) throw new InvalidOperationException($"ir_file_set_thermal_size failed: {ret}");

                ret = IRtekFileSDK.ir_file_set_isp_type(h, IRtekFileSDK.IR_FILE_ISP_HOLLY);
                if (ret != IRtekFileSDK.IR_FILE_EC_OK) throw new InvalidOperationException($"ir_file_set_isp_type failed: {ret}");

                UvcSdkNative.UvcColorDistType colorDist;
                ret = UvcSdkNative.uvc_get_thermal_color_dist(_uvcHandle, out colorDist);
                if (!UvcSdkNative.IsOk(ret))
                    throw new InvalidOperationException($"uvc_get_thermal_color_dist failed: {ret}");
                ret = IRtekFileSDK.ir_file_set_color_dist(h, (int)colorDist);
                if (ret != IRtekFileSDK.IR_FILE_EC_OK) throw new InvalidOperationException($"ir_file_set_color_dist failed: {ret}");

                SetFileLut(h);
                SetFileTempRange(h);
                SetFileCameraInfo(h);
                SetFileLensInfo(h);
                SetFileThermalParams(h);

                var devicePalette = UvcSdkNative.UvcPalette.Create();
                ret = UvcSdkNative.uvc_get_thermal_palette(_uvcHandle, ref devicePalette);
                if (!UvcSdkNative.IsOk(ret))
                    throw new InvalidOperationException($"uvc_get_thermal_palette failed: {ret}");
                var palette = IRtekFileSDK.IrFilePalette.Create();
                palette.refNo = devicePalette.ref_no;
                palette.inverted = devicePalette.inverted;
                IRtekFileSDK.ir_file_set_palette(h, ref palette);

                ThermalAdLut lut;
                string lutError;
                if (!ThermalAdLut.TryLoad(_uvcHandle, out lut, out lutError))
                    throw new InvalidOperationException(lutError);

                var deviceAlarm = new UvcSdkNative.UvcAlarm();
                ret = UvcSdkNative.uvc_get_thermal_alarm(_uvcHandle, ref deviceAlarm);
                if (!UvcSdkNative.IsOk(ret))
                    throw new InvalidOperationException($"uvc_get_thermal_alarm failed: {ret}");
                var alarm = new IRtekFileSDK.IrFileAlarm
                {
                    type = (int)deviceAlarm.type,
                    custom = deviceAlarm.custom,
                    color = deviceAlarm.color,
                    max = lut.ToTemp(deviceAlarm.max),
                    min = lut.ToTemp(deviceAlarm.min),
                    rhLimit = 1.0f,
                    indoorTemp = 293.15f,
                    outdoorTemp = 283.15f,
                    heatIndex = 0.8f
                };
                IRtekFileSDK.ir_file_set_alarm(h, ref alarm);

                var deviceGain = new UvcSdkNative.UvcThermalGain();
                ret = UvcSdkNative.uvc_get_thermal_gain(_uvcHandle, ref deviceGain);
                if (!UvcSdkNative.IsOk(ret))
                    throw new InvalidOperationException($"uvc_get_thermal_gain failed: {ret}");
                var gain = new IRtekFileSDK.IrFileGain
                {
                    mode = (int)deviceGain.mode,
                    min = lut.ToTemp(deviceGain.min),
                    max = lut.ToTemp(deviceGain.max),
                    fixedSpan = deviceGain.fixed_span,
                    span = lut.SpanToTemp(deviceGain.min, deviceGain.span)
                };
                IRtekFileSDK.ir_file_set_gain(h, ref gain);

                DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan tsSpan = DateTime.Now.ToUniversalTime() - dtStart;
                ulong lSpan = (ulong)tsSpan.Ticks / TimeSpan.TicksPerMillisecond;

                int pixelCount = width * height;
                ushort[] pixels = new ushort[pixelCount];
                Buffer.BlockCopy(rawFrame, 0, pixels, 0, pixelCount * 2);

                GCHandle pin = GCHandle.Alloc(pixels, GCHandleType.Pinned);
                try
                {
                    var frame = new IRtekFileSDK.IrFileFrame
                    {
                        timeStamp = (ulong)lSpan,
                        length = (uint)pixelCount,
                        buffer = pin.AddrOfPinnedObject()
                    };
                    ret = IRtekFileSDK.ir_file_append_frame(h, ref frame);
                    if (ret != IRtekFileSDK.IR_FILE_EC_OK)
                        throw new InvalidOperationException($"ir_file_append_frame failed: {ret}");
                }
                finally
                {
                    pin.Free();
                }

                // Save a cover image (current visible bitmap as JPEG)
                if (pbThermalImage.Image != null)
                {
                    try
                    {
                        using (var ms2 = new MemoryStream())
                        {
                            pbThermalImage.Image.Save(ms2, ImageFormat.Jpeg);
                            byte[] jpegBytes = ms2.ToArray();
                            GCHandle coverPin = GCHandle.Alloc(jpegBytes, GCHandleType.Pinned);
                            try
                            {
                                var cover = new IRtekFileSDK.IrFileBuffer
                                {
                                    length = (uint)jpegBytes.Length,
                                    buffer = coverPin.AddrOfPinnedObject()
                                };
                                IRtekFileSDK.ir_file_set_cover_image(h, ref cover);
                            }
                            finally
                            {
                                coverPin.Free();
                            }
                        }
                    }
                    catch { }
                }

                ret = IRtekFileSDK.ir_file_save(h);
                if (ret != IRtekFileSDK.IR_FILE_EC_OK)
                    throw new InvalidOperationException($"ir_file_save failed: {ret}");
            }
            finally
            {
                IRtekFileSDK.ir_file_close(h);
            }
        }

        private void SetFileLut(IntPtr fileHandle)
        {
            ushort[] ads;
            float[] temps;
            lock (_frameLock)
            {
                ads = _lastLutAd;
                temps = _lastLutTemp;
            }

            if (ads == null || temps == null || ads.Length == 0) return;

            int count = ads.Length;
            IntPtr adPtr = Marshal.AllocHGlobal(count * sizeof(ushort));
            IntPtr tPtr = Marshal.AllocHGlobal(count * sizeof(float));
            try
            {
                for (int i = 0; i < count; i++)
                    Marshal.WriteInt16(adPtr, i * 2, (short)ads[i]);
                Marshal.Copy(temps, 0, tPtr, count);

                var fileLut = new IRtekFileSDK.IrFileLut
                {
                    radMethod = 0x01,
                    count = (uint)count,
                    adArray = adPtr,
                    tempArray = tPtr
                };
                IRtekFileSDK.ir_file_set_lut(fileHandle, ref fileLut);
            }
            finally
            {
                Marshal.FreeHGlobal(adPtr);
                Marshal.FreeHGlobal(tPtr);
            }
        }

        private void SetFileTempRange(IntPtr fileHandle)
        {
            ushort rangeId;
            if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_thermal_range(_uvcHandle, out rangeId)))
            {
                var lens = UvcSdkNative.UvcLensInfo.Create();
                if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_lens_info(_uvcHandle, ref lens)))
                {
                    for (int i = 0; i < lens.ranges_count; i++)
                    {
                        if (lens.range_map[i].id == rangeId)
                        {
                            var tr = new IRtekFileSDK.IrFileTempRange
                            {
                                min = lens.range_map[i].range.min,
                                max = lens.range_map[i].range.max
                            };
                            IRtekFileSDK.ir_file_set_temp_range(fileHandle, ref tr);
                            break;
                        }
                    }
                }
            }
        }

        private void SetFileLensInfo(IntPtr fileHandle)
        {
            var lensInfo = UvcSdkNative.UvcLensInfo.Create();
            if (!UvcSdkNative.IsOk(UvcSdkNative.uvc_get_lens_info(_uvcHandle, ref lensInfo)))
                return;

            var lens = IRtekFileSDK.IrFileLensInfoV2.Create();
            CopyBytes(lensInfo.name, lens.name);
            CopyBytes(lensInfo.band, lens.band);
            lens.focalLength = lensInfo.focal;
            lens.hfov = lensInfo.hfov;
            IRtekFileSDK.ir_file_set_lens_info_v2(fileHandle, ref lens);
        }

        private void SetFileCameraInfo(IntPtr fileHandle)
        {
            var devInfo = UvcSdkNative.UvcDeviceInfo.Create();
            if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_device_info(_uvcHandle, ref devInfo)))
            {
                var cam = IRtekFileSDK.IrFileCameraInfo.Create();
                CopyBytes(devInfo.model, cam.model);
                CopyBytes(devInfo.serial_no, cam.serialNo);
                CopyBytes(devInfo.brand, cam.brand);
                CopyBytes(devInfo.company, cam.company);
                cam.logoLength = 0;
                cam.logo = IntPtr.Zero;
                IRtekFileSDK.ir_file_set_camera_info(fileHandle, ref cam);
            }
        }

        private void SetFileThermalParams(IntPtr fileHandle)
        {
            var tp = new UvcSdkNative.UvcThermalParams();
            if (UvcSdkNative.IsOk(UvcSdkNative.uvc_get_thermal_params(_uvcHandle, ref tp)))
            {
                var ftp = new IRtekFileSDK.IrFileThermalParams
                {
                    emissivity = tp.emissivity,
                    distance = tp.distance,
                    reflTemp = tp.refl_temp,
                    atmTemp = tp.atm_temp,
                    relHumidity = tp.rel_humidity,
                    extOptTemp = 293.15f,
                    extOptTrans = 1.0f
                };
                IRtekFileSDK.ir_file_set_thermal_params(fileHandle, ref ftp);
            }
        }

        private static void CopyBytes(byte[] src, byte[] dst)
        {
            if (src == null || dst == null) return;
            int len = Math.Min(src.Length, dst.Length);
            Array.Copy(src, dst, len);
        }

        private void tsbSelect_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.None;
            tsbSelect.Checked = true;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbLine.Checked = false;
            tsbDelete.Checked = false;
            Adding = false;
            Deleting = false;
            Cursor = Cursors.Default;
        }

        private void tsbSpot_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Spot;
            tsbSelect.Checked = false;
            tsbSpot.Checked = true;
            tsbRectangle.Checked = false;
            tsbLine.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbRectangle_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Rectangle;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = true;
            tsbLine.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbLine_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Line;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbLine.Checked = true;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Line;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbLine.Checked = false;
            tsbDelete.Checked = true;
            Adding = false;
            Deleting = true;
        }

        private void tscbPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isGuiRefreshing || !_connected) return;
            if (tscbPalette.SelectedItem == null) return;

            var item = (PresetPaletteItem)tscbPalette.SelectedItem;
            _paletteRefNo = item.RefNo;
            ApplyPaletteSelection();
        }

        private void tsbInverted_Click(object sender, EventArgs e)
        {
            if (_isGuiRefreshing || !_connected) return;

            _paletteInverted = (byte)(_paletteInverted == 0 ? 1 : 0);
            tsbInverted.Checked = _paletteInverted != 0;
            ApplyPaletteSelection();
        }

        private void nudBlend_ValueChanged(object sender, EventArgs e)
        {
            if (_isGuiRefreshing || !_connected || _deviceMode != DeviceMode.Acoustic) return;
            _paletteAlpha = (byte)_nudBlend.Value;
            SetAcousticPalette();
        }

        #endregion

        #region Marker Drawing

        private Point ScreenToImage(Point screenPt)
        {
            Rectangle clientRect = pbThermalImage.ClientRectangle;
            if (clientRect.Width == 0 || clientRect.Height == 0) return screenPt;

            double imgAspect = (double)DisplayWidth / DisplayHeight;
            double ctrlAspect = (double)clientRect.Width / clientRect.Height;

            double displayW, displayH, offsetX, offsetY;
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

            int x = (int)((screenPt.X - offsetX) * DisplayWidth / displayW + 0.5);
            int y = (int)((screenPt.Y - offsetY) * DisplayHeight / displayH + 0.5);
            return ClipPoint(new Point(x, y));
        }

        private Bitmap PresentThermalView(Bitmap bmp)
        {
            _previewFrameW = bmp.Width;
            _previewFrameH = bmp.Height;
            return bmp;
        }

        private Point ClipPoint(Point pt)
        {
            return new Point(
                Math.Max(0, Math.Min(DisplayWidth - 1, pt.X)),
                Math.Max(0, Math.Min(DisplayHeight - 1, pt.Y)));
        }

        private void pbThermalImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_connected) return;

            Point point = ScreenToImage(e.Location);

            if (Deleting)
            {
                lock (_markerLock)
                {
                    int idx = FindMarkerAt(point);
                    if (idx >= 0)
                    {
                        string removedName = _markers[idx].Name;
                        for (int i = idx; i < _markerCount - 1; i++)
                            _markers[i] = _markers[i + 1];
                        _markerCount--;
                        lock (_renderLock)
                        {
                            _renderer?.RemoveMarker(removedName);
                        }
                    }
                }
                RefreshMarkerListView();

                CurrentMarkerType = MarkerType.None;
                Deleting = false;
                tsbSelect.Checked = true;
                tsbSpot.Checked = false;
                tsbRectangle.Checked = false;
                tsbLine.Checked = false;
                tsbDelete.Checked = false;
                return;
            }

            if (Adding)
            {
                if (_markerCount >= MaxMarkers)
                {
                    MessageBox.Show($"Maximum markers reached ({MaxMarkers}).", "Warning");
                    Adding = false;
                    return;
                }

                LocalMarker newMarker;
                switch (CurrentMarkerType)
                {
                    case MarkerType.Spot:
                        newMarker = new LocalMarker
                        {
                            Type = MarkerType.Spot,
                            Name = NextSpotName(),
                            Points = new[] { point }
                        };
                        break;
                    case MarkerType.Rectangle:
                        {
                            Point p2 = ClipPoint(new Point(point.X + 30, point.Y + 30));
                            newMarker = new LocalMarker
                            {
                                Type = MarkerType.Rectangle,
                                Name = NextAreaName(),
                                Points = new[] { point, p2 }
                            };
                            break;
                        }
                    case MarkerType.Line:
                        {
                            Point p2 = ClipPoint(new Point(point.X + 40, point.Y + 40));
                            newMarker = new LocalMarker
                            {
                                Type = MarkerType.Line,
                                Name = NextLineName(),
                                Points = new[] { point, p2 }
                            };
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

                int fileMarkerType = MarkerTypeToFileType(newMarker.Type);
                lock (_renderLock)
                {
                    _renderer?.SetMarker(newMarker.Name, fileMarkerType, newMarker.Points);
                }
                RefreshMarkerListView();

                CurrentMarkerType = MarkerType.None;
                Adding = false;
                tsbSelect.Checked = true;
                tsbSpot.Checked = false;
                tsbRectangle.Checked = false;
                tsbLine.Checked = false;
                tsbDelete.Checked = false;
                return;
            }

            // Select mode: start tracking if we hit a marker
            string hitName = string.Empty;
            MarkerType hitMarkerType = MarkerType.None;
            TrackerHitType hitType = TrackerHitType.hitNothing;

            if (HitTest(point, ref hitName, ref hitMarkerType, ref hitType))
            {
                _tracker.IsTracking = true;
                _tracker.Name = hitName;
                _tracker.MarkerType = hitMarkerType;
                _tracker.HitType = hitType;
                _tracker.PrevPos = point;
            }
        }

        private void pbThermalImage_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateCursorTemperature(e.Location, true);
            if (!_connected) return;

            if (Deleting || Adding)
            {
                Cursor = Cursors.Cross;
                return;
            }

            Point point = ScreenToImage(e.Location);

            if (_tracker.IsTracking)
            {
                TrackSymbol(point);
                return;
            }

            string name = string.Empty;
            MarkerType mt = MarkerType.None;
            TrackerHitType ht = TrackerHitType.hitNothing;
            if (HitTest(point, ref name, ref mt, ref ht))
            {
                switch (ht)
                {
                    case TrackerHitType.hitTopLeft:
                    case TrackerHitType.hitBottomRight:
                        Cursor = Cursors.SizeNWSE; break;
                    case TrackerHitType.hitTopRight:
                    case TrackerHitType.hitBottomLeft:
                        Cursor = Cursors.SizeNESW; break;
                    case TrackerHitType.hitMiddle:
                    case TrackerHitType.hitEdge:
                    case TrackerHitType.hitStart:
                    case TrackerHitType.hitEnd:
                        Cursor = Cursors.SizeAll; break;
                    default:
                        Cursor = Cursors.Default; break;
                }
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        private void pbThermalImage_MouseUp(object sender, MouseEventArgs e)
        {
            _tracker.Reset();
        }

        private void pbThermalImage_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _tracker.Reset();
            HideCursorTemperature(true);
        }

        private void UpdateCursorTemperature(Point clientPt, bool invalidate)
        {
            if (!_connected || _isDisposing ||
                (_deviceMode != DeviceMode.Thermal && _deviceMode != DeviceMode.Acoustic))
            {
                HideCursorTemperature(invalidate);
                return;
            }

            float u, v;
            string text = null;
            bool located = TryGetVideoNormalizedPoint(clientPt, out u, out v);
            bool formatted = located && (_deviceMode == DeviceMode.Acoustic
                ? TryFormatAcousticDb(u, v, out text)
                : TryFormatGridTemperature(u, v, out text));
            if (!formatted)
            {
                HideCursorTemperature(invalidate);
                return;
            }

            bool changed = !_cursorOverVideo || _cursorClient != clientPt || _cursorTempText != text;
            _cursorOverVideo = true;
            _cursorClient = clientPt;
            _cursorTempText = text;
            if (invalidate && changed && !pbThermalImage.IsDisposed)
                pbThermalImage.Invalidate();
        }

        private void HideCursorTemperature(bool invalidate)
        {
            if (!_cursorOverVideo && _cursorTempText == null)
                return;
            _cursorOverVideo = false;
            _cursorTempText = null;
            if (invalidate && !_isDisposing && !pbThermalImage.IsDisposed)
                pbThermalImage.Invalidate();
        }

        private bool TryGetVideoNormalizedPoint(Point clientPt, out float u, out float v)
        {
            u = 0f;
            v = 0f;
            Image image = pbThermalImage.Image;
            if (image == null || image.Width <= 0 || image.Height <= 0)
                return false;

            Rectangle client = pbThermalImage.ClientRectangle;
            if (client.Width <= 0 || client.Height <= 0)
                return false;

            float ratio = Math.Min(
                (float)client.Width / image.Width,
                (float)client.Height / image.Height);
            int displayW = (int)(image.Width * ratio);
            int displayH = (int)(image.Height * ratio);
            if (displayW <= 0 || displayH <= 0)
                return false;

            int offsetX = (client.Width - displayW) / 2;
            int offsetY = (client.Height - displayH) / 2;
            int x = clientPt.X - offsetX;
            int y = clientPt.Y - offsetY;
            if (x < 0 || y < 0 || x >= displayW || y >= displayH)
                return false;

            u = (float)x / displayW;
            v = (float)y / displayH;
            return true;
        }

        private bool TryFormatGridTemperature(float u, float v, out string text)
        {
            text = null;
            ushort ad;
            ushort[] ads;
            float[] temps;
            lock (_frameLock)
            {
                int w = _thermalGridW;
                int h = _thermalGridH;
                if (_thermalGrid == null || w <= 0 || h <= 0 || _lastLutAd == null || _lastLutTemp == null)
                    return false;

                int x = (int)(u * w);
                int y = (int)(v * h);
                if (x < 0) x = 0;
                else if (x >= w) x = w - 1;
                if (y < 0) y = 0;
                else if (y >= h) y = h - 1;
                // The preview bitmap is flipped for display; the AD grid is not.
                if (_flipMode == 2 || _flipMode == 3)
                    x = w - 1 - x;
                if (_flipMode == 1 || _flipMode == 3)
                    y = h - 1 - y;

                int offset = (y * w + x) * 2;
                if (offset < 0 || offset + 1 >= _thermalGrid.Length)
                    return false;
                ad = BitConverter.ToUInt16(_thermalGrid, offset);
                ads = _lastLutAd;
                temps = _lastLutTemp;
            }

            if (_cursorLut == null || !ReferenceEquals(_cursorLutAd, ads) || !ReferenceEquals(_cursorLutTemp, temps))
            {
                _cursorLut = ThermalAdLut.FromSamples(ads, temps);
                _cursorLutAd = ads;
                _cursorLutTemp = temps;
            }
            if (_cursorLut == null)
                return false;

            float kelvin = _cursorLut.ToTemp(ad);
            if (float.IsNaN(kelvin) || float.IsInfinity(kelvin))
                return false;
            text = FormatMarkerTempCelsius(kelvin);
            return true;
        }

        private bool TryFormatAcousticDb(float u, float v, out string text)
        {
            text = null;
            int rawW = _acousticRawW;
            int rawH = _acousticRawH;
            if (rawW <= 0 || rawH <= 0)
                return false;

            byte[] frame;
            int width;
            int height;
            lock (_frameLock)
            {
                frame = _lastAcousticFrame;
                width = _lastAcousticFrameW;
                height = _lastAcousticFrameH;
            }
            if (frame == null || width <= 0 || height <= 0)
                return false;

            int x = (int)(u * rawW);
            int y = (int)(v * rawH);
            if (x < 0) x = 0;
            else if (x >= rawW) x = rawW - 1;
            if (y < 0) y = 0;
            else if (y >= rawH) y = rawH - 1;
            if (_flipMode == 2 || _flipMode == 3)
                x = rawW - 1 - x;
            if (_flipMode == 1 || _flipMode == 3)
                y = rawH - 1 - y;

            long offset = (long)width * height * 3 + ((long)y * rawW + x) * 2;
            if (offset < 0 || offset + 1 >= frame.Length)
                return false;
            int index = (int)offset;
            int sample = frame[index] | (frame[index + 1] << 8);
            float db = AcousticSampleToDb(sample);
            if (float.IsNaN(db) || float.IsInfinity(db))
                return false;
            text = string.Format("{0:F1} dB", db);
            return true;
        }

        private static Bitmap LoadAcousticPointMarker()
        {
            using (Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("IRtekUVCSDKDemo.SPLPointImage.png"))
            {
                if (stream == null)
                    return null;
                using (var loaded = new Bitmap(stream))
                    return new Bitmap(loaded);
            }
        }

        private void DrawAcousticExtremePoints(Graphics graphics)
        {
            if (_deviceMode != DeviceMode.Acoustic || _acousticExtremeCount <= 0)
                return;

            Image image = pbThermalImage.Image;
            if (image == null || image.Width <= 0 || image.Height <= 0)
                return;

            int count = _acousticExtremeCount;
            for (int i = count - 1; i >= 0; i--)
            {
                AcousticSourcePoint source = _acousticExtremes[i];
                int x = source.X;
                int y = source.Y;
                if (_flipMode == 2 || _flipMode == 3)
                    x = image.Width - 1 - x;
                if (_flipMode == 1 || _flipMode == 3)
                    y = image.Height - 1 - y;
                if (x < 0 || y < 0 || x >= image.Width || y >= image.Height)
                    continue;

                Point client;
                if (!TryMapImagePixelToClient(image, x, y, out client))
                    continue;

                Bitmap marker = _acousticPointMarker;
                float labelTop = client.Y;
                if (marker != null)
                {
                    float left = client.X - AcousticPointMarkerSize / 2f;
                    float top = client.Y - AcousticPointMarkerSize / 2f;
                    InterpolationMode previous = graphics.InterpolationMode;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(marker, left, top, AcousticPointMarkerSize, AcousticPointMarkerSize);
                    graphics.InterpolationMode = previous;
                    labelTop = top;
                }

                if (float.IsNaN(source.Value) || float.IsInfinity(source.Value))
                    continue;
                DrawAcousticPointLabel(graphics, client.X, labelTop, string.Format("{0:F1} dB", source.Value));
            }
        }

        private bool TryMapImagePixelToClient(Image image, int pixelX, int pixelY, out Point client)
        {
            client = Point.Empty;
            Rectangle bounds = pbThermalImage.ClientRectangle;
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return false;

            float ratio = Math.Min(
                (float)bounds.Width / image.Width,
                (float)bounds.Height / image.Height);
            int displayW = (int)(image.Width * ratio);
            int displayH = (int)(image.Height * ratio);
            if (displayW <= 0 || displayH <= 0)
                return false;

            int offsetX = (bounds.Width - displayW) / 2;
            int offsetY = (bounds.Height - displayH) / 2;
            client = new Point(
                offsetX + (int)((pixelX + 0.5f) * displayW / image.Width),
                offsetY + (int)((pixelY + 0.5f) * displayH / image.Height));
            return true;
        }

        private void DrawAcousticPointLabel(Graphics graphics, float centerX, float markerTop, string text)
        {
            Size size = TextRenderer.MeasureText(text, _cursorTempFont);
            int x = (int)(centerX - size.Width / 2f);
            int y = (int)(markerTop - size.Height);
            Rectangle client = pbThermalImage.ClientRectangle;
            if (y < 0)
            {
                int markerHeight = _acousticPointMarker != null ? (int)AcousticPointMarkerSize : 0;
                y = (int)markerTop + markerHeight;
            }
            if (x < 0)
                x = 0;
            else if (x + size.Width > client.Width)
                x = Math.Max(0, client.Width - size.Width);

            TextRenderer.DrawText(graphics, text, _cursorTempFont, new Point(x, y), Color.White);
        }

        private void pbThermalImage_Paint(object sender, PaintEventArgs e)
        {
            DrawAcousticExtremePoints(e.Graphics);
            if (string.IsNullOrEmpty(_cursorTempText))
                return;

            const int gap = 16;
            Size size = TextRenderer.MeasureText(_cursorTempText, _cursorTempFont);
            int x = _cursorClient.X + gap;
            int y = _cursorClient.Y + gap;
            Rectangle client = pbThermalImage.ClientRectangle;
            if (x + size.Width > client.Right - 2)
                x = _cursorClient.X - gap - size.Width;
            if (y + size.Height > client.Bottom - 2)
                y = _cursorClient.Y - gap - size.Height;
            if (x < 2) x = 2;
            if (y < 2) y = 2;

            TextRenderer.DrawText(e.Graphics, _cursorTempText, _cursorTempFont, new Point(x, y), Color.White);
        }

        private bool HitTest(Point pt, ref string name, ref MarkerType markerType, ref TrackerHitType hitType)
        {
            lock (_markerLock)
            {
                for (int i = _markerCount - 1; i >= 0; i--)
                {
                    var m = _markers[i];
                    if (m.Points == null) continue;

                    switch (m.Type)
                    {
                        case MarkerType.Spot:
                            if (Math.Abs(pt.X - m.Points[0].X) <= 5 && Math.Abs(pt.Y - m.Points[0].Y) <= 5)
                            {
                                name = m.Name; markerType = MarkerType.Spot;
                                hitType = TrackerHitType.hitMiddle; return true;
                            }
                            break;

                        case MarkerType.Rectangle:
                            if (m.Points.Length >= 2)
                            {
                                int x0 = Math.Min(m.Points[0].X, m.Points[1].X);
                                int y0 = Math.Min(m.Points[0].Y, m.Points[1].Y);
                                int x1 = Math.Max(m.Points[0].X, m.Points[1].X);
                                int y1 = Math.Max(m.Points[0].Y, m.Points[1].Y);

                                if (Math.Abs(pt.X - x0) <= 5 && Math.Abs(pt.Y - y0) <= 5)
                                { name = m.Name; markerType = MarkerType.Rectangle; hitType = TrackerHitType.hitTopLeft; return true; }
                                if (Math.Abs(pt.X - x1) <= 5 && Math.Abs(pt.Y - y0) <= 5)
                                { name = m.Name; markerType = MarkerType.Rectangle; hitType = TrackerHitType.hitTopRight; return true; }
                                if (Math.Abs(pt.X - x1) <= 5 && Math.Abs(pt.Y - y1) <= 5)
                                { name = m.Name; markerType = MarkerType.Rectangle; hitType = TrackerHitType.hitBottomRight; return true; }
                                if (Math.Abs(pt.X - x0) <= 5 && Math.Abs(pt.Y - y1) <= 5)
                                { name = m.Name; markerType = MarkerType.Rectangle; hitType = TrackerHitType.hitBottomLeft; return true; }

                                if (pt.X >= x0 && pt.X <= x1 && pt.Y >= y0 && pt.Y <= y1)
                                { name = m.Name; markerType = MarkerType.Rectangle; hitType = TrackerHitType.hitMiddle; return true; }
                            }
                            break;

                        case MarkerType.Line:
                            if (m.Points.Length >= 2)
                            {
                                if (Math.Abs(pt.X - m.Points[0].X) <= 5 && Math.Abs(pt.Y - m.Points[0].Y) <= 5)
                                { name = m.Name; markerType = MarkerType.Line; hitType = TrackerHitType.hitStart; return true; }
                                if (Math.Abs(pt.X - m.Points[1].X) <= 5 && Math.Abs(pt.Y - m.Points[1].Y) <= 5)
                                { name = m.Name; markerType = MarkerType.Line; hitType = TrackerHitType.hitEnd; return true; }

                                if (DistToSegment(pt, m.Points[0], m.Points[1]) <= 5)
                                { name = m.Name; markerType = MarkerType.Line; hitType = TrackerHitType.hitEdge; return true; }
                            }
                            break;
                    }
                }
            }
            return false;
        }

        private static double DistToSegment(Point p, Point a, Point b)
        {
            double dx = b.X - a.X, dy = b.Y - a.Y;
            double lenSq = dx * dx + dy * dy;
            if (lenSq < 1e-6) return Math.Sqrt((p.X - a.X) * (p.X - a.X) + (p.Y - a.Y) * (p.Y - a.Y));
            double t = Math.Max(0, Math.Min(1, ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lenSq));
            double projX = a.X + t * dx, projY = a.Y + t * dy;
            return Math.Sqrt((p.X - projX) * (p.X - projX) + (p.Y - projY) * (p.Y - projY));
        }

        private void TrackSymbol(Point point)
        {
            point = ClipPoint(point);
            int dx = point.X - _tracker.PrevPos.X;
            int dy = point.Y - _tracker.PrevPos.Y;
            if (dx == 0 && dy == 0) return;

            lock (_markerLock)
            {
                int idx = FindMarkerByName(_tracker.Name);
                if (idx < 0) { _tracker.Reset(); return; }

                var m = _markers[idx];

                switch (m.Type)
                {
                    case MarkerType.Spot:
                        m.Points[0] = point;
                        break;

                    case MarkerType.Rectangle:
                        {
                            int x0 = Math.Min(m.Points[0].X, m.Points[1].X);
                            int y0 = Math.Min(m.Points[0].Y, m.Points[1].Y);
                            int x1 = Math.Max(m.Points[0].X, m.Points[1].X);
                            int y1 = Math.Max(m.Points[0].Y, m.Points[1].Y);

                            switch (_tracker.HitType)
                            {
                                case TrackerHitType.hitTopLeft:
                                    x0 = point.X; y0 = point.Y; break;
                                case TrackerHitType.hitTopRight:
                                    x1 = point.X; y0 = point.Y; break;
                                case TrackerHitType.hitBottomRight:
                                    x1 = point.X; y1 = point.Y; break;
                                case TrackerHitType.hitBottomLeft:
                                    x0 = point.X; y1 = point.Y; break;
                                case TrackerHitType.hitMiddle:
                                    int w = x1 - x0, h = y1 - y0;
                                    x0 = Math.Max(0, Math.Min(DisplayWidth - 1 - w, x0 + dx));
                                    y0 = Math.Max(0, Math.Min(DisplayHeight - 1 - h, y0 + dy));
                                    x1 = x0 + w; y1 = y0 + h;
                                    break;
                            }
                            m.Points[0] = new Point(Math.Min(x0, x1), Math.Min(y0, y1));
                            m.Points[1] = new Point(Math.Max(x0, x1), Math.Max(y0, y1));
                        }
                        break;

                    case MarkerType.Line:
                        switch (_tracker.HitType)
                        {
                            case TrackerHitType.hitStart:
                                m.Points[0] = point; break;
                            case TrackerHitType.hitEnd:
                                m.Points[1] = point; break;
                            case TrackerHitType.hitEdge:
                                m.Points[0] = ClipPoint(new Point(m.Points[0].X + dx, m.Points[0].Y + dy));
                                m.Points[1] = ClipPoint(new Point(m.Points[1].X + dx, m.Points[1].Y + dy));
                                break;
                        }
                        break;
                }

                _markers[idx] = m;

                int fileType = MarkerTypeToFileType(m.Type);
                lock (_renderLock)
                {
                    _renderer?.SetMarker(m.Name, fileType, m.Points);
                }
            }

            _tracker.PrevPos = point;
        }

        private int FindMarkerByName(string name)
        {
            for (int i = 0; i < _markerCount; i++)
                if (_markers[i].Name == name) return i;
            return -1;
        }

        private int FindMarkerAt(Point pt)
        {
            for (int i = _markerCount - 1; i >= 0; i--)
            {
                var m = _markers[i];
                switch (m.Type)
                {
                    case MarkerType.Spot:
                        if (Math.Abs(pt.X - m.Points[0].X) <= 8 && Math.Abs(pt.Y - m.Points[0].Y) <= 8)
                            return i;
                        break;
                    case MarkerType.Rectangle:
                        if (m.Points.Length >= 2)
                        {
                            int x0 = Math.Min(m.Points[0].X, m.Points[1].X);
                            int x1 = Math.Max(m.Points[0].X, m.Points[1].X);
                            int y0 = Math.Min(m.Points[0].Y, m.Points[1].Y);
                            int y1 = Math.Max(m.Points[0].Y, m.Points[1].Y);
                            if (pt.X >= x0 && pt.X <= x1 && pt.Y >= y0 && pt.Y <= y1)
                                return i;
                        }
                        break;
                    case MarkerType.Line:
                        if (m.Points.Length >= 2)
                        {
                            if (DistToSegment(pt, m.Points[0], m.Points[1]) <= 8)
                                return i;
                        }
                        break;
                }
            }
            return -1;
        }

        private string NextSpotName() { return "Sp" + (++_spotCounter); }
        private string NextAreaName() { return "Ar" + (++_areaCounter); }
        private string NextLineName() { return "Li" + (++_lineCounter); }

        private static int MarkerTypeToFileType(MarkerType t)
        {
            switch (t)
            {
                case MarkerType.Spot: return IRtekFileSDK.IR_FILE_SPOT;
                case MarkerType.Rectangle: return IRtekFileSDK.IR_FILE_RECTANGLE;
                case MarkerType.Line: return IRtekFileSDK.IR_FILE_POLYLINE;
                default: return IRtekFileSDK.IR_FILE_SPOT;
            }
        }

        private void DrawMarkerOverlay(Bitmap bmp)
        {
            LocalMarker[] snapshot;
            int count;
            lock (_markerLock)
            {
                count = _markerCount;
                if (count == 0) return;
                snapshot = new LocalMarker[count];
                Array.Copy(_markers, snapshot, count);
            }

            using (var g = Graphics.FromImage(bmp))
            using (var pen = new Pen(Color.White, 1f))
            using (var brush = new SolidBrush(Color.White))
            {
                if (bmp.Width != DisplayWidth || bmp.Height != DisplayHeight)
                {
                    g.ScaleTransform(
                        bmp.Width / (float)DisplayWidth,
                        bmp.Height / (float)DisplayHeight);
                }

                for (int i = 0; i < count; i++)
                {
                    var m = snapshot[i];
                    if (m.Points == null || m.Points.Length == 0) continue;

                    float tempMax = 0, tempMin = 0, tempAvg = 0;
                    lock (_renderLock)
                    {
                        if (_renderer != null)
                            _renderer.GetMarkerTemps(m.Name, out tempMax, out tempMin, out tempAvg);
                    }

                    switch (m.Type)
                    {
                        case MarkerType.Spot:
                            DrawSpotMarker(g, pen, Font, brush, m, tempMax);
                            break;
                        case MarkerType.Rectangle:
                            DrawRectMarker(g, pen, Font, brush, m, tempMax, tempMin, tempAvg);
                            break;
                        case MarkerType.Line:
                            DrawLineMarker(g, pen, Font, brush, m, tempMax, tempMin, tempAvg);
                            break;
                    }
                }
            }
        }

        private static void DrawSpotMarker(Graphics g, Pen pen, Font font, Brush brush, LocalMarker m, float temp)
        {
            Point p = m.Points[0];
            int r = 4;
            int dist = 17;

            g.DrawEllipse(pen, p.X - r / 2, p.Y - r / 2, r, r);
            g.DrawLine(pen, p.X, p.Y - dist, p.X, p.Y - r);
            g.DrawLine(pen, p.X, p.Y + r, p.X, p.Y + dist);
            g.DrawLine(pen, p.X - dist, p.Y, p.X - r, p.Y);
            g.DrawLine(pen, p.X + r, p.Y, p.X + dist, p.Y);

            string label = m.Name;
            if (temp != 0) label = $"{m.Name} {FormatMarkerTempCelsius(temp)}";
            g.DrawString(label, font, brush, p.X + dist / 2, p.Y - dist);
        }

        private static Rectangle PadHitArea(Point pt)
        {
            return new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
        }

        private static void DrawRectSelectionHandles(Graphics g, Pen pen, Rectangle rect)
        {
            Rectangle sel;
            // Top-left
            sel = PadHitArea(new Point(rect.X, rect.Y));
            g.DrawLine(pen, sel.Location, new Point(sel.X + sel.Width, sel.Y));
            g.DrawLine(pen, sel.Location, new Point(sel.X, sel.Y + sel.Height));
            // Top-right
            sel = PadHitArea(new Point(rect.Right, rect.Y));
            g.DrawLine(pen, sel.Location, new Point(sel.X + sel.Width, sel.Y));
            g.DrawLine(pen, new Point(sel.X + sel.Width, sel.Y), new Point(sel.X + sel.Width, sel.Y + sel.Height));
            // Bottom-right
            sel = PadHitArea(new Point(rect.Right, rect.Bottom));
            g.DrawLine(pen, new Point(sel.X + sel.Width, sel.Y), new Point(sel.X + sel.Width, sel.Y + sel.Height));
            g.DrawLine(pen, new Point(sel.X + sel.Width, sel.Y + sel.Height), new Point(sel.X, sel.Y + sel.Height));
            // Bottom-left
            sel = PadHitArea(new Point(rect.X, rect.Bottom));
            g.DrawLine(pen, sel.Location, new Point(sel.X, sel.Y + sel.Height));
            g.DrawLine(pen, new Point(sel.X, sel.Y + sel.Height), new Point(sel.X + sel.Width, sel.Y + sel.Height));
        }

        private static void DrawRectMarker(Graphics g, Pen pen, Font font, Brush brush, LocalMarker m, float max, float min, float avg)
        {
            if (m.Points.Length < 2) return;
            int x0 = Math.Min(m.Points[0].X, m.Points[1].X);
            int y0 = Math.Min(m.Points[0].Y, m.Points[1].Y);
            int w = Math.Abs(m.Points[1].X - m.Points[0].X);
            int h = Math.Abs(m.Points[1].Y - m.Points[0].Y);
            var rect = new Rectangle(x0, y0, w, h);

            g.DrawRectangle(pen, rect);
            DrawRectSelectionHandles(g, pen, rect);

            string label = m.Name;
            g.DrawString(label, font, brush, x0 + 5, y0 + 5);
        }

        private static void DrawLineMarker(Graphics g, Pen pen, Font font, Brush brush, LocalMarker m, float max, float min, float avg)
        {
            if (m.Points.Length < 2) return;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.DrawLine(pen, m.Points[0], m.Points[1]);

            g.DrawRectangle(pen, PadHitArea(m.Points[0]));
            g.DrawRectangle(pen, PadHitArea(m.Points[1]));

            string label = m.Name;
            g.DrawString(label, font, brush, m.Points[0].X + 5, m.Points[0].Y + 5);
        }

        private void RefreshMarkerListView()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshMarkerListView));
                return;
            }
            mlvMarkers.BeginUpdate();
            mlvMarkers.Items.Clear();
            lock (_markerLock)
            {
                for (int i = 0; i < _markerCount; i++)
                {
                    var m = _markers[i];
                    float tMax = 0, tMin = 0, tAvg = 0;
                    bool hasTemp = false;
                    lock (_renderLock)
                    {
                        if (_renderer != null)
                            hasTemp = _renderer.GetMarkerTemps(m.Name, out tMax, out tMin, out tAvg);
                    }

                    var item = new ListViewItem(m.Name);

                    if (m.Type == MarkerType.Spot)
                    {
                        item.SubItems.Add(hasTemp ? FormatMarkerTempCelsius(tMax) : "---");
                        item.SubItems.Add("---");
                        item.SubItems.Add("---");
                        item.SubItems.Add("---");
                        item.SubItems.Add(m.Points[0].X.ToString());
                        item.SubItems.Add(m.Points[0].Y.ToString());
                        item.SubItems.Add("---");
                        item.SubItems.Add("---");
                    }
                    else if (m.Type == MarkerType.Rectangle && m.Points.Length >= 2)
                    {
                        int x0 = Math.Min(m.Points[0].X, m.Points[1].X);
                        int y0 = Math.Min(m.Points[0].Y, m.Points[1].Y);
                        int x1 = Math.Max(m.Points[0].X, m.Points[1].X);
                        int y1 = Math.Max(m.Points[0].Y, m.Points[1].Y);
                        item.SubItems.Add("---");
                        item.SubItems.Add(hasTemp ? FormatMarkerTempCelsius(tMin) : "---");
                        item.SubItems.Add(hasTemp ? FormatMarkerTempCelsius(tMax) : "---");
                        item.SubItems.Add(hasTemp ? FormatMarkerTempCelsius(tAvg) : "---");
                        item.SubItems.Add(x0.ToString());
                        item.SubItems.Add(y0.ToString());
                        item.SubItems.Add(x1.ToString());
                        item.SubItems.Add(y1.ToString());
                    }
                    else if (m.Type == MarkerType.Line && m.Points.Length >= 2)
                    {
                        item.SubItems.Add("---");
                        item.SubItems.Add(hasTemp ? FormatMarkerTempCelsius(tMin) : "---");
                        item.SubItems.Add(hasTemp ? FormatMarkerTempCelsius(tMax) : "---");
                        item.SubItems.Add(hasTemp ? FormatMarkerTempCelsius(tAvg) : "---");
                        item.SubItems.Add(m.Points[0].X.ToString());
                        item.SubItems.Add(m.Points[0].Y.ToString());
                        item.SubItems.Add(m.Points[1].X.ToString());
                        item.SubItems.Add(m.Points[1].Y.ToString());
                    }

                    item.Tag = i;
                    mlvMarkers.Items.Add(item);
                }
            }
            mlvMarkers.EndUpdate();
        }

        #endregion

        #region Display

        private void LoadDisplayParams(bool showError = false)
        {
            if (!_connected) return;

            _isGuiRefreshing = true;
            tlpDisplay.SuspendLayout();
            try
            {
                var p = new UvcSdkNative.UvcImageParams();
                int ret = DeviceReadCache.GetImageParams(_uvcHandle, ref p);
                if (UvcSdkNative.IsOk(ret))
                {
                    nudContrast.Value = Clamp(p.contrast, nudContrast.Minimum, nudContrast.Maximum);
                    nudBrightness.Value = Clamp(p.brightness, nudBrightness.Minimum, nudBrightness.Maximum);
                    cbDdeEnable.Checked = p.dde_enable != 0;
                    nudDdeValue.Value = Clamp(p.dde_value, nudDdeValue.Minimum, nudDdeValue.Maximum);
                }
                else if (showError)
                {
                    MessageBox.Show($"Get image params failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (_deviceMode == DeviceMode.Thermal)
                {
                    UvcSdkNative.UvcSrMode srMode;
                    int srRet = DeviceReadCache.GetThermalSr(_uvcHandle, out srMode);
                    if (UvcSdkNative.IsOk(srRet))
                        cbSrEnable.Checked = srMode == UvcSdkNative.UvcSrMode.SR;
                    else if (showError)
                        MessageBox.Show($"Get SR mode failed: {srRet}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                UpdateDdeValueVisible();
            }
            finally
            {
                tlpDisplay.ResumeLayout(true);
                _isGuiRefreshing = false;
            }
        }

        private void UpdateDdeValueVisible()
        {
            bool show = cbDdeEnable.Checked;
            lblDdeValue.Visible = show;
            nudDdeValue.Visible = show;
            SetLayoutRowVisible(tlpDisplay, nudDdeValue, show);
        }

        private void UpdateSrVisible()
        {
            bool show = _deviceMode == DeviceMode.Thermal;
            cbSrEnable.Visible = show;
            SetLayoutRowVisible(tlpDisplay, cbSrEnable, show);
        }

        private static void SetLayoutRowVisible(TableLayoutPanel layout, Control control, bool show)
        {
            TableLayoutPanelCellPosition pos = layout.GetPositionFromControl(control);
            if (pos.Row >= 0 && pos.Row < layout.RowStyles.Count)
            {
                layout.RowStyles[pos.Row] = show
                    ? new RowStyle(SizeType.AutoSize)
                    : new RowStyle(SizeType.Absolute, 0f);
            }
        }

        private void btnDisplayGet_Click(object sender, EventArgs e)
        {
            if (!_connected) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                LoadDisplayParams(true);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnDisplaySet_Click(object sender, EventArgs e)
        {
            if (!_connected) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var p = new UvcSdkNative.UvcImageParams
                {
                    contrast = (ushort)nudContrast.Value,
                    brightness = (ushort)nudBrightness.Value,
                    dde_enable = (byte)(cbDdeEnable.Checked ? 1 : 0),
                    dde_value = (ushort)nudDdeValue.Value
                };

                int ret = UvcSdkNative.uvc_set_image_params(_uvcHandle, ref p);
                if (!UvcSdkNative.IsOk(ret))
                    MessageBox.Show($"Set image params failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    DeviceReadCache.RememberImageParams(_uvcHandle, p);

                if (_deviceMode == DeviceMode.Thermal)
                {
                    var srMode = cbSrEnable.Checked
                        ? UvcSdkNative.UvcSrMode.SR
                        : UvcSdkNative.UvcSrMode.None;
                    int srRet = UvcSdkNative.uvc_set_thermal_sr(_uvcHandle, ref srMode);
                    if (!UvcSdkNative.IsOk(srRet))
                        MessageBox.Show($"Set SR mode failed: {srRet}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (UvcSdkNative.IsOk(ret))
                        UpdateRendererParams();
                    if (UvcSdkNative.IsOk(srRet))
                        LoadProperties();
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Device Management

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
            if (!_connected) return;

            if (string.IsNullOrWhiteSpace(tbUpgradePath.Text))
            {
                MessageBox.Show("Select a firmware file first.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Upgrade device firmware?\nDo NOT power off during upgrade.",
                Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                Cursor = Cursors.WaitCursor;

                byte[] pathBytes = UvcSdkNative.StringToFixedBytes(tbUpgradePath.Text, UvcSdkNative.UVC_LEN_128);
                int ret = UvcSdkNative.uvc_device_upgrade(_uvcHandle, pathBytes, (ushort)pathBytes.Length);
                if (!UvcSdkNative.IsOk(ret))
                {
                    MessageBox.Show($"Upgrade failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var pollResult = WaitForUpgradeComplete();
                MessageBox.Show(pollResult.Message, Text, MessageBoxButtons.OK,
                    pollResult.IsSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private struct UpgradePollResult
        {
            public string Message;
            public bool IsSuccess;

            public UpgradePollResult(string message, bool isSuccess)
            {
                Message = message;
                IsSuccess = isSuccess;
            }
        }

        private UpgradePollResult WaitForUpgradeComplete()
        {
            const int pollIntervalMs = 500;
            const int timeoutMs = 120000;
            const int queryRetryCount = 6;
            int elapsed = 0;
            UvcSdkNative.UvcUpgradeState lastState = UvcSdkNative.UvcUpgradeState.Idle;
            bool sawUpgradeProgress = false;

            while (elapsed < timeoutMs)
            {
                UvcSdkNative.UvcUpgradeState state;
                int ret = UvcSdkNative.uvc_get_upgrade_state(_uvcHandle, out state);
                if (!UvcSdkNative.IsOk(ret))
                {
                    var disconnectResult = InferSuccessAfterDisconnect(lastState, sawUpgradeProgress);
                    if (disconnectResult != null)
                        return disconnectResult.Value;

                    if (sawUpgradeProgress && TryRecoverUpgradeState(queryRetryCount, pollIntervalMs, ref elapsed, out state))
                    {
                        lastState = state;
                        if (state != UvcSdkNative.UvcUpgradeState.Idle)
                            sawUpgradeProgress = true;
                    }
                    else
                    {
                        if (sawUpgradeProgress)
                            return LikelyCompletedResult();
                        return new UpgradePollResult($"Failed to query upgrade state: {ret}", false);
                    }
                }

                lastState = state;
                if (state != UvcSdkNative.UvcUpgradeState.Idle)
                    sawUpgradeProgress = true;

                switch (state)
                {
                    case UvcSdkNative.UvcUpgradeState.Done:
                        return new UpgradePollResult(
                            "Upgrade completed. Device will restart automatically.", true);

                    case UvcSdkNative.UvcUpgradeState.Failed:
                        if (ConfirmUpgradeFailed(queryRetryCount, pollIntervalMs, ref elapsed, ref lastState, ref sawUpgradeProgress, out state))
                            return new UpgradePollResult("Upgrade failed on device.", false);

                        if (state == UvcSdkNative.UvcUpgradeState.Done)
                            return new UpgradePollResult(
                                "Upgrade completed. Device will restart automatically.", true);

                        var failedDisconnect = InferSuccessAfterDisconnect(lastState, sawUpgradeProgress);
                        if (failedDisconnect != null)
                            return failedDisconnect.Value;

                        if (state != UvcSdkNative.UvcUpgradeState.Failed)
                            continue;
                        return LikelyCompletedResult();

                    case UvcSdkNative.UvcUpgradeState.Idle:
                        if (elapsed > 0)
                        {
                            if (sawUpgradeProgress)
                                return LikelyCompletedResult();
                            return new UpgradePollResult("Upgrade ended (device idle).", false);
                        }
                        break;
                }

                Thread.Sleep(pollIntervalMs);
                elapsed += pollIntervalMs;
            }

            return new UpgradePollResult("Upgrade timeout. Check device status manually.", false);
        }

        private static UpgradePollResult LikelyCompletedResult()
        {
            return new UpgradePollResult(
                "Upgrade likely completed. Device may have restarted and disconnected. " +
                "Please reconnect and verify the firmware version.", true);
        }

        private static UpgradePollResult? InferSuccessAfterDisconnect(
            UvcSdkNative.UvcUpgradeState lastState, bool sawUpgradeProgress)
        {
            if (lastState == UvcSdkNative.UvcUpgradeState.Done)
                return new UpgradePollResult(
                    "Upgrade completed. Device is restarting.", true);

            if (lastState == UvcSdkNative.UvcUpgradeState.Flashing ||
                lastState == UvcSdkNative.UvcUpgradeState.Staged)
                return LikelyCompletedResult();

            if (sawUpgradeProgress)
                return LikelyCompletedResult();

            return null;
        }

        private bool TryRecoverUpgradeState(
            int retryCount, int pollIntervalMs, ref int elapsed,
            out UvcSdkNative.UvcUpgradeState state)
        {
            for (int i = 0; i < retryCount; i++)
            {
                Thread.Sleep(pollIntervalMs);
                elapsed += pollIntervalMs;
                int ret = UvcSdkNative.uvc_get_upgrade_state(_uvcHandle, out state);
                if (UvcSdkNative.IsOk(ret))
                    return true;
            }

            state = UvcSdkNative.UvcUpgradeState.Idle;
            return false;
        }

        private bool ConfirmUpgradeFailed(
            int retryCount, int pollIntervalMs, ref int elapsed,
            ref UvcSdkNative.UvcUpgradeState lastState, ref bool sawUpgradeProgress,
            out UvcSdkNative.UvcUpgradeState state)
        {
            state = UvcSdkNative.UvcUpgradeState.Failed;
            for (int i = 0; i < retryCount; i++)
            {
                Thread.Sleep(pollIntervalMs);
                elapsed += pollIntervalMs;
                int ret = UvcSdkNative.uvc_get_upgrade_state(_uvcHandle, out state);
                if (!UvcSdkNative.IsOk(ret))
                    return false;

                lastState = state;
                if (state != UvcSdkNative.UvcUpgradeState.Idle)
                    sawUpgradeProgress = true;

                if (state == UvcSdkNative.UvcUpgradeState.Done)
                    return false;
                if (state != UvcSdkNative.UvcUpgradeState.Failed)
                    return false;
            }

            return state == UvcSdkNative.UvcUpgradeState.Failed;
        }

        private void btnDeviceRestart_Click(object sender, EventArgs e)
        {
            if (!_connected) return;

            if (MessageBox.Show("Restart device?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            if (_streaming)
                StopStream();

            int ret = UvcSdkNative.uvc_device_restart(_uvcHandle);
            if (UvcSdkNative.IsOk(ret))
                MessageBox.Show("Device restarting.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Restart failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnDeviceReset_Click(object sender, EventArgs e)
        {
            if (!_connected) return;

            if (MessageBox.Show("Reset device to factory defaults?\nAll settings will be lost.",
                Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            int ret = UvcSdkNative.uvc_device_reset(_uvcHandle);
            if (UvcSdkNative.IsOk(ret))
                MessageBox.Show("Device reset.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Reset failed: {ret}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Color scale

        private void ThermalPreview_Resize(object sender, EventArgs e)
        {
            if (_deviceMode == DeviceMode.Thermal)
                LayoutThermalPreview();
        }

        private void LayoutThermalPreview()
        {
            int clientW = Math.Max(1, scMain.Panel1.ClientSize.Width);
            int clientH = Math.Max(1, scMain.Panel1.ClientSize.Height);
            int left = Math.Max(0, _thermalPreviewBounds.X);
            int top = Math.Max(0, _thermalPreviewBounds.Y);
            const int markerHeight = 120;
            const int gap = 6;
            const int edge = 3;
            int fullWidth = Math.Max(1, clientW - left - edge);
            int barWidth = ColorScaleBar.BarWidth;
            int imageWidth = Math.Max(1, fullWidth - gap - barWidth);
            int markerTop = clientH - edge - markerHeight;
            if (markerTop < top + 80)
                markerTop = top + 80;
            int imageHeight = Math.Max(1, markerTop - gap - top);

            pbThermalImage.Dock = DockStyle.None;
            pbThermalImage.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pbThermalImage.Bounds = new Rectangle(left, top, imageWidth, imageHeight);

            mlvMarkers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            mlvMarkers.Bounds = new Rectangle(left, markerTop, fullWidth, markerHeight);

            _colorScale.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _colorScale.Bounds = new Rectangle(left + imageWidth + gap, top, barWidth, imageHeight);
            _colorScale.BringToFront();

            if (_cursorOverVideo && pbThermalImage.IsHandleCreated)
            {
                Point client = pbThermalImage.PointToClient(System.Windows.Forms.Cursor.Position);
                UpdateCursorTemperature(client, true);
            }
        }

        private void SyncColorScaleFromDevice()
        {
            if (!_connected || _deviceMode != DeviceMode.Thermal || _uvcHandle == IntPtr.Zero)
                return;
            ThermalAdLut lut = EnsureColorScaleLut();
            if (lut == null)
                return;

            var gain = new UvcSdkNative.UvcThermalGain();
            if (!UvcSdkNative.IsOk(DeviceReadCache.GetGain(_uvcHandle, ref gain)))
                return;

            bool auto = gain.mode == UvcSdkNative.UvcThermalGainMode.Auto;
            float minC = lut.ToTemp(gain.min) - ColorScaleCelsiusOffset;
            float maxC = lut.ToTemp(gain.max) - ColorScaleCelsiusOffset;
            if (minC > maxC)
            {
                float swap = minC;
                minC = maxC;
                maxC = swap;
            }
            if (!auto || !_colorScaleFixedReady)
            {
                _colorScaleFixedMinC = minC;
                _colorScaleFixedMaxC = maxC;
                _colorScaleFixedReady = true;
            }
            _colorScaleFollowFrame = auto;
            _colorScale.SetState(auto, auto ? minC : _colorScaleFixedMinC, auto ? maxC : _colorScaleFixedMaxC);
            if (!auto)
            {
                _colorScaleFixedMinC = _colorScale.MinCelsius;
                _colorScaleFixedMaxC = _colorScale.MaxCelsius;
            }
            RefreshColorScalePalette();
        }

        private void SyncAcousticColorScale()
        {
            if (!_connected || _deviceMode != DeviceMode.Acoustic || _uvcHandle == IntPtr.Zero)
                return;
            var scale = new UvcSdkNative.UvcAcousticDbScale();
            if (!UvcSdkNative.IsOk(DeviceReadCache.GetAcousticScale(_uvcHandle, ref scale)))
                return;
            ApplyAcousticColorScale(scale.min, scale.max);
            RefreshColorScalePalette();
        }

        private void ApplyAcousticColorScale(float minDb, float maxDb)
        {
            if (_colorScale == null || _deviceMode != DeviceMode.Acoustic)
                return;
            if (float.IsNaN(minDb) || float.IsNaN(maxDb) ||
                float.IsInfinity(minDb) || float.IsInfinity(maxDb))
                return;
            _colorScale.SetState(true, minDb, maxDb);
        }

        private void ColorScale_ModeChanged(object sender, EventArgs e)
        {
            if (_isGuiRefreshing || !_connected || _deviceMode != DeviceMode.Thermal)
                return;

            if (_colorScale.IsAuto)
            {
                if (!ApplyColorScaleGain(UvcSdkNative.UvcThermalGainMode.Auto, _colorScaleFixedMinC, _colorScaleFixedMaxC))
                {
                    _colorScaleFollowFrame = false;
                    _colorScale.SetState(false, _colorScaleFixedMinC, _colorScaleFixedMaxC);
                    return;
                }
                _colorScaleFollowFrame = true;
                lock (_colorScaleLock)
                {
                    _colorScalePendingMinC = float.NaN;
                    _colorScalePendingMaxC = float.NaN;
                }
                return;
            }

            _colorScaleFollowFrame = false;
            float minC;
            float maxC;
            lock (_colorScaleLock)
            {
                minC = _colorScaleLastMinC;
                maxC = _colorScaleLastMaxC;
            }
            if (float.IsNaN(minC) || float.IsNaN(maxC))
            {
                minC = _colorScale.MinCelsius;
                maxC = _colorScale.MaxCelsius;
            }
            _colorScale.SetState(false, minC, maxC);
            _colorScaleFixedMinC = _colorScale.MinCelsius;
            _colorScaleFixedMaxC = _colorScale.MaxCelsius;
            if (!ApplyColorScaleGain(UvcSdkNative.UvcThermalGainMode.Fixed, _colorScaleFixedMinC, _colorScaleFixedMaxC))
                SyncColorScaleFromDevice();
        }

        private void ColorScale_RangeChanged(object sender, EventArgs e)
        {
            if (_isGuiRefreshing || !_connected || _colorScale.IsAuto)
                return;
            float minC = _colorScale.MinCelsius;
            float maxC = _colorScale.MaxCelsius;
            if (!ApplyColorScaleGain(UvcSdkNative.UvcThermalGainMode.Fixed, minC, maxC))
            {
                _colorScale.SetState(false, _colorScaleFixedMinC, _colorScaleFixedMaxC);
                return;
            }
            _colorScaleFixedMinC = minC;
            _colorScaleFixedMaxC = maxC;
            _colorScaleFixedReady = true;
        }

        private bool ApplyColorScaleGain(UvcSdkNative.UvcThermalGainMode mode, float minC, float maxC)
        {
            if (!_connected || _deviceMode != DeviceMode.Thermal || _uvcHandle == IntPtr.Zero)
                return false;
            if (minC > maxC)
            {
                float swap = minC;
                minC = maxC;
                maxC = swap;
            }

            ThermalAdLut lut = EnsureColorScaleLut();
            if (lut == null)
            {
                MessageBox.Show("Thermal LUT is empty.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            float minK = minC + ColorScaleCelsiusOffset;
            float maxK = maxC + ColorScaleCelsiusOffset;
            float minAd = lut.ToAd(minK);
            float maxAd = lut.ToAd(maxK);
            float spanAd = maxAd - minAd;
            if (spanAd < 0f)
                spanAd = -spanAd;
            if (spanAd < 1f)
                spanAd = 1f;
            if (spanAd > 65535f)
                spanAd = 65535f;

            var gain = new UvcSdkNative.UvcThermalGain
            {
                mode = mode,
                min = (ushort)minAd,
                max = (ushort)maxAd,
                fixed_span = 0,
                span = (ushort)spanAd
            };
            int ret = UvcSdkNative.uvc_set_thermal_gain(_uvcHandle, ref gain);
            if (!UvcSdkNative.IsOk(ret))
            {
                MessageBox.Show("Set thermal gain failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            float spanK = maxK - minK;
            if (spanK < 0.1f)
                spanK = 0.1f;
            lock (_renderLock)
                _renderer?.SetGain((int)mode, minK, maxK, 0, spanK);
            return true;
        }

        private ThermalAdLut EnsureColorScaleLut()
        {
            lock (_colorScaleLock)
            {
                if (_colorScaleLut != null)
                    return _colorScaleLut;
            }
            if (_uvcHandle == IntPtr.Zero)
                return null;

            ThermalAdLut loaded;
            string error;
            if (!ThermalAdLut.TryLoad(_uvcHandle, out loaded, out error) || loaded == null)
                return null;
            lock (_colorScaleLock)
            {
                if (_colorScaleLut == null)
                    _colorScaleLut = loaded;
                return _colorScaleLut;
            }
        }

        private void PublishAutoColorScale(UvcSdkNative.UvcThermalGain gain)
        {
            if (!_colorScaleFollowFrame || _isDisposing)
                return;

            ushort[] ad;
            float[] temp;
            lock (_frameLock)
            {
                ad = _lastLutAd;
                temp = _lastLutTemp;
            }

            ushort[] cachedAd;
            float[] cachedTemp;
            ThermalAdLut lut;
            lock (_colorScaleLock)
            {
                if (!_colorScaleFollowFrame || _isDisposing)
                    return;
                cachedAd = _colorScaleLutAd;
                cachedTemp = _colorScaleLutTemp;
                lut = _colorScaleLut;
            }

            bool canBuild = ad != null && temp != null && ad.Length > 0 && ad.Length == temp.Length;
            ThermalAdLut built = null;
            if (canBuild && !SameLut(cachedAd, cachedTemp, ad, temp))
                built = ThermalAdLut.FromSamples(ad, temp);

            lock (_colorScaleLock)
            {
                if (!_colorScaleFollowFrame || _isDisposing)
                    return;
                if (built != null)
                {
                    _colorScaleLutAd = ad;
                    _colorScaleLutTemp = temp;
                    _colorScaleLut = built;
                    lut = built;
                }
            }
            if (lut == null)
                return;

            float minC = lut.ToTemp(gain.min) - ColorScaleCelsiusOffset;
            float maxC = lut.ToTemp(gain.max) - ColorScaleCelsiusOffset;
            if (minC > maxC)
            {
                float swap = minC;
                minC = maxC;
                maxC = swap;
            }

            bool post;
            lock (_colorScaleLock)
            {
                if (!_colorScaleFollowFrame || _isDisposing)
                    return;
                _colorScaleLastMinC = minC;
                _colorScaleLastMaxC = maxC;
                if (Math.Abs(minC - _colorScalePendingMinC) < 0.05f &&
                    Math.Abs(maxC - _colorScalePendingMaxC) < 0.05f)
                    return;
                _colorScalePendingMinC = minC;
                _colorScalePendingMaxC = maxC;
                post = !_colorScaleUpdateQueued;
                _colorScaleUpdateQueued = true;
            }
            if (!post)
                return;
            if (!IsHandleCreated || _isDisposing)
            {
                lock (_colorScaleLock)
                    _colorScaleUpdateQueued = false;
                return;
            }
            try
            {
                BeginInvoke(new Action(ApplyPendingColorScale));
            }
            catch (InvalidOperationException)
            {
                lock (_colorScaleLock)
                    _colorScaleUpdateQueued = false;
            }
        }

        private void ApplyPendingColorScale()
        {
            float minC;
            float maxC;
            lock (_colorScaleLock)
            {
                minC = _colorScalePendingMinC;
                maxC = _colorScalePendingMaxC;
                _colorScaleUpdateQueued = false;
            }
            if (_isDisposing || !_colorScaleFollowFrame || _deviceMode != DeviceMode.Thermal)
                return;
            if (float.IsNaN(minC) || float.IsNaN(maxC))
                return;
            _colorScale.SetState(true, minC, maxC);
        }

        private static bool SameLut(ushort[] cachedAd, float[] cachedTemp, ushort[] ad, float[] temp)
        {
            if (cachedAd == null || cachedTemp == null || ad == null || temp == null)
                return false;
            if (cachedAd.Length != ad.Length || cachedTemp.Length != temp.Length)
                return false;
            for (int i = 0; i < ad.Length; i++)
            {
                if (cachedAd[i] != ad[i] || cachedTemp[i] != temp[i])
                    return false;
            }
            return true;
        }

        private void RefreshColorScalePalette()
        {
            if (_colorScale == null)
                return;
            if (!_connected ||
                (_deviceMode != DeviceMode.Thermal && _deviceMode != DeviceMode.Acoustic))
            {
                _colorScale.SetColors(null);
                return;
            }

            if (_deviceMode == DeviceMode.Acoustic)
            {
                Color[] acoustic = ReadAcousticPresetRibbon(_paletteRefNo);
                if (acoustic != null && _paletteInverted != 0)
                    Array.Reverse(acoustic);
                if (ColorScaleBar.HasColor(acoustic))
                {
                    _colorScale.SetColors(acoustic);
                    return;
                }

                var devicePalette = UvcSdkNative.UvcPalette.Create();
                if (UvcSdkNative.IsOk(DeviceReadCache.GetAcousticPalette(_uvcHandle, ref devicePalette)))
                    _colorScale.SetPalette(devicePalette.colors, devicePalette.inverted != 0);
                else
                    _colorScale.SetColors(null);
                return;
            }

            IntPtr file = IntPtr.Zero;
            lock (_renderLock)
            {
                if (_renderer != null)
                    file = _renderer.FileHandle;
            }

            Color[] colors = ReadPaletteRibbon(IntPtr.Zero, _paletteRefNo, true);
            if (colors != null && _paletteInverted != 0)
                Array.Reverse(colors);
            if (!ColorScaleBar.HasColor(colors))
                colors = ReadPaletteRibbon(file, 0, false);
            if (ColorScaleBar.HasColor(colors))
            {
                _colorScale.SetColors(colors);
                return;
            }

            var palette = UvcSdkNative.UvcPalette.Create();
            if (UvcSdkNative.IsOk(DeviceReadCache.GetThermalPalette(_uvcHandle, ref palette)))
                _colorScale.SetPalette(palette.colors, palette.inverted != 0);
            else
                _colorScale.SetColors(null);
        }

        private static Color[] ReadPaletteRibbon(IntPtr file, byte refNo, bool preset)
        {
            if (!preset && file == IntPtr.Zero)
                return null;
            return ReadColorRibbon((scan0, pad) =>
            {
                var image = new IRtekFileSDK.IrFileImage
                {
                    offset = pad,
                    data = scan0
                };
                return preset
                    ? IRtekFileSDK.ir_file_get_preset_palette_image(refNo, ref image)
                    : IRtekFileSDK.ir_file_get_palette_image(file, ref image);
            }, IRtekFileSDK.IR_FILE_EC_OK);
        }

        private static Color[] ReadAcousticPresetRibbon(byte refNo)
        {
            if (refNo == 0xFF)
                return null;
            return ReadColorRibbon((scan0, pad) =>
            {
                var image = new IRtekACFileSDK.AcFileBuffer
                {
                    length = pad,
                    buffer = scan0
                };
                return IRtekACFileSDK.ac_file_get_preset_palette_image(refNo, ref image);
            }, IRtekACFileSDK.AC_FILE_EC_OK);
        }

        private static Color[] ReadColorRibbon(Func<IntPtr, uint, int> fill, int okCode)
        {
            const int height = 256;
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(1, height, PixelFormat.Format24bppRgb);
                var rect = new Rectangle(0, 0, 1, height);
                BitmapData bits = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                byte[] raw = null;
                int stride = bits.Stride;
                try
                {
                    int ret = fill(bits.Scan0, (uint)Math.Max(0, stride - 3));
                    if (ret != okCode || stride < 3)
                        return null;
                    raw = new byte[stride * height];
                    Marshal.Copy(bits.Scan0, raw, 0, raw.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bits);
                }

                var colors = new Color[height];
                for (int y = 0; y < height; y++)
                {
                    int i = y * stride;
                    colors[y] = Color.FromArgb(raw[i + 2], raw[i + 1], raw[i]);
                }
                return colors;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (bitmap != null)
                    bitmap.Dispose();
            }
        }

        private void ResetColorScale()
        {
            _colorScaleFollowFrame = false;
            _colorScaleFixedReady = false;
            _colorScaleFixedMinC = 20f;
            _colorScaleFixedMaxC = 40f;
            lock (_colorScaleLock)
            {
                _colorScaleLut = null;
                _colorScaleLutAd = null;
                _colorScaleLutTemp = null;
                _colorScalePendingMinC = float.NaN;
                _colorScalePendingMaxC = float.NaN;
                _colorScaleLastMinC = float.NaN;
                _colorScaleLastMaxC = float.NaN;
                _colorScaleUpdateQueued = false;
            }
            _colorScale.SetState(false, _colorScaleFixedMinC, _colorScaleFixedMaxC);
            _colorScale.SetColors(null);
        }

        #endregion

        #region Helpers

        private void SetAcousticMaintenanceVisible(bool visible)
        {
            lblUpgradePath.Visible = visible;
            tbUpgradePath.Visible = visible;
            btnUpgradeBrowse.Visible = visible;
            btnUpgrade.Visible = visible;
            btnDeviceReset.Visible = visible;
            btnDeviceRestart.Location = visible ? new Point(80, 42) : new Point(6, 6);
            pnlDeviceMgmt.Height = visible ? 71 : 32;
        }

        private void StatsTimer_Tick(object sender, EventArgs e)
        {
            if (!_connected)
                return;

            if (_deviceMode == DeviceMode.Thermal)
            {
                int frames = _frameCounter;
                int fps = frames - _lastStatsFrames;
                _lastStatsFrames = frames;
                int frameW = _previewFrameW > 0 ? _previewFrameW : DisplayWidth;
                int frameH = _previewFrameH > 0 ? _previewFrameH : DisplayHeight;
                _tsslStats.Text = $"{frameW}x{frameH}  {fps} fps";
            }
            else if (_deviceMode == DeviceMode.Acoustic)
            {
                _tsslStats.Text = _acousticFps > 0
                    ? $"{_acousticVisualW}x{_acousticVisualH}  {_acousticFps:F1} fps"
                    : $"{_acousticVisualW}x{_acousticVisualH}";
            }
        }

        private void BuildSettingsSections(bool thermal, bool acoustic)
        {
            if (_settingsSectionsReady
                && _settingsSectionsThermal == thermal
                && _settingsSectionsAcoustic == acoustic)
                return;

            Control[] sections;
            if (thermal)
                sections = new Control[] { grpProperties, grpThermal, _grpImaging, grpDisplay };
            else if (acoustic)
                sections = new Control[] { grpProperties, _grpAcoustic, grpDisplay };
            else
                sections = new Control[] { grpProperties };

            _tcSettings.SuspendLayout();
            try
            {
                string selected = _tcSettings.SelectedTab == null ? null : _tcSettings.SelectedTab.Text;
                var oldPages = new TabPage[_tcSettings.TabPages.Count];
                for (int i = 0; i < oldPages.Length; i++)
                    oldPages[i] = _tcSettings.TabPages[i];
                foreach (TabPage page in oldPages)
                {
                    while (page.Controls.Count > 0)
                        page.Controls.RemoveAt(0);
                }
                _tcSettings.TabPages.Clear();
                foreach (TabPage page in oldPages)
                    page.Dispose();

                foreach (Control section in sections)
                {
                    string title = section.Tag as string;
                    if (string.IsNullOrEmpty(title))
                    {
                        title = section.Text;
                        section.Tag = title;
                        section.Text = string.Empty;
                    }
                    section.Dock = DockStyle.Fill;
                    var page = new TabPage(title)
                    {
                        Padding = new Padding(4),
                        UseVisualStyleBackColor = true
                    };
                    page.Controls.Add(section);
                    Control buttonHost;
                    if (_sectionButtonHosts.TryGetValue(section, out buttonHost))
                        page.Controls.Add(buttonHost);
                    _tcSettings.TabPages.Add(page);
                    if (title == selected)
                        _tcSettings.SelectedTab = page;
                }

                _settingsSectionsReady = true;
                _settingsSectionsThermal = thermal;
                _settingsSectionsAcoustic = acoustic;
            }
            finally
            {
                _tcSettings.ResumeLayout(false);
            }
        }

        private void RefitSettingsSections()
        {
            BuildSettingsSections(_deviceMode == DeviceMode.Thermal, _deviceMode == DeviceMode.Acoustic);
        }

        private void ConfigureForDeviceMode(DeviceMode mode)
        {
            _deviceMode = mode;
            bool thermal = mode == DeviceMode.Thermal;
            bool acoustic = mode == DeviceMode.Acoustic;

            _tsslMode.Text = mode == DeviceMode.Unknown ? string.Empty : mode.ToString();
            tsbCapture.Enabled = _connected;
            tsbCalibrate.Enabled = _connected && thermal;

            tsMainForm.SuspendLayout();
            ToolStripItem[] thermalItems =
            {
                tslRange, tscbRange, toolStripSeparator4,
                tsbSelect, tsbSpot, tsbRectangle, tsbLine, tsbDelete,
                toolStripSeparator2a, toolStripSeparator8, tsbCalibrate
            };
            foreach (ToolStripItem item in thermalItems)
                item.Visible = thermal;

            ToolStripItem[] imagingItems =
            {
                tslPalette, tscbPalette, tsbInverted,
                toolStripSeparator5, toolStripSeparator7, tslImageFlip, tscbImageFlip
            };
            foreach (ToolStripItem item in imagingItems)
                item.Visible = true;
            _tslBlend.Visible = acoustic;
            _tsBlend.Visible = acoustic;
            tsMainForm.ResumeLayout(false);

            SetAcousticMaintenanceVisible(!acoustic);
            UpdateSrVisible();

            BuildSettingsSections(thermal, acoustic);

            mlvMarkers.Visible = thermal;
            _pbAcousticSpectrum.Visible = acoustic;
            if (!acoustic)
                ClearAcousticSpectrumImage();
            if (thermal)
            {
                _colorScale.ShowMode(true);
                LayoutThermalPreview();
                _colorScale.Visible = true;
            }
            else if (acoustic)
            {
                _colorScale.ShowMode(false);
                LayoutAcousticSpectrum();
                _colorScale.Visible = true;
                _pbAcousticSpectrum.BringToFront();
                _colorScale.BringToFront();
            }
            else
            {
                _colorScale.Visible = false;
                pbThermalImage.Anchor = AnchorStyles.None;
                pbThermalImage.Dock = DockStyle.Fill;
            }
        }

        private void LoadPalettes()
        {
            _isGuiRefreshing = true;
            try
            {
                tscbPalette.Items.Clear();

                string[] names = _deviceMode == DeviceMode.Acoustic
                    ? new[] { "Red-Blue", "Iron", "Grey" }
                    : new[]
                    {
                        "Iron", "Grey", "Iron10", "Rainbow", "Grey10", "GreyRed",
                        "MidGrey", "Yellow", "Rain", "Rain10", "Blue", "GlowBow",
                        "Medical", "MidGreen", "Prism", "Medical10"
                    };
                for (int i = 0; i < names.Length; i++)
                    tscbPalette.Items.Add(new PresetPaletteItem { RefNo = (byte)(i + 1), Name = names[i] });

                if (tscbPalette.Items.Count > 0)
                {
                    tscbPalette.SelectedIndex = 0;
                    _paletteRefNo = ((PresetPaletteItem)tscbPalette.Items[0]).RefNo;
                }
                _paletteInverted = 0;
                _paletteAlpha = 60;
                tsbInverted.Checked = false;
                _nudBlend.Value = _paletteAlpha;
            }
            finally
            {
                _isGuiRefreshing = false;
            }
        }

        private void LoadPalettesFromDevice()
        {
            bool acoustic = _deviceMode == DeviceMode.Acoustic;
            UvcSdkNative.NamedPreset[] presets;
            if (!DeviceReadCache.TryGetPresetPalettes(_uvcHandle, acoustic, out presets))
            {
                LoadPalettes();
                return;
            }

            byte refNo = _paletteRefNo;
            byte inverted = _paletteInverted;
            byte alpha = _paletteAlpha;
            var palette = UvcSdkNative.UvcPalette.Create();
            int ret = acoustic
                ? DeviceReadCache.GetAcousticPalette(_uvcHandle, ref palette)
                : DeviceReadCache.GetThermalPalette(_uvcHandle, ref palette);
            if (UvcSdkNative.IsOk(ret))
            {
                refNo = palette.ref_no;
                inverted = palette.inverted;
                alpha = palette.alpha;
            }

            _isGuiRefreshing = true;
            try
            {
                tscbPalette.Items.Clear();
                int index = 0;
                for (int i = 0; i < presets.Length; i++)
                {
                    tscbPalette.Items.Add(new PresetPaletteItem
                    {
                        RefNo = presets[i].RefNo,
                        Name = presets[i].Name
                    });
                    if (presets[i].RefNo == refNo)
                        index = i;
                }
                if (tscbPalette.Items.Count > 0)
                    tscbPalette.SelectedIndex = index;
                _paletteRefNo = refNo;
                _paletteInverted = inverted;
                _paletteAlpha = alpha > 100 ? (byte)60 : alpha;
                tsbInverted.Checked = inverted != 0;
                _nudBlend.Value = _paletteAlpha;
            }
            finally
            {
                _isGuiRefreshing = false;
            }
        }

        private void UpdateRendererParams()
        {
            if (_renderer == null || !_connected) return;

            try
            {
                var thermalParams = new UvcSdkNative.UvcThermalParams();
                int ret = DeviceReadCache.GetThermalParams(_uvcHandle, ref thermalParams);
                if (UvcSdkNative.IsOk(ret))
                    _renderer.SetThermalParams(thermalParams);

                ushort rangeId;
                ret = DeviceReadCache.GetThermalRange(_uvcHandle, out rangeId);
                if (UvcSdkNative.IsOk(ret))
                {
                    var lensInfo = UvcSdkNative.UvcLensInfo.Create();
                    ret = DeviceReadCache.GetLensInfo(_uvcHandle, ref lensInfo);
                    if (UvcSdkNative.IsOk(ret))
                    {
                        for (int i = 0; i < lensInfo.ranges_count && i < lensInfo.range_map.Length; i++)
                        {
                            if (lensInfo.range_map[i].id == rangeId)
                            {
                                _renderer.SetTempRange(lensInfo.range_map[i].range.min, lensInfo.range_map[i].range.max);
                                break;
                            }
                        }
                    }
                }

                UvcSdkNative.UvcLutTable lut;
                if (!DeviceReadCache.TryGetLutTable(_uvcHandle, out ret, out lut))
                {
                    lut = UvcSdkNative.UvcLutTable.Create();
                    ret = UvcSdkNative.uvc_get_thermal_lut(_uvcHandle, ref lut);
                }
                if (UvcSdkNative.IsOk(ret) && lut.lut_count > 0 && lut.ad != null && lut.t != null)
                {
                    int lutLen = lut.lut_count;
                    ushort[] adValues = new ushort[lutLen];
                    float[] tempValues = new float[lutLen];
                    for (int i = 0; i < lutLen; i++)
                    {
                        adValues[i] = lut.ad[i];
                        tempValues[i] = lut.t[i];
                    }
                    _renderer.SetLut(adValues, tempValues);
                }

                ThermalAdLut adLut;
                string adLutError;
                bool hasLut = ThermalAdLut.TryLoad(_uvcHandle, out adLut, out adLutError);

                var gain = new UvcSdkNative.UvcThermalGain();
                ret = DeviceReadCache.GetGain(_uvcHandle, ref gain);
                if (UvcSdkNative.IsOk(ret) && hasLut)
                    _renderer.SetGain((int)gain.mode, adLut.ToTemp(gain.min), adLut.ToTemp(gain.max), gain.fixed_span, adLut.SpanToTemp(gain.min, gain.span));

                UvcSdkNative.UvcColorDistType colorDist;
                ret = DeviceReadCache.GetColorDist(_uvcHandle, out colorDist);
                if (UvcSdkNative.IsOk(ret))
                    _renderer.SetColorDist((int)colorDist);

                var devicePalette = UvcSdkNative.UvcPalette.Create();
                ret = DeviceReadCache.GetThermalPalette(_uvcHandle, ref devicePalette);
                if (UvcSdkNative.IsOk(ret))
                    _renderer.SetPalette(devicePalette.ref_no, devicePalette.inverted);
                else
                    _renderer.SetPalette(_paletteRefNo, _paletteInverted);

                var alarm = new UvcSdkNative.UvcAlarm();
                ret = DeviceReadCache.GetAlarm(_uvcHandle, ref alarm);
                if (UvcSdkNative.IsOk(ret) && hasLut)
                    _renderer.SetAlarm((int)alarm.type, adLut.ToTemp(alarm.max), adLut.ToTemp(alarm.min), alarm.custom, alarm.color);
            }
            catch { }
            RefreshColorScalePalette();
        }

        private void ApplyPaletteSelection()
        {
            if (_deviceMode == DeviceMode.Acoustic)
            {
                SetAcousticPalette();
                RefreshColorScalePalette();
                return;
            }

            SetThermalPalette();
            lock (_renderLock)
            {
                _renderer?.SetPalette(_paletteRefNo, _paletteInverted);
            }
            RefreshColorScalePalette();
        }

        private void SetThermalPalette()
        {
            if (!_connected || _deviceMode != DeviceMode.Thermal) return;

            var palette = UvcSdkNative.UvcPalette.Create();
            palette.ref_no = _paletteRefNo;
            palette.inverted = _paletteInverted;
            UvcSdkNative.uvc_set_thermal_palette(_uvcHandle, ref palette);
        }

        private void SetAcousticPalette()
        {
            if (!_connected || _deviceMode != DeviceMode.Acoustic) return;

            var palette = UvcSdkNative.UvcPalette.Create();
            int ret = UvcSdkNative.uvc_get_acoustic_palette(_uvcHandle, ref palette);
            if (!UvcSdkNative.IsOk(ret)) return;

            palette.ref_no = _paletteRefNo;
            palette.inverted = _paletteInverted;
            palette.alpha = _paletteAlpha;
            UvcSdkNative.uvc_set_acoustic_palette(_uvcHandle, ref palette);
        }

        private static string FormatMarkerTempCelsius(float kelvin, int decimals = 2)
        {
            return string.Format("{0:F" + decimals + "}°C", kelvin - 273.15f);
        }

        private static string FormatPathBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return string.Empty;
            int len = Array.IndexOf(bytes, (byte)0);
            if (len < 0) len = bytes.Length;
            if (len == 0) return string.Empty;
            var sb = new StringBuilder(len * 3);
            for (int i = 0; i < len; i++)
            {
                if (i > 0) sb.Append('-');
                sb.Append(bytes[i]);
            }
            return sb.ToString();
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static string ValueOrDash(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "—" : value;
        }

        private static void EnableControlBuffer(Control control)
        {
            System.Reflection.PropertyInfo prop = typeof(Control).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (prop != null)
                prop.SetValue(control, true, null);
        }

        #endregion

        private sealed class LoginUiFilter : IMessageFilter
        {
            private readonly MainForm _form;

            public LoginUiFilter(MainForm form)
            {
                _form = form;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (!_form._loginUiLocked || _form.IsDisposed)
                    return false;

                int msg = m.Msg;
                bool mouse = msg >= 0x200 && msg <= 0x20E;
                bool key = msg >= 0x100 && msg <= 0x108;
                if (!mouse && !key)
                    return false;

                Control source = Control.FromChildHandle(m.HWnd);
                return source != null && (source == _form || _form.Contains(source));
            }
        }
    }
}
