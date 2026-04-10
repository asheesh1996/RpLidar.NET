namespace RpLidar.NET.Entities
{
    /// <summary>
    /// A single HQ scan measurement node (16 bytes). Available on firmware 1.24+.
    /// Response type 0x83 delivers a continuous stream of these nodes.
    /// </summary>
    public sealed class RplidarResponseHqMeasurementNode
    {
        /// <summary>Angle in Q14 format (upper 16 bits). Full 32-bit value: bits[31:16]=angle_q14, bits[15:0]=reserved.</summary>
        public uint AngleZQ14 { get; set; }
        /// <summary>Distance in Q2 format (mm × 4). Use DistanceMm property for converted value.</summary>
        public uint DistMmQ2 { get; set; }
        /// <summary>Measurement quality (0–255).</summary>
        public byte Quality { get; set; }
        /// <summary>Flags byte. Bit 0 = new scan start.</summary>
        public byte Flag { get; set; }
        /// <summary>Hardware timestamp in microseconds (64-bit).</summary>
        public ulong TimestampUs { get; set; }

        /// <summary>Distance in millimetres.</summary>
        public float DistanceMm => DistMmQ2 / 4.0f;

        /// <summary>Angle in degrees (0–360).</summary>
        public float AngleDegrees => ((AngleZQ14 >> 8) * 90.0f) / 16384.0f;

        /// <summary>Whether this node starts a new 360° revolution.</summary>
        public bool IsNewScan => (Flag & 0x01) != 0;
    }
}
