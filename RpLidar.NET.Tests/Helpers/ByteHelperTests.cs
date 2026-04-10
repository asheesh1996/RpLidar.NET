using RpLidar.NET.Entities;
using RpLidar.NET.Helpers;
using Xunit;

namespace RpLidar.NET.Tests.Helpers
{
    public class ByteHelperTests
    {
        [Fact]
        public void GetBytes_MotorPwm660_ProducesLittleEndianBytes()
        {
            var payload = new RplidarPayloadMotorPwm { pwm_value = 660 };

            var bytes = payload.GetBytes();

            // 660 = 0x0294 in LE = [0x94, 0x02]
            Assert.Equal(2, bytes.Length);
            Assert.Equal(0x94, bytes[0]);
            Assert.Equal(0x02, bytes[1]);
        }

        [Fact]
        public void FromBytes_RoundTrip_MotorPwm660()
        {
            var original = new RplidarPayloadMotorPwm { pwm_value = 660 };
            var bytes = original.GetBytes();

            var results = bytes.FromBytes<RplidarPayloadMotorPwm>();

            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal(660, results[0].pwm_value);
        }

        [Fact]
        public void GetBytes_MotorPwm0_ProducesZeroBytes()
        {
            var payload = new RplidarPayloadMotorPwm { pwm_value = 0 };

            var bytes = payload.GetBytes();

            Assert.Equal(2, bytes.Length);
            Assert.Equal(0x00, bytes[0]);
            Assert.Equal(0x00, bytes[1]);
        }

        [Fact]
        public void GetBytes_MotorPwmMax_ProducesCorrectBytes()
        {
            // ushort max = 65535 = 0xFFFF
            var payload = new RplidarPayloadMotorPwm { pwm_value = 0xFFFF };

            var bytes = payload.GetBytes();

            Assert.Equal(2, bytes.Length);
            Assert.Equal(0xFF, bytes[0]);
            Assert.Equal(0xFF, bytes[1]);
        }
    }
}
