namespace RpLidar.NET.Entities
{
    /// <summary>SLAMTEC RPLidar product series.</summary>
    public enum LidarMajorType : byte
    {
        /// <summary>A-series (triangulation, A1/A2/A3).</summary>
        A = 0,

        /// <summary>S-series (high performance).</summary>
        S = 1,

        /// <summary>T-series (outdoor).</summary>
        T = 2,

        /// <summary>M-series (mechanical).</summary>
        M = 3,

        /// <summary>C-series (compact).</summary>
        C = 4,

        /// <summary>Unknown or unrecognised model.</summary>
        Unknown = 0xFF
    }
}
