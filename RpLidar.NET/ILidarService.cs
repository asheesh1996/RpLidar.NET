using RpLidar.NET.Entities;
using System;
using System.Collections.Generic;

namespace RpLidar.NET
{
    /// <summary>
    /// Delegate for events that deliver a group of lidar scan points as a <see cref="LidarPointGroup"/>.
    /// </summary>
    /// <param name="points">The scanned point group containing angle/distance/quality data.</param>
    public delegate void LidarPointGroupScanEvenHandler(LidarPointGroup points);

    /// <summary>
    /// Delegate for events that deliver a list of lidar scan points.
    /// </summary>
    /// <param name="points">The list of scanned <see cref="LidarPoint"/> values.</param>
    public delegate void LidarPointScanEvenHandler(List<LidarPoint> points);

    /// <summary>
    /// Represents a high-level lidar scanning service that manages the device lifecycle
    /// and raises events as scan data arrives.
    /// </summary>
    public interface ILidarService : IDisposable
    {
        /// <summary>
        /// Raised each time a batch of <see cref="LidarPoint"/> measurements is ready.
        /// The batch interval is controlled by <see cref="Entities.ILidarSettings.ElapsedMilliseconds"/>.
        /// </summary>
        event LidarPointScanEvenHandler LidarPointScanEvent;

        /// <summary>
        /// Raised each time a <see cref="LidarPointGroup"/> (a full or partial 360° sweep) is ready.
        /// </summary>
        event LidarPointGroupScanEvenHandler LidarPointGroupScanEvent;

        /// <summary>
        /// Starts the motor and begins streaming scan data.
        /// Raises <see cref="LidarPointScanEvent"/> and <see cref="LidarPointGroupScanEvent"/> as data arrives.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the scan and powers down the motor.
        /// </summary>
        void Stop();

        /// <summary>Queries the device's standard and express scan sample rates.</summary>
        SampleRateResponse GetSampleRate();

        /// <summary>Queries a device configuration value.</summary>
        LidarConfResponse GetLidarConf(LidarConfSubType subType, uint argument = 0);

        /// <summary>Resets the device. Re-connect after ~2 seconds.</summary>
        void Reset();

        /// <summary>Returns all scan modes supported by the connected device.</summary>
        List<LidarScanMode> GetAllSupportedScanModes();

        /// <summary>Returns the device's recommended scan mode, or null if not supported.</summary>
        LidarScanMode? GetTypicalScanMode();

        /// <summary>Sets the motor speed in RPM. Pass 0 to stop the motor.</summary>
        void SetMotorSpeed(ushort rpm);

        /// <summary>Determines the motor control method supported by the connected device.</summary>
        MotorCtrlSupport CheckMotorCtrlSupport();

        /// <summary>Returns motor capability information (S-series only).</summary>
        LidarMotorInfo GetMotorInfo();

        /// <summary>Returns the human-readable model name of the connected device, e.g. "A1M8".</summary>
        string GetModelNameDescriptionString();

        /// <summary>Returns the product series of the connected device.</summary>
        LidarMajorType GetLidarMajorType();

        /// <summary>Returns the ranging technology of the connected device.</summary>
        LidarTechnologyType GetLidarTechnologyType();
    }
}
