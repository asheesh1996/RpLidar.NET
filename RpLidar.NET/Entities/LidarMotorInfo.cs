namespace RpLidar.NET.Entities
{
    /// <summary>Motor capability and speed information for S-series devices.</summary>
    public class LidarMotorInfo
    {
        /// <summary>The type of motor control supported by this device.</summary>
        public MotorCtrlSupport CtrlSupport { get; set; }

        /// <summary>Current/desired motor speed.</summary>
        public ushort DesiredSpeed { get; set; }

        /// <summary>Maximum motor speed.</summary>
        public ushort MaxSpeed { get; set; }

        /// <summary>Minimum motor speed.</summary>
        public ushort MinSpeed { get; set; }
    }
}
