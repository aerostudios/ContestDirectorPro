//---------------------------------------------------------------
// Date: 12/15/2017
// Rights: 
// FileName: F3KRoundScoringAlgo.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Scoring
{
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Scoring.Exceptions;
    using CDP.AppDomain.Tasks;
    using CDP.CoreApp.Interfaces.Scoring;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the round scoring method for F3K contests.
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Scoring.IRoundScoringAlgo" />
    public sealed class RoundScoringAlgo : IRoundScoringAlgo
    {
        /// <summary>
        /// Scores the round.
        /// </summary>
        /// <param name="roundScores">The round scores.</param>
        /// <param name="task">The task.</param>
        /// <exception cref="ArgumentNullException">
        /// roundScores
        /// or
        /// task
        /// </exception>
        /// <exception cref="CDP.AppDomain.Scoring.InvalidRoundScoreException"></exception>
        /// <exception cref="CDP.AppDomain.Scoring.GeneralScoringException"></exception>
        public void ScoreRound(IEnumerable<RoundScoreBase> roundScores, TaskBase task)
        {
            if (roundScores == null || roundScores.Count() < 1)
            {
                throw new ArgumentNullException($"{nameof(roundScores)} cannot be null or empty.");
            }

            if (task == null)
            {
                throw new ArgumentNullException($"{nameof(task)} cannot be null.");
            }

            // Validate the roundScores based on overall F3K rules.
            try
            {
                ValidateRoundScoresByF3KRules(roundScores);
            }
            catch (InvalidRoundScoreException irsEx)
            {
                throw irsEx;
            }

            // Time to score the round.
            try
            {
                var cnt = 0;
                // Validate each of the pilots' score entries against the rules of the task flown.
                foreach (var roundScore in roundScores)
                {
                    if (!task.ValidateTask(roundScore))
                    {
                        throw new InvalidRoundScoreException($"Round score for pilot id {roundScore?.PilotId} is invalid for the {task.Name} task.  Position: {cnt}");
                    }

                    ++cnt;
                }

                // Total up each of the pilots' time sheets
                roundScores = roundScores.Select(rs => { rs.Score = task.ScoreTask(rs); return rs; }).ToList();

                // Sort the scores, highest to lowest
                roundScores = roundScores.OrderByDescending(ts => ts.Score);
                var hightestPreNormalizedScoreInRound = 0.0;

                cnt = 0;
                // Apply the normalized value to the score
                foreach (var roundScore in roundScores)
                {
                    // First place gets the granny, use that time to apply scores to the rest
                    if (cnt == 0)
                    {
                        if(roundScore.Score == 0.0)
                        {
                            // If the highest is a zero, zero them all out.
                            // ToList() needed to apply lazy evaluations in linq, fwiw.
                            roundScores = roundScores.Select(rs => { rs.Score = 0.0; return rs; }).ToList();
                            // Bail out, nothing to do beyond this point.
                            return;
                        }

                        hightestPreNormalizedScoreInRound = roundScore.Score;
                        roundScore.Score = 1000;
                        ++cnt;
                        
                        // Bail out, nothing left to calculate here on the first score.
                        continue;
                    }

                    roundScore.Score = Math.Round((roundScore.Score / hightestPreNormalizedScoreInRound) * 1000);
                    ++cnt;
                }

                // Apply any penalties after normalized scores are computed.
                roundScores = roundScores.Select(rs => { rs.Score = rs.Score - rs.TotalPenalties; return rs; });

                // Protect from negative scores????
                // ToList() needed to apply lazy evaluations in linq, fwiw.
                roundScores = roundScores.Select(rs => { rs.Score = rs.Score < 0 ? 0.0 : rs.Score; return rs; }).ToList(); 

                return;
            }
            catch (Exception ex)
            {
                // If we hit an error during scoring, 0 out all of the round scores that 
                // we may have set during this process.
                foreach (var score in roundScores)
                {
                    score.Score = 0.0;
                }

                // Re-throw is it is a know exception type.
                if (ex.GetType() == typeof(InvalidRoundScoreException))
                {
                    throw;
                }
                else
                {
                    throw new GeneralScoringException($"An Error occured while scoring: {ex.Message}", ex);
                }
                
            }
        }

        /// <summary>
        /// Validates the round scores by f3k rules.
        /// </summary>
        /// <param name="roundScores">The round scores.</param>
        /// <exception cref="CDP.AppDomain.Scoring.InvalidRoundScoreException">
        /// One of the round scores is null.
        /// or a score includes a negative time.
        /// or a score includes a non-task time window.
        /// or a score includes a null or empty pilot id...
        /// </exception>
        private void ValidateRoundScoresByF3KRules(IEnumerable<RoundScoreBase> roundScores)
        {
            // Set some F3K ground rules, no null scores...
            if (roundScores.Any(x => x == null))
            {
                throw new InvalidRoundScoreException("One of the round scores is null.");
            }

            // ...no negative times...
            if (roundScores.Any(x => x.TimeGates.Any(y => y.Time.Seconds < 0)))
            {
                throw new InvalidRoundScoreException($"A score includes a negative time.");
            }

            // ...only time gates of type 'task' (no practice time etc...)
            if (roundScores.Any(x => x.TimeGates.Any(y => y.GateType != TimeGateType.Task)))
            {
                throw new InvalidRoundScoreException("A score includes a non-task time window.");
            }

            // ...no invalid pilot Ids...
            if (roundScores.Any(x => string.IsNullOrEmpty(x.PilotId)))
            {
                throw new InvalidRoundScoreException($"A score includes a null or empty pilot id.");
            }

            // ...no negative penalties
            if (roundScores.Any(x => x.TotalPenalties < 0))
            {
                throw new InvalidRoundScoreException($"A score includes a negative penalty.");
            }
        }
    }
}
