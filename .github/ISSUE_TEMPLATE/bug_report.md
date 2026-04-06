---
name: Bug Report
about: Something is not working as documented
title: '[Bug] '
labels: bug
assignees: asheesh1996
---

## Environment

| Field | Value |
|---|---|
| OS and version | <!-- e.g. Windows 11, Ubuntu 22.04, Raspberry Pi OS Bookworm --> |
| .NET version | <!-- e.g. .NET 8.0.3 --> |
| RpLidar.NET version | <!-- e.g. 1.0.0 --> |
| Sensor model | <!-- e.g. A1M8, A2M8, A3M1 --> |
| Scan mode | <!-- e.g. ScanMode.Sensitivity --> |
| Connection interface | <!-- e.g. USB, UART --> |

## What I Expected

<!-- Describe what you expected to happen -->

## What Actually Happened

<!-- Describe the actual behaviour. Include the full exception message and stack trace if applicable. -->

## Steps to Reproduce

1. 
2. 
3. 

## Minimal Code Reproduction

```csharp
// Please provide the smallest possible snippet that demonstrates the issue
using RpLidar.NET;

using var lidar = new RPLidar("COM3");
lidar.LidarPointScanEvent += points =>
{
    // Describe what goes wrong here
};
Console.ReadLine();
```

## Diagnostic Output

<!-- Run the snippet below and paste the output -->

```csharp
var device = new RpLidarSerialDevice(new LidarSettings { Port = "COM3" });
device.Connect();
var info   = device.SendRequest(Command.GetInfo)   as InfoDataResponse;
var health = device.SendRequest(Command.GetHealth) as RpHealthResponse;
Console.WriteLine($"Model: {info?.ModelId}  FW: {info?.FirmwareVersion}");
Console.WriteLine($"Health: {health?.Status}  Error: {health?.ErrorCode}");
```

**Output:**
```

```

## Additional Context

<!-- Screenshots, logs, or any other information that might help -->
