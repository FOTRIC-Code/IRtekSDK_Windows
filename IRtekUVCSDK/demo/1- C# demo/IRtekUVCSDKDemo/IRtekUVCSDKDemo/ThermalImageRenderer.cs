using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using IRtekUVCSDK;

namespace IRtekUVCSDKDemo
{
    public class ThermalImageRenderer : IDisposable
    {
        private const float CelsiusToKelvin = 273.15f;
        private IntPtr _fileHandle;
        private string _tempFilePath;
        private byte[] _processBuffer;
        private int _width;
        private int _height;
        private bool _disposed;

        public int Width => _width;
        public int Height => _height;

        public ThermalImageRenderer(int width, int height)
        {
            _width = width;
            _height = height;
            InitializeFileHandle();
        }

        private void InitializeFileHandle()
        {
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"ThermalPreview_{Guid.NewGuid():N}.irs");
            IntPtr h = IntPtr.Zero;
            int ret = IRtekFileSDK.ir_file_create(_tempFilePath, IRtekFileSDK.IR_FILE_THERMAL_PICTURE, ref h);
            if (ret != IRtekFileSDK.IR_FILE_EC_OK)
                throw new InvalidOperationException($"Failed to create file handle: {ret}");

            ret = IRtekFileSDK.ir_file_set_isp_type(h, IRtekFileSDK.IR_FILE_ISP_HOLLY);
            if (ret != IRtekFileSDK.IR_FILE_EC_OK)
            {
                IRtekFileSDK.ir_file_close(h);
                throw new InvalidOperationException($"Failed to set ISP type: {ret}");
            }

            var size = new IRtekFileSDK.IrFileSize { width = (short)_width, height = (short)_height };
            IRtekFileSDK.ir_file_set_thermal_size(h, ref size);

            SetDefaultLut(h);
            SetDefaultGain(h);
            SetDefaultColorDist(h);

            _fileHandle = h;
        }

        private void SetDefaultLut(IntPtr fileHandle)
        {
            const int fakeCount = 20;
            const int adMax = 16383;
            const float tMin = -20f;
            const float tMax = 150f;

            ushort[] ad = new ushort[fakeCount];
            float[] t = new float[fakeCount];
            for (int i = 0; i < fakeCount; i++)
            {
                ad[i] = (ushort)(i * adMax / (fakeCount - 1));
                t[i] = tMin + (tMax - tMin) * i / (fakeCount - 1);
            }

            IntPtr adPtr = Marshal.AllocHGlobal(fakeCount * sizeof(ushort));
            IntPtr tempPtr = Marshal.AllocHGlobal(fakeCount * sizeof(float));
            try
            {
                for (int i = 0; i < fakeCount; i++)
                    Marshal.WriteInt16(adPtr, i * 2, (short)ad[i]);
                Marshal.Copy(t, 0, tempPtr, fakeCount);

                var lut = new IRtekFileSDK.IrFileLut
                {
                    radMethod = 0x01,
                    count = (uint)fakeCount,
                    adArray = adPtr,
                    tempArray = tempPtr
                };
                IRtekFileSDK.ir_file_set_lut(fileHandle, ref lut);
            }
            finally
            {
                Marshal.FreeHGlobal(adPtr);
                Marshal.FreeHGlobal(tempPtr);
            }
        }

        private void SetDefaultGain(IntPtr fileHandle)
        {
            var gain = new IRtekFileSDK.IrFileGain
            {
                mode = IRtekFileSDK.IR_FILE_GAIN_AUTO,
                min = -20f + CelsiusToKelvin,
                max = 150f + CelsiusToKelvin,
                fixedSpan = 0,
                span = 2.0f
            };
            IRtekFileSDK.ir_file_set_gain(fileHandle, ref gain);
        }

        private void SetDefaultColorDist(IntPtr fileHandle)
        {
            IRtekFileSDK.ir_file_set_color_dist(fileHandle, IRtekFileSDK.IR_FILE_AD_LINEAR);
        }

        public void SetThermalParams(UvcSdkNative.UvcThermalParams thermalParams)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;

            var fileThermalParam = new IRtekFileSDK.IrFileThermalParams
            {
                emissivity = thermalParams.emissivity,
                distance = thermalParams.distance,
                reflTemp = thermalParams.refl_temp,
                atmTemp = thermalParams.atm_temp,
                relHumidity = thermalParams.rel_humidity,
                extOptTemp = 293.15f,
                extOptTrans = 1.0f
            };
            IRtekFileSDK.ir_file_set_thermal_params(_fileHandle, ref fileThermalParam);
        }

        public void SetTempRange(float min, float max)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;

            var tr = new IRtekFileSDK.IrFileTempRange { max = max, min = min };
            IRtekFileSDK.ir_file_set_temp_range(_fileHandle, ref tr);
        }

        public void SetLut(ushort[] adValues, float[] tempValues)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;
            if (adValues == null || tempValues == null || adValues.Length != tempValues.Length) return;
            int count = adValues.Length;
            IntPtr adPtr = Marshal.AllocHGlobal(count * sizeof(ushort));
            IntPtr tempPtr = Marshal.AllocHGlobal(count * sizeof(float));
            try
            {
                for (int i = 0; i < count; i++)
                    Marshal.WriteInt16(adPtr, i * 2, (short)adValues[i]);
                Marshal.Copy(tempValues, 0, tempPtr, count);

                var lut = new IRtekFileSDK.IrFileLut
                {
                    radMethod = 0x01,
                    count = (uint)count,
                    adArray = adPtr,
                    tempArray = tempPtr
                };
                IRtekFileSDK.ir_file_set_lut(_fileHandle, ref lut);
            }
            finally
            {
                Marshal.FreeHGlobal(adPtr);
                Marshal.FreeHGlobal(tempPtr);
            }
        }

        public void SetGain(int mode, float minKelvin, float maxKelvin, byte fixedSpan, float span)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;

            var gain = new IRtekFileSDK.IrFileGain
            {
                mode = mode,
                min = minKelvin,
                max = maxKelvin,
                fixedSpan = fixedSpan,
                span = span
            };
            IRtekFileSDK.ir_file_set_gain(_fileHandle, ref gain);
        }

        public void SetColorDist(int colorDist)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;
            IRtekFileSDK.ir_file_set_color_dist(_fileHandle, colorDist);
        }

        public void SetPalette(byte refNo, byte inverted)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;
            var palette = IRtekFileSDK.IrFilePalette.Create();
            palette.refNo = refNo;
            palette.inverted = inverted;
            IRtekFileSDK.ir_file_set_palette(_fileHandle, ref palette);
        }

        public void SetAlarm(int alarmType, float maxKelvin, float minKelvin, byte custom, uint color)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;
            var alarm = new IRtekFileSDK.IrFileAlarm
            {
                type = alarmType,
                custom = custom,
                color = color,
                max = maxKelvin,
                min = minKelvin,
                rhLimit = 1.0f,
                indoorTemp = 293.15f,
                outdoorTemp = 283.15f,
                heatIndex = 0.8f
            };
            IRtekFileSDK.ir_file_set_alarm(_fileHandle, ref alarm);
        }

        public IntPtr FileHandle => _fileHandle;

        public bool SetMarker(string name, int type, Point[] points)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return false;
            if (points == null || points.Length == 0) return false;

            int pointSize = Marshal.SizeOf(typeof(IRtekFileSDK.IrFilePoint));
            IntPtr ptsPtr = Marshal.AllocHGlobal(pointSize * points.Length);
            try
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var fp = new IRtekFileSDK.IrFilePoint { x = (short)points[i].X, y = (short)points[i].Y };
                    Marshal.StructureToPtr(fp, IntPtr.Add(ptsPtr, i * pointSize), false);
                }

                var marker = new IRtekFileSDK.IrFileMarker
                {
                    type = type,
                    name = IRtekFileSDK.StringToMarkerName(name),
                    localParams = 0,
                    reflTemp = 293.15f,
                    distance = 1.0f,
                    emissivity = 1.0f,
                    pointCount = (uint)points.Length,
                    points = ptsPtr,
                    tempValues = new float[3],
                    tempSpots = new IRtekFileSDK.IrFilePoint[2],
                    exInfo = new IRtekFileSDK.IrFileBuffer()
                };

                int ret = IRtekFileSDK.ir_file_set_marker(_fileHandle, ref marker);
                return ret == IRtekFileSDK.IR_FILE_EC_OK;
            }
            finally
            {
                Marshal.FreeHGlobal(ptsPtr);
            }
        }

        public bool GetMarkerTemps(string name, out float max, out float min, out float avg)
        {
            max = min = avg = 0;
            if (_disposed || _fileHandle == IntPtr.Zero) return false;

            var marker = new IRtekFileSDK.IrFileMarker
            {
                name = IRtekFileSDK.StringToMarkerName(name),
                points = IntPtr.Zero,
                tempValues = new float[3],
                tempSpots = new IRtekFileSDK.IrFilePoint[2],
                exInfo = new IRtekFileSDK.IrFileBuffer()
            };

            int ret = IRtekFileSDK.ir_file_get_marker(_fileHandle, ref marker);
            if (ret != IRtekFileSDK.IR_FILE_EC_OK) return false;

            max = marker.tempValues[IRtekFileSDK.IR_FILE_TEMP_MAX];
            min = marker.tempValues[IRtekFileSDK.IR_FILE_TEMP_MIN];
            avg = marker.tempValues[IRtekFileSDK.IR_FILE_TEMP_AVG];
            return true;
        }

        public void RemoveMarker(string name)
        {
            if (_disposed || _fileHandle == IntPtr.Zero) return;
            byte[] nameBytes = IRtekFileSDK.StringToMarkerName(name);
            IRtekFileSDK.ir_file_remove_marker(_fileHandle, nameBytes);
        }

        public void ProcessFrame(byte[] rawData)
        {
            if (_disposed || _fileHandle == IntPtr.Zero)
                return;

            int pixelCount = _width * _height;
            int bytesNeeded = pixelCount * 2;
            if (rawData == null || rawData.Length < bytesNeeded)
                return;

            int imageSize = _width * 3 * _height;
            if (_processBuffer == null || _processBuffer.Length != imageSize)
                _processBuffer = new byte[imageSize];

            GCHandle pinHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            GCHandle imagePin = GCHandle.Alloc(_processBuffer, GCHandleType.Pinned);
            try
            {
                var frame = new IRtekFileSDK.IrFileFrame
                {
                    timeStamp = (ulong)(uint)Environment.TickCount,
                    length = (uint)pixelCount,
                    buffer = pinHandle.AddrOfPinnedObject()
                };
                var image = new IRtekFileSDK.IrFileImage
                {
                    offset = 0,
                    data = imagePin.AddrOfPinnedObject()
                };

                // Feeds the FileSDK so marker temperatures stay current; the BGR
                // output buffer is intentionally discarded (the UVC SDK renders the
                // display bitmap with super-resolution separately).
                IRtekFileSDK.ir_file_get_thermal_image_from_frame(_fileHandle, ref frame, IRtekFileSDK.IR_FILE_BGR, ref image);
            }
            finally
            {
                if (imagePin.IsAllocated)
                    imagePin.Free();
                if (pinHandle.IsAllocated)
                    pinHandle.Free();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_fileHandle != IntPtr.Zero)
                {
                    try { IRtekFileSDK.ir_file_close(_fileHandle); } catch { }
                    _fileHandle = IntPtr.Zero;
                }

                if (!string.IsNullOrEmpty(_tempFilePath))
                {
                    try { if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath); } catch { }
                    _tempFilePath = null;
                }

                _disposed = true;
            }
        }

        ~ThermalImageRenderer()
        {
            Dispose(false);
        }
    }
}
