---
layout: default
title: RpLidarSerialDevice
parent: API Reference
nav_order: 2
---

# RpLidarSerialDevice
{: .no_toc }

Namespace: `RpLidar.NET`
{: .label .label-green }

Low-level serial device implementation of `ILidarService` with full protocol control.
{: .fs-5 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Definition

```csharp
public class RpLidarSerialDevice : ILidarService
```

`RpLidarSerialDevice` implements the complete SLAMTEC serial protocol v2.1. It gives you explicit control over connection, disconnection, scan mode, motor speed, and request/response cycles.

---

## Constructors

### `RpLidarSerialDevice(ILidarSettings settings)`

Creates a new device instance. Does **not** connect to the port until `Connect()` is called.

| Parameter | Type | Description |
|---|---|---|
| `settings` | `ILidarSettings` | Configuration: port, baud rate, scan mode, PWM, etc. |

```csharp
var settings = new LidarSettings
{
    Port     = "/dev/ttyUSB0",
    ScanMode = ScanMode.Standard
};
var device = new RpLidarSerialDevice(settings);
```

---

## Events

### `LidarPointScanEvent`

```csharp
public event LidarPointScanEvenHandler LidarPointScanEvent;
```

Fired every `settings.ElapsedMilliseconds` (default 400 ms) with raw points from the current scan window.

### `LidarPointGroupScanEvent`

```csharp
public event LidarPointGroupScanEvenHandler LidarPointGroupScanEvent;
```

Fired alongside `LidarPointScanEvent` with the same points wrapped in a `LidarPointGroup` (angle-indexed at 0.1° resolution).

---

## Methods — Lifecycle

### `Connect()`

```csharp
public void Connect()
```

Opens the serial port and initialises the device. Call before any other method.

---

### `Disconnect()`

```csharp
public void Disconnect()
```

Closes the serial port. Call after `Stop()`.

---

### `Start()`

```csharp
public void Start()
```

Convenience method that:
1. Calls `Connect()` (if not already connected)
2. Sends `StopScan`
3. Detects the scan mode from `settings.ScanMode`
4. Starts the appropriate scan command
5. Launches the background scan thread

---

### `Stop()`

```csharp
public void Stop()
```

Sends `StopScan` and terminates the background scan thread.

---

### `Dispose()`

```csharp
public void Dispose()
```

Stops the scan, stops the motor, disconnects, and releases all resources.

---

## Methods — Motor Control

### `StartMotor()`

```csharp
public void StartMotor()
```

Sends the motor-start PWM command using `settings.Pwm` (default 660).

---

### `StopMotor()`

```csharp
public void StopMotor()
```

Sends the motor-stop command (PWM = 0). The sensor will stop rotating.

---

## Methods — Scanning

### `StartScan()`

```csharp
public void StartScan()
```

Sends the standard `Scan` command (0x20). Compatible with all supported models.

---

### `ForceScan()`

```csharp
public void ForceScan()
```

Sends the `ForceScan` command (0x21). Bypasses the motor-sync check — use when motor synchronisation is not needed.

---

### `ForceScanExpress()`

```csharp
public void ForceScanExpress()
```

Sends the `ExpressScan` command using `settings.ScanMode`. Issues the **Boost** (`working_mode=3`) or **Sensitivity** (`working_mode=4`) sub-mode as appropriate. Only supported on A2M8 and A3M1.

---

### `StopScan()`

```csharp
public void StopScan()
```

Sends the `Stop` command (0x25) to halt the current scan without stopping the motor.

---

## Methods — Protocol

### `SendRequest(Command command)`

```csharp
public IDataResponse SendRequest(Command command)
```

Sends a single-response command and returns the parsed response.

| Parameter | Type | Description |
|---|---|---|
| `command` | `Command` | The protocol command to send |

**Returns** `IDataResponse` — cast to `InfoDataResponse` or `RpHealthResponse` as appropriate.

```csharp
var info = device.SendRequest(Command.GetInfo) as InfoDataResponse;
Console.WriteLine(info?.ModelId);

var health = device.SendRequest(Command.GetHealth) as RpHealthResponse;
Console.WriteLine(health?.Status); // 0=OK, 1=Warning, 2=Error
```

---

### `SendCommand(byte command, byte[] data = null)`

```csharp
public IDataResponse SendCommand(byte command, byte[] data = null)
```

Low-level raw command send. Prefer `SendRequest` with the `Command` enum for standard commands.

---

### `Read(int len, int timeout)`

```csharp
public byte[] Read(int len, int timeout)
```

Reads exactly `len` bytes from the serial port, waiting up to `timeout` milliseconds.

---

## Remarks

- Call `Connect()` before any other method (or call `Start()` which calls it internally).
- All events fire on a background thread. Do not update UI controls directly from event handlers.
- To change scan mode after construction, create a new `RpLidarSerialDevice` with updated settings.

---

## See Also

- [RPLidar](rplidar) — simpler high-level wrapper
- [ILidarSettings / LidarSettings](settings) — full settings reference
- [Command](enums#command) — all available protocol commands
