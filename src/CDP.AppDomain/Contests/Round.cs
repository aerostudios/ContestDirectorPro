//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: Round.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Contests
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a round in a contest.
    /// </summary>
    [JsonObject]
    public sealed class Round
    {
        /// <summary>
        /// Gets or sets the assigned task.
        /// </summary>
        /// <value>
        /// The assigned task.
        /// </value>
        [JsonProperty]
        public string AssignedTaskId { get; set; }

        /// <summary>
        /// Gets or sets the flight groups.
        /// </summa0ry>
        /// <value>
        /// The flight groups.
        /// </value>
        [JsonProperty]
        public Dictionary<FlightGroup, List<Pilot>> FlightGroups { get; set; } = new Dictionary<FlightGroup, List<Pilot>>();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fly off round.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fly off round; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty]
        public bool IsFlyOffRound { get; set; } = false;
    }
}