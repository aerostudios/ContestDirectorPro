//---------------------------------------------------------------
// Date: 2/19/2017
// Rights: 
// FileName: ScoringContestScoreAggInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Scoring.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.Scoring;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Scoring;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Handles scoring contests all up.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public class ScoringContestScoreAggInteractor : InteractorBase
    {
        /// <summary>
        /// The contest score aggregator
        /// </summary>
        private readonly IContestScoreAggregator contestScoreAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringContestScoreAggInteractor"/> class.
        /// </summary>
        /// <param name="contestScoreAggregator">The contest score aggregator.</param>
        /// <param name="logger">The logger.</param>
        public ScoringContestScoreAggInteractor(IContestScoreAggregator contestScoreAggregator, ILoggingService logger) : base(logger)
        {
            this.contestScoreAggregator = contestScoreAggregator ?? throw new ArgumentNullException($"{nameof(contestScoreAggregator)} cannot be null.");
        }

        /// <summary>
        /// Scores a group of rounds.
        /// </summary>
        /// <param name="allRoundScores">All contest scores.</param>
        /// <returns>
        /// A collection with the PilotId as the key and a collection of scores for each round as the value.
        /// In the collection of scores, the round number is the key and the score for that round is value.
        /// The last entry (int.MaxValue) in the scores collection is the total for the set of rounds passed in.
        /// </returns>
        public Result<Dictionary<string, PilotContestScoreCollection>> GetAggregateRoundScoresForPilots(ContestScoresCollection allRoundScores)
        {
            if (allRoundScores == null || allRoundScores.Count() < 1)
            {
                return Error<Dictionary<string, PilotContestScoreCollection>>(null, $"{nameof(allRoundScores)} cannot be null or empty");
            }

            var result = contestScoreAggregator.GenerateContestScores(allRoundScores);

            return Success(result, nameof(GetAggregateRoundScoresForPilots));
        }
    }
}
