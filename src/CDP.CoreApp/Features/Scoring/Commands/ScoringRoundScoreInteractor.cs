//---------------------------------------------------------------
// Date: 2/19/2017
// Rights: 
// FileName: ScoringRoundScoreInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Scoring.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Scoring.Exceptions;
    using CDP.AppDomain.Tasks;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Scoring;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Handles the scoring of rounds in a contest.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public class ScoringRoundScoreInteractor : InteractorBase
    {
        private readonly IRoundScoringAlgo roundScoringAlgo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringRoundScoreInteractor" /> class.
        /// </summary>
        /// <param name="roundScoringAlgo">The round scoring algo.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">roundScoringAlgo</exception>
        public ScoringRoundScoreInteractor(IRoundScoringAlgo roundScoringAlgo, ILoggingService logger) : base(logger)
        {
            this.roundScoringAlgo = roundScoringAlgo ?? throw new ArgumentNullException($"{nameof(roundScoringAlgo)} cannot be null.");
        }

        /// <summary>
        /// Scores the round.
        /// </summary>
        /// <param name="roundScores">The pilot round scores.</param>
        /// <param name="task">The task.</param>
        /// <param name="roundScoringAlgo">The round scoring algo.</param>
        /// <returns>True if the scoring action completed successfully, false if it didn't.</returns>
        public Result<bool> ScoreRound(IEnumerable<TimeSheet> roundScores, TaskBase task)
        {
            if (roundScores == null)
            {
                return Error(false, $"{nameof(roundScores)} cannot be null");
            }

            if (roundScoringAlgo == null)
            {
                return Error(false, $"{nameof(roundScoringAlgo)} cannot be null.");
            }

            if (task == null)
            {
                return Error(false, $"{nameof(task)} cannot be null.");
            }

            try
            {
                // Let the scoring algo that was passed in do it's thing.
                roundScoringAlgo.ScoreRound(roundScores, task);
                roundScores = roundScores.OrderByDescending(rs => rs.Score);

                // Make sure the output is 'valid'.
                if (roundScores == null || roundScores.Count() < 1)
                {
                    return Error(false, "An error occured while scoring a timesheet.");
                }

                return Success(true, nameof(ScoreRound));
            }
            catch (InvalidRoundScoreException irsEx)
            {
                return Error(false, irsEx);
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }
        }
    }
}
