using RpLidar.NET;
using RpLidar.NET.Entities;

// -------------------------------------------------------------------------
// RpLidar.NET — Example
// -------------------------------------------------------------------------
// Adjust the port name to match your system:
//   Windows : "COM3", "COM4", ...
//   Linux   : "/dev/ttyUSB0", "/dev/ttyAMA0", ...
//   macOS   : "/dev/tty.usbserial-..."
// -------------------------------------------------------------------------
const string PORT_NAME = "/dev/ttyUSB0";

Console.WriteLine("RpLidar.NET — Example");
Console.WriteLine($"Connecting on {PORT_NAME} ...");
Console.WriteLine("Press Ctrl+C to stop.");
Console.WriteLine();

// ── Option 1: High-level RPLidar wrapper ──────────────────────────────────
// Connects, starts the motor and scanning automatically.
using var lidar = new RPLidar(PORT_NAME);
lidar.LidarPointScanEvent += OnScanReceived;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException) { }

Console.WriteLine("Stopping...");

// ── Option 2: Low-level RpLidarSerialDevice ───────────────────────────────
// Uncomment the block below if you need finer control over settings.
//
// var settings = new LidarSettings
// {
//     Port               = PORT_NAME,
//     BaudRate           = 115200,
//     ScanMode           = ScanMode.Sensitivity,  // Ultra-Capsule (default)
//     Pwm                = 660,                   // Motor speed 0–1023
//     MaxDistance        = 25000,                 // mm  (25 m)
//     ElapsedMilliseconds = 200,                  // fire event every 200 ms
// };
//
// using var device = new RpLidarSerialDevice(settings);
// device.LidarPointScanEvent += OnScanReceived;
// device.Start();
// Console.ReadLine();
// device.Stop();

static void OnScanReceived(List<LidarPoint> points)
{
    Console.WriteLine($"[Scan] {points.Count} points received");
    foreach (var point in points)
    {
        // point.Angle    — heading in degrees  (0.0 – 360.0)
        // point.Distance — distance in mm
        // point.Quality  — measurement quality (higher = better)
        Console.WriteLine($"  Angle: {point.Angle,7:F2}°   Distance: {point.Distance,8:F1} mm   Quality: {point.Quality}");
    }
}
