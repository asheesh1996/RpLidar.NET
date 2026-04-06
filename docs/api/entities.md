---
layout: default
title: Data Types
parent: API Reference
nav_order: 4
---

# Data Types
{: .no_toc }

Namespace: `RpLidar.NET.Entities`
{: .label .label-green }

All measurement and response data types returned by the library.
{: .fs-5 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## LidarPoint

```csharp
public class LidarPoint
```

Represents a single distance measurement at a given angle. This is the fundamental unit of scan data you receive in `LidarPointScanEvent`.

### Properties

| Property | Type | Description |
|---|---|---|
| `Angle` | `float` | Measurement angle in degrees (0–360). |
| `Distance` | `float` | Distance in millimetres. `0` means the sensor could not measure at this angle. |
| `Quality` | `ushort` | Signal quality (higher = better). Typical range 0–63. |
| `StartFlag` | `bool` | `true` for the first point of a new 360° revolution. |
| `Flag` | `int` | Raw internal flag byte from the protocol packet. |
| `IsValid` | `bool` | Computed: `true` when `Distance > 0`. Always check this before using `Distance`. |

### Example

```csharp
lidar.LidarPointScanEvent += points =>
{
    foreach (var pt in points)
    {
        if (!pt.IsValid) continue;             // skip zero-distance readings
        if (pt.Distance > 5000) continue;      // skip far readings

        Console.WriteLine($"{pt.Angle:F1}° → {pt.Distance:F0} mm (quality {pt.Quality})");
    }
};
```

---

## LidarPointGroup

```csharp
public sealed class LidarPointGroup : IEnumerable<LidarPointGroupItem>
```

An angle-indexed map of scan points at **0.1° resolution** (3 600 possible slots). Received via `LidarPointGroupScanEvent`. Useful for map-building and inter-sweep comparison.

### Constructor

```csharp
public LidarPointGroup(double x, double y)
```

| Parameter | Type | Description |
|---|---|---|
| `x` | `double` | X coordinate of the sensor origin in your world space. |
| `y` | `double` | Y coordinate of the sensor origin in your world space. |

### Properties

| Property | Type | Description |
|---|---|---|
| `X` | `double` | X position of the sensor. |
| `Y` | `double` | Y position of the sensor. |
| `Settings` | `ILidarSettings` | The settings applied when this group was built. |
| `Count` | `int` | Number of occupied angle slots. |
| `Items` | `IEnumerable<LidarPointGroupItem>` | All occupied slots. |

### Indexers

```csharp
public LidarPointGroupItem this[float angle]   // angle in degrees (e.g. 90.5f)
public LidarPointGroupItem this[int angle]     // integer degrees (e.g. 90)
```

Returns the point group item nearest to the requested angle, or `null` if no measurement exists.

### Methods

#### `Add(LidarPoint point)`

Adds a single `LidarPoint` to the group.

#### `AddRange(IEnumerable<LidarPoint> points)`

Adds multiple points at once.

#### `GetPoints()`

```csharp
public List<LidarPoint> GetPoints()
```

Returns all stored points as a flat list.

#### `Filter()`

```csharp
public List<LidarPoint> Filter()
```

Returns a noise-filtered list of points. Removes isolated outliers.

#### `Compare(LidarPointGroup group)`

```csharp
public double Compare(LidarPointGroup group)
```

Compares this group with another and returns a similarity score from **0.0** (completely different) to **1.0** (identical). Useful for change/motion detection.

```csharp
double similarity = sweep1.Compare(sweep2);
if (similarity < 0.85)
    Console.WriteLine("Significant change detected!");
```

---

## LidarPointGroupItem

```csharp
public sealed class LidarPointGroupItem
```

One slot in a `LidarPointGroup`, representing the best measurement for a 0.1° angle bin.

### Properties & Fields

| Member | Type | Description |
|---|---|---|
| `Angle` | `int` | Angle key (degrees × 10, e.g. 905 = 90.5°). |
| `OriginalAngle` | `float` | The original float angle from the sensor. |
| `Distance` | `float` | Distance in millimetres. |
| `Count` | `int` | Number of raw points that contributed to this slot. |
| `Cathetus` | `double?` | Optional computed perpendicular component. |
| `CanIgnore` | `bool` | Marks this item as a noise outlier (set by `Filter()`). |

---

## InfoDataResponse

```csharp
public class InfoDataResponse : IDataResponse
```

Returned by `device.SendRequest(Command.GetInfo)`.

### Properties

| Property | Type | Description |
|---|---|---|
| `Type` | `RpDataType` | Always `RpDataType.GetInfo`. |
| `ModelId` | `string` | Sensor model identifier (e.g. `"A1M8"`). |
| `FirmwareVersion` | `string` | Firmware version string. |
| `HardwareVersion` | `string` | Hardware revision string. |
| `SerialNumber` | `string` | Unique device serial number. |

```csharp
var info = device.SendRequest(Command.GetInfo) as InfoDataResponse;
Console.WriteLine($"Model: {info?.ModelId}, FW: {info?.FirmwareVersion}");
```

---

## RpHealthResponse

```csharp
public class RpHealthResponse : IDataResponse
```

Returned by `device.SendRequest(Command.GetHealth)`.

### Properties

| Property | Type | Description |
|---|---|---|
| `Type` | `RpDataType` | Always `RpDataType.GetHealth`. |
| `Status` | `int` | `0` = Good, `1` = Warning, `2` = Error. |
| `ErrorCode` | `int` | Non-zero when `Status` is `Warning` or `Error`. |

```csharp
var health = device.SendRequest(Command.GetHealth) as RpHealthResponse;
if (health?.Status != 0)
    Console.WriteLine($"Sensor issue! Status={health.Status} Code={health.ErrorCode}");
```

---

## ResponseDescriptor

```csharp
public class ResponseDescriptor
```

Low-level protocol response descriptor (7-byte header). Typically used internally.

| Property | Type | Description |
|---|---|---|
| `ResponseLength` | `int` | Expected payload length in bytes. |
| `SendMode` | `SendMode` | Single or multi-response. |
| `RpDataType` | `RpDataType` | The data type of the following payload. |

---

## IDataResponse

```csharp
public interface IDataResponse
```

Base interface for all response objects returned by `SendRequest`.

| Member | Type | Description |
|---|---|---|
| `Type` | `RpDataType` | Identifies the concrete response type. |

---

## See Also

- [Enumerations](enums) — `ScanMode`, `Command`, `RpDataType`, `SendMode`
- [RpLidarSerialDevice](rplidarserialdevice) — source of all these responses
