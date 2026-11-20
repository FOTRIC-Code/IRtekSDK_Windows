using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal sealed partial class AlarmQueryPanel : UserControl
    {
        DeviceSession _session;
        AlarmVideoPlayer _player;
        readonly AlarmWavePlay _wave = new AlarmWavePlay();
        string _playingPath = "";
        string _pendingChartPath = "";
        string _chartBase = "";
        string _chartPlayLine = "";

        uint _pageNo;
        uint _total;
        bool _busy;
        bool _splitReady;
        const int WatchLimit = 20;

        ToolTip _hoverTip;
        Timer _hoverTimer;
        ListView _hoverList;
        ListViewItem _hoverItem;
        Point _hoverPoint;

        public AlarmQueryPanel()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            Enabled = false;
            _start.Value = DateTime.Now.AddDays(-7);
            _stop.Value = DateTime.Now;
            _localDir.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alarm_files");
            _chartBody.SizeChanged += (sender, e) => LayoutChart(_chartBody);
            SizeChanged += (sender, e) =>
            {
                FitHeader();
                FitPickBar();
                if (!_splitReady && _body.Width > 200 && _media.Width > 200 && _side.Height > 160)
                {
                    _body.SplitterDistance = Math.Max(240, _body.Width * 42 / 100);
                    _media.SplitterDistance = Math.Max(220, _media.Width * 55 / 100);
                    _side.SplitterDistance = Math.Max(120, _side.Height * 48 / 100);
                    _splitReady = true;
                }
                LayoutChart(_chartBody);
            };

            _hoverTip = new ToolTip(components);
            _hoverTip.ShowAlways = true;
            _hoverTip.UseAnimation = false;
            _hoverTip.UseFading = false;
            _hoverTimer = new Timer(components) { Interval = 400 };
            _hoverTimer.Tick += OnHoverTick;
            AttachHover(_list);
            var buttonTip = new ToolTip(components);
            buttonTip.SetToolTip(_watchCount, "这一次同时播放几条，最多 20 条。勾选了就播勾选的，没勾选就从点中的那条往后数。");
            buttonTip.SetToolTip(_watchPick, "按一次播放的条数，同时看视频、截图和曲线。");
            buttonTip.SetToolTip(_closeWatch, "关闭多条一起观看的画面，回到单条预览。");

            _query.Click += async (sender, e) => await QueryAsync(0);
            _prev.Click += async (sender, e) => await QueryAsync(_pageNo == 0 ? 0 : _pageNo - 1);
            _next.Click += async (sender, e) => await QueryAsync(_pageNo + 1);
            _browse.Click += (sender, e) => BrowseDir();
            _download.Click += async (sender, e) => await DownloadSelectedAsync();
            _openDir.Click += (sender, e) => OpenDir();
            _watchPick.Click += async (sender, e) => await WatchPickedAsync();
            _closeWatch.Click += (sender, e) => CloseWatch();
            _list.DoubleClick += async (sender, e) => await DownloadSelectedAsync();
            _list.SelectedIndexChanged += (sender, e) => ShowCachedPreview();
            Load += (sender, e) =>
            {
                FitHeader();
                EnsurePlayer();
            };
            Disposed += (sender, e) => ClearPreview();
        }

        bool _fitting;

        void FitHeader()
        {
            if (_fitting || IsDisposed)
                return;
            _fitting = true;
            try
            {
                FitSection(_filterWrap, _filterBar);
                FitSection(_sftpWrap, _sftpBar);
            }
            finally
            {
                _fitting = false;
            }
        }

        static void FitSection(Panel wrap, FlowLayoutPanel bar)
        {
            if (wrap == null || bar == null)
                return;
            int width = wrap.ClientSize.Width - wrap.Padding.Horizontal;
            if (width < 80)
                return;
            Size preferred = bar.GetPreferredSize(new Size(width, 0));
            int barHeight = Math.Max(32, preferred.Height);
            if (bar.Height != barHeight)
                bar.Height = barHeight;
            int wrapHeight = wrap.Padding.Vertical + 20 + barHeight;
            if (wrap.Height != wrapHeight)
                wrap.Height = wrapHeight;
        }

        void FitPickBar()
        {
            if (_fitting || _pickBar == null || _watchWrap == null || _pickHint == null)
                return;
            int width = _watchWrap.ClientSize.Width;
            if (width < 40)
                return;
            _fitting = true;
            try
            {
                Size preferred = _pickBar.GetPreferredSize(new Size(width, 0));
                int barHeight = Math.Max(32, preferred.Height);
                if (_pickBar.Height != barHeight)
                    _pickBar.Height = barHeight;
                int wrapHeight = barHeight + _pickHint.Height;
                if (_watchWrap.Height != wrapHeight)
                    _watchWrap.Height = wrapHeight;
            }
            finally
            {
                _fitting = false;
            }
        }

        public void Bind(DeviceSession session)
        {
            _session = session;
            if (session == null)
            {
                Unbind();
                return;
            }
            _sftpHost.Text = session.Ip;
            _sftpUser.Text = string.IsNullOrWhiteSpace(session.UserName) ? "root" : session.UserName;
            _sftpPassword.Text = string.IsNullOrEmpty(session.Password) && _sftpUser.Text == "root" ? "root" : session.Password;
            Enabled = true;
        }

        public void Unbind()
        {
            ClearPreview();
            CancelHover();
            _list.Items.Clear();
            _session = null;
            Enabled = false;
        }

        void LayoutChart(Control host)
        {
            if (host == null)
                return;
            int width = host.ClientSize.Width;
            int height = host.ClientSize.Height;
            if (width < 1 || height < 1)
                return;
            if (_chart.Parent != host)
                host.Controls.Add(_chart);
            _chart.Visible = true;
            _chart.SetBounds(0, 0, width, height);
            if (!string.IsNullOrEmpty(_pendingChartPath))
            {
                string path = _pendingChartPath;
                _pendingChartPath = "";
                BindChart(path);
            }
        }

        async Task QueryAsync(uint pageNo)
        {
            if (_busy || _session == null || _session.Handle == IntPtr.Zero)
                return;
            _busy = true;
            SetBusy(true);
            var param = new NetSearchAlarmParam
            {
                AlarmType = SelectedType(),
                ChannelId = (uint)_channel.Value,
                StartTimestampUs = _start.Checked ? AlarmTime.ToUnixUs(_start.Value) : 0UL,
                StopTimestampUs = _stop.Checked ? AlarmTime.ToUnixUs(_stop.Value) : 0UL,
                PageNo = pageNo,
                PageSize = (uint)_pageSize.Value
            };
            int capacity = (int)_pageSize.Value;
            IntPtr handle = _session.Handle;
            try
            {
                var outcome = await Task.Run(() =>
                {
                    NetSearchPage foundPage;
                    AlarmSearchItem[] foundItems;
                    int found = NetSdk.FindAlarmDatas(handle, param, capacity, out foundPage, out foundItems);
                    return Tuple.Create(found, foundPage, foundItems);
                });
                if (IsDisposed)
                    return;
                int code = outcome.Item1;
                NetSearchPage pageInfo = outcome.Item2;
                AlarmSearchItem[] rows = outcome.Item3 ?? new AlarmSearchItem[0];
                if (code != NetSdk.Ok)
                {
                    MessageBox.Show(this, "查询失败：" + NetSdk.ErrorText(code), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _pageNo = pageInfo.PageNo;
                _total = pageInfo.Total;
                FillList(rows);
                uint pages = _total == 0 ? 1 : (uint)Math.Ceiling(_total / (double)_pageSize.Value);
                _pageStatus.Text = string.Format(CultureInfo.InvariantCulture,
                    "第 {0}/{1} 页，本页 {2} 条，共 {3} 条",
                    _pageNo + 1, Math.Max(pages, 1), rows.Length, _total);
                _prev.Enabled = _pageNo > 0;
                _next.Enabled = (_pageNo + 1) * (uint)_pageSize.Value < _total;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "查询异常：" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _busy = false;
                SetBusy(false);
            }
        }

        void FillList(AlarmSearchItem[] items)
        {
            CancelHover();
            _list.BeginUpdate();
            _list.Items.Clear();
            foreach (AlarmSearchItem item in items)
            {
                DateTime time = AlarmTime.FromUnixUs(item.TimestampUs);
                string clock = time == DateTime.MinValue ? item.TimestampUs.ToString(CultureInfo.InvariantCulture) : time.ToString("yyyy-MM-dd HH:mm:ss");
                var row = new ListViewItem(clock);
                row.SubItems.Add(AlarmHub.TypeName((int)item.AlarmType));
                row.SubItems.Add(item.AlarmId.ToString(CultureInfo.InvariantCulture));
                row.SubItems.Add(item.ChannelId.ToString(CultureInfo.InvariantCulture));
                row.SubItems.Add(item.Detail);
                row.SubItems.Add(item.RecordPath);
                row.SubItems.Add(item.SnapshotPath);
                row.SubItems.Add(item.DataPath);
                row.Tag = item;
                _list.Items.Add(row);
            }
            _list.EndUpdate();
            if (_list.Items.Count > 0)
                _list.Items[0].Selected = true;
        }

        async Task DownloadSelectedAsync()
        {
            AlarmSearchItem item = SelectedItem();
            if (item == null)
            {
                MessageBox.Show(this, "请先选中一条报警记录。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(item.RecordPath) &&
                string.IsNullOrWhiteSpace(item.SnapshotPath) &&
                string.IsNullOrWhiteSpace(item.DataPath))
            {
                MessageBox.Show(this, "这条记录没有录像、截图或数据文件路径。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string host = _sftpHost.Text.Trim();
            int port = (int)_sftpPort.Value;
            string user = _sftpUser.Text.Trim();
            string password = _sftpPassword.Text;
            string root = LocalRoot();
            _busy = true;
            SetBusy(true);
            try
            {
                AlarmDownloadResult result = await Task.Run(() =>
                    AlarmFileClient.Download(host, port, user, password, item, root));
                if (IsDisposed)
                    return;
                ShowFiles(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "SFTP 下载失败：" + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _busy = false;
                SetBusy(false);
            }
        }

        void ShowFiles(AlarmDownloadResult result)
        {
            ShowSingleMedia();
            BindChart(result.DataJsonPath);
            ShowImage(result.SnapshotPath);
            if (!string.IsNullOrWhiteSpace(result.RecordPath) && File.Exists(result.RecordPath))
            {
                PlayVideo(result.RecordPath);
                return;
            }
            StopVideo();
            _videoStatus.Text = "没有录像文件";
        }

        void ShowCachedPreview()
        {
            if (_watchBoard != null && _watchBoard.Visible)
                return;
            AlarmSearchItem item = SelectedItem();
            if (item == null)
                return;
            string folder = AlarmFileClient.LocalFolder(LocalRoot(), item);
            if (!Directory.Exists(folder))
            {
                StopVideo();
                return;
            }
            BindChart(AlarmFileClient.FindLocalJson(folder));
            ShowImage(AlarmFileClient.FindLocalImage(folder));
            PlayVideo(AlarmFileClient.FindLocalVideo(folder));
        }

        void BindChart(string jsonPath)
        {
            string status;
            if (_chart.Width < 1 || _chart.Height < 1)
            {
                _pendingChartPath = jsonPath ?? "";
                _chartStatus.Text = "等待曲线区域就绪";
                return;
            }
            _pendingChartPath = "";
            _wave.Stop(false);
            int alarmType = 0;
            AlarmSearchItem selected = SelectedItem();
            if (selected != null)
                alarmType = (int)selected.AlarmType;
            if (!AlarmDataChart.TryBind(jsonPath, _chart, alarmType, out status))
            {
                try
                {
                    _chart.Series.Clear();
                    _chart.ChartAreas.Clear();
                }
                catch (Exception)
                {
                }
                _chartBase = status;
                _chartPlayLine = "";
                ShowChartStatus();
                return;
            }
            _chartBase = status;
            _chartPlayLine = "";
            ShowChartStatus();
            FollowWave();
        }

        void ShowImage(string path)
        {
            Image old = _preview.Image;
            _preview.Image = null;
            if (old != null)
                old.Dispose();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;
            using (Image loaded = Image.FromFile(path))
                _preview.Image = new Bitmap(loaded);
        }

        void PlayVideo(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                StopVideo();
                _videoStatus.Text = "没有录像文件";
                return;
            }
            if (string.Equals(_playingPath, path, StringComparison.OrdinalIgnoreCase) && _player != null)
                return;

            EnsurePlayer();
            if (_player == null)
            {
                _videoStatus.Text = "当前页面无法创建播放器";
                return;
            }
            try
            {
                if (_player.PlayFile(path))
                {
                    _playingPath = path;
                    _videoStatus.Text = "正在播放 " + Path.GetFileName(path);
                    FollowWave();
                    return;
                }
            }
            catch (Exception ex)
            {
                _videoStatus.Text = "播放失败：" + ex.Message;
                return;
            }
            _videoStatus.Text = "播放失败：" + Path.GetFileName(path);
        }

        void StopVideo()
        {
            _playingPath = "";
            _wave.Stop(true);
            _chartPlayLine = "";
            ShowChartStatus();
            if (_player != null)
                _player.StopPlay();
        }

        void FollowWave()
        {
            if (string.IsNullOrEmpty(_playingPath) || _player == null || !_player.IsActive || _chart.Series.Count == 0)
                return;
            _wave.Start(_chart, _player, ShowChartPlayLine);
        }

        void ShowChartPlayLine(string line)
        {
            if (IsDisposed)
                return;
            _chartPlayLine = line ?? "";
            ShowChartStatus();
        }

        void ShowChartStatus()
        {
            if (string.IsNullOrEmpty(_chartBase))
            {
                _chartStatus.Text = _chartPlayLine ?? "";
                return;
            }
            if (string.IsNullOrEmpty(_chartPlayLine))
            {
                _chartStatus.Text = _chartBase;
                return;
            }
            string first = _chartBase;
            int cut = first.IndexOf('\r');
            if (cut < 0)
                cut = first.IndexOf('\n');
            if (cut > 0)
                first = first.Substring(0, cut);
            _chartStatus.Text = _chartPlayLine + Environment.NewLine + first;
        }

        void EnsurePlayer()
        {
            if (_player != null)
                return;
            try
            {
                _player = new AlarmVideoPlayer { Dock = DockStyle.Fill };
                _player.StatusChanged += text =>
                {
                    if (!IsDisposed)
                        _videoStatus.Text = text;
                };
                _videoHost.Controls.Add(_player);
            }
            catch (Exception ex)
            {
                _player = null;
                _videoStatus.Text = "无法创建播放器：" + ex.Message;
            }
        }

        void ClearPreview()
        {
            StopVideo();
            Image old = _preview.Image;
            _preview.Image = null;
            if (old != null)
                old.Dispose();
            if (_player != null)
            {
                _videoHost.Controls.Remove(_player);
                _player.Dispose();
                _player = null;
            }
            if (_watchBoard != null)
                _watchBoard.ClearItems();
        }

        AlarmSearchItem SelectedItem()
        {
            if (_list.SelectedItems.Count == 0)
                return null;
            return _list.SelectedItems[0].Tag as AlarmSearchItem;
        }

        uint SelectedType()
        {
            EnumChoice choice = _type.SelectedItem as EnumChoice;
            return choice == null ? 0u : (uint)choice.Value;
        }

        void BrowseDir()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "选择报警文件保存目录";
                dialog.SelectedPath = Directory.Exists(_localDir.Text) ? _localDir.Text : AppDomain.CurrentDomain.BaseDirectory;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    _localDir.Text = dialog.SelectedPath;
            }
        }

        void OpenDir()
        {
            string path = _localDir.Text.Trim();
            if (string.IsNullOrWhiteSpace(path))
                return;
            Directory.CreateDirectory(path);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        void SetBusy(bool busy)
        {
            _query.Enabled = !busy;
            _prev.Enabled = !busy && _pageNo > 0;
            _next.Enabled = !busy && (_pageNo + 1) * (uint)_pageSize.Value < _total;
            _download.Enabled = !busy;
            _watchCount.Enabled = !busy;
            _watchPick.Enabled = !busy;
            _closeWatch.Enabled = !busy;
            UseWaitCursor = busy;
        }

        string LocalRoot()
        {
            string root = _localDir.Text.Trim();
            if (string.IsNullOrWhiteSpace(root))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alarm_files");
            return root;
        }

        void AttachHover(ListView list)
        {
            list.MouseMove += ListMouseMove;
            list.MouseLeave += ListMouseLeave;
            list.MouseWheel += (sender, e) => CancelHover();
        }

        void ListMouseMove(object sender, MouseEventArgs e)
        {
            var list = sender as ListView;
            if (list == null)
                return;
            ListViewHitTestInfo hit = list.HitTest(e.Location);
            if (hit.Item == null || !(hit.Item.Tag is AlarmSearchItem))
            {
                if (list == _hoverList)
                    CancelHover();
                return;
            }
            if (list == _hoverList && hit.Item == _hoverItem)
                return;
            _hoverList = list;
            _hoverItem = hit.Item;
            _hoverPoint = e.Location;
            _hoverTip.Hide(this);
            _hoverTimer.Stop();
            _hoverTimer.Start();
        }

        void ListMouseLeave(object sender, EventArgs e)
        {
            var list = sender as ListView;
            if (list == null || list != _hoverList)
                return;
            Point client = list.PointToClient(Cursor.Position);
            if (list.ClientRectangle.Contains(client))
                return;
            CancelHover();
        }

        void OnHoverTick(object sender, EventArgs e)
        {
            _hoverTimer.Stop();
            if (IsDisposed || _hoverItem == null || _hoverList == null || _hoverItem.ListView != _hoverList)
                return;
            var item = _hoverItem.Tag as AlarmSearchItem;
            if (item == null)
                return;
            string text = FormatAlarmTip(item);
            if (string.IsNullOrEmpty(text))
                return;
            Point screen = _hoverList.PointToScreen(_hoverPoint);
            Point client = PointToClient(screen);
            _hoverTip.Show(text, this, client.X + 18, client.Y + 22, 20000);
        }

        void CancelHover()
        {
            if (_hoverTimer != null)
                _hoverTimer.Stop();
            _hoverList = null;
            _hoverItem = null;
            if (_hoverTip != null && !IsDisposed)
                _hoverTip.Hide(this);
        }

        public static string FormatAlarmTip(AlarmSearchItem item)
        {
            if (item == null)
                return "";
            DateTime time = AlarmTime.FromUnixUs(item.TimestampUs);
            string clock = time == DateTime.MinValue
                ? item.TimestampUs.ToString(CultureInfo.InvariantCulture)
                : time.ToString("yyyy-MM-dd HH:mm:ss");
            string detail = string.IsNullOrWhiteSpace(item.Detail) ? "（无内容）" : item.Detail.Trim();
            string text = "内容：" + detail
                + "\r\n时间：" + clock
                + "\r\n类型：" + AlarmHub.TypeName((int)item.AlarmType)
                + "\r\nID：" + item.AlarmId.ToString(CultureInfo.InvariantCulture)
                + "\r\n通道：" + item.ChannelId.ToString(CultureInfo.InvariantCulture)
                + "\r\n录像：" + ShowPath(item.RecordPath)
                + "\r\n截图：" + ShowPath(item.SnapshotPath)
                + "\r\n数据：" + ShowPath(item.DataPath);
            if (text.Length > 900)
                text = text.Substring(0, 900) + "…";
            return text;
        }

        static string ShowPath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? "（无）" : path.Trim();
        }

        void SetHint(string text, bool warn)
        {
            _pickHint.ForeColor = warn ? Color.Firebrick : SystemColors.ControlText;
            _pickHint.Text = text ?? "";
        }

        void CloseWatch()
        {
            ShowSingleMedia();
            ShowCachedPreview();
            SetHint("已关闭一同观看。", false);
        }

        int WatchCount()
        {
            int count = (int)_watchCount.Value;
            if (count < 1)
                return 1;
            if (count > WatchLimit)
                return WatchLimit;
            return count;
        }

        List<AlarmSearchItem> ItemsToWatch()
        {
            int count = WatchCount();
            var items = new List<AlarmSearchItem>();
            if (_list.CheckedItems.Count > 0)
            {
                var rows = new List<ListViewItem>();
                foreach (ListViewItem row in _list.CheckedItems)
                    rows.Add(row);
                rows.Sort((a, b) => a.Index.CompareTo(b.Index));
                int take = Math.Min(count, rows.Count);
                for (int i = 0; i < take; i++)
                {
                    var item = rows[i].Tag as AlarmSearchItem;
                    if (item != null)
                        items.Add(item);
                }
                return items;
            }
            if (_list.Items.Count == 0)
                return items;
            int start = 0;
            if (_list.FocusedItem != null)
                start = _list.FocusedItem.Index;
            else if (_list.SelectedItems.Count > 0)
                start = _list.SelectedItems[0].Index;
            for (int i = start; i < _list.Items.Count && items.Count < count; i++)
            {
                var item = _list.Items[i].Tag as AlarmSearchItem;
                if (item != null)
                    items.Add(item);
            }
            return items;
        }

        void MarkWatchRows(IList<AlarmSearchItem> items)
        {
            _list.BeginUpdate();
            try
            {
                foreach (ListViewItem row in _list.Items)
                    row.Selected = false;
                ListViewItem first = null;
                foreach (ListViewItem row in _list.Items)
                {
                    var item = row.Tag as AlarmSearchItem;
                    if (item == null || !Listed(items, item))
                        continue;
                    row.Selected = true;
                    if (first == null)
                        first = row;
                }
                if (first != null)
                {
                    first.Focused = true;
                    first.EnsureVisible();
                }
            }
            finally
            {
                _list.EndUpdate();
            }
        }

        static bool Listed(IList<AlarmSearchItem> items, AlarmSearchItem item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                AlarmSearchItem have = items[i];
                if (have != null && have.AlarmId == item.AlarmId && have.ChannelId == item.ChannelId && have.TimestampUs == item.TimestampUs)
                    return true;
            }
            return false;
        }

        internal List<AlarmSearchItem> ItemsToWatchForCheck()
        {
            return ItemsToWatch();
        }

        async Task WatchPickedAsync()
        {
            if (_busy)
                return;
            int checkedCount = _list.CheckedItems.Count;
            List<AlarmSearchItem> items = ItemsToWatch();
            if (items.Count == 0)
            {
                SetHint("请先查询出报警，再选择一次播放多少条。", true);
                return;
            }

            string host = _sftpHost.Text.Trim();
            int port = (int)_sftpPort.Value;
            string user = _sftpUser.Text.Trim();
            string password = _sftpPassword.Text;
            string root = LocalRoot();
            ShowWatchBoard(items);
            MarkWatchRows(items);
            _busy = true;
            SetBusy(true);
            int failed = 0;
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (IsDisposed || _watchBoard.CardCount <= i)
                        return;
                    AlarmSearchItem item = items[i];
                    AlarmWatchCard card = _watchBoard.CardAt(i);
                    if (card.IsDisposed)
                        return;
                    SetHint("正在准备第 " + (i + 1) + "/" + items.Count + " 条…", false);
                    AlarmDownloadResult result;
                    try
                    {
                        if (!TryCached(item, root, out result))
                        {
                            if (string.IsNullOrWhiteSpace(item.RecordPath) &&
                                string.IsNullOrWhiteSpace(item.SnapshotPath) &&
                                string.IsNullOrWhiteSpace(item.DataPath))
                            {
                                card.SetStatus("没有录像、截图或数据文件");
                                failed++;
                                continue;
                            }
                            card.SetStatus("正在下载…");
                            AlarmSearchItem downloadItem = item;
                            result = await Task.Run(() =>
                                AlarmFileClient.Download(host, port, user, password, downloadItem, root));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!card.IsDisposed)
                            card.SetStatus("下载失败：" + ex.Message);
                        failed++;
                        continue;
                    }
                    if (IsDisposed || card.IsDisposed)
                        return;
                    try
                    {
                        card.Present(result, (int)item.AlarmType);
                    }
                    catch (Exception ex)
                    {
                        if (!card.IsDisposed)
                            card.SetStatus("打开失败：" + ex.Message);
                        failed++;
                    }
                }
                if (!IsDisposed)
                {
                    string done = failed == 0
                        ? "正在同时观看 " + items.Count + " 条报警的视频、截图和曲线。"
                        : "已打开 " + (items.Count - failed) + " 条，" + failed + " 条没有完整文件。";
                    if (checkedCount > items.Count)
                        done = "勾选了 " + checkedCount + " 条，按一次播放 " + items.Count + " 条。" + done;
                    SetHint(done, failed > 0);
                }
            }
            finally
            {
                _busy = false;
                if (!IsDisposed)
                    SetBusy(false);
            }
        }

        bool TryCached(AlarmSearchItem item, string root, out AlarmDownloadResult result)
        {
            result = new AlarmDownloadResult();
            string folder = AlarmFileClient.LocalFolder(root, item);
            result.LocalDir = folder;
            if (!Directory.Exists(folder))
                return false;
            result.DataJsonPath = AlarmFileClient.FindLocalJson(folder);
            result.SnapshotPath = AlarmFileClient.FindLocalImage(folder);
            result.RecordPath = AlarmFileClient.FindLocalVideo(folder);
            bool needRecord = !string.IsNullOrWhiteSpace(item.RecordPath);
            bool needShot = !string.IsNullOrWhiteSpace(item.SnapshotPath);
            bool needData = !string.IsNullOrWhiteSpace(item.DataPath);
            if (!needRecord && !needShot && !needData)
                return false;
            if (needRecord && string.IsNullOrEmpty(result.RecordPath))
                return false;
            if (needShot && string.IsNullOrEmpty(result.SnapshotPath))
                return false;
            if (needData && string.IsNullOrEmpty(result.DataJsonPath))
                return false;
            return true;
        }

        internal int ShowWatchForCheck(IList<AlarmSearchItem> items)
        {
            ShowWatchBoard(items);
            _watchBoard.LayoutCards();
            return _watchBoard.CardCount;
        }

        void ShowWatchBoard(IList<AlarmSearchItem> items)
        {
            StopVideo();
            _media.Visible = false;
            _watchBoard.Visible = true;
            _watchBoard.BringToFront();
            _watchBoard.ShowItems(items);
        }

        void ShowSingleMedia()
        {
            if (_watchBoard != null)
            {
                _watchBoard.ClearItems();
                _watchBoard.Visible = false;
            }
            if (_media != null)
            {
                _media.Visible = true;
                _media.BringToFront();
            }
        }

    }
}
