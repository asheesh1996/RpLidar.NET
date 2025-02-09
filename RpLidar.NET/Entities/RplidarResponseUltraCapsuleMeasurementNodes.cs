namespace RpLidar.NET.Entities
{

    /// <summary>
    /// The rplidar response ultra capsule measurement nodes.
    /// </summary>
    public sealed class RplidarResponseUltraCapsuleMeasurementNodes
    {
        /// <summary>
        /// The schecksum1.
        /// </summary>
        public byte s_checksum_1; // see [s_checksum_1]
        /// <summary>
        /// The schecksum2.
        /// </summary>
        public byte s_checksum_2; // see [s_checksum_1]
        /// <summary>
        /// The startanglesyncq6.
        /// </summary>
        public ushort start_angle_sync_q6;
        /// <summary>
        /// The ultracabins.
        /// </summary>
        public uint[] ultra_cabins;
        /// <summary>
        /// The has bit.
        /// </summary>
        public bool hasBit;
    }
}