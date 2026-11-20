using System;
using System.Drawing;
using System.Windows.Forms;
using IRtekUVCSDK;

namespace IRtekUVCSDKDemo
{
    internal sealed class ThermalImagingPanel : UserControl
    {
        private IntPtr _handle;

        private readonly ComboBox _colorDist = EnumCombo(typeof(UvcSdkNative.UvcColorDistType));
        private readonly ComboBox _alarmType = EnumCombo(typeof(UvcSdkNative.UvcAlarmType));
        private readonly CheckBox _alarmCustom = new CheckBox { Text = "Enabled", AutoSize = true };
        private uint _alarmColorValue = 0x7FFF0000;
        private readonly Button _alarmColor = new Button
        {
            Width = 112,
            Height = 23,
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false,
            Text = string.Empty
        };
        private Label _alarmColorLabel;
        private TableLayoutPanel _alarmLayout;
        private GroupBox _alarmGroup;
        private int _alarmColorRow = -1;
        private readonly NumericUpDown _alarmMin = CelsiusBox();
        private readonly NumericUpDown _alarmMax = CelsiusBox();

        public event EventHandler Applied;

        internal Control CommandButtons { get; }

        public int ContentHeight { get; }

        public ThermalImagingPanel()
        {
            Dock = DockStyle.Fill;
            _alarmCustom.CheckedChanged += (sender, args) => UpdateAlarmColorEnabled();
            _alarmColor.Click += (sender, args) => ChooseAlarmColor();

            var colorLayout = FormLayout();
            AddRow(colorLayout, "Type:", _colorDist);
            var colorGroup = Group("Color Distribution", colorLayout);

            _alarmLayout = FormLayout();
            AddRow(_alarmLayout, "Type:", _alarmType, "Custom Color:", _alarmCustom);
            _alarmColorLabel = FieldLabel("Color:");
            _alarmColorRow = _alarmLayout.RowCount++;
            _alarmLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _alarmLayout.Controls.Add(_alarmColorLabel, 0, _alarmColorRow);
            _alarmColor.Anchor = AnchorStyles.Left;
            _alarmLayout.Controls.Add(_alarmColor, 1, _alarmColorRow);
            AddRow(_alarmLayout, "Min (°C):", _alarmMin, "Max (°C):", _alarmMax);
            _alarmGroup = Group("Alarm", _alarmLayout);
            UpdateAlarmColorEnabled();

            Controls.Add(_alarmGroup);
            Controls.Add(colorGroup);
            CommandButtons = MakeButtons(Reload, Apply);
            ContentHeight = colorGroup.Height + _alarmGroup.Height;
        }

        private static GroupBox Group(string text, TableLayoutPanel layout)
        {
            var group = new GroupBox
            {
                Text = text,
                Dock = DockStyle.Top,
                Padding = new Padding(3)
            };
            group.Controls.Add(layout);
            int chrome = group.Height - group.DisplayRectangle.Height;
            if (chrome < 18)
                chrome = 18;
            group.Height = Math.Max(1, layout.PreferredSize.Height + chrome + group.Padding.Vertical);
            return group;
        }

        public void LoadFromDevice(IntPtr handle)
        {
            _handle = handle;
            Reload();
        }

        public void ClearDevice()
        {
            _handle = IntPtr.Zero;
        }

        private void Reload()
        {
            if (_handle == IntPtr.Zero) return;

            SuspendLayout();
            try
            {
                ReloadCore();
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        private void ReloadCore()
        {
            ThermalAdLut lut;
            string lutError;
            bool hasLut = ThermalAdLut.TryLoad(_handle, out lut, out lutError);

            UvcSdkNative.UvcColorDistType colorDist;
            int ret = DeviceReadCache.GetColorDist(_handle, out colorDist);
            if (UvcSdkNative.IsOk(ret))
                _colorDist.SelectedItem = colorDist;
            else
                ShowError("Get color distribution", ret);

            var alarm = new UvcSdkNative.UvcAlarm();
            ret = DeviceReadCache.GetAlarm(_handle, ref alarm);
            if (!UvcSdkNative.IsOk(ret))
            {
                ShowError("Get thermal alarm", ret);
                return;
            }
            _alarmType.SelectedItem = alarm.type;
            _alarmCustom.Checked = alarm.custom != 0;
            SetAlarmColor(alarm.color);
            UpdateAlarmColorEnabled();
            if (hasLut)
            {
                _alarmMin.Value = Clamp(CelsiusFromKelvin(lut.ToTemp(alarm.min)), _alarmMin);
                _alarmMax.Value = Clamp(CelsiusFromKelvin(lut.ToTemp(alarm.max)), _alarmMax);
            }
            else
                MessageBox.Show(lutError, FindForm()?.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Apply()
        {
            if (_handle == IntPtr.Zero || _colorDist.SelectedItem == null || _alarmType.SelectedItem == null)
                return;

            if (_alarmMax.Value <= _alarmMin.Value)
            {
                MessageBox.Show("Max must be greater than Min.", FindForm()?.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ThermalAdLut lut;
            string lutError;
            if (!ThermalAdLut.TryLoad(_handle, out lut, out lutError))
            {
                MessageBox.Show(lutError, FindForm()?.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var colorDist = (UvcSdkNative.UvcColorDistType)_colorDist.SelectedItem;
            int ret = UvcSdkNative.uvc_set_thermal_color_dist(_handle, ref colorDist);
            if (!UvcSdkNative.IsOk(ret))
            {
                ShowError("Set color distribution", ret);
                return;
            }

            var alarm = new UvcSdkNative.UvcAlarm
            {
                type = (UvcSdkNative.UvcAlarmType)_alarmType.SelectedItem,
                custom = (byte)(_alarmCustom.Checked ? 1 : 0),
                color = _alarmColorValue,
                min = (ushort)lut.ToAd(KelvinFromCelsius((float)_alarmMin.Value)),
                max = (ushort)lut.ToAd(KelvinFromCelsius((float)_alarmMax.Value))
            };
            ret = UvcSdkNative.uvc_set_thermal_alarm(_handle, ref alarm);
            if (!UvcSdkNative.IsOk(ret))
            {
                ShowError("Set thermal alarm", ret);
                return;
            }

            Applied?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateAlarmColorEnabled()
        {
            bool show = _alarmCustom.Checked;
            _alarmColor.Enabled = show;
            _alarmColor.Visible = show;
            if (_alarmColorLabel != null)
                _alarmColorLabel.Visible = show;
            if (_alarmLayout != null && _alarmColorRow >= 0)
            {
                _alarmLayout.RowStyles[_alarmColorRow] = show
                    ? new RowStyle(SizeType.AutoSize)
                    : new RowStyle(SizeType.Absolute, 0f);
            }
            ShowAlarmColor();
            if (_alarmGroup == null || _alarmLayout == null)
                return;
            int chrome = _alarmGroup.Height - _alarmGroup.DisplayRectangle.Height;
            if (chrome < 18)
                chrome = 18;
            _alarmGroup.Height = Math.Max(1, _alarmLayout.PreferredSize.Height + chrome + _alarmGroup.Padding.Vertical);
        }

        private void ChooseAlarmColor()
        {
            using (var dialog = new ColorDialog())
            {
                dialog.Color = Color.FromArgb(
                    (int)((_alarmColorValue >> 16) & 0xFF),
                    (int)((_alarmColorValue >> 8) & 0xFF),
                    (int)(_alarmColorValue & 0xFF));
                dialog.FullOpen = true;
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                    return;
                byte alpha = (byte)(_alarmColorValue >> 24);
                SetAlarmColor(((uint)alpha << 24) |
                    ((uint)dialog.Color.R << 16) |
                    ((uint)dialog.Color.G << 8) |
                    dialog.Color.B);
            }
        }

        private void SetAlarmColor(uint argb)
        {
            _alarmColorValue = argb;
            ShowAlarmColor();
        }

        private void ShowAlarmColor()
        {
            if (!_alarmColor.Enabled)
            {
                _alarmColor.BackColor = SystemColors.Control;
                _alarmColor.FlatAppearance.BorderColor = SystemColors.ControlDark;
                return;
            }

            _alarmColor.BackColor = Color.FromArgb(
                (int)((_alarmColorValue >> 16) & 0xFF),
                (int)((_alarmColorValue >> 8) & 0xFF),
                (int)(_alarmColorValue & 0xFF));
            _alarmColor.FlatAppearance.BorderColor = Color.DimGray;
        }

        private void ShowError(string operation, int errorCode)
        {
            MessageBox.Show($"{operation} failed: {errorCode}", FindForm()?.Text,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private const float CelsiusOffset = 273.15f;

        private static decimal CelsiusFromKelvin(float kelvin)
        {
            return (decimal)kelvin - 273.15m;
        }

        private static float KelvinFromCelsius(float celsius)
        {
            return celsius + CelsiusOffset;
        }

        private static NumericUpDown CelsiusBox()
        {
            return Number(-273.15m, 5726.85m, 0m, 2);
        }

        private static NumericUpDown Number(decimal min, decimal max, decimal value, int decimals)
        {
            return new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Value = value,
                DecimalPlaces = decimals,
                Increment = decimals == 0 ? 1 : 0.1m,
                Width = 112
            };
        }

        private static ComboBox EnumCombo(Type enumType)
        {
            var combo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 112
            };
            foreach (object item in Enum.GetValues(enumType))
                combo.Items.Add(item);
            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
            return combo;
        }

        private static decimal Clamp(decimal value, NumericUpDown control)
        {
            return Math.Max(control.Minimum, Math.Min(control.Maximum, value));
        }

        private static TableLayoutPanel FormLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(8)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
            return layout;
        }

        private static void AddRow(TableLayoutPanel layout, string label, Control control)
        {
            AddRow(layout, label, control, null, null);
        }

        private static void AddRow(
            TableLayoutPanel layout, string label, Control control, string label2, Control control2)
        {
            int row = layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(FieldLabel(label), 0, row);
            control.Anchor = AnchorStyles.Left;
            layout.Controls.Add(control, 1, row);
            if (label2 == null)
                return;
            row = layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(FieldLabel(label2), 0, row);
            control2.Anchor = AnchorStyles.Left;
            layout.Controls.Add(control2, 1, row);
        }

        private static Label FieldLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(3, 4, 3, 3)
            };
        }

        private static FlowLayoutPanel MakeButtons(Action get, Action set)
        {
            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = Padding.Empty
            };
            buttons.Controls.Add(Button("Get", get));
            buttons.Controls.Add(Button("Set", set));
            return buttons;
        }

        private static Button Button(string text, Action action)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = false,
                Size = new Size(75, 23),
                MinimumSize = new Size(75, 23),
                MaximumSize = new Size(75, 23),
                Margin = new Padding(6, 0, 0, 0),
                UseVisualStyleBackColor = true
            };
            button.Click += (sender, args) => action();
            return button;
        }
    }
}
