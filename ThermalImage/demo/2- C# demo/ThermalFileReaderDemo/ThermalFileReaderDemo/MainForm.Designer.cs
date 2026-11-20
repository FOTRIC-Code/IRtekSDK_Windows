
namespace ThermalFileReaderDemo
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
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Size", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Geo Location", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Image Enhancement", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("File Info", System.Windows.Forms.HorizontalAlignment.Left);
            this.msMainForm = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveFrameAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTransform = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHorizontalMirror = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiVerticalMirror = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMainForm = new System.Windows.Forms.ToolStrip();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tslFileFormat = new System.Windows.Forms.ToolStripLabel();
            this.tscbFileFormat = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSelect = new System.Windows.Forms.ToolStripButton();
            this.tsbSpot = new System.Windows.Forms.ToolStripButton();
            this.tsbRectangle = new System.Windows.Forms.ToolStripButton();
            this.tsbEllipse = new System.Windows.Forms.ToolStripButton();
            this.tsbLine = new System.Windows.Forms.ToolStripButton();
            this.tsbDelta = new System.Windows.Forms.ToolStripButton();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tslPalette = new System.Windows.Forms.ToolStripLabel();
            this.tsbPalette = new System.Windows.Forms.ToolStripButton();
            this.tscbPalette = new System.Windows.Forms.ToolStripComboBox();
            this.tsbInverted = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tslAlarm = new System.Windows.Forms.ToolStripLabel();
            this.tscbAlarm = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tslColorDistributionType = new System.Windows.Forms.ToolStripLabel();
            this.tscbColorDistributionType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tslViewType = new System.Windows.Forms.ToolStripLabel();
            this.tscbViewType = new System.Windows.Forms.ToolStripComboBox();
            this.pbThermalImage = new System.Windows.Forms.PictureBox();
            this.cmsThermalImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExportFrameAD = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportFrameTemperature = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiImportFrameAD = new System.Windows.Forms.ToolStripMenuItem();
            this.tcThermalFile = new System.Windows.Forms.TabControl();
            this.tpThermalParams = new System.Windows.Forms.TabPage();
            this.nudRealRefTemp = new System.Windows.Forms.NumericUpDown();
            this.lblRealRefTempUnit = new System.Windows.Forms.Label();
            this.lblRealRefTemp = new System.Windows.Forms.Label();
            this.lblRefTempUnit = new System.Windows.Forms.Label();
            this.lblRefTemp = new System.Windows.Forms.Label();
            this.lblRefValue = new System.Windows.Forms.Label();
            this.lblRefName = new System.Windows.Forms.Label();
            this.nudRefTemp = new System.Windows.Forms.NumericUpDown();
            this.cbRefValue = new System.Windows.Forms.ComboBox();
            this.cbRefName = new System.Windows.Forms.ComboBox();
            this.cbIsRefEnabled = new System.Windows.Forms.CheckBox();
            this.lblExtOptTempUnit = new System.Windows.Forms.Label();
            this.nudExtOptTemp = new System.Windows.Forms.NumericUpDown();
            this.lblExtOptTemp = new System.Windows.Forms.Label();
            this.nudExtOptTrans = new System.Windows.Forms.NumericUpDown();
            this.lblExtOptTrans = new System.Windows.Forms.Label();
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
            this.tpProperties = new System.Windows.Forms.TabPage();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.lvProperties = new System.Windows.Forms.ListView();
            this.chPropertyKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chPropertyValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tpAnnotation = new System.Windows.Forms.TabPage();
            this.gbNoteAnnotation = new System.Windows.Forms.GroupBox();
            this.tbNoteAnnotation = new System.Windows.Forms.TextBox();
            this.gbTagAnnotation = new System.Windows.Forms.GroupBox();
            this.lblAddTag = new System.Windows.Forms.Label();
            this.lblEditTag = new System.Windows.Forms.Label();
            this.tbEditTag = new System.Windows.Forms.TextBox();
            this.tbAddTag = new System.Windows.Forms.TextBox();
            this.btnEditTag = new System.Windows.Forms.Button();
            this.btnAddTag = new System.Windows.Forms.Button();
            this.lvTagAnnotation = new System.Windows.Forms.ListView();
            this.chTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbAudioAnnotation = new System.Windows.Forms.GroupBox();
            this.btnAudioPause = new System.Windows.Forms.Button();
            this.btnAudioStop = new System.Windows.Forms.Button();
            this.btnAudioPlay = new System.Windows.Forms.Button();
            this.tpUserData = new System.Windows.Forms.TabPage();
            this.tbAddUserDataKey = new System.Windows.Forms.TextBox();
            this.lblAddUserData = new System.Windows.Forms.Label();
            this.lblEditUserData = new System.Windows.Forms.Label();
            this.rtbEditUserData = new System.Windows.Forms.RichTextBox();
            this.rtbAddUserDataValue = new System.Windows.Forms.RichTextBox();
            this.btnEditUserData = new System.Windows.Forms.Button();
            this.btnAddUserData = new System.Windows.Forms.Button();
            this.lvUserData = new System.Windows.Forms.ListView();
            this.chUserDataKey = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chUserDataValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbCoverImage = new System.Windows.Forms.PictureBox();
            this.pbVisualImage = new System.Windows.Forms.PictureBox();
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
            this.nudAlarmMin = new System.Windows.Forms.NumericUpDown();
            this.nudAlarmMax = new System.Windows.Forms.NumericUpDown();
            this.lblAlarm = new System.Windows.Forms.Label();
            this.nudSpan = new System.Windows.Forms.NumericUpDown();
            this.cbFixedSpan = new System.Windows.Forms.CheckBox();
            this.cbAlarmCustom = new System.Windows.Forms.CheckBox();
            this.btnAlarmColor = new System.Windows.Forms.Button();
            this.nudAlarmAlpha = new System.Windows.Forms.NumericUpDown();
            this.lblAlarmColor = new System.Windows.Forms.Label();
            this.lblAlarmAlpha = new System.Windows.Forms.Label();
            this.lblAlarmMax = new System.Windows.Forms.Label();
            this.lblAlarmMin = new System.Windows.Forms.Label();
            this.lblMaxUnit = new System.Windows.Forms.Label();
            this.lblSpanUnit = new System.Windows.Forms.Label();
            this.lblAlarmMaxUnit = new System.Windows.Forms.Label();
            this.lblAlarmMinUnit = new System.Windows.Forms.Label();
            this.lblMinUnit = new System.Windows.Forms.Label();
            this.lblView = new System.Windows.Forms.Label();
            this.lblViewMinUnit = new System.Windows.Forms.Label();
            this.lblViewMaxUnit = new System.Windows.Forms.Label();
            this.lblViewMin = new System.Windows.Forms.Label();
            this.lblViewMax = new System.Windows.Forms.Label();
            this.lblViewAlpha = new System.Windows.Forms.Label();
            this.nudViewAlpha = new System.Windows.Forms.NumericUpDown();
            this.nudViewMax = new System.Windows.Forms.NumericUpDown();
            this.nudViewMin = new System.Windows.Forms.NumericUpDown();
            this.lblViewArea = new System.Windows.Forms.Label();
            this.nudViewAreaX = new System.Windows.Forms.NumericUpDown();
            this.nudViewAreaY = new System.Windows.Forms.NumericUpDown();
            this.nudViewAreaWidth = new System.Windows.Forms.NumericUpDown();
            this.nudViewAreaHeight = new System.Windows.Forms.NumericUpDown();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnNextFrame = new System.Windows.Forms.Button();
            this.btnPreviousFrame = new System.Windows.Forms.Button();
            this.tPlay = new System.Windows.Forms.Timer(this.components);
            this.nudFromFrame = new System.Windows.Forms.NumericUpDown();
            this.nudToFrame = new System.Windows.Forms.NumericUpDown();
            this.btnClip = new System.Windows.Forms.Button();
            this.lblFromFrame = new System.Windows.Forms.Label();
            this.lblToFrame = new System.Windows.Forms.Label();
            this.nudGoto = new System.Windows.Forms.NumericUpDown();
            this.btnGoto = new System.Windows.Forms.Button();
            this.pAlarmTemp = new System.Windows.Forms.Panel();
            this.pHumidity = new System.Windows.Forms.Panel();
            this.lblRHLimit = new System.Windows.Forms.Label();
            this.nudRHLimit = new System.Windows.Forms.NumericUpDown();
            this.pInsulation = new System.Windows.Forms.Panel();
            this.nudHeatIndex = new System.Windows.Forms.NumericUpDown();
            this.lblHeatIndex = new System.Windows.Forms.Label();
            this.lblIndoorTemp = new System.Windows.Forms.Label();
            this.nudOutdoorTemp = new System.Windows.Forms.NumericUpDown();
            this.nudIndoorTemp = new System.Windows.Forms.NumericUpDown();
            this.lblOutdoorTemp = new System.Windows.Forms.Label();
            this.lblIndoorTempUnit = new System.Windows.Forms.Label();
            this.lblOutdoorTempUnit = new System.Windows.Forms.Label();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.nudThreshold = new System.Windows.Forms.NumericUpDown();
            this.mlvMarkers = new ThermalFileReaderDemo.MarkersListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAverage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chX1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chY1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chX2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chY2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chArea = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLocalParameters = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chExInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.msMainForm.SuspendLayout();
            this.tsMainForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThermalImage)).BeginInit();
            this.cmsThermalImage.SuspendLayout();
            this.tcThermalFile.SuspendLayout();
            this.tpThermalParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRealRefTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRefTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtOptTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtOptTrans)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRelHumidity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAtmTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivity)).BeginInit();
            this.tpProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.tpAnnotation.SuspendLayout();
            this.gbNoteAnnotation.SuspendLayout();
            this.gbTagAnnotation.SuspendLayout();
            this.gbAudioAnnotation.SuspendLayout();
            this.tpUserData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoverImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualImage)).BeginInit();
            this.cmsMarkers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPalleteImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFromFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGoto)).BeginInit();
            this.pAlarmTemp.SuspendLayout();
            this.pHumidity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRHLimit)).BeginInit();
            this.pInsulation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeatIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOutdoorTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIndoorTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // msMainForm
            // 
            this.msMainForm.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.msMainForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiTransform});
            this.msMainForm.Location = new System.Drawing.Point(0, 0);
            this.msMainForm.Name = "msMainForm";
            this.msMainForm.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.msMainForm.Size = new System.Drawing.Size(1377, 25);
            this.msMainForm.TabIndex = 1;
            this.msMainForm.Text = "mainMenuStrip";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpen,
            this.tsmiClose,
            this.toolStripSeparator1,
            this.tsmiSave,
            this.tsmiSaveAs,
            this.tsmiSaveFrameAs});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(39, 21);
            this.tsmiFile.Text = "File";
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.Name = "tsmiOpen";
            this.tsmiOpen.Size = new System.Drawing.Size(170, 22);
            this.tsmiOpen.Text = "Open...";
            this.tsmiOpen.Click += new System.EventHandler(this.tsmiOpen_Click);
            // 
            // tsmiClose
            // 
            this.tsmiClose.Name = "tsmiClose";
            this.tsmiClose.Size = new System.Drawing.Size(170, 22);
            this.tsmiClose.Text = "Close";
            this.tsmiClose.Click += new System.EventHandler(this.tsmiClose_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new System.Drawing.Size(170, 22);
            this.tsmiSave.Text = "Save...";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // tsmiSaveAs
            // 
            this.tsmiSaveAs.Name = "tsmiSaveAs";
            this.tsmiSaveAs.Size = new System.Drawing.Size(170, 22);
            this.tsmiSaveAs.Text = "Save As...";
            this.tsmiSaveAs.Click += new System.EventHandler(this.tsmiSaveAs_Click);
            // 
            // tsmiSaveFrameAs
            // 
            this.tsmiSaveFrameAs.Name = "tsmiSaveFrameAs";
            this.tsmiSaveFrameAs.Size = new System.Drawing.Size(170, 22);
            this.tsmiSaveFrameAs.Text = "Save Frame As...";
            this.tsmiSaveFrameAs.Click += new System.EventHandler(this.tsmiSaveFrameAs_Click);
            // 
            // tsmiTransform
            // 
            this.tsmiTransform.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRotate,
            this.tsmiHorizontalMirror,
            this.tsmiVerticalMirror});
            this.tsmiTransform.Name = "tsmiTransform";
            this.tsmiTransform.Size = new System.Drawing.Size(80, 21);
            this.tsmiTransform.Text = "Transform";
            // 
            // tsmiRotate
            // 
            this.tsmiRotate.Name = "tsmiRotate";
            this.tsmiRotate.Size = new System.Drawing.Size(178, 22);
            this.tsmiRotate.Text = "Rotate";
            this.tsmiRotate.Click += new System.EventHandler(this.tsmiRotate_Click);
            // 
            // tsmiHorizontalMirror
            // 
            this.tsmiHorizontalMirror.Name = "tsmiHorizontalMirror";
            this.tsmiHorizontalMirror.Size = new System.Drawing.Size(178, 22);
            this.tsmiHorizontalMirror.Text = "Horizontal Mirror";
            this.tsmiHorizontalMirror.Click += new System.EventHandler(this.tsmiHorizontalMirror_Click);
            // 
            // tsmiVerticalMirror
            // 
            this.tsmiVerticalMirror.Name = "tsmiVerticalMirror";
            this.tsmiVerticalMirror.Size = new System.Drawing.Size(178, 22);
            this.tsmiVerticalMirror.Text = "Vertical Mirror";
            this.tsmiVerticalMirror.Click += new System.EventHandler(this.tsmiVerticalMirror_Click);
            // 
            // tsMainForm
            // 
            this.tsMainForm.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tsMainForm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSave,
            this.tslFileFormat,
            this.tscbFileFormat,
            this.toolStripSeparator4,
            this.tsbSelect,
            this.tsbSpot,
            this.tsbRectangle,
            this.tsbEllipse,
            this.tsbLine,
            this.tsbDelta,
            this.tsbDelete,
            this.toolStripSeparator2,
            this.tslPalette,
            this.tsbPalette,
            this.tscbPalette,
            this.tsbInverted,
            this.toolStripSeparator5,
            this.tslAlarm,
            this.tscbAlarm,
            this.toolStripSeparator3,
            this.tslColorDistributionType,
            this.tscbColorDistributionType,
            this.toolStripSeparator6,
            this.tslViewType,
            this.tscbViewType});
            this.tsMainForm.Location = new System.Drawing.Point(0, 25);
            this.tsMainForm.Name = "tsMainForm";
            this.tsMainForm.Size = new System.Drawing.Size(1377, 31);
            this.tsMainForm.TabIndex = 10;
            this.tsMainForm.Text = "toolStrip1";
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(28, 28);
            this.tsbSave.Text = "Save image";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // tslFileFormat
            // 
            this.tslFileFormat.Name = "tslFileFormat";
            this.tslFileFormat.Size = new System.Drawing.Size(72, 28);
            this.tslFileFormat.Text = "File Format";
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
            // tsbEllipse
            // 
            this.tsbEllipse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEllipse.Image = ((System.Drawing.Image)(resources.GetObject("tsbEllipse.Image")));
            this.tsbEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEllipse.Name = "tsbEllipse";
            this.tsbEllipse.Size = new System.Drawing.Size(28, 28);
            this.tsbEllipse.Text = "Ellipse Tool";
            this.tsbEllipse.Click += new System.EventHandler(this.tsbEllipse_Click);
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
            // tsbDelta
            // 
            this.tsbDelta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelta.Image = ((System.Drawing.Image)(resources.GetObject("tsbDelta.Image")));
            this.tsbDelta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelta.Name = "tsbDelta";
            this.tsbDelta.Size = new System.Drawing.Size(28, 28);
            this.tsbDelta.Text = "Delta Tool";
            this.tsbDelta.Click += new System.EventHandler(this.tsbDelta_Click);
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
            this.tsbInverted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbInverted.Image = ((System.Drawing.Image)(resources.GetObject("tsbInverted.Image")));
            this.tsbInverted.ImageTransparentColor = System.Drawing.Color.Magenta;
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
            // tslAlarm
            // 
            this.tslAlarm.Name = "tslAlarm";
            this.tslAlarm.Size = new System.Drawing.Size(42, 28);
            this.tslAlarm.Text = "Alarm";
            // 
            // tscbAlarm
            // 
            this.tscbAlarm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbAlarm.Name = "tscbAlarm";
            this.tscbAlarm.Size = new System.Drawing.Size(92, 31);
            this.tscbAlarm.SelectedIndexChanged += new System.EventHandler(this.tscbAlarm_SelectedIndexChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // tslColorDistributionType
            // 
            this.tslColorDistributionType.Name = "tslColorDistributionType";
            this.tslColorDistributionType.Size = new System.Drawing.Size(143, 28);
            this.tslColorDistributionType.Text = "Color Distribution Type";
            // 
            // tscbColorDistributionType
            // 
            this.tscbColorDistributionType.AutoSize = false;
            this.tscbColorDistributionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbColorDistributionType.DropDownWidth = 140;
            this.tscbColorDistributionType.Name = "tscbColorDistributionType";
            this.tscbColorDistributionType.Size = new System.Drawing.Size(106, 25);
            this.tscbColorDistributionType.SelectedIndexChanged += new System.EventHandler(this.tscbColorDistributionType_SelectedIndexChanged);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
            // 
            // tslViewType
            // 
            this.tslViewType.Name = "tslViewType";
            this.tslViewType.Size = new System.Drawing.Size(67, 28);
            this.tslViewType.Text = "View Type";
            // 
            // tscbViewType
            // 
            this.tscbViewType.AutoSize = false;
            this.tscbViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbViewType.DropDownWidth = 140;
            this.tscbViewType.Name = "tscbViewType";
            this.tscbViewType.Size = new System.Drawing.Size(92, 25);
            this.tscbViewType.SelectedIndexChanged += new System.EventHandler(this.tscbViewType_SelectedIndexChanged);
            // 
            // pbThermalImage
            // 
            this.pbThermalImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbThermalImage.BackColor = System.Drawing.SystemColors.Window;
            this.pbThermalImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbThermalImage.ContextMenuStrip = this.cmsThermalImage;
            this.pbThermalImage.Location = new System.Drawing.Point(12, 59);
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
            // cmsThermalImage
            // 
            this.cmsThermalImage.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsThermalImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExportFrameAD,
            this.tsmiExportFrameTemperature,
            this.tsmiImportFrameAD});
            this.cmsThermalImage.Name = "cmsMarkers";
            this.cmsThermalImage.Size = new System.Drawing.Size(243, 70);
            // 
            // tsmiExportFrameAD
            // 
            this.tsmiExportFrameAD.Name = "tsmiExportFrameAD";
            this.tsmiExportFrameAD.Size = new System.Drawing.Size(242, 22);
            this.tsmiExportFrameAD.Text = "Export Frame AD...";
            this.tsmiExportFrameAD.Click += new System.EventHandler(this.tsmiExportFrameAD_Click);
            // 
            // tsmiExportFrameTemperature
            // 
            this.tsmiExportFrameTemperature.Name = "tsmiExportFrameTemperature";
            this.tsmiExportFrameTemperature.Size = new System.Drawing.Size(242, 22);
            this.tsmiExportFrameTemperature.Text = "Export Frame Temperature...";
            this.tsmiExportFrameTemperature.Click += new System.EventHandler(this.tsmiExportFrameTemperature_Click);
            // 
            // tsmiImportFrameAD
            // 
            this.tsmiImportFrameAD.Name = "tsmiImportFrameAD";
            this.tsmiImportFrameAD.Size = new System.Drawing.Size(242, 22);
            this.tsmiImportFrameAD.Text = "Import Frame AD...";
            this.tsmiImportFrameAD.Click += new System.EventHandler(this.tsmiImportFrameAD_Click);
            // 
            // tcThermalFile
            // 
            this.tcThermalFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcThermalFile.Controls.Add(this.tpThermalParams);
            this.tcThermalFile.Controls.Add(this.tpProperties);
            this.tcThermalFile.Controls.Add(this.tpAnnotation);
            this.tcThermalFile.Controls.Add(this.tpUserData);
            this.tcThermalFile.Location = new System.Drawing.Point(879, 245);
            this.tcThermalFile.Name = "tcThermalFile";
            this.tcThermalFile.SelectedIndex = 0;
            this.tcThermalFile.Size = new System.Drawing.Size(486, 563);
            this.tcThermalFile.TabIndex = 13;
            // 
            // tpThermalParams
            // 
            this.tpThermalParams.Controls.Add(this.nudRealRefTemp);
            this.tpThermalParams.Controls.Add(this.lblRealRefTempUnit);
            this.tpThermalParams.Controls.Add(this.lblRealRefTemp);
            this.tpThermalParams.Controls.Add(this.lblRefTempUnit);
            this.tpThermalParams.Controls.Add(this.lblRefTemp);
            this.tpThermalParams.Controls.Add(this.lblRefValue);
            this.tpThermalParams.Controls.Add(this.lblRefName);
            this.tpThermalParams.Controls.Add(this.nudRefTemp);
            this.tpThermalParams.Controls.Add(this.cbRefValue);
            this.tpThermalParams.Controls.Add(this.cbRefName);
            this.tpThermalParams.Controls.Add(this.cbIsRefEnabled);
            this.tpThermalParams.Controls.Add(this.lblExtOptTempUnit);
            this.tpThermalParams.Controls.Add(this.nudExtOptTemp);
            this.tpThermalParams.Controls.Add(this.lblExtOptTemp);
            this.tpThermalParams.Controls.Add(this.nudExtOptTrans);
            this.tpThermalParams.Controls.Add(this.lblExtOptTrans);
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
            this.tpThermalParams.Location = new System.Drawing.Point(4, 22);
            this.tpThermalParams.Name = "tpThermalParams";
            this.tpThermalParams.Padding = new System.Windows.Forms.Padding(3);
            this.tpThermalParams.Size = new System.Drawing.Size(478, 537);
            this.tpThermalParams.TabIndex = 3;
            this.tpThermalParams.Text = "Thermal Parameters";
            this.tpThermalParams.UseVisualStyleBackColor = true;
            // 
            // nudRealRefTemp
            // 
            this.nudRealRefTemp.DecimalPlaces = 1;
            this.nudRealRefTemp.Enabled = false;
            this.nudRealRefTemp.Location = new System.Drawing.Point(114, 324);
            this.nudRealRefTemp.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudRealRefTemp.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudRealRefTemp.Name = "nudRealRefTemp";
            this.nudRealRefTemp.ReadOnly = true;
            this.nudRealRefTemp.Size = new System.Drawing.Size(104, 21);
            this.nudRealRefTemp.TabIndex = 31;
            this.nudRealRefTemp.Visible = false;
            // 
            // lblRealRefTempUnit
            // 
            this.lblRealRefTempUnit.AutoSize = true;
            this.lblRealRefTempUnit.Location = new System.Drawing.Point(224, 328);
            this.lblRealRefTempUnit.Name = "lblRealRefTempUnit";
            this.lblRealRefTempUnit.Size = new System.Drawing.Size(23, 12);
            this.lblRealRefTempUnit.TabIndex = 29;
            this.lblRealRefTempUnit.Text = "°C";
            this.lblRealRefTempUnit.Visible = false;
            // 
            // lblRealRefTemp
            // 
            this.lblRealRefTemp.AutoSize = true;
            this.lblRealRefTemp.Location = new System.Drawing.Point(11, 328);
            this.lblRealRefTemp.Name = "lblRealRefTemp";
            this.lblRealRefTemp.Size = new System.Drawing.Size(95, 12);
            this.lblRealRefTemp.TabIndex = 28;
            this.lblRealRefTemp.Text = "Real Ref. Temp.";
            this.lblRealRefTemp.Visible = false;
            // 
            // lblRefTempUnit
            // 
            this.lblRefTempUnit.AutoSize = true;
            this.lblRefTempUnit.Location = new System.Drawing.Point(224, 301);
            this.lblRefTempUnit.Name = "lblRefTempUnit";
            this.lblRefTempUnit.Size = new System.Drawing.Size(23, 12);
            this.lblRefTempUnit.TabIndex = 26;
            this.lblRefTempUnit.Text = "°C";
            // 
            // lblRefTemp
            // 
            this.lblRefTemp.AutoSize = true;
            this.lblRefTemp.Location = new System.Drawing.Point(11, 301);
            this.lblRefTemp.Name = "lblRefTemp";
            this.lblRefTemp.Size = new System.Drawing.Size(65, 12);
            this.lblRefTemp.TabIndex = 25;
            this.lblRefTemp.Text = "Ref. Temp.";
            // 
            // lblRefValue
            // 
            this.lblRefValue.AutoSize = true;
            this.lblRefValue.Location = new System.Drawing.Point(11, 275);
            this.lblRefValue.Name = "lblRefValue";
            this.lblRefValue.Size = new System.Drawing.Size(65, 12);
            this.lblRefValue.TabIndex = 24;
            this.lblRefValue.Text = "Ref. Value";
            // 
            // lblRefName
            // 
            this.lblRefName.AutoSize = true;
            this.lblRefName.Location = new System.Drawing.Point(11, 249);
            this.lblRefName.Name = "lblRefName";
            this.lblRefName.Size = new System.Drawing.Size(59, 12);
            this.lblRefName.TabIndex = 23;
            this.lblRefName.Text = "Ref. Name";
            // 
            // nudRefTemp
            // 
            this.nudRefTemp.DecimalPlaces = 1;
            this.nudRefTemp.Enabled = false;
            this.nudRefTemp.Location = new System.Drawing.Point(114, 297);
            this.nudRefTemp.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudRefTemp.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudRefTemp.Name = "nudRefTemp";
            this.nudRefTemp.Size = new System.Drawing.Size(104, 21);
            this.nudRefTemp.TabIndex = 22;
            this.nudRefTemp.ValueChanged += new System.EventHandler(this.nudRefTemp2_ValueChanged);
            // 
            // cbRefValue
            // 
            this.cbRefValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRefValue.Enabled = false;
            this.cbRefValue.FormattingEnabled = true;
            this.cbRefValue.Items.AddRange(new object[] {
            "Max",
            "Min",
            "Avg"});
            this.cbRefValue.Location = new System.Drawing.Point(114, 271);
            this.cbRefValue.Name = "cbRefValue";
            this.cbRefValue.Size = new System.Drawing.Size(104, 20);
            this.cbRefValue.TabIndex = 21;
            this.cbRefValue.SelectedIndexChanged += new System.EventHandler(this.cbRefValue_SelectedIndexChanged);
            // 
            // cbRefName
            // 
            this.cbRefName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRefName.Enabled = false;
            this.cbRefName.FormattingEnabled = true;
            this.cbRefName.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cbRefName.Location = new System.Drawing.Point(114, 245);
            this.cbRefName.Name = "cbRefName";
            this.cbRefName.Size = new System.Drawing.Size(104, 20);
            this.cbRefName.TabIndex = 20;
            this.cbRefName.DropDown += new System.EventHandler(this.cbRefName_DropDown);
            this.cbRefName.SelectedIndexChanged += new System.EventHandler(this.cbRefName_SelectedIndexChanged);
            // 
            // cbIsRefEnabled
            // 
            this.cbIsRefEnabled.AutoSize = true;
            this.cbIsRefEnabled.Location = new System.Drawing.Point(11, 223);
            this.cbIsRefEnabled.Name = "cbIsRefEnabled";
            this.cbIsRefEnabled.Size = new System.Drawing.Size(102, 16);
            this.cbIsRefEnabled.TabIndex = 19;
            this.cbIsRefEnabled.Text = "Use Reference";
            this.cbIsRefEnabled.UseVisualStyleBackColor = true;
            this.cbIsRefEnabled.CheckedChanged += new System.EventHandler(this.cbIsRefEnabled_CheckedChanged);
            // 
            // lblExtOptTempUnit
            // 
            this.lblExtOptTempUnit.AutoSize = true;
            this.lblExtOptTempUnit.Location = new System.Drawing.Point(224, 181);
            this.lblExtOptTempUnit.Name = "lblExtOptTempUnit";
            this.lblExtOptTempUnit.Size = new System.Drawing.Size(23, 12);
            this.lblExtOptTempUnit.TabIndex = 18;
            this.lblExtOptTempUnit.Text = "°C";
            // 
            // nudExtOptTemp
            // 
            this.nudExtOptTemp.DecimalPlaces = 1;
            this.nudExtOptTemp.Location = new System.Drawing.Point(114, 177);
            this.nudExtOptTemp.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudExtOptTemp.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudExtOptTemp.Name = "nudExtOptTemp";
            this.nudExtOptTemp.Size = new System.Drawing.Size(104, 21);
            this.nudExtOptTemp.TabIndex = 17;
            this.nudExtOptTemp.ValueChanged += new System.EventHandler(this.nudExtOptTemp_ValueChanged);
            // 
            // lblExtOptTemp
            // 
            this.lblExtOptTemp.AutoSize = true;
            this.lblExtOptTemp.Location = new System.Drawing.Point(9, 181);
            this.lblExtOptTemp.Name = "lblExtOptTemp";
            this.lblExtOptTemp.Size = new System.Drawing.Size(95, 12);
            this.lblExtOptTemp.TabIndex = 16;
            this.lblExtOptTemp.Text = "Ext. Opt. Temp.";
            // 
            // nudExtOptTrans
            // 
            this.nudExtOptTrans.DecimalPlaces = 2;
            this.nudExtOptTrans.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudExtOptTrans.Location = new System.Drawing.Point(114, 150);
            this.nudExtOptTrans.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.nudExtOptTrans.Name = "nudExtOptTrans";
            this.nudExtOptTrans.Size = new System.Drawing.Size(104, 21);
            this.nudExtOptTrans.TabIndex = 15;
            this.nudExtOptTrans.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudExtOptTrans.ValueChanged += new System.EventHandler(this.nudExtOptTrans_ValueChanged);
            // 
            // lblExtOptTrans
            // 
            this.lblExtOptTrans.AutoSize = true;
            this.lblExtOptTrans.Location = new System.Drawing.Point(9, 154);
            this.lblExtOptTrans.Name = "lblExtOptTrans";
            this.lblExtOptTrans.Size = new System.Drawing.Size(101, 12);
            this.lblExtOptTrans.TabIndex = 14;
            this.lblExtOptTrans.Text = "Ext. Opt. Trans.";
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
            this.nudRelHumidity.ValueChanged += new System.EventHandler(this.nudRelHumidity_ValueChanged);
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
            this.nudAtmTemp.ValueChanged += new System.EventHandler(this.nudAtmTemp_ValueChanged);
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
            this.nudDistance.ValueChanged += new System.EventHandler(this.nudDistance_ValueChanged);
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
            this.nudReflTemp.ValueChanged += new System.EventHandler(this.nudRefTemp_ValueChanged);
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
            this.nudEmissivity.ValueChanged += new System.EventHandler(this.nudEmissivity_ValueChanged);
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
            // tpProperties
            // 
            this.tpProperties.Controls.Add(this.pbLogo);
            this.tpProperties.Controls.Add(this.lvProperties);
            this.tpProperties.Location = new System.Drawing.Point(4, 22);
            this.tpProperties.Name = "tpProperties";
            this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperties.Size = new System.Drawing.Size(478, 537);
            this.tpProperties.TabIndex = 0;
            this.tpProperties.Text = "Properties";
            this.tpProperties.UseVisualStyleBackColor = true;
            // 
            // pbLogo
            // 
            this.pbLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbLogo.BackColor = System.Drawing.Color.LightGray;
            this.pbLogo.Location = new System.Drawing.Point(6, 390);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(466, 65);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbLogo.TabIndex = 16;
            this.pbLogo.TabStop = false;
            // 
            // lvProperties
            // 
            this.lvProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
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
            listViewGroup4.Header = "Size";
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
            this.lvProperties.Location = new System.Drawing.Point(6, 6);
            this.lvProperties.Name = "lvProperties";
            this.lvProperties.Size = new System.Drawing.Size(466, 378);
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
            // tpAnnotation
            // 
            this.tpAnnotation.Controls.Add(this.gbNoteAnnotation);
            this.tpAnnotation.Controls.Add(this.gbTagAnnotation);
            this.tpAnnotation.Controls.Add(this.gbAudioAnnotation);
            this.tpAnnotation.Location = new System.Drawing.Point(4, 22);
            this.tpAnnotation.Name = "tpAnnotation";
            this.tpAnnotation.Padding = new System.Windows.Forms.Padding(3);
            this.tpAnnotation.Size = new System.Drawing.Size(478, 537);
            this.tpAnnotation.TabIndex = 1;
            this.tpAnnotation.Text = "Annotation";
            this.tpAnnotation.UseVisualStyleBackColor = true;
            // 
            // gbNoteAnnotation
            // 
            this.gbNoteAnnotation.Controls.Add(this.tbNoteAnnotation);
            this.gbNoteAnnotation.Location = new System.Drawing.Point(6, 63);
            this.gbNoteAnnotation.Name = "gbNoteAnnotation";
            this.gbNoteAnnotation.Size = new System.Drawing.Size(303, 104);
            this.gbNoteAnnotation.TabIndex = 2;
            this.gbNoteAnnotation.TabStop = false;
            this.gbNoteAnnotation.Text = "Note Annotation";
            // 
            // tbNoteAnnotation
            // 
            this.tbNoteAnnotation.Location = new System.Drawing.Point(6, 20);
            this.tbNoteAnnotation.Multiline = true;
            this.tbNoteAnnotation.Name = "tbNoteAnnotation";
            this.tbNoteAnnotation.Size = new System.Drawing.Size(291, 78);
            this.tbNoteAnnotation.TabIndex = 10;
            this.tbNoteAnnotation.TextChanged += new System.EventHandler(this.tbNoteAnnotation_TextChanged);
            // 
            // gbTagAnnotation
            // 
            this.gbTagAnnotation.Controls.Add(this.lblAddTag);
            this.gbTagAnnotation.Controls.Add(this.lblEditTag);
            this.gbTagAnnotation.Controls.Add(this.tbEditTag);
            this.gbTagAnnotation.Controls.Add(this.tbAddTag);
            this.gbTagAnnotation.Controls.Add(this.btnEditTag);
            this.gbTagAnnotation.Controls.Add(this.btnAddTag);
            this.gbTagAnnotation.Controls.Add(this.lvTagAnnotation);
            this.gbTagAnnotation.Location = new System.Drawing.Point(6, 173);
            this.gbTagAnnotation.Name = "gbTagAnnotation";
            this.gbTagAnnotation.Size = new System.Drawing.Size(303, 285);
            this.gbTagAnnotation.TabIndex = 1;
            this.gbTagAnnotation.TabStop = false;
            this.gbTagAnnotation.Text = "Tag Annotation";
            // 
            // lblAddTag
            // 
            this.lblAddTag.AutoSize = true;
            this.lblAddTag.Location = new System.Drawing.Point(4, 243);
            this.lblAddTag.Name = "lblAddTag";
            this.lblAddTag.Size = new System.Drawing.Size(53, 12);
            this.lblAddTag.TabIndex = 11;
            this.lblAddTag.Text = "Add Tag:";
            // 
            // lblEditTag
            // 
            this.lblEditTag.AutoSize = true;
            this.lblEditTag.Location = new System.Drawing.Point(4, 204);
            this.lblEditTag.Name = "lblEditTag";
            this.lblEditTag.Size = new System.Drawing.Size(59, 12);
            this.lblEditTag.TabIndex = 10;
            this.lblEditTag.Text = "Edit Tag:";
            // 
            // tbEditTag
            // 
            this.tbEditTag.Location = new System.Drawing.Point(6, 219);
            this.tbEditTag.Name = "tbEditTag";
            this.tbEditTag.Size = new System.Drawing.Size(250, 21);
            this.tbEditTag.TabIndex = 9;
            // 
            // tbAddTag
            // 
            this.tbAddTag.Location = new System.Drawing.Point(6, 258);
            this.tbAddTag.Name = "tbAddTag";
            this.tbAddTag.Size = new System.Drawing.Size(250, 21);
            this.tbAddTag.TabIndex = 9;
            // 
            // btnEditTag
            // 
            this.btnEditTag.Location = new System.Drawing.Point(262, 219);
            this.btnEditTag.Name = "btnEditTag";
            this.btnEditTag.Size = new System.Drawing.Size(34, 21);
            this.btnEditTag.TabIndex = 7;
            this.btnEditTag.Text = "Set";
            this.btnEditTag.UseVisualStyleBackColor = true;
            this.btnEditTag.Click += new System.EventHandler(this.btnEditTag_Click);
            // 
            // btnAddTag
            // 
            this.btnAddTag.Location = new System.Drawing.Point(262, 258);
            this.btnAddTag.Name = "btnAddTag";
            this.btnAddTag.Size = new System.Drawing.Size(34, 21);
            this.btnAddTag.TabIndex = 7;
            this.btnAddTag.Text = "Add";
            this.btnAddTag.UseVisualStyleBackColor = true;
            this.btnAddTag.Click += new System.EventHandler(this.btnAddTag_Click);
            // 
            // lvTagAnnotation
            // 
            this.lvTagAnnotation.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTag});
            this.lvTagAnnotation.FullRowSelect = true;
            this.lvTagAnnotation.GridLines = true;
            this.lvTagAnnotation.HideSelection = false;
            this.lvTagAnnotation.LabelEdit = true;
            this.lvTagAnnotation.Location = new System.Drawing.Point(6, 18);
            this.lvTagAnnotation.MultiSelect = false;
            this.lvTagAnnotation.Name = "lvTagAnnotation";
            this.lvTagAnnotation.Size = new System.Drawing.Size(291, 183);
            this.lvTagAnnotation.TabIndex = 6;
            this.lvTagAnnotation.UseCompatibleStateImageBehavior = false;
            this.lvTagAnnotation.View = System.Windows.Forms.View.Details;
            this.lvTagAnnotation.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvTagAnnotation_ItemSelectionChanged);
            this.lvTagAnnotation.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lvTagAnnotation_PreviewKeyDown);
            // 
            // chTag
            // 
            this.chTag.Text = "Tag";
            this.chTag.Width = 250;
            // 
            // gbAudioAnnotation
            // 
            this.gbAudioAnnotation.Controls.Add(this.btnAudioPause);
            this.gbAudioAnnotation.Controls.Add(this.btnAudioStop);
            this.gbAudioAnnotation.Controls.Add(this.btnAudioPlay);
            this.gbAudioAnnotation.Location = new System.Drawing.Point(6, 6);
            this.gbAudioAnnotation.Name = "gbAudioAnnotation";
            this.gbAudioAnnotation.Size = new System.Drawing.Size(133, 51);
            this.gbAudioAnnotation.TabIndex = 0;
            this.gbAudioAnnotation.TabStop = false;
            this.gbAudioAnnotation.Text = "Audio Annotation";
            // 
            // btnAudioPause
            // 
            this.btnAudioPause.Image = ((System.Drawing.Image)(resources.GetObject("btnAudioPause.Image")));
            this.btnAudioPause.Location = new System.Drawing.Point(46, 18);
            this.btnAudioPause.Name = "btnAudioPause";
            this.btnAudioPause.Size = new System.Drawing.Size(34, 24);
            this.btnAudioPause.TabIndex = 2;
            this.btnAudioPause.UseVisualStyleBackColor = true;
            this.btnAudioPause.Click += new System.EventHandler(this.btnAudioPause_Click);
            // 
            // btnAudioStop
            // 
            this.btnAudioStop.Image = ((System.Drawing.Image)(resources.GetObject("btnAudioStop.Image")));
            this.btnAudioStop.Location = new System.Drawing.Point(86, 18);
            this.btnAudioStop.Name = "btnAudioStop";
            this.btnAudioStop.Size = new System.Drawing.Size(34, 24);
            this.btnAudioStop.TabIndex = 1;
            this.btnAudioStop.UseVisualStyleBackColor = true;
            this.btnAudioStop.Click += new System.EventHandler(this.btnAudioStop_Click);
            // 
            // btnAudioPlay
            // 
            this.btnAudioPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnAudioPlay.Image")));
            this.btnAudioPlay.Location = new System.Drawing.Point(6, 18);
            this.btnAudioPlay.Name = "btnAudioPlay";
            this.btnAudioPlay.Size = new System.Drawing.Size(34, 24);
            this.btnAudioPlay.TabIndex = 0;
            this.btnAudioPlay.UseVisualStyleBackColor = true;
            this.btnAudioPlay.Click += new System.EventHandler(this.btnAudioPlay_Click);
            // 
            // tpUserData
            // 
            this.tpUserData.Controls.Add(this.tbAddUserDataKey);
            this.tpUserData.Controls.Add(this.lblAddUserData);
            this.tpUserData.Controls.Add(this.lblEditUserData);
            this.tpUserData.Controls.Add(this.rtbEditUserData);
            this.tpUserData.Controls.Add(this.rtbAddUserDataValue);
            this.tpUserData.Controls.Add(this.btnEditUserData);
            this.tpUserData.Controls.Add(this.btnAddUserData);
            this.tpUserData.Controls.Add(this.lvUserData);
            this.tpUserData.Location = new System.Drawing.Point(4, 22);
            this.tpUserData.Name = "tpUserData";
            this.tpUserData.Padding = new System.Windows.Forms.Padding(3);
            this.tpUserData.Size = new System.Drawing.Size(478, 537);
            this.tpUserData.TabIndex = 4;
            this.tpUserData.Text = "User Data";
            this.tpUserData.UseVisualStyleBackColor = true;
            // 
            // tbAddUserDataKey
            // 
            this.tbAddUserDataKey.Location = new System.Drawing.Point(4, 329);
            this.tbAddUserDataKey.Name = "tbAddUserDataKey";
            this.tbAddUserDataKey.Size = new System.Drawing.Size(250, 21);
            this.tbAddUserDataKey.TabIndex = 18;
            // 
            // lblAddUserData
            // 
            this.lblAddUserData.AutoSize = true;
            this.lblAddUserData.Location = new System.Drawing.Point(4, 314);
            this.lblAddUserData.Name = "lblAddUserData";
            this.lblAddUserData.Size = new System.Drawing.Size(83, 12);
            this.lblAddUserData.TabIndex = 17;
            this.lblAddUserData.Text = "Add UserData:";
            // 
            // lblEditUserData
            // 
            this.lblEditUserData.AutoSize = true;
            this.lblEditUserData.Location = new System.Drawing.Point(6, 193);
            this.lblEditUserData.Name = "lblEditUserData";
            this.lblEditUserData.Size = new System.Drawing.Size(89, 12);
            this.lblEditUserData.TabIndex = 16;
            this.lblEditUserData.Text = "Edit UserData:";
            // 
            // rtbEditUserData
            // 
            this.rtbEditUserData.Location = new System.Drawing.Point(6, 208);
            this.rtbEditUserData.Name = "rtbEditUserData";
            this.rtbEditUserData.Size = new System.Drawing.Size(250, 100);
            this.rtbEditUserData.TabIndex = 14;
            this.rtbEditUserData.Text = "";
            // 
            // rtbAddUserDataValue
            // 
            this.rtbAddUserDataValue.Location = new System.Drawing.Point(4, 356);
            this.rtbAddUserDataValue.Name = "rtbAddUserDataValue";
            this.rtbAddUserDataValue.Size = new System.Drawing.Size(250, 100);
            this.rtbAddUserDataValue.TabIndex = 15;
            this.rtbAddUserDataValue.Text = "";
            // 
            // btnEditUserData
            // 
            this.btnEditUserData.Location = new System.Drawing.Point(262, 287);
            this.btnEditUserData.Name = "btnEditUserData";
            this.btnEditUserData.Size = new System.Drawing.Size(34, 21);
            this.btnEditUserData.TabIndex = 12;
            this.btnEditUserData.Text = "Set";
            this.btnEditUserData.UseVisualStyleBackColor = true;
            this.btnEditUserData.Click += new System.EventHandler(this.btnEditUserData_Click);
            // 
            // btnAddUserData
            // 
            this.btnAddUserData.Location = new System.Drawing.Point(260, 435);
            this.btnAddUserData.Name = "btnAddUserData";
            this.btnAddUserData.Size = new System.Drawing.Size(34, 21);
            this.btnAddUserData.TabIndex = 13;
            this.btnAddUserData.Text = "Add";
            this.btnAddUserData.UseVisualStyleBackColor = true;
            this.btnAddUserData.Click += new System.EventHandler(this.btnAddUserData_Click);
            // 
            // lvUserData
            // 
            this.lvUserData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chUserDataKey,
            this.chUserDataValue});
            this.lvUserData.FullRowSelect = true;
            this.lvUserData.GridLines = true;
            this.lvUserData.HideSelection = false;
            this.lvUserData.Location = new System.Drawing.Point(6, 6);
            this.lvUserData.Name = "lvUserData";
            this.lvUserData.Size = new System.Drawing.Size(385, 183);
            this.lvUserData.TabIndex = 1;
            this.lvUserData.UseCompatibleStateImageBehavior = false;
            this.lvUserData.View = System.Windows.Forms.View.Details;
            this.lvUserData.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvUserData_ItemSelectionChanged);
            this.lvUserData.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lvUserData_PreviewKeyDown);
            // 
            // chUserDataKey
            // 
            this.chUserDataKey.Text = "Key";
            this.chUserDataKey.Width = 100;
            // 
            // chUserDataValue
            // 
            this.chUserDataValue.Text = "Value";
            this.chUserDataValue.Width = 250;
            // 
            // pbCoverImage
            // 
            this.pbCoverImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCoverImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCoverImage.Location = new System.Drawing.Point(879, 59);
            this.pbCoverImage.Name = "pbCoverImage";
            this.pbCoverImage.Size = new System.Drawing.Size(240, 180);
            this.pbCoverImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCoverImage.TabIndex = 15;
            this.pbCoverImage.TabStop = false;
            // 
            // pbVisualImage
            // 
            this.pbVisualImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbVisualImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbVisualImage.Location = new System.Drawing.Point(1125, 59);
            this.pbVisualImage.Name = "pbVisualImage";
            this.pbVisualImage.Size = new System.Drawing.Size(240, 180);
            this.pbVisualImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbVisualImage.TabIndex = 16;
            this.pbVisualImage.TabStop = false;
            // 
            // cmsMarkers
            // 
            this.cmsMarkers.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsMarkers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRename,
            this.tsmiLocalParameters,
            this.tsmiDeltaParameters,
            this.tsmiExportTemperature,
            this.tsmiExportTemperatureCurve});
            this.cmsMarkers.Name = "cmsMarkers";
            this.cmsMarkers.Size = new System.Drawing.Size(240, 114);
            this.cmsMarkers.Opening += new System.ComponentModel.CancelEventHandler(this.cmsMarkers_Opening);
            // 
            // tsmiRename
            // 
            this.tsmiRename.Name = "tsmiRename";
            this.tsmiRename.Size = new System.Drawing.Size(239, 22);
            this.tsmiRename.Text = "Rename";
            this.tsmiRename.Click += new System.EventHandler(this.tsmiRename_Click);
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
            this.tsmiDeltaParameters.Click += new System.EventHandler(this.tsmiDeltaParameters_Click);
            // 
            // tsmiExportTemperature
            // 
            this.tsmiExportTemperature.Name = "tsmiExportTemperature";
            this.tsmiExportTemperature.Size = new System.Drawing.Size(239, 22);
            this.tsmiExportTemperature.Text = "Export Temperature...";
            this.tsmiExportTemperature.Click += new System.EventHandler(this.tsmiExportTemperature_Click);
            // 
            // tsmiExportTemperatureCurve
            // 
            this.tsmiExportTemperatureCurve.Name = "tsmiExportTemperatureCurve";
            this.tsmiExportTemperatureCurve.Size = new System.Drawing.Size(239, 22);
            this.tsmiExportTemperatureCurve.Text = "Export Temperature Curve...";
            this.tsmiExportTemperatureCurve.Click += new System.EventHandler(this.tsmiExportTemperatureCurve_Click);
            // 
            // ttTempValue
            // 
            this.ttTempValue.ShowAlways = true;
            // 
            // btnAutoAdjust
            // 
            this.btnAutoAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoAdjust.Location = new System.Drawing.Point(741, 636);
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
            this.pbPalleteImage.Location = new System.Drawing.Point(741, 59);
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
            this.nudMax.Location = new System.Drawing.Point(774, 59);
            this.nudMax.Maximum = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            this.nudMax.Minimum = new decimal(new int[] {
            300,
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
            this.nudMin.Location = new System.Drawing.Point(774, 609);
            this.nudMin.Maximum = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            this.nudMin.Minimum = new decimal(new int[] {
            300,
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
            // nudAlarmMin
            // 
            this.nudAlarmMin.DecimalPlaces = 1;
            this.nudAlarmMin.Location = new System.Drawing.Point(0, 54);
            this.nudAlarmMin.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudAlarmMin.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudAlarmMin.Name = "nudAlarmMin";
            this.nudAlarmMin.Size = new System.Drawing.Size(60, 21);
            this.nudAlarmMin.TabIndex = 25;
            this.nudAlarmMin.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudAlarmMin.ValueChanged += new System.EventHandler(this.nudAlarmMin_ValueChanged);
            // 
            // nudAlarmMax
            // 
            this.nudAlarmMax.DecimalPlaces = 1;
            this.nudAlarmMax.Location = new System.Drawing.Point(0, 15);
            this.nudAlarmMax.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudAlarmMax.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudAlarmMax.Name = "nudAlarmMax";
            this.nudAlarmMax.Size = new System.Drawing.Size(60, 21);
            this.nudAlarmMax.TabIndex = 26;
            this.nudAlarmMax.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudAlarmMax.ValueChanged += new System.EventHandler(this.nudAlarmMax_ValueChanged);
            // 
            // lblAlarm
            // 
            this.lblAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAlarm.AutoSize = true;
            this.lblAlarm.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblAlarm.Location = new System.Drawing.Point(774, 147);
            this.lblAlarm.Name = "lblAlarm";
            this.lblAlarm.Size = new System.Drawing.Size(40, 12);
            this.lblAlarm.TabIndex = 27;
            this.lblAlarm.Text = "Alarm";
            // 
            // nudSpan
            // 
            this.nudSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSpan.DecimalPlaces = 1;
            this.nudSpan.Enabled = false;
            this.nudSpan.Location = new System.Drawing.Point(774, 108);
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
            this.cbFixedSpan.Location = new System.Drawing.Point(774, 86);
            this.cbFixedSpan.Name = "cbFixedSpan";
            this.cbFixedSpan.Size = new System.Drawing.Size(84, 16);
            this.cbFixedSpan.TabIndex = 30;
            this.cbFixedSpan.Text = "Fixed Span";
            this.cbFixedSpan.UseVisualStyleBackColor = true;
            this.cbFixedSpan.CheckedChanged += new System.EventHandler(this.cbFixedSpan_CheckedChanged);
            // 
            // cbAlarmCustom
            // 
            this.cbAlarmCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAlarmCustom.AutoSize = true;
            this.cbAlarmCustom.Location = new System.Drawing.Point(774, 162);
            this.cbAlarmCustom.Name = "cbAlarmCustom";
            this.cbAlarmCustom.Size = new System.Drawing.Size(60, 16);
            this.cbAlarmCustom.TabIndex = 31;
            this.cbAlarmCustom.Text = "Custom";
            this.cbAlarmCustom.UseVisualStyleBackColor = true;
            this.cbAlarmCustom.CheckedChanged += new System.EventHandler(this.cbAlarmCustom_CheckedChanged);
            // 
            // btnAlarmColor
            // 
            this.btnAlarmColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlarmColor.Enabled = false;
            this.btnAlarmColor.Location = new System.Drawing.Point(774, 200);
            this.btnAlarmColor.Name = "btnAlarmColor";
            this.btnAlarmColor.Size = new System.Drawing.Size(28, 28);
            this.btnAlarmColor.TabIndex = 32;
            this.btnAlarmColor.UseVisualStyleBackColor = true;
            this.btnAlarmColor.Click += new System.EventHandler(this.btnAlarmColor_Click);
            // 
            // nudAlarmAlpha
            // 
            this.nudAlarmAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudAlarmAlpha.Enabled = false;
            this.nudAlarmAlpha.Location = new System.Drawing.Point(774, 246);
            this.nudAlarmAlpha.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudAlarmAlpha.Name = "nudAlarmAlpha";
            this.nudAlarmAlpha.Size = new System.Drawing.Size(60, 21);
            this.nudAlarmAlpha.TabIndex = 33;
            this.nudAlarmAlpha.ValueChanged += new System.EventHandler(this.nudAlarmAlpha_ValueChanged);
            // 
            // lblAlarmColor
            // 
            this.lblAlarmColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAlarmColor.AutoSize = true;
            this.lblAlarmColor.Location = new System.Drawing.Point(774, 185);
            this.lblAlarmColor.Name = "lblAlarmColor";
            this.lblAlarmColor.Size = new System.Drawing.Size(35, 12);
            this.lblAlarmColor.TabIndex = 34;
            this.lblAlarmColor.Text = "Color";
            // 
            // lblAlarmAlpha
            // 
            this.lblAlarmAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAlarmAlpha.AutoSize = true;
            this.lblAlarmAlpha.Location = new System.Drawing.Point(774, 231);
            this.lblAlarmAlpha.Name = "lblAlarmAlpha";
            this.lblAlarmAlpha.Size = new System.Drawing.Size(35, 12);
            this.lblAlarmAlpha.TabIndex = 35;
            this.lblAlarmAlpha.Text = "Alpha";
            // 
            // lblAlarmMax
            // 
            this.lblAlarmMax.AutoSize = true;
            this.lblAlarmMax.Location = new System.Drawing.Point(0, 0);
            this.lblAlarmMax.Name = "lblAlarmMax";
            this.lblAlarmMax.Size = new System.Drawing.Size(23, 12);
            this.lblAlarmMax.TabIndex = 36;
            this.lblAlarmMax.Text = "Max";
            // 
            // lblAlarmMin
            // 
            this.lblAlarmMin.AutoSize = true;
            this.lblAlarmMin.Location = new System.Drawing.Point(0, 39);
            this.lblAlarmMin.Name = "lblAlarmMin";
            this.lblAlarmMin.Size = new System.Drawing.Size(23, 12);
            this.lblAlarmMin.TabIndex = 37;
            this.lblAlarmMin.Text = "Min";
            // 
            // lblMaxUnit
            // 
            this.lblMaxUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMaxUnit.AutoSize = true;
            this.lblMaxUnit.Location = new System.Drawing.Point(840, 63);
            this.lblMaxUnit.Name = "lblMaxUnit";
            this.lblMaxUnit.Size = new System.Drawing.Size(23, 12);
            this.lblMaxUnit.TabIndex = 38;
            this.lblMaxUnit.Text = "°C";
            // 
            // lblSpanUnit
            // 
            this.lblSpanUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpanUnit.AutoSize = true;
            this.lblSpanUnit.Location = new System.Drawing.Point(840, 112);
            this.lblSpanUnit.Name = "lblSpanUnit";
            this.lblSpanUnit.Size = new System.Drawing.Size(23, 12);
            this.lblSpanUnit.TabIndex = 39;
            this.lblSpanUnit.Text = "°C";
            // 
            // lblAlarmMaxUnit
            // 
            this.lblAlarmMaxUnit.AutoSize = true;
            this.lblAlarmMaxUnit.Location = new System.Drawing.Point(66, 19);
            this.lblAlarmMaxUnit.Name = "lblAlarmMaxUnit";
            this.lblAlarmMaxUnit.Size = new System.Drawing.Size(23, 12);
            this.lblAlarmMaxUnit.TabIndex = 40;
            this.lblAlarmMaxUnit.Text = "°C";
            // 
            // lblAlarmMinUnit
            // 
            this.lblAlarmMinUnit.AutoSize = true;
            this.lblAlarmMinUnit.Location = new System.Drawing.Point(66, 58);
            this.lblAlarmMinUnit.Name = "lblAlarmMinUnit";
            this.lblAlarmMinUnit.Size = new System.Drawing.Size(23, 12);
            this.lblAlarmMinUnit.TabIndex = 41;
            this.lblAlarmMinUnit.Text = "°C";
            // 
            // lblMinUnit
            // 
            this.lblMinUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMinUnit.AutoSize = true;
            this.lblMinUnit.Location = new System.Drawing.Point(840, 613);
            this.lblMinUnit.Name = "lblMinUnit";
            this.lblMinUnit.Size = new System.Drawing.Size(23, 12);
            this.lblMinUnit.TabIndex = 42;
            this.lblMinUnit.Text = "°C";
            // 
            // lblView
            // 
            this.lblView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblView.AutoSize = true;
            this.lblView.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblView.Location = new System.Drawing.Point(774, 409);
            this.lblView.Name = "lblView";
            this.lblView.Size = new System.Drawing.Size(33, 12);
            this.lblView.TabIndex = 43;
            this.lblView.Text = "View";
            // 
            // lblViewMinUnit
            // 
            this.lblViewMinUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewMinUnit.AutoSize = true;
            this.lblViewMinUnit.Location = new System.Drawing.Point(840, 521);
            this.lblViewMinUnit.Name = "lblViewMinUnit";
            this.lblViewMinUnit.Size = new System.Drawing.Size(23, 12);
            this.lblViewMinUnit.TabIndex = 51;
            this.lblViewMinUnit.Text = "°C";
            // 
            // lblViewMaxUnit
            // 
            this.lblViewMaxUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewMaxUnit.AutoSize = true;
            this.lblViewMaxUnit.Location = new System.Drawing.Point(840, 482);
            this.lblViewMaxUnit.Name = "lblViewMaxUnit";
            this.lblViewMaxUnit.Size = new System.Drawing.Size(23, 12);
            this.lblViewMaxUnit.TabIndex = 50;
            this.lblViewMaxUnit.Text = "°C";
            // 
            // lblViewMin
            // 
            this.lblViewMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewMin.AutoSize = true;
            this.lblViewMin.Location = new System.Drawing.Point(774, 502);
            this.lblViewMin.Name = "lblViewMin";
            this.lblViewMin.Size = new System.Drawing.Size(23, 12);
            this.lblViewMin.TabIndex = 49;
            this.lblViewMin.Text = "Min";
            // 
            // lblViewMax
            // 
            this.lblViewMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewMax.AutoSize = true;
            this.lblViewMax.Location = new System.Drawing.Point(774, 463);
            this.lblViewMax.Name = "lblViewMax";
            this.lblViewMax.Size = new System.Drawing.Size(23, 12);
            this.lblViewMax.TabIndex = 48;
            this.lblViewMax.Text = "Max";
            // 
            // lblViewAlpha
            // 
            this.lblViewAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewAlpha.AutoSize = true;
            this.lblViewAlpha.Location = new System.Drawing.Point(774, 424);
            this.lblViewAlpha.Name = "lblViewAlpha";
            this.lblViewAlpha.Size = new System.Drawing.Size(35, 12);
            this.lblViewAlpha.TabIndex = 47;
            this.lblViewAlpha.Text = "Alpha";
            // 
            // nudViewAlpha
            // 
            this.nudViewAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudViewAlpha.Location = new System.Drawing.Point(774, 439);
            this.nudViewAlpha.Name = "nudViewAlpha";
            this.nudViewAlpha.Size = new System.Drawing.Size(60, 21);
            this.nudViewAlpha.TabIndex = 46;
            this.nudViewAlpha.ValueChanged += new System.EventHandler(this.nudViewAlpha_ValueChanged);
            // 
            // nudViewMax
            // 
            this.nudViewMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudViewMax.DecimalPlaces = 1;
            this.nudViewMax.Location = new System.Drawing.Point(774, 478);
            this.nudViewMax.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudViewMax.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudViewMax.Name = "nudViewMax";
            this.nudViewMax.Size = new System.Drawing.Size(60, 21);
            this.nudViewMax.TabIndex = 45;
            this.nudViewMax.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudViewMax.ValueChanged += new System.EventHandler(this.nudViewMax_ValueChanged);
            // 
            // nudViewMin
            // 
            this.nudViewMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudViewMin.DecimalPlaces = 1;
            this.nudViewMin.Location = new System.Drawing.Point(774, 517);
            this.nudViewMin.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudViewMin.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudViewMin.Name = "nudViewMin";
            this.nudViewMin.Size = new System.Drawing.Size(60, 21);
            this.nudViewMin.TabIndex = 44;
            this.nudViewMin.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudViewMin.ValueChanged += new System.EventHandler(this.nudViewMin_ValueChanged);
            // 
            // lblViewArea
            // 
            this.lblViewArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewArea.AutoSize = true;
            this.lblViewArea.Location = new System.Drawing.Point(774, 540);
            this.lblViewArea.Name = "lblViewArea";
            this.lblViewArea.Size = new System.Drawing.Size(29, 12);
            this.lblViewArea.TabIndex = 52;
            this.lblViewArea.Text = "Area";
            // 
            // nudViewAreaX
            // 
            this.nudViewAreaX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudViewAreaX.DecimalPlaces = 3;
            this.nudViewAreaX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudViewAreaX.Location = new System.Drawing.Point(774, 555);
            this.nudViewAreaX.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudViewAreaX.Name = "nudViewAreaX";
            this.nudViewAreaX.Size = new System.Drawing.Size(50, 21);
            this.nudViewAreaX.TabIndex = 53;
            this.nudViewAreaX.ValueChanged += new System.EventHandler(this.nudViewAreaX_ValueChanged);
            // 
            // nudViewAreaY
            // 
            this.nudViewAreaY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudViewAreaY.DecimalPlaces = 3;
            this.nudViewAreaY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudViewAreaY.Location = new System.Drawing.Point(827, 555);
            this.nudViewAreaY.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudViewAreaY.Name = "nudViewAreaY";
            this.nudViewAreaY.Size = new System.Drawing.Size(50, 21);
            this.nudViewAreaY.TabIndex = 54;
            this.nudViewAreaY.ValueChanged += new System.EventHandler(this.nudViewAreaY_ValueChanged);
            // 
            // nudViewAreaWidth
            // 
            this.nudViewAreaWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudViewAreaWidth.DecimalPlaces = 3;
            this.nudViewAreaWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudViewAreaWidth.Location = new System.Drawing.Point(774, 582);
            this.nudViewAreaWidth.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudViewAreaWidth.Name = "nudViewAreaWidth";
            this.nudViewAreaWidth.Size = new System.Drawing.Size(50, 21);
            this.nudViewAreaWidth.TabIndex = 55;
            this.nudViewAreaWidth.ValueChanged += new System.EventHandler(this.nudViewAreaWidth_ValueChanged);
            // 
            // nudViewAreaHeight
            // 
            this.nudViewAreaHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudViewAreaHeight.DecimalPlaces = 3;
            this.nudViewAreaHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudViewAreaHeight.Location = new System.Drawing.Point(827, 582);
            this.nudViewAreaHeight.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudViewAreaHeight.Name = "nudViewAreaHeight";
            this.nudViewAreaHeight.Size = new System.Drawing.Size(50, 21);
            this.nudViewAreaHeight.TabIndex = 56;
            this.nudViewAreaHeight.ValueChanged += new System.EventHandler(this.nudViewAreaHeight_ValueChanged);
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.Location = new System.Drawing.Point(92, 636);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(34, 28);
            this.btnPause.TabIndex = 59;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Visible = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.Location = new System.Drawing.Point(132, 636);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(34, 28);
            this.btnStop.TabIndex = 58;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
            this.btnPlay.Location = new System.Drawing.Point(52, 636);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(34, 28);
            this.btnPlay.TabIndex = 57;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Visible = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnNextFrame
            // 
            this.btnNextFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNextFrame.Image = ((System.Drawing.Image)(resources.GetObject("btnNextFrame.Image")));
            this.btnNextFrame.Location = new System.Drawing.Point(172, 636);
            this.btnNextFrame.Name = "btnNextFrame";
            this.btnNextFrame.Size = new System.Drawing.Size(34, 28);
            this.btnNextFrame.TabIndex = 60;
            this.btnNextFrame.UseVisualStyleBackColor = true;
            this.btnNextFrame.Visible = false;
            this.btnNextFrame.Click += new System.EventHandler(this.btnNextFrame_Click);
            // 
            // btnPreviousFrame
            // 
            this.btnPreviousFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreviousFrame.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousFrame.Image")));
            this.btnPreviousFrame.Location = new System.Drawing.Point(12, 636);
            this.btnPreviousFrame.Name = "btnPreviousFrame";
            this.btnPreviousFrame.Size = new System.Drawing.Size(34, 28);
            this.btnPreviousFrame.TabIndex = 61;
            this.btnPreviousFrame.UseVisualStyleBackColor = true;
            this.btnPreviousFrame.Visible = false;
            this.btnPreviousFrame.Click += new System.EventHandler(this.btnPreviousFrame_Click);
            // 
            // tPlay
            // 
            this.tPlay.Interval = 40;
            this.tPlay.Tick += new System.EventHandler(this.tPlay_Tick);
            // 
            // nudFromFrame
            // 
            this.nudFromFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudFromFrame.Location = new System.Drawing.Point(454, 640);
            this.nudFromFrame.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudFromFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFromFrame.Name = "nudFromFrame";
            this.nudFromFrame.Size = new System.Drawing.Size(60, 21);
            this.nudFromFrame.TabIndex = 62;
            this.nudFromFrame.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFromFrame.Visible = false;
            this.nudFromFrame.ValueChanged += new System.EventHandler(this.nudFromFrame_ValueChanged);
            // 
            // nudToFrame
            // 
            this.nudToFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudToFrame.Location = new System.Drawing.Point(543, 640);
            this.nudToFrame.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudToFrame.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudToFrame.Name = "nudToFrame";
            this.nudToFrame.Size = new System.Drawing.Size(60, 21);
            this.nudToFrame.TabIndex = 63;
            this.nudToFrame.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudToFrame.Visible = false;
            this.nudToFrame.ValueChanged += new System.EventHandler(this.nudToFrame_ValueChanged);
            // 
            // btnClip
            // 
            this.btnClip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClip.Location = new System.Drawing.Point(609, 636);
            this.btnClip.Name = "btnClip";
            this.btnClip.Size = new System.Drawing.Size(89, 28);
            this.btnClip.TabIndex = 65;
            this.btnClip.Text = "Clip";
            this.btnClip.UseVisualStyleBackColor = true;
            this.btnClip.Visible = false;
            this.btnClip.Click += new System.EventHandler(this.btnClip_Click);
            // 
            // lblFromFrame
            // 
            this.lblFromFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFromFrame.AutoSize = true;
            this.lblFromFrame.Location = new System.Drawing.Point(419, 644);
            this.lblFromFrame.Name = "lblFromFrame";
            this.lblFromFrame.Size = new System.Drawing.Size(29, 12);
            this.lblFromFrame.TabIndex = 66;
            this.lblFromFrame.Text = "From";
            this.lblFromFrame.Visible = false;
            // 
            // lblToFrame
            // 
            this.lblToFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblToFrame.AutoSize = true;
            this.lblToFrame.Location = new System.Drawing.Point(520, 644);
            this.lblToFrame.Name = "lblToFrame";
            this.lblToFrame.Size = new System.Drawing.Size(17, 12);
            this.lblToFrame.TabIndex = 67;
            this.lblToFrame.Text = "To";
            this.lblToFrame.Visible = false;
            // 
            // nudGoto
            // 
            this.nudGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudGoto.Location = new System.Drawing.Point(235, 640);
            this.nudGoto.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudGoto.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGoto.Name = "nudGoto";
            this.nudGoto.Size = new System.Drawing.Size(60, 21);
            this.nudGoto.TabIndex = 69;
            this.nudGoto.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGoto.Visible = false;
            // 
            // btnGoto
            // 
            this.btnGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGoto.Location = new System.Drawing.Point(301, 636);
            this.btnGoto.Name = "btnGoto";
            this.btnGoto.Size = new System.Drawing.Size(89, 28);
            this.btnGoto.TabIndex = 70;
            this.btnGoto.Text = "Goto";
            this.btnGoto.UseVisualStyleBackColor = true;
            this.btnGoto.Visible = false;
            this.btnGoto.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // pAlarmTemp
            // 
            this.pAlarmTemp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pAlarmTemp.Controls.Add(this.lblAlarmMax);
            this.pAlarmTemp.Controls.Add(this.nudAlarmMin);
            this.pAlarmTemp.Controls.Add(this.nudAlarmMax);
            this.pAlarmTemp.Controls.Add(this.lblAlarmMin);
            this.pAlarmTemp.Controls.Add(this.lblAlarmMaxUnit);
            this.pAlarmTemp.Controls.Add(this.lblAlarmMinUnit);
            this.pAlarmTemp.Location = new System.Drawing.Point(774, 270);
            this.pAlarmTemp.Name = "pAlarmTemp";
            this.pAlarmTemp.Size = new System.Drawing.Size(98, 136);
            this.pAlarmTemp.TabIndex = 71;
            this.pAlarmTemp.Visible = false;
            // 
            // pHumidity
            // 
            this.pHumidity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pHumidity.Controls.Add(this.lblRHLimit);
            this.pHumidity.Controls.Add(this.nudRHLimit);
            this.pHumidity.Location = new System.Drawing.Point(774, 270);
            this.pHumidity.Name = "pHumidity";
            this.pHumidity.Size = new System.Drawing.Size(98, 136);
            this.pHumidity.TabIndex = 72;
            this.pHumidity.Visible = false;
            // 
            // lblRHLimit
            // 
            this.lblRHLimit.AutoSize = true;
            this.lblRHLimit.Location = new System.Drawing.Point(0, 0);
            this.lblRHLimit.Name = "lblRHLimit";
            this.lblRHLimit.Size = new System.Drawing.Size(53, 12);
            this.lblRHLimit.TabIndex = 42;
            this.lblRHLimit.Text = "RH Limit";
            // 
            // nudRHLimit
            // 
            this.nudRHLimit.DecimalPlaces = 1;
            this.nudRHLimit.Location = new System.Drawing.Point(0, 15);
            this.nudRHLimit.Name = "nudRHLimit";
            this.nudRHLimit.Size = new System.Drawing.Size(60, 21);
            this.nudRHLimit.TabIndex = 41;
            this.nudRHLimit.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudRHLimit.ValueChanged += new System.EventHandler(this.nudRHLimit_ValueChanged);
            // 
            // pInsulation
            // 
            this.pInsulation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pInsulation.Controls.Add(this.nudHeatIndex);
            this.pInsulation.Controls.Add(this.lblHeatIndex);
            this.pInsulation.Controls.Add(this.lblIndoorTemp);
            this.pInsulation.Controls.Add(this.nudOutdoorTemp);
            this.pInsulation.Controls.Add(this.nudIndoorTemp);
            this.pInsulation.Controls.Add(this.lblOutdoorTemp);
            this.pInsulation.Controls.Add(this.lblIndoorTempUnit);
            this.pInsulation.Controls.Add(this.lblOutdoorTempUnit);
            this.pInsulation.Location = new System.Drawing.Point(774, 270);
            this.pInsulation.Name = "pInsulation";
            this.pInsulation.Size = new System.Drawing.Size(98, 136);
            this.pInsulation.TabIndex = 73;
            this.pInsulation.Visible = false;
            // 
            // nudHeatIndex
            // 
            this.nudHeatIndex.DecimalPlaces = 2;
            this.nudHeatIndex.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudHeatIndex.Location = new System.Drawing.Point(0, 93);
            this.nudHeatIndex.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHeatIndex.Name = "nudHeatIndex";
            this.nudHeatIndex.Size = new System.Drawing.Size(60, 21);
            this.nudHeatIndex.TabIndex = 48;
            this.nudHeatIndex.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            this.nudHeatIndex.ValueChanged += new System.EventHandler(this.nudHeatIndex_ValueChanged);
            // 
            // lblHeatIndex
            // 
            this.lblHeatIndex.AutoSize = true;
            this.lblHeatIndex.Location = new System.Drawing.Point(0, 78);
            this.lblHeatIndex.Name = "lblHeatIndex";
            this.lblHeatIndex.Size = new System.Drawing.Size(65, 12);
            this.lblHeatIndex.TabIndex = 49;
            this.lblHeatIndex.Text = "Heat Index";
            // 
            // lblIndoorTemp
            // 
            this.lblIndoorTemp.AutoSize = true;
            this.lblIndoorTemp.Location = new System.Drawing.Point(0, 0);
            this.lblIndoorTemp.Name = "lblIndoorTemp";
            this.lblIndoorTemp.Size = new System.Drawing.Size(77, 12);
            this.lblIndoorTemp.TabIndex = 44;
            this.lblIndoorTemp.Text = "Indoor Temp.";
            // 
            // nudOutdoorTemp
            // 
            this.nudOutdoorTemp.DecimalPlaces = 1;
            this.nudOutdoorTemp.Location = new System.Drawing.Point(0, 54);
            this.nudOutdoorTemp.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudOutdoorTemp.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudOutdoorTemp.Name = "nudOutdoorTemp";
            this.nudOutdoorTemp.Size = new System.Drawing.Size(60, 21);
            this.nudOutdoorTemp.TabIndex = 42;
            this.nudOutdoorTemp.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudOutdoorTemp.ValueChanged += new System.EventHandler(this.nudOutdoorTemp_ValueChanged);
            // 
            // nudIndoorTemp
            // 
            this.nudIndoorTemp.DecimalPlaces = 1;
            this.nudIndoorTemp.Location = new System.Drawing.Point(0, 15);
            this.nudIndoorTemp.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudIndoorTemp.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudIndoorTemp.Name = "nudIndoorTemp";
            this.nudIndoorTemp.Size = new System.Drawing.Size(60, 21);
            this.nudIndoorTemp.TabIndex = 43;
            this.nudIndoorTemp.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudIndoorTemp.ValueChanged += new System.EventHandler(this.nudIndoorTemp_ValueChanged);
            // 
            // lblOutdoorTemp
            // 
            this.lblOutdoorTemp.AutoSize = true;
            this.lblOutdoorTemp.Location = new System.Drawing.Point(0, 39);
            this.lblOutdoorTemp.Name = "lblOutdoorTemp";
            this.lblOutdoorTemp.Size = new System.Drawing.Size(83, 12);
            this.lblOutdoorTemp.TabIndex = 45;
            this.lblOutdoorTemp.Text = "Outdoor Temp.";
            // 
            // lblIndoorTempUnit
            // 
            this.lblIndoorTempUnit.AutoSize = true;
            this.lblIndoorTempUnit.Location = new System.Drawing.Point(66, 19);
            this.lblIndoorTempUnit.Name = "lblIndoorTempUnit";
            this.lblIndoorTempUnit.Size = new System.Drawing.Size(23, 12);
            this.lblIndoorTempUnit.TabIndex = 46;
            this.lblIndoorTempUnit.Text = "°C";
            // 
            // lblOutdoorTempUnit
            // 
            this.lblOutdoorTempUnit.AutoSize = true;
            this.lblOutdoorTempUnit.Location = new System.Drawing.Point(66, 58);
            this.lblOutdoorTempUnit.Name = "lblOutdoorTempUnit";
            this.lblOutdoorTempUnit.Size = new System.Drawing.Size(23, 12);
            this.lblOutdoorTempUnit.TabIndex = 47;
            this.lblOutdoorTempUnit.Text = "°C";
            // 
            // lblThreshold
            // 
            this.lblThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(811, 185);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(59, 12);
            this.lblThreshold.TabIndex = 74;
            this.lblThreshold.Text = "Threshold";
            // 
            // nudThreshold
            // 
            this.nudThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudThreshold.DecimalPlaces = 1;
            this.nudThreshold.Enabled = false;
            this.nudThreshold.Location = new System.Drawing.Point(813, 200);
            this.nudThreshold.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudThreshold.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudThreshold.Name = "nudThreshold";
            this.nudThreshold.Size = new System.Drawing.Size(60, 21);
            this.nudThreshold.TabIndex = 75;
            this.nudThreshold.Value = new decimal(new int[] {
            800,
            0,
            0,
            0});
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
            this.chY1,
            this.chX2,
            this.chY2,
            this.chArea,
            this.chLocalParameters,
            this.chExInfo});
            this.mlvMarkers.ContextMenuStrip = this.cmsMarkers;
            this.mlvMarkers.FullRowSelect = true;
            this.mlvMarkers.HideSelection = false;
            this.mlvMarkers.Location = new System.Drawing.Point(12, 670);
            this.mlvMarkers.MultiSelect = false;
            this.mlvMarkers.Name = "mlvMarkers";
            this.mlvMarkers.Size = new System.Drawing.Size(861, 138);
            this.mlvMarkers.TabIndex = 14;
            this.mlvMarkers.UseCompatibleStateImageBehavior = false;
            this.mlvMarkers.View = System.Windows.Forms.View.Details;
            this.mlvMarkers.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mlvMarkers_KeyUp);
            // 
            // chName
            // 
            this.chName.Text = "Name";
            // 
            // chValue
            // 
            this.chValue.Text = "Value";
            // 
            // chMin
            // 
            this.chMin.Text = "Min";
            // 
            // chMax
            // 
            this.chMax.Text = "Max";
            // 
            // chAverage
            // 
            this.chAverage.Text = "Average";
            // 
            // chX1
            // 
            this.chX1.Text = "X1/SrcName";
            this.chX1.Width = 80;
            // 
            // chY1
            // 
            this.chY1.Text = "Y1/SrcValue";
            this.chY1.Width = 80;
            // 
            // chX2
            // 
            this.chX2.Text = "X2/DestName";
            this.chX2.Width = 100;
            // 
            // chY2
            // 
            this.chY2.Text = "Y2/DestValue";
            this.chY2.Width = 120;
            // 
            // chArea
            // 
            this.chArea.Text = "Area/Length";
            this.chArea.Width = 80;
            // 
            // chLocalParameters
            // 
            this.chLocalParameters.Text = "Local Parameters";
            this.chLocalParameters.Width = 150;
            // 
            // chExInfo
            // 
            this.chExInfo.Text = "ExInfo";
            this.chExInfo.Width = 150;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1377, 820);
            this.Controls.Add(this.nudThreshold);
            this.Controls.Add(this.lblThreshold);
            this.Controls.Add(this.btnGoto);
            this.Controls.Add(this.nudGoto);
            this.Controls.Add(this.lblToFrame);
            this.Controls.Add(this.lblFromFrame);
            this.Controls.Add(this.btnClip);
            this.Controls.Add(this.nudToFrame);
            this.Controls.Add(this.nudFromFrame);
            this.Controls.Add(this.btnPreviousFrame);
            this.Controls.Add(this.btnNextFrame);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.nudViewAreaHeight);
            this.Controls.Add(this.nudViewAreaWidth);
            this.Controls.Add(this.nudViewAreaY);
            this.Controls.Add(this.nudViewAreaX);
            this.Controls.Add(this.lblViewArea);
            this.Controls.Add(this.lblViewMinUnit);
            this.Controls.Add(this.lblViewMaxUnit);
            this.Controls.Add(this.lblViewMin);
            this.Controls.Add(this.lblViewMax);
            this.Controls.Add(this.lblViewAlpha);
            this.Controls.Add(this.nudViewAlpha);
            this.Controls.Add(this.nudViewMax);
            this.Controls.Add(this.nudViewMin);
            this.Controls.Add(this.lblView);
            this.Controls.Add(this.lblMinUnit);
            this.Controls.Add(this.lblSpanUnit);
            this.Controls.Add(this.lblMaxUnit);
            this.Controls.Add(this.lblAlarmAlpha);
            this.Controls.Add(this.lblAlarmColor);
            this.Controls.Add(this.nudAlarmAlpha);
            this.Controls.Add(this.btnAlarmColor);
            this.Controls.Add(this.cbAlarmCustom);
            this.Controls.Add(this.cbFixedSpan);
            this.Controls.Add(this.nudSpan);
            this.Controls.Add(this.lblAlarm);
            this.Controls.Add(this.nudMin);
            this.Controls.Add(this.nudMax);
            this.Controls.Add(this.btnAutoAdjust);
            this.Controls.Add(this.pbPalleteImage);
            this.Controls.Add(this.pbVisualImage);
            this.Controls.Add(this.pbCoverImage);
            this.Controls.Add(this.mlvMarkers);
            this.Controls.Add(this.tcThermalFile);
            this.Controls.Add(this.pbThermalImage);
            this.Controls.Add(this.tsMainForm);
            this.Controls.Add(this.msMainForm);
            this.Controls.Add(this.pInsulation);
            this.Controls.Add(this.pHumidity);
            this.Controls.Add(this.pAlarmTemp);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo ThermalFileReader";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.msMainForm.ResumeLayout(false);
            this.msMainForm.PerformLayout();
            this.tsMainForm.ResumeLayout(false);
            this.tsMainForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbThermalImage)).EndInit();
            this.cmsThermalImage.ResumeLayout(false);
            this.tcThermalFile.ResumeLayout(false);
            this.tpThermalParams.ResumeLayout(false);
            this.tpThermalParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRealRefTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRefTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtOptTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudExtOptTrans)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRelHumidity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAtmTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudReflTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEmissivity)).EndInit();
            this.tpProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.tpAnnotation.ResumeLayout(false);
            this.gbNoteAnnotation.ResumeLayout(false);
            this.gbNoteAnnotation.PerformLayout();
            this.gbTagAnnotation.ResumeLayout(false);
            this.gbTagAnnotation.PerformLayout();
            this.gbAudioAnnotation.ResumeLayout(false);
            this.tpUserData.ResumeLayout(false);
            this.tpUserData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoverImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualImage)).EndInit();
            this.cmsMarkers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPalleteImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAlarmAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewAreaHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFromFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudToFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudGoto)).EndInit();
            this.pAlarmTemp.ResumeLayout(false);
            this.pAlarmTemp.PerformLayout();
            this.pHumidity.ResumeLayout(false);
            this.pHumidity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRHLimit)).EndInit();
            this.pInsulation.ResumeLayout(false);
            this.pInsulation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeatIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOutdoorTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIndoorTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip msMainForm;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
        private System.Windows.Forms.ToolStripMenuItem tsmiTransform;
        private System.Windows.Forms.ToolStrip tsMainForm;
        private System.Windows.Forms.ToolStripButton tsbSave;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel tslColorDistributionType;
        private System.Windows.Forms.ToolStripComboBox tscbColorDistributionType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel tslViewType;
        private System.Windows.Forms.ToolStripComboBox tscbViewType;
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
        private System.Windows.Forms.TabPage tpAnnotation;
        private System.Windows.Forms.GroupBox gbTagAnnotation;
        private System.Windows.Forms.Label lblAddTag;
        private System.Windows.Forms.Label lblEditTag;
        private System.Windows.Forms.TextBox tbEditTag;
        private System.Windows.Forms.TextBox tbAddTag;
        private System.Windows.Forms.Button btnEditTag;
        private System.Windows.Forms.Button btnAddTag;
        private System.Windows.Forms.ListView lvTagAnnotation;
        private System.Windows.Forms.ColumnHeader chTag;
        private System.Windows.Forms.GroupBox gbAudioAnnotation;
        private System.Windows.Forms.Button btnAudioPause;
        private System.Windows.Forms.Button btnAudioStop;
        private System.Windows.Forms.Button btnAudioPlay;
        private System.Windows.Forms.Label lblExtOptTempUnit;
        private System.Windows.Forms.NumericUpDown nudExtOptTemp;
        private System.Windows.Forms.Label lblExtOptTemp;
        private System.Windows.Forms.NumericUpDown nudExtOptTrans;
        private System.Windows.Forms.Label lblExtOptTrans;
        private System.Windows.Forms.ListView lvProperties;
        private System.Windows.Forms.ColumnHeader chPropertyKey;
        private System.Windows.Forms.ColumnHeader chPropertyValue;
        private System.Windows.Forms.GroupBox gbNoteAnnotation;
        private System.Windows.Forms.TextBox tbNoteAnnotation;
        private MarkersListView mlvMarkers;
        private System.Windows.Forms.TabPage tpUserData;
        private System.Windows.Forms.ListView lvUserData;
        private System.Windows.Forms.ColumnHeader chUserDataKey;
        private System.Windows.Forms.ColumnHeader chUserDataValue;
        private System.Windows.Forms.Label lblAddUserData;
        private System.Windows.Forms.Label lblEditUserData;
        private System.Windows.Forms.RichTextBox rtbEditUserData;
        private System.Windows.Forms.RichTextBox rtbAddUserDataValue;
        private System.Windows.Forms.Button btnEditUserData;
        private System.Windows.Forms.Button btnAddUserData;
        private System.Windows.Forms.TextBox tbAddUserDataKey;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chValue;
        private System.Windows.Forms.ColumnHeader chMin;
        private System.Windows.Forms.ColumnHeader chMax;
        private System.Windows.Forms.ColumnHeader chAverage;
        private System.Windows.Forms.ColumnHeader chX1;
        private System.Windows.Forms.ColumnHeader chY1;
        private System.Windows.Forms.ColumnHeader chX2;
        private System.Windows.Forms.ColumnHeader chY2;
        private System.Windows.Forms.PictureBox pbCoverImage;
        private System.Windows.Forms.PictureBox pbVisualImage;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.ToolStripButton tsbEllipse;
        private System.Windows.Forms.CheckBox cbIsRefEnabled;
        private System.Windows.Forms.ComboBox cbRefName;
        private System.Windows.Forms.ComboBox cbRefValue;
        private System.Windows.Forms.NumericUpDown nudRefTemp;
        private System.Windows.Forms.Label lblRefName;
        private System.Windows.Forms.Label lblRefValue;
        private System.Windows.Forms.Label lblRefTemp;
        private System.Windows.Forms.Label lblRefTempUnit;
        private System.Windows.Forms.ContextMenuStrip cmsMarkers;
        private System.Windows.Forms.ToolStripMenuItem tsmiRename;
        private System.Windows.Forms.ToolStripMenuItem tsmiLocalParameters;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeltaParameters;
        private System.Windows.Forms.ToolStripButton tsbDelta;
        private System.Windows.Forms.ColumnHeader chLocalParameters;
        private System.Windows.Forms.ToolTip ttTempValue;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportTemperature;
        private System.Windows.Forms.ToolStripMenuItem tsmiRotate;
        private System.Windows.Forms.ToolStripMenuItem tsmiHorizontalMirror;
        private System.Windows.Forms.Button btnAutoAdjust;
        private System.Windows.Forms.PictureBox pbPalleteImage;
        private System.Windows.Forms.NumericUpDown nudMax;
        private System.Windows.Forms.NumericUpDown nudMin;
        private System.Windows.Forms.NumericUpDown nudAlarmMin;
        private System.Windows.Forms.NumericUpDown nudAlarmMax;
        private System.Windows.Forms.Label lblAlarm;
        private System.Windows.Forms.NumericUpDown nudSpan;
        private System.Windows.Forms.CheckBox cbFixedSpan;
        private System.Windows.Forms.ToolStripLabel tslAlarm;
        private System.Windows.Forms.ToolStripComboBox tscbAlarm;
        private System.Windows.Forms.CheckBox cbAlarmCustom;
        private System.Windows.Forms.Button btnAlarmColor;
        private System.Windows.Forms.NumericUpDown nudAlarmAlpha;
        private System.Windows.Forms.Label lblAlarmColor;
        private System.Windows.Forms.Label lblAlarmAlpha;
        private System.Windows.Forms.Label lblAlarmMax;
        private System.Windows.Forms.Label lblAlarmMin;
        private System.Windows.Forms.Label lblMaxUnit;
        private System.Windows.Forms.Label lblSpanUnit;
        private System.Windows.Forms.Label lblAlarmMaxUnit;
        private System.Windows.Forms.Label lblAlarmMinUnit;
        private System.Windows.Forms.Label lblMinUnit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.Label lblView;
        private System.Windows.Forms.Label lblViewMinUnit;
        private System.Windows.Forms.Label lblViewMaxUnit;
        private System.Windows.Forms.Label lblViewMin;
        private System.Windows.Forms.Label lblViewMax;
        private System.Windows.Forms.Label lblViewAlpha;
        private System.Windows.Forms.NumericUpDown nudViewAlpha;
        private System.Windows.Forms.NumericUpDown nudViewMax;
        private System.Windows.Forms.NumericUpDown nudViewMin;
        private System.Windows.Forms.Label lblViewArea;
        private System.Windows.Forms.NumericUpDown nudViewAreaX;
        private System.Windows.Forms.NumericUpDown nudViewAreaY;
        private System.Windows.Forms.NumericUpDown nudViewAreaWidth;
        private System.Windows.Forms.NumericUpDown nudViewAreaHeight;
        private System.Windows.Forms.ToolStripMenuItem tsmiVerticalMirror;
        private System.Windows.Forms.ColumnHeader chArea;
        private System.Windows.Forms.ToolStripButton tsbPalette;
        private System.Windows.Forms.ToolStripLabel tslFileFormat;
        private System.Windows.Forms.ToolStripComboBox tscbFileFormat;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnNextFrame;
        private System.Windows.Forms.Button btnPreviousFrame;
        private System.Windows.Forms.Timer tPlay;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveFrameAs;
        private System.Windows.Forms.Label lblRealRefTempUnit;
        private System.Windows.Forms.Label lblRealRefTemp;
        private System.Windows.Forms.NumericUpDown nudRealRefTemp;
        private System.Windows.Forms.ContextMenuStrip cmsThermalImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportFrameAD;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportFrameTemperature;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportTemperatureCurve;
        private System.Windows.Forms.ToolStripMenuItem tsmiClose;
        private System.Windows.Forms.NumericUpDown nudFromFrame;
        private System.Windows.Forms.NumericUpDown nudToFrame;
        private System.Windows.Forms.Button btnClip;
        private System.Windows.Forms.Label lblFromFrame;
        private System.Windows.Forms.Label lblToFrame;
        private System.Windows.Forms.ToolStripMenuItem tsmiImportFrameAD;
        private System.Windows.Forms.NumericUpDown nudGoto;
        private System.Windows.Forms.Button btnGoto;
        private System.Windows.Forms.Panel pAlarmTemp;
        private System.Windows.Forms.Panel pHumidity;
        private System.Windows.Forms.Panel pInsulation;
        private System.Windows.Forms.Label lblRHLimit;
        private System.Windows.Forms.NumericUpDown nudRHLimit;
        private System.Windows.Forms.Label lblIndoorTemp;
        private System.Windows.Forms.NumericUpDown nudOutdoorTemp;
        private System.Windows.Forms.NumericUpDown nudIndoorTemp;
        private System.Windows.Forms.Label lblOutdoorTemp;
        private System.Windows.Forms.Label lblIndoorTempUnit;
        private System.Windows.Forms.Label lblOutdoorTempUnit;
        private System.Windows.Forms.NumericUpDown nudHeatIndex;
        private System.Windows.Forms.Label lblHeatIndex;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.NumericUpDown nudThreshold;
        private System.Windows.Forms.ColumnHeader chExInfo;
    }
}

