using RpLidar.NET;
using RpLidar.NET.Entities;
using System;

namespace RpLidar.NET
{
    /// <summary>
    /// The RP lidar.
    /// </summary>
    public class RPLidar : IDisposable
    {
        /// <summary>
        /// The service.
        /// </summary>
        private RpLidarSerialDevice _service;
        /// <summary>
        /// The settings.
        /// </summary>
        private LidarSettings _settings;

        public event LidarPointScanEvenHandler LidarPointScanEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="RPLidar"/> class.
        /// </summary>
        /// <param name="serialport">The serialport.</param>
        /// <param name="baudrate">The baudrate.</param>
        public RPLidar(string serialport, int baudrate = 115200)
        {
            try
            {
                _settings = new LidarSettings() { Port = serialport, BaudRate = baudrate };
                _service = new RpLidarSerialDevice(_settings);
                //_service.LidarPointGroupScanEvent += Service_LidarPointGroupEvent;
                _service.LidarPointScanEvent += _service_LidarPointScanEvent;
                _service.Start();
            }
            catch (Exception E)
            {
                Console.WriteLine(E);
            }
        }

        /// <summary>
        /// services the lidar point scan event.
        /// </summary>
        /// <param name="points">The points.</param>
        private void _service_LidarPointScanEvent(System.Collections.Generic.List<LidarPoint> points)
        {
            try
            {
                this.LidarPointScanEvent(points);
            }
            catch { }
        }

        /// <summary>
        /// Services the lidar point group event.
        /// </summary>
        /// <param name="points">The points.</param>
        private void Service_LidarPointGroupEvent(LidarPointGroup points)
        {
            try
            {
                this.LidarPointScanEvent(points.GetPoints());
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop() => _service.StopMotor();

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _service.StopMotor();
            _service.Stop();
            _service.Dispose();
            _service.LidarPointScanEvent -= this.LidarPointScanEvent;
        }
    }
}