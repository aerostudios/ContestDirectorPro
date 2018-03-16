//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: AddPilotParameters.cs
//---------------------------------------------------------------

namespace CDP.UWP.Helpers
{
    using CDP.AppDomain.Contests;
    using System;

    /// <summary>
    /// Navigation helper.  Used to pass parameters to the AddPilot Page
    /// </summary>
    public class AddPilotParameters
    {
        /// <summary>
        /// Gets or sets the originating page.
        /// </summary>
        /// <value>
        /// The originating page.
        /// </value>
        public Type OriginatingPage { get; set; }

        /// <summary>
        /// Gets or sets the contest.
        /// </summary>
        /// <value>
        /// The contest.
        /// </value>
        public Contest Contest { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPilotParameters"/> class.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <param name="originatingPage">The originating page.</param>
        /// <exception cref="ArgumentNullException">
        /// AddPilotParameters
        /// or
        /// AddPilotParameters
        /// </exception>
        public AddPilotParameters(Contest contest, Type originatingPage)
        {
            this.Contest = contest ?? throw new ArgumentNullException($"{nameof(AddPilotParameters)}:Contructor - {nameof(contest)} cannot be null.");
            this.OriginatingPage = originatingPage ?? throw new ArgumentNullException($"{nameof(AddPilotParameters)}:Contructor - {nameof(originatingPage)} cannot be null.");
        }
    }
}
