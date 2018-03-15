//---------------------------------------------------------------
// Date: 2/7/2018
// Rights: 
// FileName: LiveScoreViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    /// <summary>
    /// View model for an entry in the "Live Scores" section of the contest rounds page
    /// </summary>
    public class LiveScoreViewModel
    {
        /// <summary>
        /// Gets or sets the name of the pilot.
        /// </summary>
        /// <value>
        /// The name of the pilot.
        /// </value>
        public string PilotName { get; set; }

        /// <summary>
        /// Gets or sets the flight time.
        /// </summary>
        /// <value>
        /// The flight time.
        /// </value>
        public string FlightTime { get; set; }
    }
}
