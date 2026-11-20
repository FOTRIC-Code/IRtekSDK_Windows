using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using IRtekHLAPI;

namespace IRtekHLAPIDemo
{
    internal sealed class ThermalDataSnapshot
    {
        public DateTime CapturedAt { get; private set; }
        public string RequestedDataTypes { get; private set; }
        public NetSdkNative.NetThermalDataType DataTypes { get; private set; }
        public ulong TimestampUs { get; private set; }
        public NetSdkNative.NetTempPointInfo GlobalTempInfo { get; private set; }
        public NetSdkNative.NetMarkerTempInfo[] MarkerTemps { get; private set; }
        public ushort MarkersCount { get; private set; }
        public NetSdkNative.NetRawFrame RawFrame { get; private set; }
        public ushort[] RawData { get; private set; }
        public float[] LutTempValue { get; private set; }
        public ushort[] LutAdValue { get; private set; }

        public static ThermalDataSnapshot Capture(
            NetSdkNative.NetThermalData thermalData,
            NetSdkNative.NetThermalDataType requestedTypes)
        {
            RawFrameSnapshot rawSnapshot;
            bool hasRawPayload = RawFrameSnapshot.TryCapture(thermalData, "net_get_thermal_data", out rawSnapshot, out _);

            var snapshot = new ThermalDataSnapshot
            {
                CapturedAt = DateTime.Now,
                RequestedDataTypes = FormatDataTypes(requestedTypes),
                DataTypes = thermalData.data_types,
                TimestampUs = thermalData.timestamp_us,
                GlobalTempInfo = thermalData.global_temp_info,
                MarkersCount = thermalData.markers_count,
                RawFrame = thermalData.raw_frame
            };

            if (thermalData.marker_temps != null)
            {
                int count = Math.Min((int)thermalData.markers_count, NetSdkNative.NET_MAX_MARKERS);
                if (count > 0)
                {
                    snapshot.MarkerTemps = new NetSdkNative.NetMarkerTempInfo[count];
                    Array.Copy(thermalData.marker_temps, snapshot.MarkerTemps, count);
                }
            }

            if (hasRawPayload)
            {
                snapshot.RawData = rawSnapshot.Data;
                snapshot.LutTempValue = rawSnapshot.LutTempValue;
                snapshot.LutAdValue = rawSnapshot.LutAdValue;
            }

            return snapshot;
        }

        public void WriteJson(string path)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine("{");
                writer.WriteLine("  \"captured_at\": \"" + CapturedAt.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture) + "\",");
                writer.WriteLine("  \"requested_data_types\": \"" + EscapeJson(RequestedDataTypes) + "\",");
                writer.WriteLine("  \"data_types\": " + ((uint)DataTypes).ToString(CultureInfo.InvariantCulture) + ",");
                writer.WriteLine("  \"timestamp_us\": " + TimestampUs.ToString(CultureInfo.InvariantCulture) + ",");
                WriteRawFrame(writer);
                writer.WriteLine(",");
                WriteGlobalTempInfo(writer);
                writer.WriteLine(",");
                WriteMarkerTemps(writer);
                writer.WriteLine(",");
                writer.Write("  \"markers_count\": " + MarkersCount);
                writer.WriteLine();
                writer.WriteLine("}");
            }
        }

        private void WriteGlobalTempInfo(TextWriter writer)
        {
            writer.WriteLine("  \"global_temp_info\": {");
            WriteTempPoint(writer, "max", GlobalTempInfo.max, 4);
            writer.WriteLine(",");
            WriteTempPoint(writer, "min", GlobalTempInfo.min, 4);
            writer.WriteLine(",");
            writer.Write("    \"avg_temp\": ");
            writer.Write(GlobalTempInfo.avg_temp.ToString("G9", CultureInfo.InvariantCulture));
            writer.WriteLine();
            writer.Write("  }");
        }

        private void WriteMarkerTemps(TextWriter writer)
        {
            writer.Write("  \"marker_temps\": ");
            if (MarkerTemps == null || MarkerTemps.Length == 0)
            {
                writer.WriteLine("[]");
                return;
            }

            writer.WriteLine("[");
            for (int i = 0; i < MarkerTemps.Length; i++)
            {
                var marker = MarkerTemps[i];
                writer.WriteLine("    {");
                writer.WriteLine("      \"marker_id\": " + marker.marker_id + ",");
                writer.WriteLine("      \"type\": " + ((int)marker.type).ToString(CultureInfo.InvariantCulture) + ",");
                writer.WriteLine("      \"marker_name\": \"" + EscapeJson(NetSdkNative.BytesToString(marker.marker_name)) + "\",");
                writer.WriteLine("      \"temp_point_info\": {");
                WriteTempPoint(writer, "max", marker.temp_point_info.max, 8);
                writer.WriteLine(",");
                WriteTempPoint(writer, "min", marker.temp_point_info.min, 8);
                writer.WriteLine(",");
                writer.Write("        \"avg_temp\": ");
                writer.Write(marker.temp_point_info.avg_temp.ToString("G9", CultureInfo.InvariantCulture));
                writer.WriteLine();
                writer.Write("      }");
                writer.WriteLine();
                writer.Write("    }");
                if (i < MarkerTemps.Length - 1)
                    writer.WriteLine(",");
                else
                    writer.WriteLine();
            }
            writer.Write("  ]");
        }

        private void WriteRawFrame(TextWriter writer)
        {
            var raw = RawFrame;
            writer.WriteLine("  \"raw_frame\": {");
            writer.WriteLine("    \"width\": " + raw.width + ",");
            writer.WriteLine("    \"height\": " + raw.height + ",");
            writer.WriteLine("    \"range_id\": " + raw.range_id + ",");
            writer.Write("    \"data\": ");
            WriteUshortArrayValue(writer, RawData);
            writer.WriteLine(",");
            writer.WriteLine("    \"points_count\": " + raw.points_count + ",");
            writer.WriteLine("    \"lut_len\": " + raw.lut_len + ",");
            writer.WriteLine("    \"lut_env_temp\": " + raw.lut_env_temp + ",");
            writer.WriteLine("    \"sensor_temp\": " + raw.sensor_temp + ",");
            writer.WriteLine("    \"shutter_state\": " + raw.shutter_state + ",");
            writer.WriteLine("    \"frame_no\": " + raw.frame_no + ",");
            writer.WriteLine("    \"pts\": " + raw.pts.ToString(CultureInfo.InvariantCulture) + ",");
            writer.WriteLine("    \"raw_type\": " + raw.raw_type + ",");
            writer.Write("    \"lut_temp_value\": ");
            WriteFloatArrayValue(writer, LutTempValue);
            writer.WriteLine(",");
            writer.Write("    \"lut_ad_value\": ");
            WriteUshortArrayValue(writer, LutAdValue);
            writer.WriteLine(",");
            writer.WriteLine("    \"shutter_adv\": " + raw.shutter_adv);
            writer.Write("  }");
        }

        private static void WriteTempPoint(TextWriter writer, string name, NetSdkNative.NetTempPoint point, int indent)
        {
            string pad = new string(' ', indent);
            writer.Write(pad + "\"" + name + "\": {");
            writer.Write("\"temperature\": ");
            writer.Write(point.temperature.ToString("G9", CultureInfo.InvariantCulture));
            writer.Write(", \"point\": {\"x\": " + point.point.x + ", \"y\": " + point.point.y + "}}");
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

        private static string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
