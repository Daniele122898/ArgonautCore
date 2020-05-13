using System;

namespace ArgonautCore.Lw
{
    // Inspired / Copied from https://github.com/sn0w bcs it's better than what i could write by myself kekw

    /// <summary>
    /// Struct implementation of the <see cref="Maybe"/> type. This could me faster since we avoid
    /// heap allocations. Make sure that it doesn't get copied too much though.
    /// </summary>
    /// <typeparam name="TVal"></typeparam>
    /// <typeparam name="TErr"></typeparam>
    public readonly struct Result<TVal, TErr> : IResult, IEquatable<Result<TVal, TErr>>
    where TVal : notnull
    where TErr : notnull
    {
        /// <inheritdoc />
        public bool HasValue { get; }

        /// <inheritdoc />
        public bool HasError { get; }

        private readonly TVal _value;
        private readonly TErr _error;

        public object BoxValue()
        {
            throw new NotImplementedException();
        }

        public object BoxError()
        {
            throw new NotImplementedException();
        }

        public bool Equals(Result<TVal, TErr> other)
        {
            throw new NotImplementedException();
        }
    }
}