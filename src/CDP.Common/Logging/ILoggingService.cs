//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: ILoggingService.cs
//---------------------------------------------------------------

namespace CDP.Common.Logging
{
    using System;

    /// <summary>
    /// Defines a logger for an application
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        void LogException(Exception ex);

        /// <summary>
        /// Logs a trace.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogTrace(string message);
    }
}
