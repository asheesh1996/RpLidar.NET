namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The rp data type.
    /// </summary>
    public enum RpDataType : byte
    {
        Scan = 0x81,
        GetInfo = 0x04,
        GetHealth = 0x06,
        GetSampleRate = 0x15,
        GetLidarConf = 0x20,
    }
}