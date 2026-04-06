# RpLidar.NET

[![NuGet Version](https://img.shields.io/nuget/v/RpLidar.NET?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RpLidar.NET)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RpLidar.NET?logo=nuget)](https://www.nuget.org/packages/RpLidar.NET)
[![Build & Publish](https://github.com/asheesh1996/RpLidar.NET/actions/workflows/publish.yml/badge.svg)](https://github.com/asheesh1996/RpLidar.NET/actions/workflows/publish.yml)

A .NET library for interfacing with **Slamtec RPLidar A-series** LiDAR sensors over a serial connection.
Tested with real hardware (A1M8). Targets **netstandard2.0** — compatible with .NET 6, 7, 8, and .NET Framework 4.6.1+.

---

## Supported Hardware

| Model   | Scan Mode          | Max Range |
|---------|--------------------|-----------|
| A1M8    | Standard / Sensitivity | 12 m  |
| A2M8    | Standard / Boost / Sensitivity | 18 m |
| A3M1    | Standard / Boost / Sensitivity | 25 m |

---

## Installation

```shell
dotnet add package RpLidar.NET
```

Or via the NuGet Package Manager in Visual Studio: search for **RpLidar.NET**.

---

## Quick Start

```csharp
using RpLidar.NET;
using RpLidar.NET.Entities;

// Replace with your actual port:
//   Windows : "COM3"
//   Linux   : "/dev/ttyUSB0"
//   macOS   : "/dev/tty.usbserial-..."
using var lidar = new RPLidar("COM3");

lidar.LidarPointScanEvent += points =>
{
    foreach (var p in points)
        Console.WriteLine($"Angle: {p.Angle:F1}°  Distance: {p.Distance:F0} mm  Quality: {p.Quality}");
};

Console.WriteLine("Scanning — press Enter to stop.");
Console.ReadLine();
```

The `RPLidar` constructor **connects, starts the motor, and begins scanning immediately**.
Scan data is delivered via `LidarPointScanEvent` approximately every 400 ms (configurable).

---

## Scan Modes

| Mode | Enum Value | Protocol | Description |
|------|-----------|----------|-------------|
| Standard | `ScanMode.Standard` | Scan (0x20) | Basic scan, ~2,000 samples/rev. All A-series. |
| Boost | `ScanMode.Boost` | Express Scan (0x82, mode=3) | Higher frequency. A2/A3 only. |
| Sensitivity | `ScanMode.Sensitivity` | Express Scan (0x82, mode=4) | Ultra-Capsule; highest density. **Default.** |

---

## Custom Configuration

Use `RpLidarSerialDevice` directly for full control:

```csharp
using RpLidar.NET;
using RpLidar.NET.Entities;

var settings = new LidarSettings
{
    Port                = "COM3",
    BaudRate            = 115200,
    ScanMode            = ScanMode.Sensitivity,
    Pwm                 = 660,     // Motor speed: 0 (stop) – 1023 (full)
    MaxDistance         = 25000,   // mm — consumers can filter by this
    ElapsedMilliseconds = 400,     // Scan event fires at most every 400 ms
};

using var device = new RpLidarSerialDevice(settings);
device.LidarPointScanEvent += OnScan;
device.Start();

Console.ReadLine();
device.Stop();

static void OnScan(List<LidarPoint> points)
{
    Console.WriteLine($"{points.Count} points");
}
```

---

## API Reference

### `RPLidar` — high-level wrapper

| Member | Description |
|--------|-------------|
| `RPLidar(string port, int baudrate = 115200)` | Connect and start scanning. |
| `event LidarPointScanEvent` | Raised with batched scan points. |
| `Stop()` | Stop scanning and motor. |
| `Dispose()` | Stop and release all resources. |

### `RpLidarSerialDevice` — low-level device

| Member | Description |
|--------|-------------|
| `Connect()` | Open the serial port. |
| `Disconnect()` | Close the serial port. |
| `StartMotor()` | Enable motor at the configured PWM. |
| `StopMotor()` | Set PWM to 0 and stop motor. |
| `StartScan()` | Start Standard scan and read thread. |
| `ForceScan()` | Force scan (no motor-sync check). |
| `ForceScanExpress()` | Start Express scan (Boost or Sensitivity). |
| `StopScan()` | Stop the read thread and motor. |
| `Start()` | `Connect` → `StopScan` → pick scan mode → start. |
| `Stop()` | `StopScan`. |
| `SendRequest(Command)` | Send a raw protocol command. |
| `event LidarPointScanEvent` | Raw point list, fired on timer. |
| `event LidarPointGroupScanEvent` | Angle-keyed `LidarPointGroup`, fired on timer. |

### `LidarPoint`

| Property | Type | Description |
|----------|------|-------------|
| `Angle` | `float` | Heading in degrees (0 – 360). |
| `Distance` | `float` | Distance in millimetres. |
| `Quality` | `ushort` | Measurement quality (higher = better). |
| `StartFlag` | `bool` | `true` at the start of a new 360° revolution. |
| `IsValid` | `bool` | `true` when `Distance > 0`. |

### `LidarPointGroup`

An angle-keyed collection (0.1° resolution) built from a batch of `LidarPoint` values.
Useful for per-angle lookups:

```csharp
// Nearest obstacle directly ahead
var ahead = group[0];   // angle = 0°
if (ahead != null)
    Console.WriteLine($"Obstacle at {ahead.Distance} mm");
```

---

## Platform Notes

- **Windows**: Use `COMn` port names (e.g. `COM3`). The USB-UART adapter appears in Device Manager.
- **Linux**: Typically `/dev/ttyUSB0`. You may need to add your user to the `dialout` group:
  ```shell
  sudo usermod -aG dialout $USER
  ```
- **macOS**: `/dev/tty.usbserial-*` or `/dev/tty.SLAB_USBtoUART`.

---

## Building from Source

```shell
git clone https://github.com/asheesh1996/RpLidar.NET.git
cd RpLidar.NET
dotnet build
```

### Creating a NuGet package

```shell
dotnet pack RpLidar.NET/RpLidar.NET.csproj -c Release -o ./nupkg
```

The `.nupkg` file will be in `./nupkg/`.

### Publishing to NuGet.org

```shell
dotnet nuget push ./nupkg/RpLidar.NET.*.nupkg \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

---

## License

[MIT](LICENSE)
