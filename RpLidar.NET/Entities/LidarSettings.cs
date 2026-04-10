using System.IO.Ports;
using System.Linq;

namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Default implementation of <see cref="ILidarSettings"/> with sensible defaults
    /// for RPLidar A1M8, A2M8, and A3M1 devices.
    /// </summary>
    public sealed class LidarSettings : ILidarSettings
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LidarSettings"/> with default values.
        /// The serial port is set to the last available port on the system, if any.
        /// </summary>
        public LidarSettings()
        {
            Pwm = Constants.DefaultMotorPwm;
            MotorRpm = Constants.DefaultMotorRpm;
            ScanMode = ScanMode.Sensitivity;
            MaxDistance = 25000;
            BaudRate = 115200;
            Port = SerialPort.GetPortNames().LastOrDefault() ?? string.Empty;
            ElapsedMilliseconds = 400;
        }

        /// <inheritdoc/>
        public int MaxDistance { get; set; }

        /// <inheritdoc/>
        public string Port { get; set; }

        /// <inheritdoc/>
        public int BaudRate { get; set; }

        /// <inheritdoc/>
        public ushort Pwm { get; set; }

        /// <inheritdoc/>
        public ScanMode ScanMode { get; set; }

        /// <inheritdoc/>
        public int ElapsedMilliseconds { get; set; }

        /// <inheritdoc/>
        public ushort MotorRpm { get; set; }
    }
}
