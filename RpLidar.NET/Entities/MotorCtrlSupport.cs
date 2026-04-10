namespace RpLidar.NET.Entities
{
    /// <summary>Indicates the motor control method supported by the connected device.</summary>
    public enum MotorCtrlSupport : byte
    {
        /// <summary>No motor control support detected.</summary>
        None = 0,
        /// <summary>PWM-based motor control (A-series devices with accessory board).</summary>
        Pwm = 1,
        /// <summary>RPM-based motor control (S-series devices).</summary>
        Rpm = 2,
    }
}
