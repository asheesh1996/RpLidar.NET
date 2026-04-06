---
layout: default
title: Getting Started
nav_order: 2
---

# Getting Started
{: .no_toc }

This guide covers installation, platform configuration, and the most common usage patterns.
{: .fs-6 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Prerequisites

- **.NET SDK 5.0+** (or any .NET Standard 2.0-compatible runtime)
- A **SLAMTEC RPLidar** sensor: A1M8, A2M8, or A3M1
- The USB-to-serial adapter that ships with the sensor

---

## Installation

### .NET CLI

```shell
dotnet add package RpLidar.NET
```

### Package Manager Console (Visual Studio)

```powershell
Install-Package RpLidar.NET
```

### PackageReference (`.csproj`)

```xml
<PackageReference Include="RpLidar.NET" Version="*" />
```

---

## Platform Setup

| Platform | Port Format | Notes |
|---|---|---|
| Windows | `COM3`, `COM4` … | Check *Device Manager → Ports* |
| Linux | `/dev/ttyUSB0`, `/dev/ttyUSB1` | Run `sudo usermod -aG dialout $USER` and re-login |
| macOS | `/dev/cu.usbserial-*` | No additional setup required |
| Raspberry Pi | `/dev/ttyUSB0` | Same as Linux |

{: .note }
On Linux and Raspberry Pi, always add your user to the `dialout` group before running. Otherwise the serial port open will fail with a permission error.

---

## High-Level API — `RPLidar`

The `RPLidar` class is the quickest way to start scanning. It automatically connects, sends a reset, and begins scanning using the **Sensitivity** (Ultra-Capsule) mode.

```csharp
using RpLidar.NET;

// Pass the serial port name; baud rate defaults to 115200
using var lidar = new RPLidar("COM3");

lidar.LidarPointScanEvent += points =>
{
    Console.WriteLine($"Received {points.Count} points in this sweep");

    foreach (var pt in points.Where(p => p.IsValid))
    {
        Console.WriteLine($"  Angle: {pt.Angle,6:F1}°  Distance: {pt.Distance,7:F0} mm  Quality: {pt.Quality}");
    }
};

Console.WriteLine("Scanning — press Enter to stop.");
Console.ReadLine();
// Disposal calls Stop() automatically
```

---

## Low-Level API — `RpLidarSerialDevice`

Use `RpLidarSerialDevice` with a `LidarSettings` object when you need full control over scan mode, motor PWM, event interval, or maximum distance filtering.

```csharp
using RpLidar.NET;
using RpLidar.NET.Entities;

var settings = new LidarSettings
{
    Port              = "COM3",
    BaudRate          = 115200,
    ScanMode          = ScanMode.Sensitivity, // Standard | Boost | Sensitivity
    MaxDistance       = 12000,               // millimetres — discard farther points
    Pwm               = 660,                 // motor speed 0–1023
    ElapsedMilliseconds = 400               // ms between LidarPointScanEvent fires
};

using var device = new RpLidarSerialDevice(settings);

device.LidarPointScanEvent      += OnScanReceived;
device.LidarPointGroupScanEvent += OnGroupReceived;

device.Connect();
device.Start();

Console.ReadLine();

device.Stop();
device.Disconnect();

// -----------------------------------------------
static void OnScanReceived(List<LidarPoint> points)
{
    // Called every ElapsedMilliseconds with raw points
    Console.WriteLine($"{points.Count} points");
}

static void OnGroupReceived(LidarPointGroup group)
{
    // Called with angle-indexed group (0.1° resolution)
    Console.WriteLine($"Group has {group.Count} unique angles");
}
```

---

## Reading Device Info and Health

```csharp
// Must call Connect() first
device.Connect();

var info = device.SendRequest(Command.GetInfo) as InfoDataResponse;
if (info != null)
{
    Console.WriteLine($"Model      : {info.ModelId}");
    Console.WriteLine($"Firmware   : {info.FirmwareVersion}");
    Console.WriteLine($"Hardware   : {info.HardwareVersion}");
    Console.WriteLine($"Serial No. : {info.SerialNumber}");
}

var health = device.SendRequest(Command.GetHealth) as RpHealthResponse;
if (health != null)
{
    // Status: 0 = Good, 1 = Warning, 2 = Error
    Console.WriteLine($"Health     : {health.Status}  Error code: {health.ErrorCode}");
}
```

---

## Working with `LidarPointGroup`

`LidarPointGroup` stores scan points indexed by angle at 0.1° resolution. It is useful for map-building, obstacle detection, and comparing consecutive sweeps.

```csharp
LidarPointGroup previous = null;

device.LidarPointGroupScanEvent += group =>
{
    // Access a specific angle
    var pt = group[90];             // nearest point at 90°
    var ptF = group[180.5f];        // nearest point at 180.5°

    // Noise-filtered point list
    var filtered = group.Filter();

    // Compare with the previous scan (1.0 = identical, 0.0 = completely different)
    if (previous != null)
    {
        double similarity = previous.Compare(group);
        Console.WriteLine($"Scene similarity: {similarity * 100:F1}%");
    }

    previous = group;
};
```

---

## Scan Modes

| Mode | Enum Value | Protocol | Supported Sensors | Notes |
|---|---|---|---|---|
| **Standard** | `ScanMode.Standard` | Scan (0x20) | A1M8, A2M8, A3M1 | Most compatible, lower sample rate |
| **Boost** | `ScanMode.Boost` | Express Scan, mode 3 | A2M8, A3M1 | Higher sample rate |
| **Sensitivity** | `ScanMode.Sensitivity` | Ultra-Capsule, mode 4 | A2M8, A3M1 | **Default** — best accuracy and range |

{: .warning }
`Boost` and `Sensitivity` modes are **not supported** on the A1M8. Using them will silently fall back to `Standard` scan at the firmware level.

---

## Next Steps

- [API Reference](api/) — Complete class and member documentation
- [Support](support) — How to report a bug or request a feature
