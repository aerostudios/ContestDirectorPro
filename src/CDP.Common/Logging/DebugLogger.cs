//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: DebugLogger.cs
//---------------------------------------------------------------

namespace CDP.Common.Logging
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Basic debugging logger.  Writes output to the dev console.
    /// </summary>
    /// <seealso cref="CDP.Common.Logging.ILoggingService" />
    public class DebugLogger : ILoggingService
    {
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public void LogException(Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}.");
        }

        /// <summary>
        /// Logs a trace.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogTrace(string message)
        {
            Debug.WriteLine($"Trace: {message}.");
        }
    }
}
