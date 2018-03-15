//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: ScoringQueryInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Scoring.Queries
{
    using CDP.AppDomain;
    using CDP.AppDomain.Scoring;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Scoring;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles scoring queries
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class ScoringQueryInteractor : InteractorBase
    {
        /// <summary>
        /// The scoring repository
        /// </summary>
        private readonly IScoringRepository scoringRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringQueryInteractor"/> class.
        /// </summary>
        /// <param name="scoringRepository">The scoring repository.</param>
        /// <param name="logging">The logging.</param>
        public ScoringQueryInteractor(IScoringRepository scoringRepository, ILoggingService logging) : base(logging)
        {
            Validate.IsNotNull(scoringRepository, nameof(scoringRepository));
            this.scoringRepository = scoringRepository;
        }

        /// <summary>
        /// Gets the scores for contest.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        public async Task<Result<ContestScoresCollection>> GetScoresForContest(string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<ContestScoresCollection>(null, $"{nameof(contestId)} cannot be null or empty");
            }

            try
            {
                var result = await this.scoringRepository.ReadAsync(contestId);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<ContestScoresCollection>(null, $"An error occured while saving the scores.");
                }

                return Success(result.Value, nameof(GetScoresForContest));
            }
            catch (Exception ex)
            {
                return Error<ContestScoresCollection>(null, ex);
            }
        }

        /// <summary>
        /// Gets the scores for round.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="roundId">The round identifier.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<TimeSheet>>> GetScoresForRound(string contestId, int roundOrdinal)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<IEnumerable<TimeSheet>>(null, $"{nameof(roundOrdinal)} cannot be null or empty");
            }

            if (roundOrdinal < 0)
            {
                return Error<IEnumerable<TimeSheet>>(null, $"{nameof(roundOrdinal)} cannot be null or empty");
            }

            try
            {
                var result = await this.scoringRepository.ReadAsync(contestId, roundOrdinal);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<IEnumerable<TimeSheet>>(null, $"An error occured while getting the scores.");
                }

                return Success(result.Value, nameof(GetScoresForRound));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<TimeSheet>>(null, ex);
            }
        }

        /// <summary>
        /// Gets the score for single pilot.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="roundOrdinal">The round ordinal.</param>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <returns></returns>
        public async Task<Result<TimeSheet>> GetScoreForSinglePilot(string contestId, int roundOrdinal, string pilotId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<TimeSheet>(null, $"{nameof(contestId)} cannot be null or empty.");
            }

            if (roundOrdinal < 0)
            {
                return Error<TimeSheet>(null, $"{nameof(roundOrdinal)} cannot be less than zero.");
            }

            if (string.IsNullOrEmpty(pilotId))
            {
                return Error<TimeSheet>(null, $"{nameof(pilotId)} cannot be null or empty.");
            }

            try
            {
                var result = await this.scoringRepository.ReadAsync(contestId, roundOrdinal, pilotId);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<TimeSheet>(null, $"An error occured while getting the score.");
                }

                return Success(result.Value, nameof(GetScoreForSinglePilot));
            }
            catch (Exception ex)
            {
                return Error<TimeSheet>(null, ex);
            }
        }
    }
}