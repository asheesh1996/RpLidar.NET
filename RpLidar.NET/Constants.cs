using System;

namespace RpLidar.NET
{
    /// <summary>
    /// Protocol-level constants for the SLAMTEC RPLidar serial protocol.
    /// </summary>
    public static class Constants
    {
        /// <summary>Length of a response descriptor in bytes.</summary>
        public const int DescriptorLength = 7;

        /// <summary>Size of a Standard Scan response packet in bytes.</summary>
        public const int StandardScanPacketSize = 5;

        /// <summary>Size of a Classic Capsule Express packet in bytes (response 0x82).</summary>
        public const int CapsulePacketSize = 80;

        /// <summary>Size of a Dense Capsule packet in bytes (response 0x85).</summary>
        public const int DenseCapsulePacketSize = 84;

        /// <summary>Size of an Ultra-Capsule Express packet in bytes (response 0x84).</summary>
        public const int UltraCapsulePacketSize = 132;

        /// <summary>Size of an HQ scan node in bytes (response 0x83).</summary>
        public const int HqScanNodeSize = 16;

        /// <summary>Maximum number of scan nodes in one full 360° sweep buffer.</summary>
        public const int MaxScanNodes = 8192;

        /// <summary>Legacy standard scan sample duration in microseconds.</summary>
        public const int LegacySampleDurationUs = 476;

        /// <summary>Default motor PWM for A-series devices via accessory board.</summary>
        public const ushort DefaultMotorPwm = 660;

        /// <summary>Default motor RPM for S-series devices.</summary>
        public const ushort DefaultMotorRpm = 600;

        /// <summary>Sync byte that begins every command or response descriptor (0xA5).</summary>
        public const byte SYNC_BYTE = 0xA5;

        /// <summary>Second sync byte in a response descriptor (0x5A).</summary>
        public const byte StartFlag2 = 0x5A;

        /// <summary>Flag indicating the descriptor has a payload (OR'd into the send-mode byte).</summary>
        public const byte HAS_PAYLOAD_FLAG = 0x80;

        /// <summary>Bit mask for the sync bit in a standard scan measurement byte.</summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_SYNCBIT = (0x1 << 0);

        /// <summary>Bit position of the quality field in a standard scan measurement byte.</summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT = 2;

        /// <summary>Bit position of the angle LSB in a standard scan measurement byte.</summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_ANGLE_SHIFT = 1;

        /// <summary>Check bit mask for a standard scan measurement byte.</summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_CHECKBIT = (0x1 << 0);

        /// <summary>Sync bit used in express-scan (capsule) measurement nodes. Alias of <see cref="RPLIDAR_RESP_MEASUREMENT_SYNCBIT"/>.</summary>
        public const int RpLidarRespMeasurementSyncBit = RPLIDAR_RESP_MEASUREMENT_SYNCBIT;

        /// <summary>Sync bit used in ultra-capsule measurement nodes (bit 15).</summary>
        public const int RpLidarRespMeasurementSyncBitExp = (0x1 << 15);

        /// <summary>
        /// Synonym for <see cref="SYNC_BYTE"/>. Preserved for source compatibility.
        /// </summary>
        [Obsolete("Use SYNC_BYTE instead.")]
        public const byte StartFlag1 = SYNC_BYTE;

        /// <summary>
        /// Synonym for <see cref="StandardScanPacketSize"/>. Preserved for source compatibility.
        /// </summary>
        [Obsolete("Use StandardScanPacketSize instead.")]
        public const int ScanDataResponseLength = StandardScanPacketSize;
    }
}
