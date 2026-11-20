using System;
using System.Drawing;
using System.Windows.Forms;

namespace IRtekUVCSDKDemo
{
    internal sealed class ColorScaleBar : UserControl
    {
        internal const int BarWidth = 92;
        private const decimal TempMinimum = -273.1m;
        private const decimal TempMaximum = 5726.8m;
        private const decimal Gap = 0.1m;

        private static readonly Color[] DefaultColors = BuildDefaultColors();

        private readonly ComboBox _mode = new ComboBox();
        private readonly NumericUpDown _max = Box(40m);
        private readonly NumericUpDown _min = Box(20m);
        private readonly ScaleGraph _graph = new ScaleGraph();
        private readonly TableLayoutPanel _layout;
        private readonly RowStyle _modeRow = new RowStyle(SizeType.AutoSize);
        private bool _updating;

        internal event EventHandler ModeChanged;
        internal event EventHandler RangeChanged;

        internal bool IsAuto
        {
            get { return _mode.SelectedIndex == 1; }
        }

        internal float MinCelsius
        {
            get { return (float)_min.Value; }
        }

        internal float MaxCelsius
        {
            get { return (float)_max.Value; }
        }

        public ColorScaleBar()
        {
            BackColor = SystemColors.Window;
            Width = BarWidth;
            MinimumSize = new Size(BarWidth, 0);
            BorderStyle = BorderStyle.FixedSingle;
            _mode.DropDownStyle = ComboBoxStyle.DropDownList;
            _mode.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _mode.Margin = new Padding(0, 0, 0, 6);
            _mode.Items.Add("Fixed");
            _mode.Items.Add("Auto");
            _mode.SelectedIndex = 0;

            _layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Window,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(6, 8, 6, 8)
            };
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _layout.RowStyles.Add(_modeRow);
            _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            _layout.Controls.Add(_mode, 0, 0);
            _layout.Controls.Add(_max, 0, 1);
            _graph.Dock = DockStyle.Fill;
            _graph.Margin = new Padding(0, 12, 0, 12);
            _layout.Controls.Add(_graph, 0, 2);
            _layout.Controls.Add(_min, 0, 3);
            Controls.Add(_layout);

            _graph.SetColors(null);
            _max.ValueChanged += OnLimitChanged;
            _min.ValueChanged += OnLimitChanged;
            _mode.SelectedIndexChanged += OnModeChanged;
        }

        internal void ShowMode(bool visible)
        {
            _mode.Visible = visible;
            _mode.TabStop = visible;
            _mode.Margin = visible ? new Padding(0, 0, 0, 6) : Padding.Empty;
            _modeRow.SizeType = visible ? SizeType.AutoSize : SizeType.Absolute;
            _modeRow.Height = 0f;
            _layout.PerformLayout();
        }

        internal void SetState(bool auto, float minCelsius, float maxCelsius)
        {
            if (float.IsNaN(minCelsius) || float.IsNaN(maxCelsius) ||
                float.IsInfinity(minCelsius) || float.IsInfinity(maxCelsius))
                return;
            if (minCelsius > maxCelsius)
            {
                float swap = minCelsius;
                minCelsius = maxCelsius;
                maxCelsius = swap;
            }
            if (!auto && minCelsius >= maxCelsius)
                minCelsius = maxCelsius - (float)Gap;

            _updating = true;
            try
            {
                _mode.SelectedIndex = auto ? 1 : 0;
                Assign(_max, maxCelsius);
                Assign(_min, minCelsius);
                _max.Enabled = !auto;
                _min.Enabled = !auto;
            }
            finally
            {
                _updating = false;
            }
        }

        internal void SetColors(Color[] topToBottom)
        {
            _graph.SetColors(topToBottom);
        }

        internal void SetPalette(uint[] packed, bool inverted)
        {
            _graph.SetColors(PackedToColors(packed, inverted));
        }

        internal static bool HasColor(Color[] colors)
        {
            if (colors == null || colors.Length < 2)
                return false;
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i].R != 0 || colors[i].G != 0 || colors[i].B != 0)
                    return true;
            }
            return false;
        }

        internal static bool HasColor(uint[] packed)
        {
            if (packed == null || packed.Length < 2)
                return false;
            for (int i = 0; i < packed.Length; i++)
            {
                if ((packed[i] & 0xFFFFFF00u) != 0)
                    return true;
            }
            return false;
        }

        private void OnModeChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            bool editable = _mode.SelectedIndex == 0;
            _max.Enabled = editable;
            _min.Enabled = editable;
            ModeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnLimitChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            _updating = true;
            try
            {
                CorrectLowerLimit();
            }
            finally
            {
                _updating = false;
            }
            if (_mode.SelectedIndex == 0)
                RangeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CorrectLowerLimit()
        {
            if (_min.Value < _max.Value)
                return;

            decimal lower = _max.Value - Gap;
            if (lower < _min.Minimum)
            {
                lower = _min.Minimum;
                decimal upper = lower + Gap;
                if (upper > _max.Maximum)
                    upper = _max.Maximum;
                if (_max.Value != upper)
                    _max.Value = upper;
            }
            if (lower > _max.Value - Gap)
                lower = _max.Value - Gap;
            if (lower < _min.Minimum)
                lower = _min.Minimum;
            if (lower > _min.Maximum)
                lower = _min.Maximum;
            lower = Math.Round(lower, _min.DecimalPlaces, MidpointRounding.AwayFromZero);
            if (_min.Value != lower)
                _min.Value = lower;
        }

        private static void Assign(NumericUpDown box, float celsius)
        {
            decimal value;
            try
            {
                value = (decimal)celsius;
            }
            catch (OverflowException)
            {
                value = celsius > 0f ? box.Maximum : box.Minimum;
            }
            if (value < box.Minimum)
                value = box.Minimum;
            if (value > box.Maximum)
                value = box.Maximum;
            value = Math.Round(value, box.DecimalPlaces, MidpointRounding.AwayFromZero);
            if (box.Value != value)
                box.Value = value;
        }

        private static NumericUpDown Box(decimal value)
        {
            return new NumericUpDown
            {
                Minimum = TempMinimum,
                Maximum = TempMaximum,
                DecimalPlaces = 1,
                Increment = Gap,
                Value = value,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                TextAlign = HorizontalAlignment.Right,
                Margin = new Padding(0, 2, 0, 2)
            };
        }

        private static Color[] PackedToColors(uint[] packed, bool inverted)
        {
            if (!HasColor(packed))
                return null;
            var colors = new Color[packed.Length];
            for (int i = 0; i < packed.Length; i++)
            {
                int source = inverted ? i : packed.Length - 1 - i;
                uint value = packed[source];
                colors[i] = Color.FromArgb(
                    (int)((value >> 24) & 0xFF),
                    (int)((value >> 16) & 0xFF),
                    (int)((value >> 8) & 0xFF));
            }
            return colors;
        }

        private static Color[] BuildDefaultColors()
        {
            Color[] stops =
            {
                Color.White,
                Color.Yellow,
                Color.FromArgb(220, 80, 0),
                Color.FromArgb(160, 0, 0),
                Color.FromArgb(0, 0, 90),
                Color.Black
            };
            var colors = new Color[256];
            for (int i = 0; i < colors.Length; i++)
            {
                float x = i / (float)(colors.Length - 1) * (stops.Length - 1);
                int left = (int)x;
                int right = Math.Min(left + 1, stops.Length - 1);
                float blend = x - left;
                colors[i] = Color.FromArgb(
                    (int)(stops[left].R + (stops[right].R - stops[left].R) * blend),
                    (int)(stops[left].G + (stops[right].G - stops[left].G) * blend),
                    (int)(stops[left].B + (stops[right].B - stops[left].B) * blend));
            }
            return colors;
        }

        private sealed class ScaleGraph : Control
        {
            private Color[] _colors = DefaultColors;

            public ScaleGraph()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw, true);
                TabStop = false;
            }

            public void SetColors(Color[] topToBottom)
            {
                _colors = topToBottom != null && topToBottom.Length >= 2 ? topToBottom : DefaultColors;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.Clear(Parent != null ? Parent.BackColor : BackColor);
                if (ClientSize.Width < 8 || ClientSize.Height < 8)
                    return;

                const int insetY = 4;
                int barWidth = 28;
                int left = Math.Max(0, ClientSize.Width - barWidth - 1);
                int barHeight = Math.Max(1, ClientSize.Height - insetY * 2);
                var bar = new Rectangle(left, insetY, barWidth, barHeight);
                DrawGradient(g, bar);
                using (var border = new Pen(Color.DimGray))
                    g.DrawRectangle(border, bar.X, bar.Y, bar.Width - 1, bar.Height - 1);
            }

            private void DrawGradient(Graphics g, Rectangle bar)
            {
                Color[] colors = _colors ?? DefaultColors;
                using (var brush = new SolidBrush(colors[0]))
                {
                    int span = 0;
                    for (int y = 0; y < bar.Height; y++)
                    {
                        float t = bar.Height == 1 ? 0f : y / (float)(bar.Height - 1);
                        int index = (int)Math.Round(t * (colors.Length - 1));
                        if (index < 0)
                            index = 0;
                        if (index >= colors.Length)
                            index = colors.Length - 1;
                        Color color = colors[index];
                        if (color.A != 255)
                            color = Color.FromArgb(color.R, color.G, color.B);
                        if (brush.Color != color)
                        {
                            if (span > 0)
                                g.FillRectangle(brush, bar.Left, bar.Top + y - span, bar.Width, span);
                            brush.Color = color;
                            span = 0;
                        }
                        span++;
                    }
                    if (span > 0)
                        g.FillRectangle(brush, bar.Left, bar.Bottom - span, bar.Width, span);
                }
            }
        }
    }
}
