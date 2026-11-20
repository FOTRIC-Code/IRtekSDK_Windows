using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IRtekNetSDKApiDemo
{

internal static partial class NetSdk
{
    public const string DllName = "IRtekNetSDK.dll";

    public const int Ok = 0;
    public const int StreamBgra = 4;
    public const int ExceptionNetworkDisconnected = 1;
    public const int ExceptionNetworkResumed = 2;
    public const int ExceptionVideoLost = 3;
    public const int ExceptionAlarmLost = 4;

    public const int LoginIpLen = 32;
    public const int LoginTextLen = 32;
    public const int DiscoveredIpLen = 64;
    public const int DiscoveredNameLen = 128;
    public const int DiscoveredSerialLen = 16;
    public const int DiscoveredIdLen = 32;
    public const int CodecLen = 16;
    public const int PathLen = 128;
    public const int Text16 = 16;
    public const int Text32 = 32;
    public const int Text64 = 64;
    public const int Text128 = 128;
    public const int Text256 = 256;
    public const int MaxDns = 4;
    public const int MaxGateways = 4;
    public const int MaxRanges = 4;
    public const int MaxLutPoints = 64;
    public const int MaxPaletteColors = 256;
    public const int MaxMarkerPoints = 10;
    public const int MaxMarkers = 16;
    public const int MaxRegions = 3;
    public const int MaxScheduleDays = 7;
    public const int MaxSchedulePeriods = 8;
    public const int MaxWebhooks = 3;

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void LogCallback(int level, IntPtr message, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void StreamCallback(IntPtr streamData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void AlarmCallback(IntPtr alarmInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void ExceptionCallback(IntPtr exceptionInfo);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_sdk_init();

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_sdk_cleanup();

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_enable_log(byte enable);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_log_callback(LogCallback cb, IntPtr userData);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_search_devices(
        [In, Out] NetDiscoveredDevice[] devices,
        ushort length,
        out ushort outLength,
        uint timeoutMs);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_login(ref NetLoginInfo info, out IntPtr handle);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_logout(ref IntPtr handle);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_port(IntPtr handle, out NetPortInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_streams(
        IntPtr handle,
        [In, Out] NetStreamInfo[] streams,
        ushort length,
        out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_stream(IntPtr handle, uint streamId, ref NetStreamInfo stream);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_start_stream_play(
        IntPtr handle,
        ref NetStreamInfo stream,
        int streamType,
        StreamCallback cb);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_stop_stream_play(IntPtr streamHandle);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_start_alarm_listener(IntPtr handle, AlarmCallback cb);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_stop_alarm_listener(IntPtr alarmListenerHandle);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_exception_callback(IntPtr handle, ExceptionCallback cb);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_device_upgrade(IntPtr handle, ref NetUpgradeInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_device_restart(IntPtr handle);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_device_reset(IntPtr handle);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_sdk_version(out NetSdkVersion version);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_device_info(IntPtr handle, ref NetDeviceInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_device_info(IntPtr handle, ref NetDeviceInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_time(IntPtr handle, ref NetTimeInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_time(IntPtr handle, ref NetTimeInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_ntp_server(IntPtr handle, ref NetNtpInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_ntp_server(IntPtr handle, ref NetNtpInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_port(IntPtr handle, ref NetPortInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_networks(IntPtr handle, [In, Out] NetNetworkInfo[] networks, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_network(IntPtr handle, uint id, ref NetNetworkInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_network(IntPtr handle, uint id, ref NetNetworkInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_ftp(IntPtr handle, ref NetFtpInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_ftp(IntPtr handle, ref NetFtpInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_test_ftp(IntPtr handle, ref NetFtpInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_webhook(IntPtr handle, ref NetWebhookInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_webhook(IntPtr handle, ref NetWebhookInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_webhooks(IntPtr handle, [In, Out] NetWebhookInfo[] list, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_webhooks(IntPtr handle, [In] NetWebhookInfo[] list, ushort length);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_users(IntPtr handle, [In, Out] NetUserInfo[] users, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_user(IntPtr handle, uint id, ref NetUserInfo user);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_stream_status(IntPtr handle, uint streamId, out NetStreamStatus status);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_channels(IntPtr handle, [In, Out] NetChannelInfo[] channels, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_channel(IntPtr handle, uint channelId, ref NetChannelInfo channel);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_channel(IntPtr handle, uint channelId, ref NetChannelInfo channel);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_overlay(IntPtr handle, uint channelId, ref NetOverlayInfo overlay);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_overlay(IntPtr handle, uint channelId, ref NetOverlayInfo overlay);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_image_params(IntPtr handle, uint channelId, out NetImageParams info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_image_params(IntPtr handle, uint channelId, ref NetImageParams info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_image_flip(IntPtr handle, uint channelId, out int mode);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_image_flip(IntPtr handle, uint channelId, ref int mode);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_lens_info(IntPtr handle, ref NetThermalLensInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_thermal_calibrate(IntPtr handle);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_shutter_mode(IntPtr handle, out int mode);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_shutter_mode(IntPtr handle, ref int mode);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_shutter_time(IntPtr handle, out ushort minutes);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_shutter_time(IntPtr handle, ref ushort minutes);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_resolution(IntPtr handle, out NetSize size);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_range(IntPtr handle, out ushort rangeId);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_range(IntPtr handle, ref ushort rangeId);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_lut(IntPtr handle, ref NetLutTable lut);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_params(IntPtr handle, out NetThermalParams info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_params(IntPtr handle, ref NetThermalParams info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_color_dist(IntPtr handle, out int type);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_color_dist(IntPtr handle, ref int type);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_gain(IntPtr handle, ref NetGain info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_gain(IntPtr handle, ref NetGain info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_preset_palettes(IntPtr handle, [In, Out] NetPresetPalette[] palettes, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_palette(IntPtr handle, ref NetPalette palette);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_palette(IntPtr handle, ref NetPalette palette);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_overlay(IntPtr handle, ref NetThermalOverlayInfo overlay);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_overlay(IntPtr handle, ref NetThermalOverlayInfo overlay);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_unit(IntPtr handle, out NetUnitInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_unit(IntPtr handle, ref NetUnitInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_capture(IntPtr handle, int mode, ref NetCaptureFile captureFile);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_markers(IntPtr handle, [In, Out] NetMarkerInfo[] markers, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_markers(IntPtr handle, [In] NetMarkerInfo[] markers, ushort length);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_temp_alarms(IntPtr handle, [In, Out] NetThermalTempAlarm[] alarms, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_temp_alarms(IntPtr handle, [In] NetThermalTempAlarm[] alarms, ushort length);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_temp_alarm_interval(IntPtr handle, out NetThermalTempAlarmInterval interval);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_temp_alarm_interval(IntPtr handle, ref NetThermalTempAlarmInterval interval);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_intrusion_alarm(IntPtr handle, out NetIntrusionAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_intrusion_alarm(IntPtr handle, ref NetIntrusionAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_high_temp_alarm(IntPtr handle, out NetHighTempAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_high_temp_alarm(IntPtr handle, ref NetHighTempAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_thermal_temp_detect_alarm(IntPtr handle, out NetTempDetectAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_thermal_temp_detect_alarm(IntPtr handle, ref NetTempDetectAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_alarm_trigger(IntPtr handle, ref int type, ref NetAlarmTrigger trigger);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_alarm_trigger(IntPtr handle, ref int type, ref NetAlarmTrigger trigger);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_alarm_schedule(IntPtr handle, ref int type, ref NetAlarmSchedule schedule);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_alarm_schedule(IntPtr handle, ref int type, ref NetAlarmSchedule schedule);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_sensor_info(IntPtr handle, ref NetAcousticSensorInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_freq_range(IntPtr handle, out NetAcousticFreqRange range);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_acoustic_freq_range(IntPtr handle, ref NetAcousticFreqRange range);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_db_scale(IntPtr handle, ref NetAcousticDbScale scale);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_acoustic_db_scale(IntPtr handle, ref NetAcousticDbScale scale);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_overlay(IntPtr handle, ref NetAcousticOverlayInfo overlay);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_acoustic_overlay(IntPtr handle, ref NetAcousticOverlayInfo overlay);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_capture(IntPtr handle, int mode, ref NetCaptureFile captureFile);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_source_alarm(IntPtr handle, out NetAcousticSourceAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_acoustic_source_alarm(IntPtr handle, ref NetAcousticSourceAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_db_alarm(IntPtr handle, out NetAcousticDbAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_acoustic_db_alarm(IntPtr handle, ref NetAcousticDbAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_acoustic_freq_alarm(IntPtr handle, out NetAcousticFreqAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_acoustic_freq_alarm(IntPtr handle, ref NetAcousticFreqAlarm alarm);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_storages(IntPtr handle, [In, Out] NetStorageInfo[] storages, ushort length, out ushort outLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_set_storage(IntPtr handle, uint id, ref NetStorageInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_find_alarm_datas(IntPtr handle, ref NetSearchAlarmParam param, ref NetSearchAlarmResult result);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_sensor_sn(IntPtr handle, byte[] data, ushort dataLength);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_seal(IntPtr handle, ref NetSealParam param);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_preset_info(IntPtr handle, out NetPresetInfo info);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_sensor_temp(IntPtr handle, out uint temperature);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern int net_get_license_code(IntPtr handle, byte[] data, ushort dataLength);

    public static void CheckLayouts()
    {
        bool x64 = IntPtr.Size == 8;
        ExpectSize<NetLoginInfo>(104);
        ExpectSize<NetDiscoveredDevice>(689);
        ExpectSize<NetStreamInfo>(68);
        ExpectSize<NetStreamData>(40);
        ExpectSize<NetPortInfo>(6);
        ExpectSize<NetAlarmer>(322);
        ExpectSize<NetAlarmInfo>(x64 ? 352 : 340);
        ExpectSize<NetAlarmIntrusionInfo>(96);
        ExpectSize<NetAlarmHighTempInfo>(x64 ? 104 : 96);
        ExpectSize<NetAlarmTempDetectInfo>(104);
        ExpectSize<NetAlarmThermalTempInfo>(x64 ? 2760 : 2744);
        ExpectSize<NetAlarmAcousticSourceInfo>(104);
        ExpectSize<NetAlarmAcousticDbInfo>(x64 ? 104 : 96);
        ExpectSize<NetAlarmAcousticFreqInfo>(104);
        ExpectSize<NetUpgradeInfo>(128);
        ExpectSize<NetSdkVersion>(8);
        ExpectSize<NetDeviceInfo>(784);
        ExpectSize<NetTimeInfo>(68);
        ExpectSize<NetNtpInfo>(136);
        ExpectSize<NetNetworkInfo>(684);
        ExpectSize<NetFtpInfo>(294);
        ExpectSize<NetWebhookInfo>(256);
        ExpectSize<NetUserInfo>(72);
        ExpectSize<NetChannelInfo>(68);
        ExpectSize<NetImageParams>(8);
        ExpectSize<NetOverlayInfo>(188);
        ExpectSize<NetThermalLensInfo>(160);
        ExpectSize<NetLutTable>(388);
        ExpectSize<NetPalette>(1036);
        ExpectSize<NetThermalOverlayInfo>(120);
        ExpectSize<NetMarkerInfo>(132);
        ExpectSize<NetThermalTempAlarm>(96);
        ExpectSize<NetAlarmTrigger>(2);
        ExpectSize<NetAlarmSchedule>(1822);
        ExpectSize<NetAcousticSensorInfo>(216);
        ExpectSize<NetAcousticOverlayInfo>(56);
        ExpectSize<NetStorageInfo>(88);
        ExpectSize<NetSealParam>(129);
        ExpectSize<NetPresetInfo>(8);
        ExpectSize<NetSearchAlarmParam>(32);
        ExpectSize<NetSearchPage>(12);
        ExpectSize<NetSearchAlarmData>(416);
        ExpectSize<NetSearchAlarmResult>(x64 ? 24 : 12);
        ExpectSize<NetExceptionInfo>(136);
    }

    public static string ErrorText(int code)
    {
        switch (code)
        {
            case 0: return "成功";
            case -1: return "失败";
            case -2: return "内存不足";
            case -3: return "空指针";
            case -4: return "参数无效";
            case -5: return "不支持";
            case -6: return "SDK 未初始化";
            case -7: return "HTTP 错误";
            case -8: return "认证失败";
            case -9: return "超时";
            default: return "错误 " + code;
        }
    }

    public static string PtrToUtf8(IntPtr pointer)
    {
        if (pointer == IntPtr.Zero)
            return "";
        int length = 0;
        while (Marshal.ReadByte(pointer, length) != 0)
        {
            length++;
            if (length > 65536)
                break;
        }
        if (length == 0)
            return "";
        byte[] raw = new byte[length];
        Marshal.Copy(pointer, raw, 0, length);
        return Encoding.UTF8.GetString(raw);
    }

    public static string CString(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return "";
        int length = Array.IndexOf(bytes, (byte)0);
        if (length < 0)
            length = bytes.Length;
        return Encoding.UTF8.GetString(bytes, 0, length);
    }

    public static string CString(IntPtr pointer, uint length)
    {
        if (pointer == IntPtr.Zero || length == 0)
            return "";
        int count = (int)Math.Min(length, 4096u);
        byte[] raw = new byte[count];
        Marshal.Copy(pointer, raw, 0, count);
        return CString(raw);
    }

    public static void SetCString(byte[] destination, string value)
    {
        Array.Clear(destination, 0, destination.Length);
        if (string.IsNullOrEmpty(value) || destination.Length == 0)
            return;
        byte[] raw = Encoding.UTF8.GetBytes(value);
        Buffer.BlockCopy(raw, 0, destination, 0, Math.Min(raw.Length, destination.Length - 1));
    }

    public static int FindAlarmDatas(IntPtr handle, NetSearchAlarmParam param, int capacity, out NetSearchPage page, out AlarmSearchItem[] items)
    {
        page = new NetSearchPage();
        items = new AlarmSearchItem[0];
        if (capacity < 0)
            capacity = 0;

        int pageSize = Marshal.SizeOf(typeof(NetSearchPage));
        int itemSize = Marshal.SizeOf(typeof(NetSearchAlarmData));
        IntPtr pagePtr = Marshal.AllocHGlobal(pageSize);
        IntPtr dataPtr = IntPtr.Zero;
        try
        {
            Marshal.StructureToPtr(page, pagePtr, false);
            var result = new NetSearchAlarmResult
            {
                Datas = IntPtr.Zero,
                Length = 0,
                Page = pagePtr
            };
            if (capacity > 0)
            {
                dataPtr = Marshal.AllocHGlobal(itemSize * capacity);
                for (int i = 0; i < capacity; i++)
                    Marshal.StructureToPtr(NetSearchAlarmData.CreateBlank(), IntPtr.Add(dataPtr, i * itemSize), false);
                result.Datas = dataPtr;
                result.Length = (ushort)capacity;
            }

            int code = net_find_alarm_datas(handle, ref param, ref result);
            page = (NetSearchPage)Marshal.PtrToStructure(pagePtr, typeof(NetSearchPage));
            if (code != Ok || capacity == 0 || page.Count == 0)
                return code;

            int count = (int)Math.Min(page.Count, (uint)capacity);
            items = new AlarmSearchItem[count];
            for (int i = 0; i < count; i++)
            {
                var raw = (NetSearchAlarmData)Marshal.PtrToStructure(IntPtr.Add(dataPtr, i * itemSize), typeof(NetSearchAlarmData));
                items[i] = new AlarmSearchItem
                {
                    AlarmId = raw.AlarmId,
                    ChannelId = raw.ChannelId,
                    TimestampUs = raw.TimestampUs,
                    AlarmType = raw.AlarmType,
                    RecordPath = CString(raw.RecordPath),
                    SnapshotPath = CString(raw.SnapshotPath),
                    DataPath = CString(raw.DataPath),
                    Detail = AlarmHub.DescribeDetail((int)raw.AlarmType, raw.AlarmInfo, raw.AlarmInfoLength)
                };
            }
            return code;
        }
        finally
        {
            if (dataPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(dataPtr);
            Marshal.FreeHGlobal(pagePtr);
        }
    }

    static void ExpectSize<T>(int expected) where T : struct
    {
        int actual = Marshal.SizeOf(typeof(T));
        if (actual != expected)
            throw new InvalidOperationException($"{typeof(T).Name} 布局大小 {actual}，期望 {expected}");
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetLoginInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.LoginIpLen)]
    public byte[] Ip;

    public ushort PortNo;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.LoginTextLen)]
    public byte[] UserName;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.LoginTextLen)]
    public byte[] Password;

    public uint TimeoutMs;

    public static NetLoginInfo Create(string ip, ushort port, string user, string password, uint timeoutMs)
    {
        var info = new NetLoginInfo
        {
            Ip = new byte[NetSdk.LoginIpLen],
            UserName = new byte[NetSdk.LoginTextLen],
            Password = new byte[NetSdk.LoginTextLen],
            PortNo = port,
            TimeoutMs = timeoutMs
        };
        NetSdk.SetCString(info.Ip, ip);
        NetSdk.SetCString(info.UserName, user);
        NetSdk.SetCString(info.Password, password);
        return info;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetDiscoveredDevice
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredIpLen)]
    public byte[] Ip;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredIpLen)]
    public byte[] Netmask;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredNameLen)]
    public byte[] Name;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredNameLen)]
    public byte[] Model;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredSerialLen)]
    public byte[] SerialNo;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredIdLen)]
    public byte[] Id;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredNameLen)]
    public byte[] FirmwareVersion;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.DiscoveredNameLen)]
    public byte[] FirmwareName;

    public byte Wandering;

    public static NetDiscoveredDevice CreateBlank()
    {
        return new NetDiscoveredDevice
        {
            Ip = new byte[NetSdk.DiscoveredIpLen],
            Netmask = new byte[NetSdk.DiscoveredIpLen],
            Name = new byte[NetSdk.DiscoveredNameLen],
            Model = new byte[NetSdk.DiscoveredNameLen],
            SerialNo = new byte[NetSdk.DiscoveredSerialLen],
            Id = new byte[NetSdk.DiscoveredIdLen],
            FirmwareVersion = new byte[NetSdk.DiscoveredNameLen],
            FirmwareName = new byte[NetSdk.DiscoveredNameLen]
        };
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetStreamInfo
{
    public uint Id;
    public uint BitRate;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.CodecLen)]
    public byte[] CodecType;

    public byte DisableSkip;
    public byte EnableRc;
    public uint GopSize;
    public uint H264Profile;
    public uint InitialQp;
    public uint MaxFrameRate;
    public uint MaximumQp;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.CodecLen)]
    public byte[] QualityControlType;

    public ushort ResolutionWidth;
    public ushort ResolutionHeight;

    public static NetStreamInfo CreateBlank()
    {
        return new NetStreamInfo
        {
            CodecType = new byte[NetSdk.CodecLen],
            QualityControlType = new byte[NetSdk.CodecLen]
        };
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetStreamData
{
    public uint StreamId;
    public int StreamType;
    public ushort Width;
    public ushort Height;
    public uint DataLength;
    public IntPtr Data;
    public ulong TimestampUs;
    public byte KeyFrame;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetPortInfo
{
    public ushort HttpPortNo;
    public ushort RtspPortNo;
    public ushort RtpPortNo;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct NetAlarmer
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public byte[] SerialNo;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    public byte[] DeviceName;

    public ushort LinkPort;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    public byte[] DeviceIp;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmInfo
{
    public int Type;
    public NetAlarmer Alarmer;
    public uint Length;
    public IntPtr Payload;
    public uint AlarmId;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmIntrusionInfo
{
    public ulong TimestampUs;
    public NetAlarmDeviceInfo Device;
    public int MotionLevel;
    public uint MessageLength;
    public IntPtr Message;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmHighTempInfo
{
    public ulong TimestampUs;
    public NetAlarmDeviceInfo Device;
    public float Temperature;
    public float Threshold;
    public uint MessageLength;
    public IntPtr Message;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmTempDetectInfo
{
    public ulong TimestampUs;
    public NetAlarmDeviceInfo Device;
    public float Temperature;
    public float Baseline;
    public float Slope;
    public uint MessageLength;
    public IntPtr Message;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetTempPoint
{
    public float Temperature;
    public ushort X;
    public ushort Y;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetTempPointInfo
{
    public NetTempPoint Max;
    public NetTempPoint Min;
    public float AvgTemp;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetMarkerAlarmInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public byte[] Name;

    public int AlarmLevel;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetMarkerTempInfo
{
    public uint Id;
    public int Type;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public byte[] Name;

    public NetTempPointInfo Temp;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetRawFrame
{
    public ushort Width;
    public ushort Height;
    public ushort RangeId;
    public uint PointsCount;
    public IntPtr Data;
    public uint LutLen;
    public IntPtr LutTempValue;
    public IntPtr LutAdValue;
    public uint LutEnvTemp;
    public uint SensorTemp;
    public uint ShutterState;
    public uint FrameNo;
    public ulong Pts;
    public uint RawType;
    public uint ShutterAdv;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmDeviceInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public byte[] Ip;

    public ushort PortNo;
    public uint ChannelId;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmThermalTempInfo
{
    public ulong TimestampUs;
    public NetAlarmDeviceInfo Device;
    public NetRawFrame RawFrame;
    public NetTempPointInfo GlobalTemp;

    public ushort MarkerAlarmsCount;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public NetMarkerAlarmInfo[] MarkerAlarms;

    public ushort MarkersCount;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public NetMarkerTempInfo[] MarkerTemps;
    public uint MessageLength;
    public IntPtr Message;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetPoint
{
    public ushort X;
    public ushort Y;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmAcousticSourceInfo
{
    public ulong TimestampUs;
    public NetAlarmDeviceInfo Device;
    public uint Duration;
    public uint TolerancePixels;
    public NetPoint Point;
    public uint MessageLength;
    public IntPtr Message;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmAcousticDbInfo
{
    public ulong TimestampUs;
    public NetAlarmDeviceInfo Device;
    public float Db;
    public float DbThreshold;
    public uint MessageLength;
    public IntPtr Message;
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetUpgradeInfo
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.PathLen)]
    public byte[] Path;

    public static NetUpgradeInfo FromFile(string path)
    {
        var info = new NetUpgradeInfo
        {
            Path = new byte[NetSdk.PathLen]
        };
        NetSdk.SetCString(info.Path, path);
        return info;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct NetAlarmAcousticFreqInfo
{
    public ulong TimestampUs;
    public NetAlarmDeviceInfo Device;
    public float Frequency;
    public float Db;
    public float DbThreshold;
    public uint MessageLength;
    public IntPtr Message;
}

internal static class ChannelIds
{
    public const uint Default = 101;
}

internal sealed class StreamDescriptor
{
    public uint Id { get; set; }
    public string Codec { get; set; } = "";
    public int Width { get; set; }
    public int Height { get; set; }

    public override string ToString()
    {
        string size = Width > 0 && Height > 0 ? $"{Width}x{Height}" : "未知分辨率";
        string codec = string.IsNullOrWhiteSpace(Codec) ? "video" : Codec;
        return $"流 {Id}  {codec}  {size}";
    }
}

internal sealed class DeviceSession
{
    public IntPtr Handle { get; set; }
    public string Ip { get; set; } = "";
    public ushort HttpPort { get; set; }
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public ushort RtspPort { get; set; } = 554;
    public List<StreamDescriptor> Streams { get; private set; } = new List<StreamDescriptor>();

    public string BuildRtspUrl(uint streamId)
    {
        ushort port = RtspPort == 0 ? (ushort)554 : RtspPort;
        string auth = "";
        if (!string.IsNullOrEmpty(UserName))
        {
            auth = Uri.EscapeDataString(UserName);
            if (!string.IsNullOrEmpty(Password))
                auth += ":" + Uri.EscapeDataString(Password);
            auth += "@";
        }
        return $"rtsp://{auth}{Ip}:{port}/video/{streamId}";
    }
}
}
