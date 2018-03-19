//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: PilotRegistration.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Registration
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Model for registrations
    /// </summary>
    /// <seealso cref="ContestDirectorPro.Data.Models.PilotRegistrationBase" />
    [JsonObject]
    public class PilotRegistration
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the contest identifier.
        /// </summary>
        /// <value>
        /// The contest identifier.
        /// </value>
        [JsonProperty]
        public string ContestId { get; set; }

        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        [JsonProperty]
        public string PilotId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is paid; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty]
        public bool IsPaid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [airframes signed off].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [airframes signed off]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty]
        public bool AirframesSignedOff { get; set; }

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        /// <value>
        /// The name of the team.
        /// </value>
        [JsonProperty]
        public string TeamName { get; set; }

        /// <summary>
        /// Gets or sets the airframe registration numbers.
        /// </summary>
        /// <value>
        /// The airframe registration numbers.
        /// </value>
        [JsonProperty]
        public IEnumerable<string> AirframeRegistrationNumbers { get; set; } = new List<string>();
    }
}
