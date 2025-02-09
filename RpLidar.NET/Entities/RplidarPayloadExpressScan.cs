namespace RpLidar.NET.Entities
{
    internal struct RplidarPayloadExpressScan
    {
        /// <summary>
        /// The workingmode.
        /// </summary>
        internal byte working_mode;
        /// <summary>
        /// The workingflags.
        /// </summary>
        internal ushort working_flags;
        /// <summary>
        /// The param.
        /// </summary>
        internal ushort param;
    }
}