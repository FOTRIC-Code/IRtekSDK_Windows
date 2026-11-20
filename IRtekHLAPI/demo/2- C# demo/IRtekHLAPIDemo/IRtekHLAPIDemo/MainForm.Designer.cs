
namespace IRtekHLAPIDemo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Camera Info", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Lens Info", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Temp. Range", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Video", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Geo Location", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Image Enhancement", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("File Info", System.Windows.Forms.HorizontalAlignment.Left);
            this.tsConnection = new System.Windows.Forms.ToolStrip();
            this.tslIP = new System.Windows.Forms.ToolStripLabel();
            this.tstbIP = new System.Windows.Forms.ToolStripTextBox();
            this.tslPort = new System.Windows.Forms.ToolStripLabel();
            this.tstbPort = new System.Windows.Forms.ToolStripTextBox();
            this.tslUser = new System.Windows.Forms.ToolStripLabel();
            this.tstbUser = new System.Windows.Forms.ToolStripTextBox();
            this.tslPassword = new System.Windows.Forms.ToolStripLabel();
            this.tstbPassword = new System.Windows.Forms.ToolStripTextBox();
            this.tsbConnect = new System.Windows.Forms.ToolStripButton();
            this.tsActionSpacer = new System.Windows.Forms.ToolStripSeparator();
            this.tsMainForm = new System.Windows.Forms.ToolStrip();
            this.tsbCapture = new System.Windows.Forms.ToolStripButton();
            this.tsbGetThermalData = new System.Windows.Forms.ToolStripButton();
            this.tsbGetThermalLut = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.tslThermalStreamConfig = new System.Windows.Forms.ToolStripLabel();
            this.tscbThermalStreamConfig = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tslFileFormat = new System.Windows.Forms.ToolStripLabel();
            this.tscbFileFormat = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSelect = new System.Windows.Forms.ToolStripButton();
            this.tsbSpot = new System.Windows.Forms.ToolStripButton();
            this.tsbRectangle = new System.Windows.Forms.ToolStripButton();
            this.tsbLine = new System.Windows.Forms.ToolStripButton();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tslPalette = new System.Windows.Forms.ToolStripLabel();
            this.tsbPalette = new System.Windows.Forms.ToolStripButton();
            this.tscbPalette = new System.Windows.Forms.ToolStripComboBox();
            this.tsbInverted = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tslColorDistributionType = new System.Windows.Forms.ToolStripLabel();
            this.tscbColorDistributionType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tslImageFlip = new System.Windows.Forms.ToolStripLabel();
            this.tscbImageFlip = new System.Windows.Forms.ToolStripComboBox();
            this.tsbCalibrate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.tslStreamId = new System.Windows.Forms.ToolStripLabel();
            this.tscbStreamId = new System.Windows.Forms.ToolStripComboBox();
            this.pbThermalImage = new System.Windows.Forms.PictureBox();
            this.tcThermalFile = new System.Windows.Forms.TabControl();
            this.tpProperties = new System.Windows.Forms.TabPage();
            this.tbDeviceName = new System.Windows.Forms.TextBox();
            this.btnSetDeviceName = new System.Windows.Forms.Button();
            this.lvProperties = new System.Windows.Forms.ListView();
            this.chPropertyKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPropertyValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbUpgradePath = new System.Windows.Forms.TextBox();
            this.btnUpgradeBrowse = new System.Windows.Forms.Button();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.btnDeviceReset = new System.Windows.Forms.Button();
            this.btnDeviceRestart = new System.Windows.Forms.Button();
            this.btnPropertiesGet = new System.Windows.Forms.Button();
            this.tpThermalParams = new System.Windows.Forms.TabPage();
            this.btnThermalParamsGet = new System.Windows.Forms.Button();
            this.btnThermalParamsSet = new System.Windows.Forms.Button();
            this.lblRelHumidityUnit = new System.Windows.Forms.Label();
            this.lblAtmTempUnit = new System.Windows.Forms.Label();
            this.lblDistanceUnit = new System.Windows.Forms.Label();
            this.lblReflTempUnit = new System.Windows.Forms.Label();
            this.nudRelHumidity = new System.Windows.Forms.NumericUpDown();
            this.nudAtmTemp = new System.Windows.Forms.NumericUpDown();
            this.nudDistance = new System.Windows.Forms.NumericUpDown();
            this.nudReflTemp = new System.Windows.Forms.NumericUpDown();
            this.nudEmissivity = new System.Windows.Forms.NumericUpDown();
            this.lblDistance = new System.Windows.Forms.Label();
            this.lblRelHumidity = new System.Windows.Forms.Label();
            this.lblAtmTemp = new System.Windows.Forms.Label();
            this.lblReflTemp = new System.Windows.Forms.Label();
            this.lblEmissivity = new System.Windows.Forms.Label();
            this.lblShutterMode = new System.Windows.Forms.Label();
            this.cbShutterMode = new System.Windows.Forms.ComboBox();
            this.lblShutterTime = new System.Windows.Forms.Label();
            this.nudShutterTime = new System.Windows.Forms.NumericUpDown();
            this.lblTempUnit = new System.Windows.Forms.Label();
            this.cbTempUnit = new System.Windows.Forms.ComboBox();
            this.lblDistUnit = new System.Windows.Forms.Label();
            this.cbDistUnit = new System.Windows.Forms.ComboBox();
            this.tpTime = new System.Windows.Forms.TabPage();
            this.lblTimeZone = new System.Windows.Forms.Label();
            this.cbTimeZone = new System.Windows.Forms.ComboBox();
            this.lblTimeMode = new System.Windows.Forms.Label();
            this.cbTimeMode = new System.Windows.Forms.ComboBox();
            this.lblLocalTime = new System.Windows.Forms.Label();
            this.tbLocalTime = new System.Windows.Forms.TextBox();
            this.btnGetTime = new System.Windows.Forms.Button();
            this.btnSyncTime = new System.Windows.Forms.Button();
            this.btnApplyTime = new System.Windows.Forms.Button();
            this.lblNtpHost = new System.Windows.Forms.Label();
            this.tbNtpHost = new System.Windows.Forms.TextBox();
            this.lblNtpPort = new System.Windows.Forms.Label();
            this.nudNtpPort = new System.Windows.Forms.NumericUpDown();
            this.lblNtpInterval = new System.Windows.Forms.Label();
            this.nudNtpInterval = new System.Windows.Forms.NumericUpDown();
            this.btnNtpApply = new System.Windows.Forms.Button();
            this.tpNetwork = new System.Windows.Forms.TabPage();
            this.lblNetDhcp = new System.Windows.Forms.Label();
            this.cbNetDhcp = new System.Windows.Forms.CheckBox();
            this.lblNetIp = new System.Windows.Forms.Label();
            this.tbNetIp = new System.Windows.Forms.TextBox();
            this.lblNetMask = new System.Windows.Forms.Label();
            this.tbNetMask = new System.Windows.Forms.TextBox();
            this.lblNetGateway = new System.Windows.Forms.Label();
            this.tbNetGateway = new System.Windows.Forms.TextBox();
            this.lblNetDns = new System.Windows.Forms.Label();
            this.tbNetDns = new System.Windows.Forms.TextBox();
            this.btnNetApply = new System.Windows.Forms.Button();
            this.btnNetRefresh = new System.Windows.Forms.Button();
            this.lblFtpServer = new System.Windows.Forms.Label();
            this.tbFtpServer = new System.Windows.Forms.TextBox();
            this.lblFtpPort = new System.Windows.Forms.Label();
            this.nudFtpPort = new System.Windows.Forms.NumericUpDown();
            this.lblFtpUser = new System.Windows.Forms.Label();
            this.tbFtpUser = new System.Windows.Forms.TextBox();
            this.lblFtpPwd = new System.Windows.Forms.Label();
            this.tbFtpPwd = new System.Windows.Forms.TextBox();
            this.cbFtpSsl = new System.Windows.Forms.CheckBox();
            this.cbFtpAnonymous = new System.Windows.Forms.CheckBox();
            this.lblFtpPath = new System.Windows.Forms.Label();
            this.tbFtpPath = new System.Windows.Forms.TextBox();
            this.btnFtpApply = new System.Windows.Forms.Button();
            this.btnFtpTest = new System.Windows.Forms.Button();
            this.lblHttpPort = new System.Windows.Forms.Label();
            this.nudHttpPort = new System.Windows.Forms.NumericUpDown();
            this.lblRtspPort = new System.Windows.Forms.Label();
            this.nudRtspPort = new System.Windows.Forms.NumericUpDown();
            this.lblRtpPort = new System.Windows.Forms.Label();
            this.nudRtpPort = new System.Windows.Forms.NumericUpDown();
            this.btnPortApply = new System.Windows.Forms.Button();
            this.tpUser = new System.Windows.Forms.TabPage();
            this.lvUsers = new System.Windows.Forms.ListView();
            this.chUserId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chUserName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chUserLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblUserName = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.lblUserPwd = new System.Windows.Forms.Label();
            this.tbUserPwd = new System.Windows.Forms.TextBox();
            this.lblUserLevel = new System.Windows.Forms.Label();
            this.cbUserLevel = new System.Windows.Forms.ComboBox();
            this.lblUserLoginPwd = new System.Windows.Forms.Label();
            this.tbUserLoginPwd = new System.Windows.Forms.TextBox();
            this.btnUserRefresh = new System.Windows.Forms.Button();
            this.btnUserAdd = new System.Windows.Forms.Button();
            this.btnUserUpdate = new System.Windows.Forms.Button();
            this.btnUserDelete = new System.Windows.Forms.Button();
            this.tpDisplay = new System.Windows.Forms.TabPage();
            this.lblChannelName = new System.Windows.Forms.Label();
            this.tbChannelName = new System.Windows.Forms.TextBox();
            this.btnSetChannelName = new System.Windows.Forms.Button();
            this.cbChNameEnabled = new System.Windows.Forms.CheckBox();
            this.cbChNameAlign = new System.Windows.Forms.ComboBox();
            this.tbChNameX = new System.Windows.Forms.TrackBar();
            this.lblChNameXVal = new System.Windows.Forms.Label();
            this.tbChNameY = new System.Windows.Forms.TrackBar();
            this.lblChNameYVal = new System.Windows.Forms.Label();
            this.cbTimeEnabled = new System.Windows.Forms.CheckBox();
            this.cbTimeAlign = new System.Windows.Forms.ComboBox();
            this.tbTimeX = new System.Windows.Forms.TrackBar();
            this.lblTimeXVal = new System.Windows.Forms.Label();
            this.tbTimeY = new System.Windows.Forms.TrackBar();
            this.lblTimeYVal = new System.Windows.Forms.Label();
            this.cbCustomEnabled = new System.Windows.Forms.CheckBox();
            this.cbCustomAlign = new System.Windows.Forms.ComboBox();
            this.cbCustomMode = new System.Windows.Forms.ComboBox();
            this.lblCustomText = new System.Windows.Forms.Label();
            this.tbCustomText = new System.Windows.Forms.TextBox();
            this.tbCustomX = new System.Windows.Forms.TrackBar();
            this.lblCustomXVal = new System.Windows.Forms.Label();
            this.tbCustomY = new System.Windows.Forms.TrackBar();
            this.lblCustomYVal = new System.Windows.Forms.Label();
            this.tpThermalOsd = new System.Windows.Forms.TabPage();
            this.thermalOsdPanel = new IRtekHLAPIDemo.ThermalOsdPanel();
            this.lblContrast = new System.Windows.Forms.Label();
            this.nudContrast = new System.Windows.Forms.NumericUpDown();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.nudBrightness = new System.Windows.Forms.NumericUpDown();
            this.cbDdeEnable = new System.Windows.Forms.CheckBox();
            this.lblDdeValue = new System.Windows.Forms.Label();
            this.nudDdeValue = new System.Windows.Forms.NumericUpDown();
            this.btnDisplayGet = new System.Windows.Forms.Button();
            this.btnDisplaySet = new System.Windows.Forms.Button();
            this.tpAlarm = new System.Windows.Forms.TabPage();
            this.lvAlarms = new System.Windows.Forms.ListView();
            this.chAlarmMarker = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmTemp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmDelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmThreshTemp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmThreshDelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmTrigTemp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlarmTrigDelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblAlarmMarker = new System.Windows.Forms.Label();
            this.tbAlarmMarker = new System.Windows.Forms.TextBox();
            this.cbAlarmEnabled = new System.Windows.Forms.CheckBox();
            this.lblAlarmType = new System.Windows.Forms.Label();
            this.cbAlarmType = new System.Windows.Forms.ComboBox();
            this.lblAlarmTemp = new System.Windows.Forms.Label();
            this.nudAlarmTemp = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmTempUnit = new System.Windows.Forms.Label();
            this.lblAlarmDelay = new System.Windows.Forms.Label();
            this.nudAlarmDelay = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmDelayUnit = new System.Windows.Forms.Label();
            this.lblAlarmThreshTemp = new System.Windows.Forms.Label();
            this.nudAlarmThreshTemp = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmThreshTempUnit = new System.Windows.Forms.Label();
            this.lblAlarmThreshDelay = new System.Windows.Forms.Label();
            this.nudAlarmThreshDelay = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmThreshDelayUnit = new System.Windows.Forms.Label();
            this.lblAlarmTrigTemp = new System.Windows.Forms.Label();
            this.nudAlarmTrigTemp = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmTrigTempUnit = new System.Windows.Forms.Label();
            this.lblAlarmTrigDelay = new System.Windows.Forms.Label();
            this.nudAlarmTrigDelay = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmTrigDelayUnit = new System.Windows.Forms.Label();
            this.btnAlarmApply = new System.Windows.Forms.Button();
            this.lblAlarmInterval = new System.Windows.Forms.Label();
            this.nudAlarmInterval = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmIntervalUnit = new System.Windows.Forms.Label();
            this.cbAlarmFtpEnabled = new System.Windows.Forms.CheckBox();
            this.btnAlarmGet = new System.Windows.Forms.Button();
            this.btnAlarmSet = new System.Windows.Forms.Button();
            this.tpDpc = new System.Windows.Forms.TabPage();
            this.lblDpcResolution = new System.Windows.Forms.Label();
            this.lblDpcHint = new System.Windows.Forms.Label();
            this.lvDpcPoints = new System.Windows.Forms.ListView();
            this.chDpcIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDpcX = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDpcY = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblDpcX = new System.Windows.Forms.Label();
            this.nudDpcX = new System.Windows.Forms.NumericUpDown();
            this.lblDpcY = new System.Windows.Forms.Label();
            this.nudDpcY = new System.Windows.Forms.NumericUpDown();
            this.btnDpcAdd = new System.Windows.Forms.Button();
            this.btnDpcRemove = new System.Windows.Forms.Button();
            this.btnDpcClear = new System.Windows.Forms.Button();
            this.btnDpcSet = new System.Windows.Forms.Button();
            this.cmsMarkers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLocalParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeltaParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportTemperature = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportTemperatureCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.ttTempValue = new System.Windows.Forms.ToolTip(this.components);
            this.btnAutoAdjust = new System.Windows.Forms.Button();
            this.pbPalleteImage = new System.Windows.Forms.PictureBox();
            this.nudMax = new System.Windows.Forms.NumericUpDown();
            this.nudMin = new System.Windows.Forms.NumericUpDown();
            this.nudSpan = new System.Windows.Forms.NumericUpDown();
            this.cbFixedSpan = new System.Windows.Forms.CheckBox();
            this.lblMaxUnit = new System.Windows.Forms.Label();
            this.lblMinUnit = new System.Windows.Forms.Label();
            this.tPlay = new System.Windows.Forms.Timer(this.components);
            this.mlvMarkers = new System.Windows.Forms.ListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAverage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chX1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLocalParameters = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbTimeZone = new System.Windows.Forms.TextBox();
            this.tsConnection.SuspendLayout();
            this.tsMainForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThermalImage)).BeginInit();
            this.tcThermalFile.SuspendLayout();
            this.tpProperties.SuspendLayout();
            this.tpThermalParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRelHumidity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAtmTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShutterTime)).BeginInit();
            this.tpTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNtpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNtpInterval)).BeginInit();
            this.tpNetwork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFtpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHttpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRtspPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRtpPort)).BeginInit();
            this.tpUser.SuspendLayout();
            this.tpDisplay.SuspendLayout();
            this.tpThermalOsd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbChNameX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbChNameY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbCustomX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbCustomY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDdeValue)).BeginInit();
            this.tpAlarm.SuspendLayout();
            this.tpDpc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDpcX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDpcY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmThreshTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmThreshDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmTrigTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmTrigDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmInterval)).BeginInit();
            this.cmsMarkers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPalleteImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpan)).BeginInit();
            this.SuspendLayout();
            // 
            // tsConnection
            // 
            this.tsConnection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslIP,
            this.tstbIP,
            this.tslPort,
            this.tstbPort,
            this.tslUser,
            this.tstbUser,
            this.tslPassword,
            this.tstbPassword,
            this.tsbConnect,
            this.tsActionSpacer,
            this.tsbCalibrate,
            this.tsbCapture});
            this.tsConnection.Location = new System.Drawing.Point(0, 0);
            this.tsConnection.Name = "tsConnection";
            this.tsConnection.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.tsConnection.Size = new System.Drawing.Size(1377, 25);
            this.tsConnection.TabIndex = 100;
            // 
            // tslIP
            // 
            this.tslIP.Name = "tslIP";
            this.tslIP.Size = new System.Drawing.Size(22, 22);
            this.tslIP.Text = "IP:";
            // 
            // tstbIP
            // 
            this.tstbIP.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.tstbIP.Name = "tstbIP";
            this.tstbIP.Size = new System.Drawing.Size(110, 25);
            this.tstbIP.Text = "192.168.23.33";
            // 
            // tslPort
            // 
            this.tslPort.Name = "tslPort";
            this.tslPort.Size = new System.Drawing.Size(35, 22);
            this.tslPort.Text = "Port:";
            // 
            // tstbPort
            // 
            this.tstbPort.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.tstbPort.Name = "tstbPort";
            this.tstbPort.Size = new System.Drawing.Size(40, 25);
            this.tstbPort.Text = "80";
            // 
            // tslUser
            // 
            this.tslUser.Name = "tslUser";
            this.tslUser.Size = new System.Drawing.Size(38, 22);
            this.tslUser.Text = "User:";
            // 
            // tstbUser
            // 
            this.tstbUser.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.tstbUser.Name = "tstbUser";
            this.tstbUser.Size = new System.Drawing.Size(60, 25);
            this.tstbUser.Text = "admin";
            // 
            // tslPassword
            // 
            this.tslPassword.Name = "tslPassword";
            this.tslPassword.Size = new System.Drawing.Size(35, 22);
            this.tslPassword.Text = "Pwd:";
            // 
            // tstbPassword
            // 
            this.tstbPassword.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.tstbPassword.Name = "tstbPassword";
            this.tstbPassword.Size = new System.Drawing.Size(60, 25);
            this.tstbPassword.Text = "admin";
            // 
            // tsbConnect
            // 
            this.tsbConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbConnect.Name = "tsbConnect";
            this.tsbConnect.Size = new System.Drawing.Size(59, 22);
            this.tsbConnect.Text = "Connect";
            this.tsbConnect.Click += new System.EventHandler(this.tsbConnect_Click);
            // 
            // tsActionSpacer
            // 
            this.tsActionSpacer.Name = "tsActionSpacer";
            this.tsActionSpacer.Size = new System.Drawing.Size(6, 25);
            this.tsActionSpacer.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            // 
            // tsbCalibrate
            // 
            this.tsbCalibrate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCalibrate.Name = "tsbCalibrate";
            this.tsbCalibrate.Padding = new System.Windows.Forms.Padding(4, 1, 4, 1);
            this.tsbCalibrate.Size = new System.Drawing.Size(64, 20);
            this.tsbCalibrate.Text = "Calibrate";
            this.tsbCalibrate.Click += new System.EventHandler(this.tsbCalibrate_Click);
            // 
            // tsMainForm
            // 
            this.tsMainForm.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tsMainForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslFileFormat,
            this.tscbFileFormat,
            this.toolStripSeparator4,
            this.tsbSelect,
            this.tsbSpot,
            this.tsbRectangle,
            this.tsbLine,
            this.tsbDelete,
            this.toolStripSeparator2,
            this.tslPalette,
            this.tsbPalette,
            this.tscbPalette,
            this.tsbInverted,
            this.toolStripSeparator5,
            this.tslColorDistributionType,
            this.tscbColorDistributionType,
            this.toolStripSeparator6,
            this.toolStripSeparator7,
            this.tslImageFlip,
            this.tscbImageFlip,
            this.toolStripSeparator10,
            this.tslStreamId,
            this.tscbStreamId,
            this.toolStripSeparator11,
            this.tslThermalStreamConfig,
            this.tscbThermalStreamConfig,
            this.toolStripSeparator12,
            this.tsbGetThermalData,
            this.tsbGetThermalLut});
            this.tsMainForm.Location = new System.Drawing.Point(0, 25);
            this.tsMainForm.Name = "tsMainForm";
            this.tsMainForm.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.tsMainForm.Size = new System.Drawing.Size(1377, 31);
            this.tsMainForm.TabIndex = 10;
            this.tsMainForm.Text = "toolStrip1";
            // 
            // tsbCapture
            // 
            this.tsbCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCapture.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.tsbCapture.Name = "tsbCapture";
            this.tsbCapture.Padding = new System.Windows.Forms.Padding(4, 1, 4, 1);
            this.tsbCapture.Size = new System.Drawing.Size(58, 20);
            this.tsbCapture.Text = "Capture";
            this.tsbCapture.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tsbGetThermalData
            // 
            this.tsbGetThermalData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbGetThermalData.Margin = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.tsbGetThermalData.Name = "tsbGetThermalData";
            this.tsbGetThermalData.Padding = new System.Windows.Forms.Padding(4, 1, 4, 1);
            this.tsbGetThermalData.Size = new System.Drawing.Size(88, 22);
            this.tsbGetThermalData.Text = "Thermal Data";
            this.tsbGetThermalData.Click += new System.EventHandler(this.tsbGetThermalData_Click);
            // 
            // tsbGetThermalLut
            // 
            this.tsbGetThermalLut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbGetThermalLut.Margin = new System.Windows.Forms.Padding(4, 2, 0, 4);
            this.tsbGetThermalLut.Name = "tsbGetThermalLut";
            this.tsbGetThermalLut.Padding = new System.Windows.Forms.Padding(4, 1, 4, 1);
            this.tsbGetThermalLut.Size = new System.Drawing.Size(78, 22);
            this.tsbGetThermalLut.Text = "Thermal LUT";
            this.tsbGetThermalLut.Click += new System.EventHandler(this.tsbGetThermalLut_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 31);
            // 
            // tslThermalStreamConfig
            // 
            this.tslThermalStreamConfig.Name = "tslThermalStreamConfig";
            this.tslThermalStreamConfig.Size = new System.Drawing.Size(88, 28);
            this.tslThermalStreamConfig.Text = "Thermal Stream";
            // 
            // tscbThermalStreamConfig
            // 
            this.tscbThermalStreamConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbThermalStreamConfig.Name = "tscbThermalStreamConfig";
            this.tscbThermalStreamConfig.Size = new System.Drawing.Size(170, 31);
            this.tscbThermalStreamConfig.SelectedIndexChanged += new System.EventHandler(this.tscbThermalStreamConfig_SelectedIndexChanged);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 31);
            // 
            // tslFileFormat
            // 
            this.tslFileFormat.Name = "tslFileFormat";
            this.tslFileFormat.Size = new System.Drawing.Size(45, 28);
            this.tslFileFormat.Text = "Range";
            // 
            // tscbFileFormat
            // 
            this.tscbFileFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbFileFormat.Name = "tscbFileFormat";
            this.tscbFileFormat.Size = new System.Drawing.Size(92, 31);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbSelect
            // 
            this.tsbSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSelect.Image = ((System.Drawing.Image)(resources.GetObject("tsbSelect.Image")));
            this.tsbSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSelect.Name = "tsbSelect";
            this.tsbSelect.Size = new System.Drawing.Size(28, 28);
            this.tsbSelect.Text = "Select Tool";
            this.tsbSelect.Click += new System.EventHandler(this.tsbSelect_Click);
            // 
            // tsbSpot
            // 
            this.tsbSpot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSpot.Image = ((System.Drawing.Image)(resources.GetObject("tsbSpot.Image")));
            this.tsbSpot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSpot.Name = "tsbSpot";
            this.tsbSpot.Size = new System.Drawing.Size(28, 28);
            this.tsbSpot.Text = "Spot Tool";
            this.tsbSpot.Click += new System.EventHandler(this.tsbSpot_Click);
            // 
            // tsbRectangle
            // 
            this.tsbRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRectangle.Image = ((System.Drawing.Image)(resources.GetObject("tsbRectangle.Image")));
            this.tsbRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRectangle.Name = "tsbRectangle";
            this.tsbRectangle.Size = new System.Drawing.Size(28, 28);
            this.tsbRectangle.Text = "Rectangle Tool";
            this.tsbRectangle.Click += new System.EventHandler(this.tsbRectangle_Click);
            // 
            // tsbLine
            // 
            this.tsbLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLine.Image = ((System.Drawing.Image)(resources.GetObject("tsbLine.Image")));
            this.tsbLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLine.Name = "tsbLine";
            this.tsbLine.Size = new System.Drawing.Size(28, 28);
            this.tsbLine.Text = "Line Tool";
            this.tsbLine.Click += new System.EventHandler(this.tsbLine_Click);
            // 
            // tsbDelete
            // 
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsbDelete.Image")));
            this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(28, 28);
            this.tsbDelete.Text = "Delete Tool";
            this.tsbDelete.ToolTipText = "Delete Tool";
            this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // tslPalette
            // 
            this.tslPalette.Name = "tslPalette";
            this.tslPalette.Size = new System.Drawing.Size(47, 28);
            this.tslPalette.Text = "Palette";
            // 
            // tsbPalette
            // 
            this.tsbPalette.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPalette.Image = ((System.Drawing.Image)(resources.GetObject("tsbPalette.Image")));
            this.tsbPalette.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPalette.Name = "tsbPalette";
            this.tsbPalette.Size = new System.Drawing.Size(28, 28);
            // 
            // tscbPalette
            // 
            this.tscbPalette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbPalette.Name = "tscbPalette";
            this.tscbPalette.Size = new System.Drawing.Size(121, 31);
            this.tscbPalette.SelectedIndexChanged += new System.EventHandler(this.tscbPalette_SelectedIndexChanged);
            // 
            // tsbInverted
            // 
            this.tsbInverted.CheckOnClick = true;
            this.tsbInverted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInverted.Image = ((System.Drawing.Image)(resources.GetObject("tsbInverted.Image")));
            this.tsbInverted.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInverted.Name = "tsbInverted";
            this.tsbInverted.Size = new System.Drawing.Size(60, 28);
            this.tsbInverted.Text = "Inverted";
            this.tsbInverted.CheckedChanged += new System.EventHandler(this.tsbInverted_CheckedChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
            // 
            // tslColorDistributionType
            // 
            this.tslColorDistributionType.Name = "tslColorDistributionType";
            this.tslColorDistributionType.Size = new System.Drawing.Size(143, 28);
            this.tslColorDistributionType.Text = "Color Distribution Type";
            // 
            // tscbColorDistributionType
            // 
            this.tscbColorDistributionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbColorDistributionType.DropDownWidth = 140;
            this.tscbColorDistributionType.Name = "tscbColorDistributionType";
            this.tscbColorDistributionType.Size = new System.Drawing.Size(110, 31);
            this.tscbColorDistributionType.SelectedIndexChanged += new System.EventHandler(this.tscbColorDistributionType_SelectedIndexChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 31);
            // 
            // tslImageFlip
            // 
            this.tslImageFlip.Name = "tslImageFlip";
            this.tslImageFlip.Size = new System.Drawing.Size(31, 28);
            this.tslImageFlip.Text = "Flip:";
            // 
            // tscbImageFlip
            // 
            this.tscbImageFlip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbImageFlip.DropDownWidth = 120;
            this.tscbImageFlip.Items.AddRange(new object[] {
            "None",
            "Up-Down",
            "Left-Right",
            "Center"});
            this.tscbImageFlip.Name = "tscbImageFlip";
            this.tscbImageFlip.Size = new System.Drawing.Size(110, 31);
            this.tscbImageFlip.SelectedIndexChanged += new System.EventHandler(this.tscbImageFlip_SelectedIndexChanged);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 31);
            // 
            // tslStreamId
            // 
            this.tslStreamId.Name = "tslStreamId";
            this.tslStreamId.Size = new System.Drawing.Size(62, 28);
            this.tslStreamId.Text = "Stream ID:";
            // 
            // tscbStreamId
            // 
            this.tscbStreamId.AutoSize = false;
            this.tscbStreamId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbStreamId.DropDownWidth = 140;
            this.tscbStreamId.Name = "tscbStreamId";
            this.tscbStreamId.Size = new System.Drawing.Size(100, 25);
            this.tscbStreamId.SelectedIndexChanged += new System.EventHandler(this.tscbStreamId_SelectedIndexChanged);
            // 
            // pbThermalImage
            // 
            this.pbThermalImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbThermalImage.BackColor = System.Drawing.SystemColors.Window;
            this.pbThermalImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbThermalImage.Location = new System.Drawing.Point(12, 58);
            this.pbThermalImage.Name = "pbThermalImage";
            this.pbThermalImage.Size = new System.Drawing.Size(723, 571);
            this.pbThermalImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbThermalImage.TabIndex = 11;
            this.pbThermalImage.TabStop = false;
            this.pbThermalImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbThermalImage_MouseDown);
            this.pbThermalImage.MouseLeave += new System.EventHandler(this.pbThermalImage_MouseLeave);
            this.pbThermalImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbThermalImage_MouseMove);
            this.pbThermalImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbThermalImage_MouseUp);
            // 
            // tcThermalFile
            // 
            this.tcThermalFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcThermalFile.Controls.Add(this.tpProperties);
            this.tcThermalFile.Controls.Add(this.tpThermalParams);
            this.tcThermalFile.Controls.Add(this.tpTime);
            this.tcThermalFile.Controls.Add(this.tpNetwork);
            this.tcThermalFile.Controls.Add(this.tpUser);
            this.tcThermalFile.Controls.Add(this.tpDisplay);
            this.tcThermalFile.Controls.Add(this.tpThermalOsd);
            this.tcThermalFile.Controls.Add(this.tpAlarm);
            this.tcThermalFile.Controls.Add(this.tpDpc);
            this.tcThermalFile.Location = new System.Drawing.Point(879, 58);
            this.tcThermalFile.Name = "tcThermalFile";
            this.tcThermalFile.SelectedIndex = 0;
            this.tcThermalFile.Size = new System.Drawing.Size(486, 520);
            this.tcThermalFile.TabIndex = 13;
            this.tcThermalFile.SelectedIndexChanged += new System.EventHandler(this.tcThermalFile_SelectedIndexChanged);
            // 
            // tpProperties
            // 
            this.tpProperties.Controls.Add(this.tbDeviceName);
            this.tpProperties.Controls.Add(this.btnSetDeviceName);
            this.tpProperties.Controls.Add(this.lvProperties);
            this.tpProperties.Controls.Add(this.tbUpgradePath);
            this.tpProperties.Controls.Add(this.btnUpgradeBrowse);
            this.tpProperties.Controls.Add(this.btnUpgrade);
            this.tpProperties.Controls.Add(this.btnDeviceReset);
            this.tpProperties.Controls.Add(this.btnDeviceRestart);
            this.tpProperties.Controls.Add(this.btnPropertiesGet);
            this.tpProperties.Location = new System.Drawing.Point(4, 22);
            this.tpProperties.Name = "tpProperties";
            this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperties.Size = new System.Drawing.Size(478, 494);
            this.tpProperties.TabIndex = 0;
            this.tpProperties.Text = "Properties";
            this.tpProperties.UseVisualStyleBackColor = true;
            // 
            // tbDeviceName
            // 
            this.tbDeviceName.Location = new System.Drawing.Point(6, 6);
            this.tbDeviceName.Name = "tbDeviceName";
            this.tbDeviceName.Size = new System.Drawing.Size(260, 21);
            this.tbDeviceName.TabIndex = 0;
            // 
            // btnSetDeviceName
            // 
            this.btnSetDeviceName.Location = new System.Drawing.Point(272, 5);
            this.btnSetDeviceName.Name = "btnSetDeviceName";
            this.btnSetDeviceName.Size = new System.Drawing.Size(80, 23);
            this.btnSetDeviceName.TabIndex = 1;
            this.btnSetDeviceName.Text = "Set Name";
            this.btnSetDeviceName.UseVisualStyleBackColor = true;
            this.btnSetDeviceName.Click += new System.EventHandler(this.btnSetDeviceName_Click);
            // 
            // lvProperties
            // 
            this.lvProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chPropertyKey,
            this.chPropertyValue});
            this.lvProperties.FullRowSelect = true;
            this.lvProperties.GridLines = true;
            listViewGroup1.Header = "Camera Info";
            listViewGroup1.Name = "lvgCameraInfo";
            listViewGroup2.Header = "Lens Info";
            listViewGroup2.Name = "lvgLensInfo";
            listViewGroup3.Header = "Temp. Range";
            listViewGroup3.Name = "lvgTempRange";
            listViewGroup4.Header = "Video";
            listViewGroup4.Name = "lvgSize";
            listViewGroup5.Header = "Geo Location";
            listViewGroup5.Name = "lvgGeoLocation";
            listViewGroup6.Header = "Image Enhancement";
            listViewGroup6.Name = "lvgImageEnhancement";
            listViewGroup7.Header = "File Info";
            listViewGroup7.Name = "lvgFileInfo";
            this.lvProperties.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7});
            this.lvProperties.HideSelection = false;
            this.lvProperties.Location = new System.Drawing.Point(6, 90);
            this.lvProperties.Name = "lvProperties";
            this.lvProperties.Size = new System.Drawing.Size(466, 400);
            this.lvProperties.TabIndex = 0;
            this.lvProperties.UseCompatibleStateImageBehavior = false;
            this.lvProperties.View = System.Windows.Forms.View.Details;
            // 
            // chPropertyKey
            // 
            this.chPropertyKey.Text = "Property";
            this.chPropertyKey.Width = 100;
            // 
            // chPropertyValue
            // 
            this.chPropertyValue.Text = "Value";
            this.chPropertyValue.Width = 350;
            // 
            // tbUpgradePath
            // 
            this.tbUpgradePath.Location = new System.Drawing.Point(6, 33);
            this.tbUpgradePath.Name = "tbUpgradePath";
            this.tbUpgradePath.ReadOnly = true;
            this.tbUpgradePath.Size = new System.Drawing.Size(260, 21);
            this.tbUpgradePath.TabIndex = 2;
            // 
            // btnUpgradeBrowse
            // 
            this.btnUpgradeBrowse.Location = new System.Drawing.Point(272, 32);
            this.btnUpgradeBrowse.Name = "btnUpgradeBrowse";
            this.btnUpgradeBrowse.Size = new System.Drawing.Size(35, 23);
            this.btnUpgradeBrowse.TabIndex = 3;
            this.btnUpgradeBrowse.Text = "...";
            this.btnUpgradeBrowse.UseVisualStyleBackColor = true;
            this.btnUpgradeBrowse.Click += new System.EventHandler(this.btnUpgradeBrowse_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(313, 32);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(70, 23);
            this.btnUpgrade.TabIndex = 4;
            this.btnUpgrade.Text = "Upgrade";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // btnDeviceReset
            // 
            this.btnDeviceReset.BackColor = System.Drawing.Color.LightCoral;
            this.btnDeviceReset.Location = new System.Drawing.Point(82, 60);
            this.btnDeviceReset.Name = "btnDeviceReset";
            this.btnDeviceReset.Size = new System.Drawing.Size(70, 23);
            this.btnDeviceReset.TabIndex = 5;
            this.btnDeviceReset.Text = "Reset";
            this.btnDeviceReset.UseVisualStyleBackColor = false;
            this.btnDeviceReset.Click += new System.EventHandler(this.btnDeviceReset_Click);
            // 
            // btnDeviceRestart
            // 
            this.btnDeviceRestart.Location = new System.Drawing.Point(6, 60);
            this.btnDeviceRestart.Name = "btnDeviceRestart";
            this.btnDeviceRestart.Size = new System.Drawing.Size(70, 23);
            this.btnDeviceRestart.TabIndex = 6;
            this.btnDeviceRestart.Text = "Restart";
            this.btnDeviceRestart.UseVisualStyleBackColor = true;
            this.btnDeviceRestart.Click += new System.EventHandler(this.btnDeviceRestart_Click);
            // 
            // btnPropertiesGet
            // 
            this.btnPropertiesGet.Location = new System.Drawing.Point(158, 60);
            this.btnPropertiesGet.Name = "btnPropertiesGet";
            this.btnPropertiesGet.Size = new System.Drawing.Size(80, 23);
            this.btnPropertiesGet.TabIndex = 7;
            this.btnPropertiesGet.Text = "Get";
            this.btnPropertiesGet.UseVisualStyleBackColor = true;
            this.btnPropertiesGet.Click += new System.EventHandler(this.btnPropertiesGet_Click);
            // 
            // tpThermalParams
            // 
            this.tpThermalParams.AutoScroll = true;
            this.tpThermalParams.Controls.Add(this.btnThermalParamsGet);
            this.tpThermalParams.Controls.Add(this.btnThermalParamsSet);
            this.tpThermalParams.Controls.Add(this.lblRelHumidityUnit);
            this.tpThermalParams.Controls.Add(this.lblAtmTempUnit);
            this.tpThermalParams.Controls.Add(this.lblDistanceUnit);
            this.tpThermalParams.Controls.Add(this.lblReflTempUnit);
            this.tpThermalParams.Controls.Add(this.nudRelHumidity);
            this.tpThermalParams.Controls.Add(this.nudAtmTemp);
            this.tpThermalParams.Controls.Add(this.nudDistance);
            this.tpThermalParams.Controls.Add(this.nudReflTemp);
            this.tpThermalParams.Controls.Add(this.nudEmissivity);
            this.tpThermalParams.Controls.Add(this.lblDistance);
            this.tpThermalParams.Controls.Add(this.lblRelHumidity);
            this.tpThermalParams.Controls.Add(this.lblAtmTemp);
            this.tpThermalParams.Controls.Add(this.lblReflTemp);
            this.tpThermalParams.Controls.Add(this.lblEmissivity);
            this.tpThermalParams.Controls.Add(this.lblShutterMode);
            this.tpThermalParams.Controls.Add(this.cbShutterMode);
            this.tpThermalParams.Controls.Add(this.lblShutterTime);
            this.tpThermalParams.Controls.Add(this.nudShutterTime);
            this.tpThermalParams.Controls.Add(this.lblTempUnit);
            this.tpThermalParams.Controls.Add(this.cbTempUnit);
            this.tpThermalParams.Controls.Add(this.lblDistUnit);
            this.tpThermalParams.Controls.Add(this.cbDistUnit);
            this.tpThermalParams.Location = new System.Drawing.Point(4, 22);
            this.tpThermalParams.Name = "tpThermalParams";
            this.tpThermalParams.Padding = new System.Windows.Forms.Padding(3);
            this.tpThermalParams.Size = new System.Drawing.Size(478, 723);
            this.tpThermalParams.TabIndex = 3;
            this.tpThermalParams.Text = "T.Params";
            this.tpThermalParams.UseVisualStyleBackColor = true;
            // 
            // lblRelHumidityUnit
            // 
            this.lblRelHumidityUnit.AutoSize = true;
            this.lblRelHumidityUnit.Location = new System.Drawing.Point(224, 127);
            this.lblRelHumidityUnit.Name = "lblRelHumidityUnit";
            this.lblRelHumidityUnit.Size = new System.Drawing.Size(11, 12);
            this.lblRelHumidityUnit.TabIndex = 13;
            this.lblRelHumidityUnit.Text = "%";
            // 
            // lblAtmTempUnit
            // 
            this.lblAtmTempUnit.AutoSize = true;
            this.lblAtmTempUnit.Location = new System.Drawing.Point(224, 100);
            this.lblAtmTempUnit.Name = "lblAtmTempUnit";
            this.lblAtmTempUnit.Size = new System.Drawing.Size(23, 12);
            this.lblAtmTempUnit.TabIndex = 12;
            this.lblAtmTempUnit.Text = "°C";
            // 
            // lblDistanceUnit
            // 
            this.lblDistanceUnit.AutoSize = true;
            this.lblDistanceUnit.Location = new System.Drawing.Point(224, 73);
            this.lblDistanceUnit.Name = "lblDistanceUnit";
            this.lblDistanceUnit.Size = new System.Drawing.Size(11, 12);
            this.lblDistanceUnit.TabIndex = 11;
            this.lblDistanceUnit.Text = "m";
            // 
            // lblReflTempUnit
            // 
            this.lblReflTempUnit.AutoSize = true;
            this.lblReflTempUnit.Location = new System.Drawing.Point(224, 46);
            this.lblReflTempUnit.Name = "lblReflTempUnit";
            this.lblReflTempUnit.Size = new System.Drawing.Size(23, 12);
            this.lblReflTempUnit.TabIndex = 10;
            this.lblReflTempUnit.Text = "°C";
            // 
            // nudRelHumidity
            // 
            this.nudRelHumidity.DecimalPlaces = 1;
            this.nudRelHumidity.Location = new System.Drawing.Point(114, 123);
            this.nudRelHumidity.Name = "nudRelHumidity";
            this.nudRelHumidity.Size = new System.Drawing.Size(104, 21);
            this.nudRelHumidity.TabIndex = 9;
            // 
            // nudAtmTemp
            // 
            this.nudAtmTemp.DecimalPlaces = 1;
            this.nudAtmTemp.Location = new System.Drawing.Point(114, 96);
            this.nudAtmTemp.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudAtmTemp.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudAtmTemp.Name = "nudAtmTemp";
            this.nudAtmTemp.Size = new System.Drawing.Size(104, 21);
            this.nudAtmTemp.TabIndex = 8;
            // 
            // nudDistance
            // 
            this.nudDistance.DecimalPlaces = 1;
            this.nudDistance.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudDistance.Location = new System.Drawing.Point(114, 69);
            this.nudDistance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDistance.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudDistance.Name = "nudDistance";
            this.nudDistance.Size = new System.Drawing.Size(104, 21);
            this.nudDistance.TabIndex = 7;
            // 
            // nudReflTemp
            // 
            this.nudReflTemp.DecimalPlaces = 1;
            this.nudReflTemp.Location = new System.Drawing.Point(114, 42);
            this.nudReflTemp.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudReflTemp.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudReflTemp.Name = "nudReflTemp";
            this.nudReflTemp.Size = new System.Drawing.Size(104, 21);
            this.nudReflTemp.TabIndex = 6;
            // 
            // nudEmissivity
            // 
            this.nudEmissivity.DecimalPlaces = 2;
            this.nudEmissivity.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudEmissivity.Location = new System.Drawing.Point(114, 15);
            this.nudEmissivity.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.nudEmissivity.Name = "nudEmissivity";
            this.nudEmissivity.Size = new System.Drawing.Size(104, 21);
            this.nudEmissivity.TabIndex = 5;
            this.nudEmissivity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(9, 73);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(53, 12);
            this.lblDistance.TabIndex = 4;
            this.lblDistance.Text = "Distance";
            // 
            // lblRelHumidity
            // 
            this.lblRelHumidity.AutoSize = true;
            this.lblRelHumidity.Location = new System.Drawing.Point(9, 127);
            this.lblRelHumidity.Name = "lblRelHumidity";
            this.lblRelHumidity.Size = new System.Drawing.Size(59, 12);
            this.lblRelHumidity.TabIndex = 3;
            this.lblRelHumidity.Text = "Rel. Hum.";
            // 
            // lblAtmTemp
            // 
            this.lblAtmTemp.AutoSize = true;
            this.lblAtmTemp.Location = new System.Drawing.Point(9, 100);
            this.lblAtmTemp.Name = "lblAtmTemp";
            this.lblAtmTemp.Size = new System.Drawing.Size(65, 12);
            this.lblAtmTemp.TabIndex = 2;
            this.lblAtmTemp.Text = "Atm. Temp.";
            // 
            // lblReflTemp
            // 
            this.lblReflTemp.AutoSize = true;
            this.lblReflTemp.Location = new System.Drawing.Point(9, 46);
            this.lblReflTemp.Name = "lblReflTemp";
            this.lblReflTemp.Size = new System.Drawing.Size(71, 12);
            this.lblReflTemp.TabIndex = 1;
            this.lblReflTemp.Text = "Refl. Temp.";
            // 
            // lblEmissivity
            // 
            this.lblEmissivity.AutoSize = true;
            this.lblEmissivity.Location = new System.Drawing.Point(9, 19);
            this.lblEmissivity.Name = "lblEmissivity";
            this.lblEmissivity.Size = new System.Drawing.Size(65, 12);
            this.lblEmissivity.TabIndex = 0;
            this.lblEmissivity.Text = "Emissivity";
            // 
            // lblShutterMode
            // 
            this.lblShutterMode.AutoSize = true;
            this.lblShutterMode.Location = new System.Drawing.Point(9, 150);
            this.lblShutterMode.Name = "lblShutterMode";
            this.lblShutterMode.Size = new System.Drawing.Size(77, 12);
            this.lblShutterMode.TabIndex = 14;
            this.lblShutterMode.Text = "Shutter Mode";
            // 
            // cbShutterMode
            // 
            this.cbShutterMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShutterMode.Items.AddRange(new object[] {
            "Automatic",
            "Manual"});
            this.cbShutterMode.Location = new System.Drawing.Point(114, 147);
            this.cbShutterMode.Name = "cbShutterMode";
            this.cbShutterMode.Size = new System.Drawing.Size(120, 20);
            this.cbShutterMode.TabIndex = 15;
            // 
            // lblShutterTime
            // 
            this.lblShutterTime.AutoSize = true;
            this.lblShutterTime.Location = new System.Drawing.Point(9, 177);
            this.lblShutterTime.Name = "lblShutterTime";
            this.lblShutterTime.Size = new System.Drawing.Size(107, 12);
            this.lblShutterTime.TabIndex = 16;
            this.lblShutterTime.Text = "Shutter Time(min)";
            // 
            // nudShutterTime
            // 
            this.nudShutterTime.Location = new System.Drawing.Point(114, 174);
            this.nudShutterTime.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.nudShutterTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudShutterTime.Name = "nudShutterTime";
            this.nudShutterTime.Size = new System.Drawing.Size(80, 21);
            this.nudShutterTime.TabIndex = 17;
            this.nudShutterTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lblTempUnit
            // 
            this.lblTempUnit.AutoSize = true;
            this.lblTempUnit.Location = new System.Drawing.Point(9, 204);
            this.lblTempUnit.Name = "lblTempUnit";
            this.lblTempUnit.Size = new System.Drawing.Size(59, 12);
            this.lblTempUnit.TabIndex = 18;
            this.lblTempUnit.Text = "Temp Unit";
            // 
            // cbTempUnit
            // 
            this.cbTempUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTempUnit.Items.AddRange(new object[] {
            "Celsius",
            "Kelvin",
            "Fahrenheit"});
            this.cbTempUnit.Location = new System.Drawing.Point(114, 201);
            this.cbTempUnit.Name = "cbTempUnit";
            this.cbTempUnit.Size = new System.Drawing.Size(120, 20);
            this.cbTempUnit.TabIndex = 19;
            this.cbTempUnit.SelectedIndexChanged += new System.EventHandler(this.cbTempUnit_SelectedIndexChanged);
            // 
            // lblDistUnit
            // 
            this.lblDistUnit.AutoSize = true;
            this.lblDistUnit.Location = new System.Drawing.Point(9, 231);
            this.lblDistUnit.Name = "lblDistUnit";
            this.lblDistUnit.Size = new System.Drawing.Size(83, 12);
            this.lblDistUnit.TabIndex = 20;
            this.lblDistUnit.Text = "Distance Unit";
            // 
            // cbDistUnit
            // 
            this.cbDistUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDistUnit.Items.AddRange(new object[] {
            "Meter",
            "Foot"});
            this.cbDistUnit.Location = new System.Drawing.Point(114, 228);
            this.cbDistUnit.Name = "cbDistUnit";
            this.cbDistUnit.Size = new System.Drawing.Size(120, 20);
            this.cbDistUnit.TabIndex = 21;
            this.cbDistUnit.SelectedIndexChanged += new System.EventHandler(this.cbDistUnit_SelectedIndexChanged);
            // 
            // btnThermalParamsGet
            // 
            this.btnThermalParamsGet.Location = new System.Drawing.Point(6, 265);
            this.btnThermalParamsGet.Name = "btnThermalParamsGet";
            this.btnThermalParamsGet.Size = new System.Drawing.Size(80, 25);
            this.btnThermalParamsGet.TabIndex = 22;
            this.btnThermalParamsGet.Text = "Get";
            this.btnThermalParamsGet.UseVisualStyleBackColor = true;
            this.btnThermalParamsGet.Click += new System.EventHandler(this.btnThermalParamsGet_Click);
            // 
            // btnThermalParamsSet
            // 
            this.btnThermalParamsSet.Location = new System.Drawing.Point(96, 265);
            this.btnThermalParamsSet.Name = "btnThermalParamsSet";
            this.btnThermalParamsSet.Size = new System.Drawing.Size(80, 25);
            this.btnThermalParamsSet.TabIndex = 23;
            this.btnThermalParamsSet.Text = "Set";
            this.btnThermalParamsSet.UseVisualStyleBackColor = true;
            this.btnThermalParamsSet.Click += new System.EventHandler(this.btnThermalParamsSet_Click);
            // 
            // tpTime
            // 
            this.tpTime.Controls.Add(this.lblTimeZone);
            this.tpTime.Controls.Add(this.cbTimeZone);
            this.tpTime.Controls.Add(this.lblTimeMode);
            this.tpTime.Controls.Add(this.cbTimeMode);
            this.tpTime.Controls.Add(this.lblLocalTime);
            this.tpTime.Controls.Add(this.tbLocalTime);
            this.tpTime.Controls.Add(this.btnGetTime);
            this.tpTime.Controls.Add(this.btnSyncTime);
            this.tpTime.Controls.Add(this.btnApplyTime);
            this.tpTime.Controls.Add(this.lblNtpHost);
            this.tpTime.Controls.Add(this.tbNtpHost);
            this.tpTime.Controls.Add(this.lblNtpPort);
            this.tpTime.Controls.Add(this.nudNtpPort);
            this.tpTime.Controls.Add(this.lblNtpInterval);
            this.tpTime.Controls.Add(this.nudNtpInterval);
            this.tpTime.Controls.Add(this.btnNtpApply);
            this.tpTime.Location = new System.Drawing.Point(4, 22);
            this.tpTime.Name = "tpTime";
            this.tpTime.Padding = new System.Windows.Forms.Padding(3);
            this.tpTime.Size = new System.Drawing.Size(478, 723);
            this.tpTime.TabIndex = 5;
            this.tpTime.Text = "Time";
            this.tpTime.UseVisualStyleBackColor = true;
            // 
            // lblTimeZone
            // 
            this.lblTimeZone.AutoSize = true;
            this.lblTimeZone.Location = new System.Drawing.Point(15, 15);
            this.lblTimeZone.Name = "lblTimeZone";
            this.lblTimeZone.Size = new System.Drawing.Size(59, 12);
            this.lblTimeZone.TabIndex = 0;
            this.lblTimeZone.Text = "Time Zone";
            // 
            // cbTimeZone
            // 
            this.cbTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTimeZone.Items.AddRange(new object[] {
            "UTC-12:00",
            "UTC-11:00",
            "UTC-10:00",
            "UTC-09:00",
            "UTC-08:00",
            "UTC-07:00",
            "UTC-06:00",
            "UTC-05:00",
            "UTC-04:00",
            "UTC-03:00",
            "UTC-02:00",
            "UTC-01:00",
            "UTC+00:00",
            "UTC+01:00",
            "UTC+02:00",
            "UTC+03:00",
            "UTC+04:00",
            "UTC+05:00",
            "UTC+05:30",
            "UTC+06:00",
            "UTC+07:00",
            "UTC+08:00",
            "UTC+09:00",
            "UTC+09:30",
            "UTC+10:00",
            "UTC+11:00",
            "UTC+12:00",
            "UTC+13:00"});
            this.cbTimeZone.Location = new System.Drawing.Point(110, 12);
            this.cbTimeZone.Name = "cbTimeZone";
            this.cbTimeZone.Size = new System.Drawing.Size(200, 20);
            this.cbTimeZone.TabIndex = 1;
            // 
            // lblTimeMode
            // 
            this.lblTimeMode.AutoSize = true;
            this.lblTimeMode.Location = new System.Drawing.Point(15, 48);
            this.lblTimeMode.Name = "lblTimeMode";
            this.lblTimeMode.Size = new System.Drawing.Size(59, 12);
            this.lblTimeMode.TabIndex = 2;
            this.lblTimeMode.Text = "Time Mode";
            // 
            // cbTimeMode
            // 
            this.cbTimeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTimeMode.Items.AddRange(new object[] {
            "Local",
            "NTP"});
            this.cbTimeMode.Location = new System.Drawing.Point(110, 45);
            this.cbTimeMode.Name = "cbTimeMode";
            this.cbTimeMode.Size = new System.Drawing.Size(150, 20);
            this.cbTimeMode.TabIndex = 3;
            this.cbTimeMode.SelectedIndexChanged += new System.EventHandler(this.cbTimeMode_SelectedIndexChanged);
            // 
            // lblLocalTime
            // 
            this.lblLocalTime.AutoSize = true;
            this.lblLocalTime.Location = new System.Drawing.Point(15, 83);
            this.lblLocalTime.Name = "lblLocalTime";
            this.lblLocalTime.Size = new System.Drawing.Size(71, 12);
            this.lblLocalTime.TabIndex = 4;
            this.lblLocalTime.Text = "Device Time";
            // 
            // tbLocalTime
            // 
            this.tbLocalTime.Location = new System.Drawing.Point(110, 80);
            this.tbLocalTime.Name = "tbLocalTime";
            this.tbLocalTime.ReadOnly = true;
            this.tbLocalTime.Size = new System.Drawing.Size(200, 21);
            this.tbLocalTime.TabIndex = 5;
            // 
            // btnGetTime
            // 
            this.btnGetTime.Location = new System.Drawing.Point(110, 112);
            this.btnGetTime.Name = "btnGetTime";
            this.btnGetTime.Size = new System.Drawing.Size(75, 25);
            this.btnGetTime.TabIndex = 6;
            this.btnGetTime.Text = "Refresh";
            this.btnGetTime.UseVisualStyleBackColor = true;
            this.btnGetTime.Click += new System.EventHandler(this.btnGetTime_Click);
            // 
            // btnSyncTime
            // 
            this.btnSyncTime.Location = new System.Drawing.Point(195, 112);
            this.btnSyncTime.Name = "btnSyncTime";
            this.btnSyncTime.Size = new System.Drawing.Size(100, 25);
            this.btnSyncTime.TabIndex = 7;
            this.btnSyncTime.Text = "Sync PC Time";
            this.btnSyncTime.UseVisualStyleBackColor = true;
            this.btnSyncTime.Click += new System.EventHandler(this.btnSyncTime_Click);
            // 
            // btnApplyTime
            // 
            this.btnApplyTime.Location = new System.Drawing.Point(305, 112);
            this.btnApplyTime.Name = "btnApplyTime";
            this.btnApplyTime.Size = new System.Drawing.Size(90, 25);
            this.btnApplyTime.TabIndex = 8;
            this.btnApplyTime.Text = "Apply Time";
            this.btnApplyTime.UseVisualStyleBackColor = true;
            this.btnApplyTime.Click += new System.EventHandler(this.btnApplyTime_Click);
            // 
            // lblNtpHost
            // 
            this.lblNtpHost.AutoSize = true;
            this.lblNtpHost.Location = new System.Drawing.Point(15, 155);
            this.lblNtpHost.Name = "lblNtpHost";
            this.lblNtpHost.Size = new System.Drawing.Size(65, 12);
            this.lblNtpHost.TabIndex = 8;
            this.lblNtpHost.Text = "NTP Server";
            // 
            // tbNtpHost
            // 
            this.tbNtpHost.Location = new System.Drawing.Point(120, 152);
            this.tbNtpHost.Name = "tbNtpHost";
            this.tbNtpHost.Size = new System.Drawing.Size(200, 21);
            this.tbNtpHost.TabIndex = 9;
            // 
            // lblNtpPort
            // 
            this.lblNtpPort.AutoSize = true;
            this.lblNtpPort.Location = new System.Drawing.Point(15, 185);
            this.lblNtpPort.Name = "lblNtpPort";
            this.lblNtpPort.Size = new System.Drawing.Size(53, 12);
            this.lblNtpPort.TabIndex = 10;
            this.lblNtpPort.Text = "NTP Port";
            // 
            // nudNtpPort
            // 
            this.nudNtpPort.Location = new System.Drawing.Point(120, 183);
            this.nudNtpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudNtpPort.Name = "nudNtpPort";
            this.nudNtpPort.Size = new System.Drawing.Size(100, 21);
            this.nudNtpPort.TabIndex = 11;
            this.nudNtpPort.Value = new decimal(new int[] {
            123,
            0,
            0,
            0});
            // 
            // lblNtpInterval
            // 
            this.lblNtpInterval.AutoSize = true;
            this.lblNtpInterval.Location = new System.Drawing.Point(15, 218);
            this.lblNtpInterval.Name = "lblNtpInterval";
            this.lblNtpInterval.Size = new System.Drawing.Size(101, 12);
            this.lblNtpInterval.TabIndex = 12;
            this.lblNtpInterval.Text = "Sync Interval(s)";
            // 
            // nudNtpInterval
            // 
            this.nudNtpInterval.Location = new System.Drawing.Point(120, 216);
            this.nudNtpInterval.Maximum = new decimal(new int[] {
            86400,
            0,
            0,
            0});
            this.nudNtpInterval.Name = "nudNtpInterval";
            this.nudNtpInterval.Size = new System.Drawing.Size(100, 21);
            this.nudNtpInterval.TabIndex = 13;
            this.nudNtpInterval.Value = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            // 
            // btnNtpApply
            // 
            this.btnNtpApply.Location = new System.Drawing.Point(110, 250);
            this.btnNtpApply.Name = "btnNtpApply";
            this.btnNtpApply.Size = new System.Drawing.Size(100, 25);
            this.btnNtpApply.TabIndex = 14;
            this.btnNtpApply.Text = "Apply NTP";
            this.btnNtpApply.UseVisualStyleBackColor = true;
            this.btnNtpApply.Click += new System.EventHandler(this.btnNtpApply_Click);
            // 
            // tpNetwork
            // 
            this.tpNetwork.Controls.Add(this.lblNetDhcp);
            this.tpNetwork.Controls.Add(this.cbNetDhcp);
            this.tpNetwork.Controls.Add(this.lblNetIp);
            this.tpNetwork.Controls.Add(this.tbNetIp);
            this.tpNetwork.Controls.Add(this.lblNetMask);
            this.tpNetwork.Controls.Add(this.tbNetMask);
            this.tpNetwork.Controls.Add(this.lblNetGateway);
            this.tpNetwork.Controls.Add(this.tbNetGateway);
            this.tpNetwork.Controls.Add(this.lblNetDns);
            this.tpNetwork.Controls.Add(this.tbNetDns);
            this.tpNetwork.Controls.Add(this.btnNetApply);
            this.tpNetwork.Controls.Add(this.btnNetRefresh);
            this.tpNetwork.Controls.Add(this.lblFtpServer);
            this.tpNetwork.Controls.Add(this.tbFtpServer);
            this.tpNetwork.Controls.Add(this.lblFtpPort);
            this.tpNetwork.Controls.Add(this.nudFtpPort);
            this.tpNetwork.Controls.Add(this.lblFtpUser);
            this.tpNetwork.Controls.Add(this.tbFtpUser);
            this.tpNetwork.Controls.Add(this.lblFtpPwd);
            this.tpNetwork.Controls.Add(this.tbFtpPwd);
            this.tpNetwork.Controls.Add(this.cbFtpSsl);
            this.tpNetwork.Controls.Add(this.cbFtpAnonymous);
            this.tpNetwork.Controls.Add(this.lblFtpPath);
            this.tpNetwork.Controls.Add(this.tbFtpPath);
            this.tpNetwork.Controls.Add(this.btnFtpApply);
            this.tpNetwork.Controls.Add(this.btnFtpTest);
            this.tpNetwork.Controls.Add(this.lblHttpPort);
            this.tpNetwork.Controls.Add(this.nudHttpPort);
            this.tpNetwork.Controls.Add(this.lblRtspPort);
            this.tpNetwork.Controls.Add(this.nudRtspPort);
            this.tpNetwork.Controls.Add(this.lblRtpPort);
            this.tpNetwork.Controls.Add(this.nudRtpPort);
            this.tpNetwork.Controls.Add(this.btnPortApply);
            this.tpNetwork.Location = new System.Drawing.Point(4, 22);
            this.tpNetwork.Name = "tpNetwork";
            this.tpNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tpNetwork.Size = new System.Drawing.Size(478, 723);
            this.tpNetwork.TabIndex = 6;
            this.tpNetwork.Text = "Network";
            this.tpNetwork.UseVisualStyleBackColor = true;
            // 
            // lblNetDhcp
            // 
            this.lblNetDhcp.AutoSize = true;
            this.lblNetDhcp.Location = new System.Drawing.Point(15, 12);
            this.lblNetDhcp.Name = "lblNetDhcp";
            this.lblNetDhcp.Size = new System.Drawing.Size(29, 12);
            this.lblNetDhcp.TabIndex = 0;
            this.lblNetDhcp.Text = "DHCP";
            // 
            // cbNetDhcp
            // 
            this.cbNetDhcp.AutoSize = true;
            this.cbNetDhcp.Location = new System.Drawing.Point(110, 12);
            this.cbNetDhcp.Name = "cbNetDhcp";
            this.cbNetDhcp.Size = new System.Drawing.Size(15, 14);
            this.cbNetDhcp.TabIndex = 1;
            // 
            // lblNetIp
            // 
            this.lblNetIp.AutoSize = true;
            this.lblNetIp.Location = new System.Drawing.Point(15, 38);
            this.lblNetIp.Name = "lblNetIp";
            this.lblNetIp.Size = new System.Drawing.Size(17, 12);
            this.lblNetIp.TabIndex = 2;
            this.lblNetIp.Text = "IP";
            // 
            // tbNetIp
            // 
            this.tbNetIp.Location = new System.Drawing.Point(110, 35);
            this.tbNetIp.Name = "tbNetIp";
            this.tbNetIp.Size = new System.Drawing.Size(200, 21);
            this.tbNetIp.TabIndex = 3;
            // 
            // lblNetMask
            // 
            this.lblNetMask.AutoSize = true;
            this.lblNetMask.Location = new System.Drawing.Point(15, 64);
            this.lblNetMask.Name = "lblNetMask";
            this.lblNetMask.Size = new System.Drawing.Size(47, 12);
            this.lblNetMask.TabIndex = 4;
            this.lblNetMask.Text = "Netmask";
            // 
            // tbNetMask
            // 
            this.tbNetMask.Location = new System.Drawing.Point(110, 61);
            this.tbNetMask.Name = "tbNetMask";
            this.tbNetMask.Size = new System.Drawing.Size(200, 21);
            this.tbNetMask.TabIndex = 5;
            // 
            // lblNetGateway
            // 
            this.lblNetGateway.AutoSize = true;
            this.lblNetGateway.Location = new System.Drawing.Point(15, 90);
            this.lblNetGateway.Name = "lblNetGateway";
            this.lblNetGateway.Size = new System.Drawing.Size(47, 12);
            this.lblNetGateway.TabIndex = 6;
            this.lblNetGateway.Text = "Gateway";
            // 
            // tbNetGateway
            // 
            this.tbNetGateway.Location = new System.Drawing.Point(110, 87);
            this.tbNetGateway.Name = "tbNetGateway";
            this.tbNetGateway.Size = new System.Drawing.Size(200, 21);
            this.tbNetGateway.TabIndex = 7;
            // 
            // lblNetDns
            // 
            this.lblNetDns.AutoSize = true;
            this.lblNetDns.Location = new System.Drawing.Point(15, 116);
            this.lblNetDns.Name = "lblNetDns";
            this.lblNetDns.Size = new System.Drawing.Size(23, 12);
            this.lblNetDns.TabIndex = 8;
            this.lblNetDns.Text = "DNS";
            // 
            // tbNetDns
            // 
            this.tbNetDns.Location = new System.Drawing.Point(110, 113);
            this.tbNetDns.Name = "tbNetDns";
            this.tbNetDns.Size = new System.Drawing.Size(200, 21);
            this.tbNetDns.TabIndex = 9;
            // 
            // btnNetApply
            // 
            this.btnNetApply.Location = new System.Drawing.Point(110, 142);
            this.btnNetApply.Name = "btnNetApply";
            this.btnNetApply.Size = new System.Drawing.Size(90, 25);
            this.btnNetApply.TabIndex = 10;
            this.btnNetApply.Text = "Apply";
            this.btnNetApply.UseVisualStyleBackColor = true;
            this.btnNetApply.Click += new System.EventHandler(this.btnNetApply_Click);
            // 
            // btnNetRefresh
            // 
            this.btnNetRefresh.Location = new System.Drawing.Point(210, 142);
            this.btnNetRefresh.Name = "btnNetRefresh";
            this.btnNetRefresh.Size = new System.Drawing.Size(90, 25);
            this.btnNetRefresh.TabIndex = 11;
            this.btnNetRefresh.Text = "Refresh";
            this.btnNetRefresh.UseVisualStyleBackColor = true;
            this.btnNetRefresh.Click += new System.EventHandler(this.btnNetRefresh_Click);
            // 
            // lblFtpServer
            // 
            this.lblFtpServer.AutoSize = true;
            this.lblFtpServer.Location = new System.Drawing.Point(15, 185);
            this.lblFtpServer.Name = "lblFtpServer";
            this.lblFtpServer.Size = new System.Drawing.Size(65, 12);
            this.lblFtpServer.TabIndex = 12;
            this.lblFtpServer.Text = "FTP Server";
            // 
            // tbFtpServer
            // 
            this.tbFtpServer.Location = new System.Drawing.Point(110, 182);
            this.tbFtpServer.Name = "tbFtpServer";
            this.tbFtpServer.Size = new System.Drawing.Size(200, 21);
            this.tbFtpServer.TabIndex = 13;
            // 
            // lblFtpPort
            // 
            this.lblFtpPort.AutoSize = true;
            this.lblFtpPort.Location = new System.Drawing.Point(15, 211);
            this.lblFtpPort.Name = "lblFtpPort";
            this.lblFtpPort.Size = new System.Drawing.Size(53, 12);
            this.lblFtpPort.TabIndex = 14;
            this.lblFtpPort.Text = "FTP Port";
            // 
            // nudFtpPort
            // 
            this.nudFtpPort.Location = new System.Drawing.Point(110, 209);
            this.nudFtpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudFtpPort.Name = "nudFtpPort";
            this.nudFtpPort.Size = new System.Drawing.Size(100, 21);
            this.nudFtpPort.TabIndex = 15;
            this.nudFtpPort.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // lblFtpUser
            // 
            this.lblFtpUser.AutoSize = true;
            this.lblFtpUser.Location = new System.Drawing.Point(15, 237);
            this.lblFtpUser.Name = "lblFtpUser";
            this.lblFtpUser.Size = new System.Drawing.Size(29, 12);
            this.lblFtpUser.TabIndex = 16;
            this.lblFtpUser.Text = "User";
            // 
            // tbFtpUser
            // 
            this.tbFtpUser.Location = new System.Drawing.Point(110, 234);
            this.tbFtpUser.Name = "tbFtpUser";
            this.tbFtpUser.Size = new System.Drawing.Size(150, 21);
            this.tbFtpUser.TabIndex = 17;
            // 
            // lblFtpPwd
            // 
            this.lblFtpPwd.AutoSize = true;
            this.lblFtpPwd.Location = new System.Drawing.Point(15, 263);
            this.lblFtpPwd.Name = "lblFtpPwd";
            this.lblFtpPwd.Size = new System.Drawing.Size(53, 12);
            this.lblFtpPwd.TabIndex = 18;
            this.lblFtpPwd.Text = "Password";
            // 
            // tbFtpPwd
            // 
            this.tbFtpPwd.Location = new System.Drawing.Point(110, 260);
            this.tbFtpPwd.Name = "tbFtpPwd";
            this.tbFtpPwd.Size = new System.Drawing.Size(150, 21);
            this.tbFtpPwd.TabIndex = 19;
            this.tbFtpPwd.UseSystemPasswordChar = true;
            // 
            // cbFtpSsl
            // 
            this.cbFtpSsl.AutoSize = true;
            this.cbFtpSsl.Location = new System.Drawing.Point(110, 288);
            this.cbFtpSsl.Name = "cbFtpSsl";
            this.cbFtpSsl.Size = new System.Drawing.Size(66, 16);
            this.cbFtpSsl.TabIndex = 20;
            this.cbFtpSsl.Text = "Use SSL";
            // 
            // cbFtpAnonymous
            // 
            this.cbFtpAnonymous.AutoSize = true;
            this.cbFtpAnonymous.Location = new System.Drawing.Point(200, 288);
            this.cbFtpAnonymous.Name = "cbFtpAnonymous";
            this.cbFtpAnonymous.Size = new System.Drawing.Size(114, 16);
            this.cbFtpAnonymous.TabIndex = 21;
            this.cbFtpAnonymous.Text = "Anonymous Login";
            this.cbFtpAnonymous.UseVisualStyleBackColor = true;
            this.cbFtpAnonymous.CheckedChanged += new System.EventHandler(this.cbFtpAnonymous_CheckedChanged);
            // 
            // lblFtpPath
            // 
            this.lblFtpPath.AutoSize = true;
            this.lblFtpPath.Location = new System.Drawing.Point(15, 313);
            this.lblFtpPath.Name = "lblFtpPath";
            this.lblFtpPath.Size = new System.Drawing.Size(83, 12);
            this.lblFtpPath.TabIndex = 21;
            this.lblFtpPath.Text = "Path Template";
            // 
            // tbFtpPath
            // 
            this.tbFtpPath.Location = new System.Drawing.Point(110, 310);
            this.tbFtpPath.Name = "tbFtpPath";
            this.tbFtpPath.Size = new System.Drawing.Size(250, 21);
            this.tbFtpPath.TabIndex = 22;
            // 
            // btnFtpApply
            // 
            this.btnFtpApply.Location = new System.Drawing.Point(110, 340);
            this.btnFtpApply.Name = "btnFtpApply";
            this.btnFtpApply.Size = new System.Drawing.Size(90, 25);
            this.btnFtpApply.TabIndex = 23;
            this.btnFtpApply.Text = "Apply";
            this.btnFtpApply.UseVisualStyleBackColor = true;
            this.btnFtpApply.Click += new System.EventHandler(this.btnFtpApply_Click);
            // 
            // btnFtpTest
            // 
            this.btnFtpTest.Location = new System.Drawing.Point(210, 340);
            this.btnFtpTest.Name = "btnFtpTest";
            this.btnFtpTest.Size = new System.Drawing.Size(90, 25);
            this.btnFtpTest.TabIndex = 24;
            this.btnFtpTest.Text = "Test FTP";
            this.btnFtpTest.UseVisualStyleBackColor = true;
            this.btnFtpTest.Click += new System.EventHandler(this.btnFtpTest_Click);
            // 
            // lblHttpPort
            // 
            this.lblHttpPort.AutoSize = true;
            this.lblHttpPort.Location = new System.Drawing.Point(15, 385);
            this.lblHttpPort.Name = "lblHttpPort";
            this.lblHttpPort.Size = new System.Drawing.Size(59, 12);
            this.lblHttpPort.TabIndex = 24;
            this.lblHttpPort.Text = "HTTP Port";
            // 
            // nudHttpPort
            // 
            this.nudHttpPort.Location = new System.Drawing.Point(110, 383);
            this.nudHttpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudHttpPort.Name = "nudHttpPort";
            this.nudHttpPort.Size = new System.Drawing.Size(100, 21);
            this.nudHttpPort.TabIndex = 25;
            this.nudHttpPort.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // lblRtspPort
            // 
            this.lblRtspPort.AutoSize = true;
            this.lblRtspPort.Location = new System.Drawing.Point(15, 411);
            this.lblRtspPort.Name = "lblRtspPort";
            this.lblRtspPort.Size = new System.Drawing.Size(59, 12);
            this.lblRtspPort.TabIndex = 26;
            this.lblRtspPort.Text = "RTSP Port";
            // 
            // nudRtspPort
            // 
            this.nudRtspPort.Location = new System.Drawing.Point(110, 409);
            this.nudRtspPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudRtspPort.Name = "nudRtspPort";
            this.nudRtspPort.Size = new System.Drawing.Size(100, 21);
            this.nudRtspPort.TabIndex = 27;
            this.nudRtspPort.Value = new decimal(new int[] {
            554,
            0,
            0,
            0});
            // 
            // lblRtpPort
            // 
            this.lblRtpPort.AutoSize = true;
            this.lblRtpPort.Location = new System.Drawing.Point(15, 437);
            this.lblRtpPort.Name = "lblRtpPort";
            this.lblRtpPort.Size = new System.Drawing.Size(53, 12);
            this.lblRtpPort.TabIndex = 28;
            this.lblRtpPort.Text = "RTP Port";
            // 
            // nudRtpPort
            // 
            this.nudRtpPort.Location = new System.Drawing.Point(110, 435);
            this.nudRtpPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudRtpPort.Name = "nudRtpPort";
            this.nudRtpPort.Size = new System.Drawing.Size(100, 21);
            this.nudRtpPort.TabIndex = 29;
            this.nudRtpPort.Value = new decimal(new int[] {
            5004,
            0,
            0,
            0});
            // 
            // btnPortApply
            // 
            this.btnPortApply.Location = new System.Drawing.Point(110, 464);
            this.btnPortApply.Name = "btnPortApply";
            this.btnPortApply.Size = new System.Drawing.Size(90, 25);
            this.btnPortApply.TabIndex = 30;
            this.btnPortApply.Text = "Apply";
            this.btnPortApply.UseVisualStyleBackColor = true;
            this.btnPortApply.Click += new System.EventHandler(this.btnPortApply_Click);
            // 
            // tpUser
            // 
            this.tpUser.Controls.Add(this.lvUsers);
            this.tpUser.Controls.Add(this.lblUserName);
            this.tpUser.Controls.Add(this.tbUserName);
            this.tpUser.Controls.Add(this.lblUserPwd);
            this.tpUser.Controls.Add(this.tbUserPwd);
            this.tpUser.Controls.Add(this.lblUserLevel);
            this.tpUser.Controls.Add(this.cbUserLevel);
            this.tpUser.Controls.Add(this.lblUserLoginPwd);
            this.tpUser.Controls.Add(this.tbUserLoginPwd);
            this.tpUser.Controls.Add(this.btnUserRefresh);
            this.tpUser.Controls.Add(this.btnUserAdd);
            this.tpUser.Controls.Add(this.btnUserUpdate);
            this.tpUser.Controls.Add(this.btnUserDelete);
            this.tpUser.Location = new System.Drawing.Point(4, 22);
            this.tpUser.Name = "tpUser";
            this.tpUser.Padding = new System.Windows.Forms.Padding(3);
            this.tpUser.Size = new System.Drawing.Size(478, 723);
            this.tpUser.TabIndex = 7;
            this.tpUser.Text = "User";
            this.tpUser.UseVisualStyleBackColor = true;
            // 
            // lvUsers
            // 
            this.lvUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chUserId,
            this.chUserName,
            this.chUserLevel});
            this.lvUsers.FullRowSelect = true;
            this.lvUsers.GridLines = true;
            this.lvUsers.HideSelection = false;
            this.lvUsers.Location = new System.Drawing.Point(6, 6);
            this.lvUsers.Name = "lvUsers";
            this.lvUsers.Size = new System.Drawing.Size(466, 200);
            this.lvUsers.TabIndex = 0;
            this.lvUsers.UseCompatibleStateImageBehavior = false;
            this.lvUsers.View = System.Windows.Forms.View.Details;
            this.lvUsers.SelectedIndexChanged += new System.EventHandler(this.lvUsers_SelectedIndexChanged);
            // 
            // chUserId
            // 
            this.chUserId.Text = "ID";
            this.chUserId.Width = 50;
            // 
            // chUserName
            // 
            this.chUserName.Text = "Name";
            this.chUserName.Width = 150;
            // 
            // chUserLevel
            // 
            this.chUserLevel.Text = "Level";
            this.chUserLevel.Width = 100;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(15, 220);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(59, 12);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "User Name";
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(110, 217);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(200, 21);
            this.tbUserName.TabIndex = 2;
            // 
            // lblUserPwd
            // 
            this.lblUserPwd.AutoSize = true;
            this.lblUserPwd.Location = new System.Drawing.Point(15, 304);
            this.lblUserPwd.Name = "lblUserPwd";
            this.lblUserPwd.Size = new System.Drawing.Size(77, 12);
            this.lblUserPwd.TabIndex = 3;
            this.lblUserPwd.Text = "New Password";
            // 
            // tbUserPwd
            // 
            this.tbUserPwd.Location = new System.Drawing.Point(110, 301);
            this.tbUserPwd.Name = "tbUserPwd";
            this.tbUserPwd.Size = new System.Drawing.Size(200, 21);
            this.tbUserPwd.TabIndex = 4;
            this.tbUserPwd.UseSystemPasswordChar = true;
            // 
            // lblUserLevel
            // 
            this.lblUserLevel.AutoSize = true;
            this.lblUserLevel.Location = new System.Drawing.Point(15, 248);
            this.lblUserLevel.Name = "lblUserLevel";
            this.lblUserLevel.Size = new System.Drawing.Size(35, 12);
            this.lblUserLevel.TabIndex = 5;
            this.lblUserLevel.Text = "Level";
            // 
            // cbUserLevel
            // 
            this.cbUserLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUserLevel.Items.AddRange(new object[] {
            "Root",
            "Operator",
            "Viewer"});
            this.cbUserLevel.Location = new System.Drawing.Point(110, 245);
            this.cbUserLevel.Name = "cbUserLevel";
            this.cbUserLevel.Size = new System.Drawing.Size(150, 20);
            this.cbUserLevel.TabIndex = 6;
            // 
            // lblUserLoginPwd
            // 
            this.lblUserLoginPwd.AutoSize = true;
            this.lblUserLoginPwd.Location = new System.Drawing.Point(15, 276);
            this.lblUserLoginPwd.Name = "lblUserLoginPwd";
            this.lblUserLoginPwd.Size = new System.Drawing.Size(89, 12);
            this.lblUserLoginPwd.TabIndex = 7;
            this.lblUserLoginPwd.Text = "Login Password";
            // 
            // tbUserLoginPwd
            // 
            this.tbUserLoginPwd.Location = new System.Drawing.Point(110, 273);
            this.tbUserLoginPwd.Name = "tbUserLoginPwd";
            this.tbUserLoginPwd.Size = new System.Drawing.Size(200, 21);
            this.tbUserLoginPwd.TabIndex = 8;
            this.tbUserLoginPwd.UseSystemPasswordChar = true;
            // 
            // btnUserRefresh
            // 
            this.btnUserRefresh.Location = new System.Drawing.Point(15, 335);
            this.btnUserRefresh.Name = "btnUserRefresh";
            this.btnUserRefresh.Size = new System.Drawing.Size(80, 25);
            this.btnUserRefresh.TabIndex = 9;
            this.btnUserRefresh.Text = "Refresh";
            this.btnUserRefresh.UseVisualStyleBackColor = true;
            this.btnUserRefresh.Click += new System.EventHandler(this.btnUserRefresh_Click);
            // 
            // btnUserAdd
            // 
            this.btnUserAdd.Location = new System.Drawing.Point(105, 335);
            this.btnUserAdd.Name = "btnUserAdd";
            this.btnUserAdd.Size = new System.Drawing.Size(80, 25);
            this.btnUserAdd.TabIndex = 10;
            this.btnUserAdd.Text = "Add";
            this.btnUserAdd.UseVisualStyleBackColor = true;
            this.btnUserAdd.Click += new System.EventHandler(this.btnUserAdd_Click);
            // 
            // btnUserUpdate
            // 
            this.btnUserUpdate.Location = new System.Drawing.Point(195, 335);
            this.btnUserUpdate.Name = "btnUserUpdate";
            this.btnUserUpdate.Size = new System.Drawing.Size(80, 25);
            this.btnUserUpdate.TabIndex = 11;
            this.btnUserUpdate.Text = "Update";
            this.btnUserUpdate.UseVisualStyleBackColor = true;
            this.btnUserUpdate.Click += new System.EventHandler(this.btnUserUpdate_Click);
            // 
            // btnUserDelete
            // 
            this.btnUserDelete.Location = new System.Drawing.Point(285, 335);
            this.btnUserDelete.Name = "btnUserDelete";
            this.btnUserDelete.Size = new System.Drawing.Size(80, 25);
            this.btnUserDelete.TabIndex = 12;
            this.btnUserDelete.Text = "Delete";
            this.btnUserDelete.UseVisualStyleBackColor = true;
            this.btnUserDelete.Click += new System.EventHandler(this.btnUserDelete_Click);
            // 
            // tpDisplay
            // 
            this.tpDisplay.Controls.Add(this.lblChannelName);
            this.tpDisplay.Controls.Add(this.tbChannelName);
            this.tpDisplay.Controls.Add(this.btnSetChannelName);
            this.tpDisplay.Controls.Add(this.cbChNameEnabled);
            this.tpDisplay.Controls.Add(this.cbChNameAlign);
            this.tpDisplay.Controls.Add(this.tbChNameX);
            this.tpDisplay.Controls.Add(this.lblChNameXVal);
            this.tpDisplay.Controls.Add(this.tbChNameY);
            this.tpDisplay.Controls.Add(this.lblChNameYVal);
            this.tpDisplay.Controls.Add(this.cbTimeEnabled);
            this.tpDisplay.Controls.Add(this.cbTimeAlign);
            this.tpDisplay.Controls.Add(this.tbTimeX);
            this.tpDisplay.Controls.Add(this.lblTimeXVal);
            this.tpDisplay.Controls.Add(this.tbTimeY);
            this.tpDisplay.Controls.Add(this.lblTimeYVal);
            this.tpDisplay.Controls.Add(this.cbCustomEnabled);
            this.tpDisplay.Controls.Add(this.cbCustomAlign);
            this.tpDisplay.Controls.Add(this.cbCustomMode);
            this.tpDisplay.Controls.Add(this.lblCustomText);
            this.tpDisplay.Controls.Add(this.tbCustomText);
            this.tpDisplay.Controls.Add(this.tbCustomX);
            this.tpDisplay.Controls.Add(this.lblCustomXVal);
            this.tpDisplay.Controls.Add(this.tbCustomY);
            this.tpDisplay.Controls.Add(this.lblCustomYVal);
            this.tpDisplay.Controls.Add(this.lblContrast);
            this.tpDisplay.Controls.Add(this.nudContrast);
            this.tpDisplay.Controls.Add(this.lblBrightness);
            this.tpDisplay.Controls.Add(this.nudBrightness);
            this.tpDisplay.Controls.Add(this.cbDdeEnable);
            this.tpDisplay.Controls.Add(this.lblDdeValue);
            this.tpDisplay.Controls.Add(this.nudDdeValue);
            this.tpDisplay.Controls.Add(this.btnDisplayGet);
            this.tpDisplay.Controls.Add(this.btnDisplaySet);
            this.tpDisplay.Location = new System.Drawing.Point(4, 22);
            this.tpDisplay.Name = "tpDisplay";
            this.tpDisplay.Padding = new System.Windows.Forms.Padding(3);
            this.tpDisplay.Size = new System.Drawing.Size(478, 723);
            this.tpDisplay.TabIndex = 8;
            this.tpDisplay.Text = "Display";
            this.tpDisplay.UseVisualStyleBackColor = true;
            // 
            // lblChannelName
            // 
            this.lblChannelName.AutoSize = true;
            this.lblChannelName.Location = new System.Drawing.Point(15, 10);
            this.lblChannelName.Name = "lblChannelName";
            this.lblChannelName.Size = new System.Drawing.Size(47, 12);
            this.lblChannelName.TabIndex = 0;
            this.lblChannelName.Text = "Channel";
            // 
            // tbChannelName
            // 
            this.tbChannelName.Location = new System.Drawing.Point(80, 8);
            this.tbChannelName.Name = "tbChannelName";
            this.tbChannelName.Size = new System.Drawing.Size(180, 21);
            this.tbChannelName.TabIndex = 1;
            // 
            // btnSetChannelName
            // 
            this.btnSetChannelName.Location = new System.Drawing.Point(270, 7);
            this.btnSetChannelName.Name = "btnSetChannelName";
            this.btnSetChannelName.Size = new System.Drawing.Size(60, 23);
            this.btnSetChannelName.TabIndex = 2;
            this.btnSetChannelName.Text = "Set";
            this.btnSetChannelName.UseVisualStyleBackColor = true;
            this.btnSetChannelName.Click += new System.EventHandler(this.btnSetChannelName_Click);
            // 
            // cbChNameEnabled
            // 
            this.cbChNameEnabled.AutoSize = true;
            this.cbChNameEnabled.Location = new System.Drawing.Point(15, 38);
            this.cbChNameEnabled.Name = "cbChNameEnabled";
            this.cbChNameEnabled.Size = new System.Drawing.Size(96, 16);
            this.cbChNameEnabled.TabIndex = 3;
            this.cbChNameEnabled.Text = "Channel Name";
            // 
            // cbChNameAlign
            // 
            this.cbChNameAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChNameAlign.Items.AddRange(new object[] {
            "TopLeft",
            "TopRight",
            "BottomLeft",
            "BottomRight"});
            this.cbChNameAlign.Location = new System.Drawing.Point(130, 36);
            this.cbChNameAlign.Name = "cbChNameAlign";
            this.cbChNameAlign.Size = new System.Drawing.Size(90, 20);
            this.cbChNameAlign.TabIndex = 4;
            // 
            // tbChNameX
            // 
            this.tbChNameX.AutoSize = false;
            this.tbChNameX.Location = new System.Drawing.Point(230, 38);
            this.tbChNameX.Maximum = 640;
            this.tbChNameX.Name = "tbChNameX";
            this.tbChNameX.Size = new System.Drawing.Size(130, 20);
            this.tbChNameX.TabIndex = 5;
            this.tbChNameX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbChNameX.Scroll += new System.EventHandler(this.tbChNameX_Scroll);
            // 
            // lblChNameXVal
            // 
            this.lblChNameXVal.Location = new System.Drawing.Point(365, 40);
            this.lblChNameXVal.Name = "lblChNameXVal";
            this.lblChNameXVal.Size = new System.Drawing.Size(50, 15);
            this.lblChNameXVal.TabIndex = 6;
            this.lblChNameXVal.Text = "x:0";
            // 
            // tbChNameY
            // 
            this.tbChNameY.AutoSize = false;
            this.tbChNameY.Location = new System.Drawing.Point(230, 60);
            this.tbChNameY.Maximum = 480;
            this.tbChNameY.Name = "tbChNameY";
            this.tbChNameY.Size = new System.Drawing.Size(130, 20);
            this.tbChNameY.TabIndex = 7;
            this.tbChNameY.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbChNameY.Scroll += new System.EventHandler(this.tbChNameY_Scroll);
            // 
            // lblChNameYVal
            // 
            this.lblChNameYVal.Location = new System.Drawing.Point(365, 62);
            this.lblChNameYVal.Name = "lblChNameYVal";
            this.lblChNameYVal.Size = new System.Drawing.Size(50, 15);
            this.lblChNameYVal.TabIndex = 8;
            this.lblChNameYVal.Text = "y:0";
            // 
            // cbTimeEnabled
            // 
            this.cbTimeEnabled.AutoSize = true;
            this.cbTimeEnabled.Location = new System.Drawing.Point(15, 86);
            this.cbTimeEnabled.Name = "cbTimeEnabled";
            this.cbTimeEnabled.Size = new System.Drawing.Size(48, 16);
            this.cbTimeEnabled.TabIndex = 9;
            this.cbTimeEnabled.Text = "Time";
            // 
            // cbTimeAlign
            // 
            this.cbTimeAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTimeAlign.Items.AddRange(new object[] {
            "TopLeft",
            "TopRight",
            "BottomLeft",
            "BottomRight"});
            this.cbTimeAlign.Location = new System.Drawing.Point(130, 84);
            this.cbTimeAlign.Name = "cbTimeAlign";
            this.cbTimeAlign.Size = new System.Drawing.Size(90, 20);
            this.cbTimeAlign.TabIndex = 10;
            // 
            // tbTimeX
            // 
            this.tbTimeX.AutoSize = false;
            this.tbTimeX.Location = new System.Drawing.Point(230, 86);
            this.tbTimeX.Maximum = 640;
            this.tbTimeX.Name = "tbTimeX";
            this.tbTimeX.Size = new System.Drawing.Size(130, 20);
            this.tbTimeX.TabIndex = 11;
            this.tbTimeX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTimeX.Scroll += new System.EventHandler(this.tbTimeX_Scroll);
            // 
            // lblTimeXVal
            // 
            this.lblTimeXVal.Location = new System.Drawing.Point(365, 88);
            this.lblTimeXVal.Name = "lblTimeXVal";
            this.lblTimeXVal.Size = new System.Drawing.Size(50, 15);
            this.lblTimeXVal.TabIndex = 12;
            this.lblTimeXVal.Text = "x:0";
            // 
            // tbTimeY
            // 
            this.tbTimeY.AutoSize = false;
            this.tbTimeY.Location = new System.Drawing.Point(230, 108);
            this.tbTimeY.Maximum = 480;
            this.tbTimeY.Name = "tbTimeY";
            this.tbTimeY.Size = new System.Drawing.Size(130, 20);
            this.tbTimeY.TabIndex = 13;
            this.tbTimeY.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTimeY.Scroll += new System.EventHandler(this.tbTimeY_Scroll);
            // 
            // lblTimeYVal
            // 
            this.lblTimeYVal.Location = new System.Drawing.Point(365, 110);
            this.lblTimeYVal.Name = "lblTimeYVal";
            this.lblTimeYVal.Size = new System.Drawing.Size(50, 15);
            this.lblTimeYVal.TabIndex = 14;
            this.lblTimeYVal.Text = "y:0";
            // 
            // cbCustomEnabled
            // 
            this.cbCustomEnabled.AutoSize = true;
            this.cbCustomEnabled.Location = new System.Drawing.Point(15, 134);
            this.cbCustomEnabled.Name = "cbCustomEnabled";
            this.cbCustomEnabled.Size = new System.Drawing.Size(60, 16);
            this.cbCustomEnabled.TabIndex = 15;
            this.cbCustomEnabled.Text = "Custom";
            // 
            // cbCustomAlign
            // 
            this.cbCustomAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomAlign.Items.AddRange(new object[] {
            "TopLeft",
            "TopRight",
            "BottomLeft",
            "BottomRight"});
            this.cbCustomAlign.Location = new System.Drawing.Point(90, 132);
            this.cbCustomAlign.Name = "cbCustomAlign";
            this.cbCustomAlign.Size = new System.Drawing.Size(90, 20);
            this.cbCustomAlign.TabIndex = 16;
            // 
            // cbCustomMode
            // 
            this.cbCustomMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCustomMode.Items.AddRange(new object[] {
            "Light",
            "Blink"});
            this.cbCustomMode.Location = new System.Drawing.Point(190, 132);
            this.cbCustomMode.Name = "cbCustomMode";
            this.cbCustomMode.Size = new System.Drawing.Size(70, 20);
            this.cbCustomMode.TabIndex = 17;
            // 
            // lblCustomText
            // 
            this.lblCustomText.AutoSize = true;
            this.lblCustomText.Location = new System.Drawing.Point(15, 160);
            this.lblCustomText.Name = "lblCustomText";
            this.lblCustomText.Size = new System.Drawing.Size(29, 12);
            this.lblCustomText.TabIndex = 18;
            this.lblCustomText.Text = "Text";
            // 
            // tbCustomText
            // 
            this.tbCustomText.Location = new System.Drawing.Point(55, 156);
            this.tbCustomText.MaxLength = 127;
            this.tbCustomText.Name = "tbCustomText";
            this.tbCustomText.Size = new System.Drawing.Size(345, 21);
            this.tbCustomText.TabIndex = 19;
            // 
            // tbCustomX
            // 
            this.tbCustomX.AutoSize = false;
            this.tbCustomX.Location = new System.Drawing.Point(230, 182);
            this.tbCustomX.Maximum = 640;
            this.tbCustomX.Name = "tbCustomX";
            this.tbCustomX.Size = new System.Drawing.Size(130, 20);
            this.tbCustomX.TabIndex = 20;
            this.tbCustomX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbCustomX.Scroll += new System.EventHandler(this.tbCustomX_Scroll);
            // 
            // lblCustomXVal
            // 
            this.lblCustomXVal.Location = new System.Drawing.Point(365, 184);
            this.lblCustomXVal.Name = "lblCustomXVal";
            this.lblCustomXVal.Size = new System.Drawing.Size(50, 15);
            this.lblCustomXVal.TabIndex = 21;
            this.lblCustomXVal.Text = "x:0";
            // 
            // tbCustomY
            // 
            this.tbCustomY.AutoSize = false;
            this.tbCustomY.Location = new System.Drawing.Point(230, 204);
            this.tbCustomY.Maximum = 480;
            this.tbCustomY.Name = "tbCustomY";
            this.tbCustomY.Size = new System.Drawing.Size(130, 20);
            this.tbCustomY.TabIndex = 22;
            this.tbCustomY.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbCustomY.Scroll += new System.EventHandler(this.tbCustomY_Scroll);
            // 
            // lblCustomYVal
            // 
            this.lblCustomYVal.Location = new System.Drawing.Point(365, 206);
            this.lblCustomYVal.Name = "lblCustomYVal";
            this.lblCustomYVal.Size = new System.Drawing.Size(50, 15);
            this.lblCustomYVal.TabIndex = 23;
            this.lblCustomYVal.Text = "y:0";
            // 
            // tpThermalOsd
            // 
            this.tpThermalOsd.Controls.Add(this.thermalOsdPanel);
            this.tpThermalOsd.Location = new System.Drawing.Point(4, 22);
            this.tpThermalOsd.Name = "tpThermalOsd";
            this.tpThermalOsd.Padding = new System.Windows.Forms.Padding(3);
            this.tpThermalOsd.Size = new System.Drawing.Size(478, 723);
            this.tpThermalOsd.TabIndex = 9;
            this.tpThermalOsd.Text = "T.OSD";
            this.tpThermalOsd.UseVisualStyleBackColor = true;
            // 
            // thermalOsdPanel
            // 
            this.thermalOsdPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thermalOsdPanel.Location = new System.Drawing.Point(3, 3);
            this.thermalOsdPanel.Name = "thermalOsdPanel";
            this.thermalOsdPanel.Size = new System.Drawing.Size(472, 717);
            this.thermalOsdPanel.TabIndex = 0;
            // 
            // lblContrast
            // 
            this.lblContrast.AutoSize = true;
            this.lblContrast.Location = new System.Drawing.Point(15, 236);
            this.lblContrast.Name = "lblContrast";
            this.lblContrast.Size = new System.Drawing.Size(53, 12);
            this.lblContrast.TabIndex = 35;
            this.lblContrast.Text = "Contrast";
            // 
            // nudContrast
            // 
            this.nudContrast.Location = new System.Drawing.Point(110, 234);
            this.nudContrast.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudContrast.Name = "nudContrast";
            this.nudContrast.Size = new System.Drawing.Size(80, 21);
            this.nudContrast.TabIndex = 36;
            // 
            // lblBrightness
            // 
            this.lblBrightness.AutoSize = true;
            this.lblBrightness.Location = new System.Drawing.Point(200, 236);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(65, 12);
            this.lblBrightness.TabIndex = 37;
            this.lblBrightness.Text = "Brightness";
            // 
            // nudBrightness
            // 
            this.nudBrightness.Location = new System.Drawing.Point(280, 234);
            this.nudBrightness.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudBrightness.Name = "nudBrightness";
            this.nudBrightness.Size = new System.Drawing.Size(80, 21);
            this.nudBrightness.TabIndex = 38;
            // 
            // cbDdeEnable
            // 
            this.cbDdeEnable.AutoSize = true;
            this.cbDdeEnable.Location = new System.Drawing.Point(15, 264);
            this.cbDdeEnable.Name = "cbDdeEnable";
            this.cbDdeEnable.Size = new System.Drawing.Size(84, 16);
            this.cbDdeEnable.TabIndex = 39;
            this.cbDdeEnable.Text = "DDE Enable";
            // 
            // lblDdeValue
            // 
            this.lblDdeValue.AutoSize = true;
            this.lblDdeValue.Location = new System.Drawing.Point(140, 266);
            this.lblDdeValue.Name = "lblDdeValue";
            this.lblDdeValue.Size = new System.Drawing.Size(59, 12);
            this.lblDdeValue.TabIndex = 40;
            this.lblDdeValue.Text = "DDE Value";
            // 
            // nudDdeValue
            // 
            this.nudDdeValue.Location = new System.Drawing.Point(210, 264);
            this.nudDdeValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudDdeValue.Name = "nudDdeValue";
            this.nudDdeValue.Size = new System.Drawing.Size(80, 21);
            this.nudDdeValue.TabIndex = 41;
            // 
            this.btnDisplayGet.Location = new System.Drawing.Point(15, 292);
            this.btnDisplayGet.Name = "btnDisplayGet";
            this.btnDisplayGet.Size = new System.Drawing.Size(80, 25);
            this.btnDisplayGet.TabIndex = 43;
            this.btnDisplayGet.Text = "Get";
            this.btnDisplayGet.UseVisualStyleBackColor = true;
            this.btnDisplayGet.Click += new System.EventHandler(this.btnDisplayGet_Click);
            // 
            // btnDisplaySet
            // 
            this.btnDisplaySet.Location = new System.Drawing.Point(105, 292);
            this.btnDisplaySet.Name = "btnDisplaySet";
            this.btnDisplaySet.Size = new System.Drawing.Size(80, 25);
            this.btnDisplaySet.TabIndex = 44;
            this.btnDisplaySet.Text = "Set";
            this.btnDisplaySet.UseVisualStyleBackColor = true;
            this.btnDisplaySet.Click += new System.EventHandler(this.btnDisplaySet_Click);
            // 
            // tpAlarm
            // 
            this.tpAlarm.Controls.Add(this.lvAlarms);
            this.tpAlarm.Controls.Add(this.lblAlarmMarker);
            this.tpAlarm.Controls.Add(this.tbAlarmMarker);
            this.tpAlarm.Controls.Add(this.cbAlarmEnabled);
            this.tpAlarm.Controls.Add(this.lblAlarmType);
            this.tpAlarm.Controls.Add(this.cbAlarmType);
            this.tpAlarm.Controls.Add(this.lblAlarmTemp);
            this.tpAlarm.Controls.Add(this.nudAlarmTemp);
            this.tpAlarm.Controls.Add(this.lblAlarmTempUnit);
            this.tpAlarm.Controls.Add(this.lblAlarmDelay);
            this.tpAlarm.Controls.Add(this.nudAlarmDelay);
            this.tpAlarm.Controls.Add(this.lblAlarmDelayUnit);
            this.tpAlarm.Controls.Add(this.lblAlarmThreshTemp);
            this.tpAlarm.Controls.Add(this.nudAlarmThreshTemp);
            this.tpAlarm.Controls.Add(this.lblAlarmThreshTempUnit);
            this.tpAlarm.Controls.Add(this.lblAlarmThreshDelay);
            this.tpAlarm.Controls.Add(this.nudAlarmThreshDelay);
            this.tpAlarm.Controls.Add(this.lblAlarmThreshDelayUnit);
            this.tpAlarm.Controls.Add(this.lblAlarmTrigTemp);
            this.tpAlarm.Controls.Add(this.nudAlarmTrigTemp);
            this.tpAlarm.Controls.Add(this.lblAlarmTrigTempUnit);
            this.tpAlarm.Controls.Add(this.lblAlarmTrigDelay);
            this.tpAlarm.Controls.Add(this.nudAlarmTrigDelay);
            this.tpAlarm.Controls.Add(this.lblAlarmTrigDelayUnit);
            this.tpAlarm.Controls.Add(this.btnAlarmApply);
            this.tpAlarm.Controls.Add(this.lblAlarmInterval);
            this.tpAlarm.Controls.Add(this.nudAlarmInterval);
            this.tpAlarm.Controls.Add(this.lblAlarmIntervalUnit);
            this.tpAlarm.Controls.Add(this.cbAlarmFtpEnabled);
            this.tpAlarm.Controls.Add(this.btnAlarmGet);
            this.tpAlarm.Controls.Add(this.btnAlarmSet);
            this.tpAlarm.Location = new System.Drawing.Point(4, 22);
            this.tpAlarm.Name = "tpAlarm";
            this.tpAlarm.Padding = new System.Windows.Forms.Padding(3);
            this.tpAlarm.Size = new System.Drawing.Size(478, 723);
            this.tpAlarm.TabIndex = 9;
            this.tpAlarm.Text = "Alarm";
            this.tpAlarm.UseVisualStyleBackColor = true;
            // 
            // lvAlarms
            // 
            this.lvAlarms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chAlarmMarker,
            this.chAlarmEnabled,
            this.chAlarmType,
            this.chAlarmTemp,
            this.chAlarmDelay,
            this.chAlarmThreshTemp,
            this.chAlarmThreshDelay,
            this.chAlarmTrigTemp,
            this.chAlarmTrigDelay});
            this.lvAlarms.FullRowSelect = true;
            this.lvAlarms.GridLines = true;
            this.lvAlarms.HideSelection = false;
            this.lvAlarms.Location = new System.Drawing.Point(6, 6);
            this.lvAlarms.MultiSelect = false;
            this.lvAlarms.Name = "lvAlarms";
            this.lvAlarms.Size = new System.Drawing.Size(466, 220);
            this.lvAlarms.TabIndex = 0;
            this.lvAlarms.UseCompatibleStateImageBehavior = false;
            this.lvAlarms.View = System.Windows.Forms.View.Details;
            this.lvAlarms.SelectedIndexChanged += new System.EventHandler(this.lvAlarms_SelectedIndexChanged);
            // 
            // chAlarmMarker
            // 
            this.chAlarmMarker.Text = "Marker";
            this.chAlarmMarker.Width = 70;
            // 
            // chAlarmEnabled
            // 
            this.chAlarmEnabled.Text = "En";
            this.chAlarmEnabled.Width = 35;
            // 
            // chAlarmType
            // 
            this.chAlarmType.Text = "Type";
            this.chAlarmType.Width = 100;
            // 
            // chAlarmTemp
            // 
            this.chAlarmTemp.Text = "AlarmT";
            this.chAlarmTemp.Width = 55;
            // 
            // chAlarmDelay
            // 
            this.chAlarmDelay.Text = "ADelay";
            this.chAlarmDelay.Width = 50;
            // 
            // chAlarmThreshTemp
            // 
            this.chAlarmThreshTemp.Text = "WarnT";
            this.chAlarmThreshTemp.Width = 55;
            // 
            // chAlarmThreshDelay
            // 
            this.chAlarmThreshDelay.Text = "TDelay";
            this.chAlarmThreshDelay.Width = 50;
            // 
            // chAlarmTrigTemp
            // 
            this.chAlarmTrigTemp.Text = "AlertT";
            this.chAlarmTrigTemp.Width = 55;
            // 
            // chAlarmTrigDelay
            // 
            this.chAlarmTrigDelay.Text = "GDelay";
            this.chAlarmTrigDelay.Width = 50;
            // 
            // lblAlarmMarker
            // 
            this.lblAlarmMarker.AutoSize = true;
            this.lblAlarmMarker.Location = new System.Drawing.Point(9, 236);
            this.lblAlarmMarker.Name = "lblAlarmMarker";
            this.lblAlarmMarker.Size = new System.Drawing.Size(41, 12);
            this.lblAlarmMarker.TabIndex = 1;
            this.lblAlarmMarker.Text = "Marker";
            // 
            // tbAlarmMarker
            // 
            this.tbAlarmMarker.Location = new System.Drawing.Point(70, 233);
            this.tbAlarmMarker.Name = "tbAlarmMarker";
            this.tbAlarmMarker.ReadOnly = true;
            this.tbAlarmMarker.Size = new System.Drawing.Size(120, 21);
            this.tbAlarmMarker.TabIndex = 2;
            // 
            // cbAlarmEnabled
            // 
            this.cbAlarmEnabled.AutoSize = true;
            this.cbAlarmEnabled.Location = new System.Drawing.Point(210, 235);
            this.cbAlarmEnabled.Name = "cbAlarmEnabled";
            this.cbAlarmEnabled.Size = new System.Drawing.Size(66, 16);
            this.cbAlarmEnabled.TabIndex = 3;
            this.cbAlarmEnabled.Text = "Enabled";
            // 
            // lblAlarmType
            // 
            this.lblAlarmType.AutoSize = true;
            this.lblAlarmType.Location = new System.Drawing.Point(9, 266);
            this.lblAlarmType.Name = "lblAlarmType";
            this.lblAlarmType.Size = new System.Drawing.Size(29, 12);
            this.lblAlarmType.TabIndex = 4;
            this.lblAlarmType.Text = "Type";
            // 
            // cbAlarmType
            // 
            this.cbAlarmType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAlarmType.FormattingEnabled = true;
            this.cbAlarmType.Location = new System.Drawing.Point(70, 263);
            this.cbAlarmType.Name = "cbAlarmType";
            this.cbAlarmType.Size = new System.Drawing.Size(200, 20);
            this.cbAlarmType.TabIndex = 5;
            // 
            // lblAlarmTemp
            // 
            this.lblAlarmTemp.AutoSize = true;
            this.lblAlarmTemp.Location = new System.Drawing.Point(9, 296);
            this.lblAlarmTemp.Name = "lblAlarmTemp";
            this.lblAlarmTemp.Size = new System.Drawing.Size(59, 12);
            this.lblAlarmTemp.TabIndex = 6;
            this.lblAlarmTemp.Text = "Alarm Temp";
            // 
            // nudAlarmTemp
            // 
            this.nudAlarmTemp.DecimalPlaces = 1;
            this.nudAlarmTemp.Location = new System.Drawing.Point(90, 292);
            this.nudAlarmTemp.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudAlarmTemp.Minimum = new decimal(new int[] {
            273,
            0,
            0,
            -2147483648});
            this.nudAlarmTemp.Name = "nudAlarmTemp";
            this.nudAlarmTemp.Size = new System.Drawing.Size(70, 21);
            this.nudAlarmTemp.TabIndex = 7;
            // 
            // lblAlarmTempUnit
            // 
            this.lblAlarmTempUnit.AutoSize = true;
            this.lblAlarmTempUnit.Location = new System.Drawing.Point(164, 296);
            this.lblAlarmTempUnit.Name = "lblAlarmTempUnit";
            this.lblAlarmTempUnit.Size = new System.Drawing.Size(17, 12);
            this.lblAlarmTempUnit.TabIndex = 8;
            this.lblAlarmTempUnit.Text = "°C";
            // 
            // lblAlarmDelay
            // 
            this.lblAlarmDelay.AutoSize = true;
            this.lblAlarmDelay.Location = new System.Drawing.Point(200, 296);
            this.lblAlarmDelay.Name = "lblAlarmDelay";
            this.lblAlarmDelay.Size = new System.Drawing.Size(35, 12);
            this.lblAlarmDelay.TabIndex = 9;
            this.lblAlarmDelay.Text = "Delay";
            // 
            // nudAlarmDelay
            // 
            this.nudAlarmDelay.Location = new System.Drawing.Point(245, 292);
            this.nudAlarmDelay.Maximum = new decimal(new int[] {
            86400000,
            0,
            0,
            0});
            this.nudAlarmDelay.Name = "nudAlarmDelay";
            this.nudAlarmDelay.Size = new System.Drawing.Size(80, 21);
            this.nudAlarmDelay.TabIndex = 10;
            // 
            // lblAlarmDelayUnit
            // 
            this.lblAlarmDelayUnit.AutoSize = true;
            this.lblAlarmDelayUnit.Location = new System.Drawing.Point(329, 296);
            this.lblAlarmDelayUnit.Name = "lblAlarmDelayUnit";
            this.lblAlarmDelayUnit.Size = new System.Drawing.Size(11, 12);
            this.lblAlarmDelayUnit.TabIndex = 11;
            this.lblAlarmDelayUnit.Text = "s";
            // 
            // lblAlarmThreshTemp
            // 
            this.lblAlarmThreshTemp.AutoSize = true;
            this.lblAlarmThreshTemp.Location = new System.Drawing.Point(9, 326);
            this.lblAlarmThreshTemp.Name = "lblAlarmThreshTemp";
            this.lblAlarmThreshTemp.Size = new System.Drawing.Size(71, 12);
            this.lblAlarmThreshTemp.TabIndex = 12;
            this.lblAlarmThreshTemp.Text = "Warning Temp";
            // 
            // nudAlarmThreshTemp
            // 
            this.nudAlarmThreshTemp.DecimalPlaces = 1;
            this.nudAlarmThreshTemp.Location = new System.Drawing.Point(90, 322);
            this.nudAlarmThreshTemp.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudAlarmThreshTemp.Minimum = new decimal(new int[] {
            273,
            0,
            0,
            -2147483648});
            this.nudAlarmThreshTemp.Name = "nudAlarmThreshTemp";
            this.nudAlarmThreshTemp.Size = new System.Drawing.Size(70, 21);
            this.nudAlarmThreshTemp.TabIndex = 13;
            // 
            // lblAlarmThreshTempUnit
            // 
            this.lblAlarmThreshTempUnit.AutoSize = true;
            this.lblAlarmThreshTempUnit.Location = new System.Drawing.Point(164, 326);
            this.lblAlarmThreshTempUnit.Name = "lblAlarmThreshTempUnit";
            this.lblAlarmThreshTempUnit.Size = new System.Drawing.Size(17, 12);
            this.lblAlarmThreshTempUnit.TabIndex = 14;
            this.lblAlarmThreshTempUnit.Text = "°C";
            // 
            // lblAlarmThreshDelay
            // 
            this.lblAlarmThreshDelay.AutoSize = true;
            this.lblAlarmThreshDelay.Location = new System.Drawing.Point(200, 326);
            this.lblAlarmThreshDelay.Name = "lblAlarmThreshDelay";
            this.lblAlarmThreshDelay.Size = new System.Drawing.Size(35, 12);
            this.lblAlarmThreshDelay.TabIndex = 15;
            this.lblAlarmThreshDelay.Text = "Delay";
            // 
            // nudAlarmThreshDelay
            // 
            this.nudAlarmThreshDelay.Location = new System.Drawing.Point(245, 322);
            this.nudAlarmThreshDelay.Maximum = new decimal(new int[] {
            86400000,
            0,
            0,
            0});
            this.nudAlarmThreshDelay.Name = "nudAlarmThreshDelay";
            this.nudAlarmThreshDelay.Size = new System.Drawing.Size(80, 21);
            this.nudAlarmThreshDelay.TabIndex = 16;
            // 
            // lblAlarmThreshDelayUnit
            // 
            this.lblAlarmThreshDelayUnit.AutoSize = true;
            this.lblAlarmThreshDelayUnit.Location = new System.Drawing.Point(329, 326);
            this.lblAlarmThreshDelayUnit.Name = "lblAlarmThreshDelayUnit";
            this.lblAlarmThreshDelayUnit.Size = new System.Drawing.Size(11, 12);
            this.lblAlarmThreshDelayUnit.TabIndex = 17;
            this.lblAlarmThreshDelayUnit.Text = "s";
            // 
            // lblAlarmTrigTemp
            // 
            this.lblAlarmTrigTemp.AutoSize = true;
            this.lblAlarmTrigTemp.Location = new System.Drawing.Point(9, 356);
            this.lblAlarmTrigTemp.Name = "lblAlarmTrigTemp";
            this.lblAlarmTrigTemp.Size = new System.Drawing.Size(53, 12);
            this.lblAlarmTrigTemp.TabIndex = 18;
            this.lblAlarmTrigTemp.Text = "Alert Temp";
            // 
            // nudAlarmTrigTemp
            // 
            this.nudAlarmTrigTemp.DecimalPlaces = 1;
            this.nudAlarmTrigTemp.Location = new System.Drawing.Point(90, 352);
            this.nudAlarmTrigTemp.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudAlarmTrigTemp.Minimum = new decimal(new int[] {
            273,
            0,
            0,
            -2147483648});
            this.nudAlarmTrigTemp.Name = "nudAlarmTrigTemp";
            this.nudAlarmTrigTemp.Size = new System.Drawing.Size(70, 21);
            this.nudAlarmTrigTemp.TabIndex = 19;
            // 
            // lblAlarmTrigTempUnit
            // 
            this.lblAlarmTrigTempUnit.AutoSize = true;
            this.lblAlarmTrigTempUnit.Location = new System.Drawing.Point(164, 356);
            this.lblAlarmTrigTempUnit.Name = "lblAlarmTrigTempUnit";
            this.lblAlarmTrigTempUnit.Size = new System.Drawing.Size(17, 12);
            this.lblAlarmTrigTempUnit.TabIndex = 20;
            this.lblAlarmTrigTempUnit.Text = "°C";
            // 
            // lblAlarmTrigDelay
            // 
            this.lblAlarmTrigDelay.AutoSize = true;
            this.lblAlarmTrigDelay.Location = new System.Drawing.Point(200, 356);
            this.lblAlarmTrigDelay.Name = "lblAlarmTrigDelay";
            this.lblAlarmTrigDelay.Size = new System.Drawing.Size(35, 12);
            this.lblAlarmTrigDelay.TabIndex = 21;
            this.lblAlarmTrigDelay.Text = "Delay";
            // 
            // nudAlarmTrigDelay
            // 
            this.nudAlarmTrigDelay.Location = new System.Drawing.Point(245, 352);
            this.nudAlarmTrigDelay.Maximum = new decimal(new int[] {
            86400000,
            0,
            0,
            0});
            this.nudAlarmTrigDelay.Name = "nudAlarmTrigDelay";
            this.nudAlarmTrigDelay.Size = new System.Drawing.Size(80, 21);
            this.nudAlarmTrigDelay.TabIndex = 22;
            // 
            // lblAlarmTrigDelayUnit
            // 
            this.lblAlarmTrigDelayUnit.AutoSize = true;
            this.lblAlarmTrigDelayUnit.Location = new System.Drawing.Point(329, 356);
            this.lblAlarmTrigDelayUnit.Name = "lblAlarmTrigDelayUnit";
            this.lblAlarmTrigDelayUnit.Size = new System.Drawing.Size(11, 12);
            this.lblAlarmTrigDelayUnit.TabIndex = 23;
            this.lblAlarmTrigDelayUnit.Text = "s";
            // 
            // btnAlarmApply
            // 
            this.btnAlarmApply.Location = new System.Drawing.Point(360, 350);
            this.btnAlarmApply.Name = "btnAlarmApply";
            this.btnAlarmApply.Size = new System.Drawing.Size(100, 25);
            this.btnAlarmApply.TabIndex = 24;
            this.btnAlarmApply.Text = "Apply Selected";
            this.btnAlarmApply.UseVisualStyleBackColor = true;
            this.btnAlarmApply.Click += new System.EventHandler(this.btnAlarmApply_Click);
            // 
            // lblAlarmInterval
            // 
            this.lblAlarmInterval.AutoSize = true;
            this.lblAlarmInterval.Location = new System.Drawing.Point(9, 391);
            this.lblAlarmInterval.Name = "lblAlarmInterval";
            this.lblAlarmInterval.Size = new System.Drawing.Size(53, 12);
            this.lblAlarmInterval.TabIndex = 25;
            this.lblAlarmInterval.Text = "Interval";
            // 
            // nudAlarmInterval
            // 
            this.nudAlarmInterval.Location = new System.Drawing.Point(70, 387);
            this.nudAlarmInterval.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudAlarmInterval.Name = "nudAlarmInterval";
            this.nudAlarmInterval.Size = new System.Drawing.Size(70, 21);
            this.nudAlarmInterval.TabIndex = 26;
            // 
            // lblAlarmIntervalUnit
            // 
            this.lblAlarmIntervalUnit.AutoSize = true;
            this.lblAlarmIntervalUnit.Location = new System.Drawing.Point(144, 391);
            this.lblAlarmIntervalUnit.Name = "lblAlarmIntervalUnit";
            this.lblAlarmIntervalUnit.Size = new System.Drawing.Size(11, 12);
            this.lblAlarmIntervalUnit.TabIndex = 27;
            this.lblAlarmIntervalUnit.Text = "s";
            // 
            // cbAlarmFtpEnabled
            // 
            this.cbAlarmFtpEnabled.AutoSize = true;
            this.cbAlarmFtpEnabled.Location = new System.Drawing.Point(9, 420);
            this.cbAlarmFtpEnabled.Name = "cbAlarmFtpEnabled";
            this.cbAlarmFtpEnabled.Size = new System.Drawing.Size(90, 16);
            this.cbAlarmFtpEnabled.TabIndex = 28;
            this.cbAlarmFtpEnabled.Text = "FTP Trigger";
            // 
            // btnAlarmGet
            // 
            this.btnAlarmGet.Location = new System.Drawing.Point(6, 445);
            this.btnAlarmGet.Name = "btnAlarmGet";
            this.btnAlarmGet.Size = new System.Drawing.Size(80, 25);
            this.btnAlarmGet.TabIndex = 33;
            this.btnAlarmGet.Text = "Get";
            this.btnAlarmGet.UseVisualStyleBackColor = true;
            this.btnAlarmGet.Click += new System.EventHandler(this.btnAlarmGet_Click);
            // 
            // btnAlarmSet
            // 
            this.btnAlarmSet.Location = new System.Drawing.Point(96, 445);
            this.btnAlarmSet.Name = "btnAlarmSet";
            this.btnAlarmSet.Size = new System.Drawing.Size(80, 25);
            this.btnAlarmSet.TabIndex = 34;
            this.btnAlarmSet.Text = "Set";
            this.btnAlarmSet.UseVisualStyleBackColor = true;
            this.btnAlarmSet.Click += new System.EventHandler(this.btnAlarmSet_Click);
            // 
            // tpDpc
            // 
            this.tpDpc.Controls.Add(this.lblDpcResolution);
            this.tpDpc.Controls.Add(this.lblDpcHint);
            this.tpDpc.Controls.Add(this.lvDpcPoints);
            this.tpDpc.Controls.Add(this.lblDpcX);
            this.tpDpc.Controls.Add(this.nudDpcX);
            this.tpDpc.Controls.Add(this.lblDpcY);
            this.tpDpc.Controls.Add(this.nudDpcY);
            this.tpDpc.Controls.Add(this.btnDpcAdd);
            this.tpDpc.Controls.Add(this.btnDpcRemove);
            this.tpDpc.Controls.Add(this.btnDpcClear);
            this.tpDpc.Controls.Add(this.btnDpcSet);
            this.tpDpc.Location = new System.Drawing.Point(4, 22);
            this.tpDpc.Name = "tpDpc";
            this.tpDpc.Padding = new System.Windows.Forms.Padding(3);
            this.tpDpc.Size = new System.Drawing.Size(478, 723);
            this.tpDpc.TabIndex = 10;
            this.tpDpc.Text = "DPC";
            this.tpDpc.UseVisualStyleBackColor = true;
            // 
            // lblDpcResolution
            // 
            this.lblDpcResolution.AutoSize = true;
            this.lblDpcResolution.Location = new System.Drawing.Point(6, 10);
            this.lblDpcResolution.Name = "lblDpcResolution";
            this.lblDpcResolution.Size = new System.Drawing.Size(161, 12);
            this.lblDpcResolution.TabIndex = 0;
            this.lblDpcResolution.Text = "Thermal Resolution: 320x240";
            // 
            // lblDpcHint
            // 
            this.lblDpcHint.Location = new System.Drawing.Point(6, 28);
            this.lblDpcHint.Name = "lblDpcHint";
            this.lblDpcHint.Size = new System.Drawing.Size(466, 32);
            this.lblDpcHint.TabIndex = 1;
            this.lblDpcHint.Text = "Set sends only new points since last successful submit. Device data cannot be read back.";
            // 
            // lvDpcPoints
            // 
            this.lvDpcPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDpcIndex,
            this.chDpcX,
            this.chDpcY});
            this.lvDpcPoints.FullRowSelect = true;
            this.lvDpcPoints.GridLines = true;
            this.lvDpcPoints.HideSelection = false;
            this.lvDpcPoints.Location = new System.Drawing.Point(6, 66);
            this.lvDpcPoints.MultiSelect = false;
            this.lvDpcPoints.Name = "lvDpcPoints";
            this.lvDpcPoints.Size = new System.Drawing.Size(466, 280);
            this.lvDpcPoints.TabIndex = 2;
            this.lvDpcPoints.UseCompatibleStateImageBehavior = false;
            this.lvDpcPoints.View = System.Windows.Forms.View.Details;
            // 
            // chDpcIndex
            // 
            this.chDpcIndex.Text = "#";
            this.chDpcIndex.Width = 40;
            // 
            // chDpcX
            // 
            this.chDpcX.Text = "X";
            this.chDpcX.Width = 80;
            // 
            // chDpcY
            // 
            this.chDpcY.Text = "Y";
            this.chDpcY.Width = 80;
            // 
            // lblDpcX
            // 
            this.lblDpcX.AutoSize = true;
            this.lblDpcX.Location = new System.Drawing.Point(6, 360);
            this.lblDpcX.Name = "lblDpcX";
            this.lblDpcX.Size = new System.Drawing.Size(17, 12);
            this.lblDpcX.TabIndex = 3;
            this.lblDpcX.Text = "X:";
            // 
            // nudDpcX
            // 
            this.nudDpcX.Location = new System.Drawing.Point(30, 356);
            this.nudDpcX.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudDpcX.Name = "nudDpcX";
            this.nudDpcX.Size = new System.Drawing.Size(80, 21);
            this.nudDpcX.TabIndex = 4;
            // 
            // lblDpcY
            // 
            this.lblDpcY.AutoSize = true;
            this.lblDpcY.Location = new System.Drawing.Point(130, 360);
            this.lblDpcY.Name = "lblDpcY";
            this.lblDpcY.Size = new System.Drawing.Size(17, 12);
            this.lblDpcY.TabIndex = 5;
            this.lblDpcY.Text = "Y:";
            // 
            // nudDpcY
            // 
            this.nudDpcY.Location = new System.Drawing.Point(154, 356);
            this.nudDpcY.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudDpcY.Name = "nudDpcY";
            this.nudDpcY.Size = new System.Drawing.Size(80, 21);
            this.nudDpcY.TabIndex = 6;
            // 
            // btnDpcAdd
            // 
            this.btnDpcAdd.Location = new System.Drawing.Point(6, 390);
            this.btnDpcAdd.Name = "btnDpcAdd";
            this.btnDpcAdd.Size = new System.Drawing.Size(80, 25);
            this.btnDpcAdd.TabIndex = 7;
            this.btnDpcAdd.Text = "Add";
            this.btnDpcAdd.UseVisualStyleBackColor = true;
            this.btnDpcAdd.Click += new System.EventHandler(this.btnDpcAdd_Click);
            // 
            // btnDpcRemove
            // 
            this.btnDpcRemove.Location = new System.Drawing.Point(96, 390);
            this.btnDpcRemove.Name = "btnDpcRemove";
            this.btnDpcRemove.Size = new System.Drawing.Size(80, 25);
            this.btnDpcRemove.TabIndex = 8;
            this.btnDpcRemove.Text = "Remove";
            this.btnDpcRemove.UseVisualStyleBackColor = true;
            this.btnDpcRemove.Click += new System.EventHandler(this.btnDpcRemove_Click);
            // 
            // btnDpcClear
            // 
            this.btnDpcClear.Location = new System.Drawing.Point(186, 390);
            this.btnDpcClear.Name = "btnDpcClear";
            this.btnDpcClear.Size = new System.Drawing.Size(80, 25);
            this.btnDpcClear.TabIndex = 9;
            this.btnDpcClear.Text = "Clear All";
            this.btnDpcClear.UseVisualStyleBackColor = true;
            this.btnDpcClear.Click += new System.EventHandler(this.btnDpcClear_Click);
            // 
            // btnDpcSet
            // 
            this.btnDpcSet.Location = new System.Drawing.Point(6, 425);
            this.btnDpcSet.Name = "btnDpcSet";
            this.btnDpcSet.Size = new System.Drawing.Size(100, 25);
            this.btnDpcSet.TabIndex = 10;
            this.btnDpcSet.Text = "Set DPC";
            this.btnDpcSet.UseVisualStyleBackColor = true;
            this.btnDpcSet.Click += new System.EventHandler(this.btnDpcSet_Click);
            // 
            // cmsMarkers
            // 
            this.cmsMarkers.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsMarkers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLocalParameters});
            this.cmsMarkers.Name = "cmsMarkers";
            this.cmsMarkers.Size = new System.Drawing.Size(240, 26);
            // 
            // tsmiRename
            // 
            this.tsmiRename.Name = "tsmiRename";
            this.tsmiRename.Size = new System.Drawing.Size(239, 22);
            this.tsmiRename.Text = "Rename";
            this.tsmiRename.Visible = false;
            // 
            // tsmiLocalParameters
            // 
            this.tsmiLocalParameters.Name = "tsmiLocalParameters";
            this.tsmiLocalParameters.Size = new System.Drawing.Size(239, 22);
            this.tsmiLocalParameters.Text = "Change Local Parameters";
            this.tsmiLocalParameters.Click += new System.EventHandler(this.tsmiLocalParameters_Click);
            // 
            // tsmiDeltaParameters
            // 
            this.tsmiDeltaParameters.Name = "tsmiDeltaParameters";
            this.tsmiDeltaParameters.Size = new System.Drawing.Size(239, 22);
            this.tsmiDeltaParameters.Text = "Change Delta Parameters";
            this.tsmiDeltaParameters.Visible = false;
            // 
            // tsmiExportTemperature
            // 
            this.tsmiExportTemperature.Name = "tsmiExportTemperature";
            this.tsmiExportTemperature.Size = new System.Drawing.Size(239, 22);
            this.tsmiExportTemperature.Text = "Export Temperature...";
            this.tsmiExportTemperature.Visible = false;
            // 
            // tsmiExportTemperatureCurve
            // 
            this.tsmiExportTemperatureCurve.Name = "tsmiExportTemperatureCurve";
            this.tsmiExportTemperatureCurve.Size = new System.Drawing.Size(239, 22);
            this.tsmiExportTemperatureCurve.Text = "Export Temperature Curve...";
            this.tsmiExportTemperatureCurve.Visible = false;
            // 
            // ttTempValue
            // 
            this.ttTempValue.ShowAlways = true;
            // 
            // btnAutoAdjust
            // 
            this.btnAutoAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoAdjust.Location = new System.Drawing.Point(774, 601);
            this.btnAutoAdjust.Name = "btnAutoAdjust";
            this.btnAutoAdjust.Size = new System.Drawing.Size(89, 28);
            this.btnAutoAdjust.TabIndex = 20;
            this.btnAutoAdjust.Text = "Auto";
            this.btnAutoAdjust.UseVisualStyleBackColor = true;
            this.btnAutoAdjust.Click += new System.EventHandler(this.btnAutoAdjust_Click);
            // 
            // pbPalleteImage
            // 
            this.pbPalleteImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbPalleteImage.BackColor = System.Drawing.Color.White;
            this.pbPalleteImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPalleteImage.Location = new System.Drawing.Point(741, 58);
            this.pbPalleteImage.Margin = new System.Windows.Forms.Padding(0);
            this.pbPalleteImage.Name = "pbPalleteImage";
            this.pbPalleteImage.Size = new System.Drawing.Size(30, 571);
            this.pbPalleteImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPalleteImage.TabIndex = 17;
            this.pbPalleteImage.TabStop = false;
            // 
            // nudMax
            // 
            this.nudMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudMax.DecimalPlaces = 1;
            this.nudMax.Location = new System.Drawing.Point(774, 58);
            this.nudMax.Maximum = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            this.nudMax.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
            this.nudMax.Name = "nudMax";
            this.nudMax.Size = new System.Drawing.Size(60, 21);
            this.nudMax.TabIndex = 23;
            this.nudMax.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudMax.ValueChanged += new System.EventHandler(this.nudMax_ValueChanged);
            // 
            // nudMin
            // 
            this.nudMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nudMin.DecimalPlaces = 1;
            this.nudMin.Location = new System.Drawing.Point(774, 576);
            this.nudMin.Maximum = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            this.nudMin.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
            this.nudMin.Name = "nudMin";
            this.nudMin.Size = new System.Drawing.Size(60, 21);
            this.nudMin.TabIndex = 24;
            this.nudMin.Value = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudMin.ValueChanged += new System.EventHandler(this.nudMin_ValueChanged);
            // 
            // nudSpan
            // 
            this.nudSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSpan.DecimalPlaces = 1;
            this.nudSpan.Enabled = false;
            this.nudSpan.Location = new System.Drawing.Point(774, 107);
            this.nudSpan.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudSpan.Name = "nudSpan";
            this.nudSpan.Size = new System.Drawing.Size(60, 21);
            this.nudSpan.TabIndex = 29;
            this.nudSpan.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudSpan.ValueChanged += new System.EventHandler(this.nudSpan_ValueChanged);
            // 
            // cbFixedSpan
            // 
            this.cbFixedSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFixedSpan.AutoSize = true;
            this.cbFixedSpan.Location = new System.Drawing.Point(774, 89);
            this.cbFixedSpan.Name = "cbFixedSpan";
            this.cbFixedSpan.Size = new System.Drawing.Size(84, 16);
            this.cbFixedSpan.TabIndex = 30;
            this.cbFixedSpan.Text = "Fixed Span";
            this.cbFixedSpan.UseVisualStyleBackColor = true;
            this.cbFixedSpan.CheckedChanged += new System.EventHandler(this.cbFixedSpan_CheckedChanged);
            // 
            // lblMaxUnit
            // 
            this.lblMaxUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMaxUnit.AutoSize = true;
            this.lblMaxUnit.Location = new System.Drawing.Point(840, 111);
            this.lblMaxUnit.Name = "lblMaxUnit";
            this.lblMaxUnit.Size = new System.Drawing.Size(23, 12);
            this.lblMaxUnit.TabIndex = 38;
            this.lblMaxUnit.Text = "°C";
            // 
            // lblMinUnit
            // 
            this.lblMinUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMinUnit.AutoSize = true;
            this.lblMinUnit.Location = new System.Drawing.Point(840, 580);
            this.lblMinUnit.Name = "lblMinUnit";
            this.lblMinUnit.Size = new System.Drawing.Size(23, 12);
            this.lblMinUnit.TabIndex = 42;
            this.lblMinUnit.Text = "°C";
            // 
            // tPlay
            // 
            this.tPlay.Interval = 40;
            // 
            // mlvMarkers
            // 
            this.mlvMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlvMarkers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chValue,
            this.chMin,
            this.chMax,
            this.chAverage,
            this.chX1,
            this.chLocalParameters});
            this.mlvMarkers.ContextMenuStrip = this.cmsMarkers;
            this.mlvMarkers.FullRowSelect = true;
            this.mlvMarkers.GridLines = true;
            this.mlvMarkers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.mlvMarkers.HideSelection = false;
            this.mlvMarkers.Location = new System.Drawing.Point(12, 646);
            this.mlvMarkers.MultiSelect = false;
            this.mlvMarkers.Name = "mlvMarkers";
            this.mlvMarkers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mlvMarkers_MouseDown);
            this.mlvMarkers.DoubleClick += new System.EventHandler(this.mlvMarkers_DoubleClick);
            this.mlvMarkers.Size = new System.Drawing.Size(861, 162);
            this.mlvMarkers.TabIndex = 14;
            this.mlvMarkers.UseCompatibleStateImageBehavior = false;
            this.mlvMarkers.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 55;
            // 
            // chValue
            // 
            this.chValue.Text = "Value";
            this.chValue.Width = 100;
            // 
            // chMin
            // 
            this.chMin.Text = "Min";
            this.chMin.Width = 115;
            // 
            // chMax
            // 
            this.chMax.Text = "Max";
            this.chMax.Width = 115;
            // 
            // chAverage
            // 
            this.chAverage.Text = "Average";
            this.chAverage.Width = 72;
            // 
            // chX1
            // 
            this.chX1.Text = "count/points";
            this.chX1.Width = 185;
            // 
            // chLocalParameters
            // 
            this.chLocalParameters.Text = "Local Parameters";
            this.chLocalParameters.Width = 195;
            // 
            // tbTimeZone
            // 
            this.tbTimeZone.Location = new System.Drawing.Point(0, 0);
            this.tbTimeZone.Name = "tbTimeZone";
            this.tbTimeZone.Size = new System.Drawing.Size(100, 21);
            this.tbTimeZone.TabIndex = 0;
            this.tbTimeZone.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1377, 820);
            this.Controls.Add(this.lblMinUnit);
            this.Controls.Add(this.lblMaxUnit);
            this.Controls.Add(this.cbFixedSpan);
            this.Controls.Add(this.nudSpan);
            this.Controls.Add(this.nudMin);
            this.Controls.Add(this.nudMax);
            this.Controls.Add(this.btnAutoAdjust);
            this.Controls.Add(this.pbPalleteImage);
            this.Controls.Add(this.mlvMarkers);
            this.Controls.Add(this.tcThermalFile);
            this.Controls.Add(this.pbThermalImage);
            this.Controls.Add(this.tsMainForm);
            this.Controls.Add(this.tsConnection);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo ThermalFileReader";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tsConnection.ResumeLayout(false);
            this.tsConnection.PerformLayout();
            this.tsMainForm.ResumeLayout(false);
            this.tsMainForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThermalImage)).EndInit();
            this.tcThermalFile.ResumeLayout(false);
            this.tpProperties.ResumeLayout(false);
            this.tpProperties.PerformLayout();
            this.tpThermalParams.ResumeLayout(false);
            this.tpThermalParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRelHumidity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAtmTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShutterTime)).EndInit();
            this.tpTime.ResumeLayout(false);
            this.tpTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNtpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNtpInterval)).EndInit();
            this.tpNetwork.ResumeLayout(false);
            this.tpNetwork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFtpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHttpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRtspPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRtpPort)).EndInit();
            this.tpUser.ResumeLayout(false);
            this.tpUser.PerformLayout();
            this.tpDisplay.ResumeLayout(false);
            this.tpDisplay.PerformLayout();
            this.tpThermalOsd.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbChNameX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbChNameY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTimeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbCustomX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbCustomY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDdeValue)).EndInit();
            this.tpAlarm.ResumeLayout(false);
            this.tpAlarm.PerformLayout();
            this.tpDpc.ResumeLayout(false);
            this.tpDpc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDpcX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDpcY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmThreshTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmThreshDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmTrigTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmTrigDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmInterval)).EndInit();
            this.cmsMarkers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPalleteImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpan)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsConnection;
        private System.Windows.Forms.ToolStripLabel tslIP;
        private System.Windows.Forms.ToolStripTextBox tstbIP;
        private System.Windows.Forms.ToolStripLabel tslPort;
        private System.Windows.Forms.ToolStripTextBox tstbPort;
        private System.Windows.Forms.ToolStripLabel tslUser;
        private System.Windows.Forms.ToolStripTextBox tstbUser;
        private System.Windows.Forms.ToolStripLabel tslPassword;
        private System.Windows.Forms.ToolStripTextBox tstbPassword;
        private System.Windows.Forms.ToolStripButton tsbConnect;
        private System.Windows.Forms.ToolStrip tsMainForm;
        private System.Windows.Forms.ToolStripButton tsbCapture;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsbSelect;
        private System.Windows.Forms.ToolStripButton tsbSpot;
        private System.Windows.Forms.ToolStripButton tsbRectangle;
        private System.Windows.Forms.ToolStripButton tsbLine;
        private System.Windows.Forms.ToolStripButton tsbDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel tslPalette;
        private System.Windows.Forms.ToolStripComboBox tscbPalette;
        private System.Windows.Forms.ToolStripButton tsbInverted;
        private System.Windows.Forms.ToolStripLabel tslColorDistributionType;
        private System.Windows.Forms.ToolStripComboBox tscbColorDistributionType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton tsbCalibrate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripLabel tslStreamId;
        private System.Windows.Forms.ToolStripComboBox tscbStreamId;
        private System.Windows.Forms.PictureBox pbThermalImage;
        private System.Windows.Forms.TabControl tcThermalFile;
        private System.Windows.Forms.TabPage tpThermalParams;
        private System.Windows.Forms.Label lblRelHumidityUnit;
        private System.Windows.Forms.Label lblAtmTempUnit;
        private System.Windows.Forms.Label lblDistanceUnit;
        private System.Windows.Forms.Label lblReflTempUnit;
        private System.Windows.Forms.NumericUpDown nudRelHumidity;
        private System.Windows.Forms.NumericUpDown nudAtmTemp;
        private System.Windows.Forms.NumericUpDown nudDistance;
        private System.Windows.Forms.NumericUpDown nudReflTemp;
        private System.Windows.Forms.NumericUpDown nudEmissivity;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Label lblRelHumidity;
        private System.Windows.Forms.Label lblAtmTemp;
        private System.Windows.Forms.Label lblReflTemp;
        private System.Windows.Forms.Label lblEmissivity;
        private System.Windows.Forms.TabPage tpProperties;
        private System.Windows.Forms.TabPage tpTime;
        private System.Windows.Forms.Label lblTimeMode;
        private System.Windows.Forms.Label lblLocalTime;
        private System.Windows.Forms.Label lblTimeZone;
        private System.Windows.Forms.ComboBox cbTimeMode;
        private System.Windows.Forms.TextBox tbLocalTime;
        private System.Windows.Forms.TextBox tbTimeZone;
        private System.Windows.Forms.ComboBox cbTimeZone;
        private System.Windows.Forms.Label lblNtpHost;
        private System.Windows.Forms.TextBox tbNtpHost;
        private System.Windows.Forms.Label lblNtpPort;
        private System.Windows.Forms.NumericUpDown nudNtpPort;
        private System.Windows.Forms.Label lblNtpInterval;
        private System.Windows.Forms.NumericUpDown nudNtpInterval;
        private System.Windows.Forms.Button btnNtpApply;
        private System.Windows.Forms.TabPage tpNetwork;
        private System.Windows.Forms.Label lblNetDhcp;
        private System.Windows.Forms.CheckBox cbNetDhcp;
        private System.Windows.Forms.Label lblNetIp;
        private System.Windows.Forms.TextBox tbNetIp;
        private System.Windows.Forms.Label lblNetMask;
        private System.Windows.Forms.TextBox tbNetMask;
        private System.Windows.Forms.Label lblNetGateway;
        private System.Windows.Forms.TextBox tbNetGateway;
        private System.Windows.Forms.Label lblNetDns;
        private System.Windows.Forms.TextBox tbNetDns;
        private System.Windows.Forms.Button btnNetApply;
        private System.Windows.Forms.Button btnNetRefresh;
        private System.Windows.Forms.Label lblFtpServer;
        private System.Windows.Forms.TextBox tbFtpServer;
        private System.Windows.Forms.Label lblFtpPort;
        private System.Windows.Forms.NumericUpDown nudFtpPort;
        private System.Windows.Forms.Label lblFtpUser;
        private System.Windows.Forms.TextBox tbFtpUser;
        private System.Windows.Forms.Label lblFtpPwd;
        private System.Windows.Forms.TextBox tbFtpPwd;
        private System.Windows.Forms.CheckBox cbFtpSsl;
        private System.Windows.Forms.CheckBox cbFtpAnonymous;
        private System.Windows.Forms.Label lblFtpPath;
        private System.Windows.Forms.TextBox tbFtpPath;
        private System.Windows.Forms.Button btnFtpApply;
        private System.Windows.Forms.Button btnFtpTest;
        private System.Windows.Forms.Label lblHttpPort;
        private System.Windows.Forms.NumericUpDown nudHttpPort;
        private System.Windows.Forms.Label lblRtspPort;
        private System.Windows.Forms.NumericUpDown nudRtspPort;
        private System.Windows.Forms.Label lblRtpPort;
        private System.Windows.Forms.NumericUpDown nudRtpPort;
        private System.Windows.Forms.Button btnPortApply;
        private System.Windows.Forms.TextBox tbUpgradePath;
        private System.Windows.Forms.Button btnUpgradeBrowse;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button btnDeviceReset;
        private System.Windows.Forms.Button btnDeviceRestart;
        private System.Windows.Forms.Button btnPropertiesGet;
        private System.Windows.Forms.Label lblShutterMode;
        private System.Windows.Forms.ComboBox cbShutterMode;
        private System.Windows.Forms.Label lblShutterTime;
        private System.Windows.Forms.NumericUpDown nudShutterTime;
        private System.Windows.Forms.Label lblTempUnit;
        private System.Windows.Forms.ComboBox cbTempUnit;
        private System.Windows.Forms.Label lblDistUnit;
        private System.Windows.Forms.ComboBox cbDistUnit;
        private System.Windows.Forms.Button btnThermalParamsGet;
        private System.Windows.Forms.Button btnThermalParamsSet;
        private System.Windows.Forms.TabPage tpUser;
        private System.Windows.Forms.ListView lvUsers;
        private System.Windows.Forms.ColumnHeader chUserId;
        private System.Windows.Forms.ColumnHeader chUserName;
        private System.Windows.Forms.ColumnHeader chUserLevel;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.Label lblUserPwd;
        private System.Windows.Forms.TextBox tbUserPwd;
        private System.Windows.Forms.Label lblUserLevel;
        private System.Windows.Forms.ComboBox cbUserLevel;
        private System.Windows.Forms.Label lblUserLoginPwd;
        private System.Windows.Forms.TextBox tbUserLoginPwd;
        private System.Windows.Forms.Button btnUserRefresh;
        private System.Windows.Forms.Button btnUserAdd;
        private System.Windows.Forms.Button btnUserUpdate;
        private System.Windows.Forms.Button btnUserDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripLabel tslImageFlip;
        private System.Windows.Forms.ToolStripComboBox tscbImageFlip;
        private System.Windows.Forms.Button btnSyncTime;
        private System.Windows.Forms.Button btnGetTime;
        private System.Windows.Forms.Button btnApplyTime;
        private System.Windows.Forms.ListView lvProperties;
        private System.Windows.Forms.ColumnHeader chPropertyKey;
        private System.Windows.Forms.ColumnHeader chPropertyValue;
        private System.Windows.Forms.ListView mlvMarkers;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chValue;
        private System.Windows.Forms.ColumnHeader chMin;
        private System.Windows.Forms.ColumnHeader chMax;
        private System.Windows.Forms.ColumnHeader chAverage;
        private System.Windows.Forms.ColumnHeader chX1;
        private System.Windows.Forms.ContextMenuStrip cmsMarkers;
        private System.Windows.Forms.ToolStripMenuItem tsmiRename;
        private System.Windows.Forms.ToolStripMenuItem tsmiLocalParameters;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeltaParameters;
        private System.Windows.Forms.ColumnHeader chLocalParameters;
        private System.Windows.Forms.ToolTip ttTempValue;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportTemperature;
        private System.Windows.Forms.Button btnAutoAdjust;
        private System.Windows.Forms.PictureBox pbPalleteImage;
        private System.Windows.Forms.NumericUpDown nudMax;
        private System.Windows.Forms.NumericUpDown nudMin;
        private System.Windows.Forms.NumericUpDown nudSpan;
        private System.Windows.Forms.CheckBox cbFixedSpan;
        private System.Windows.Forms.Label lblMaxUnit;
        private System.Windows.Forms.Label lblMinUnit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton tsbPalette;
        private System.Windows.Forms.ToolStripLabel tslFileFormat;
        private System.Windows.Forms.ToolStripComboBox tscbFileFormat;
        private System.Windows.Forms.Timer tPlay;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportTemperatureCurve;
        private System.Windows.Forms.ToolStripButton tsbGetThermalData;
        private System.Windows.Forms.ToolStripButton tsbGetThermalLut;
        private System.Windows.Forms.TextBox tbDeviceName;
        private System.Windows.Forms.Button btnSetDeviceName;
        private System.Windows.Forms.TabPage tpDisplay;
        private System.Windows.Forms.TabPage tpThermalOsd;
        private ThermalOsdPanel thermalOsdPanel;
        private System.Windows.Forms.CheckBox cbChNameEnabled;
        private System.Windows.Forms.CheckBox cbTimeEnabled;
        private System.Windows.Forms.NumericUpDown nudContrast;
        private System.Windows.Forms.NumericUpDown nudBrightness;
        private System.Windows.Forms.CheckBox cbDdeEnable;
        private System.Windows.Forms.NumericUpDown nudDdeValue;
        private System.Windows.Forms.Label lblContrast;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.Label lblDdeValue;
        private System.Windows.Forms.Button btnDisplayGet;
        private System.Windows.Forms.Button btnDisplaySet;
        private System.Windows.Forms.Label lblChannelName;
        private System.Windows.Forms.TextBox tbChannelName;
        private System.Windows.Forms.Button btnSetChannelName;
        private System.Windows.Forms.ComboBox cbChNameAlign;
        private System.Windows.Forms.TrackBar tbChNameX;
        private System.Windows.Forms.TrackBar tbChNameY;
        private System.Windows.Forms.Label lblChNameXVal;
        private System.Windows.Forms.Label lblChNameYVal;
        private System.Windows.Forms.ComboBox cbTimeAlign;
        private System.Windows.Forms.TrackBar tbTimeX;
        private System.Windows.Forms.TrackBar tbTimeY;
        private System.Windows.Forms.Label lblTimeXVal;
        private System.Windows.Forms.Label lblTimeYVal;
        private System.Windows.Forms.CheckBox cbCustomEnabled;
        private System.Windows.Forms.ComboBox cbCustomAlign;
        private System.Windows.Forms.ComboBox cbCustomMode;
        private System.Windows.Forms.Label lblCustomText;
        private System.Windows.Forms.TextBox tbCustomText;
        private System.Windows.Forms.TrackBar tbCustomX;
        private System.Windows.Forms.TrackBar tbCustomY;
        private System.Windows.Forms.Label lblCustomXVal;
        private System.Windows.Forms.Label lblCustomYVal;
        private System.Windows.Forms.ToolStripSeparator tsActionSpacer;
        private System.Windows.Forms.TabPage tpAlarm;
        private System.Windows.Forms.ListView lvAlarms;
        private System.Windows.Forms.ColumnHeader chAlarmMarker;
        private System.Windows.Forms.ColumnHeader chAlarmEnabled;
        private System.Windows.Forms.ColumnHeader chAlarmType;
        private System.Windows.Forms.ColumnHeader chAlarmTemp;
        private System.Windows.Forms.ColumnHeader chAlarmDelay;
        private System.Windows.Forms.ColumnHeader chAlarmThreshTemp;
        private System.Windows.Forms.ColumnHeader chAlarmThreshDelay;
        private System.Windows.Forms.ColumnHeader chAlarmTrigTemp;
        private System.Windows.Forms.ColumnHeader chAlarmTrigDelay;
        private System.Windows.Forms.Label lblAlarmMarker;
        private System.Windows.Forms.TextBox tbAlarmMarker;
        private System.Windows.Forms.CheckBox cbAlarmEnabled;
        private System.Windows.Forms.Label lblAlarmType;
        private System.Windows.Forms.ComboBox cbAlarmType;
        private System.Windows.Forms.Label lblAlarmTemp;
        private System.Windows.Forms.NumericUpDown nudAlarmTemp;
        private System.Windows.Forms.Label lblAlarmTempUnit;
        private System.Windows.Forms.Label lblAlarmDelay;
        private System.Windows.Forms.NumericUpDown nudAlarmDelay;
        private System.Windows.Forms.Label lblAlarmDelayUnit;
        private System.Windows.Forms.Label lblAlarmThreshTemp;
        private System.Windows.Forms.NumericUpDown nudAlarmThreshTemp;
        private System.Windows.Forms.Label lblAlarmThreshTempUnit;
        private System.Windows.Forms.Label lblAlarmThreshDelay;
        private System.Windows.Forms.NumericUpDown nudAlarmThreshDelay;
        private System.Windows.Forms.Label lblAlarmThreshDelayUnit;
        private System.Windows.Forms.Label lblAlarmTrigTemp;
        private System.Windows.Forms.NumericUpDown nudAlarmTrigTemp;
        private System.Windows.Forms.Label lblAlarmTrigTempUnit;
        private System.Windows.Forms.Label lblAlarmTrigDelay;
        private System.Windows.Forms.NumericUpDown nudAlarmTrigDelay;
        private System.Windows.Forms.Label lblAlarmTrigDelayUnit;
        private System.Windows.Forms.Button btnAlarmApply;
        private System.Windows.Forms.Label lblAlarmInterval;
        private System.Windows.Forms.NumericUpDown nudAlarmInterval;
        private System.Windows.Forms.Label lblAlarmIntervalUnit;
        private System.Windows.Forms.CheckBox cbAlarmFtpEnabled;
        private System.Windows.Forms.Button btnAlarmGet;
        private System.Windows.Forms.Button btnAlarmSet;
        private System.Windows.Forms.TabPage tpDpc;
        private System.Windows.Forms.Label lblDpcResolution;
        private System.Windows.Forms.Label lblDpcHint;
        private System.Windows.Forms.ListView lvDpcPoints;
        private System.Windows.Forms.ColumnHeader chDpcIndex;
        private System.Windows.Forms.ColumnHeader chDpcX;
        private System.Windows.Forms.ColumnHeader chDpcY;
        private System.Windows.Forms.Label lblDpcX;
        private System.Windows.Forms.NumericUpDown nudDpcX;
        private System.Windows.Forms.Label lblDpcY;
        private System.Windows.Forms.NumericUpDown nudDpcY;
        private System.Windows.Forms.Button btnDpcAdd;
        private System.Windows.Forms.Button btnDpcRemove;
        private System.Windows.Forms.Button btnDpcClear;
        private System.Windows.Forms.Button btnDpcSet;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripLabel tslThermalStreamConfig;
        private System.Windows.Forms.ToolStripComboBox tscbThermalStreamConfig;
    }
}

