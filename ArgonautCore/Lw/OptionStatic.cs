namespace ArgonautCore.Lw
{
    // Inspired / Copied from https://github.com/sn0w bcs it's better than what i could write by myself kekw
    
    /// <summary>
    /// Static helpers to work and better initialize <see cref="Option{TVal}"/> types
    /// </summary>
    public static class Option
    {
        
        /// <summary>
        /// Constructs a new <see cref="Option{TVal}"/> with the not null value.
        /// </summary>
        /// <param name="val"></param>
        /// <typeparam name="TVal"></typeparam>
        /// <returns></returns>
        public static Option<TVal> Some<TVal>(TVal val) where TVal : notnull
            => new Option<TVal>(val);

        /// <summary>
        /// Constructs a new <see cref="Option{TVal}"/> as None.
        /// </summary>
        /// <typeparam name="TVal"></typeparam>
        /// <returns></returns>
        public static Option<TVal> None<TVal>() where TVal : notnull
            => new Option<TVal>(false);
    }
}