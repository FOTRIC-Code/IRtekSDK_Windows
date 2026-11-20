/*
 * IRtekACFileSDK 3.0
 * Copyright 2021, fotric.cn
 *
 * Library for IRTek AC file format
*/
#ifndef IRTEKACFILESDK_H_INCLUDED
#define IRTEKACFILESDK_H_INCLUDED

#ifdef _WIN32
#ifdef IRTEKACFILESDK_EXPORTS
#ifdef __cplusplus
#define AC_FILE_API extern "C" __declspec(dllexport)
#else
#define AC_FILE_API __declspec(dllexport)
#endif
#else
#ifdef __cplusplus
#define AC_FILE_API extern "C" __declspec(dllimport)
#else
#define AC_FILE_API __declspec(dllexport)
#endif
#endif
#else
#ifdef __cplusplus
#define AC_FILE_API extern "C"
#else
#define AC_FILE_API
#endif
#endif

#pragma pack(push, 8)

typedef void*               HANDLE; /**< AC_FILE句柄 */
typedef unsigned char       BYTE;   /**< 单字节或布尔值 */
typedef short               SHORT;
typedef unsigned short      USHORT;
typedef int                 INT32;
typedef unsigned int        UINT32;
typedef unsigned long long  UINT64;

/** 所有函数的返回值 */
typedef enum 
{
    AC_FILE_EC_OK = 0,              /**< 函数调用成功 */
    AC_FILE_EC_FAIL = -1,           /**< 函数调用失败 */
    AC_FILE_EC_MEMORY = -2,         /**< 内存分配失败 */
    AC_FILE_EC_NULL_POINTER = -3,   /**< 空指针 */
    AC_FILE_EC_INVALID_KEY = -4,    /**< 索引不存在 */
    AC_FILE_EC_NOT_SUPPORTED = -5   /**< 功能不支持 */
} ac_file_enum_error_code;

/** 点 */
typedef struct
{
    SHORT x;                        /**< 点的X轴坐标 */
    SHORT y;                        /**< 点的Y轴坐标 */
} ac_file_point;

/** 归一化浮点坐标 */
typedef struct
{
    float x;                        /**< 点的X轴相对坐标 */
    float y;                        /**< 点的Y轴相对坐标 */
} ac_file_point_f;

/** 矩形 */
typedef struct
{
    SHORT x;                        /**< 矩形左上角点的X轴坐标 */
    SHORT y;                        /**< 矩形左上角点的Y轴坐标 */
    SHORT width;                    /**< 矩形的长度 */
    SHORT height;                   /**< 矩形的宽度 */
} ac_file_rectangle;

/** 归一化浮点矩形 */
typedef struct
{
    float x;                        /**< 矩形左上角点的X轴相对坐标 */
    float y;                        /**< 矩形左上角点的Y轴相对坐标 */
    float width;                    /**< 矩形的相对长度 */
    float height;                   /**< 矩形的相对宽度 */
} ac_file_rectangle_f;

/** 缓存区 */
typedef struct
{
    UINT32 length;                  /**< 缓存区长度 */
    BYTE* buffer;                   /**< 字节数组首地址 */
} ac_file_buffer;

/** 获取SDK版本信息
* @param ver    [out] SDK版本号，四字节形式，如0x02040100为2.4.1.0。若为空，则
*                     不读取。
* @param str    [out] SDK版本号，字符串形式。若为空，则不读取。
*/
AC_FILE_API INT32 ac_file_get_sdk_version(UINT32* ver, ac_file_buffer* str);

/** 声像文件类型 */
typedef enum 
{
    AC_FILE_OTHER = 0,              /**< 非声像文件 */
    AC_FILE_PICTURE = 1,            /**< 声像图片 */
    AC_FILE_VIDEO = 2,              /**< 声像视频 */
} ac_file_enum_file_type;

/** 检查文件是否为声像格式
* @param file_name [in] 文件路径。
* @param value     [out] 声像文件类型。
*/
AC_FILE_API INT32 ac_file_peek_file_type(const char* file_name, ac_file_enum_file_type* value);

/** 打开声像文件
* 若文件不存在则失败。
* @param file_name [in] 文件路径。
* @param h         [out] AC_FILE句柄。
*/
AC_FILE_API INT32 ac_file_open(const char* file_name, HANDLE* h);

/** 创建声像文件
* 若文件已存在将被覆盖。
* @param file_name [in] 文件路径。
* @param file_type [in] 文件类型。可创建类型为：
*                       AC_FILE_PICTURE，
*                       AC_FILE_VIDEO。
* @param h         [out] AC_FILE句柄。
*/
AC_FILE_API INT32 ac_file_create(const char* file_name, ac_file_enum_file_type file_type, HANDLE* h);

/** 保存声像文件
* @param  h [in] AC_FILE句柄。
*/
AC_FILE_API INT32 ac_file_save(HANDLE h);

/** 声像文件另存为
* @param  h         [in] AC_FILE句柄。
* @param file_name  [in] 文件路径。
*/
AC_FILE_API INT32 ac_file_save_as(HANDLE h, const char* file_name);

/** 关闭声像文件 */
AC_FILE_API INT32 ac_file_close(HANDLE h);

/** 获取声像文件版本号
* @param h      [in]  AC_FILE句柄
* @param ver    [out] 文件版本号
*/
AC_FILE_API INT32 ac_file_get_file_version(HANDLE h, USHORT* value);

/** 读声像文件名
* @param h      [in] AC_FILE句柄。
* @param value  [out] 声像文件名。buffer需要用户分配内存，若buffer为空，不读取
*                     备注信息，length返回所需内存大小。
*/
AC_FILE_API INT32 ac_file_get_file_name(HANDLE h, ac_file_buffer* value);

/** 读声像文件类型
* @param h      [in] AC_FILE句柄。
* @param value  [out] 声像文件类型。
*/
AC_FILE_API INT32 ac_file_get_file_type(HANDLE h, ac_file_enum_file_type* value);

/** 设备信息 */
typedef struct
{
    BYTE model[32];                 /**< 设备型号 */
    BYTE serial_no[48];             /**< 设备序列号 */
    BYTE brand[32];                 /**< 品牌 */
    BYTE company[64];               /**< 厂商 */
    UINT32 logo_length;             /**< 图片大小 */
    BYTE* logo;                     /**< logo图片，jpg文件，字节数组首地址 */
} ac_file_camera_info;

/** 读设备信息
* @param h      [in] AC_FILE句柄。
* @param value  [out] 设备信息。logo需要用户分配内存，若logo为空，不 读取logo图
*                     片，logoLength返回所需内存大小。
*/
AC_FILE_API INT32 ac_file_get_camera_info(HANDLE h, ac_file_camera_info* value);

/** 写设备信息
* @param h      [in] AC_FILE句柄。
* @param value  [in] 设备信息。若logo为空，不写入logo图片。若value为空，清空设
*                    备信息。
*/
AC_FILE_API INT32 ac_file_set_camera_info(HANDLE h, const ac_file_camera_info* value);

/** 传感器信息 */
typedef struct
{
    BYTE serial_no[48];             /**< 传感器序列号 */
    INT32 channel_count;            /**< 传感器通道数 */
    BYTE version[48];               /**< 传感器固件版本号 */
    INT32 ref_channel;              /**< 传感器监视麦克风编号 */
    INT32 sample_rate;              /**< 传感器采样率 */
    float amplitude;                /**< 传感器声音信号的最大幅度值 */
} ac_file_sensor_info;

/** 读传感器信息
* @param h      [in] AC_FILE句柄。
* @param value  [out] 传感器信息。
*/
AC_FILE_API INT32 ac_file_get_sensor_info(HANDLE h, ac_file_sensor_info* value);

/** 写传感器信息
* @param h      [in] AC_FILE句柄。
* @param value  [in] 传感器信息。若value为空，清空镜头信息。
*/
AC_FILE_API INT32 ac_file_set_sensor_info(HANDLE h, const ac_file_sensor_info* value);

/** 频率范围 */
typedef struct
{
    float max;                      /**< 频率范围上限，取值范围0.0~1000000.0，默认值为
                                     *   100000.0 */
    float min;                      /**< 频率范围下限，取值范围0.0~1000000.0，默认值为
                                     *   5000.0 */
} ac_file_band;

/** 读频率范围
* @param h      [in] AC_FILE句柄。
* @param value  [out] 频率范围。
*/
AC_FILE_API INT32 ac_file_get_frequency_range(HANDLE h, ac_file_band* value);

/** 写频率范围
* @param h      [in] AC_FILE句柄。
* @param value  [in] 频率范围。若value为空，清空频率范围。
*/
AC_FILE_API INT32 ac_file_set_frequency_range(HANDLE h, const ac_file_band* value);

/** 调色板 */
typedef struct
{
    BYTE ref_no;                    /**< 调色板编号，0xFF表示使用自定义调色
                                     *   板 */
    UINT32 colors[256];             /**< 调色板，每个UINT32前24位分别为RGB，最
                                     *   后8位不使用 */
    BYTE alpha;                     /**< 调色板透明度，取值范围0~100，表示透明度百分比
                                        默认值为60 */
} ac_file_palette;

/** 读调色板
* @param h      [in] AC_FILE句柄。
* @param value  [out] 调色板信息。
*/
AC_FILE_API INT32 ac_file_get_palette(HANDLE h, ac_file_palette* value);

/** 写调色板
* @param h      [in] AC_FILE句柄。
* @param value  [in] 调色板信息。若value为空，清空调色板信息。
*/
AC_FILE_API INT32 ac_file_set_palette(HANDLE h, const ac_file_palette* value);

/** 图像分辨率 */
typedef struct
{
    SHORT width; 					/**< 宽度 */
    SHORT height; 					/**< 高度 */
} acoustic_size;

/** 读图像分辨率
* @param h      [in] AC_FILE句柄。
* @param size   [out] 传感器成像分辨率。
*/
AC_FILE_API INT32 ac_file_get_acoustic_size(HANDLE h, acoustic_size* value);

/** 写图像分辨率
* @param h      [in] AC_FILE句柄。
* @param size   [in] 传感器成像分辨率。若value为空，清空传感器成像分辨率。
*/
AC_FILE_API INT32 ac_file_set_acoustic_size(HANDLE h, const acoustic_size* value);

/** 声像参数 */
typedef struct
{
    UINT32 count;
    ac_file_band* bands;           	/**< 传感器成像频带的上限值，取值范围0.0~1000000.0
                                         默认值50000.0
                                         传感器成像频带的下限值，取值范围0.0~1000000.0
                                         默认值30000*/
    INT32 gain;                    	/**< 传感器成像算法增益，枚举类型。枚举值从-1~5 */
    float smooth;                  	/**< 帧间平均系数，取值范围0~1.0
                                            默认值为0 */
} ac_file_acoustic_params;

/** 读声像参数
* @param h      [in] AC_FILE句柄。
* @param value  [out] 声像参数。
*/
AC_FILE_API INT32 ac_file_get_acoustic_params(HANDLE h, ac_file_acoustic_params* value);

/** 写声像参数
* @param h      [in] AC_FILE句柄。
* @paramvalue   [in] 声像参数。若value为空，清空声像参数。
*/
AC_FILE_API INT32 ac_file_set_acoustic_params(HANDLE h, const ac_file_acoustic_params* value);

/** 声宽模式 */
typedef enum 
{
    AC_FILE_AUTO_MONO = 1,          /**< 单声源模式 */
    AC_FILE_AUTO_MULTI = 2,         /**< 多声源模式 */
    AC_FILE_FIXED = 3,              /**< 固定模式 */
    AC_FILE_AUTO_HOLO = 4,          /**< 全息模式 */
} ac_file_enum_db_scale_mode;

/** 声宽信息 */
typedef struct
{
    ac_file_enum_db_scale_mode mode; /**< 声宽模式，默认为自动模式 */
    float max;                      /**< 声宽上限值，取值范围-100~1000 */
    float min;                      /**< 声宽下限值，取值范围-100~1000 */
    BYTE fixed_span;
    float span;
} ac_file_db_scale;

/** 声宽信息V2 */
typedef struct
{
    ac_file_enum_db_scale_mode mode; /**< 声宽模式，默认为自动模式 */
    float max; /**< 声宽上限值，取值范围-100~1000 */
    float min; /**< 声宽下限值，取值范围-100~1000 */
    BYTE fixed_span;
    float span;
    float sensitivity;
    int source_size;
    float limit;
} ac_file_db_scale_v2;

/** 读声宽信息
* @param h      [in] AC_FILE句柄。
* @param value  [out]声宽信息。
*/
AC_FILE_API INT32 ac_file_get_db_scale(HANDLE h, ac_file_db_scale* value);

/** 写声宽信息
* @param h      [in] AC_FILE句柄。
* @param value  [in|out] 声宽信息。若value为空，清空声宽信息。若为自动模式，写
*                        声宽信息成功后min和max将会被更新。
*/
AC_FILE_API INT32 ac_file_set_db_scale(HANDLE h, const ac_file_db_scale* value);

/** 读声宽信息
* @param h      [in] AC_FILE句柄。
* @param value  [out]声宽信息。
*/
AC_FILE_API INT32 ac_file_get_db_scale_v2(HANDLE h, ac_file_db_scale_v2* value);

/** 写声宽信息
* @param h      [in] AC_FILE句柄。
* @param value  [in|out] 声宽信息。若value为空，清空声宽信息。若为自动模式，写
*                        声宽信息成功后min和max将会被更新。
*/
AC_FILE_API INT32 ac_file_set_db_scale_v2(HANDLE h, const ac_file_db_scale_v2* value);

/** 读检测模式
* @param h      [in] AC_FILE句柄。
* @param value  [out] 检测模式。
*/
AC_FILE_API INT32 ac_file_get_capture_mode(HANDLE h, BYTE* value);

/** 写检测模式
* @param h      [in] AC_FILE句柄。
* @param value  [in] 检测模式。
*/
AC_FILE_API INT32 ac_file_set_capture_mode(HANDLE h, BYTE value);

/** 聚焦模式 */
typedef struct
{
    BYTE enabled; 					/**< 聚焦模式启用标识：
										 1.0x01表示启用
										 2.其它表示不启用
										 默认值为0x00 */
    float scope; 					/**< 聚焦范围，取值范围0.1~1.0
										 默认值为0.5 */
} ac_file_focus_mode;

/** 读聚焦模式
* @param h      [in] AC_FILE句柄。
* @param value  [out] 聚焦模式。
*/
AC_FILE_API INT32 ac_file_get_focus_mode(HANDLE h, ac_file_focus_mode* value);

/** 写聚焦模式
* @param h      [in] AC_FILE句柄。
* @param value  [in] 聚焦模式。
*/
AC_FILE_API INT32 ac_file_set_focus_mode(HANDLE h, const ac_file_focus_mode* value);

/** 读目标距离
* @param h      [in] AC_FILE句柄。
* @param value  [out] 目标距离。
*/
AC_FILE_API INT32 ac_file_get_distance(HANDLE h, float* value);

/** 写目标距离
* @param h      [in] AC_FILE句柄。
* @param value  [in] 目标距离。
*/
AC_FILE_API INT32 ac_file_set_distance(HANDLE h, float value);

/** 读标准声压级启用标识
* @param h      [in] AC_FILE句柄。
* @param value  [out] 标准声压级启用标识。
*/
AC_FILE_API INT32 ac_file_get_normalize_distance(HANDLE h, BYTE* value);

/** 写标准声压级启用标识
* @param h      [in] AC_FILE句柄。
* @param value  [in] 标准声压级启用标识。
*/
AC_FILE_API INT32 ac_file_set_normalize_distance(HANDLE h, BYTE value);

/** 读PRPD散点图对应交流电频率
* @param h      [in] AC_FILE句柄。
* @param value  [out] PRPD散点图对应交流电频率。
*/
AC_FILE_API INT32 ac_file_get_ac_frequency(HANDLE h, float* value);

/** 写PRPD散点图对应交流电频率
* @param h      [in] AC_FILE句柄。
* @param value  [in] PRPD散点图对应交流电频率。
*/
AC_FILE_API INT32 ac_file_set_ac_frequency(HANDLE h, float value);

/** 地理位置 */
typedef struct
{
    BYTE is_gps_valid;              /**< GPS信息有效标识。0x01表示启用，其它表
                                     *   示不启用，默认值为0x00 */
    double longitude;               /**< 经度 */
    double latitude;                /**< 纬度 */
    double altitude;                /**< 海拔 */
    BYTE is_compass_valid;          /**< 指南针有效标识。0x01表示启用，其它表示
                                     *   不启用，默认值为0x00 */
    SHORT direction;                /**< 方向，为-180~180的角度值，+-180表示正
                                     *   南方向，0度表示正北，-90表示正西，+90
                                     *   表示正东。-180~180范围外的值表示无意义 */
} ac_file_geo_location;

/** 读地理位置信息
* @param h      [in] AC_FILE句柄。
* @param value  [out] 地理位置信息。
*/
AC_FILE_API INT32 ac_file_get_geo_location(HANDLE h, ac_file_geo_location* value);

/** 写地理位置信息
* @param h      [in] AC_FILE句柄。
* @param value  [in] 地理位置信息。若value为空，清空地理位置信息。
*/
AC_FILE_API INT32 ac_file_set_geo_location(HANDLE h, const ac_file_geo_location* value);

/** 读文本备注信息
* @param h      [in] AC_FILE句柄。
* @param value  [out] 文本备注信息。buffer需要用户分配内存，若buffer为空，不读
*                     取备注信息，length返回所需内存大小。
  */
AC_FILE_API INT32 ac_file_get_note(HANDLE h, ac_file_buffer* value);

/** 写文本备注信息
* @param h      [in] AC_FILE句柄。
* @param value  [in] 文本备注信息。若buffer为空，写入失败。若value为空，清空备
*                    注信息。
*/
AC_FILE_API INT32 ac_file_set_note(HANDLE h, const ac_file_buffer* value);

/** 语音文件格式 */
typedef enum 
{
    AC_FILE_WAV = 1,                /**< WAV */
    AC_FILE_MP3 = 2,                /**< MP3 */
} ac_file_enum_audio_type;

/** 音频文件 */
typedef struct
{
    ac_file_enum_audio_type type;   /**< 语音文件格式，默认为MP3 */
    UINT32 length;                  /**< 音频大小 */
    BYTE* buffer;                   /**< 字节数组首地址 */
} ac_file_audio;

/** 读音频文件
* @param h      [in] AC_FILE句柄。
* @param value  [out] 音频文件。buffer需要用户分配内存，若buffer为空，不读取音
*                     频文件，length返回所需内存大小。
*/
AC_FILE_API INT32 ac_file_get_audio(HANDLE h, ac_file_audio* value);

/** 写音频文件
* @param h      [in] AC_FILE句柄。
* @param value  [in] 音频文件。若buffer为空，写入失败。若value为空，清空音频文
*                    件。
*/
AC_FILE_API INT32 ac_file_set_audio(HANDLE h, const ac_file_audio* value);

/** 用户数据键 */
typedef BYTE ac_file_key[16];

/** 用户数据键列表 */
typedef struct
{
    UINT32 count;                   /**< 用户数据键数量 */
    ac_file_key* keys;              /**< 用户数据键数组首地址 */
} ac_file_keys;

/** 获取用户数据键列表
* @param h      [in] AC_FILE句柄。
* @param value  [out] 用户数据键列表。
*/
AC_FILE_API INT32 ac_file_get_keys(HANDLE h, ac_file_keys* value);

/** 键值对 */
typedef struct 
{
    ac_file_key key;                /**< 键 */
    UINT32 value_length;            /**< 值长度 */
    BYTE* value_buffer;             /**< 值 */
}ac_file_kvPair;

/** 读用户数据列表
* @param h      [in] AC_FILE句柄。
* @param value  [in|out] 用户数据项，key为输入项，其它为输出项。vBuffer需要用户
*                        分配内存vLength字节，若为空，不读取值，vLength返回值长
*                        度。
* @retval AC_FILE_EC_INVALID_KEY 键不存在。
*/
AC_FILE_API INT32 ac_file_get_user_data(HANDLE h, ac_file_kvPair* value);

/** 写用户数据列表
* 用户数据以键值对方式存取，其中已定义：
* 其中已定义：
* 1.收藏标签：
* Key：“FAVORITE”
* Value：收藏标签，长度为1个字节，“Y”表示收藏，其它表示正常
* 2.注释标签：
* Key：“TAG”
* Value：注释标签，若存在多个标签采用字符‘\n’分隔
* 3.分组文件名列表：
* Key：“GROUPFILES”
* Value：关联分组文件名列表，若存在多个文件采用字符‘\n’分隔
* 4.定时拍照文件名列表：
* Key：“TIMERFILE”
* Value：采用定时拍摄方式时，保存GUID和编号，GUID采用编码格式“EDDCC222-97B3-47
* 24-A4C9-742A7A28EA25”，属于同一定时拍摄的文件保持GUID不变，GUID和编号之间采
* 用‘\n’分隔符，编号采用字符串编码格式。
* 5.泄漏量评估：
* Key：“LEAK”
* Value：
* {
* Version:100,
* LeakUnit:1
* Currency:“¥”,
* EnergyCost:2.7
* GasCost:0.0
* LeakCorrection:1.0
* SpecificPower:7
* LeakLevel:5.5
* LeakRate:100
* }
* 1）泄漏量单位：该单位可以调整，用于显示泄漏量估计值。可用选项包括:
* 0x01：公制，L/min(升/分钟)
* 0x02：英制，CFM(立方英尺/分钟)。
* 2）货币单位：显示泄漏成本估计的货币，可以从可用选项中进行选择。
* 3）能源成本：要估计压缩空气泄漏的成本，须了解能源成本。默认为上海工业用电的电费(每千瓦时)。
* 4）气体成本：特殊气体的价格（每升或立方英尺的价格，根据泄漏量单位变化）。
* 5）泄漏校正：由于泄漏量取决于许多不同的因素，如果泄漏量估计值与观察的结果之间存在差异，可以通过设置泄漏校正系数来进行调整。为大于0的数值。
* 6）比功率：比功率可以衡量产生一定量的压缩空气需要多少能量。单位是
* 公制：kW/m³/min 
* 英制：kW/100CFM
* 根据泄漏量单位选择变化。默认值为7kW/m³/min，7kWh能压缩60m³的空气。
* 7）泄漏等级：根据泄漏量模型评估出来的泄漏等级。
* 8）泄漏量：根据泄漏量模型评估出来的泄漏量。单位是
* 公制：L/min 
* 英制：CFM
* 6.局放评估：
* Key：“PD”
* Value：
* {
* Version:100,
* PDProbability:[]
* }
* 1）放电类型概率使用一个长度为8的无符号整型，表示仪器诊断的放电类型概率。
* 数组元素[0]至[7]分别存储正常、尖端放电、悬浮放电、沿面放电、绝缘气隙放电（或空穴放电）、颗粒放电、外部干扰和其它共8种情况的概率，每个元素可以是整数0至100中的某数。
* 如：数组元素[3]为69，表示沿面放电的概率为69%。
* @param h      [in] AC_FILE句柄。
* @param value  [in] 用户数据项。若vBuffer为空，删除该项用户数据。若value为空，
*                    清空用户数据列表。
*/
AC_FILE_API INT32 ac_file_set_user_data(HANDLE h, const ac_file_kvPair* value);

/** 读拍摄时间
* @param h      [in] AC_FILE句柄。
* @param value  [out] 拍摄时间，采用UTC时间，从1970-01-01 00:00:00.000起的毫秒
*                     数。
*/
AC_FILE_API INT32 ac_file_get_shot_time(HANDLE h, UINT64* value);


/** 读修改时间
* @param h      [in] AC_FILE句柄。
* @param value  [out] 修改时间，采用UTC时间，从1970-01-01 00:00:00.000起的毫秒
*                     数。
*/
AC_FILE_API INT32 ac_file_get_modified_time(HANDLE h, UINT64* value);

/** 数码变倍信息 */
typedef struct
{
    BYTE enabled;                   /**< 是否启用数码变倍，0x01表示启用，其它表
                                     *   示不启用，默认值为0x00 */
    ac_file_point_f location;       /**< 左上角相对坐标，最小相对坐标为(0.0,0.0)，
                                     *   最大相对坐标为(1.0,1.0)，默认值为
                                     *   (0.0,0.0) */
    float ratio;                    /**< 数码变倍数，变焦倍数需大于1.0，默认值
                                     *   为1 */
} ac_file_zoom_info;

/** 读数码变焦倍数
* @param h      [in] AC_FILE句柄。
* @param value  [out] 数码变焦倍数。
*/
AC_FILE_API INT32 ac_file_get_zoom_info(HANDLE h, ac_file_zoom_info* value);

/** 写数码变焦倍数
* @param h      [in] IR_FILE句柄。
* @param value  [out] 数码变焦倍数。
*/
AC_FILE_API INT32 ac_file_set_zoom_info(HANDLE h, const ac_file_zoom_info* value);

/** 读可见光灰度模式启用标识
* @param h      [in] AC_FILE句柄。
* @param value  [out] 可见光灰度模式启用标识。
*/
AC_FILE_API INT32 ac_file_get_grey_scale(HANDLE h, BYTE* value);

/** 写可见光灰度模式启用标识
* @param h      [in] AC_FILE句柄。
* @param value  [in] 可见光灰度模式启用标识。
*/
AC_FILE_API INT32 ac_file_set_grey_scale(HANDLE h, BYTE value);

/** 测量标识名 */
typedef BYTE ac_file_marker_name[32];

/** 测量标识名列表 */
typedef struct
{
    UINT32 count;                   /**< 测量标识名数量 */
    ac_file_marker_name* names;     /**< 测量标识名数组首地址 */
} ac_file_marker_names;

/** 获取测量标识名列表
* @param h      [in] AC_FILE句柄。
* @param value  [out] 测量标识名列表。
*/
AC_FILE_API INT32 ac_file_get_marker_names(HANDLE h, ac_file_marker_names* value);

/** 测量标识类型 */
typedef enum 
{
    AC_FILE_SPOT = 1,               /**< 点 */
    AC_FILE_ELLIPSE = 2,            /**< 椭圆 */
} ac_file_enum_marker_type;

/** 测量标识 */
typedef struct
{
    ac_file_enum_marker_type type;  /**< 测量标识类型，默认值为点 */
    ac_file_marker_name name;       /**< 名称 */
    UINT32 point_count;		        /**< 点数量 */
    ac_file_point* points;          /**< 点数组首地址 */
    float db_value;                 /**< 最大分贝数*/
    ac_file_point point;			/**< 最大分贝点*/
} ac_file_marker;

/** 读测量标识
* @param h      [in] AC_FILE句柄。
* @param value  [in|out] 测量标识，name为输入项，其它为输出项。points需要用户分
*                        配内存（ac_file_point[pointCount]），若为空，不读取点
*                        列表，pointCount返回点列表长度。
* @retval AC_FILE_EC_INVALID_KEY 该测量标识不存在。
*/
AC_FILE_API INT32 ac_file_get_marker(HANDLE h, ac_file_marker* value);

/** 写测量标识
* @param h      [in] AC_FILE句柄。
* @param value  [in|out] 测量标识，tempValues、tempPos、area为输出项，其它为输
*                        入项。若测量标识存在则修改，不存在则添加。若points为空，
*                        写入失败。写测量标识成功后输出项将会被更新。
*/
AC_FILE_API INT32 ac_file_set_marker(HANDLE h, ac_file_marker* value);

/** 删除测量标识
* @param h      [in] AC_FILE句柄。
* @param value  [in] 测量标识名。若value为空，清空测量标识列表。
*/
AC_FILE_API INT32 ac_file_remove_marker(HANDLE h, const ac_file_marker_name value);

/** 重命名测量标识
* @param h      [in] AC_FILE句柄。
* @param from   [in] 旧测量标识名。
* @param to     [in] 新测量标识名。
*/
AC_FILE_API INT32 ac_file_rename_marker(HANDLE h, const ac_file_marker_name from
    , const ac_file_marker_name to);


/** 读封面图
* 封面图即声像文件中的普通jpg部分，使用看图工具查看图片将显示封面图。
* @param h      [in] AC_FILE句柄。
* @param value  [out] 封面图，jpg文件。buffer需要用户分配内存，若buffer为空，不
*                     读取封面图，length返回图片大小。
*/
AC_FILE_API INT32 ac_file_get_cover_image(HANDLE h, ac_file_buffer* value);

/** 写封面图
* 封面图即声像文件中的普通jpg部分，使用看图工具查看图片将显示封面图。
* @param h      [in] AC_FILE句柄。
* @param value  [in] 封面图，jpg文件，单边最小尺寸240像素。若buffer为空，写入失
*                    败。若value为空，清空封面图。
*/
AC_FILE_API INT32 ac_file_set_cover_image(HANDLE h, const ac_file_buffer* value);

/** 旋转方式 */
typedef enum 
{
    AC_FILE_ROTATE_0 = 0,           /**< 未旋转 */
    AC_FILE_ROTATE_90 = 1,          /**< 顺时针旋转90° */
    AC_FILE_ROTATE_180 = 2,         /**< 顺时针旋转180° */
    AC_FILE_ROTATE_270 = 3,         /**< 顺时针旋转270° */
} ac_file_enum_rotate_type;

/** 读旋转方式
* @param h      [in] IR_FILE句柄。
* @param value  [in] 旋转方式。
*/
AC_FILE_API INT32 ac_file_get_rotation(HANDLE h, ac_file_enum_rotate_type* value);

/** 写旋转方式
* @param h      [in] IR_FILE句柄。
* @param value  [in] 旋转方式。
*/
AC_FILE_API INT32 ac_file_set_rotation(HANDLE h, ac_file_enum_rotate_type value);

/** 镜像方式 */
typedef enum 
{
    AC_FILE_MIRROR_HORIZONTAL = 1,  /**< 左右镜像 */
    AC_FILE_MIRROR_VERTICAL = 2,    /**< 上下镜像 */
} ac_file_enum_mirror_type;


/** 读总帧数
* @param h      [in] AC_FILE句柄。
* @param value  [out] 总帧数。
*/
AC_FILE_API INT32 ac_file_get_frame_count(HANDLE h, UINT32* value);

/** 读帧频
* @param h      [in] AC_FILE句柄。
* @param value  [out] 帧频。
*/
AC_FILE_API INT32 ac_file_get_frame_rate(HANDLE h, float* value);

/** 写帧频
* @param h      [in] AC_FILE句柄。
* @param value  [in] 帧频。
*/
AC_FILE_API INT32 ac_file_set_frame_rate(HANDLE h, float value);

/** 数据帧 */
typedef struct 
{
    UINT32 length;                  /**< 字节数组大小 */
    BYTE* buffer;                 	/**< 字节数组首地址 */
} ac_file;

typedef struct
{
    UINT32 length;					/**< float数组大小 */
    float* buffer;					/**< float数组首地址 */
} ac_file_raw;

typedef struct
{
    ac_file_raw fft;         		/**< fft sound的频谱幅值 */
    ac_file_band range;      		/**< 频率上下限 */
    float delta;             		/**< fft 相邻两点对应的差值 */
} ac_file_spectrum;

typedef struct
{
    ac_file_raw source;				/**< 原始声音 */
    ac_file_raw filter;				/**< 滤波声音 */
    ac_file_raw down_sample;		/**< 降采样声音 */
} ac_file_sound;

typedef struct
{
    UINT32 points_count;			/**< 局放数据数组大小 */
    ac_file_point_f* points;		/**< 局放数据数组首地址 */
} ac_file_prpd;

/** 声像原始帧 */
typedef struct
{
    UINT64 time_stamp; 				/**< 拍摄时间，采用UTC时间，从1970-01-01 00:00:00.000起的毫秒数 */
    ac_file_raw acoustic_image_spl;	/**< 声像传感器声压级网格数据 */
    ac_file visual_image; 			/**< 可见光图像 */
    ac_file_sound sound; 			/**< 声像传感器声音数据 */
    ac_file_spectrum spectrum;		/**< 声像传感器最后一帧的频谱幅值 */
    ac_file_prpd prpd;				/**< 声像传感器prpd数据 */
} ac_file_frame;

/** 获取声像原始帧
* @param h      [in] AC_FILE句柄。
* @param index  [in] 帧索引。
* @param value  [in] 声像原始帧。buffer需要用户分配内存，若buffer为空，不读
*                    取声像原始帧，length返回buffer数组所需大小。
*/
AC_FILE_API INT32 ac_file_get_frame(HANDLE h, UINT32 index, ac_file_frame* value);

/** 获取声像原始帧V2
* @param h      [in] AC_FILE句柄。
* @param index  [in] 帧索引。
* @param value  [in] 声像原始帧。buffer需要用户分配内存，若buffer为空，不读
*                    取声像原始帧，length返回buffer数组所需大小。
*/
AC_FILE_API INT32 ac_file_get_frame_v2(HANDLE h, UINT32 index, ac_file_frame* value);

/** 追加声像原始帧
* @param h      [in] AC_FILE句柄。
* @param value  [in] 声像原始帧。若buffer为空，写入失败。若value为空，清空帧
*                    数据区。
*/
AC_FILE_API INT32 ac_file_append_frame(HANDLE h, const ac_file_frame* value);

/** 标记点 */
typedef struct
{
    ac_file_point point; 			/**< 点坐标 */
    float value; 					/**< 声压值 */
} ac_file_db_point;

/** 读点声压值
* @param h      [in] AC_FILE句柄。
* @param value  [out] 点。
*/
AC_FILE_API INT32 ac_file_get_db_spot(HANDLE h, ac_file_db_point* value);

/** 标记矩形 */
typedef struct
{
    ac_file_rectangle rect; 		/**< 矩形坐标 */
    UINT32 count; 					/**< 区域数组大小 */
    float* values; 					/**< 声压值 */
} ac_file_db_rect;


/** 读椭圆区域声压值
* @param h      [in] AC_FILE句柄。
* @param value  [out] 椭圆区域。
*/
AC_FILE_API INT32 ac_file_get_db_ellipse(HANDLE h, ac_file_db_rect* value);


/** 读调色板色带
* @param h      [in] AC_FILE句柄。
* @param value  [out] 调色板色带，256行1列BGR。
*/
AC_FILE_API INT32 ac_file_get_palette_image(HANDLE h, ac_file* value);

/** 预置调色板 */
typedef struct
{
    BYTE ref_no;                    /**< 调色板编号 */
    BYTE name[32];                  /**< 调色板名 */
} ac_file_preset_palette;

/** 预置调色板列表 */
typedef struct
{
    UINT32 count;                    /**< 预置调色板数量 */
    ac_file_preset_palette* palettes;/**< 预置调色板数组首地址 */
} ac_file_preset_palettes;

/** 读预置调色板
* @param value  [out] 预置调色板。
*/
AC_FILE_API INT32 ac_file_get_preset_palettes(ac_file_preset_palettes* value);

/** 读预置调色板色带
* @param refNo  [in] 调色板编号。
* @param value  [out] 预置调色板色带，256行1列BGR。
*/
AC_FILE_API INT32 ac_file_get_preset_palette_image(BYTE refNo, ac_file* value);


/** 获取当前全息数据帧索引
* @param h      [in] AC_FILE句柄。
* @param value  [out] 帧索引，取值范围0到总帧数-1。
*/
AC_FILE_API INT32 ac_file_get_frame_index(HANDLE h, UINT32* value);

/** 设置当前全息数据帧索引
* @param h      [in] AC_FILE句柄。
* @param value  [in] 帧索引，取值范围0到总帧数-1。
*/
AC_FILE_API INT32 ac_file_set_frame_index(HANDLE h, UINT32 value);

/** 声像文件当前帧另存为
* @param h          [in] AC_FILE句柄。
* @param file_name  [in] 文件路径。
*/
AC_FILE_API INT32 ac_file_save_frame_as(HANDLE h, const char* file_name, ac_file* cover_image);

/** 读指定帧的拍摄时间
* @param h      [in] AC_FILE句柄。
* @param index	[in] 帧索引。
* @param value  [out] 拍摄时间，采用UTC时间，从1970-01-01 00:00:00.000起的毫秒数。
*/
AC_FILE_API INT32 ac_file_get_index_shot_time(HANDLE h, UINT32 index, UINT64* value);
#pragma pack(pop)
#endif