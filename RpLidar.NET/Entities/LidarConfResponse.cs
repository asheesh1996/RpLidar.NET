using System;
using System.Text;

namespace RpLidar.NET.Entities
{
    /// <summary>Response to a GetLidarConf query. The payload interpretation depends on SubType.</summary>
    public class LidarConfResponse : IDataResponse
    {
        public RpDataType Type => RpDataType.GetLidarConf;

        /// <summary>The configuration sub-type that was queried.</summary>
        public LidarConfSubType SubType { get; set; }

        /// <summary>Raw response payload bytes (variable length, excludes the 4-byte type prefix).</summary>
        public byte[] RawPayload { get; set; } = Array.Empty<byte>();

        /// <summary>Interprets the payload as a little-endian uint16.</summary>
        public ushort ToUInt16() => RawPayload.Length >= 2
            ? (ushort)(RawPayload[0] | (RawPayload[1] << 8))
            : (ushort)0;

        /// <summary>Interprets the payload as a little-endian uint32.</summary>
        public uint ToUInt32() => RawPayload.Length >= 4
            ? (uint)(RawPayload[0] | (RawPayload[1] << 8) | (RawPayload[2] << 16) | (RawPayload[3] << 24))
            : 0u;

        /// <summary>Interprets the payload as a little-endian IEEE 754 float.</summary>
        public float ToFloat() => RawPayload.Length >= 4
            ? BitConverter.ToSingle(RawPayload, 0)
            : 0f;

        /// <summary>Interprets the payload as a null-terminated UTF-8 string.</summary>
        public new string ToString()
        {
            if (RawPayload.Length == 0) return string.Empty;
            int nullIdx = Array.IndexOf(RawPayload, (byte)0);
            int len = nullIdx >= 0 ? nullIdx : RawPayload.Length;
            return Encoding.UTF8.GetString(RawPayload, 0, len);
        }
    }
}
