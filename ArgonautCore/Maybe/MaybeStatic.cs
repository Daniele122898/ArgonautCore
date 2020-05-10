using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgonautCore.Maybe
{
    /// <summary>
    /// Static helper class for the Maybe wrapper
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// If your initialization must be async then use this function.
        /// </summary>
        /// <param name="initializer">The task that will initialize the maybe</param>
        /// <typeparam name="T">What type of value the maybe shall hold</typeparam>
        /// <returns>The new maybe</returns>
        /// <exception cref="NullReferenceException">If your initializer failed to deliver either a value or an exception</exception>
        public static async Task<Maybe<T>> InitAsync<T>(Func<Task<(T Value, Exception error)>> initializer) 
        {
            (T val, Exception ex) = await initializer().ConfigureAwait(false);
            if (!EqualityComparer<T>.Default.Equals(val, default(T)))
            {
                return new Maybe<T>(val);
            }

            if (ex != null)
            {
                return new Maybe<T>(ex);
            }
            throw new NullReferenceException("Your initializer failed to deliver a value or exception. At least one of both must be initialized");
        }

        /// <summary>
        /// Helper function to easily create a Maybe with a value
        /// </summary>
        /// <param name="val">The value to store</param>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <returns>Newly created maybe</returns>
        public static Maybe<T> FromVal<T>(T val) 
        {
            return new Maybe<T>(val);
        }
        
        /// <summary>
        /// Helper function to easily create a Maybe that stores an error
        /// </summary>
        /// <param name="e">Exception to store</param>
        /// <typeparam name="T">The type of the Maybe</typeparam>
        /// <returns>Newly created Maybe</returns>
        public static Maybe<T> FromErr<T>(Exception e) 
        {
            return new Maybe<T>(e);
        }
        
        /// <summary>
        /// Helper function to easily create a Maybe that stores an error
        /// </summary>
        /// <param name="error">The error message to be passed to the exception</param>
        /// <typeparam name="T">The type of the Maybe</typeparam>
        /// <returns>Newly created Maybe</returns>
        public static Maybe<T> FromErr<T>(string error) 
        {
            return new Maybe<T>(new Exception(error));
        }

        /// <summary>
        /// Helper function to easily create a Maybe wrapped in a Task with a value.
        /// </summary>
        /// <param name="val">The value to store</param>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <returns>Newly created Maybe wrapped in a Task</returns>
        public static Task<Maybe<T>> FromValTask<T>(T val) 
        {
            return Task.FromResult(new Maybe<T>(val));
        }
        
        /// <summary>
        /// Helper function to easily create a Maybe wrapped in a Task that stores an error
        /// </summary>
        /// <param name="e">Exception to store</param>
        /// <typeparam name="T">The type of the Maybe</typeparam>
        /// <returns>Newly created Maybe in a Task</returns>
        public static Task<Maybe<T>> FromErrTask<T>(Exception e) 
        {
            return Task.FromResult(new Maybe<T>(e));
        }
        
        /// <summary>
        /// Helper function to easily create a Maybe wrapped in a Task that stores an error
        /// </summary>
        /// <param name="err">The error message to be passed to the exception</param>
        /// <typeparam name="T">The type of the Maybe</typeparam>
        /// <returns>Newly created Maybe in a Task</returns>
        public static Task<Maybe<T>> FromErrTask<T>(string err) 
        {
            return Task.FromResult(new Maybe<T>(new Exception(err)));
        }

        /// <summary>
        /// Helper function that returns a Maybe as a ZERO. Carring no value and no exception.
        /// Lightweight null wrapper
        /// </summary>
        /// <param name="hasVal">If it has a value that is just nothing</param>
        /// <param name="hasError">If it has an error that is just null</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Maybe<T> Zero<T>(bool hasVal = false, bool hasError = false)
        {
            return new Maybe<T>(hasVal, hasError);
        }
        
    }
}