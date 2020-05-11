using System;
using System.Collections.Generic;

namespace ArgonautCore.Lw
{
    // Inspired / Copied from https://github.com/sn0w bcs it's better than what i could write by myself kekw
    
    /// <summary>
    /// Wraps a value that is required to be not null. Basically guaranteeing that anything wrapped in Some
    /// will always have a value. Trying to construct a Some wrapper with null will throw
    /// </summary>
    public readonly struct Some<TVal> : IEquatable<Some<TVal>>
    {
        /// <summary>
        /// Actual value that is held by this Some
        /// </summary>
        public readonly TVal Value;

        // Initialized is needed because structs will always have a default empty constructor.
        // We want to basically disallow that :)
        private readonly bool _initialized;

        /// <summary>
        /// Construct a new Some. If the value passed is null this constructor will throw
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NullReferenceException"></exception>
        public Some(TVal value)
        {
            if (value == null)
                throw new NullReferenceException("Cannot construct a Some wrapper with a null value");

            this.Value = value;
            _initialized = true;
        }

        /// <summary>
        /// With this you can implicitly wrap any value in a Some struct.
        /// This will throw if value is null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static implicit operator Some<TVal>(TVal value) => new Some<TVal>(value);

        /// <summary>
        /// Implicitly get the value of the some if it was initialized properly.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">If it was initialized with default ctor</exception>
        public static implicit operator TVal(Some<TVal> value) => value.GetDataIfInitialized();

        /// <summary>
        /// Custom operator to get the wrapped value if the implicit operators can't be applied.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TVal operator ~(Some<TVal> value) => value.GetDataIfInitialized();

        /// <summary>
        /// Helper to get wrapped value.
        /// </summary>
        /// <returns></returns>
        public TVal Get() => this.GetDataIfInitialized();

        private TVal GetDataIfInitialized()
        {
            if (!_initialized)
                throw new NullReferenceException("This Some struct was created without initializing any data");

            return Value;
        }

        /// <summary>
        /// Checks for equality of the wrapped values.
        /// </summary>
        public static bool operator ==(Some<TVal> left, Some<TVal> right) => left.Equals(right);
        
        /// <summary>
        /// Checks for inequality of the wrapped values.
        /// </summary>
        public static bool operator !=(Some<TVal> left, Some<TVal> right) => !left.Equals(right);

        /// <inheritdoc />
        public bool Equals(Some<TVal> other) => 
            EqualityComparer<TVal>.Default.Equals(Value, other.Value) && _initialized == other._initialized;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Some<TVal> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Value, _initialized);
    }
}