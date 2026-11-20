using System.Drawing;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    partial class LivePreviewPanel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleasePreview();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._view = new PictureBox();
            this._streams = new ComboBox();
            this._url = new TextBox();
            this._status = new Label();
            this._play = new Button();
            this._stop = new Button();
            this._title = new Label();
            this._actions = new FlowLayoutPanel();
            this._bar = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this._view)).BeginInit();
            this._actions.SuspendLayout();
            this._bar.SuspendLayout();
            this.SuspendLayout();

            this._view.BackColor = Color.Black;
            this._view.Dock = DockStyle.Fill;
            this._view.Margin = new Padding(0);
            this._view.Name = "_view";
            this._view.SizeMode = PictureBoxSizeMode.Zoom;

            this._streams.Dock = DockStyle.Fill;
            this._streams.DropDownStyle = ComboBoxStyle.DropDownList;
            this._streams.Enabled = false;
            this._streams.Margin = new Padding(0, 0, 0, 4);
            this._streams.Name = "_streams";

            this._url.Dock = DockStyle.Fill;
            this._url.Margin = new Padding(0, 0, 0, 2);
            this._url.Name = "_url";
            this._url.ReadOnly = true;

            this._status.AutoSize = true;
            this._status.Name = "_status";
            this._status.Padding = new Padding(8, 6, 0, 0);
            this._status.Text = "未连接";

            this._play.AutoSize = true;
            this._play.Enabled = false;
            this._play.Name = "_play";
            this._play.Text = "播放";

            this._stop.AutoSize = true;
            this._stop.Enabled = false;
            this._stop.Name = "_stop";
            this._stop.Text = "停止";

            this._title.Dock = DockStyle.Top;
            this._title.Height = 22;
            this._title.Name = "_title";
            this._title.Text = "RTSP 预览";
            this._title.TextAlign = ContentAlignment.MiddleLeft;

            this._actions.AutoSize = true;
            this._actions.Controls.Add(this._play);
            this._actions.Controls.Add(this._stop);
            this._actions.Controls.Add(this._status);
            this._actions.Dock = DockStyle.Fill;
            this._actions.Name = "_actions";
            this._actions.Padding = new Padding(0, 2, 0, 2);
            this._actions.WrapContents = true;

            this._bar.AutoSize = true;
            this._bar.ColumnCount = 1;
            this._bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._bar.Controls.Add(this._streams, 0, 0);
            this._bar.Controls.Add(this._url, 0, 1);
            this._bar.Controls.Add(this._actions, 0, 2);
            this._bar.Dock = DockStyle.Top;
            this._bar.Name = "_bar";
            this._bar.Padding = new Padding(0, 0, 0, 4);
            this._bar.RowCount = 3;
            this._bar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this._bar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this._bar.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            this.Controls.Add(this._view);
            this.Controls.Add(this._bar);
            this.Controls.Add(this._title);
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.Name = "LivePreviewPanel";

            ((System.ComponentModel.ISupportInitialize)(this._view)).EndInit();
            this._actions.ResumeLayout(false);
            this._actions.PerformLayout();
            this._bar.ResumeLayout(false);
            this._bar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private PictureBox _view;
        private ComboBox _streams;
        private TextBox _url;
        private Label _status;
        private Button _play;
        private Button _stop;
        private Label _title;
        private FlowLayoutPanel _actions;
        private TableLayoutPanel _bar;
    }
}
