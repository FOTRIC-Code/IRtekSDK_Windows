# HL API

## Device Management


| HTTP API                  | Method    | Header Symbol                                 | API Name                          | Request JSON   | Response JSON    |
| ------------------------- | --------- | --------------------------------------------- | --------------------------------- | -------------- | ---------------- |
| `system/device_info`      | `GET`     | `net_get_device_info`                         | Get device information            | -              | `device_info`    |
| `system/device_info`      | `PUT`     | `net_set_device_info`                         | Set device information            | `device_name`  | -                |
| `system/device_restart`   | `PUT`     | `net_device_restart`                          | Restart device                    | -              | -                |
| `system/device_reset`     | `PUT`     | `net_device_reset`                            | Reset device                      | -              | -                |
| `system/device_upgrade`   | `POST`    | `net_device_upgrade`                          | Upgrade device                    | `upgrade_info` | -                |
| `system/time`             | `GET/PUT` | `net_get_time` / `net_set_time`               | Get device time / Set device time | `time_info`    | `time_info`      |
| `system/ntp_server` | `GET/PUT` | `net_get_ntp_server` / `net_set_ntp_server`   | Get NTP service / Set NTP service | `ntp_info`     | `ntp_info`       |
| `system/networks`         | `GET`     | `net_get_networks`                            | Get network parameter list        | -              | `network_info[]` |
| `system/networks/{id}`    | `GET/PUT` | `net_get_network` / `net_set_network`         | Get network parameters / Set network parameters | `network_info` | `network_info` |
| `system/port`             | `GET/PUT` | `net_get_port` / `net_set_port`               | Get device service ports / Set device service ports | `port_info` | `port_info` |
| `system/ftp`              | `GET/PUT` | `net_get_ftp` / `net_set_ftp`                 | Get FTP configuration / Set FTP configuration | `ftp_info` | `ftp_info` |
| `system/ftp/test`              | `POST` | `net_test_ftp`                 | Test FTP | `ftp_info`     | `ftp_info`       |
| `system/webhook`          | `GET/PUT` | `net_get_webhook` / `net_set_webhook`         | Get webhook URL / Set webhook URL | `webhook_info` | `webhook_info`   |


## Alarm Linkage


| HTTP API                | Method    | Header Symbol                                       | API Name                          | Request JSON    | Response JSON   |
| ----------------------- | --------- | --------------------------------------------------- | --------------------------------- | --------------- | --------------- |
| `alarm/triggers/{type}` | `GET/PUT` | `net_get_alarm_trigger` / `net_set_alarm_trigger` | Get alarm linkage / Set alarm linkage | `alarm_trigger` | `alarm_trigger` |


## User Management


| HTTP API              | Method   | Header Symbol     | API Name          | Request JSON    | Response JSON |
| --------------------- | -------- | ----------------- | ----------------- | --------------- | ------------- |
| `security/users`      | `GET`    | `net_get_users`   | Get user list     | -               | `user_info[]` |
| `security/users/{id}` | `GET`    | `net_get_user`    | Get user information | -            | `user_info`   |
| `security/users`      | `PUT`    | `net_update_user` | Add or update user | `user_pwd_info` | -             |
| `security/users/{id}` | `DELETE` | `net_delete_user` | Delete user       | -               | -             |


## Video Stream


| HTTP API                                | Method | Header Symbol           | API Name                    | Request JSON | Response JSON   |
| --------------------------------------- | ------ | ----------------------- | --------------------------- | ------------ | --------------- |
| `streaming/channels`                    | `GET`  | `net_get_streams`       | Get video stream list       | -            | `stream_info[]` |
| `streaming/channels/{stream_id}`        | `GET`  | `net_get_stream`        | Get video stream parameters | -            | `stream_info`   |
| `streaming/channels/{stream_id}/status` | `GET`  | `net_get_stream_status` | Get video stream status     | -            | `stream_status` |


## Display Settings


| HTTP API                                   | Method    | Header Symbol                                     | API Name                            | Request JSON   | Response JSON    |
| ------------------------------------------ | --------- | ------------------------------------------------- | ----------------------------------- | -------------- | ---------------- |
| `video/channels`                           | `GET`     | `net_get_channels`                                | Get channel list                    | -              | `channel_info[]` |
| `video/channels/{channel_id}`              | `GET/PUT` | `net_get_channel` / `net_set_channel`             | Get channel information / Set channel information | `channel_info` | `channel_info` |
| `video/channels/{channel_id}/overlay`      | `GET/PUT` | `net_get_overlay` / `net_set_overlay`             | Get character overlay / Set character overlay | `overlay_info` | `overlay_info` |
| `image/channels/{channel_id}/image_params` | `GET/PUT` | `net_get_image_params` / `net_set_image_params`   | Get image parameters / Set image parameters | `image_params` | `image_params` |
| `image/channels/{channel_id}/image_flip`   | `GET/PUT` | `net_get_image_flip` / `net_set_image_flip`       | Get image flip mode / Set image flip mode | `image_flip` | `image_flip` |


## Thermal Settings


| HTTP API                       | Method    | Header Symbol                                                   | API Name                                      | Request JSON           | Response JSON          |
| ------------------------------ | --------- | --------------------------------------------------------------- | --------------------------------------------- | ---------------------- | ---------------------- |
| `thermal/lens_info`            | `GET`     | `net_get_thermal_lens_info`                                     | Get thermal lens information                  | -                      | `thermal_lens_info`    |
| `thermal/thermal_calibrate`    | `PUT`     | `net_thermal_calibrate`                                         | Perform thermal calibration                   | -                      | -                      |
| `thermal/thermal_shutter_mode` | `GET/PUT` | `net_get_thermal_shutter_mode` / `net_set_thermal_shutter_mode` | Get thermal calibration mode / Set thermal calibration mode | `shutter_mode` | `shutter_mode` |
| `thermal/thermal_shutter_time` | `GET/PUT` | `net_get_thermal_shutter_time` / `net_set_thermal_shutter_time` | Get thermal auto-calibration interval / Set shutter auto-calibration interval | `shutter_time` | `shutter_time` |
| `thermal/thermal_resolution`   | `GET`     | `net_get_thermal_resolution`                                    | Get thermal resolution                        | -                      | `size`                 |
| `thermal/thermal_range`        | `GET/PUT` | `net_get_thermal_range` / `net_set_thermal_range`               | Get thermal measurement range / Set thermal measurement range | `thermal_range` | `thermal_range` |
| `thermal/thermal_lut`          | `GET`     | `net_get_thermal_lut`                                           | Get thermal LUT table                         | -                      | `lut_table`            |
| `thermal/thermal_params`       | `GET/PUT` | `net_get_thermal_params` / `net_set_thermal_params`             | Get thermal measurement parameters / Set thermal measurement parameters | `thermal_params` | `thermal_params` |
| `thermal/color_dist`           | `GET/PUT` | `net_get_thermal_color_dist` / `net_set_thermal_color_dist`     | Get thermal color distribution type / Set thermal color distribution type | `color_dist` | `color_dist` |
| `thermal/gain`                 | `GET/PUT` | `net_get_thermal_gain` / `net_set_thermal_gain`                 | Get temperature span mode / Set temperature span mode | `gain`         | `gain`                 |
| `thermal/preset_palettes`             | `GET`     | `net_get_thermal_preset_palettes`                                      | Get thermal palette list               | -                      | `preset_palettes`       |
| `thermal/palette`              | `GET/PUT` | `net_get_thermal_palette` / `net_set_thermal_palette`           | Get thermal palette / Set thermal palette     | `palette`              | `palette`              |
| `thermal/thermal_overlay`      | `GET/PUT` | `net_get_thermal_overlay` / `net_set_thermal_overlay`           | Get thermal overlay / Set thermal overlay     | `thermal_overlay_info` | `thermal_overlay_info` |
| `thermal/thermal_unit`         | `GET/PUT` | `net_get_thermal_unit` / `net_set_thermal_unit`                 | Get thermal units / Set thermal units         | `unit_info`            | `unit_info`            |
| `thermal/capture/{mode}`       | `GET`     | `net_get_thermal_capture`                                       | Get thermal capture file                      | -                      | `capture_file`         |
| `thermal/marker`               | `GET/PUT` | `net_get_thermal_markers` / `net_set_thermal_markers`           | Get thermal measurement marker list / Set thermal measurement marker list | `marker_info[]` | `marker_info[]` |
| `thermal/temp/alarm`                | `GET/PUT` | `net_get_thermal_temp_alarms` / `net_set_thermal_temp_alarms`             | Get thermal temperature alarm list / Set thermal temperature alarm list | `thermal_temp_alarm[]` | `thermal_temp_alarm[]` |
| `thermal/temp/alarm/interval`                | `GET/PUT` | `net_get_thermal_temp_alarm_interval` / `net_set_thermal_temp_alarm_interval`             | Get thermal temperature alarm interval / Set thermal temperature alarm interval | `thermal_temp_alarm_interval` | `thermal_temp_alarm_interval` |
| `thermal/dpc`                  | `PUT`     | `net_set_thermal_dpc`                                           | Set thermal defective pixels                  | `dpc_params`           | -                      |
| `thermal/data/{type}`                 | `GET`     | `net_get_thermal_data`                                          | Get thermal real-time data                    | - | `thermal_data`         |


## Local Extension APIs (No Standalone HTTP Mapping)


| Header Symbol | API Name | Description |
| --- | --- | --- |
| `net_search_devices` | Search devices | LAN device discovery |
| `net_set_exception_callback` | Set exception callback | Disconnect / reconnect / video lost |
| `net_start_alarm_listener` / `net_stop_alarm_listener` | Start/stop alarm listener | Takes `NET_HANDLE`; returns/consumes `alarm_listener_handle` |
| `net_start_stream_play` / `net_stop_stream_play` | Start/stop video stream playback | Returns/consumes `stream_handle` |
| `net_set_stream_callback` | Set video stream callback | Input is `stream_handle` |
| `net_start_stream_record` / `net_stop_stream_record` | Start/stop local recording | Input is `stream_handle` |
| `net_start_thermal_data_stream` / `net_stop_thermal_data_stream` | Start/stop thermal data stream | On start, pass `thermal_data_config` (including `sample_interval` in milliseconds) plus a callback; handshake `role=thermal` |


## Data Types

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
- `thermal_temp_alarm.alarm_type` (`net_enum_thermal_temp_alarm_type`):
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
- `alarm_trigger.type` / `alarm_info.type`: `thermal_temp` (SDK: `NET_ALARM_THERMAL_TEMP`)
- `thermal/capture/{mode}`:
  - `0` (or omitted): output a raw JPEG image
  - `1`: output a TCAP binary container that includes a JPEG image and a raw thermal data frame
- `color_dist.type`:
  - `1`: `t_twb` — high-temperature-difference equalization imaging distribution
  - `2`: `temp_linear` — temperature linear distribution
  - `3`: `ad_linear` — signal linear distribution
- `gain.mode`: `fixed` / `auto`
- `sr_mode.mode`: `none` / `sr`
- `unit_info.temp_unit`: `centigrade` / `kelvin` / `fahrenheit`
- `unit_info.distance_unit`: `meter` / `foot`
- `thermal_data_request.data_types`, `thermal_data_config.data_types`, and `thermal_data.data_types` are bitwise combination numeric enums:
  - `0x01`: `raw`
  - `0x02`: `global`
  - `0x04`: `marker`
- `thermal_data` is binary data, consistent with `net_thermal_data`; `marker_temps` is a fixed-length array. Alarm details are not in this structure; they are carried by `net_alarm_info` from the alarm listener callback

### Device Management Models

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

List APIs return `ntp_info[]`.

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

List APIs return `network_info[]`.

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

`enable_annoy` is the existing field name in the header; semantically it is the anonymous-login switch. `path_template` corresponds to the template string field in the header.

#### `webhook_info`

```json
{
  "url": "https://example.com/webhook"
}
```

On the SDK side this maps to `net_webhook_info`. The `url` buffer length is `NET_LEN_256`.

### Alarm Linkage Models

#### `alarm_trigger`

```json
{
  "type": "thermal_temp",
  "trigger_ftp_enabled": true,
  "trigger_webhook_enabled": false
}
```

The `{type}` path parameter in `alarm/triggers/{type}` corresponds to the separate `type` argument of the SDK APIs `net_get_alarm_trigger()` / `net_set_alarm_trigger()`. The current SDK enum value is `NET_ALARM_THERMAL_TEMP`. The SDK struct `net_alarm_trigger` contains only `trigger_ftp_enabled` and `trigger_webhook_enabled`; `type` is passed separately as an API argument.

### User Management Models

#### `user_info`

```json
{
  "id": 1,
  "user_level": "root",
  "user_name": "admin"
}
```

List APIs return `user_info[]`.

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

### Video Stream Models

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

List APIs return `stream_info[]`.

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

### Display Settings Models

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

The SDK uses `net_enum_image_flip_mode` directly.

#### `channel_info`

```json
{
  "id": 1,
  "name": "channel1"
}
```

List APIs return `channel_info[]`.

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

### Thermal Settings Models

#### `shutter_mode`

```json
{
  "mode": "automatic"
}
```

The SDK uses `net_enum_shutter_mode` directly.

#### `shutter_time`

```json
{
  "interval": 10
}
```

The SDK uses `USHORT minutes` directly.

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

The SDK uses `USHORT range_id` directly.

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

The GET API of `thermal/temp/alarm` returns `thermal_temp_alarm[]`; the PUT API uses `thermal_temp_alarm[]` as the request body. On the SDK side this maps to `net_thermal_temp_alarm` (`net_get_thermal_temp_alarms` / `net_set_thermal_temp_alarms`).

#### `thermal_temp_alarm_interval`

```json
{
  "interval": 10
}
```

On the SDK side this maps to `net_thermal_temp_alarm_interval` (`net_get_thermal_temp_alarm_interval` / `net_set_thermal_temp_alarm_interval`).

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

The SDK uses `net_enum_color_dist_type` directly.

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

When `mode` is `fixed`, `min`/`max` specify the temperature span range manually; when `mode` is `auto`, the device adjusts automatically. When `fixedSpan` is enabled, a fixed span `span` is used (range 0.1–5750, default 2.0).

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

List APIs return `preset_palettes`.

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

List APIs return `marker_info[]`.

#### `capture_file`

`thermal/capture/{mode}` is a GET API; `{mode}` is a path parameter:

- `/thermal/capture/0` or `/thermal/capture` — output a raw JPEG image
- `/thermal/capture/1` — output a TCAP binary container that includes a JPEG image and a raw thermal data frame

The SDK uses `net_enum_thermal_capture_mode mode` directly.

```json
{
  "local_file": "D:/captures/thermal_capture.jpg"
}
```

HTTP response content:

| mode | Response Content-Type | Content |
| --- | --- | --- |
| `0` (or omitted) | `image/jpeg` | Raw JPEG image |
| `1` | `application/octet-stream` | TCAP binary container containing a JPEG image + a serialized raw thermal data frame |

TCAP container format (mode 1):

| Offset | Size | Content |
| --- | --- | --- |
| `0` | `4` | Magic number `"TCAP"` (`0x50414354`, little-endian) |
| `4` | `4` | JPEG data length `jpeg_len` (`uint32`, little-endian) |
| `8` | `4` | Raw data length `raw_len` (`uint32`, little-endian) |
| `12` | `jpeg_len` | JPEG image data |
| `12 + jpeg_len` | `raw_len` | Raw thermal data frame produced by `tframe_serialize()` (same frame format as RTP transport) |

### Other Public Models

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

On the SDK side this maps to `net_exception_info`, delivered via the `net_set_exception_callback()` callback.

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

The SDK uses `net_enum_sr_mode` directly.

#### `thermal_data_config`

```json
{
  "data_types": 7,
  "sample_interval": 1000
}
```

This model is passed together to `net_start_thermal_data_stream()`. `data_types` is a bitwise combination; `sample_interval` is the sampling interval in milliseconds (minimum 50). Parameters are bound to the connection via the RTP handshake `GET /session?role=thermal&data_types=…&sample_interval=…`; `PUT /thermal/data/config` is no longer sent.

The SDK uses a `UINT32 data_types` bitmask argument (`RAW | GLOBAL | MARKER`) directly.

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

Generic alarm-listener payload, corresponding to `net_alarm_info`.

The SDK callback (binary) contains `type` / `alarm_info_length` / `alarm_info`. The Webhook POST JSON contains only:

```json
{
  "type": 1,
  "alarm_info": { "...": "see alarm_thermal_temp_info; does not include raw / message_length" }
}
```

When `type` is `NET_ALARM_THERMAL_TEMP` (=1), `alarm_info` is interpreted as `net_alarm_thermal_temp_info`.

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

On the SDK side this maps to `net_alarm_thermal_temp_info`: `dev_info` is sent directly in the firmware alarm payload and contains the source device IP, port, and channel ID; the valid count of `marker_alarms` is given by `marker_alarms_count` (may include `global`); the valid count of `marker_temps` is given by `markers_count` (does not include `global`); `message` is valid only during the callback. Delivered via the `net_start_alarm_listener()` callback; it is no longer attached to `net_thermal_data`.

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

`raw_frame` corresponds to `net_raw_frame` in the header:

| Field | Type | Description |
| --- | --- | --- |
| `width` | `USHORT` | Thermal width |
| `height` | `USHORT` | Thermal height |
| `range_id` | `USHORT` | Range ID |
| `data` | `const USHORT*` | Raw AD data, length `points_count` |
| `points_count` | `UINT32` | Number of raw AD samples, typically `width * height` |
| `lut_len` | `UINT32` | LUT sample count |
| `lut_env_temp` | `UINT32` | LUT ambient temperature |
| `sensor_temp` | `UINT32` | Sensor temperature |
| `shutter_state` | `UINT32` | Shutter state |
| `frame_no` | `UINT32` | Frame number |
| `pts` | `UINT64` | Synchronization timestamp |
| `raw_type` | `UINT32` | Frame type |
| `lut_temp_value` | `const float*` | LUT temperature array, length `lut_len` |
| `lut_ad_value` | `const USHORT*` | LUT AD array, length `lut_len` |
| `shutter_adv` | `UINT32` | Shutter temperature |

