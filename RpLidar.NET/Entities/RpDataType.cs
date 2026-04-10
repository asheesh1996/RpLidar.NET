namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The rp data type.
    /// </summary>
    public enum RpDataType : byte
    {
        Scan = 0x81,
        CapsuleScan = 0x82,
        GetInfo = 0x04,
        GetHealth = 0x06
    }
}