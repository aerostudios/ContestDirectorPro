//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: ScoringStorageCmdInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Scoring.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Scoring;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Scoring;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles scoring commands
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class ScoringStorageCmdInteractor : InteractorBase
    {
        /// <summary>
        /// The scoring repository
        /// </summary>
        private readonly IScoringRepository scoringRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringStorageCmdInteractor"/> class.
        /// </summary>
        /// <param name="scoringRepository">The scoring repository.</param>
        /// <param name="logger">The logger.</param>
        public ScoringStorageCmdInteractor(IScoringRepository scoringRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(scoringRepository, nameof(scoringRepository));
            this.scoringRepository = scoringRepository;
        }

        /// <summary>
        /// Saves round scores for a contest.
        /// </summary>
        /// <param name="pilotRoundScores">The pilot round scores.</param>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<TimeSheet>>> SaveRoundScoresAsync(IEnumerable<TimeSheet> pilotRoundScores, string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<IEnumerable<TimeSheet>>(null, $"{nameof(contestId)} cannot be null or empty");
            }

            var validationResult = ValidateRoundScoreData(pilotRoundScores);

            if (!validationResult.IsValid)
            {
                return Error<IEnumerable<TimeSheet>>(null, validationResult.Message);
            }

            try
            {
                var result = await this.scoringRepository.CreateAsync(pilotRoundScores);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<IEnumerable<TimeSheet>>(null, $"An error occured while saving the scores for contest id {contestId}.");
                }

                return Success(result.Value, nameof(SaveRoundScoresAsync));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<TimeSheet>>(null, ex);
            }
        }

        /// <summary>
        /// Updates a pilot's score.
        /// </summary>
        /// <param name="pilotRoundScore">The pilot round score.</param>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        public async Task<Result<TimeSheet>> UpdatePilotsScoreAsync(TimeSheet pilotRoundScore, string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<TimeSheet>(null, $"{nameof(contestId)} cannot be null or empty");
            }

            var validationResult = ValidateSingleRoundScore(pilotRoundScore);

            if (!validationResult.IsValid)
            {
                return Error<TimeSheet>(null, validationResult.Message);
            }

            try
            {
                var result = await this.scoringRepository.UpdateAsync(pilotRoundScore);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<TimeSheet>(null, $"An error occured while updating the score for pilot id {pilotRoundScore.PilotId}.");
                }

                return Success(result.Value, nameof(UpdatePilotsScoreAsync));
            }
            catch (Exception ex)
            {
                return Error<TimeSheet>(null, ex);
            }
        }

        /// <summary>
        /// Deletes a pilot's score.
        /// </summary>
        /// <param name="timeSheet">The pilot round score.</param>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeletePilotScoreAsync(TimeSheet timeSheet, string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error(false, $"{nameof(contestId)} cannot be null or empty");
            }

            var validationResult = ValidateSingleRoundScore(timeSheet);

            if (!validationResult.IsValid)
            {
                return Error(false, validationResult.Message);
            }

            try
            {
                var result = await this.scoringRepository.DeleteAsync(timeSheet);

                if (result.IsFaulted || !result.Value)
                {
                    return Error(false, $"An error occured while deleting the score for pilot id {timeSheet.PilotId}.");
                }

                return Success(result.Value, nameof(DeletePilotScoreAsync));
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }
        }

        /// <summary>
        /// Validates the round score data.
        /// </summary>
        /// <param name="timeSheets">The pilot round scores.</param>
        /// <returns></returns>
        private ValidationResult ValidateRoundScoreData(IEnumerable<TimeSheet> timeSheets)
        {
            // Validate the set as a whole 
            if (timeSheets == null)
            {
                return new ValidationResult(false, $"{nameof(timeSheets)} cannont be null");
            }

            if (timeSheets.Count() < 1)
            {
                return new ValidationResult(false, $"{nameof(timeSheets)} does not have any scores");
            }

            // Loop through all of the individual scores and validate them.
            // Grab the first and validate the rest against it.

            var firstTimeSheet = timeSheets.First();
            var cntr = 0;
            foreach (var prs in timeSheets)
            {
                ValidationResult validationResult = null;

                if (prs.FlightGroup != firstTimeSheet.FlightGroup)
                {
                    validationResult = new ValidationResult(false, $"Pilot id {prs.PilotId} is not in the correct flight group.");
                }

                if (prs.RoundOrdinal != firstTimeSheet.RoundOrdinal)
                {
                    validationResult = new ValidationResult(false, $"Pilot id {prs.PilotId} is not in the correct round.");
                }

                if (prs.TaskId != firstTimeSheet.TaskId)
                {
                    validationResult = new ValidationResult(false, $"Pilot id {prs.PilotId} is not scoring agaist the proper task.");
                }

                if (prs.ContestId != firstTimeSheet.ContestId)
                {
                    validationResult = new ValidationResult(false, $"Pilot id {prs.PilotId} is not scoring against the proper contest.");
                }

                // TODO: This is odd logic, take a look at it again.

                if (validationResult != null)
                {
                    validationResult.Message += $" Position: {cntr}.";
                    return validationResult;
                }

                validationResult = ValidateSingleRoundScore(prs);

                if (!validationResult.IsValid)
                {
                    validationResult.Message += $" Position: {cntr}.";
                    return validationResult;
                }
            }

            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates the single round score.
        /// </summary>
        /// <param name="timeSheet">The pilot round score.</param>
        /// <returns></returns>
        private ValidationResult ValidateSingleRoundScore(TimeSheet timeSheet)
        {
            if (timeSheet == null)
            {
                return new ValidationResult(false, "Timesheet cannot be null.");
            }

            if (string.IsNullOrEmpty(timeSheet.ContestId))
            {
                return new ValidationResult(false, "ContestId is null or empty.");
            }

            if (timeSheet.RoundOrdinal < 0)
            {
                return new ValidationResult(false, "RoundOrdinal is out of range.");
            }

            if (timeSheet.FlightGroup == FlightGroup.Unknown)
            {
                return new ValidationResult(false, "FlightGroup is unknown.");
            }

            if (timeSheet.TimeGates == null)
            {
                return new ValidationResult(false, "RoundTimes is null.");
            }

            if (timeSheet.TimeGates.Count < 1)
            {
                return new ValidationResult(false, "RoundTimes is out of range or empty.");
            }

            if (string.IsNullOrEmpty(timeSheet.PilotId))
            {
                return new ValidationResult(false, "PilotId is null or empty.");
            }

            if (string.IsNullOrEmpty(timeSheet.TaskId))
            {
                return new ValidationResult(false, "The TaskId is null or empty .");
            }

            return new ValidationResult(true);
        }
    }
}
