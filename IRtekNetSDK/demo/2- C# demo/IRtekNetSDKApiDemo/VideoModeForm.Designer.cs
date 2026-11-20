using System;
using System.Drawing;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    partial class VideoModeForm
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
            this._raw = new Button();
            this._rtsp = new Button();
            this._choices = new TableLayoutPanel();
            this._cancel = new Button();
            this._choices.SuspendLayout();
            this.SuspendLayout();

            this._title.Dock = DockStyle.Top;
            this._title.Height = 48;
            this._title.Name = "_title";
            this._title.Text = "选择一种看视频的方式";
            this._title.TextAlign = ContentAlignment.MiddleCenter;

            this._raw.Dock = DockStyle.Fill;
            this._raw.Margin = new Padding(8);
            this._raw.Name = "_raw";
            this._raw.Text = "原始视频流" + Environment.NewLine + Environment.NewLine + "SDK 解码后的 BGRA 帧，直接画到窗口";
            this._raw.TextAlign = ContentAlignment.MiddleCenter;
            this._raw.Click += new System.EventHandler(this.OnRawClick);

            this._rtsp.Dock = DockStyle.Fill;
            this._rtsp.Margin = new Padding(8);
            this._rtsp.Name = "_rtsp";
            this._rtsp.Text = "RTSP" + Environment.NewLine + Environment.NewLine + "SDK 用内置 FFmpeg 拉 RTSP，再把画面画到窗口";
            this._rtsp.TextAlign = ContentAlignment.MiddleCenter;
            this._rtsp.Click += new System.EventHandler(this.OnRtspClick);

            this._choices.ColumnCount = 2;
            this._choices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this._choices.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this._choices.Controls.Add(this._raw, 0, 0);
            this._choices.Controls.Add(this._rtsp, 1, 0);
            this._choices.Dock = DockStyle.Fill;
            this._choices.Name = "_choices";
            this._choices.Padding = new Padding(16, 0, 16, 16);

            this._cancel.DialogResult = DialogResult.Cancel;
            this._cancel.Name = "_cancel";
            this._cancel.Size = new Size(0, 0);
            this._cancel.TabStop = false;
            this._cancel.Visible = false;

            this.CancelButton = this._cancel;
            this.ClientSize = new Size(460, 210);
            this.Controls.Add(this._choices);
            this.Controls.Add(this._title);
            this.Controls.Add(this._cancel);
            this.Font = new Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VideoModeForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "看视频";

            this._choices.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private Label _title;
        private Button _raw;
        private Button _rtsp;
        private TableLayoutPanel _choices;
        private Button _cancel;
    }
}
