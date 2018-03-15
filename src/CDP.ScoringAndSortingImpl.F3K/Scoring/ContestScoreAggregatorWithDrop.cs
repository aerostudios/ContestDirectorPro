//---------------------------------------------------------------
// Date: 12/19/2017
// Rights: 
// FileName: F3KContestScoreAggregatorWithDrop.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Scoring
{
    using CDP.AppDomain.Scoring;
    using CDP.CoreApp.Interfaces.Scoring;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the process to tally up contest scores, dropping the lowest round after four have been completed.
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Scoring.IContestScoreAggregator" />
    public class ContestScoreAggregatorWithDrop : IContestScoreAggregator
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

            // Remove the lowest score for each, if four rounds have been completed.
            foreach(var individualPilotScoreSet in totals)
            {
                double lowestScore = 0.0;
                int droppedRoundOrdinal = -1;
                
                // Only do this if the pilot has completed 4 rounds
                if (individualPilotScoreSet.Value.Count > 4)
                {
                    // Loop through and find the lowest round score.
                    for (var i = 0; i < individualPilotScoreSet.Value.Count; ++i)
                    {
                        if (individualPilotScoreSet.Value[i] < lowestScore)
                        {
                            lowestScore = individualPilotScoreSet.Value[i];
                            droppedRoundOrdinal = i;
                        }
                    }
                }

                // Total the score and remove the lowest if applicable.
                individualPilotScoreSet.Value.TotalScore = individualPilotScoreSet.Value.Sum(x => x.Value) - lowestScore;

                // Mark the dropped round
                individualPilotScoreSet.Value.DroppedRoundOrdinal = droppedRoundOrdinal;
            }

            // Total the scores for each round, for each pilot and put it in the last "column" / entry (int.MaxValue will be the key) for the pilot
            foreach (var pilotScoreSet in totals)
            {
                
            }

            return totals;
        }
    }
}
