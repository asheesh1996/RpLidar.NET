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
        /// Raised periodically with a grouped scan result.
        /// The interval is controlled by <see cref="LidarSettings.ElapsedMilliseconds"/>.
        /// </summary>
        public event LidarPointGroupScanEvenHandler LidarPointGroupScanEvent;

        /// <summary>
        /// Initializes a new instance of <see cref="RPLidar"/>, connects to the device,
        /// and starts scanning using the specified scan mode.
        /// </summary>
        /// <param name="serialport">
        /// Serial port name, e.g. <c>COM3</c> on Windows or <c>/dev/ttyUSB0</c> on Linux.
        /// </param>
        /// <param name="baudrate">Baud rate. Default is 115,200.</param>
        /// <param name="scanMode">Scan mode to use. Default is <see cref="ScanMode.Sensitivity"/>.</param>
        public RPLidar(string serialport, int baudrate = 115200, ScanMode scanMode = ScanMode.Sensitivity)
        {
            try
            {
                _settings = new LidarSettings() { Port = serialport, BaudRate = baudrate, ScanMode = scanMode };
                _service = new RpLidarSerialDevice(_settings);
                _service.LidarPointScanEvent += OnServiceScanEvent;
                _service.LidarPointGroupScanEvent += group => LidarPointGroupScanEvent?.Invoke(group);
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

        /// <summary>Sets the motor speed in RPM. Pass 0 to stop the motor.</summary>
        public void SetMotorSpeed(ushort rpm) => _service.SetMotorSpeed(rpm);

        /// <summary>Determines the motor control method supported by the connected device.</summary>
        public MotorCtrlSupport CheckMotorCtrlSupport() => _service.CheckMotorCtrlSupport();

        /// <summary>Returns motor capability information (S-series only).</summary>
        public LidarMotorInfo GetMotorInfo() => _service.GetMotorInfo();

        /// <summary>Returns the human-readable model name of the connected device, e.g. "A1M8".</summary>
        public string GetModelNameDescriptionString() => _service.GetModelNameDescriptionString();

        /// <summary>Returns the product series of the connected device.</summary>
        public LidarMajorType GetLidarMajorType() => _service.GetLidarMajorType();

        /// <summary>Returns the ranging technology of the connected device.</summary>
        public LidarTechnologyType GetLidarTechnologyType() => _service.GetLidarTechnologyType();

        /// <summary>Sorts scan points in ascending angle order (0° → 360°).</summary>
        public static void AscendScanData(List<LidarPoint> points) => RpLidarSerialDevice.AscendScanData(points);

        /// <summary>Estimates scan rotation frequency in Hz from a list of points.</summary>
        public static float GetFrequency(List<LidarPoint> points, float elapsedMs) => RpLidarSerialDevice.GetFrequency(points, elapsedMs);

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
