namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Wire-format packet for Dense Capsule scan (response type 0x85).
    /// 84-byte packet: 2 sync bytes + 2-byte start angle + 40 × 2-byte distance samples + 2 checksum bytes.
    /// Angles are linearly interpolated between consecutive packet start angles.
    /// </summary>
    public sealed class RplidarResponseDenseCapsuleMeasurementNodes
    {
        public byte SyncByte1 { get; set; }
        public byte SyncByte2 { get; set; }
        /// <summary>Start angle in Q6 format. Bit 15 = new-scan sync flag.</summary>
        public ushort StartAngleSyncQ6 { get; set; }
        /// <summary>40 distance samples in Q2 format (divide by 4 for mm).</summary>
        public ushort[] Distances { get; set; } = new ushort[40];
    }
}
