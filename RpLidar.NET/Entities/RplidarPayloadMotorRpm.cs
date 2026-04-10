using System.Runtime.InteropServices;

namespace RpLidar.NET.Entities
{
    /// <summary>Command payload for SetMotorSpeed (command 0xA8) — RPM-based motor control.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RplidarPayloadMotorRpm
    {
        /// <summary>Desired motor speed in RPM. Use 0 to stop. Default: 600.</summary>
        internal ushort RpmValue;
    }
}
