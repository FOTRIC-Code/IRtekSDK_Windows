using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using IRtekNetSDK;

namespace IRtekNetSDKDemo
{
    internal sealed class ThermalLutSnapshot
    {
        public ushort RangeId { get; private set; }
        public ushort Length { get; private set; }
        public ushort[] Ad { get; private set; }
        public float[] T { get; private set; }

        public static ThermalLutSnapshot Capture(NetSdkNative.NetLutTable lut)
        {
            int len = lut.length;
            if (len < 0)
                len = 0;
            if (len > NetSdkNative.NET_MAX_LUT_POINTS)
                len = NetSdkNative.NET_MAX_LUT_POINTS;

            ushort[] ad = null;
            float[] t = null;
            if (len > 0 && lut.ad != null && lut.t != null)
            {
                ad = new ushort[len];
                t = new float[len];
                Array.Copy(lut.ad, ad, len);
                Array.Copy(lut.t, t, len);
            }

            return new ThermalLutSnapshot
            {
                RangeId = lut.range_id,
                Length = (ushort)len,
                Ad = ad,
                T = t
            };
        }

        public string BuildSummary()
        {
            return "range_id=" + RangeId + " length=" + Length;
        }

        public IEnumerable<string> BuildLogLines()
        {
            yield return "thermal_lut:";
            yield return "  range_id=" + RangeId;
            yield return "  length=" + Length;
            yield return "  ad=" + FormatArraySummary(Ad);
            yield return "  t=" + FormatArraySummary(T);
        }

        public void WriteJson(string path)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine("{");
                writer.WriteLine("  \"range_id\": " + RangeId + ",");
                writer.WriteLine("  \"length\": " + Length + ",");
                writer.Write("  \"ad\": ");
                WriteUshortArrayValue(writer, Ad);
                writer.WriteLine(",");
                writer.Write("  \"t\": ");
                WriteFloatArrayValue(writer, T);
                writer.WriteLine();
                writer.WriteLine("}");
            }
        }

        private static string FormatArraySummary<T>(T[] values)
        {
            if (values == null || values.Length == 0)
                return "null";

            return "length=" + values.Length + " preview=[" + FormatPreview(values, 4) + "]";
        }

        private static string FormatPreview<T>(T[] values, int maxItems)
        {
            if (values == null || values.Length == 0)
                return string.Empty;

            int count = Math.Min(maxItems, values.Length);
            var parts = new string[count];
            for (int i = 0; i < count; i++)
                parts[i] = Convert.ToString(values[i], CultureInfo.InvariantCulture);
            return string.Join(", ", parts);
        }

        private static void WriteUshortArrayValue(TextWriter writer, ushort[] values)
        {
            if (values == null || values.Length == 0)
            {
                writer.Write("null");
                return;
            }

            writer.Write("[");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    writer.Write(", ");
                writer.Write(values[i]);
            }
            writer.Write("]");
        }

        private static void WriteFloatArrayValue(TextWriter writer, float[] values)
        {
            if (values == null || values.Length == 0)
            {
                writer.Write("null");
                return;
            }

            writer.Write("[");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    writer.Write(", ");
                writer.Write(values[i].ToString("G9", CultureInfo.InvariantCulture));
            }
            writer.Write("]");
        }
    }
}
