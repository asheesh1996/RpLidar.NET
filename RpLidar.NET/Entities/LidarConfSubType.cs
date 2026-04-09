namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Sub-type values for the GetLidarConf (0x84) command.
    /// Matches the SLAMTEC C++ SDK rplidar_cmd.h definitions.
    /// </summary>
    public enum LidarConfSubType : uint
    {
        /// <summary>Number of scan modes supported by the device.</summary>
        ScanModeCount = 0x0070,

        /// <summary>The default scan mode ID used by the device.</summary>
        ScanModeTypical = 0x0071,

        /// <summary>Microseconds per sample for the specified scan mode (requires mode ID argument).</summary>
        ScanModeUsPerSample = 0x0072,

        /// <summary>Maximum distance in metres for the specified scan mode (requires mode ID argument).</summary>
        ScanModeMaxDistance = 0x0074,

        /// <summary>Answer type byte for the specified scan mode (requires mode ID argument).</summary>
        ScanModeAnsType = 0x0075,

        /// <summary>Human-readable name string for the specified scan mode (requires mode ID argument).</summary>
        ScanModeName = 0x0078,
    }
}
