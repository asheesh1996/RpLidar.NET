namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The lidar settings interface.
    /// </summary>
    public interface ILidarSettings
    {
        /// <summary>
        /// Gets or Sets the max distance.
        /// </summary>
        int MaxDistance { get; set; }

        /// <summary>
        /// Gets or Sets the port.
        /// </summary>
        string Port { get; set; }

        /// <summary>
        /// Gets or Sets the baud rate.
        /// </summary>
        int BaudRate { get; set; }

        /// <summary>
        /// Gets or Sets the pwm.
        /// </summary>
        ushort Pwm { get; set; }

        /// <summary>
        /// Gets or Sets the type.
        /// </summary>
        byte Type { get; set; }

        /// <summary>
        /// Gets or Sets the elapsed milliseconds.
        /// </summary>
        int ElapsedMilliseconds { get; set; }
    }
}