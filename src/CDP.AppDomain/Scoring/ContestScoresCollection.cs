//---------------------------------------------------------------
// Date: 2/3/2018
// Rights: 
// FileName: ContestScoresCollection.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Scoring
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A collection for Round Scores.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{CDP.AppDomain.Scoring.ContestRoundScoresCollection}" />
    public class ContestScoresCollection : List<ContestRoundScoresCollection>
    {
        /// <summary>
        /// Gets the rounds.
        /// </summary>
        /// <value>
        /// The rounds.
        /// </value>
        public List<int> Rounds { get { return this.Select(r => r.RoundOrdinal)?.Distinct()?.ToList(); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestScoresCollection"/> class.
        /// </summary>
        public ContestScoresCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestScoresCollection"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ContestScoresCollection(IEnumerable<ContestRoundScoresCollection> collection) : base(collection) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestScoresCollection"/> class.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public ContestScoresCollection(int capacity) : base(capacity) { }
    }
}
