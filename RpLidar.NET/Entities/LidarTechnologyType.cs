namespace RpLidar.NET.Entities
{
    /// <summary>Identifies the ranging technology used by an RPLidar device.</summary>
    public enum LidarTechnologyType : byte
    {
        /// <summary>Triangulation (laser + camera).</summary>
        Triangulation = 0,
        /// <summary>Direct Time-of-Flight.</summary>
        DToF = 1,
        /// <summary>Encoded Time-of-Flight.</summary>
        EToF = 2,
        /// <summary>Frequency-Modulated Continuous Wave.</summary>
        Fmcw = 3,
        /// <summary>Technology type is not known.</summary>
        Unknown = 0xFF
    }
}
