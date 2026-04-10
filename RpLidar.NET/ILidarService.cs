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
    }
}
