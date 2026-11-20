using System.Drawing;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    partial class UpgradeForm
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
            this._title = new Label();
            this._path = new TextBox();
            this._browse = new Button();
            this._pathRow = new TableLayoutPanel();
            this._start = new Button();
            this._close = new Button();
            this._actions = new FlowLayoutPanel();
            this._status = new Label();
            this._root = new TableLayoutPanel();
            this._pathRow.SuspendLayout();
            this._actions.SuspendLayout();
            this._root.SuspendLayout();
            this.SuspendLayout();

            this._title.AutoSize = true;
            this._title.Margin = new Padding(0, 0, 0, 8);
            this._title.Name = "_title";
            this._title.Text = "选择固件包后上传到设备。";

            this._path.Dock = DockStyle.Fill;
            this._path.Name = "_path";

            this._browse.AutoSize = true;
            this._browse.Name = "_browse";
            this._browse.Text = "浏览";
            this._browse.Click += new System.EventHandler(this.OnBrowse);

            this._pathRow.AutoSize = true;
            this._pathRow.ColumnCount = 2;
            this._pathRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._pathRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this._pathRow.Controls.Add(this._path, 0, 0);
            this._pathRow.Controls.Add(this._browse, 1, 0);
            this._pathRow.Dock = DockStyle.Top;
            this._pathRow.Margin = new Padding(0);
            this._pathRow.Name = "_pathRow";

            this._start.AutoSize = true;
            this._start.Name = "_start";
            this._start.Text = "开始升级";
            this._start.Click += new System.EventHandler(this.OnStart);

            this._close.AutoSize = true;
            this._close.Name = "_close";
            this._close.Text = "关闭";
            this._close.Click += new System.EventHandler(this.OnClose);

            this._actions.AutoSize = true;
            this._actions.Controls.Add(this._start);
            this._actions.Controls.Add(this._close);
            this._actions.Margin = new Padding(0, 16, 0, 0);
            this._actions.Name = "_actions";
            this._actions.WrapContents = false;

            this._status.AutoSize = true;
            this._status.ForeColor = SystemColors.GrayText;
            this._status.Margin = new Padding(0, 12, 0, 0);
            this._status.Name = "_status";
            this._status.Text = "升级过程中请勿断电或断开网络。完成后设备通常需要重启。";

            this._root.ColumnCount = 1;
            this._root.Controls.Add(this._title, 0, 0);
            this._root.Controls.Add(this._pathRow, 0, 1);
            this._root.Controls.Add(this._status, 0, 2);
            this._root.Controls.Add(this._actions, 0, 3);
            this._root.Dock = DockStyle.Fill;
            this._root.Name = "_root";
            this._root.Padding = new Padding(16);
            this._root.RowCount = 4;

            this.AcceptButton = this._start;
            this.CancelButton = this._close;
            this.ClientSize = new Size(560, 220);
            this.Controls.Add(this._root);
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpgradeForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "设备升级";

            this._pathRow.ResumeLayout(false);
            this._pathRow.PerformLayout();
            this._actions.ResumeLayout(false);
            this._actions.PerformLayout();
            this._root.ResumeLayout(false);
            this._root.PerformLayout();
            this.ResumeLayout(false);
        }

        private Label _title;
        private TextBox _path;
        private Button _browse;
        private TableLayoutPanel _pathRow;
        private Button _start;
        private Button _close;
        private FlowLayoutPanel _actions;
        private Label _status;
        private TableLayoutPanel _root;
    }
}
