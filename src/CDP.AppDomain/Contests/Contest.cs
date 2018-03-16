//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: Contest.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Contests
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a contest
    /// </summary>
    [JsonObject]
    public sealed class Contest
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty]
        public string Id { get; set; } = string.Empty;

         /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [JsonProperty]
        public ContestState State { get; set; } = new ContestState();

        /// <summary>
        /// Gets or sets the type of the sorting.
        /// </summary>
        /// <value>
        /// The type of the sorting.
        /// </value>
        [JsonProperty]
        public string SortingAlgoId { get; set; }

        /// <summary>
        /// Gets or sets the number of groups.
        /// </summary>
        /// <value>
        /// The number of groups.
        /// </value>
        [JsonProperty]
        public int SuggestedNumberOfPilotsPerGroup { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of pilots.
        /// </summary>
        /// <value>
        /// The number of pilots.
        /// </value>
        [JsonIgnore]
        public int NumberOfPilots => this.PilotRoster.Count;

        /// <summary>
        /// Gets or sets the number of flyoff rounds.
        /// </summary>
        /// <value>
        /// The number of flyoff rounds.
        /// </value>
        [JsonProperty]
        public int NumberOfFlyoffRounds { get; set; } = 0;

        /// <summary>
        /// Gets or sets the fly off selection algo identifier.
        /// </summary>
        /// <value>
        /// The fly off selection algo identifier.
        /// </value>
        [JsonProperty]
        public string FlyOffSelectionAlgoId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [JsonProperty]
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        [JsonProperty]
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// Gets or sets the pilot roster.
        /// </summary>
        /// <value>
        /// The pilot roster.
        /// </value>s
        [JsonProperty]
        public List<string> PilotRoster { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the rounds.
        /// </summary>
        /// <value>
        /// The rounds.
        /// </value>
        [JsonProperty]
        public Dictionary<int, Round> Rounds { get; set; } = new Dictionary<int, Round>();

        /// <summary>
        /// Gets or sets a value indicating whether [allow dropped round].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow dropped round]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty]
        public bool AllowDroppedRound { get; set; } = true;

        /// <summary>
        /// Gets or sets the contest category.
        /// </summary>
        /// <value>
        /// The contest category.
        /// </value>
        [JsonProperty]
        public ContestType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the Contest Registration Fee.
        /// </summary>
        /// <value>
        /// The Contest Registration Fee.
        /// </value>
        [JsonProperty]
        public double ContestRegistrationFee { get; set; }     
    }
}