using RpLidar.NET.Entities;
using RpLidar.NET.Helpers;
using Xunit;

namespace RpLidar.NET.Tests.Helpers
{
    public class DataResponseHelperTests
    {
        // ToInfoDataResponse reads: model(1), fw_minor(1), fw_major(1), hw(1), serial(16) = 20 bytes total
        private static byte[] BuildInfoBytes(byte model = 0x18, byte fwMinor = 0x19, byte fwMajor = 0x01, byte hw = 0x07)
        {
            var data = new byte[20];
            data[0] = model;
            data[1] = fwMinor;
            data[2] = fwMajor;
            data[3] = hw;
            return data;
        }

        [Fact]
        public void ToInfoDataResponse_ParsesModelId()
        {
            var response = DataResponseHelper.ToInfoDataResponse(BuildInfoBytes(model: 0x18));

            Assert.Equal("24", response.ModelId);
        }

        [Fact]
        public void ToInfoDataResponse_ParsesFirmwareVersion()
        {
            var response = DataResponseHelper.ToInfoDataResponse(BuildInfoBytes(fwMinor: 0x19, fwMajor: 0x01));

            Assert.Equal("1.25", response.FirmwareVersion);
        }

        [Fact]
        public void ToInfoDataResponse_ParsesHardwareVersion()
        {
            var response = DataResponseHelper.ToInfoDataResponse(BuildInfoBytes(hw: 0x07));

            Assert.Equal("7", response.HardwareVersion);
        }

        [Fact]
        public void ToHealthDataResponse_OkStatus_ParsesCorrectly()
        {
            var response = DataResponseHelper.ToHealthDataResponse(new byte[] { 0x00, 0x00, 0x00 });

            Assert.Equal(0, response.Status);
            Assert.Equal(0, response.ErrorCode);
        }

        [Fact]
        public void ToHealthDataResponse_WarningStatus_ParsesCorrectly()
        {
            var response = DataResponseHelper.ToHealthDataResponse(new byte[] { 0x01, 0x42, 0x00 });

            Assert.Equal(1, response.Status);
            Assert.Equal(0x42, response.ErrorCode);
        }

        [Fact]
        public void ToHealthDataResponse_ErrorStatus_ParsesCorrectly()
        {
            var response = DataResponseHelper.ToHealthDataResponse(new byte[] { 0x02, 0xFF, 0x00 });

            Assert.Equal(2, response.Status);
            Assert.Equal(0xFF, response.ErrorCode);
        }

        [Fact]
        public void ToDataResponse_GetHealthType_ReturnsHealthResponse()
        {
            var response = DataResponseHelper.ToDataResponse(RpDataType.GetHealth, new byte[] { 0x00, 0x00, 0x00 });

            Assert.IsType<RpHealthResponse>(response);
        }

        [Fact]
        public void ToDataResponse_GetInfoType_ReturnsInfoResponse()
        {
            var response = DataResponseHelper.ToDataResponse(RpDataType.GetInfo, BuildInfoBytes());

            Assert.IsType<InfoDataResponse>(response);
        }
    }
}
