namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Defines the scan mode used by the RPLidar device.
    /// </summary>
    /// <remarks>
    /// Corresponds to the <c>working_mode</c> field in the Express Scan protocol payload.
    /// See Slamtec protocol specification: LR001_SLAMTEC_rplidar_protocol_v2.1_en.
    /// </remarks>
    public enum ScanMode : byte
    {
        /// <summary>
        /// Standard scan mode. Uses the basic Scan command (0x20).
        /// Compatible with all RPLidar A-series devices.
        /// Approximately 2,000–4,000 samples per revolution depending on model.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// Boost scan mode. Uses Express Scan (0x82) with <c>working_mode = 3</c>.
        /// Higher scan frequency than Standard mode.
        /// Supported on RPLidar A2 and A3.
        /// </summary>
        Boost = 3,

        /// <summary>
        /// Sensitivity scan mode (Ultra-Capsule). Uses Express Scan (0x82) with <c>working_mode = 4</c>.
        /// Highest sample density; recommended for RPLidar A1M8, A2M8, A3M1.
        /// This is the default mode.
        /// </summary>
        Sensitivity = 4,

        /// <summary>Dense Capsule Express scan (response 0x85). A3/S-series.</summary>
        Dense = 5,

        /// <summary>Ultra-Dense Capsule Express scan (response 0x86), includes timestamps.</summary>
        UltraDense = 6,
    }
}
