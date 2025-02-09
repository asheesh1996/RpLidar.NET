namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The lidar point.
    /// </summary>
    public class LidarPoint
    {
        /// <summary>
        /// Heading angle of the measurement (Unit : degree)
        /// </summary>
        public float Angle;

        /// <summary>
        /// Measured distance value between the rotating core of the RPLIDAR and the sampling point (Unit : mm)
        /// </summary>
        public float Distance;

        /// <summary>
        /// Quality of the measurement
        /// </summary>
        public ushort Quality;

        /// <summary>
        /// New 360 degree scan indicator
        /// </summary>
        public bool StartFlag;

        /// <summary>
        /// The flag.
        /// </summary>
        public int Flag;
        /// <summary>
        /// Gets a value indicating whether is valid.
        /// </summary>
        public bool IsValid => Distance > 0f;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return $"Angle:{Angle};Distance:{Distance};Quality:{Quality}";
        }
    }
}