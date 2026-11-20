using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace IRtekNetSDKApiDemo
{
    internal sealed class AlarmWatchBoard : Panel
    {
        readonly List<AlarmWatchCard> _cards = new List<AlarmWatchCard>();
        bool _laying;

        public AlarmWatchBoard()
        {
            AutoScroll = true;
            BackColor = SystemColors.Control;
            Resize += (sender, e) => LayoutCards();
        }

        public int CardCount
        {
            get { return _cards.Count; }
        }

        public AlarmWatchCard CardAt(int index)
        {
            return _cards[index];
        }

        public void ShowItems(IList<AlarmSearchItem> items)
        {
            ClearItems();
            if (items == null)
                return;
            for (int i = 0; i < items.Count; i++)
            {
                var card = new AlarmWatchCard(items[i], i == 0);
                _cards.Add(card);
                Controls.Add(card);
            }
            LayoutCards();
        }

        public void ClearItems()
        {
            foreach (AlarmWatchCard card in _cards)
            {
                Controls.Remove(card);
                card.Dispose();
            }
            _cards.Clear();
            AutoScrollMinSize = Size.Empty;
        }

        public void LayoutCards()
        {
            if (_laying)
                return;
            _laying = true;
            try
            {
                int count = _cards.Count;
                if (count == 0)
                {
                    AutoScrollMinSize = Size.Empty;
                    return;
                }

                int gap = 8;
                int width = ClientSize.Width;
                if (width < 40)
                    return;
                if (VerticalScroll.Visible)
                    width = Math.Max(40, width - SystemInformation.VerticalScrollBarWidth);

                int cols = 1;
                if (count > 1 && width >= 860)
                    cols = 2;
                if (count > 2 && width >= 1280)
                    cols = 3;
                if (cols > count)
                    cols = count;
                int rows = (count + cols - 1) / cols;
                int cardW = Math.Max(280, (width - gap * (cols + 1)) / cols);
                int availH = Math.Max(1, ClientSize.Height);
                int cardH;
                if (rows <= 1)
                    cardH = Math.Max(280, availH - gap * 2);
                else if (rows == 2 && availH >= 640)
                    cardH = Math.Max(300, (availH - gap * 3) / 2);
                else
                    cardH = 340;

                for (int i = 0; i < count; i++)
                {
                    int col = i % cols;
                    int row = i / cols;
                    _cards[i].SetBounds(
                        gap + col * (cardW + gap),
                        gap + row * (cardH + gap),
                        cardW,
                        cardH);
                }
                AutoScrollMinSize = new Size(gap + cols * (cardW + gap), gap + rows * (cardH + gap));
            }
            finally
            {
                _laying = false;
            }
        }
    }

    internal sealed class AlarmWatchCard : UserControl
    {
        const int HeaderHeight = 26;
        const int VideoStatusHeight = 20;
        const int ChartStatusHeight = 36;

        readonly AlarmSearchItem _item;
        readonly Panel _header;
        readonly Label _title;
        readonly CheckBox _audio;
        readonly Label _videoStatus;
        readonly Label _chartStatus;
        readonly Panel _videoHost;
        readonly PictureBox _preview;
        readonly SafeChart _chart;
        readonly ToolTip _tip;
        AlarmVideoPlayer _player;
        readonly AlarmWavePlay _wave = new AlarmWavePlay();
        string _jsonPath = "";
        string _chartBase = "";
        int _alarmType;
        bool _wantChart;
        bool _chartBound;
        bool _laying;
        bool _disposed;

        public AlarmWatchCard(AlarmSearchItem item, bool audioOn)
        {
            _item = item;
            DoubleBuffered = true;
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = SystemColors.Control;
            Font = new Font("Microsoft YaHei UI", 9F);

            _tip = new ToolTip();
            _title = new Label
            {
                Dock = DockStyle.Fill,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 0, 0),
                Text = TitleOf(item)
            };
            _audio = new CheckBox
            {
                Dock = DockStyle.Right,
                Text = "声音",
                Width = 64,
                Checked = audioOn,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _header = new Panel
            {
                Dock = DockStyle.Top,
                Height = HeaderHeight
            };
            _header.Controls.Add(_title);
            _header.Controls.Add(_audio);

            _chartStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = ChartStatusHeight,
                Text = "等待曲线"
            };
            _videoStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = VideoStatusHeight,
                Text = "等待录像"
            };
            _videoHost = new Panel
            {
                BackColor = Color.Black
            };
            _preview = new PictureBox
            {
                BackColor = Color.Black,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            _chart = new SafeChart
            {
                AntiAliasing = AntiAliasingStyles.Graphics
            };

            Controls.Add(_videoHost);
            Controls.Add(_preview);
            Controls.Add(_chart);
            Controls.Add(_videoStatus);
            Controls.Add(_chartStatus);
            Controls.Add(_header);

            _tip.SetToolTip(_title, AlarmQueryPanel.FormatAlarmTip(item));
            Resize += (sender, e) => LayoutParts();

            try
            {
                _player = new AlarmVideoPlayer { Dock = DockStyle.Fill };
                _player.StatusChanged += text =>
                {
                    if (!_disposed)
                        _videoStatus.Text = text;
                };
                _videoHost.Controls.Add(_player);
            }
            catch (Exception ex)
            {
                _player = null;
                _videoStatus.Text = "无法创建播放器：" + ex.Message;
            }

            _audio.CheckedChanged += (sender, e) =>
            {
                if (_player != null)
                    _player.SetMuted(!_audio.Checked);
            };
            if (_player != null)
                _player.SetMuted(!_audio.Checked);
        }

        public void SetStatus(string text)
        {
            _videoStatus.Text = text ?? "";
        }

        public void Present(AlarmDownloadResult result, int alarmType)
        {
            _alarmType = alarmType;
            _jsonPath = result == null ? "" : (result.DataJsonPath ?? "");
            _wantChart = true;
            _chartBound = false;
            ShowImage(result == null ? "" : result.SnapshotPath);
            LayoutParts();

            string video = result == null ? "" : result.RecordPath;
            if (_player == null)
                _videoStatus.Text = "无法创建播放器";
            else if (string.IsNullOrWhiteSpace(video) || !File.Exists(video))
            {
                _wave.Stop(true);
                _player.StopPlay();
                _videoStatus.Text = "没有录像文件";
            }
            else
            {
                _player.SetMuted(!_audio.Checked);
                if (!_player.PlayFile(video))
                    _videoStatus.Text = "播放失败：" + Path.GetFileName(video);
            }
            BindChart();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _wave.Stop(false);
                if (_player != null)
                    _player.StopPlay();
                Image old = _preview.Image;
                _preview.Image = null;
                if (old != null)
                    old.Dispose();
                _tip.Dispose();
            }
            base.Dispose(disposing);
        }

        void LayoutParts()
        {
            if (_laying || _disposed)
                return;
            _laying = true;
            try
            {
                int top = HeaderHeight;
                int height = ClientSize.Height - HeaderHeight - VideoStatusHeight - ChartStatusHeight;
                int width = ClientSize.Width;
                if (width < 20 || height < 20)
                    return;
                int leftW = Math.Max(100, width * 56 / 100);
                if (leftW > width - 80)
                    leftW = width / 2;
                _videoHost.SetBounds(0, top, leftW, height);
                int imageH = Math.Max(48, height * 42 / 100);
                _preview.SetBounds(leftW, top, width - leftW, imageH);
                int chartH = Math.Max(1, height - imageH);
                _chart.SetBounds(leftW, top + imageH, Math.Max(1, width - leftW), chartH);
                if (_wantChart && !_chartBound)
                    BindChart();
            }
            finally
            {
                _laying = false;
            }
        }

        void BindChart()
        {
            if (_chart.Width < 2 || _chart.Height < 2)
            {
                _chartStatus.Text = "等待曲线区域就绪";
                return;
            }
            _wave.Stop(false);
            string status;
            if (!AlarmDataChart.TryBind(_jsonPath, _chart, _alarmType, out status))
            {
                _chartBase = status ?? "";
                _chartStatus.Text = FirstLine(status);
                _tip.SetToolTip(_chartStatus, status ?? "");
                _chartBound = true;
                return;
            }
            _chartBase = status ?? "";
            _chartStatus.Text = FirstLine(status);
            _tip.SetToolTip(_chartStatus, status ?? "");
            _chartBound = true;
            FollowWave();
        }

        void FollowWave()
        {
            if (_disposed || _player == null || !_player.IsActive || _chart.Series.Count == 0)
                return;
            _wave.Start(_chart, _player, text =>
            {
                if (_disposed)
                    return;
                _chartStatus.Text = text;
                _tip.SetToolTip(_chartStatus, string.IsNullOrEmpty(_chartBase) ? text : text + Environment.NewLine + _chartBase);
            });
        }

        static string FirstLine(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "";
            int cut = status.IndexOf('\r');
            if (cut < 0)
                cut = status.IndexOf('\n');
            if (cut > 0)
                return status.Substring(0, cut);
            return status;
        }

        void ShowImage(string path)
        {
            Image old = _preview.Image;
            _preview.Image = null;
            if (old != null)
                old.Dispose();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;
            try
            {
                using (Image loaded = Image.FromFile(path))
                    _preview.Image = new Bitmap(loaded);
            }
            catch (Exception)
            {
            }
        }

        static string TitleOf(AlarmSearchItem item)
        {
            if (item == null)
                return "";
            DateTime time = AlarmTime.FromUnixUs(item.TimestampUs);
            string clock = time == DateTime.MinValue
                ? item.TimestampUs.ToString(CultureInfo.InvariantCulture)
                : time.ToString("yyyy-MM-dd HH:mm:ss");
            return clock + "  " + AlarmHub.TypeName((int)item.AlarmType)
                + "  ID " + item.AlarmId.ToString(CultureInfo.InvariantCulture)
                + "  通道 " + item.ChannelId.ToString(CultureInfo.InvariantCulture);
        }
    }
}
