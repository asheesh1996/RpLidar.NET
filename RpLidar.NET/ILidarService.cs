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
    }
}