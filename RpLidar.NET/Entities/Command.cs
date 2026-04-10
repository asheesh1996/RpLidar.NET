namespace RpLidar.NET.Entities
{
    /// <summary>
    /// RPLidar serial protocol command codes.
    /// </summary>
    /// <remarks>
    /// Values are defined in LR001_SLAMTEC_rplidar_protocol_v2.1_en, page 13.
    /// </remarks>
    public enum Command : byte
    {
        /// <summary>Stop an ongoing scan. No response.</summary>
        Stop = 0x25,

        /// <summary>
        /// Soft-reset the device. The device reboots; no response is sent.
        /// Allow at least 2 000 ms before issuing the next command.
        /// </summary>
        Reset = 0x40,

        /// <summary>Start a standard scan. Returns a stream of 5-byte measurement packets.</summary>
        Scan = 0x20,

        /// <summary>
        /// Start a Classic Capsule Express scan (response type 0x82).
        /// Returns 80-byte capsule packets at a higher sample rate than <see cref="Scan"/>.
        /// </summary>
        ExpressScan = 0x82,

        /// <summary>Force a scan even if the device is not rotating. Returns standard scan packets.</summary>
        ForceScan = 0x21,

        /// <summary>Request device info (model, firmware version, serial number). Single response.</summary>
        GetInfo = 0x50,

        /// <summary>Request device health status. Single response.</summary>
        GetHealth = 0x52,

        /// <summary>Request standard and express scan sample durations. Single response.</summary>
        GetSampleRate = 0x59,

        /// <summary>Query or set lidar configuration parameters. Single or multi-response.</summary>
        GetLidarConf = 0x84,

        /// <summary>Set the motor PWM duty cycle (A-series accessory board). No response.</summary>
        StartPwm = 0xF0,

        /// <summary>
        /// Start an HQ (high-quality) scan (response type 0x83).
        /// Returns 16-byte HQ scan nodes. Requires firmware 1.24 or later.
        /// </summary>
        HqScan = 0x83,

        /// <summary>
        /// Set the motor speed in RPM (S-series devices). No response.
        /// Use <see cref="ILidarSettings.MotorRpm"/> to configure the target speed.
        /// </summary>
        SetMotorSpeed = 0xA8,
    }
}
