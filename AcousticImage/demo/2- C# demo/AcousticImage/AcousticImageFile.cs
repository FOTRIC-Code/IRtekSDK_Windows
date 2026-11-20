using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AcousticImage
{
    public partial class AcousticImageFile : ImageBase
    {
        private bool _isMarkersSynced;
        private Bitmap _acousticImageCache;
        private Bitmap _paletteImageCache;
        private Bitmap _visualImageCache;
        private Bitmap _coverImageCache;

        private const string USER_DATA_KEY_FAVORITE = "FAVORITE";
        private const string USER_DATA_KEY_TAG = "TAG";
        private const string USER_DATA_KEY_GROUPFILES = "GROUPFILES";
        private const string USER_DATA_KEY_TIMERFILE = "TIMERFILE";

        private const string USER_DATA_SEPARATOR = "\n";
        private const string USER_DATA_FAVORITE_YES = "Y";
        private const string USER_DATA_FAVORITE_NO = "N";

        private const string SPOT_PREFIX = "Sp";
        private const string AREA_PREFIX = "Ar";

        public event MarkerEventHandler MarkerAdded = null;

        public event MarkerEventHandler MarkerChanged = null;

        public event MarkerEventHandler MarkerRenamed = null;

        public event MarkerEventHandler MarkerRemoved = null;

        public event EventHandler ImageChanged = null;

        public event EventHandler PaletteChanged = null;

        public event EventHandler DbScaleChanged = null;

        public event EventHandler RotationTypeChanged = null;

        public event EventHandler MirrorTypeChanged = null;

        public event EventHandler FrameIndexChanged = null;

        public AcousticImageFile()
        {
            _isMarkersSynced = false;
            Markers = new MarkerCollection(this);
            _acousticImageCache = null;
            _paletteImageCache = null;
            _visualImageCache = null;
            _coverImageCache = null;
        }

        public MarkerCollection Markers { get; private set; }

        /// <summary>
        /// 获取声像参数
        /// </summary>
        /// <returns></returns>
        public AcousticParams GetAcousticParams()
        {
            if (HImage == IntPtr.Zero)
            {
                return AcousticParams.DEFAULT;
            }
            var bandSize = 0;
            unsafe
            {
                bandSize = sizeof(IRtekACFileSDK.ac_file_band);
            }
            IRtekACFileSDK.ac_file_acoustic_params acParams = new IRtekACFileSDK.ac_file_acoustic_params();
            int result = IRtekACFileSDK.ac_file_get_acoustic_params(HImage, ref acParams);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return AcousticParams.DEFAULT;
            }

            try
            {
                acParams.bands = Marshal.AllocHGlobal(acParams.count * bandSize);
                result = IRtekACFileSDK.ac_file_get_acoustic_params(HImage, ref acParams);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    return AcousticParams.DEFAULT;
                }

                AcousticParams value = new AcousticParams
                {
                    Gain = acParams.gain,
                    Smooth = acParams.smooth,
                    BandList = new List<Band>()
                };

                if (acParams.count > 0)
                {
                    var bandList = new byte[acParams.count * bandSize];
                    var bandBuffer = new byte[bandSize];
                    Marshal.Copy(acParams.bands, bandList, 0, bandList.Length);
                    for (int i = 0; i < acParams.count; i++)
                    {
                        Buffer.BlockCopy(bandList, i * bandSize, bandBuffer, 0, bandSize);
                        var band = BytesToStruct(bandBuffer, typeof(Band));
                        if (band == null)
                        {
                            continue;
                        }
                        value.BandList.Add((Band)band);
                    }
                }
                return value;
            }
            finally
            {
                Marshal.FreeHGlobal(acParams.bands);
            }
        }

        /// <summary>
        /// 获取摄像头参数
        /// </summary>
        /// <returns></returns>
        public CameraInfo GetCameraInfo()
        {
            if (HImage == IntPtr.Zero)
            {
                return CameraInfo.DEFAULT;
            }

            IRtekACFileSDK.ac_file_camera_info acCameraInfo = new IRtekACFileSDK.ac_file_camera_info();
            int result = IRtekACFileSDK.ac_file_get_camera_info(HImage, ref acCameraInfo);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                CameraInfo value = new CameraInfo();
                value.Model = BytesToString(acCameraInfo.model, Encoding.UTF8);
                value.SerialNo = BytesToString(acCameraInfo.serialNo, Encoding.UTF8);
                value.Brand = BytesToString(acCameraInfo.brand, Encoding.UTF8);
                value.Company = BytesToString(acCameraInfo.company, Encoding.UTF8);
                if (acCameraInfo.logoLength != 0)
                {
                    try
                    {
                        acCameraInfo.logo = Marshal.AllocHGlobal(acCameraInfo.logoLength);
                        IRtekACFileSDK.ac_file_get_camera_info(HImage, ref acCameraInfo);
                        byte[] buffer = new byte[acCameraInfo.logoLength];
                        Marshal.Copy(acCameraInfo.logo, buffer, 0, acCameraInfo.logoLength);
                        MemoryStream ms = new MemoryStream(buffer);
                        value.Logo = new Bitmap(ms);
                    }
                    catch (Exception)
                    {
                        value.Logo = null;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(acCameraInfo.logo);
                    }
                }
                else
                {
                    value.Logo = null;
                }

                return value;
            }
            else
            {
                return CameraInfo.DEFAULT;
            }
        }

        /// <summary>
        /// 获取传感器信息
        /// </summary>
        /// <returns></returns>
        public SensorInfo GetSensorInfo()
        {
            if (HImage == IntPtr.Zero)
            {
                return SensorInfo.DEFAULT;
            }

            IRtekACFileSDK.ac_file_sensor_info acSensorInfo = new IRtekACFileSDK.ac_file_sensor_info();
            int result = IRtekACFileSDK.ac_file_get_sensor_info(HImage, ref acSensorInfo);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                SensorInfo value = new SensorInfo
                {
                    SerialNo = BytesToString(acSensorInfo.serial_no, Encoding.UTF8),
                    ChannelCount = acSensorInfo.channel_count,
                    RefChannel = acSensorInfo.ref_channel,
                    SampleRate = acSensorInfo.sample_rate,
                    Version = BytesToString(acSensorInfo.version, Encoding.UTF8),
                    Amplitude = acSensorInfo.amplitude
                };

                return value;
            }
            else
            {
                return SensorInfo.DEFAULT;
            }
        }

        /// <summary>
        /// 获取封面图
        /// </summary>
        /// <returns></returns>
        public Bitmap GetCoverImage()
        {
            if (HImage == IntPtr.Zero)
            {
                return null;
            }

            if (_coverImageCache != null)
            {
                return _coverImageCache;
            }

            IRtekACFileSDK.ac_file_buffer acBuffer = new IRtekACFileSDK.ac_file_buffer();
            IRtekACFileSDK.ac_file_get_cover_image(HImage, ref acBuffer);
            if (acBuffer.length != 0)
            {
                try
                {
                    acBuffer.buffer = Marshal.AllocHGlobal(acBuffer.length);
                    IRtekACFileSDK.ac_file_get_cover_image(HImage, ref acBuffer);
                    byte[] buffer = new byte[acBuffer.length];
                    Marshal.Copy(acBuffer.buffer, buffer, 0, acBuffer.length);
                    MemoryStream ms = new MemoryStream(buffer);
                    _coverImageCache = new Bitmap(ms);
                    return _coverImageCache;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    Marshal.FreeHGlobal(acBuffer.buffer);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置封面图
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetCoverImage(Bitmap value)
        {
            if (HImage == IntPtr.Zero || value == null)
            {
                return false;
            }

            MemoryStream ms = new MemoryStream();
            value.Save(ms, ImageFormat.Jpeg);
            byte[] buffer = ms.GetBuffer();
            IRtekACFileSDK.ac_file_buffer acBuffer = new IRtekACFileSDK.ac_file_buffer();

            try
            {
                acBuffer.length = (int)ms.Length;
                acBuffer.buffer = Marshal.AllocHGlobal(acBuffer.length);
                Marshal.Copy(buffer, 0, acBuffer.buffer, acBuffer.length);

                int result = IRtekACFileSDK.ac_file_set_cover_image(HImage, ref acBuffer);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    return false;
                }

                IsModified = true;
            }
            finally
            {
                Marshal.FreeHGlobal(acBuffer.buffer);
            }

            return true;
        }

        /// <summary>
        /// 获取帧序号
        /// </summary>
        /// <returns></returns>
        public uint GetFrameIndex()
        {
            if (HImage == IntPtr.Zero)
                return 0;
            uint frameIndex = 0;
            IRtekACFileSDK.ac_file_get_frame_index(HImage, ref frameIndex);
            return frameIndex;
        }

        /// <summary>
        /// 获取可见光图
        /// </summary>
        /// <returns></returns>
        public Bitmap GetVisualImage()
        {
            if (HImage == IntPtr.Zero)
            {
                return null;
            }

            if (_visualImageCache != null)
            {
                return _visualImageCache;
            }

            try
            {
                var acFrame = GetFrame();
                MemoryStream ms = new MemoryStream(acFrame.VisualImage);
                _visualImageCache = new Bitmap(ms);

                return _visualImageCache;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取调色板图
        /// </summary>
        /// <returns></returns>
        public Bitmap GetPaletteImage()
        {
            if (HImage == IntPtr.Zero)
            {
                return null;
            }

            if (_paletteImageCache != null)
            {
                return _paletteImageCache;
            }

            try
            {
                int width = 1;
                int height = IRtekACFileSDK.COLORS_LEN;
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                IRtekACFileSDK.ac_file_image acImage = new IRtekACFileSDK.ac_file_image
                {
                    offset = bmpData.Stride - 3 * width,
                    data = bmpData.Scan0
                };
                IRtekACFileSDK.ac_file_get_palette_image(HImage, ref acImage);

                bitmap.UnlockBits(bmpData);
                _paletteImageCache = bitmap;
                return _paletteImageCache;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取声像图
        /// </summary>
        /// <returns></returns>
        public Bitmap GetAcousticImage()
        {
            if (HImage == IntPtr.Zero)
            {
                return null;
            }

            if (_acousticImageCache != null)
            {
                return _acousticImageCache;
            }

            try
            {
                Size acousticSize = GetAcousticSize();
                int width = acousticSize.Width;
                int height = acousticSize.Height;
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                //获取当前帧
                var acFrame = GetFrame();

                //获取调色板
                IRtekACFileSDK.ac_file_palette acPalette = new IRtekACFileSDK.ac_file_palette();
                int result = IRtekACFileSDK.ac_file_get_palette(HImage, ref acPalette);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    throw new Exception("ac_file_get_palette fail.");
                }

                //获取聚焦模式
                var acFocusMode = new IRtekACFileSDK.ac_file_focus_mode();
                result = IRtekACFileSDK.ac_file_get_focus_mode(HImage, ref acFocusMode);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    throw new Exception("ac_file_get_focus_mode fail.");
                }

                //获取Zoom
                var zoomInfo = GetZoomInfo();

                var gridSpl = new AcousticUtils.grid_spl()
                {
                    width = width,
                    height = height,
                    value = Marshal.UnsafeAddrOfPinnedArrayElement(acFrame.AcousticImageSpl, 0)
                };

                var specialPoints = new AcousticUtils.acoustic_summary()
                {
                    extreme_points = new AcousticUtils.acoustic_point[AcousticUtils.OUT_EXTREME_POINT_COUNT],
                };

                //获取模式
                IRtekACFileSDK.ac_file_db_scale_v2 acDbScale = new IRtekACFileSDK.ac_file_db_scale_v2();
                result = IRtekACFileSDK.ac_file_get_db_scale_v2(HImage, ref acDbScale);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    throw new Exception("ac_file_get_db_scale fail.");
                }

                //获取声像图
                var useApiV1 = false;
                if (useApiV1)
                {
                    IRtekACFileSDK.ac_file_db_scale acDbScale_old = new IRtekACFileSDK.ac_file_db_scale();
                    result = IRtekACFileSDK.ac_file_get_db_scale(HImage, ref acDbScale_old);
                    if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                    {
                        throw new Exception("ac_file_get_db_scale fail.");
                    }

                    result = AcousticUtils.acoustic_get_spl_image(ref gridSpl, ref acDbScale_old, ref acFocusMode, ref acPalette, AcousticUtils.ARGB, zoomInfo.Enabled ? zoomInfo.Ratio : 1, bmpData.Scan0, ref specialPoints);
                }
                else
                {
                    var isShow = FALSE;
                    result = AcousticUtils.acoustic_get_spl_image_v2(ref gridSpl, ref acDbScale, ref acFocusMode, ref acPalette, AcousticUtils.ARGB, zoomInfo.Enabled ? zoomInfo.Ratio : 1, bmpData.Scan0, ref specialPoints, ref isShow);
                }

                if (result != AcousticUtils.EC_OK)
                {
                    throw new Exception("acoustic_get_spl_image fail.");
                }

                bitmap.UnlockBits(bmpData);

                //重写上下限，因为设备写入的dbscale是错的
                result = IRtekACFileSDK.ac_file_set_db_scale_v2(HImage, ref acDbScale);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    throw new Exception("ac_file_set_db_scale fail.");
                }
                DoDbScaleChanged();

                //取可见光图
                MemoryStream ms = new MemoryStream(acFrame.VisualImage);
                var visual = new Bitmap(ms);
                if (GetGreyScale())
                {
                    visual = RGBToGrey(visual);
                }

                RotateType rotateType = GetRotateType();
                switch (rotateType)
                {
                    case RotateType.Rotate90:
                        visual.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case RotateType.Rotate180:
                        visual.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case RotateType.Rotate270:
                        visual.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }

                //绘制
                
                var g = Graphics.FromImage(visual);
                var attrs = new ImageAttributes();
                attrs.SetColorKey(Color.Black, Color.Black);
                g.DrawImage(bitmap, new Rectangle(0, 0, visual.Width, visual.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attrs);
                bitmap.Dispose();

                if (zoomInfo.Enabled)
                {
                    //裁剪出窗口图
                    var windowImage = new Bitmap((int) (visual.Width / zoomInfo.Ratio), (int) (visual.Height / zoomInfo.Ratio));
                    var gWindow = Graphics.FromImage(windowImage);
                    var range = new Rectangle((int) (visual.Width * zoomInfo.Location.X), (int)(visual.Height * zoomInfo.Location.Y), windowImage.Width, windowImage.Height);
                    gWindow.DrawImage(visual, 0, 0, range, GraphicsUnit.Pixel);
                    //画回到原图
                    g.DrawImage(windowImage, new Rectangle(0, 0, visual.Width, visual.Height), 0, 0, windowImage.Width, windowImage.Height, GraphicsUnit.Pixel);
                }

                if (acFocusMode.enabled == TRUE)
                {
                    DrawFocus(g, visual.Width, visual.Height, acFocusMode.scope);
                }

                _acousticImageCache = visual;
                return _acousticImageCache;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 转灰度图
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap RGBToGrey(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var size = bmpData.Height * bmpData.Width;
            var ptr = bmpData.Scan0;
            var buffer = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(ptr, buffer, 0, buffer.Length);
            for (int i = 0; i < size; i++)
            {
                var color = (byte)((buffer[i * 4] + buffer[i * 4 + 1] + buffer[i * 4 + 2]) / 3);
                buffer[i * 4] = color;
                buffer[i * 4 + 1] = color;
                buffer[i * 4 + 2] = color;
            }

            Marshal.Copy(buffer, 0, ptr, buffer.Length);
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        public PDLevel GetPDLevel()
        {
            if (HImage == IntPtr.Zero)
            {
                return PDLevel.None;
            }
        
            try
            {
                var acFrame = GetFrame();
                var pointList = acFrame.PRPD.Select(pointF => new AcousticUtils.PDPoint() {X = pointF.X, Y = pointF.Y}).ToArray();
                var pdSummary = new AcousticUtils.PDSummary();
                AcousticUtils.acoustic_get_discharge_level(pointList, pointList.Length, ref pdSummary);

                return (PDLevel)pdSummary.Level;
            }
            catch(Exception e)
            {
                return PDLevel.None;
            }
        }

        /// <summary>
        /// 绘制可见光蒙版
        /// </summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="scope"></param>
        private void DrawFocus(Graphics g, int width, int height, float scope)
        {
            var d = scope * Math.Min(width, height);
            SolidBrush b = new SolidBrush(Color.FromArgb((int)(0xff * 0.2), Color.White));
            GraphicsPath path1 = new GraphicsPath();
            GraphicsPath path2 = new GraphicsPath();
            path1.AddRectangle(new RectangleF(0, 0, width, height));
            path2.AddEllipse(new RectangleF(((width - d) / 2), ((height - d) / 2), d, d));
            Region region = new Region(path1);
            region.Exclude(path2);
            g.FillRegion(b, region);
            var pen = new Pen(Color.Black, 3);
            g.DrawPath(pen, path2);
        }

        /// <summary>
        /// 获取频率范围
        /// </summary>
        /// <returns></returns>
        public Band GetFrequencyRange()
        {
            if (HImage == IntPtr.Zero)
            {
                return Band.DEFAULT;
            }

            IRtekACFileSDK.ac_file_band acTempRange = new IRtekACFileSDK.ac_file_band();
            int result = IRtekACFileSDK.ac_file_get_frequency_range(HImage, ref acTempRange);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                Band value = new Band
                {
                    Min = acTempRange.min,
                    Max = acTempRange.max
                };

                return value;
            }
            else
            {
                return Band.DEFAULT;
            }
        }

        /// <summary>
        /// 获取声像图尺寸
        /// </summary>
        /// <returns></returns>
        public Size GetAcousticSize()
        {
            if (HImage == IntPtr.Zero)
            {
                return new Size(0, 0);
            }

            IRtekACFileSDK.ac_file_size acSize = new IRtekACFileSDK.ac_file_size();
            int result = IRtekACFileSDK.ac_file_get_acoustic_size(HImage, ref acSize);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                Size value = new Size(acSize.width, acSize.height);
                return value;
            }

            return new Size(0, 0);
        }

        /// <summary>
        /// 获取GPS信息
        /// </summary>
        /// <returns></returns>
        public GeoLocation GetGeoLocation()
        {
            if (HImage == IntPtr.Zero)
            {
                return GeoLocation.DEFAULT;
            }

            IRtekACFileSDK.ac_file_geo_location acGeoLocation = new IRtekACFileSDK.ac_file_geo_location();
            int result = IRtekACFileSDK.ac_file_get_geo_location(HImage, ref acGeoLocation);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                GeoLocation value = new GeoLocation();
                value.IsGPSValid = acGeoLocation.isGPSValid == TRUE;
                value.Longitude = acGeoLocation.longitude;
                value.Latitude = acGeoLocation.latitude;
                value.Altitude = acGeoLocation.altitude;
                value.IsCompassValid = acGeoLocation.isCompassValid == TRUE;
                value.Direction = acGeoLocation.direction;
                return value;
            }
            else
            {
                return GeoLocation.DEFAULT;
            }
        }

        /// <summary>
        /// 获取拍摄时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetShotTime()
        {
            if (HImage == IntPtr.Zero)
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            ulong lSpan = 0;
            int result = IRtekACFileSDK.ac_file_get_shot_time(HImage, ref lSpan);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan tsSpan = new TimeSpan((long)lSpan * TimeSpan.TicksPerMillisecond);
                return (dtStart + tsSpan).ToLocalTime();
            }
            else
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// 获取修改时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetModifiedTime()
        {
            if (HImage == IntPtr.Zero)
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }

            ulong lSpan = 0;
            int result = IRtekACFileSDK.ac_file_get_modified_time(HImage, ref lSpan);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan tsSpan = new TimeSpan((long)lSpan * TimeSpan.TicksPerMillisecond);
                return (dtStart + tsSpan).ToLocalTime();
            }
            else
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// 获取音频文件
        /// </summary>
        /// <returns></returns>
        public AudioAnnotation GetAudioAnnotation()
        {
            if (HImage == IntPtr.Zero)
            {
                return new AudioAnnotation(false, AudioType.Mp3, null);
            }

            IRtekACFileSDK.ac_file_audio acAudio = new IRtekACFileSDK.ac_file_audio();
            int result = IRtekACFileSDK.ac_file_get_audio(HImage, ref acAudio);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                try
                {
                    acAudio.buffer = Marshal.AllocHGlobal(acAudio.length);
                    result = IRtekACFileSDK.ac_file_get_audio(HImage, ref acAudio);
                    if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                    {
                        bool isValid = acAudio.length != 0;
                        AudioType type = (AudioType)acAudio.type;
                        byte[] buffer = new byte[acAudio.length];
                        Marshal.Copy(acAudio.buffer, buffer, 0, acAudio.length);
                        return new AudioAnnotation(isValid, type, buffer);
                    }
                    else
                    {
                        return new AudioAnnotation(false, AudioType.Mp3, null);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(acAudio.buffer);
                }
            }
            else
            {
                return new AudioAnnotation(false, AudioType.Mp3, null);
            }
        }

        /// <summary>
        /// 获取文本备注信息
        /// </summary>
        /// <returns></returns>
        public string GetNoteAnnotation()
        {
            if (HImage == IntPtr.Zero)
            {
                return string.Empty;
            }

            IRtekACFileSDK.ac_file_buffer acNote = new IRtekACFileSDK.ac_file_buffer();
            int result = IRtekACFileSDK.ac_file_get_note(HImage, ref acNote);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                try
                {
                    acNote.buffer = Marshal.AllocHGlobal(acNote.length);
                    result = IRtekACFileSDK.ac_file_get_note(HImage, ref acNote);
                    if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                    {
                        byte[] buffer = new byte[acNote.length];
                        Marshal.Copy(acNote.buffer, buffer, 0, acNote.length);
                        return BytesToString(buffer, Encoding.UTF8);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(acNote.buffer);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 设置文本备注信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetNoteAnnotation(string value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            IRtekACFileSDK.ac_file_buffer acNote = new IRtekACFileSDK.ac_file_buffer();
            byte[] buffer = StringToBytes(value, Encoding.UTF8);
            try
            {
                acNote.length = buffer.Length;
                acNote.buffer = Marshal.AllocHGlobal(acNote.length);
                Marshal.Copy(buffer, 0, acNote.buffer, acNote.length);
                int result = IRtekACFileSDK.ac_file_set_note(HImage, ref acNote);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    return false;
                }

                IsModified = true;
            }
            finally
            {
                Marshal.FreeHGlobal(acNote.buffer);
            }

            return true;
        }

        /// <summary>
        /// 获取用户标志Tag
        /// </summary>
        /// <returns></returns>
        public string[] GetTagAnnotation()
        {
            if (HImage == IntPtr.Zero)
            {
                return new string[0];
            }

            return GetUserData(USER_DATA_KEY_TAG).Split(new string[] { USER_DATA_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 设置用户标志Tag
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetTagAnnotation(string[] value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            return SetUserData(USER_DATA_KEY_TAG, string.Join(USER_DATA_SEPARATOR, value));
        }

        /// <summary>
        /// 获取用户标识Favorite
        /// </summary>
        /// <returns></returns>
        public bool GetFavorite()
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            return USER_DATA_FAVORITE_YES.Equals(GetUserData(USER_DATA_KEY_FAVORITE));
        }

        /// <summary>
        /// 设置用户标识Favorite
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetFavorite(bool value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            return SetUserData(USER_DATA_KEY_FAVORITE, value ? USER_DATA_FAVORITE_YES : USER_DATA_FAVORITE_NO);
        }

        /// <summary>
        /// 获取用户标识GroupFiles
        /// </summary>
        /// <returns></returns>
        public string[] GetGroupFiles()
        {
            if (HImage == IntPtr.Zero)
            {
                return new string[0];
            }

            return GetUserData(USER_DATA_KEY_GROUPFILES).Split(new string[] { USER_DATA_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 设置用户标识GroupFiles
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetGroupFiles(string[] value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            return SetUserData(USER_DATA_KEY_TAG, string.Join(USER_DATA_SEPARATOR, value));
        }

        /// <summary>
        /// 获取用户标识TimerFileInfo
        /// </summary>
        /// <returns></returns>
        public TimerFileInfo GetTimerFileInfo()
        {
            if (HImage == IntPtr.Zero)
            {
                return null;
            }

            string sTimerFileInfo = GetUserData(USER_DATA_KEY_TIMERFILE);
            if (!string.IsNullOrEmpty(sTimerFileInfo))
            {
                string[] s = sTimerFileInfo.Split(new string[] { USER_DATA_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                return new TimerFileInfo(s[0], Convert.ToInt32(s[1]));
            }
            return null;
        }

        /// <summary>
        /// 设置用户标识TimerFileInfo
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetTimerFileInfo(TimerFileInfo value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            string sTimerFileInfo = string.Empty;
            if (value != null)
            {
                sTimerFileInfo = string.Format("{0}\n{1:D}", value.GUID, value.No);
            }

            return SetUserData(USER_DATA_KEY_TIMERFILE, sTimerFileInfo);
        }

        /// <summary>
        /// 获取声宽
        /// </summary>
        /// <returns></returns>
        public DbScale GetDbScale()
        {
            if (HImage == IntPtr.Zero)
            {
                return DbScale.DEFAULT;
            }

            IRtekACFileSDK.ac_file_db_scale_v2 acDbScale = new IRtekACFileSDK.ac_file_db_scale_v2();
            int result = IRtekACFileSDK.ac_file_get_db_scale_v2(HImage, ref acDbScale);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return DbScale.DEFAULT;
            }

            DbScale dbScale = new DbScale();
            switch (acDbScale.mode)
            {
                case IRtekACFileSDK.AC_FILE_AUTO_MONO:
                    dbScale.Mode = DbScaleMode.AutoMono;
                    break;
                case IRtekACFileSDK.AC_FILE_AUTO_MULTI:
                    dbScale.Mode = DbScaleMode.AutoMulti;
                    break;
                case IRtekACFileSDK.AC_FILE_FIXED:
                    dbScale.Mode = DbScaleMode.Fixed;
                    break;
                case IRtekACFileSDK.AC_FILE_AUTO_HOLO:
                    dbScale.Mode = DbScaleMode.AutoHolo;
                    break;
                default:
                    break;
            }
            dbScale.Max = acDbScale.max;
            dbScale.Min = acDbScale.min;
            dbScale.FixedSpan = acDbScale.fixedSpan != FALSE;
            dbScale.Span = acDbScale.span;
            dbScale.Sensitivity = acDbScale.sensitivity;
            dbScale.SourceSize = acDbScale.sourceSize;
            dbScale.Limit = acDbScale.limit;
            return dbScale;
        }

        /// <summary>
        /// 设置声宽
        /// </summary>
        /// <param name="value"></param>
        /// <param name="handleEvent"></param>
        /// <returns></returns>
        public bool SetDbScale(DbScale value, bool handleEvent = true)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            IRtekACFileSDK.ac_file_db_scale_v2 dbScale = new IRtekACFileSDK.ac_file_db_scale_v2();
            switch (value.Mode)
            {
                case DbScaleMode.AutoMono:
                    dbScale.mode = IRtekACFileSDK.AC_FILE_AUTO_MONO;
                    break;
                case DbScaleMode.AutoMulti:
                    dbScale.mode = IRtekACFileSDK.AC_FILE_AUTO_MULTI;
                    break;
                case DbScaleMode.Fixed:
                    dbScale.mode = IRtekACFileSDK.AC_FILE_FIXED;
                    break;
                case DbScaleMode.AutoHolo:
                    dbScale.mode = IRtekACFileSDK.AC_FILE_AUTO_HOLO;
                    break;
                default:
                    dbScale.mode = IRtekACFileSDK.AC_FILE_FIXED;
                    break;
            }
            dbScale.max = value.Max;
            dbScale.min = value.Min;
            dbScale.fixedSpan = value.FixedSpan ? TRUE : FALSE;
            dbScale.span = value.Span;
            dbScale.sensitivity = value.Sensitivity;
            dbScale.sourceSize = value.SourceSize;
            dbScale.limit = value.Limit;
            int result = IRtekACFileSDK.ac_file_set_db_scale_v2(HImage, ref dbScale);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            if (handleEvent)
            {
                IsModified = true;

                HandleDbScaleChangedEvent();
            }

            return true;
        }

        /// <summary>
        /// 获取可见光灰度模式
        /// </summary>
        /// <returns></returns>
        public bool GetGreyScale()
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            byte value = FALSE;
            int result = IRtekACFileSDK.ac_file_get_grey_scale(HImage, ref value);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return value == TRUE;
            }

            return false;
        }

        /// <summary>
        /// 获取检测模式
        /// </summary>
        /// <returns></returns>
        public CaptureMode GetCaptureMode()
        {
            if (HImage == IntPtr.Zero)
            {
                return CaptureMode.Other;
            }

            byte value = (byte)CaptureMode.Other;
            int result = IRtekACFileSDK.ac_file_get_capture_mode(HImage, ref value);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                switch (value)
                {
                    case IRtekACFileSDK.AC_FILE_CAPTURE_MODE_LEAK:
                        return CaptureMode.LeakMode;
                    case IRtekACFileSDK.AC_FILE_CAPTURE_MODE_PD:
                        return CaptureMode.PDMode;
                }
                return CaptureMode.Other;
            }

            return CaptureMode.Other;
        }

        /// <summary>
        /// 获取聚焦模式
        /// </summary>
        /// <returns></returns>
        public FocusMode GetFocusMode()
        {
            if (HImage == IntPtr.Zero)
            {
                return FocusMode.DEFAULT;
            }

            var acFocusMode = new IRtekACFileSDK.ac_file_focus_mode();
            int result = IRtekACFileSDK.ac_file_get_focus_mode(HImage, ref acFocusMode);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                var focusMode = new FocusMode
                {
                    Enabled = acFocusMode.enabled == TRUE,
                    Scope = acFocusMode.scope
                };

                return focusMode;
            }

            return FocusMode.DEFAULT;
        }

        /// <summary>
        /// 设置聚焦模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetFocusMode(FocusMode value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            IRtekACFileSDK.ac_file_focus_mode focusMode = new IRtekACFileSDK.ac_file_focus_mode()
            {
                enabled = value.Enabled ? TRUE : FALSE,
                scope = value.Scope
            };

            int result = IRtekACFileSDK.ac_file_set_focus_mode(HImage, ref focusMode);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            IsModified = true;

            HandleFocusModeChangedEvent();

            return true;
        }

        /// <summary>
        /// 获取目标距离
        /// </summary>
        /// <returns></returns>
        public float GetDistance()
        {
            if (HImage == IntPtr.Zero)
            {
                return 0;
            }

            float value = 0;
            int result = IRtekACFileSDK.ac_file_get_distance(HImage, ref value);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return value;
            }

            return 0;
        }

        /// <summary>
        /// 设置目标距离
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetDistance(float value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            int result = IRtekACFileSDK.ac_file_set_distance(HImage, value);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            IsModified = true;

            return true;
        }

        /// <summary>
        /// 获取标准声压级启用标识
        /// </summary>
        /// <returns></returns>
        public bool GetNormalizeDistance()
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            byte value = FALSE;
            int result = IRtekACFileSDK.ac_file_get_normalize_distance(HImage, ref value);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return value == TRUE;
            }

            return false;
        }

        /// <summary>
        /// 设置标准声压级启用标识
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetNormalizeDistance(bool value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            byte setValue = value ? TRUE : FALSE;
            int result = IRtekACFileSDK.ac_file_set_normalize_distance(HImage, setValue);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            IsModified = true;
            foreach (Marker marker in Markers)
            {
                var fileMarker = GetMarker(marker.Name);
                marker.MaxInternal = fileMarker.MaxInternal;
                HandleMarkerChangedEvent(marker);
            }

            return true;
        }

        /// <summary>
        /// 读PRPD散点图对应交流电频率
        /// </summary>
        /// <returns></returns>
        public float GetAcFrequency()
        {
            if (HImage == IntPtr.Zero)
            {
                return 0;
            }

            float value = 0;
            int result = IRtekACFileSDK.ac_file_get_ac_frequency(HImage, ref value);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return value;
            }

            return 0;
        }

        /// <summary>
        /// 获取调色板
        /// </summary>
        /// <returns></returns>
        public Palette GetPalette()
        {
            if (HImage == IntPtr.Zero)
            {
                return Palette.DEFAULT;
            }

            IRtekACFileSDK.ac_file_palette acPalette = new IRtekACFileSDK.ac_file_palette();
            int result = IRtekACFileSDK.ac_file_get_palette(HImage, ref acPalette);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return Palette.DEFAULT;
            }

            Palette palette = new Palette
            {
                RefNo = acPalette.refNo,
                Colors = acPalette.colors,
                Alpha = acPalette.alpha
            };

            return palette;
        }

        /// <summary>
        /// 设置调色板
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPalette(Palette value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            IRtekACFileSDK.ac_file_palette acPalette = new IRtekACFileSDK.ac_file_palette
            {
                refNo = value.RefNo,
                colors = value.Colors,
                alpha = value.Alpha
            };

            int result = IRtekACFileSDK.ac_file_set_palette(HImage, ref acPalette);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            IsModified = true;

            HandlePaletteChangedEvent();

            return true;
        }

        /// <summary>
        /// 获取数码变焦倍数
        /// </summary>
        /// <returns></returns>
        public ZoomInfo GetZoomInfo()
        {
            if (HImage == IntPtr.Zero)
            {
                return ZoomInfo.DEFAULT;
            }

            IRtekACFileSDK.ac_file_zoom_info acZoomInfo = new IRtekACFileSDK.ac_file_zoom_info();
            int result = IRtekACFileSDK.ac_file_get_zoom_info(HImage, ref acZoomInfo);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return ZoomInfo.DEFAULT;
            }

            ZoomInfo zoomInfo = new ZoomInfo
            {
                Enabled = acZoomInfo.enabled == TRUE,
                Location = new PointF(acZoomInfo.location.x, acZoomInfo.location.y),
                Ratio = acZoomInfo.ratio
            };

            return zoomInfo;
        }

        /// <summary>
        /// 获取帧总数
        /// </summary>
        /// <returns></returns>
        public int GetFrameCount()
        {
            if (HImage == IntPtr.Zero)
            {
                return 0;
            }

            uint acCount = 0;
            int result = IRtekACFileSDK.ac_file_get_frame_count(HImage, ref acCount);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return (int)acCount;
            }

            return 0;
        }

        /// <summary>
        /// 获取帧率
        /// </summary>
        /// <returns></returns>
        public float GetFrameRate()
        {
            if (HImage == IntPtr.Zero)
            {
                return 1.0F;
            }

            float acFrameRate = 0;
            int result = IRtekACFileSDK.ac_file_get_frame_rate(HImage, ref acFrameRate);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return acFrameRate;
            }
            else
            {
                return 1.0F;
            }
        }

        /// <summary>
        /// 设置帧序号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetFrameIndex(int value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            int result = IRtekACFileSDK.ac_file_set_frame_index(HImage, (uint)value);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            HandleFrameIndexChangedEvent();

            return true;
        }

        /// <summary>
        /// 获取当前帧
        /// </summary>
        /// <returns></returns>
        public Frame GetFrame()
        {
            var prpdPointSize = 0;
            unsafe
            {
                prpdPointSize = sizeof(IRtekACFileSDK.ac_file_point_f);
            }

            var size = GetAcousticSize();

            var acFrame = new IRtekACFileSDK.ac_file_frame();
            if (IRtekACFileSDK.ac_file_get_frame(HImage, GetFrameIndex(), ref acFrame) != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                throw new Exception("ac_file_get_frame fail.");
            }

            try
            {
                acFrame.acoustic_image_spl.buffer = Marshal.AllocHGlobal(acFrame.acoustic_image_spl.count * sizeof(float));
                acFrame.visual_image.buffer = Marshal.AllocHGlobal(acFrame.visual_image.count);
                acFrame.sound.source.buffer = Marshal.AllocHGlobal(acFrame.sound.source.count * sizeof(float));
                acFrame.sound.filter.buffer = Marshal.AllocHGlobal(acFrame.sound.filter.count * sizeof(float));
                acFrame.sound.down_sample.buffer = Marshal.AllocHGlobal(acFrame.sound.down_sample.count * sizeof(float));
                acFrame.spectrum.fft.buffer = Marshal.AllocHGlobal(acFrame.spectrum.fft.count * sizeof(float));
                acFrame.prpd.points = Marshal.AllocHGlobal((int)(acFrame.prpd.points_count * prpdPointSize));
                if (IRtekACFileSDK.ac_file_get_frame(HImage, GetFrameIndex(), ref acFrame) != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    throw new Exception("ac_file_get_frame fail.");
                }

                var currentFrame = new Frame
                {
                    TimeStamp = acFrame.timeStamp,
                    AcousticImageSpl = new float[size.Height, size.Width],
                    VisualImage = new byte[acFrame.visual_image.count],
                    Sound = new Sound()
                    {
                        Source = new float[acFrame.sound.source.count],
                        Filter = new float[acFrame.sound.filter.count],
                        DownSample = new float[acFrame.sound.down_sample.count],
                    },
                    Spectrum = new Spectrum()
                    {
                        FFT = new float[acFrame.spectrum.fft.count],
                        Range = new Band()
                        {
                            Max = acFrame.spectrum.range.max,
                            Min = acFrame.spectrum.range.min,
                        },
                        Delta = acFrame.spectrum.delta,
                    },
                    PRPD = new PointF[acFrame.prpd.points_count]
                };

                var tempBuffer = new float[acFrame.acoustic_image_spl.count];
                Marshal.Copy(acFrame.acoustic_image_spl.buffer, tempBuffer, 0, acFrame.acoustic_image_spl.count);
                Buffer.BlockCopy(tempBuffer, 0, currentFrame.AcousticImageSpl, 0, tempBuffer.Length * sizeof(float));
                Marshal.Copy(acFrame.visual_image.buffer, currentFrame.VisualImage, 0, currentFrame.VisualImage.Length);
                Marshal.Copy(acFrame.sound.source.buffer, currentFrame.Sound.Source, 0, currentFrame.Sound.Source.Length);
                Marshal.Copy(acFrame.sound.filter.buffer, currentFrame.Sound.Filter, 0, currentFrame.Sound.Filter.Length);
                Marshal.Copy(acFrame.sound.down_sample.buffer, currentFrame.Sound.DownSample, 0, currentFrame.Sound.DownSample.Length);
                Marshal.Copy(acFrame.spectrum.fft.buffer, currentFrame.Spectrum.FFT, 0, currentFrame.Spectrum.FFT.Length);

                var pointCount = acFrame.prpd.points_count;
                var data = new float[pointCount * prpdPointSize / sizeof(float)];
                Marshal.Copy(acFrame.prpd.points, data, 0, data.Length);
                for (int i = 0; i < pointCount; i++)
                {
                    currentFrame.PRPD[i] = new PointF(data[2 * i], data[2 * i + 1]);
                }

                return currentFrame;
            }
            finally
            {
                Marshal.FreeHGlobal(acFrame.acoustic_image_spl.buffer);
                Marshal.FreeHGlobal(acFrame.visual_image.buffer);
                Marshal.FreeHGlobal(acFrame.sound.source.buffer);
                Marshal.FreeHGlobal(acFrame.sound.filter.buffer);
                Marshal.FreeHGlobal(acFrame.sound.down_sample.buffer);
                Marshal.FreeHGlobal(acFrame.spectrum.fft.buffer);
                Marshal.FreeHGlobal(acFrame.prpd.points);
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Open(string fileName)
        {
            if (HImage != IntPtr.Zero)
            {
                _isMarkersSynced = false;
                Markers.Clear();
                _acousticImageCache = null;
                _paletteImageCache = null;
                _visualImageCache = null;
                _coverImageCache = null;
                IRtekACFileSDK.ac_file_close(HImage);
                HImage = IntPtr.Zero;
            }

            var fileType = PeekFileType(fileName);
            if (fileType == FileType.Other)
            {
                return false;
            }

            IntPtr h = IntPtr.Zero;
            int result = IRtekACFileSDK.ac_file_open(fileName, ref h);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                HImage = h;
                _isMarkersSynced = false;
                string[] names = GetMarkerNames();
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i];
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    Marker marker = GetMarker(name);
                    if (marker == null)
                    {
                        continue;
                    }

                    Markers.Add(marker);
                }

                _isMarkersSynced = true;

                IsModified = false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            int result = IRtekACFileSDK.ac_file_save(HImage);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            IsModified = false;

            return true;
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool SaveAs(string fileName)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            int result = IRtekACFileSDK.ac_file_save_as(HImage, fileName);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return false;
            }

            IsModified = false;

            return true;
        }

        public bool SaveFrameAs(string fileName, Image coverImage)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            MemoryStream ms = new MemoryStream();
            coverImage.Save(ms, ImageFormat.Jpeg);
            byte[] buffer = ms.GetBuffer();
            IRtekACFileSDK.ac_file_buffer irBuffer = new IRtekACFileSDK.ac_file_buffer();

            try
            {
                irBuffer.length = buffer.Length;
                irBuffer.buffer = Marshal.AllocHGlobal(irBuffer.length);
                Marshal.Copy(buffer, 0, irBuffer.buffer, irBuffer.length);

                int result = IRtekACFileSDK.ac_file_save_frame_as(HImage, fileName, ref irBuffer);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    return false;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(irBuffer.buffer);
            }

            return true;
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close()
        {
            if (HImage == IntPtr.Zero)
            {
                return;
            }

            _isMarkersSynced = false;
            Markers.Clear();
            _acousticImageCache = null;
            _paletteImageCache = null;
            _visualImageCache = null;
            _coverImageCache = null;
            IRtekACFileSDK.ac_file_close(HImage);
            HImage = IntPtr.Zero;
            IsModified = false;
        }

        /// <summary>
        /// 获取用户键
        /// </summary>
        /// <returns></returns>
        public string[] GetUserDataKeys()
        {
            if (HImage == IntPtr.Zero)
            {
                return new string[0];
            }

            IRtekACFileSDK.ac_file_keys acKeyList = new IRtekACFileSDK.ac_file_keys();
            int result = IRtekACFileSDK.ac_file_get_keys(HImage, ref acKeyList);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                try
                {
                    int bufferSize = acKeyList.count * IRtekACFileSDK.KEY_LEN;
                    acKeyList.keys = Marshal.AllocHGlobal(bufferSize);
                    result = IRtekACFileSDK.ac_file_get_keys(HImage, ref acKeyList);
                    if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                    {
                        byte[] buffer = new byte[bufferSize];
                        Marshal.Copy(acKeyList.keys, buffer, 0, bufferSize);
                        string[] keys = new string[acKeyList.count];
                        for (int i = 0; i < acKeyList.count; i++)
                        {
                            keys[i] = BytesToString(buffer, i * IRtekACFileSDK.KEY_LEN, IRtekACFileSDK.KEY_LEN, Encoding.UTF8);
                        }
                        return keys;
                    }
                    else
                    {
                        return new string[0];
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(acKeyList.keys);
                }
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetUserData(string key)
        {
            if (HImage == IntPtr.Zero)
            {
                return string.Empty;
            }

            IRtekACFileSDK.ac_file_kvPair acKVPair = new IRtekACFileSDK.ac_file_kvPair();
            acKVPair.key = StringToBytes(key, IRtekACFileSDK.KEY_LEN, Encoding.UTF8);
            int result = IRtekACFileSDK.ac_file_get_user_data(HImage, ref acKVPair);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                try
                {
                    acKVPair.vBuffer = Marshal.AllocHGlobal(acKVPair.vLength);
                    result = IRtekACFileSDK.ac_file_get_user_data(HImage, ref acKVPair);
                    if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                    {
                        byte[] buffer = new byte[acKVPair.vLength];
                        Marshal.Copy(acKVPair.vBuffer, buffer, 0, acKVPair.vLength);
                        return BytesToString(buffer, Encoding.UTF8);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(acKVPair.vBuffer);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 设置用户数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetUserData(string key, string value)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            IRtekACFileSDK.ac_file_kvPair irKVPair = new IRtekACFileSDK.ac_file_kvPair();
            irKVPair.key = StringToBytes(key, IRtekACFileSDK.KEY_LEN, Encoding.UTF8);
            byte[] buffer = StringToBytes(value, Encoding.UTF8);
            irKVPair.vLength = buffer.Length;
            try
            {
                irKVPair.vBuffer = Marshal.AllocHGlobal(irKVPair.vLength);
                Marshal.Copy(buffer, 0, irKVPair.vBuffer, irKVPair.vLength);
                int result = IRtekACFileSDK.ac_file_set_user_data(HImage, ref irKVPair);
                if (result != IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    return false;
                }

                IsModified = true;
            }
            finally
            {
                Marshal.FreeHGlobal(irKVPair.vBuffer);
            }

            return true;
        }

        public RotateType GetRotateType()
        {
            if (HImage == IntPtr.Zero)
            {
                return RotateType.Rotate0;
            }

            int rotateType = IRtekACFileSDK.AC_FILE_ROTATE_0;
            int result = IRtekACFileSDK.ac_file_get_rotation(HImage, ref rotateType);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                RotateType type;
                switch (rotateType)
                {
                    case IRtekACFileSDK.AC_FILE_ROTATE_0:
                        type = RotateType.Rotate0;
                        break;
                    case IRtekACFileSDK.AC_FILE_ROTATE_90:
                        type = RotateType.Rotate90;
                        break;
                    case IRtekACFileSDK.AC_FILE_ROTATE_180:
                        type = RotateType.Rotate180;
                        break;
                    case IRtekACFileSDK.AC_FILE_ROTATE_270:
                        type = RotateType.Rotate270;
                        break;
                    default:
                        type = RotateType.Rotate0;
                        break;
                }
                return type;
            }
            else
            {
                return RotateType.Rotate0;
            }
        }

        public void Rotate(RotateType rotation)
        {
            if (HImage == IntPtr.Zero)
            {
                return;
            }

            int irRotation = IRtekACFileSDK.AC_FILE_ROTATE_90;
            switch (rotation)
            {
                case RotateType.Rotate90:
                default:
                    irRotation = IRtekACFileSDK.AC_FILE_ROTATE_90;
                    break;
                case RotateType.Rotate180:
                    irRotation = IRtekACFileSDK.AC_FILE_ROTATE_180;
                    break;
                case RotateType.Rotate270:
                    irRotation = IRtekACFileSDK.AC_FILE_ROTATE_270;
                    break;
            }

            int result = IRtekACFileSDK.ac_file_set_rotation(HImage, irRotation);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                IsModified = true;

                HandleRotationTypeChangedEvent();
            }
        }

        /// <summary>
        /// 获取点数据
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetSpotValue(Point point)
        {
            if (HImage == IntPtr.Zero)
            {
                return 0.0F;
            }

            IRtekACFileSDK.ac_file_db_point dbPoint = new IRtekACFileSDK.ac_file_db_point
            {
                point = new IRtekACFileSDK.ac_file_point
                {
                    x = (short)point.X,
                    y = (short)point.Y
                }
            };
            int result = IRtekACFileSDK.ac_file_get_db_spot(HImage, ref dbPoint);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return 0;
            }

            var mode = GetCaptureMode();
            var nor = GetNormalizeDistance();
            var distance = GetDistance();
            if (mode == CaptureMode.PDMode && nor)
            {
                return (float)(dbPoint.value + 20 * Math.Log10(distance));
            }

            return dbPoint.value;
        }

        /// <summary>
        /// 获取椭圆数据
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public float[] GetEllipseValues(Rectangle rectangle)
        {
            if (HImage == IntPtr.Zero)
            {
                return new float[] { };
            }

            IRtekACFileSDK.ac_file_db_rect acRectangle = new IRtekACFileSDK.ac_file_db_rect();
            IRtekACFileSDK.ac_file_rectangle irRectangle = new IRtekACFileSDK.ac_file_rectangle();
            irRectangle.x = (short)rectangle.X;
            irRectangle.y = (short)rectangle.Y;
            irRectangle.width = (short)rectangle.Width;
            irRectangle.height = (short)rectangle.Height;
            acRectangle.rect = irRectangle;
            int result = IRtekACFileSDK.ac_file_get_db_ellipse(HImage, ref acRectangle);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return new float[] { };
            }

            try
            {
                var count = (int)acRectangle.count;
                acRectangle.value = Marshal.AllocHGlobal(count * sizeof(float));
                result = IRtekACFileSDK.ac_file_get_db_ellipse(HImage, ref acRectangle);
                if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    float[] buffer = new float[count];
                    Marshal.Copy(acRectangle.value, buffer, 0, count);

                    var mode = GetCaptureMode();
                    var nor = GetNormalizeDistance();
                    var distance = GetDistance();
                    if (mode == CaptureMode.PDMode && nor)
                    {
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            buffer[i] = (float)(buffer[i] + 20 * Math.Log10(distance));
                        }
                    }

                    return buffer;
                }
                else
                {
                    return new float[] { };
                }
            }
            finally
            {
                Marshal.FreeHGlobal(acRectangle.value);
            }
        }

        /// <summary>
        /// 获取测量标识名称列表
        /// </summary>
        /// <returns></returns>
        private string[] GetMarkerNames()
        {
            if (HImage == IntPtr.Zero)
            {
                return new string[0];
            }

            IRtekACFileSDK.ac_file_marker_names irMarkerNameList = new IRtekACFileSDK.ac_file_marker_names();
            int result = IRtekACFileSDK.ac_file_get_marker_names(HImage, ref irMarkerNameList);
            if (result != IRtekACFileSDK.AC_FILE_EC_OK)
            {
                return new string[0];
            }

            try
            {
                int bufferSize = irMarkerNameList.count * IRtekACFileSDK.NAME_LEN;
                irMarkerNameList.names = Marshal.AllocHGlobal(bufferSize);
                result = IRtekACFileSDK.ac_file_get_marker_names(HImage, ref irMarkerNameList);
                if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    byte[] buffer = new byte[bufferSize];
                    Marshal.Copy(irMarkerNameList.names, buffer, 0, bufferSize);
                    string[] markerNames = new string[irMarkerNameList.count];
                    for (int i = 0; i < irMarkerNameList.count; i++)
                    {
                        markerNames[i] = BytesToString(buffer, i * IRtekACFileSDK.NAME_LEN, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
                    }
                    return markerNames;
                }
                else
                {
                    return new string[0];
                }
            }
            finally
            {
                Marshal.FreeHGlobal(irMarkerNameList.names);
            }
        }

        /// <summary>
        /// 获取测量标识
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Marker GetMarker(string name)
        {
            if (HImage == IntPtr.Zero)
            {
                return null;
            }

            IRtekACFileSDK.ac_file_marker acFileMarker = new IRtekACFileSDK.ac_file_marker();
            acFileMarker.name = StringToBytes(name, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
            int result = IRtekACFileSDK.ac_file_get_marker(HImage, ref acFileMarker);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                try
                {
                    int size = 0;
                    unsafe
                    {
                        size = sizeof(IRtekACFileSDK.ac_file_point);
                    }

                    acFileMarker.points = Marshal.AllocHGlobal(acFileMarker.point_count * size);
                    result = IRtekACFileSDK.ac_file_get_marker(HImage, ref acFileMarker);
                    if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                    {
                        IRtekACFileSDK.ac_file_point p;
                        Marker marker;
                        switch (acFileMarker.type)
                        {
                            case IRtekACFileSDK.AC_FILE_SPOT:
                                p = (IRtekACFileSDK.ac_file_point)Marshal.PtrToStructure(acFileMarker.points, typeof(IRtekACFileSDK.ac_file_point));
                                Point spot = new Point(p.x, p.y);
                                marker = new MarkerSpot(this, BytesToString(acFileMarker.name, Encoding.UTF8), spot);
                                break;

                            case IRtekACFileSDK.AC_FILE_ELLIPSE:
                                Rectangle rect = new Rectangle();
                                p = (IRtekACFileSDK.ac_file_point)Marshal.PtrToStructure(acFileMarker.points, typeof(IRtekACFileSDK.ac_file_point));
                                Point p1 = new Point(p.x, p.y);
                                p = (IRtekACFileSDK.ac_file_point)Marshal.PtrToStructure(acFileMarker.points + size, typeof(IRtekACFileSDK.ac_file_point));
                                Point p2 = new Point(p.x, p.y);
                                rect.Location = new Point(p1.X <= p2.X ? p1.X : p2.X, p1.Y <= p2.Y ? p1.Y : p2.Y);
                                rect.Size = new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
                                marker = new MarkerEllipse(this, BytesToString(acFileMarker.name, Encoding.UTF8), rect);
                                break;
                            default:
                                return null;
                        }

                        float temp = acFileMarker.db_value;
                        var nor = GetNormalizeDistance();
                        var mode = GetCaptureMode();
                        var distance = GetDistance();
                        if (mode == CaptureMode.PDMode && nor)
                        {
                            temp = (float)(temp + 20 * Math.Log10(distance));
                        }
                        marker.MaxInternal = temp;
                        marker.MaxSpotInternal = new Point(acFileMarker.point.x, acFileMarker.point.y);

                        return marker;
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(acFileMarker.points);
                }
            }
            return null;
        }

        /// <summary>
        /// 设置测量标识
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        private bool SetMarker(Marker marker)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            IRtekACFileSDK.ac_file_marker acMarker = new IRtekACFileSDK.ac_file_marker();
            acMarker.name = StringToBytes(marker.Name, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
            try
            {
                int size = 0;
                unsafe
                {
                    size = sizeof(IRtekACFileSDK.ac_file_point);
                }

                IRtekACFileSDK.ac_file_point p;
                switch (marker.Type)
                {
                    case MarkerType.Spot:
                        acMarker.type = IRtekACFileSDK.AC_FILE_SPOT;
                        MarkerSpot markerSpot = marker as MarkerSpot;
                        acMarker.point_count = 1;
                        acMarker.points = Marshal.AllocHGlobal(acMarker.point_count * size);
                        p = new IRtekACFileSDK.ac_file_point
                        {
                            x = (short)markerSpot.Point.X,
                            y = (short)markerSpot.Point.Y
                        };
                        Marshal.StructureToPtr(p, acMarker.points, false);
                        break;
                    case MarkerType.Ellipse:
                        acMarker.type = IRtekACFileSDK.AC_FILE_ELLIPSE;
                        MarkerEllipse markerEllipse = marker as MarkerEllipse;
                        acMarker.point_count = 2;
                        acMarker.points = Marshal.AllocHGlobal(acMarker.point_count * size);
                        p = new IRtekACFileSDK.ac_file_point
                        {
                            x = (short)markerEllipse.Rectangle.X,
                            y = (short)markerEllipse.Rectangle.Y
                        };

                        Marshal.StructureToPtr(p, acMarker.points, false);
                        p = new IRtekACFileSDK.ac_file_point
                        {
                            x = (short)markerEllipse.Rectangle.Right,
                            y = (short)markerEllipse.Rectangle.Bottom
                        };

                        Marshal.StructureToPtr(p, acMarker.points + size, false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                int result = IRtekACFileSDK.ac_file_set_marker(HImage, ref acMarker);
                if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    IsModified = true;
                    float temp = acMarker.db_value;
                    var nor = GetNormalizeDistance();
                    var mode = GetCaptureMode();
                    var distance = GetDistance();
                    if (mode == CaptureMode.PDMode && nor)
                    {
                        temp = (float)(temp + 20 * Math.Log10(distance));
                    }
                    marker.MaxInternal = temp;
                    marker.MaxSpotInternal = new Point(acMarker.point.x, acMarker.point.y);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(acMarker.points);
            }
        }

        /// <summary>
        /// 删除测量标识
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool RemoveMarker(string name)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            byte[] buffer = StringToBytes(name, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
            int result = IRtekACFileSDK.ac_file_remove_marker(HImage, buffer);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK || result == IRtekACFileSDK.AC_FILE_EC_INVALID_KEY)
            {
                IsModified = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 重命名测量标识
        /// </summary>
        /// <param name="fromName"></param>
        /// <param name="toName"></param>
        /// <returns></returns>
        private bool RenameMarker(string fromName, string toName)
        {
            if (HImage == IntPtr.Zero)
            {
                return false;
            }

            byte[] fromBuffer = StringToBytes(fromName, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
            byte[] toBuffer = StringToBytes(toName, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
            int result = IRtekACFileSDK.ac_file_rename_marker(HImage, fromBuffer, toBuffer);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                IsModified = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void DoFrameIndexChanged()
        {
            if (FrameIndexChanged != null)
            {
                FrameIndexChanged(this, new EventArgs());
            }
        }

        private void DoPaletteChanged()
        {
            _paletteImageCache = null;
            if (PaletteChanged != null)
            {
                PaletteChanged(this, new EventArgs());
            }
        }

        private void DoImageChanged()
        {
            _acousticImageCache = null;
            if (ImageChanged != null)
            {
                ImageChanged(this, new EventArgs());
            }
        }

        private void DoMirrorTypeChanged()
        {
            if (MirrorTypeChanged != null)
            {
                MirrorTypeChanged(this, new EventArgs());
            }
        }

        private void DoMarkerChanged(Marker marker)
        {
            if (MarkerChanged != null)
            {
                MarkerChanged(this, new MarkerEventArgs(marker));
            }
        }

        private void DoRotationTypeChanged()
        {
            if (RotationTypeChanged != null)
            {
                RotationTypeChanged(this, new EventArgs());
            }
        }

        private void DoDbScaleChanged()
        {
            if (DbScaleChanged != null)
            {
                DbScaleChanged(this, new EventArgs());
            }
        }

        private void DoMarkerAdded(Marker marker)
        {
            if (MarkerAdded != null)
            {
                MarkerAdded(this, new MarkerEventArgs(marker));
            }
        }

        private void DoMarkerRemoved(Marker marker)
        {
            if (MarkerRemoved != null)
            {
                MarkerRemoved(this, new MarkerEventArgs(marker));
            }
        }

        private void DoMarkerRenamed(string oldName, Marker marker)
        {
            if (MarkerRenamed != null)
            {
                MarkerRenamed(this, new MarkerEventArgs(oldName, marker));
            }
        }

        private void HandleFrameIndexChangedEvent()
        {
            DoFrameIndexChanged();
            DoImageChanged();

            for (int i = 0; i < Markers.Count; i++)
            {
                Marker marker = Markers[i];
                marker.Sync();
                DoMarkerChanged(marker);
            }

            DbScale dbScale = GetDbScale();
            if (dbScale.Mode == DbScaleMode.AutoMulti)
            {
                DoDbScaleChanged();
            }
        }

        private void HandleMirrorTypeChangedEvent()
        {
            DoMirrorTypeChanged();

            for (int i = 0; i < Markers.Count; i++)
            {
                Marker marker = Markers[i];
                marker.Sync();
                DoMarkerChanged(marker);
            }

            DoImageChanged();
        }

        private void HandleRotationTypeChangedEvent()
        {
            DoRotationTypeChanged();

            for (int i = 0; i < Markers.Count; i++)
            {
                Marker marker = Markers[i];
                marker.Sync();
                DoMarkerChanged(marker);
            }

            DoImageChanged();
        }

        private void HandleDbScaleChangedEvent()
        {
            DoDbScaleChanged();
            DoImageChanged();
        }

        private void HandlePaletteChangedEvent()
        {
            DoPaletteChanged();
            DoImageChanged();
        }

        private void HandleFocusModeChangedEvent()
        {
            DoImageChanged();
        }

        private void HandleMarkerAddedEvent(Marker marker)
        {
            DoMarkerAdded(marker);
        }

        private void HandleMarkerChangedEvent(Marker marker)
        {
            DoMarkerChanged(marker);
        }

        private void HandleMarkerRemovedEvent(Marker marker)
        {
            DoMarkerRemoved(marker);
        }

        private void HandleMarkerRenamedEvent(string oldName, Marker marker)
        {
            DoMarkerRenamed(oldName, marker);
        }

        public static List<PresetPalette> GetPresetPalettes()
        {
            List<PresetPalette> presetPalettes = new List<PresetPalette>();

            IRtekACFileSDK.ac_file_preset_palettes irPresetPalettes = new IRtekACFileSDK.ac_file_preset_palettes();
            int result = IRtekACFileSDK.ac_file_get_preset_palettes(ref irPresetPalettes);
            if (result == IRtekACFileSDK.AC_FILE_EC_OK)
            {
                try
                {
                    int size = 1 + IRtekACFileSDK.NAME_LEN;
                    irPresetPalettes.presetPalettes = Marshal.AllocHGlobal(irPresetPalettes.count * size);
                    result = IRtekACFileSDK.ac_file_get_preset_palettes(ref irPresetPalettes);
                    if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                    {
                        for (int i = 0; i < irPresetPalettes.count; i++)
                        {
                            IRtekACFileSDK.ac_file_preset_palette irPresetPalette = (IRtekACFileSDK.ac_file_preset_palette)Marshal.PtrToStructure(irPresetPalettes.presetPalettes + i * size, typeof(IRtekACFileSDK.ac_file_preset_palette));
                            PresetPalette presetPalette = new PresetPalette();
                            presetPalette.RefNo = irPresetPalette.refNo;
                            presetPalette.Name = BytesToString(irPresetPalette.name, Encoding.UTF8);
                            presetPalettes.Add(presetPalette);
                        }
                    }

                    return presetPalettes;
                }
                finally
                {
                    Marshal.FreeHGlobal(irPresetPalettes.presetPalettes);
                }
            }
            else
            {
                return presetPalettes;
            }
        }

        public static Bitmap GetPresetPaletteImage(byte refNo)
        {
            try
            {
                int width = 1;
                int height = IRtekACFileSDK.COLORS_LEN;
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                IRtekACFileSDK.ac_file_image irImage = new IRtekACFileSDK.ac_file_image();
                irImage.offset = bmpData.Stride - 3 * width;
                irImage.data = bmpData.Scan0;
                IRtekACFileSDK.ac_file_get_preset_palette_image(refNo, ref irImage);
                bitmap.UnlockBits(bmpData);
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] StructToByeBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            var bytes = new byte[size];
            var structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }

        public static object BytesToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }

            var structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            var obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        public abstract class Marker
        {
            private string name;

            public Marker(AcousticImageFile owner, string name)
            {
                Owner = owner;
                this.name = name;

                MaxInternal = 313.15F;
                MaxSpotInternal = Point.Empty;
                MeasuredValueInternal = 0.0F;
            }

            public AcousticImageFile Owner { get; private set; }
            public abstract MarkerType Type { get; }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    if (string.Equals(name, value))
                    {
                        return;
                    }

                    string oldValue = name;
                    name = value;
                    if (Owner.RenameMarker(oldValue, name))
                    {
                        Owner.HandleMarkerRenamedEvent(oldValue, this);
                    }
                    else
                    {
                        name = oldValue;
                    }
                }
            }

            public float MaxInternal { get; set; }
            public Point MaxSpotInternal { get; set; }
            public float MeasuredValueInternal { get; set; }

            public abstract void Sync();

            protected static bool Equals(Point[] srcPoints, Point[] destPoints)
            {
                if (srcPoints == destPoints)
                {
                    return true;
                }

                if (srcPoints != null && destPoints != null && srcPoints.Length == destPoints.Length)
                {
                    for (int i = 0; i < srcPoints.Length; i++)
                    {
                        if (srcPoints[i] != destPoints[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public class MarkerSpot : Marker
        {
            private Point point;

            public MarkerSpot(AcousticImageFile owner, string name, Point point)
                : base(owner, name)
            {
                this.point = point;
            }

            public override MarkerType Type
            {
                get { return MarkerType.Spot; }
            }

            public Point Point
            {
                get
                {
                    return point;
                }
                set
                {
                    if (point == value)
                    {
                        return;
                    }

                    Point oldValue = point;
                    point = value;
                    if (Owner.SetMarker(this))
                    {
                        Owner.HandleMarkerChangedEvent(this);
                    }
                    else
                    {
                        point = oldValue;
                    }
                }
            }

            public float Value
            {
                get { return MaxInternal; }
            }

            public override void Sync()
            {
                IntPtr hImage = Owner.HImage;
                if (hImage == IntPtr.Zero)
                {
                    return;
                }

                IRtekACFileSDK.ac_file_marker irMarker = new IRtekACFileSDK.ac_file_marker();
                irMarker.name = StringToBytes(Name, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
                int result = IRtekACFileSDK.ac_file_get_marker(hImage, ref irMarker);
                if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    try
                    {
                        int size = 0;
                        unsafe
                        {
                            size = sizeof(IRtekACFileSDK.ac_file_point);
                        }

                        irMarker.points = Marshal.AllocHGlobal(irMarker.point_count * size);
                        result = IRtekACFileSDK.ac_file_get_marker(hImage, ref irMarker);
                        if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                        {
                            IRtekACFileSDK.ac_file_point p = (IRtekACFileSDK.ac_file_point)Marshal.PtrToStructure(irMarker.points, typeof(IRtekACFileSDK.ac_file_point));
                            point = new Point(p.x, p.y);

                            float temp = irMarker.db_value;
                            MaxInternal = temp;
                            MeasuredValueInternal = irMarker.db_value;
                        }
                        else
                        {
                            throw new Exception(string.Format("invoke ac_file_get_marker error code {0}.", result));
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(irMarker.points);
                    }
                }
                else
                {
                    throw new Exception(string.Format("invoke ac_file_get_marker error code {0}.", result));
                }
            }
        }

        public class MarkerEllipse : Marker
        {
            private Rectangle rectangle;

            public MarkerEllipse(AcousticImageFile owner, string name, Rectangle rectangle)
                : base(owner, name)
            {
                this.rectangle = rectangle;
            }

            public override MarkerType Type
            {
                get { return MarkerType.Ellipse; }
            }

            public Rectangle Rectangle
            {
                get
                {
                    return rectangle;
                }
                set
                {
                    if (rectangle == value)
                    {
                        return;
                    }

                    Rectangle oldValue = rectangle;
                    rectangle = value;
                    if (Owner.SetMarker(this))
                    {
                        Owner.HandleMarkerChangedEvent(this);
                    }
                    else
                    {
                        rectangle = oldValue;
                    }
                }
            }

            public float Max
            {
                get { return MaxInternal; }
            }

            public Point HotSpot
            {
                get { return MaxSpotInternal; }
            }

            public float Area
            {
                get { return MeasuredValueInternal; }
            }

            public override void Sync()
            {
                IntPtr hImage = Owner.HImage;
                if (hImage == IntPtr.Zero)
                {
                    return;
                }

                IRtekACFileSDK.ac_file_marker irMarker = new IRtekACFileSDK.ac_file_marker();
                irMarker.name = StringToBytes(Name, IRtekACFileSDK.NAME_LEN, Encoding.UTF8);
                int result = IRtekACFileSDK.ac_file_get_marker(hImage, ref irMarker);
                if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                {
                    try
                    {
                        int size = 0;
                        unsafe
                        {
                            size = sizeof(IRtekACFileSDK.ac_file_point);
                        }

                        irMarker.points = Marshal.AllocHGlobal(irMarker.point_count * size);
                        result = IRtekACFileSDK.ac_file_get_marker(hImage, ref irMarker);
                        if (result == IRtekACFileSDK.AC_FILE_EC_OK)
                        {
                            Rectangle rect = new Rectangle();
                            IRtekACFileSDK.ac_file_point p = (IRtekACFileSDK.ac_file_point)Marshal.PtrToStructure(irMarker.points, typeof(IRtekACFileSDK.ac_file_point));
                            Point p1 = new Point(p.x, p.y);
                            p = (IRtekACFileSDK.ac_file_point)Marshal.PtrToStructure(irMarker.points + size, typeof(IRtekACFileSDK.ac_file_point));
                            Point p2 = new Point(p.x, p.y);
                            rect.Location = new Point(p1.X <= p2.X ? p1.X : p2.X, p1.Y <= p2.Y ? p1.Y : p2.Y);
                            rect.Size = new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
                            rectangle = rect;

                            MaxInternal = irMarker.db_value;
                        }
                        else
                        {
                            throw new Exception(string.Format("invoke ac_file_get_marker error code {0}.", result));
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(irMarker.points);
                    }
                }
                else
                {
                    throw new Exception(string.Format("invoke ac_file_get_marker error code {0}.", result));
                }
            }
        }

        public class MarkerCollection : IEnumerable
        {
            private readonly List<Marker> Entries = new List<Marker>();

            public MarkerCollection(AcousticImageFile owner)
            {
                Owner = owner;
            }

            public AcousticImageFile Owner { get; private set; }

            public int Count
            {
                get { return Entries.Count; }
            }

            public Marker this[int index]
            {
                get
                {
                    return Entries[index];
                }
            }

            public Marker this[string name]
            {
                get
                {
                    for (int i = 0; i < Entries.Count; i++)
                    {
                        if (string.Equals(Entries[i].Name, name))
                        {
                            return Entries[i];
                        }
                    }
                    throw new Exception("name not existed");
                }
            }

            public string NextSpotName
            {
                get
                {
                    return string.Format("{0}{1}", SPOT_PREFIX, GetNextIndex(SPOT_PREFIX));
                }
            }

            public string NextAreaName
            {
                get
                {
                    return string.Format("{0}{1}", AREA_PREFIX, GetNextIndex(AREA_PREFIX));
                }
            }

            public int IndexOf(string name)
            {
                for (int i = 0; i < Entries.Count; i++)
                {
                    if (string.Equals(Entries[i].Name, name))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public void Add(Marker marker)
            {
                if (!Owner._isMarkersSynced)
                {
                    Entries.Add(marker);
                }
                else if (Owner.SetMarker(marker))
                {
                    Entries.Add(marker);
                    Owner.HandleMarkerAddedEvent(marker);
                }
                else
                {
                    throw new Exception("add failed");
                }
            }

            public bool Remove(string name)
            {
                int index = -1;
                for (int i = 0; i < Entries.Count; i++)
                {
                    if (string.Equals(Entries[i].Name, name))
                    {
                        index = i;
                    }
                }

                if (index == -1)
                {
                    return false;
                }

                RemoveAt(index);

                return true;
            }

            public void RemoveAt(int index)
            {
                Marker marker = Entries[index];
                if (!Owner._isMarkersSynced)
                {
                    Entries.RemoveAt(index);
                }
                else if (Owner.RemoveMarker(marker.Name))
                {
                    Entries.RemoveAt(index);
                    Owner.HandleMarkerRemovedEvent(marker);
                }
                else
                {
                    throw new Exception("remove failed");
                }
            }

            internal void Clear()
            {
                for (int i = Entries.Count - 1; i >= 0; i--)
                {
                    Marker marker = Entries[i];
                    Remove(marker.Name);
                }
            }

            public IEnumerator GetEnumerator()
            {
                return Entries.GetEnumerator();
            }

            private int GetNextIndex(string prefix)
            {
                int index;
                SortedDictionary<int, int> idMap = new SortedDictionary<int, int>();
                for (int i = 0; i < Entries.Count; i++)
                {
                    string name = Entries[i].Name;
                    if (name.StartsWith(prefix))
                    {
                        if (int.TryParse(name.Substring(prefix.Length), out index))
                        {
                            idMap.Add(index, index);
                        }
                    }
                }

                index = 1;
                foreach (KeyValuePair<int, int> itr in idMap)
                {
                    if (itr.Key != index)
                    {
                        break;
                    }
                    index++;
                }
                return index;
            }
        }

        public delegate void MarkerEventHandler(object sender, MarkerEventArgs e);

        public class MarkerEventArgs : EventArgs
        {
            public MarkerEventArgs(Marker marker)
            {
                OldName = marker.Name;
                Marker = marker;
            }

            public MarkerEventArgs(string oldName, Marker marker)
            {
                OldName = oldName;
                Marker = marker;
            }

            public string OldName { get; private set; }
            public Marker Marker { get; private set; }
        }
    }
}
