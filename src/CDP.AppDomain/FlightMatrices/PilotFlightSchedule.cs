//---------------------------------------------------------------
// Date: 1/1/2018
// Rights: 
// FileName: PilotFlightSchedule.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.FlightMatrices
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains a pilot and their drawn flight groups
    /// </summary>
    public class PilotFlightSchedule
    {
        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        public string PilotId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the flight group draw.  Optimally, ordered sequencially by round.
        /// </summary>
        /// <value>
        /// The flight group draw.
        /// </value>
        public List<FlightGroup> FlightGroupDraw { get; set; } = new List<FlightGroup>();
    }
}
