using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IRtekHLAPI;

namespace IRtekHLAPIDemo
{
    public class ThermalOsdPanel : UserControl
    {
        private readonly OverlayBlockEditor _unit = new OverlayBlockEditor("Unit");
        private readonly OverlayBlockEditor _plt = new OverlayBlockEditor("Palette");
        private readonly OverlayBlockEditor _params = new OverlayBlockEditor("Params");
        private readonly FollowBlockEditor _global = new FollowBlockEditor("Global");
        private readonly FollowBlockEditor _marker = new FollowBlockEditor("Marker");
        private readonly OverlayBlockEditor _alarm = new OverlayBlockEditor("Alarm");
        private readonly Button _btnGet = new Button { Text = "Get", Width = 80, Height = 25 };
        private readonly Button _btnSet = new Button { Text = "Set", Width = 80, Height = 25 };
        private readonly Button _btnToggleEnable = new Button { Width = 90, Height = 25 };

        public event EventHandler GetClicked;
        public event EventHandler SetClicked;

        public ThermalOsdPanel()
        {
            AutoScroll = true;
            Dock = DockStyle.Fill;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Padding = new Padding(6)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            AddBlock(layout, _unit);
            AddBlock(layout, _plt);
            AddBlock(layout, _params);
            AddBlock(layout, _global);
            AddBlock(layout, _marker);
            AddBlock(layout, _alarm);

            WireEnableChanged(_unit);
            WireEnableChanged(_plt);
            WireEnableChanged(_params);
            WireEnableChanged(_global);
            WireEnableChanged(_marker);
            WireEnableChanged(_alarm);

            var buttonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(3, 12, 3, 3)
            };
            _btnGet.Click += (s, e) => GetClicked?.Invoke(this, EventArgs.Empty);
            _btnSet.Click += (s, e) => SetClicked?.Invoke(this, EventArgs.Empty);
            _btnToggleEnable.Click += (s, e) => ToggleAllEnableCheckboxes();
            UpdateToggleEnableButtonText();
            buttonPanel.Controls.Add(_btnGet);
            buttonPanel.Controls.Add(_btnSet);
            buttonPanel.Controls.Add(_btnToggleEnable);
            layout.Controls.Add(buttonPanel);

            Controls.Add(layout);
        }

        private void WireEnableChanged(OverlayBlockEditor editor)
        {
            editor.EnableChanged += (s, e) => UpdateToggleEnableButtonText();
        }

        private void WireEnableChanged(FollowBlockEditor editor)
        {
            editor.EnableChanged += (s, e) => UpdateToggleEnableButtonText();
        }

        private static void AddBlock(TableLayoutPanel layout, Control block)
        {
            block.Margin = new Padding(3, 3, 3, 6);
            block.Dock = DockStyle.Top;
            layout.Controls.Add(block);
        }

        public void SetAxisLimits(ushort width, ushort height)
        {
            _unit.SetAxisLimits(width, height);
            _plt.SetAxisLimits(width, height);
            _params.SetAxisLimits(width, height);
            _global.SetAxisLimits(width, height);
            _marker.SetAxisLimits(width, height);
            _alarm.SetAxisLimits(width, height);
        }

        public void LoadFrom(NetSdkNative.NetThermalOverlayInfo info)
        {
            _unit.LoadFrom(info.unit_overlay);
            _plt.LoadFrom(info.plt_overlay);
            _params.LoadFrom(info.thermal_params_overlay);
            _global.LoadFrom(info.global_overlay);
            _marker.LoadFrom(info.marker_overlay);
            _alarm.LoadFrom(info.alarm_overlay);
            UpdateToggleEnableButtonText();
        }

        private void ToggleAllEnableCheckboxes()
        {
            bool enableAll = !AreAllEnableCheckboxesChecked();
            _unit.SetEnableChecked(enableAll);
            _plt.SetEnableChecked(enableAll);
            _params.SetEnableChecked(enableAll);
            _global.SetEnableChecked(enableAll);
            _marker.SetEnableChecked(enableAll);
            _alarm.SetEnableChecked(enableAll);
            UpdateToggleEnableButtonText();
        }

        private bool AreAllEnableCheckboxesChecked()
        {
            return _unit.IsEnableChecked
                && _plt.IsEnableChecked
                && _params.IsEnableChecked
                && _global.IsEnableChecked
                && _marker.IsEnableChecked
                && _alarm.IsEnableChecked;
        }

        private void UpdateToggleEnableButtonText()
        {
            _btnToggleEnable.Text = AreAllEnableCheckboxesChecked() ? "Disable All" : "Enable All";
        }

        public NetSdkNative.NetThermalOverlayInfo BuildStruct()
        {
            return new NetSdkNative.NetThermalOverlayInfo
            {
                unit_overlay = _unit.BuildStruct(),
                plt_overlay = _plt.BuildStruct(),
                thermal_params_overlay = _params.BuildStruct(),
                global_overlay = _global.BuildStruct(),
                marker_overlay = _marker.BuildStruct(),
                alarm_overlay = _alarm.BuildStruct()
            };
        }

        public static string FormatSummary(NetSdkNative.NetThermalOverlayInfo info)
        {
            var sb = new StringBuilder();
            AppendBlock(sb, "unit", info.unit_overlay);
            AppendBlock(sb, "plt", info.plt_overlay);
            AppendBlock(sb, "params", info.thermal_params_overlay);
            AppendFollow(sb, "global", info.global_overlay);
            AppendFollow(sb, "marker", info.marker_overlay);
            AppendBlock(sb, "alarm", info.alarm_overlay);
            return sb.ToString().Trim();
        }

        private static void AppendBlock(StringBuilder sb, string name, NetSdkNative.NetOverlayBlock block)
        {
            sb.Append(name).Append('=').Append(block.enabled != 0 ? 1 : 0)
                .Append("(align=").Append(block.align)
                .Append(",mode=").Append(block.mode)
                .Append(",x=").Append(block.x)
                .Append(",y=").Append(block.y)
                .Append(") ");
        }

        private static void AppendFollow(StringBuilder sb, string name, NetSdkNative.NetOverlayFollowBlock block)
        {
            sb.Append(name).Append('=').Append(block.enabled != 0 ? 1 : 0)
                .Append("(align=").Append(block.align)
                .Append(",follow=").Append(block.follow != 0 ? 1 : 0)
                .Append(",max=").Append(block.max_enabled != 0 ? 1 : 0)
                .Append(",min=").Append(block.min_enabled != 0 ? 1 : 0)
                .Append(",avg=").Append(block.avg_enabled != 0 ? 1 : 0)
                .Append(",emi=").Append(block.emissivity_enabled != 0 ? 1 : 0)
                .Append(",x=").Append(block.x)
                .Append(",y=").Append(block.y)
                .Append(") ");
        }

        private sealed class OverlayBlockEditor : GroupBox
        {
            private readonly CheckBox _enabled = new CheckBox { Text = "Enabled", AutoSize = true };
            private readonly ComboBox _align = CreateAlignCombo();
            private readonly ComboBox _mode = CreateModeCombo();
            private readonly TrackBar _x = CreateTrackBar();
            private readonly TrackBar _y = CreateTrackBar();
            private readonly Label _lblX = new Label { Text = "x:0", Size = new Size(50, 15), AutoSize = false };
            private readonly Label _lblY = new Label { Text = "y:0", Size = new Size(50, 15), AutoSize = false };

            public OverlayBlockEditor(string title)
            {
                Text = title;
                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                Padding = new Padding(8, 16, 8, 8);

                var grid = new TableLayoutPanel
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = 4
                };
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70f));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130f));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50f));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

                AddRow(grid, 0, "Enabled", _enabled, "Align", _align);
                AddRow(grid, 1, "Mode", _mode, null, null);
                AddAxisRow(grid, 2, "X", _x, _lblX);
                AddAxisRow(grid, 3, "Y", _y, _lblY);

                _x.Scroll += (s, e) => _lblX.Text = "x:" + _x.Value;
                _y.Scroll += (s, e) => _lblY.Text = "y:" + _y.Value;
                _enabled.CheckedChanged += (s, e) => EnableChanged?.Invoke(this, EventArgs.Empty);

                Controls.Add(grid);
            }

            public event EventHandler EnableChanged;

            public void SetAxisLimits(ushort width, ushort height)
            {
                _x.Maximum = width;
                _y.Maximum = height;
                if (_x.Value > _x.Maximum) _x.Value = _x.Maximum;
                if (_y.Value > _y.Maximum) _y.Value = _y.Maximum;
            }

            public bool IsEnableChecked => _enabled.Checked;

            public void SetEnableChecked(bool value)
            {
                _enabled.Checked = value;
            }

            public void LoadFrom(NetSdkNative.NetOverlayBlock block)
            {
                _enabled.Checked = block.enabled != 0;
                _align.SelectedIndex = Math.Max(0, Math.Min(_align.Items.Count - 1, (int)block.align));
                _mode.SelectedIndex = Math.Max(0, Math.Min(_mode.Items.Count - 1, (int)block.mode));
                _x.Value = Math.Min((int)block.x, _x.Maximum);
                _y.Value = Math.Min((int)block.y, _y.Maximum);
                _lblX.Text = "x:" + block.x;
                _lblY.Text = "y:" + block.y;
            }

            public NetSdkNative.NetOverlayBlock BuildStruct()
            {
                return new NetSdkNative.NetOverlayBlock
                {
                    enabled = (byte)(_enabled.Checked ? 1 : 0),
                    align = _align.SelectedIndex >= 0
                        ? (NetSdkNative.NetOverlayAlign)_align.SelectedIndex
                        : NetSdkNative.NetOverlayAlign.TopLeft,
                    mode = _mode.SelectedIndex >= 0
                        ? (NetSdkNative.NetOverlayDisplayMode)_mode.SelectedIndex
                        : NetSdkNative.NetOverlayDisplayMode.Light,
                    x = (uint)_x.Value,
                    y = (uint)_y.Value
                };
            }
        }

        private sealed class FollowBlockEditor : GroupBox
        {
            private readonly CheckBox _enabled = new CheckBox { Text = "Enabled", AutoSize = true };
            private readonly ComboBox _align = CreateAlignCombo();
            private readonly CheckBox _follow = new CheckBox { Text = "Follow", AutoSize = true };
            private readonly CheckBox _max = new CheckBox { Text = "Max", AutoSize = true };
            private readonly CheckBox _min = new CheckBox { Text = "Min", AutoSize = true };
            private readonly CheckBox _avg = new CheckBox { Text = "Avg", AutoSize = true };
            private readonly CheckBox _emissivity = new CheckBox { Text = "Emissivity", AutoSize = true };
            private readonly TrackBar _x = CreateTrackBar();
            private readonly TrackBar _y = CreateTrackBar();
            private readonly Label _lblX = new Label { Text = "x:0", Size = new Size(50, 15), AutoSize = false };
            private readonly Label _lblY = new Label { Text = "y:0", Size = new Size(50, 15), AutoSize = false };

            public FollowBlockEditor(string title)
            {
                Text = title;
                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                Padding = new Padding(8, 16, 8, 8);

                var grid = new TableLayoutPanel
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = 4
                };
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70f));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130f));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50f));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

                AddRow(grid, 0, "Enabled", _enabled, "Align", _align);
                AddRow(grid, 1, "Follow", _follow, "Max", _max);
                AddRow(grid, 2, "Min", _min, "Avg", _avg);
                AddRow(grid, 3, "Emissivity", _emissivity, null, null);
                AddAxisRow(grid, 4, "X", _x, _lblX);
                AddAxisRow(grid, 5, "Y", _y, _lblY);

                _enabled.CheckedChanged += (s, e) =>
                {
                    UpdateChildState();
                    EnableChanged?.Invoke(this, EventArgs.Empty);
                };
                _x.Scroll += (s, e) => _lblX.Text = "x:" + _x.Value;
                _y.Scroll += (s, e) => _lblY.Text = "y:" + _y.Value;

                Controls.Add(grid);
                UpdateChildState();
            }

            public event EventHandler EnableChanged;

            private void UpdateChildState()
            {
                bool on = _enabled.Checked;
                _follow.Enabled = on;
                _max.Enabled = on;
                _min.Enabled = on;
                _avg.Enabled = on;
                _emissivity.Enabled = on;
            }

            public void SetAxisLimits(ushort width, ushort height)
            {
                _x.Maximum = width;
                _y.Maximum = height;
                if (_x.Value > _x.Maximum) _x.Value = _x.Maximum;
                if (_y.Value > _y.Maximum) _y.Value = _y.Maximum;
            }

            public bool IsEnableChecked => _enabled.Checked;

            public void SetEnableChecked(bool value)
            {
                _enabled.Checked = value;
            }

            public void LoadFrom(NetSdkNative.NetOverlayFollowBlock block)
            {
                _enabled.Checked = block.enabled != 0;
                _align.SelectedIndex = Math.Max(0, Math.Min(_align.Items.Count - 1, (int)block.align));
                _follow.Checked = block.follow != 0;
                _max.Checked = block.max_enabled != 0;
                _min.Checked = block.min_enabled != 0;
                _avg.Checked = block.avg_enabled != 0;
                _emissivity.Checked = block.emissivity_enabled != 0;
                _x.Value = Math.Min((int)block.x, _x.Maximum);
                _y.Value = Math.Min((int)block.y, _y.Maximum);
                _lblX.Text = "x:" + block.x;
                _lblY.Text = "y:" + block.y;
                UpdateChildState();
            }

            public NetSdkNative.NetOverlayFollowBlock BuildStruct()
            {
                return new NetSdkNative.NetOverlayFollowBlock
                {
                    enabled = (byte)(_enabled.Checked ? 1 : 0),
                    align = _align.SelectedIndex >= 0
                        ? (NetSdkNative.NetOverlayAlign)_align.SelectedIndex
                        : NetSdkNative.NetOverlayAlign.TopLeft,
                    follow = (byte)(_follow.Checked ? 1 : 0),
                    max_enabled = (byte)(_max.Checked ? 1 : 0),
                    min_enabled = (byte)(_min.Checked ? 1 : 0),
                    avg_enabled = (byte)(_avg.Checked ? 1 : 0),
                    emissivity_enabled = (byte)(_emissivity.Checked ? 1 : 0),
                    x = (uint)_x.Value,
                    y = (uint)_y.Value
                };
            }
        }

        private static ComboBox CreateAlignCombo()
        {
            return new NoWheelComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "TopLeft", "TopRight", "BottomLeft", "BottomRight" }
            };
        }

        private static ComboBox CreateModeCombo()
        {
            return new NoWheelComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Light", "Blink" }
            };
        }

        private static TrackBar CreateTrackBar()
        {
            return new NoWheelTrackBar
            {
                AutoSize = false,
                Size = new Size(130, 20),
                Maximum = 640,
                TickStyle = TickStyle.None
            };
        }

        private sealed class NoWheelComboBox : ComboBox
        {
            private const int WmMouseWheel = 0x020A;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WmMouseWheel && !DroppedDown)
                    return;
                base.WndProc(ref m);
            }
        }

        private sealed class NoWheelTrackBar : TrackBar
        {
            private const int WmMouseWheel = 0x020A;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WmMouseWheel)
                    return;
                base.WndProc(ref m);
            }
        }

        private static void AddRow(TableLayoutPanel grid, int row, string label1, Control c1, string label2, Control c2)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lbl1 = new Label { Text = label1, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 3, 3) };
            grid.Controls.Add(lbl1, 0, row);
            if (c1 != null)
            {
                c1.Margin = new Padding(0, 3, 3, 3);
                c1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                grid.Controls.Add(c1, 1, row);
            }

            if (label2 == null)
                return;

            var lbl2 = new Label { Text = label2, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 3, 3) };
            grid.Controls.Add(lbl2, 2, row);

            if (c2 == null)
                return;

            c2.Margin = new Padding(0, 3, 3, 3);
            c2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            grid.Controls.Add(c2, 3, row);
        }

        private static void AddAxisRow(TableLayoutPanel grid, int row, string label, TrackBar trackBar, Label valueLabel)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lbl = new Label
            {
                Text = label,
                AutoSize = false,
                Size = new Size(70, 15),
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 3, 3)
            };
            grid.Controls.Add(lbl, 0, row);

            trackBar.Margin = new Padding(0, 3, 3, 3);
            trackBar.Anchor = AnchorStyles.Left;
            grid.Controls.Add(trackBar, 1, row);

            valueLabel.Margin = new Padding(3, 6, 0, 3);
            valueLabel.Anchor = AnchorStyles.Left;
            grid.Controls.Add(valueLabel, 2, row);
        }
    }
}
