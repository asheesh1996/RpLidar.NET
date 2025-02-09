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
    }
}