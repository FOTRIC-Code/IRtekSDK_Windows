using System;
using System.Runtime.InteropServices;

namespace IRtekNetSDKApiDemo
{
    internal static class ApiLayout
    {
        public static void PrintSizes()
        {
            Print<NetSdkVersion>();
            Print<NetSize>();
            Print<NetDeviceInfo>();
            Print<NetTimeInfo>();
            Print<NetNtpInfo>();
            Print<NetNetworkInfo>();
            Print<NetFtpInfo>();
            Print<NetWebhookInfo>();
            Print<NetUserInfo>();
            Print<NetStreamStatus>();
            Print<NetChannelInfo>();
            Print<NetImageParams>();
            Print<NetOverlayBlock>();
            Print<NetOverlayCustomBlock>();
            Print<NetOverlayInfo>();
            Print<NetThermalLensInfo>();
            Print<NetLutTable>();
            Print<NetThermalParams>();
            Print<NetGain>();
            Print<NetPresetPalette>();
            Print<NetPalette>();
            Print<NetOverlayFollowBlock>();
            Print<NetThermalOverlayInfo>();
            Print<NetUnitInfo>();
            Print<NetCaptureFile>();
            Print<NetMarkerInfo>();
            Print<NetThermalTempAlarm>();
            Print<NetThermalTempAlarmInterval>();
            Print<NetRegion>();
            Print<NetIntrusionAlarm>();
            Print<NetHighTempAlarm>();
            Print<NetTempDetectAlarm>();
            Print<NetAlarmTrigger>();
            Print<NetAlarmSchedulePeriod>();
            Print<NetAlarmScheduleDay>();
            Print<NetAlarmSchedule>();
            Print<NetAcousticSensorInfo>();
            Print<NetAcousticFreqRange>();
            Print<NetAcousticDbScale>();
            Print<NetOverlayAcousticBlock>();
            Print<NetAcousticOverlayInfo>();
            Print<NetAcousticSourceAlarm>();
            Print<NetAcousticDbAlarm>();
            Print<NetAcousticFreqAlarm>();
            Print<NetStorageInfo>();
            Print<NetSearchAlarmParam>();
            Print<NetSearchPage>();
            Print<NetSearchAlarmData>();
            Print<NetSearchAlarmResult>();
            Print<NetSealParam>();
            Print<NetPresetInfo>();
            Print<NetExceptionInfo>();
        }

        static void Print<T>()
        {
            Console.WriteLine(typeof(T).Name + "=" + Marshal.SizeOf(typeof(T)));
        }
    }
}
