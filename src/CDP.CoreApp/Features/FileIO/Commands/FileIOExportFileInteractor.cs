//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: FileIOExportFileInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.FileIO.Commands
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FIleIO;
    using CDP.AppDomain.Scoring;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Contests;
    using CDP.CoreApp.Interfaces.FileIO;
    using CDP.CoreApp.Interfaces.FlightMatrices;
    using CDP.CoreApp.Interfaces.Pilots;
    using CDP.CoreApp.Interfaces.Registration;
    using CDP.CoreApp.Interfaces.Scoring;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles exporting contest data to a file.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class FileIOExportFileInteractor : InteractorBase
    {
        /// <summary>
        /// The file exporter
        /// </summary>
        private readonly IFileExporter fileExporter;

        /// <summary>
        /// The contest repository
        /// </summary>
        private readonly IContestRepository contestRepository;

        /// <summary>
        /// The flight matrix repository
        /// </summary>
        private readonly IFlightMatrixRepository flightMatrixRepository;

        /// <summary>
        /// The pilot repository
        /// </summary>
        private readonly IPilotRepository pilotRepository;

        /// <summary>
        /// The pilot registration repository
        /// </summary>
        private readonly IPilotRegistrationRepository pilotRegistrationRepository;

        /// <summary>
        /// The scoring repository
        /// </summary>
        private readonly IScoringRepository scoringRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileIOExportFileInteractor" /> class.
        /// </summary>
        /// <param name="fileExporter">The file exporter.</param>
        /// <param name="contestRepository">The contest repository.</param>
        /// <param name="flightMatrixRepository">The flight matrix repository.</param>
        /// <param name="pilotRepository">The pilot repository.</param>
        /// <param name="pilotRegistrationRepository">The pilot registration repository.</param>
        /// <param name="scoringRepository">The scoring repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">fileExporter</exception>
        /// <exception cref="System.ArgumentNullException">fileExporter</exception>
        public FileIOExportFileInteractor(IFileExporter fileExporter,
            IContestRepository contestRepository,
            IFlightMatrixRepository flightMatrixRepository,
            IPilotRepository pilotRepository,
            IPilotRegistrationRepository pilotRegistrationRepository,
            IScoringRepository scoringRepository,
            ILoggingService logger) : base(logger)
        {
            this.fileExporter = fileExporter ?? throw new ArgumentNullException($"{nameof(fileExporter)} cannot be null.");
            this.contestRepository = contestRepository ?? throw new ArgumentNullException($"{nameof(contestRepository)} cannot be null.");
            this.flightMatrixRepository = flightMatrixRepository ?? throw new ArgumentNullException($"{nameof(flightMatrixRepository)} cannot be null.");
            this.pilotRepository = pilotRepository ?? throw new ArgumentNullException($"{nameof(pilotRepository)} cannot be null.");
            this.pilotRegistrationRepository = pilotRegistrationRepository ?? throw new ArgumentNullException($"{nameof(pilotRegistrationRepository)} cannot be null.");
            this.scoringRepository = scoringRepository ?? throw new ArgumentNullException($"{nameof(scoringRepository)} cannot be null.");
        }

        /// <summary>
        /// Exports the specified contest identifier.
        /// </summary>
        /// <param name="contest">The contest identifier.</param>
        /// <returns></returns>
        public async Task<FileFormat> Export(Contest contest)
        {
            var flightMatrix = await flightMatrixRepository.ReadAsync(contest.Id);
            var pilots = await pilotRepository.ReadAsync();
            var registrations = await pilotRegistrationRepository.ReadAsync(contest.Id);
            var scores = await scoringRepository.ReadAsync(contest.Id);
            var pilotsToSave = pilots.Value.Where(p => contest.PilotRoster.Contains(p.Id));
            var scoresToSave = new List<TimeSheet>();

            // Flatten the scores (returned as a round sorted collection from the repository).
            foreach(var roundScoresCollection in scores.Value)
            {
                scoresToSave.AddRange(roundScoresCollection);
            }

            return new FileFormat(contest, pilotsToSave, flightMatrix.Value, registrations.Value, scoresToSave);
        }
    }
}
