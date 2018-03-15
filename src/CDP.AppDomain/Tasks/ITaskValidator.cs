//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: ITaskValidator.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks
{
    using CDP.AppDomain.Scoring;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a TaskCalidator
    /// </summary>
    public interface ITaskValidator
    {
        /// <summary>
        /// Validates the specified gates.
        /// </summary>
        /// <param name="contestTaskToValidate">The contest task to validate.</param>
        /// <returns></returns>
        bool ValidateTask(RoundScoreBase contestTaskToValidate);
    }
}
