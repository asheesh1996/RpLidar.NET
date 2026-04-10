using System.Runtime.InteropServices;

namespace RpLidar.NET.Entities
{
    /// <summary>Command payload for HQ Scan (command 0x83).</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RplidarPayloadHqScan
    {
        internal byte WorkingMode;
        internal ushort WorkingFlags;
        internal ushort Param;
    }
}
