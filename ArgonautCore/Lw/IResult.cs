namespace ArgonautCore.Lw
{
    // Inspired / Copied from https://github.com/sn0w bcs it's better than what i could write by myself kekw

    /// <summary>
    /// Lightweight version of <see cref="Maybe"/> designed for structs to reduce heap allocation of just data
    /// objects. Use this when it makes more sense than a <see cref="Maybe"/>.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Boolean indicating if this instance of <see cref="Result{TVal,TErr}"/> carries a value
        /// </summary>
        bool HasValue { get; }
        
        /// <summary>
        /// Boolean indicating if this instance of <see cref="Result{TVal,TErr}"/> carries an <see cref="Error"/>
        /// </summary>
        bool HasError { get; }

        /// <summary>
        /// Boxes the value and returns it as a generic object.
        /// <remarks>
        /// This function is built for speed and is thus rather risky to use as it does not do ANY checks.
        /// Make sure to first check if <see cref="HasValue"/> is true before using this.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        object BoxValue();
        
        /// <summary>
        /// Boxes the <see cref="Error"/> and returns it as a generic object.
        /// <remarks>
        /// This function is built for speed and is thus rather risky to use as it does not do ANY checks.
        /// Make sure to first check if <see cref="HasError"/> is true before using this.
        /// </remarks>
        /// </summary>
        object BoxError();
    }
}