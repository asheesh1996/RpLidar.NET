namespace RpLidar.NET.Entities
{
    /// <summary>Sub-type codes for the GetLidarConf command (0x84).</summary>
    public enum LidarConfSubType : uint
    {
        /// <summary>Angle range of the sensor.</summary>
        AngleRange          = 0x00000000,

        /// <summary>Desired rotation frequency.</summary>
        DesiredRotFreq      = 0x00000001,

        /// <summary>Bitmap of supported scan commands.</summary>
        ScanCommandBitmap   = 0x00000002,

        /// <summary>Minimum rotation frequency.</summary>
        MinRotFreq          = 0x00000004,

        /// <summary>Maximum rotation frequency.</summary>
        MaxRotFreq          = 0x00000005,

        /// <summary>Maximum measurement distance.</summary>
        MaxDistance         = 0x00000060,

        /// <summary>Number of supported scan modes.</summary>
        ScanModeCount       = 0x00000070,

        /// <summary>Microseconds per sample for a given scan mode.</summary>
        ScanModeUsPerSample = 0x00000071,

        /// <summary>Maximum distance for a given scan mode.</summary>
        ScanModeMaxDistance = 0x00000074,

        /// <summary>Answer type byte for a given scan mode.</summary>
        ScanModeAnsType     = 0x00000075,

        /// <summary>Device MAC address.</summary>
        LidarMacAddr        = 0x00000079,

        /// <summary>Typical/default scan mode ID.</summary>
        ScanModeTypical     = 0x0000007C,

        /// <summary>Name string for a given scan mode.</summary>
        ScanModeName        = 0x0000007F,

        /// <summary>Model revision ID.</summary>
        ModelRevisionId     = 0x00000080,

        /// <summary>Model name alias string.</summary>
        ModelNameAlias      = 0x00000081,

        /// <summary>Detected serial baud rate.</summary>
        DetectedSerialBps   = 0x000000A1,

        /// <summary>Static IP address (network devices).</summary>
        LidarStaticIpAddr   = 0x0001CCC0
    }
}
