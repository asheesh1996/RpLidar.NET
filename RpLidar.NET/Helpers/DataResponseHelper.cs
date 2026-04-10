using RpLidar.NET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RpLidar.NET.Helpers
{
    /// <summary>
    /// Helper methods for decoding raw RPLidar serial response packets into structured objects.
    /// </summary>
    public static class DataResponseHelper
    {
        // -----------------------------------------------------------------------
        // Ultra-Capsule varbit-scale constants — specific to the Ultra-Capsule
        // decoder and not part of the general protocol constant set.
        // -----------------------------------------------------------------------

        /// <summary>Source bit position for ×2 scale in the varbit-scale scheme.</summary>
        public const int RPLIDAR_VARBITSCALE_X2_SRC_BIT = 9;
        /// <summary>Source bit position for ×4 scale in the varbit-scale scheme.</summary>
        public const int RPLIDAR_VARBITSCALE_X4_SRC_BIT = 11;
        /// <summary>Source bit position for ×8 scale in the varbit-scale scheme.</summary>
        public const int RPLIDAR_VARBITSCALE_X8_SRC_BIT = 12;
        /// <summary>Source bit position for ×16 scale in the varbit-scale scheme.</summary>
        public const int RPLIDAR_VARBITSCALE_X16_SRC_BIT = 14;
        /// <summary>Destination value base for ×2 scale.</summary>
        public const int RPLIDAR_VARBITSCALE_X2_DEST_VAL = 512;
        /// <summary>Destination value base for ×4 scale.</summary>
        public const int RPLIDAR_VARBITSCALE_X4_DEST_VAL = 1280;
        /// <summary>Destination value base for ×8 scale.</summary>
        public const int RPLIDAR_VARBITSCALE_X8_DEST_VAL = 1792;
        /// <summary>Destination value base for ×16 scale.</summary>
        public const int RPLIDAR_VARBITSCALE_X16_DEST_VAL = 3328;

        private static readonly int[] VBS_SCALED_BASE = {
            RPLIDAR_VARBITSCALE_X16_DEST_VAL,
            RPLIDAR_VARBITSCALE_X8_DEST_VAL,
            RPLIDAR_VARBITSCALE_X4_DEST_VAL,
            RPLIDAR_VARBITSCALE_X2_DEST_VAL,
            0,
        };

        private static readonly int[] VBS_SCALED_LVL = {
            4,
            3,
            2,
            1,
            0,
        };

        private static readonly uint[] VBS_TARGET_BASE =
        {
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X16_SRC_BIT),
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X8_SRC_BIT),
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X4_SRC_BIT),
            ((uint) 0x1 <<  RPLIDAR_VARBITSCALE_X2_SRC_BIT),
            (uint) 0
        };

        /// <summary>
        /// Decodes a varbit-scaled distance value into a raw distance and its scale level.
        /// </summary>
        /// <param name="scaled">The scaled distance value read from the packet.</param>
        /// <param name="scaleLevel">Receives the power-of-two scale level (0–4).</param>
        /// <returns>The decoded raw distance value.</returns>
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

        /// <summary>
        /// Parses a raw Ultra-Capsule Express scan buffer into a queue of processed results.
        /// </summary>
        /// <param name="data">Raw bytes received from the serial port.</param>
        /// <returns>A queue of <see cref="RplidarProcessedResult"/> objects decoded from the buffer.</returns>
        public static Queue<RplidarProcessedResult> WaitUltraCapsuledNode(
            this byte[] data)
        {
            var queue = new Queue<RplidarProcessedResult>();
            if (data.Length < Constants.UltraCapsulePacketSize)
                return queue;
            var size = Constants.UltraCapsulePacketSize;
            var nodeBuffer = new byte[size];

            var pos = 0;
            if (data[0] == Constants.SYNC_BYTE && data[1] == Constants.StartFlag2)
            {
                pos = Constants.DescriptorLength;
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
                        // only consider valid if the checksum matches...

                        if ((start_angle_sync_q6 & Constants.RpLidarRespMeasurementSyncBitExp) > 0)
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

                    queue.Enqueue(new RplidarProcessedResult()
                    {
                        IsStartAngleSyncQ6 = false,
                        Value = new RplidarResponseUltraCapsuleMeasurementNodes(),
                        IsRpLidarRespMeasurementSyncBitExp = false
                    });
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
        /// Dispatches a raw response byte buffer to the appropriate typed decoder based on
        /// <paramref name="rpDataType"/>.
        /// </summary>
        /// <param name="rpDataType">The type of data response expected.</param>
        /// <param name="dataResponseBytes">The raw bytes of the response payload.</param>
        /// <returns>
        /// A typed <see cref="IDataResponse"/> instance, or <c>null</c> when the data type
        /// is not handled.
        /// </returns>
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
        /// Decodes a GetInfo response payload into an <see cref="InfoDataResponse"/>.
        /// </summary>
        /// <param name="data">The raw response payload bytes (20 bytes expected).</param>
        /// <returns>An <see cref="InfoDataResponse"/> populated with model, firmware, hardware, and serial number.</returns>
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
        /// Decodes a GetHealth response payload into an <see cref="RpHealthResponse"/>.
        /// </summary>
        /// <param name="data">The raw response payload bytes (3 bytes expected).</param>
        /// <returns>An <see cref="RpHealthResponse"/> with status and error code.</returns>
        public static RpHealthResponse ToHealthDataResponse(byte[] data)
        {
            RpHealthResponse response = new RpHealthResponse();
            response.Status = data[0];
            response.ErrorCode = BitConverter.ToUInt16(data, 1);
            return response;
        }
    }
}
