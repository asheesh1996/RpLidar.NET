namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The rp data type.
    /// </summary>
    public enum RpDataType : byte
    {
        GetInfo        = 0x04,
        GetHealth      = 0x06,
        SampleRate     = 0x15,
        GetLidarConf   = 0x20,
        Scan           = 0x81,
        CapsuleScan    = 0x82,
        HqScan         = 0x83,
        DenseScan      = 0x85,
        UltraDenseScan = 0x86,
    }
}
