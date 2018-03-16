//---------------------------------------------------------------
// Date: 12/9/2017
// Rights: 
// FileName: PilotRegistrationCmdInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Registration.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.Registration;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Registration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles all of the pilot registration commands
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public class PilotRegistrationStorageCmdInteractor : InteractorBase
    {
        /// <summary>
        /// The pilot registration repository
        /// </summary>
        private readonly IPilotRegistrationRepository pilotRegistrationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotRegistrationStorageCmdInteractor"/> class.
        /// </summary>
        /// <param name="pilotRegistrationRepository">The pilot registration repository.</param>
        /// <param name="logger">The logger.</param>
        public PilotRegistrationStorageCmdInteractor(IPilotRegistrationRepository pilotRegistrationRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(pilotRegistrationRepository, nameof(pilotRegistrationRepository));
            this.pilotRegistrationRepository = pilotRegistrationRepository;
        }

        /// <summary>
        /// Registers the pilots for contest asynchronous.
        /// </summary>
        /// <param name="pilotsToRegister">The pilots to register.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<PilotRegistration>>> RegisterPilotsForContestAsync(IEnumerable<PilotRegistration> pilotsToRegister)
        {
            var validationResult = ValidatePilotRegistrations(pilotsToRegister);

            if (!validationResult.IsValid)
            {
                return Error<IEnumerable<PilotRegistration>>(null, validationResult.Message);
            }

            try
            {
                var result = await this.pilotRegistrationRepository.CreateAsync(pilotsToRegister);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<IEnumerable<PilotRegistration>>(null, $"An error occured while registering pilots.");
                }

                return Success(result.Value, nameof(RegisterPilotsForContestAsync));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<PilotRegistration>>(null, ex);
            }
        }

        /// <summary>
        /// Registers the single pilot for contest.
        /// </summary>
        /// <param name="pilotToRegister">The pilot registration.</param>
        /// <returns></returns>
        public async Task<Result<PilotRegistration>> RegisterSinglePilotForContestAsync(PilotRegistration pilotToRegister)
        {
            var validationResult = ValidatePilotRegistration(pilotToRegister);

            if (!validationResult.IsValid)
            {
                return Error<PilotRegistration>(null, validationResult.Message);
            }

            try
            {
                var result = await this.pilotRegistrationRepository.CreateAsync(pilotToRegister);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<PilotRegistration>(null, $"An error occured while registering pilots.");
                }

                return Success(result.Value, nameof(RegisterPilotsForContestAsync));
            }
            catch (Exception ex)
            {
                return Error<PilotRegistration>(null, ex);
            }
        }

        /// <summary>
        /// Unregisters a set of pilots for contest.
        /// </summary>
        /// <param name="pilotsToUnregister">The pilots to unregister.</param>
        /// <returns></returns>
        public async Task<Result<bool>> UnRegisterPilotsForContestAsync(IEnumerable<PilotRegistration> pilotsToUnregister)
        {
            var validationResult = ValidatePilotRegistrations(pilotsToUnregister);

            if (!validationResult.IsValid)
            {
                return Error<bool>(false, validationResult.Message);
            }

            try
            {
                var result = await this.pilotRegistrationRepository.DeleteAsync(pilotsToUnregister);

                if (result.IsFaulted || !result.Value)
                {
                    return Error(false, $"An error occured while unregistering pilots.");
                }

                return Success(result.Value, nameof(RegisterPilotsForContestAsync));
            }
            catch (Exception ex)
            {
                return Error<bool>(false, ex);
            }

        }

        /// <summary>
        /// Uns the register pilot for contest.
        /// </summary>
        /// <param name="pilotToUnregister">The pilot to unregister.</param>
        /// <returns></returns>
        public async Task<Result<bool>> UnRegisterPilotForContestAsync(PilotRegistration pilotToUnregister)
        {
            var validationResult = ValidatePilotRegistration(pilotToUnregister, true);

            if (!validationResult.IsValid)
            {
                return Error<bool>(false, $"The {nameof(pilotToUnregister)} parameter is not valid.");
            }

            try
            {
                var result = await this.pilotRegistrationRepository.DeleteAsync(pilotToUnregister.Id);

                if (result.IsFaulted || !result.Value)
                {
                    return Error<bool>(false, $"An error occured while unregistering a pilot.");
                }

                return Success(result.Value, nameof(RegisterPilotsForContestAsync));
            }
            catch (Exception ex)
            {
                return Error<bool>(false, ex);
            }
        }

        /// <summary>
        /// Validates the pilot registration list.
        /// </summary>
        /// <param name="pilots">The pilots.</param>
        /// <returns></returns>
        private static ValidationResult ValidatePilotRegistrations(IEnumerable<PilotRegistration> pilots)
        {
            if (pilots == null)
            {
                return new ValidationResult(false, "The PilotRegistration enumeration cannot be null.");
            }

            if (pilots.Count() < 1)
            {
                return new ValidationResult(false, "The PilotRegistration enumeration is out of Range.");
            }

            int position = 0;
            foreach(var registration in pilots)
            {
                var validationResult = ValidatePilotRegistration(registration);

                if (!validationResult.IsValid)
                {
                    validationResult.Message += $" Position: {position}.";
                    return validationResult;
                }

                ++position;
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates the pilot registration.
        /// </summary>
        /// <param name="pilotRegistration">The pilot registration.</param>
        /// <returns></returns>
        private static ValidationResult ValidatePilotRegistration(PilotRegistration pilotRegistration, bool isUnregister = false)
        {
            if (pilotRegistration == null)
            {
                return new ValidationResult(false, "Pilot registration cannot be null.");
            }

            if (string.IsNullOrEmpty(pilotRegistration.PilotId))
            {
                return new ValidationResult(false, "Pilot Id cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(pilotRegistration.ContestId))
            {
                return new ValidationResult(false, "Contest Id cannot be null or empty.");
            }

            return new ValidationResult(true);
        }
    }
}
