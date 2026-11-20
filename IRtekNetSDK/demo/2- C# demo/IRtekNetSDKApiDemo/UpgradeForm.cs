using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal sealed partial class UpgradeForm : Form
    {
        readonly DeviceSession _session;
        bool _working;

        public bool DeviceRestarted { get; private set; }

        public UpgradeForm()
        {
            InitializeComponent();
        }

        public UpgradeForm(DeviceSession session) : this()
        {
            _session = session;
            if (session != null)
                _title.Text = "已连接 " + session.Ip + "。选择固件包后上传到设备。";
        }

        void OnBrowse(object sender, System.EventArgs e)
        {
            Browse();
        }

        async void OnStart(object sender, System.EventArgs e)
        {
            await StartAsync();
        }

        void OnClose(object sender, System.EventArgs e)
        {
            if (!_working)
                Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_working)
                e.Cancel = true;
            base.OnFormClosing(e);
        }

        void Browse()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "选择固件包";
                dialog.Filter = "固件文件 (*.bin)|*.bin|所有文件 (*.*)|*.*";
                dialog.CheckFileExists = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    _path.Text = dialog.FileName;
            }
        }

        async Task StartAsync()
        {
            if (_working)
                return;
            if (_session == null || _session.Handle == IntPtr.Zero)
            {
                MessageBox.Show(this, "当前没有已连接的设备。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selected = _path.Text.Trim();
            string sdkPath;
            string tempCopy;
            string error;
            if (!TryPrepareUpgradePath(selected, out sdkPath, out tempCopy, out error))
            {
                MessageBox.Show(this, error, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                    this,
                    "即将向 " + _session.Ip + " 上传固件并升级，过程中请勿断电。是否继续？",
                    Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                DeleteTemp(tempCopy);
                return;
            }

            SetWorking(true);
            _status.ForeColor = SystemColors.ControlText;
            _status.Text = "正在上传固件，请稍候…";
            SdkLifetime.Write("开始升级：" + sdkPath);
            IntPtr handle = _session.Handle;
            try
            {
                int code = await Task.Run(() =>
                {
                    NetUpgradeInfo info = NetUpgradeInfo.FromFile(sdkPath);
                    return NetSdk.net_device_upgrade(handle, ref info);
                });

                if (code != NetSdk.Ok)
                {
                    _status.ForeColor = Color.Firebrick;
                    _status.Text = "升级失败：" + NetSdk.ErrorText(code);
                    SdkLifetime.Write(_status.Text);
                    MessageBox.Show(this, _status.Text, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _status.Text = "固件已上传。";
                SdkLifetime.Write("升级成功");
                if (MessageBox.Show(
                        this,
                        "固件已上传。设备通常需要重启后生效。是否立即重启？",
                        Text,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                _status.Text = "正在重启设备…";
                int restartCode = await Task.Run(() => NetSdk.net_device_restart(handle));
                DeviceRestarted = true;
                if (restartCode == NetSdk.Ok)
                {
                    _status.Text = "设备正在重启，连接将断开。";
                    SdkLifetime.Write("已发送重启命令");
                }
                else
                {
                    _status.Text = "重启调用返回：" + NetSdk.ErrorText(restartCode) + "。设备可能已自行重启，连接将断开。";
                    SdkLifetime.Write("重启失败：" + NetSdk.ErrorText(restartCode));
                }
                MessageBox.Show(this, _status.Text, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetWorking(false);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                _status.ForeColor = Color.Firebrick;
                _status.Text = "升级异常：" + ex.Message;
                SdkLifetime.Write(_status.Text);
                MessageBox.Show(this, _status.Text, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DeleteTemp(tempCopy);
                if (!IsDisposed && DialogResult != DialogResult.OK)
                    SetWorking(false);
            }
        }

        void SetWorking(bool working)
        {
            _working = working;
            _path.Enabled = !working;
            _browse.Enabled = !working;
            _start.Enabled = !working;
            _close.Enabled = !working;
            UseWaitCursor = working;
        }

        static bool TryPrepareUpgradePath(string source, out string sdkPath, out string tempCopy, out string error)
        {
            sdkPath = source;
            tempCopy = null;
            error = null;
            if (string.IsNullOrWhiteSpace(source) || !File.Exists(source))
            {
                error = "请先选择有效的固件文件。";
                return false;
            }

            if (Encoding.UTF8.GetByteCount(source) < NetSdk.PathLen)
                return true;

            string dest = Path.Combine(Path.GetTempPath(), "parkia_upgrade.bin");
            if (Encoding.UTF8.GetByteCount(dest) >= NetSdk.PathLen)
                dest = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory) ?? @"C:\", "parkia_upgrade.bin");
            if (Encoding.UTF8.GetByteCount(dest) >= NetSdk.PathLen)
            {
                error = "固件路径超过 SDK 限制（127 字节），且无法复制到更短的临时路径。";
                return false;
            }

            try
            {
                File.Copy(source, dest, true);
            }
            catch (Exception ex)
            {
                error = "固件路径过长，复制到临时目录失败：" + ex.Message;
                return false;
            }

            sdkPath = dest;
            tempCopy = dest;
            SdkLifetime.Write("固件路径过长，已复制到 " + dest);
            return true;
        }

        static void DeleteTemp(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
            }
        }
    }
}
