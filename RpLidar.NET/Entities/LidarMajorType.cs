namespace RpLidar.NET.Entities
{
    /// <summary>Identifies the SLAMTEC RPLidar product series.</summary>
    public enum LidarMajorType : byte
    {
        /// <summary>A-series (triangulation-based).</summary>
        A = 0,
        /// <summary>S-series (dToF-based).</summary>
        S = 1,
        /// <summary>T-series (outdoor dToF).</summary>
        T = 2,
        /// <summary>M-series.</summary>
        M = 3,
        /// <summary>C-series (compact).</summary>
        C = 4,
        /// <summary>Model series is not known.</summary>
        Unknown = 0xFF
    }
}
