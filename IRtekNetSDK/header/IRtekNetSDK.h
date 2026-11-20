#pragma once

#include <stddef.h>
#include <stdint.h>

#if defined(_WIN32) || defined(_WIN64)
#if defined(NETSDK_EXPORT)
#define IRTEK_NETSDK_API __declspec(dllexport)
#else
#define IRTEK_NETSDK_API __declspec(dllimport)
#endif
#define IRTEK_NETSDK_CALL __stdcall
#else
#define IRTEK_NETSDK_API __attribute__((visibility("default")))
#define IRTEK_NETSDK_CALL
#endif

#ifdef __cplusplus
extern "C" {
#endif

#define IRTEK_NETSDK_NAME "IRtekNetSDK"

#define NET_LEN_16 16
#define NET_LEN_32 32
#define NET_LEN_64 64
#define NET_LEN_128 128
#define NET_LEN_256 256

#define NET_MAX_RANGES 4
#define NET_MAX_NTP_SERVERS 4
#define NET_MAX_DNS 4
#define NET_MAX_GATEWAYS 4
#define NET_MAX_LUT_POINTS 64
#define NET_MAX_PALETTE_COLORS 256
#define NET_MAX_MARKER_POINTS 10
#define NET_MAX_MARKERS 16
#define NET_MAX_REGIONS 3
#define NET_MAX_ACTIVE_ALARMS 16
#define NET_MAX_SCHEDULE_DAYS 7
#define NET_MAX_SCHEDULE_PERIODS 8
#define NET_MAX_WEBHOOKS 3
#define NET_DISK_ALL 0xFF /* 格式化全部可写硬盘，跳过只读硬盘 / Format every writable disk, skipping read-only disks */

typedef void* NET_HANDLE;
typedef unsigned char BYTE;
typedef unsigned short USHORT;
typedef int INT32;
typedef unsigned int UINT32;
typedef unsigned long long UINT64;

typedef enum
{
    NET_EC_OK = 0,                  /* 成功 / Success */
    NET_EC_FAIL = -1,               /* 通用失败 / General failure */
    NET_EC_MEMORY = -2,             /* 内存不足 / Out of memory */
    NET_EC_NULL_POINTER = -3,       /* 空指针 / Null pointer */
    NET_EC_INVALID_PARAM = -4,      /* 参数无效 / Invalid parameter */
    NET_EC_NOT_SUPPORTED = -5,      /* 不支持 / Not supported */
    NET_EC_NOT_INITIALIZED = -6,    /* 未初始化 / Not initialized */
    NET_EC_HTTP_ERROR = -7,         /* HTTP 错误 / HTTP error */
    NET_EC_AUTH_FAILED = -8,        /* 认证失败 / Authentication failed */
    NET_EC_TIMEOUT = -9             /* 超时 / Timeout */
} net_enum_error_code;

typedef enum
{
    NET_SHUTTER_AUTOMATIC = 0,      /* 自动快门 / Automatic shutter */
    NET_SHUTTER_MANUAL = 1          /* 手动快门 / Manual shutter */
} net_enum_shutter_mode;

typedef enum
{
    NET_IMAGE_FLIP_NONE = 0,        /* 不翻转 / No flip */
    NET_IMAGE_FLIP_UP_DOWN = 1,     /* 上下翻转 / Vertical flip */
    NET_IMAGE_FLIP_LEFT_RIGHT = 2,  /* 左右翻转 / Horizontal flip */
    NET_IMAGE_FLIP_CENTER = 3       /* 中心翻转 / Center flip (both) */
} net_enum_image_flip_mode;

typedef enum
{
    NET_TIME_MODE_LOCAL = 0,        /* 本地时间 / Local time */
    NET_TIME_MODE_NTP = 1           /* NTP 时间 / NTP time */
} net_enum_time_mode;

/* ------------- VIEWER (只读 / Read-only) -------------
 * 只能查看设备、网络、热成像、传感器和外设状态；不能修改任何配置。
 * Can only view device, network, thermal, sensor and peripheral status;
 * cannot modify any configuration.
 */
/* ------------- OPERATOR (日常操作 / Daily operation) -------------
 * 可进行日常调优和控制：图像/视频参数、热成像参数、传感器运行模式。
 * Allows daily tuning and control: image/video parameters, thermal
 * parameters, sensor operating modes.
 */
/* ------------- ROOT (管理员 / Administrator) -------------
 * 可进行系统级和维护级操作：用户管理、网络/时间/固件配置、
 * 事件与告警、设备重启/恢复出厂、传感器底层维护等。
 * Allows system-level and maintenance operations: user management,
 * network/time/firmware configuration, events and alarms,
 * device reboot/factory reset, sensor low-level maintenance, etc.
 */
typedef enum
{
    NET_USER_LEVEL_ROOT = 0,        /* 管理员 / Administrator */
    NET_USER_LEVEL_OPERATOR = 1,    /* 操作员 / Operator */
    NET_USER_LEVEL_VIEWER = 2       /* 观察者 / Viewer */
} net_enum_user_level;

typedef enum
{
    NET_OVERLAY_ALIGN_TOP_LEFT = 0,     /* 左上 / Top-left */
    NET_OVERLAY_ALIGN_TOP_RIGHT = 1,    /* 右上 / Top-right */
    NET_OVERLAY_ALIGN_BOTTOM_LEFT = 2,  /* 左下 / Bottom-left */
    NET_OVERLAY_ALIGN_BOTTOM_RIGHT = 3  /* 右下 / Bottom-right */
} net_enum_overlay_align;

typedef enum
{
    NET_OVERLAY_LIGHT = 0,         /* 常亮 / Steady */
    NET_OVERLAY_BLINK = 1          /* 闪烁 / Blinking */
} net_enum_overlay_display_mode;

typedef enum
{
    NET_EXCEPTION_NETWORK_DISCONNECTED = 1, /* 网络断开 / Network disconnected */
    NET_EXCEPTION_NETWORK_RESUMED = 2,      /* 网络恢复 / Network resumed */
    NET_EXCEPTION_VIDEO_LOST = 3,           /* 视频丢失 / Video lost */
    NET_EXCEPTION_ALARM_LOST = 4            /* 报警丢失 / Alarm lost */
} net_enum_exception_type;

typedef enum
{
    NET_H264 = 1,                      /* H.264 视频流 / H.264 video stream */
    NET_RGBA = 2,                      /* RGBA 视频流 / RGBA video stream */
    NET_YUV422 = 3,                    /* YUV422 视频流 / YUV422 video stream */
    NET_BGRA = 4                       /* BGRA 视频流 / BGRA video stream */
} net_enum_stream_type;

typedef enum
{
    NET_ALARM_HIGHESTGREATER = 1,     /* 最高温大于阈值 / Max temp above threshold */
    NET_ALARM_HIGHESTLESS = 2,        /* 最高温小于阈值 / Max temp below threshold */
    NET_ALARM_LOWESTGREATER = 3,      /* 最低温大于阈值 / Min temp above threshold */
    NET_ALARM_LOWESTLESS = 4,         /* 最低温小于阈值 / Min temp below threshold */
    NET_ALARM_AVERAGEGREATER = 5,     /* 平均温大于阈值 / Avg temp above threshold */
    NET_ALARM_AVERAGELESS = 6,        /* 平均温小于阈值 / Avg temp below threshold */
} net_enum_thermal_temp_alarm_type;

typedef enum
{
    NET_THERMAL_ALARM_NONE = 0,       /* 无报警 / No alarm */
    NET_THERMAL_ALARM_ALERT = 1,      /* 提醒 / Alert */
    NET_THERMAL_ALARM_WARNING = 2,    /* 预警 / Warning */
    NET_THERMAL_ALARM_ALARM = 3       /* 报警 / Alarm */
} net_enum_thermal_temp_alarm_level;

typedef enum
{
    NET_THERMAL_CAPTURE_PURE_IMAGE = 0, /* 纯图像 / Pure image */
    NET_THERMAL_CAPTURE_IRTEK_IMAGE = 1 /* IRtek 图像 / IRtek image */
} net_enum_thermal_capture_mode;

typedef enum 
{
    NET_THERMAL_T_TWB = 1,              /* 高温差均衡成像分布 / High-contrast balanced imaging distribution */
    NET_THERMAL_TEMP_LINEAR = 2,        /* 温度线性分布 / Temperature linear distribution */
    NET_THERMAL_AD_LINEAR = 3,          /* 信号线性分布 / Signal linear distribution */
} net_enum_color_dist_type;

typedef enum 
{
    NET_THERMAL_FIXED = 1,              /* 固定模式 / Fixed mode */
    NET_THERMAL_AUTO = 2,               /* 自动模式 / Auto mode */
} net_enum_gain_mode;

typedef enum 
{
    IR_FILE_SPOT = 1,               /* 点 / Spot */
    IR_FILE_POLYGON = 5,            /* 多边形: 大于等于三个点，小于等于六个点 / Polygon：points count greater than or equal to 3 and less than or equal to 6 */
    IR_FILE_POLYLINE = 6            /* 折线：由两个点连接而成 / Polyline: points count equal to 2 */
} net_enum_marker_type;

typedef enum
{
    NET_THERMAL_DATA_RAW = 0x01,        /* 原始数据 / Raw data */
    NET_THERMAL_DATA_GLOBAL = 0x02,     /* 全局数据 / Global data */
    NET_THERMAL_DATA_MARKER = 0x04,     /* 标记数据 / Marker data */
} net_enum_thermal_data_type;

typedef enum
{
    NET_ACOUSTIC_CAPTURE_PURE_IMAGE = 0,  /* 纯图像 / Pure image */
    NET_ACOUSTIC_CAPTURE_IRTEK_IMAGE = 1  /* IRtek 声像图像 / IRtek acoustic image */
} net_enum_acoustic_capture_mode;

typedef enum
{
    NET_ACOUSTIC_DB_SCALE_AUTO_MONO = 1,  /* 单声源模式 / Single-source mode */
    NET_ACOUSTIC_DB_SCALE_AUTO_MULTI = 2, /* 多声源模式 / Multi-source mode */
    NET_ACOUSTIC_DB_SCALE_FIXED = 3,      /* 固定模式 / Fixed mode */
    NET_ACOUSTIC_DB_SCALE_AUTO_HOLO = 4   /* 全息模式 / Holographic mode */
} net_enum_acoustic_db_scale_mode;

typedef enum
{
    NET_RAW_VISUAL_BGR = 0              /* BGR，按 B、G、R 交错 / BGR, interleaved B, G, R */
} net_raw_visual_frame_type;

typedef enum
{
    NET_ACOUSTIC_DATA_VISUAL = 0x01,    /* 可见光图像 / Visual image */
    NET_ACOUSTIC_DATA_SPL = 0x02,       /* 声压网格 / Sound pressure grid */
    NET_ACOUSTIC_DATA_SPECTRUM = 0x04,  /* 频谱数据 / Spectrum data */
    NET_ACOUSTIC_DATA_GLOBAL = 0x08     /* 全局声压统计 / Global sound-pressure statistics */
} net_enum_acoustic_data_type;

typedef enum
{
    NET_ALARM_THERMAL_TEMP = 1,         /* 热成像温度报警 / Thermal temperature alarm */
    NET_ALARM_INTRUSION = 2,            /* 区域入侵报警 / Intrusion alarm */
    NET_ALARM_HIGH_TEMP = 3,            /* 高温报警 / High temperature alarm */
    NET_ALARM_TEMP_DETECT = 4,          /* 温度监测报警 / Temperature detection alarm */
    NET_ALARM_ACOUSTIC_SOURCE = 5,      /* 声源报警 / Acoustic source alarm */
    NET_ALARM_ACOUSTIC_DB = 6,          /* 分贝报警 / Acoustic decibel alarm */
    NET_ALARM_ACOUSTIC_FREQ = 7         /* 频率报警 / Acoustic frequency alarm */
} net_enum_alarm_type;

typedef enum
{
    NONE = 0,                       /* 无 / None */
    SR = 1                          /* 超分辨率模式 / Super-resolution mode */
} net_enum_sr_mode;

typedef enum
{
    CENTIGRADE = 0,                 /* 摄氏度 / Celsius */
    KELVIN = 1,                     /* 开尔文 / Kelvin */
    FAHRENHEIT = 2                  /* 华氏度 / Fahrenheit */
} net_enum_temp_unit;

typedef enum
{
    METER = 0,                      /* 米 / Meter */
    FOOT = 1                        /* 英尺 / Foot */
} net_enum_distance_unit;

typedef struct
{
    BYTE ip[NET_LEN_32];         /* 设备 IP 地址 / Device IP address */
    USHORT port_no;              /* 设备端口号 / Device port number */
    BYTE user_name[NET_LEN_32];  /* 登录用户名 / Login username */
    BYTE password[NET_LEN_32];   /* 登录密码 / Login password */
    UINT32 timeout_ms;           /* 登录超时时间，单位毫秒 / Login timeout in milliseconds */
} net_login_info;

typedef struct
{
    USHORT major;            /* 主版本号 / Major version */
    USHORT minor;            /* 次版本号 / Minor version */
    USHORT patch;            /* 补丁版本号 / Patch version */
    USHORT build;            /* 构建号 / Build number */
} net_sdk_version;

typedef struct
{
    float min;               /* 温度下限 / Temperature lower bound */
    float max;               /* 温度上限 / Temperature upper bound */
} net_temp_range;

typedef struct
{
    USHORT id;               /* 量程编号 / Range ID */
    net_temp_range range;    /* 对应的温度范围 / Corresponding temperature range */
} net_temp_range_map;

typedef struct
{
    USHORT width;            /* 宽度 / Width */
    USHORT height;           /* 高度 / Height */
} net_size;

typedef struct
{
    BYTE name[NET_LEN_128];              /* 设备名称 / Device name */
    BYTE model[NET_LEN_128];             /* 设备型号 / Device model */
    BYTE spec[NET_LEN_128];              /* 设备规格 / Device specification */
    BYTE serial_no[NET_LEN_16];          /* 设备序列号 / Device serial number */
    BYTE id[NET_LEN_32];                 /* 设备 ID / Device ID */
    BYTE brand[NET_LEN_32];              /* 品牌 / Brand */
    BYTE company[NET_LEN_64];            /* 公司名称 / Company name */
    BYTE firmware_name[NET_LEN_128];     /* 固件名称 / Firmware name */
    BYTE firmware_version[NET_LEN_128];  /* 固件版本 / Firmware version */
} net_device_info;

typedef struct
{
    BYTE name[NET_LEN_64];                           /* 镜头名称 / Lens name */
    float focal;                                     /* 焦距 / Focal length */
    float vfov;                                      /* 垂直视场角 / Vertical FOV */
    float hfov;                                      /* 水平视场角 / Horizontal FOV */
    BYTE band[NET_LEN_32];                           /* 光谱波段 / Spectral band */
    USHORT ranges_count;                             /* 量程映射数量 / Number of range mappings */
    net_temp_range_map range_map[NET_MAX_RANGES];    /* 量程映射表 / Range mapping table */
} net_thermal_lens_info;

typedef struct
{
    net_enum_time_mode time_mode;    /* 时间模式 / Time mode */
    BYTE local_time[NET_LEN_32];     /* 本地时间字符串，time_mode 为 NET_TIME_MODE_LOCAL 时有效 / Local time string, valid when time_mode is NET_TIME_MODE_LOCAL */
    BYTE time_zone[NET_LEN_32];      /* 时区字符串 / Timezone string */
} net_time_info;

typedef struct
{
    USHORT http_port_no;             /* HTTP 端口号 / HTTP port number */
    USHORT rtsp_port_no;             /* RTSP 端口号 / RTSP port number */
    USHORT rtp_port_no;              /* RTP 端口号 / RTP port number */
} net_port_info;

typedef struct
{
    BYTE host_name[NET_LEN_128];    /* NTP 主机名或地址 / NTP hostname or address */
    USHORT port_no;                 /* NTP 端口号 / NTP port number */
    UINT32 syn_interval;            /* 同步间隔 / Synchronization interval */
} net_ntp_info;

typedef struct
{
    UINT32 id;                                   /* 网络接口 ID / Network interface ID */
    BYTE name[NET_LEN_32];                       /* 网卡名称 / NIC name */
    BYTE dhcp;                                   /* 是否启用 DHCP / DHCP enabled */
    BYTE ip[NET_LEN_64];                         /* IP 地址 / IP address */
    BYTE netmask[NET_LEN_64];                    /* 子网掩码 / Subnet mask */
    USHORT dns_count;                            /* DNS 数量 / DNS count */
    BYTE dns[NET_MAX_DNS][NET_LEN_64];           /* DNS 地址列表 / DNS address list */
    USHORT gateway_count;                        /* 网关数量 / Gateway count */
    BYTE gateway[NET_MAX_GATEWAYS][NET_LEN_64];  /* 网关地址列表 / Gateway address list */
} net_network_info;

typedef struct
{
    BYTE use_ssl;                    /* 是否启用 SSL/TLS / SSL/TLS enabled */
    BYTE ip[NET_LEN_64];             /* FTP 服务器地址 / FTP server address */
    USHORT port_no;                  /* FTP 端口号 / FTP port number */
    BYTE user_name[NET_LEN_32];      /* FTP 用户名 / FTP username */
    BYTE pwd[NET_LEN_64];            /* FTP 密码 / FTP password */
    BYTE enable_annoy;               /* 是否启用匿名登录 / Anonymous login enabled */
    BYTE path_template[NET_LEN_128]; /* 文件路径模板 / File path template, e.g. {DEVICE_NAME}/{DEVICE_SERIAL_NO}/123/{DEVICE_IP}/{CHANNEL_NAME}/{CHANNEL_ID}/{yyyyMMddHHmmssfff}.jpg */
} net_ftp_info;

typedef struct
{
    BYTE url[NET_LEN_256]; /* Webhook 地址，须以 http:// 或 https:// 开头，且在缓冲区内以 0 结尾；超长在截断前拒绝 / Webhook URL. Must start with http:// or https:// and be NUL-terminated inside the buffer. Overlong values are rejected before truncation */
} net_webhook_info;

typedef struct
{
    BYTE path[NET_LEN_128];          /* 升级包本地路径 / Local path to upgrade package */
} net_upgrade_info;

typedef struct
{
    UINT32 id;                       /* 用户 ID / User ID */
    net_enum_user_level user_level;  /* 用户权限级别 / User permission level */
    BYTE user_name[NET_LEN_64];      /* 用户名 / Username */
} net_user_info;

typedef struct
{
    UINT32 id;                       /* 用户 ID / User ID */
    net_enum_user_level user_level;  /* 用户权限级别 / User permission level */
    BYTE user_name[NET_LEN_64];      /* 用户名 / Username */
    BYTE login_pwd[NET_LEN_64];      /* 当前登录密码摘要，用于鉴权 / Current login password digest for authentication */
    BYTE pwd[NET_LEN_64];            /* 新密码摘要 / New password digest */
} net_user_pwd_info;

typedef struct
{
    UINT32 id;                         /* 视频流 ID / Stream ID */
    UINT32 bit_rate;                   /* 码率 / Bitrate */
    BYTE codec_type[NET_LEN_16];       /* 编码类型 / Codec type */
    BYTE disable_skip;                 /* 是否禁用跳帧 / Frame skip disabled */
    BYTE enable_rc;                    /* 是否启用码率控制 / Rate control enabled */
    UINT32 gop_size;                   /* GOP 长度 / GOP size */
    UINT32 h264_profile;               /* H.264 Profile / H.264 profile */
    UINT32 initial_qp;                 /* 初始 QP 值 / Initial QP value */
    UINT32 max_frame_rate;             /* 最大帧率 / Max frame rate */
    UINT32 maximum_qp;                /* 最大 QP 值 / Maximum QP value */
    BYTE quality_control_type[NET_LEN_16]; /* 码控类型 / Quality control type */
    USHORT resolution_width;           /* 分辨率宽度 / Resolution width */
    USHORT resolution_height;          /* 分辨率高度 / Resolution height */
} net_stream_info;

typedef struct
{
    UINT32 id;                /* 视频流 ID / Stream ID */
    UINT32 connection_count;  /* 当前连接数 / Current connection count */
} net_stream_status;

typedef struct
{
    USHORT contrast;      /* 对比度 / Contrast */
    USHORT brightness;    /* 亮度 / Brightness */
    BYTE dde_enable;      /* 是否启用 DDE / DDE enabled */
    USHORT dde_value;     /* DDE 强度 / DDE intensity */
} net_image_params;

typedef struct
{
    UINT32 id;                 /* 通道 ID / Channel ID */
    BYTE name[NET_LEN_64];     /* 通道名称 / Channel name */
} net_channel_info;

typedef struct
{
    net_enum_overlay_align align;            /* 对齐方式 / Alignment */
    BYTE enabled;                            /* 是否启用 / Enabled */
    net_enum_overlay_display_mode mode;      /* 显示模式 / Display mode */
    UINT32 x;                                /* X 坐标偏移 / X offset */
    UINT32 y;                                /* Y 坐标偏移 / Y offset */
} net_overlay_block;

typedef struct
{
    net_enum_overlay_align align;            /* 对齐方式 / Alignment */
    BYTE enabled;                            /* 是否启用 / Enabled */
    net_enum_overlay_display_mode mode;      /* 显示模式 / Display mode */
    UINT32 x;                                /* X 坐标偏移 / X offset */
    UINT32 y;                                /* Y 坐标偏移 / Y offset */
    BYTE text[NET_LEN_128];                   /* 文本内容 / Text content */
} net_overlay_custom_block;

typedef struct
{
    net_overlay_block channel_name_overlay;  /* 通道名叠加 / Channel name overlay */
    net_overlay_block time_overlay;          /* 时间叠加 / Time overlay */
    net_overlay_custom_block custom_overlay; /* 自定义信息叠加 / Custom info overlay */
} net_overlay_info;

typedef struct
{
    USHORT range_id;                          /* 量程编号 / Range ID */
    USHORT length;                            /* LUT 有效长度 / LUT valid length */
    USHORT ad[NET_MAX_LUT_POINTS];            /* AD 值数组 / AD value array */
    float t[NET_MAX_LUT_POINTS];              /* 温度值数组 / Temperature value array */
} net_lut_table;

typedef struct
{
    BYTE marker_name[NET_LEN_64];     /* 标识名称：报警配置通过名称和测温对象匹配 / Marker name: matching marker alarm config with thermal objects */
    BYTE enabled;                     /* 是否启用 / Enabled */
    net_enum_thermal_temp_alarm_type alarm_type;   /* 报警类型 / Alarm type */
    float alarm_temp;                 /* 报警温度 / Alarm temperature */
    UINT32 alarm_delay_time;          /* 报警延迟时间 / Alarm delay time */
    float warning_temp;               /* 预警温度 / Warning temperature */
    UINT32 warning_delay_time;        /* 预警延迟时间 / Warning delay time */
    float alert_temp;                 /* 提醒温度 / Alert temperature */
    UINT32 alert_delay_time;          /* 提醒延迟时间 / Alert delay time */
} net_thermal_temp_alarm;

typedef struct
{
    USHORT interval;                  /* 报警时间间隔 / Alarm time interval */
} net_thermal_temp_alarm_interval;

typedef struct
{
    USHORT x;    /* X 坐标 / X coordinate */
    USHORT y;    /* Y 坐标 / Y coordinate */
} net_point;

typedef struct
{
    UINT32 id;                                  /* 区域 ID / Region ID */
    USHORT points_count;                        /* 点数量 / Point count */
    net_point points[NET_MAX_MARKER_POINTS];    /* 组成区域的点集 / Points composing the region */
} net_region;

typedef struct
{
    BYTE enabled;                     /* 是否启用 / Enabled */
    UINT32 no_repeat_minutes;         /* 重复报警抑制间隔，单位分钟 / Repeat-alarm suppression interval in minutes */
    UINT32 sensitivity;               /* 灵敏度 / Sensitivity */
    UINT32 record_duration_sec;       /* 联动录像时长，单位秒 / Linked recording duration in seconds */
    USHORT regions_count;             /* 区域数量 / Region count */
    net_region regions[NET_MAX_REGIONS];    /* 入侵区域 / Intrusion region */
} net_intrusion_alarm;

typedef struct
{
    BYTE enabled;                     /* 是否启用 / Enabled */
    UINT32 no_repeat_minutes;         /* 重复报警抑制间隔，单位分钟 / Repeat-alarm suppression interval in minutes */
    float alarm_temp;                 /* 报警温度阈值 / Alarm temperature threshold */
    UINT32 record_duration_sec;       /* 联动录像时长，单位秒 / Linked recording duration in seconds */
    USHORT regions_count;             /* 区域数量 / Region count */
    net_region regions[NET_MAX_REGIONS];    /* 高温区域 / High-temperature region */
} net_high_temp_alarm;

typedef struct
{
    BYTE enabled;                     /* 是否启用 / Enabled */
    UINT32 sensitivity;               /* 灵敏度：0=低、1=中、2=高 / Sensitivity: 0=low, 1=medium, 2=high */
    USHORT regions_count;             /* 区域数量 / Region count */
    net_region regions[NET_MAX_REGIONS];    /* 温度监测区域 / Temperature-monitoring region */
} net_temp_detect_alarm;

typedef struct
{
    float emissivity;     /* 发射率 / Emissivity */
    float distance;       /* 测量距离 / Measurement distance */
    float refl_temp;      /* 反射温度 / Reflected temperature */
    float atm_temp;       /* 大气温度 / Atmospheric temperature */
    float rel_humidity;   /* 相对湿度 / Relative humidity */
} net_thermal_params;

typedef struct
{
    net_enum_gain_mode mode;    /* 温宽模式，默认为自动模式 / Gain mode, defaults to auto */
    float max;                  /* 温宽上限值，取值范围 0~5750 / Gain upper bound, range 0~5750 */
    float min;                  /* 温宽下限值，取值范围 0~5750 / Gain lower bound, range 0~5750 */
    BYTE fixedSpan;             /* 是否使用固定温宽跨度，0x01 启用，其它不启用，默认 0x00 / Fixed span enabled (0x01=on), default 0x00 */
    float span;                 /* 温宽跨度，取值范围 0.1~5750，默认 2.0 / Gain span, range 0.1~5750, default 2.0 */
} net_gain;

/**
 * ref_no 与调色板名称对应关系 / ref_no to palette name mapping:
 *   1  = Iron
 *   2  = Grey
 *   3  = Iron10
 *   4  = Rainbow
 *   5  = Grey10
 *   6  = GreyRed
 *   7  = MidGrey
 *   8  = Yellow
 *   9  = Rain
 *   10 = Rain10
 *   11 = Blue
 *   12 = GlowBow
 *   13 = Medical
 *   14 = MidGreen
 *   15 = Prism
 *   16 = Medical10
 */
typedef struct
{
    UINT32 ref_no;             /* 调色板编号 / Palette reference number */
    BYTE name[NET_LEN_64];    /* 调色板名称 / Palette name */
} net_preset_palette;

typedef struct
{
    UINT32 ref_no;                           /* 调色板编号 / Palette reference number */
    USHORT color_count;                      /* 颜色数量 / Color count */
    UINT32 colors[NET_MAX_PALETTE_COLORS];   /* 调色板颜色值 / Palette color values */
    BYTE inverted;                           /* 是否反转 / Inverted */
} net_palette;

typedef struct
{
    net_enum_overlay_align align;            /* 对齐方式 / Alignment */
    BYTE enabled;                            /* 是否启用 / Enabled */
    BYTE follow;                             /* 是否自动跟踪测温区域内最高温点和最低温点 / Show max/min following spot */
    BYTE max_enabled;                        /* 是否显示最高温 / Show max temperature */
    BYTE min_enabled;                        /* 是否显示最低温 / Show min temperature */
    BYTE avg_enabled;                        /* 是否显示平均温 / Show avg temperature */
    BYTE emissivity_enabled;                 /* 是否显示发射率 / Show emissivity */
    UINT32 x;                                /* X 坐标偏移 / X offset */
    UINT32 y;                                /* Y 坐标偏移 / Y offset */
} net_overlay_follow_block;

typedef struct
{
    net_enum_exception_type exception_type;  /* 异常类型 / Exception type */
    UINT32 stream_id;                /* 关联的视频流 ID / Associated stream ID */
    BYTE message[NET_LEN_128];       /* 异常消息 / Exception message */
} net_exception_info;

typedef struct
{
    UINT32 stream_id;                            /* 视频流 ID / Stream ID */
    net_enum_stream_type stream_type;            /* 解码视频流类型 / Decoded stream type */
    USHORT width;                                /* 图像宽度 / Image width */
    USHORT height;                               /* 图像高度 / Image height */
    UINT32 data_length;                          /* 数据长度 / Data length */
    const BYTE* data;                            /* 视频流数据指针 / Stream data pointer */
    UINT64 timestamp_us;                         /* 时间戳，单位微秒 / Timestamp in microseconds */
    BYTE key_frame;                              /* 是否关键帧 / Key frame flag */
} net_stream_data;

typedef struct
{
    net_overlay_block unit_overlay;               /* 单位叠加 / Unit overlay */
    net_overlay_block plt_overlay;                /* 调色板叠加 / Palette overlay */
    net_overlay_block thermal_params_overlay;     /* 测温参数叠加 / Thermal params overlay */
    net_overlay_follow_block global_overlay;      /* 全局温度叠加 / Global temperature overlay */
    net_overlay_follow_block marker_overlay;      /* 测温标识叠加 / Marker temperature overlay */
    net_overlay_block alarm_overlay;              /* 报警信息叠加 / Alarm info overlay */
} net_thermal_overlay_info;

typedef struct
{
    float temperature;    /* 温度值 / Temperature value */
    net_point point;      /* 对应坐标 / Corresponding coordinate */
} net_temp_point;

typedef struct
{
    net_temp_point max;   /* 最高温点 / Max temperature point */
    net_temp_point min;   /* 最低温点 / Min temperature point */
    float avg_temp;       /* 平均温度 / Average temperature */
} net_temp_point_info;

typedef struct
{
    UINT32 id;                                  /* 标识 ID / Marker ID */
    net_enum_marker_type type;                  /* 标识类型 / Marker type */
    BYTE name[NET_LEN_64];                      /* 标识名称 / Marker name */
    UINT32 local_params;                        /* 是否启用该测温对象的参数 / Use local params for this marker */
    float refl_temp;                            /* 反射温度 / Reflected temperature */
    float distance;                             /* 测量距离 / Measurement distance */
    float emissivity;                           /* 发射率 / Emissivity */
    USHORT points_count;                        /* 点数量 / Point count */
    net_point points[NET_MAX_MARKER_POINTS];    /* 组成测温对象的点集 / Points composing the marker */
} net_marker_info;

typedef struct
{
    net_enum_temp_unit temp_unit;          /* 温度单位 / Temperature unit */
    net_enum_distance_unit distance_unit;  /* 距离单位 / Distance unit */
} net_unit_info;

typedef struct
{
    BYTE marker_name[NET_LEN_64];                 /* 标识名称 / Marker name */
    net_enum_thermal_temp_alarm_level alarm_level;     /* 报警级别 / Alarm level */
} net_marker_alarm_info;

typedef struct
{
    UINT32 marker_id;                  /* 标识 ID / Marker ID */
    net_enum_marker_type type;         /* 标识类型 / Marker type */
    BYTE marker_name[NET_LEN_64];      /* 标识名称 / Marker name */
    net_temp_point_info temp_point_info; /* 该标识的温度统计信息 / Temperature statistics for this marker */
} net_marker_temp_info;

typedef struct
{
    BYTE ip[NET_LEN_64];                 /* 设备 IP 地址 / Device IP address */
    BYTE netmask[NET_LEN_64];            /* 设备子网掩码 / Device subnet mask */
    BYTE name[NET_LEN_128];              /* 设备名称 / Device name */
    BYTE model[NET_LEN_128];             /* 设备型号 / Device model */
    BYTE serial_no[NET_LEN_16];          /* 设备序列号 / Device serial number */
    BYTE id[NET_LEN_32];                 /* 设备 ID / Device ID */
    BYTE firmware_version[NET_LEN_128];  /* 固件版本 / Firmware version */
    BYTE firmware_name[NET_LEN_128];     /* 固件名称 / Firmware name */
    BYTE wandering;                      /* 是否冲突（地址冲突/可恢复） / Address conflict (recoverable) */
} net_discovered_device;

typedef struct
{
    BYTE local_file[NET_LEN_128];    /* 保存图片到本地文件的路径 / Local file path for captured image */
} net_capture_file;

typedef struct
{
    USHORT width;                  /* 原始图像宽度 / Raw image width */
    USHORT height;                 /* 原始图像高度 / Raw image height */
    USHORT range_id;               /* 测温量程 ID / Temperature range ID */
    UINT32 points_count;           /* 原始数据点数 / Raw data point count */
    const USHORT* data;            /* 原始 AD 数据指针（get：至下次 get/关闭句柄；流回调：仅回调期间有效） / Raw AD data pointer (get: valid until next get/handle close; stream callback: valid during callback only) */
    UINT32 lut_len;                /* LUT 数据长度 / LUT data length */
    const float* lut_temp_value;   /* LUT 温度值指针（生命周期同 data） / LUT temperature values (same lifetime as data) */
    const USHORT* lut_ad_value;    /* LUT AD 值指针（生命周期同 data） / LUT AD values (same lifetime as data) */
    UINT32 lut_env_temp;           /* LUT 环境温度 / LUT ambient temperature */
    UINT32 sensor_temp;            /* 机芯温度 / Sensor core temperature */
    UINT32 shutter_state;          /* 挡片状态 / Shutter state */
    UINT32 frame_no;               /* 帧序号 / Frame number */
    UINT64 pts;                    /* 同步时间戳 / Synchronization timestamp */
    UINT32 raw_type;               /* 帧类型 / Frame type */
    UINT32 shutter_adv;            /* 挡片温度 / Shutter temperature */
} net_raw_frame;

typedef struct
{
    net_enum_thermal_data_type data_types;      /* 本次携带的数据类型集合 / Data types included in this frame */
    UINT64 timestamp_us;                        /* 数据时间戳，单位微秒 / Data timestamp in microseconds */
    net_raw_frame raw_frame;                    /* 原始帧数据 / Raw frame data */
    net_temp_point_info global_temp_info;       /* 全局温度信息 / Global temperature info */
    USHORT markers_count;                       /* 测温对象的数量 / Marker count */
    net_marker_temp_info marker_temps[NET_MAX_MARKERS]; /* 测温对象的温度信息 / Marker temperature info */
} net_thermal_data;

typedef struct
{
    net_enum_thermal_data_type data_types;             /* 默认输出的数据类型 / Default output data types */
    UINT32 sample_interval;                            /* 采样间隔时间，单位毫秒，最小 100 / Sample interval in ms, minimum 100 */
} net_thermal_data_config;

typedef struct
{
    float min;                          /* 频率下限，单位 Hz / Frequency lower bound in Hz */
    float max;                          /* 频率上限，单位 Hz / Frequency upper bound in Hz */
} net_acoustic_freq_range;

typedef struct
{
    BYTE firmware_version[NET_LEN_128]; /* 固件版本 / Firmware version */
    BYTE serial_no[NET_LEN_64];         /* 序列号 / Serial number */
    UINT32 sample_rate;                 /* 采样率，单位 Hz / Sample rate in Hz */
    UINT32 mic_count;                   /* 麦克风数量 / Microphone count */
    UINT32 ref_mic_index;               /* 参考麦克风序号 / Reference microphone index */
    float amplitude;                    /* 幅度系数 / Amplitude factor */
    net_acoustic_freq_range freq_range; /* 模组频率范围，单位 Hz / Module frequency range in Hz */
} net_acoustic_sensor_info;

typedef struct
{
    net_enum_acoustic_db_scale_mode mode; /* 图像模式 / Image mode */
    float max;                            /* 声宽上限 / Scale upper bound */
    float min;                            /* 声宽下限 / Scale lower bound */
    BYTE fixed_span;                      /* 固定声宽跨度，0x01 启用 / Fixed span enabled (0x01=on) */
    float span;                           /* 声宽跨度 / Scale span */
    float sensitivity;                    /* 灵敏度 / Sensitivity */
    UINT32 source_size;                   /* 声源尺寸 / Source size */
    float limit;                          /* 声压下限 / Sound-pressure lower limit */
} net_acoustic_db_scale;

typedef struct
{
    net_enum_overlay_align align;       /* 对齐方式 / Alignment */
    BYTE enabled;                       /* 是否启用 / Enabled */
    BYTE max_db_enabled;                /* 是否显示最大声压 / Show max sound pressure */
    BYTE max_freq_enabled;              /* 是否显示最大频率 / Show max frequency */
    UINT32 x;                           /* X 坐标偏移 / X offset */
    UINT32 y;                           /* Y 坐标偏移 / Y offset */
} net_overlay_acoustic_block;

typedef struct
{
    net_overlay_block palette_overlay;          /* 调色板叠加 / Palette overlay */
    net_overlay_block spectrum_overlay;         /* 频谱叠加 / Spectrum overlay */
    net_overlay_acoustic_block spl_overlay;     /* 声压叠加 / Sound-pressure overlay */
} net_acoustic_overlay_info;

typedef struct
{
    BYTE enabled;                       /* 是否启用 / Enabled */
    UINT32 duration;                    /* 持续时间，单位秒 / Duration in seconds */
    UINT32 tolerance_pixels;            /* 容差像素 / Tolerance in pixels */
    UINT32 no_repeat_minutes;           /* 重复报警抑制间隔，单位分钟 / Repeat-alarm suppression interval in minutes */
} net_acoustic_source_alarm;

typedef struct
{
    BYTE enabled;                       /* 是否启用 / Enabled */
    float db_threshold;                 /* 分贝阈值 / Decibel threshold */
    UINT32 no_repeat_minutes;           /* 重复报警抑制间隔，单位分钟 / Repeat-alarm suppression interval in minutes */
} net_acoustic_db_alarm;

typedef struct
{
    BYTE enabled;                       /* 是否启用 / Enabled */
    float frequency;                    /* 频率，单位 Hz / Frequency in Hz */
    float db_threshold;                 /* 分贝阈值 / Decibel threshold */
    UINT32 no_repeat_minutes;           /* 重复报警抑制间隔，单位分钟 / Repeat-alarm suppression interval in minutes */
} net_acoustic_freq_alarm;

typedef struct
{
    float db;                           /* 声压级，单位 dB / Sound pressure level in dB */
    net_point point;                    /* 对应坐标 / Corresponding coordinate */
} net_acoustic_spl_point;

typedef struct
{
    net_acoustic_spl_point max;         /* 最大声压点 / Max sound-pressure point */
    net_acoustic_spl_point min;         /* 最小声压点 / Min sound-pressure point */
    float avg_db;                       /* 平均声压 / Average sound pressure */
} net_acoustic_spl_info;

typedef struct
{
    USHORT width;                       /* 声压网格宽度 / Sound-pressure grid width */
    USHORT height;                      /* 声压网格高度 / Sound-pressure grid height */
    UINT32 points_count;                /* 声压数据点数 / Sound-pressure point count */
    const float* data;                  /* 声压级数组，单位 dB / Sound-pressure array in dB */
} net_acoustic_spl_frame;

/* 文档未规定字段。当前按 float 采样数组承载，指针生命周期与 spl_frame.data 相同。
 * Fields are unspecified by the API doc. Carried as a float sample array;
 * the pointer has the same lifetime as spl_frame.data. */
typedef struct
{
    UINT32 bin_count;                   /* 频谱采样点数 / Spectrum sample count */
    const float* data;                  /* 频谱采样 / Spectrum samples */
} net_acoustic_spectrum_frame;

typedef struct
{
    USHORT width;                       /* 可见光图像宽度 / Visual image width */
    USHORT height;                      /* 可见光图像高度 / Visual image height */
    net_raw_visual_frame_type type;     /* 可见光数据格式，当前仅 BGR / Visual format, currently BGR only */
    UINT32 points_count;                /* 图像数据字节数；BGR 时为 width * height * 3 / Image byte count; BGR is width * height * 3 */
    const BYTE* data;                   /* 可见光图像数据。get：至下次 get/关闭句柄；流回调：仅回调期间有效 / Visual image bytes (get: valid until next get/handle close; stream callback: valid during callback only) */
} net_raw_visual_frame;

typedef struct
{
    net_enum_acoustic_data_type data_types; /* 本次携带的数据类型集合 / Data types included */
    UINT64 timestamp_us;                   /* 数据时间戳，单位微秒 / Timestamp in microseconds */
    net_raw_visual_frame visual_frame;     /* 可见光图像，data_types 含 0x01 时有效 / Visual image, valid when data_types includes 0x01 */
    net_acoustic_spl_frame spl_frame;      /* 声压网格，data_types 含 0x02 时有效 / Sound-pressure grid, valid when data_types includes 0x02 */
    net_acoustic_spectrum_frame spectrum_frame; /* 频谱数据，data_types 含 0x04 时有效 / Spectrum data, valid when data_types includes 0x04 */
    net_acoustic_spl_info global_spl_info; /* 全局声压统计，data_types 含 0x08 时有效 / Global sound-pressure statistics, valid when data_types includes 0x08 */
} net_acoustic_data;

typedef struct
{
    net_enum_acoustic_data_type data_types; /* 输出的数据类型 / Output data types */
    UINT32 sample_interval;                 /* 采样间隔，单位毫秒，最小 100 / Sample interval in ms, minimum 100 */
} net_acoustic_data_config;

typedef struct
{
    USHORT count;                    /* 坏点数量 / Dead pixel count */
    net_point points[NET_LEN_64];    /* 坏点坐标列表 / Dead pixel coordinate list */
} net_dpc_params;

typedef struct
{
    BYTE trigger_ftp_enabled;           /* 是否触发 FTP 上传 / FTP upload trigger enabled */
    BYTE trigger_webhook_enabled;       /* 是否触发 Webhook 上传 / Webhook trigger enabled */
} net_alarm_trigger;

typedef struct
{
    BYTE start_time[NET_LEN_16];        /* 开始时间，格式 HH:mm:ss / Start time, format HH:mm:ss */
    BYTE end_time[NET_LEN_16];          /* 结束时间，格式 HH:mm:ss / End time, format HH:mm:ss */
} net_alarm_schedule_period;

typedef struct
{
    BYTE weekday;                                                /* 星期几，1=Monday ... 7=Sunday / Weekday, 1=Monday ... 7=Sunday */
    USHORT periods_count;                                        /* 时段数量 / Number of periods */
    net_alarm_schedule_period periods[NET_MAX_SCHEDULE_PERIODS]; /* 当天布防时段列表 / Schedule periods for the day */
} net_alarm_schedule_day;

typedef struct
{
    USHORT days_count;                                           /* 天数 / Number of configured days */
    net_alarm_schedule_day days[NET_MAX_SCHEDULE_DAYS];          /* 周计划 / Weekly schedule */
} net_alarm_schedule;

typedef struct
{
    BYTE ip[NET_LEN_64];                          /* 设备 IP 地址 / Device IP address */
    USHORT port_no;                               /* 设备端口号 / Device port number */
    UINT32 channel_id;                            /* 报警通道号 / Alarm channel ID */
} net_alarm_device_info;

typedef struct
{
    UINT64 timestamp_us;                          /* 报警数据时间戳，单位微秒 / Alarm data timestamp in microseconds */
    net_alarm_device_info dev_info;               /* 报警来源设备信息 / Alarm source device info */
    net_raw_frame raw_frame;                      /* 原始帧数据 / Raw frame data */
    net_temp_point_info global_temp_info;         /* 报警时的全局温度统计信息 / Global temperature statistics at alarm time */
    USHORT marker_alarms_count;                   /* marker_alarms 有效个数（可含 global） / Valid marker_alarms count (may include global) */
    net_marker_alarm_info marker_alarms[NET_MAX_MARKERS];         /* 各标识报警状态（可含 global） / Alarm status for each marker (may include global) */
    USHORT markers_count;                         /* marker_temps 有效个数（不含 global） / Valid marker_temps count (excludes global) */
    net_marker_temp_info marker_temps[NET_MAX_MARKERS];           /* 报警时的标识温度统计信息 / Marker temperature statistics at alarm time */
    UINT32 message_length;                        /* 报警消息长度 / Alarm message length */
    BYTE* message;                                /* 报警消息 / Alarm message */
} net_alarm_thermal_temp_info;

typedef struct
{
    UINT64 timestamp_us;                          /* 报警数据时间戳，单位微秒 / Alarm data timestamp in microseconds */
    net_alarm_device_info dev_info;               /* 报警来源设备信息 / Alarm source device info */
    INT32 motion_level;                           /* 入侵运动量 0–100 / Intrusion motion level, 0-100 */
    UINT32 message_length;                        /* 报警消息长度 / Alarm message length */
    BYTE* message;                                /* 报警消息 / Alarm message */
} net_alarm_intrusion_info;

typedef struct
{
    UINT64 timestamp_us;                          /* 报警数据时间戳，单位微秒 / Alarm data timestamp in microseconds */
    net_alarm_device_info dev_info;               /* 报警来源设备信息 / Alarm source device info */
    float temperature;                            /* 触发报警的温度 / Temperature that triggered the alarm */
    float threshold;                              /* 报警阈值温度 / Alarm threshold temperature */
    UINT32 message_length;                        /* 报警消息长度 / Alarm message length */
    BYTE* message;                                /* 报警消息 / Alarm message */
} net_alarm_high_temp_info;

typedef struct
{
    UINT64 timestamp_us;                          /* 报警数据时间戳，单位微秒 / Alarm data timestamp in microseconds */
    net_alarm_device_info dev_info;               /* 报警来源设备信息 / Alarm source device info */
    float temperature;                            /* 监测温度 / Detected temperature */
    float baseline;                               /* 温度基线 / Temperature baseline */
    float slope;                                  /* 温度斜率 / Temperature slope */
    UINT32 message_length;                        /* 报警消息长度 / Alarm message length */
    BYTE* message;                                /* 报警消息 / Alarm message */
} net_alarm_temp_detect_info;

typedef struct
{
    UINT64 timestamp_us;                          /* 报警数据时间戳，单位微秒 / Alarm data timestamp in microseconds */
    net_alarm_device_info dev_info;               /* 报警来源设备信息 / Alarm source device info */
    UINT32 duration;                              /* 声源持续时长，单位秒 / Source duration in seconds */
    UINT32 tolerance_pixels;                      /* 容差像素 / Tolerance in pixels */
    net_point point;                              /* 声源坐标 / Sound-source coordinate */
    UINT32 message_length;                        /* 报警消息长度 / Alarm message length */
    BYTE* message;                                /* 报警消息 / Alarm message */
} net_alarm_acoustic_source_info;

typedef struct
{
    UINT64 timestamp_us;                          /* 报警数据时间戳，单位微秒 / Alarm data timestamp in microseconds */
    net_alarm_device_info dev_info;               /* 报警来源设备信息 / Alarm source device info */
    float db;                                     /* 触发报警的声压级，单位 dB / Sound pressure level that triggered the alarm, in dB */
    float db_threshold;                           /* 分贝阈值，取值范围 -100~1000 / Decibel threshold, range -100~1000 */
    UINT32 message_length;                        /* 报警消息长度 / Alarm message length */
    BYTE* message;                                /* 报警消息 / Alarm message */
} net_alarm_acoustic_db_info;

typedef struct
{
    UINT64 timestamp_us;                          /* 报警数据时间戳，单位微秒 / Alarm data timestamp in microseconds */
    net_alarm_device_info dev_info;               /* 报警来源设备信息 / Alarm source device info */
    float frequency;                              /* 触发报警的频率，单位 Hz / Frequency that triggered the alarm, in Hz */
    float db;                                     /* 触发报警的声压级，单位 dB / Sound pressure level that triggered the alarm, in dB */
    float db_threshold;                           /* 分贝阈值，取值范围 -100~1000 / Decibel threshold, range -100~1000 */
    UINT32 message_length;                        /* 报警消息长度 / Alarm message length */
    BYTE* message;                                /* 报警消息 / Alarm message */
} net_alarm_acoustic_freq_info;

typedef struct
{
    BYTE serial_no[NET_LEN_64];             /* 设备序列号 / Device serial number */
    BYTE device_name[NET_LEN_128];          /* 设备名称 / Device name */
    USHORT link_port;                       /* 报警上传端口 / Alarm upload port */
    BYTE device_ip[NET_LEN_128];            /* 设备 IP / Device IP */
} net_alarmer;

/**
 * type 决定 alarm_info 指向的结构体 / type selects the alarm_info payload:
 *
 * 1. 测温报警（与 marker 相关）/ Thermal temperature alarm (marker-based):
 *    NET_ALARM_THERMAL_TEMP    -> net_alarm_thermal_temp_info
 *
 * 2. 智能报警（无 marker）/ Smart alarms (not marker-based):
 *    NET_ALARM_INTRUSION       -> net_alarm_intrusion_info
 *    NET_ALARM_HIGH_TEMP       -> net_alarm_high_temp_info
 *    NET_ALARM_TEMP_DETECT     -> net_alarm_temp_detect_info
 *
 * 3. 声像报警 / Acoustic alarms:
 *    NET_ALARM_ACOUSTIC_SOURCE -> net_alarm_acoustic_source_info
 *    NET_ALARM_ACOUSTIC_DB     -> net_alarm_acoustic_db_info
 *    NET_ALARM_ACOUSTIC_FREQ   -> net_alarm_acoustic_freq_info
 *
 * message_length 为 0 时 message 为 NULL。
 * 实时回调中 alarm_info 与 message 仅在回调期间有效 / In the live callback, alarm_info and message are only valid during the callback.
 */
typedef struct
{
    net_enum_alarm_type type;                          /* 报警类型 / Alarm type */
    net_alarmer alarmer;                               /* 报警来源，由 RTP 报警帧下发 / Alarm source from the RTP alarm frame */
    UINT32 alarm_info_length;                          /* 报警信息长度 / Alarm info length */
    BYTE* alarm_info;                                  /* 类型相关报警详情 / Type-specific alarm payload */
    UINT32 alarm_id;                                   /* 报警记录 ID / Alarm record ID */
} net_alarm_info;

typedef enum
{
    NET_STORAGE_NORMAL = 0,         /* 正常：/dev/mmcblk0p1 已挂到 /userdata/parkia/record / Normal: mmcblk0p1 mounted at /userdata/parkia/record */
    NET_STORAGE_UNFORMATTED = 1,    /* 未格式化：有块设备，但 mount -t vfat 失败（没分区、不是 FAT） / Unformatted: block device present but vfat mount failed (no partition or not FAT) */
    NET_STORAGE_ERROR = 2           /* 错误：有卡但异常（意外只读、读写失败、格式化失败） / Error: card present but abnormal (unexpected RO, I/O fail, format fail) */
} net_enum_storage_status;

typedef enum
{
    NET_STORAGE_RW = 0,             /* 可读写 / Read-write */
    NET_STORAGE_READONLY = 1        /* 只读 / Read-only */
} net_enum_storage_attr;

typedef struct
{
    UINT32 id;                              /* 存储卷编号 / Storage volume number */
    BYTE name[NET_LEN_64];                  /* 卷名称 / Volume name */
    UINT32 capacity_mb;                     /* 总容量，单位 MB / Total capacity in MB */
    UINT32 free_mb;                          /* 剩余容量，单位 MB / Free space in MB */
    net_enum_storage_status status;         /* 卷状态 / Volume status */
    net_enum_storage_attr attr;             /* 卷属性 / Volume attribute */
    BYTE recycling;                         /* 是否循环覆盖，0/1 / Recycle when full, 0/1 */
} net_storage_info;

typedef struct
{
    UINT32 alarm_type;                  /* 报警类型，0 表示全部；非 0 时取值同 net_enum_alarm_type / Alarm type, 0 means all; non-zero values match net_enum_alarm_type */
    UINT32 channel_id;                  /* 通道号，0 表示全部通道 / Channel ID, 0 means all channels */
    UINT64 start_timestamp_us;          /* 起始时间戳，单位微秒 / Timestamp in microseconds */
    UINT64 stop_timestamp_us;           /* 结束时间戳，单位微秒 / Timestamp in microseconds */
    UINT32 page_no;                 /* 页码，从 0 开始 / Page number, starting at 0 */
    UINT32 page_size;                 /* 本页条数上限，0 表示按调用方缓冲区长度取页 / Maximum items in this page; 0 uses the caller buffer length */
} net_search_alarm_param;

typedef struct
{
    UINT32 total;                       /* 符合条件的总条数 / Total number of matches */
    UINT32 page_no;                 /* 页码，回显请求 / Page number echoed from the request */
    UINT32 count;                       /* 本页实际条数 / Number of items in this page */
} net_search_page;

typedef struct
{
    UINT32 alarm_id;                    /* 报警编号 / Alarm ID */
    UINT32 channel_id;                  /* 通道号 / Channel ID */
    UINT64 timestamp_us;                /* 报警时间戳，单位微秒 / Alarm timestamp in microseconds */
    UINT32 alarm_type;                  /* 命中的报警类型，取值同 net_enum_alarm_type。1=测温(marker)，2/3/4=入侵/高温/温度监测，5/6/7=声像 / Matched alarm type. 1=thermal temp (marker), 2/3/4=intrusion/high temp/temp detect, 5/6/7=acoustic */
    UINT32 alarm_info_length;           /* 报警信息长度 / Alarm info length */
    BYTE* alarm_info;                   /* 按 alarm_type 解释，与 net_alarm_info.alarm_info 相同。有效到下一次 find 或 logout / Typed by alarm_type, same mapping as net_alarm_info.alarm_info. Valid until the next find or logout */
    BYTE record_path[NET_LEN_128];      /* 录像文件路径，无则空字符串；可与图像、数据文件同时存在 / Recording path, empty if none; may coexist with the image and data files */
    BYTE snapshot_path[NET_LEN_128];    /* 截图文件路径，无则空字符串；可与录像、数据文件同时存在 / Snapshot path, empty if none; may coexist with the recording and data files */
    BYTE data_path[NET_LEN_128];        /* 数据文件路径，无则空字符串；可与录像、图像文件同时存在 / Data file path, empty if none; may coexist with the recording and image files */
} net_search_alarm_data;

typedef struct
{
    USHORT length;                      /* datas 可容纳的条数。为 0 时只返回 total / Capacity of datas. 0 returns total only */
    net_search_alarm_data* datas;       /* 本页结果缓冲区，由调用方分配。length 为 0 时可为 NULL / Caller-allocated buffer for this page. May be NULL when length is 0 */
    net_search_page* page;        /* 分页信息。count 为本页条数，total 为总条数 / Page info. count is this page; total is the full match count */
} net_search_alarm_result;

typedef enum
{
    NET_FORMAT_IN_PROGRESS = 0,     /* 正在格式化 / Formatting */
    NET_FORMAT_FINISHED = 1,        /* 格式化完成 / Format finished */
    NET_FORMAT_ERROR = 2,           /* 格式化出错，网络或硬盘异常 / Format error: network or disk fault */
    NET_FORMAT_INTERRUPTED = 3      /* 格式化中断 / Format interrupted */
} net_enum_format_status;

typedef struct
{
    INT32 disk_number;                  /* 当前正在格式化的硬盘号，从 0 开始；尚未选中硬盘时为 -1 / Disk being formatted, starting at 0; -1 before a disk is selected */
    INT32 progress;                     /* 当前硬盘进度，0–100 / Progress of the current disk, 0–100 */
    net_enum_format_status status;      /* 格式化状态 / Format status */
} net_format_progress;

typedef void(IRTEK_NETSDK_CALL* on_net_exception_cb)(const net_exception_info* exception_info);
typedef void(IRTEK_NETSDK_CALL* on_net_alarm_cb)(const net_alarm_info* alarm_info);
typedef void(IRTEK_NETSDK_CALL* on_net_stream_received_cb)(const net_stream_data* stream_data);
typedef void(IRTEK_NETSDK_CALL* on_net_thermal_data_stream_received_cb)(const net_thermal_data* frame);
typedef void(IRTEK_NETSDK_CALL* on_net_acoustic_data_stream_received_cb)(const net_acoustic_data* frame);

/* SDK 生命周期 / SDK lifecycle */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_sdk_init(void); /* 初始化 SDK / Initialize SDK */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_sdk_cleanup(void); /* 释放 SDK / Cleanup SDK */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_enable_log(BYTE enable); /* 启停 SDK 日志 / Enable/disable SDK logging */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_login(const net_login_info* info, NET_HANDLE* handle); /* 设备登录 / Device login */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_logout(NET_HANDLE* handle); /* 设备注销 / Device logout */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_sdk_version(net_sdk_version* version); /* 获取 SDK 版本信息 / Get SDK version */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_exception_callback(NET_HANDLE handle, on_net_exception_cb cb); /* 设置异常回调 / Set exception callback */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_start_alarm_listener(NET_HANDLE handle, on_net_alarm_cb cb); /* 开启报警监听 / Start alarm listener */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_stop_alarm_listener(NET_HANDLE alarm_listener_handle); /* 停止报警监听，alarm_listener_handle 为开启报警监听时返回的句柄 / Stop alarm listener; alarm_listener_handle is the handle returned by net_start_alarm_listener */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_alarm_trigger(NET_HANDLE handle, const net_enum_alarm_type* type, net_alarm_trigger* trigger); /* 获取报警联动，type 支持全部报警类型 / Get alarm trigger; type accepts every alarm type */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_alarm_trigger(NET_HANDLE handle, const net_enum_alarm_type* type, const net_alarm_trigger* trigger); /* 设置报警联动，type 支持全部报警类型 / Set alarm trigger; type accepts every alarm type */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_alarm_schedule(NET_HANDLE handle, const net_enum_alarm_type* type, net_alarm_schedule* schedule); /* 获取报警布防计划 / Get alarm schedule */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_alarm_schedule(NET_HANDLE handle, const net_enum_alarm_type* type, const net_alarm_schedule* schedule); /* 设置报警布防计划 / Set alarm schedule */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_search_devices(net_discovered_device* devices, USHORT length, USHORT* out_length, UINT32 timeout_ms); /* 搜索设备 / Search devices */

/* 设备管理 / Device management */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_device_info(NET_HANDLE handle, net_device_info* info); /* 获取设备信息 / Get device info */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_device_info(NET_HANDLE handle, const net_device_info* info); /* 设置设备信息 / Set device info */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_device_restart(NET_HANDLE handle); /* 设备重启 / Restart device */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_device_reset(NET_HANDLE handle); /* 设备重置 / Factory reset device */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_device_upgrade(NET_HANDLE handle, const net_upgrade_info* info); /* 远程升级 / Remote upgrade */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_ntp_server(NET_HANDLE handle, net_ntp_info* info); /* 获取 NTP 服务 / Get NTP server */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_ntp_server(NET_HANDLE handle, const net_ntp_info* info); /* 设置 NTP 服务 / Set NTP server */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_time(NET_HANDLE handle, net_time_info* info); /* 获取设备时间 / Get device time */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_time(NET_HANDLE handle, const net_time_info* info); /* 设置设备时间 / Set device time */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_port(NET_HANDLE handle, net_port_info* info); /* 获取设备服务端口 / Get device service ports */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_port(NET_HANDLE handle, const net_port_info* info); /* 设置设备服务端口 / Set device service ports */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_networks(NET_HANDLE handle, net_network_info* networks, USHORT length, USHORT* out_length); /* 获取网络参数列表 / Get network parameter list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_network(NET_HANDLE handle, UINT32 id, net_network_info* info); /* 获取网络参数 / Get network parameters */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_network(NET_HANDLE handle, UINT32 id, const net_network_info* info); /* 设置网络参数 / Set network parameters */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_ftp(NET_HANDLE handle, net_ftp_info* info); /* 获取 FTP 配置 / Get FTP configuration */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_ftp(NET_HANDLE handle, const net_ftp_info* info); /* 设置 FTP 配置 / Set FTP configuration */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_test_ftp(NET_HANDLE handle, const net_ftp_info* info); /* 测试 FTP 配置 / Test FTP configuration */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_webhook(NET_HANDLE handle, net_webhook_info* info); /* 获取最近一次写入列表的 Webhook 地址；尚未设置时 url 为空。该记录与列表保存在一起，默认取最新的一条数据 / Get the Webhook URL for the most recent write to the list; if it has not been set, url is empty. This record is stored together with the list, and by default the latest data entry is used. */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_webhook(NET_HANDLE handle, const net_webhook_info* info); /* 设置单条 Webhook 地址。已在列表中则直接成功，不更新最近写入记录 / Set one webhook URL. An address already in the list succeeds without updating the last-written record */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_webhooks(NET_HANDLE handle, net_webhook_info* list, USHORT length, USHORT* out_length); /* 获取 Webhook 地址列表，最多 NET_MAX_WEBHOOKS 条 / Get the webhook URL list, at most NET_MAX_WEBHOOKS entries */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_webhooks(NET_HANDLE handle, const net_webhook_info* list, USHORT length); /* 设置 Webhook 地址列表，length 为 0 表示清空，并清空最近写入记录 / Set the webhook URL list; length 0 clears the list and the last-written record */

/* 用户管理 / User management */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_users(NET_HANDLE handle, net_user_info* users, USHORT length, USHORT* out_length); /* 获取用户列表 / Get user list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_user(NET_HANDLE handle, UINT32 id, net_user_info* user); /* 获取用户信息 / Get user info */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_update_user(NET_HANDLE handle, const net_user_pwd_info* user); /* 添加或修改用户 / Add or update user */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_delete_user(NET_HANDLE handle, UINT32 id); /* 删除用户 / Delete user */

/* 视频流 / Streaming */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_streams(NET_HANDLE handle, net_stream_info* streams, USHORT length, USHORT* out_length); /* 获取视频流列表 / Get stream list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_stream(NET_HANDLE handle, UINT32 stream_id, net_stream_info* stream); /* 获取视频流参数 / Get stream parameters */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_stream_status(NET_HANDLE handle, UINT32 stream_id, net_stream_status* status); /* 获取视频流状态 / Get stream status */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_start_stream_play(NET_HANDLE handle, net_stream_info* stream, net_enum_stream_type stream_type, on_net_stream_received_cb cb); /* 开启视频流回调 / Start stream playback */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_stop_stream_play(NET_HANDLE stream_handle); /* 停止视频流回调，stream_handle 为开启视频流回调时返回的句柄 / Stop stream playback; stream_handle is the handle returned by net_start_stream_play */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_stream_callback(NET_HANDLE stream_handle, on_net_stream_received_cb cb); /* 设置视频流回调，stream_handle 为开启视频流回调时返回的句柄 / Set stream callback; stream_handle is the handle returned by net_start_stream_play */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_start_stream_record(NET_HANDLE stream_handle, const BYTE* local_file, USHORT length); /* 开始本地录制视频流，stream_handle 为开启视频流回调时返回的句柄 / Start local recording; stream_handle is the handle returned by net_start_stream_play */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_stop_stream_record(NET_HANDLE stream_handle); /* 停止本地录制视频流，stream_handle 为开启视频流回调时返回的句柄 / Stop local recording; stream_handle is the handle returned by net_start_stream_play */

/* 显示 / Display */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_channels(NET_HANDLE handle, net_channel_info* channel, USHORT length, USHORT* out_length); /* 获取通道列表 / Get channel list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_channel(NET_HANDLE handle, UINT32 channel_id, net_channel_info* channel); /* 获取通道信息 / Get channel info */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_channel(NET_HANDLE handle, UINT32 channel_id, const net_channel_info* channel); /* 设置通道信息 / Set channel info */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_overlay(NET_HANDLE handle, UINT32 channel_id, net_overlay_info* overlay); /* 获取字符叠加 / Get text overlay */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_overlay(NET_HANDLE handle, UINT32 channel_id, const net_overlay_info* overlay); /* 设置字符叠加 / Set text overlay */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_image_params(NET_HANDLE handle, UINT32 channel_id, net_image_params* params); /* 获取图像参数 / Get image parameters */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_image_params(NET_HANDLE handle, UINT32 channel_id, const net_image_params* params); /* 设置图像参数 / Set image parameters */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_image_flip(NET_HANDLE handle, UINT32 channel_id, net_enum_image_flip_mode* mode); /* 获取图像翻转模式 / Get image flip mode */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_image_flip(NET_HANDLE handle, UINT32 channel_id, const net_enum_image_flip_mode* mode); /* 设置图像翻转模式 / Set image flip mode */

/* 热成像 / Thermal */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_lens_info(NET_HANDLE handle, net_thermal_lens_info* info); /* 获取热像镜头信息 / Get thermal lens info */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_thermal_calibrate(NET_HANDLE handle); /* 执行热像校正 / Perform thermal calibration */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_shutter_mode(NET_HANDLE handle, net_enum_shutter_mode* mode); /* 获取热像校正模式 / Get thermal shutter mode */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_shutter_mode(NET_HANDLE handle, const net_enum_shutter_mode* mode); /* 设置热像校正模式 / Set thermal shutter mode */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_shutter_time(NET_HANDLE handle, USHORT* minutes); /* 获取热像自动校正时间 / Get thermal auto-calibration interval */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_shutter_time(NET_HANDLE handle, const USHORT* minutes); /* 设置挡片自动校正时间 / Set shutter auto-calibration interval */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_resolution(NET_HANDLE handle, net_size* size); /* 获取热像分辨率 / Get thermal resolution */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_range(NET_HANDLE handle, USHORT* range_id); /* 获取热像测温量程 / Get thermal temperature range */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_range(NET_HANDLE handle, const USHORT* range_id); /* 设置热像测温量程 / Set thermal temperature range */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_lut(NET_HANDLE handle, net_lut_table* lut); /* 获取热像 LUT 表 / Get thermal LUT table */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_params(NET_HANDLE handle, net_thermal_params* params); /* 获取热像测温参数 / Get thermal measurement parameters */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_params(NET_HANDLE handle, const net_thermal_params* params); /* 设置热像测温参数 / Set thermal measurement parameters */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_color_dist(NET_HANDLE handle, net_enum_color_dist_type* type); /* 获取热像色彩分布类型 / Get thermal color distribution type */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_color_dist(NET_HANDLE handle, const net_enum_color_dist_type* type); /* 设置热像色彩分布类型 / Set thermal color distribution type */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_gain(NET_HANDLE handle, net_gain* gain); /* 获取热像温宽模式 / Get thermal gain mode */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_gain(NET_HANDLE handle, const net_gain* gain); /* 设置热像温宽模式 / Set thermal gain mode */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_preset_palettes(NET_HANDLE handle, net_preset_palette* palettes, USHORT length, USHORT* out_length); /* 获取热像调色板列表 / Get thermal preset palette list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_palette(NET_HANDLE handle, net_palette* palette); /* 获取热像调色板 / Get thermal palette */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_palette(NET_HANDLE handle, const net_palette* palette); /* 设置热像调色板 / Set thermal palette */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_overlay(NET_HANDLE handle, net_thermal_overlay_info* overlay); /* 获取热像叠加 / Get thermal overlay */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_overlay(NET_HANDLE handle, const net_thermal_overlay_info* overlay); /* 设置热像叠加 / Set thermal overlay */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_unit(NET_HANDLE handle, net_unit_info* info); /* 获取热像单位 / Get thermal unit */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_unit(NET_HANDLE handle, const net_unit_info* info); /* 设置热像单位 / Set thermal unit */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_capture(NET_HANDLE handle, net_enum_thermal_capture_mode mode, net_capture_file* capture_file); /* 获取热像抓拍文件 / Get thermal capture file */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_markers(NET_HANDLE handle, net_marker_info* markers, USHORT length, USHORT* out_length); /* 获取热像测温标识列表 / Get thermal marker list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_markers(NET_HANDLE handle, const net_marker_info* markers, USHORT length); /* 设置热像测温标识列表 / Set thermal marker list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_temp_alarms(NET_HANDLE handle, net_thermal_temp_alarm* alarms, USHORT length, USHORT* out_length); /* 获取热像测温报警列表 / Get thermal temperature alarm list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_temp_alarms(NET_HANDLE handle, const net_thermal_temp_alarm* alarms, USHORT length); /* 设置热像测温报警列表 / Set thermal temperature alarm list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_temp_alarm_interval(NET_HANDLE handle, net_thermal_temp_alarm_interval* interval); /* 获取热像测温报警时间间隔 / Get thermal alarm interval */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_temp_alarm_interval(NET_HANDLE handle, const net_thermal_temp_alarm_interval* interval); /* 设置热像测温报警时间间隔 / Set thermal alarm interval */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_intrusion_alarm(NET_HANDLE handle, net_intrusion_alarm* alarm); /* 获取入侵报警 / Get intrusion alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_intrusion_alarm(NET_HANDLE handle, const net_intrusion_alarm* alarm); /* 设置入侵报警 / Set intrusion alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_high_temp_alarm(NET_HANDLE handle, net_high_temp_alarm* alarm); /* 获取高温报警 / Get high temperature alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_high_temp_alarm(NET_HANDLE handle, const net_high_temp_alarm* alarm); /* 设置高温报警 / Set high temperature alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_temp_detect_alarm(NET_HANDLE handle, net_temp_detect_alarm* alarm); /* 获取温度监测 / Get temperature detection alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_temp_detect_alarm(NET_HANDLE handle, const net_temp_detect_alarm* alarm); /* 设置温度监测 / Set temperature detection alarm */

IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_thermal_dpc(NET_HANDLE handle, const net_dpc_params* param); /* 设置热像坏点 / Set thermal dead pixel correction */

IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_start_thermal_data_stream(NET_HANDLE handle, const net_thermal_data_config* config, on_net_thermal_data_stream_received_cb cb); /* 开启热像数据流回调，配置为临时配置，不会保存到设备 / Start thermal data stream callback; config is temporary and not persisted to device */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_stop_thermal_data_stream(NET_HANDLE thermal_data_handle); /* 停止热像数据流回调，thermal_data_handle 为开启热像数据流回调时返回的句柄 / Stop thermal data stream; thermal_data_handle is the handle returned by net_start_thermal_data_stream */

IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_thermal_data(NET_HANDLE handle, const UINT32 data_types, net_thermal_data* realdata); /* 获取热像数据 / Get thermal data snapshot */

/* 声像 / Acoustic */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_sensor_info(NET_HANDLE handle, net_acoustic_sensor_info* info); /* 获取声像传感器信息 / Get acoustic sensor info */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_freq_range(NET_HANDLE handle, net_acoustic_freq_range* range); /* 获取声像频率范围 / Get acoustic frequency range */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_acoustic_freq_range(NET_HANDLE handle, const net_acoustic_freq_range* range); /* 设置声像频率范围 / Set acoustic frequency range */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_db_scale(NET_HANDLE handle, net_acoustic_db_scale* scale); /* 获取声像图像模式 / Get acoustic image mode */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_acoustic_db_scale(NET_HANDLE handle, const net_acoustic_db_scale* scale); /* 设置声像图像模式 / Set acoustic image mode */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_overlay(NET_HANDLE handle, net_acoustic_overlay_info* overlay); /* 获取声像叠加 / Get acoustic overlay */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_acoustic_overlay(NET_HANDLE handle, const net_acoustic_overlay_info* overlay); /* 设置声像叠加 / Set acoustic overlay */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_capture(NET_HANDLE handle, net_enum_acoustic_capture_mode mode, net_capture_file* capture_file); /* 获取声像抓拍文件 / Get acoustic capture file */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_source_alarm(NET_HANDLE handle, net_acoustic_source_alarm* alarm); /* 获取声源报警 / Get acoustic source alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_acoustic_source_alarm(NET_HANDLE handle, const net_acoustic_source_alarm* alarm); /* 设置声源报警 / Set acoustic source alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_db_alarm(NET_HANDLE handle, net_acoustic_db_alarm* alarm); /* 获取分贝报警 / Get acoustic decibel alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_acoustic_db_alarm(NET_HANDLE handle, const net_acoustic_db_alarm* alarm); /* 设置分贝报警 / Set acoustic decibel alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_freq_alarm(NET_HANDLE handle, net_acoustic_freq_alarm* alarm); /* 获取频率报警 / Get acoustic frequency alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_acoustic_freq_alarm(NET_HANDLE handle, const net_acoustic_freq_alarm* alarm); /* 设置频率报警 / Set acoustic frequency alarm */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_acoustic_data(NET_HANDLE handle, const UINT32 data_types, net_acoustic_data* data); /* 获取声像实时数据 / Get acoustic data snapshot */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_start_acoustic_data_stream(NET_HANDLE handle, const net_acoustic_data_config* config, on_net_acoustic_data_stream_received_cb cb); /* 开启声像数据流回调，配置为临时配置，不会保存到设备 / Start acoustic data stream callback; config is temporary and not persisted to device */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_stop_acoustic_data_stream(NET_HANDLE acoustic_data_handle); /* 停止声像数据流回调，acoustic_data_handle 为开启声像数据流回调时返回的句柄 / Stop acoustic data stream; acoustic_data_handle is the handle returned by net_start_acoustic_data_stream */

/* 存储 / Storage */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_storages(NET_HANDLE handle, net_storage_info* storages, USHORT length, USHORT* out_length); /* 获取存储卷列表 / Get storage volume list */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_set_storage(NET_HANDLE handle, UINT32 id, const net_storage_info* info); /* 设置存储卷。id 与 net_storage_info.id 相同。只写入 name、attr、recycling；capacity_mb、free_mb、status 为设备状态，设置时忽略 / Set a storage volume. id matches net_storage_info.id. Only name, attr, and recycling are written; capacity_mb, free_mb, and status are device state and are ignored */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_find_alarm_datas(NET_HANDLE handle, const net_search_alarm_param* param, net_search_alarm_result* result); /* 按报警分页查找数据。result->datas 写入本页，条数不超过 result->length 与 page_size；length 为 0 时 datas 可为 NULL，只返回 total。page->count 为本页条数，page->total 为总条数 / Find alarm datas one page at a time. result->datas receives this page, limited by result->length and page_size. length 0 allows a NULL datas pointer and returns total only. page->count is this page; page->total is the full match count */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_format_disk(NET_HANDLE handle, UINT32 disk_number, NET_HANDLE* format_handle); /* 远程格式化硬盘。disk_number 从 0 开始，与 net_storage_info.id 相同；NET_DISK_ALL(0xFF) 格式化全部可写硬盘并跳过只读硬盘。format_handle 用于查询进度和关闭 / Remotely format a disk. disk_number starts at 0 and matches net_storage_info.id; NET_DISK_ALL (0xFF) formats every writable disk and skips read-only disks. format_handle is used to query progress and to close */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_get_format_progress(NET_HANDLE format_handle, net_format_progress* progress); /* 获取格式化进度 / Get format progress */
IRTEK_NETSDK_API INT32 IRTEK_NETSDK_CALL net_close_format_handle(NET_HANDLE format_handle); /* 关闭格式化句柄并释放资源，不撤销设备上已开始的格式化 / Close the format handle and release it. A format already started on the device keeps running */

#ifdef __cplusplus
}
#endif
