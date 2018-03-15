//---------------------------------------------------------------
// Date: 12/19/2017
// Rights: 
// FileName: F3KContestScoreAggregatorNoDrop.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Scoring
{
    using CDP.AppDomain.Scoring;
    using CDP.CoreApp.Interfaces.Scoring;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the process to tally up all the scores and *not* drop the lowest round
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Scoring.IContestScoreAggregator" />
    public class ContestScoreAggregatorNoDrop : IContestScoreAggregator
    {
        /// <summary>
        /// Generates the contest scores by pilot.
        /// </summary>
        /// <param name="allContestRoundScores">All contest round scores.</param>
        /// <returns></returns>
        public Dictionary<string, PilotContestScoreCollection> GenerateContestScores(ContestScoresCollection allContestRoundScores)
        {
            // Init the output collection.
            var totals = new Dictionary</*PilotId*/string, PilotContestScoreCollection>();

            // Flatten the rounds and
            // group all of the scores by pilot id
            var pilotTimeSheets = new List<TimeSheet>();
            foreach (var round in allContestRoundScores)
            {
                pilotTimeSheets.AddRange(round.Select(r => r));
            }

            var pilotScoreGroups = pilotTimeSheets.GroupBy(pts => pts.PilotId);

            // Take each pilot and their scores and place each round score in the output collection.
            foreach (var individualPilotScoreSet in pilotScoreGroups)
            {
                foreach (var roundScore in individualPilotScoreSet)
                {
                    // Check to see if we have a pilot entry in the output collection, if not, create one.
                    if (!totals.ContainsKey(roundScore.PilotId))
                    {
                        totals.Add(roundScore.PilotId, new PilotContestScoreCollection(roundScore.PilotId, 
                            individualPilotScoreSet.Sum(scores => scores.Score),
                            individualPilotScoreSet.ToDictionary(k => k.RoundOrdinal, v => v.Score)));
                    }

                    // If there are multiple scores for the same round for a pilot, only take the first
                    if (!totals[roundScore.PilotId].ContainsKey(roundScore.RoundOrdinal))
                    {
                        totals[roundScore.PilotId].Add(roundScore.RoundOrdinal, roundScore.Score);
                    }
                }
            }

            return totals;
        }
    }
}
