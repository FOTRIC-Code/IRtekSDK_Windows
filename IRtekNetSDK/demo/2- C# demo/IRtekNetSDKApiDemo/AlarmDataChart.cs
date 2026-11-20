using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json.Linq;

namespace IRtekNetSDKApiDemo
{
    internal static class AlarmTime
    {
        public static DateTime FromUnixUs(ulong timestampUs)
        {
            if (timestampUs == 0)
                return DateTime.MinValue;
            try
            {
                long ms = (long)(timestampUs / 1000UL);
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static ulong ToUnixUs(DateTime time)
        {
            try
            {
                long ms = new DateTimeOffset(DateTime.SpecifyKind(time, DateTimeKind.Local)).ToUnixTimeMilliseconds();
                if (ms < 0)
                    return 0;
                return (ulong)ms * 1000UL;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    internal sealed class SafeChart : Chart
    {
        public SafeChart()
        {
            Size = new Size(240, 160);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (Width < 1 || Height < 1)
                return;
            base.OnSizeChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (Width < 1 || Height < 1)
                return;
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 1 || Height < 1)
                return;
            base.OnPaint(e);
        }
    }

    internal static class AlarmDataChart
    {
        const int TypeIntrusion = 2;
        const int TypeHighTemp = 3;
        const int TypeTempDetect = 4;

        static readonly string[] TimeKeys = { "t_us", "timestamp_us", "timestamp", "time", "ts", "t" };

        public static bool TryBind(string jsonPath, Chart chart, out string status)
        {
            return TryBind(jsonPath, chart, 0, out status);
        }

        public static bool TryBind(string jsonPath, Chart chart, int alarmType, out string status)
        {
            status = "";
            if (chart == null)
            {
                status = "图表控件无效";
                return false;
            }
            if (chart.Width < 1 || chart.Height < 1)
            {
                status = "曲线区域尚未就绪";
                return false;
            }
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();
            chart.Titles.Clear();
            if (string.IsNullOrWhiteSpace(jsonPath) || !File.Exists(jsonPath))
            {
                status = "没有 data.json";
                return false;
            }

            JToken root;
            try
            {
                root = JToken.Parse(File.ReadAllText(jsonPath));
            }
            catch (Exception ex)
            {
                status = "解析 JSON 失败：" + ex.Message;
                return false;
            }

            int type = ReadAlarmType(root, alarmType);
            JArray samples = FindSamples(root);
            if (samples == null || samples.Count == 0)
            {
                status = "data.json 里没有 samples";
                return false;
            }

            string yKey;
            string seriesName;
            string yTitle;
            ResolveSeries(type, samples, out yKey, out seriesName, out yTitle);
            if (string.IsNullOrEmpty(yKey))
            {
                status = AlarmHub.TypeName(type) + " 的元数据没有可绘制字段";
                return false;
            }

            var points = new List<DataPoint>();
            DateTime firstTime = DateTime.MinValue;
            DateTime lastTime = DateTime.MinValue;
            ulong firstUs = 0;
            ulong lastUs = 0;
            int skipped = 0;
            for (int i = 0; i < samples.Count; i++)
            {
                JObject row = samples[i] as JObject;
                if (row == null)
                {
                    skipped++;
                    continue;
                }

                ulong timestampUs;
                DateTime time;
                if (!TrySampleTime(row, out timestampUs, out time))
                {
                    skipped++;
                    continue;
                }

                double y;
                if (!TryNumber(row[yKey], out y))
                {
                    skipped++;
                    continue;
                }
                if (string.Equals(yKey, "motion_level", StringComparison.OrdinalIgnoreCase) && y < 0)
                {
                    skipped++;
                    continue;
                }

                points.Add(new DataPoint(time.ToOADate(), y));
                if (firstTime == DateTime.MinValue)
                {
                    firstTime = time;
                    firstUs = timestampUs;
                }
                lastTime = time;
                lastUs = timestampUs;
            }

            if (points.Count < 2)
            {
                status = "有效采样不足，无法画曲线";
                return false;
            }

            var area = new ChartArea("main");
            area.AxisX.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            area.AxisX.Title = "采样时间";
            area.AxisY.Title = yTitle;
            area.AxisX.LabelStyle.Format = "HH:mm:ss";
            area.AxisX.LabelStyle.Angle = -30;
            area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            area.AxisX.IntervalType = DateTimeIntervalType.Seconds;
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend("legend") { Docking = Docking.Top });

            var line = new Series(seriesName)
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                ChartArea = "main",
                Legend = "legend",
                XValueType = ChartValueType.DateTime,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 4,
                ToolTip = "时间 #VALX{yyyy-MM-dd HH:mm:ss.fff}\n" + seriesName + " #VALY{0.##}"
            };
            List<DataPoint> drawnPoints = Downsample(points, 4000);
            foreach (DataPoint point in drawnPoints)
                line.Points.Add(new DataPoint(point.XValue, point.YValues[0]));
            chart.Series.Add(line);

            string typeName = AlarmHub.TypeName(type);
            status = typeName + "：Y 轴为" + yTitle + "，共 " + points.Count + " 点"
                + (skipped > 0 ? "，跳过 " + skipped + " 个空值" : "")
                + "。" + Environment.NewLine
                + "X 轴来自 samples.t_us（微秒 Unix 时间戳，已转本地时间）："
                + firstTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                + " ~ "
                + lastTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                + "  t_us=" + firstUs.ToString(CultureInfo.InvariantCulture)
                + " ~ " + lastUs.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        static int ReadAlarmType(JToken root, int fallback)
        {
            JObject obj = root as JObject;
            if (obj == null)
                return fallback;
            double value;
            if (TryNumber(obj["type"], out value) || TryNumber(obj["alarm_type"], out value))
                return (int)value;
            return fallback;
        }

        static JArray FindSamples(JToken root)
        {
            if (root is JArray array)
                return array;
            JObject obj = root as JObject;
            if (obj == null)
                return null;
            JArray samples = obj["samples"] as JArray;
            if (samples != null)
                return samples;
            return obj["data"] as JArray;
        }

        static void ResolveSeries(int type, JArray samples, out string yKey, out string seriesName, out string yTitle)
        {
            yKey = "";
            seriesName = "";
            yTitle = "";
            if (type == TypeIntrusion || (type == 0 && HasField(samples, "motion_level")))
            {
                yKey = "motion_level";
                seriesName = "移动侦测";
                yTitle = "移动侦测值";
                return;
            }
            if (type == TypeHighTemp || type == TypeTempDetect || HasField(samples, "max"))
            {
                yKey = "max";
                seriesName = "最高温";
                yTitle = "最高温";
            }
        }

        static bool HasField(JArray samples, string name)
        {
            int probe = Math.Min(samples.Count, 8);
            for (int i = 0; i < probe; i++)
            {
                JObject row = samples[i] as JObject;
                if (row != null && row[name] != null && row[name].Type != JTokenType.Null)
                    return true;
            }
            return false;
        }

        static bool TrySampleTime(JObject row, out ulong timestampUs, out DateTime time)
        {
            timestampUs = 0;
            time = DateTime.MinValue;
            foreach (string key in TimeKeys)
            {
                JToken token = row[key];
                if (token == null || token.Type == JTokenType.Null)
                    continue;
                if (TryNumber(token, out double number))
                {
                    time = FromNumericTime(number);
                    if (time == DateTime.MinValue)
                        continue;
                    if (number >= 1e14)
                        timestampUs = (ulong)number;
                    else if (number >= 1e11)
                        timestampUs = (ulong)(number * 1000.0);
                    else
                        timestampUs = (ulong)(number * 1000000.0);
                    return true;
                }
                if (token.Type == JTokenType.Date)
                {
                    time = ToLocal((DateTime)token);
                    timestampUs = AlarmTime.ToUnixUs(time);
                    return true;
                }
                if (token.Type == JTokenType.String)
                {
                    DateTime parsed;
                    if (DateTime.TryParse((string)token, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out parsed))
                    {
                        time = ToLocal(parsed);
                        timestampUs = AlarmTime.ToUnixUs(time);
                        return true;
                    }
                }
            }
            time = DateTime.MinValue;
            timestampUs = 0;
            return false;
        }

        static DateTime FromNumericTime(double value)
        {
            try
            {
                if (value > 1e14)
                    return DateTimeOffset.FromUnixTimeMilliseconds((long)(value / 1000.0)).LocalDateTime;
                if (value > 1e12)
                    return DateTimeOffset.FromUnixTimeMilliseconds((long)value).LocalDateTime;
                if (value > 1e9)
                    return DateTimeOffset.FromUnixTimeSeconds((long)value).LocalDateTime;
            }
            catch (Exception)
            {
            }
            return DateTime.MinValue;
        }

        static DateTime ToLocal(DateTime value)
        {
            if (value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(value, DateTimeKind.Local);
            return value.ToLocalTime();
        }

        static List<DataPoint> Downsample(List<DataPoint> points, int max)
        {
            if (points.Count <= max)
                return points;
            var output = new List<DataPoint>(max);
            double step = (points.Count - 1) / (double)(max - 1);
            for (int i = 0; i < max; i++)
                output.Add(points[Math.Min(points.Count - 1, (int)Math.Round(i * step))]);
            return output;
        }

        static bool TryNumber(JToken token, out double value)
        {
            value = 0;
            if (token == null)
                return false;
            if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
            {
                value = token.Value<double>();
                return !double.IsNaN(value) && !double.IsInfinity(value);
            }
            if (token.Type == JTokenType.String)
                return double.TryParse((string)token, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
            return false;
        }
    }

    internal sealed class AlarmWavePlay
    {
        const string BackSeriesName = "_wave_back";
        const string TitleName = "wave-play";

        struct Sample
        {
            public double X;
            public double Y;
        }

        readonly List<Sample> _points = new List<Sample>();
        Chart _chart;
        Series _played;
        AlarmVideoPlayer _player;
        Title _title;
        Color _savedColor;
        double _origin;
        int _frame = -1;
        Action<string> _status;

        public void Start(Chart chart, AlarmVideoPlayer player, Action<string> status)
        {
            if (chart == null || player == null || !player.IsActive)
                return;
            if (_chart == chart && _player == player && _points.Count > 0)
                return;

            Stop(true);
            if (!Capture(chart))
                return;

            _chart = chart;
            _player = player;
            _status = status;
            _player.SecondChanged += OnSecond;
            Apply(player.CurrentSecond);
        }

        public void Stop(bool restore)
        {
            if (_player != null)
            {
                _player.SecondChanged -= OnSecond;
                _player = null;
            }
            if (restore)
                Restore();
            _chart = null;
            _played = null;
            _title = null;
            _points.Clear();
            _frame = -1;
            _status = null;
        }

        bool Capture(Chart chart)
        {
            Series source = null;
            foreach (Series series in chart.Series)
            {
                if (series.Name == BackSeriesName)
                    continue;
                if (source == null || series.Points.Count > source.Points.Count)
                    source = series;
            }
            if (source == null || source.Points.Count == 0)
                return false;

            _points.Clear();
            foreach (DataPoint point in source.Points)
            {
                if (point.YValues == null || point.YValues.Length == 0)
                    continue;
                _points.Add(new Sample { X = point.XValue, Y = point.YValues[0] });
            }
            if (_points.Count == 0)
                return false;

            _origin = _points[0].X;
            _played = source;
            _savedColor = source.Color;
            source.MarkerStyle = MarkerStyle.None;
            if (source.Color.IsEmpty)
                source.Color = Color.FromArgb(0, 114, 178);

            if (chart.Series.IndexOf(BackSeriesName) < 0)
            {
                var back = new Series(BackSeriesName)
                {
                    ChartType = SeriesChartType.Line,
                    ChartArea = source.ChartArea,
                    XValueType = ChartValueType.DateTime,
                    Color = Color.FromArgb(170, 176, 176, 176),
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.None,
                    IsVisibleInLegend = false
                };
                foreach (Sample sample in _points)
                    back.Points.Add(new DataPoint(sample.X, sample.Y));
                chart.Series.Insert(0, back);
            }

            _title = chart.Titles.FindByName(TitleName);
            if (_title == null)
            {
                _title = new Title { Name = TitleName, Docking = Docking.Bottom, ForeColor = Color.DimGray };
                chart.Titles.Add(_title);
            }
            return true;
        }

        void OnSecond(int second)
        {
            if (_chart == null || _chart.IsDisposed || _played == null)
                return;
            if (_chart.Width < 2 || _chart.Height < 2)
                return;
            Apply(second);
        }

        void Apply(int second)
        {
            if (second < 0)
                second = 0;
            int frame = VisibleUntil(second);
            string text = "跟随视频 第 " + (second + 1) + " 秒";
            try
            {
                if (_title != null)
                    _title.Text = text;
                Action<string> status = _status;
                if (status != null)
                    status(text);
                if (frame < 0 || frame == _frame)
                    return;
                _frame = frame;
                _played.Points.Clear();
                for (int i = 0; i <= frame; i++)
                {
                    var point = new DataPoint(_points[i].X, _points[i].Y);
                    if (i == frame)
                    {
                        point.MarkerStyle = MarkerStyle.Circle;
                        point.MarkerSize = 8;
                        point.MarkerColor = Color.OrangeRed;
                        point.MarkerBorderColor = Color.White;
                        point.MarkerBorderWidth = 1;
                    }
                    _played.Points.Add(point);
                }
            }
            catch (Exception)
            {
            }
        }

        int VisibleUntil(int second)
        {
            double limit = _origin + (second + 1) / 86400.0 - 1e-9;
            int last = -1;
            for (int i = 0; i < _points.Count; i++)
            {
                if (_points[i].X <= limit)
                    last = i;
            }
            return last < 0 ? 0 : last;
        }

        void Restore()
        {
            if (_chart == null || _chart.IsDisposed || _played == null || _points.Count == 0)
                return;
            try
            {
                Series back = _chart.Series.FindByName(BackSeriesName);
                if (back != null)
                    _chart.Series.Remove(back);
                _played.Points.Clear();
                foreach (Sample sample in _points)
                    _played.Points.Add(new DataPoint(sample.X, sample.Y));
                _played.MarkerStyle = MarkerStyle.Circle;
                _played.MarkerSize = 4;
                _played.Color = _savedColor;
                if (_title != null && _chart.Titles.Contains(_title))
                    _chart.Titles.Remove(_title);
            }
            catch (Exception)
            {
            }
        }
    }
}
