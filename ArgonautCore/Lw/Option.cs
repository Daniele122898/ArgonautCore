using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgonautCore.Lw
{
    // Inspired / Copied from https://github.com/sn0w bcs it's better than what i could write by myself kekw

    /// <summary>
    /// Wrapper for a value that might exist or maybe not. Comparable to <see cref="Maybe"/> but more lightweight
    /// and does not carry an exception or error. Just the absence or presence of a value.
    /// </summary>
    /// <typeparam name="TVal"></typeparam>
    public readonly struct Option<TVal> : IOption, IEquatable<Option<TVal>> where TVal : notnull
    {
        /// <inheritdoc />
        public bool HasValue { get; }

        /// <summary>
        /// Actual value held by Option. 
        /// </summary>
        private readonly TVal _value;

        private readonly bool _initialized;

        /// <summary>
        /// Constructs a new optional using the some value safety
        /// </summary>
        /// <param name="value"></param>
        public Option(Some<TVal> value)
        {
            this._value = value;
            this.HasValue = true;
            _initialized = true;
        }

        /// <summary>
        /// Creates a new option without the conversion to Some first
        /// </summary>
        /// <param name="value"></param>
        public Option(TVal value)
        {
            if (value == null)
                throw new NullReferenceException(
                    "Cannot construct a Option wrapper with a null value. Use different constructor for a none type");

            this._value = value;
            this.HasValue = true;
            _initialized = true;
        }

        /// <summary>
        /// Use this to create a None option.
        /// </summary>
        /// <param name="hasVal"></param>
        public Option(bool hasVal)
        {
            if (hasVal)
                throw new ArgumentException("This constructor can only be used to create none types");

            this._value = default;
            this.HasValue = false;
            _initialized = true;
        }

        /// <summary>
        /// Converts any TVal to an optional wrapper.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static implicit operator Option<TVal>(TVal val)
            => new Option<TVal>(val);

        /// <summary>
        /// You can use this type conversion to unwrap the Option, given that it has a value.
        /// This will throw otherwise
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static explicit operator TVal(Option<TVal> opt)
        {
            if (opt.HasValue)
                return opt._value;

            throw new NullReferenceException("Tried casting an Option{TVal} to TVal that doesn't have a value!");
        }

        /// <summary>
        /// Implicitly get the boolean value of <see cref="HasValue"/> of this <see cref="Option{TVal}"/>
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public static implicit operator bool(Option<TVal> opt)
            => opt.HasValue;

        /// <summary>
        /// Unwraps the Option and returns the value if present. If there is no value this will throw 
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static TVal operator ~(Option<TVal> opt)
        {
            if (opt.HasValue)
                return opt._value;

            throw new NullReferenceException("Tried unwrapping an Option that doesn't have a value!");
        }

/*
        private TVal GetDataIfInitialized()
        {
            if (!_initialized)
                throw new NullReferenceException("This Some struct was created without initializing any data");

            return Value;
        }
*/

        /// <summary>
        /// Fast getter for the value if you're sure it has one. If no value is present this will
        /// throw.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public TVal Some()
        {
            if (this.HasValue)
                return this._value;

            throw new NullReferenceException("Tried to access an option that had no value");
        }

        /// <summary>
        /// Executes the lambda when a value is present, does nothing if there's none.
        /// </summary>
        /// <param name="some"></param>
        public void MatchSome(Action<TVal> some)
        {
            if (this.HasValue)
                some(this._value);
        }

        /// <summary>
        /// Executes the lambda when a value is present, does nothing if there's none.
        /// </summary>
        /// <param name="some"></param>
        public async Task MatchSomeAsync(Func<TVal, Task> some)
        {
            if (this.HasValue)
                await some(this._value).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute lambda when no value is present. 
        /// </summary>
        /// <param name="none"></param>
        public void MatchNone(Action none)
        {
            if (!this.HasValue)
                none();
        }

        /// <summary>
        /// Execute lambda when no value is present. 
        /// </summary>
        /// <param name="none"></param>
        public async Task MatchNoneAsync(Func<Task> none)
        {
            if (!this.HasValue)
                await none().ConfigureAwait(false);
        }

        /// <summary>
        /// Execute the matching lambda based on whether a value is present or not
        /// </summary>
        /// <param name="some"></param>
        /// <param name="none"></param>
        public void Match(Action<TVal> some, Action none)
        {
            if (this.HasValue)
            {
                some(this._value);
                return;
            }

            none();
        }

        /// <summary>
        /// Execute the matching lambda and return the result
        /// </summary>
        /// <param name="some"></param>
        /// <param name="none"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Match<T>(Func<TVal, T> some, Func<T> none)
        {
            if (this.HasValue)
                return some(this._value);
            return none();
        }

        /// <summary>
        /// Return the value if present or a fixed default
        /// </summary>
        /// <param name="default"></param>
        /// <returns></returns>
        public TVal SomeOrDefault(Some<TVal> @default)
            => this.HasValue ? this._value : ~@default;

        /// <summary>
        /// Return the value if present or a computed default
        /// </summary>
        /// <param name="default"></param>
        /// <returns></returns>
        public TVal SomeOrDefault(Func<TVal> @default)
            => this.HasValue ? this._value : @default();

        /// <summary>
        /// Return the value if present or a computed default
        /// </summary>
        /// <param name="default"></param>
        /// <returns></returns>
        public async Task<TVal> SomeOrDefaultAsync(Func<Task<TVal>> @default)
            => this.HasValue ? this._value : await @default().ConfigureAwait(false);

        /// <inheritdoc />
        public object BoxVal()
            => this._value;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this._value, this.HasValue, _initialized);
        }

        /// <inheritdoc />
        public bool Equals(Option<TVal> other)
        {
            return
                EqualityComparer<TVal>.Default.Equals(this._value, other._value) &&
                this.HasValue == other.HasValue &&
                _initialized == other._initialized;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Option<TVal> other && Equals(other);
        }

        /// <summary>
        /// Checks for equality of the Option
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Option<TVal> left, Option<TVal> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks for inequality of the Option
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Option<TVal> left, Option<TVal> right)
        {
            return !left.Equals(right);
        }
    }
}