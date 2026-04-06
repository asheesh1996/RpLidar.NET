---
layout: home
title: Home
nav_order: 1
---

# RpLidar.NET
{: .fs-9 }

A .NET Standard 2.0 library for communicating with SLAMTEC RPLidar sensors over a serial port.
{: .fs-6 .fw-300 }

[![NuGet Version](https://img.shields.io/nuget/v/RpLidar.NET?style=flat-square&label=NuGet)](https://www.nuget.org/packages/RpLidar.NET)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RpLidar.NET?style=flat-square)](https://www.nuget.org/packages/RpLidar.NET)
[![CI](https://github.com/asheesh1996/RpLidar.NET/actions/workflows/publish.yml/badge.svg)](https://github.com/asheesh1996/RpLidar.NET/actions/workflows/publish.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://github.com/asheesh1996/RpLidar.NET/blob/main/LICENSE)

[Get Started](getting-started){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 }
[View on GitHub](https://github.com/asheesh1996/RpLidar.NET){: .btn .fs-5 .mb-4 .mb-md-0 }

---

## What is RpLidar.NET?

**RpLidar.NET** is a cross-platform .NET library for reading 2D point-cloud data from [SLAMTEC RPLidar](https://www.slamtec.com/en/Lidar) sensors. It supports the full SLAMTEC serial protocol (v2.1) and exposes both a simple high-level API for quick projects and a rich low-level API for full control.

---

## Features

| Feature | Details |
|---|---|
| **Supported sensors** | A1M8, A2M8, A3M1 |
| **Scan modes** | Standard, Boost (Express), Sensitivity (Ultra-Capsule) |
| **Architecture** | Event-driven — subscribe to scan events, receive `LidarPoint` lists |
| **Cross-platform** | Windows, Linux, macOS, Raspberry Pi |
| **Target framework** | .NET Standard 2.0 (compatible with .NET 5/6/7/8, .NET Framework 4.x, Mono) |
| **Package** | Available on [NuGet](https://www.nuget.org/packages/RpLidar.NET) |

---

## Quick Install

```shell
dotnet add package RpLidar.NET
```

---

## 30-Second Example

```csharp
using RpLidar.NET;

// Auto-connects and starts scanning
using var lidar = new RPLidar("COM3");

lidar.LidarPointScanEvent += points =>
{
    foreach (var pt in points.Where(p => p.IsValid))
        Console.WriteLine($"Angle: {pt.Angle:F1}°  Distance: {pt.Distance:F0} mm  Quality: {pt.Quality}");
};

Console.ReadLine(); // Keep running
```

---

## Supported Hardware

| Model | Scan Modes | Max Range |
|---|---|---|
| **A1M8** | Standard, Sensitivity | 12 m |
| **A2M8** | Standard, Boost, Sensitivity | 18 m |
| **A3M1** | Standard, Boost, Sensitivity | 25 m |

---

## Next Steps

- [Getting Started](getting-started) — Installation, platform setup, and usage patterns
- [API Reference](api/) — Every class, method, and property documented
- [Support](support) — Reporting bugs and requesting features
