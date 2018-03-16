//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: Result.cs
//---------------------------------------------------------------

namespace CDP.AppDomain
{
    /// <summary>
    /// Define an outcome of an operation in our core app.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Result<T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is faulted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is faulted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFaulted { get { return this.Error != null; } }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public ResultError Error { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Result(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="resultError">The result error.</param>
        public Result(T value, ResultError resultError) : this(value)
        {
            this.Error = resultError;
        }
    }
}
