//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: RoundScore.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Timesheet
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Scoring.RoundScoreBase" />
    [JsonObject]
    public sealed class TimeSheet : RoundScoreBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contest identifier.
        /// </summary>
        /// <value>
        /// The contest identifier.
        /// </value>
        [JsonProperty("contestId")]
        public string ContestId { get; set; }

        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>
        /// The task identifier.
        /// </value>
        [JsonProperty("taskId")]
        public string TaskId { get; set; }
    }
}