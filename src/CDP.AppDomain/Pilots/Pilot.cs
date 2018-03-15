//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: Pilot.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Pilots
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Represents a pilot
    /// </summary>
    [JsonObject]
    public sealed class Pilot
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [JsonProperty]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [JsonProperty]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the airframe.
        /// </summary>
        /// <value>
        /// The airframe.
        /// </value>
        [JsonProperty]
        public string Airframe { get; set; }

        /// <summary>
        /// Gets or sets the standards body identifier.
        /// </summary>
        /// <value>
        /// The standards body identifier.
        /// </value>
        [JsonProperty]
        public string StandardsBodyId { get; set; }

        /// <summary>
        /// Gets the pilot id.
        /// </summary>
        /// <value>
        /// The pilot id.
        /// </value>
        [JsonProperty]
        public string Id { get; set; }

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="Pilot" /> class.
        /// </summary>
        /// <param name="firstName">The pilots' first name.</param>
        /// <param name="lastName">The pilots' last name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="amaNumber">The ama number.</param>
        /// <param name="airframe">The airframe.</param>
        /// <exception cref="ArgumentNullException">
        /// firstName
        /// or
        /// lastName
        /// </exception>
        public Pilot(string firstName, string lastName, string id, string amaNumber = null, string airframe = null)
        {
            this.FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            this.LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            this.StandardsBodyId = amaNumber;
            this.Airframe = airframe;
            this.Id = id;
        }

        #endregion
    }
}
