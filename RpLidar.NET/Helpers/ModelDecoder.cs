using RpLidar.NET.Entities;
using System.Collections.Generic;

namespace RpLidar.NET.Helpers
{
    /// <summary>Decodes SLAMTEC RPLidar model identifiers.</summary>
    public static class ModelDecoder
    {
        private struct ModelInfo
        {
            public string Name;
            public LidarMajorType MajorType;
            public LidarTechnologyType TechType;

            public ModelInfo(string name, LidarMajorType majorType, LidarTechnologyType techType)
            {
                Name = name;
                MajorType = majorType;
                TechType = techType;
            }
        }

        private static readonly Dictionary<byte, ModelInfo> _models = new Dictionary<byte, ModelInfo>
        {
            // A-series (Triangulation)
            { 0x18, new ModelInfo("A1M8",  LidarMajorType.A, LidarTechnologyType.Triangulation) },
            { 0x28, new ModelInfo("A2M8",  LidarMajorType.A, LidarTechnologyType.Triangulation) },
            { 0x36, new ModelInfo("A3M1",  LidarMajorType.A, LidarTechnologyType.Triangulation) },
            { 0x38, new ModelInfo("A2M6",  LidarMajorType.A, LidarTechnologyType.Triangulation) },
            { 0x3A, new ModelInfo("A2M12", LidarMajorType.A, LidarTechnologyType.Triangulation) },
            // S-series (dToF)
            { 0x61, new ModelInfo("S1",    LidarMajorType.S, LidarTechnologyType.DToF) },
            { 0x62, new ModelInfo("S2",    LidarMajorType.S, LidarTechnologyType.DToF) },
            { 0x63, new ModelInfo("S3",    LidarMajorType.S, LidarTechnologyType.DToF) },
            // C-series (compact)
            { 0x80, new ModelInfo("C1",    LidarMajorType.C, LidarTechnologyType.Triangulation) },
            // T-series (outdoor)
            { 0xA0, new ModelInfo("T1",    LidarMajorType.T, LidarTechnologyType.DToF) },
        };

        /// <summary>Returns the human-readable model name for a given model byte, e.g. "A1M8".</summary>
        public static string GetModelName(byte modelId)
            => _models.TryGetValue(modelId, out var info) ? info.Name : $"Unknown(0x{modelId:X2})";

        /// <summary>Returns the product series for a given model byte.</summary>
        public static LidarMajorType GetMajorType(byte modelId)
            => _models.TryGetValue(modelId, out var info) ? info.MajorType : LidarMajorType.Unknown;

        /// <summary>Returns the ranging technology for a given model byte.</summary>
        public static LidarTechnologyType GetTechnologyType(byte modelId)
            => _models.TryGetValue(modelId, out var info) ? info.TechType : LidarTechnologyType.Unknown;
    }
}
