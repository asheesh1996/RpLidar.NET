namespace RpLidar.NET.Entities
{
    /// <summary>Describes a scan mode supported by the connected device, as reported by GetLidarConf.</summary>
    public struct LidarScanMode
    {
        /// <summary>Scan mode ID used in startScanExpress.</summary>
        public ushort Id { get; set; }

        /// <summary>Microseconds per sample.</summary>
        public float UsPerSample { get; set; }

        /// <summary>Maximum measurement distance in metres.</summary>
        public float MaxDistance { get; set; }

        /// <summary>Answer/response type byte for this mode.</summary>
        public byte AnsType { get; set; }

        /// <summary>Human-readable mode name, e.g. "Standard", "Sensitivity".</summary>
        public string Name { get; set; }
    }
}
