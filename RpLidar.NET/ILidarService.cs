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

        /// <summary>Resets the device. Re-connect after ~2 seconds.</summary>
        void Reset();

        /// <summary>Returns all scan modes supported by the connected device.</summary>
        List<LidarScanMode> GetAllSupportedScanModes();

        /// <summary>Returns the device's recommended scan mode, or null if not supported.</summary>
        LidarScanMode? GetTypicalScanMode();
    }
}