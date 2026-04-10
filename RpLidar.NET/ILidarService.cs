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

        /// <summary>Returns the human-readable model name of the connected device, e.g. "A1M8".</summary>
        string GetModelNameDescriptionString();

        /// <summary>Returns the product series of the connected device.</summary>
        LidarMajorType GetLidarMajorType();

        /// <summary>Returns the ranging technology of the connected device.</summary>
        LidarTechnologyType GetLidarTechnologyType();
    }
}