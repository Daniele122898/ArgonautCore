using System;
using System.Text;

namespace ArgonautCore.Lw
{
    // Inspired / Copied from https://github.com/sn0w bcs it's better than what i could write by myself kekw

    /// <summary>
    /// Custom error type for use in <see cref="Result{TVal,TErr}"/>. It's generally more light weight as it supports
    /// just error messages instead of full stack traces in <see cref="Maybe"/>.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Error message, usually more humanly readable than the actual exception and stacktrace. 
        /// </summary>
        public Some<string> Message { get; private set; }
        
        /// <summary>
        /// Equivalent to Inner exception. Is built up in a recursive manner.
        /// </summary>
        public Option<Error> Cause { get; private set; }
        
        /// <summary>
        /// Stacktrace if available.
        /// </summary>
        public Option<string> Trace { get; private set; }

        /// <summary>
        /// Construct a custom Error type with mainly just a message. If you rly wish to add a cause and trace creating
        /// an error type from an exception is probably easier
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        /// <param name="trace"></param>
        public Error(string message, Option<Error> cause = default, Option<string> trace = default)
        {
            this.Message = message;
            this.Cause = cause;
            this.Trace = trace;
        }

        /// <summary>
        /// Construct a custom Error type from a <see cref="Exception"/>. This will include the stacktrace if
        /// given and if it has inner exceptions it will include those in a recursive manner.
        /// </summary>
        /// <param name="e"></param>
        public Error(Some<Exception> e)
        {
            var ex = ~e;
            this.Message = ex.Message;
            this.Cause = ex.InnerException != null
                ? Option.Some<Error>(new Error(ex.InnerException))
                : Option.None<Error>();
            this.Trace = ex.StackTrace != null ? Option.Some(ex.StackTrace) : Option.None<string>();
        }

        /// <summary>
        /// Pretty print the Error function with recursion in the Inner exception
        /// </summary>
        /// <returns>Formatted error string</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(this.Message.Get());
            this.Cause.MatchSome(err => sb.AppendLine(err.ToString()));
            this.Trace.MatchSome(trace => sb.AppendLine(trace));
            return sb.ToString();
        }
    }
}