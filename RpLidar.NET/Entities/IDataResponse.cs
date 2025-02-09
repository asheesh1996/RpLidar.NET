namespace RpLidar.NET.Entities
{
    /// <summary>
    /// The data response interface.
    /// </summary>
    public interface IDataResponse
    {
        /// <summary>
        /// <see cref="RpDataType"/>
        /// </summary>
        RpDataType Type { get; }
    }
}