using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal enum VideoChoice
    {
        None,
        Raw,
        Rtsp
    }

    internal sealed partial class VideoModeForm : Form
    {
        public VideoChoice Selected { get; private set; }

        public VideoModeForm()
        {
            InitializeComponent();
        }

        public VideoModeForm(string ip) : this()
        {
            if (!string.IsNullOrEmpty(ip))
                _title.Text = "已连接 " + ip + "，选择一种看视频的方式";
        }

        void OnRawClick(object sender, System.EventArgs e)
        {
            Choose(VideoChoice.Raw);
        }

        void OnRtspClick(object sender, System.EventArgs e)
        {
            Choose(VideoChoice.Rtsp);
        }

        void Choose(VideoChoice choice)
        {
            Selected = choice;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
