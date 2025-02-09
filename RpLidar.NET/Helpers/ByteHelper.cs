using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RpLidar.NET.Helpers
{
    /// <summary>
    /// The byte helper.
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// Converts to str.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A string.</returns>
        public static string ToStr(this byte[] source)
        {
            var sBuilder = new StringBuilder(256);
            foreach (byte t in source)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="str">The str.</param>
        /// <returns>An array of bytes</returns>
        public static byte[] GetBytes<TStruct>(this TStruct str) where TStruct : struct
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Froms the bytes.
        /// </summary>
        /// <typeparam name="TStruct"></typeparam>
        /// <param name="arr">The arr.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><![CDATA[List<TStruct>]]></returns>
        public static List<TStruct> FromBytes<TStruct>(this byte[] arr, int offset = 0) where TStruct : struct
        {
            var value = new TStruct();
            var structureType = value.GetType();
            var size = Marshal.SizeOf(value);

            if (size == 0)
                return null;

            var len = arr.Length / size;
            var result = new List<TStruct>(len);

            while (offset < arr.Length)
            {
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(arr, offset, ptr, size);
                value = (TStruct)Marshal.PtrToStructure(ptr, structureType);
                Marshal.FreeHGlobal(ptr);

                result.Add(value);
                offset += size;
            }

            return result;
        }
    }
}