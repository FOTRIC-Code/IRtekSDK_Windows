namespace IRtekUVCSDKDemo
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cursorTempFont != null)
                    _cursorTempFont.Dispose();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Camera Info", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Lens Info", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Temp. Range", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Thermal", System.Windows.Forms.HorizontalAlignment.Left);
            this.tsConnection = new System.Windows.Forms.ToolStrip();
            this.tslDevice = new System.Windows.Forms.ToolStripLabel();
            this.tscbDeviceList = new System.Windows.Forms.ToolStripComboBox();
            this.tsbRefreshDevices = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLogin = new System.Windows.Forms.ToolStripButton();
            this.tsbLogout = new System.Windows.Forms.ToolStripButton();
            this.tsMainForm = new System.Windows.Forms.ToolStrip();
            this.tsbCapture = new System.Windows.Forms.ToolStripButton();
            this.tslRange = new System.Windows.Forms.ToolStripLabel();
            this.tscbRange = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSelect = new System.Windows.Forms.ToolStripButton();
            this.tsbSpot = new System.Windows.Forms.ToolStripButton();
            this.tsbRectangle = new System.Windows.Forms.ToolStripButton();
            this.tsbLine = new System.Windows.Forms.ToolStripButton();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2a = new System.Windows.Forms.ToolStripSeparator();
            this.tslPalette = new System.Windows.Forms.ToolStripLabel();
            this.tscbPalette = new System.Windows.Forms.ToolStripComboBox();
            this.tsbInverted = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tslImageFlip = new System.Windows.Forms.ToolStripLabel();
            this.tscbImageFlip = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCalibrate = new System.Windows.Forms.ToolStripButton();
            this.scMain = new System.Windows.Forms.SplitContainer();
            this.pbThermalImage = new System.Windows.Forms.PictureBox();
            this.mlvMarkers = new System.Windows.Forms.ListView();
            this.chMkName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkAverage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkX1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkY1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkX2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMkY2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.tlpSections = new System.Windows.Forms.TableLayoutPanel();
            this.grpProperties = new System.Windows.Forms.GroupBox();
            this.lvProperties = new System.Windows.Forms.ListView();
            this.chProperty = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlDeviceMgmt = new System.Windows.Forms.Panel();
            this.btnDeviceReset = new System.Windows.Forms.Button();
            this.btnDeviceRestart = new System.Windows.Forms.Button();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.btnUpgradeBrowse = new System.Windows.Forms.Button();
            this.tbUpgradePath = new System.Windows.Forms.TextBox();
            this.lblUpgradePath = new System.Windows.Forms.Label();
            this.grpThermal = new System.Windows.Forms.GroupBox();
            this.tlpThermalParams = new System.Windows.Forms.TableLayoutPanel();
            this.nudShutterTime = new System.Windows.Forms.NumericUpDown();
            this.lblShutterTimeUnit = new System.Windows.Forms.Label();
            this.lblShutterTime = new System.Windows.Forms.Label();
            this.cbShutterMode = new System.Windows.Forms.ComboBox();
            this.lblShutterMode = new System.Windows.Forms.Label();
            this.nudRelHumidity = new System.Windows.Forms.NumericUpDown();
            this.lblRelHumidityUnit = new System.Windows.Forms.Label();
            this.lblRelHumidity = new System.Windows.Forms.Label();
            this.nudAtmTemp = new System.Windows.Forms.NumericUpDown();
            this.lblAtmTempUnit = new System.Windows.Forms.Label();
            this.lblAtmTemp = new System.Windows.Forms.Label();
            this.nudDistance = new System.Windows.Forms.NumericUpDown();
            this.lblDistanceUnit = new System.Windows.Forms.Label();
            this.lblDistance = new System.Windows.Forms.Label();
            this.nudReflTemp = new System.Windows.Forms.NumericUpDown();
            this.lblReflTempUnit = new System.Windows.Forms.Label();
            this.lblReflTemp = new System.Windows.Forms.Label();
            this.nudEmissivity = new System.Windows.Forms.NumericUpDown();
            this.lblEmissivity = new System.Windows.Forms.Label();
            this.grpDisplay = new System.Windows.Forms.GroupBox();
            this.tlpDisplay = new System.Windows.Forms.TableLayoutPanel();
            this.btnDisplaySet = new System.Windows.Forms.Button();
            this.btnDisplayGet = new System.Windows.Forms.Button();
            this.cbSrEnable = new System.Windows.Forms.CheckBox();
            this.nudDdeValue = new System.Windows.Forms.NumericUpDown();
            this.lblDdeValue = new System.Windows.Forms.Label();
            this.cbDdeEnable = new System.Windows.Forms.CheckBox();
            this.nudBrightness = new System.Windows.Forms.NumericUpDown();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.nudContrast = new System.Windows.Forms.NumericUpDown();
            this.lblContrast = new System.Windows.Forms.Label();
            this.tsConnection.SuspendLayout();
            this.tsMainForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).BeginInit();
            this.scMain.Panel1.SuspendLayout();
            this.scMain.Panel2.SuspendLayout();
            this.scMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThermalImage)).BeginInit();
            this.pnlSettings.SuspendLayout();
            this.tlpSections.SuspendLayout();
            this.grpProperties.SuspendLayout();
            this.pnlDeviceMgmt.SuspendLayout();
            this.grpThermal.SuspendLayout();
            this.tlpThermalParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudShutterTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRelHumidity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAtmTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivity)).BeginInit();
            this.grpDisplay.SuspendLayout();
            this.tlpDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDdeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).BeginInit();
            this.SuspendLayout();
            // 
            // tsConnection
            // 
            this.tsConnection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslDevice,
            this.tscbDeviceList,
            this.tsbRefreshDevices,
            this.toolStripSeparator1,
            this.tsbLogin,
            this.tsbLogout});
            this.tsConnection.Location = new System.Drawing.Point(0, 0);
            this.tsConnection.Name = "tsConnection";
            this.tsConnection.Size = new System.Drawing.Size(1374, 25);
            this.tsConnection.TabIndex = 0;
            // 
            // tslDevice
            // 
            this.tslDevice.Name = "tslDevice";
            this.tslDevice.Size = new System.Drawing.Size(49, 22);
            this.tslDevice.Text = "Device:";
            // 
            // tscbDeviceList
            // 
            this.tscbDeviceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbDeviceList.Name = "tscbDeviceList";
            this.tscbDeviceList.Size = new System.Drawing.Size(250, 25);
            // 
            // tsbRefreshDevices
            // 
            this.tsbRefreshDevices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbRefreshDevices.Name = "tsbRefreshDevices";
            this.tsbRefreshDevices.Size = new System.Drawing.Size(56, 22);
            this.tsbRefreshDevices.Text = "Refresh";
            this.tsbRefreshDevices.Click += new System.EventHandler(this.tsbRefreshDevices_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbLogin
            // 
            this.tsbLogin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbLogin.Name = "tsbLogin";
            this.tsbLogin.Size = new System.Drawing.Size(44, 22);
            this.tsbLogin.Text = "Login";
            this.tsbLogin.Click += new System.EventHandler(this.tsbLogin_Click);
            // 
            // tsbLogout
            // 
            this.tsbLogout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbLogout.Name = "tsbLogout";
            this.tsbLogout.Size = new System.Drawing.Size(53, 22);
            this.tsbLogout.Text = "Logout";
            this.tsbLogout.Click += new System.EventHandler(this.tsbLogout_Click);
            // 
            // tsMainForm
            // 
            this.tsMainForm.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tsMainForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCapture,
            this.tslRange,
            this.tscbRange,
            this.toolStripSeparator4,
            this.tsbSelect,
            this.tsbSpot,
            this.tsbRectangle,
            this.tsbLine,
            this.tsbDelete,
            this.toolStripSeparator2a,
            this.tslPalette,
            this.tscbPalette,
            this.tsbInverted,
            this.toolStripSeparator5,
            this.toolStripSeparator7,
            this.tslImageFlip,
            this.tscbImageFlip,
            this.toolStripSeparator8,
            this.tsbCalibrate});
            this.tsMainForm.Location = new System.Drawing.Point(0, 25);
            this.tsMainForm.Name = "tsMainForm";
            this.tsMainForm.Size = new System.Drawing.Size(1374, 31);
            this.tsMainForm.TabIndex = 1;
            // 
            // tsbCapture
            // 
            this.tsbCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCapture.Name = "tsbCapture";
            this.tsbCapture.Size = new System.Drawing.Size(58, 28);
            this.tsbCapture.Text = "Capture";
            this.tsbCapture.Click += new System.EventHandler(this.tsbCapture_Click);
            // 
            // tslRange
            // 
            this.tslRange.Name = "tslRange";
            this.tslRange.Size = new System.Drawing.Size(71, 28);
            this.tslRange.Text = "Temp. Range:";
            // 
            // tscbRange
            // 
            this.tscbRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbRange.Name = "tscbRange";
            this.tscbRange.Size = new System.Drawing.Size(92, 31);
            this.tscbRange.SelectedIndexChanged += new System.EventHandler(this.tscbRange_SelectedIndexChanged);
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
            this.tsbSelect.Text = "Select";
            this.tsbSelect.Click += new System.EventHandler(this.tsbSelect_Click);
            // 
            // tsbSpot
            // 
            this.tsbSpot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSpot.Image = ((System.Drawing.Image)(resources.GetObject("tsbSpot.Image")));
            this.tsbSpot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSpot.Name = "tsbSpot";
            this.tsbSpot.Size = new System.Drawing.Size(28, 28);
            this.tsbSpot.Text = "Spot";
            this.tsbSpot.Click += new System.EventHandler(this.tsbSpot_Click);
            // 
            // tsbRectangle
            // 
            this.tsbRectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRectangle.Image = ((System.Drawing.Image)(resources.GetObject("tsbRectangle.Image")));
            this.tsbRectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRectangle.Name = "tsbRectangle";
            this.tsbRectangle.Size = new System.Drawing.Size(28, 28);
            this.tsbRectangle.Text = "Rect";
            this.tsbRectangle.Click += new System.EventHandler(this.tsbRectangle_Click);
            // 
            // tsbLine
            // 
            this.tsbLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLine.Image = ((System.Drawing.Image)(resources.GetObject("tsbLine.Image")));
            this.tsbLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLine.Name = "tsbLine";
            this.tsbLine.Size = new System.Drawing.Size(28, 28);
            this.tsbLine.Text = "Line";
            this.tsbLine.Click += new System.EventHandler(this.tsbLine_Click);
            // 
            // tsbDelete
            // 
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelete.Image = ((System.Drawing.Image)(resources.GetObject("tsbDelete.Image")));
            this.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(28, 28);
            this.tsbDelete.Text = "Delete";
            this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            // 
            // toolStripSeparator2a
            // 
            this.toolStripSeparator2a.Name = "toolStripSeparator2a";
            this.toolStripSeparator2a.Size = new System.Drawing.Size(6, 31);
            // 
            // tslPalette
            // 
            this.tslPalette.Name = "tslPalette";
            this.tslPalette.Size = new System.Drawing.Size(47, 28);
            this.tslPalette.Text = "Palette";
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
            this.tsbInverted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInverted.Name = "tsbInverted";
            this.tsbInverted.Size = new System.Drawing.Size(60, 28);
            this.tsbInverted.Text = "Inverted";
            this.tsbInverted.Click += new System.EventHandler(this.tsbInverted_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
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
            this.tscbImageFlip.AutoSize = false;
            this.tscbImageFlip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbImageFlip.DropDownWidth = 120;
            this.tscbImageFlip.Items.AddRange(new object[] {
            "None",
            "Up-Down",
            "Left-Right",
            "Center"});
            this.tscbImageFlip.Name = "tscbImageFlip";
            this.tscbImageFlip.Size = new System.Drawing.Size(110, 25);
            this.tscbImageFlip.SelectedIndexChanged += new System.EventHandler(this.tscbImageFlip_SelectedIndexChanged);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbCalibrate
            // 
            this.tsbCalibrate.BackColor = System.Drawing.Color.LightCoral;
            this.tsbCalibrate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCalibrate.Name = "tsbCalibrate";
            this.tsbCalibrate.Size = new System.Drawing.Size(64, 28);
            this.tsbCalibrate.Text = "Calibrate";
            this.tsbCalibrate.Click += new System.EventHandler(this.tsbCalibrate_Click);
            // 
            // scMain
            // 
            this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMain.Location = new System.Drawing.Point(0, 56);
            this.scMain.Name = "scMain";
            // 
            // scMain.Panel1
            // 
            this.scMain.Panel1.Controls.Add(this.pbThermalImage);
            this.scMain.Panel1.Controls.Add(this.mlvMarkers);
            // 
            // scMain.Panel2
            // 
            this.scMain.Panel2.Controls.Add(this.pnlSettings);
            this.scMain.Size = new System.Drawing.Size(1374, 693);
            this.scMain.SplitterDistance = 858;
            this.scMain.TabIndex = 2;
            // 
            // pbThermalImage
            // 
            this.pbThermalImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbThermalImage.BackColor = System.Drawing.SystemColors.Window;
            this.pbThermalImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbThermalImage.Location = new System.Drawing.Point(0, 0);
            this.pbThermalImage.Name = "pbThermalImage";
            this.pbThermalImage.Size = new System.Drawing.Size(855, 560);
            this.pbThermalImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbThermalImage.TabIndex = 0;
            this.pbThermalImage.TabStop = false;
            this.pbThermalImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbThermalImage_MouseDown);
            this.pbThermalImage.MouseLeave += new System.EventHandler(this.pbThermalImage_MouseLeave);
            this.pbThermalImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbThermalImage_MouseMove);
            this.pbThermalImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbThermalImage_MouseUp);
            // 
            // mlvMarkers
            // 
            this.mlvMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlvMarkers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chMkName,
            this.chMkValue,
            this.chMkMin,
            this.chMkMax,
            this.chMkAverage,
            this.chMkX1,
            this.chMkY1,
            this.chMkX2,
            this.chMkY2});
            this.mlvMarkers.FullRowSelect = true;
            this.mlvMarkers.HideSelection = false;
            this.mlvMarkers.Location = new System.Drawing.Point(0, 566);
            this.mlvMarkers.MultiSelect = false;
            this.mlvMarkers.Name = "mlvMarkers";
            this.mlvMarkers.Size = new System.Drawing.Size(855, 120);
            this.mlvMarkers.TabIndex = 3;
            this.mlvMarkers.UseCompatibleStateImageBehavior = false;
            this.mlvMarkers.View = System.Windows.Forms.View.Details;
            // 
            // chMkName
            // 
            this.chMkName.Text = "Name";
            // 
            // chMkValue
            // 
            this.chMkValue.Text = "Value";
            // 
            // chMkMin
            // 
            this.chMkMin.Text = "Min";
            // 
            // chMkMax
            // 
            this.chMkMax.Text = "Max";
            // 
            // chMkAverage
            // 
            this.chMkAverage.Text = "Average";
            // 
            // chMkX1
            // 
            this.chMkX1.Text = "X1";
            this.chMkX1.Width = 60;
            // 
            // chMkY1
            // 
            this.chMkY1.Text = "Y1";
            this.chMkY1.Width = 60;
            // 
            // chMkX2
            // 
            this.chMkX2.Text = "X2";
            this.chMkX2.Width = 60;
            // 
            // chMkY2
            // 
            this.chMkY2.Text = "Y2";
            this.chMkY2.Width = 60;
            // 
            // pnlSettings
            // 
            this.pnlSettings.AutoScroll = true;
            this.pnlSettings.Controls.Add(this.tlpSections);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(512, 693);
            this.pnlSettings.TabIndex = 0;
            // 
            // tlpSections
            // 
            this.tlpSections.ColumnCount = 1;
            this.tlpSections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpSections.Location = new System.Drawing.Point(0, 0);
            this.tlpSections.Name = "tlpSections";
            this.tlpSections.RowCount = 1;
            this.tlpSections.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSections.Size = new System.Drawing.Size(512, 10);
            this.tlpSections.TabIndex = 0;
            // 
            // grpProperties
            // 
            this.grpProperties.Controls.Add(this.lvProperties);
            this.grpProperties.Controls.Add(this.pnlDeviceMgmt);
            this.grpProperties.Location = new System.Drawing.Point(4, 4);
            this.grpProperties.Name = "grpProperties";
            this.grpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.grpProperties.Size = new System.Drawing.Size(504, 260);
            this.grpProperties.TabIndex = 0;
            this.grpProperties.TabStop = false;
            this.grpProperties.Text = "Device Properties";
            // 
            // lvProperties
            // 
            this.lvProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chProperty,
            this.chValue});
            this.lvProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvProperties.FullRowSelect = true;
            this.lvProperties.GridLines = true;
            listViewGroup1.Header = "Camera Info";
            listViewGroup1.Name = "lvgCameraInfo";
            listViewGroup2.Header = "Lens Info";
            listViewGroup2.Name = "lvgLensInfo";
            listViewGroup3.Header = "Temp. Range";
            listViewGroup3.Name = "lvgTempRange";
            listViewGroup4.Header = "Thermal";
            listViewGroup4.Name = "lvgThermal";
            this.lvProperties.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4});
            this.lvProperties.HideSelection = false;
            this.lvProperties.Location = new System.Drawing.Point(3, 3);
            this.lvProperties.Name = "lvProperties";
            this.lvProperties.Size = new System.Drawing.Size(498, 590);
            this.lvProperties.TabIndex = 0;
            this.lvProperties.UseCompatibleStateImageBehavior = false;
            this.lvProperties.View = System.Windows.Forms.View.Details;
            // 
            // chProperty
            // 
            this.chProperty.Text = "Property";
            this.chProperty.Width = 110;
            // 
            // chValue
            // 
            this.chValue.Text = "Value";
            this.chValue.Width = 280;
            // 
            // pnlDeviceMgmt
            // 
            this.pnlDeviceMgmt.Controls.Add(this.btnDeviceReset);
            this.pnlDeviceMgmt.Controls.Add(this.btnDeviceRestart);
            this.pnlDeviceMgmt.Controls.Add(this.btnUpgrade);
            this.pnlDeviceMgmt.Controls.Add(this.btnUpgradeBrowse);
            this.pnlDeviceMgmt.Controls.Add(this.tbUpgradePath);
            this.pnlDeviceMgmt.Controls.Add(this.lblUpgradePath);
            this.pnlDeviceMgmt.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDeviceMgmt.Location = new System.Drawing.Point(3, 593);
            this.pnlDeviceMgmt.Name = "pnlDeviceMgmt";
            this.pnlDeviceMgmt.Size = new System.Drawing.Size(498, 71);
            this.pnlDeviceMgmt.TabIndex = 1;
            // 
            // btnDeviceReset
            // 
            this.btnDeviceReset.BackColor = System.Drawing.Color.LightCoral;
            this.btnDeviceReset.Location = new System.Drawing.Point(168, 42);
            this.btnDeviceReset.Name = "btnDeviceReset";
            this.btnDeviceReset.Size = new System.Drawing.Size(75, 21);
            this.btnDeviceReset.TabIndex = 5;
            this.btnDeviceReset.Text = "Reset";
            this.btnDeviceReset.UseVisualStyleBackColor = false;
            this.btnDeviceReset.Click += new System.EventHandler(this.btnDeviceReset_Click);
            // 
            // btnDeviceRestart
            // 
            this.btnDeviceRestart.Location = new System.Drawing.Point(80, 42);
            this.btnDeviceRestart.Name = "btnDeviceRestart";
            this.btnDeviceRestart.Size = new System.Drawing.Size(75, 21);
            this.btnDeviceRestart.TabIndex = 4;
            this.btnDeviceRestart.Text = "Restart";
            this.btnDeviceRestart.UseVisualStyleBackColor = true;
            this.btnDeviceRestart.Click += new System.EventHandler(this.btnDeviceRestart_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(6, 42);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(65, 21);
            this.btnUpgrade.TabIndex = 3;
            this.btnUpgrade.Text = "Upgrade";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // btnUpgradeBrowse
            // 
            this.btnUpgradeBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpgradeBrowse.Location = new System.Drawing.Point(465, 13);
            this.btnUpgradeBrowse.Name = "btnUpgradeBrowse";
            this.btnUpgradeBrowse.Size = new System.Drawing.Size(30, 21);
            this.btnUpgradeBrowse.TabIndex = 2;
            this.btnUpgradeBrowse.Text = "...";
            this.btnUpgradeBrowse.UseVisualStyleBackColor = true;
            this.btnUpgradeBrowse.Click += new System.EventHandler(this.btnUpgradeBrowse_Click);
            // 
            // tbUpgradePath
            // 
            this.tbUpgradePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUpgradePath.Location = new System.Drawing.Point(80, 14);
            this.tbUpgradePath.Name = "tbUpgradePath";
            this.tbUpgradePath.Size = new System.Drawing.Size(379, 21);
            this.tbUpgradePath.TabIndex = 1;
            // 
            // lblUpgradePath
            // 
            this.lblUpgradePath.AutoSize = true;
            this.lblUpgradePath.Location = new System.Drawing.Point(6, 17);
            this.lblUpgradePath.Name = "lblUpgradePath";
            this.lblUpgradePath.Size = new System.Drawing.Size(59, 12);
            this.lblUpgradePath.TabIndex = 0;
            this.lblUpgradePath.Text = "Firmware:";
            // 
            // grpThermal
            // 
            this.grpThermal.Controls.Add(this.tlpThermalParams);
            this.grpThermal.Location = new System.Drawing.Point(4, 270);
            this.grpThermal.Name = "grpThermal";
            this.grpThermal.Padding = new System.Windows.Forms.Padding(3);
            this.grpThermal.Size = new System.Drawing.Size(504, 250);
            this.grpThermal.TabIndex = 1;
            this.grpThermal.TabStop = false;
            this.grpThermal.Text = "Thermal Parameters";
            // 
            // tlpThermalParams
            // 
            this.tlpThermalParams.AutoSize = true;
            this.tlpThermalParams.ColumnCount = 6;
            this.tlpThermalParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpThermalParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tlpThermalParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpThermalParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpThermalParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tlpThermalParams.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpThermalParams.Controls.Add(this.lblEmissivity, 0, 0);
            this.tlpThermalParams.Controls.Add(this.nudEmissivity, 1, 0);
            this.tlpThermalParams.Controls.Add(this.lblReflTemp, 3, 0);
            this.tlpThermalParams.Controls.Add(this.nudReflTemp, 4, 0);
            this.tlpThermalParams.Controls.Add(this.lblReflTempUnit, 5, 0);
            this.tlpThermalParams.Controls.Add(this.lblDistance, 0, 1);
            this.tlpThermalParams.Controls.Add(this.nudDistance, 1, 1);
            this.tlpThermalParams.Controls.Add(this.lblDistanceUnit, 2, 1);
            this.tlpThermalParams.Controls.Add(this.lblAtmTemp, 3, 1);
            this.tlpThermalParams.Controls.Add(this.nudAtmTemp, 4, 1);
            this.tlpThermalParams.Controls.Add(this.lblAtmTempUnit, 5, 1);
            this.tlpThermalParams.Controls.Add(this.lblRelHumidity, 0, 2);
            this.tlpThermalParams.Controls.Add(this.nudRelHumidity, 1, 2);
            this.tlpThermalParams.Controls.Add(this.lblRelHumidityUnit, 2, 2);
            this.tlpThermalParams.Controls.Add(this.lblShutterMode, 3, 2);
            this.tlpThermalParams.Controls.Add(this.cbShutterMode, 4, 2);
            this.tlpThermalParams.Controls.Add(this.lblShutterTime, 0, 3);
            this.tlpThermalParams.Controls.Add(this.nudShutterTime, 1, 3);
            this.tlpThermalParams.Controls.Add(this.lblShutterTimeUnit, 2, 3);
            this.tlpThermalParams.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpThermalParams.Name = "tlpThermalParams";
            this.tlpThermalParams.Padding = new System.Windows.Forms.Padding(8);
            this.tlpThermalParams.RowCount = 4;
            this.tlpThermalParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpThermalParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpThermalParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpThermalParams.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpThermalParams.Size = new System.Drawing.Size(498, 100);
            this.tlpThermalParams.TabIndex = 19;
            // 
            // nudShutterTime
            // 
            this.nudShutterTime.Location = new System.Drawing.Point(120, 183);
            this.nudShutterTime.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudShutterTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudShutterTime.Name = "nudShutterTime";
            this.nudShutterTime.Size = new System.Drawing.Size(115, 21);
            this.nudShutterTime.TabIndex = 17;
            this.nudShutterTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lblShutterTimeUnit
            // 
            this.lblShutterTimeUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblShutterTimeUnit.AutoSize = true;
            this.lblShutterTimeUnit.Location = new System.Drawing.Point(250, 185);
            this.lblShutterTimeUnit.Name = "lblShutterTimeUnit";
            this.lblShutterTimeUnit.Size = new System.Drawing.Size(29, 12);
            this.lblShutterTimeUnit.TabIndex = 18;
            this.lblShutterTimeUnit.Text = "min";
            // 
            // lblShutterTime
            // 
            this.lblShutterTime.AutoSize = true;
            this.lblShutterTime.Location = new System.Drawing.Point(20, 185);
            this.lblShutterTime.Name = "lblShutterTime";
            this.lblShutterTime.Size = new System.Drawing.Size(83, 12);
            this.lblShutterTime.TabIndex = 16;
            this.lblShutterTime.Text = "Shutter Time:";
            // 
            // cbShutterMode
            // 
            this.cbShutterMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShutterMode.FormattingEnabled = true;
            this.cbShutterMode.Items.AddRange(new object[] {
            "Automatic",
            "Manual"});
            this.cbShutterMode.Location = new System.Drawing.Point(120, 156);
            this.cbShutterMode.Name = "cbShutterMode";
            this.cbShutterMode.Size = new System.Drawing.Size(115, 20);
            this.cbShutterMode.TabIndex = 15;
            // 
            // lblShutterMode
            // 
            this.lblShutterMode.AutoSize = true;
            this.lblShutterMode.Location = new System.Drawing.Point(20, 159);
            this.lblShutterMode.Name = "lblShutterMode";
            this.lblShutterMode.Size = new System.Drawing.Size(53, 12);
            this.lblShutterMode.TabIndex = 14;
            this.lblShutterMode.Text = "Shutter:";
            // 
            // nudRelHumidity
            // 
            this.nudRelHumidity.DecimalPlaces = 1;
            this.nudRelHumidity.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudRelHumidity.Location = new System.Drawing.Point(120, 129);
            this.nudRelHumidity.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudRelHumidity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRelHumidity.Name = "nudRelHumidity";
            this.nudRelHumidity.Size = new System.Drawing.Size(115, 21);
            this.nudRelHumidity.TabIndex = 12;
            this.nudRelHumidity.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblRelHumidityUnit
            // 
            this.lblRelHumidityUnit.AutoSize = true;
            this.lblRelHumidityUnit.Location = new System.Drawing.Point(250, 131);
            this.lblRelHumidityUnit.Name = "lblRelHumidityUnit";
            this.lblRelHumidityUnit.Size = new System.Drawing.Size(11, 12);
            this.lblRelHumidityUnit.TabIndex = 13;
            this.lblRelHumidityUnit.Text = "%";
            // 
            // lblRelHumidity
            // 
            this.lblRelHumidity.AutoSize = true;
            this.lblRelHumidity.Location = new System.Drawing.Point(20, 131);
            this.lblRelHumidity.Name = "lblRelHumidity";
            this.lblRelHumidity.Size = new System.Drawing.Size(83, 12);
            this.lblRelHumidity.TabIndex = 11;
            this.lblRelHumidity.Text = "Rel.Humidity:";
            // 
            // nudAtmTemp
            // 
            this.nudAtmTemp.DecimalPlaces = 2;
            this.nudAtmTemp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudAtmTemp.Location = new System.Drawing.Point(120, 102);
            this.nudAtmTemp.Maximum = new decimal(new int[] {
            5750,
            0,
            0,
            0});
            this.nudAtmTemp.Name = "nudAtmTemp";
            this.nudAtmTemp.Size = new System.Drawing.Size(115, 21);
            this.nudAtmTemp.TabIndex = 9;
            this.nudAtmTemp.Value = new decimal(new int[] {
            29315,
            0,
            0,
            131072});
            // 
            // lblAtmTempUnit
            // 
            this.lblAtmTempUnit.AutoSize = true;
            this.lblAtmTempUnit.Location = new System.Drawing.Point(250, 103);
            this.lblAtmTempUnit.Name = "lblAtmTempUnit";
            this.lblAtmTempUnit.Size = new System.Drawing.Size(11, 12);
            this.lblAtmTempUnit.TabIndex = 10;
            this.lblAtmTempUnit.Text = "°C";
            // 
            // lblAtmTemp
            // 
            this.lblAtmTemp.AutoSize = true;
            this.lblAtmTemp.Location = new System.Drawing.Point(20, 103);
            this.lblAtmTemp.Name = "lblAtmTemp";
            this.lblAtmTemp.Size = new System.Drawing.Size(59, 12);
            this.lblAtmTemp.TabIndex = 8;
            this.lblAtmTemp.Text = "Atm.Temp:";
            // 
            // nudDistance
            // 
            this.nudDistance.DecimalPlaces = 2;
            this.nudDistance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudDistance.Location = new System.Drawing.Point(120, 74);
            this.nudDistance.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudDistance.Name = "nudDistance";
            this.nudDistance.Size = new System.Drawing.Size(115, 21);
            this.nudDistance.TabIndex = 6;
            this.nudDistance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblDistanceUnit
            // 
            this.lblDistanceUnit.AutoSize = true;
            this.lblDistanceUnit.Location = new System.Drawing.Point(250, 76);
            this.lblDistanceUnit.Name = "lblDistanceUnit";
            this.lblDistanceUnit.Size = new System.Drawing.Size(11, 12);
            this.lblDistanceUnit.TabIndex = 7;
            this.lblDistanceUnit.Text = "m";
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.Location = new System.Drawing.Point(20, 76);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(59, 12);
            this.lblDistance.TabIndex = 5;
            this.lblDistance.Text = "Distance:";
            // 
            // nudReflTemp
            // 
            this.nudReflTemp.DecimalPlaces = 2;
            this.nudReflTemp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudReflTemp.Location = new System.Drawing.Point(120, 46);
            this.nudReflTemp.Maximum = new decimal(new int[] {
            5750,
            0,
            0,
            0});
            this.nudReflTemp.Name = "nudReflTemp";
            this.nudReflTemp.Size = new System.Drawing.Size(115, 21);
            this.nudReflTemp.TabIndex = 3;
            this.nudReflTemp.Value = new decimal(new int[] {
            29315,
            0,
            0,
            131072});
            // 
            // lblReflTempUnit
            // 
            this.lblReflTempUnit.AutoSize = true;
            this.lblReflTempUnit.Location = new System.Drawing.Point(250, 48);
            this.lblReflTempUnit.Name = "lblReflTempUnit";
            this.lblReflTempUnit.Size = new System.Drawing.Size(11, 12);
            this.lblReflTempUnit.TabIndex = 4;
            this.lblReflTempUnit.Text = "°C";
            // 
            // lblReflTemp
            // 
            this.lblReflTemp.AutoSize = true;
            this.lblReflTemp.Location = new System.Drawing.Point(20, 48);
            this.lblReflTemp.Name = "lblReflTemp";
            this.lblReflTemp.Size = new System.Drawing.Size(65, 12);
            this.lblReflTemp.TabIndex = 2;
            this.lblReflTemp.Text = "Refl.Temp:";
            // 
            // nudEmissivity
            // 
            this.nudEmissivity.DecimalPlaces = 2;
            this.nudEmissivity.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudEmissivity.Location = new System.Drawing.Point(120, 18);
            this.nudEmissivity.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEmissivity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudEmissivity.Name = "nudEmissivity";
            this.nudEmissivity.Size = new System.Drawing.Size(115, 21);
            this.nudEmissivity.TabIndex = 1;
            this.nudEmissivity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblEmissivity
            // 
            this.lblEmissivity.AutoSize = true;
            this.lblEmissivity.Location = new System.Drawing.Point(20, 20);
            this.lblEmissivity.Name = "lblEmissivity";
            this.lblEmissivity.Size = new System.Drawing.Size(71, 12);
            this.lblEmissivity.TabIndex = 0;
            this.lblEmissivity.Text = "Emissivity:";
            // 
            // grpDisplay
            // 
            this.grpDisplay.Controls.Add(this.tlpDisplay);
            this.grpDisplay.Location = new System.Drawing.Point(4, 530);
            this.grpDisplay.Name = "grpDisplay";
            this.grpDisplay.Padding = new System.Windows.Forms.Padding(3);
            this.grpDisplay.Size = new System.Drawing.Size(504, 210);
            this.grpDisplay.TabIndex = 2;
            this.grpDisplay.TabStop = false;
            this.grpDisplay.Text = "Display Parameters";
            // 
            // tlpDisplay
            // 
            this.tlpDisplay.AutoSize = true;
            this.tlpDisplay.ColumnCount = 4;
            this.tlpDisplay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpDisplay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpDisplay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpDisplay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpDisplay.Controls.Add(this.lblContrast, 0, 0);
            this.tlpDisplay.Controls.Add(this.nudContrast, 1, 0);
            this.tlpDisplay.Controls.Add(this.lblBrightness, 2, 0);
            this.tlpDisplay.Controls.Add(this.nudBrightness, 3, 0);
            this.tlpDisplay.Controls.Add(this.cbDdeEnable, 0, 1);
            this.tlpDisplay.Controls.Add(this.lblDdeValue, 2, 1);
            this.tlpDisplay.Controls.Add(this.nudDdeValue, 3, 1);
            this.tlpDisplay.Controls.Add(this.cbSrEnable, 0, 2);
            this.tlpDisplay.Controls.Add(this.btnDisplayGet, 2, 2);
            this.tlpDisplay.Controls.Add(this.btnDisplaySet, 3, 2);
            this.tlpDisplay.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpDisplay.Name = "tlpDisplay";
            this.tlpDisplay.Padding = new System.Windows.Forms.Padding(8);
            this.tlpDisplay.RowCount = 3;
            this.tlpDisplay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpDisplay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpDisplay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpDisplay.Size = new System.Drawing.Size(498, 100);
            this.tlpDisplay.TabIndex = 10;
            // 
            // btnDisplaySet
            // 
            this.btnDisplaySet.Location = new System.Drawing.Point(110, 132);
            this.btnDisplaySet.Name = "btnDisplaySet";
            this.btnDisplaySet.Size = new System.Drawing.Size(75, 21);
            this.btnDisplaySet.TabIndex = 8;
            this.btnDisplaySet.Text = "Set";
            this.btnDisplaySet.UseVisualStyleBackColor = true;
            this.btnDisplaySet.Click += new System.EventHandler(this.btnDisplaySet_Click);
            // 
            // btnDisplayGet
            // 
            this.btnDisplayGet.Location = new System.Drawing.Point(20, 132);
            this.btnDisplayGet.Name = "btnDisplayGet";
            this.btnDisplayGet.Size = new System.Drawing.Size(75, 21);
            this.btnDisplayGet.TabIndex = 7;
            this.btnDisplayGet.Text = "Get";
            this.btnDisplayGet.UseVisualStyleBackColor = true;
            this.btnDisplayGet.Click += new System.EventHandler(this.btnDisplayGet_Click);
            // 
            // cbSrEnable
            // 
            this.cbSrEnable.AutoSize = true;
            this.cbSrEnable.Location = new System.Drawing.Point(20, 104);
            this.cbSrEnable.Name = "cbSrEnable";
            this.cbSrEnable.Size = new System.Drawing.Size(78, 16);
            this.cbSrEnable.TabIndex = 9;
            this.cbSrEnable.Text = "SR Enable";
            this.cbSrEnable.UseVisualStyleBackColor = true;
            // 
            // nudDdeValue
            // 
            this.nudDdeValue.Location = new System.Drawing.Point(190, 75);
            this.nudDdeValue.Name = "nudDdeValue";
            this.nudDdeValue.Size = new System.Drawing.Size(80, 21);
            this.nudDdeValue.TabIndex = 6;
            this.nudDdeValue.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblDdeValue
            // 
            this.lblDdeValue.AutoSize = true;
            this.lblDdeValue.Location = new System.Drawing.Point(120, 77);
            this.lblDdeValue.Name = "lblDdeValue";
            this.lblDdeValue.Size = new System.Drawing.Size(65, 12);
            this.lblDdeValue.TabIndex = 5;
            this.lblDdeValue.Text = "DDE Value:";
            // 
            // cbDdeEnable
            // 
            this.cbDdeEnable.AutoSize = true;
            this.cbDdeEnable.Location = new System.Drawing.Point(20, 76);
            this.cbDdeEnable.Name = "cbDdeEnable";
            this.cbDdeEnable.Size = new System.Drawing.Size(84, 16);
            this.cbDdeEnable.TabIndex = 4;
            this.cbDdeEnable.Text = "DDE Enable";
            this.cbDdeEnable.UseVisualStyleBackColor = true;
            // 
            // nudBrightness
            // 
            this.nudBrightness.Location = new System.Drawing.Point(120, 46);
            this.nudBrightness.Name = "nudBrightness";
            this.nudBrightness.Size = new System.Drawing.Size(120, 21);
            this.nudBrightness.TabIndex = 3;
            this.nudBrightness.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblBrightness
            // 
            this.lblBrightness.AutoSize = true;
            this.lblBrightness.Location = new System.Drawing.Point(20, 48);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(71, 12);
            this.lblBrightness.TabIndex = 2;
            this.lblBrightness.Text = "Brightness:";
            // 
            // nudContrast
            // 
            this.nudContrast.Location = new System.Drawing.Point(120, 18);
            this.nudContrast.Name = "nudContrast";
            this.nudContrast.Size = new System.Drawing.Size(120, 21);
            this.nudContrast.TabIndex = 1;
            this.nudContrast.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblContrast
            // 
            this.lblContrast.AutoSize = true;
            this.lblContrast.Location = new System.Drawing.Point(20, 20);
            this.lblContrast.Name = "lblContrast";
            this.lblContrast.Size = new System.Drawing.Size(59, 12);
            this.lblContrast.TabIndex = 0;
            this.lblContrast.Text = "Contrast:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1440, 800);
            this.MinimumSize = new System.Drawing.Size(1024, 640);
            this.Controls.Add(this.scMain);
            this.Controls.Add(this.tsMainForm);
            this.Controls.Add(this.tsConnection);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IRtekUVCSDKDemo";
            this.tsConnection.ResumeLayout(false);
            this.tsConnection.PerformLayout();
            this.tsMainForm.ResumeLayout(false);
            this.tsMainForm.PerformLayout();
            this.scMain.Panel1.ResumeLayout(false);
            this.scMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMain)).EndInit();
            this.scMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbThermalImage)).EndInit();
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.tlpSections.ResumeLayout(false);
            this.tlpSections.PerformLayout();
            this.grpProperties.ResumeLayout(false);
            this.pnlDeviceMgmt.ResumeLayout(false);
            this.pnlDeviceMgmt.PerformLayout();
            this.grpThermal.ResumeLayout(false);
            this.grpThermal.PerformLayout();
            this.tlpThermalParams.ResumeLayout(false);
            this.tlpThermalParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudShutterTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRelHumidity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAtmTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivity)).EndInit();
            this.grpDisplay.ResumeLayout(false);
            this.grpDisplay.PerformLayout();
            this.tlpDisplay.ResumeLayout(false);
            this.tlpDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDdeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tsConnection;
        private System.Windows.Forms.ToolStripLabel tslDevice;
        private System.Windows.Forms.ToolStripComboBox tscbDeviceList;
        private System.Windows.Forms.ToolStripButton tsbRefreshDevices;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbLogin;
        private System.Windows.Forms.ToolStripButton tsbLogout;
        private System.Windows.Forms.ToolStrip tsMainForm;
        private System.Windows.Forms.ToolStripButton tsbCapture;
        private System.Windows.Forms.ToolStripLabel tslRange;
        private System.Windows.Forms.ToolStripComboBox tscbRange;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsbSelect;
        private System.Windows.Forms.ToolStripButton tsbSpot;
        private System.Windows.Forms.ToolStripButton tsbRectangle;
        private System.Windows.Forms.ToolStripButton tsbLine;
        private System.Windows.Forms.ToolStripButton tsbDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2a;
        private System.Windows.Forms.ToolStripLabel tslPalette;
        private System.Windows.Forms.ToolStripComboBox tscbPalette;
        private System.Windows.Forms.ToolStripButton tsbInverted;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripLabel tslImageFlip;
        private System.Windows.Forms.ToolStripComboBox tscbImageFlip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton tsbCalibrate;
        private System.Windows.Forms.SplitContainer scMain;
        private System.Windows.Forms.PictureBox pbThermalImage;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.TableLayoutPanel tlpSections;
        private System.Windows.Forms.GroupBox grpProperties;
        private System.Windows.Forms.Panel pnlDeviceMgmt;
        private System.Windows.Forms.Button btnDeviceReset;
        private System.Windows.Forms.Button btnDeviceRestart;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button btnUpgradeBrowse;
        private System.Windows.Forms.TextBox tbUpgradePath;
        private System.Windows.Forms.Label lblUpgradePath;
        private System.Windows.Forms.ListView lvProperties;
        private System.Windows.Forms.ColumnHeader chProperty;
        private System.Windows.Forms.ColumnHeader chValue;
        private System.Windows.Forms.GroupBox grpThermal;
        private System.Windows.Forms.TableLayoutPanel tlpThermalParams;
        private System.Windows.Forms.NumericUpDown nudShutterTime;
        private System.Windows.Forms.Label lblShutterTimeUnit;
        private System.Windows.Forms.Label lblShutterTime;
        private System.Windows.Forms.ComboBox cbShutterMode;
        private System.Windows.Forms.Label lblShutterMode;
        private System.Windows.Forms.NumericUpDown nudRelHumidity;
        private System.Windows.Forms.Label lblRelHumidityUnit;
        private System.Windows.Forms.Label lblRelHumidity;
        private System.Windows.Forms.NumericUpDown nudAtmTemp;
        private System.Windows.Forms.Label lblAtmTempUnit;
        private System.Windows.Forms.Label lblAtmTemp;
        private System.Windows.Forms.NumericUpDown nudDistance;
        private System.Windows.Forms.Label lblDistanceUnit;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.NumericUpDown nudReflTemp;
        private System.Windows.Forms.Label lblReflTempUnit;
        private System.Windows.Forms.Label lblReflTemp;
        private System.Windows.Forms.NumericUpDown nudEmissivity;
        private System.Windows.Forms.Label lblEmissivity;
        private System.Windows.Forms.GroupBox grpDisplay;
        private System.Windows.Forms.TableLayoutPanel tlpDisplay;
        private System.Windows.Forms.Button btnDisplaySet;
        private System.Windows.Forms.Button btnDisplayGet;
        private System.Windows.Forms.CheckBox cbSrEnable;
        private System.Windows.Forms.NumericUpDown nudDdeValue;
        private System.Windows.Forms.Label lblDdeValue;
        private System.Windows.Forms.CheckBox cbDdeEnable;
        private System.Windows.Forms.NumericUpDown nudBrightness;
        private System.Windows.Forms.Label lblBrightness;
        private System.Windows.Forms.NumericUpDown nudContrast;
        private System.Windows.Forms.Label lblContrast;
        private System.Windows.Forms.ListView mlvMarkers;
        private System.Windows.Forms.ColumnHeader chMkName;
        private System.Windows.Forms.ColumnHeader chMkValue;
        private System.Windows.Forms.ColumnHeader chMkMin;
        private System.Windows.Forms.ColumnHeader chMkMax;
        private System.Windows.Forms.ColumnHeader chMkAverage;
        private System.Windows.Forms.ColumnHeader chMkX1;
        private System.Windows.Forms.ColumnHeader chMkY1;
        private System.Windows.Forms.ColumnHeader chMkX2;
        private System.Windows.Forms.ColumnHeader chMkY2;
    }
}
