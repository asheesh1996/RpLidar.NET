namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The rp health response.
    /// </summary>
    public class RpHealthResponse : IDataResponse
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        public RpDataType Type => RpDataType.GetHealth;

        /// <summary>
        /// Status Code
        /// 0x0 = OK, 0x1 = Warning, 0x2 = Error
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Error Code
        /// </summary>
        public int ErrorCode { get; set; }
    }
}