using RpLidar.NET.Entities;
using RpLidar.NET.Helpers;
using System.Collections.Generic;
using Xunit;

namespace RpLidar.NET.Tests.Decoders
{
    public class UltraCapsuleDecoderTests
    {
        // Builds a valid 132-byte Ultra-Capsule packet.
        // All cabin bytes are zero, so XOR checksum = 0 and the sync byte low nibbles = 0.
        private static byte[] BuildPacket(ushort startAngleSyncQ6)
        {
            var packet = new byte[132];
            packet[0] = 0xA0; // high nibble 0xA = sync1, low nibble 0 = checksum[3:0]
            packet[1] = 0x50; // high nibble 0x5 = sync2, low nibble 0 = checksum[7:4]
            packet[2] = (byte)(startAngleSyncQ6 & 0xFF);
            packet[3] = (byte)(startAngleSyncQ6 >> 8);
            return packet;
        }

        private static RplidarProcessedResult? FirstSuccessful(Queue<RplidarProcessedResult> queue)
        {
            foreach (var item in queue)
                if (item.IsStartAngleSyncQ6)
                    return item;
            return null;
        }

        [Fact]
        public void WaitUltraCapsuledNode_TwoValidPackets_QueueContainsTwoSuccessfulEntries()
        {
            var packet1 = BuildPacket(0x0000);
            var packet2 = BuildPacket(0x8050);
            var combined = new byte[264];
            packet1.CopyTo(combined, 0);
            packet2.CopyTo(combined, 132);

            var queue = combined.WaitUltraCapsuledNode();

            int count = 0;
            foreach (var item in queue)
                if (item.IsStartAngleSyncQ6)
                    count++;

            Assert.Equal(2, count);
        }

        [Fact]
        public void WaitUltraCapsuledNode_PacketWithSyncBit_IsRpLidarRespMeasurementSyncBitExpFalse()
        {
            // Bit 15 of start_angle_sync_q6 set → code sets IsRpLidarRespMeasurementSyncBitExp = false
            var queue = BuildPacket(0x8050).WaitUltraCapsuledNode();

            var result = FirstSuccessful(queue);
            Assert.NotNull(result);
            Assert.False(result!.IsRpLidarRespMeasurementSyncBitExp);
        }

        [Fact]
        public void WaitUltraCapsuledNode_PacketWithoutSyncBit_IsRpLidarRespMeasurementSyncBitExpTrue()
        {
            // Bit 15 of start_angle_sync_q6 clear → code sets IsRpLidarRespMeasurementSyncBitExp = true
            var queue = BuildPacket(0x0000).WaitUltraCapsuledNode();

            var result = FirstSuccessful(queue);
            Assert.NotNull(result);
            Assert.True(result!.IsRpLidarRespMeasurementSyncBitExp);
        }

        [Fact]
        public void WaitUltraCapsuledNode_TooShortData_ReturnsEmptyQueue()
        {
            var queue = new byte[64].WaitUltraCapsuledNode();

            Assert.Empty(queue);
        }

        [Fact]
        public void WaitUltraCapsuledNode_ZeroCabinData_ParsesStartAngle()
        {
            var queue = BuildPacket(0x0000).WaitUltraCapsuledNode();

            var result = FirstSuccessful(queue);
            Assert.NotNull(result);
            Assert.Equal(0x0000, result!.Value.start_angle_sync_q6);
        }
    }
}
