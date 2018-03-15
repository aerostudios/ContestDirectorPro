//---------------------------------------------------------------
// Date: 2/3/2018
// Rights: 
// FileName: ContestRoundScoresCollection.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring
{
    using System.Collections.Generic;

    /// <summary>
    /// Collection for a Round's Timesheets
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{CDP.AppDomain.Scoring.TimeSheet}" />
    public class ContestRoundScoresCollection : List<TimeSheet>
    {
        /// <summary>
        /// Gets or sets the round ordinal.
        /// </summary>
        /// <value>
        /// The round ordinal.
        /// </value>
        public int RoundOrdinal { get; set; } = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestRoundScoresCollection"/> class.
        /// </summary>
        public ContestRoundScoresCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestRoundScoresCollection" /> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <param name="roundOrdinal">The round ordinal.</param>
        public ContestRoundScoresCollection(IEnumerable<TimeSheet> collection, int roundOrdinal) : base(collection)
        {
            this.RoundOrdinal = roundOrdinal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestRoundScoresCollection" /> class.
        /// </summary>
        /// <param name="roundOrdinal">The round ordinal.</param>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public ContestRoundScoresCollection(int roundOrdinal, int capacity) : base(capacity)
        {
            this.RoundOrdinal = roundOrdinal;
        }
    }
}
