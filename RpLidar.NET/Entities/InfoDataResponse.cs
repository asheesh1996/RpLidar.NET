namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Response Information
    /// </summary>
    public class InfoDataResponse : IDataResponse
    {
        public RpDataType Type => RpDataType.GetInfo;

        /// <summary>
        /// Device Serial Number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Device Firmware Version
        /// </summary>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Device Hardware Version
        /// </summary>
        public string HardwareVersion { get; set; }

        /// <summary>
        /// Device Mode ID
        /// </summary>
        public string ModelId { get; set; }
    }
}