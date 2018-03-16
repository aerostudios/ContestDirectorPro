//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: FlightMatrix.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.FlightMatrices
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a flight martrix for a contest.  The flight matrix deermines the order
    /// in which pilots will fly.
    /// </summary>
    [JsonObject]
    public sealed class FlightMatrix
    {
        /// <summary>
        /// Gets or sets the contest identifier.
        /// </summary>
        /// <value>
        /// The contest identifier.
        /// </value>
        [JsonProperty]
        public string ContestId { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the matrix.
        /// </summary>
        /// <value>
        /// The matrix.
        /// </value>
        [JsonProperty]
        public List<FlightMatrixRound> Matrix { get; set; } = new List<FlightMatrixRound>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrix"/> class.
        /// </summary>
        [JsonConstructor]
        public FlightMatrix() { }
    }
}
