//---------------------------------------------------------------
// Date: 12/15/2017
// Rights: 
// FileName: InvalidRoundScoreException.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when an invalid Round score is found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidRoundScoreException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRoundScoreException"/> class.
        /// </summary>
        public InvalidRoundScoreException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRoundScoreException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidRoundScoreException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRoundScoreException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidRoundScoreException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRoundScoreException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected InvalidRoundScoreException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
