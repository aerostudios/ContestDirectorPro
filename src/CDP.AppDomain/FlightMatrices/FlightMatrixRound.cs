//---------------------------------------------------------------
// Date: 1/1/2018
// Rights: 
// FileName: FlightMatrixRound.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.FlightMatrices
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a round stored in a flight matrix
    /// </summary>
    public class FlightMatrixRound
    {
        /// <summary>
        /// Gets or sets the round ordinal.
        /// </summary>
        /// <value>
        /// The round ordinal.
        /// </value>
        public int RoundOrdinal { get; set; } = 0;

        /// <summary>
        /// Gets or sets the pilot slots.
        /// </summary>
        /// <value>
        /// The pilot slots.
        /// </value>
        public List<FlightMatrixPilotSlot> PilotSlots { get; set; } = new List<FlightMatrixPilotSlot>();
    }
}
