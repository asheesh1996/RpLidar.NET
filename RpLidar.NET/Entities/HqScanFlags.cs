using System;

namespace RpLidar.NET.Entities
{
    /// <summary>Flags for the HQ Scan command (0x83, FW 1.24+).</summary>
    [Flags]
    public enum HqScanFlags : byte
    {
        /// <summary>No flags set.</summary>
        None = 0,

        /// <summary>Counter-clockwise rotation.</summary>
        Ccw = 0x01,

        /// <summary>Include raw encoder data in response.</summary>
        RawEncoder = 0x02,

        /// <summary>Include raw distance data in response.</summary>
        RawDistance = 0x04
    }
}
