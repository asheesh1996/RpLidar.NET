namespace RpLidar.NET.Entities
{
    /// <summary>
    ///
    /// </summary>
    public class ResponseDescriptor
    {
        /// <summary>
        /// Size of a single incoming data response packet in bytes
        /// </summary>
        public int ResponseLength { get; set; }

        /// <summary>
        /// <see cref="SendMode"/>
        /// </summary>
        public SendMode SendMode { get; set; }

        /// <summary>
        /// <see cref="RpDataType"/>
        /// </summary>
        public RpDataType RpDataType { get; set; }
    }
}