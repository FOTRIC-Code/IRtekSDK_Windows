using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace IRtekNetSDKApiDemo
{
    partial class AlarmVideoPlayer
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopPlay();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._host = new ElementHost();
            this.SuspendLayout();

            this._host.Dock = DockStyle.Fill;
            this._host.Name = "_host";

            this.BackColor = Color.Black;
            this.Controls.Add(this._host);
            this.Name = "AlarmVideoPlayer";
            this.Size = new Size(320, 240);
            this.ResumeLayout(false);
        }

        private ElementHost _host;
    }
}
