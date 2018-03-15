//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: IFileExporter.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.FileIO
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Registration;
    using CDP.AppDomain.Scoring;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a file exporter
    /// </summary>
    public interface IFileExporter
    {
        /// <summary>
        /// Exports the specified contest.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <param name="pilots">The pilots.</param>
        /// <param name="pilotRegistrations">The pilot registrations.</param>
        /// <param name="timeSheets">The time sheets.</param>
        /// <returns></returns>
        bool Export(Contest contest,
            FlightMatrix flightMatrix,
            IEnumerable<Pilot> pilots,
            IEnumerable<PilotRegistration> pilotRegistrations,
            IEnumerable<TimeSheet> timeSheets);
    }
}
