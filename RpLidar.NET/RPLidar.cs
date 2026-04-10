using RpLidar.NET.Entities;
using System;
using System.Collections.Generic;

namespace RpLidar.NET
{
    /// <summary>
    /// High-level wrapper for an RPLidar A-series device.
    /// Connects, starts the motor and scanning automatically on construction,
    /// and exposes scan data via <see cref="LidarPointScanEvent"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// using var lidar = new RPLidar("COM3");
    /// lidar.LidarPointScanEvent += points =>
    /// {
    ///     foreach (var p in points)
    ///         Console.WriteLine($"{p.Angle:F1}°  {p.Distance:F0} mm");
    /// };
    /// Console.ReadLine();
    /// </code>
    /// </example>
    public class RPLidar : IDisposable
    {
        private RpLidarSerialDevice _service;
        private LidarSettings _settings;

        /// <summary>
        /// Raised periodically with a batch of scan points.
        /// The interval is controlled by <see cref="LidarSettings.ElapsedMilliseconds"/>.
        /// </summary>
        public event LidarPointScanEvenHandler LidarPointScanEvent;

        /// <summary>
        /// Initializes a new instance of <see cref="RPLidar"/>, connects to the device,
        /// and starts scanning using <see cref="ScanMode.Sensitivity"/> (Ultra-Capsule Express Scan).
        /// </summary>
        /// <param name="serialport">
        /// Serial port name, e.g. <c>COM3</c> on Windows or <c>/dev/ttyUSB0</c> on Linux.
        /// </param>
        /// <param name="baudrate">Baud rate. Default is 115,200.</param>
        public RPLidar(string serialport, int baudrate = 115200)
        {
            try
            {
                _settings = new LidarSettings() { Port = serialport, BaudRate = baudrate };
                _service = new RpLidarSerialDevice(_settings);
                _service.LidarPointScanEvent += OnServiceScanEvent;
                _service.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine($"RPLidar initialisation error: {e.Message}");
            }
        }

        private void OnServiceScanEvent(List<LidarPoint> points)
        {
            try
            {
                LidarPointScanEvent?.Invoke(points);
            }
            catch { }
        }

        /// <summary>
        /// Stops scanning and the motor.
        /// </summary>
        public void Stop() => _service.Stop();

        /// <summary>Resets the sensor. Re-connect after ~2 seconds.</summary>
        public void Reset() => _service.Reset();

        /// <summary>Returns all scan modes supported by this sensor.</summary>
        public List<LidarScanMode> GetAllSupportedScanModes() => _service.GetAllSupportedScanModes();

        /// <summary>Returns the sensor's recommended scan mode.</summary>
        public LidarScanMode? GetTypicalScanMode() => _service.GetTypicalScanMode();

        /// <summary>
        /// Stops scanning, stops the motor, and releases all resources.
        /// </summary>
        public void Dispose()
        {
            _service.LidarPointScanEvent -= OnServiceScanEvent;
            _service.Stop();
            _service.Dispose();
        }
    }
}
