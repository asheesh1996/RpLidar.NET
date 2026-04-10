namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Represents a scan mode supported by the connected RPLidar device,
    /// as reported via the GetLidarConf protocol command.
    /// </summary>
    public struct LidarScanMode
    {
        /// <summary>Gets or sets the numeric mode identifier.</summary>
        public ushort Id { get; set; }

        /// <summary>Gets or sets the human-readable mode name (e.g. "Sensitivity").</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the sampling period in microseconds.</summary>
        public float UsPerSample { get; set; }

        /// <summary>Gets or sets the maximum measurable distance in metres.</summary>
        public float MaxDistance { get; set; }

        /// <summary>Gets or sets the answer type byte returned by the device for this mode.</summary>
        public byte AnsType { get; set; }
    }
}
