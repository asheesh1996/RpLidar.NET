namespace RpLidar.NET.Entities
{
    /// <summary>Indicates the motor control method supported by the device.</summary>
    public enum MotorCtrlSupport : byte
    {
        /// <summary>No motor control (motor runs at fixed speed).</summary>
        None = 0,

        /// <summary>PWM-based motor control via accessory board (A-series).</summary>
        Pwm = 1,

        /// <summary>RPM-based motor control (S-series and newer).</summary>
        Rpm = 2
    }
}
