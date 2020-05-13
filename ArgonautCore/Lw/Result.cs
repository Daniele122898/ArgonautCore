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
        private readonly bool _initialized;

        /// <summary>
        /// Constructor taking a <see cref="Some{TVal}"/> with a value
        /// </summary>
        /// <param name="value"></param>
        public Result(Some<TVal> value)
        {
            _error = default;
            _value = value;

            this.HasValue = true;
            this.HasError = false;
            _initialized = true;
        }

        /// <summary>
        /// Constructor taking a <see cref="Some{TErr}"/> with an error.
        /// </summary>
        /// <param name="error"></param>
        public Result(Some<TErr> error)
        {
            _value = default;
            _error = error;

            this.HasValue = false;
            this.HasError = true;
            _initialized = true;
        }

        /// <summary>
        /// Constructor taking a value. This has to be not null or it will throw.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public Result(TVal value)
        {
            if (value == null)
                throw new NullReferenceException("Cannot construct a Result wrapper with a null value");
            
            _error = default;
            _value = value;

            this.HasValue = true;
            this.HasError = false;
            _initialized = true;
        }
        
        /// <summary>
        /// Constructor taking an error. This has to be not null or it will throw.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public Result(TErr error)
        {
            if (error == null)
                throw new NullReferenceException("Cannot construct a Result wrapper with a null error");
            
            _error = error;
            _value = default;

            this.HasValue = false;
            this.HasError = true;
            _initialized = true;
        }

        /// <inheritdoc />
        public object BoxValue() => this._value;

        /// <inheritdoc />
        public object BoxError() => this._error;

        public bool Equals(Result<TVal, TErr> other)
        {
            throw new NotImplementedException();
        }
    }
}