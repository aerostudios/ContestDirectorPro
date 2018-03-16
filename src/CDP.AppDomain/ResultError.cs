//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: ResultError.cs
//---------------------------------------------------------------

namespace CDP.AppDomain
{
    using System;

    /// <summary>
    /// Defines an error outcome in our core app.
    /// </summary>
    public sealed class ResultError
    {
        /// <summary>
        /// The exception
        /// </summary>
        private readonly Exception exception;

        /// <summary>
        /// The error message
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage
        {
            get
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (exception != null)
                    {
                        return exception.Message;
                    }
                }

                return errorMessage;
            }

            set { errorMessage = value; }
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get { return exception; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultError"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ResultError(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultError"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public ResultError(Exception ex)
        {
            this.exception = ex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultError"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="ex">The ex.</param>
        public ResultError(string errorMessage, Exception ex) : this(ex)
        {
            this.ErrorMessage = errorMessage;
        }
    }
}