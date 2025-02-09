using RpLidar.NET.Entities;
using System;
using System.IO;

namespace RpLidar.NET.Helpers
{
    /// <summary>
    /// The response descriptor helper.
    /// </summary>
    public static class ResponseDescriptorHelper
    {
        /// <summary>
        /// The data response length mask.
        /// </summary>
        private const int DataResponseLengthMask = 0x3FFFFFFF;
        /// <summary>
        /// The send mode shift.
        /// </summary>
        private const int SendModeShift = 30;

        /// <summary>
        /// Converts to response descriptor.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="InvalidDataException"></exception>
        /// <returns>A ResponseDescriptor.</returns>
        public static ResponseDescriptor ToResponseDescriptor(this byte[] data)
        {
            if (data.Length < Constants.DescriptorLength)
                throw new InvalidDataException("RESULT_INVALID_ANS_TYPE");

            if (!IsValid(data[0], data[1]))
            {
                throw new InvalidDataException("RESULT_INVALID_ANS_TYPE");
            }

            var lenAndMode = BitConverter.ToUInt32(data, 2);
            var len = lenAndMode & DataResponseLengthMask;
            var sendMode = (SendMode)(lenAndMode >> SendModeShift);

            var result = new ResponseDescriptor
            {
                ResponseLength = (int)len,
                SendMode = sendMode,
                RpDataType = (RpDataType)data[6]
            };

            return result;
        }

        /// <summary>
        /// Is valid.
        /// </summary>
        /// <param name="startFlag1">The start flag1.</param>
        /// <param name="startFlag2">The start flag2.</param>
        /// <returns>A bool.</returns>
        public static bool IsValid(byte startFlag1, byte startFlag2)
        {
            if (startFlag1 != Constants.StartFlag1 ||
                startFlag2 != Constants.StartFlag2)
            {
                return false;
            }
            return true;
        }
    }
}