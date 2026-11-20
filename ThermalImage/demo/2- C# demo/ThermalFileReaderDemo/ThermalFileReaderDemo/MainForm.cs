using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ThermalImage;

namespace ThermalFileReaderDemo
{
    public partial class MainForm : Form
    {
        public enum MarkerType
        {
            None,
            Spot,
            Line,
            Rectangle,
            Ellipse,
            Delta,
        }

        private enum TrackerHitType
        {
            hitNothing = -1,
            hitTopLeft = 0,
            hitTopRight = 1,
            hitBottomRight = 2,
            hitBottomLeft = 3,
            hitTop = 4,
            hitRight = 5,
            hitBottom = 6,
            hitLeft = 7,
            hitMiddle = 8,
            hitEdge = 9,
            hitStart = 10,
            hitEnd = 11
        };

        public enum FileFormat
        {
            Default,
            FJPG
        }


        private class MarkerTracker
        {
            public MarkerTracker()
            {
                Reset();
            }

            public void Reset()
            {
                IsTracking = false;
                Name = string.Empty;
                MarkerType = MarkerType.None;
                HitType = TrackerHitType.hitNothing;
                PrevPos = new Point(0, 0);
                HitPos = new Point(0, 0);
            }

            public bool IsTracking { get; set; }
            public string Name { get; set; }
            public MarkerType MarkerType { get; set; }
            public TrackerHitType HitType { get; set; }
            public Point PrevPos { get; set; }
            public Point HitPos { get; set; }
        }

        private const float CELSIUS_0 = 273.15F;
        private const int PERCENT_100 = 100;

        private const int PALETTE_ICON_WIDTH = 24;
        private const int PALETTE_ICON_HEIGHT = 24;

        private static readonly uint[] USER_DEFINED_COLORS = new uint[]
        {
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            3841982464,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4287430656,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            4293787648,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            42017024,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            5046016,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520,
            1996523520
        };

        private bool IsGUIOperating { get; set; }
        private bool IsGUIRefreshing { get; set; }
        private MarkerTracker Tracker { get; set; }
        private bool Adding { get; set; }
        private bool Deleting { get; set; }
        private MarkerType CurrentMarkerType { get; set; }
        private string AudioFileName { get; set; }
        private MediaPlayer Player { get; set; }
        private ThermalImageFile Image { get; set; }
        private int FrameIndex { get; set; }
        private int FrameCount { get; set; }

        public struct PresetPaletteItem
        {
            public byte RefNo { get; set; }
            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                if (!(obj is PresetPaletteItem))
                {
                    return false;
                }

                return RefNo == ((PresetPaletteItem)obj).RefNo;
            }

            public override int GetHashCode()
            {
                return RefNo.GetHashCode();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            tscbFileFormat.Items.Add(FileFormat.Default);
            tscbFileFormat.Items.Add(FileFormat.FJPG);
            tscbFileFormat.SelectedItem = FileFormat.Default;

            List<PresetPalette> presetPalettes = ThermalImageFile.GetPresetPalettes();
            for (int i = 0; i < presetPalettes.Count; i++)
            {
                PresetPalette presetPalette = presetPalettes[i];
                tscbPalette.Items.Add(new PresetPaletteItem() { RefNo = presetPalette.RefNo, Name = presetPalette.Name });
            }
            tscbPalette.Items.Add(new PresetPaletteItem() { RefNo = 0xFF, Name = "<UserDefined>" });

            tscbAlarm.Items.Add(AlarmType.Normal);
            tscbAlarm.Items.Add(AlarmType.Below);
            tscbAlarm.Items.Add(AlarmType.Above);
            tscbAlarm.Items.Add(AlarmType.Between);
            tscbAlarm.Items.Add(AlarmType.MixPalette);
            tscbAlarm.Items.Add(AlarmType.Outside);
            tscbAlarm.Items.Add(AlarmType.Humidity);
            tscbAlarm.Items.Add(AlarmType.Insulation);

            tscbColorDistributionType.Items.Add(ColorDistributionType.T_TWB);
            tscbColorDistributionType.Items.Add(ColorDistributionType.Tempertature);
            tscbColorDistributionType.Items.Add(ColorDistributionType.Linear);

            tscbViewType.Items.Add(ViewType.Thermal);
            tscbViewType.Items.Add(ViewType.PicInPic);
            tscbViewType.Items.Add(ViewType.T_DEF);
            tscbViewType.Items.Add(ViewType.Fusion);

            IsGUIOperating = false;
            IsGUIRefreshing = false;
            Tracker = new MarkerTracker();
            Adding = false;
            Deleting = false;
            CurrentMarkerType = MarkerType.None;
            AudioFileName = string.Empty;
            Player = new MediaPlayer();
            Image = new ThermalImageFile();
            Image.MarkerAdded += Image_MarkerAdded;
            Image.MarkerChanged += Image_MarkerChanged;
            Image.MarkerRenamed += Image_MarkerRenamed;
            Image.MarkerRemoved += Image_MarkerRemoved;
            Image.DeltaAdded += Image_DeltaAdded;
            Image.DeltaChanged += Image_DeltaChanged;
            Image.DeltaRemoved += Image_DeltaRemoved;
            Image.ReferenceChanged += Image_ReferenceChanged;
            Image.GainChanged += Image_GainChanged;
            Image.PaletteChanged += Image_PaletteChanged;
            Image.ImageChanged += Image_ImageChanged;
            Image.AlarmChanged += Image_AlarmChanged;

            DoGUIRefresh();

            Text = "Thermal File Reader Sample, running IRtekFileSDK version: " + ImageBase.Version;
        }

        private void Image_AlarmChanged(object sender, EventArgs e)
        {
            bool oldValue = IsGUIRefreshing;
            IsGUIRefreshing = true;
            AlarmV2 alarm = Image.GetAlarmV2();
            tscbAlarm.SelectedItem = alarm.Type;
            cbAlarmCustom.Checked = alarm.Custom;
            btnAlarmColor.BackColor = Color.FromArgb(alarm.Color.R, alarm.Color.G, alarm.Color.B);
            nudAlarmAlpha.Value = alarm.Color.A;
            nudAlarmMax.Value = (decimal)(alarm.Max - CELSIUS_0);
            nudAlarmMin.Value = (decimal)(alarm.Min - CELSIUS_0);
            nudRHLimit.Value = (decimal)(alarm.RHLimit * PERCENT_100);
            nudIndoorTemp.Value = (decimal)(alarm.IndoorTemp - CELSIUS_0);
            nudOutdoorTemp.Value = (decimal)(alarm.OutdoorTemp - CELSIUS_0);
            nudHeatIndex.Value = (decimal)alarm.HeatIndex;
            nudThreshold.Value = (decimal)(alarm.Threshold - CELSIUS_0);
            IsGUIRefreshing = oldValue;
        }

        private void Image_ImageChanged(object sender, EventArgs e)
        {
            pbThermalImage.Image = BuildImage();
        }

        private void Image_PaletteChanged(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pbPalleteImage.Width, pbPalleteImage.Height);
            Image paletteImage = Image.GetPaletteImage();
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
            }
            pbPalleteImage.Image = bmp;

            if (IsGUIOperating)
            {
                return;
            }

            bool oldValue = IsGUIRefreshing;
            IsGUIRefreshing = true;
            PaletteV2 palette = Image.GetPaletteV2();
            tscbPalette.SelectedItem = new PresetPaletteItem() { RefNo = palette.RefNo, Name = string.Empty };
            if (palette.Inverted)
            {
                tsbInverted.BackColor = SystemColors.Highlight;
            }
            else
            {
                tsbInverted.BackColor = SystemColors.Control;
            }

            bmp = new Bitmap(PALETTE_ICON_WIDTH, PALETTE_ICON_HEIGHT);
            paletteImage = ThermalImageFile.GetPresetPaletteImage(palette.RefNo);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
            }
            tsbPalette.Image = bmp;
            IsGUIRefreshing = oldValue;
        }

        private void Image_GainChanged(object sender, EventArgs e)
        {
            bool oldValue = IsGUIRefreshing;
            IsGUIRefreshing = true;
            Gain gain = Image.GetGain();
            switch (gain.Mode)
            {
                case GainMode.Fixed:
                case GainMode.Touch:
                    nudMax.Value = (decimal)(gain.Max - CELSIUS_0);
                    nudMin.Value = (decimal)(gain.Min - CELSIUS_0);
                    cbFixedSpan.Checked = gain.FixedSpan;
                    nudSpan.Value = (decimal)gain.Span;
                    break;
                case GainMode.Auto:
                default:
                    nudMax.Value = (decimal)(gain.Max - CELSIUS_0);
                    nudMin.Value = (decimal)(gain.Min - CELSIUS_0);
                    cbFixedSpan.Checked = gain.FixedSpan;
                    nudSpan.Value = (decimal)gain.Span;
                    break;
            }
            IsGUIRefreshing = oldValue;
        }

        private void Image_ReferenceChanged(object sender, EventArgs e)
        {
            Reference irReference = Image.GetReference();
            if (irReference.Enabled)
            {
                float referenceTemperature = Image.GetReferenceTemperature();
                nudRealRefTemp.Value = (decimal)(referenceTemperature - CELSIUS_0);
            }

            if (IsGUIOperating)
            {
                return;
            }

            bool oldValue = IsGUIRefreshing;
            IsGUIRefreshing = true;
            cbIsRefEnabled.Checked = irReference.Enabled;
            ThermalImageFile.MarkerCollection markers = Image.Markers;
            cbRefName.Items.Clear();
            cbRefName.Items.Add(string.Empty);
            for (int i = 1; i < markers.Count; i++)
            {
                cbRefName.Items.Add(markers[i].Name);
            }
            cbRefName.SelectedItem = irReference.Name;
            switch (irReference.ValueType)
            {
                case ThermalImage.ValueType.Max:
                    cbRefValue.SelectedItem = "Max";
                    break;
                case ThermalImage.ValueType.Min:
                    cbRefValue.SelectedItem = "Min";
                    break;
                case ThermalImage.ValueType.Avg:
                    cbRefValue.SelectedItem = "Avg";
                    break;
            }
            nudRefTemp.Value = (decimal)(irReference.Temp - CELSIUS_0);
            IsGUIRefreshing = oldValue;
        }

        private void Image_DeltaAdded(object sender, ThermalImageFile.DeltaEventArgs e)
        {
            mlvMarkers.AddDelta(e.Delta);
        }

        private void Image_DeltaChanged(object sender, ThermalImageFile.DeltaEventArgs e)
        {
            mlvMarkers.UpdateDelta(e.Delta);
        }

        private void Image_DeltaRemoved(object sender, ThermalImageFile.DeltaEventArgs e)
        {
            mlvMarkers.RemoveDelta(e.Delta);
        }

        private void Image_MarkerAdded(object sender, ThermalImageFile.MarkerEventArgs e)
        {
            pbThermalImage.Image = BuildImage();

            switch (e.Marker.Type)
            {
                case ThermalImage.MarkerType.Spot:
                    mlvMarkers.AddSpot(e.Marker as ThermalImageFile.MarkerSpot);
                    break;
                case ThermalImage.MarkerType.Rectangle:
                    mlvMarkers.AddRectangle(e.Marker as ThermalImageFile.MarkerRectangle);
                    break;
                case ThermalImage.MarkerType.Ellipse:
                    mlvMarkers.AddEllipse(e.Marker as ThermalImageFile.MarkerEllipse);
                    break;
                case ThermalImage.MarkerType.Polygon:
                    mlvMarkers.AddPolygon(e.Marker as ThermalImageFile.MarkerPolygon);
                    break;
                case ThermalImage.MarkerType.Polyline:
                    mlvMarkers.AddPolyline(e.Marker as ThermalImageFile.MarkerPolyline);
                    break;
            }
        }

        private void Image_MarkerChanged(object sender, ThermalImageFile.MarkerEventArgs e)
        {
            pbThermalImage.Image = BuildImage();

            switch (e.Marker.Type)
            {
                case ThermalImage.MarkerType.Spot:
                    mlvMarkers.UpdateSpot(e.Marker as ThermalImageFile.MarkerSpot);
                    break;
                case ThermalImage.MarkerType.Rectangle:
                    mlvMarkers.UpdateRectangle(e.Marker as ThermalImageFile.MarkerRectangle);
                    break;
                case ThermalImage.MarkerType.Ellipse:
                    mlvMarkers.UpdateEllipse(e.Marker as ThermalImageFile.MarkerEllipse);
                    break;
                case ThermalImage.MarkerType.Polygon:
                    mlvMarkers.UpdatePolygon(e.Marker as ThermalImageFile.MarkerPolygon);
                    break;
                case ThermalImage.MarkerType.Polyline:
                    mlvMarkers.UpdatePolyline(e.Marker as ThermalImageFile.MarkerPolyline);
                    break;
            }
        }

        private void Image_MarkerRenamed(object sender, ThermalImageFile.MarkerEventArgs e)
        {
            pbThermalImage.Image = BuildImage();

            switch (e.Marker.Type)
            {
                case ThermalImage.MarkerType.Spot:
                    mlvMarkers.UpdateSpot(e.Marker as ThermalImageFile.MarkerSpot);
                    break;
                case ThermalImage.MarkerType.Rectangle:
                    mlvMarkers.UpdateRectangle(e.Marker as ThermalImageFile.MarkerRectangle);
                    break;
                case ThermalImage.MarkerType.Ellipse:
                    mlvMarkers.UpdateEllipse(e.Marker as ThermalImageFile.MarkerEllipse);
                    break;
                case ThermalImage.MarkerType.Polygon:
                    mlvMarkers.UpdatePolygon(e.Marker as ThermalImageFile.MarkerPolygon);
                    break;
                case ThermalImage.MarkerType.Polyline:
                    mlvMarkers.UpdatePolyline(e.Marker as ThermalImageFile.MarkerPolyline);
                    break;
            }
        }

        private void Image_MarkerRemoved(object sender, ThermalImageFile.MarkerEventArgs e)
        {
            pbThermalImage.Image = BuildImage();

            mlvMarkers.RemoveMarker(e.Marker);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == MediaPlayer.MM_MCINOTIFY)
            {
                EnableAudioAnnotation();
            }
            base.WndProc(ref m);
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofdThermalFile = new OpenFileDialog())
            {
                ofdThermalFile.Filter = "Thermal Files(*.jpg,*.irsx)|*.jpg;*.irsx";
                if (ofdThermalFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        Image.Open(ofdThermalFile.FileName);
                        DoGUIRefresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("An error occurred when trying to open the selected file:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                Image.Close();
                DoGUIRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred when trying to open the selected file:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void tsmiRotate_Click(object sender, EventArgs e)
        {
            Image.Rotate(RotateType.Rotate90);
        }

        private void tsmiHorizontalMirror_Click(object sender, EventArgs e)
        {
            Image.Mirror(MirrorType.MirrorHorizontal);
        }

        private void tsmiVerticalMirror_Click(object sender, EventArgs e)
        {
            Image.Mirror(MirrorType.MirrorVertical);
        }

        private void DoGUIRefresh()
        {
            if (Image.IsValid)
            {
                if (Image.FileType == FileType.ThermalVideo)
                {
                    FrameIndex = 0;
                    FrameCount = Image.GetFrameCount();
                    tPlay.Interval = (int)(1000 / Image.GetFrameRate());
                    tPlay.Enabled = false;

                    btnPreviousFrame.Visible = true;
                    btnPlay.Visible = true;
                    btnPause.Visible = true;
                    btnStop.Visible = true;
                    nudGoto.Visible = true;
                    btnGoto.Visible = true;
                    lblFromFrame.Visible = true;
                    nudFromFrame.Visible = true;
                    lblToFrame.Visible = true;
                    nudToFrame.Visible = true;
                    btnClip.Visible = true;
                    btnPlay.Enabled = true;
                    btnPause.Enabled = false;
                    btnStop.Enabled = false;
                    btnNextFrame.Visible = true;
                }
                else
                {
                    btnPreviousFrame.Visible = false;
                    btnPlay.Visible = false;
                    btnPause.Visible = false;
                    btnStop.Visible = false;
                    nudGoto.Visible = false;
                    btnGoto.Visible = false;
                    btnNextFrame.Visible = false;
                    lblFromFrame.Visible = false;
                    nudFromFrame.Visible = false;
                    lblToFrame.Visible = false;
                    nudToFrame.Visible = false;
                    btnClip.Visible = false;
                }

                bool oldValue = IsGUIRefreshing;
                IsGUIRefreshing = true;
                ThermalParams irParams = Image.GetThermalParams();
                nudEmissivity.Value = (decimal)irParams.Emissivity;
                nudReflTemp.Value = (decimal)(irParams.ReflTemp - CELSIUS_0);
                nudDistance.Value = (decimal)irParams.Distance;
                nudAtmTemp.Value = (decimal)(irParams.AtmTemp - CELSIUS_0);
                nudRelHumidity.Value = (decimal)(irParams.RelHumidity * PERCENT_100);
                nudExtOptTrans.Value = (decimal)irParams.ExtOptTrans;
                nudExtOptTemp.Value = (decimal)(irParams.ExtOptTemp - CELSIUS_0);

                Reference irReference = Image.GetReference();
                cbIsRefEnabled.Checked = irReference.Enabled;
                ThermalImageFile.MarkerCollection markers = Image.Markers;
                cbRefName.Items.Clear();
                cbRefName.Items.Add(string.Empty);
                for (int i = 1; i < markers.Count; i++)
                {
                    cbRefName.Items.Add(markers[i].Name);
                }
                cbRefName.SelectedItem = irReference.Name;
                switch (irReference.ValueType)
                {
                    case ThermalImage.ValueType.Max:
                        cbRefValue.SelectedItem = "Max";
                        break;
                    case ThermalImage.ValueType.Min:
                        cbRefValue.SelectedItem = "Min";
                        break;
                    case ThermalImage.ValueType.Avg:
                        cbRefValue.SelectedItem = "Avg";
                        break;
                }
                nudRefTemp.Value = (decimal)(irReference.Temp - CELSIUS_0);

                if (irReference.Enabled)
                {
                    float referenceTemperature = Image.GetReferenceTemperature();
                    nudRealRefTemp.Value = (decimal)(referenceTemperature - CELSIUS_0);
                }

                FillProperties();

                Player.Stop();
                CleanUpAudioAnnotation();
                EnableAudioAnnotation();

                tbNoteAnnotation.Text = Image.GetNoteAnnotation();

                tbEditTag.Text = string.Empty;
                tbAddTag.Text = string.Empty;
                FillTagAnnotation();

                rtbEditUserData.Text = string.Empty;
                tbAddUserDataKey.Text = string.Empty;
                rtbAddUserDataValue.Text = string.Empty;
                FillUserData();

                mlvMarkers.Items.Clear();
                markers = Image.Markers;
                for (int i = 0; i < markers.Count; i++)
                {
                    ThermalImageFile.Marker marker = markers[i];
                    switch (marker.Type)
                    {
                        case ThermalImage.MarkerType.Spot:
                            mlvMarkers.AddSpot(marker as ThermalImageFile.MarkerSpot);
                            break;
                        case ThermalImage.MarkerType.Rectangle:
                            mlvMarkers.AddRectangle(marker as ThermalImageFile.MarkerRectangle);
                            break;
                        case ThermalImage.MarkerType.Ellipse:
                            mlvMarkers.AddEllipse(marker as ThermalImageFile.MarkerEllipse);
                            break;
                        case ThermalImage.MarkerType.Polygon:
                            mlvMarkers.AddPolygon(marker as ThermalImageFile.MarkerPolygon);
                            break;
                        case ThermalImage.MarkerType.Polyline:
                            mlvMarkers.AddPolyline(marker as ThermalImageFile.MarkerPolyline);
                            break;
                    }
                }
                ThermalImageFile.DeltaCollection deltas = Image.Deltas;
                for (int i = 0; i < deltas.Count; i++)
                {
                    ThermalImageFile.Delta delta = deltas[i];
                    mlvMarkers.AddDelta(delta);
                }

                pbCoverImage.Image = Image.GetCoverImage();
                pbVisualImage.Image = Image.GetVisualImage();

                Gain gain = Image.GetGain();
                switch (gain.Mode)
                {
                    case GainMode.Fixed:
                    case GainMode.Touch:
                        nudMax.Value = (decimal)(gain.Max - CELSIUS_0);
                        nudMin.Value = (decimal)(gain.Min - CELSIUS_0);
                        cbFixedSpan.Checked = gain.FixedSpan;
                        nudSpan.Value = (decimal)gain.Span;
                        break;
                    case GainMode.Auto:
                    default:
                        nudMax.Value = (decimal)(gain.Max - CELSIUS_0);
                        nudMin.Value = (decimal)(gain.Min - CELSIUS_0);
                        cbFixedSpan.Checked = gain.FixedSpan;
                        nudSpan.Value = (decimal)gain.Span;
                        break;
                }

                AlarmV2 alarm = Image.GetAlarmV2();
                tscbAlarm.SelectedItem = alarm.Type;
                cbAlarmCustom.Checked = alarm.Custom;
                btnAlarmColor.BackColor = Color.FromArgb(alarm.Color.R, alarm.Color.G, alarm.Color.B);
                nudAlarmAlpha.Value = alarm.Color.A;
                nudAlarmMax.Value = (decimal)(alarm.Max - CELSIUS_0);
                nudAlarmMin.Value = (decimal)(alarm.Min - CELSIUS_0);
                nudRHLimit.Value = (decimal)(alarm.RHLimit * PERCENT_100);
                nudIndoorTemp.Value = (decimal)(alarm.IndoorTemp - CELSIUS_0);
                nudOutdoorTemp.Value = (decimal)(alarm.OutdoorTemp - CELSIUS_0);
                nudHeatIndex.Value = (decimal)alarm.HeatIndex;
                nudThreshold.Value = (decimal)(alarm.Threshold - CELSIUS_0);

                Bitmap bmp = new Bitmap(pbPalleteImage.Width, pbPalleteImage.Height);
                Image paletteImage = Image.GetPaletteImage();
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
                }
                pbPalleteImage.Image = bmp;

                PaletteV2 palette = Image.GetPaletteV2();
                tscbPalette.SelectedItem = new PresetPaletteItem() { RefNo = palette.RefNo, Name = string.Empty };
                if (palette.Inverted)
                {
                    tsbInverted.BackColor = SystemColors.Highlight;
                }
                else
                {
                    tsbInverted.BackColor = SystemColors.Control;
                }

                bmp = new Bitmap(PALETTE_ICON_WIDTH, PALETTE_ICON_HEIGHT);
                paletteImage = ThermalImageFile.GetPresetPaletteImage(palette.RefNo);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
                }
                tsbPalette.Image = bmp;

                ColorDistributionType colorDistributionType = Image.GetColorDistribution();
                tscbColorDistributionType.SelectedItem = colorDistributionType;

                ViewInfo viewInfo = Image.GetViewInfo();
                tscbViewType.SelectedItem = viewInfo.Type;
                nudViewAlpha.Value = viewInfo.Alpha;
                nudViewMax.Value = (decimal)(viewInfo.Max - CELSIUS_0);
                nudViewMin.Value = (decimal)(viewInfo.Min - CELSIUS_0);
                nudViewAreaX.Value = (decimal)viewInfo.Area.X;
                nudViewAreaY.Value = (decimal)viewInfo.Area.Y;
                nudViewAreaWidth.Value = (decimal)viewInfo.Area.Width;
                nudViewAreaHeight.Value = (decimal)viewInfo.Area.Height;

                pbThermalImage.Image = BuildImage();
                IsGUIRefreshing = oldValue;
            }
            else
            {
                tPlay.Enabled = false;

                bool oldValue = IsGUIRefreshing;
                IsGUIRefreshing = true;
                nudEmissivity.Value = 1.0M;
                nudReflTemp.Value = 20.0M;
                nudDistance.Value = 1.0M;
                nudAtmTemp.Value = 20.0M;
                nudRelHumidity.Value = 50M;
                nudExtOptTrans.Value = 1.0M;
                nudExtOptTemp.Value = 20.0M;

                cbIsRefEnabled.Checked = false;
                cbRefName.Items.Clear();
                cbRefName.SelectedItem = string.Empty;
                cbRefValue.SelectedItem = "Max";
                nudRefTemp.Value = 20.0M;

                lvProperties.Items.Clear();

                Player.Stop();
                CleanUpAudioAnnotation();

                tbNoteAnnotation.Text = string.Empty;

                tbEditTag.Text = string.Empty;
                tbAddTag.Text = string.Empty;
                lvTagAnnotation.Items.Clear();

                rtbEditUserData.Text = string.Empty;
                tbAddUserDataKey.Text = string.Empty;
                rtbAddUserDataValue.Text = string.Empty;
                lvUserData.Items.Clear();

                mlvMarkers.Items.Clear();

                pbCoverImage.Image = null;
                pbVisualImage.Image = null;

                nudMax.Value = 5000.0M;
                nudMin.Value = -300.0M;
                cbFixedSpan.Checked = false;
                nudSpan.Value = 2.0M;

                tscbAlarm.SelectedItem = null;
                cbAlarmCustom.Checked = false;
                btnAlarmColor.BackColor = SystemColors.Control;
                nudAlarmAlpha.Value = 0M;
                nudAlarmMax.Value = 1000.0M;
                nudAlarmMin.Value = 500.0M;
                nudRHLimit.Value = 100M;
                nudIndoorTemp.Value = 1000.0M;
                nudOutdoorTemp.Value = 500.0M;
                nudHeatIndex.Value = 0.8M;
                nudThreshold.Value = 800M;

                pbPalleteImage.Image = null;

                tsbPalette.Image = null;
                tscbPalette.SelectedItem = null;
                tsbInverted.BackColor = SystemColors.Control;

                tscbColorDistributionType.SelectedItem = null;

                tscbViewType.SelectedItem = null;
                nudViewAlpha.Value = 0M;
                nudViewMax.Value = 1000.0M;
                nudViewMin.Value = 500.0M;
                nudViewAreaX.Value = 0.000M;
                nudViewAreaY.Value = 0.000M;
                nudViewAreaWidth.Value = 0.000M;
                nudViewAreaHeight.Value = 0.000M;

                pbThermalImage.Image = null;
                IsGUIRefreshing = oldValue;
            }
        }

        private Bitmap BuildImage()
        {
            Bitmap tmpImage = Image.GetThermalImage();

            if (tmpImage == null)
            {
                return null;
            }

            Bitmap bitmap = (Bitmap)tmpImage.Clone();
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Overlay overlay = new Overlay(Image);
                overlay.Draw(graphics);
            }
            return bitmap;
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void DoSave()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                FileFormat fileFormat = (FileFormat)tscbFileFormat.SelectedItem;
                Image.SetCoverImage(BuildImage());
                switch (fileFormat)
                {
                    case FileFormat.FJPG:
                        if (!Image.SaveFJPG())
                        {
                            throw new Exception("unkown error");
                        }
                        break;
                    default:
                        if (!Image.Save())
                        {
                            throw new Exception("unkown error");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occurred when trying to save the selected file:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfdThermalFile = new SaveFileDialog())
            {
                sfdThermalFile.Filter = "Thermal Files(*.jpg)|*.jpg";
                if (sfdThermalFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        FileFormat fileFormat = (FileFormat)tscbFileFormat.SelectedItem;
                        Image.SetCoverImage(BuildImage());
                        switch (fileFormat)
                        {
                            case FileFormat.FJPG:
                                Image.SaveAsFJPG(sfdThermalFile.FileName);
                                break;
                            default:
                                Image.SaveAs(sfdThermalFile.FileName);
                                break;
                        }
                        for (int i = 0; i < lvProperties.Items.Count; i++)
                        {
                            ListViewItem item = lvProperties.Items[i];
                            if (string.Equals(item.Text, "File Name"))
                            {
                                item.SubItems[1].Text = Image.FileName;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("An error occurred when trying to save the selected file:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void tsmiSaveFrameAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfdThermalFile = new SaveFileDialog())
            {
                sfdThermalFile.Filter = "Thermal Files(*.jpg)|*.jpg";
                if (sfdThermalFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        Image.SaveFrameAs(sfdThermalFile.FileName, new Bitmap(640, 480));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("An error occurred when trying to save the selected file:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void btnAutoAdjust_Click(object sender, EventArgs e)
        {
            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Gain gain = Image.GetGain();
            gain.Mode = GainMode.Auto;
            Image.SetGain(gain);
            IsGUIOperating = oldValue;
        }

        private void btnAudioPlay_Click(object sender, EventArgs e)
        {
            AudioAnnotation audioAnnotation = Image.GetAudioAnnotation();
            if (audioAnnotation.IsValid)
            {
                if (string.IsNullOrEmpty(AudioFileName))
                {
                    AudioFileName = Path.GetTempFileName();
                    FileStream fis = new FileStream(AudioFileName, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] buffer = audioAnnotation.Data;
                    fis.Write(buffer, 0, buffer.Length);
                    fis.Close();
                }
                Player.Play(AudioFileName, this);
                btnAudioPlay.Enabled = false;
                btnAudioStop.Enabled = true;
                btnAudioPause.Enabled = true;
            }
        }

        private void btnAudioPause_Click(object sender, EventArgs e)
        {
            Player.Pause();
            btnAudioPlay.Enabled = true;
            btnAudioStop.Enabled = true;
            btnAudioPause.Enabled = false;
        }

        private void btnAudioStop_Click(object sender, EventArgs e)
        {
            Player.Stop();
            btnAudioPlay.Enabled = true;
            btnAudioStop.Enabled = false;
            btnAudioPause.Enabled = false;
        }

        private void EnableAudioAnnotation()
        {
            if (!Player.IsPlaying)
            {
                AudioAnnotation audioAnnotation = Image.GetAudioAnnotation();
                btnAudioPlay.Enabled = audioAnnotation.IsValid;
                btnAudioStop.Enabled = false;
                btnAudioPause.Enabled = false;
            }
        }

        private void CleanUpAudioAnnotation()
        {
            if (string.IsNullOrEmpty(AudioFileName) == false)
            {
                try
                {
                    File.Delete(AudioFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred when trying to clean up voice annotations:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                AudioFileName = string.Empty;
            }
        }

        private void FillProperties()
        {
            lvProperties.Items.Clear();

            CameraInfo irCameraInfo = Image.GetCameraInfo();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Model", irCameraInfo.Model }, lvProperties.Groups["lvgCameraInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Serial No.", irCameraInfo.SerialNo }, lvProperties.Groups["lvgCameraInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Company", irCameraInfo.Company }, lvProperties.Groups["lvgCameraInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Brand", irCameraInfo.Brand }, lvProperties.Groups["lvgCameraInfo"]));
            pbLogo.Image = irCameraInfo.Logo;

            LensInfoV2 irLensInfo = Image.GetLensInfoV2();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Name", irLensInfo.Name }, lvProperties.Groups["lvgLensInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Focal Length", irLensInfo.FocalLength.ToString("F1") + "mm" }, lvProperties.Groups["lvgLensInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "HFov", irLensInfo.HFov.ToString("F1") + "°" }, lvProperties.Groups["lvgLensInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Band", irLensInfo.Band }, lvProperties.Groups["lvgLensInfo"]));

            TempRange irTempRange = Image.GetTempRange();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Temp. Range", (irTempRange.Min - CELSIUS_0).ToString("F0") + "℃~" + (irTempRange.Max - CELSIUS_0).ToString("F0") + "℃" }, lvProperties.Groups["lvgTempRange"]));

            Size thermalSize = Image.GetThermalSize();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Thermal Size", thermalSize.Width.ToString() + "*" + thermalSize.Height.ToString() }, lvProperties.Groups["lvgSize"]));
            MappingInfo mappingInfo = Image.GetMappingInfo();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Visual Size", mappingInfo.VisualSize.Width.ToString() + "*" + mappingInfo.VisualSize.Height.ToString() }, lvProperties.Groups["lvgSize"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Mapping Area", "(" + mappingInfo.MappingArea.X.ToString("F3") + "," + mappingInfo.MappingArea.Y.ToString("F3") + "," + mappingInfo.MappingArea.Width.ToString("F3") + "," + mappingInfo.MappingArea.Height.ToString("F3") + ")" }, lvProperties.Groups["lvgSize"]));
            ZoomInfo zoomInfo = Image.GetZoomInfo();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Zoom Enabled", zoomInfo.Enabled ? "True" : "False" }, lvProperties.Groups["lvgSize"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Zoom Location", "(" + zoomInfo.Location.X.ToString("F3") + "," + zoomInfo.Location.Y.ToString("F3") + ")" }, lvProperties.Groups["lvgSize"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Zoom Size", zoomInfo.Ratio.ToString("F1") }, lvProperties.Groups["lvgSize"]));

            GeoLocation irGeoLocation = Image.GetGeoLocation();
            lvProperties.Items.Add(new ListViewItem(new string[] { "GPS Valid", irGeoLocation.IsGPSValid ? "True" : "False" }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Longitude", irGeoLocation.Longitude.ToString() }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Latitude", irGeoLocation.Latitude.ToString() }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Altitude", irGeoLocation.Altitude.ToString() }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Compass Valid", irGeoLocation.IsCompassValid ? "True" : "False" }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Direction", irGeoLocation.Direction.ToString() }, lvProperties.Groups["lvgGeoLocation"]));

            ImageEnhancementType irEnhancementType = Image.GetImageEnhancement();
            lvProperties.Items.Add(new ListViewItem(new string[] { "IREdge", ((irEnhancementType & ImageEnhancementType.IREdge) == ImageEnhancementType.IREdge) ? "True" : "False" }, lvProperties.Groups["lvgImageEnhancement"]));

            lvProperties.Items.Add(new ListViewItem(new string[] { "File Name", Image.FileName }, lvProperties.Groups["lvgFileInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "File Version", Image.FileVersion.ToString() }, lvProperties.Groups["lvgFileInfo"]));
            string fileType = string.Empty;
            switch (Image.FileType)
            {
                case FileType.Other:
                    fileType = "Other";
                    break;
                case FileType.ThermalPicture:
                    fileType = "Thermal Picture";
                    break;
                case FileType.ThermalVideo:
                    fileType = "Thermal Video";
                    break;
                case FileType.VisualPicture:
                    fileType = "Visual Picture";
                    break;
                case FileType.GridThermalPicture:
                    fileType = "Grid Thermal Picture";
                    break;
            }
            lvProperties.Items.Add(new ListViewItem(new string[] { "File Type", fileType }, lvProperties.Groups["lvgFileInfo"]));
            DateTime irShotTime = Image.GetShotTime();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Shot Time", irShotTime.ToString() }, lvProperties.Groups["lvgFileInfo"]));
            DateTime irModifiedTime = Image.GetModifiedTime();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Modified Time", irModifiedTime.ToString() }, lvProperties.Groups["lvgFileInfo"]));
        }

        private void FillTagAnnotation()
        {
            lvTagAnnotation.Items.Clear();
            string[] tag = Image.GetTagAnnotation();
            for (int i = 0; i < tag.Length; i++)
            {
                ListViewItem item = new ListViewItem(tag[i]);
                lvTagAnnotation.Items.Add(item);
            }
        }

        private void FillUserData()
        {
            lvUserData.Items.Clear();
            string[] keys = Image.GetUserDataKeys();
            string value = string.Empty;
            for (int i = 0; i < keys.Length; i++)
            {
                value = Image.GetUserData(keys[i]);
                ListViewItem item = new ListViewItem(new string[] { keys[i], value });
                lvUserData.Items.Add(item);
            }
        }

        private void btnEditTag_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbEditTag.Text))
            {
                if (lvTagAnnotation.SelectedItems.Count > 0)
                {
                    ListViewItem item = lvTagAnnotation.SelectedItems[0];
                    item.Text = tbEditTag.Text;
                }

                string[] tag = new string[lvTagAnnotation.Items.Count];
                for (int i = 0; i < lvTagAnnotation.Items.Count; i++)
                {
                    ListViewItem item = lvTagAnnotation.Items[i];
                    tag[i] = item.Text;
                }

                Image.SetTagAnnotation(tag);

                FillTagAnnotation();
            }
        }

        private void btnAddTag_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbAddTag.Text))
            {
                ListViewItem item = new ListViewItem(tbAddTag.Text);
                lvTagAnnotation.Items.Add(item);

                string[] tag = new string[lvTagAnnotation.Items.Count];
                for (int i = 0; i < lvTagAnnotation.Items.Count; i++)
                {
                    item = lvTagAnnotation.Items[i];
                    tag[i] = item.Text;
                }

                Image.SetTagAnnotation(tag);

                FillTagAnnotation();
            }
        }

        private void lvTagAnnotation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (lvTagAnnotation.SelectedIndices.Count > 0)
                {
                    ListView.SelectedListViewItemCollection items = lvTagAnnotation.SelectedItems;
                    if (items.Count > 0)
                    {
                        ListViewItem item = items[0];
                        lvTagAnnotation.Items.Remove(item);
                    }

                    string[] tag = new string[lvTagAnnotation.Items.Count];
                    for (int i = 0; i < lvTagAnnotation.Items.Count; i++)
                    {
                        ListViewItem item = lvTagAnnotation.Items[i];
                        tag[i] = item.Text;
                    }

                    Image.SetTagAnnotation(tag);

                    FillTagAnnotation();
                }
            }
        }

        private void lvTagAnnotation_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item != null)
            {
                tbEditTag.Text = e.Item.Text;
            }
            else
            {
                tbEditTag.Text = string.Empty;
            }
        }

        private void btnEditUserData_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbEditUserData.Text))
            {
                if (lvUserData.SelectedItems.Count > 0)
                {
                    ListViewItem item = lvUserData.SelectedItems[0];
                    string key = item.Text.Trim();
                    string value = rtbEditUserData.Text.Trim();
                    Image.SetUserData(key, value);
                    item.SubItems[1].Text = value;
                }
            }
        }

        private void btnAddUserData_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbAddUserDataKey.Text) && !string.IsNullOrEmpty(rtbAddUserDataValue.Text))
            {
                string key = tbAddUserDataKey.Text.Trim();
                string value = rtbAddUserDataValue.Text.Trim();
                string[] keys = Image.GetUserDataKeys();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == key)
                    {
                        return;
                    }
                }

                Image.SetUserData(key, value);
                ListViewItem item = new ListViewItem(new string[] { key, value });
                lvUserData.Items.Add(item);
            }
        }

        private void lvUserData_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (lvUserData.SelectedIndices.Count > 0)
                {
                    ListView.SelectedListViewItemCollection items = lvUserData.SelectedItems;
                    if (items.Count > 0)
                    {
                        ListViewItem item = items[0];
                        string key = item.Text.Trim();
                        Image.SetUserData(key, string.Empty);
                        lvUserData.Items.Remove(item);
                    }
                }
            }
        }

        private void lvUserData_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item != null)
            {
                rtbEditUserData.Text = e.Item.SubItems[1].Text;
            }
            else
            {
                rtbEditUserData.Text = string.Empty;
            }
        }

        private void tsbSelect_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.None;
            tsbSelect.Checked = true;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            tsbLine.Checked = false;
            tsbDelta.Checked = false;
            tsbDelete.Checked = false;
            Adding = false;
            Deleting = false;

            Cursor = Cursors.Default;
        }

        private void tsbSpot_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Spot;
            tsbSelect.Checked = false;
            tsbSpot.Checked = true;
            tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            tsbLine.Checked = false;
            tsbDelta.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbRectangle_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Rectangle;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = true;
            tsbEllipse.Checked = false;
            tsbLine.Checked = false;
            tsbDelta.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbEllipse_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Ellipse;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbEllipse.Checked = true;
            tsbLine.Checked = false;
            tsbDelta.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbLine_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Line;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            tsbLine.Checked = true;
            tsbDelta.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }


        private void tsbDelta_Click(object sender, EventArgs e)
        {
            if (Image.Markers.Count == 0)
            {
                return;
            }

            DeltaParams deltaParams = new DeltaParams();
            Random r = new Random();
            int srcIndex = r.Next(0, Image.Markers.Count + 1);
            int destIndex = r.Next(0, Image.Markers.Count + 1);
            int srcType = r.Next(1, 4);
            int destType = r.Next(1, 4);
            if (srcIndex != 0)
            {
                ThermalImageFile.Marker marker = Image.Markers[srcIndex - 1];
                deltaParams.SrcName = marker.Name;
                deltaParams.SrcValueType = (ThermalImage.ValueType)srcType;
                deltaParams.SrcTemp = 293.15F;
            }
            else
            {
                deltaParams.SrcName = string.Empty;
                deltaParams.SrcValueType = (ThermalImage.ValueType)srcType;
                deltaParams.SrcTemp = 293.15F;
            }
            if (destIndex != 0)
            {
                ThermalImageFile.Marker marker = Image.Markers[destIndex - 1];
                deltaParams.DestName = marker.Name;
                deltaParams.DestValueType = (ThermalImage.ValueType)destType;
                deltaParams.DestTemp = 293.15F;
            }
            else
            {
                deltaParams.DestName = string.Empty;
                deltaParams.DestValueType = (ThermalImage.ValueType)destType;
                deltaParams.DestTemp = 293.15F;
            }
            ThermalImageFile.Delta delta = new ThermalImageFile.Delta(Image, Image.Deltas.NextDeltaName, deltaParams);
            Image.Deltas.Add(delta);

            CurrentMarkerType = MarkerType.None;
            Adding = false;
            Deleting = false;

            tsbSelect.Checked = true;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            tsbLine.Checked = false;
            tsbDelta.Checked = false;
            tsbDelete.Checked = false;
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Line;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            tsbLine.Checked = false;
            tsbDelta.Checked = false;
            tsbDelete.Checked = true;
            Adding = false;
            Deleting = true;
        }

        private void mlvMarkers_KeyUp(object sender, KeyEventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            if (mlvMarkers.SelectedItems[0].Tag is ThermalImageFile.Marker)
            {
                ThermalImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Marker;
                if (string.Equals(marker.Name, IRtekFileSDK.GLOBAL_MARKER))
                {
                    return;
                }

                if (e.KeyCode == Keys.Delete)
                {
                    if (mlvMarkers.SelectedItems.Count == 1)
                    {
                        Image.Markers.Remove(marker.Name);
                    }
                }
            }
            else if (mlvMarkers.SelectedItems[0].Tag is ThermalImageFile.Delta)
            {
                ThermalImageFile.Delta delta = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Delta;
                if (e.KeyCode == Keys.Delete)
                {
                    if (mlvMarkers.SelectedItems.Count == 1)
                    {
                        Image.Deltas.Remove(delta.Name);
                    }
                }
            }
        }

        private bool HitTest(Point point, ref string name, ref MarkerType markerType, ref TrackerHitType hitType)
        {
            ThermalImageFile.MarkerCollection markers = Image.Markers;
            for (int i = 1; i < markers.Count; i++)
            {
                ThermalImageFile.Marker marker = markers[i];
                switch (marker.Type)
                {
                    case ThermalImage.MarkerType.Spot:
                        ThermalImageFile.MarkerSpot markerSpot = marker as ThermalImageFile.MarkerSpot;
                        Point spotPoint = markerSpot.Point;
                        Rectangle rect = new Rectangle(spotPoint.X - 5, spotPoint.Y - 5, 10, 10);
                        if (IsInsideRect(rect, point))
                        {
                            Cursor = Cursors.SizeAll;

                            name = marker.Name;
                            markerType = MarkerType.Spot;
                            hitType = TrackerHitType.hitMiddle;

                            return true;
                        }
                        break;
                    case ThermalImage.MarkerType.Polyline:
                        ThermalImageFile.MarkerPolyline markerPolyline = marker as ThermalImageFile.MarkerPolyline;
                        if (markerPolyline.Points.Length != 2)
                        {
                            continue;
                        }

                        Point startPoint = markerPolyline.Points[0];
                        Point endPoint = markerPolyline.Points[1];
                        // Check last point
                        Rectangle hitArea = PadHitRect(endPoint);
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeAll;

                            name = marker.Name;
                            markerType = MarkerType.Line;
                            hitType = TrackerHitType.hitEnd;

                            return true;
                        }

                        // Check first point
                        hitArea = PadHitRect(startPoint);
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeAll;

                            name = marker.Name;
                            markerType = MarkerType.Line;
                            hitType = TrackerHitType.hitStart;

                            return true;
                        }

                        // Check if we hit the actual line
                        Rectangle rc = new Rectangle(new Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y)), new Size(Math.Max(Math.Abs(startPoint.X - endPoint.X), 1), Math.Max(Math.Abs(startPoint.Y - endPoint.Y), 1)));
                        if (IsInsideRect(rc, point))
                        {
                            float dist = GetDistanceFromLine(startPoint, endPoint, point);
                            if (dist < 5.0f && dist > -5.0f)
                            {
                                Cursor = Cursors.SizeAll;

                                name = marker.Name;
                                markerType = MarkerType.Line;
                                hitType = TrackerHitType.hitEdge;

                                return true;
                            }
                        }
                        break;
                    case ThermalImage.MarkerType.Rectangle:
                        ThermalImageFile.MarkerRectangle markerRectangle = marker as ThermalImageFile.MarkerRectangle;
                        rect = markerRectangle.Rectangle;
                        // Check upper left corner
                        hitArea = PadHitRect(rect.Location);
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNWSE;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitTopLeft;

                            return true;
                        }

                        // Check upper mid
                        hitArea = PadHitRect(new Point(rect.X + rect.Width / 2, rect.Y));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNS;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitTop;

                            return true;
                        }

                        // Check upper right
                        hitArea = PadHitRect(new Point(rect.X + rect.Width, rect.Y));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNESW;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitTopRight;

                            return true;
                        }

                        // Check mid right
                        hitArea = PadHitRect(new Point(rect.X + rect.Width, rect.Y + rect.Height / 2));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeWE;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitRight;

                            return true;
                        }

                        // Check lower right
                        hitArea = PadHitRect(new Point(rect.X + rect.Width, rect.Y + rect.Height));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNWSE;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitBottomRight;

                            return true;
                        }

                        // Check lower mid
                        hitArea = PadHitRect(new Point(rect.X + rect.Width / 2, rect.Y + rect.Height));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNS;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitBottom;

                            return true;
                        }

                        // Check lower left
                        hitArea = PadHitRect(new Point(rect.X, rect.Y + rect.Height));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNESW;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitBottomLeft;

                            return true;
                        }

                        // Check left mid
                        hitArea = PadHitRect(new Point(rect.X, rect.Y + rect.Height / 2));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeWE;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitLeft;

                            return true;
                        }

                        // Check mid
                        if (IsInsideRect(new Rectangle(rect.Location, new Size(rect.Width, rect.Height)), point))
                        {
                            Cursor = Cursors.SizeAll;

                            name = marker.Name;
                            markerType = MarkerType.Rectangle;
                            hitType = TrackerHitType.hitMiddle;

                            return true;
                        }
                        break;
                    case ThermalImage.MarkerType.Ellipse:
                        ThermalImageFile.MarkerEllipse markerEllipse = marker as ThermalImageFile.MarkerEllipse;
                        rect = markerEllipse.Rectangle;
                        // Check upper left corner
                        hitArea = PadHitRect(rect.Location);
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNWSE;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitTopLeft;

                            return true;
                        }

                        // Check upper mid
                        hitArea = PadHitRect(new Point(rect.X + rect.Width / 2, rect.Y));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNS;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitTop;

                            return true;
                        }

                        // Check upper right
                        hitArea = PadHitRect(new Point(rect.X + rect.Width, rect.Y));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNESW;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitTopRight;

                            return true;
                        }

                        // Check mid right
                        hitArea = PadHitRect(new Point(rect.X + rect.Width, rect.Y + rect.Height / 2));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeWE;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitRight;

                            return true;
                        }

                        // Check lower right
                        hitArea = PadHitRect(new Point(rect.X + rect.Width, rect.Y + rect.Height));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNWSE;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitBottomRight;

                            return true;
                        }

                        // Check lower mid
                        hitArea = PadHitRect(new Point(rect.X + rect.Width / 2, rect.Y + rect.Height));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNS;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitBottom;

                            return true;
                        }

                        // Check lower left
                        hitArea = PadHitRect(new Point(rect.X, rect.Y + rect.Height));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeNESW;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitBottomLeft;

                            return true;
                        }

                        // Check left mid
                        hitArea = PadHitRect(new Point(rect.X, rect.Y + rect.Height / 2));
                        if (IsInsideRect(hitArea, point))
                        {
                            Cursor = Cursors.SizeWE;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitLeft;

                            return true;
                        }

                        // Check mid
                        if (IsInsideRect(new Rectangle(rect.Location, new Size(rect.Width, rect.Height)), point))
                        {
                            Cursor = Cursors.SizeAll;

                            name = marker.Name;
                            markerType = MarkerType.Ellipse;
                            hitType = TrackerHitType.hitMiddle;

                            return true;
                        }
                        break;
                }
            }

            Cursor = Cursors.Default;

            name = string.Empty;
            markerType = MarkerType.None;
            hitType = TrackerHitType.hitNothing;

            return false;
        }

        private void TrackSymbol(Point point)
        {
            if (Tracker.PrevPos.X == point.X && Tracker.PrevPos.Y == point.Y)
            {
                return;
            }

            Size thermalSize = Image.GetThermalSize();
            Rectangle imageRect = new Rectangle(0, 0, thermalSize.Width - 1, thermalSize.Height - 1);

            if (Tracker.MarkerType == MarkerType.Spot)
            {
                ThermalImageFile.MarkerCollection markers = Image.Markers;
                ThermalImageFile.MarkerSpot spot = markers[Tracker.Name] as ThermalImageFile.MarkerSpot;

                if (IsInsideRect(imageRect, point))
                {
                    spot.Point = point;
                }
                else
                {
                    // Clip against boundaries
                    Point pt = ClipPoint(point);
                    spot.Point = pt;
                }
            }
            else if (Tracker.MarkerType == MarkerType.Rectangle)
            {
                ThermalImageFile.MarkerCollection markers = Image.Markers;
                ThermalImageFile.MarkerRectangle markerRectangle = markers[Tracker.Name] as ThermalImageFile.MarkerRectangle;
                Rectangle rect = markerRectangle.Rectangle;

                Point delta = ComputeDelta(rect.Location, new Point(rect.X + rect.Width, rect.Y + rect.Height), point);

                if (Tracker.HitType == TrackerHitType.hitTopLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y + delta.Y);

                    rect.Width -= delta.X;
                    rect.Height -= delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitBottomRight;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitTopRight;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitBottomLeft;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitTop)
                {
                    rect.Location = new Point(rect.X, rect.Y + delta.Y);
                    rect.Height -= delta.Y;

                    if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitBottom;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitTopRight)
                {
                    rect.Location = new Point(rect.X, rect.Y + delta.Y);
                    rect.Width += delta.X;
                    rect.Height -= delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitBottomLeft;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitTopLeft;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitBottomRight;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitRight)
                {
                    rect.Width += delta.X;

                    if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitLeft;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitBottomRight)
                {
                    rect.Width += delta.X;
                    rect.Height += delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitTopLeft;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitBottomLeft;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitTopRight;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitBottom)
                {
                    rect.Height += delta.Y;

                    if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitTop;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitBottomLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y);
                    rect.Width -= delta.X;
                    rect.Height += delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitTopRight;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitBottomRight;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitTopLeft;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y);
                    rect.Width -= delta.X;

                    if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitRight;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitMiddle)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y + delta.Y);
                }

                markerRectangle.Rectangle = rect;
                Tracker.HitPos = AddPoints(Tracker.HitPos, delta);
            }
            else if (Tracker.MarkerType == MarkerType.Ellipse)
            {
                ThermalImageFile.MarkerCollection markers = Image.Markers;
                ThermalImageFile.MarkerEllipse markerEllipse = markers[Tracker.Name] as ThermalImageFile.MarkerEllipse;
                Rectangle rect = markerEllipse.Rectangle;

                Point delta = ComputeDelta(rect.Location, new Point(rect.X + rect.Width, rect.Y + rect.Height), point);

                if (Tracker.HitType == TrackerHitType.hitTopLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y + delta.Y);

                    rect.Width -= delta.X;
                    rect.Height -= delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitBottomRight;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitTopRight;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitBottomLeft;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitTop)
                {
                    rect.Location = new Point(rect.X, rect.Y + delta.Y);
                    rect.Height -= delta.Y;

                    if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitBottom;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitTopRight)
                {
                    rect.Location = new Point(rect.X, rect.Y + delta.Y);
                    rect.Width += delta.X;
                    rect.Height -= delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitBottomLeft;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitTopLeft;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitBottomRight;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitRight)
                {
                    rect.Width += delta.X;

                    if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitLeft;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitBottomRight)
                {
                    rect.Width += delta.X;
                    rect.Height += delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitTopLeft;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitBottomLeft;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitTopRight;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitBottom)
                {
                    rect.Height += delta.Y;

                    if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitTop;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitBottomLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y);
                    rect.Width -= delta.X;
                    rect.Height += delta.Y;

                    if (rect.Width < 0 && rect.Height < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        rect.Height = Math.Abs(rect.Height);

                        Tracker.HitType = TrackerHitType.hitTopRight;
                    }
                    else if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitBottomRight;
                    }
                    else if (rect.Height < 0)
                    {
                        rect.Height = Math.Abs(rect.Height);
                        Tracker.HitType = TrackerHitType.hitTopLeft;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y);
                    rect.Width -= delta.X;

                    if (rect.Width < 0)
                    {
                        rect.Width = Math.Abs(rect.Width);
                        Tracker.HitType = TrackerHitType.hitRight;
                    }
                }
                else if (Tracker.HitType == TrackerHitType.hitMiddle)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y + delta.Y);
                }

                markerEllipse.Rectangle = rect;
                Tracker.HitPos = AddPoints(Tracker.HitPos, delta);
            }
            else if (Tracker.MarkerType == MarkerType.Line)
            {
                ThermalImageFile.MarkerCollection markers = Image.Markers;
                ThermalImageFile.MarkerPolyline markerPolyline = markers[Tracker.Name] as ThermalImageFile.MarkerPolyline;
                Point[] points = markerPolyline.Points;
                Point startPoint = points[0];
                Point endPoint = points[1];

                if (Tracker.HitType == TrackerHitType.hitStart)
                {
                    startPoint = ClipPoint(point);
                }
                else if (Tracker.HitType == TrackerHitType.hitEnd)
                {
                    endPoint = ClipPoint(point);
                }
                else
                {
                    Point upperLeft = new Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y));
                    Point lowerRight = new Point(upperLeft.X + Math.Abs(startPoint.X - endPoint.X), upperLeft.Y + Math.Abs(startPoint.Y - endPoint.Y));

                    Point delta = ComputeDelta(upperLeft, lowerRight, point);

                    Point start = new Point(startPoint.X + delta.X, startPoint.Y + delta.Y);
                    Point end = new Point(endPoint.X + delta.X, endPoint.Y + delta.Y);

                    startPoint = start;
                    endPoint = end;

                    Tracker.HitPos = AddPoints(Tracker.HitPos, delta);
                }

                points[0] = startPoint;
                points[1] = endPoint;
                markerPolyline.Points = points;
            }

            Tracker.PrevPos = point;

            Update();
        }

        private Point ComputeDelta(Point upperLeft, Point lowerRight, Point pos)
        {
            Point delta = new Point();

            Size thermalSize = Image.GetThermalSize();
            Rectangle imageRect = new Rectangle(0, 0, thermalSize.Width - 1, thermalSize.Height - 1);

            if (IsInsideRect(imageRect, pos))
            {
                // Cursor point is inside of the image 
                delta = SubPoints(pos, Tracker.PrevPos);

                // Need to check against image boundaries and possibly re-calculate delta
                if (!IsInsideRect(imageRect, AddPoints(upperLeft, delta)) || !IsInsideRect(imageRect, AddPoints(lowerRight, delta)))
                {
                    Point imgTopLeft = new Point(0, 0);
                    Point imgBottomRight = new Point(thermalSize.Width - 1, thermalSize.Height - 1);

                    // Check the upper left coordinates and re-calculate the delta if needed
                    if (imgTopLeft.X > (upperLeft.X + delta.X))
                    {
                        delta.X = imgTopLeft.X - upperLeft.X;
                    }

                    if (imgTopLeft.Y > (upperLeft.Y + delta.Y))
                    {
                        delta.Y = imgTopLeft.Y - upperLeft.Y;
                    }

                    // Check the lower left coordinates and re-calculate the delta if needed
                    if (imgBottomRight.X < (lowerRight.X + delta.X))
                    {
                        delta.X = imgBottomRight.X - lowerRight.X;
                    }

                    if (imgBottomRight.Y < (lowerRight.Y + delta.Y))
                    {
                        delta.Y = imgBottomRight.Y - lowerRight.Y;
                    }
                }
            }
            else
            {
                // Cursor point is outside of image
                Point imgTopLeft = new Point(0, 0);
                Point imgBottomRight = new Point(thermalSize.Width - 1, thermalSize.Height - 1);

                // Check the upper left coordinates and re-calculate the delta if needed
                if ((pos.X < imgTopLeft.X) && (pos.Y < imgTopLeft.Y))
                {
                    // Top left
                    delta = SubPoints(imgTopLeft, upperLeft);
                }
                else if ((pos.X > imgTopLeft.X) && (pos.X < imgBottomRight.X) && (pos.Y < imgTopLeft.Y))
                {
                    // Top
                    delta = SubPoints(pos, Tracker.PrevPos);

                    if ((upperLeft.X + delta.X) < imgTopLeft.X)
                    {
                        delta.X = imgTopLeft.X - upperLeft.X;
                    }

                    if ((lowerRight.X + delta.X) > imgBottomRight.X)
                    {
                        delta.X = imgBottomRight.X - lowerRight.X;
                    }

                    delta.Y = imgTopLeft.Y - upperLeft.Y;
                }
                else if ((pos.X > imgBottomRight.X) && (pos.Y < imgTopLeft.Y))
                {
                    // Top right
                    delta.X = imgBottomRight.X - lowerRight.X;
                    delta.Y = imgTopLeft.Y - upperLeft.Y;
                }
                else if ((pos.X > imgBottomRight.X) && (pos.Y > imgTopLeft.Y) && (pos.Y < imgBottomRight.Y))
                {
                    // Right
                    delta = SubPoints(pos, Tracker.PrevPos);

                    if ((upperLeft.Y + delta.Y) < imgTopLeft.Y)
                    {
                        delta.Y = imgTopLeft.Y - upperLeft.Y;
                    }

                    if ((lowerRight.Y + delta.Y) > imgBottomRight.Y)
                    {
                        delta.Y = imgBottomRight.Y - lowerRight.Y;
                    }

                    delta.X = imgBottomRight.X - lowerRight.X;
                }
                else if ((pos.X > imgBottomRight.X) && (pos.Y > imgBottomRight.Y))
                {
                    // Down right
                    delta = SubPoints(imgBottomRight, lowerRight);
                }
                else if ((pos.X > imgTopLeft.X) && (pos.X < imgBottomRight.X) && (pos.Y > imgBottomRight.Y))
                {
                    // Down
                    delta = SubPoints(pos, Tracker.PrevPos);

                    if ((upperLeft.X + delta.X) < imgTopLeft.X)
                    {
                        delta.X = imgTopLeft.X - upperLeft.X;
                    }

                    if ((lowerRight.X + delta.X) > imgBottomRight.X)
                    {
                        delta.X = imgBottomRight.X - lowerRight.X;
                    }

                    delta.Y = imgBottomRight.Y - lowerRight.Y;
                }
                else if ((pos.X < imgTopLeft.X) && (pos.Y > imgBottomRight.Y))
                {
                    // Down left
                    delta.X = imgTopLeft.X - upperLeft.X;
                    delta.Y = imgBottomRight.Y - lowerRight.Y;
                }
                else if ((pos.X < imgTopLeft.X) && (pos.Y > imgTopLeft.Y) && (pos.Y < imgBottomRight.Y))
                {
                    // Left
                    delta = SubPoints(pos, Tracker.PrevPos);

                    if ((upperLeft.Y + delta.Y) < imgTopLeft.Y)
                    {
                        delta.Y = imgTopLeft.Y - upperLeft.Y;
                    }

                    if ((lowerRight.Y + delta.Y) > imgBottomRight.Y)
                    {
                        delta.Y = imgBottomRight.Y - lowerRight.Y;
                    }

                    delta.X = imgTopLeft.X - upperLeft.X;
                }
            }

            // Check if we're allowed to move
            if (!IsPassingMoveBoundaryX(pos))
            {
                delta.X = 0;
            }

            if (!IsPassingMoveBoundaryY(pos))
            {
                delta.Y = 0;
            }

            return delta;
        }

        private Point ClipPoint(Point pos)
        {
            Size thermalSize = Image.GetThermalSize();
            return new Point(Math.Min(Math.Max(pos.X, 0), thermalSize.Width - 1), Math.Min(Math.Max(pos.Y, 0), thermalSize.Height - 1));
        }

        private bool IsPassingMoveBoundaryX(Point pos)
        {
            if (pos.X > Tracker.PrevPos.X)
            {
                // Moving right
                if (pos.X > Tracker.HitPos.X)
                {
                    return true;
                }
            }
            else
            {
                // Moving left
                if (pos.X < Tracker.HitPos.X)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPassingMoveBoundaryY(Point pos)
        {
            if (pos.Y < Tracker.PrevPos.Y)
            {
                // Moving up
                if (pos.Y < Tracker.HitPos.Y)
                {
                    return true;
                }
            }
            else
            {
                // Moving down
                if (pos.Y > Tracker.HitPos.Y)
                {
                    return true;
                }
            }

            return false;
        }

        private void pbThermalImage_MouseDown(object sender, MouseEventArgs e)
        {
            ZoomOffset zoomOffset = new ZoomOffset(pbThermalImage.ClientRectangle, Image);
            Point point = zoomOffset.LogicalToImage(e.Location);

            string name = string.Empty;
            MarkerType markerType = MarkerType.None;
            TrackerHitType hitType = TrackerHitType.hitNothing;

            if (Deleting)
            {
                if (HitTest(point, ref name, ref markerType, ref hitType))
                {
                    Image.Markers.Remove(name);
                }

                CurrentMarkerType = MarkerType.None;
                Deleting = false;

                tsbSelect.Checked = true;
                tsbSpot.Checked = false;
                tsbRectangle.Checked = false;
                tsbEllipse.Checked = false;
                tsbLine.Checked = false;
                tsbDelta.Checked = false;
                tsbDelete.Checked = false;
            }

            if (Adding)
            {
                LocalThermalParams localThermalParams = new LocalThermalParams();
                ThermalParams thermalParams = Image.GetThermalParams();
                localThermalParams.Emissivity = thermalParams.Emissivity;
                localThermalParams.ReflTemp = thermalParams.ReflTemp;
                localThermalParams.Distance = thermalParams.Distance;
                switch (CurrentMarkerType)
                {
                    case MarkerType.Spot:
                        Image.Markers.Add(new ThermalImageFile.MarkerSpot(Image, Image.Markers.NextSpotName, localThermalParams, point));
                        break;
                    case MarkerType.Rectangle:
                        Rectangle rect = new Rectangle(point, new Size(1, 1));
                        Image.Markers.Add(new ThermalImageFile.MarkerRectangle(Image, Image.Markers.NextAreaName, localThermalParams, rect));
                        break;
                    case MarkerType.Ellipse:
                        rect = new Rectangle(point, new Size(1, 1));
                        Image.Markers.Add(new ThermalImageFile.MarkerEllipse(Image, Image.Markers.NextAreaName, localThermalParams, rect));
                        break;
                    case MarkerType.Line:
                        Point p2 = point;
                        p2.Offset(1, 1);
                        Image.Markers.Add(new ThermalImageFile.MarkerPolyline(Image, Image.Markers.NextPolylineName, localThermalParams, new Point[] { point, p2 }));
                        break;
                }

                CurrentMarkerType = MarkerType.None;
                Adding = false;

                tsbSelect.Checked = true;
                tsbSpot.Checked = false;
                tsbRectangle.Checked = false;
                tsbEllipse.Checked = false;
                tsbLine.Checked = false;
                tsbDelta.Checked = false;
                tsbDelete.Checked = false;
            }

            if (HitTest(point, ref name, ref markerType, ref hitType))
            {
                Tracker.IsTracking = true;
                Tracker.Name = name;
                Tracker.MarkerType = markerType;
                Tracker.HitType = hitType;
                Tracker.PrevPos = point;
                Tracker.HitPos = point;
            }
        }

        private void pbThermalImage_MouseUp(object sender, MouseEventArgs e)
        {
            Tracker.Reset();
        }

        private void pbThermalImage_MouseMove(object sender, MouseEventArgs e)
        {
            Point location = e.Location;

            ZoomOffset gainOffset = new ZoomOffset(pbThermalImage.ClientRectangle, Image);
            Point point = gainOffset.LogicalToImage(e.Location);

            if (Deleting)
            {
                Cursor = Cursors.Cross;
            }
            else
            {
                int offsetX = 10;
                int offsetY = 20;
                Size thermalSize = Image.GetThermalSize();
                location.Offset(new Point(offsetX, offsetY));
                if (point.X >= 0 && point.X < thermalSize.Width && point.Y >= 0 && point.Y < thermalSize.Height)
                {
                    Reference reference = Image.GetReference();
                    float value = Image.GetSpotValue(point);
                    ttTempValue.Show((reference.Enabled ? value : value - CELSIUS_0).ToString("F4"), pbThermalImage, location);
                }
                else
                {
                    ttTempValue.Hide(pbThermalImage);
                }

                if (!Tracker.IsTracking)
                {
                    string name = string.Empty;
                    MarkerType markerType = MarkerType.None;
                    TrackerHitType hitType = TrackerHitType.hitNothing;
                    HitTest(point, ref name, ref markerType, ref hitType);
                }
                else
                {
                    TrackSymbol(point);
                }
            }
        }

        private void pbThermalImage_MouseLeave(object sender, EventArgs e)
        {
            ttTempValue.Hide(pbThermalImage);
            Cursor = Cursors.Default;
        }

        private void nudEmissivity_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            ThermalParams thermalParams = Image.GetThermalParams();
            thermalParams.Emissivity = (float)nudEmissivity.Value;
            Image.SetThermalParams(thermalParams);
        }

        private void nudRefTemp_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            ThermalParams thermalParams = Image.GetThermalParams();
            thermalParams.ReflTemp = (float)nudReflTemp.Value + CELSIUS_0;
            Image.SetThermalParams(thermalParams);
        }

        private void nudDistance_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            ThermalParams thermalParams = Image.GetThermalParams();
            thermalParams.Distance = (float)nudDistance.Value;
            Image.SetThermalParams(thermalParams);
        }

        private void nudAtmTemp_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            ThermalParams thermalParams = Image.GetThermalParams();
            thermalParams.AtmTemp = (float)nudAtmTemp.Value + CELSIUS_0;
            Image.SetThermalParams(thermalParams);

            AlarmV2 alarm = Image.GetAlarmV2();
            if (alarm.Type == AlarmType.Humidity || alarm.Type == AlarmType.Insulation)
            {
                nudThreshold.Value = (decimal)(alarm.Threshold - CELSIUS_0);
            }
        }

        private void nudRelHumidity_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            ThermalParams thermalParams = Image.GetThermalParams();
            thermalParams.RelHumidity = (float)nudRelHumidity.Value / PERCENT_100;
            Image.SetThermalParams(thermalParams);

            AlarmV2 alarm = Image.GetAlarmV2();
            if (alarm.Type == AlarmType.Humidity || alarm.Type == AlarmType.Insulation)
            {
                nudThreshold.Value = (decimal)(alarm.Threshold - CELSIUS_0);
            }
        }

        private void nudExtOptTrans_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            ThermalParams thermalParams = Image.GetThermalParams();
            thermalParams.ExtOptTrans = (float)nudExtOptTrans.Value;
            Image.SetThermalParams(thermalParams);
        }

        private void nudExtOptTemp_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            ThermalParams thermalParams = Image.GetThermalParams();
            thermalParams.ExtOptTemp = (float)nudExtOptTemp.Value + CELSIUS_0;
            Image.SetThermalParams(thermalParams);
        }

        private void cbIsRefEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIsRefEnabled.Checked)
            {
                cbRefName.Enabled = true;
                if (string.IsNullOrEmpty(cbRefName.SelectedItem as string))
                {
                    cbRefValue.Enabled = false;
                    nudRefTemp.Enabled = true;
                }
                else
                {
                    cbRefValue.Enabled = true;
                    nudRefTemp.Enabled = false;
                }

                lblRealRefTemp.Visible = true;
                nudRealRefTemp.Visible = true;
                lblRealRefTempUnit.Visible = true;
            }
            else
            {
                cbRefName.Enabled = false;
                cbRefValue.Enabled = false;
                nudRefTemp.Enabled = false;

                lblRealRefTemp.Visible = false;
                nudRealRefTemp.Visible = false;
                lblRealRefTempUnit.Visible = false;
            }

            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Reference reference = Image.GetReference();
            reference.Enabled = cbIsRefEnabled.Checked;
            Image.SetReference(reference);
            IsGUIOperating = oldValue;
        }

        private void cbRefName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbIsRefEnabled.Checked)
            {
                if (string.IsNullOrEmpty(cbRefName.SelectedItem as string))
                {
                    cbRefValue.Enabled = false;
                    nudRefTemp.Enabled = true;
                }
                else
                {
                    cbRefValue.Enabled = true;
                    nudRefTemp.Enabled = false;
                }
            }

            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Reference reference = Image.GetReference();
            reference.Name = cbRefName.SelectedItem as string;
            Image.SetReference(reference);
            IsGUIOperating = oldValue;
        }

        private void cbRefValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Reference reference = Image.GetReference();
            switch (cbRefValue.SelectedItem as string)
            {
                case "Max":
                default:
                    reference.ValueType = ThermalImage.ValueType.Max;
                    break;
                case "Min":
                    reference.ValueType = ThermalImage.ValueType.Min;
                    break;
                case "Avg":
                    reference.ValueType = ThermalImage.ValueType.Avg;
                    break;
            }
            Image.SetReference(reference);
            IsGUIOperating = oldValue;
        }

        private void nudRefTemp2_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Reference reference = Image.GetReference();
            reference.Temp = (float)nudRefTemp.Value + CELSIUS_0;
            Image.SetReference(reference);
            IsGUIOperating = oldValue;
        }

        private void cbRefName_DropDown(object sender, EventArgs e)
        {
            ThermalImageFile.MarkerCollection markers = Image.Markers;
            cbRefName.Items.Clear();
            cbRefName.Items.Add(string.Empty);
            for (int i = 1; i < markers.Count; i++)
            {
                cbRefName.Items.Add(markers[i].Name);
            }
        }

        private void cmsMarkers_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                e.Cancel = true;
                return;
            }

            if (mlvMarkers.SelectedItems[0].Tag is ThermalImageFile.Marker)
            {
                ThermalImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Marker;
                if (string.Equals(marker.Name, IRtekFileSDK.GLOBAL_MARKER))
                {
                    tsmiRename.Visible = false;
                    tsmiLocalParameters.Visible = false;
                    tsmiDeltaParameters.Visible = false;
                    tsmiExportTemperature.Visible = true;
                }
                else
                {
                    tsmiRename.Visible = true;
                    tsmiLocalParameters.Visible = true;
                    tsmiDeltaParameters.Visible = false;
                    tsmiExportTemperature.Visible = true;
                }
                if (Image.FileType == FileType.ThermalVideo)
                {
                    tsmiExportTemperatureCurve.Visible = true;
                }
                else
                {
                    tsmiExportTemperatureCurve.Visible = false;
                }
            }
            else if (mlvMarkers.SelectedItems[0].Tag is ThermalImageFile.Delta)
            {
                tsmiRename.Visible = false;
                tsmiLocalParameters.Visible = false;
                tsmiDeltaParameters.Visible = true;
                tsmiExportTemperature.Visible = false;
                if (Image.FileType == FileType.ThermalVideo)
                {
                    tsmiExportTemperatureCurve.Visible = true;
                }
                else
                {
                    tsmiExportTemperatureCurve.Visible = false;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void tsmiRename_Click(object sender, EventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            ThermalImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Marker;
            if (marker == null)
            {
                return;
            }

            switch (marker.Type)
            {
                case ThermalImage.MarkerType.Spot:
                    marker.Name = Image.Markers.NextSpotName;
                    break;
                case ThermalImage.MarkerType.Rectangle:
                    marker.Name = Image.Markers.NextAreaName;
                    break;
                case ThermalImage.MarkerType.Ellipse:
                    marker.Name = Image.Markers.NextAreaName;
                    break;
                case ThermalImage.MarkerType.Polygon:
                    marker.Name = Image.Markers.NextAreaName;
                    break;
                case ThermalImage.MarkerType.Polyline:
                    marker.Name = Image.Markers.NextPolylineName;
                    break;
            }
        }

        private void tsmiLocalParameters_Click(object sender, EventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            ThermalImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Marker;
            if (marker == null)
            {
                return;
            }

            LocalThermalParams thermalParams = marker.ThermalParams;
            thermalParams.LocalParams = !thermalParams.LocalParams;
            marker.ThermalParams = thermalParams;
        }

        private void tsmiDeltaParameters_Click(object sender, EventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            ThermalImageFile.Delta delta = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Delta;
            if (delta == null)
            {
                return;
            }

            DeltaParams deltaParams = delta.DeltaParams;
            Random r = new Random();
            int srcIndex = r.Next(0, Image.Markers.Count + 1);
            int destIndex = r.Next(0, Image.Markers.Count + 1);
            int srcType = r.Next(1, 4);
            int destType = r.Next(1, 4);
            if (srcIndex != 0)
            {
                ThermalImageFile.Marker marker = Image.Markers[srcIndex - 1];
                deltaParams.SrcName = marker.Name;
                deltaParams.SrcValueType = (ThermalImage.ValueType)srcType;
                deltaParams.SrcTemp = 293.15F;
            }
            else
            {
                deltaParams.SrcName = string.Empty;
                deltaParams.SrcValueType = (ThermalImage.ValueType)srcType;
                deltaParams.SrcTemp = 293.15F;
            }
            if (destIndex != 0)
            {
                ThermalImageFile.Marker marker = Image.Markers[destIndex - 1];
                deltaParams.DestName = marker.Name;
                deltaParams.DestValueType = (ThermalImage.ValueType)destType;
                deltaParams.DestTemp = 293.15F;
            }
            else
            {
                deltaParams.DestName = string.Empty;
                deltaParams.DestValueType = (ThermalImage.ValueType)destType;
                deltaParams.DestTemp = 293.15F;
            }
            delta.DeltaParams = deltaParams;
        }

        private void tsmiExportTemperature_Click(object sender, EventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            ThermalImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Marker;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "SaveAs";
            sfd.Filter = "CSV File(*.csv)|*.csv";
            sfd.FileName = marker.Name + "_Temperature";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    Reference reference = Image.GetReference();
                    using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, FileMode.Create), Encoding.Default))
                    {
                        switch (marker.Type)
                        {
                            case ThermalImage.MarkerType.Spot:
                                ThermalImageFile.MarkerSpot markerSpot = marker as ThermalImageFile.MarkerSpot;

                                float value = Image.GetSpotValue(markerSpot.Point);
                                sw.Write((reference.Enabled ? value : value - CELSIUS_0).ToString("F4"));
                                break;
                            case ThermalImage.MarkerType.Rectangle:
                                ThermalImageFile.MarkerRectangle markerRectangle = marker as ThermalImageFile.MarkerRectangle;
                                int left = markerRectangle.Rectangle.Left;
                                int top = markerRectangle.Rectangle.Top;
                                int lastX = 0;
                                int lastY = 0;

                                PointValue[] values = Image.GetRectangleValues(markerRectangle.Rectangle);
                                for (int i = 0; i < values.Length; i++)
                                {
                                    Point p = values[i].Point;
                                    int x = p.X - left;
                                    int y = p.Y - top;

                                    for (int j = 0; j < y - lastY; j++)
                                    {
                                        sw.WriteLine();
                                        lastX = 0;
                                    }

                                    for (int j = 0; j < x - lastX; j++)
                                    {
                                        sw.Write(",");
                                    }

                                    sw.Write((reference.Enabled ? values[i].Temp : values[i].Temp - CELSIUS_0).ToString("F4"));

                                    lastX = x;
                                    lastY = y;
                                }
                                break;
                            case ThermalImage.MarkerType.Ellipse:
                                ThermalImageFile.MarkerEllipse markerEllipse = marker as ThermalImageFile.MarkerEllipse;
                                left = markerEllipse.Rectangle.Left;
                                top = markerEllipse.Rectangle.Top;
                                lastX = 0;
                                lastY = 0;

                                values = Image.GetEllipseValues(markerEllipse.Rectangle);
                                for (int i = 0; i < values.Length; i++)
                                {
                                    Point p = values[i].Point;
                                    int x = p.X - left;
                                    int y = p.Y - top;

                                    for (int j = 0; j < y - lastY; j++)
                                    {
                                        sw.WriteLine();
                                        lastX = 0;
                                    }

                                    for (int j = 0; j < x - lastX; j++)
                                    {
                                        sw.Write(",");
                                    }

                                    sw.Write((reference.Enabled ? values[i].Temp : values[i].Temp - CELSIUS_0).ToString("F4"));

                                    lastX = x;
                                    lastY = y;
                                }
                                break;
                            case ThermalImage.MarkerType.Polygon:
                                ThermalImageFile.MarkerPolygon markerPolygon = marker as ThermalImageFile.MarkerPolygon;
                                left = int.MaxValue;
                                top = int.MaxValue;
                                for (int i = 0; i < markerPolygon.Points.Length; i++)
                                {
                                    Point p = markerPolygon.Points[i];
                                    if (left > p.X)
                                    {
                                        left = p.X;
                                    }
                                    if (top > p.Y)
                                    {
                                        top = p.Y;
                                    }
                                }

                                lastX = 0;
                                lastY = 0;

                                values = Image.GetPolygonValues(markerPolygon.Points);
                                for (int i = 0; i < values.Length; i++)
                                {
                                    Point p = values[i].Point;
                                    int x = p.X - left;
                                    int y = p.Y - top;

                                    for (int j = 0; j < y - lastY; j++)
                                    {
                                        sw.WriteLine();
                                        lastX = 0;
                                    }

                                    for (int j = 0; j < x - lastX; j++)
                                    {
                                        sw.Write(",");
                                    }

                                    sw.Write((reference.Enabled ? values[i].Temp : values[i].Temp - CELSIUS_0).ToString("F4"));

                                    lastX = x;
                                    lastY = y;
                                }
                                break;
                            case ThermalImage.MarkerType.Polyline:
                                ThermalImageFile.MarkerPolyline markerPolyline = marker as ThermalImageFile.MarkerPolyline;
                                left = int.MaxValue;
                                top = int.MaxValue;
                                for (int i = 0; i < markerPolyline.Points.Length; i++)
                                {
                                    Point p = markerPolyline.Points[i];
                                    if (left > p.X)
                                    {
                                        left = p.X;
                                    }
                                    if (top > p.Y)
                                    {
                                        top = p.Y;
                                    }
                                }

                                lastX = 0;
                                lastY = 0;

                                values = Image.GetPolylineValues(markerPolyline.Points);
                                Array.Sort(values, (a, b) =>
                                {
                                    int compareY = a.Point.Y.CompareTo(b.Point.Y);
                                    return compareY != 0 ? compareY : a.Point.X.CompareTo(b.Point.X);
                                });
                                for (int i = 0; i < values.Length; i++)
                                {
                                    Point p = values[i].Point;
                                    int x = p.X - left;
                                    int y = p.Y - top;

                                    for (int j = 0; j < y - lastY; j++)
                                    {
                                        sw.WriteLine();
                                        lastX = 0;
                                    }

                                    for (int j = 0; j < x - lastX; j++)
                                    {
                                        sw.Write(",");
                                    }

                                    sw.Write((reference.Enabled ? values[i].Temp : values[i].Temp - CELSIUS_0).ToString("F4"));

                                    lastX = x;
                                    lastY = y;
                                }
                                break;
                        }
                    }
                    MessageBox.Show("Export to csv file succeeded", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Export to csv file failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void tsmiExportTemperatureCurve_Click(object sender, EventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            ThermalImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Marker;
            if (marker != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "SaveAs";
                sfd.Filter = "CSV File(*.csv)|*.csv";
                sfd.FileName = marker.Name + "_TemperatureCurve";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    try
                    {
                        int frameCount = Image.GetFrameCount();
                        Reference reference = Image.GetReference();
                        using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, FileMode.Create), Encoding.Default))
                        {
                            for (int i = 0; i < frameCount; i++)
                            {
                                sw.Write(i);
                                DateTime shotTime = Image.GetIndexShotTime(i);
                                sw.Write(",");
                                sw.Write(shotTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                                ThermalImageFile.Marker tmpMarker = Image.GetIndexMarker(i, marker.Name);
                                switch (tmpMarker.Type)
                                {
                                    case ThermalImage.MarkerType.Spot:
                                        ThermalImageFile.MarkerSpot tmpMarkerSpot = tmpMarker as ThermalImageFile.MarkerSpot;
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerSpot.Value : tmpMarkerSpot.Value - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerSpot.Value : tmpMarkerSpot.Value - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerSpot.Value : tmpMarkerSpot.Value - CELSIUS_0).ToString("F4"));
                                        break;
                                    case ThermalImage.MarkerType.Rectangle:
                                        ThermalImageFile.MarkerRectangle tmpMarkerRectangle = tmpMarker as ThermalImageFile.MarkerRectangle;
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerRectangle.Max : tmpMarkerRectangle.Max - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerRectangle.Min : tmpMarkerRectangle.Min - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerRectangle.Avg : tmpMarkerRectangle.Avg - CELSIUS_0).ToString("F4"));
                                        break;
                                    case ThermalImage.MarkerType.Ellipse:
                                        ThermalImageFile.MarkerEllipse tmpMarkerEllipse = tmpMarker as ThermalImageFile.MarkerEllipse;
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerEllipse.Max : tmpMarkerEllipse.Max - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerEllipse.Min : tmpMarkerEllipse.Min - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerEllipse.Avg : tmpMarkerEllipse.Avg - CELSIUS_0).ToString("F4"));
                                        break;
                                    case ThermalImage.MarkerType.Polygon:
                                        ThermalImageFile.MarkerPolygon tmpMarkerPolygon = tmpMarker as ThermalImageFile.MarkerPolygon;
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerPolygon.Max : tmpMarkerPolygon.Max - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerPolygon.Min : tmpMarkerPolygon.Min - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerPolygon.Avg : tmpMarkerPolygon.Avg - CELSIUS_0).ToString("F4"));
                                        break;
                                    case ThermalImage.MarkerType.Polyline:
                                        ThermalImageFile.MarkerPolyline tmpMarkerPolyline = tmpMarker as ThermalImageFile.MarkerPolyline;
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerPolyline.Max : tmpMarkerPolyline.Max - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerPolyline.Min : tmpMarkerPolyline.Min - CELSIUS_0).ToString("F4"));
                                        sw.Write(",");
                                        sw.Write((reference.Enabled ? tmpMarkerPolyline.Avg : tmpMarkerPolyline.Avg - CELSIUS_0).ToString("F4"));
                                        break;
                                }
                                sw.WriteLine();
                            }
                        }
                        MessageBox.Show("Export to csv file succeeded", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Export to csv file failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
                return;
            }

            ThermalImageFile.Delta delta = mlvMarkers.SelectedItems[0].Tag as ThermalImageFile.Delta;
            if (delta != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "SaveAs";
                sfd.Filter = "CSV File(*.csv)|*.csv";
                sfd.FileName = delta.Name + "_TemperatureCurve";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    try
                    {
                        int frameCount = Image.GetFrameCount();
                        using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, FileMode.Create), Encoding.Default))
                        {
                            for (int i = 0; i < frameCount; i++)
                            {
                                sw.Write(i);
                                DateTime shotTime = Image.GetIndexShotTime(i);
                                sw.Write(",");
                                sw.Write(shotTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                                ThermalImageFile.Delta tmpDelta = Image.GetIndexDelta(i, delta.Name);
                                sw.Write(",");
                                sw.Write(tmpDelta.Value.ToString("F4"));
                                sw.WriteLine();
                            }
                        }
                        MessageBox.Show("Export to csv file succeeded", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Export to csv file failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
                return;
            }
        }

        private void tsmiExportFrameAD_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "SaveAs";
            sfd.Filter = "CSV File(*.csv)|*.csv";
            sfd.FileName = "frame_AD";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, FileMode.Create), Encoding.Default))
                    {
                        Frame frame = Image.GetFrame();
                        int height = frame.Values.GetLength(0);
                        int width = frame.Values.GetLength(1);
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                if (x != 0)
                                {
                                    sw.Write(",");
                                }
                                sw.Write(frame.Values[y, x].ToString());
                            }
                            sw.WriteLine();
                        }
                    }
                    MessageBox.Show("Export to csv file succeeded", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Export to csv file failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void tsmiExportFrameTemperature_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "SaveAs";
            sfd.Filter = "CSV File(*.csv)|*.csv";
            sfd.FileName = "frame_Temperature";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, FileMode.Create), Encoding.Default))
                    {
                        LUT lut = Image.GetCalculatedLUT();
                        float[] tempArray = new float[ushort.MaxValue + 1];
                        int from = lut.ADArray[0];
                        int to = lut.ADArray[0];
                        for (int i = 0; i < lut.ADArray.Length - 1; i++)
                        {
                            int tmpFrom = lut.ADArray[i];
                            int tmpTo = lut.ADArray[i + 1];
                            to = tmpTo;
                            float fromTemp = lut.TempArray[i];
                            float toTemp = lut.TempArray[i + 1];
                            for (int j = tmpFrom; j <= tmpTo; j++)
                            {
                                tempArray[j] = fromTemp + (toTemp - fromTemp) * (j - tmpFrom) / (tmpTo - tmpFrom);
                            }
                        }
                        for (int i = 0; i < from; i++)
                        {
                            tempArray[i] = tempArray[from];
                        }
                        for (int i = to + 1; i <= ushort.MaxValue; i++)
                        {
                            tempArray[i] = tempArray[to];
                        }

                        Frame frame = Image.GetFrame();
                        int height = frame.Values.GetLength(0);
                        int width = frame.Values.GetLength(1);
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                if (x != 0)
                                {
                                    sw.Write(",");
                                }
                                sw.Write((tempArray[frame.Values[y, x]] - CELSIUS_0).ToString("F4"));
                            }
                            sw.WriteLine();
                        }
                    }
                    MessageBox.Show("Export to csv file succeeded", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("Export to csv file failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void tsmiImportFrameAD_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV File(*.csv)|*.csv";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    try
                    {
                        using (StreamReader sr = new StreamReader(File.Open(ofd.FileName, FileMode.Open), Encoding.Default))
                        {
                            Size thermalSize = Image.GetThermalSize();
                            int width = thermalSize.Width;
                            int height = thermalSize.Height;
                            Frame frame = new Frame();
                            frame.Values = new ushort[height, width];

                            for (int y = 0; y < height; y++)
                            {
                                string s = sr.ReadLine();
                                string[] ss = s.Split(',');

                                if (ss.Length != width)
                                {
                                    throw new Exception();
                                }

                                for (int x = 0; x < width; x++)
                                {
                                    frame.Values[y, x] = Convert.ToUInt16(ss[x]);
                                }
                            }

                            Bitmap tmpImage = Image.GetThermalImage(frame);
                            tmpImage.Save(Path.ChangeExtension(ofd.FileName, ".jpg"));
                        }
                        MessageBox.Show("Import from csv file succeeded", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show("Import from csv file failed", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void tbNoteAnnotation_TextChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            Image.SetNoteAnnotation(tbNoteAnnotation.Text);
        }

        private void nudMax_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Gain gain = Image.GetGain();
            gain.Max = (float)nudMax.Value + CELSIUS_0;
            if (gain.FixedSpan)
            {
                gain.Min = gain.Max - gain.Span;
            }
            gain.Mode = GainMode.Fixed;
            Image.SetGain(gain);
            IsGUIOperating = oldValue;
        }

        private void cbFixedSpan_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFixedSpan.Checked)
            {
                nudSpan.Enabled = true;
            }
            else
            {
                nudSpan.Enabled = false;
            }

            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Gain gain = Image.GetGain();
            gain.FixedSpan = cbFixedSpan.Checked;
            if (gain.FixedSpan)
            {
                float max = gain.Max;
                float min = gain.Min;
                float span = gain.Span;
                gain.Max = (max + min + span) / 2;
                gain.Min = (max + min - span) / 2;
            }
            gain.Mode = GainMode.Fixed;
            Image.SetGain(gain);
            IsGUIOperating = oldValue;
        }

        private void nudSpan_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Gain gain = Image.GetGain();
            gain.Span = (float)nudSpan.Value;
            if (gain.FixedSpan)
            {
                float max = gain.Max;
                float min = gain.Min;
                float span = gain.Span;
                gain.Max = (max + min + span) / 2;
                gain.Min = (max + min - span) / 2;

                if (gain.Min < 0)
                {
                    gain.Min = 0;
                    gain.Span = gain.Max - gain.Min;
                }
            }
            gain.Mode = GainMode.Fixed;
            Image.SetGain(gain);
            IsGUIOperating = oldValue;
        }

        private void nudMin_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Gain gain = Image.GetGain();
            gain.Min = (float)nudMin.Value + CELSIUS_0;
            if (gain.FixedSpan)
            {
                gain.Max = gain.Min + gain.Span;
            }
            gain.Mode = GainMode.Fixed;
            Image.SetGain(gain);
            IsGUIOperating = oldValue;
        }

        private void tscbPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Palette palette = Image.GetPalette();
            palette.RefNo = ((PresetPaletteItem)tscbPalette.SelectedItem).RefNo;
            if (palette.RefNo == 0xFF)
            {
                palette.Colors = USER_DEFINED_COLORS;
            }
            Image.SetPalette(palette);

            Bitmap bmp = new Bitmap(PALETTE_ICON_WIDTH, PALETTE_ICON_HEIGHT);
            if (palette.RefNo != 0xFF)
            {
                Bitmap paletteImage = ThermalImageFile.GetPresetPaletteImage(palette.RefNo);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
                }
            }
            tsbPalette.Image = bmp;
            IsGUIOperating = oldValue;
        }

        private void tsbInverted_Click(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            Palette palette = Image.GetPalette();
            palette.Inverted = !palette.Inverted;
            Image.SetPalette(palette);
        }

        private void tscbAlarm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscbAlarm.SelectedItem == null)
            {
                cbAlarmCustom.Enabled = false;
                btnAlarmColor.Enabled = false;
                nudAlarmAlpha.Enabled = false;
                pAlarmTemp.Visible = true;
                pHumidity.Visible = false;
                pInsulation.Visible = false;
                nudAlarmMax.Enabled = false;
                nudAlarmMin.Enabled = false;
                return;
            }

            AlarmType type = (AlarmType)tscbAlarm.SelectedItem;
            switch (type)
            {
                case AlarmType.Normal:
                    cbAlarmCustom.Enabled = false;
                    btnAlarmColor.Enabled = false;
                    nudAlarmAlpha.Enabled = false;
                    pAlarmTemp.Visible = true;
                    pHumidity.Visible = false;
                    pInsulation.Visible = false;
                    nudAlarmMax.Enabled = false;
                    nudAlarmMin.Enabled = false;
                    break;
                case AlarmType.Below:
                    cbAlarmCustom.Enabled = true;
                    if (cbAlarmCustom.Checked)
                    {
                        btnAlarmColor.Enabled = true;
                        nudAlarmAlpha.Enabled = true;
                    }
                    else
                    {
                        btnAlarmColor.Enabled = false;
                        nudAlarmAlpha.Enabled = false;
                    }
                    pAlarmTemp.Visible = true;
                    pHumidity.Visible = false;
                    pInsulation.Visible = false;
                    nudAlarmMax.Enabled = false;
                    nudAlarmMin.Enabled = true;
                    break;
                case AlarmType.Above:
                    cbAlarmCustom.Enabled = true;
                    if (cbAlarmCustom.Checked)
                    {
                        btnAlarmColor.Enabled = true;
                        nudAlarmAlpha.Enabled = true;
                    }
                    else
                    {
                        btnAlarmColor.Enabled = false;
                        nudAlarmAlpha.Enabled = false;
                    }
                    pAlarmTemp.Visible = true;
                    pHumidity.Visible = false;
                    pInsulation.Visible = false;
                    nudAlarmMax.Enabled = true;
                    nudAlarmMin.Enabled = false;
                    break;
                case AlarmType.Between:
                case AlarmType.Outside:
                    cbAlarmCustom.Enabled = true;
                    if (cbAlarmCustom.Checked)
                    {
                        btnAlarmColor.Enabled = true;
                        nudAlarmAlpha.Enabled = true;
                    }
                    else
                    {
                        btnAlarmColor.Enabled = false;
                        nudAlarmAlpha.Enabled = false;
                    }
                    pAlarmTemp.Visible = true;
                    pHumidity.Visible = false;
                    pInsulation.Visible = false;
                    nudAlarmMax.Enabled = true;
                    nudAlarmMin.Enabled = true;
                    break;
                case AlarmType.MixPalette:
                    cbAlarmCustom.Enabled = false;
                    btnAlarmColor.Enabled = false;
                    nudAlarmAlpha.Enabled = false;
                    pAlarmTemp.Visible = true;
                    pHumidity.Visible = false;
                    pInsulation.Visible = false;
                    nudAlarmMax.Enabled = true;
                    nudAlarmMin.Enabled = true;
                    break;
                case AlarmType.Humidity:
                    cbAlarmCustom.Enabled = true;
                    if (cbAlarmCustom.Checked)
                    {
                        btnAlarmColor.Enabled = true;
                        nudAlarmAlpha.Enabled = true;
                    }
                    else
                    {
                        btnAlarmColor.Enabled = false;
                        nudAlarmAlpha.Enabled = false;
                    }
                    pAlarmTemp.Visible = false;
                    pHumidity.Visible = true;
                    pInsulation.Visible = false;
                    nudAlarmMax.Enabled = false;
                    nudAlarmMin.Enabled = false;
                    break;
                case AlarmType.Insulation:
                    cbAlarmCustom.Enabled = true;
                    if (cbAlarmCustom.Checked)
                    {
                        btnAlarmColor.Enabled = true;
                        nudAlarmAlpha.Enabled = true;
                    }
                    else
                    {
                        btnAlarmColor.Enabled = false;
                        nudAlarmAlpha.Enabled = false;
                    }
                    pAlarmTemp.Visible = false;
                    pHumidity.Visible = false;
                    pInsulation.Visible = true;
                    nudAlarmMax.Enabled = false;
                    nudAlarmMin.Enabled = false;
                    break;
            }

            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.Type = type;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void cbAlarmCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAlarmCustom.Checked)
            {
                btnAlarmColor.Enabled = true;
                nudAlarmAlpha.Enabled = true;
            }
            else
            {
                btnAlarmColor.Enabled = false;
                nudAlarmAlpha.Enabled = false;
            }

            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.Custom = cbAlarmCustom.Checked;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void btnAlarmColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cdAlarmColor = new ColorDialog())
            {
                cdAlarmColor.Color = btnAlarmColor.BackColor;
                if (cdAlarmColor.ShowDialog() == DialogResult.OK)
                {
                    btnAlarmColor.BackColor = cdAlarmColor.Color;

                    bool oldValue = IsGUIOperating;
                    IsGUIOperating = true;
                    Alarm alarm = Image.GetAlarm();
                    alarm.Color = Color.FromArgb((int)nudAlarmAlpha.Value, btnAlarmColor.BackColor);
                    Image.SetAlarm(alarm);
                    IsGUIOperating = oldValue;
                }
            }
        }

        private void nudAlarmAlpha_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.Color = Color.FromArgb((int)nudAlarmAlpha.Value, btnAlarmColor.BackColor);
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void nudAlarmMax_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.Max = (float)nudAlarmMax.Value + CELSIUS_0;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void nudAlarmMin_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.Min = (float)nudAlarmMin.Value + CELSIUS_0;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void nudRHLimit_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.RHLimit = (float)nudRHLimit.Value / PERCENT_100;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void nudIndoorTemp_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.IndoorTemp = (float)nudIndoorTemp.Value + CELSIUS_0;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void nudOutdoorTemp_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.OutdoorTemp = (float)nudOutdoorTemp.Value + CELSIUS_0;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void nudHeatIndex_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            Alarm alarm = Image.GetAlarm();
            alarm.HeatIndex = (float)nudHeatIndex.Value;
            Image.SetAlarm(alarm);
            IsGUIOperating = oldValue;
        }

        private void tscbColorDistributionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ColorDistributionType colorDistributionType = (ColorDistributionType)tscbColorDistributionType.SelectedItem;
            Image.SetColorDistribution(colorDistributionType);
            IsGUIOperating = oldValue;
        }

        private void tscbViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscbViewType.SelectedItem == null)
            {
                nudViewAlpha.Enabled = false;
                nudViewMax.Enabled = false;
                nudViewMin.Enabled = false;
                nudViewAreaX.Enabled = false;
                nudViewAreaY.Enabled = false;
                nudViewAreaWidth.Enabled = false;
                nudViewAreaHeight.Enabled = false;
                return;
            }

            ViewType type = (ViewType)tscbViewType.SelectedItem;
            switch (type)
            {
                case ViewType.Thermal:
                default:
                    nudViewAlpha.Enabled = false;
                    nudViewMax.Enabled = false;
                    nudViewMin.Enabled = false;
                    nudViewAreaX.Enabled = false;
                    nudViewAreaY.Enabled = false;
                    nudViewAreaWidth.Enabled = false;
                    nudViewAreaHeight.Enabled = false;
                    break;
                case ViewType.PicInPic:
                    nudViewAlpha.Enabled = false;
                    nudViewMax.Enabled = false;
                    nudViewMin.Enabled = false;
                    nudViewAreaX.Enabled = true;
                    nudViewAreaY.Enabled = true;
                    nudViewAreaWidth.Enabled = true;
                    nudViewAreaHeight.Enabled = true;
                    break;
                case ViewType.T_DEF:
                    nudViewAlpha.Enabled = true;
                    nudViewMax.Enabled = false;
                    nudViewMin.Enabled = false;
                    nudViewAreaX.Enabled = false;
                    nudViewAreaY.Enabled = false;
                    nudViewAreaWidth.Enabled = false;
                    nudViewAreaHeight.Enabled = false;
                    break;
                case ViewType.Fusion:
                    nudViewAlpha.Enabled = false;
                    nudViewMax.Enabled = true;
                    nudViewMin.Enabled = true;
                    nudViewAreaX.Enabled = false;
                    nudViewAreaY.Enabled = false;
                    nudViewAreaWidth.Enabled = false;
                    nudViewAreaHeight.Enabled = false;
                    break;
            }

            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            viewInfo.Type = type;
            Image.SetViewInfo(viewInfo);
            IsGUIOperating = oldValue;
        }

        private void nudViewAlpha_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            viewInfo.Alpha = (byte)nudViewAlpha.Value;
            Image.SetViewInfo(viewInfo);
            IsGUIOperating = oldValue;
        }

        private void nudViewMax_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            viewInfo.Max = (float)nudViewMax.Value + CELSIUS_0;
            Image.SetViewInfo(viewInfo);
            IsGUIOperating = oldValue;
        }

        private void nudViewMin_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            viewInfo.Min = (float)nudViewMin.Value + CELSIUS_0;
            Image.SetViewInfo(viewInfo);
            IsGUIOperating = oldValue;
        }

        private void nudViewAreaX_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            float oldX = viewInfo.Area.X;
            RectangleF area = viewInfo.Area;
            float newX = (float)nudViewAreaX.Value;
            viewInfo.Area = new RectangleF(newX, area.Y, area.Width, area.Height);
            if (!Image.SetViewInfo(viewInfo))
            {
                nudViewAreaX.Value = (decimal)oldX;
            }
            IsGUIOperating = oldValue;
        }

        private void nudViewAreaY_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            float oldY = viewInfo.Area.Y;
            RectangleF area = viewInfo.Area;
            float newY = (float)nudViewAreaY.Value;
            viewInfo.Area = new RectangleF(area.X, newY, area.Width, area.Height);
            if (!Image.SetViewInfo(viewInfo))
            {
                nudViewAreaY.Value = (decimal)oldY;
            }
            IsGUIOperating = oldValue;
        }

        private void nudViewAreaWidth_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            float oldWidth = viewInfo.Area.Width;
            RectangleF area = viewInfo.Area;
            float newWidth = (float)nudViewAreaWidth.Value;
            viewInfo.Area = new RectangleF(area.X, area.Y, newWidth, area.Height);
            if (!Image.SetViewInfo(viewInfo))
            {
                nudViewAreaWidth.Value = (decimal)oldWidth;
            }
            IsGUIOperating = oldValue;
        }

        private void nudViewAreaHeight_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            ViewInfo viewInfo = Image.GetViewInfo();
            float oldHeight = viewInfo.Area.Height;
            RectangleF area = viewInfo.Area;
            float newHeight = (float)nudViewAreaHeight.Value;
            viewInfo.Area = new RectangleF(area.X, area.Y, area.Width, newHeight);
            if (!Image.SetViewInfo(viewInfo))
            {
                nudViewAreaHeight.Value = (decimal)oldHeight;
            }
            IsGUIOperating = oldValue;
        }

        private static bool IsInsideRect(Rectangle rect, Point pt)
        {
            bool isInside = false;

            if (pt.X >= rect.X &&
                pt.X < (rect.X + rect.Width) &&
                pt.Y >= rect.Y &&
                pt.Y < (rect.Y + rect.Height))
            {
                isInside = true;
            }

            return isInside;
        }

        private static Rectangle PadHitRect(Point pt)
        {
            return new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
        }

        private static float GetDistanceFromLine(Point start, Point end, Point pt)
        {
            if (start.X != end.X)
            {
                float m = (float)(end.Y - start.Y) / (end.X - start.X);
                float b = -1.0F;
                float c = start.Y - m * start.X;

                float x = m * pt.X + b * pt.Y + c;
                float y = (float)(Math.Sqrt(m * m + b * b));

                return x / y;
            }
            else
            {
                return (float)(pt.X - start.X);
            }
        }

        private static Point AddPoints(Point pt1, Point pt2)
        {
            return new Point(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }

        private static Point SubPoints(Point pt1, Point pt2)
        {
            return new Point(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            tPlay.Enabled = true;
            btnPlay.Enabled = false;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            tPlay.Enabled = false;
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tPlay.Enabled = false;
            FrameIndex = 0;
            Image.SetFrameIndex(FrameIndex);
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
        }

        private void btnPreviousFrame_Click(object sender, EventArgs e)
        {
            if (FrameIndex > 0)
            {
                FrameIndex--;
                Image.SetFrameIndex(FrameIndex);
            }
        }

        private void btnNextFrame_Click(object sender, EventArgs e)
        {
            if (FrameIndex < FrameCount - 1)
            {
                FrameIndex++;
                Image.SetFrameIndex(FrameIndex);
            }
        }

        private void tPlay_Tick(object sender, EventArgs e)
        {
            if (FrameIndex < FrameCount - 1)
            {
                FrameIndex++;
            }
            else
            {
                FrameIndex = 0;
            }

            Image.SetFrameIndex(FrameIndex);
        }

        private void btnClip_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfdThermalFile = new SaveFileDialog())
            {
                sfdThermalFile.Filter = "Thermal Files(*.irsx)|*.irsx";
                if (sfdThermalFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        IntPtr h = IntPtr.Zero;
                        if (Image.SaveAsStream(sfdThermalFile.FileName, ref h))
                        {
                            ThermalImageFile newImage = new ThermalImageFile(h);
                            int frameIndex = (int)nudFromFrame.Value - 1;
                            for (; frameIndex < (int)nudToFrame.Value; frameIndex++)
                            {
                                newImage.AppendFrame(Image.GetFrame(frameIndex));
                            }
                            newImage.Save();
                            newImage.Close();
                            MessageBox.Show("Clip to irsx file succeeded", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("An error occurred when trying to save the selected file:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void nudFromFrame_ValueChanged(object sender, EventArgs e)
        {
            if ((int)nudFromFrame.Value >= FrameCount)
            {
                nudFromFrame.Value = FrameCount;
            }

            if ((int)nudFromFrame.Value >= (int)nudToFrame.Value)
            {
                nudFromFrame.Value = nudToFrame.Value;
            }
        }

        private void nudToFrame_ValueChanged(object sender, EventArgs e)
        {
            if ((int)nudToFrame.Value <= (int)nudFromFrame.Value)
            {
                nudToFrame.Value = nudFromFrame.Value;
            }
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            FrameIndex = (int)nudGoto.Value;
            Image.SetFrameIndex(FrameIndex);
        }
    }
}
