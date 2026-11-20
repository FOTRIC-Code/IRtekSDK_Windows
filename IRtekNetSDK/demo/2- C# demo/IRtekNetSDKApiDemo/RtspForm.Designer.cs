using System.Drawing;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    partial class RtspForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseFrame();
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
            this._stopCallback = new Button();
            this._bar = new FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this._view)).BeginInit();
            this._bar.SuspendLayout();
            this.SuspendLayout();

            this._view.BackColor = Color.Black;
            this._view.Dock = DockStyle.Fill;
            this._view.Name = "_view";
            this._view.SizeMode = PictureBoxSizeMode.Zoom;

            this._streams.DropDownStyle = ComboBoxStyle.DropDownList;
            this._streams.Name = "_streams";
            this._streams.Width = 280;

            this._url.Name = "_url";
            this._url.ReadOnly = true;
            this._url.Width = 420;

            this._status.AutoSize = true;
            this._status.Name = "_status";
            this._status.Padding = new Padding(8, 8, 0, 0);
            this._status.Text = "未播放";

            this._play.AutoSize = true;
            this._play.Name = "_play";
            this._play.Text = "播放";

            this._stop.AutoSize = true;
            this._stop.Name = "_stop";
            this._stop.Text = "停止 net_stop";

            this._stopCallback.AutoSize = true;
            this._stopCallback.Name = "_stopCallback";
            this._stopCallback.Text = "停止 回调置空";

            this._bar.AutoSize = true;
            this._bar.Controls.Add(this._streams);
            this._bar.Controls.Add(this._url);
            this._bar.Controls.Add(this._play);
            this._bar.Controls.Add(this._stop);
            this._bar.Controls.Add(this._stopCallback);
            this._bar.Controls.Add(this._status);
            this._bar.Dock = DockStyle.Top;
            this._bar.Name = "_bar";
            this._bar.Padding = new Padding(8);
            this._bar.WrapContents = true;

            this.ClientSize = new Size(960, 680);
            this.Controls.Add(this._view);
            this.Controls.Add(this._bar);
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new Size(640, 520);
            this.Name = "RtspForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "RTSP";

            ((System.ComponentModel.ISupportInitialize)(this._view)).EndInit();
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
        private Button _stopCallback;
        private FlowLayoutPanel _bar;
    }
}
