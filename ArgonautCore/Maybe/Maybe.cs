using System;
using System.Threading.Tasks;

namespace ArgonautCore.Maybe
{
    public class Maybe<TVal>
    {
        /// <summary>
        /// The value that might be stored in the Maybe
        /// </summary>
        public TVal Value { get; private set; }
        
        /// <summary>
        /// The Exception that might be stored in the Maybe
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Check if the maybe has a value
        /// </summary>
        public bool HasValue { get; private set; } = false;
        
        /// <summary>
        /// Check if the maybe has an error (eg. an Exception)
        /// </summary>
        public bool HasError { get; private set; } = false;

        /// <summary>
        /// Create a Maybe using an init function/lambda that returns a tuple with the value and/or exception
        /// </summary>
        /// <param name="initializer"></param>
        public Maybe(Func<(TVal Value, Exception error)> initializer)
        {
            (TVal val, Exception ex) = initializer();
            this.Value = val;
            this.Error = ex;
            HasValue = val != null;
            HasError = ex != null;
        }

        /// <summary>
        /// Create a maybe with a value if you already have it
        /// </summary>
        /// <param name="value">Object you wish to store</param>
        public Maybe(TVal value)
        {
            Value = value;
            HasValue = true;
        }

        /// <summary>
        /// Create a maybe with an exception
        /// </summary>
        /// <param name="exception">Exception to store</param>
        public Maybe(Exception exception)
        {
            Error = exception;
            HasError = true;
        }

        /// <summary>
        /// Create a maybe with an error message. This will create a new Exception with the specified message
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public Maybe(string errorMessage)
        {
            Error = new Exception(errorMessage);
            HasError = true;
        }
        
        /// <summary>
        /// This should be used to set the Maybe to ZERO. Meaning it has no value and no error
        /// used for null wrapping
        /// </summary>
        /// <param name="hasVal">If it has a value that is just nothing</param>
        /// <param name="hasError">If it has an error that is just null</param>
        public Maybe(bool hasVal = false, bool hasError = false)
        {
            HasValue = hasVal;
            HasError = hasError;
        }

        /// <summary>
        /// Pass in two actions. The some action will be called if the maybe has a value stored.
        /// The none action will be called if an exception is stored.
        /// </summary>
        /// <param name="some">The action/function to run with the passed value</param>
        /// <param name="none">The action to run with the passed exception</param>
        /// <exception cref="NullReferenceException">Throws if both value and exception are null</exception>
        public void Get(Action<TVal> some, Action<Exception> none)
        {
            if (this.HasValue)
            {
                some(this.Value);
            }
            else if (this.HasError)
            {
                none(this.Error);
            }
            else
            {
                throw new NullReferenceException($"Both {nameof(Value)}, {nameof(Error)} are null");
            }
        }

        /// <summary>
        /// Pass in one async functions and one action. The some task will be called if the maybe has a value stored.
        /// The none action will be called if an exception is stored.
        /// </summary>
        /// <param name="some">The function to run with the passed value</param>
        /// <param name="none">The action to run with the passed exception</param>
        /// <exception cref="NullReferenceException">Throws if both value and exception are null</exception>
        public async Task GetAsync(Func<TVal, Task> some, Action<Exception> none)
        {
            if (this.HasValue)
            {
                await some(this.Value).ConfigureAwait(false);
            }
            else if (this.HasError)
            {
                none(this.Error);
            }
            else
            {
                throw new NullReferenceException($"Both {nameof(Value)}, {nameof(Error)} are null");
            }
        }
        
        /// <summary>
        /// Pass in two async functions. The some task will be called if the maybe has a value stored.
        /// The none function will be called if an exception is stored.
        /// </summary>
        /// <param name="some">The function to run with the passed value</param>
        /// <param name="none">The function to run with the passed exception</param>
        /// <exception cref="NullReferenceException">Throws if both value and exception are null</exception>
        public async Task GetAsync(Func<TVal, Task> some, Func<Exception, Task> none)
        {
            if (this.HasValue)
            {
                await some(this.Value).ConfigureAwait(false);
            }
            else if (this.HasError)
            {
                await none(this.Error).ConfigureAwait(false);
            }
            else
            {
                throw new NullReferenceException($"Both {nameof(Value)}, {nameof(Error)} are null");
            }
        }
        
        /// <summary>
        /// Pass in two functions. The some function will be called if the maybe has a value stored.
        /// The none function will be called if an exception is stored.
        /// Both must return the specified type.
        /// </summary>
        /// <param name="some"></param>
        /// <param name="none"></param>
        /// <typeparam name="TReturn"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public TReturn Get<TReturn>(Func<TVal, TReturn> some, Func<Exception, TReturn> none)
        {
            if (this.HasValue)
            {
                return some(this.Value);
            }

            if (this.HasError)
            {
                return none(this.Error);
            }

            throw new NullReferenceException($"Both {nameof(Value)}, {nameof(Error)} are null");
        }

        /// <summary>
        /// If this maybe has a value, do the action, otherwise do nothing and return this
        /// </summary>
        /// <param name="action">Action to perform</param>
        /// <returns>Returns itself</returns>
        public Maybe<TVal> Do(Action<TVal> action)
        {
            if (this.HasValue)
            {
                action(this.Value);
            }

            return this;
        }

        /// <summary>
        /// Transforms current maybe to success maybe. Thus if there is a value it will return a new maybe with a true boolean.
        /// Otherwise it will return a new maybe with the error
        /// </summary>
        /// <returns>Maybe indicating success</returns>
        public Maybe<bool> ToSuccessMaybe()
        {
            return this.Get((val) => Maybe.FromVal(true), Maybe.FromErr<bool>);
        }
    }
}