namespace RpLidar.NET
{
    /// <summary>
    /// The constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The descriptor length.
        /// </summary>
        public const int DescriptorLength = 7;
        /// <summary>
        /// The scan data response length.
        /// </summary>
        public const int ScanDataResponseLength = 5;

        /// <summary>
        /// The RPLIDA R RES P MEASUREMEN T SYNCBIT.
        /// </summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_SYNCBIT = (0x1 << 0);
        /// <summary>
        /// The RPLIDA R RES P MEASUREMEN T QUALIT Y SHIFT.
        /// </summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT = 2;
        /// <summary>
        /// The RPLIDA R RES P MEASUREMEN T ANGL E SHIFT.
        /// </summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_ANGLE_SHIFT = 1;

        /// <summary>
        /// The RPLIDA R RES P MEASUREMEN T CHECKBIT.
        /// </summary>
        public const byte RPLIDAR_RESP_MEASUREMENT_CHECKBIT = (0x1 << 0);

        /// <summary>
        /// The SYN C BYTE.
        /// </summary>
        public const byte SYNC_BYTE = 0xA5;
        /// <summary>
        /// The HA S PAYLOA D FLAG.
        /// </summary>
        public const byte HAS_PAYLOAD_FLAG = 0x80;
        /// <summary>
        /// The start flag1.
        /// </summary>
        public const byte StartFlag1 = 0xA5;
        /// <summary>
        /// The start flag2.
        /// </summary>
        public const byte StartFlag2 = 0x5A;
    }
}