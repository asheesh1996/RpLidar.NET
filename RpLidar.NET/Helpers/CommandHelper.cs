using RpLidar.NET.Entities;

namespace RpLidar.NET.Helpers
{
    /// <summary>
    /// The command helper.
    /// </summary>
    public static class CommandHelper
    {
        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A byte.</returns>
        public static byte GetByte(this Command command)
        {
            return (byte)command;
        }

        /// <summary>
        /// Has response.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A bool.</returns>
        public static bool HasResponse(this Command command)
        {
            return command != Command.Stop && command != Command.Reset;
        }

        /// <summary>
        /// Get the has response.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A bool.</returns>
        public static bool GetHasResponse(byte command)
        {
            return command != (byte)Command.Stop && command != (byte)Command.Reset;
        }

        /// <summary>
        /// Gets the sleep interval.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>An int.</returns>
        public static int GetSleepInterval(this Command command)
        {
            if (command == Command.Reset || command == Command.Stop)
                return 20;
            return 0;
        }

        /// <summary>
        /// Get the must sleep.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>An int.</returns>
        public static int GetMustSleep(this byte command)
        {
            return ((Command)command).GetSleepInterval();
        }
    }
}