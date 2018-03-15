//---------------------------------------------------------------
// Date: 12/9/2017
// Rights: 
// FileName: PilotRegistrationQueryInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Registration.Queries
{
    using CDP.AppDomain;
    using CDP.AppDomain.Registration;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Registration;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles requests for contest registrations.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class PilotRegistrationQueryInteractor : InteractorBase
    {
        /// <summary>
        /// The pilot repository
        /// </summary>
        private readonly IPilotRegistrationRepository pilotRegistrationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotRegistrationQueryInteractor"/> class.
        /// </summary>
        /// <param name="pilotRegistrationRepository">The pilot registration repository.</param>
        /// <param name="logger">The logger.</param>
        public PilotRegistrationQueryInteractor(IPilotRegistrationRepository pilotRegistrationRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(pilotRegistrationRepository, nameof(pilotRegistrationRepository));
            this.pilotRegistrationRepository = pilotRegistrationRepository;
        }

        /// <summary>
        /// Gets the pilot registrations for contest.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<PilotRegistration>>> GetPilotRegistrationsForContest(string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {             
                return Error<IEnumerable<PilotRegistration>>(null, "Contest is not valid");
            }

            try
            {
                var result = await this.pilotRegistrationRepository.ReadAsync(contestId);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<IEnumerable<PilotRegistration>>(null, "No registrations were found for this contest.");
                }

                return Success(result.Value, nameof(GetPilotRegistrationsForContest));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<PilotRegistration>>(null, ex);
            }
        }
    }
}
