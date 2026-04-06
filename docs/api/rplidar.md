---
layout: default
title: RPLidar
parent: API Reference
nav_order: 1
---

# RPLidar
{: .no_toc }

Namespace: `RpLidar.NET`
{: .label .label-green }

High-level wrapper that auto-connects, resets, and starts scanning on construction.
{: .fs-5 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Definition

```csharp
public class RPLidar : IDisposable
```

`RPLidar` wraps `RpLidarSerialDevice` with sensible defaults. Construction immediately connects to the sensor and begins scanning using `ScanMode.Sensitivity`. Call `Dispose()` (or use a `using` statement) to stop the motor and close the serial port.

---

## Constructors

### `RPLidar(string serialport, int baudrate = 115200)`

Creates a new `RPLidar`, connects to the sensor, and starts scanning.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `serialport` | `string` | — | Serial port name, e.g. `"COM3"` or `"/dev/ttyUSB0"` |
| `baudrate` | `int` | `115200` | Baud rate. RPLidar sensors default to 115 200. |

**Example**

```csharp
using var lidar = new RPLidar("COM3");
// Scanning has already begun
```

{: .note }
The constructor is **blocking** during the initial reset and calibration sequence. Expect ~1–2 seconds before the first `LidarPointScanEvent` fires.

---

## Events

### `LidarPointScanEvent`

```csharp
public event LidarPointScanEvenHandler LidarPointScanEvent;
```

Fired approximately every 400 ms (default `ElapsedMilliseconds`) with a list of `LidarPoint` measurements from the current sweep.

**Signature**

```csharp
delegate void LidarPointScanEvenHandler(List<LidarPoint> points);
```

| Parameter | Type | Description |
|---|---|---|
| `points` | `List<LidarPoint>` | All measurements in the current event window. Some may have `IsValid = false`. |

**Example**

```csharp
lidar.LidarPointScanEvent += points =>
{
    var valid = points.Where(p => p.IsValid).ToList();
    Console.WriteLine($"{valid.Count} valid points this sweep");
};
```

---

## Methods

### `Stop()`

```csharp
public void Stop()
```

Stops the scan and the motor. After calling `Stop()`, no further events will fire. You may call `Dispose()` afterwards.

---

### `Dispose()`

```csharp
public void Dispose()
```

Stops scanning, stops the motor, and releases the serial port. Called automatically at the end of a `using` block.

---

## Remarks

- `RPLidar` always uses `ScanMode.Sensitivity`. For other scan modes or advanced settings, use [`RpLidarSerialDevice`](rplidarserialdevice) directly.
- The underlying serial thread is a background thread; your application will exit cleanly even if you forget to call `Dispose()`.
- All event callbacks are raised on the background scan thread. Marshal to the UI thread if updating UI controls.

---

## See Also

- [RpLidarSerialDevice](rplidarserialdevice) — full low-level control
- [LidarPoint](entities#lidarpoint) — the measurement data type
- [LidarSettings](settings) — configuring scan mode, PWM, and intervals
