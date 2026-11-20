using System;
using System.Drawing;
using System.Windows.Forms;

namespace IRtekHLAPIDemo
{
    internal sealed class LocalParamsDialog : Form
    {
        private readonly CheckBox _chkEnabled;
        private readonly NumericUpDown _nudEmissivity;
        private readonly NumericUpDown _nudDistance;
        private readonly NumericUpDown _nudReflTemp;
        private readonly Label _lblDistanceUnit;
        private readonly Label _lblReflTempUnit;

        public bool LocalParamsEnabled
        {
            get { return _chkEnabled.Checked; }
        }

        public float Emissivity
        {
            get { return (float)_nudEmissivity.Value; }
        }

        public float Distance
        {
            get { return (float)_nudDistance.Value; }
        }

        public float ReflTemp
        {
            get { return (float)_nudReflTemp.Value; }
        }

        public LocalParamsDialog(string markerName, bool enabled, float emissivity, float distance, float reflTemp,
            string tempUnit, string distanceUnit)
        {
            Text = "Local Parameters - " + markerName;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(320, 190);
            ShowInTaskbar = false;

            _chkEnabled = new CheckBox
            {
                Text = "Enable Local Parameters",
                AutoSize = true,
                Location = new Point(16, 16),
                Checked = enabled
            };

            var lblEmissivity = new Label
            {
                Text = "Emissivity",
                AutoSize = true,
                Location = new Point(16, 52)
            };
            _nudEmissivity = new NumericUpDown
            {
                DecimalPlaces = 2,
                Increment = 0.01M,
                Minimum = 0.01M,
                Maximum = 1.00M,
                Location = new Point(120, 48),
                Width = 100,
                Value = Clamp(emissivity, 0.01M, 1.00M)
            };

            var lblDistance = new Label
            {
                Text = "Distance",
                AutoSize = true,
                Location = new Point(16, 84)
            };
            _nudDistance = new NumericUpDown
            {
                DecimalPlaces = 2,
                Increment = 0.1M,
                Minimum = 0M,
                Maximum = 50000M,
                Location = new Point(120, 80),
                Width = 100,
                Value = Clamp(distance, 0M, 50000M)
            };
            _lblDistanceUnit = new Label
            {
                Text = distanceUnit,
                AutoSize = true,
                Location = new Point(226, 84)
            };

            var lblReflTemp = new Label
            {
                Text = "Refl. Temp",
                AutoSize = true,
                Location = new Point(16, 116)
            };
            _nudReflTemp = new NumericUpDown
            {
                DecimalPlaces = 1,
                Increment = 0.1M,
                Minimum = -273M,
                Maximum = 2000M,
                Location = new Point(120, 112),
                Width = 100,
                Value = Clamp(reflTemp, -273M, 2000M)
            };
            _lblReflTempUnit = new Label
            {
                Text = tempUnit,
                AutoSize = true,
                Location = new Point(226, 116)
            };

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(120, 150),
                Width = 80
            };
            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(210, 150),
                Width = 80
            };

            Controls.Add(_chkEnabled);
            Controls.Add(lblEmissivity);
            Controls.Add(_nudEmissivity);
            Controls.Add(lblDistance);
            Controls.Add(_nudDistance);
            Controls.Add(_lblDistanceUnit);
            Controls.Add(lblReflTemp);
            Controls.Add(_nudReflTemp);
            Controls.Add(_lblReflTempUnit);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
            UpdateEditorsEnabled();
            _chkEnabled.CheckedChanged += (s, e) => UpdateEditorsEnabled();
        }

        private void UpdateEditorsEnabled()
        {
            bool on = _chkEnabled.Checked;
            _nudEmissivity.Enabled = on;
            _nudDistance.Enabled = on;
            _nudReflTemp.Enabled = on;
        }

        private static decimal Clamp(float value, decimal min, decimal max)
        {
            decimal d = (decimal)value;
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
