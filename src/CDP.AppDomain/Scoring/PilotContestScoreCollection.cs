//---------------------------------------------------------------
// Date: 12/19/2017
// Rights: 
// FileName: PilotContestScoreCollection.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a collection of round scores for a pilot
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.Int32, System.Double}" />
    [JsonObject]
    public class PilotContestScoreCollection : Dictionary<int, double>
    {
        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        [JsonProperty]
        public string PilotId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total score.
        /// </summary>
        /// <value>
        /// The total score.
        /// </value>
        [JsonProperty]
        public double TotalScore { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the dropped round ordinal.
        /// </summary>
        /// <value>
        /// The dropped round ordinal.
        /// </value>
        [JsonProperty]
        public int DroppedRoundOrdinal { get; set; } = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class (for serialization).
        /// </summary>
        public PilotContestScoreCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class.
        /// </summary>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="totalScore">The total score.</param>
        public PilotContestScoreCollection(string pilotId, double totalScore): base()
        {
            this.PilotId = pilotId;
            this.TotalScore = totalScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class.
        /// </summary>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="totalScore">The total score.</param>
        /// <param name="capacity">The capacity.</param>
        public PilotContestScoreCollection(string pilotId, double totalScore, int capacity): base(capacity)
        {
            this.PilotId = pilotId;
            this.TotalScore = totalScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class.
        /// </summary>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="totalScore">The total score.</param>
        /// <param name="comparer">The comparer.</param>
        public PilotContestScoreCollection(string pilotId, double totalScore, IEqualityComparer<int> comparer) : base(comparer)
        {
            this.PilotId = pilotId;
            this.TotalScore = totalScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class.
        /// </summary>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="totalScore">The total score.</param>
        /// <param name="dictionary">The dictionary.</param>
        public PilotContestScoreCollection(string pilotId, double totalScore, IDictionary<int, double> dictionary): base(dictionary)
        {
            this.PilotId = pilotId;
            this.TotalScore = totalScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class.
        /// </summary>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="totalScore">The total score.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public PilotContestScoreCollection(string pilotId, double totalScore, int capacity, IEqualityComparer<int> comparer) : base(capacity, comparer)
        {
            this.PilotId = pilotId;
            this.TotalScore = totalScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class.
        /// </summary>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="totalScore">The total score.</param>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public PilotContestScoreCollection(string pilotId, double totalScore, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.PilotId = pilotId;
            this.TotalScore = totalScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotContestScoreCollection"/> class.
        /// </summary>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="totalScore">The total score.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="comparer">The comparer.</param>
        public PilotContestScoreCollection(string pilotId, double totalScore, IDictionary<int, double> dictionary, IEqualityComparer<int> comparer) : base(dictionary, comparer)
        {
            this.PilotId = pilotId;
            this.TotalScore = totalScore;
        }

    }
}