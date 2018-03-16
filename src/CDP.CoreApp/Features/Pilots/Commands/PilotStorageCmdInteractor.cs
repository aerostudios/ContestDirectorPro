//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: PilotCmdInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Pilots.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.Pilots;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Pilots;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles all pilot commands for the application
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class PilotStorageCmdInteractor : InteractorBase
    {
        /// <summary>
        /// The pilot repository
        /// </summary>
        private readonly IPilotRepository pilotRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotStorageCmdInteractor"/> class.
        /// </summary>
        /// <param name="pilotRepository">The pilot repository.</param>
        /// <param name="logger">The logger.</param>
        public PilotStorageCmdInteractor(IPilotRepository pilotRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(pilotRepository, nameof(pilotRepository));
            this.pilotRepository = pilotRepository;
        }

        /// <summary>
        /// Creates a new pilot.
        /// </summary>
        /// <param name="pilotToCreate">The pilot to create.</param>
        /// <returns></returns>
        public async Task<Result<Pilot>> CreateNewPilotAsync(Pilot pilotToCreate)
        {
            var validationResult = ValidatePilot(pilotToCreate);

            if (!validationResult.IsValid)
            {
                return Error<Pilot>(null, validationResult.Message);
            }

            try
            {
                var result = await this.pilotRepository.CreateAsync(pilotToCreate);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<Pilot>(null, $"An error occured while creating a pilot.");
                }

                return Success(result.Value, nameof(CreateNewPilotAsync));
            }
            catch (Exception ex)
            {
                return Error<Pilot>(null, ex);
            }
        }

        /// <summary>
        /// Updates a pilot.
        /// </summary>
        /// <param name="pilotToUpdate">The pilot to update.</param>
        /// <returns></returns>
        public async Task<Result<Pilot>> UpdatePilotAsync(Pilot pilotToUpdate)
        {
            var validationResult = ValidatePilot(pilotToUpdate);

            if (!validationResult.IsValid)
            {
                return Error<Pilot>(null, validationResult.Message);
            }

            try
            {
                var result = await this.pilotRepository.UpdateAsync(pilotToUpdate);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<Pilot>(null, $"An error occured while creating a pilot.");
                }

                return Success(result.Value, nameof(UpdatePilotAsync));
            }
            catch (Exception ex)
            {
                return Error<Pilot>(null, ex);
            }
        }

        /// <summary>
        /// Deletes a pilot.
        /// </summary>
        /// <param name="pilotToDelete">The pilot to delete.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeletePilotAsync(Pilot pilotToDelete)
        {
            var validationResult = ValidatePilot(pilotToDelete);

            if (!validationResult.IsValid)
            {
                return Error<bool>(false, validationResult.Message);
            }

            try
            {
                var result = await this.pilotRepository.DeleteAsync(pilotToDelete.Id);

                if (result.IsFaulted || !result.Value)
                {
                    return Error<bool>(false, $"An error occured while creating a pilot.");
                }

                return Success(result.Value, nameof(DeletePilotAsync));
            }
            catch (Exception ex)
            {
                return Error<bool>(false, ex);
            }
        }

        /// <summary>
        /// Validates a pilot.
        /// </summary>
        /// <param name="pilot">The pilot.</param>
        /// <returns></returns>
        private ValidationResult ValidatePilot(Pilot pilot)
        {
            if (pilot == null)
            {
                return new ValidationResult(false, "The pilot cannot be null.");
            }

            if (string.IsNullOrEmpty(pilot.FirstName))
            {
                return new ValidationResult(false, "The pilot first name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(pilot.LastName))
            {
                return new ValidationResult(false, "The pilot last name cannot be null or empty.");
            }

            return new ValidationResult(true);
        }
    }
}