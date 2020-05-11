namespace ArgonautCore.Lw
{
    // Inspired / Copied from https://github.com/sn0w bcs it's better than what i could write by myself kekw
    
    /// <summary>
    /// Interface that describes an <see cref="Option{TVal}"/>. Use this if you do not know the underlying type and need
    /// to do pattern matching or similar cases.
    /// </summary>
    public interface IOption
    {
        /// <summary>
        /// Boolean indicating if this instance of option carries a value
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Boxes the value and returns it as a generic object.
        /// <remarks>
        /// This function is built for speed and is thus rather risky to use as it does not do ANY checks.
        /// Make sure to first check if <see cref="HasValue"/> is true before using this.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        object BoxVal();
    }
}