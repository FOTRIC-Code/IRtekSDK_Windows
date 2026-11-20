# HL API

## 设备管理


| HTTP 接口                   | 方法        | 头文件符号                                         | 接口名称                  | 请求 JSON        | 响应 JSON          |
| ------------------------- | --------- | --------------------------------------------- | --------------------- | -------------- | ---------------- |
| `system/device_info`      | `GET`     | `net_get_device_info`                          | 获取设备信息                | -              | `device_info`      |
| `system/device_info`      | `PUT`     | `net_set_device_info`                          | 设置设备信息                | `device_name`  | -                  |
| `system/device_restart`   | `PUT`     | `net_device_restart`                          | 设备重启                  | -              | -                |
| `system/device_reset`     | `PUT`     | `net_device_reset`                            | 设备重置                  | -              | -                |
| `system/device_upgrade`   | `POST`    | `net_device_upgrade`                          | 设备升级                  | `upgrade_info` | -                |
| `system/time`             | `GET/PUT` | `net_get_time` / `net_set_time`               | 获取设备时间 / 设置设备时间       | `time_info`    | `time_info`      |
| `system/ntp_server` | `GET/PUT` | `net_get_ntp_server` / `net_set_ntp_server`   | 获取 NTP 服务 / 设置 NTP 服务 | `ntp_info`     | `ntp_info`       |
| `system/networks`         | `GET`     | `net_get_networks`                            | 获取网络参数列表              | -              | `network_info[]` |
| `system/networks/{id}`    | `GET/PUT` | `net_get_network` / `net_set_network`         | 获取网络参数 / 设置网络参数       | `network_info` | `network_info`   |
| `system/port`             | `GET/PUT` | `net_get_port` / `net_set_port`               | 获取设备服务端口 / 设置设备服务端口   | `port_info`    | `port_info`      |
| `system/ftp`              | `GET/PUT` | `net_get_ftp` / `net_set_ftp`                 | 获取 FTP 配置 / 设置 FTP 配置 | `ftp_info`     | `ftp_info`       |
| `system/ftp/test`              | `POST` | `net_test_ftp`                 | FTP 测试 | `ftp_info`     | `ftp_info`       |
| `system/webhook`          | `GET/PUT` | `net_get_webhook` / `net_set_webhook`         | 获取 Webhook 地址 / 设置 Webhook 地址 | `webhook_info` | `webhook_info`   |


## 报警联动


| HTTP 接口                 | 方法        | 头文件符号                                             | 接口名称            | 请求 JSON         | 响应 JSON         |
| ----------------------- | --------- | ------------------------------------------------- | --------------- | --------------- | --------------- |
| `alarm/triggers/{type}` | `GET/PUT` | `net_get_alarm_trigger` / `net_set_alarm_trigger` | 获取报警联动 / 设置报警联动 | `alarm_trigger` | `alarm_trigger` |


## 用户管理


| HTTP 接口               | 方法       | 头文件符号             | 接口名称    | 请求 JSON         | 响应 JSON       |
| --------------------- | -------- | ----------------- | ------- | --------------- | ------------- |
| `security/users`      | `GET`    | `net_get_users`   | 获取用户列表  | -               | `user_info[]` |
| `security/users/{id}` | `GET`    | `net_get_user`    | 获取用户信息  | -               | `user_info`   |
| `security/users`      | `PUT`    | `net_update_user` | 添加或修改用户 | `user_pwd_info` | -             |
| `security/users/{id}` | `DELETE` | `net_delete_user` | 删除用户    | -               | -             |


## 视频流


| HTTP 接口                                 | 方法    | 头文件符号                   | 接口名称    | 请求 JSON | 响应 JSON         |
| --------------------------------------- | ----- | ----------------------- | ------- | ------- | --------------- |
| `streaming/channels`                    | `GET` | `net_get_streams`       | 获取视频流列表 | -       | `stream_info[]` |
| `streaming/channels/{stream_id}`        | `GET` | `net_get_stream`        | 获取视频流参数 | -       | `stream_info`   |
| `streaming/channels/{stream_id}/status` | `GET` | `net_get_stream_status` | 获取视频流状态 | -       | `stream_status` |


## 显示设置


| HTTP 接口                                    | 方法        | 头文件符号                                           | 接口名称                | 请求 JSON        | 响应 JSON          |
| ------------------------------------------ | --------- | ----------------------------------------------- | ------------------- | -------------- | ---------------- |
| `video/channels`                           | `GET`     | `net_get_channels`                              | 获取通道列表              | -              | `channel_info[]` |
| `video/channels/{channel_id}`              | `GET/PUT` | `net_get_channel` / `net_set_channel`           | 获取通道信息 / 设置通道信息     | `channel_info` | `channel_info`   |
| `video/channels/{channel_id}/overlay`      | `GET/PUT` | `net_get_overlay` / `net_set_overlay`           | 获取字符叠加 / 设置字符叠加     | `overlay_info` | `overlay_info`   |
| `image/channels/{channel_id}/image_params` | `GET/PUT` | `net_get_image_params` / `net_set_image_params` | 获取图像参数 / 设置图像参数     | `image_params` | `image_params`   |
| `image/channels/{channel_id}/image_flip`   | `GET/PUT` | `net_get_image_flip` / `net_set_image_flip`     | 获取图像翻转模式 / 设置图像翻转模式 | `image_flip`   | `image_flip`     |


## 热像设置


| HTTP 接口                        | 方法        | 头文件符号                                                           | 接口名称                    | 请求 JSON                | 响应 JSON                |
| ------------------------------ | --------- | --------------------------------------------------------------- | ----------------------- | ---------------------- | ---------------------- |
| `thermal/lens_info`            | `GET`     | `net_get_thermal_lens_info`                                      | 获取热像镜头信息              | -                      | `thermal_lens_info`     |
| `thermal/thermal_calibrate`    | `PUT`     | `net_thermal_calibrate`                                         | 执行热像校正                  | -                      | -                      |
| `thermal/thermal_shutter_mode` | `GET/PUT` | `net_get_thermal_shutter_mode` / `net_set_thermal_shutter_mode` | 获取热像校正模式/ 设置热像校正模式      | `shutter_mode`         | `shutter_mode`         |
| `thermal/thermal_shutter_time` | `GET/PUT` | `net_get_thermal_shutter_time` / `net_set_thermal_shutter_time` | 获取热像自动校正时间 / 设置挡片自动校正时间 | `shutter_time`         | `shutter_time`         |
| `thermal/thermal_resolution`   | `GET`     | `net_get_thermal_resolution`                                    | 获取热像分辨率                 | -                      | `size`                 |
| `thermal/thermal_range`        | `GET/PUT` | `net_get_thermal_range` / `net_set_thermal_range`               | 获取热像测温量程 / 设置热像测温量程     | `thermal_range`        | `thermal_range`        |
| `thermal/thermal_lut`          | `GET`     | `net_get_thermal_lut`                                           | 获取热像 LUT 表              | -                      | `lut_table`            |
| `thermal/thermal_params`       | `GET/PUT` | `net_get_thermal_params` / `net_set_thermal_params`             | 获取热像测温参数 / 设置热像测温参数     | `thermal_params`       | `thermal_params`       |
| `thermal/color_dist`           | `GET/PUT` | `net_get_thermal_color_dist` / `net_set_thermal_color_dist`     | 获取热像色彩分布类型 / 设置热像色彩分布类型 | `color_dist`           | `color_dist`           |
| `thermal/gain`                 | `GET/PUT` | `net_get_thermal_gain` / `net_set_thermal_gain`                 | 获取温宽模式 / 设置温宽模式         | `gain`                 | `gain`                 |
| `thermal/preset_palettes`             | `GET`     | `net_get_thermal_preset_palettes`                                      | 获取热像调色板列表               | -                      | `preset_palettes`       |
| `thermal/palette`              | `GET/PUT` | `net_get_thermal_palette` / `net_set_thermal_palette`           | 获取热像调色板 / 设置热像调色板       | `palette`              | `palette`              |
| `thermal/thermal_overlay`      | `GET/PUT` | `net_get_thermal_overlay` / `net_set_thermal_overlay`           | 获取热像叠加 / 设置热像叠加         | `thermal_overlay_info` | `thermal_overlay_info` |
| `thermal/thermal_unit`         | `GET/PUT` | `net_get_thermal_unit` / `net_set_thermal_unit`                 | 获取热像单位 / 设置热像单位         | `unit_info`            | `unit_info`            |
| `thermal/capture/{mode}`       | `GET`     | `net_get_thermal_capture`                                       | 获取热像抓拍文件                | -                      | `capture_file`         |
| `thermal/marker`               | `GET/PUT` | `net_get_thermal_markers` / `net_set_thermal_markers`           | 获取热像测温标识列表 / 设置热像测温标识列表 | `marker_info[]`        | `marker_info[]`        |
| `thermal/temp/alarm`                | `GET/PUT` | `net_get_thermal_temp_alarms` / `net_set_thermal_temp_alarms`             | 获取热像测温报警列表 / 设置热像测温报警列表 | `thermal_temp_alarm[]`              | `thermal_temp_alarm[]`              |
| `thermal/temp/alarm/interval`                | `GET/PUT` | `net_get_thermal_temp_alarm_interval` / `net_set_thermal_temp_alarm_interval`             | 获取热像测温报警时间间隔 / 设置热像测温报警时间间隔 | `thermal_temp_alarm_interval`              | `thermal_temp_alarm_interval`              |
| `thermal/dpc`                  | `PUT`     | `net_set_thermal_dpc`                                           | 设置热像坏点                  | `dpc_params`           | -                      |
| `thermal/data/{type}`                 | `GET`     | `net_get_thermal_data`                                          | 获取热像实时数据                | - | `thermal_data`         |


## 本地扩展接口（无独立 HTTP 映射）


| 头文件符号 | 接口名称 | 说明 |
| --- | --- | --- |
| `net_search_devices` | 搜索设备 | 局域网设备发现 |
| `net_set_exception_callback` | 设置异常回调 | 断网/恢复/视频丢失 |
| `net_start_alarm_listener` / `net_stop_alarm_listener` | 开启/停止报警监听 | 传入 `NET_HANDLE`，返回/消费 `alarm_listener_handle` |
| `net_start_stream_play` / `net_stop_stream_play` | 开启/停止视频流播放 | 返回/消费 `stream_handle` |
| `net_set_stream_callback` | 设置视频流回调 | 入参为 `stream_handle` |
| `net_start_stream_record` / `net_stop_stream_record` | 开始/停止本地录制 | 入参为 `stream_handle` |
| `net_start_thermal_data_stream` / `net_stop_thermal_data_stream` | 开启/停止热像数据流 | 开启时传入 `thermal_data_config`（含毫秒 `sample_interval`）+ 回调；握手 `role=thermal` |


## 数据类型

- `time_mode`: `local` / `ntp`
- `user_level`: `root` / `operator` / `viewer`
- `align`: `topleft` / `topright` / `bottomleft` / `bottomright`
- `overlay.mode`: `light` / `blink`
- `shutter_mode.mode`: `automatic` / `manual`
- `image_flip.mode`: `none` / `up_down` / `left_right` / `center`
- `exception_info.exception_type`:
  - `1`: `network_disconnected`
  - `2`: `network_resumed`
  - `3`: `video_lost`
- `thermal_temp_alarm.alarm_type`（`net_enum_thermal_temp_alarm_type`）:
  - `1`: `highestgreater`
  - `2`: `highestless`
  - `3`: `lowestgreater`
  - `4`: `lowestless`
  - `5`: `averagegreater`
  - `6`: `averageless`
  - `7`: `difftempgreater`
  - `8`: `difftempless`
- `marker.type` / `marker_temp_info.type`:
  - `1`: `spot`
  - `3`: `rectangle`
  - `4`: `ellipse`
  - `5`: `polygon`
  - `6`: `polyline`
- `marker_alarm_info.alarm_level` / `alarm_thermal_temp_info.marker_alarms[].alarm_level`:
  - `0`: `none`
  - `1`: `trigger`
  - `2`: `threshold`
  - `3`: `alarm`
- `alarm_trigger.type` / `alarm_info.type`: `thermal_temp`（SDK：`NET_ALARM_THERMAL_TEMP`）
- `thermal/capture/{mode}`:
  - `0`（或省略）: 输出纯 JPEG 图像
  - `1`: 输出 TCAP 二进制容器，包含 JPEG 图像和原始热数据帧
- `color_dist.type`:
  - `1`: `t_twb` — 高温差均衡成像分布
  - `2`: `temp_linear` — 温度线性分布
  - `3`: `ad_linear` — 信号线性分布
- `gain.mode`: `fixed` / `auto`
- `sr_mode.mode`: `none` / `sr`
- `unit_info.temp_unit`: `centigrade` / `kelvin` / `fahrenheit`
- `unit_info.distance_unit`: `meter` / `foot`
- `thermal_data_request.data_types`、`thermal_data_config.data_types`、`thermal_data.data_types` 为按位组合的数值枚举:
  - `0x01`: `raw`
  - `0x02`: `global`
  - `0x04`: `marker`
- `thermal_data` 为二进制数据，与 `net_thermal_data` 一致；`marker_temps` 为固定长度数组。报警详情不在本结构中，由报警监听回调的 `net_alarm_info` 承载

### 设备管理模型

#### `login_info`

```json
{
  "ip": "192.168.10.177",
  "user_name": "admin",
  "password": "admin",
  "timeout_ms": 5000
}
```

#### `sdk_version`

```json
{
  "major": 1,
  "minor": 0,
  "patch": 26,
  "build": 0
}
```

#### `device_name`

```json
{
  "name": "IR-Cam-01",
  "model": "IR-640",
  "spec": "Fotric IR-640#L25",
  "serial_no": "1234567890",
  "id": "device-001",
  "brand": "IRtek",
  "company": "Volga",
  "firmware_name": "main",
  "firmware_version": "1.0.26"
}
```

#### `device_info`

```json
{
  "name": "IR-Cam-01"
}
```

#### `thermal_lens_info`

```json
{
  "name": "L50",
  "focal": 23.4,
  "vfov": 50.0,
  "hfov": 50.0,
  "band": "8-14um",
  "range_map": [
    {
      "id": 0,
      "range": {
        "min": 253.15,
        "max": 423.15
      }
    },
    {
      "id": 1,
      "range": {
        "min": 273.15,
        "max": 823.15
      }
    }
  ]
}
```

#### `upgrade_info`

```json
{
  "path": "D:/firmware/upgrade.bin"
}
```

#### `time_info`

```json
{
  "time_mode": "ntp",
  "local_time": "2026-05-20T10:09:00+08:00",
  "time_zone": "UTC+08:00"
}
```

#### `port_info`

```json
{
  "http_port_no": 80,
  "rtsp_port_no": 554,
  "rtp_port_no": 8001
}
```

#### `ntp_info`

```json
{
  "host_name": "pool.ntp.org",
  "port_no": 123,
  "syn_interval": 1440
}
```

列表接口返回 `ntp_info[]`。

#### `network_info`

```json
{
  "id": 1,
  "name": "eth0",
  "dhcp": true,
  "ip": "192.168.10.177",
  "netmask": "255.255.255.0",
  "dns": [
    "192.168.10.11",
    "192.168.10.12"
  ],
  "gateway": [
    "192.168.10.1"
  ]
}
```

列表接口返回 `network_info[]`。

#### `ftp_info`

```json
{
  "use_ssl": false,
  "ip": "192.168.10.20",
  "port_no": 21,
  "user_name": "ftp_user",
  "pwd": "hashed-or-plain-password",
  "enable_annoy": false,
  "path_template": "{DEVICE_NAME}/{DEVICE_SERIAL_NO}/fixed name/{DEVICE_IP}/{CHANNEL_NAME}/{CHANNEL_ID}/{yyyyMMddHHmmssfff}.jpg"
}
```

`enable_annoy` 为头文件现有字段名，语义上表示匿名登录开关；`path_template` 对应头文件中的模板字符串字段。

#### `webhook_info`

```json
{
  "url": "https://example.com/webhook"
}
```

SDK 侧对应 `net_webhook_info`。`url` 缓冲区长度为 `NET_LEN_256`。

### 报警联动模型

#### `alarm_trigger`

```json
{
  "type": "thermal_temp",
  "trigger_ftp_enabled": true,
  "trigger_webhook_enabled": false
}
```

`alarm/triggers/{type}` 中的 `{type}` 与 SDK 接口 `net_get_alarm_trigger()` / `net_set_alarm_trigger()` 的单独 `type` 入参对应；当前 SDK 枚举值为 `NET_ALARM_THERMAL_TEMP`。SDK 结构体 `net_alarm_trigger` 仅包含 `trigger_ftp_enabled` 与 `trigger_webhook_enabled`，`type` 由接口入参单独传入。

### 用户管理模型

#### `user_info`

```json
{
  "id": 1,
  "user_level": "root",
  "user_name": "admin"
}
```

列表接口返回 `user_info[]`。

#### `user_pwd_info`

```json
{
  "id": 1,
  "user_level": "root",
  "user_name": "admin",
  "login_pwd": "md5(user_name:realm:login_pwd)",
  "pwd": "md5(user_name:realm:new_pwd)"
}
```

### 视频流模型

#### `stream_info`

```json
{
  "id": 101,
  "bit_rate": 65780,
  "codec_type": "h264",
  "disable_skip": false,
  "enable_rc": false,
  "gop_size": 1,
  "h264_profile": 1,
  "initial_qp": 0,
  "max_frame_rate": 30,
  "maximum_qp": 10,
  "quality_control_type": "MDQ",
  "resolution_width": 240,
  "resolution_height": 320
}
```

列表接口返回 `stream_info[]`。

#### `stream_status`

```json
{
  "id": 101,
  "connection_count": 5
}
```

#### `stream_data`

```json
{
  "stream_id": 101,
  "codec_type": "h264",
  "data": "<binary>",
  "data_length": 4096,
  "timestamp_us": 1716170940000000,
  "key_frame": true
}
```

### 显示设置模型

#### `image_params`

```json
{
  "contrast": 50,
  "brightness": 50,
  "dde_enable": true,
  "dde_value": 50
}
```

#### `image_flip`

```json
{
  "mode": "none"
}
```

SDK 侧直接使用 `net_enum_image_flip_mode`。

#### `channel_info`

```json
{
  "id": 1,
  "name": "channel1"
}
```

列表接口返回 `channel_info[]`。

#### `overlay_block`

```json
{
  "align": "topleft",
  "enabled": true,
  "mode": "light",
  "x": 12,
  "y": 12
}
```

#### `overlay_info`

```json
{
  "channel_name_overlay": {
    "align": "topleft",
    "enabled": true,
    "mode": "light",
    "x": 12,
    "y": 12
  },
  "time_overlay": {
    "align": "topright",
    "enabled": true,
    "mode": "blink",
    "x": 18,
    "y": 12
  },
  "custom_overlay": {
    "align": "bottomleft",
    "enabled": false,
    "mode": "light",
    "x": 24,
    "y": 40,
    "text": "custom"
  }
}
```

### 热像设置模型

#### `shutter_mode`

```json
{
  "mode": "automatic"
}
```

SDK 侧直接使用 `net_enum_shutter_mode`。

#### `shutter_time`

```json
{
  "interval": 10
}
```

SDK 侧直接使用 `USHORT minutes`。

#### `size`

```json
{
  "width": 640,
  "height": 512
}
```

#### `thermal_range`

```json
{
  "range_id": 0
}
```

SDK 侧直接使用 `USHORT range_id`。

#### `lut_table`

```json
{
  "range_id": 0,
  "length": 4,
  "ad": [
    1200,
    1800,
    2400,
    3000
  ],
  "t": [
    273.15,
    293.15,
    313.15,
    333.15
  ]
}
```

#### `thermal_temp_alarm`

```json
{
  "marker_name": "sp1",
  "enabled": true,
  "alarm_type": 1,
  "alarm_temp": 373.15,
  "alarm_delay_time": 5,
  "threshold_temp": 353.15,
  "threshold_delay_time": 3,
  "trigger_temp": 343.15,
  "trigger_delay_time": 1
}
```

`thermal/temp/alarm` 的 GET 接口返回 `thermal_temp_alarm[]`，PUT 接口使用 `thermal_temp_alarm[]` 作为请求体。SDK 侧对应 `net_thermal_temp_alarm`（`net_get_thermal_temp_alarms` / `net_set_thermal_temp_alarms`）。

#### `thermal_temp_alarm_interval`

```json
{
  "interval": 10
}
```

SDK 侧对应 `net_thermal_temp_alarm_interval`（`net_get_thermal_temp_alarm_interval` / `net_set_thermal_temp_alarm_interval`）。

#### `thermal_params`

```json
{
  "emissivity": 0.95,
  "distance": 1.0,
  "refl_temp": 293.15,
  "atm_temp": 293.15,
  "rel_humidity": 0.5
}
```

#### `color_dist`

```json
{
  "type": 1
}
```

SDK 侧直接使用 `net_enum_color_dist_type`。

#### `gain`

```json
{
  "mode": "auto",
  "max": 5750.0,
  "min": 0.0,
  "fixedSpan": false,
  "span": 2.0
}
```

`mode` 为 `fixed` 时，手动指定 `min`/`max` 温宽范围；为 `auto` 时由设备自动调节。`fixedSpan` 启用后使用固定温宽跨度 `span`（范围 0.1~5750，默认 2.0）。

#### `unit_info`

```json
{
  "temp_unit": "centigrade",
  "distance_unit": "meter"
}
```

#### `preset_palettes`

```json
[
  {
    "ref_no": 0,
    "name": "iron"
  }
]
```

列表接口返回 `preset_palettes`。

#### `palette`

```json
{
  "ref_no": 0,
  "colors": [
    4294967295,
    4278190080
  ],
  "inverted": false
}
```

#### `overlay_follow_block`

```json
{
  "align": "topright",
  "enabled": true,
  "follow": true,
  "max_enabled": true,
  "min_enabled": true,
  "avg_enabled": true,
  "emissivity_enabled": true,
  "x": 18,
  "y": 12
}
```

#### `thermal_overlay_info`

```json
{
  "unit_overlay": {
    "align": "bottomleft",
    "enabled": true,
    "mode": "light",
    "x": 12,
    "y": 12
  },
  "plt_overlay": {
    "align": "topright",
    "enabled": true,
    "mode": "light",
    "x": 18,
    "y": 12
  },
  "thermal_params_overlay": {
    "align": "topleft",
    "enabled": true,
    "mode": "light",
    "x": 12,
    "y": 12
  },
  "global_overlay": {
    "align": "topright",
    "enabled": true,
    "follow": true,
    "max_enabled": true,
    "min_enabled": true,
    "avg_enabled": true,
    "emissivity_enabled": true,
    "x": 18,
    "y": 12
  },
  "marker_overlay": {
    "align": "topright",
    "enabled": true,
    "follow": true,
    "max_enabled": true,
    "min_enabled": true,
    "avg_enabled": true,
    "emissivity_enabled": true,
    "x": 18,
    "y": 12
  },
  "alarm_overlay": {
    "align": "bottomright",
    "enabled": true,
    "mode": "blink",
    "x": 24,
    "y": 24
  }
}
```

#### `point`

```json
{
  "x": 18,
  "y": 12
}
```

#### `marker_info`

```json
{
  "id": 1,
  "type": 1,
  "name": "sp1",
  "local_params": false,
  "refl_temp": 293.15,
  "distance": 1.0,
  "emissivity": 0.9,
  "points": [
    {
      "x": 18,
      "y": 12
    },
    {
      "x": 150,
      "y": 12
    }
  ]
}
```

列表接口返回 `marker_info[]`。

#### `capture_file`

`thermal/capture/{mode}` 为 GET 接口，`{mode}` 为路径参数：

- `/thermal/capture/0` 或 `/thermal/capture` — 输出纯 JPEG 图像
- `/thermal/capture/1` — 输出 TCAP 二进制容器，包含 JPEG 图像和原始热数据帧

SDK 侧直接使用 `net_enum_thermal_capture_mode mode`。

```json
{
  "local_file": "D:/captures/thermal_capture.jpg"
}
```

HTTP 响应内容如下：

| mode | 响应 Content-Type | 内容 |
| --- | --- | --- |
| `0`（或省略） | `image/jpeg` | 纯 JPEG 图像 |
| `1` | `application/octet-stream` | TCAP 二进制容器，包含 JPEG 图像 + 序列化的原始热数据帧 |

TCAP 容器格式（mode 1）：

| 偏移 | 大小 | 内容 |
| --- | --- | --- |
| `0` | `4` | 魔数 `"TCAP"` (`0x50414354`，小端) |
| `4` | `4` | JPEG 数据长度 `jpeg_len` (`uint32`，小端) |
| `8` | `4` | 原始数据长度 `raw_len` (`uint32`，小端) |
| `12` | `jpeg_len` | JPEG 图像数据 |
| `12 + jpeg_len` | `raw_len` | `tframe_serialize()` 输出的原始热数据帧（与 RTP 传输的帧格式一致） |

### 其他公开模型

#### `discovered_device`

```json
{
  "ip": "192.168.10.177",
  "netmask": "255.255.255.0",
  "name": "IR-Cam-01",
  "model": "IR-640",
  "serial_no": "1234567890",
  "id": "device-001",
  "firmware_version": "1.0.26",
  "firmware_name": "fw.bin",
  "wandering": false
}
```

#### `exception_info`

```json
{
  "exception_type": 1,
  "stream_id": 101,
  "message": "network disconnected"
}
```

SDK 侧对应 `net_exception_info`，由 `net_set_exception_callback()` 回调下发。

#### `dpc_params`

```json
{
  "points": [
    {
      "x": 100,
      "y": 120
    },
    {
      "x": 101,
      "y": 120
    }
  ]
}
```

#### `sr_mode`

```json
{
  "mode": "sr"
}
```

SDK 侧直接使用 `net_enum_sr_mode`。

#### `thermal_data_config`

```json
{
  "data_types": 7,
  "sample_interval": 1000
}
```

该模型由 `net_start_thermal_data_stream()` 一并传入；`data_types` 为按位组合，`sample_interval` 为采样间隔（单位毫秒，最小 50）。参数经 RTP 握手 `GET /session?role=thermal&data_types=…&sample_interval=…` 绑定到该连接，不再下发 `PUT /thermal/data/config`。

SDK 侧直接使用 `UINT32 data_types` 位掩码入参（`RAW | GLOBAL | MARKER`）。

#### `temp_point`

```json
{
  "temperature": 323.15,
  "point": {
    "x": 120,
    "y": 80
  }
}
```

#### `temp_point_info`

```json
{
  "max": {
    "temperature": 353.15,
    "point": {
      "x": 120,
      "y": 80
    }
  },
  "min": {
    "temperature": 293.15,
    "point": {
      "x": 10,
      "y": 20
    }
  },
  "avg_temp": 321.15
}
```

#### `marker_alarm_info`

```json
{
  "marker_name": "sp1",
  "alarm_level": 2
}
```

#### `alarm_info`

报警监听通用载荷，对应 `net_alarm_info`。

SDK 回调（二进制）含 `type` / `alarm_info_length` / `alarm_info`。Webhook POST JSON 仅含：

```json
{
  "type": 1,
  "alarm_info": { "...": "见 alarm_thermal_temp_info；不含 raw / message_length" }
}
```

当 `type` 为 `NET_ALARM_THERMAL_TEMP`（=1）时，`alarm_info` 按 `net_alarm_thermal_temp_info` 解释。

#### `alarm_thermal_temp_info`

```json
{
  "timestamp_us": 1710000000000000,
  "dev_info": {
    "ip": "192.168.10.177",
    "port_no": 80,
    "channel_id": 1
  },
  "global_temp_info": {
    "max": {
      "temperature": 353.15,
      "point": { "x": 120, "y": 80 }
    },
    "min": {
      "temperature": 293.15,
      "point": { "x": 10, "y": 20 }
    },
    "avg_temp": 321.15
  },
  "marker_alarms": [
    {
      "marker_name": "sp1",
      "alarm_level": 2
    }
  ],
  "marker_temps": [
    {
      "marker_id": 1,
      "type": 1,
      "marker_name": "sp1",
      "temp_point_info": {
        "max": {
          "temperature": 353.15,
          "point": { "x": 120, "y": 80 }
        },
        "min": {
          "temperature": 293.15,
          "point": { "x": 10, "y": 20 }
        },
        "avg_temp": 321.15
      }
    }
  ],
  "message": "thermal temperature alarm"
}
```

SDK 侧对应 `net_alarm_thermal_temp_info`：`dev_info` 由固件报警载荷直接上送，包含报警来源设备的 IP、端口和通道号；`marker_alarms` 有效个数由 `marker_alarms_count` 决定（可含 `global`），`marker_temps` 有效个数由 `markers_count` 决定（不含 `global`）；`message` 仅在回调期间有效。由 `net_start_alarm_listener()` 回调下发，不再挂在 `net_thermal_data` 上。

#### `marker_temp_info`

```json
{
  "marker_id": 1,
  "type": 1,
  "marker_name": "sp1",
  "temp_point_info": {
    "max": {
      "temperature": 353.15,
      "point": {
        "x": 120,
        "y": 80
      }
    },
    "min": {
      "temperature": 293.15,
      "point": {
        "x": 10,
        "y": 20
      }
    },
    "avg_temp": 321.15
  }
}
```

#### `raw_frame`

`raw_frame` 对应头文件中的 `net_raw_frame`：

| 字段 | 类型 | 说明 |
| --- | --- | --- |
| `width` | `USHORT` | 热像宽度 |
| `height` | `USHORT` | 热像高度 |
| `range_id` | `USHORT` | 量程 ID |
| `data` | `const USHORT*` | 原始 AD 数据，长度为 `points_count` |
| `points_count` | `UINT32` | 原始 AD 点数，通常为 `width * height` |
| `lut_len` | `UINT32` | LUT 点数 |
| `lut_env_temp` | `UINT32` | LUT 环境温度 |
| `sensor_temp` | `UINT32` | 传感器温度 |
| `shutter_state` | `UINT32` | 挡片状态 |
| `frame_no` | `UINT32` | 帧序号 |
| `pts` | `UINT64` | 同步时间戳 |
| `raw_type` | `UINT32` | 帧类型 |
| `lut_temp_value` | `const float*` | LUT 温度数组，长度为 `lut_len` |
| `lut_ad_value` | `const USHORT*` | LUT AD 数组，长度为 `lut_len` |
| `shutter_adv` | `UINT32` | 挡片温度 |


