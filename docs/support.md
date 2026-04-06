---
layout: default
title: Support
nav_order: 4
---

# Support
{: .no_toc }

How to get help, report bugs, and request new features for RpLidar.NET.
{: .fs-6 .fw-300 }

## Table of Contents
{: .no_toc .text-delta }

1. TOC
{:toc}

---

## Before You Open an Issue

Please check the following before submitting a new issue — it saves everyone time:

1. **Read the docs** — Browse the [Getting Started](getting-started) guide and [API Reference](api/) first. Many common questions are answered there.
2. **Search existing issues** — Your problem may already be reported or resolved. Search [open and closed issues](https://github.com/asheesh1996/RpLidar.NET/issues?q=is%3Aissue) before creating a new one.
3. **Check your hardware** — Verify the sensor works with the official SLAMTEC tools (RoboStudio) to rule out a hardware or driver problem.
4. **Confirm the serial port** — Make sure the port name is correct (e.g. `COM3`, `/dev/ttyUSB0`) and your user has permission to access it.

---

## How to Report a Bug

[Open a Bug Report](https://github.com/asheesh1996/RpLidar.NET/issues/new?template=bug_report.md&labels=bug){: .btn .btn-red }

A good bug report includes all of the following:

### 1. Environment

```
OS and version        : e.g. Windows 11, Ubuntu 22.04, Raspberry Pi OS Bookworm
.NET version          : e.g. .NET 8.0.3
RpLidar.NET version   : e.g. 1.0.0 (check your .csproj or NuGet)
Sensor model          : e.g. A1M8, A2M8, A3M1
Scan mode used        : e.g. ScanMode.Sensitivity
Connection interface  : e.g. USB, UART
```

### 2. What You Expected

Describe what you expected to happen.

### 3. What Actually Happened

Describe the actual behaviour. Include:
- The **full exception message and stack trace** (if any)
- The **output** of your program
- Whether the issue is consistent or intermittent

### 4. Minimal Reproduction

Provide the **smallest possible code snippet** that reproduces the issue:

```csharp
// Minimal reproduction
using RpLidar.NET;

using var lidar = new RPLidar("COM3");
lidar.LidarPointScanEvent += points =>
{
    // Describe what goes wrong here
};
Console.ReadLine();
```

{: .important }
Issues without a reproduction case may take significantly longer to investigate. Attaching a minimal project (as a `.zip` or linked repository) is even better.

### 5. Diagnostic Info

Where possible, run the health check and include the output:

```csharp
var device = new RpLidarSerialDevice(new LidarSettings { Port = "COM3" });
device.Connect();

var info = device.SendRequest(Command.GetInfo) as InfoDataResponse;
Console.WriteLine($"Model: {info?.ModelId}  FW: {info?.FirmwareVersion}  HW: {info?.HardwareVersion}");

var health = device.SendRequest(Command.GetHealth) as RpHealthResponse;
Console.WriteLine($"Health: {health?.Status}  Error: {health?.ErrorCode}");
```

---

## How to Request a Feature

[Open a Feature Request](https://github.com/asheesh1996/RpLidar.NET/issues/new?template=feature_request.md&labels=enhancement){: .btn .btn-blue }

When requesting a new feature, please describe:

1. **The problem you are trying to solve** — not just the solution. "I want X" is less useful than "When doing Y, I cannot Z because…"
2. **Your proposed solution** — how you imagine the API or behaviour should work.
3. **Alternatives you have considered** — any workarounds you tried.
4. **Who else would benefit** — is this specific to your use case, or broadly useful?

---

## Issue Labels Guide

| Label | Meaning |
|---|---|
| `bug` | Something is not working as documented |
| `enhancement` | A new feature or improvement request |
| `question` | General usage question |
| `documentation` | Docs are missing or incorrect |
| `good first issue` | Suitable for first-time contributors |
| `help wanted` | Extra eyes or effort needed |
| `hardware: A1M8` | Specific to the A1M8 sensor |
| `hardware: A2M8` | Specific to the A2M8 sensor |
| `hardware: A3M1` | Specific to the A3M1 sensor |

---

## Asking a Question

For general usage questions that are not bugs or feature requests, open an issue with the `question` label or start a [GitHub Discussion](https://github.com/asheesh1996/RpLidar.NET/discussions).

---

## Contributing

Pull requests are welcome! Please:

1. Fork the repository and create your branch from `main`.
2. Write or update tests for any behaviour you change.
3. Ensure all tests pass: `dotnet test`
4. Follow existing code style (C# conventions, no unnecessary abstractions).
5. Open a pull request with a clear description of what and why.

For large changes, open an issue first to discuss the approach before investing time in implementation.

---

## Security

If you believe you have found a security vulnerability, **do not open a public issue**. Please contact the maintainer directly at [asheeshagra@gmail.com](mailto:asheeshagra@gmail.com).

---

## Maintainer

This project is maintained by [Asheesh Maheshwari](https://github.com/asheesh1996). Response times may vary — thank you for your patience.
