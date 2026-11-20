using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace IRtekNetSDKApiDemo
{
    internal sealed class AlarmNotice
    {
        public AlarmNotice(string time, string type, string detail)
        {
            Time = time;
            Type = type;
            Detail = detail;
        }

        public string Time { get; private set; }
        public string Type { get; private set; }
        public string Detail { get; private set; }
    }

    internal static class AlarmHub
    {
        public static readonly NetSdk.AlarmCallback Callback = OnAlarm;
        public static event Action<AlarmNotice> Received;

        static void OnAlarm(IntPtr alarmInfo)
        {
            if (alarmInfo == IntPtr.Zero)
                return;

            NetAlarmInfo header;
            try
            {
                header = Marshal.PtrToStructure<NetAlarmInfo>(alarmInfo);
            }
            catch (Exception)
            {
                return;
            }

            AlarmNotice notice;
            try
            {
                notice = Describe(header);
            }
            catch (Exception ex)
            {
                notice = new AlarmNotice(Clock(), TypeName(header.Type), "解析失败：" + ex.Message);
            }

            Action<AlarmNotice> handler = Received;
            if (handler != null)
                handler(notice);
        }

        public static string DescribeDetail(int type, IntPtr payload, uint length)
        {
            var header = new NetAlarmInfo
            {
                Type = type,
                Length = length,
                Payload = payload
            };
            return Describe(header).Detail;
        }

        public static string TypeName(int type)
        {
            switch (type)
            {
                case 0: return "全部";
                case 1: return "热像温度";
                case 2: return "区域入侵";
                case 3: return "高温";
                case 4: return "温度监测";
                case 5: return "声源";
                case 6: return "分贝";
                case 7: return "频率";
                default: return "报警 " + type;
            }
        }

        static AlarmNotice Describe(NetAlarmInfo header)
        {
            string type = TypeName(header.Type);
            string detail;
            switch (header.Type)
            {
                case 1:
                    detail = DescribeThermal(header);
                    break;
                case 2:
                    detail = DescribeFixed<NetAlarmIntrusionInfo>(header, delegate (NetAlarmIntrusionInfo info)
                    {
                        return Join(Timestamp(info.TimestampUs), Device(info.Device), "运动强度 " + info.MotionLevel, Message(info.Message, info.MessageLength));
                    });
                    break;
                case 3:
                    detail = DescribeFixed<NetAlarmHighTempInfo>(header, delegate (NetAlarmHighTempInfo info)
                    {
                        return Join(Timestamp(info.TimestampUs), Device(info.Device), "温度 " + F(info.Temperature), "阈值 " + F(info.Threshold), Message(info.Message, info.MessageLength));
                    });
                    break;
                case 4:
                    detail = DescribeFixed<NetAlarmTempDetectInfo>(header, delegate (NetAlarmTempDetectInfo info)
                    {
                        return Join(Timestamp(info.TimestampUs), Device(info.Device), "温度 " + F(info.Temperature), "基线 " + F(info.Baseline), "斜率 " + F(info.Slope), Message(info.Message, info.MessageLength));
                    });
                    break;
                case 5:
                    detail = DescribeFixed<NetAlarmAcousticSourceInfo>(header, delegate (NetAlarmAcousticSourceInfo info)
                    {
                        return Join(Timestamp(info.TimestampUs), Device(info.Device), "持续 " + info.Duration + "s", "容差 " + info.TolerancePixels + "px", "坐标 (" + info.Point.X + "," + info.Point.Y + ")", Message(info.Message, info.MessageLength));
                    });
                    break;
                case 6:
                    detail = DescribeFixed<NetAlarmAcousticDbInfo>(header, delegate (NetAlarmAcousticDbInfo info)
                    {
                        return Join(Timestamp(info.TimestampUs), Device(info.Device), "声压 " + F(info.Db) + " dB", "阈值 " + F(info.DbThreshold) + " dB", Message(info.Message, info.MessageLength));
                    });
                    break;
                case 7:
                    detail = DescribeFixed<NetAlarmAcousticFreqInfo>(header, delegate (NetAlarmAcousticFreqInfo info)
                    {
                        return Join(Timestamp(info.TimestampUs), Device(info.Device), "频率 " + F(info.Frequency) + " Hz", "声压 " + F(info.Db) + " dB", "阈值 " + F(info.DbThreshold) + " dB", Message(info.Message, info.MessageLength));
                    });
                    break;
                default:
                    detail = header.Length == 0 ? "无载荷" : "载荷 " + header.Length + " 字节";
                    break;
            }
            if (header.Alarmer.SerialNo != null)
                detail = Join("编号 " + header.AlarmId, Alarmer(header.Alarmer), detail);
            return new AlarmNotice(Clock(), type, detail);
        }

        static string DescribeThermal(NetAlarmInfo header)
        {
            int size = Marshal.SizeOf(typeof(NetAlarmThermalTempInfo));
            if (header.Payload == IntPtr.Zero || header.Length != (uint)size)
                return "载荷 " + header.Length + " 字节";

            NetAlarmThermalTempInfo info = (NetAlarmThermalTempInfo)Marshal.PtrToStructure(header.Payload, typeof(NetAlarmThermalTempInfo));
            return Join(
                Timestamp(info.TimestampUs),
                Device(info.Device),
                TempInfo("全局", info.GlobalTemp),
                Raw(info.RawFrame),
                MarkerAlarms(info),
                MarkerTemps(info),
                Message(info.Message, info.MessageLength));
        }

        static string Alarmer(NetAlarmer info)
        {
            return Join(
                "序列号 " + Text(NetSdk.CString(info.SerialNo)),
                "名称 " + Text(NetSdk.CString(info.DeviceName)),
                "端口 " + info.LinkPort,
                "IP " + Text(NetSdk.CString(info.DeviceIp)));
        }

        static string Device(NetAlarmDeviceInfo info)
        {
            return Join(
                "设备IP " + Text(NetSdk.CString(info.Ip)),
                "设备端口 " + info.PortNo,
                "通道 " + info.ChannelId);
        }

        static string Timestamp(ulong timestampUs)
        {
            DateTime time = AlarmTime.FromUnixUs(timestampUs);
            string clock = time == DateTime.MinValue ? "-" : time.ToString("yyyy-MM-dd HH:mm:ss");
            return "时间戳 " + timestampUs + "(" + clock + ")";
        }

        static string TempInfo(string name, NetTempPointInfo info)
        {
            return Join(
                name + "最高 " + Point(info.Max),
                "最低 " + Point(info.Min),
                "平均 " + F(info.AvgTemp));
        }

        static string Point(NetTempPoint point)
        {
            return F(point.Temperature) + "@(" + point.X + "," + point.Y + ")";
        }

        static string Raw(NetRawFrame raw)
        {
            return Join(
                "原始 " + raw.Width + "x" + raw.Height,
                "量程 " + raw.RangeId,
                "点数 " + raw.PointsCount,
                "LUT点数 " + raw.LutLen,
                "LUT环境温度 " + raw.LutEnvTemp,
                "传感器温度 " + raw.SensorTemp,
                "挡片状态 " + raw.ShutterState,
                "帧号 " + raw.FrameNo,
                "同步时间戳 " + raw.Pts,
                "帧类型 " + raw.RawType,
                "挡片温度 " + raw.ShutterAdv);
        }

        static string MarkerAlarms(NetAlarmThermalTempInfo info)
        {
            int count = info.MarkerAlarms == null ? 0 : Math.Min(info.MarkerAlarmsCount, info.MarkerAlarms.Length);
            if (count == 0)
                return "标识报警 0";
            string[] items = new string[count];
            for (int i = 0; i < count; i++)
            {
                NetMarkerAlarmInfo item = info.MarkerAlarms[i];
                items[i] = "标识报警[" + i + "] " + Text(NetSdk.CString(item.Name)) + " 等级 " + item.AlarmLevel + "(" + AlarmLevelName(item.AlarmLevel) + ")";
            }
            return string.Join("  ", items);
        }

        static string MarkerTemps(NetAlarmThermalTempInfo info)
        {
            int count = info.MarkerTemps == null ? 0 : Math.Min(info.MarkersCount, info.MarkerTemps.Length);
            if (count == 0)
                return "标识温度 0";
            string[] items = new string[count];
            for (int i = 0; i < count; i++)
            {
                NetMarkerTempInfo item = info.MarkerTemps[i];
                items[i] = Join(
                    "标识温度[" + i + "] id " + item.Id,
                    "类型 " + item.Type + "(" + MarkerTypeName(item.Type) + ")",
                    Text(NetSdk.CString(item.Name)),
                    TempInfo("", item.Temp));
            }
            return string.Join("  ", items);
        }

        static string AlarmLevelName(int level)
        {
            switch (level)
            {
                case 0: return "无";
                case 1: return "提醒";
                case 2: return "预警";
                case 3: return "报警";
                default: return "未知";
            }
        }

        static string MarkerTypeName(int type)
        {
            switch (type)
            {
                case 1: return "点";
                case 5: return "多边形";
                case 6: return "折线";
                default: return "未知";
            }
        }

        static string Message(IntPtr message, uint length)
        {
            return "消息 " + Text(NetSdk.CString(message, length));
        }

        static string Text(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        static string DescribeFixed<T>(NetAlarmInfo header, Func<T, string> format) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            if (header.Payload == IntPtr.Zero || header.Length != (uint)size)
                return "载荷 " + header.Length + " 字节";
            T payload = (T)Marshal.PtrToStructure(header.Payload, typeof(T));
            return format(payload);
        }

        static string Join(params string[] parts)
        {
            return string.Join("  ", parts);
        }

        static string F(float value)
        {
            return value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        static string Clock()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }

    internal sealed class ExceptionNotice
    {
        public ExceptionNotice(string time, int type, uint streamId, string message)
        {
            Time = time;
            Type = type;
            StreamId = streamId;
            Message = message;
        }

        public string Time { get; private set; }
        public int Type { get; private set; }
        public uint StreamId { get; private set; }
        public string Message { get; private set; }

        public bool IsAlarmLost { get { return Type == NetSdk.ExceptionAlarmLost; } }

        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case NetSdk.ExceptionNetworkDisconnected: return "网络断开";
                    case NetSdk.ExceptionNetworkResumed: return "网络恢复";
                    case NetSdk.ExceptionVideoLost: return "视频丢失";
                    case NetSdk.ExceptionAlarmLost: return "报警丢失";
                    default: return "异常 " + Type;
                }
            }
        }
    }

    internal static class ExceptionHub
    {
        public static readonly NetSdk.ExceptionCallback Callback = OnException;
        public static event Action<ExceptionNotice> Received;

        static void OnException(IntPtr info)
        {
            if (info == IntPtr.Zero)
                return;

            NetExceptionInfo header;
            try
            {
                header = Marshal.PtrToStructure<NetExceptionInfo>(info);
            }
            catch (Exception)
            {
                return;
            }

            var notice = new ExceptionNotice(
                DateTime.Now.ToString("HH:mm:ss"),
                header.ExceptionType,
                header.StreamId,
                NetSdk.CString(header.Message));
            Action<ExceptionNotice> handler = Received;
            if (handler != null)
                handler(notice);
        }
    }
}
