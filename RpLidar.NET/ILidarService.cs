using RpLidar.NET.Entities;
using System;
using System.Collections.Generic;

namespace RpLidar.NET
{
    public delegate void LidarPointGroupScanEvenHandler(LidarPointGroup points);

    public delegate void LidarPointScanEvenHandler(List<LidarPoint> points);

    /// <summary>
    /// The lidar service interface.
    /// </summary>
    public interface ILidarService : IDisposable
    {
        event LidarPointScanEvenHandler LidarPointScanEvent;

        event LidarPointGroupScanEvenHandler LidarPointGroupScanEvent;

        void Start();

        void Stop();

        /// <summary>Sets the motor speed in RPM. Pass 0 to stop the motor.</summary>
        void SetMotorSpeed(ushort rpm);

        /// <summary>Determines the motor control method supported by the connected device.</summary>
        MotorCtrlSupport CheckMotorCtrlSupport();

        /// <summary>Returns motor capability information (S-series only).</summary>
        LidarMotorInfo GetMotorInfo();
    }
}