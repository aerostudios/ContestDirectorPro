//---------------------------------------------------------------
// Date: 12/19/2017
// Rights: 
// FileName: IContestScoreAggregator.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Scoring
{
    using CDP.AppDomain.Scoring;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the process for building contest scores
    /// </summary>
    public interface IContestScoreAggregator
    {
        /// <summary>
        /// Generates the contest scores by pilot.
        /// </summary>
        /// <param name="allContestRoundScores">All contest round scores.</param>
        /// <returns></returns>
        Dictionary<string, PilotContestScoreCollection> GenerateContestScores(ContestScoresCollection allContestRoundScores);
    }
}
