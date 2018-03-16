//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: FinalRoundScoreViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    /// <summary>
    /// Details a final round score
    /// </summary>
    public class FinalRoundScoreViewModel
    {
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the round ordinal.
        /// </summary>
        /// <value>
        /// The round ordinal.
        /// </value>
        public int RoundOrdinal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dropped.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dropped; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropped { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [includes penalty].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [includes penalty]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludesPenalty { get; set; }
    }
}
