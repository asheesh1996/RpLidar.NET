using System.IO.Ports;
using System.Linq;

namespace RpLidar.NET.Entities
{

    /// <summary>
    /// The lidar settings.
    /// </summary>
    public sealed class LidarSettings : ILidarSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LidarSettings"/> class.
        /// </summary>
        public LidarSettings()
        {
            Pwm = 660;
            Type = 4;
            MaxDistance = 25000;
            BaudRate = 115200;
            Port = SerialPort.GetPortNames().ToList().LastOrDefault();
            ElapsedMilliseconds = 400;
        }

        /// <summary>
        /// Gets or Sets the max distance.
        /// </summary>
        public int MaxDistance { get; set; }

        /// <summary>
        /// Gets or Sets the port.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Gets or Sets the baud rate.
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// Gets or Sets the pwm.
        /// </summary>
        public ushort Pwm { get; set; }

        /// <summary>
        /// Gets or Sets the type.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or Sets the elapsed milliseconds.
        /// </summary>
        public int ElapsedMilliseconds { get; set; }
    }
}