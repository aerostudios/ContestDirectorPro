//---------------------------------------------------------------
// Date: 2/19/2018
// Rights: 
// FileName: ContestOpenParameters.cs
//---------------------------------------------------------------

namespace CDP.UWP.Helpers
{
    using CDP.AppDomain.Contests;
    using System;

    /// <summary>
    /// Helps us override a contest that is currently loaded in the app.
    /// </summary>
    public class ContestOpenParameters
    {
        /// <summary>
        /// Gets a value indicating whether [override existing contest].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [override existing contest]; otherwise, <c>false</c>.
        /// </value>
        public bool OverrideExistingContest { get; private set; }

        /// <summary>
        /// Gets the contest.
        /// </summary>
        /// <value>
        /// The contest.
        /// </value>
        public Contest Contest { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestOpenParameters"/> class.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <param name="overrideExistingContest">if set to <c>true</c> [override existing contest].</param>
        /// <exception cref="ArgumentNullException">ContestOpenParameters</exception>
        public ContestOpenParameters(Contest contest, bool overrideExistingContest = false)
        {
            this.Contest = contest ?? throw new ArgumentNullException($"{nameof(ContestOpenParameters)}:Ctor - {nameof(contest)} cannot be null.");
            this.OverrideExistingContest = overrideExistingContest;
        }
    }
}
