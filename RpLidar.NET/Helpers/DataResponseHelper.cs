using RpLidar.NET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RpLidar.NET.Helpers
{
    /// <summary>
    /// The data response helper.
    /// </summary>
    public static class DataResponseHelper
    {
        /// <summary>
        /// The rp lidar resp measurement sync bit.
        /// </summary>
        public static int RpLidarRespMeasurementSyncBit = (0x1 << 0);
        /// <summary>
        /// The rp lidar resp measurement sync bit exp.
        /// </summary>
        public static int RpLidarRespMeasurementSyncBitExp = (0x1 << 15);

        /// <summary>
        /// Parses a stream of 80-byte Classic Capsule packets (response type 0x82) from a raw byte buffer.
        /// Sync pattern: byte[0] high-nibble == 0xA, byte[1] high-nibble == 0x5.
        /// Checksum: XOR of bytes [2..79] must equal ((byte[0] &amp; 0x0F) | ((byte[1] &amp; 0x0F) &lt;&lt; 4)).
        /// </summary>
        /// <param name="data">Raw byte buffer from the serial port.</param>
        /// <returns>Queue of parsed <see cref="RplidarResponseCapsuleMeasurementNodes"/> packets.</returns>
        public static Queue<RplidarResponseCapsuleMeasurementNodes> WaitCapsuledNode(this byte[] data)
        {
            var result = new Queue<RplidarResponseCapsuleMeasurementNodes>();
            const int PacketSize = 80;
            int pos = 0;

            while (pos + PacketSize <= data.Length)
            {
                // Locate sync pattern
                if ((data[pos] >> 4) != 0xA || (data[pos + 1] >> 4) != 0x5)
                {
                    pos++;
                    continue;
                }

                // Verify checksum: XOR of bytes [2..79] == low nibbles of sync bytes
                byte expectedChecksum = (byte)((data[pos] & 0x0F) | ((data[pos + 1] & 0x0F) << 4));
                byte actualChecksum = 0;
                for (int i = pos + 2; i < pos + PacketSize; i++)
                    actualChecksum ^= data[i];

                if (actualChecksum != expectedChecksum)
                {
                    pos++;
                    continue;
                }

                var node = new RplidarResponseCapsuleMeasurementNodes
                {
                    SyncByte1 = data[pos],
                    SyncByte2 = data[pos + 1],
                    StartAngleSyncQ6 = (ushort)(data[pos + 2] | (data[pos + 3] << 8)),
                    CabinData = new byte[76]
                };

                // Copy the 16 × 5-byte cabin data verbatim (bytes 4..79 of the packet = 76 bytes)
                Array.Copy(data, pos + 4, node.CabinData, 0, 76);

                result.Enqueue(node);
                pos += PacketSize;
            }

            return result;
        }

        /// <summary>
        /// Wait ultra capsuled node.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns><![CDATA[Queue<RplidarProcessedResult>]]></returns>
        public static Queue<RplidarProcessedResult> WaitUltraCapsuledNode(
            this byte[] data)
        {
            var queue = new Queue<RplidarProcessedResult>();
            if (data.Length < 132)
                return queue;
            var size = 132;
            var nodeBuffer = new byte[size];

            var pos = 0;
            if (data[0] == 0xA5 && data[1] == 0x5A)
            {
                pos = 7;
            }

            int recvPos = 0;
            var lastFoundPos = 0;
            for (; pos < data.Length; ++pos)
            {
                var currentByte = data[pos];
                switch (recvPos)
                {
                    case 0: // expect the sync bit 1
                        {
                            var tmp = (currentByte >> 4);
                            if (tmp == 0xA)
                            {
                                // pass
                            }
                            else
                            {
                                queue.Enqueue(new RplidarProcessedResult()
                                {
                                    IsStartAngleSyncQ6 = false,
                                    Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
                                    IsRpLidarRespMeasurementSyncBitExp = false
                                });
                                continue;
                            }
                        }
                        break;

                    case 1: // expect the sync bit 2
                        {
                            var tmp = (currentByte >> 4);
                            if (tmp == 0x5)
                            {
                                // pass
                            }
                            else
                            {
                                queue.Enqueue(new RplidarProcessedResult()
                                {
                                    IsStartAngleSyncQ6 = false,
                                    Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
                                    IsRpLidarRespMeasurementSyncBitExp = false
                                });
                                recvPos = 0;
                                continue;
                            }
                        }
                        break;
                }

                nodeBuffer[recvPos] = currentByte;
                recvPos++;
                if (recvPos == size)
                {
                    lastFoundPos = pos;
                    recvPos = 0;

                    var recvChecksum = (((nodeBuffer[1] & 0xF) << 4) | (nodeBuffer[0] & 0xF));

                    byte checksum = 0;
                    for (var cpos = 2; cpos < size; cpos++)
                    {
                        checksum ^= (byte)nodeBuffer[cpos];
                    }

                    if (recvChecksum == checksum)
                    {
                        var result = new RplidarResponseUltraCapsuleMeasurementNodes()
                        {
                        };
                        result.s_checksum_1 = nodeBuffer[0];
                        result.s_checksum_2 = nodeBuffer[1];
                        var cabin = nodeBuffer.Skip(4).ToArray().FromBytes<uint>();

                        var start_angle_sync_q6 = BitConverter.ToUInt16(nodeBuffer, 2);
                        result.ultra_cabins = cabin.ToArray();

                        result.start_angle_sync_q6 = (ushort)start_angle_sync_q6;
                        // only consider vaild if the checksum matches...

                        if ((start_angle_sync_q6 & DataResponseHelper.RpLidarRespMeasurementSyncBitExp) > 0)
                        {
                            queue.Enqueue(new RplidarProcessedResult()
                            {
                                IsStartAngleSyncQ6 = true,
                                Value = result,
                                IsRpLidarRespMeasurementSyncBitExp = false
                            });
                        }
                        else
                        {
                            queue.Enqueue(new RplidarProcessedResult()
                            {
                                IsStartAngleSyncQ6 = true,
                                Value = result,
                                IsRpLidarRespMeasurementSyncBitExp = true
                            });
                        }

                        continue;
                    }

                    //_is_previous_capsuledataRdy = false;
                    queue.Enqueue(new RplidarProcessedResult()
                    {
                        IsStartAngleSyncQ6 = false,
                        Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
                        IsRpLidarRespMeasurementSyncBitExp = false
                    });
                    //return RESULT_INVALID_DATA;
                }
            }

            var last = new RplidarProcessedResult()
            {
                IsStartAngleSyncQ6 = false,
                Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
                IsRpLidarRespMeasurementSyncBitExp = false
            };
            if (lastFoundPos < pos - 1)
            {
                var foundPos = lastFoundPos + 1;
                last.RemainderData = data.Skip(foundPos).Take(pos - foundPos).ToArray();
            }

            queue.Enqueue(last);
            return queue;
        }

        /// <summary>
        /// The RPLIDA R RES P MEASUREMEN T ANGL E SHIFT.
        /// </summary>
        public static int RPLIDAR_RESP_MEASUREMENT_ANGLE_SHIFT = 1;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x2 SR C BIT.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X2_SRC_BIT = 9;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x4 SR C BIT.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X4_SRC_BIT = 11;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x8 SR C BIT.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X8_SRC_BIT = 12;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x16 SR C BIT.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X16_SRC_BIT = 14;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x2 DES T VAL.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X2_DEST_VAL = 512;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x4 DES T VAL.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X4_DEST_VAL = 1280;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x8 DES T VAL.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X8_DEST_VAL = 1792;
        /// <summary>
        /// The RPLIDA R VARBITSCAL E x16 DES T VAL.
        /// </summary>
        public static int RPLIDAR_VARBITSCALE_X16_DEST_VAL = 3328;
        /// <summary>
        /// The RPLIDA R RES P MEASUREMEN T QUALIT Y SHIFT.
        /// </summary>
        public static int RPLIDAR_RESP_MEASUREMENT_QUALITY_SHIFT = 2;

        /// <summary>
        /// The VB S SCALE D BASE.
        /// </summary>
        private static int[] VBS_SCALED_BASE = {
            RPLIDAR_VARBITSCALE_X16_DEST_VAL,
            RPLIDAR_VARBITSCALE_X8_DEST_VAL,
            RPLIDAR_VARBITSCALE_X4_DEST_VAL,
            RPLIDAR_VARBITSCALE_X2_DEST_VAL,
            0,
        };

        /// <summary>
        /// The VB S SCALE D LVL.
        /// </summary>
        private static int[] VBS_SCALED_LVL = {
            4,
            3,
            2,
            1,
            0,
        };

        /// <summary>
        /// The VB S TARGE T BASE.
        /// </summary>
        private static uint[] VBS_TARGET_BASE =
        {
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X16_SRC_BIT),
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X8_SRC_BIT),
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X4_SRC_BIT),
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X2_SRC_BIT),
            (uint) 0
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaled">The scaled.</param>
        /// <param name="scaleLevel">The scale level.</param>
        /// <returns>An uint.</returns>
        public static uint _varbitscale_decode(int scaled, out int scaleLevel)
        {
            scaleLevel = 0;
            for (var i = 0; i < VBS_TARGET_BASE.Length; i++)
            {
                int remain = (scaled - VBS_SCALED_BASE[i]);
                if (remain >= 0)
                {
                    scaleLevel = VBS_SCALED_LVL[i];
                    var sc = (remain << scaleLevel);
                    var varbitscaleDecode = (uint)(VBS_TARGET_BASE[i] + sc);
                    return varbitscaleDecode;
                }
            }
            return 0;
        }

        /// <summary>Parses Dense Capsule packets (84 bytes each) from a raw buffer.</summary>
        public static Queue<RplidarResponseDenseCapsuleMeasurementNodes> WaitDenseCapsuledNode(this byte[] data)
        {
            var result = new Queue<RplidarResponseDenseCapsuleMeasurementNodes>();
            const int PacketSize = 84;
            int pos = 0;
            while (pos + PacketSize <= data.Length)
            {
                if ((data[pos] >> 4) != 0xA || (data[pos + 1] >> 4) != 0x5) { pos++; continue; }

                var node = new RplidarResponseDenseCapsuleMeasurementNodes
                {
                    SyncByte1 = data[pos],
                    SyncByte2 = data[pos + 1],
                    StartAngleSyncQ6 = (ushort)(data[pos + 2] | (data[pos + 3] << 8)),
                    Distances = new ushort[40]
                };
                for (int i = 0; i < 40; i++)
                    node.Distances[i] = (ushort)(data[pos + 4 + i * 2] | (data[pos + 5 + i * 2] << 8));

                result.Enqueue(node);
                pos += PacketSize;
            }
            return result;
        }

        /// <summary>Parses Ultra-Dense Capsule packets from a raw buffer.</summary>
        public static Queue<RplidarResponseUltraDenseCapsuleMeasurementNodes> WaitUltraDenseCapsuledNode(this byte[] data)
        {
            // Packet: 2 sync + 4 timestamp + 2 startAngle + 40*(2 dist + 1 quality) + 2 checksum = 130 bytes
            var result = new Queue<RplidarResponseUltraDenseCapsuleMeasurementNodes>();
            const int PacketSize = 130;
            int pos = 0;
            while (pos + PacketSize <= data.Length)
            {
                if ((data[pos] >> 4) != 0xA || (data[pos + 1] >> 4) != 0x5) { pos++; continue; }

                var node = new RplidarResponseUltraDenseCapsuleMeasurementNodes
                {
                    SyncByte1 = data[pos],
                    SyncByte2 = data[pos + 1],
                    TimestampUs = (uint)(data[pos + 2] | (data[pos + 3] << 8) | (data[pos + 4] << 16) | (data[pos + 5] << 24)),
                    StartAngleSyncQ6 = (ushort)(data[pos + 6] | (data[pos + 7] << 8)),
                    Distances = new ushort[40],
                    Qualities = new byte[40]
                };
                for (int i = 0; i < 40; i++)
                {
                    node.Distances[i] = (ushort)(data[pos + 8 + i * 3] | (data[pos + 9 + i * 3] << 8));
                    node.Qualities[i] = data[pos + 10 + i * 3];
                }
                result.Enqueue(node);
                pos += PacketSize;
            }
            return result;
        }

        /// <summary>
        /// Converts to data response.
        /// </summary>
        /// <param name="rpDataType">The rp data type.</param>
        /// <param name="dataResponseBytes">The data response bytes.</param>
        /// <returns>An IDataResponse.</returns>
        public static IDataResponse ToDataResponse(RpDataType rpDataType, byte[] dataResponseBytes)
        {
            switch (rpDataType)
            {
                case RpDataType.GetHealth:
                    return ToHealthDataResponse(dataResponseBytes);

                case RpDataType.GetInfo:
                    return ToInfoDataResponse(dataResponseBytes);
            }
            return null;
        }

        /// <summary>
        /// Converts to info data response.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>An InfoDataResponse.</returns>
        public static InfoDataResponse ToInfoDataResponse(byte[] data)
        {
            InfoDataResponse dataResponse = new InfoDataResponse();
            //Mode ID
            var model = data[0];
            dataResponse.ModelId = model.ToString();
            // Firmware version number, the minor value part, decimal
            var firmwareVersionMinor = data[1];
            // Firmware version number, the major value part, integer
            var firmwareVersionMajor = data[2];
            dataResponse.FirmwareVersion = firmwareVersionMajor + "." + firmwareVersionMinor;
            //Hardware version number
            var hardwareVersion = data[3];
            dataResponse.HardwareVersion = hardwareVersion.ToString();
            // 128bit unique serial number
            byte[] serialNumber = new byte[16];
            for (int i = 4; i < 20; i++)
            {
                serialNumber[i - 4] = data[i];
            }
            string serial = BitConverter.ToString(serialNumber).Replace("-", "");
            dataResponse.SerialNumber = serial;

            return dataResponse;
        }

        /// <summary>
        /// Converts to health data response.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A RpHealthResponse.</returns>
        public static RpHealthResponse ToHealthDataResponse(byte[] data)
        {
            RpHealthResponse response = new RpHealthResponse();
            response.Status = data[0];
            response.ErrorCode = BitConverter.ToUInt16(data, 1);
            return response;
        }

        /// <summary>
        /// Parses a stream of 16-byte HQ scan nodes from a raw buffer.
        /// Framing: no sync bytes — rely on Flag bit 0 (new-scan indicator) for alignment.
        /// </summary>
        public static List<RplidarResponseHqMeasurementNode> ParseHqNodes(this byte[] data)
        {
            const int NodeSize = 16;
            var result = new List<RplidarResponseHqMeasurementNode>();
            int pos = 0;
            while (pos + NodeSize <= data.Length)
            {
                var node = new RplidarResponseHqMeasurementNode
                {
                    AngleZQ14 = (uint)(data[pos] | (data[pos + 1] << 8) | (data[pos + 2] << 16) | (data[pos + 3] << 24)),
                    DistMmQ2  = (uint)(data[pos + 4] | (data[pos + 5] << 8) | (data[pos + 6] << 16) | (data[pos + 7] << 24)),
                    Quality   = data[pos + 8],
                    Flag      = data[pos + 9],
                    TimestampUs = (ulong)data[pos + 10] | ((ulong)data[pos + 11] << 8) | ((ulong)data[pos + 12] << 16) |
                                  ((ulong)data[pos + 13] << 24) | ((ulong)data[pos + 14] << 32) | ((ulong)data[pos + 15] << 40)
                };
                result.Add(node);
                pos += NodeSize;
            }
            return result;
        }
    }
}