using RpLidar.NET.Entities;

namespace RpLidar.NET.Helpers
{
    /// <summary>
    /// Extension methods and utilities for the <see cref="Command"/> enum.
    /// </summary>
    public static class CommandHelper
    {
        /// <summary>
        /// Returns the wire byte value of the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The byte representation of the command code.</returns>
        public static byte GetByte(this Command command)
        {
            return (byte)command;
        }

        /// <summary>
        /// Returns <c>true</c> when the device is expected to send one or more response packets
        /// after the command is issued.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// <c>true</c> if the device sends a response; <c>false</c> for fire-and-forget commands.
        /// </returns>
        public static bool HasResponse(this Command command)
        {
            switch (command)
            {
                case Command.Stop:
                case Command.Reset:
                case Command.SetMotorSpeed:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns <c>true</c> when the device is expected to send one or more response packets
        /// after the raw command byte is issued.
        /// </summary>
        /// <param name="command">The raw command byte.</param>
        /// <returns>
        /// <c>true</c> if the device sends a response; <c>false</c> for fire-and-forget commands.
        /// </returns>
        public static bool GetHasResponse(byte command)
        {
            return ((Command)command).HasResponse();
        }

        /// <summary>
        /// Returns the number of milliseconds the driver should sleep after issuing the command
        /// before reading or sending the next one.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Sleep duration in milliseconds (0 = no sleep required).</returns>
        public static int GetSleepInterval(this Command command)
        {
            switch (command)
            {
                case Command.Reset:
                    return 2000; // device reboots; wait for it to be ready
                case Command.Stop:
                    return 20;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns the number of milliseconds the driver should sleep after issuing the raw
        /// command byte.
        /// </summary>
        /// <param name="command">The raw command byte.</param>
        /// <returns>Sleep duration in milliseconds (0 = no sleep required).</returns>
        public static int GetMustSleep(this byte command)
        {
            return ((Command)command).GetSleepInterval();
        }
    }
}
