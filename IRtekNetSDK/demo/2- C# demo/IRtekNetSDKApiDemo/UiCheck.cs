using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal static class UiCheck
    {
        static readonly List<string> Failures = new List<string>();
        static readonly List<string> Lines = new List<string>();
        static int Passed;

        public static int Run()
        {
            Failures.Clear();
            Lines.Clear();
            Passed = 0;
            RunSection("主窗口", CheckMainForm);
            RunSection("独立预览", CheckPreview);
            RunSection("原始视频流", delegate { CheckStreams(new RawStreamForm(SampleSession())); });
            RunSection("RTSP", delegate { CheckStreams(new RtspForm(SampleSession())); });
            RunSection("看视频", CheckVideoMode);
            RunSection("升级", CheckUpgrade);
            RunSection("接口测试", CheckApi);
            RunSection("录像播放器", CheckPlayer);

            string summary = Passed + " passed, " + Failures.Count + " failed";
            Lines.Add(summary);
            foreach (string failure in Failures)
                Lines.Add("FAIL " + failure);
            string logPath = Path.Combine(Path.GetTempPath(), "IRtekNetSDKApiDemo_ui_check.txt");
            File.WriteAllLines(logPath, Lines, new UTF8Encoding(true));
            Console.WriteLine(summary);
            Console.WriteLine(logPath);
            foreach (string failure in Failures)
                Console.WriteLine("FAIL " + failure);
            return Failures.Count == 0 ? 0 : 1;
        }

        static void CheckMainForm()
        {
            using (var form = new MainForm())
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(-3200, -3200);
                form.Opacity = 0;
                form.ShowInTaskbar = false;
                form.Show();
                Application.DoEvents();

                Expect(form.Text == "IRtekNetSDK API Demo", "主窗口标题");
                Expect(Find(form, "_search") is Button search && search.Text == "搜索设备" && search.Enabled, "搜索按钮");
                Expect(Find(form, "_connect") is Button connect && connect.Text == "连接", "连接按钮");
                Expect(Find(form, "_disconnect") is Button disconnect && !disconnect.Enabled, "断开按钮初始禁用");
                Expect(Find(form, "_watch") is Button watch && watch.Text == "原始视频流" && !watch.Enabled, "原始视频流按钮初始禁用");
                Expect(Find(form, "_upgrade") is Button upgrade && !upgrade.Enabled, "升级按钮初始禁用");
                Expect(Find(form, "_apiTest") is Button api && api.Text == "接口测试" && !api.Enabled, "接口测试按钮初始禁用");
                Expect(Find(form, "_port") is TextBox port && port.Text == "80", "HTTP 端口默认 80");
                Expect(Find(form, "_password") is TextBox password && password.UseSystemPasswordChar, "密码框遮罩");
                Expect(Find(form, "_status") is Label status && status.Text == "未连接", "连接状态");
                Expect(Find(form, "_tabs") is TabControl tabs && tabs.TabPages.Count == 2, "两个页签");
                Expect(Find(form, "_devicePage") is TabPage devices && devices.Text == "设备", "设备页");
                Expect(Find(form, "_alarmPage") is TabPage alarms && alarms.Text == "报警查询与录像", "报警页");
                var deviceList = Find(form, "_devices") as ListView;
                var alarmList = Find(form, "_alarms") as ListView;
                Expect(deviceList != null && deviceList.Columns.Count == 6, "设备列表列 " + (deviceList == null ? "missing" : deviceList.Columns.Count.ToString()));
                Expect(alarmList != null && alarmList.Columns.Count == 3, "实时报警列 " + (alarmList == null ? "missing" : alarmList.Columns.Count.ToString()));
                Expect(Find(form, "_log") is TextBox log && log.Multiline && log.ReadOnly, "日志框");
                Expect(FindType<LivePreviewPanel>(form) != null, "右侧预览面板");
                Expect(Find(form, "_title") is Label previewTitle && previewTitle.Text == "RTSP 预览", "预览标题");
                Expect(Find(form, "_view") is PictureBox view && view.BackColor == Color.Black, "预览画面");

                var tabsControl = (TabControl)Find(form, "_tabs");
                tabsControl.SelectedTab = (TabPage)Find(form, "_alarmPage");
                Application.DoEvents();

                Expect(Find(form, "_channel") is NumericUpDown channel && channel.Value == ChannelIds.Default, "报警查询通道号是 101");
                Expect(Find(form, "_channel") is NumericUpDown channelMax && channelMax.Maximum >= ChannelIds.Default, "通道号上限能放下 101");
                Expect(Find(form, "_type") is ComboBox type && type.Items.Count == 8 && type.SelectedIndex == 0, "报警类型下拉");
                Expect(Find(form, "_pageSize") is NumericUpDown pageSize && pageSize.Value == 20, "每页 20");
                Expect(Find(form, "_sftpPort") is NumericUpDown sftpPort && sftpPort.Value == 22, "SFTP 端口 22");
                Expect(Find(form, "_list") is ListView list && list.Columns.Count == 8 && list.CheckBoxes && list.MultiSelect, "报警记录可勾选多条");
                Expect(Find(form, "_watchCount") is NumericUpDown watchCount && watchCount.Minimum == 1 && watchCount.Maximum == 20 && watchCount.Value == 2, "一次播放条数");
                Expect(Find(form, "_addPick") == null && Find(form, "_removePick") == null && Find(form, "_clearPick") == null && Find(form, "_picked") == null, "不再有加入、移除、清空和选中列表");
                Expect(Find(form, "_watchPick") is Button watchPick && watchPick.Text == "一同观看", "一同观看按钮");
                Expect(Find(form, "_closeWatch") is Button closeWatch && closeWatch.Text == "关闭观看", "关闭观看按钮");
                Expect(Find(form, "_watchBoard") is Panel watchBoard && !watchBoard.Visible, "多报警观看区初始隐藏");
                Expect(Find(form, "_query") is Button query && query.Text == "查询", "查询按钮");
                Expect(Find(form, "_videoHost") is Panel videoHost && videoHost.BackColor == Color.Black, "录像区域");
                Expect(Find(form, "_chart") is SafeChart, "曲线控件");

                var panel = (AlarmQueryPanel)Find(form, "_alarmQuery");
                var session = SampleSession();
                panel.Bind(session);
                Expect(panel.Enabled, "绑定设备后报警页可用");
                Expect(Find(form, "_sftpHost") is TextBox host && host.Text == session.Ip, "SFTP 主机填入设备 IP");
                Expect(Find(form, "_start") is DateTimePicker start && Find(form, "_stop") is DateTimePicker stop && start.Value < stop.Value, "查询开始早于结束");
                var sample = new AlarmSearchItem
                {
                    AlarmId = 9,
                    ChannelId = 101,
                    AlarmType = 3,
                    TimestampUs = 1_700_000_000_000_000UL,
                    Detail = "温度 88 阈值 60",
                    RecordPath = "/userdata/a.mp4",
                    SnapshotPath = "/userdata/a.jpg",
                    DataPath = "/userdata/data.json"
                };
                string tip = AlarmQueryPanel.FormatAlarmTip(sample);
                Expect(tip.StartsWith("内容：温度 88 阈值 60") && tip.Contains("通道：101") && tip.Contains("/userdata/a.mp4"), "报警内容提示");
                var searchList = (ListView)Find(form, "_list");
                var countBox = (NumericUpDown)Find(form, "_watchCount");
                watchPick = (Button)Find(form, "_watchPick");
                closeWatch = (Button)Find(form, "_closeWatch");
                var hint = (Label)Find(form, "_pickHint");
                watchPick.PerformClick();
                Expect(hint.Text.Contains("请先查询"), "没有报警时一同观看给出提示");
                var row = new ListViewItem("2026-01-01 00:00:00");
                row.SubItems.Add("高温");
                row.SubItems.Add("9");
                row.SubItems.Add("101");
                row.SubItems.Add(sample.Detail);
                row.Tag = sample;
                searchList.Items.Add(row);
                var later = new AlarmSearchItem
                {
                    AlarmId = 11,
                    ChannelId = 101,
                    AlarmType = 4,
                    TimestampUs = 1_700_000_200_000_000UL,
                    Detail = "温度监测"
                };
                var laterRow = new ListViewItem("2026-01-01 00:01:00");
                laterRow.SubItems.Add("温度监测");
                laterRow.SubItems.Add("11");
                laterRow.SubItems.Add("101");
                laterRow.SubItems.Add(later.Detail);
                laterRow.Tag = later;
                searchList.Items.Add(laterRow);
                var third = new AlarmSearchItem
                {
                    AlarmId = 12,
                    ChannelId = 101,
                    AlarmType = 2,
                    TimestampUs = 1_700_000_300_000_000UL,
                    Detail = "另一条运动"
                };
                var thirdRow = new ListViewItem("2026-01-01 00:02:00");
                thirdRow.SubItems.Add("入侵");
                thirdRow.SubItems.Add("12");
                thirdRow.SubItems.Add("101");
                thirdRow.SubItems.Add(third.Detail);
                thirdRow.Tag = third;
                searchList.Items.Add(thirdRow);
                countBox.Value = 2;
                row.Selected = true;
                row.Focused = true;
                List<AlarmSearchItem> fromSelection = panel.ItemsToWatchForCheck();
                Expect(fromSelection.Count == 2 && fromSelection[0].AlarmId == 9 && fromSelection[1].AlarmId == 11, "没勾选时从点中的报警按条数往后取");
                thirdRow.Selected = true;
                thirdRow.Focused = true;
                fromSelection = panel.ItemsToWatchForCheck();
                Expect(fromSelection.Count == 1 && fromSelection[0].AlarmId == 12, "后面不够条数时只取剩下的");
                row.Checked = true;
                laterRow.Checked = true;
                thirdRow.Checked = true;
                countBox.Value = 2;
                fromSelection = panel.ItemsToWatchForCheck();
                Expect(fromSelection.Count == 2 && fromSelection[0].AlarmId == 9 && fromSelection[1].AlarmId == 11, "勾选多于一次播放条数时只播前面的");
                thirdRow.Checked = false;
                laterRow.Checked = false;
                fromSelection = panel.ItemsToWatchForCheck();
                Expect(fromSelection.Count == 1 && fromSelection[0].AlarmId == 9, "勾选少于一次播放条数时只播勾选的");
                var board = (AlarmWatchBoard)Find(form, "_watchBoard");
                var second = new AlarmSearchItem
                {
                    AlarmId = 10,
                    ChannelId = 101,
                    AlarmType = 2,
                    TimestampUs = 1_700_000_100_000_000UL,
                    Detail = "运动强度 3"
                };
                int shown = panel.ShowWatchForCheck(new List<AlarmSearchItem> { sample, second });
                Application.DoEvents();
                Rectangle firstCard = board.CardAt(0).Bounds;
                Rectangle secondCard = board.CardAt(1).Bounds;
                Expect(shown == 2 && board.Visible && firstCard.Width > 80 && secondCard.Width > 80 && !firstCard.IntersectsWith(secondCard), "两条报警可以同时摆开观看");
                closeWatch.PerformClick();
                Expect(!board.Visible && board.CardCount == 0, "关闭观看后回到单条预览");
                searchList.Items.Clear();
                query = (Button)Find(form, "_query");
                query.PerformClick();
                Application.DoEvents();
                Expect(true, "未连接句柄时点查询不会卡住");
                panel.Unbind();
                Expect(!panel.Enabled, "解绑后报警页禁用");

            var preview = FindType<LivePreviewPanel>(form);
            Expect(preview != null, "能找到预览面板实例");
            if (preview == null)
                return;
            preview.Bind(session);
                Expect(Find(form, "_streams") is ComboBox streams && SelectedStreamId(streams) == ChannelIds.Default, "主界面预览选中通道 101");
                Expect(Find(form, "_url") is TextBox url && url.Text.Contains("/video/" + ChannelIds.Default), "预览地址带 101");
                preview.PresentFrame(2, 2, new byte[2 * 2 * 4]);
                Expect(Find(form, "_view") is PictureBox painted && painted.Image != null && painted.Image.Width == 2, "预览能画一帧");
                preview.Unbind();
                Expect(Find(form, "_url") is TextBox cleared && cleared.Text == "", "停止预览后地址清空");

                var clear = (Button)Find(form, "_clearAlarms");
                var live = (ListView)Find(form, "_alarms");
                live.Items.Add("x");
                clear.PerformClick();
                Expect(live.Items.Count == 0, "清空实时报警");

                var current = new DeviceSession { Ip = "10.0.0.1", HttpPort = 80 };
                current.Streams.Add(new StreamDescriptor { Id = ChannelIds.Default, Codec = "video" });
                form.UseSessionForCheck(current);
                preview.Bind(current);
                Expect(form.ReleaseForTarget("10.0.0.1", 80, true) && !form.HasSessionForCheck, "双击当前设备也会断开原连接");
                Expect(Find(form, "_url") is TextBox droppedSame && droppedSame.Text == "", "双击当前设备后停止旧视频");
                form.UseSessionForCheck(current);
                preview.Bind(current);
                Expect(form.ReleaseForTarget("10.0.0.2", 80, false) == false && form.HasSessionForCheck, "连接按钮不自动切换设备");
                live.Items.Add("旧设备报警");
                Expect(form.ReleaseForTarget("10.0.0.2", 80, true), "双击新设备先断开原设备");
                Expect(!form.HasSessionForCheck, "原设备会话已清除");
                Expect(live.Items.Count == 0, "切换设备后清空旧报警");
                Expect(Find(form, "_url") is TextBox dropped && dropped.Text == "", "切换设备后停止旧视频");
                Expect(Find(form, "_status") is Label switched && switched.Text == "未连接", "切换后等待新设备连接");
                Expect(Find(form, "_disconnect") is Button disconnectOff && !disconnectOff.Enabled, "原设备断开按钮关闭");
                Expect(Find(form, "_alarmStatus") is Label alarmStatus && alarmStatus.Text == "未监听", "原设备报警监听已停止");

                Layout(form);
                form.ClientSize = form.MinimumSize - new Size(16, 39);
                Application.DoEvents();
                Layout(form);

                form.Hide();
            }
            Pass("主窗口关闭");
        }

        static void Layout(Form form)
        {
            var search = Find(form, "_searchBar");
            var side = FindDock(form, "_side", DockStyle.Right);
            var bottom = Find(form, "_bottom");
            var body = Find(form, "_body");
            var preview = FindType<LivePreviewPanel>(form);
            Expect(search != null && search.Width > 200 && search.Top <= 2, "搜索栏在顶部并且有宽度，实际 " + Bounds(search));
            Expect(side != null && side.Width >= 300 && side.Right >= form.ClientSize.Width - 2, "登录和预览贴在右侧，实际 " + Bounds(side));
            Expect(bottom != null && bottom.Height >= 200 && bottom.Bottom >= form.ClientSize.Height - 2, "报警和日志贴在底部，实际 " + Bounds(bottom));
            Expect(body != null && body.Width > 200 && body.Height > 100, "页签区域有面积，实际 " + Bounds(body));
            Expect(preview != null && preview.Width > 100 && preview.Height > 80, "预览画面有面积，实际 " + Bounds(preview));
            Expect(search.Bottom <= side.Top + 2 && search.Bottom <= body.Top + 2, "搜索栏不压住下面的区域");
            Expect(body.Right <= side.Left + 2, "页签不压住右侧栏");
            Expect(body.Bottom <= bottom.Top + 2, "页签不压住底部栏");
            var fields = Find(form, "_fields");
            ExpectSeparate(Find(fields, "_lblHint"), Find(fields, "_actions"), "说明文字和按钮");
            ExpectSeparate(Find(fields, "_actions"), Find(fields, "_status"), "按钮和连接状态");
            ExpectSeparate(Find(fields, "_port"), Find(fields, "_user"), "端口和用户名");
            ExpectSeparate(Find(form, "_connect"), Find(form, "_disconnect"), "连接和断开");
            ExpectSeparate(Find(form, "_filterWrap"), Find(form, "_sftpWrap"), "查询条件和 SFTP");
            ExpectSeparate(Find(form, "_sftpWrap"), Find(form, "_list"), "SFTP 和报警列表");
            ExpectSeparate(Find(form, "_list"), Find(form, "_watchWrap"), "查询列表和一次播放条");
            ExpectSeparate(Find(form, "_play"), Find(form, "_streams"), "播放按钮和流列表");
        }

        static void ExpectSeparate(Control a, Control b, string name)
        {
            Expect(!Intersects(a, b), name + " 不应重叠 " + Bounds(a) + " / " + Bounds(b));
        }

        static bool Intersects(Control a, Control b)
        {
            if (a == null || b == null || !a.Visible || !b.Visible || !a.IsHandleCreated || !b.IsHandleCreated)
                return false;
            Rectangle ra = a.RectangleToScreen(a.ClientRectangle);
            Rectangle rb = b.RectangleToScreen(b.ClientRectangle);
            ra.Inflate(-2, -2);
            rb.Inflate(-2, -2);
            if (ra.Width <= 0 || ra.Height <= 0 || rb.Width <= 0 || rb.Height <= 0)
                return false;
            return ra.IntersectsWith(rb);
        }

        static void CheckPreview()
        {
            using (var preview = new LivePreviewPanel())
            {
                Expect(Find(preview, "_play") is Button play && play.Text == "播放" && !play.Enabled, "未连接时播放禁用");
                Expect(Find(preview, "_stop") is Button stop && !stop.Enabled, "未连接时停止禁用");
                var session = SampleSession();
                session.Streams.Insert(0, new StreamDescriptor { Id = 201, Codec = "h264", Width = 320, Height = 240 });
                preview.Bind(session);
                var streams = (ComboBox)Find(preview, "_streams");
                Expect(SelectedStreamId(streams) == ChannelIds.Default, "多路流时优先 101，实际 " + streams.Text);
                Expect(((TextBox)Find(preview, "_url")).Text.EndsWith("/video/101"), "预览 RTSP 以 /video/101 结尾");
                preview.Unbind();
                Expect(streams.Items.Count == 0, "解绑后清空流列表");
            }
            Pass("独立预览面板");
        }

        static void CheckStreams(Form form)
        {
            using (form)
            {
                Expect(Find(form, "_play") is Button, form.GetType().Name + " 有播放按钮");
                Expect(Find(form, "_stop") is Button stop && stop.Text.Contains("停止"), form.GetType().Name + " 有停止按钮");
                Expect(Find(form, "_stopCallback") is Button, form.GetType().Name + " 有回调置空按钮");
                Expect(Find(form, "_view") is PictureBox, form.GetType().Name + " 有画面");
                var streams = (ComboBox)Find(form, "_streams");
                Expect(SelectedStreamId(streams) == ChannelIds.Default, form.GetType().Name + " 默认选中 101，实际 " + streams.Text);
                var url = Find(form, "_url") as TextBox;
                if (url != null)
                    Expect(url.Text.Contains("/video/101") && url.ReadOnly, form.GetType().Name + " 地址只读且包含 101");
            }
            Pass(form.GetType().Name);
        }

        static void CheckVideoMode()
        {
            using (var raw = new VideoModeForm("10.0.0.8"))
            {
                ShowHidden(raw);
                Expect(Find(raw, "_title") is Label title && title.Text.Contains("10.0.0.8"), "看视频标题带 IP");
                ((Button)Find(raw, "_raw")).PerformClick();
                Expect(raw.Selected == VideoChoice.Raw && raw.DialogResult == DialogResult.OK, "选择原始视频流");
            }
            using (var rtsp = new VideoModeForm("10.0.0.8"))
            {
                ShowHidden(rtsp);
                ((Button)Find(rtsp, "_rtsp")).PerformClick();
                Expect(rtsp.Selected == VideoChoice.Rtsp && rtsp.DialogResult == DialogResult.OK, "选择 RTSP");
            }
            Pass("看视频对话框");
        }

        static void CheckUpgrade()
        {
            using (var form = new UpgradeForm(SampleSession()))
            {
                Expect(Find(form, "_title") is Label title && title.Text.Contains("10.0.0.8"), "升级标题带 IP");
                Expect(Find(form, "_browse") is Button browse && browse.Text == "浏览", "升级浏览按钮");
                Expect(Find(form, "_start") is Button start && start.Text == "开始升级", "开始升级按钮");
                Expect(Find(form, "_path") is TextBox path && path.Text.Length == 0, "固件路径初始为空");
                Expect(Find(form, "_status") is Label status && status.Text.Length > 0, "升级提示");
                ((Button)Find(form, "_close")).PerformClick();
                Expect(form.IsDisposed || !form.Visible, "关闭升级窗口");
            }
            Pass("升级窗口");
        }

        static void CheckApi()
        {
            using (var form = new ApiTestForm(SampleSession()))
            {
                ShowHidden(form);
                var tree = (TreeView)Find(form, "_tree");
                Expect(tree.Nodes.Count > 0, "接口树有分组");
                Expect(Select(tree, "net_get_channel"), "能选中 net_get_channel");
                Application.DoEvents();
                var title = Find(form, "_title") as Label;
                Expect(title != null && title.Text.Contains("net_get_channel"), "接口标题 " + (title == null ? "missing" : title.Text));
                var channelBox = FirstNumeric(Find(form, "_paramBar"));
                Expect(channelBox != null && channelBox.Value == ChannelIds.Default, "通道参数默认 101，实际 " + (channelBox == null ? "missing" : channelBox.Value.ToString()));
                var endpoint = Find(form, "_endpoint") as TextBox;
                Expect(endpoint != null && endpoint.Text.Contains("101"), "请求地址带 101");

                Expect(Select(tree, "net_get_stream"), "能选中 net_get_stream");
                Application.DoEvents();
                var streamBox = FirstNumeric(Find(form, "_paramBar"));
                Expect(streamBox != null && streamBox.Value == ChannelIds.Default, "流 ID 默认 101，实际 " + (streamBox == null ? "missing" : streamBox.Value.ToString()));

                Expect(Select(tree, "net_find_alarm_datas"), "能选中报警查询接口");
                Application.DoEvents();
                var send = Find(form, "_send") as TextBox;
                Expect(send != null && send.Text.Contains("\"channel_id\": 101"), "报警查询示例通道号是 101");
                var boxes = Numerics(Find(form, "_paramBar"));
                Expect(boxes.Count >= 1 && boxes[0].Value == ChannelIds.Default, "报警查询参数通道号是 101，实际 " + (boxes.Count == 0 ? "missing" : boxes[0].Value.ToString()));
            }
            Pass("接口测试窗口");
        }

        static void CheckPlayer()
        {
            using (var player = new AlarmVideoPlayer())
            {
                Expect(player.BackColor == Color.Black, "播放器背景是黑色");
                Expect(Find(player, "_host") != null && player.Controls.Count == 1, "播放器宿主在设计器里");
                Expect(!player.PlayFile(null) && !player.PlayFile(""), "空路径不播放");
                Expect(!player.PlayFile(@"C:\missing\parkia-ui-check.mp4"), "不存在的录像返回失败");
            }
            Pass("录像播放器");
        }

        static DeviceSession SampleSession()
        {
            var session = new DeviceSession
            {
                Ip = "10.0.0.8",
                HttpPort = 80,
                UserName = "admin",
                Password = "admin",
                RtspPort = 554
            };
            session.Streams.Add(new StreamDescriptor { Id = 201, Codec = "h264", Width = 640, Height = 480 });
            session.Streams.Add(new StreamDescriptor { Id = ChannelIds.Default, Codec = "h264", Width = 320, Height = 240 });
            return session;
        }

        static uint SelectedStreamId(ComboBox streams)
        {
            var item = streams.SelectedItem as StreamDescriptor;
            return item == null ? 0 : item.Id;
        }

        static bool Select(TreeView tree, string name)
        {
            TreeNode node = FindNode(tree.Nodes, name);
            if (node == null)
                return false;
            tree.SelectedNode = node;
            return tree.SelectedNode == node;
        }

        static TreeNode FindNode(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == name)
                    return node;
                TreeNode child = FindNode(node.Nodes, name);
                if (child != null)
                    return child;
            }
            return null;
        }

        static NumericUpDown FirstNumeric(Control root)
        {
            var all = Numerics(root);
            return all.Count == 0 ? null : all[0];
        }

        static List<NumericUpDown> Numerics(Control root)
        {
            var list = new List<NumericUpDown>();
            if (root == null)
                return list;
            var box = root as NumericUpDown;
            if (box != null)
                list.Add(box);
            foreach (Control child in root.Controls)
                list.AddRange(Numerics(child));
            return list;
        }

        static void RunSection(string name, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Fail(name + " 抛出 " + ex.GetType().Name + "：" + ex.Message);
            }
        }

        static T FindType<T>(Control root) where T : Control
        {
            if (root == null)
                return null;
            var match = root as T;
            if (match != null)
                return match;
            foreach (Control child in root.Controls)
            {
                T found = FindType<T>(child);
                if (found != null)
                    return found;
            }
            return null;
        }

        static void ShowHidden(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(-3200, -3200);
            form.Opacity = 0;
            form.ShowInTaskbar = false;
            form.Show();
            Application.DoEvents();
        }

        static Control FindDock(Control root, string name, DockStyle dock)
        {
            if (root == null)
                return null;
            if (root.Name == name && root.Dock == dock)
                return root;
            foreach (Control child in root.Controls)
            {
                Control found = FindDock(child, name, dock);
                if (found != null)
                    return found;
            }
            return null;
        }

        static Control Find(Control root, string name)
        {
            if (root == null)
                return null;
            if (root.Name == name)
                return root;
            foreach (Control child in root.Controls)
            {
                Control found = Find(child, name);
                if (found != null)
                    return found;
            }
            return null;
        }

        static string Bounds(Control control)
        {
            if (control == null)
                return "null";
            return control.Name + " " + control.Bounds.ToString();
        }

        static void Expect(bool condition, string name)
        {
            if (condition)
                Pass(name);
            else
                Failures.Add(name);
        }

        static void Pass(string name)
        {
            Passed++;
            Lines.Add("PASS " + name);
        }

        static void Fail(string name)
        {
            Failures.Add(name);
            Lines.Add("FAIL " + name);
        }
    }
}
