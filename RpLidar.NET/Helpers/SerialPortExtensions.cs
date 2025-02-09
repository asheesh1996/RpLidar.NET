using RpLidar.NET.Entities;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace RpLidar.NET.Helpers
{
    /// <summary>
    /// The serial port extensions.
    /// </summary>
    public static class SerialPortExtensions
    {
        /// <summary>
        /// Send the request.
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="command">The command.</param>
        public static void SendRequest(this SerialPort serialPort, Command command)
        {
            var commandByte = command.GetByte();

            byte[] bytes = { Constants.SYNC_BYTE, commandByte };

            serialPort.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Send the command.
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="command">The command.</param>
        /// <param name="data">The data.</param>
        public static void SendCommand(this SerialPort serialPort, byte command, byte[] data = null)
        {
            byte[] bytes;

            if ((command & 0x80) > 0 && data != null)
            {
                byte checksum = 0;
                checksum ^= (byte)0xA5;
                checksum ^= command;
                checksum ^= (byte)(data.Length & 0xFF);

                // calc checksum
                for (int pos = 0; pos < data.Length; pos++)
                {
                    checksum ^= (byte)(data[pos]);
                }
                var temp = new List<byte>()
                {
                    (byte)0xA5,command,(byte)data.Length
                };
                temp.AddRange(data);
                temp.Add(checksum);
                bytes = temp.ToArray();
            }
            else
            {
                bytes = new byte[]
                {
                    Constants.SYNC_BYTE, command
                };
            }

            serialPort.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Reads the an array of bytes.
        /// </summary>
        /// <param name="serialPort">The serial port.</param>
        /// <param name="size">The size.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>An array of bytes</returns>
        public static byte[] Read(this SerialPort serialPort, int size, int timeout)
        {
            var data = new byte[size];
            var sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < timeout)
            {
                if (serialPort.BytesToRead < size)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    serialPort.Read(data, 0, size);
                    return data;
                }
            }

            return data;
        }
    }
}