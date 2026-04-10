namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Wire-format packet for Ultra-Dense Capsule scan (response type 0x86).
    /// Adds a microsecond hardware timestamp and per-sample quality bytes to the Dense format.
    /// Packet layout: 2 sync + 4-byte timestamp (µs) + 2-byte start angle + 40 × (2-byte dist + 1-byte quality) + checksum.
    /// </summary>
    public sealed class RplidarResponseUltraDenseCapsuleMeasurementNodes
    {
        public byte SyncByte1 { get; set; }
        public byte SyncByte2 { get; set; }
        /// <summary>Hardware timestamp in microseconds.</summary>
        public uint TimestampUs { get; set; }
        public ushort StartAngleSyncQ6 { get; set; }
        public ushort[] Distances { get; set; } = new ushort[40];
        /// <summary>Per-sample quality values (0–255).</summary>
        public byte[] Qualities { get; set; } = new byte[40];
    }
}
