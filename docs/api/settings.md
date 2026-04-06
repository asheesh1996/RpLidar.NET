---
layout: default
title: Settings
parent: API Reference
nav_order: 3
---

# Settings
{: .no_toc }

Namespace: `RpLidar.NET.Entities`
{: .label .label-green }

Configuration types for controlling how `RpLidarSerialDevice` connects and scans.
{: .fs-5 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## ILidarSettings

```csharp
public interface ILidarSettings
```

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Port` | `string` | Last available port | Serial port name, e.g. `"COM3"` or `"/dev/ttyUSB0"` |
| `BaudRate` | `int` | `115200` | Serial baud rate. RPLidar sensors use 115 200 bps. |
| `ScanMode` | `ScanMode` | `ScanMode.Sensitivity` | The scan algorithm to use. |
| `MaxDistance` | `int` | `25000` | Maximum distance in millimetres. Points beyond this are discarded. |
| `Pwm` | `ushort` | `660` | Motor PWM value (0–1023). Higher = faster rotation. |
| `ElapsedMilliseconds` | `int` | `400` | Interval in ms between `LidarPointScanEvent` firings. |

---

## LidarSettings

```csharp
public sealed class LidarSettings : ILidarSettings
```

`LidarSettings` is the concrete, default implementation of `ILidarSettings`. Construct it, override only what you need, and pass it to `RpLidarSerialDevice`.

### Default Values

| Property | Default |
|---|---|
| `Port` | Last available serial port on the system |
| `BaudRate` | `115200` |
| `ScanMode` | `ScanMode.Sensitivity` |
| `MaxDistance` | `25000` mm (25 m) |
| `Pwm` | `660` |
| `ElapsedMilliseconds` | `400` ms |

### Example — All Defaults

```csharp
var settings = new LidarSettings();
// Uses the last serial port, Sensitivity mode, 115200 baud
var device = new RpLidarSerialDevice(settings);
```

### Example — Custom Settings

```csharp
var settings = new LidarSettings
{
    Port                = "COM5",
    BaudRate            = 115200,
    ScanMode            = ScanMode.Standard,   // Compatible with A1M8
    MaxDistance         = 8000,                // Only keep points within 8 m
    Pwm                 = 800,                 // Slightly faster motor
    ElapsedMilliseconds = 200                  // Fire events every 200 ms
};
```

---

## Tuning Recommendations

| Scenario | Recommended Settings |
|---|---|
| A1M8 sensor | `ScanMode.Standard`, `MaxDistance = 12000` |
| A2M8 / A3M1 — best accuracy | `ScanMode.Sensitivity` (default) |
| A2M8 / A3M1 — fastest updates | `ScanMode.Boost`, `ElapsedMilliseconds = 100` |
| Obstacle detection (near range) | `MaxDistance = 3000`, `ElapsedMilliseconds = 200` |
| Mapping (large space) | `MaxDistance = 20000`, `ElapsedMilliseconds = 500` |

{: .tip }
Set `MaxDistance` to the physical size of your environment. Filtering out far points reduces noise and speeds up processing in your event handler.

---

## See Also

- [RpLidarSerialDevice](rplidarserialdevice) — consumes `ILidarSettings`
- [ScanMode](enums#scanmode) — scan mode options
