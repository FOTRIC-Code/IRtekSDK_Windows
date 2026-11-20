using System.Drawing;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    partial class ApiTestForm
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
            this._tree = new TreeView();
            this._title = new Label();
            this._summary = new Label();
            this._paramBar = new FlowLayoutPanel();
            this._includeDanger = new CheckBox();
            this._invoke = new Button();
            this._runGroup = new Button();
            this._runAll = new Button();
            this._send = new TextBox();
            this._receive = new TextBox();
            this._endpoint = new TextBox();
            this._actionBar = new FlowLayoutPanel();
            this._panes = new TableLayoutPanel();
            this._sendHost = new Panel();
            this._lblSend = new Label();
            this._receiveHost = new Panel();
            this._lblReceive = new Label();
            this._endpointHost = new Panel();
            this._lblEndpoint = new Label();
            this._right = new Panel();
            this._left = new Panel();
            this._left.SuspendLayout();
            this._actionBar.SuspendLayout();
            this._panes.SuspendLayout();
            this._sendHost.SuspendLayout();
            this._receiveHost.SuspendLayout();
            this._endpointHost.SuspendLayout();
            this._right.SuspendLayout();
            this.SuspendLayout();

            this.Font = new Font("Microsoft YaHei UI", 9F);
            Font mono = new Font("Consolas", 9F);
            Font bold = new Font(this.Font, FontStyle.Bold);

            this._tree.Dock = DockStyle.Fill;
            this._tree.HideSelection = false;
            this._tree.ItemHeight = 22;
            this._tree.Name = "_tree";

            this._title.AutoSize = true;
            this._title.Dock = DockStyle.Top;
            this._title.Font = bold;
            this._title.Name = "_title";
            this._title.Padding = new Padding(0, 4, 0, 4);
            this._title.Text = "选择左侧接口后调用";

            this._summary.Dock = DockStyle.Top;
            this._summary.Height = 36;
            this._summary.Name = "_summary";
            this._summary.Text = "获取接口若需要参数，在上方填写或下拉选择。Set 默认把刚读到的值原样写回。";

            this._paramBar.AutoSize = true;
            this._paramBar.Dock = DockStyle.Top;
            this._paramBar.Name = "_paramBar";
            this._paramBar.Padding = new Padding(0, 0, 0, 4);
            this._paramBar.Visible = false;
            this._paramBar.WrapContents = true;

            this._includeDanger.AutoSize = true;
            this._includeDanger.Name = "_includeDanger";
            this._includeDanger.Text = "包含危险接口";

            this._invoke.AutoSize = true;
            this._invoke.Name = "_invoke";
            this._invoke.Text = "调用";

            this._runGroup.AutoSize = true;
            this._runGroup.Name = "_runGroup";
            this._runGroup.Text = "本组全部";

            this._runAll.AutoSize = true;
            this._runAll.Name = "_runAll";
            this._runAll.Text = "全部调用";

            TuneBox(this._send, mono);
            this._send.Name = "_send";
            TuneBox(this._receive, mono);
            this._receive.Name = "_receive";
            this._receive.ReadOnly = true;
            TuneBox(this._endpoint, mono);
            this._endpoint.Name = "_endpoint";
            this._endpoint.ReadOnly = true;

            this._actionBar.AutoSize = true;
            this._actionBar.Controls.Add(this._invoke);
            this._actionBar.Controls.Add(this._runGroup);
            this._actionBar.Controls.Add(this._runAll);
            this._actionBar.Controls.Add(this._includeDanger);
            this._actionBar.Dock = DockStyle.Top;
            this._actionBar.Name = "_actionBar";
            this._actionBar.Padding = new Padding(0, 4, 0, 8);
            this._actionBar.WrapContents = true;

            this._lblSend.Dock = DockStyle.Top;
            this._lblSend.Font = bold;
            this._lblSend.Height = 22;
            this._lblSend.Name = "_lblSend";
            this._lblSend.Text = "1 需要发送的内容";

            this._sendHost.Controls.Add(this._send);
            this._sendHost.Controls.Add(this._lblSend);
            this._sendHost.Dock = DockStyle.Fill;
            this._sendHost.Name = "_sendHost";
            this._sendHost.Padding = new Padding(0, 0, 0, 6);

            this._lblReceive.Dock = DockStyle.Top;
            this._lblReceive.Font = bold;
            this._lblReceive.Height = 22;
            this._lblReceive.Name = "_lblReceive";
            this._lblReceive.Text = "2 获取到的内容";

            this._receiveHost.Controls.Add(this._receive);
            this._receiveHost.Controls.Add(this._lblReceive);
            this._receiveHost.Dock = DockStyle.Fill;
            this._receiveHost.Name = "_receiveHost";
            this._receiveHost.Padding = new Padding(0, 0, 0, 6);

            this._lblEndpoint.Dock = DockStyle.Top;
            this._lblEndpoint.Font = bold;
            this._lblEndpoint.Height = 22;
            this._lblEndpoint.Name = "_lblEndpoint";
            this._lblEndpoint.Text = "3 请求的地址和方法名称";

            this._endpointHost.Controls.Add(this._endpoint);
            this._endpointHost.Controls.Add(this._lblEndpoint);
            this._endpointHost.Dock = DockStyle.Fill;
            this._endpointHost.Name = "_endpointHost";
            this._endpointHost.Padding = new Padding(0, 0, 0, 6);

            this._panes.ColumnCount = 1;
            this._panes.Controls.Add(this._sendHost, 0, 0);
            this._panes.Controls.Add(this._receiveHost, 0, 1);
            this._panes.Controls.Add(this._endpointHost, 0, 2);
            this._panes.Dock = DockStyle.Fill;
            this._panes.Name = "_panes";
            this._panes.Padding = new Padding(0, 4, 0, 0);
            this._panes.RowCount = 3;
            this._panes.RowStyles.Add(new RowStyle(SizeType.Percent, 38F));
            this._panes.RowStyles.Add(new RowStyle(SizeType.Percent, 38F));
            this._panes.RowStyles.Add(new RowStyle(SizeType.Percent, 24F));

            this._right.Controls.Add(this._panes);
            this._right.Controls.Add(this._actionBar);
            this._right.Controls.Add(this._paramBar);
            this._right.Controls.Add(this._summary);
            this._right.Controls.Add(this._title);
            this._right.Dock = DockStyle.Fill;
            this._right.Name = "_right";
            this._right.Padding = new Padding(8);

            this._left.Controls.Add(this._tree);
            this._left.Dock = DockStyle.Left;
            this._left.Name = "_left";
            this._left.Padding = new Padding(8);
            this._left.Width = 340;

            this.ClientSize = new Size(1280, 780);
            this.Controls.Add(this._right);
            this.Controls.Add(this._left);
            this.MinimumSize = new Size(1000, 640);
            this.Name = "ApiTestForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "接口测试";

            this._left.ResumeLayout(false);
            this._actionBar.ResumeLayout(false);
            this._actionBar.PerformLayout();
            this._panes.ResumeLayout(false);
            this._sendHost.ResumeLayout(false);
            this._sendHost.PerformLayout();
            this._receiveHost.ResumeLayout(false);
            this._receiveHost.PerformLayout();
            this._endpointHost.ResumeLayout(false);
            this._endpointHost.PerformLayout();
            this._right.ResumeLayout(false);
            this._right.PerformLayout();
            this.ResumeLayout(false);
        }

        static void TuneBox(TextBox box, Font font)
        {
            box.Multiline = true;
            box.ScrollBars = ScrollBars.Both;
            box.Dock = DockStyle.Fill;
            box.Font = font;
            box.WordWrap = false;
            box.AcceptsReturn = true;
            box.AcceptsTab = true;
        }

        private TreeView _tree;
        private Label _title;
        private Label _summary;
        private FlowLayoutPanel _paramBar;
        private CheckBox _includeDanger;
        private Button _invoke;
        private Button _runGroup;
        private Button _runAll;
        private TextBox _send;
        private TextBox _receive;
        private TextBox _endpoint;
        private FlowLayoutPanel _actionBar;
        private TableLayoutPanel _panes;
        private Panel _sendHost;
        private Label _lblSend;
        private Panel _receiveHost;
        private Label _lblReceive;
        private Panel _endpointHost;
        private Label _lblEndpoint;
        private Panel _right;
        private Panel _left;
    }
}
