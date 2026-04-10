namespace RpLidar.NET.Entities
{
    /// <summary>Response to the GetSampleRate command (0x59).</summary>
    public class SampleRateResponse : IDataResponse
    {
        public RpDataType Type => RpDataType.SampleRate;

        /// <summary>Standard scan sample duration in microseconds.</summary>
        public ushort StandardSampleDurationUs { get; set; }

        /// <summary>Express scan sample duration in microseconds.</summary>
        public ushort ExpressSampleDurationUs { get; set; }
    }
}
