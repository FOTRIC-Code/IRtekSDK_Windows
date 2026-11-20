using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IRtekNetSDK;

namespace IRtekNetSDKDemo
{
    public partial class MainForm
    {
        private const int AcousticViewWidth = 640;
        private const int AcousticViewHeight = 480;
        private const int AcousticSpectrumHeight = 88;

        private static readonly NetSdkNative.NetAcousticDbScaleMode[] AcousticModes =
        {
            NetSdkNative.NetAcousticDbScaleMode.AutoMono,
            NetSdkNative.NetAcousticDbScaleMode.AutoMulti,
            NetSdkNative.NetAcousticDbScaleMode.Fixed,
            NetSdkNative.NetAcousticDbScaleMode.AutoHolo
        };

        private static readonly string[] AcousticModeNames = { "Mono", "Multi", "Fixed", "Holo" };

        private bool _acousticActive;
        private volatile bool _acousticDeviceVideoPreview;
        private bool _acousticDataWanted;
        private bool _acousticUtilsWarned;
        private IntPtr _acousticDataHandle = IntPtr.Zero;
        private NetSdkNative.OnNetAcousticDataStreamReceivedCallback _acousticDataCb;
        private Font _acousticOverlayFont;
        private NetSdkNative.NetAcousticSensorInfo _acousticSensor;
        private NetSdkNative.NetAcousticDbScale _acousticScale;
        private NetSdkNative.NetAcousticDataConfig _acousticStreamConfig;
        private AcousticFrameCopy _pendingAcousticFrame;
        private readonly object _acousticFrameLock = new object();
        private string _acousticPayloadSignature;
        private int _acousticPayloadLogCount;
        private string _acousticSourceAlarmText = string.Empty;
        private string _acousticDbAlarmText = string.Empty;
        private string _acousticFreqAlarmText = string.Empty;
        private ColumnHeader chAcAlarmDb;
        private ColumnHeader chAcAlarmFreq;
        private int _alarmColumnWidth = -1;
        private int _valueColumnWidth = -1;
        private int _countPointsColumnWidth = -1;
        private string _savedMaxUnit = "°C";
        private string _savedMinUnit = "°C";

        private ToolStripComboBox tscbAcousticScale;
        private Panel pnlDeviceScale;
        private Panel pnlDeviceFixed;
        private Panel pnlDeviceHolo;
        private NumericUpDown nudDeviceMax;
        private NumericUpDown nudDeviceMin;
        private CheckBox cbDeviceFixedSpan;
        private NumericUpDown nudDeviceSpan;
        private bool _syncingDeviceScale;
        private ToolStripButton tsbAcousticData;
        private ToolStripTextBox tstbAcousticInterval;
        private ToolStripButton tsbAcousticVisual;
        private ToolStripButton tsbAcousticSpl;
        private ToolStripButton tsbAcousticSpectrum;
        private ToolStripButton tsbAcousticGlobal;
        private ToolStripButton tsbAcousticRefresh;
        private ToolStripComboBox tscbAcousticPreview;
        private ToolStripItem[] _acousticLocalDataTools;
        private ToolStripItem[] _acousticTools;
        private ToolStripItem[] _thermalOnlyTools;
        private PictureBox pbAcousticSpectrum;
        private int _acousticLayoutDelta;

        private Panel pnlAcousticParams;
        private Panel pnlAcousticOverlay;
        private Panel pnlAcousticAlarm;
        private NumericUpDown nudAcousticSensitivity;
        private NumericUpDown nudAcousticSourceSize;
        private NumericUpDown nudAcousticLimit;
        private NumericUpDown nudAcousticFreqMin;
        private NumericUpDown nudAcousticFreqMax;
        private CheckBox cbAcPalEnabled;
        private ComboBox cbAcPalAlign;
        private ComboBox cbAcPalMode;
        private NumericUpDown nudAcPalX;
        private NumericUpDown nudAcPalY;
        private CheckBox cbAcSpecEnabled;
        private ComboBox cbAcSpecAlign;
        private ComboBox cbAcSpecMode;
        private NumericUpDown nudAcSpecX;
        private NumericUpDown nudAcSpecY;
        private CheckBox cbAcSplEnabled;
        private ComboBox cbAcSplAlign;
        private NumericUpDown nudAcSplX;
        private NumericUpDown nudAcSplY;
        private CheckBox cbAcSplMaxDb;
        private CheckBox cbAcSplMaxFreq;
        private CheckBox cbAcSourceEnabled;
        private NumericUpDown nudAcSourceDuration;
        private NumericUpDown nudAcSourceTolerance;
        private NumericUpDown nudAcSourceNoRepeat;
        private CheckBox cbAcSourceFtp;
        private CheckBox cbAcSourceWebhook;
        private CheckBox cbAcDbEnabled;
        private NumericUpDown nudAcDbThreshold;
        private NumericUpDown nudAcDbNoRepeat;
        private CheckBox cbAcDbFtp;
        private CheckBox cbAcDbWebhook;
        private CheckBox cbAcFreqEnabled;
        private NumericUpDown nudAcFreqFrequency;
        private NumericUpDown nudAcFreqThreshold;
        private NumericUpDown nudAcFreqNoRepeat;
        private CheckBox cbAcFreqFtp;
        private CheckBox cbAcFreqWebhook;
        private Button btnAcAlarmListenerStart;
        private Button btnAcAlarmListenerStop;

        private void EnsureAcousticUi()
        {
            if (tscbAcousticScale != null)
                return;

            tscbAcousticScale = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 70 };
            foreach (string name in AcousticModeNames)
                tscbAcousticScale.Items.Add(name);
            tscbAcousticScale.SelectedIndex = 0;
            tscbAcousticScale.SelectedIndexChanged += AcousticScaleModeChanged;
            pbPalleteImage.SizeChanged += AcousticPaletteSizeChanged;
            pbThermalImage.SizeChanged += AcousticPreviewLayoutChanged;
            pbThermalImage.LocationChanged += AcousticPreviewLayoutChanged;
            mlvMarkers.LocationChanged += AcousticPreviewLayoutChanged;
            mlvMarkers.SizeChanged += AcousticPreviewLayoutChanged;
            Resize += AcousticPreviewLayoutChanged;
            chAcAlarmDb = new ColumnHeader { Text = "Alarm (Decibel)", Width = 0 };
            chAcAlarmFreq = new ColumnHeader { Text = "Alarm (Frequency)", Width = 0 };
            mlvMarkers.Columns.Add(chAcAlarmDb);
            mlvMarkers.Columns.Add(chAcAlarmFreq);

            _acousticOverlayFont = new Font(Font.FontFamily, 8f);

            tsbAcousticData = new ToolStripButton("Acoustic Data") { CheckOnClick = true, DisplayStyle = ToolStripItemDisplayStyle.Text };
            tsbAcousticData.CheckedChanged += AcousticDataToggleChanged;
            tstbAcousticInterval = new ToolStripTextBox { Width = 48, Text = "100" };
            tstbAcousticInterval.Leave += AcousticStreamOptionChanged;
            tsbAcousticVisual = CheckedTool("Visual");
            tsbAcousticSpl = CheckedTool("SPL");
            tsbAcousticSpectrum = CheckedTool("Spectrum");
            tsbAcousticGlobal = CheckedTool("Global");
            tsbAcousticVisual.CheckedChanged += AcousticStreamOptionChanged;
            tsbAcousticSpl.CheckedChanged += AcousticStreamOptionChanged;
            tsbAcousticSpectrum.CheckedChanged += AcousticStreamOptionChanged;
            tsbAcousticGlobal.CheckedChanged += AcousticStreamOptionChanged;
            tsbAcousticRefresh = new ToolStripButton("Refresh") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            tsbAcousticRefresh.Click += AcousticRefreshClick;
            var lblAcousticInterval = new ToolStripLabel("Interval");
            var lblAcousticIntervalUnit = new ToolStripLabel("ms");
            tscbAcousticPreview = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110 };
            tscbAcousticPreview.Items.Add("Local Data");
            tscbAcousticPreview.Items.Add("Device Video");
            tscbAcousticPreview.SelectedIndex = 0;
            tscbAcousticPreview.SelectedIndexChanged += AcousticPreviewModeChanged;

            _acousticLocalDataTools = new ToolStripItem[]
            {
                tsbAcousticData,
                lblAcousticInterval,
                tstbAcousticInterval,
                lblAcousticIntervalUnit,
                tsbAcousticVisual,
                tsbAcousticSpl,
                tsbAcousticSpectrum,
                tsbAcousticGlobal,
                tsbAcousticRefresh
            };
            _acousticTools = new ToolStripItem[]
            {
                new ToolStripLabel("Db Scale"),
                tscbAcousticScale,
                new ToolStripSeparator(),
                tsbAcousticData,
                lblAcousticInterval,
                tstbAcousticInterval,
                lblAcousticIntervalUnit,
                tsbAcousticVisual,
                tsbAcousticSpl,
                tsbAcousticSpectrum,
                tsbAcousticGlobal,
                tsbAcousticRefresh,
                new ToolStripSeparator(),
                new ToolStripLabel("Preview"),
                tscbAcousticPreview
            };
            foreach (ToolStripItem item in _acousticTools)
                item.Visible = false;
            int insertAt = 0;
            foreach (ToolStripItem item in _acousticTools)
                tsMainForm.Items.Insert(insertAt++, item);

            _thermalOnlyTools = new ToolStripItem[]
            {
                tslFileFormat, tscbFileFormat, toolStripSeparator4,
                tsbSelect, tsbSpot, tsbRectangle, tsbPolygon, tsbLine, tsbDelete, toolStripSeparator2,
                tslPalette, tsbPalette, tscbPalette, tsbInverted, toolStripSeparator5,
                tslColorDistributionType, tscbColorDistributionType, toolStripSeparator6, toolStripSeparator7,
                tslImageFlip, tscbImageFlip,
                toolStripSeparator11, tslThermalStreamConfig, tscbThermalStreamConfig, toolStripSeparator12,
                tsbGetThermalData, tsbGetThermalLut, tsbStreamCallbackToggle
            };

            pnlAcousticParams = BuildParamsPanel();
            pnlAcousticOverlay = BuildOverlayPanel();
            pnlAcousticAlarm = BuildAlarmPanel();
            tpThermalParams.Controls.Add(pnlAcousticParams);
            tpThermalOsd.Controls.Add(pnlAcousticOverlay);
            tpAlarm.Controls.Add(pnlAcousticAlarm);
            pnlAcousticParams.BringToFront();
            pnlAcousticOverlay.BringToFront();
            pnlAcousticAlarm.BringToFront();

            BuildDeviceScalePanel();

            pbAcousticSpectrum = new PictureBox
            {
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false,
                TabStop = false
            };
            Controls.Add(pbAcousticSpectrum);
        }

        private static ToolStripButton CheckedTool(string text)
        {
            return new ToolStripButton(text)
            {
                CheckOnClick = true,
                Checked = true,
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
        }

        private Panel BuildParamsPanel()
        {
            var panel = CoverPanel();
            int y = 8;
            nudAcousticSensitivity = DecimalBox(0, 10, 3.2m, 0.1m);
            nudAcousticSourceSize = new NumericUpDown { Minimum = 1, Maximum = 10, Value = 3, Width = 80 };
            nudAcousticLimit = DecimalBox(-100, 1000, -100, 1);
            nudAcousticFreqMin = DecimalBox(0, 200000, 20000, 100);
            nudAcousticFreqMax = DecimalBox(0, 200000, 40000, 100);
            y = AddRow(panel, y, "Sensitivity", nudAcousticSensitivity);
            y = AddRow(panel, y, "Source Size", nudAcousticSourceSize);
            y = AddRow(panel, y, "Limit (dB)", nudAcousticLimit);
            y = AddRow(panel, y, "Freq Min (Hz)", nudAcousticFreqMin);
            y = AddRow(panel, y, "Freq Max (Hz)", nudAcousticFreqMax);
            var get = new Button { Text = "Get", Left = 8, Top = y + 8, Width = 72 };
            var apply = new Button { Text = "Apply", Left = 88, Top = y + 8, Width = 72 };
            get.Click += (s, e) => LoadAcousticSettings(true);
            apply.Click += (s, e) => ApplyAcousticParams();
            panel.Controls.Add(get);
            panel.Controls.Add(apply);
            var hint = new Label
            {
                Left = 8,
                Top = y + 40,
                Width = 430,
                Height = 48,
                Text = "Sensitivity and source size apply to Mono and Multi. Limit applies to every mode. Max, min and span stay on the color bar."
            };
            panel.Controls.Add(hint);
            return panel;
        }

        private Panel BuildOverlayPanel()
        {
            var panel = CoverPanel();
            int y = 8;
            cbAcPalEnabled = new CheckBox();
            cbAcPalAlign = AlignBox();
            cbAcPalMode = ModeBox();
            nudAcPalX = UintBox();
            nudAcPalY = UintBox();
            y = AddRow(panel, y, "Palette", cbAcPalEnabled);
            y = AddRow(panel, y, "Align", cbAcPalAlign);
            y = AddRow(panel, y, "Display", cbAcPalMode);
            y = AddRow(panel, y, "X / Y", Pair(nudAcPalX, nudAcPalY));
            cbAcSpecEnabled = new CheckBox();
            cbAcSpecAlign = AlignBox();
            cbAcSpecMode = ModeBox();
            nudAcSpecX = UintBox();
            nudAcSpecY = UintBox();
            y = AddRow(panel, y, "Spectrum", cbAcSpecEnabled);
            y = AddRow(panel, y, "Align", cbAcSpecAlign);
            y = AddRow(panel, y, "Display", cbAcSpecMode);
            y = AddRow(panel, y, "X / Y", Pair(nudAcSpecX, nudAcSpecY));
            cbAcSplEnabled = new CheckBox();
            cbAcSplAlign = AlignBox();
            nudAcSplX = UintBox();
            nudAcSplY = UintBox();
            cbAcSplMaxDb = new CheckBox { Text = "Max dB" };
            cbAcSplMaxFreq = new CheckBox { Text = "Max freq" };
            y = AddRow(panel, y, "SPL", cbAcSplEnabled);
            y = AddRow(panel, y, "Align", cbAcSplAlign);
            y = AddRow(panel, y, "X / Y", Pair(nudAcSplX, nudAcSplY));
            y = AddRow(panel, y, "Readout", Pair(cbAcSplMaxDb, cbAcSplMaxFreq));
            var get = new Button { Text = "Get", Left = 8, Top = y + 8, Width = 72 };
            var apply = new Button { Text = "Apply", Left = 88, Top = y + 8, Width = 72 };
            get.Click += (s, e) => LoadAcousticOverlay(true);
            apply.Click += (s, e) => ApplyAcousticOverlay();
            panel.Controls.Add(get);
            panel.Controls.Add(apply);
            return panel;
        }

        private Panel BuildAlarmPanel()
        {
            var panel = CoverPanel();
            int y = 8;
            cbAcSourceEnabled = new CheckBox { Text = "Source" };
            nudAcSourceDuration = UintBox();
            nudAcSourceTolerance = UintBox();
            y = AddRow(panel, y, "Source", cbAcSourceEnabled);
            nudAcSourceNoRepeat = UintBox();
            y = AddRow(panel, y, "Duration (s)", nudAcSourceDuration);
            y = AddRow(panel, y, "Tolerance px", nudAcSourceTolerance);
            y = AddRow(panel, y, "No Repeat (min)", nudAcSourceNoRepeat);
            y = AddTriggerRow(panel, y, out cbAcSourceFtp, out cbAcSourceWebhook);
            var sourceApply = new Button { Text = "Apply", Left = 124, Top = y, Width = 72 };
            sourceApply.Click += (s, e) => ApplySourceAlarm();
            panel.Controls.Add(sourceApply);
            y += 36;
            cbAcDbEnabled = new CheckBox { Text = "Decibel" };
            nudAcDbThreshold = DecimalBox(-100, 1000, 55, 1);
            nudAcDbNoRepeat = UintBox();
            y = AddRow(panel, y, "Decibel", cbAcDbEnabled);
            y = AddRow(panel, y, "Threshold (dB)", nudAcDbThreshold);
            y = AddRow(panel, y, "No Repeat (min)", nudAcDbNoRepeat);
            y = AddTriggerRow(panel, y, out cbAcDbFtp, out cbAcDbWebhook);
            var dbApply = new Button { Text = "Apply", Left = 124, Top = y, Width = 72 };
            dbApply.Click += (s, e) => ApplyDbAlarm();
            panel.Controls.Add(dbApply);
            y += 36;
            cbAcFreqEnabled = new CheckBox { Text = "Frequency" };
            nudAcFreqFrequency = DecimalBox(0, 200000, 20000, 100);
            nudAcFreqThreshold = DecimalBox(-100, 1000, 55, 1);
            y = AddRow(panel, y, "Frequency", cbAcFreqEnabled);
            nudAcFreqNoRepeat = UintBox();
            y = AddRow(panel, y, "Freq (Hz)", nudAcFreqFrequency);
            y = AddRow(panel, y, "Threshold (dB)", nudAcFreqThreshold);
            y = AddRow(panel, y, "No Repeat (min)", nudAcFreqNoRepeat);
            y = AddTriggerRow(panel, y, out cbAcFreqFtp, out cbAcFreqWebhook);
            var freqApply = new Button { Text = "Apply", Left = 124, Top = y, Width = 72 };
            freqApply.Click += (s, e) => ApplyFreqAlarm();
            panel.Controls.Add(freqApply);
            var get = new Button { Text = "Get", Left = 8, Top = y, Width = 72 };
            get.Click += (s, e) => LoadAcousticAlarms(true);
            panel.Controls.Add(get);
            y += 48;
            var hint = new Label
            {
                Text = "Alarm listener uses the current login session.",
                AutoSize = true,
                Left = 8,
                Top = y
            };
            panel.Controls.Add(hint);
            y += 24;
            bool listenerRunning = _alarmListenerHandle != IntPtr.Zero;
            btnAcAlarmListenerStart = new Button
            {
                Text = "Start Listener",
                Left = 8,
                Top = y,
                Width = 110,
                Enabled = !listenerRunning
            };
            btnAcAlarmListenerStop = new Button
            {
                Text = "Stop Listener",
                Left = 124,
                Top = y,
                Width = 110,
                Enabled = listenerRunning
            };
            btnAcAlarmListenerStart.Click += btnAlarmListenerStart_Click;
            btnAcAlarmListenerStop.Click += btnAlarmListenerStop_Click;
            panel.Controls.Add(btnAcAlarmListenerStart);
            panel.Controls.Add(btnAcAlarmListenerStop);
            return panel;
        }

        private static Panel CoverPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                AutoScroll = true,
                BackColor = SystemColors.Control
            };
        }

        private static NumericUpDown DecimalBox(decimal min, decimal max, decimal value, decimal increment)
        {
            return new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                DecimalPlaces = 1,
                Increment = increment,
                Value = value,
                Width = 80
            };
        }

        private static NumericUpDown UintBox()
        {
            return new NumericUpDown { Maximum = 1000000, Width = 80 };
        }

        private static ComboBox AlignBox()
        {
            var box = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            foreach (object value in Enum.GetValues(typeof(NetSdkNative.NetOverlayAlign)))
                box.Items.Add(value);
            box.SelectedIndex = 0;
            return box;
        }

        private static ComboBox ModeBox()
        {
            var box = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            foreach (object value in Enum.GetValues(typeof(NetSdkNative.NetOverlayDisplayMode)))
                box.Items.Add(value);
            box.SelectedIndex = 0;
            return box;
        }

        private static Control Pair(Control left, Control right)
        {
            var host = new Panel { Width = 180, Height = 24 };
            left.Left = 0;
            left.Top = 0;
            right.Left = 90;
            right.Top = 0;
            host.Controls.Add(left);
            host.Controls.Add(right);
            return host;
        }

        private static int AddTriggerRow(Panel panel, int y, out CheckBox ftp, out CheckBox webhook)
        {
            ftp = new CheckBox { Text = "FTP Trigger", AutoSize = true, Left = 124, Top = y };
            webhook = new CheckBox { Text = "Webhook Trigger", AutoSize = true, Left = 240, Top = y };
            panel.Controls.Add(ftp);
            panel.Controls.Add(webhook);
            return y + 26;
        }

        private static int AddRow(Panel panel, int y, string caption, Control editor)
        {
            panel.Controls.Add(new Label { Text = caption, Left = 8, Top = y + 4, Width = 110 });
            editor.Left = 124;
            editor.Top = y;
            panel.Controls.Add(editor);
            return y + 28;
        }

        private bool TryActivateAcousticDevice()
        {
            EnsureAcousticUi();
            var info = NetSdkNative.NetAcousticSensorInfo.Create();
            int ret = NetSdkNative.net_get_acoustic_sensor_info(_netHandle, ref info);
            LogApi("net_get_acoustic_sensor_info", ret);
            if (!NetSdkNative.IsOk(ret))
                return false;

            _acousticSensor = info;
            _acousticActive = true;
            LoadDeviceTime();
            LoadNetworkInfo();
            LoadUsers();
            ShowAcousticChrome();
            LoadProperties();
            LoadAcousticSettings(false);
            LoadAcousticOverlay(false);
            LoadAcousticAlarms(false);
            if (tsbAcousticData.Checked)
                StartAcousticDataStream();
            return true;
        }

        private void ShowAcousticChrome()
        {
            SetAcousticMaintenanceVisible(false);
            EnsureAcousticUi();
            SetMarkerTool(MarkerType.None, false, false);
            foreach (ToolStripItem item in _thermalOnlyTools)
            {
                if (item != null)
                    item.Visible = false;
            }
            foreach (ToolStripItem item in _acousticTools)
                item.Visible = true;

            tsbCalibrate.Enabled = false;
            pnlAcousticParams.Visible = true;
            pnlAcousticOverlay.Visible = true;
            pnlAcousticAlarm.Visible = true;
            SetAlarmPageScroll(false);
            SetDpcTabVisible(false);
            SetLocalParametersVisible(false);
            SetAcousticAlarmColumns(true);
            SetAcousticMarkerStatColumns(true);
            _savedMaxUnit = lblMaxUnit.Text;
            _savedMinUnit = lblMinUnit.Text;
            lblMaxUnit.Text = "dB";
            lblMinUnit.Text = "dB";
            mlvMarkers.Items.Clear();
            UpdatePaletteStrip();
            LayoutAcousticSpectrum(true);
        }

        private void HideAcousticChrome()
        {
            if (!_acousticActive)
                return;

            _acousticDeviceVideoPreview = false;
            SetAcousticLocalDataToolsEnabled(true);
            SetLocalScaleColumnVisible(true);
            if (pnlDeviceScale != null)
                pnlDeviceScale.Visible = false;
            if (tscbAcousticPreview != null && tscbAcousticPreview.SelectedIndex != 0)
            {
                IsGUIRefreshing = true;
                tscbAcousticPreview.SelectedIndex = 0;
                IsGUIRefreshing = false;
            }

            StopAcousticDataStream();
            SetAcousticMaintenanceVisible(true);
            _acousticDataWanted = false;
            LayoutAcousticSpectrum(false);
            _acousticActive = false;
            if (tsbAcousticData != null)
            {
                IsGUIRefreshing = true;
                tsbAcousticData.Checked = false;
                IsGUIRefreshing = false;
            }

            if (_thermalOnlyTools != null)
            {
                foreach (ToolStripItem item in _thermalOnlyTools)
                {
                    if (item != null)
                        item.Visible = true;
                }
            }
            if (_acousticTools != null)
            {
                foreach (ToolStripItem item in _acousticTools)
                    item.Visible = false;
            }

            tsbCalibrate.Enabled = true;
            if (pnlAcousticParams != null)
                pnlAcousticParams.Visible = false;
            if (pnlAcousticOverlay != null)
                pnlAcousticOverlay.Visible = false;
            if (pnlAcousticAlarm != null)
                pnlAcousticAlarm.Visible = false;
            SetAlarmPageScroll(true);
            SetDpcTabVisible(true);
            SetLocalParametersVisible(true);
            SetAcousticAlarmColumns(false);
            SetAcousticMarkerStatColumns(false);
            lblMaxUnit.Text = string.IsNullOrEmpty(_savedMaxUnit) ? "°C" : _savedMaxUnit;
            lblMinUnit.Text = string.IsNullOrEmpty(_savedMinUnit) ? "°C" : _savedMinUnit;
            btnAutoAdjust.Text = "Auto";
        }

        private void SetAcousticMaintenanceVisible(bool visible)
        {
            tbUpgradePath.Visible = visible;
            btnUpgradeBrowse.Visible = visible;
            btnUpgrade.Visible = visible;
            btnDeviceReset.Visible = visible;
            if (visible)
            {
                btnDeviceRestart.Location = new Point(6, 60);
                btnPropertiesGet.Location = new Point(158, 60);
                lvProperties.SetBounds(lvProperties.Left, 90, lvProperties.Width, 400);
            }
            else
            {
                btnDeviceRestart.Location = new Point(6, 33);
                btnPropertiesGet.Location = new Point(82, 33);
                lvProperties.SetBounds(lvProperties.Left, 62, lvProperties.Width, 428);
            }
        }

        private void SetAlarmPageScroll(bool enabled)
        {
            if (tpAlarm == null)
                return;
            if (!enabled)
                tpAlarm.AutoScrollPosition = new Point(0, 0);
            tpAlarm.AutoScroll = enabled;
        }

        private int _localParametersColumnWidth = -1;

        private void SetDpcTabVisible(bool visible)
        {
            bool present = tcThermalFile.TabPages.Contains(tpDpc);
            if (visible)
            {
                if (!present)
                    tcThermalFile.TabPages.Add(tpDpc);
                return;
            }
            if (!present)
                return;
            if (tcThermalFile.SelectedTab == tpDpc)
                tcThermalFile.SelectedTab = tpProperties;
            tcThermalFile.TabPages.Remove(tpDpc);
        }

        private void SetLocalParametersVisible(bool visible)
        {
            if (_localParametersColumnWidth < 0)
                _localParametersColumnWidth = chLocalParameters.Width > 0 ? chLocalParameters.Width : 195;
            chLocalParameters.Width = visible ? _localParametersColumnWidth : 0;
            tsmiLocalParameters.Visible = visible;
        }

        private void SetAcousticAlarmColumns(bool acoustic)
        {
            if (chAcAlarmDb == null)
                return;
            if (_alarmColumnWidth < 0)
                _alarmColumnWidth = chExInfo.Width > 0 ? chExInfo.Width : 72;
            if (acoustic)
            {
                chExInfo.Text = "Alarm (Source)";
                chExInfo.Width = 140;
                chAcAlarmDb.Width = 140;
                chAcAlarmFreq.Width = 160;
                return;
            }
            chExInfo.Text = "Alarm";
            chExInfo.Width = _alarmColumnWidth;
            chAcAlarmDb.Width = 0;
            chAcAlarmFreq.Width = 0;
            _acousticSourceAlarmText = string.Empty;
            _acousticDbAlarmText = string.Empty;
            _acousticFreqAlarmText = string.Empty;
        }

        private void SetAcousticMarkerStatColumns(bool acoustic)
        {
            if (_valueColumnWidth < 0)
                _valueColumnWidth = chValue.Width > 0 ? chValue.Width : 100;
            if (_countPointsColumnWidth < 0)
                _countPointsColumnWidth = chX1.Width > 0 ? chX1.Width : 185;
            chValue.Width = acoustic ? 0 : _valueColumnWidth;
            chX1.Width = acoustic ? 0 : _countPointsColumnWidth;
        }

        private void AppendAcousticPropertyGroups(NetSdkNative.NetStreamInfo stream, bool hasStream)
        {
            if (!_acousticActive)
                return;

            var sensor = NetSdkNative.NetAcousticSensorInfo.Create();
            int ret = NetSdkNative.net_get_acoustic_sensor_info(_netHandle, ref sensor);
            LogApi("net_get_acoustic_sensor_info", ret);
            if (NetSdkNative.IsOk(ret))
                _acousticSensor = sensor;
            else
                sensor = _acousticSensor;

            var sensorGrp = EnsurePropertyGroup("Sensor Info");
            AddPropertyRow(sensorGrp, "Firmware", NetSdkNative.BytesToString(sensor.firmware_version));
            AddPropertyRow(sensorGrp, "Serial No.", NetSdkNative.BytesToString(sensor.serial_no));
            AddPropertyRow(sensorGrp, "Sample Rate", sensor.sample_rate + " Hz");
            AddPropertyRow(sensorGrp, "Mic Count", sensor.mic_count.ToString());
            AddPropertyRow(sensorGrp, "Ref Mic", sensor.ref_mic_index.ToString());
            AddPropertyRow(sensorGrp, "Amplitude", sensor.amplitude.ToString("F3"));
            AddPropertyRow(sensorGrp, "Module Range",
                sensor.freq_range.min.ToString("F0") + "-" + sensor.freq_range.max.ToString("F0") + " Hz");

            var freqGrp = EnsurePropertyGroup("Frequency Range");
            var range = new NetSdkNative.NetAcousticFreqRange();
            ret = NetSdkNative.net_get_acoustic_freq_range(_netHandle, ref range);
            LogApi("net_get_acoustic_freq_range", ret);
            string freqText = NetSdkNative.IsOk(ret)
                ? range.min.ToString("F0") + "-" + range.max.ToString("F0") + " Hz"
                : "—";
            AddPropertyRow(freqGrp, "Frequency Range", freqText);

            int splW;
            int splH;
            int visualW;
            int visualH;
            ReadAcousticImageSize(out splW, out splH, out visualW, out visualH);
            if (visualW <= 0 && hasStream && stream.resolution_width > 0 && stream.resolution_height > 0)
            {
                visualW = stream.resolution_width;
                visualH = stream.resolution_height;
            }
            string frameRate = hasStream && stream.max_frame_rate > 0
                ? stream.max_frame_rate + " fps"
                : "—";

            var sizeGrp = EnsurePropertyGroup("Size");
            AddPropertyRow(sizeGrp, "Acoustic Size", FormatPixelSize(splW, splH));
            AddPropertyRow(sizeGrp, "Visual Size", FormatPixelSize(visualW, visualH));
            AddPropertyRow(sizeGrp, "Frame Rate", frameRate);
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
                if (existing == "Camera Info" || existing == "Sensor Info" ||
                    existing == "Frequency Range" || existing == "Size")
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

        private void ReadAcousticImageSize(out int splW, out int splH, out int visualW, out int visualH)
        {
            splW = 0;
            splH = 0;
            visualW = 0;
            visualH = 0;

            AcousticFrameCopy frame;
            lock (_acousticFrameLock)
                frame = _pendingAcousticFrame ?? _lastAcousticFrame;
            if (frame != null)
            {
                splW = frame.splW;
                splH = frame.splH;
                visualW = frame.visualW;
                visualH = frame.visualH;
            }
            if ((splW > 0 && visualW > 0) || _acousticDataHandle != IntPtr.Zero)
                return;

            var data = new NetSdkNative.NetAcousticData();
            uint types = (uint)(NetSdkNative.NetAcousticDataType.Visual | NetSdkNative.NetAcousticDataType.Spl);
            int ret = NetSdkNative.net_get_acoustic_data(_netHandle, types, ref data);
            LogApi("net_get_acoustic_data", ret, "properties");
            if (!NetSdkNative.IsOk(ret))
                return;
            if (data.spl_frame.width > 0 && data.spl_frame.height > 0)
            {
                splW = data.spl_frame.width;
                splH = data.spl_frame.height;
            }
            if (data.visual_frame.width > 0 && data.visual_frame.height > 0)
            {
                visualW = data.visual_frame.width;
                visualH = data.visual_frame.height;
            }
        }

        private static string FormatPixelSize(int width, int height)
        {
            return width > 0 && height > 0 ? width + "x" + height : "—";
        }

        private void LoadAcousticSettings(bool notify)
        {
            var scale = new NetSdkNative.NetAcousticDbScale();
            int ret = NetSdkNative.net_get_acoustic_db_scale(_netHandle, ref scale);
            LogApi("net_get_acoustic_db_scale", ret);
            if (NetSdkNative.IsOk(ret))
            {
                _acousticScale = scale;
                PushScaleToEditors();
            }
            else if (notify)
            {
                MessageBox.Show("Get acoustic image mode failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var range = new NetSdkNative.NetAcousticFreqRange();
            ret = NetSdkNative.net_get_acoustic_freq_range(_netHandle, ref range);
            LogApi("net_get_acoustic_freq_range", ret);
            if (NetSdkNative.IsOk(ret))
            {
                IsGUIRefreshing = true;
                nudAcousticFreqMin.Value = Clamp((decimal)range.min, nudAcousticFreqMin.Minimum, nudAcousticFreqMin.Maximum);
                nudAcousticFreqMax.Value = Clamp((decimal)range.max, nudAcousticFreqMax.Minimum, nudAcousticFreqMax.Maximum);
                IsGUIRefreshing = false;
            }
            else if (notify)
            {
                MessageBox.Show("Get frequency range failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PushScaleToEditors()
        {
            IsGUIRefreshing = true;
            int index = 0;
            for (int i = 0; i < AcousticModes.Length; i++)
            {
                if (AcousticModes[i] == _acousticScale.mode)
                    index = i;
            }
            tscbAcousticScale.SelectedIndex = index;
            btnAutoAdjust.Text = AcousticModeNames[index];
            nudMax.Value = Clamp((decimal)_acousticScale.max, nudMax.Minimum, nudMax.Maximum);
            nudMin.Value = Clamp((decimal)_acousticScale.min, nudMin.Minimum, nudMin.Maximum);
            cbFixedSpan.Checked = _acousticScale.fixed_span == 0x01;
            decimal span = (decimal)_acousticScale.span;
            if (span < nudSpan.Minimum)
                span = nudSpan.Minimum;
            nudSpan.Value = Clamp(span, nudSpan.Minimum, nudSpan.Maximum);
            nudAcousticSensitivity.Value = Clamp((decimal)_acousticScale.sensitivity, nudAcousticSensitivity.Minimum, nudAcousticSensitivity.Maximum);
            decimal source = _acousticScale.source_size;
            if (source < nudAcousticSourceSize.Minimum)
                source = nudAcousticSourceSize.Minimum;
            nudAcousticSourceSize.Value = Clamp(source, nudAcousticSourceSize.Minimum, nudAcousticSourceSize.Maximum);
            nudAcousticLimit.Value = Clamp((decimal)_acousticScale.limit, nudAcousticLimit.Minimum, nudAcousticLimit.Maximum);
            UpdateAcousticScaleEditors();
            IsGUIRefreshing = false;
            UpdatePaletteStrip();
            CopyEditorsToDevicePanel();
        }

        private void UpdateAcousticScaleEditors()
        {
            bool mono = _acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.AutoMono
                || _acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.AutoMulti;
            bool fixedMode = _acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.Fixed;
            bool holo = _acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.AutoHolo;
            nudMax.Enabled = fixedMode;
            nudMin.Enabled = fixedMode;
            cbFixedSpan.Enabled = holo;
            nudSpan.Enabled = holo && cbFixedSpan.Checked;
            nudAcousticSensitivity.Enabled = mono;
            nudAcousticSourceSize.Enabled = mono;
            ApplyDeviceScaleRows();
        }

        private void ReadScaleFromEditors()
        {
            int index = tscbAcousticScale.SelectedIndex;
            if (index < 0)
                index = 0;
            _acousticScale.mode = AcousticModes[index];
            if (_acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.Fixed)
            {
                _acousticScale.max = (float)nudMax.Value;
                _acousticScale.min = (float)nudMin.Value;
            }
            _acousticScale.fixed_span = (byte)(cbFixedSpan.Checked ? 0x01 : 0x00);
            _acousticScale.span = (float)nudSpan.Value;
            _acousticScale.sensitivity = (float)nudAcousticSensitivity.Value;
            _acousticScale.source_size = (uint)nudAcousticSourceSize.Value;
            _acousticScale.limit = (float)nudAcousticLimit.Value;
        }

        private bool WriteAcousticScale(bool rereadAuto)
        {
            ReadScaleFromEditors();
            var scale = _acousticScale;
            int ret = NetSdkNative.net_set_acoustic_db_scale(_netHandle, ref scale);
            LogApi("net_set_acoustic_db_scale", ret);
            if (!NetSdkNative.IsOk(ret))
                return false;

            if (rereadAuto && scale.mode != NetSdkNative.NetAcousticDbScaleMode.Fixed)
            {
                var updated = new NetSdkNative.NetAcousticDbScale();
                int getRet = NetSdkNative.net_get_acoustic_db_scale(_netHandle, ref updated);
                LogApi("net_get_acoustic_db_scale", getRet);
                if (NetSdkNative.IsOk(getRet))
                {
                    _acousticScale = updated;
                    PushScaleToEditors();
                }
            }
            RenderAcousticAgain();
            return true;
        }

        private void ApplyAcousticParams()
        {
            var range = new NetSdkNative.NetAcousticFreqRange
            {
                min = (float)nudAcousticFreqMin.Value,
                max = (float)nudAcousticFreqMax.Value
            };
            int ret = NetSdkNative.net_set_acoustic_freq_range(_netHandle, ref range);
            LogApi("net_set_acoustic_freq_range", ret);
            bool scaleOk = WriteAcousticScale(true);
            if (NetSdkNative.IsOk(ret))
                LoadProperties();
            if (!NetSdkNative.IsOk(ret) || !scaleOk)
                MessageBox.Show("Apply acoustic parameters failed.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void AcousticScaleModeChanged(object sender, EventArgs e)
        {
            if (!_acousticActive || IsGUIRefreshing)
                return;
            ReadScaleFromEditors();
            UpdateAcousticScaleEditors();
            btnAutoAdjust.Text = AcousticModeNames[tscbAcousticScale.SelectedIndex < 0 ? 0 : tscbAcousticScale.SelectedIndex];
            WriteAcousticScale(true);
        }

        private void CycleAcousticMode()
        {
            int index = tscbAcousticScale.SelectedIndex;
            if (index < 0)
                index = 0;
            tscbAcousticScale.SelectedIndex = (index + 1) % AcousticModes.Length;
        }

        private static readonly byte[,] WebPaletteStops =
        {
            { 255, 36, 2 }, { 251, 71, 0 }, { 255, 115, 2 }, { 255, 147, 4 },
            { 254, 188, 2 }, { 251, 224, 0 }, { 252, 255, 3 }, { 211, 253, 0 },
            { 172, 255, 2 }, { 135, 255, 1 }, { 89, 253, 0 }, { 54, 253, 0 },
            { 17, 255, 1 }, { 1, 253, 24 }, { 0, 253, 58 }, { 0, 254, 88 },
            { 0, 255, 135 }, { 0, 254, 172 }, { 1, 254, 207 }, { 1, 254, 249 },
            { 0, 223, 252 }, { 2, 183, 254 }, { 3, 152, 255 }, { 4, 117, 255 },
            { 1, 67, 255 }, { 1, 34, 254 }, { 0, 0, 253 }, { 17, 0, 254 },
            { 44, 2, 255 }, { 63, 2, 255 }, { 83, 1, 253 }, { 101, 2, 255 },
            { 119, 0, 255 }
        };

        private void AcousticPaletteSizeChanged(object sender, EventArgs e)
        {
            if (_acousticActive)
                UpdatePaletteStrip();
        }

        private void UpdatePaletteStrip()
        {
            int width = pbPalleteImage.ClientSize.Width;
            int height = pbPalleteImage.ClientSize.Height;
            if (width < 2)
                width = 30;
            if (height < 2)
                height = 256;
            int stops = WebPaletteStops.GetLength(0);
            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var bits = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            try
            {
                for (int y = 0; y < height; y++)
                {
                    double t = height == 1 ? 0 : y * (stops - 1) / (double)(height - 1);
                    int index = (int)t;
                    if (index >= stops - 1)
                        index = stops - 2;
                    double blend = t - index;
                    byte r = (byte)Math.Round(WebPaletteStops[index, 0] * (1 - blend) + WebPaletteStops[index + 1, 0] * blend);
                    byte green = (byte)Math.Round(WebPaletteStops[index, 1] * (1 - blend) + WebPaletteStops[index + 1, 1] * blend);
                    byte b = (byte)Math.Round(WebPaletteStops[index, 2] * (1 - blend) + WebPaletteStops[index + 1, 2] * blend);
                    IntPtr row = bits.Scan0 + y * bits.Stride;
                    for (int x = 0; x < width; x++)
                    {
                        Marshal.WriteByte(row, x * 3, b);
                        Marshal.WriteByte(row, x * 3 + 1, green);
                        Marshal.WriteByte(row, x * 3 + 2, r);
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bits);
            }
            var old = pbPalleteImage.Image;
            pbPalleteImage.Image = bmp;
            if (old != null)
                old.Dispose();
        }

        private static uint[] SelectedPaletteColors()
        {
            return AcousticPalettes.REFNO_1;
        }

        private void AcousticDataToggleChanged(object sender, EventArgs e)
        {
            if (!_acousticActive || IsGUIRefreshing || _acousticDeviceVideoPreview)
                return;
            if (tsbAcousticData.Checked)
                StartAcousticDataStream();
            else
                StopAcousticDataStream();
        }

        private void AcousticPreviewModeChanged(object sender, EventArgs e)
        {
            if (!_acousticActive || IsGUIRefreshing || tscbAcousticPreview == null)
                return;
            if (tscbAcousticPreview.SelectedIndex == 1)
                EnterDeviceVideoPreview();
            else
                LeaveDeviceVideoPreview();
        }

        private void EnterDeviceVideoPreview()
        {
            _acousticDeviceVideoPreview = true;
            SetAcousticLocalDataToolsEnabled(false);
            SetLocalScaleColumnVisible(false);
            PauseAcousticDataStream();
            LayoutAcousticSpectrum(false);
            CopyEditorsToDevicePanel();
            ApplyDeviceScaleRows();
            RestartDeviceVideo();
            LogApiCall("[Acoustic] preview=device video");
        }

        private void LeaveDeviceVideoPreview()
        {
            _acousticDeviceVideoPreview = false;
            SetAcousticLocalDataToolsEnabled(true);
            SetLocalScaleColumnVisible(true);
            if (pnlDeviceScale != null)
                pnlDeviceScale.Visible = false;
            LayoutAcousticSpectrum(true);
            if (tsbAcousticData != null && tsbAcousticData.Checked)
                StartAcousticDataStream();
            else if (_lastAcousticFrame != null)
                RenderAcousticFrame(_lastAcousticFrame);
            LogApiCall("[Acoustic] preview=local data");
        }

        private void SetLocalScaleColumnVisible(bool visible)
        {
            pbPalleteImage.Visible = visible;
            nudMax.Visible = visible;
            nudMin.Visible = visible;
            nudSpan.Visible = visible;
            cbFixedSpan.Visible = visible;
            lblMaxUnit.Visible = visible;
            lblMinUnit.Visible = visible;
            btnAutoAdjust.Visible = visible;
        }

        private void BuildDeviceScalePanel()
        {
            nudDeviceMax = CopyScaleBox(nudMax);
            nudDeviceMin = CopyScaleBox(nudMin);
            nudDeviceSpan = CopyScaleBox(nudSpan);
            cbDeviceFixedSpan = new CheckBox { Text = "Fixed Span", AutoSize = true };
            nudDeviceMax.ValueChanged += DeviceScaleEdited;
            nudDeviceMin.ValueChanged += DeviceScaleEdited;
            nudDeviceSpan.ValueChanged += DeviceScaleEdited;
            cbDeviceFixedSpan.CheckedChanged += DeviceScaleEdited;

            pnlDeviceFixed = new Panel { Dock = DockStyle.Fill, Visible = false };
            AddDeviceRow(pnlDeviceFixed, 4, "Max", nudDeviceMax);
            AddDeviceRow(pnlDeviceFixed, 32, "Min", nudDeviceMin);
            pnlDeviceHolo = new Panel { Dock = DockStyle.Fill, Visible = false };
            cbDeviceFixedSpan.Left = 4;
            cbDeviceFixedSpan.Top = 6;
            pnlDeviceHolo.Controls.Add(cbDeviceFixedSpan);
            AddDeviceRow(pnlDeviceHolo, 32, "Span", nudDeviceSpan);

            pnlDeviceScale = new Panel
            {
                Visible = false,
                Height = 64,
                BackColor = SystemColors.Control,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pnlDeviceScale.Controls.Add(pnlDeviceFixed);
            pnlDeviceScale.Controls.Add(pnlDeviceHolo);
            Controls.Add(pnlDeviceScale);
        }

        private static NumericUpDown CopyScaleBox(NumericUpDown source)
        {
            return new NumericUpDown
            {
                DecimalPlaces = source.DecimalPlaces,
                Increment = source.Increment,
                Minimum = source.Minimum,
                Maximum = source.Maximum,
                Value = Clamp(source.Value, source.Minimum, source.Maximum),
                Width = 64
            };
        }

        private static void AddDeviceRow(Panel panel, int top, string caption, NumericUpDown value)
        {
            panel.Controls.Add(new Label { Text = caption, AutoSize = true, Left = 4, Top = top + 4 });
            value.Left = 40;
            value.Top = top;
            panel.Controls.Add(value);
            panel.Controls.Add(new Label { Text = "dB", AutoSize = true, Left = 108, Top = top + 4 });
        }

        private void PositionDeviceScalePanel()
        {
            if (pnlDeviceScale == null || pbPalleteImage == null)
                return;
            int width = 132;
            if (tcThermalFile != null)
                width = tcThermalFile.Left - pbPalleteImage.Left - 6;
            if (width < 120)
                width = 120;
            pnlDeviceScale.SetBounds(pbPalleteImage.Left, pbPalleteImage.Top, width, 64);
        }

        private void CopyEditorsToDevicePanel()
        {
            if (nudDeviceMax == null)
                return;
            _syncingDeviceScale = true;
            nudDeviceMax.Value = Clamp(nudMax.Value, nudDeviceMax.Minimum, nudDeviceMax.Maximum);
            nudDeviceMin.Value = Clamp(nudMin.Value, nudDeviceMin.Minimum, nudDeviceMin.Maximum);
            cbDeviceFixedSpan.Checked = cbFixedSpan.Checked;
            nudDeviceSpan.Value = Clamp(nudSpan.Value, nudDeviceSpan.Minimum, nudDeviceSpan.Maximum);
            _syncingDeviceScale = false;
        }

        private void DeviceScaleEdited(object sender, EventArgs e)
        {
            if (_syncingDeviceScale || IsGUIRefreshing || !_acousticDeviceVideoPreview || !_netConnected)
                return;
            _syncingDeviceScale = true;
            IsGUIRefreshing = true;
            nudMax.Value = Clamp(nudDeviceMax.Value, nudMax.Minimum, nudMax.Maximum);
            nudMin.Value = Clamp(nudDeviceMin.Value, nudMin.Minimum, nudMin.Maximum);
            cbFixedSpan.Checked = cbDeviceFixedSpan.Checked;
            nudSpan.Value = Clamp(nudDeviceSpan.Value, nudSpan.Minimum, nudSpan.Maximum);
            IsGUIRefreshing = false;
            _syncingDeviceScale = false;
            if (nudDeviceSpan != null)
                nudDeviceSpan.Enabled = _acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.AutoHolo
                    && cbDeviceFixedSpan.Checked;
            WriteAcousticScale(false);
        }

        private void ApplyDeviceScaleRows()
        {
            if (pnlDeviceScale == null)
                return;
            bool fixedMode = _acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.Fixed;
            bool holo = _acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.AutoHolo;
            bool show = _acousticDeviceVideoPreview && _acousticActive && (fixedMode || holo);
            pnlDeviceFixed.Visible = show && fixedMode;
            pnlDeviceHolo.Visible = show && holo;
            if (nudDeviceSpan != null && cbDeviceFixedSpan != null)
                nudDeviceSpan.Enabled = holo && cbDeviceFixedSpan.Checked;
            pnlDeviceScale.Visible = show;
            if (!show)
                return;
            PositionDeviceScalePanel();
            pnlDeviceScale.BringToFront();
        }

        private void SetAcousticLocalDataToolsEnabled(bool enabled)
        {
            if (_acousticLocalDataTools == null)
                return;
            foreach (ToolStripItem item in _acousticLocalDataTools)
            {
                if (item != null)
                    item.Enabled = enabled;
            }
        }

        private void PauseAcousticDataStream()
        {
            if (_acousticDataHandle == IntPtr.Zero)
                return;

            NetSdkNative.OnNetAcousticDataStreamReceivedCallback callback = _acousticDataCb;
            int ret = NetSdkNative.net_stop_acoustic_data_stream(_acousticDataHandle);
            LogAcousticApi("net_stop_acoustic_data_stream", ret);
            _acousticDataHandle = IntPtr.Zero;
            _acousticDataCb = null;
            GC.KeepAlive(callback);
        }

        private void RestartDeviceVideo()
        {
            if (!_netConnected)
                return;
            if (_currentStream.id == 0)
                StartVideoStream();
            else
                StartVideoStreamCallback(_currentStreamType, true);
        }

        private void AcousticStreamOptionChanged(object sender, EventArgs e)
        {
            if (!_acousticActive || IsGUIRefreshing || _acousticDeviceVideoPreview || !tsbAcousticData.Checked)
                return;
            StartAcousticDataStream();
        }

        private NetSdkNative.NetAcousticDataType SelectedAcousticTypes()
        {
            NetSdkNative.NetAcousticDataType types = 0;
            if (tsbAcousticVisual.Checked)
                types |= NetSdkNative.NetAcousticDataType.Visual;
            if (tsbAcousticSpl.Checked)
                types |= NetSdkNative.NetAcousticDataType.Spl;
            if (tsbAcousticSpectrum.Checked)
                types |= NetSdkNative.NetAcousticDataType.Spectrum;
            if (tsbAcousticGlobal.Checked)
                types |= NetSdkNative.NetAcousticDataType.Global;
            return types;
        }

        private uint SelectedAcousticInterval()
        {
            uint interval;
            if (!uint.TryParse(tstbAcousticInterval.Text.Trim(), out interval) || interval < 100)
                interval = 100;
            return interval;
        }

        private int StartAcousticDataStream()
        {
            bool useSavedConfig = _acousticDataWanted && InvokeRequired;
            StopAcousticDataStream();
            if (_netHandle == IntPtr.Zero)
                return (int)NetSdkNative.NetErrorCode.NotInitialized;

            if (!useSavedConfig)
            {
                _acousticStreamConfig = new NetSdkNative.NetAcousticDataConfig
                {
                    data_types = SelectedAcousticTypes(),
                    sample_interval = SelectedAcousticInterval()
                };
            }

            _acousticDataCb = OnAcousticDataReceived;
            int ret = NetSdkNative.net_start_acoustic_data_stream(_netHandle, ref _acousticStreamConfig, _acousticDataCb);
            LogAcousticApi(
                "net_start_acoustic_data_stream types=0x" + ((uint)_acousticStreamConfig.data_types).ToString("X")
                + " interval=" + _acousticStreamConfig.sample_interval,
                ret);
            _acousticPayloadSignature = null;
            _acousticPayloadLogCount = 0;
            if (!NetSdkNative.TryGetSdkHandle(ret, out _acousticDataHandle))
            {
                _acousticDataHandle = IntPtr.Zero;
                _acousticDataCb = null;
                _acousticDataWanted = false;
                ReportAcousticStreamStartFailed(ret);
                return ret;
            }

            _acousticDataWanted = true;
            SetAcousticRefreshEnabled(false);
            return ret;
        }

        private void StopAcousticDataStream()
        {
            _acousticDataWanted = false;
            SetAcousticRefreshEnabled(true);
            if (_acousticDataHandle == IntPtr.Zero)
                return;

            NetSdkNative.OnNetAcousticDataStreamReceivedCallback callback = _acousticDataCb;
            int ret = NetSdkNative.net_stop_acoustic_data_stream(_acousticDataHandle);
            LogAcousticApi("net_stop_acoustic_data_stream", ret);
            _acousticDataHandle = IntPtr.Zero;
            _acousticDataCb = null;
            GC.KeepAlive(callback);
        }

        private int RestartAcousticDataStreamForReconnect()
        {
            if (!_acousticDataWanted)
                return 0;
            return StartAcousticDataStream();
        }

        private void SetAcousticRefreshEnabled(bool enabled)
        {
            if (tsbAcousticRefresh == null)
                return;
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetAcousticRefreshEnabled(enabled)));
                return;
            }

            tsbAcousticRefresh.Enabled = enabled;
        }

        private void ReportAcousticStreamStartFailed(int ret)
        {
            Action apply = () =>
            {
                if (tsbAcousticRefresh != null)
                    tsbAcousticRefresh.Enabled = true;
                if (tsbAcousticData != null && tsbAcousticData.Checked)
                {
                    IsGUIRefreshing = true;
                    tsbAcousticData.Checked = false;
                    IsGUIRefreshing = false;
                }
                if (!IsDisposed)
                {
                    MessageBox.Show(
                        "Start acoustic data stream failed: " + ret + " (" + (NetSdkNative.NetErrorCode)ret + ")",
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            };

            if (InvokeRequired)
                BeginInvoke(apply);
            else
                apply();
        }

        private void LogAcousticApi(string apiName, int ret)
        {
            string text = apiName + " → " + ret + " (" + NetSdkNative.FormatReturnStatus(ret) + ")";
            if (InvokeRequired)
                LogSafe("[Acoustic] " + text);
            else
                LogApi(apiName, ret);
        }

        private void AcousticRefreshClick(object sender, EventArgs e)
        {
            if (!_acousticActive || _acousticDeviceVideoPreview || _netHandle == IntPtr.Zero || _acousticDataHandle != IntPtr.Zero)
            {
                WriteAcousticRefreshNote("skipped active=" + _acousticActive
                    + " login=" + _netHandle.ToInt64().ToString("X")
                    + " stream=" + _acousticDataHandle.ToInt64().ToString("X"));
                return;
            }
            var data = new NetSdkNative.NetAcousticData();
            uint types = (uint)SelectedAcousticTypes();
            int ret = NetSdkNative.net_get_acoustic_data(_netHandle, types, ref data);
            LogApi("net_get_acoustic_data", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                WriteAcousticRefreshNote("net_get_acoustic_data failed ret=" + ret + " types=0x" + types.ToString("X"));
                MessageBox.Show("Get acoustic data failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            WriteAcousticRefreshSpectrum(data);
            RenderAcousticFrame(CopyAcousticFrame(data));
        }

        private static string AcousticRefreshDumpPath()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "acoustic-spectrum-refresh.txt");
        }

        private static void WriteAcousticRefreshNote(string text)
        {
            System.IO.File.WriteAllText(AcousticRefreshDumpPath(), text + Environment.NewLine);
        }

        private void WriteAcousticRefreshSpectrum(NetSdkNative.NetAcousticData data)
        {
            IntPtr ptr = data.spectrum_frame.data;
            uint bins = data.spectrum_frame.bin_count;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("types=0x" + ((uint)data.data_types).ToString("X"));
            sb.AppendLine("bin_count=" + bins);
            sb.AppendLine("data=0x" + ptr.ToInt64().ToString("X"));
            int copied = 0;
            if (ptr != IntPtr.Zero && bins > 0 && bins <= 1000000)
            {
                copied = (int)bins;
                float[] samples = new float[copied];
                Marshal.Copy(ptr, samples, 0, copied);
                for (int i = 0; i < copied; i++)
                    sb.AppendLine(samples[i].ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            }
            System.IO.File.WriteAllText(AcousticRefreshDumpPath(), sb.ToString());
            LogApiCall("[AcousticRefresh] types=0x" + ((uint)data.data_types).ToString("X")
                + " bin_count=" + bins
                + " data=0x" + ptr.ToInt64().ToString("X")
                + " copied=" + copied);
        }

        private void NoteAcousticPayload(NetSdkNative.NetAcousticData data)
        {
            string signature = ((uint)data.data_types).ToString("X")
                + ":" + data.visual_frame.width + "x" + data.visual_frame.height
                + ":" + data.visual_frame.points_count
                + ":" + (data.visual_frame.data != IntPtr.Zero ? 1 : 0)
                + ":" + data.spl_frame.width + "x" + data.spl_frame.height
                + ":" + data.spl_frame.points_count
                + ":" + (data.spl_frame.data != IntPtr.Zero ? 1 : 0)
                + ":" + data.spectrum_frame.bin_count
                + ":" + (data.spectrum_frame.data != IntPtr.Zero ? 1 : 0);
            if (signature == _acousticPayloadSignature || _acousticPayloadLogCount >= 8)
                return;
            _acousticPayloadSignature = signature;
            _acousticPayloadLogCount++;
            string text = "[AcousticStream] types=0x" + ((uint)data.data_types).ToString("X")
                + " visual=" + data.visual_frame.width + "x" + data.visual_frame.height
                + " bytes=" + data.visual_frame.points_count
                + " ptr=" + (data.visual_frame.data != IntPtr.Zero ? "yes" : "no")
                + " spl=" + data.spl_frame.width + "x" + data.spl_frame.height
                + " points=" + data.spl_frame.points_count
                + " ptr=" + (data.spl_frame.data != IntPtr.Zero ? "yes" : "no")
                + " spectrum=" + data.spectrum_frame.bin_count
                + " ptr=" + (data.spectrum_frame.data != IntPtr.Zero ? "yes" : "no")
                + " globalMax=" + data.global_spl_info.max.db.ToString("F1");
            try
            {
                if (IsHandleCreated && !IsDisposed)
                    BeginInvoke(new Action(() => LogApiCall(text)));
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void OnAcousticDataReceived(IntPtr framePtr)
        {
            try
            {
                if (framePtr == IntPtr.Zero)
                    return;
                var data = (NetSdkNative.NetAcousticData)Marshal.PtrToStructure(framePtr, typeof(NetSdkNative.NetAcousticData));
                NoteAcousticPayload(data);
                AcousticFrameCopy copy = CopyAcousticFrame(data);
                if (!_acousticDataWanted || !IsHandleCreated || IsDisposed)
                    return;
                lock (_acousticFrameLock)
                    _pendingAcousticFrame = copy;
                BeginInvoke(new Action(RenderPendingAcousticFrame));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("acoustic data: " + ex.Message);
            }
        }

        private void RenderPendingAcousticFrame()
        {
            AcousticFrameCopy frame;
            lock (_acousticFrameLock)
            {
                frame = _pendingAcousticFrame;
                _pendingAcousticFrame = null;
            }
            if (frame != null && _acousticActive && _acousticDataWanted && !_acousticDeviceVideoPreview)
                RenderAcousticFrame(frame);
        }

        private void RenderAcousticAgain()
        {
            AcousticFrameCopy frame;
            lock (_acousticFrameLock)
                frame = _pendingAcousticFrame;
            if (frame == null)
                frame = _lastAcousticFrame;
            if (frame != null)
                RenderAcousticFrame(frame);
        }

        private AcousticFrameCopy _lastAcousticFrame;

        private static AcousticFrameCopy CopyAcousticFrame(NetSdkNative.NetAcousticData data)
        {
            var copy = new AcousticFrameCopy();
            if ((data.data_types & NetSdkNative.NetAcousticDataType.Visual) != 0
                && data.visual_frame.data != IntPtr.Zero
                && data.visual_frame.width > 0
                && data.visual_frame.height > 0)
            {
                int bytes = (int)data.visual_frame.points_count;
                int expected = data.visual_frame.width * data.visual_frame.height * 3;
                if (bytes <= 0)
                    bytes = expected;
                if (bytes > expected)
                    bytes = expected;
                copy.visual = new byte[bytes];
                Marshal.Copy(data.visual_frame.data, copy.visual, 0, bytes);
                copy.visualW = data.visual_frame.width;
                copy.visualH = data.visual_frame.height;
            }
            if ((data.data_types & NetSdkNative.NetAcousticDataType.Spl) != 0 && data.spl_frame.data != IntPtr.Zero)
            {
                int count = (int)data.spl_frame.points_count;
                if (count <= 0)
                    count = data.spl_frame.width * data.spl_frame.height;
                if (count > 0)
                {
                    copy.spl = new float[count];
                    Marshal.Copy(data.spl_frame.data, copy.spl, 0, count);
                    copy.splW = data.spl_frame.width;
                    copy.splH = data.spl_frame.height;
                }
            }
            if ((data.data_types & NetSdkNative.NetAcousticDataType.Spectrum) != 0
                && data.spectrum_frame.data != IntPtr.Zero
                && data.spectrum_frame.bin_count > 0)
            {
                int count = (int)data.spectrum_frame.bin_count;
                copy.spectrum = new float[count];
                Marshal.Copy(data.spectrum_frame.data, copy.spectrum, 0, count);
            }
            if ((data.data_types & NetSdkNative.NetAcousticDataType.Global) != 0)
            {
                copy.hasGlobal = true;
                copy.maxDb = data.global_spl_info.max.db;
                copy.minDb = data.global_spl_info.min.db;
                copy.avgDb = data.global_spl_info.avg_db;
                copy.maxX = data.global_spl_info.max.point.x;
                copy.maxY = data.global_spl_info.max.point.y;
                copy.minX = data.global_spl_info.min.point.x;
                copy.minY = data.global_spl_info.min.point.y;
            }
            return copy;
        }

        private void RenderAcousticFrame(AcousticFrameCopy frame)
        {
            if (_acousticDeviceVideoPreview)
                return;
            _lastAcousticFrame = frame;
            Bitmap visual = null;
            Bitmap cloud = null;
            bool showCloud = false;
            float imageMax = frame.hasGlobal ? frame.maxDb : _acousticScale.max;
            float imageMin = frame.hasGlobal ? frame.minDb : _acousticScale.min;
            AcousticUtilsNative.AcousticSummary summary = AcousticUtilsNative.AcousticSummary.Create();
            try
            {
                if (frame.visual != null && frame.visualW > 0 && frame.visualH > 0)
                    visual = CreateBgrBitmap(frame.visual, frame.visualW, frame.visualH);
                if (frame.spl != null && frame.splW > 0 && frame.splH > 0)
                    showCloud = TryBuildCloud(frame, out cloud, out summary, out imageMax, out imageMin);
            }
            catch (DllNotFoundException)
            {
                if (!_acousticUtilsWarned)
                {
                    _acousticUtilsWarned = true;
                    MessageBox.Show("AcousticUtils.dll was not found. Build the x86 demo so the DLL is copied beside the program.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                LogApiCall("Acoustic render failed: " + ex.Message);
            }

            var canvas = new Bitmap(AcousticViewWidth, AcousticViewHeight, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.FromArgb(32, 32, 32));
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                if (visual != null)
                    g.DrawImage(visual, new Rectangle(0, 0, AcousticViewWidth, AcousticViewHeight));
                if (showCloud && cloud != null)
                    g.DrawImage(cloud, new Rectangle(0, 0, AcousticViewWidth, AcousticViewHeight));

                float? measuredDb = null;
                if (frame.hasGlobal)
                    measuredDb = frame.maxDb;
                else if (showCloud)
                    measuredDb = summary.max.value;
                DrawMaxFreqLabel(g, frame, measuredDb);
            }
            if (visual != null)
                visual.Dispose();
            if (cloud != null)
                cloud.Dispose();

            var old = pbThermalImage.Image;
            pbThermalImage.Image = canvas;
            if (old != null)
                old.Dispose();

            if (showCloud)
                ShowLiveScale(imageMax, imageMin);
            if (frame.hasGlobal)
                UpdateAcousticGlobalRow(frame);
            RenderAcousticSpectrum(frame.spectrum);
        }

        private void ShowLiveScale(float max, float min)
        {
            if (_acousticScale.mode == NetSdkNative.NetAcousticDbScaleMode.Fixed || IsGUIRefreshing)
                return;
            decimal maxValue = Clamp((decimal)max, nudMax.Minimum, nudMax.Maximum);
            decimal minValue = Clamp((decimal)min, nudMin.Minimum, nudMin.Maximum);
            if (nudMax.Value == maxValue && nudMin.Value == minValue)
                return;
            IsGUIRefreshing = true;
            nudMax.Value = maxValue;
            nudMin.Value = minValue;
            IsGUIRefreshing = false;
        }

        private bool TryBuildCloud(AcousticFrameCopy frame, out Bitmap cloud, out AcousticUtilsNative.AcousticSummary summary, out float imageMax, out float imageMin)
        {
            cloud = null;
            summary = AcousticUtilsNative.AcousticSummary.Create();
            imageMax = _acousticScale.max;
            imageMin = _acousticScale.min;
            int bytes = frame.splW * frame.splH * 4;
            IntPtr buffer = Marshal.AllocHGlobal(bytes);
            var pin = GCHandle.Alloc(frame.spl, GCHandleType.Pinned);
            try
            {
                var grid = new AcousticUtilsNative.GridSpl
                {
                    width = frame.splW,
                    height = frame.splH,
                    spl = pin.AddrOfPinnedObject()
                };
                int sourceSize = (int)_acousticScale.source_size;
                if (sourceSize < 1)
                    sourceSize = 1;
                if (sourceSize > 10)
                    sourceSize = 10;
                float sensitivity = _acousticScale.sensitivity;
                if (sensitivity <= 0f || sensitivity > 10f)
                    sensitivity = 3.2f;
                var db = new AcousticUtilsNative.DbScaleV2
                {
                    mode = (int)_acousticScale.mode,
                    max = _acousticScale.max,
                    min = _acousticScale.min,
                    fixed_span = _acousticScale.fixed_span,
                    span = _acousticScale.span,
                    sensitivity = sensitivity,
                    source_size = sourceSize,
                    limit = _acousticScale.limit
                };
                var focus = new AcousticUtilsNative.FocusMode
                {
                    enabled = 0,
                    scope = 0.5f
                };
                var palette = new AcousticUtilsNative.Palette
                {
                    ref_no = 1,
                    colors = (uint[])SelectedPaletteColors().Clone(),
                    alpha = 160
                };
                byte isShow = 0;
                int ret = AcousticUtilsNative.acoustic_get_spl_image_v2(
                    ref grid, ref db, ref focus, ref palette, AcousticUtilsNative.Argb, 1f, buffer, ref summary, ref isShow);
                if (ret != 0)
                {
                    LogApiCall("acoustic_get_spl_image_v2 → " + ret);
                    return false;
                }
                imageMax = db.max;
                imageMin = db.min;
                if (isShow == 0)
                    return false;

                cloud = new Bitmap(frame.splW, frame.splH, PixelFormat.Format32bppArgb);
                BitmapData locked = cloud.LockBits(new Rectangle(0, 0, frame.splW, frame.splH), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                int row = frame.splW * 4;
                for (int y = 0; y < frame.splH; y++)
                {
                    byte[] rowBytes = new byte[row];
                    Marshal.Copy(IntPtr.Add(buffer, y * row), rowBytes, 0, row);
                    Marshal.Copy(rowBytes, 0, IntPtr.Add(locked.Scan0, y * locked.Stride), row);
                }
                cloud.UnlockBits(locked);
                return true;
            }
            finally
            {
                if (pin.IsAllocated)
                    pin.Free();
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static Bitmap CreateBgrBitmap(byte[] bgr, int width, int height)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int row = width * 3;
            for (int y = 0; y < height; y++)
            {
                int src = y * row;
                if (src + row > bgr.Length)
                    break;
                Marshal.Copy(bgr, src, IntPtr.Add(data.Scan0, y * data.Stride), row);
            }
            bmp.UnlockBits(data);
            return bmp;
        }

        private Font OverlayFont
        {
            get { return _acousticOverlayFont ?? Font; }
        }

        private void DrawMaxFreqLabel(Graphics g, AcousticFrameCopy frame, float? measuredDb)
        {
            bool hasSpectrum = frame.spectrum != null && frame.spectrum.Length > 0;
            if (!measuredDb.HasValue && !hasSpectrum)
                return;

            string detail = measuredDb.HasValue ? measuredDb.Value.ToString("F1") + " dB" : string.Empty;
            if (hasSpectrum)
            {
                int peak = 0;
                for (int i = 1; i < frame.spectrum.Length; i++)
                {
                    if (frame.spectrum[i] > frame.spectrum[peak])
                        peak = i;
                }
                int bins = frame.spectrum.Length;
                double hz = bins <= 1
                    ? SpectrumAxisMinHz
                    : SpectrumAxisMinHz + peak * (SpectrumAxisMaxHz - SpectrumAxisMinHz) / (bins - 1);
                if (detail.Length > 0)
                    detail += "   ";
                detail += hz.ToString("F0") + " Hz";
            }
            using (var text = new SolidBrush(Color.White))
            {
                g.DrawString("max freq", OverlayFont, text, 8, 4);
                g.DrawString(detail, OverlayFont, text, 8, 18);
            }
        }

        private double SpectrumAxisMinHz
        {
            get
            {
                double minHz;
                double maxHz;
                ReadModuleFreqRange(out minHz, out maxHz);
                return minHz;
            }
        }

        private double SpectrumAxisMaxHz
        {
            get
            {
                double minHz;
                double maxHz;
                ReadModuleFreqRange(out minHz, out maxHz);
                return maxHz;
            }
        }

        private void ReadModuleFreqRange(out double minHz, out double maxHz)
        {
            minHz = _acousticSensor.freq_range.min;
            maxHz = _acousticSensor.freq_range.max;
            if (maxHz < minHz)
            {
                double swap = minHz;
                minHz = maxHz;
                maxHz = swap;
            }
        }

        private float SpectrumFreqToX(double hz, int width)
        {
            double span = SpectrumAxisMaxHz - SpectrumAxisMinHz;
            if (span <= 0)
                return 0;
            double t = (hz - SpectrumAxisMinHz) / span;
            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;
            return (float)(t * Math.Max(1, width - 1));
        }

        private void LayoutAcousticSpectrum(bool show)
        {
            if (pbAcousticSpectrum == null)
                return;
            if (!show)
            {
                pbAcousticSpectrum.Visible = false;
                if (pbAcousticSpectrum.Image != null)
                {
                    Image oldImage = pbAcousticSpectrum.Image;
                    pbAcousticSpectrum.Image = null;
                    oldImage.Dispose();
                }
                if (_acousticLayoutDelta != 0)
                {
                    int restore = _acousticLayoutDelta;
                    _acousticLayoutDelta = 0;
                    pbThermalImage.Height += restore;
                    pbPalleteImage.Height += restore;
                    nudMin.Top += restore;
                    lblMinUnit.Top += restore;
                    btnAutoAdjust.Top += restore;
                }
                return;
            }

            if (_acousticLayoutDelta == 0)
            {
                int delta = AcousticSpectrumHeight;
                if (pbThermalImage.Height - delta < 200)
                    delta = Math.Max(0, pbThermalImage.Height - 200);
                _acousticLayoutDelta = delta;
                pbThermalImage.Height -= delta;
                pbPalleteImage.Height -= delta;
                nudMin.Top -= delta;
                lblMinUnit.Top -= delta;
                btnAutoAdjust.Top -= delta;
            }

            pbAcousticSpectrum.Visible = true;
            pbAcousticSpectrum.BringToFront();
            PositionAcousticSpectrum();
        }

        private bool _spectrumLayoutQueued;

        private void AcousticPreviewLayoutChanged(object sender, EventArgs e)
        {
            if (_acousticDeviceVideoPreview)
                PositionDeviceScalePanel();
            QueueAcousticSpectrumLayout();
        }

        private void QueueAcousticSpectrumLayout()
        {
            if (!_acousticActive || pbAcousticSpectrum == null || !pbAcousticSpectrum.Visible || IsDisposed)
                return;
            if (!IsHandleCreated)
                return;
            if (_spectrumLayoutQueued)
                return;
            _spectrumLayoutQueued = true;
            BeginInvoke(new Action(ApplyQueuedAcousticSpectrumLayout));
        }

        private void ApplyQueuedAcousticSpectrumLayout()
        {
            _spectrumLayoutQueued = false;
            if (!_acousticActive || pbAcousticSpectrum == null || !pbAcousticSpectrum.Visible || IsDisposed)
                return;
            PositionAcousticSpectrum();
        }

        private Rectangle AcousticPreviewDisplayRect()
        {
            int boxW = pbThermalImage.ClientSize.Width;
            int boxH = pbThermalImage.ClientSize.Height;
            if (boxW < 2 || boxH < 2)
                return pbThermalImage.Bounds;
            float ratio = Math.Min(boxW / (float)AcousticViewWidth, boxH / (float)AcousticViewHeight);
            int w = Math.Max(1, (int)(AcousticViewWidth * ratio));
            int h = Math.Max(1, (int)(AcousticViewHeight * ratio));
            int x = pbThermalImage.Left + (pbThermalImage.Width - boxW) / 2 + (boxW - w) / 2;
            int y = pbThermalImage.Top + (pbThermalImage.Height - boxH) / 2 + (boxH - h) / 2;
            return new Rectangle(x, y, w, h);
        }

        private void PositionAcousticSpectrum()
        {
            if (pbAcousticSpectrum == null || pbThermalImage == null || mlvMarkers == null)
                return;
            Rectangle shown = AcousticPreviewDisplayRect();
            int imageBottom = Math.Max(pbThermalImage.Bottom, shown.Bottom);
            int gapBottom = mlvMarkers.Top - 4;
            int top = gapBottom - AcousticSpectrumHeight;
            if (top < imageBottom + 6)
                top = imageBottom + 6;
            var bounds = new Rectangle(pbThermalImage.Left, top, pbThermalImage.Width, AcousticSpectrumHeight);
            Rectangle plot = SpectrumPlotRect(bounds, shown);
            const AnchorStyles spectrumAnchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            bool placed = pbAcousticSpectrum.Bounds != bounds
                || pbAcousticSpectrum.Anchor != spectrumAnchor
                || _spectrumPlotX != plot.X
                || _spectrumPlotW != plot.Width;
            if (!placed)
                return;
            _spectrumPlotX = plot.X;
            _spectrumPlotW = plot.Width;
            pbAcousticSpectrum.Bounds = bounds;
            pbAcousticSpectrum.Anchor = spectrumAnchor;
            pbAcousticSpectrum.BringToFront();
            float[] spectrum = _lastAcousticFrame == null ? null : _lastAcousticFrame.spectrum;
            RenderAcousticSpectrum(spectrum);
        }

        private int _spectrumPlotX = int.MinValue;
        private int _spectrumPlotW;

        private Rectangle SpectrumPlotRect(Rectangle spectrumBounds, Rectangle shown)
        {
            int border = Math.Max(0, (pbThermalImage.Width - pbThermalImage.ClientSize.Width) / 2);
            int x = shown.Left - spectrumBounds.Left - border;
            int w = shown.Width;
            int clientW = Math.Max(1, spectrumBounds.Width - border * 2);
            if (x < 0)
            {
                w += x;
                x = 0;
            }
            if (x + w > clientW)
                w = clientW - x;
            if (w < 1)
                w = 1;
            return new Rectangle(x, 0, w, 0);
        }

        private void RenderAcousticSpectrum(float[] spectrum)
        {
            if (pbAcousticSpectrum == null || !pbAcousticSpectrum.Visible)
                return;
            int width = pbAcousticSpectrum.ClientSize.Width;
            int height = pbAcousticSpectrum.ClientSize.Height;
            if (width < 2 || height < 2)
                return;
            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bmp))
            {
                Color outside = pbAcousticSpectrum.Parent == null
                    ? SystemColors.Control
                    : pbAcousticSpectrum.Parent.BackColor;
                g.Clear(outside);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                int plotX = _spectrumPlotX;
                int plotW = _spectrumPlotW;
                if (plotX < 0 || plotW < 2 || plotX >= width)
                {
                    plotX = 0;
                    plotW = width;
                }
                if (plotX + plotW > width)
                    plotW = width - plotX;
                g.FillRectangle(Brushes.Black, plotX, 0, plotW, height);
                g.TranslateTransform(plotX, 0);
                DrawSpectrum(g, spectrum, plotW, height);
            }
            Image previous = pbAcousticSpectrum.Image;
            pbAcousticSpectrum.Image = bmp;
            if (previous != null)
                previous.Dispose();
        }

        private bool TryGetFrequencyRange(out float minHz, out float maxHz)
        {
            minHz = 0;
            maxHz = 0;
            if (nudAcousticFreqMin == null || nudAcousticFreqMax == null)
                return false;
            minHz = (float)nudAcousticFreqMin.Value;
            maxHz = (float)nudAcousticFreqMax.Value;
            if (maxHz < minHz)
            {
                float swap = minHz;
                minHz = maxHz;
                maxHz = swap;
            }
            return maxHz > minHz;
        }

        private Color SpectrumColor(double unitFromBottom)
        {
            if (unitFromBottom < 0)
                unitFromBottom = 0;
            if (unitFromBottom > 1)
                unitFromBottom = 1;
            int stops = WebPaletteStops.GetLength(0);
            double t = (1.0 - unitFromBottom) * (stops - 1);
            int index = (int)t;
            if (index < 0)
                index = 0;
            if (index >= stops - 1)
                index = stops - 2;
            double blend = t - index;
            int r = (int)Math.Round(WebPaletteStops[index, 0] * (1 - blend) + WebPaletteStops[index + 1, 0] * blend);
            int green = (int)Math.Round(WebPaletteStops[index, 1] * (1 - blend) + WebPaletteStops[index + 1, 1] * blend);
            int b = (int)Math.Round(WebPaletteStops[index, 2] * (1 - blend) + WebPaletteStops[index + 1, 2] * blend);
            return Color.FromArgb(r, green, b);
        }

        private void DrawSpectrum(Graphics g, float[] spectrum, int viewWidth, int viewHeight)
        {
            const int labelHeight = 16;
            int plotTop = labelHeight;
            int plotHeight = viewHeight - plotTop - 2;
            if (plotHeight < 8)
                return;

            float rangeMin;
            float rangeMax;
            bool hasRange = TryGetFrequencyRange(out rangeMin, out rangeMax);
            float rangeX0 = 0;
            float rangeX1 = viewWidth - 1;
            if (hasRange)
            {
                rangeX0 = SpectrumFreqToX(rangeMin, viewWidth);
                rangeX1 = SpectrumFreqToX(rangeMax, viewWidth);
            }

            if (spectrum != null && spectrum.Length > 0)
            {
                float min = spectrum[0];
                float max = spectrum[0];
                for (int i = 1; i < spectrum.Length; i++)
                {
                    if (spectrum[i] < min)
                        min = spectrum[i];
                    if (spectrum[i] > max)
                        max = spectrum[i];
                }
                float span = max - min;
                if (span < 0.001f)
                    span = 0.001f;
                float slot = viewWidth / (float)spectrum.Length;
                int bottom = plotTop + plotHeight - 1;
                for (int i = 0; i < spectrum.Length; i++)
                {
                    float unit = (spectrum[i] - min) / span;
                    if (unit < 0f)
                        unit = 0f;
                    if (unit > 1f)
                        unit = 1f;
                    int drawHeight = Math.Max(1, (int)Math.Ceiling(plotHeight * unit));
                    float x = i * slot;
                    float barWidth = Math.Max(1f, slot);
                    for (int py = 0; py < drawHeight; py++)
                    {
                        double level = (py + 0.5) / plotHeight;
                        using (var brush = new SolidBrush(SpectrumColor(level)))
                            g.FillRectangle(brush, x, bottom - py, barWidth, 1f);
                    }
                }
            }

            if (hasRange)
            {
                using (var edge = new Pen(Color.White, 2))
                {
                    g.DrawLine(edge, rangeX0, plotTop, rangeX0, viewHeight - 2);
                    g.DrawLine(edge, rangeX1, plotTop, rangeX1, viewHeight - 2);
                    g.DrawLine(edge, rangeX0, plotTop, rangeX1, plotTop);
                }
            }

            int[] ticks = BuildSpectrumTicks();
            float labelRight = 0;
            using (var tickPen = new Pen(Color.White, 2))
            using (var text = new SolidBrush(Color.White))
            using (var plate = new SolidBrush(Color.FromArgb(230, 0, 0, 0)))
            {
                foreach (int hz in ticks)
                {
                    if (hz < SpectrumAxisMinHz || hz > SpectrumAxisMaxHz)
                        continue;
                    float x = SpectrumFreqToX(hz, viewWidth);
                    bool axisStart = hz == (int)Math.Round(SpectrumAxisMinHz);
                    float tickX = x;
                    if (tickX < 1f)
                        tickX = 1f;
                    if (tickX > viewWidth - 2)
                        tickX = viewWidth - 2;
                    g.DrawLine(tickPen, tickX, axisStart ? plotTop : 1, tickX, plotTop + 8);
                    string label = FormatSpectrumKhz(hz);
                    SizeF size = g.MeasureString(label, OverlayFont);
                    float textX = x - size.Width / 2f;
                    if (axisStart)
                        textX = tickX - tickPen.Width / 2f - GlyphLeft(g, label, OverlayFont);
                    if (textX < 1f && !axisStart)
                        textX = 1f;
                    if (textX < labelRight && !axisStart)
                        textX = labelRight;
                    if (textX + size.Width > viewWidth)
                        textX = viewWidth - size.Width;
                    g.FillRectangle(plate, Math.Max(0f, textX), 0, size.Width, labelHeight - 1);
                    if (axisStart)
                        g.DrawString(label, OverlayFont, text, textX, 0, StringFormat.GenericTypographic);
                    else
                        g.DrawString(label, OverlayFont, text, textX, 0);
                    labelRight = textX + size.Width;
                }
            }

            if (hasRange)
            {
                DrawSpectrumRangeLabel(g, FormatSpectrumKhz(rangeMin), rangeX0, viewWidth);
                DrawSpectrumRangeLabel(g, FormatSpectrumKhz(rangeMax), rangeX1, viewWidth);
            }
        }

        private int[] BuildSpectrumTicks()
        {
            double minHz = SpectrumAxisMinHz;
            double maxHz = SpectrumAxisMaxHz;
            double span = maxHz - minHz;
            if (span <= 0)
                return new int[0];
            double step = 1000;
            if (span > 80000)
                step = 20000;
            else if (span > 40000)
                step = 10000;
            else if (span > 20000)
                step = 5000;
            else if (span > 8000)
                step = 2000;
            var ticks = new System.Collections.Generic.List<int>();
            ticks.Add((int)Math.Round(minHz));
            double start = Math.Ceiling((minHz + 1) / step) * step;
            for (double hz = start; hz < maxHz - step * 0.25; hz += step)
                ticks.Add((int)Math.Round(hz));
            int end = (int)Math.Round(maxHz);
            if (ticks[ticks.Count - 1] != end)
                ticks.Add(end);
            return ticks.ToArray();
        }

        private static float GlyphLeft(Graphics g, string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;
            using (StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone())
            {
                format.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, 1) });
                Region[] regions = g.MeasureCharacterRanges(text, font, new RectangleF(0, 0, 200, 40), format);
                try
                {
                    return regions[0].GetBounds(g).X;
                }
                finally
                {
                    foreach (Region region in regions)
                        region.Dispose();
                }
            }
        }

        private static string FormatSpectrumKhz(float hz)
        {
            float khz = hz / 1000f;
            float rounded = (float)Math.Round(khz);
            if (Math.Abs(khz - rounded) < 0.05f)
                return ((int)rounded) + "k";
            return khz.ToString("0.0") + "k";
        }

        private void DrawSpectrumRangeLabel(Graphics g, string label, float x, int viewWidth)
        {
            SizeF size = g.MeasureString(label, OverlayFont);
            float textX = x - size.Width / 2f;
            if (textX < 1f)
                textX = 1f;
            if (textX + size.Width > viewWidth)
                textX = viewWidth - size.Width;
            using (var plate = new SolidBrush(Color.FromArgb(230, 0, 0, 0)))
            using (var text = new SolidBrush(Color.Yellow))
                g.FillRectangle(plate, textX, 0, size.Width, 15);
            using (var text = new SolidBrush(Color.Yellow))
                g.DrawString(label, OverlayFont, text, textX, 0);
        }

        private void UpdateAcousticGlobalRow(AcousticFrameCopy frame)
        {
            while (mlvMarkers.Items.Count < 1)
            {
                var created = new ListViewItem("Global");
                while (created.SubItems.Count < mlvMarkers.Columns.Count)
                    created.SubItems.Add(string.Empty);
                mlvMarkers.Items.Add(created);
            }
            ListViewItem item = mlvMarkers.Items[0];
            while (item.SubItems.Count < mlvMarkers.Columns.Count)
                item.SubItems.Add(string.Empty);
            item.SubItems[0].Text = "Global";
            item.SubItems[1].Text = string.Empty;
            item.SubItems[2].Text = FormatAcousticSplPoint(frame.minDb, frame.minX, frame.minY);
            item.SubItems[3].Text = FormatAcousticSplPoint(frame.maxDb, frame.maxX, frame.maxY);
            item.SubItems[4].Text = frame.avgDb.ToString("F1");
            item.SubItems[5].Text = string.Empty;
            item.SubItems[7].Text = _acousticSourceAlarmText;
            item.SubItems[8].Text = _acousticDbAlarmText;
            item.SubItems[9].Text = _acousticFreqAlarmText;
        }

        private void LoadAcousticOverlay(bool notify)
        {
            var overlay = new NetSdkNative.NetAcousticOverlayInfo();
            int ret = NetSdkNative.net_get_acoustic_overlay(_netHandle, ref overlay);
            LogApi("net_get_acoustic_overlay", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                if (notify)
                    MessageBox.Show("Get acoustic overlay failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            cbAcPalEnabled.Checked = overlay.palette_overlay.enabled != 0;
            cbAcPalAlign.SelectedItem = overlay.palette_overlay.align;
            cbAcPalMode.SelectedItem = overlay.palette_overlay.mode;
            nudAcPalX.Value = Clamp(overlay.palette_overlay.x, 0, 1000000);
            nudAcPalY.Value = Clamp(overlay.palette_overlay.y, 0, 1000000);
            cbAcSpecEnabled.Checked = overlay.spectrum_overlay.enabled != 0;
            cbAcSpecAlign.SelectedItem = overlay.spectrum_overlay.align;
            cbAcSpecMode.SelectedItem = overlay.spectrum_overlay.mode;
            nudAcSpecX.Value = Clamp(overlay.spectrum_overlay.x, 0, 1000000);
            nudAcSpecY.Value = Clamp(overlay.spectrum_overlay.y, 0, 1000000);
            cbAcSplEnabled.Checked = overlay.spl_overlay.enabled != 0;
            cbAcSplAlign.SelectedItem = overlay.spl_overlay.align;
            nudAcSplX.Value = Clamp(overlay.spl_overlay.x, 0, 1000000);
            nudAcSplY.Value = Clamp(overlay.spl_overlay.y, 0, 1000000);
            cbAcSplMaxDb.Checked = overlay.spl_overlay.max_db_enabled != 0;
            cbAcSplMaxFreq.Checked = overlay.spl_overlay.max_freq_enabled != 0;
        }

        private void ApplyAcousticOverlay()
        {
            var overlay = new NetSdkNative.NetAcousticOverlayInfo();
            overlay.palette_overlay = ReadBlock(cbAcPalEnabled, cbAcPalAlign, cbAcPalMode, nudAcPalX, nudAcPalY);
            overlay.spectrum_overlay = ReadBlock(cbAcSpecEnabled, cbAcSpecAlign, cbAcSpecMode, nudAcSpecX, nudAcSpecY);
            overlay.spl_overlay = new NetSdkNative.NetOverlayAcousticBlock
            {
                enabled = (byte)(cbAcSplEnabled.Checked ? 1 : 0),
                align = (NetSdkNative.NetOverlayAlign)cbAcSplAlign.SelectedItem,
                x = (uint)nudAcSplX.Value,
                y = (uint)nudAcSplY.Value,
                max_db_enabled = (byte)(cbAcSplMaxDb.Checked ? 1 : 0),
                max_freq_enabled = (byte)(cbAcSplMaxFreq.Checked ? 1 : 0)
            };
            int ret = NetSdkNative.net_set_acoustic_overlay(_netHandle, ref overlay);
            LogApi("net_set_acoustic_overlay", ret);
            if (!NetSdkNative.IsOk(ret))
                MessageBox.Show("Set acoustic overlay failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static NetSdkNative.NetOverlayBlock ReadBlock(CheckBox enabled, ComboBox align, ComboBox mode, NumericUpDown x, NumericUpDown y)
        {
            return new NetSdkNative.NetOverlayBlock
            {
                enabled = (byte)(enabled.Checked ? 1 : 0),
                align = (NetSdkNative.NetOverlayAlign)align.SelectedItem,
                mode = (NetSdkNative.NetOverlayDisplayMode)mode.SelectedItem,
                x = (uint)x.Value,
                y = (uint)y.Value
            };
        }

        private void LoadAcousticAlarms(bool notify)
        {
            var source = new NetSdkNative.NetAcousticSourceAlarm();
            int ret = NetSdkNative.net_get_acoustic_source_alarm(_netHandle, ref source);
            LogApi("net_get_acoustic_source_alarm", ret);
            if (NetSdkNative.IsOk(ret))
            {
                cbAcSourceEnabled.Checked = source.enabled != 0;
                nudAcSourceDuration.Value = Clamp(source.duration, 0, 1000000);
                nudAcSourceTolerance.Value = Clamp(source.tolerance_pixels, 0, 1000000);
                nudAcSourceNoRepeat.Value = Clamp(source.no_repeat_minutes, 0, 1000000);
            }
            else if (notify)
            {
                MessageBox.Show("Get source alarm failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var db = new NetSdkNative.NetAcousticDbAlarm();
            ret = NetSdkNative.net_get_acoustic_db_alarm(_netHandle, ref db);
            LogApi("net_get_acoustic_db_alarm", ret);
            if (NetSdkNative.IsOk(ret))
            {
                cbAcDbEnabled.Checked = db.enabled != 0;
                nudAcDbThreshold.Value = Clamp((decimal)db.db_threshold, nudAcDbThreshold.Minimum, nudAcDbThreshold.Maximum);
                nudAcDbNoRepeat.Value = Clamp(db.no_repeat_minutes, 0, 1000000);
            }

            var freq = new NetSdkNative.NetAcousticFreqAlarm();
            ret = NetSdkNative.net_get_acoustic_freq_alarm(_netHandle, ref freq);
            LogApi("net_get_acoustic_freq_alarm", ret);
            if (NetSdkNative.IsOk(ret))
            {
                cbAcFreqEnabled.Checked = freq.enabled != 0;
                nudAcFreqFrequency.Value = Clamp((decimal)freq.frequency, nudAcFreqFrequency.Minimum, nudAcFreqFrequency.Maximum);
                nudAcFreqThreshold.Value = Clamp((decimal)freq.db_threshold, nudAcFreqThreshold.Minimum, nudAcFreqThreshold.Maximum);
                nudAcFreqNoRepeat.Value = Clamp(freq.no_repeat_minutes, 0, 1000000);
            }

            LoadAcousticTrigger(NetSdkNative.NetAlarmType.AcousticSource, cbAcSourceFtp, cbAcSourceWebhook, notify);
            LoadAcousticTrigger(NetSdkNative.NetAlarmType.AcousticDb, cbAcDbFtp, cbAcDbWebhook, notify);
            LoadAcousticTrigger(NetSdkNative.NetAlarmType.AcousticFreq, cbAcFreqFtp, cbAcFreqWebhook, notify);
        }

        private void LoadAcousticTrigger(NetSdkNative.NetAlarmType type, CheckBox ftp, CheckBox webhook, bool notify)
        {
            var trigger = new NetSdkNative.NetAlarmTrigger();
            var trigType = type;
            int ret = NetSdkNative.net_get_alarm_trigger(_netHandle, ref trigType, ref trigger);
            LogApi("net_get_alarm_trigger", ret, type.ToString());
            if (NetSdkNative.IsOk(ret))
            {
                ftp.Checked = trigger.trigger_ftp_enabled != 0;
                webhook.Checked = trigger.trigger_webhook_enabled != 0;
            }
            else if (notify)
            {
                MessageBox.Show("Get " + type + " trigger failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ApplyAcousticTrigger(NetSdkNative.NetAlarmType type, CheckBox ftp, CheckBox webhook)
        {
            var trigger = new NetSdkNative.NetAlarmTrigger
            {
                trigger_ftp_enabled = (byte)(ftp.Checked ? 1 : 0),
                trigger_webhook_enabled = (byte)(webhook.Checked ? 1 : 0)
            };
            var trigType = type;
            int ret = NetSdkNative.net_set_alarm_trigger(_netHandle, ref trigType, ref trigger);
            LogApi("net_set_alarm_trigger", ret, type.ToString());
            if (!NetSdkNative.IsOk(ret))
                MessageBox.Show("Set " + type + " trigger failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ApplySourceAlarm()
        {
            var alarm = new NetSdkNative.NetAcousticSourceAlarm
            {
                enabled = (byte)(cbAcSourceEnabled.Checked ? 1 : 0),
                duration = (uint)nudAcSourceDuration.Value,
                tolerance_pixels = (uint)nudAcSourceTolerance.Value,
                no_repeat_minutes = (uint)nudAcSourceNoRepeat.Value
            };
            int ret = NetSdkNative.net_set_acoustic_source_alarm(_netHandle, ref alarm);
            LogApi("net_set_acoustic_source_alarm", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("Set source alarm failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ApplyAcousticTrigger(NetSdkNative.NetAlarmType.AcousticSource, cbAcSourceFtp, cbAcSourceWebhook);
        }

        private void ApplyDbAlarm()
        {
            var alarm = new NetSdkNative.NetAcousticDbAlarm
            {
                enabled = (byte)(cbAcDbEnabled.Checked ? 1 : 0),
                db_threshold = (float)nudAcDbThreshold.Value,
                no_repeat_minutes = (uint)nudAcDbNoRepeat.Value
            };
            int ret = NetSdkNative.net_set_acoustic_db_alarm(_netHandle, ref alarm);
            LogApi("net_set_acoustic_db_alarm", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("Set decibel alarm failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ApplyAcousticTrigger(NetSdkNative.NetAlarmType.AcousticDb, cbAcDbFtp, cbAcDbWebhook);
        }

        private void ApplyFreqAlarm()
        {
            var alarm = new NetSdkNative.NetAcousticFreqAlarm
            {
                enabled = (byte)(cbAcFreqEnabled.Checked ? 1 : 0),
                frequency = (float)nudAcFreqFrequency.Value,
                db_threshold = (float)nudAcFreqThreshold.Value,
                no_repeat_minutes = (uint)nudAcFreqNoRepeat.Value
            };
            int ret = NetSdkNative.net_set_acoustic_freq_alarm(_netHandle, ref alarm);
            LogApi("net_set_acoustic_freq_alarm", ret);
            if (!NetSdkNative.IsOk(ret))
            {
                MessageBox.Show("Set frequency alarm failed: " + ret, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ApplyAcousticTrigger(NetSdkNative.NetAlarmType.AcousticFreq, cbAcFreqFtp, cbAcFreqWebhook);
        }

        private void DoAcousticCapture()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "IRTEK Acoustic Image (*.jpg)|*.jpg|JPEG Image (*.jpg)|*.jpg";
                sfd.DefaultExt = "jpg";
                sfd.FilterIndex = 1;
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    Cursor = Cursors.WaitCursor;
                    var captureFile = NetSdkNative.NetCaptureFile.Create();
                    byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(sfd.FileName);
                    Array.Copy(pathBytes, captureFile.local_file, Math.Min(pathBytes.Length, NetSdkNative.NET_LEN_128 - 1));
                    var mode = sfd.FilterIndex == 1
                        ? NetSdkNative.NetAcousticCaptureMode.IrtekImage
                        : NetSdkNative.NetAcousticCaptureMode.PureImage;
                    int ret = NetSdkNative.net_get_acoustic_capture(_netHandle, mode, ref captureFile);
                    LogApi("net_get_acoustic_capture", ret);
                    if (NetSdkNative.IsOk(ret))
                        MessageBox.Show("Saved: " + sfd.FileName, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Capture failed: " + ret + " (" + (NetSdkNative.NetErrorCode)ret + ")", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void HandleAcousticAlarm(NetSdkNative.NetAlarmInfo alarmInfo)
        {
            int column = 7;
            string text = "type=" + alarmInfo.type + " id=" + alarmInfo.alarm_id;
            if (alarmInfo.alarm_info != IntPtr.Zero)
            {
                if (alarmInfo.type == NetSdkNative.NetAlarmType.AcousticSource)
                {
                    var info = (NetSdkNative.NetAlarmAcousticSourceInfo)Marshal.PtrToStructure(
                        alarmInfo.alarm_info, typeof(NetSdkNative.NetAlarmAcousticSourceInfo));
                    text = "Source id=" + alarmInfo.alarm_id
                        + " (" + info.point.x + "," + info.point.y + ") "
                        + NetSdkNative.ReadUtf8(info.message, info.message_length);
                }
                else if (alarmInfo.type == NetSdkNative.NetAlarmType.AcousticDb)
                {
                    column = 8;
                    var info = (NetSdkNative.NetAlarmAcousticDbInfo)Marshal.PtrToStructure(
                        alarmInfo.alarm_info, typeof(NetSdkNative.NetAlarmAcousticDbInfo));
                    text = "dB id=" + alarmInfo.alarm_id + " "
                        + info.db.ToString("F1") + " / " + info.db_threshold.ToString("F1") + " "
                        + NetSdkNative.ReadUtf8(info.message, info.message_length);
                }
                else if (alarmInfo.type == NetSdkNative.NetAlarmType.AcousticFreq)
                {
                    column = 9;
                    var info = (NetSdkNative.NetAlarmAcousticFreqInfo)Marshal.PtrToStructure(
                        alarmInfo.alarm_info, typeof(NetSdkNative.NetAlarmAcousticFreqInfo));
                    text = "Freq id=" + alarmInfo.alarm_id + " "
                        + info.frequency.ToString("F0") + " Hz " + info.db.ToString("F1") + " dB "
                        + NetSdkNative.ReadUtf8(info.message, info.message_length);
                }
            }

            int capturedColumn = column;
            string captured = text;
            if (!IsHandleCreated || IsDisposed)
                return;
            BeginInvoke(new Action(() =>
            {
                if (capturedColumn == 8)
                    _acousticDbAlarmText = captured;
                else if (capturedColumn == 9)
                    _acousticFreqAlarmText = captured;
                else
                    _acousticSourceAlarmText = captured;
                LogApiCall("[Alarm] " + captured);
                if (mlvMarkers.Items.Count > 0 && mlvMarkers.Items[0].SubItems.Count > capturedColumn)
                    mlvMarkers.Items[0].SubItems[capturedColumn].Text = captured;
            }));
        }

        private sealed class AcousticFrameCopy
        {
            public byte[] visual;
            public int visualW;
            public int visualH;
            public float[] spl;
            public int splW;
            public int splH;
            public float[] spectrum;
            public bool hasGlobal;
            public float maxDb;
            public float minDb;
            public float avgDb;
            public int maxX;
            public int maxY;
            public int minX;
            public int minY;
        }

        private static string FormatAcousticSplPoint(float db, int x, int y)
        {
            return db.ToString("F1") + "(" + x + "," + y + ")";
        }
    }
}
