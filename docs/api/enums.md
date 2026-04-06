---
layout: default
title: Enumerations
parent: API Reference
nav_order: 5
---

# Enumerations
{: .no_toc }

Namespace: `RpLidar.NET.Entities`
{: .label .label-green }

All enumerations used in the RpLidar.NET API.
{: .fs-5 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## ScanMode

```csharp
public enum ScanMode : byte
```

Selects the scanning algorithm used by `RpLidarSerialDevice`. Set via `ILidarSettings.ScanMode`.

| Member | Value | Description |
|---|---|---|
| `Standard` | `0` | Basic scan command (0x20). Widest hardware compatibility — works on **all** supported models. Lower sample rate. |
| `Boost` | `3` | Express Scan with `working_mode=3`. Faster sample rate. **A2M8 and A3M1 only.** |
| `Sensitivity` | `4` | Ultra-Capsule Express Scan with `working_mode=4`. Best accuracy and range. **Default.** **A2M8 and A3M1 only.** |

{: .important }
Using `Boost` or `Sensitivity` with an A1M8 will not raise an exception — the firmware silently ignores the working mode and uses Standard scan instead.

---

## Command

```csharp
public enum Command : byte
```

Protocol command bytes as defined in the SLAMTEC RPLidar Communication Protocol v2.1.

| Member | Value | Has Response | Description |
|---|---|---|---|
| `Stop` | `0x25` | No | Stop the current scan. |
| `Reset` | `0x40` | No | Reboot the sensor core. |
| `Scan` | `0x20` | Yes (multi) | Begin standard scan. |
| `ExpressScan` | `0x82` | Yes (multi) | Begin express (Boost/Sensitivity) scan. |
| `ForceScan` | `0x21` | Yes (multi) | Force scan without motor sync. |
| `GetInfo` | `0x50` | Yes (single) | Request device information. |
| `GetHealth` | `0x52` | Yes (single) | Request device health status. |
| `GetSampleRate` | `0x59` | Yes (single) | Request sample rate info. |
| `GetLidarConf` | `0x84` | Yes (single) | Request lidar configuration. |
| `StartPwm` | `0xF0` | No | Set motor PWM speed. |

```csharp
// Sending a raw command
var info = device.SendRequest(Command.GetInfo) as InfoDataResponse;
```

---

## RpDataType

```csharp
public enum RpDataType : byte
```

Identifies the payload type in a protocol response descriptor. Used internally and exposed in `IDataResponse.Type`.

| Member | Value | Description |
|---|---|---|
| `Scan` | `0x81` | Scan measurement data. |
| `GetInfo` | `0x04` | Device information response. |
| `GetHealth` | `0x06` | Device health response. |

---

## SendMode

```csharp
public enum SendMode : byte
```

Encodes the response mode in the 7-byte protocol descriptor. Used internally.

| Member | Value | Description |
|---|---|---|
| `SingleRequestSingleResponse` | `0x0` | One request yields exactly one response packet. |
| `SingleRequestMultipleResponse` | `0x1` | One request yields a continuous stream of response packets (used for scanning). |
| `ReservedForFutureUse1` | `0x2` | Reserved — do not use. |
| `ReservedForFutureUse2` | `0x3` | Reserved — do not use. |

---

## See Also

- [RpLidarSerialDevice](rplidarserialdevice) — uses `Command` and `ScanMode`
- [ILidarSettings](settings) — `ScanMode` is set here
- [Data Types](entities) — `RpDataType` appears in response objects
