using Xunit;

namespace RpLidar.NET.Tests.Helpers
{
    public class ModelDecoderTests
    {
        [Fact(Skip = "Requires WU-09 — ModelDecoder not yet implemented")]
        public void GetModelName_0x18_ReturnsA1M8()
        {
            // ModelDecoder.GetModelName(0x18) == "A1M8"
        }

        [Fact(Skip = "Requires WU-09 — ModelDecoder not yet implemented")]
        public void GetMajorType_0x18_ReturnsTypeA()
        {
            // ModelDecoder.GetMajorType(0x18) == LidarMajorType.A
        }

        [Fact(Skip = "Requires WU-09 — ModelDecoder not yet implemented")]
        public void GetTechnologyType_0x18_ReturnsTriangulation()
        {
            // ModelDecoder.GetTechnologyType(0x18) == LidarTechnologyType.Triangulation
        }

        [Fact(Skip = "Requires WU-09 — ModelDecoder not yet implemented")]
        public void GetModelName_UnknownId_ContainsUnknown()
        {
            // ModelDecoder.GetModelName(0xFF) should contain "Unknown"
        }

        [Fact(Skip = "Requires WU-09 — ModelDecoder not yet implemented")]
        public void GetModelName_0x61_ReturnsS1()
        {
            // ModelDecoder.GetModelName(0x61) == "S1"
        }
    }
}
