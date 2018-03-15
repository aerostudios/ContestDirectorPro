//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: PilotContestScoreViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// View model for the contest rounds, contest totals.
    /// </summary>
    public class PilotContestScoreViewModel
    {
        /// <summary>
        /// Gets or sets the name of the pilot.
        /// </summary>
        /// <value>
        /// The name of the pilot.
        /// </value>
        public string PilotName { get; set; }

        /// <summary>
        /// Gets or sets the scores.
        /// </summary>
        /// <value>
        /// The scores.
        /// </value>
        public Dictionary<int, FinalRoundScoreViewModel> Scores { get; set; }

        /// <summary>
        /// Gets the total score.
        /// </summary>
        /// <value>
        /// The total score.
        /// </value>
        public double TotalScore => Scores.Where(s => !s.Value.IsDropped).Sum(s => s.Value.Score);
    }
}
