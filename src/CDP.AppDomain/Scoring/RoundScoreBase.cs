//---------------------------------------------------------------
// Date: 2/4/2017
// Rights: 
// FileName: RoundScoreBase.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Tasks;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Basic scoring class.  Should be extended based on the contest.
    /// </summary>
    /// <seealso cref="System.IComparable{CDP.AppDomain.Scoring.RoundScoreBase}" />
    /// <seealso cref="System.IEquatable{CDP.AppDomain.Scoring.RoundScoreBase}" />
    [JsonObject]
    public abstract class RoundScoreBase : IComparable<RoundScoreBase>, IEquatable<RoundScoreBase>
    {
        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        [JsonProperty("pilotId")]
        public virtual string PilotId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the round ordinal.
        /// </summary>
        /// <value>
        /// The round ordinal.
        /// </value>
        [JsonProperty("roundOrdinal")]
        public virtual int RoundOrdinal { get; set; } = 0;

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        [JsonProperty("flightGroup")]
        public FlightGroup FlightGroup { get; set; }

        /// <summary>
        /// Gets or sets the time gates.
        /// </summary>
        /// <value>
        /// The time gates.
        /// </value>
        [JsonProperty("timeGates")]
        public virtual List<TimeGate> TimeGates { get; set; } = new List<TimeGate>();

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        [JsonProperty("score")]
        public virtual double Score { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the total penalties.
        /// </summary>
        /// <value>
        /// The total penalties.
        /// </value>
        [JsonProperty("totalPenalties")]
        public virtual double TotalPenalties { get; set; } = 0.0;

        /// <summary>
        /// Compares one round score to another for sorting (Descending order is the sort btw...)
        /// </summary>
        /// <param name="x">The other instance to compare to.</param>
        /// <returns></returns>
        public virtual int CompareTo(RoundScoreBase x)
        {
            if (x == null) return -1;

            if (x.Score == this.Score)
            {
                return 0;
            }
            else if (x.Score > this.Score)
            {
                return 1;
            }

            return -1;
        }

        /// <summary>
        /// Equality comparer.  Compares two scores to see if they are equal.
        /// This is an odd implementation, as we are only comparing scores and nothing else.
        /// </summary>
        /// <param name="x">The other instance to compare equality to.</param>
        /// <returns></returns>
        public virtual bool Equals(RoundScoreBase x)
        {
            if (x.Score == x.Score)
            {
                return true;
            }

            return false;
        }
    }
}
