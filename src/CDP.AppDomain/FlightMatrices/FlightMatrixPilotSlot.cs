//---------------------------------------------------------------
// Date: 1/1/2018
// Rights: 
// FileName: FlightMatrixPilotSlot.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.FlightMatrices
{
    /// <summary>
    /// Defines a pilot slot for a flight matrix round.
    /// </summary>
    public class FlightMatrixPilotSlot
    {
        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        public string PilotId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the flight group.
        /// </summary>
        /// <value>
        /// The flight group.
        /// </value>
        public FlightGroup FlightGroup { get; set; } = FlightGroup.Unknown;
    }
}