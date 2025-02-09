namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The lidar point group item.
    /// </summary>
    public sealed class LidarPointGroupItem
    {
        /// <summary>
        /// Gets or Sets the angle.
        /// </summary>
        public int Angle { get; set; }
        /// <summary>
        /// Gets or Sets the original angle.
        /// </summary>
        public float OriginalAngle { get; set; }
        /// <summary>
        /// Gets or Sets the distance.
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// Gets or Sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The cathetus.
        /// </summary>
        public double? Cathetus;

        /// <summary>
        /// The can ignore.
        /// </summary>
        public bool CanIgnore;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return $"{Angle};{Distance};{Count}";
        }
    }
}