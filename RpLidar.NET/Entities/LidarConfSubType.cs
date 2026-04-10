namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Sub-type codes used with the <c>GetLidarConf</c> (0x84) command.
    /// Values match the SLAMTEC protocol specification.
    /// </summary>
    internal enum LidarConfSubType : uint
    {
        ScanModeCount       = 0x00000070,
        ScanModeUsPerSample = 0x00000071,
        ScanModeMaxDistance = 0x00000074,
        ScanModeAnsType     = 0x00000075,
        ScanModeTypical     = 0x0000007C,
        ScanModeName        = 0x0000007F,
    }
}
