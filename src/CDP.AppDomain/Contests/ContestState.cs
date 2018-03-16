//---------------------------------------------------------------
// Date: 1/16/2018
// Rights: 
// FileName: ContestState.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Contests
{
    using CDP.AppDomain.FlightMatrices;

    /// <summary>
    /// Holds some basic state for a contest in progress
    /// </summary>
    public sealed class ContestState
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has started.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has started; otherwise, <c>false</c>.
        /// </value>
        public bool HasStarted { get; set; } = false;

        /// <summary>
        /// Gets or sets the current round ordinal.
        /// </summary>
        /// <value>
        /// The current round ordinal.
        /// </value>
        public int CurrentRoundOrdinal { get; set; } = 0;

        /// <summary>
        /// Gets or sets the current flight group.
        /// </summary>
        /// <value>
        /// The current flight group.
        /// </value>
        public FlightGroup CurrentFlightGroup { get; set; } = FlightGroup.A;
    }
}