using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using IRtekHLAPI;

namespace IRtekHLAPIDemo
{
    internal sealed class RawFrameSnapshot
    {
        public ulong TimestampUs { get; private set; }
        public ushort Width { get; private set; }
        public ushort Height { get; private set; }
        public ushort RangeId { get; private set; }
        public uint PointsCount { get; private set; }
        public uint LutLen { get; private set; }
        public uint LutEnvTemp { get; private set; }
        public uint SensorTemp { get; private set; }
        public uint ShutterState { get; private set; }
        public uint FrameNo { get; private set; }
        public ulong Pts { get; private set; }
        public uint RawType { get; private set; }
        public uint ShutterAdv { get; private set; }
        public ushort[] Data { get; private set; }
        public float[] LutTempValue { get; private set; }
        public ushort[] LutAdValue { get; private set; }
        public string ResponseDataTypes { get; private set; }
        public string CaptureSource { get; private set; }

        public static bool TryCapture(NetSdkNative.NetThermalData thermalData, string captureSource, out RawFrameSnapshot snapshot, out string error)
        {
            snapshot = null;
            var raw = thermalData.raw_frame;
            int pointCount = (int)raw.points_count;
            if (pointCount <= 0 && raw.width > 0 && raw.height > 0)
                pointCount = raw.width * raw.height;

            bool hasRawFlag = (thermalData.data_types & NetSdkNative.NetThermalDataType.Raw) != 0;
            bool hasRawPayload = raw.data != IntPtr.Zero && pointCount > 0;

            if (!hasRawPayload)
            {
                error = hasRawFlag
                    ? "Response includes Raw flag but raw_frame payload is empty. data_types="
                        + FormatDataTypes(thermalData.data_types)
                        + ", width=" + raw.width
                        + ", height=" + raw.height
                        + ", points_count=" + raw.points_count
                        + ", data_ptr=" + raw.data.ToInt64()
                    : "Response does not include Raw data. data_types="
                        + FormatDataTypes(thermalData.data_types)
                        + ", width=" + raw.width
                        + ", height=" + raw.height
                        + ", points_count=" + raw.points_count
                        + ", data=" + raw.data.ToInt64();
                return false;
            }

            ushort[] data = null;
            if (raw.data != IntPtr.Zero && pointCount > 0)
            {
                var shorts = new short[pointCount];
                Marshal.Copy(raw.data, shorts, 0, pointCount);
                data = new ushort[pointCount];
                for (int i = 0; i < pointCount; i++)
                    data[i] = (ushort)shorts[i];
            }
            else if (pointCount > 0)
            {
                error = "raw_frame.data is null but points_count=" + pointCount
                    + ". data_types=" + FormatDataTypes(thermalData.data_types);
                return false;
            }

            float[] lutTemp = null;
            int lutLen = (int)raw.lut_len;
            if (lutLen > 0 && raw.lut_temp_value != IntPtr.Zero)
            {
                lutTemp = new float[lutLen];
                Marshal.Copy(raw.lut_temp_value, lutTemp, 0, lutLen);
            }

            ushort[] lutAd = null;
            if (lutLen > 0 && raw.lut_ad_value != IntPtr.Zero)
            {
                var shorts = new short[lutLen];
                Marshal.Copy(raw.lut_ad_value, shorts, 0, lutLen);
                lutAd = new ushort[lutLen];
                for (int i = 0; i < lutLen; i++)
                    lutAd[i] = (ushort)shorts[i];
            }

            snapshot = new RawFrameSnapshot
            {
                TimestampUs = thermalData.timestamp_us,
                ResponseDataTypes = FormatDataTypes(thermalData.data_types),
                CaptureSource = captureSource ?? "unknown",
                Width = raw.width,
                Height = raw.height,
                RangeId = raw.range_id,
                PointsCount = raw.points_count,
                LutLen = raw.lut_len,
                LutEnvTemp = raw.lut_env_temp,
                SensorTemp = raw.sensor_temp,
                ShutterState = raw.shutter_state,
                FrameNo = raw.frame_no,
                Pts = raw.pts,
                RawType = raw.raw_type,
                ShutterAdv = raw.shutter_adv,
                Data = data,
                LutTempValue = lutTemp,
                LutAdValue = lutAd
            };

            error = null;
            return true;
        }

        public IEnumerable<string> BuildLogLines()
        {
            yield return "raw_frame:";
            yield return "  width=" + Width;
            yield return "  height=" + Height;
            yield return "  range_id=" + RangeId;
            yield return "  points_count=" + PointsCount;
            yield return "  lut_len=" + LutLen;
            yield return "  lut_env_temp=" + LutEnvTemp;
            yield return "  sensor_temp=" + SensorTemp;
            yield return "  shutter_state=" + ShutterState;
            yield return "  frame_no=" + FrameNo;
            yield return "  pts=" + Pts;
            yield return "  raw_type=" + RawType;
            yield return "  shutter_adv=" + ShutterAdv;
            yield return "  timestamp_us=" + TimestampUs;
            yield return "  capture_source=" + CaptureSource;
            yield return "  response_data_types=" + ResponseDataTypes;
            yield return "  data=" + FormatArraySummary(Data);
            yield return "  lut_temp_value=" + FormatArraySummary(LutTempValue);
            yield return "  lut_ad_value=" + FormatArraySummary(LutAdValue);
        }

        public void WriteJson(string path)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine("{");
                writer.WriteLine("  \"timestamp_us\": " + TimestampUs.ToString(CultureInfo.InvariantCulture) + ",");
                writer.WriteLine("  \"capture_source\": \"" + EscapeJson(CaptureSource) + "\",");
                writer.WriteLine("  \"response_data_types\": \"" + EscapeJson(ResponseDataTypes) + "\",");
                writer.WriteLine("  \"raw_frame\": {");
                writer.WriteLine("    \"width\": " + Width + ",");
                writer.WriteLine("    \"height\": " + Height + ",");
                writer.WriteLine("    \"range_id\": " + RangeId + ",");
                writer.WriteLine("    \"points_count\": " + PointsCount + ",");
                writer.WriteLine("    \"lut_len\": " + LutLen + ",");
                writer.WriteLine("    \"lut_env_temp\": " + LutEnvTemp + ",");
                writer.WriteLine("    \"sensor_temp\": " + SensorTemp + ",");
                writer.WriteLine("    \"shutter_state\": " + ShutterState + ",");
                writer.WriteLine("    \"frame_no\": " + FrameNo + ",");
                writer.WriteLine("    \"pts\": " + Pts.ToString(CultureInfo.InvariantCulture) + ",");
                writer.WriteLine("    \"raw_type\": " + RawType + ",");
                writer.WriteLine("    \"shutter_adv\": " + ShutterAdv + ",");
                writer.Write("    \"data\": ");
                WriteUshortArrayValue(writer, Data);
                writer.WriteLine(",");
                writer.Write("    \"lut_temp_value\": ");
                WriteFloatArrayValue(writer, LutTempValue);
                writer.WriteLine(",");
                writer.Write("    \"lut_ad_value\": ");
                WriteUshortArrayValue(writer, LutAdValue);
                writer.WriteLine();
                writer.WriteLine("  }");
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

        private static string FormatDataTypes(NetSdkNative.NetThermalDataType types)
        {
            if (types == 0)
                return "None";

            var names = new List<string>();
            if ((types & NetSdkNative.NetThermalDataType.Raw) != 0) names.Add("Raw");
            if ((types & NetSdkNative.NetThermalDataType.Global) != 0) names.Add("Global");
            if ((types & NetSdkNative.NetThermalDataType.Marker) != 0) names.Add("Marker");
            return names.Count > 0 ? string.Join("|", names.ToArray()) : types.ToString();
        }

        private static string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
