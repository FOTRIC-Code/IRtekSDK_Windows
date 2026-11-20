using System;
using System.Runtime.InteropServices;

namespace IRtekNetSDKApiDemo
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetSdkVersion
    {
        public ushort Major;
        public ushort Minor;
        public ushort Patch;
        public ushort Build;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetSize
    {
        public ushort Width;
        public ushort Height;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetTempRange
    {
        public float Min;
        public float Max;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetTempRangeMap
    {
        public ushort Id;
        public NetTempRange Range;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetDeviceInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] Model;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] Spec;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text16)]
        public byte[] SerialNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text32)]
        public byte[] Id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text32)]
        public byte[] Brand;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Company;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] FirmwareName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] FirmwareVersion;

        public static NetDeviceInfo CreateBlank()
        {
            return new NetDeviceInfo
            {
                Name = new byte[NetSdk.Text128],
                Model = new byte[NetSdk.Text128],
                Spec = new byte[NetSdk.Text128],
                SerialNo = new byte[NetSdk.Text16],
                Id = new byte[NetSdk.Text32],
                Brand = new byte[NetSdk.Text32],
                Company = new byte[NetSdk.Text64],
                FirmwareName = new byte[NetSdk.Text128],
                FirmwareVersion = new byte[NetSdk.Text128]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetTimeInfo
    {
        public int TimeMode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text32)]
        public byte[] LocalTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text32)]
        public byte[] TimeZone;

        public static NetTimeInfo CreateBlank()
        {
            return new NetTimeInfo
            {
                LocalTime = new byte[NetSdk.Text32],
                TimeZone = new byte[NetSdk.Text32]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetNtpInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] HostName;
        public ushort PortNo;
        public uint SynInterval;

        public static NetNtpInfo CreateBlank()
        {
            return new NetNtpInfo { HostName = new byte[NetSdk.Text128] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetNetworkInfo
    {
        public uint Id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text32)]
        public byte[] Name;
        public byte Dhcp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Ip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Netmask;
        public ushort DnsCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxDns * NetSdk.Text64)]
        public byte[] Dns;
        public ushort GatewayCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxGateways * NetSdk.Text64)]
        public byte[] Gateway;

        public static NetNetworkInfo CreateBlank()
        {
            return new NetNetworkInfo
            {
                Name = new byte[NetSdk.Text32],
                Ip = new byte[NetSdk.Text64],
                Netmask = new byte[NetSdk.Text64],
                Dns = new byte[NetSdk.MaxDns * NetSdk.Text64],
                Gateway = new byte[NetSdk.MaxGateways * NetSdk.Text64]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetFtpInfo
    {
        public byte UseSsl;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Ip;
        public ushort PortNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text32)]
        public byte[] UserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Pwd;
        public byte EnableAnnoy;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] PathTemplate;

        public static NetFtpInfo CreateBlank()
        {
            return new NetFtpInfo
            {
                Ip = new byte[NetSdk.Text64],
                UserName = new byte[NetSdk.Text32],
                Pwd = new byte[NetSdk.Text64],
                PathTemplate = new byte[NetSdk.Text128]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetWebhookInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text256)]
        public byte[] Url;

        public static NetWebhookInfo CreateBlank()
        {
            return new NetWebhookInfo { Url = new byte[NetSdk.Text256] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetUserInfo
    {
        public uint Id;
        public int UserLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] UserName;

        public static NetUserInfo CreateBlank()
        {
            return new NetUserInfo { UserName = new byte[NetSdk.Text64] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetStreamStatus
    {
        public uint Id;
        public uint ConnectionCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetChannelInfo
    {
        public uint Id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Name;

        public static NetChannelInfo CreateBlank()
        {
            return new NetChannelInfo { Name = new byte[NetSdk.Text64] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetImageParams
    {
        public ushort Contrast;
        public ushort Brightness;
        public byte DdeEnable;
        public ushort DdeValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetOverlayBlock
    {
        public int Align;
        public byte Enabled;
        public int Mode;
        public uint X;
        public uint Y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetOverlayCustomBlock
    {
        public int Align;
        public byte Enabled;
        public int Mode;
        public uint X;
        public uint Y;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] Text;

        public static NetOverlayCustomBlock CreateBlank()
        {
            return new NetOverlayCustomBlock { Text = new byte[NetSdk.Text128] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetOverlayInfo
    {
        public NetOverlayBlock ChannelNameOverlay;
        public NetOverlayBlock TimeOverlay;
        public NetOverlayCustomBlock CustomOverlay;

        public static NetOverlayInfo CreateBlank()
        {
            return new NetOverlayInfo { CustomOverlay = NetOverlayCustomBlock.CreateBlank() };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetThermalLensInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Name;
        public float Focal;
        public float Vfov;
        public float Hfov;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text32)]
        public byte[] Band;
        public ushort RangesCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxRanges)]
        public NetTempRangeMap[] RangeMap;

        public static NetThermalLensInfo CreateBlank()
        {
            return new NetThermalLensInfo
            {
                Name = new byte[NetSdk.Text64],
                Band = new byte[NetSdk.Text32],
                RangeMap = new NetTempRangeMap[NetSdk.MaxRanges]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetLutTable
    {
        public ushort RangeId;
        public ushort Length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxLutPoints)]
        public ushort[] Ad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxLutPoints)]
        public float[] T;

        public static NetLutTable CreateBlank()
        {
            return new NetLutTable
            {
                Ad = new ushort[NetSdk.MaxLutPoints],
                T = new float[NetSdk.MaxLutPoints]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetThermalParams
    {
        public float Emissivity;
        public float Distance;
        public float ReflTemp;
        public float AtmTemp;
        public float RelHumidity;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetGain
    {
        public int Mode;
        public float Max;
        public float Min;
        public byte FixedSpan;
        public float Span;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetPresetPalette
    {
        public uint RefNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Name;

        public static NetPresetPalette CreateBlank()
        {
            return new NetPresetPalette { Name = new byte[NetSdk.Text64] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetPalette
    {
        public uint RefNo;
        public ushort ColorCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxPaletteColors)]
        public uint[] Colors;
        public byte Inverted;

        public static NetPalette CreateBlank()
        {
            return new NetPalette { Colors = new uint[NetSdk.MaxPaletteColors] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetOverlayFollowBlock
    {
        public int Align;
        public byte Enabled;
        public byte Follow;
        public byte MaxEnabled;
        public byte MinEnabled;
        public byte AvgEnabled;
        public byte EmissivityEnabled;
        public uint X;
        public uint Y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetThermalOverlayInfo
    {
        public NetOverlayBlock UnitOverlay;
        public NetOverlayBlock PltOverlay;
        public NetOverlayBlock ThermalParamsOverlay;
        public NetOverlayFollowBlock GlobalOverlay;
        public NetOverlayFollowBlock MarkerOverlay;
        public NetOverlayBlock AlarmOverlay;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetUnitInfo
    {
        public int TempUnit;
        public int DistanceUnit;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetCaptureFile
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.PathLen)]
        public byte[] LocalFile;

        public static NetCaptureFile FromPath(string path)
        {
            var info = new NetCaptureFile { LocalFile = new byte[NetSdk.PathLen] };
            NetSdk.SetCString(info.LocalFile, path);
            return info;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetMarkerInfo
    {
        public uint Id;
        public int Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Name;
        public uint LocalParams;
        public float ReflTemp;
        public float Distance;
        public float Emissivity;
        public ushort PointsCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxMarkerPoints)]
        public NetPoint[] Points;

        public static NetMarkerInfo CreateBlank()
        {
            return new NetMarkerInfo
            {
                Name = new byte[NetSdk.Text64],
                Points = new NetPoint[NetSdk.MaxMarkerPoints]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetThermalTempAlarm
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] MarkerName;
        public byte Enabled;
        public int AlarmType;
        public float AlarmTemp;
        public uint AlarmDelayTime;
        public float ThresholdTemp;
        public uint ThresholdDelayTime;
        public float TriggerTemp;
        public uint TriggerDelayTime;

        public static NetThermalTempAlarm CreateBlank()
        {
            return new NetThermalTempAlarm { MarkerName = new byte[NetSdk.Text64] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetThermalTempAlarmInterval
    {
        public ushort Interval;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetRegion
    {
        public uint Id;
        public ushort PointsCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxMarkerPoints)]
        public NetPoint[] Points;

        public static NetRegion CreateBlank()
        {
            return new NetRegion { Points = new NetPoint[NetSdk.MaxMarkerPoints] };
        }

        public static NetRegion[] CreateArray()
        {
            var regions = new NetRegion[NetSdk.MaxRegions];
            for (int i = 0; i < regions.Length; i++)
                regions[i] = CreateBlank();
            return regions;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetIntrusionAlarm
    {
        public byte Enabled;
        public uint NoRepeatMinutes;
        public uint Sensitivity;
        public uint RecordDurationSec;
        public ushort RegionsCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxRegions)]
        public NetRegion[] Regions;

        public static NetIntrusionAlarm CreateBlank()
        {
            return new NetIntrusionAlarm { Regions = NetRegion.CreateArray() };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetHighTempAlarm
    {
        public byte Enabled;
        public uint NoRepeatMinutes;
        public float AlarmTemp;
        public uint RecordDurationSec;
        public ushort RegionsCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxRegions)]
        public NetRegion[] Regions;

        public static NetHighTempAlarm CreateBlank()
        {
            return new NetHighTempAlarm { Regions = NetRegion.CreateArray() };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetTempDetectAlarm
    {
        public byte Enabled;
        public uint Sensitivity;
        public ushort RegionsCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxRegions)]
        public NetRegion[] Regions;

        public static NetTempDetectAlarm CreateBlank()
        {
            return new NetTempDetectAlarm { Regions = NetRegion.CreateArray() };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAlarmTrigger
    {
        public byte TriggerFtpEnabled;
        public byte TriggerWebhookEnabled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAlarmSchedulePeriod
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text16)]
        public byte[] StartTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text16)]
        public byte[] EndTime;

        public static NetAlarmSchedulePeriod CreateBlank()
        {
            return new NetAlarmSchedulePeriod
            {
                StartTime = new byte[NetSdk.Text16],
                EndTime = new byte[NetSdk.Text16]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAlarmScheduleDay
    {
        public byte Weekday;
        public ushort PeriodsCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxSchedulePeriods)]
        public NetAlarmSchedulePeriod[] Periods;

        public static NetAlarmScheduleDay CreateBlank()
        {
            var day = new NetAlarmScheduleDay
            {
                Periods = new NetAlarmSchedulePeriod[NetSdk.MaxSchedulePeriods]
            };
            for (int i = 0; i < day.Periods.Length; i++)
                day.Periods[i] = NetAlarmSchedulePeriod.CreateBlank();
            return day;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAlarmSchedule
    {
        public ushort DaysCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.MaxScheduleDays)]
        public NetAlarmScheduleDay[] Days;

        public static NetAlarmSchedule CreateBlank()
        {
            var schedule = new NetAlarmSchedule
            {
                Days = new NetAlarmScheduleDay[NetSdk.MaxScheduleDays]
            };
            for (int i = 0; i < schedule.Days.Length; i++)
                schedule.Days[i] = NetAlarmScheduleDay.CreateBlank();
            return schedule;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAcousticSensorInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] FirmwareVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] SerialNo;
        public uint SampleRate;
        public uint MicCount;
        public uint RefMicIndex;
        public float Amplitude;
        public NetAcousticFreqRange FreqRange;

        public static NetAcousticSensorInfo CreateBlank()
        {
            return new NetAcousticSensorInfo
            {
                FirmwareVersion = new byte[NetSdk.Text128],
                SerialNo = new byte[NetSdk.Text64]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAcousticFreqRange
    {
        public float Min;
        public float Max;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAcousticDbScale
    {
        public int Mode;
        public float Max;
        public float Min;
        public byte FixedSpan;
        public float Span;
        public float Sensitivity;
        public uint SourceSize;
        public float Limit;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetOverlayAcousticBlock
    {
        public int Align;
        public byte Enabled;
        public byte MaxDbEnabled;
        public byte MaxFreqEnabled;
        public uint X;
        public uint Y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAcousticOverlayInfo
    {
        public NetOverlayBlock PaletteOverlay;
        public NetOverlayBlock SpectrumOverlay;
        public NetOverlayAcousticBlock SplOverlay;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAcousticSourceAlarm
    {
        public byte Enabled;
        public uint Duration;
        public uint TolerancePixels;
        public uint NoRepeatMinutes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAcousticDbAlarm
    {
        public byte Enabled;
        public float DbThreshold;
        public uint NoRepeatMinutes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetAcousticFreqAlarm
    {
        public byte Enabled;
        public float Frequency;
        public float DbThreshold;
        public uint NoRepeatMinutes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetStorageInfo
    {
        public uint Id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text64)]
        public byte[] Name;
        public uint CapacityMb;
        public uint FreeMb;
        public int Status;
        public int Attr;
        public byte Recycling;

        public static NetStorageInfo CreateBlank()
        {
            return new NetStorageInfo { Name = new byte[NetSdk.Text64] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetSearchAlarmParam
    {
        public uint AlarmType;
        public uint ChannelId;
        public ulong StartTimestampUs;
        public ulong StopTimestampUs;
        public uint PageNo;
        public uint PageSize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetSearchPage
    {
        public uint Total;
        public uint PageNo;
        public uint Count;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetSearchAlarmData
    {
        public uint AlarmId;
        public uint ChannelId;
        public ulong TimestampUs;
        public uint AlarmType;
        public uint AlarmInfoLength;
        public IntPtr AlarmInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.PathLen)]
        public byte[] RecordPath;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.PathLen)]
        public byte[] SnapshotPath;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.PathLen)]
        public byte[] DataPath;

        public static NetSearchAlarmData CreateBlank()
        {
            return new NetSearchAlarmData
            {
                RecordPath = new byte[NetSdk.PathLen],
                SnapshotPath = new byte[NetSdk.PathLen],
                DataPath = new byte[NetSdk.PathLen]
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetSearchAlarmResult
    {
        public ushort Length;
        public IntPtr Datas;
        public IntPtr Page;
    }

    internal sealed class AlarmSearchItem
    {
        public uint AlarmId;
        public uint ChannelId;
        public ulong TimestampUs;
        public uint AlarmType;
        public string RecordPath = "";
        public string SnapshotPath = "";
        public string DataPath = "";
        public string Detail = "";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetSealParam
    {
        public byte Sealed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.PathLen)]
        public byte[] Path;

        public static NetSealParam CreateBlank()
        {
            return new NetSealParam { Path = new byte[NetSdk.PathLen] };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetFlipMode
    {
        public int Mode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetShutterDirection
    {
        public byte Inverted;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetPresetInfo
    {
        public NetFlipMode Mode;
        public NetShutterDirection Direction;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NetExceptionInfo
    {
        public int ExceptionType;
        public uint StreamId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NetSdk.Text128)]
        public byte[] Message;
    }
}
