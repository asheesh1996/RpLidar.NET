using System;
using System.Text;

namespace RpLidar.NET.Entities
{
    /// <summary>
    /// Represents the raw payload returned by a <c>GetLidarConf</c> (0x84) response.
    /// Provides typed accessors for the common wire formats used by sub-type queries.
    /// </summary>
    internal class LidarConfResponse
    {
        private readonly byte[] _data;

        /// <summary>Initialises a new instance from the raw response bytes.</summary>
        /// <param name="data">Raw payload bytes (excluding the response descriptor).</param>
        public LidarConfResponse(byte[] data) => _data = data;

        /// <summary>Interprets the payload as a little-endian <see cref="ushort"/>.</summary>
        public ushort ToUInt16()
            => _data != null && _data.Length >= 2
                ? BitConverter.ToUInt16(_data, 0)
                : (ushort)0;

        /// <summary>Interprets the payload as a little-endian IEEE 754 single-precision float.</summary>
        public float ToFloat()
            => _data != null && _data.Length >= 4
                ? BitConverter.ToSingle(_data, 0)
                : 0f;

        /// <summary>Interprets the payload as a null-terminated ASCII string.</summary>
        public override string ToString()
        {
            if (_data == null || _data.Length == 0)
                return string.Empty;
            int len = Array.IndexOf(_data, (byte)0);
            return len < 0
                ? Encoding.ASCII.GetString(_data)
                : Encoding.ASCII.GetString(_data, 0, len);
        }
    }
}
