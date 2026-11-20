using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IRtekNetSDKApiDemo
{
    partial class AlarmQueryPanel
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
            this.components = new System.ComponentModel.Container();
            this._type = new ComboBox();
            this._lblType = new Label();
            this._channel = new NumericUpDown();
            this._lblChannel = new Label();
            this._start = new DateTimePicker();
            this._lblStart = new Label();
            this._stop = new DateTimePicker();
            this._lblStop = new Label();
            this._pageSize = new NumericUpDown();
            this._lblPageSize = new Label();
            this._query = new Button();
            this._prev = new Button();
            this._next = new Button();
            this._pageStatus = new Label();
            this._filterBar = new FlowLayoutPanel();
            this._filterTitle = new Label();
            this._filterWrap = new Panel();
            this._sftpHost = new TextBox();
            this._lblSftpHost = new Label();
            this._sftpPort = new NumericUpDown();
            this._lblSftpPort = new Label();
            this._sftpUser = new TextBox();
            this._lblSftpUser = new Label();
            this._sftpPassword = new TextBox();
            this._lblSftpPassword = new Label();
            this._localDir = new TextBox();
            this._lblLocalDir = new Label();
            this._browse = new Button();
            this._download = new Button();
            this._openDir = new Button();
            this._sftpBar = new FlowLayoutPanel();
            this._sftpTitle = new Label();
            this._sftpWrap = new Panel();
            this._list = new ListView();
            this._pickHint = new Label();
            this._lblWatchCount = new Label();
            this._watchCount = new NumericUpDown();
            this._watchPick = new Button();
            this._closeWatch = new Button();
            this._pickBar = new FlowLayoutPanel();
            this._watchWrap = new Panel();
            this._watchBoard = new AlarmWatchBoard();
            this._preview = new PictureBox();
            this._previewTitle = new Label();
            this._previewWrap = new Panel();
            this._chart = new SafeChart();
            this._chartStatus = new Label();
            this._chartBody = new Panel();
            this._chartHost = new Panel();
            this._curveTitle = new Label();
            this._curveWrap = new Panel();
            this._videoHost = new Panel();
            this._videoStatus = new Label();
            this._videoInner = new Panel();
            this._videoTitle = new Label();
            this._videoWrap = new Panel();
            this._side = new SplitContainer();
            this._media = new SplitContainer();
            this._body = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this._channel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pageSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._sftpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._watchCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._side)).BeginInit();
            this._side.Panel1.SuspendLayout();
            this._side.Panel2.SuspendLayout();
            this._side.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._media)).BeginInit();
            this._media.Panel1.SuspendLayout();
            this._media.Panel2.SuspendLayout();
            this._media.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._body)).BeginInit();
            this._body.Panel1.SuspendLayout();
            this._body.Panel2.SuspendLayout();
            this._body.SuspendLayout();
            this._watchWrap.SuspendLayout();
            this._pickBar.SuspendLayout();
            this._filterBar.SuspendLayout();
            this._filterWrap.SuspendLayout();
            this._sftpBar.SuspendLayout();
            this._sftpWrap.SuspendLayout();
            this._previewWrap.SuspendLayout();
            this._chartHost.SuspendLayout();
            this._curveWrap.SuspendLayout();
            this._videoInner.SuspendLayout();
            this._videoWrap.SuspendLayout();
            this.SuspendLayout();

            this._type.DropDownStyle = ComboBoxStyle.DropDownList;
            this._type.Name = "_type";
            this._type.Width = 130;
            this._type.Items.Add(new EnumChoice(0, "全部"));
            this._type.Items.Add(new EnumChoice(1, "热像温度"));
            this._type.Items.Add(new EnumChoice(2, "区域入侵"));
            this._type.Items.Add(new EnumChoice(3, "高温"));
            this._type.Items.Add(new EnumChoice(4, "温度监测"));
            this._type.Items.Add(new EnumChoice(5, "声源"));
            this._type.Items.Add(new EnumChoice(6, "分贝"));
            this._type.Items.Add(new EnumChoice(7, "频率"));
            this._type.SelectedIndex = 0;

            this._lblType.AutoSize = true;
            this._lblType.Name = "_lblType";
            this._lblType.Padding = new Padding(8, 8, 4, 0);
            this._lblType.Text = "类型";

            this._channel.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this._channel.Name = "_channel";
            this._channel.Value = new decimal(new int[] { 101, 0, 0, 0 });
            this._channel.Width = 70;

            this._lblChannel.AutoSize = true;
            this._lblChannel.Name = "_lblChannel";
            this._lblChannel.Padding = new Padding(8, 8, 4, 0);
            this._lblChannel.Text = "通道";

            this._start.Checked = true;
            this._start.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this._start.Format = DateTimePickerFormat.Custom;
            this._start.Name = "_start";
            this._start.ShowCheckBox = true;
            this._start.Width = 200;

            this._lblStart.AutoSize = true;
            this._lblStart.Name = "_lblStart";
            this._lblStart.Padding = new Padding(8, 8, 4, 0);
            this._lblStart.Text = "开始";

            this._stop.Checked = true;
            this._stop.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this._stop.Format = DateTimePickerFormat.Custom;
            this._stop.Name = "_stop";
            this._stop.ShowCheckBox = true;
            this._stop.Width = 200;

            this._lblStop.AutoSize = true;
            this._lblStop.Name = "_lblStop";
            this._lblStop.Padding = new Padding(8, 8, 4, 0);
            this._lblStop.Text = "结束";

            this._pageSize.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this._pageSize.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this._pageSize.Name = "_pageSize";
            this._pageSize.Value = new decimal(new int[] { 20, 0, 0, 0 });
            this._pageSize.Width = 60;

            this._lblPageSize.AutoSize = true;
            this._lblPageSize.Name = "_lblPageSize";
            this._lblPageSize.Padding = new Padding(8, 8, 4, 0);
            this._lblPageSize.Text = "每页";

            this._query.AutoSize = true;
            this._query.Name = "_query";
            this._query.Text = "查询";

            this._prev.AutoSize = true;
            this._prev.Enabled = false;
            this._prev.Name = "_prev";
            this._prev.Text = "上一页";

            this._next.AutoSize = true;
            this._next.Enabled = false;
            this._next.Name = "_next";
            this._next.Text = "下一页";

            this._pageStatus.AutoSize = true;
            this._pageStatus.Name = "_pageStatus";
            this._pageStatus.Padding = new Padding(8, 8, 0, 0);
            this._pageStatus.Text = "尚未查询";

            this._filterBar.AutoSize = false;
            this._filterBar.Height = 36;
            this._filterBar.Controls.Add(this._lblType);
            this._filterBar.Controls.Add(this._type);
            this._filterBar.Controls.Add(this._lblChannel);
            this._filterBar.Controls.Add(this._channel);
            this._filterBar.Controls.Add(this._lblStart);
            this._filterBar.Controls.Add(this._start);
            this._filterBar.Controls.Add(this._lblStop);
            this._filterBar.Controls.Add(this._stop);
            this._filterBar.Controls.Add(this._lblPageSize);
            this._filterBar.Controls.Add(this._pageSize);
            this._filterBar.Controls.Add(this._query);
            this._filterBar.Controls.Add(this._prev);
            this._filterBar.Controls.Add(this._next);
            this._filterBar.Controls.Add(this._pageStatus);
            this._filterBar.Dock = DockStyle.Top;
            this._filterBar.Name = "_filterBar";
            this._filterBar.Padding = new Padding(0, 4, 0, 0);
            this._filterBar.WrapContents = true;

            this._filterTitle.Dock = DockStyle.Top;
            this._filterTitle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            this._filterTitle.Height = 20;
            this._filterTitle.Name = "_filterTitle";
            this._filterTitle.Text = "查询条件";

            this._filterWrap.AutoSize = false;
            this._filterWrap.Height = 64;
            this._filterWrap.Controls.Add(this._filterBar);
            this._filterWrap.Controls.Add(this._filterTitle);
            this._filterWrap.Dock = DockStyle.Top;
            this._filterWrap.Name = "_filterWrap";
            this._filterWrap.Padding = new Padding(8, 6, 8, 4);

            this._sftpHost.Name = "_sftpHost";
            this._sftpHost.Width = 130;

            this._lblSftpHost.AutoSize = true;
            this._lblSftpHost.Name = "_lblSftpHost";
            this._lblSftpHost.Padding = new Padding(8, 8, 4, 0);
            this._lblSftpHost.Text = "主机";

            this._sftpPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this._sftpPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this._sftpPort.Name = "_sftpPort";
            this._sftpPort.Value = new decimal(new int[] { 22, 0, 0, 0 });
            this._sftpPort.Width = 60;

            this._lblSftpPort.AutoSize = true;
            this._lblSftpPort.Name = "_lblSftpPort";
            this._lblSftpPort.Padding = new Padding(8, 8, 4, 0);
            this._lblSftpPort.Text = "端口";

            this._sftpUser.Name = "_sftpUser";
            this._sftpUser.Text = "root";
            this._sftpUser.Width = 80;

            this._lblSftpUser.AutoSize = true;
            this._lblSftpUser.Name = "_lblSftpUser";
            this._lblSftpUser.Padding = new Padding(8, 8, 4, 0);
            this._lblSftpUser.Text = "用户";

            this._sftpPassword.Name = "_sftpPassword";
            this._sftpPassword.Text = "root";
            this._sftpPassword.UseSystemPasswordChar = true;
            this._sftpPassword.Width = 90;

            this._lblSftpPassword.AutoSize = true;
            this._lblSftpPassword.Name = "_lblSftpPassword";
            this._lblSftpPassword.Padding = new Padding(8, 8, 4, 0);
            this._lblSftpPassword.Text = "密码";

            this._localDir.Name = "_localDir";
            this._localDir.Width = 220;

            this._lblLocalDir.AutoSize = true;
            this._lblLocalDir.Name = "_lblLocalDir";
            this._lblLocalDir.Padding = new Padding(8, 8, 4, 0);
            this._lblLocalDir.Text = "本地目录";

            this._browse.AutoSize = true;
            this._browse.Name = "_browse";
            this._browse.Text = "浏览";

            this._download.AutoSize = true;
            this._download.Name = "_download";
            this._download.Text = "下载选中";

            this._openDir.AutoSize = true;
            this._openDir.Name = "_openDir";
            this._openDir.Text = "打开目录";

            this._sftpBar.AutoSize = false;
            this._sftpBar.Height = 36;
            this._sftpBar.Controls.Add(this._lblSftpHost);
            this._sftpBar.Controls.Add(this._sftpHost);
            this._sftpBar.Controls.Add(this._lblSftpPort);
            this._sftpBar.Controls.Add(this._sftpPort);
            this._sftpBar.Controls.Add(this._lblSftpUser);
            this._sftpBar.Controls.Add(this._sftpUser);
            this._sftpBar.Controls.Add(this._lblSftpPassword);
            this._sftpBar.Controls.Add(this._sftpPassword);
            this._sftpBar.Controls.Add(this._lblLocalDir);
            this._sftpBar.Controls.Add(this._localDir);
            this._sftpBar.Controls.Add(this._browse);
            this._sftpBar.Controls.Add(this._download);
            this._sftpBar.Controls.Add(this._openDir);
            this._sftpBar.Dock = DockStyle.Top;
            this._sftpBar.Name = "_sftpBar";
            this._sftpBar.Padding = new Padding(0, 4, 0, 0);
            this._sftpBar.WrapContents = true;

            this._sftpTitle.Dock = DockStyle.Top;
            this._sftpTitle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            this._sftpTitle.Height = 20;
            this._sftpTitle.Name = "_sftpTitle";
            this._sftpTitle.Text = "SFTP 下载";

            this._sftpWrap.AutoSize = false;
            this._sftpWrap.Height = 64;
            this._sftpWrap.Controls.Add(this._sftpBar);
            this._sftpWrap.Controls.Add(this._sftpTitle);
            this._sftpWrap.Dock = DockStyle.Top;
            this._sftpWrap.Name = "_sftpWrap";
            this._sftpWrap.Padding = new Padding(8, 6, 8, 4);

            this._list.Dock = DockStyle.Fill;
            this._list.CheckBoxes = true;
            this._list.FullRowSelect = true;
            this._list.GridLines = true;
            this._list.HideSelection = false;
            this._list.MultiSelect = true;
            this._list.Name = "_list";
            this._list.View = View.Details;
            this._list.Columns.Add("时间", 150);
            this._list.Columns.Add("类型", 80);
            this._list.Columns.Add("ID", 60);
            this._list.Columns.Add("通道", 50);
            this._list.Columns.Add("内容", 220);
            this._list.Columns.Add("录像", 150);
            this._list.Columns.Add("截图", 150);
            this._list.Columns.Add("数据", 150);

            this._pickHint.Dock = DockStyle.Bottom;
            this._pickHint.Height = 36;
            this._pickHint.Name = "_pickHint";
            this._pickHint.Text = "勾选要看的报警，或点中一条再设一次播放的条数。鼠标停在报警上可看完整内容。";

            this._lblWatchCount.AutoSize = true;
            this._lblWatchCount.Name = "_lblWatchCount";
            this._lblWatchCount.Padding = new Padding(0, 8, 4, 0);
            this._lblWatchCount.Text = "一次播放";

            this._watchCount.Minimum = 1;
            this._watchCount.Maximum = 20;
            this._watchCount.Value = 2;
            this._watchCount.Name = "_watchCount";
            this._watchCount.Width = 52;

            this._watchPick.AutoSize = true;
            this._watchPick.Name = "_watchPick";
            this._watchPick.Text = "一同观看";

            this._closeWatch.AutoSize = true;
            this._closeWatch.Name = "_closeWatch";
            this._closeWatch.Text = "关闭观看";

            this._pickBar.AutoSize = false;
            this._pickBar.Height = 36;
            this._pickBar.WrapContents = false;
            this._pickBar.Controls.Add(this._lblWatchCount);
            this._pickBar.Controls.Add(this._watchCount);
            this._pickBar.Controls.Add(this._watchPick);
            this._pickBar.Controls.Add(this._closeWatch);
            this._pickBar.Dock = DockStyle.Top;
            this._pickBar.Name = "_pickBar";
            this._pickBar.Padding = new Padding(0, 4, 0, 0);

            this._watchWrap.Controls.Add(this._pickBar);
            this._watchWrap.Controls.Add(this._pickHint);
            this._watchWrap.Dock = DockStyle.Bottom;
            this._watchWrap.Height = 72;
            this._watchWrap.Name = "_watchWrap";

            this._watchBoard.Dock = DockStyle.Fill;
            this._watchBoard.Name = "_watchBoard";
            this._watchBoard.Visible = false;

            this._preview.BackColor = Color.Black;
            this._preview.Dock = DockStyle.Fill;
            this._preview.Name = "_preview";
            this._preview.SizeMode = PictureBoxSizeMode.Zoom;

            this._previewTitle.Dock = DockStyle.Top;
            this._previewTitle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            this._previewTitle.Height = 20;
            this._previewTitle.Name = "_previewTitle";
            this._previewTitle.Text = "截图";

            this._previewWrap.Controls.Add(this._preview);
            this._previewWrap.Controls.Add(this._previewTitle);
            this._previewWrap.Dock = DockStyle.Fill;
            this._previewWrap.Name = "_previewWrap";
            this._previewWrap.Padding = new Padding(4);

            this._chart.AntiAliasing = AntiAliasingStyles.Graphics;
            this._chart.Name = "_chart";

            this._chartStatus.Dock = DockStyle.Top;
            this._chartStatus.Height = 44;
            this._chartStatus.Name = "_chartStatus";
            this._chartStatus.Text = "下载 data.json 后在此显示曲线。入侵画移动侦测，高温/温度监测画最高温。";

            this._chartBody.Controls.Add(this._chart);
            this._chartBody.Dock = DockStyle.Fill;
            this._chartBody.Name = "_chartBody";

            this._chartHost.Controls.Add(this._chartBody);
            this._chartHost.Controls.Add(this._chartStatus);
            this._chartHost.Dock = DockStyle.Fill;
            this._chartHost.Name = "_chartHost";

            this._curveTitle.Dock = DockStyle.Top;
            this._curveTitle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            this._curveTitle.Height = 20;
            this._curveTitle.Name = "_curveTitle";
            this._curveTitle.Text = "曲线";

            this._curveWrap.Controls.Add(this._chartHost);
            this._curveWrap.Controls.Add(this._curveTitle);
            this._curveWrap.Dock = DockStyle.Fill;
            this._curveWrap.Name = "_curveWrap";
            this._curveWrap.Padding = new Padding(4);

            this._videoHost.BackColor = Color.Black;
            this._videoHost.Dock = DockStyle.Fill;
            this._videoHost.Name = "_videoHost";

            this._videoStatus.Dock = DockStyle.Top;
            this._videoStatus.Height = 22;
            this._videoStatus.Name = "_videoStatus";
            this._videoStatus.Text = "下载录像后在此播放";

            this._videoInner.Controls.Add(this._videoHost);
            this._videoInner.Controls.Add(this._videoStatus);
            this._videoInner.Dock = DockStyle.Fill;
            this._videoInner.Name = "_videoInner";

            this._videoTitle.Dock = DockStyle.Top;
            this._videoTitle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            this._videoTitle.Height = 20;
            this._videoTitle.Name = "_videoTitle";
            this._videoTitle.Text = "视频";

            this._videoWrap.Controls.Add(this._videoInner);
            this._videoWrap.Controls.Add(this._videoTitle);
            this._videoWrap.Dock = DockStyle.Fill;
            this._videoWrap.Name = "_videoWrap";
            this._videoWrap.Padding = new Padding(4);

            this._side.Dock = DockStyle.Fill;
            this._side.Name = "_side";
            this._side.Orientation = Orientation.Horizontal;
            this._side.Panel1.Controls.Add(this._previewWrap);
            this._side.Panel2.Controls.Add(this._curveWrap);
            this._side.SplitterWidth = 6;

            this._media.Dock = DockStyle.Fill;
            this._media.Name = "_media";
            this._media.Panel1.Controls.Add(this._videoWrap);
            this._media.Panel2.Controls.Add(this._side);
            this._media.SplitterWidth = 6;

            this._body.Dock = DockStyle.Fill;
            this._body.Name = "_body";
            this._body.Panel1.Controls.Add(this._list);
            this._body.Panel1.Controls.Add(this._watchWrap);
            this._body.Panel2.Controls.Add(this._watchBoard);
            this._body.Panel2.Controls.Add(this._media);
            this._body.SplitterWidth = 6;

            this.Controls.Add(this._body);
            this.Controls.Add(this._sftpWrap);
            this.Controls.Add(this._filterWrap);
            this.Dock = DockStyle.Fill;
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.Name = "AlarmQueryPanel";
            this.Size = new Size(960, 640);

            ((System.ComponentModel.ISupportInitialize)(this._channel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pageSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._sftpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._watchCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._chart)).EndInit();
            this._side.Panel1.ResumeLayout(false);
            this._side.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._side)).EndInit();
            this._side.ResumeLayout(false);
            this._media.Panel1.ResumeLayout(false);
            this._media.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._media)).EndInit();
            this._media.ResumeLayout(false);
            this._body.Panel1.ResumeLayout(false);
            this._body.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._body)).EndInit();
            this._body.ResumeLayout(false);
            this._pickBar.ResumeLayout(false);
            this._pickBar.PerformLayout();
            this._watchWrap.ResumeLayout(false);
            this._filterBar.ResumeLayout(false);
            this._filterBar.PerformLayout();
            this._filterWrap.ResumeLayout(false);
            this._filterWrap.PerformLayout();
            this._sftpBar.ResumeLayout(false);
            this._sftpBar.PerformLayout();
            this._sftpWrap.ResumeLayout(false);
            this._sftpWrap.PerformLayout();
            this._previewWrap.ResumeLayout(false);
            this._chartHost.ResumeLayout(false);
            this._curveWrap.ResumeLayout(false);
            this._videoInner.ResumeLayout(false);
            this._videoWrap.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private ComboBox _type;
        private Label _lblType;
        private NumericUpDown _channel;
        private Label _lblChannel;
        private DateTimePicker _start;
        private Label _lblStart;
        private DateTimePicker _stop;
        private Label _lblStop;
        private NumericUpDown _pageSize;
        private Label _lblPageSize;
        private Button _query;
        private Button _prev;
        private Button _next;
        private Label _pageStatus;
        private FlowLayoutPanel _filterBar;
        private Label _filterTitle;
        private Panel _filterWrap;
        private TextBox _sftpHost;
        private Label _lblSftpHost;
        private NumericUpDown _sftpPort;
        private Label _lblSftpPort;
        private TextBox _sftpUser;
        private Label _lblSftpUser;
        private TextBox _sftpPassword;
        private Label _lblSftpPassword;
        private TextBox _localDir;
        private Label _lblLocalDir;
        private Button _browse;
        private Button _download;
        private Button _openDir;
        private FlowLayoutPanel _sftpBar;
        private Label _sftpTitle;
        private Panel _sftpWrap;
        private ListView _list;
        private Label _pickHint;
        private Label _lblWatchCount;
        private NumericUpDown _watchCount;
        private Button _watchPick;
        private Button _closeWatch;
        private FlowLayoutPanel _pickBar;
        private Panel _watchWrap;
        private AlarmWatchBoard _watchBoard;
        private PictureBox _preview;
        private Label _previewTitle;
        private Panel _previewWrap;
        private SafeChart _chart;
        private Label _chartStatus;
        private Panel _chartBody;
        private Panel _chartHost;
        private Label _curveTitle;
        private Panel _curveWrap;
        private Panel _videoHost;
        private Label _videoStatus;
        private Panel _videoInner;
        private Label _videoTitle;
        private Panel _videoWrap;
        private SplitContainer _side;
        private SplitContainer _media;
        private SplitContainer _body;
    }
}
