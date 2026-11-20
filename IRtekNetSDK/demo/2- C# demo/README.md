# IRtekNetSDK C# Demo

A WinForms application demonstrating the IRtekNetSDK API for live thermal camera interaction.

## Requirements

- Visual Studio 2019 or later
- .NET Framework 4.7.2
- Windows x64
- IRtekNetSDK native libraries (placed in output directory)

## Project Structure

```
IRtekNetSDK/          - P/Invoke wrapper library (IRtekNetSDK.Interop.dll)
IRtekNetSDKDemo/      - Live preview demo
IRtekNetSDKApiDemo/   - API, alarm, stream, and upgrade test program
```

## Features

### Connection
- **Search** — Discover cameras on the local network via `net_search_devices`
- **Connect/Disconnect** — Login with IP, port, username, password

### Live Video
- BGRA stream callback with real-time display
- Local stream recording (start/stop with folder selection)

### Markers
- Draw Spot, Line, and Polygon markers on the thermal image
- Drag to move or resize markers
- Live temperature display (max/min/avg with coordinates)
- Bulk marker sync via `net_get/set_thermal_markers`

### Thermal Parameters
- Temperature range selection
- Palette selection and temperature bounds
- Emissivity, reflected temperature, distance, humidity
- Shutter mode and interval
- Temperature/distance unit switching
- Color distribution type
- Calibrate (flat-field correction)

### Display & Overlay
- Channel name editing (`net_get/set_channel`)
- Overlay position control (align + x/y) for channel name and time
- Thermal overlays: unit, palette, alarm, global temp, marker temp
- Image parameters: contrast, brightness, DDE
- Super Resolution toggle

### Device Management (Properties Tab)
- Device info display (model, SN, firmware, lens)
- Stream info display
- Device name editing
- Firmware upgrade, factory reset, restart

### Time
- View/set device local time
- NTP server configuration

### Network
- Network adapter settings (IP, mask, gateway, DNS, DHCP)
- FTP server configuration
- Port settings (HTTP, RTSP, RTP)

### User Management
- List users
- Add/modify users with role assignment
- Delete users

### Alarm
- View thermal alarm rules per marker
- FTP trigger enable/disable

## API Demo

`IRtekNetSDKApiDemo` exercises the native API directly. It keeps its own P/Invoke layer in `NetSdk.cs` so struct layouts can be checked against `IRtekNetSDK.dll`. It does not use `IRtekNetSDK.Interop`.

The window covers device search and login, RTSP preview, raw stream playback, alarm listening, alarm query with charts and recording playback, a full API exercise list, and firmware upgrade. Command-line checks are `--ui-check`, `--play-check`, `--layout`, and `--sizes`.

It targets .NET Framework 4.6 and restores Newtonsoft.Json, SSH.NET, and System.ValueTuple. Alarm recordings are read from the device path `/userdata/parkia/record`.

In `IRtekNetSDKDemo.sln`, set **IRtekNetSDKApiDemo** as the startup project and build **x86** or **x64**. The post-build step copies native libraries from `library\x86` or `library\x64`.

## Build

Open `IRtekNetSDKDemo.sln` in Visual Studio, select **x64 / Debug** or **x86 / Debug**, and build.

Ensure the following DLLs are present in the output directory:
- `IRtekNetSDK.dll`
- FFmpeg libraries (`avcodec`, `avformat`, `avutil`, `swscale`, `swresample`)

## Usage

1. Click **Search** to discover devices, or enter IP manually
2. Fill in port (default 80), username, and password
3. Click **Connect** to establish connection and start live video
4. Use the toolbar to draw markers or capture images
5. Use the right-side tabs to configure device settings
