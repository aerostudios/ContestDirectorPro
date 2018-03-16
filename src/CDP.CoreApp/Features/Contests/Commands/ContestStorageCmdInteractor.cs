//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: ContestStorageCmdInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Contests.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.Contests;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Contests;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles contest commands for the application
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class ContestStorageCmdInteractor : InteractorBase
    {
        /// <summary>
        /// The contest repository
        /// </summary>
        private readonly IContestRepository contestRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestStorageCmdInteractor"/> class.
        /// </summary>
        /// <param name="contestRepository">The contest repository.</param>
        /// <param name="logger">The logger.</param>
        public ContestStorageCmdInteractor(IContestRepository contestRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(contestRepository, nameof(contestRepository));
            this.contestRepository = contestRepository;
        }

        /// <summary>
        /// Creates the contest.
        /// </summary>
        /// <param name="contestToCreate">The contest to create.</param>
        /// <returns></returns>
        public async Task<Result<Contest>> CreateContestAsync(Contest contestToCreate)
        {
            if (!ValidateContest(contestToCreate))
            {
                return Error<Contest>(null, $"The {nameof(contestToCreate)} is not a valid contest.");
            }

            try
            {
                var result = await this.contestRepository.CreateAsync(contestToCreate);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<Contest>(null, result.Error.Exception);
                }

                return Success(result.Value, nameof(CreateContestAsync));
            }
            catch (Exception ex)
            {
                return Error<Contest>(null, ex);
            }
        }

        /// <summary>
        /// Deletes the contest.
        /// </summary>
        /// <param name="contestToDelete">The contest to delete.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteContestAsync(Contest contestToDelete)
        {
            if (!ValidateContest(contestToDelete))
            {
                return Error<bool>(false, $"The {nameof(contestToDelete)} is not a valid contest.");
            }

            try
            {
                var result = await this.contestRepository.DeleteAsync(contestToDelete.Id);

                if (result.IsFaulted || !result.Value)
                {
                    return Error<bool>(false, "An error occured while creating a contest.");
                }

                return Success(result.Value, nameof(DeleteContestAsync));
            }
            catch (Exception ex)
            {
                return Error<bool>(false, ex);
            }
        }

        /// <summary>
        /// Updates the contest.
        /// </summary>
        /// <param name="contestToUpdate">The contest to update.</param>
        /// <returns></returns>
        public async Task<Result<Contest>> UpdateContestAsync(Contest contestToUpdate) 
        {
            if (!ValidateContest(contestToUpdate))
            {
                return Error<Contest>(null, $"The {nameof(contestToUpdate)} is not a valid contest.");
            }

            try
            {
                var result = await this.contestRepository.UpdateAsync(contestToUpdate);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<Contest>(null, "An error occured while creating a contest.");
                }

                return Success(result.Value, nameof(UpdateContestAsync));
            }
            catch (Exception ex)
            {
                return Error<Contest>(null, ex);
            }
        }

        /// <summary>
        /// Validates the contest.
        /// </summary>
        /// <param name="contestToCreate">The contest to create.</param>
        /// <returns></returns>
        private bool ValidateContest(Contest contestToCreate)
        {
            Validate.IsNotNull(contestToCreate, nameof(contestToCreate));

            if (string.IsNullOrEmpty(contestToCreate.Name))
            {
                return false;
            }

            if (contestToCreate.NumberOfFlyoffRounds < 0)
            {
                return false;
            }

            if (contestToCreate.NumberOfPilots < 0)
            {
                return false;
            }

            if (contestToCreate.Rounds == null ||
                contestToCreate.Rounds?.Count < 0)
            {
                return false;
            }

            if (contestToCreate.StartDate > contestToCreate.EndDate)
            {
                return false;
            }

            if (contestToCreate.Type == ContestType.Unknown)
            {
                return false;
            }

            if (contestToCreate.PilotRoster == null ||
                contestToCreate.PilotRoster.Count < 0)
            {
                return false;
            }
            
         return true;
        }
    }
}
