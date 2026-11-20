using System;
using System.Drawing;
using System.Windows.Forms;
using IRtekUVCSDK;

namespace IRtekUVCSDKDemo
{
    internal sealed class AcousticSettingsPanel : UserControl
    {
        private IntPtr _handle;
        private readonly GroupBox _grpFrequency;
        private readonly GroupBox _grpScale;
        private readonly GroupBox _grpDetail;
        private readonly GroupBox _grpImaging;
        private readonly TableLayoutPanel _frequencyLayout;
        private readonly TableLayoutPanel _scaleLayout;
        private readonly TableLayoutPanel _detailLayout;
        private readonly TableLayoutPanel _imagingLayout;

        private readonly NumericUpDown _freqMin = Number(0.001m, 1000, 5, 3);
        private readonly NumericUpDown _freqMax = Number(0.001m, 1000, 100, 3);

        private readonly ComboBox _scaleMode = EnumCombo(typeof(UvcSdkNative.UvcAcousticDbScaleMode));
        private float _scaleMin = -100.0f;
        private float _scaleMax = 1000.0f;
        private readonly CheckBox _fixedSpan = new CheckBox { Text = "Enabled", AutoSize = true };
        private readonly NumericUpDown _span = Number(0.1m, 100, 0.1m, 1);
        private readonly NumericUpDown _sensitivity = Number(0, 10, 3.2m, 1);
        private readonly NumericUpDown _sourceSize = Number(1, 10, 3);
        private readonly NumericUpDown _limit = Number(-100, 1000, -100, 1);
        private readonly CheckBox _focusEnabled = new CheckBox { Text = "Enabled", AutoSize = true };
        private readonly Label _fixedSpanLabel = FieldLabel("Fixed Span:");
        private readonly Label _spanLabel = FieldLabel("Span (dB):");
        private readonly Label _sensitivityLabel = FieldLabel("Sensitivity:");
        private readonly Label _sourceSizeLabel = FieldLabel("Source Size:");

        internal event EventHandler FrequencyRangeChanged;

        internal event EventHandler ContentSizeChanged;

        internal Control CommandButtons { get; }

        internal float FrequencyMinHz { get; private set; } = 5000f;

        internal float FrequencyMaxHz { get; private set; } = 100000f;

        public AcousticSettingsPanel()
        {
            Dock = DockStyle.Fill;
            _scaleMode.Items.Remove(UvcSdkNative.UvcAcousticDbScaleMode.Fixed);
            _scaleMode.SelectedIndexChanged += (sender, args) => UpdateScaleLayout();
            _fixedSpan.CheckedChanged += (sender, args) => UpdateScaleLayout();

            _grpFrequency = new GroupBox { Text = "Frequency", Dock = DockStyle.Top, Padding = new Padding(3) };
            _frequencyLayout = FormLayout();
            AddRow(_frequencyLayout, "Min Freq (kHz):", _freqMin, "Max Freq (kHz):", _freqMax);
            _grpFrequency.Controls.Add(_frequencyLayout);

            _grpScale = new GroupBox { Text = "dB Scale", Dock = DockStyle.Top, Padding = new Padding(3) };
            _scaleLayout = FormLayout();
            AddRow(_scaleLayout, "Scale Mode:", _scaleMode);
            _grpScale.Controls.Add(_scaleLayout);

            _grpDetail = new GroupBox { Text = "Source", Dock = DockStyle.Top, Padding = new Padding(3) };
            _detailLayout = FormLayout();
            AddRow(_detailLayout, _sensitivityLabel, _sensitivity, _sourceSizeLabel, _sourceSize);
            AddRow(_detailLayout, _fixedSpanLabel, _fixedSpan, _spanLabel, _span);
            _grpDetail.Controls.Add(_detailLayout);

            _grpImaging = new GroupBox { Text = "Imaging", Dock = DockStyle.Top, Padding = new Padding(3) };
            _imagingLayout = FormLayout();
            AddRow(_imagingLayout, "Limit (dB):", _limit, "Focus:", _focusEnabled);
            _grpImaging.Controls.Add(_imagingLayout);
            CommandButtons = CommandRow(
                Button("Get", LoadAll),
                Button("Set", SetAll));

            Controls.Add(_grpImaging);
            Controls.Add(_grpDetail);
            Controls.Add(_grpScale);
            Controls.Add(_grpFrequency);

            UpdateScaleLayout();
        }

        internal int ContentHeight
        {
            get
            {
                return GroupHeight(_grpFrequency) + GroupHeight(_grpScale) +
                    GroupHeight(_grpDetail) + GroupHeight(_grpImaging);
            }
        }

        private void FitGroups()
        {
            FitGroup(_grpFrequency, _frequencyLayout);
            FitGroup(_grpScale, _scaleLayout);
            FitGroup(_grpDetail, _detailLayout);
            FitGroup(_grpImaging, _imagingLayout);
        }

        private static int GroupHeight(GroupBox group)
        {
            return group.Visible ? group.Height : 0;
        }

        private static void FitGroup(GroupBox group, TableLayoutPanel layout)
        {
            if (!group.Visible)
                return;
            layout.PerformLayout();
            int chrome = group.Height - group.DisplayRectangle.Height;
            if (chrome < 18)
                chrome = 18;
            group.Height = Math.Max(1, layout.PreferredSize.Height + chrome + group.Padding.Vertical);
        }

        public void LoadFromDevice(IntPtr handle)
        {
            _handle = handle;
            SuspendLayout();
            try
            {
                LoadFrequency();
                LoadScale();
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        public void ClearDevice()
        {
            _handle = IntPtr.Zero;
        }

        private void LoadAll()
        {
            LoadFrequency();
            LoadScale();
        }

        private void SetAll()
        {
            if (!SetFrequency())
                return;
            SetScale();
        }

        private void LoadFrequency()
        {
            if (_handle == IntPtr.Zero) return;
            var value = new UvcSdkNative.UvcAcousticFreqRange();
            int ret = DeviceReadCache.GetAcousticFreq(_handle, ref value);
            if (!UvcSdkNative.IsOk(ret))
            {
                ShowError("Get acoustic frequency range", ret);
                return;
            }
            _freqMin.Value = Clamp((decimal)value.min / 1000m, _freqMin);
            _freqMax.Value = Clamp((decimal)value.max / 1000m, _freqMax);
            PublishFrequency(value.min, value.max);
        }

        private bool SetFrequency()
        {
            if (_handle == IntPtr.Zero) return false;
            var value = new UvcSdkNative.UvcAcousticFreqRange
            {
                min = (float)(_freqMin.Value * 1000m),
                max = (float)(_freqMax.Value * 1000m)
            };
            if (value.min >= value.max)
            {
                MessageBox.Show("Minimum frequency must be less than maximum frequency.",
                    FindForm()?.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            int ret = UvcSdkNative.uvc_set_acoustic_freq_range(_handle, ref value);
            if (!UvcSdkNative.IsOk(ret))
            {
                ShowError("Set acoustic frequency range", ret);
                return false;
            }
            PublishFrequency(value.min, value.max);
            return true;
        }

        private void PublishFrequency(float minHz, float maxHz)
        {
            FrequencyMinHz = minHz;
            FrequencyMaxHz = maxHz;
            FrequencyRangeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadScale()
        {
            if (_handle == IntPtr.Zero) return;
            var value = new UvcSdkNative.UvcAcousticDbScale();
            int ret = DeviceReadCache.GetAcousticScale(_handle, ref value);
            if (!UvcSdkNative.IsOk(ret))
            {
                ShowError("Get acoustic dB scale", ret);
                return;
            }
            ApplyScale(value);
        }

        private void SetScale()
        {
            if (_handle == IntPtr.Zero || _scaleMode.SelectedItem == null) return;
            var mode = (UvcSdkNative.UvcAcousticDbScaleMode)_scaleMode.SelectedItem;
            var value = new UvcSdkNative.UvcAcousticDbScale
            {
                mode = mode,
                min = _scaleMin,
                max = _scaleMax,
                fixed_span = (byte)(mode == UvcSdkNative.UvcAcousticDbScaleMode.AutoHolo && _fixedSpan.Checked ? 1 : 0),
                span = (float)_span.Value,
                sensitivity = (float)_sensitivity.Value,
                source_size = (int)_sourceSize.Value,
                limit = (float)_limit.Value,
                focus_enabled = (byte)(_focusEnabled.Checked ? 1 : 0)
            };
            int ret = UvcSdkNative.uvc_set_acoustic_db_scale(_handle, ref value);
            if (!UvcSdkNative.IsOk(ret))
                ShowError("Set acoustic dB scale", ret);
        }

        private void ApplyScale(UvcSdkNative.UvcAcousticDbScale value)
        {
            var mode = value.mode == UvcSdkNative.UvcAcousticDbScaleMode.Fixed
                ? UvcSdkNative.UvcAcousticDbScaleMode.AutoHolo
                : value.mode;
            _scaleMode.SelectedItem = mode;
            if (_scaleMode.SelectedIndex < 0)
                _scaleMode.SelectedItem = UvcSdkNative.UvcAcousticDbScaleMode.AutoMono;
            _scaleMin = value.min;
            _scaleMax = value.max;
            _fixedSpan.Checked = value.fixed_span != 0;
            _span.Value = Clamp((decimal)value.span, _span);
            _sensitivity.Value = Clamp((decimal)value.sensitivity, _sensitivity);
            _sourceSize.Value = Clamp(value.source_size, _sourceSize);
            _limit.Value = Clamp((decimal)value.limit, _limit);
            _focusEnabled.Checked = value.focus_enabled != 0;
            UpdateScaleLayout();
        }

        private void UpdateScaleLayout()
        {
            var mode = _scaleMode.SelectedItem is UvcSdkNative.UvcAcousticDbScaleMode
                ? (UvcSdkNative.UvcAcousticDbScaleMode)_scaleMode.SelectedItem
                : UvcSdkNative.UvcAcousticDbScaleMode.AutoMono;
            bool holo = mode == UvcSdkNative.UvcAcousticDbScaleMode.AutoHolo;
            bool source = mode == UvcSdkNative.UvcAcousticDbScaleMode.AutoMono ||
                mode == UvcSdkNative.UvcAcousticDbScaleMode.AutoMulti;

            _grpDetail.Text = holo ? "Window" : "Source";
            _grpDetail.Visible = source || holo;
            _fixedSpan.Enabled = holo;
            SetFieldVisible(_detailLayout, _sensitivityLabel, _sensitivity, source);
            SetFieldVisible(_detailLayout, _sourceSizeLabel, _sourceSize, source);
            SetFieldVisible(_detailLayout, _fixedSpanLabel, _fixedSpan, holo);
            SetFieldVisible(_detailLayout, _spanLabel, _span, holo && _fixedSpan.Checked);
            FitGroups();
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ShowError(string operation, int errorCode)
        {
            MessageBox.Show($"{operation} failed: {errorCode}", FindForm()?.Text,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            AddRow(layout, FieldLabel(label), control);
        }

        private static void AddRow(
            TableLayoutPanel layout, string label, Control control, string label2, Control control2)
        {
            AddRow(layout, FieldLabel(label), control, FieldLabel(label2), control2);
        }

        private static void AddRow(TableLayoutPanel layout, Label label, Control control)
        {
            AddRow(layout, label, control, null, null);
        }

        private static void AddRow(
            TableLayoutPanel layout, Label label, Control control, Label label2, Control control2)
        {
            int row = layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(label, 0, row);
            control.Anchor = AnchorStyles.Left;
            layout.Controls.Add(control, 1, row);
            if (label2 == null)
                return;
            row = layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(label2, 0, row);
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

        private static Control CommandRow(params Control[] parts)
        {
            var row = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            row.Controls.AddRange(parts);
            return row;
        }

        private static Button Button(string text, Action action, int width = 75)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = false,
                Size = new Size(width, 23),
                MinimumSize = new Size(width, 23),
                MaximumSize = new Size(width, 23),
                Margin = new Padding(6, 0, 0, 0),
                UseVisualStyleBackColor = true
            };
            button.Click += (sender, args) => action();
            return button;
        }

        private static void SetFieldVisible(TableLayoutPanel layout, Control label, Control control, bool visible)
        {
            label.Visible = visible;
            control.Visible = visible;
            TableLayoutPanelCellPosition pos = layout.GetPositionFromControl(label);
            if (pos.Row < 0 || pos.Row >= layout.RowStyles.Count)
                return;
            layout.RowStyles[pos.Row] = visible
                ? new RowStyle(SizeType.AutoSize)
                : new RowStyle(SizeType.Absolute, 0f);
        }

        private static NumericUpDown Number(
            decimal min, decimal max, decimal value, int decimals = 0, decimal? increment = null)
        {
            return new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Value = value,
                DecimalPlaces = decimals,
                Increment = increment ?? (decimals == 0 ? 1 : 0.1m),
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
            foreach (object value in Enum.GetValues(enumType))
                combo.Items.Add(value);
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
            return combo;
        }

        private static decimal Clamp(decimal value, NumericUpDown control)
        {
            return Math.Max(control.Minimum, Math.Min(control.Maximum, value));
        }

    }
}
