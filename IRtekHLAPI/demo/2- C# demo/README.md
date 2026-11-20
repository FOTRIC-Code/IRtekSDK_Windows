# IRtekHLAPI C# Demo

A WinForms application demonstrating the device HL HTTP API (same UI as the IRtekNetSDK 2- C# demo).

## Requirements

- Visual Studio 2019 or later
- .NET Framework 4.5+
- FFmpeg 5.1.2 runtime DLLs under `../../library/x64` or `../../library/x86`
- Reachable thermal camera HTTP + RTSP services (Basic Auth for HTTP)

## Project Structure

```
IRtekHLAPI/          - Managed HLAPI HTTP client (IRtekHLAPI.dll)
IRtekHLAPIDemo/      - Main demo application
../../library/api.md - HL API reference used by this demo
../../library/api_en.md - HL API reference used by this demo
../../library/x64/   - FFmpeg 5.1.2 x64 runtime DLLs (copied to output on x64 build)
../../library/x86/   - FFmpeg 5.1.2 x86 runtime DLLs (copied to output on x86 build)
```

## Features

Same surface as the NetSDK C# demo where possible, implemented over HTTP per `api.md`. Live preview uses RTSP + FFmpeg; alarm listener uses the current login session (`GET /session?role=alarm` on the RTP port).

### Connection
- **Connect/Disconnect** — Basic Auth session to `http://{ip}:{port}`

### Live Preview & Thermal Data
- **RTSP live preview** — `rtsp://{host}:{rtsp_port}/video/{stream_id}` (same as NetSDK `build_rtsp_url`), decoded to BGRA via FFmpeg
- Marker / global temps via HTTP poll of `GET /thermal/data/{type}` (`net_get_thermal_data`) ~100–200 ms
- Stream ID combo loads HTTP stream metadata (`net_get_streams` / `net_get_stream`); changing stream restarts RTSP
- **Capture** — pure JPEG only via `GET /thermal/capture/0` (`NetThermalCaptureMode.PureImage`)

### Alarms
- **Start Listener / Stop Listener** — `net_start_alarm_listener` / `net_stop_alarm_listener` on the current login session (`GET /session?role=alarm` on the device RTP port)
- Alarm rules / triggers still use HTTP get/set APIs; Network tab can still get/set webhook URL

### Markers / Thermal / Display / Device / Time / Network / Users
- Config APIs map 1:1 to HTTP paths in `api.md`

## Build

Open `IRtekHLAPIDemo/IRtekHLAPIDemo.sln` in Visual Studio, select **x64 / Debug** or **x86 / Debug**, and build.

Post-build copies matching FFmpeg DLLs from `IRtekHLAPI/library/x64` or `library/x86` into the output folder. No native `IRtekNetSDK.dll` is required.

## Usage

1. Enter device IP, port (default 80), username, and password
2. Click **Connect** to login; RTSP preview and thermal HTTP poll start automatically
3. Use the toolbar to draw markers or capture pure JPEG images
4. On the Alarm tab, **Start Listener** to receive alarm callbacks on the current login session
5. Use the right-side tabs to configure device settings via HLAPI
