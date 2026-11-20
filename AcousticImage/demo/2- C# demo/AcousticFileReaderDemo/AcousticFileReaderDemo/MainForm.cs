using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AcousticImage;

using ValueType = System.ValueType;

namespace AcousticFileReaderDemo
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

        private enum SoundType
        {
            Source,
            Filter,
            DownSample
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

        private const int PERCENT_100 = 100;

        private const int PALETTE_ICON_WIDTH = 24;
        private const int PALETTE_ICON_HEIGHT = 24;

        private bool IsGUIOperating { get; set; }
        private bool IsGUIRefreshing { get; set; }
        private MarkerTracker Tracker { get; set; }
        private bool Adding { get; set; }
        private bool Deleting { get; set; }
        private MarkerType CurrentMarkerType { get; set; }
        private string AudioFileName { get; set; }
        private string SoundSourceFileName { get; set; }
        private string SoundFilterFileName { get; set; }
        private string SoundDownSampleFileName { get; set; }
        private MediaPlayer Player { get; set; }
        private AcousticImageFile Image { get; set; }
        private int FrameIndex { get; set; }
        private int FrameCount { get; set; }
        private Bitmap _fftImageCache;
        private Bitmap _soundSrcCache;
        private Bitmap _soundFilterCache;
        private Bitmap _soundDownSampleCache;
        private Bitmap _prpdImageCache;

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

            List<PresetPalette> presetPalettes = AcousticImageFile.GetPresetPalettes();
            for (int i = 0; i < presetPalettes.Count; i++)
            {
                PresetPalette presetPalette = presetPalettes[i];
                tscbPalette.Items.Add(new PresetPaletteItem() { RefNo = presetPalette.RefNo, Name = presetPalette.Name });
            }

            tscbDbScaleType.Items.Add(DbScaleMode.AutoMono);
            tscbDbScaleType.Items.Add(DbScaleMode.AutoMulti);
            tscbDbScaleType.Items.Add(DbScaleMode.Fixed);
            tscbDbScaleType.Items.Add(DbScaleMode.AutoHolo);

            cbSoundType.Items.Add(SoundType.Source);
            cbSoundType.Items.Add(SoundType.Filter);
            cbSoundType.Items.Add(SoundType.DownSample);
            cbSoundType.SelectedItem = SoundType.Source;

            IsGUIOperating = false;
            IsGUIRefreshing = false;
            Tracker = new MarkerTracker();
            Adding = false;
            Deleting = false;
            CurrentMarkerType = MarkerType.None;
            AudioFileName = string.Empty;
            SoundSourceFileName = string.Empty;
            SoundFilterFileName = string.Empty;
            SoundDownSampleFileName = string.Empty;
            Player = new MediaPlayer();
            Image = new AcousticImageFile();
            Image.MarkerAdded += Image_MarkerAdded;
            Image.MarkerChanged += Image_MarkerChanged;
            Image.MarkerRenamed += Image_MarkerRenamed;
            Image.MarkerRemoved += Image_MarkerRemoved;
            Image.DbScaleChanged += ImageDbScaleChanged;
            Image.PaletteChanged += Image_PaletteChanged;
            Image.ImageChanged += Image_ImageChanged;

            DoGUIRefresh();

            Text = "Acoustic File Reader Sample, running IRtekFileSDK version: " + ImageBase.Version;
        }

        private void Image_ImageChanged(object sender, EventArgs e)
        {
            pbAcousticImage.Image = BuildImage();
            pbFFTImage.Image = GetFFTImage();

            pbSoundImage.Image = GetSoundImage((SoundType)cbSoundType.SelectedItem, (int)nudSoundDataStart.Value, (int)nudSoundDataCount.Value, (float)pbSoundImage.Width, (float)pbSoundImage.Height);
            pbPRPDImage.Image = GetPRPDImage();
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
            Palette palette = Image.GetPalette();
            tscbPalette.SelectedItem = new PresetPaletteItem() { RefNo = palette.RefNo, Name = string.Empty };
            tstbAlpha.Text = palette.Alpha.ToString("D");

            bmp = new Bitmap(PALETTE_ICON_WIDTH, PALETTE_ICON_HEIGHT);
            paletteImage = AcousticImageFile.GetPresetPaletteImage(palette.RefNo);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
            }
            tsbPalette.Image = bmp;
            IsGUIRefreshing = oldValue;
        }

        private void ImageDbScaleChanged(object sender, EventArgs e)
        {
            bool oldValue = IsGUIRefreshing;
            IsGUIRefreshing = true;
            DbScale dbScale = Image.GetDbScale();
            tscbDbScaleType.SelectedItem = dbScale.Mode;
            nudMax.Value = (decimal)(dbScale.Max);
            nudMin.Value = (decimal)(dbScale.Min);
            nudSpan.Value = (decimal)(dbScale.Span);
            cbFixSpan.Checked = dbScale.FixedSpan;

            IsGUIRefreshing = oldValue;
        }

        private void Image_MarkerAdded(object sender, AcousticImageFile.MarkerEventArgs e)
        {
            pbAcousticImage.Image = BuildImage();

            switch (e.Marker.Type)
            {
                case AcousticImage.MarkerType.Spot:
                    mlvMarkers?.AddSpot(e.Marker as AcousticImageFile.MarkerSpot);
                    break;
                case AcousticImage.MarkerType.Ellipse:
                    mlvMarkers?.AddEllipse(e.Marker as AcousticImageFile.MarkerEllipse);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Image_MarkerChanged(object sender, AcousticImageFile.MarkerEventArgs e)
        {
            pbAcousticImage.Image = BuildImage();

            switch (e.Marker.Type)
            {
                case AcousticImage.MarkerType.Spot:
                    mlvMarkers?.UpdateSpot(e.Marker as AcousticImageFile.MarkerSpot);
                    break;
                case AcousticImage.MarkerType.Ellipse:
                    mlvMarkers?.UpdateEllipse(e.Marker as AcousticImageFile.MarkerEllipse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Image_MarkerRenamed(object sender, AcousticImageFile.MarkerEventArgs e)
        {
            pbAcousticImage.Image = BuildImage();

            switch (e.Marker.Type)
            {
                case AcousticImage.MarkerType.Spot:
                    mlvMarkers?.UpdateSpot(e.Marker as AcousticImageFile.MarkerSpot);
                    break;
                case AcousticImage.MarkerType.Ellipse:
                    mlvMarkers?.UpdateEllipse(e.Marker as AcousticImageFile.MarkerEllipse);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Image_MarkerRemoved(object sender, AcousticImageFile.MarkerEventArgs e)
        {
            pbAcousticImage.Image = BuildImage();

            mlvMarkers?.RemoveMarker(e.Marker);
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
            using (OpenFileDialog ofdAcousticFile = new OpenFileDialog())
            {
                ofdAcousticFile.Filter = "Acoustic Files(*.jpg,*.acs)|*.jpg;*.acs";
                if (ofdAcousticFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        Image.Open(ofdAcousticFile.FileName);
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

        private void tsmiRotate_Click(object sender, EventArgs e)
        {
            Image.Rotate(RotateType.Rotate90);
        }

        private void tsmiHorizontalMirror_Click(object sender, EventArgs e)
        {
        }

        private void tsmiVerticalMirror_Click(object sender, EventArgs e)
        {
        }

        private void DoGUIRefresh()
        {
            if (Image.IsValid)
            {
                if (Image.FileType == FileType.AcousticVideo)
                {
                    FrameIndex = 0;
                    FrameCount = Image.GetFrameCount();
                    tPlay.Interval = (int)(1000 / Image.GetFrameRate());
                    tPlay.Enabled = false;

                    btnPreviousFrame.Visible = true;
                    btnPlay.Visible = true;
                    btnPause.Visible = true;
                    btnStop.Visible = true;
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
                    btnNextFrame.Visible = false;
                }

                bool oldValue = IsGUIRefreshing;
                IsGUIRefreshing = true;

                FillProperties();

                Player.Stop();
                CleanUpAudioAnnotation();
                CleanUpSound();
                EnableAudioAnnotation();

                tbNoteAnnotation.Text = Image.GetNoteAnnotation();

                tbEditTag.Text = string.Empty;
                tbAddTag.Text = string.Empty;
                FillTagAnnotation();

                rtbEditUserData.Text = string.Empty;
                tbAddUserDataKey.Text = string.Empty;
                rtbAddUserDataValue.Text = string.Empty;
                FillUserData();

                AcousticImageFile.MarkerCollection markers = Image.Markers;
                mlvMarkers?.Items.Clear();
                for (int i = 0; i < markers.Count; i++)
                {
                    AcousticImageFile.Marker marker = markers[i];
                    switch (marker.Type)
                    {
                        case AcousticImage.MarkerType.Spot:
                            mlvMarkers?.AddSpot(marker as AcousticImageFile.MarkerSpot);
                            break;
                        case AcousticImage.MarkerType.Ellipse:
                            mlvMarkers?.AddEllipse(marker as AcousticImageFile.MarkerEllipse);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                pbCoverImage.Image = Image.GetCoverImage();
                pbVisualImage.Image = Image.GetVisualImage();

                DbScale dbScale = Image.GetDbScale();
                nudMax.Value = (decimal)(dbScale.Max);
                nudMin.Value = (decimal)(dbScale.Min);
                nudSpan.Value = (decimal)(dbScale.Span);
                cbFixSpan.Checked = dbScale.FixedSpan;
                nudSensitivity.Value = (decimal)(dbScale.Sensitivity);
                nudSourceSize.Value = (decimal)(dbScale.SourceSize);
                nudLimit.Value = (decimal)(dbScale.Limit);
                tscbDbScaleType.SelectedItem = dbScale.Mode;
                switch (dbScale.Mode)
                {
                    case DbScaleMode.AutoMono:
                    case DbScaleMode.AutoMulti:
                        nudMax.Enabled = false;
                        nudMin.Enabled = false;
                        nudSpan.Enabled = false;
                        cbFixSpan.Enabled = false;
                        nudSensitivity.Enabled = true;
                        nudSourceSize.Enabled = true;
                        nudLimit.Enabled = true;
                        break;
                    case DbScaleMode.Fixed:
                        nudMax.Enabled = true;
                        nudMin.Enabled = true;
                        nudSpan.Enabled = false;
                        cbFixSpan.Enabled = false;
                        nudSensitivity.Enabled = false;
                        nudSourceSize.Enabled = false;
                        nudLimit.Enabled = true;
                        break;
                    case DbScaleMode.AutoHolo:
                        nudMax.Enabled = false;
                        cbFixSpan.Enabled = true;
                        nudMin.Enabled = !dbScale.FixedSpan;
                        nudSpan.Enabled = false;
                        nudSensitivity.Enabled = false;
                        nudSourceSize.Enabled = false;
                        nudLimit.Enabled = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var greyScale = Image.GetGreyScale();
                cbGreyScale.Checked = greyScale;

                pbFFTImage.Image = GetFFTImage();

                pbSoundImage.Image = GetSoundImage((SoundType)cbSoundType.SelectedItem, (int)nudSoundDataStart.Value, (int)nudSoundDataCount.Value, (float)pbSoundImage.Width, (float)pbSoundImage.Height);
                pbPRPDImage.Image = GetPRPDImage();

                Bitmap bmp = new Bitmap(pbPalleteImage.Width, pbPalleteImage.Height);
                Image paletteImage = Image.GetPaletteImage();
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
                }
                pbPalleteImage.Image = bmp;

                Palette palette = Image.GetPalette();
                tscbPalette.SelectedItem = new PresetPaletteItem() { RefNo = palette.RefNo, Name = string.Empty };
                tstbAlpha.Text = palette.Alpha.ToString("D");

                bmp = new Bitmap(PALETTE_ICON_WIDTH, PALETTE_ICON_HEIGHT);
                paletteImage = AcousticImageFile.GetPresetPaletteImage(palette.RefNo);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
                }
                tsbPalette.Image = bmp;

                nudDistance.Value = (decimal)Image.GetDistance();

                CaptureMode captureMode = Image.GetCaptureMode();
                if (captureMode == CaptureMode.PDMode)
                {
                    cbNormalizeDistance.Enabled = true;
                    cbNormalizeDistance.Checked = Image.GetNormalizeDistance();
                }

                var focusMode = Image.GetFocusMode();
                cbFocusModeEnabled.Checked = focusMode.Enabled;
                nudScope.Value = (decimal)focusMode.Scope;

                pbAcousticImage.Image = BuildImage();
                IsGUIRefreshing = oldValue;
            }
            else
            {
                tPlay.Enabled = false;

                bool oldValue = IsGUIRefreshing;
                IsGUIRefreshing = true;
                nudDistance.Value = 1.0M;

                lvProperties.Items.Clear();

                Player.Stop();
                CleanUpAudioAnnotation();
                CleanUpSound();

                tbNoteAnnotation.Text = string.Empty;

                tbEditTag.Text = string.Empty;
                tbAddTag.Text = string.Empty;
                lvTagAnnotation.Items.Clear();

                rtbEditUserData.Text = string.Empty;
                tbAddUserDataKey.Text = string.Empty;
                rtbAddUserDataValue.Text = string.Empty;
                lvUserData.Items.Clear();

                mlvMarkers?.Items.Clear();

                pbCoverImage.Image = null;
                pbVisualImage.Image = null;

                nudMax.Value = 5000.0M;
                nudMin.Value = -300.0M;

                pbPalleteImage.Image = null;

                tsbPalette.Image = null;
                tscbPalette.SelectedItem = null;

                tscbDbScaleType.SelectedItem = null;

                pbAcousticImage.Image = null;
                IsGUIRefreshing = oldValue;
            }
        }

        private Bitmap BuildImage()
        {
            Bitmap bitmap = (Bitmap)Image.GetAcousticImage().Clone();

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Overlay overlay = new Overlay(Image);
                overlay.Draw(graphics, bitmap.Width, bitmap.Height);
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
                switch (fileFormat)
                {
                    case FileFormat.FJPG:
                        //Image.SaveFJPG();
                        break;
                    default:
                        Image.SetCoverImage(Image.GetAcousticImage());
                        Image.Save();
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
            using (SaveFileDialog sfdAcousticFile = new SaveFileDialog())
            {
                sfdAcousticFile.Filter = "Acoustic Files(*.jpg)|*.jpg";
                if (sfdAcousticFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        FileFormat fileFormat = (FileFormat)tscbFileFormat.SelectedItem;
                        switch (fileFormat)
                        {
                            case FileFormat.FJPG:
                                //Image.SaveAsFJPG(sfdAcousticFile.FileName);
                                break;
                            default:
                                Image.SetCoverImage(Image.GetAcousticImage());
                                Image.SaveAs(sfdAcousticFile.FileName);
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
            using (SaveFileDialog sfdAcousticFile = new SaveFileDialog())
            {
                sfdAcousticFile.Filter = "Acoustic Files(*.jpg)|*.jpg";
                if (sfdAcousticFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        Image.SaveFrameAs(sfdAcousticFile.FileName, new Bitmap(640, 480));
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
            var frame = Image.GetFrame();
            var dataArray = new float[frame.AcousticImageSpl.Length];
            Buffer.BlockCopy(frame.AcousticImageSpl, 0, dataArray, 0, dataArray.Length * sizeof(float));

            var dbScale = Image.GetDbScale();
            dbScale.Max = dataArray.Max();
            dbScale.Min = dataArray.Min();

            Image.SetDbScale(dbScale);
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

            CameraInfo cameraInfo = Image.GetCameraInfo();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Model", cameraInfo.Model }, lvProperties.Groups["lvgCameraInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Serial No.", cameraInfo.SerialNo }, lvProperties.Groups["lvgCameraInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Company", cameraInfo.Company }, lvProperties.Groups["lvgCameraInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Brand", cameraInfo.Brand }, lvProperties.Groups["lvgCameraInfo"]));
            pbLogo.Image = cameraInfo.Logo;

            SensorInfo acSensorInfo = Image.GetSensorInfo();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Serial No.", acSensorInfo.SerialNo }, lvProperties.Groups["lvgSensorInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Channel Count", acSensorInfo.ChannelCount.ToString("F0") }, lvProperties.Groups["lvgSensorInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Version", acSensorInfo.Version }, lvProperties.Groups["lvgSensorInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Ref. Channel", acSensorInfo.RefChannel.ToString("F0") }, lvProperties.Groups["lvgSensorInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Sample Rate", acSensorInfo.SampleRate.ToString("F0") + "Hz" }, lvProperties.Groups["lvgSensorInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Amplitude", acSensorInfo.Amplitude.ToString("F2") }, lvProperties.Groups["lvgSensorInfo"]));

            Band frequencyRange = Image.GetFrequencyRange();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Frequency. Range", (frequencyRange.Min).ToString("F0") + "Hz~" + (frequencyRange.Max).ToString("F0") + "Hz" }, lvProperties.Groups["lvgFrequencyRange"]));

            AcousticParams acParams = Image.GetAcousticParams();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Gain", acParams.Gain < 0 ? "Auto" : "2^" + (acParams.Gain).ToString("F0") }, lvProperties.Groups["lvgAcousticParams"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Smooth", (acParams.Smooth).ToString("F1") }, lvProperties.Groups["lvgAcousticParams"]));
            foreach (var band in acParams.BandList)
            {
                lvProperties.Items.Add(new ListViewItem(new string[] { "User Band", (band.Min).ToString("F0") + "Hz~" + (band.Max).ToString("F0") + "Hz" }, lvProperties.Groups["lvgAcousticParams"]));
            }

            CaptureMode captureMode = Image.GetCaptureMode();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Capture Mode", (captureMode).ToString() }, lvProperties.Groups["lvgCaptureMode"]));
            if (captureMode == CaptureMode.LeakMode)
            {
                cbNormalizeDistance.Checked = true;
                cbNormalizeDistance.Enabled = false;
            }

            Size acousticSize = Image.GetAcousticSize();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Acoustic Size", acousticSize.Width.ToString() + "*" + acousticSize.Height.ToString() }, lvProperties.Groups["lvgSize"]));
            Bitmap visualImage = Image.GetVisualImage();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Visual Size", visualImage.Width.ToString() + "*" + visualImage.Height.ToString() }, lvProperties.Groups["lvgSize"]));
            ZoomInfo zoomInfo = Image.GetZoomInfo();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Zoom Enabled", zoomInfo.Enabled ? "True" : "False" }, lvProperties.Groups["lvgSize"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Zoom Location", "(" + zoomInfo.Location.X.ToString("F3") + "," + zoomInfo.Location.Y.ToString("F3") + ")" }, lvProperties.Groups["lvgSize"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Zoom Size", zoomInfo.Ratio.ToString("F1") }, lvProperties.Groups["lvgSize"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Frame Count", Image.GetFrameCount().ToString() }, lvProperties.Groups["lvgSize"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Frame Rate", Image.GetFrameRate().ToString("F3") + " fps" }, lvProperties.Groups["lvgSize"]));

            GeoLocation acGeoLocation = Image.GetGeoLocation();
            lvProperties.Items.Add(new ListViewItem(new string[] { "GPS Valid", acGeoLocation.IsGPSValid ? "True" : "False" }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Longitude", acGeoLocation.Longitude.ToString() }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Latitude", acGeoLocation.Latitude.ToString() }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Altitude", acGeoLocation.Altitude.ToString() }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Compass Valid", acGeoLocation.IsCompassValid ? "True" : "False" }, lvProperties.Groups["lvgGeoLocation"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "Direction", acGeoLocation.Direction.ToString() }, lvProperties.Groups["lvgGeoLocation"]));

            lvProperties.Items.Add(new ListViewItem(new string[] { "File Name", Image.FileName }, lvProperties.Groups["lvgFileInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "File Version", Image.FileVersion.ToString() }, lvProperties.Groups["lvgFileInfo"]));
            lvProperties.Items.Add(new ListViewItem(new string[] { "File Type", Image.FileType.ToString() }, lvProperties.Groups["lvgFileInfo"]));
            DateTime acShotTime = Image.GetShotTime();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Shot Time", acShotTime.ToString() }, lvProperties.Groups["lvgFileInfo"]));
            DateTime acModifiedTime = Image.GetModifiedTime();
            lvProperties.Items.Add(new ListViewItem(new string[] { "Modified Time", acModifiedTime.ToString() }, lvProperties.Groups["lvgFileInfo"]));
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
            //tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            //tsbLine.Checked = false;
            //tsbDelta.Checked = false;
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
            //tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            //tsbLine.Checked = false;
            //tsbDelta.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbEllipse_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Ellipse;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            //tsbRectangle.Checked = false;
            tsbEllipse.Checked = true;
            //tsbLine.Checked = false;
            //tsbDelta.Checked = false;
            tsbDelete.Checked = false;
            Adding = true;
            Deleting = false;
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            CurrentMarkerType = MarkerType.Line;
            tsbSelect.Checked = false;
            tsbSpot.Checked = false;
            //tsbRectangle.Checked = false;
            tsbEllipse.Checked = false;
            //tsbLine.Checked = false;
            //tsbDelta.Checked = false;
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

            if (mlvMarkers.SelectedItems[0].Tag is AcousticImageFile.Marker)
            {
                AcousticImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as AcousticImageFile.Marker;
                if (string.Equals(marker.Name, IRtekACFileSDK.GLOBAL_MARKER))
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
        }

        private bool HitTest(Point point, ref string name, ref MarkerType markerType, ref TrackerHitType hitType)
        {
            AcousticImageFile.MarkerCollection markers = Image.Markers;
            for (int i = 0; i < markers.Count; i++)
            {
                AcousticImageFile.Marker marker = markers[i];
                Rectangle hitArea;
                switch (marker.Type)
                {
                    case AcousticImage.MarkerType.Spot:
                        AcousticImageFile.MarkerSpot markerSpot = marker as AcousticImageFile.MarkerSpot;
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
                    case AcousticImage.MarkerType.Ellipse:
                        AcousticImageFile.MarkerEllipse markerEllipse = marker as AcousticImageFile.MarkerEllipse;
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
                    default:
                        throw new ArgumentOutOfRangeException();
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

            Size thermalSize = Image.GetAcousticSize();
            Rectangle imageRect = new Rectangle(0, 0, thermalSize.Width - 1, thermalSize.Height - 1);

            if (Tracker.MarkerType == MarkerType.Spot)
            {
                AcousticImageFile.MarkerCollection markers = Image.Markers;
                AcousticImageFile.MarkerSpot spot = markers[Tracker.Name] as AcousticImageFile.MarkerSpot;

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
            else if (Tracker.MarkerType == MarkerType.Ellipse)
            {
                AcousticImageFile.MarkerCollection markers = Image.Markers;
                AcousticImageFile.MarkerEllipse markerEllipse = markers[Tracker.Name] as AcousticImageFile.MarkerEllipse;
                Rectangle rect = markerEllipse.Rectangle;

                Point delta = ComputeDelta(rect.Location, new Point(rect.X + rect.Width, rect.Y + rect.Height), point);

                if (Tracker.HitType == TrackerHitType.hitTopLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y + delta.X);

                    rect.Width -= delta.X;
                    rect.Height -= delta.X;

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
                else if (Tracker.HitType == TrackerHitType.hitTopRight)
                {
                    rect.Location = new Point(rect.X, rect.Y - delta.X);
                    rect.Width += delta.X;
                    rect.Height += delta.X;

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
                else if (Tracker.HitType == TrackerHitType.hitBottomRight)
                {
                    rect.Width += delta.X;
                    rect.Height += delta.X;

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
                else if (Tracker.HitType == TrackerHitType.hitBottomLeft)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y);
                    rect.Width -= delta.X;
                    rect.Height -= delta.X;

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
                else if (Tracker.HitType == TrackerHitType.hitMiddle)
                {
                    rect.Location = new Point(rect.X + delta.X, rect.Y + delta.Y);
                }

                markerEllipse.Rectangle = rect;
                Tracker.HitPos = AddPoints(Tracker.HitPos, delta);
            }

            Tracker.PrevPos = point;

            Update();
        }

        private Point ComputeDelta(Point upperLeft, Point lowerRight, Point pos)
        {
            Point delta = new Point();

            Size thermalSize = Image.GetAcousticSize();
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
            Size thermalSize = Image.GetAcousticSize();
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

        private void pbAcousticImage_MouseDown(object sender, MouseEventArgs e)
        {
            ZoomOffset zoomOffset = new ZoomOffset(pbAcousticImage.ClientRectangle, Image.GetAcousticSize());
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
                tsbEllipse.Checked = false;
                tsbDelete.Checked = false;
            }

            if (Adding)
            {
                Rectangle rect;
                switch (CurrentMarkerType)
                {
                    case MarkerType.Spot:
                        Image.Markers.Add(new AcousticImageFile.MarkerSpot(Image, Image.Markers.NextSpotName, point));
                        break;
                    case MarkerType.Ellipse:
                        rect = new Rectangle(point, new Size(1, 1));
                        Image.Markers.Add(new AcousticImageFile.MarkerEllipse(Image, Image.Markers.NextAreaName, rect));
                        break;
                    case MarkerType.None:
                        break;
                    case MarkerType.Line:
                        break;
                    case MarkerType.Rectangle:
                        break;
                    case MarkerType.Delta:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                CurrentMarkerType = MarkerType.None;
                Adding = false;

                tsbSelect.Checked = true;
                tsbSpot.Checked = false;
                tsbEllipse.Checked = false;
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

        private void pbAcousticImage_MouseUp(object sender, MouseEventArgs e)
        {
            Tracker.Reset();
        }

        private void pbAcousticImage_MouseMove(object sender, MouseEventArgs e)
        {
            Point location = e.Location;
            Size thermalSize = Image.GetAcousticSize();

            ZoomOffset gainOffset = new ZoomOffset(pbAcousticImage.ClientRectangle, thermalSize);
            Point point = gainOffset.LogicalToImage(e.Location);

            if (Deleting)
            {
                Cursor = Cursors.Cross;
            }
            else
            {
                int offsetX = 10;
                int offsetY = 20;
                location.Offset(new Point(offsetX, offsetY));
                if (point.X >= 0 && point.X < thermalSize.Width && point.Y >= 0 && point.Y < thermalSize.Height)
                {
                    float value = Image.GetSpotValue(point);
                    ttTempValue.Show(value.ToString("F2"), pbAcousticImage, location);
                }
                else
                {
                    ttTempValue.Hide(pbAcousticImage);
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

        private void pbAcousticImage_MouseLeave(object sender, EventArgs e)
        {
            ttTempValue.Hide(pbAcousticImage);
            Cursor = Cursors.Default;
        }

        private void pbPalletImage_MouseMove(object sender, MouseEventArgs e)
        {
            Point location = e.Location;

            var height = Image.GetPalette().Colors?.Count() ?? 256;
            var palletSize = new Size(1, height);
            ZoomOffset gainOffset = new ZoomOffset(pbPalleteImage.ClientRectangle, palletSize);
            Point point = gainOffset.LogicalToImage(e.Location);

            if (point.Y >= 0 && point.Y < palletSize.Height)
            {
                int offsetX = 10;
                int offsetY = 20;
                location.Offset(new Point(offsetX, offsetY));
                var dbScale = Image.GetDbScale();
                float value = (dbScale.Max - dbScale.Min) / height * (height - point.Y) + dbScale.Min;
                ttTempValue.Show(value.ToString("F2"), pbPalleteImage, location);
            }
            else
            {
                ttTempValue.Hide(pbPalleteImage);
            }
        }

        private void pbPalletImage_MouseLeave(object sender, EventArgs e)
        {
            ttTempValue.Hide(pbPalleteImage);
            Cursor = Cursors.Default;
        }

        private void nudDistance_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            Image.SetDistance((float)nudDistance.Value);
        }

        private void cmsMarkers_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                e.Cancel = true;
                return;
            }

            if (mlvMarkers.SelectedItems[0].Tag is AcousticImageFile.Marker)
            {
                AcousticImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as AcousticImageFile.Marker;
                if (string.Equals(marker.Name, IRtekACFileSDK.GLOBAL_MARKER))
                {
                    tsmiRename.Visible = false;
                    tsmiLocalParameters.Visible = false;
                    tsmiDeltaParameters.Visible = false;
                    tsmiExportCSV.Visible = true;
                }
                else
                {
                    tsmiRename.Visible = true;
                    tsmiLocalParameters.Visible = true;
                    tsmiDeltaParameters.Visible = false;
                    tsmiExportCSV.Visible = true;
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

            AcousticImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as AcousticImageFile.Marker;
            if (marker == null)
            {
                return;
            }

            switch (marker.Type)
            {
                case AcousticImage.MarkerType.Spot:
                    marker.Name = Image.Markers.NextSpotName;
                    break;
                case AcousticImage.MarkerType.Ellipse:
                    marker.Name = Image.Markers.NextAreaName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void tsmiLocalParameters_Click(object sender, EventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            AcousticImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as AcousticImageFile.Marker;
            if (marker == null)
            {
                return;
            }
        }

        private void tsmiExportCSV_Click(object sender, EventArgs e)
        {
            if (mlvMarkers.SelectedItems.Count != 1)
            {
                return;
            }

            AcousticImageFile.Marker marker = mlvMarkers.SelectedItems[0].Tag as AcousticImageFile.Marker;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "SaveAs";
            sfd.Filter = "CSV File(*.csv)|*.csv";
            sfd.FileName = marker.Name;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    using (StreamWriter sw = new StreamWriter(File.Open(sfd.FileName, System.IO.FileMode.Create), Encoding.Default))
                    {
                        float[] values;
                        switch (marker.Type)
                        {
                            case AcousticImage.MarkerType.Spot:
                                AcousticImageFile.MarkerSpot markerSpot = marker as AcousticImageFile.MarkerSpot;

                                float value = Image.GetSpotValue(markerSpot.Point);
                                sw.Write((value).ToString("F2"));
                                break;
                            case AcousticImage.MarkerType.Ellipse:
                                AcousticImageFile.MarkerEllipse markerEllipse = marker as AcousticImageFile.MarkerEllipse;

                                values = Image.GetEllipseValues(markerEllipse.Rectangle);
                                for (int i = 0; i < values.Length; i++)
                                {
                                    if (i % 100 != 0)
                                    {
                                        sw.Write(",");
                                    }
                                    else if (i != 0)
                                    {
                                        sw.WriteLine();
                                    }
                                    sw.Write((values[i]).ToString("F2"));
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
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

            if ((float)nudMax.Value < (float)nudMin.Value + 0.1)
            {
                nudMax.Value = (decimal)((float)nudMin.Value + 0.1);
            }

            DbScale dbScale = Image.GetDbScale();
            dbScale.Max = (float)nudMax.Value;
            dbScale.Min = (float)nudMin.Value;
            dbScale.Span = (float)nudSpan.Value;
            dbScale.Mode = (DbScaleMode)tscbDbScaleType.SelectedItem;
            Image.SetDbScale(dbScale);
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

            if ((float)nudMin.Value > (float)nudMax.Value - 0.1)
            {
                nudMin.Value = (decimal)((float)nudMax.Value - 0.1);
            }

            DbScale dbScale = Image.GetDbScale();
            dbScale.Max = (float)nudMax.Value;
            dbScale.Min = (float)nudMin.Value;
            dbScale.Span = (float)nudSpan.Value;
            dbScale.Mode = (DbScaleMode)tscbDbScaleType.SelectedItem;
            Image.SetDbScale(dbScale);
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

            if ((float)nudSpan.Value > (float)nudMax.Value - 0.1)
            {
                nudSpan.Value = (decimal)((float)nudMax.Value - 0.1);
            }

            DbScale dbScale = Image.GetDbScale();
            dbScale.Max = (float)nudMax.Value;
            dbScale.Min = (float)nudMin.Value;
            dbScale.Span = (float)nudSpan.Value;
            dbScale.Mode = (DbScaleMode)tscbDbScaleType.SelectedItem;
            Image.SetDbScale(dbScale);
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
            Image.SetPalette(palette);
            tstbAlpha.Text = palette.Alpha.ToString("D");

            Bitmap bmp = new Bitmap(PALETTE_ICON_WIDTH, PALETTE_ICON_HEIGHT);
            Bitmap paletteImage = AcousticImageFile.GetPresetPaletteImage(palette.RefNo);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(paletteImage, 0, 0, bmp.Width * 2, bmp.Height);
            }
            tsbPalette.Image = bmp;
            IsGUIOperating = oldValue;
        }

        private void tstbAlpha_TextChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }
            Palette palette = Image.GetPalette();
            int.TryParse(tstbAlpha.Text, out var alpha);
            palette.Alpha = (byte)alpha;
            Image.SetPalette(palette);
        }

        private void cbFocusModelEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;

            var focusMode = Image.GetFocusMode();
            focusMode.Enabled = cbFocusModeEnabled.Checked;
            Image.SetFocusMode(focusMode);

            IsGUIOperating = oldValue;
        }

        private void nudScope_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            var focusMode = Image.GetFocusMode();
            focusMode.Scope = (float)nudScope.Value;
            Image.SetFocusMode(focusMode);
            IsGUIOperating = oldValue;
        }

        private void tscbDbScaleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            var dbScale = new DbScale
            {
                Mode = (DbScaleMode)tscbDbScaleType.SelectedItem,
                Max = (float)nudMax.Value,
                Min = (float)nudMin.Value,
                FixedSpan = (bool)cbFixSpan.Checked,
                Span = (float)nudSpan.Value,
                Sensitivity = (float)nudSensitivity.Value,
                SourceSize = (int)nudSourceSize.Value,
                Limit = (float)nudLimit.Value
            };
            switch (dbScale.Mode)
            {
                case DbScaleMode.AutoMono:
                case DbScaleMode.AutoMulti:
                    nudMax.Enabled = false;
                    nudMin.Enabled = false;
                    nudSpan.Enabled = false;
                    cbFixSpan.Enabled = false;
                    nudSensitivity.Enabled = true;
                    nudSourceSize.Enabled = true;
                    nudLimit.Enabled = true;
                    break;
                case DbScaleMode.AutoHolo:
                    var frame = Image.GetFrame();
                    var dataArray = new float[frame.AcousticImageSpl.Length];
                    Buffer.BlockCopy(frame.AcousticImageSpl, 0, dataArray, 0, dataArray.Length * sizeof(float));
                    dbScale.Max = dataArray.Max();
                    dbScale.Min = dataArray.Min();
                    nudMax.Enabled = false;
                    nudMin.Enabled = false;
                    cbFixSpan.Enabled = true;
                    nudSpan.Enabled = !dbScale.FixedSpan;
                    nudSensitivity.Enabled = false;
                    nudSourceSize.Enabled = false;
                    nudLimit.Enabled = true;
                    break;
                case DbScaleMode.Fixed:
                    frame = Image.GetFrame();
                    dataArray = new float[frame.AcousticImageSpl.Length];
                    Buffer.BlockCopy(frame.AcousticImageSpl, 0, dataArray, 0, dataArray.Length * sizeof(float));
                    dbScale.Max = dataArray.Max();
                    dbScale.Min = dataArray.Min();
                    nudMax.Enabled = true;
                    nudMin.Enabled = true;
                    nudSpan.Enabled = false;
                    cbFixSpan.Enabled = false;
                    nudSensitivity.Enabled = false;
                    nudSourceSize.Enabled = false;
                    nudLimit.Enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Image.SetDbScale(dbScale);
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

        private static Point AddPoints(Point pt1, Point pt2)
        {
            return new Point(pt1.X + pt2.X, pt1.Y + pt2.Y);
        }

        private static Point SubPoints(Point pt1, Point pt2)
        {
            return new Point(pt1.X - pt2.X, pt1.Y - pt2.Y);
        }

        private void btnPlaySound_Click(object sender, EventArgs e)
        {
            string fileName;
            switch ((SoundType)cbSoundType.SelectedItem)
            {
                case SoundType.Source:
                    if (string.IsNullOrEmpty(SoundSourceFileName))
                    {
                        SoundSourceFileName = GetSoundWavFile(SoundType.Source);
                    }
                    fileName = SoundSourceFileName;
                    break;
                case SoundType.Filter:
                    if (string.IsNullOrEmpty(SoundFilterFileName))
                    {
                        SoundFilterFileName = GetSoundWavFile(SoundType.Filter);
                    }
                    fileName = SoundFilterFileName;
                    break;
                case SoundType.DownSample:
                    if (string.IsNullOrEmpty(SoundDownSampleFileName))
                    {
                        SoundDownSampleFileName = GetSoundWavFile(SoundType.DownSample);
                    }
                    fileName = SoundDownSampleFileName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Player.Play(fileName, this);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            tPlay.Interval = (int)(1000 / Image.GetFrameRate());
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

        private void nudData_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            pbSoundImage.Image = GetSoundImage((SoundType)cbSoundType.SelectedItem, (int)nudSoundDataStart.Value, (int)nudSoundDataCount.Value, pbSoundImage.Width, pbSoundImage.Height);
            IsGUIOperating = oldValue;
        }

        private Bitmap GetFFTImage()
        {
            _fftImageCache?.Dispose();

            try
            {
                var acFrame = Image.GetFrame();
                var acParams = Image.GetAcousticParams();
                var frq = Image.GetFrequencyRange();

                var fftData = acFrame.Spectrum.FFT;
                //计算高度 每个数据一行
                var dataCount = (int)Math.Ceiling((frq.Max - frq.Min) / acFrame.Spectrum.Delta);
                var height = dataCount;

                //取FFT最大值作为宽度
                var max = fftData.Max();
                var width = (int)Math.Max(height / 10f, max);

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                var g = Graphics.FromImage(bitmap);
                var offset = (int)((frq.Min - acFrame.Spectrum.Range.Min) / acFrame.Spectrum.Delta);

                var points = new List<PointF>();
                for (int i = 0; i < height; i++)
                {
                    if (i + offset >= fftData.Length || i + offset < 0)
                    {
                        points.Add(new PointF(width, height - i));
                    }
                    else
                    {
                        points.Add(new PointF(width - fftData[i + offset], height - i));
                    }
                }

                points.Add(new PointF(width, 0));
                points.Add(new PointF(width, height));

                g.Clear(Color.White);
                foreach (var band in acParams.BandList)
                {
                    var offsetMax = (band.Max - frq.Min) / acFrame.Spectrum.Delta;
                    var offsetMin = (band.Min - frq.Min) / acFrame.Spectrum.Delta;
                    g.FillRectangle(Brushes.Aqua, 1, height - offsetMax, width - 2, offsetMax - offsetMin);
                }
                g.FillPolygon(Brushes.Blue, points.ToArray());
                g.DrawString(frq.Max / 1000 + "kHz", new Font(FontFamily.GenericMonospace, 10), Brushes.Black, new PointF(1, 0));
                g.DrawString(frq.Min / 1000 + "kHz", new Font(FontFamily.GenericMonospace, 10), Brushes.Black, new PointF(1, height - 15));


                foreach (var band in acParams.BandList)
                {
                    var offsetMax = (band.Max - frq.Min) / acFrame.Spectrum.Delta;
                    var offsetMin = (band.Min - frq.Min) / acFrame.Spectrum.Delta;
                    g.DrawString(band.Max / 1000 + "kHz", new Font(FontFamily.GenericMonospace, 10), Brushes.Black, new PointF(1, height - offsetMax));
                    g.DrawString(band.Min / 1000 + "kHz", new Font(FontFamily.GenericMonospace, 10), Brushes.Black, new PointF(1, height - offsetMin - 15));
                }

                _fftImageCache = bitmap;
                return _fftImageCache;
            }
            catch
            {
                return null;
            }
        }

        private Bitmap GetSoundImage(float[] soundData, int startIndex, int length, int sampleRate, int fps, float maxValue, float width, float height)
        {
            //数据值为声压值单位为Pa
            //声压级计算公式为 LP = 20*lg(P/P0)  其中 P0 为基准声压，为2*10^-5Pa 1000Hz人耳可听最低声压
            //带入转换  LP = 20*lg(P/2) + 100 声压值为2Pa时，声压级为100dB
            if (startIndex + length > soundData.Length)
            {
                return null;
            }
            //计算时长
            var resultData = new float[length];
            Array.Copy(soundData, startIndex, resultData, 0, length);

            var frameLength = sampleRate / fps;
            var soundTimespan = soundData.Length / sampleRate;//s
            var startFrame = startIndex / frameLength;
            var timeLong = 1000 / fps;
            maxValue = resultData.Max() > 1.2f ? maxValue : 1.2f; //95.56dB

            //计算比例
            var scaleX = width / resultData.Length;
            var scaleY = height / (maxValue * 2);

            //计算path
            var points = new List<PointF>();
            for (int i = 0; i < resultData.Length; i++)
            {
                points.Add(new PointF(i * scaleX, height / 2 - (resultData[i] * scaleY)));
            }

            Bitmap bitmap = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);
            var g = Graphics.FromImage(bitmap);
            g.Clear(Color.LightGray);
            var pen = Pens.Gray;
            var brush = Brushes.Gray;
            var font = new Font(FontFamily.GenericMonospace, 8);
            int gridX = 1;
            //绘制帧基准线
            while ((gridX) * frameLength * scaleX < width)
            {
                g.DrawLine(pen, gridX * frameLength * scaleX, 0, gridX * frameLength * scaleX, height);
                g.DrawString((startFrame + gridX) * timeLong + "ms", font, brush, gridX * frameLength * scaleX, 0);
                gridX++;
            }
            //绘制声压基准线
            g.DrawLine(pen, 0, height / 2 - 1f * scaleY, width, height / 2 - 1f * scaleY);//1
            g.DrawLine(pen, 0, height / 2 - 0.5f * scaleY, width, height / 2 - 0.5f * scaleY);//0.5
            g.DrawLine(pen, 0, height / 2, width, height / 2);                              //0
            g.DrawLine(pen, 0, height / 2 + 0.5f * scaleY, width, height / 2 + 0.5f * scaleY);//0.5
            g.DrawLine(pen, 0, height / 2 + 1f * scaleY, width, height / 2 + 1f * scaleY);//1
            var size = g.MeasureString("0.5Pa", font);
            g.DrawString("0.5Pa", font, brush, 0, height / 2 - 0.5f * scaleY - size.Height);
            g.DrawString("-0.5Pa", font, brush, 0, height / 2 + 0.5f * scaleY);
            g.DrawString("1.0Pa", font, brush, 0, height / 2 - 1f * scaleY - size.Height);
            g.DrawString("-1.0Pa", font, brush, 0, height / 2 + 1f * scaleY);

            var db05 = Math.Log10(0.5 / 2f) * 20 + 100;
            var db1 = Math.Log10(1 / 2f) * 20 + 100;
            size = g.MeasureString(db05.ToString("F0") + "dB", font);
            g.DrawString(db05.ToString("F0") + "dB", font, brush, width - size.Width, height / 2 - 0.5f * scaleY - size.Height);
            g.DrawString(db1.ToString("F0") + "dB", font, brush, width - size.Width, height / 2 - 1f * scaleY - size.Height);

            //绘制频谱
            g.DrawLines(Pens.Blue, points.ToArray());
            return bitmap;
        }

        private Bitmap GetSoundImage(SoundType type, int startIndex, int length, float width, float height)
        {
            if (Image == null)
            {
                return null;
            }

            try
            {
                var acFrame = Image.GetFrame();
                var sensorInfo = Image.GetSensorInfo();
                float[] soundData;
                switch (type)
                {
                    case SoundType.Source:
                        _soundSrcCache?.Dispose();
                        soundData = acFrame.Sound.Source;
                        break;
                    case SoundType.Filter:
                        _soundFilterCache?.Dispose();
                        soundData = acFrame.Sound.Filter;
                        break;
                    case SoundType.DownSample:
                        _soundDownSampleCache?.Dispose();
                        soundData = acFrame.Sound.DownSample;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                switch (type)
                {
                    case SoundType.Source:
                        _soundSrcCache = GetSoundImage(soundData, startIndex, length, sensorInfo.SampleRate, 25, sensorInfo.Amplitude, width, height);
                        return _soundSrcCache;
                    case SoundType.Filter:
                        _soundFilterCache = GetSoundImage(soundData, startIndex, length, sensorInfo.SampleRate, 25, sensorInfo.Amplitude, width, height);
                        return _soundFilterCache;
                    case SoundType.DownSample:
                        _soundDownSampleCache = GetSoundImage(soundData, startIndex, length, sensorInfo.SampleRate, 25, sensorInfo.Amplitude, width, height);
                        return _soundDownSampleCache;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
            catch
            {
                return null;
            }
        }

        private bool GetSoundWavFile(float[] soundData, string fileName)
        {
            var size = soundData.Length * sizeof(short);
            var soundDataShort = new byte[size];
            //转pcm 16位深度 200K采样率 单声道
            var index = 0;
            foreach (var f in soundData)
            {
                var value = (short)((f) * Int16.MaxValue);
                soundDataShort[index++] = (byte)(value & 0xff);
                soundDataShort[index++] = (byte)(value >> 8 & 0xff);
            }

            var header = new WavPcmFileHeader(1, 200000, 16, (uint)size);
            var headerBytes = AcousticImageFile.StructToByeBytes(header);

            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(headerBytes, 0, headerBytes.Length);
                fs.Write(soundDataShort, 0, size);
                fs.Flush();
            }

            return true;
        }

        private string GetSoundWavFile(SoundType type)
        {
            var fileName = Path.GetTempFileName();
            try
            {
                var acFrame = Image.GetFrame();
                float[] soundData;
                switch (type)
                {
                    case SoundType.Source:
                        soundData = acFrame.Sound.Source;
                        break;
                    case SoundType.Filter:
                        soundData = acFrame.Sound.Filter;
                        break;
                    case SoundType.DownSample:
                        soundData = acFrame.Sound.DownSample;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                GetSoundWavFile(soundData, fileName);
                return fileName;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void CleanUpSound()
        {
            if (string.IsNullOrEmpty(SoundSourceFileName) == false)
            {
                try
                {
                    File.Delete(SoundSourceFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred when trying to clean up sound source:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                SoundSourceFileName = string.Empty;
            }
            if (string.IsNullOrEmpty(SoundFilterFileName) == false)
            {
                try
                {
                    File.Delete(SoundFilterFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred when trying to clean up sound filter:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                SoundFilterFileName = string.Empty;
            }
            if (string.IsNullOrEmpty(SoundDownSampleFileName) == false)
            {
                try
                {
                    File.Delete(SoundDownSampleFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred when trying to clean up sound down sample:\n\n{0}", ex.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                SoundDownSampleFileName = string.Empty;
            }
        }

        private Bitmap GetPRPDImage()
        {
            _prpdImageCache?.Dispose();

            try
            {
                var acFrame = Image.GetFrame();
                var pointList = acFrame.PRPD.ToList();
                var borderSize = 15;

                var width = pbPRPDImage.Width;
                var height = pbPRPDImage.Height;
                var max = pointList.Max(x => x.Y);
                var min = pointList.Min(x => x.Y);
                var count = pointList.Count;
                var scaleY = (height - 2 * borderSize) / max;
                var scaleX = (width - 2 * borderSize) / pointList.Max(x => x.X);

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                var g = Graphics.FromImage(bitmap);

                g.Clear(Color.White);
                var font = new Font(FontFamily.GenericMonospace, 8);
                //轴
                g.DrawLine(Pens.Black, borderSize, height, borderSize, 0);
                g.DrawLine(Pens.Black, 0, (height - borderSize), width, height - borderSize);

                foreach (var pointF in pointList)
                {
                    g.FillEllipse(Brushes.Blue, pointF.X * scaleX + borderSize, height - (pointF.Y * scaleY) - borderSize, 4, 4);
                }

                g.DrawString("0", font, Brushes.Black, borderSize, height - borderSize);
                var size = g.MeasureString("180", font);
                g.DrawString("180", font, Brushes.Black, width / 2f - size.Width / 2, height - borderSize);
                size = g.MeasureString("360", font);
                g.DrawString("360", font, Brushes.Black, width - borderSize - size.Width / 2, height - borderSize);

                lvPRPDProperties.Items.Clear();
                float acFrequency = Image.GetAcFrequency();
                lvPRPDProperties.Items.Add(new ListViewItem(new string[] { "AC Frequency", acFrequency.ToString() }, lvPRPDProperties.Groups["lvgPRPDInfo"]));
                lvPRPDProperties.Items.Add(new ListViewItem(new string[] { "Point Count", count.ToString() }, lvPRPDProperties.Groups["lvgPRPDInfo"]));
                lvPRPDProperties.Items.Add(new ListViewItem(new string[] { "Max", max.ToString() }, lvPRPDProperties.Groups["lvgPRPDInfo"]));
                lvPRPDProperties.Items.Add(new ListViewItem(new string[] { "Min", min.ToString() }, lvPRPDProperties.Groups["lvgPRPDInfo"]));
                lvPRPDProperties.Items.Add(new ListViewItem(new string[] { "Level", Image.GetPDLevel().ToString() }, lvPRPDProperties.Groups["lvgPRPDInfo"]));

                _prpdImageCache = bitmap;
                return _prpdImageCache;
            }
            catch
            {
                return null;
            }
        }

        private void cbSoundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            pbSoundImage.Image = GetSoundImage((SoundType)cbSoundType.SelectedItem, (int)nudSoundDataStart.Value, (int)nudSoundDataCount.Value, pbSoundImage.Width, pbSoundImage.Height);
            IsGUIOperating = oldValue;
        }

        private void pbSoundImage_SizeChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;
            pbSoundImage.Image = GetSoundImage((SoundType)cbSoundType.SelectedItem, (int)nudSoundDataStart.Value, (int)nudSoundDataCount.Value, pbSoundImage.Width, pbSoundImage.Height);
            IsGUIOperating = oldValue;
        }

        private void cbNormalizeDistance_CheckedChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;

            Image.SetNormalizeDistance(cbNormalizeDistance.Checked);

            IsGUIOperating = oldValue;
        }

        private void cbFixSpan_CheckedChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;

            var dbScale = Image.GetDbScale();
            dbScale.FixedSpan = cbFixSpan.Checked;
            Image.SetDbScale(dbScale);

            IsGUIOperating = oldValue;
        }

        private void nudSensitivity_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;

            var dbScale = Image.GetDbScale();
            dbScale.Sensitivity = (float)nudSensitivity.Value;
            Image.SetDbScale(dbScale);

            IsGUIOperating = oldValue;
        }

        private void nudSourceSize_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;

            var dbScale = Image.GetDbScale();
            dbScale.SourceSize = (int)nudSourceSize.Value;
            Image.SetDbScale(dbScale);

            IsGUIOperating = oldValue;
        }

        private void nudLimit_ValueChanged(object sender, EventArgs e)
        {
            if (IsGUIRefreshing)
            {
                return;
            }

            bool oldValue = IsGUIOperating;
            IsGUIOperating = true;

            var dbScale = Image.GetDbScale();
            dbScale.Limit = (float)nudLimit.Value;
            Image.SetDbScale(dbScale);

            IsGUIOperating = oldValue;
        }
    }
}
