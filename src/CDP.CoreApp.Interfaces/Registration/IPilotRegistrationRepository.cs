//---------------------------------------------------------------
// Date: 12/9/2017
// Rights: 
// FileName: IPilotRegistrationRepository.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Registration
{
    using CDP.AppDomain;
    using CDP.AppDomain.Registration;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a data storage interface for Pilot Registrations.
    /// </summary>
    public interface IPilotRegistrationRepository
    {
        /// <summary>
        /// Registers the pilots for contest.
        /// </summary>
        /// <param name="pilotsToRegister">The pilots.</param>
        /// <returns></returns>
        Task<Result<IEnumerable<PilotRegistration>>> CreateAsync(IEnumerable<PilotRegistration> pilotsToRegister);

        /// <summary>
        /// Registers the single pilot for a contest.
        /// </summary>
        /// <param name="pilotToRegister">The pilot reg.</param>
        /// <returns></returns>
        Task<Result<PilotRegistration>> CreateAsync(PilotRegistration pilotToRegister);

        /// <summary>
        /// Uns the register pilots for a contest.
        /// </summary>
        /// <param name="pilotIds">The pilot ids.</param>
        /// <returns></returns>
        Task<Result<bool>> DeleteAsync(IEnumerable<PilotRegistration> pilotsToUnregister);

        /// <summary>
        /// Uns the register pilot for a contest.
        /// </summary>
        /// <param name="id">The pilot to unregister.</param>
        /// <returns></returns>
        Task<Result<bool>> DeleteAsync(string id);

        /// <summary>
        /// Gets the pilot registrations for a contest.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        Task<Result<IEnumerable<PilotRegistration>>> ReadAsync(string contestId);
    }
}
