namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Wire-format packet for Classic Capsule Express scan (response type 0x82).
    /// Each packet is exactly 80 bytes: 2 sync bytes, 2-byte start angle, 16 × 5-byte cabin nodes.
    /// </summary>
    public sealed class RplidarResponseCapsuleMeasurementNodes
    {
        /// <summary>First sync byte (high nibble must be 0xA, low nibble = checksum low bits).</summary>
        public byte SyncByte1 { get; set; }

        /// <summary>Second sync byte (high nibble must be 0x5, low nibble = checksum high bits).</summary>
        public byte SyncByte2 { get; set; }

        /// <summary>Start angle in Q6 format (multiply by 90.0/16384.0 for degrees). Bit 15 = new-scan flag.</summary>
        public ushort StartAngleSyncQ6 { get; set; }

        /// <summary>
        /// Raw cabin bytes: 16 cabins × 5 bytes each = 76 bytes (header bytes excluded).
        /// cabin[i] occupies bytes [i*5 .. i*5+4].
        /// </summary>
        public byte[] CabinData { get; set; } = new byte[76];
    }
}
