using System;
using IRtekUVCSDK;

namespace IRtekUVCSDKDemo
{
    /// <summary>
    /// Remembers device reads for the login that is currently binding controls.
    /// The read runs off the UI thread; later reads in that same bind reuse the result.
    /// </summary>
    internal static class DeviceReadCache
    {
        private static readonly object Gate = new object();
        internal static object SyncRoot { get { return Gate; } }
        private static IntPtr _handle;
        private static volatile bool _cancel;

        private static readonly Slot<UvcSdkNative.UvcDeviceInfo> DeviceInfo = new Slot<UvcSdkNative.UvcDeviceInfo>();
        private static readonly Slot<UvcSdkNative.UvcLensInfo> Lens = new Slot<UvcSdkNative.UvcLensInfo>();
        private static readonly Slot<UvcSdkNative.UvcSize> ThermalSize = new Slot<UvcSdkNative.UvcSize>();
        private static readonly Slot<UvcSdkNative.UvcSrMode> SrMode = new Slot<UvcSdkNative.UvcSrMode>();
        private static readonly Slot<UvcSdkNative.UvcThermalParams> ThermalParams = new Slot<UvcSdkNative.UvcThermalParams>();
        private static readonly Slot<UvcSdkNative.UvcShutterMode> ShutterMode = new Slot<UvcSdkNative.UvcShutterMode>();
        private static readonly Slot<ushort> ShutterMinutes = new Slot<ushort>();
        private static readonly Slot<ushort> RangeId = new Slot<ushort>();
        private static readonly Slot<UvcSdkNative.UvcImageFlipMode> FlipMode = new Slot<UvcSdkNative.UvcImageFlipMode>();
        private static readonly Slot<UvcSdkNative.UvcPalette> ThermalPalette = new Slot<UvcSdkNative.UvcPalette>();
        private static readonly Slot<UvcSdkNative.UvcThermalGain> Gain = new Slot<UvcSdkNative.UvcThermalGain>();
        private static readonly Slot<UvcSdkNative.UvcColorDistType> ColorDist = new Slot<UvcSdkNative.UvcColorDistType>();
        private static readonly Slot<UvcSdkNative.UvcAlarm> Alarm = new Slot<UvcSdkNative.UvcAlarm>();
        private static readonly Slot<UvcSdkNative.UvcImageParams> ImageParams = new Slot<UvcSdkNative.UvcImageParams>();
        private static readonly Slot<UvcSdkNative.UvcAcousticSensorInfo> AcousticSensor = new Slot<UvcSdkNative.UvcAcousticSensorInfo>();
        private static readonly Slot<UvcSdkNative.UvcSize> AcousticSize = new Slot<UvcSdkNative.UvcSize>();
        private static readonly Slot<UvcSdkNative.UvcPalette> AcousticPalette = new Slot<UvcSdkNative.UvcPalette>();
        private static readonly Slot<UvcSdkNative.UvcAcousticFreqRange> AcousticFreq = new Slot<UvcSdkNative.UvcAcousticFreqRange>();
        private static readonly Slot<UvcSdkNative.UvcAcousticDbScale> AcousticScale = new Slot<UvcSdkNative.UvcAcousticDbScale>();

        private static bool _framesReady;
        private static int _frameRet;
        private static ushort _frameCount;
        private static UvcSdkNative.UvcFrameInfo[] _frames;

        private static bool _presetsReady;
        private static bool _presetsOk;
        private static bool _presetsAcoustic;
        private static UvcSdkNative.NamedPreset[] _presets;

        private static bool _lutReady;
        private static int _lutRet;
        private static UvcSdkNative.UvcLutTable _lutTable;
        private static ThermalAdLut _lut;
        private static string _lutError;

        internal static void Begin(IntPtr handle)
        {
            lock (Gate)
            {
                Clear();
                _cancel = false;
                _handle = handle;
            }
        }

        internal static void Cancel()
        {
            _cancel = true;
        }

        internal static void End()
        {
            lock (Gate)
            {
                Clear();
                _handle = IntPtr.Zero;
            }
        }

        internal static void PrimeThermal(IntPtr handle)
        {
            if (Stopped(handle)) return;
            var device = UvcSdkNative.UvcDeviceInfo.Create();
            GetDeviceInfo(handle, ref device);
            if (Stopped(handle)) return;
            var lens = UvcSdkNative.UvcLensInfo.Create();
            GetLensInfo(handle, ref lens);
            if (Stopped(handle)) return;
            var size = new UvcSdkNative.UvcSize();
            GetThermalResolution(handle, ref size);
            if (Stopped(handle)) return;
            UvcSdkNative.UvcSrMode sr;
            GetThermalSr(handle, out sr);
            if (Stopped(handle)) return;
            var thermal = new UvcSdkNative.UvcThermalParams();
            GetThermalParams(handle, ref thermal);
            if (Stopped(handle)) return;
            UvcSdkNative.UvcShutterMode shutter;
            GetShutterMode(handle, out shutter);
            if (Stopped(handle)) return;
            ushort minutes;
            GetShutterTime(handle, out minutes);
            if (Stopped(handle)) return;
            var frames = new UvcSdkNative.UvcFrameInfo[32];
            for (int i = 0; i < frames.Length; i++)
                frames[i] = UvcSdkNative.UvcFrameInfo.Create();
            ushort frameCount;
            GetFrameList(handle, frames, (ushort)frames.Length, out frameCount);
            if (Stopped(handle)) return;
            ushort rangeId;
            GetThermalRange(handle, out rangeId);
            if (Stopped(handle)) return;
            UvcSdkNative.UvcImageFlipMode flip;
            GetImageFlip(handle, out flip);
            if (Stopped(handle)) return;
            UvcSdkNative.NamedPreset[] presets;
            TryGetPresetPalettes(handle, false, out presets);
            if (Stopped(handle)) return;
            var palette = UvcSdkNative.UvcPalette.Create();
            GetThermalPalette(handle, ref palette);
            if (Stopped(handle)) return;
            ThermalAdLut lut;
            string lutError;
            ThermalAdLut.TryLoad(handle, out lut, out lutError);
            if (Stopped(handle)) return;
            UvcSdkNative.UvcColorDistType colorDist;
            GetColorDist(handle, out colorDist);
            if (Stopped(handle)) return;
            var alarm = new UvcSdkNative.UvcAlarm();
            GetAlarm(handle, ref alarm);
            if (Stopped(handle)) return;
            var gain = new UvcSdkNative.UvcThermalGain();
            GetGain(handle, ref gain);
            if (Stopped(handle)) return;
            var image = new UvcSdkNative.UvcImageParams();
            GetImageParams(handle, ref image);
        }

        internal static void PrimeAcoustic(IntPtr handle)
        {
            if (Stopped(handle)) return;
            var device = UvcSdkNative.UvcDeviceInfo.Create();
            GetDeviceInfo(handle, ref device);
            if (Stopped(handle)) return;
            var sensor = UvcSdkNative.UvcAcousticSensorInfo.Create();
            GetAcousticSensor(handle, ref sensor);
            if (Stopped(handle)) return;
            var size = new UvcSdkNative.UvcSize();
            GetAcousticResolution(handle, ref size);
            if (Stopped(handle)) return;
            UvcSdkNative.NamedPreset[] presets;
            TryGetPresetPalettes(handle, true, out presets);
            if (Stopped(handle)) return;
            var palette = UvcSdkNative.UvcPalette.Create();
            GetAcousticPalette(handle, ref palette);
            if (Stopped(handle)) return;
            var freq = new UvcSdkNative.UvcAcousticFreqRange();
            GetAcousticFreq(handle, ref freq);
            if (Stopped(handle)) return;
            var scale = new UvcSdkNative.UvcAcousticDbScale();
            GetAcousticScale(handle, ref scale);
            if (Stopped(handle)) return;
            var image = new UvcSdkNative.UvcImageParams();
            GetImageParams(handle, ref image);
        }

        internal static int GetDeviceInfo(IntPtr handle, ref UvcSdkNative.UvcDeviceInfo info)
        {
            return Read(handle, DeviceInfo, ref info, UvcSdkNative.uvc_get_device_info);
        }

        internal static int GetLensInfo(IntPtr handle, ref UvcSdkNative.UvcLensInfo info)
        {
            return Read(handle, Lens, ref info, UvcSdkNative.uvc_get_lens_info);
        }

        internal static int GetThermalResolution(IntPtr handle, ref UvcSdkNative.UvcSize size)
        {
            return Read(handle, ThermalSize, ref size, UvcSdkNative.uvc_get_thermal_resolution);
        }

        internal static int GetThermalSr(IntPtr handle, out UvcSdkNative.UvcSrMode mode)
        {
            return ReadOut(handle, SrMode, out mode, UvcSdkNative.uvc_get_thermal_sr);
        }

        internal static int GetThermalParams(IntPtr handle, ref UvcSdkNative.UvcThermalParams param)
        {
            return Read(handle, ThermalParams, ref param, UvcSdkNative.uvc_get_thermal_params);
        }

        internal static int GetShutterMode(IntPtr handle, out UvcSdkNative.UvcShutterMode mode)
        {
            return ReadOut(handle, ShutterMode, out mode, UvcSdkNative.uvc_get_thermal_shutter_mode);
        }

        internal static int GetShutterTime(IntPtr handle, out ushort minutes)
        {
            return ReadOut(handle, ShutterMinutes, out minutes, UvcSdkNative.uvc_get_thermal_shutter_time);
        }

        internal static int GetThermalRange(IntPtr handle, out ushort rangeId)
        {
            return ReadOut(handle, RangeId, out rangeId, UvcSdkNative.uvc_get_thermal_range);
        }

        internal static int GetImageFlip(IntPtr handle, out UvcSdkNative.UvcImageFlipMode mode)
        {
            return ReadOut(handle, FlipMode, out mode, UvcSdkNative.uvc_get_image_flip);
        }

        internal static int GetThermalPalette(IntPtr handle, ref UvcSdkNative.UvcPalette palette)
        {
            return Read(handle, ThermalPalette, ref palette, UvcSdkNative.uvc_get_thermal_palette);
        }

        internal static int GetGain(IntPtr handle, ref UvcSdkNative.UvcThermalGain gain)
        {
            return Read(handle, Gain, ref gain, UvcSdkNative.uvc_get_thermal_gain);
        }

        internal static int GetColorDist(IntPtr handle, out UvcSdkNative.UvcColorDistType type)
        {
            return ReadOut(handle, ColorDist, out type, UvcSdkNative.uvc_get_thermal_color_dist);
        }

        internal static int GetAlarm(IntPtr handle, ref UvcSdkNative.UvcAlarm alarm)
        {
            return Read(handle, Alarm, ref alarm, UvcSdkNative.uvc_get_thermal_alarm);
        }

        internal static int GetImageParams(IntPtr handle, ref UvcSdkNative.UvcImageParams param)
        {
            return Read(handle, ImageParams, ref param, UvcSdkNative.uvc_get_image_params);
        }

        internal static void RememberImageParams(IntPtr handle, UvcSdkNative.UvcImageParams param)
        {
            lock (Gate)
            {
                if (handle != _handle || handle == IntPtr.Zero)
                    return;
                ImageParams.Value = param;
                ImageParams.Ret = (int)UvcSdkNative.UvcErrorCode.OK;
                ImageParams.Ready = true;
            }
        }

        internal static int GetAcousticSensor(IntPtr handle, ref UvcSdkNative.UvcAcousticSensorInfo info)
        {
            return Read(handle, AcousticSensor, ref info, UvcSdkNative.uvc_get_acoustic_sensor_info);
        }

        internal static int GetAcousticResolution(IntPtr handle, ref UvcSdkNative.UvcSize size)
        {
            return Read(handle, AcousticSize, ref size, UvcSdkNative.uvc_get_acoustic_resolution);
        }

        internal static int GetAcousticPalette(IntPtr handle, ref UvcSdkNative.UvcPalette palette)
        {
            return Read(handle, AcousticPalette, ref palette, UvcSdkNative.uvc_get_acoustic_palette);
        }

        internal static int GetAcousticFreq(IntPtr handle, ref UvcSdkNative.UvcAcousticFreqRange range)
        {
            return Read(handle, AcousticFreq, ref range, UvcSdkNative.uvc_get_acoustic_freq_range);
        }

        internal static int GetAcousticScale(IntPtr handle, ref UvcSdkNative.UvcAcousticDbScale scale)
        {
            return Read(handle, AcousticScale, ref scale, UvcSdkNative.uvc_get_acoustic_db_scale);
        }

        internal static int GetFrameList(
            IntPtr handle, UvcSdkNative.UvcFrameInfo[] frames, ushort capacity, out ushort outLen)
        {
            lock (Gate)
            {
                if (handle == _handle && handle != IntPtr.Zero && _framesReady)
                {
                    outLen = _frameCount;
                    int cached = _frames == null ? 0 : _frames.Length;
                    int copy = Math.Min(outLen, Math.Min(frames.Length, cached));
                    if (copy > 0)
                        Array.Copy(_frames, frames, copy);
                    return _frameRet;
                }

                int ret = UvcSdkNative.uvc_get_frame_list(handle, frames, capacity, out outLen);
                if (handle == _handle && handle != IntPtr.Zero)
                {
                    _framesReady = true;
                    _frameRet = ret;
                    _frameCount = outLen;
                    _frames = frames;
                }
                return ret;
            }
        }

        internal static bool TryGetPresetPalettes(
            IntPtr handle, bool acoustic, out UvcSdkNative.NamedPreset[] presets)
        {
            lock (Gate)
            {
                if (handle == _handle && handle != IntPtr.Zero && _presetsReady && _presetsAcoustic == acoustic)
                {
                    presets = _presets;
                    return _presetsOk;
                }

                bool ok = UvcSdkNative.TryGetPresetPalettes(handle, acoustic, out presets);
                if (handle == _handle && handle != IntPtr.Zero)
                {
                    _presetsReady = true;
                    _presetsOk = ok;
                    _presetsAcoustic = acoustic;
                    _presets = presets;
                }
                return ok;
            }
        }

        internal static bool TryGetLut(IntPtr handle, out ThermalAdLut lut, out string error)
        {
            lock (Gate)
            {
                if (handle == _handle && handle != IntPtr.Zero && _lutReady)
                {
                    lut = _lut;
                    error = _lutError;
                    return true;
                }
            }

            lut = null;
            error = null;
            return false;
        }

        internal static void StoreLut(
            IntPtr handle, int ret, UvcSdkNative.UvcLutTable table, ThermalAdLut lut, string error)
        {
            lock (Gate)
            {
                if (handle != _handle || handle == IntPtr.Zero)
                    return;
                _lutReady = true;
                _lutRet = ret;
                _lutTable = table;
                _lut = lut;
                _lutError = error;
            }
        }

        internal static bool TryGetLutTable(IntPtr handle, out int ret, out UvcSdkNative.UvcLutTable table)
        {
            lock (Gate)
            {
                if (handle == _handle && handle != IntPtr.Zero && _lutReady)
                {
                    ret = _lutRet;
                    table = _lutTable;
                    return true;
                }
            }

            ret = 0;
            table = default(UvcSdkNative.UvcLutTable);
            return false;
        }

        private static bool Stopped(IntPtr handle)
        {
            if (_cancel)
                return true;
            lock (Gate)
                return _handle != handle;
        }

        private static void Clear()
        {
            DeviceInfo.Clear();
            Lens.Clear();
            ThermalSize.Clear();
            SrMode.Clear();
            ThermalParams.Clear();
            ShutterMode.Clear();
            ShutterMinutes.Clear();
            RangeId.Clear();
            FlipMode.Clear();
            ThermalPalette.Clear();
            Gain.Clear();
            ColorDist.Clear();
            Alarm.Clear();
            ImageParams.Clear();
            AcousticSensor.Clear();
            AcousticSize.Clear();
            AcousticPalette.Clear();
            AcousticFreq.Clear();
            AcousticScale.Clear();
            _framesReady = false;
            _frameRet = 0;
            _frameCount = 0;
            _frames = null;
            _presetsReady = false;
            _presetsOk = false;
            _presets = null;
            _lutReady = false;
            _lutRet = 0;
            _lutTable = default(UvcSdkNative.UvcLutTable);
            _lut = null;
            _lutError = null;
        }

        private delegate int RefReader<T>(IntPtr handle, ref T value);
        private delegate int OutReader<T>(IntPtr handle, out T value);

        private static int Read<T>(IntPtr handle, Slot<T> slot, ref T value, RefReader<T> reader)
        {
            lock (Gate)
            {
                if (handle == _handle && handle != IntPtr.Zero && slot.Ready)
                {
                    value = slot.Value;
                    return slot.Ret;
                }

                int ret = reader(handle, ref value);
                if (handle == _handle && handle != IntPtr.Zero)
                {
                    slot.Value = value;
                    slot.Ret = ret;
                    slot.Ready = true;
                }
                return ret;
            }
        }

        private static int ReadOut<T>(IntPtr handle, Slot<T> slot, out T value, OutReader<T> reader)
        {
            lock (Gate)
            {
                if (handle == _handle && handle != IntPtr.Zero && slot.Ready)
                {
                    value = slot.Value;
                    return slot.Ret;
                }

                int ret = reader(handle, out value);
                if (handle == _handle && handle != IntPtr.Zero)
                {
                    slot.Value = value;
                    slot.Ret = ret;
                    slot.Ready = true;
                }
                return ret;
            }
        }

        private sealed class Slot<T>
        {
            public bool Ready;
            public int Ret;
            public T Value;

            public void Clear()
            {
                Ready = false;
                Ret = 0;
                Value = default(T);
            }
        }
    }
}
