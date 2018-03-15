//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: FileFormat.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.FIleIO
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Registration;
    using CDP.AppDomain.Scoring;
    using System.Collections.Generic;

    /// <summary>
    /// The File format for CDP
    /// </summary>
    public sealed class FileFormat
    {
        /// <summary>
        /// Gets the contest.
        /// </summary>
        /// <value>
        /// The contest.
        /// </value>
        public Contest Contest { get; private set; }

        /// <summary>
        /// Gets the pilots.
        /// </summary>
        /// <value>
        /// The pilots.
        /// </value>
        public List<Pilot> Pilots { get; private set; } = new List<Pilot>();

        /// <summary>
        /// Gets the flight matrix.
        /// </summary>
        /// <value>
        /// The flight matrix.
        /// </value>
        public FlightMatrix FlightMatrix { get; private set; }

        /// <summary>
        /// Gets the registrations.
        /// </summary>
        /// <value>
        /// The registrations.
        /// </value>
        public List<PilotRegistration> Registrations { get; private set; } = new List<PilotRegistration>();

        /// <summary>
        /// Gets the time sheets.
        /// </summary>
        /// <value>
        /// The time sheets.
        /// </value>
        public List<TimeSheet> TimeSheets { get; private set; } = new List<TimeSheet>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFormat" /> class.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <param name="pilots">The pilots.</param>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <param name="registrations">The registrations.</param>
        /// <param name="timeSheets">The time sheets.</param>
        public FileFormat(Contest contest,
            IEnumerable<Pilot> pilots,
            FlightMatrix flightMatrix,
            IEnumerable<PilotRegistration> registrations, 
            IEnumerable<TimeSheet> timeSheets)
        {
            this.Contest = contest;
            this.FlightMatrix = flightMatrix;
            this.Pilots = pilots == null ? new List<Pilot>(pilots) : new List<Pilot>();
            this.Registrations = registrations == null ? new List<PilotRegistration>(registrations) : new List<PilotRegistration>();
            this.TimeSheets = timeSheets == null ? new List<TimeSheet>(timeSheets) : new List<TimeSheet>();
        }
    }
}
