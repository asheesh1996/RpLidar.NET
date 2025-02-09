namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The rplidar processed result.
    /// </summary>
    public sealed class RplidarProcessedResult
    {
        /// <summary>
        /// The is start angle sync q6.
        /// </summary>
        public bool IsStartAngleSyncQ6;

        /// <summary>
        /// The is rp lidar resp measurement sync bit exp.
        /// </summary>
        public bool IsRpLidarRespMeasurementSyncBitExp;

        /// <summary>
        /// The value.
        /// </summary>
        public RplidarResponseUltraCapsuleMeasurementNodes Value;

        /// <summary>
        /// The remainder data.
        /// </summary>
        public byte[] RemainderData;
    }
}