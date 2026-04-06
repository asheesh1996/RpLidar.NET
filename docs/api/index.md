---
layout: default
title: API Reference
nav_order: 3
has_children: true
---

# API Reference
{: .no_toc }

Complete documentation for all public types, methods, and properties in **RpLidar.NET**.
{: .fs-6 .fw-300 }

---

## Namespaces

| Namespace | Contents |
|---|---|
| `RpLidar.NET` | `RPLidar`, `RpLidarSerialDevice`, `ILidarService`, `Constants` |
| `RpLidar.NET.Entities` | All data types: `LidarPoint`, `LidarPointGroup`, settings, enums, responses |
| `RpLidar.NET.Helpers` | Internal helper utilities (not typically used directly) |

---

## Type Overview

### Entry Points

| Type | Kind | Description |
|---|---|---|
| [`RPLidar`](rplidar) | Class | High-level wrapper — auto-connects and starts scanning |
| [`RpLidarSerialDevice`](rplidarserialdevice) | Class | Low-level serial device with full protocol control |
| `ILidarService` | Interface | Shared contract implemented by `RpLidarSerialDevice` |

### Data Types

| Type | Kind | Description |
|---|---|---|
| [`LidarPoint`](entities#lidarpoint) | Class | Single measurement: angle, distance, quality |
| [`LidarPointGroup`](entities#lidarpointgroup) | Sealed Class | Angle-indexed collection of points from one sweep |
| [`LidarPointGroupItem`](entities#lidarpointgroupitem) | Sealed Class | One slot in a `LidarPointGroup` (per 0.1° bin) |
| [`InfoDataResponse`](entities#infodataresponse) | Class | Device info: model, firmware, serial number |
| [`RpHealthResponse`](entities#rphealthresponse) | Class | Device health status |
| [`ResponseDescriptor`](entities#responsedescriptor) | Class | Raw protocol response descriptor |

### Configuration

| Type | Kind | Description |
|---|---|---|
| [`ILidarSettings`](settings#iliidarsettings) | Interface | Settings contract |
| [`LidarSettings`](settings#lidarsettings) | Sealed Class | Default settings implementation |

### Enumerations

| Type | Description |
|---|---|
| [`ScanMode`](enums#scanmode) | Standard, Boost, Sensitivity |
| [`Command`](enums#command) | Protocol command bytes |
| [`RpDataType`](enums#rpdatatype) | Response data type identifiers |
| [`SendMode`](enums#sendmode) | Single or multi-response request mode |

### Delegates

| Delegate | Signature | Description |
|---|---|---|
| `LidarPointScanEvenHandler` | `(List<LidarPoint>)` | Fired per scan event with all points |
| `LidarPointGroupScanEvenHandler` | `(LidarPointGroup)` | Fired per scan event with grouped points |
