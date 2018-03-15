//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: InteractorBase.cs
//---------------------------------------------------------------

namespace CDP.CoreApp
{
    using CDP.AppDomain;
    using CDP.Common;
    using CDP.Common.Logging;
    using System;

    /// <summary>
    /// Defines the base functionality for an interactor object in the applicaiton.
    /// </summary>
    public abstract class InteractorBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILoggingService logger;

        /// <summary>
        /// Builds an Error response for the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public virtual Result<T> Error<T>(T value, Exception ex)
        {
            logger.LogException(ex);
            var result = new Result<T>(value, new ResultError(ex));
            return result;
        }

        /// <summary>
        /// Builds an Error response for the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public virtual Result<T> Error<T>(T value, string errorMessage)
        {
            logger.LogException(new Exception(errorMessage));
            var result = new Result<T>(value, new ResultError(errorMessage));
            return result;
        }

        /// <summary>
        /// Builds a Success response for the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public virtual Result<T> Success<T>(T value, string method)
        {
            logger.LogTrace($"The call to {method} returned successfully.");
            return new Result<T>(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractorBase"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public InteractorBase(ILoggingService logger)
        {
            Validate.IsNotNull(logger, nameof(logger));
            this.logger = logger;
        }
    }
}
