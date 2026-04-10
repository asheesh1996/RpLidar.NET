namespace RpLidar.NET.Entities
{
    /// <summary>Motor capability information returned by <see cref="RpLidar.NET.RpLidarSerialDevice.GetMotorInfo" />.</summary>
    public class LidarMotorInfo
    {
        /// <summary>Gets or sets the motor control method supported by the device.</summary>
        public MotorCtrlSupport CtrlSupport { get; set; }

        /// <summary>Gets or sets the desired (target) motor speed in RPM.</summary>
        public ushort DesiredSpeed { get; set; }

        /// <summary>Gets or sets the maximum supported motor speed in RPM.</summary>
        public ushort MaxSpeed { get; set; }

        /// <summary>Gets or sets the minimum supported motor speed in RPM.</summary>
        public ushort MinSpeed { get; set; }
    }
}
