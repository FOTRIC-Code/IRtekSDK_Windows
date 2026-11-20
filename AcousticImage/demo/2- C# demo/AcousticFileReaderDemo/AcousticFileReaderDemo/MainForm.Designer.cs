
namespace AcousticFileReaderDemo
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
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Sensor Info", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Acoustic Params", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Capture Mode", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Frequency. Range", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Size", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Geo Location", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("File Info", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup9 = new System.Windows.Forms.ListViewGroup("PRPD Info", System.Windows.Forms.HorizontalAlignment.Left);
            this.msMainForm = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tsbEllipse = new System.Windows.Forms.ToolStripButton();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tslPalette = new System.Windows.Forms.ToolStripLabel();
            this.tsbPalette = new System.Windows.Forms.ToolStripButton();
            this.tscbPalette = new System.Windows.Forms.ToolStripComboBox();
            this.tslAlpha = new System.Windows.Forms.ToolStripLabel();
            this.tstbAlpha = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tslColorMode = new System.Windows.Forms.ToolStripLabel();
            this.tscbDbScaleType = new System.Windows.Forms.ToolStripComboBox();
            this.pbAcousticImage = new System.Windows.Forms.PictureBox();
            this.tcAcousticFile = new System.Windows.Forms.TabControl();
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
            this.tpPrpd = new System.Windows.Forms.TabPage();
            this.lvPRPDProperties = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pbPRPDImage = new System.Windows.Forms.PictureBox();
            this.lblDistanceUnit = new System.Windows.Forms.Label();
            this.nudDistance = new System.Windows.Forms.NumericUpDown();
            this.lblDistance = new System.Windows.Forms.Label();
            this.pbCoverImage = new System.Windows.Forms.PictureBox();
            this.pbVisualImage = new System.Windows.Forms.PictureBox();
            this.cmsMarkers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLocalParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeltaParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.ttTempValue = new System.Windows.Forms.ToolTip(this.components);
            this.btnAutoAdjust = new System.Windows.Forms.Button();
            this.pbPalleteImage = new System.Windows.Forms.PictureBox();
            this.nudMax = new System.Windows.Forms.NumericUpDown();
            this.nudMin = new System.Windows.Forms.NumericUpDown();
            this.lblMaxUnit = new System.Windows.Forms.Label();
            this.lblMinUnit = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnNextFrame = new System.Windows.Forms.Button();
            this.btnPreviousFrame = new System.Windows.Forms.Button();
            this.tPlay = new System.Windows.Forms.Timer(this.components);
            this.pbFFTImage = new System.Windows.Forms.PictureBox();
            this.lblGreyScale = new System.Windows.Forms.Label();
            this.lblFocusMode = new System.Windows.Forms.Label();
            this.nudScope = new System.Windows.Forms.NumericUpDown();
            this.lblScope = new System.Windows.Forms.Label();
            this.cbNormalizeDistance = new System.Windows.Forms.CheckBox();
            this.lblNormalizeDistance = new System.Windows.Forms.Label();
            this.cbGreyScale = new System.Windows.Forms.CheckBox();
            this.cbFocusModeEnabled = new System.Windows.Forms.CheckBox();
            this.pbSoundImage = new System.Windows.Forms.PictureBox();
            this.btnPlayDownSample = new System.Windows.Forms.Button();
            this.nudSoundDataStart = new System.Windows.Forms.NumericUpDown();
            this.nudSoundDataCount = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbSoundType = new System.Windows.Forms.ComboBox();
            this.nudSpan = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbFixSpan = new System.Windows.Forms.CheckBox();
            this.mlvMarkers = new AcousticFileReaderDemo.MarkersListView();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chX1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chY1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chX2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chY2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chArea = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nudLimit = new System.Windows.Forms.NumericUpDown();
            this.lbLimit = new System.Windows.Forms.Label();
            this.nudSourceSize = new System.Windows.Forms.NumericUpDown();
            this.nudSensitivity = new System.Windows.Forms.NumericUpDown();
            this.lbSoundstageSize = new System.Windows.Forms.Label();
            this.lbSensitivity = new System.Windows.Forms.Label();
            this.msMainForm.SuspendLayout();
            this.tsMainForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAcousticImage)).BeginInit();
            this.tcAcousticFile.SuspendLayout();
            this.tpProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.tpAnnotation.SuspendLayout();
            this.gbNoteAnnotation.SuspendLayout();
            this.gbTagAnnotation.SuspendLayout();
            this.gbAudioAnnotation.SuspendLayout();
            this.tpUserData.SuspendLayout();
            this.tpPrpd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPRPDImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoverImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualImage)).BeginInit();
            this.cmsMarkers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPalleteImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFFTImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScope)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSoundImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundDataStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundDataCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSourceSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensitivity)).BeginInit();
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
            this.msMainForm.Size = new System.Drawing.Size(1529, 25);
            this.msMainForm.TabIndex = 1;
            this.msMainForm.Text = "mainMenuStrip";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpen,
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
            this.tsbEllipse,
            this.tsbDelete,
            this.toolStripSeparator2,
            this.tslPalette,
            this.tsbPalette,
            this.tscbPalette,
            this.tslAlpha,
            this.tstbAlpha,
            this.toolStripSeparator5,
            this.tslColorMode,
            this.tscbDbScaleType});
            this.tsMainForm.Location = new System.Drawing.Point(0, 25);
            this.tsMainForm.Name = "tsMainForm";
            this.tsMainForm.Size = new System.Drawing.Size(1529, 31);
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
            this.tscbFileFormat.Size = new System.Drawing.Size(121, 31);
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
            // tslAlpha
            // 
            this.tslAlpha.Name = "tslAlpha";
            this.tslAlpha.Size = new System.Drawing.Size(41, 28);
            this.tslAlpha.Text = "Alpha";
            // 
            // tstbAlpha
            // 
            this.tstbAlpha.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.tstbAlpha.Name = "tstbAlpha";
            this.tstbAlpha.Size = new System.Drawing.Size(60, 31);
            this.tstbAlpha.TextChanged += new System.EventHandler(this.tstbAlpha_TextChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
            // 
            // tslColorMode
            // 
            this.tslColorMode.Name = "tslColorMode";
            this.tslColorMode.Size = new System.Drawing.Size(91, 28);
            this.tslColorMode.Text = "Db Scale Type";
            // 
            // tscbDbScaleType
            // 
            this.tscbDbScaleType.AutoSize = false;
            this.tscbDbScaleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbDbScaleType.DropDownWidth = 140;
            this.tscbDbScaleType.Name = "tscbDbScaleType";
            this.tscbDbScaleType.Size = new System.Drawing.Size(121, 25);
            this.tscbDbScaleType.SelectedIndexChanged += new System.EventHandler(this.tscbDbScaleType_SelectedIndexChanged);
            // 
            // pbAcousticImage
            // 
            this.pbAcousticImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbAcousticImage.BackColor = System.Drawing.SystemColors.Window;
            this.pbAcousticImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbAcousticImage.Location = new System.Drawing.Point(12, 59);
            this.pbAcousticImage.Name = "pbAcousticImage";
            this.pbAcousticImage.Size = new System.Drawing.Size(787, 488);
            this.pbAcousticImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbAcousticImage.TabIndex = 11;
            this.pbAcousticImage.TabStop = false;
            this.pbAcousticImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbAcousticImage_MouseDown);
            this.pbAcousticImage.MouseLeave += new System.EventHandler(this.pbAcousticImage_MouseLeave);
            this.pbAcousticImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbAcousticImage_MouseMove);
            this.pbAcousticImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbAcousticImage_MouseUp);
            // 
            // tcAcousticFile
            // 
            this.tcAcousticFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcAcousticFile.Controls.Add(this.tpProperties);
            this.tcAcousticFile.Controls.Add(this.tpAnnotation);
            this.tcAcousticFile.Controls.Add(this.tpUserData);
            this.tcAcousticFile.Controls.Add(this.tpPrpd);
            this.tcAcousticFile.Location = new System.Drawing.Point(1031, 245);
            this.tcAcousticFile.Name = "tcAcousticFile";
            this.tcAcousticFile.SelectedIndex = 0;
            this.tcAcousticFile.Size = new System.Drawing.Size(486, 721);
            this.tcAcousticFile.TabIndex = 13;
            // 
            // tpProperties
            // 
            this.tpProperties.Controls.Add(this.pbLogo);
            this.tpProperties.Controls.Add(this.lvProperties);
            this.tpProperties.Location = new System.Drawing.Point(4, 22);
            this.tpProperties.Name = "tpProperties";
            this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tpProperties.Size = new System.Drawing.Size(478, 695);
            this.tpProperties.TabIndex = 0;
            this.tpProperties.Text = "Properties";
            this.tpProperties.UseVisualStyleBackColor = true;
            // 
            // pbLogo
            // 
            this.pbLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbLogo.BackColor = System.Drawing.Color.Gray;
            this.pbLogo.Location = new System.Drawing.Point(6, 624);
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
            listViewGroup2.Header = "Sensor Info";
            listViewGroup2.Name = "lvgSensorInfo";
            listViewGroup3.Header = "Acoustic Params";
            listViewGroup3.Name = "lvgAcousticParams";
            listViewGroup4.Header = "Capture Mode";
            listViewGroup4.Name = "lvgCaptureMode";
            listViewGroup5.Header = "Frequency. Range";
            listViewGroup5.Name = "lvgFrequencyRange";
            listViewGroup6.Header = "Size";
            listViewGroup6.Name = "lvgSize";
            listViewGroup7.Header = "Geo Location";
            listViewGroup7.Name = "lvgGeoLocation";
            listViewGroup8.Header = "File Info";
            listViewGroup8.Name = "lvgFileInfo";
            this.lvProperties.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8});
            this.lvProperties.HideSelection = false;
            this.lvProperties.Location = new System.Drawing.Point(6, 6);
            this.lvProperties.Name = "lvProperties";
            this.lvProperties.Size = new System.Drawing.Size(466, 612);
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
            this.tpAnnotation.Size = new System.Drawing.Size(478, 695);
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
            this.tpUserData.Size = new System.Drawing.Size(478, 695);
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
            // tpPrpd
            // 
            this.tpPrpd.Controls.Add(this.lvPRPDProperties);
            this.tpPrpd.Controls.Add(this.pbPRPDImage);
            this.tpPrpd.Location = new System.Drawing.Point(4, 22);
            this.tpPrpd.Name = "tpPrpd";
            this.tpPrpd.Size = new System.Drawing.Size(478, 695);
            this.tpPrpd.TabIndex = 5;
            this.tpPrpd.Text = "PRPD";
            this.tpPrpd.UseVisualStyleBackColor = true;
            // 
            // lvPRPDProperties
            // 
            this.lvPRPDProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvPRPDProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvPRPDProperties.FullRowSelect = true;
            this.lvPRPDProperties.GridLines = true;
            listViewGroup9.Header = "PRPD Info";
            listViewGroup9.Name = "lvgPRPDInfo";
            this.lvPRPDProperties.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup9});
            this.lvPRPDProperties.HideSelection = false;
            this.lvPRPDProperties.Location = new System.Drawing.Point(3, 292);
            this.lvPRPDProperties.Name = "lvPRPDProperties";
            this.lvPRPDProperties.Size = new System.Drawing.Size(466, 310);
            this.lvPRPDProperties.TabIndex = 18;
            this.lvPRPDProperties.UseCompatibleStateImageBehavior = false;
            this.lvPRPDProperties.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Property";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 350;
            // 
            // pbPRPDImage
            // 
            this.pbPRPDImage.BackColor = System.Drawing.Color.Gray;
            this.pbPRPDImage.Location = new System.Drawing.Point(3, 5);
            this.pbPRPDImage.Name = "pbPRPDImage";
            this.pbPRPDImage.Size = new System.Drawing.Size(466, 279);
            this.pbPRPDImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbPRPDImage.TabIndex = 17;
            this.pbPRPDImage.TabStop = false;
            // 
            // lblDistanceUnit
            // 
            this.lblDistanceUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDistanceUnit.AutoSize = true;
            this.lblDistanceUnit.Location = new System.Drawing.Point(992, 198);
            this.lblDistanceUnit.Name = "lblDistanceUnit";
            this.lblDistanceUnit.Size = new System.Drawing.Size(11, 12);
            this.lblDistanceUnit.TabIndex = 11;
            this.lblDistanceUnit.Text = "m";
            // 
            // nudDistance
            // 
            this.nudDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudDistance.DecimalPlaces = 1;
            this.nudDistance.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudDistance.Location = new System.Drawing.Point(926, 196);
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
            this.nudDistance.Size = new System.Drawing.Size(60, 21);
            this.nudDistance.TabIndex = 7;
            this.nudDistance.ValueChanged += new System.EventHandler(this.nudDistance_ValueChanged);
            // 
            // lblDistance
            // 
            this.lblDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDistance.AutoSize = true;
            this.lblDistance.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.lblDistance.Location = new System.Drawing.Point(926, 181);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(61, 12);
            this.lblDistance.TabIndex = 4;
            this.lblDistance.Text = "Distance";
            // 
            // pbCoverImage
            // 
            this.pbCoverImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCoverImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbCoverImage.Location = new System.Drawing.Point(1031, 59);
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
            this.pbVisualImage.Location = new System.Drawing.Point(1277, 59);
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
            this.tsmiExportCSV});
            this.cmsMarkers.Name = "cmsMarkers";
            this.cmsMarkers.Size = new System.Drawing.Size(225, 92);
            this.cmsMarkers.Opening += new System.ComponentModel.CancelEventHandler(this.cmsMarkers_Opening);
            // 
            // tsmiRename
            // 
            this.tsmiRename.Name = "tsmiRename";
            this.tsmiRename.Size = new System.Drawing.Size(224, 22);
            this.tsmiRename.Text = "Rename";
            this.tsmiRename.Click += new System.EventHandler(this.tsmiRename_Click);
            // 
            // tsmiLocalParameters
            // 
            this.tsmiLocalParameters.Name = "tsmiLocalParameters";
            this.tsmiLocalParameters.Size = new System.Drawing.Size(224, 22);
            this.tsmiLocalParameters.Text = "Change Local Parameters";
            this.tsmiLocalParameters.Click += new System.EventHandler(this.tsmiLocalParameters_Click);
            // 
            // tsmiDeltaParameters
            // 
            this.tsmiDeltaParameters.Name = "tsmiDeltaParameters";
            this.tsmiDeltaParameters.Size = new System.Drawing.Size(224, 22);
            this.tsmiDeltaParameters.Text = "Change Delta Parameters";
            // 
            // tsmiExportCSV
            // 
            this.tsmiExportCSV.Name = "tsmiExportCSV";
            this.tsmiExportCSV.Size = new System.Drawing.Size(224, 22);
            this.tsmiExportCSV.Text = "Export CSV...";
            this.tsmiExportCSV.Click += new System.EventHandler(this.tsmiExportCSV_Click);
            // 
            // ttTempValue
            // 
            this.ttTempValue.ShowAlways = true;
            // 
            // btnAutoAdjust
            // 
            this.btnAutoAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoAdjust.Location = new System.Drawing.Point(893, 794);
            this.btnAutoAdjust.Name = "btnAutoAdjust";
            this.btnAutoAdjust.Size = new System.Drawing.Size(89, 28);
            this.btnAutoAdjust.TabIndex = 20;
            this.btnAutoAdjust.Text = "AutoAdjust";
            this.btnAutoAdjust.UseVisualStyleBackColor = true;
            this.btnAutoAdjust.Click += new System.EventHandler(this.btnAutoAdjust_Click);
            // 
            // pbPalleteImage
            // 
            this.pbPalleteImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbPalleteImage.BackColor = System.Drawing.Color.White;
            this.pbPalleteImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPalleteImage.Location = new System.Drawing.Point(893, 59);
            this.pbPalleteImage.Margin = new System.Windows.Forms.Padding(0);
            this.pbPalleteImage.Name = "pbPalleteImage";
            this.pbPalleteImage.Size = new System.Drawing.Size(30, 732);
            this.pbPalleteImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPalleteImage.TabIndex = 17;
            this.pbPalleteImage.TabStop = false;
            this.pbPalleteImage.MouseLeave += new System.EventHandler(this.pbPalletImage_MouseLeave);
            this.pbPalleteImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPalletImage_MouseMove);
            // 
            // nudMax
            // 
            this.nudMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudMax.DecimalPlaces = 1;
            this.nudMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudMax.Location = new System.Drawing.Point(926, 59);
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
            this.nudMin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudMin.Location = new System.Drawing.Point(926, 767);
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
            // lblMaxUnit
            // 
            this.lblMaxUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMaxUnit.AutoSize = true;
            this.lblMaxUnit.Location = new System.Drawing.Point(992, 63);
            this.lblMaxUnit.Name = "lblMaxUnit";
            this.lblMaxUnit.Size = new System.Drawing.Size(17, 12);
            this.lblMaxUnit.TabIndex = 38;
            this.lblMaxUnit.Text = "dB";
            // 
            // lblMinUnit
            // 
            this.lblMinUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMinUnit.AutoSize = true;
            this.lblMinUnit.Location = new System.Drawing.Point(992, 771);
            this.lblMinUnit.Name = "lblMinUnit";
            this.lblMinUnit.Size = new System.Drawing.Size(17, 12);
            this.lblMinUnit.TabIndex = 42;
            this.lblMinUnit.Text = "dB";
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
            this.btnPause.Location = new System.Drawing.Point(92, 794);
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
            this.btnStop.Location = new System.Drawing.Point(132, 794);
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
            this.btnPlay.Location = new System.Drawing.Point(52, 794);
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
            this.btnNextFrame.Location = new System.Drawing.Point(172, 794);
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
            this.btnPreviousFrame.Location = new System.Drawing.Point(12, 794);
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
            // pbFFTImage
            // 
            this.pbFFTImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFFTImage.BackColor = System.Drawing.Color.White;
            this.pbFFTImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbFFTImage.Location = new System.Drawing.Point(802, 59);
            this.pbFFTImage.Margin = new System.Windows.Forms.Padding(0);
            this.pbFFTImage.Name = "pbFFTImage";
            this.pbFFTImage.Size = new System.Drawing.Size(88, 731);
            this.pbFFTImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbFFTImage.TabIndex = 62;
            this.pbFFTImage.TabStop = false;
            // 
            // lblGreyScale
            // 
            this.lblGreyScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGreyScale.AutoSize = true;
            this.lblGreyScale.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.lblGreyScale.Location = new System.Drawing.Point(924, 227);
            this.lblGreyScale.Name = "lblGreyScale";
            this.lblGreyScale.Size = new System.Drawing.Size(68, 12);
            this.lblGreyScale.TabIndex = 63;
            this.lblGreyScale.Text = "GreyScale";
            // 
            // lblFocusMode
            // 
            this.lblFocusMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFocusMode.AutoSize = true;
            this.lblFocusMode.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.lblFocusMode.Location = new System.Drawing.Point(924, 267);
            this.lblFocusMode.Name = "lblFocusMode";
            this.lblFocusMode.Size = new System.Drawing.Size(75, 12);
            this.lblFocusMode.TabIndex = 66;
            this.lblFocusMode.Text = "Focus Mode";
            // 
            // nudScope
            // 
            this.nudScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudScope.DecimalPlaces = 1;
            this.nudScope.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudScope.Location = new System.Drawing.Point(927, 316);
            this.nudScope.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScope.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudScope.Name = "nudScope";
            this.nudScope.Size = new System.Drawing.Size(60, 21);
            this.nudScope.TabIndex = 68;
            this.nudScope.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudScope.ValueChanged += new System.EventHandler(this.nudScope_ValueChanged);
            // 
            // lblScope
            // 
            this.lblScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScope.AutoSize = true;
            this.lblScope.Location = new System.Drawing.Point(926, 301);
            this.lblScope.Name = "lblScope";
            this.lblScope.Size = new System.Drawing.Size(35, 12);
            this.lblScope.TabIndex = 69;
            this.lblScope.Text = "Scope";
            // 
            // cbNormalizeDistance
            // 
            this.cbNormalizeDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNormalizeDistance.AutoSize = true;
            this.cbNormalizeDistance.Location = new System.Drawing.Point(926, 366);
            this.cbNormalizeDistance.Name = "cbNormalizeDistance";
            this.cbNormalizeDistance.Size = new System.Drawing.Size(66, 16);
            this.cbNormalizeDistance.TabIndex = 71;
            this.cbNormalizeDistance.Text = "Enabled";
            this.cbNormalizeDistance.UseVisualStyleBackColor = true;
            this.cbNormalizeDistance.CheckedChanged += new System.EventHandler(this.cbNormalizeDistance_CheckedChanged);
            // 
            // lblNormalizeDistance
            // 
            this.lblNormalizeDistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNormalizeDistance.AutoSize = true;
            this.lblNormalizeDistance.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.lblNormalizeDistance.Location = new System.Drawing.Point(924, 351);
            this.lblNormalizeDistance.Name = "lblNormalizeDistance";
            this.lblNormalizeDistance.Size = new System.Drawing.Size(96, 12);
            this.lblNormalizeDistance.TabIndex = 72;
            this.lblNormalizeDistance.Text = "Normalize Dis";
            // 
            // cbGreyScale
            // 
            this.cbGreyScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGreyScale.AutoSize = true;
            this.cbGreyScale.Enabled = false;
            this.cbGreyScale.Location = new System.Drawing.Point(926, 245);
            this.cbGreyScale.Name = "cbGreyScale";
            this.cbGreyScale.Size = new System.Drawing.Size(66, 16);
            this.cbGreyScale.TabIndex = 75;
            this.cbGreyScale.Text = "Enabled";
            this.cbGreyScale.UseVisualStyleBackColor = true;
            // 
            // cbFocusModeEnabled
            // 
            this.cbFocusModeEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFocusModeEnabled.AutoSize = true;
            this.cbFocusModeEnabled.Location = new System.Drawing.Point(926, 282);
            this.cbFocusModeEnabled.Name = "cbFocusModeEnabled";
            this.cbFocusModeEnabled.Size = new System.Drawing.Size(66, 16);
            this.cbFocusModeEnabled.TabIndex = 76;
            this.cbFocusModeEnabled.Text = "Enabled";
            this.cbFocusModeEnabled.UseVisualStyleBackColor = true;
            this.cbFocusModeEnabled.CheckedChanged += new System.EventHandler(this.cbFocusModelEnabled_CheckedChanged);
            // 
            // pbSoundImage
            // 
            this.pbSoundImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSoundImage.BackColor = System.Drawing.Color.White;
            this.pbSoundImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSoundImage.Location = new System.Drawing.Point(12, 580);
            this.pbSoundImage.Margin = new System.Windows.Forms.Padding(0);
            this.pbSoundImage.Name = "pbSoundImage";
            this.pbSoundImage.Size = new System.Drawing.Size(787, 210);
            this.pbSoundImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSoundImage.TabIndex = 79;
            this.pbSoundImage.TabStop = false;
            this.pbSoundImage.SizeChanged += new System.EventHandler(this.pbSoundImage_SizeChanged);
            // 
            // btnPlayDownSample
            // 
            this.btnPlayDownSample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlayDownSample.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayDownSample.Image")));
            this.btnPlayDownSample.Location = new System.Drawing.Point(139, 553);
            this.btnPlayDownSample.Name = "btnPlayDownSample";
            this.btnPlayDownSample.Size = new System.Drawing.Size(34, 24);
            this.btnPlayDownSample.TabIndex = 83;
            this.btnPlayDownSample.UseVisualStyleBackColor = true;
            this.btnPlayDownSample.Click += new System.EventHandler(this.btnPlaySound_Click);
            // 
            // nudSoundDataStart
            // 
            this.nudSoundDataStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudSoundDataStart.Increment = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.nudSoundDataStart.Location = new System.Drawing.Point(221, 555);
            this.nudSoundDataStart.Maximum = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            this.nudSoundDataStart.Name = "nudSoundDataStart";
            this.nudSoundDataStart.Size = new System.Drawing.Size(60, 21);
            this.nudSoundDataStart.TabIndex = 87;
            this.nudSoundDataStart.ValueChanged += new System.EventHandler(this.nudData_ValueChanged);
            // 
            // nudSoundDataCount
            // 
            this.nudSoundDataCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudSoundDataCount.Increment = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.nudSoundDataCount.Location = new System.Drawing.Point(347, 555);
            this.nudSoundDataCount.Maximum = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            this.nudSoundDataCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSoundDataCount.Name = "nudSoundDataCount";
            this.nudSoundDataCount.Size = new System.Drawing.Size(60, 21);
            this.nudSoundDataCount.TabIndex = 88;
            this.nudSoundDataCount.Value = new decimal(new int[] {
            200000,
            0,
            0,
            0});
            this.nudSoundDataCount.ValueChanged += new System.EventHandler(this.nudData_ValueChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(179, 559);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 91;
            this.label5.Text = "Start";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(300, 559);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 92;
            this.label6.Text = "Length";
            // 
            // cbSoundType
            // 
            this.cbSoundType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbSoundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSoundType.Location = new System.Drawing.Point(12, 555);
            this.cbSoundType.Name = "cbSoundType";
            this.cbSoundType.Size = new System.Drawing.Size(121, 20);
            this.cbSoundType.TabIndex = 93;
            this.cbSoundType.SelectedIndexChanged += new System.EventHandler(this.cbSoundType_SelectedIndexChanged);
            // 
            // nudSpan
            // 
            this.nudSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSpan.DecimalPlaces = 1;
            this.nudSpan.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudSpan.Location = new System.Drawing.Point(926, 104);
            this.nudSpan.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudSpan.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            -2147483648});
            this.nudSpan.Name = "nudSpan";
            this.nudSpan.Size = new System.Drawing.Size(60, 21);
            this.nudSpan.TabIndex = 94;
            this.nudSpan.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudSpan.ValueChanged += new System.EventHandler(this.nudSpan_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(925, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 95;
            this.label1.Text = "Span";
            // 
            // cbFixSpan
            // 
            this.cbFixSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFixSpan.AutoSize = true;
            this.cbFixSpan.Enabled = false;
            this.cbFixSpan.Location = new System.Drawing.Point(926, 131);
            this.cbFixSpan.Name = "cbFixSpan";
            this.cbFixSpan.Size = new System.Drawing.Size(66, 16);
            this.cbFixSpan.TabIndex = 96;
            this.cbFixSpan.Text = "FixSpan";
            this.cbFixSpan.UseVisualStyleBackColor = true;
            this.cbFixSpan.CheckedChanged += new System.EventHandler(this.cbFixSpan_CheckedChanged);
            // 
            // mlvMarkers
            // 
            this.mlvMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mlvMarkers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chValue,
            this.chMax,
            this.chX1,
            this.chY1,
            this.chX2,
            this.chY2,
            this.chArea});
            this.mlvMarkers.ContextMenuStrip = this.cmsMarkers;
            this.mlvMarkers.FullRowSelect = true;
            this.mlvMarkers.HideSelection = false;
            this.mlvMarkers.Location = new System.Drawing.Point(12, 828);
            this.mlvMarkers.MultiSelect = false;
            this.mlvMarkers.Name = "mlvMarkers";
            this.mlvMarkers.Size = new System.Drawing.Size(1013, 138);
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
            // chMax
            // 
            this.chMax.Text = "Max";
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
            // nudLimit
            // 
            this.nudLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLimit.Enabled = false;
            this.nudLimit.Location = new System.Drawing.Point(928, 537);
            this.nudLimit.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLimit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudLimit.Name = "nudLimit";
            this.nudLimit.Size = new System.Drawing.Size(58, 21);
            this.nudLimit.TabIndex = 151;
            this.nudLimit.Value = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudLimit.ValueChanged += new System.EventHandler(this.nudLimit_ValueChanged);
            // 
            // lbLimit
            // 
            this.lbLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLimit.AutoSize = true;
            this.lbLimit.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.lbLimit.Location = new System.Drawing.Point(924, 522);
            this.lbLimit.Name = "lbLimit";
            this.lbLimit.Size = new System.Drawing.Size(54, 12);
            this.lbLimit.TabIndex = 150;
            this.lbLimit.Text = "Limit: ";
            // 
            // nudSourceSize
            // 
            this.nudSourceSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSourceSize.Enabled = false;
            this.nudSourceSize.Location = new System.Drawing.Point(928, 498);
            this.nudSourceSize.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSourceSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSourceSize.Name = "nudSourceSize";
            this.nudSourceSize.Size = new System.Drawing.Size(58, 21);
            this.nudSourceSize.TabIndex = 149;
            this.nudSourceSize.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudSourceSize.ValueChanged += new System.EventHandler(this.nudSourceSize_ValueChanged);
            // 
            // nudSensitivity
            // 
            this.nudSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSensitivity.DecimalPlaces = 2;
            this.nudSensitivity.Enabled = false;
            this.nudSensitivity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudSensitivity.Location = new System.Drawing.Point(928, 459);
            this.nudSensitivity.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSensitivity.Name = "nudSensitivity";
            this.nudSensitivity.Size = new System.Drawing.Size(58, 21);
            this.nudSensitivity.TabIndex = 148;
            this.nudSensitivity.Value = new decimal(new int[] {
            32,
            0,
            0,
            65536});
            this.nudSensitivity.ValueChanged += new System.EventHandler(this.nudSensitivity_ValueChanged);
            // 
            // lbSoundstageSize
            // 
            this.lbSoundstageSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSoundstageSize.AutoSize = true;
            this.lbSoundstageSize.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.lbSoundstageSize.Location = new System.Drawing.Point(924, 483);
            this.lbSoundstageSize.Name = "lbSoundstageSize";
            this.lbSoundstageSize.Size = new System.Drawing.Size(96, 12);
            this.lbSoundstageSize.TabIndex = 147;
            this.lbSoundstageSize.Text = "Source Size: ";
            // 
            // lbSensitivity
            // 
            this.lbSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSensitivity.AutoSize = true;
            this.lbSensitivity.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.lbSensitivity.Location = new System.Drawing.Point(925, 444);
            this.lbSensitivity.Name = "lbSensitivity";
            this.lbSensitivity.Size = new System.Drawing.Size(89, 12);
            this.lbSensitivity.TabIndex = 146;
            this.lbSensitivity.Text = "Sensitivity:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1529, 978);
            this.Controls.Add(this.nudLimit);
            this.Controls.Add(this.lbLimit);
            this.Controls.Add(this.nudSourceSize);
            this.Controls.Add(this.nudSensitivity);
            this.Controls.Add(this.lbSoundstageSize);
            this.Controls.Add(this.lbSensitivity);
            this.Controls.Add(this.cbFixSpan);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudSpan);
            this.Controls.Add(this.cbSoundType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudSoundDataCount);
            this.Controls.Add(this.nudSoundDataStart);
            this.Controls.Add(this.btnPlayDownSample);
            this.Controls.Add(this.pbSoundImage);
            this.Controls.Add(this.cbFocusModeEnabled);
            this.Controls.Add(this.cbGreyScale);
            this.Controls.Add(this.lblDistanceUnit);
            this.Controls.Add(this.nudDistance);
            this.Controls.Add(this.lblNormalizeDistance);
            this.Controls.Add(this.cbNormalizeDistance);
            this.Controls.Add(this.lblDistance);
            this.Controls.Add(this.lblScope);
            this.Controls.Add(this.nudScope);
            this.Controls.Add(this.lblFocusMode);
            this.Controls.Add(this.lblGreyScale);
            this.Controls.Add(this.pbFFTImage);
            this.Controls.Add(this.mlvMarkers);
            this.Controls.Add(this.btnPreviousFrame);
            this.Controls.Add(this.btnNextFrame);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.lblMinUnit);
            this.Controls.Add(this.lblMaxUnit);
            this.Controls.Add(this.nudMin);
            this.Controls.Add(this.nudMax);
            this.Controls.Add(this.btnAutoAdjust);
            this.Controls.Add(this.pbPalleteImage);
            this.Controls.Add(this.pbVisualImage);
            this.Controls.Add(this.pbCoverImage);
            this.Controls.Add(this.tcAcousticFile);
            this.Controls.Add(this.pbAcousticImage);
            this.Controls.Add(this.tsMainForm);
            this.Controls.Add(this.msMainForm);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo AcousticFileReader";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.msMainForm.ResumeLayout(false);
            this.msMainForm.PerformLayout();
            this.tsMainForm.ResumeLayout(false);
            this.tsMainForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAcousticImage)).EndInit();
            this.tcAcousticFile.ResumeLayout(false);
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
            this.tpPrpd.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPRPDImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCoverImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualImage)).EndInit();
            this.cmsMarkers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPalleteImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFFTImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScope)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSoundImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundDataStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundDataCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSourceSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSensitivity)).EndInit();
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
        private System.Windows.Forms.ToolStripButton tsbDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel tslPalette;
        private System.Windows.Forms.ToolStripComboBox tscbPalette;
        private System.Windows.Forms.ToolStripLabel tslColorMode;
        private System.Windows.Forms.ToolStripComboBox tscbDbScaleType;
        private System.Windows.Forms.PictureBox pbAcousticImage;
        private System.Windows.Forms.TabControl tcAcousticFile;
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
        private System.Windows.Forms.ColumnHeader chMax;
        private System.Windows.Forms.ColumnHeader chX1;
        private System.Windows.Forms.ColumnHeader chY1;
        private System.Windows.Forms.ColumnHeader chX2;
        private System.Windows.Forms.ColumnHeader chY2;
        private System.Windows.Forms.PictureBox pbCoverImage;
        private System.Windows.Forms.PictureBox pbVisualImage;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.ToolStripButton tsbEllipse;
        private System.Windows.Forms.ContextMenuStrip cmsMarkers;
        private System.Windows.Forms.ToolStripMenuItem tsmiRename;
        private System.Windows.Forms.ToolStripMenuItem tsmiLocalParameters;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeltaParameters;
        private System.Windows.Forms.ToolTip ttTempValue;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportCSV;
        private System.Windows.Forms.ToolStripMenuItem tsmiRotate;
        private System.Windows.Forms.ToolStripMenuItem tsmiHorizontalMirror;
        private System.Windows.Forms.Button btnAutoAdjust;
        private System.Windows.Forms.PictureBox pbPalleteImage;
        private System.Windows.Forms.NumericUpDown nudMax;
        private System.Windows.Forms.NumericUpDown nudMin;
        private System.Windows.Forms.Label lblMaxUnit;
        private System.Windows.Forms.Label lblMinUnit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
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
        private System.Windows.Forms.PictureBox pbFFTImage;
        private System.Windows.Forms.Label lblGreyScale;
        private System.Windows.Forms.ToolStripLabel tslAlpha;
        private System.Windows.Forms.ToolStripTextBox tstbAlpha;
        private System.Windows.Forms.Label lblFocusMode;
        private System.Windows.Forms.NumericUpDown nudScope;
        private System.Windows.Forms.Label lblScope;
        private System.Windows.Forms.CheckBox cbNormalizeDistance;
        private System.Windows.Forms.Label lblNormalizeDistance;
        private System.Windows.Forms.Label lblDistanceUnit;
        private System.Windows.Forms.NumericUpDown nudDistance;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.CheckBox cbGreyScale;
        private System.Windows.Forms.CheckBox cbFocusModeEnabled;
        private System.Windows.Forms.PictureBox pbSoundImage;
        private System.Windows.Forms.Button btnPlayDownSample;
        private System.Windows.Forms.TabPage tpPrpd;
        private System.Windows.Forms.PictureBox pbPRPDImage;
        private System.Windows.Forms.NumericUpDown nudSoundDataStart;
        private System.Windows.Forms.NumericUpDown nudSoundDataCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbSoundType;
        private System.Windows.Forms.ListView lvPRPDProperties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.NumericUpDown nudSpan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbFixSpan;
        private System.Windows.Forms.NumericUpDown nudLimit;
        private System.Windows.Forms.Label lbLimit;
        private System.Windows.Forms.NumericUpDown nudSourceSize;
        private System.Windows.Forms.NumericUpDown nudSensitivity;
        private System.Windows.Forms.Label lbSoundstageSize;
        private System.Windows.Forms.Label lbSensitivity;
    }
}

