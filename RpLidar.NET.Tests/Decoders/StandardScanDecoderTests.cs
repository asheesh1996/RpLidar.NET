using Xunit;

namespace RpLidar.NET.Tests.Decoders
{
    public class StandardScanDecoderTests
    {
        // Standard scan 5-byte frame layout (RPLIDAR protocol):
        //   Byte 0: (quality << 2) | (startFlag & 1) | (checkBit << 1)
        //   Bytes 1-2: LE uint16 — (angle_degrees * 64) << 1 | checkBit
        //   Bytes 3-4: LE uint16 — distance_mm * 4
        private static byte[] BuildStandardScanFrame(int quality, bool startFlag, bool checkBit, float angleDeg, float distanceMm)
        {
            var frame = new byte[5];
            frame[0] = (byte)((quality << 2) | (startFlag ? 1 : 0) | (checkBit ? 2 : 0));
            int angleField = ((int)(angleDeg * 64) << 1) | (checkBit ? 1 : 0);
            frame[1] = (byte)(angleField & 0xFF);
            frame[2] = (byte)(angleField >> 8);
            int distanceQ2 = (int)(distanceMm * 4);
            frame[3] = (byte)(distanceQ2 & 0xFF);
            frame[4] = (byte)(distanceQ2 >> 8);
            return frame;
        }

        [Fact]
        public void BuildStandardScanFrame_KnownValues_ProducesCorrectBytes()
        {
            // quality=10, startFlag=true, checkBit=true, angle=90°, distance=1000mm
            // byte0 = (10<<2)|1|2 = 43
            // angleField = (5760<<1)|1 = 11521 → LE [0x01, 0x2D]
            // distanceQ2 = 4000 → LE [0xA0, 0x0F]
            var frame = BuildStandardScanFrame(10, true, true, 90f, 1000f);

            Assert.Equal(5, frame.Length);
            Assert.Equal(43, frame[0]);
            Assert.Equal(0x01, frame[1]);
            Assert.Equal(0x2D, frame[2]);
            Assert.Equal(0xA0, frame[3]);
            Assert.Equal(0x0F, frame[4]);
        }

        [Fact(Skip = "Requires WU-01 standard scan parser — ParseStandardScanPackets not yet publicly accessible")]
        public void StandardScanFrame_Quality10_Angle90_Distance1000_ParsedCorrectly()
        {
            var frame = BuildStandardScanFrame(10, true, true, 90f, 1000f);
            // Once WU-01 exposes ParseStandardScanPackets, assert the decoded LidarPoint here:
            // var point = DataResponseHelper.ParseStandardScanPackets(frame)[0];
            // Assert.Equal(90f, point.Angle, 1);
            // Assert.Equal(1000f, point.Distance, 1);
            // Assert.Equal(10, point.Quality);
        }
    }
}
