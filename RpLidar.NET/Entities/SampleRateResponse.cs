namespace RpLidar.NET.Entities
{
    /// <summary>Response to a GetSampleRate (0x59) query.</summary>
    public class SampleRateResponse : IDataResponse
    {
        public RpDataType Type => RpDataType.GetSampleRate;

        /// <summary>Duration of a standard scan sample in microseconds.</summary>
        public ushort StandardSampleDurationUs { get; set; }

        /// <summary>Duration of an express scan sample in microseconds.</summary>
        public ushort ExpressSampleDurationUs { get; set; }
    }
}
