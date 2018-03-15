//---------------------------------------------------------------
// Date: 12/18/2017
// Rights: 
// FileName: GeneralScoringException.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines an exception that is thrown with scoring.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class GeneralScoringException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralScoringException"/> class.
        /// </summary>
        public GeneralScoringException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralScoringException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GeneralScoringException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralScoringException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public GeneralScoringException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralScoringException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected GeneralScoringException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
