//---------------------------------------------------------------
// Date: 12/12/2017
// Rights: 
// FileName: ScoringCommandInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp
{
    /// <summary>
    /// Defines a validation result
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; set; } = false;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="message">The message.</param>
        public ValidationResult(bool isValid, string message = null)
        {
            this.IsValid = isValid;
            this.Message = message;
        }
    }
}
