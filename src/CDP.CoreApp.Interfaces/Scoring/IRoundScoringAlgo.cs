//---------------------------------------------------------------
// Date: 12/15/2017
// Rights: 
// FileName: IRoundScoringAlgo.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Scoring
{
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Tasks;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a round scoring algo.  Allows each contest type to score as they see fit.
    /// </summary>
    public interface IRoundScoringAlgo
    {
        /// <summary>
        /// Scores the round.
        /// </summary>
        /// <param name="roundScores">The round scores.</param>
        /// <param name="task">The task.</param>
        void ScoreRound(IEnumerable<RoundScoreBase> roundScores, TaskBase task);
    }
}
