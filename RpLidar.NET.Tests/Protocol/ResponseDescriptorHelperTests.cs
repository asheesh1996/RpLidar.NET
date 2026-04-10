using RpLidar.NET.Entities;
using RpLidar.NET.Helpers;
using Xunit;

namespace RpLidar.NET.Tests.Protocol
{
    public class ResponseDescriptorHelperTests
    {
        [Fact]
        public void ToResponseDescriptor_ParsesResponseLength()
        {
            // 7-byte descriptor: [0xA5, 0x5A, len_lo, len_hi, len_hi2, len_subtype, datatype]
            // LE uint32 at bytes 2-5: bits[29:0] = length, bits[31:30] = send mode
            // length = 5, sendMode = SingleRequestMultipleResponse (0x1)
            // uint32 = (0x1 << 30) | 5 = 0x40000005
            // bytes: 0x05, 0x00, 0x00, 0x40
            var data = new byte[] { 0xA5, 0x5A, 0x05, 0x00, 0x00, 0x40, 0x81 };

            var descriptor = data.ToResponseDescriptor();

            Assert.Equal(5, descriptor.ResponseLength);
            Assert.Equal(SendMode.SingleRequestMultipleResponse, descriptor.SendMode);
            Assert.Equal(RpDataType.Scan, descriptor.RpDataType);
        }

        [Fact]
        public void ToResponseDescriptor_ParsesSingleResponseMode()
        {
            // sendMode = SingleRequestSingleResponse (0x0), length = 20, datatype = GetInfo (0x04)
            // uint32 = (0x0 << 30) | 20 = 0x00000014
            // bytes: 0x14, 0x00, 0x00, 0x00
            var data = new byte[] { 0xA5, 0x5A, 0x14, 0x00, 0x00, 0x00, 0x04 };

            var descriptor = data.ToResponseDescriptor();

            Assert.Equal(20, descriptor.ResponseLength);
            Assert.Equal(SendMode.SingleRequestSingleResponse, descriptor.SendMode);
            Assert.Equal(RpDataType.GetInfo, descriptor.RpDataType);
        }

        [Fact]
        public void IsValid_ReturnsTrueForCorrectStartFlags()
        {
            Assert.True(ResponseDescriptorHelper.IsValid(0xA5, 0x5A));
        }

        [Fact]
        public void IsValid_ReturnsFalseForIncorrectSecondFlag()
        {
            Assert.False(ResponseDescriptorHelper.IsValid(0xA5, 0x00));
        }

        [Fact]
        public void IsValid_ReturnsFalseForIncorrectFirstFlag()
        {
            Assert.False(ResponseDescriptorHelper.IsValid(0x00, 0x5A));
        }

        [Fact]
        public void IsValid_ReturnsFalseForBothFlagsWrong()
        {
            Assert.False(ResponseDescriptorHelper.IsValid(0x00, 0x00));
        }

        [Fact]
        public void ToResponseDescriptor_ThrowsOnTooShortData()
        {
            var shortData = new byte[] { 0xA5, 0x5A, 0x00 };
            Assert.Throws<System.IO.InvalidDataException>(() => shortData.ToResponseDescriptor());
        }

        [Fact]
        public void ToResponseDescriptor_ThrowsOnInvalidStartFlags()
        {
            var data = new byte[] { 0xA5, 0x00, 0x05, 0x00, 0x00, 0x40, 0x81 };
            Assert.Throws<System.IO.InvalidDataException>(() => data.ToResponseDescriptor());
        }
    }
}
