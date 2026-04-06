namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Configuration settings for an RPLidar serial device.
    /// </summary>
    public interface ILidarSettings
    {
        /// <summary>
        /// Maximum valid measurement distance in millimetres.
        /// Measurements beyond this value may be discarded by higher-level consumers.
        /// Default is 25,000 mm (25 m).
        /// </summary>
        int MaxDistance { get; set; }

        /// <summary>
        /// Serial port name.
        /// Use <c>COM3</c> style names on Windows or <c>/dev/ttyUSB0</c> on Linux/macOS.
        /// </summary>
        string Port { get; set; }

        /// <summary>
        /// Serial port baud rate. Default is 115,200 for most RPLidar A-series models.
        /// </summary>
        int BaudRate { get; set; }

        /// <summary>
        /// Motor PWM value in the range 0–1023.
        /// 0 stops the motor; 660 (~65% speed) is the recommended default.
        /// </summary>
        ushort Pwm { get; set; }

        /// <summary>
        /// Scan mode used for data acquisition.
        /// <see cref="ScanMode.Sensitivity"/> (Ultra-Capsule Express Scan) is the default
        /// and is recommended for A1M8, A2M8, and A3M1 devices.
        /// </summary>
        ScanMode ScanMode { get; set; }

        /// <summary>
        /// Minimum interval between <c>LidarPointScanEvent</c> callbacks, in milliseconds.
        /// Points collected within this window are batched into a single event.
        /// Default is 400 ms (~2.5 events per second).
        /// </summary>
        int ElapsedMilliseconds { get; set; }
    }
}
