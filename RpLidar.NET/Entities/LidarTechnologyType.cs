namespace RpLidar.NET.Entities
{
    /// <summary>Ranging technology used by the sensor.</summary>
    public enum LidarTechnologyType : byte
    {
        /// <summary>Laser triangulation (A-series).</summary>
        Triangulation = 0,

        /// <summary>Direct Time-of-Flight.</summary>
        DToF = 1,

        /// <summary>Enhanced Time-of-Flight.</summary>
        EToF = 2,

        /// <summary>Frequency-Modulated Continuous Wave.</summary>
        Fmcw = 3,

        /// <summary>Unknown technology.</summary>
        Unknown = 0xFF
    }
}
