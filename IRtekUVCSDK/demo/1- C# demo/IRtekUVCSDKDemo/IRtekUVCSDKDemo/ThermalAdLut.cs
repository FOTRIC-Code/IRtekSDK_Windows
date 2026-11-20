using System;
using System.Collections.Generic;

namespace IRtekUVCSDKDemo
{
    internal sealed class ThermalAdLut
    {
        private readonly ushort[] _ad;
        private readonly float[] _temp;

        private ThermalAdLut(ushort[] ad, float[] temp)
        {
            _ad = ad;
            _temp = temp;
        }

        public static bool TryLoad(IntPtr handle, out ThermalAdLut lut, out string error)
        {
            lock (DeviceReadCache.SyncRoot)
            {
                if (DeviceReadCache.TryGetLut(handle, out lut, out error))
                    return lut != null;

                lut = null;
                error = null;
                var table = IRtekUVCSDK.UvcSdkNative.UvcLutTable.Create();
                int ret = IRtekUVCSDK.UvcSdkNative.uvc_get_thermal_lut(handle, ref table);
                if (!IRtekUVCSDK.UvcSdkNative.IsOk(ret))
                {
                    error = "uvc_get_thermal_lut failed: " + ret;
                    DeviceReadCache.StoreLut(handle, ret, table, null, error);
                    return false;
                }

                int count = table.lut_count;
                if (count <= 0 || table.ad == null || table.t == null)
                {
                    error = "Thermal LUT is empty.";
                    DeviceReadCache.StoreLut(handle, ret, table, null, error);
                    return false;
                }
                count = Math.Min(count, Math.Min(table.ad.Length, table.t.Length));
                lut = FromSamples(table.ad, table.t, count);
                if (lut == null)
                {
                    error = "Thermal LUT is empty.";
                    DeviceReadCache.StoreLut(handle, ret, table, null, error);
                    return false;
                }
                DeviceReadCache.StoreLut(handle, ret, table, lut, null);
                return true;
            }
        }

        internal static ThermalAdLut FromSamples(ushort[] adValues, float[] tempValues)
        {
            if (adValues == null || tempValues == null)
                return null;
            return FromSamples(adValues, tempValues, Math.Min(adValues.Length, tempValues.Length));
        }

        private static ThermalAdLut FromSamples(ushort[] adValues, float[] tempValues, int count)
        {
            if (adValues == null || tempValues == null || count <= 0)
                return null;
            count = Math.Min(count, Math.Min(adValues.Length, tempValues.Length));

            var pairs = new List<KeyValuePair<ushort, float>>(count);
            for (int i = 0; i < count; i++)
            {
                if (float.IsNaN(tempValues[i]) || float.IsInfinity(tempValues[i]))
                    continue;
                pairs.Add(new KeyValuePair<ushort, float>(adValues[i], tempValues[i]));
            }
            if (pairs.Count == 0)
                return null;

            pairs.Sort((a, b) => a.Key.CompareTo(b.Key));
            var ad = new List<ushort>(pairs.Count);
            var temp = new List<float>(pairs.Count);
            for (int i = 0; i < pairs.Count; i++)
            {
                if (ad.Count > 0 && pairs[i].Key == ad[ad.Count - 1])
                    continue;
                ad.Add(pairs[i].Key);
                temp.Add(pairs[i].Value);
            }
            if (ad.Count == 0)
                return null;
            return new ThermalAdLut(ad.ToArray(), temp.ToArray());
        }

        public float ToTemp(float adValue)
        {
            int n = _ad.Length;
            if (n == 1)
                return _temp[0];
            if (adValue <= _ad[0])
                return Lerp(_ad[0], _ad[1], _temp[0], _temp[1], adValue);
            if (adValue >= _ad[n - 1])
                return Lerp(_ad[n - 2], _ad[n - 1], _temp[n - 2], _temp[n - 1], adValue);

            int lo = 0;
            int hi = n - 1;
            while (hi - lo > 1)
            {
                int mid = (lo + hi) >> 1;
                if (_ad[mid] <= adValue)
                    lo = mid;
                else
                    hi = mid;
            }
            return Lerp(_ad[lo], _ad[hi], _temp[lo], _temp[hi], adValue);
        }

        public float ToAd(float kelvin)
        {
            int n = _ad.Length;
            if (n == 1)
                return _ad[0];

            for (int i = 0; i < n - 1; i++)
            {
                float t0 = _temp[i];
                float t1 = _temp[i + 1];
                float low = t0 < t1 ? t0 : t1;
                float high = t0 < t1 ? t1 : t0;
                if (kelvin >= low && kelvin <= high)
                    return ClampAd(Lerp(t0, t1, _ad[i], _ad[i + 1], kelvin));
            }

            float start = _temp[0];
            float end = _temp[n - 1];
            if (Math.Abs(kelvin - start) <= Math.Abs(kelvin - end))
                return ClampAd(Lerp(_temp[0], _temp[1], _ad[0], _ad[1], kelvin));
            return ClampAd(Lerp(_temp[n - 2], _temp[n - 1], _ad[n - 2], _ad[n - 1], kelvin));
        }

        public float SpanToTemp(float adMin, float adSpan)
        {
            float width = ToTemp(adMin + adSpan) - ToTemp(adMin);
            return width < 0f ? -width : width;
        }

        public float SpanToAd(float tempMin, float tempSpan)
        {
            float width = ToAd(tempMin + tempSpan) - ToAd(tempMin);
            if (width < 0f)
                width = -width;
            if (width < 1f)
                width = 1f;
            if (width > 65535f)
                width = 65535f;
            return width;
        }

        private static float Lerp(float x0, float x1, float y0, float y1, float x)
        {
            float dx = x1 - x0;
            if (dx == 0f)
                return y0;
            return y0 + (y1 - y0) * ((x - x0) / dx);
        }

        private static float ClampAd(float ad)
        {
            if (ad < 0f)
                return 0f;
            if (ad > 65535f)
                return 65535f;
            return ad;
        }
    }
}
