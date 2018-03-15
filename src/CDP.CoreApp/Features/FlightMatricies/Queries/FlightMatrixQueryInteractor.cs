//---------------------------------------------------------------
// Date: 12/10/2017
// Rights: 
// FileName: FlightMatrixQueryInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.FlightMatrices.Queries
{
    using CDP.AppDomain;
    using CDP.AppDomain.FlightMatrices;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.FlightMatrices;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles queries for flight matrices
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class FlightMatrixQueryInteractor : InteractorBase
    {
        /// <summary>
        /// The flight matrix repository
        /// </summary>
        private readonly IFlightMatrixRepository flightMatrixRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixQueryInteractor"/> class.
        /// </summary>
        /// <param name="flightMatrixRepository">The flight matrix repository.</param>
        /// <param name="logger">The logger.</param>
        public FlightMatrixQueryInteractor(IFlightMatrixRepository flightMatrixRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(flightMatrixRepository, nameof(flightMatrixRepository));
            this.flightMatrixRepository = flightMatrixRepository;
        }

        /// <summary>
        /// Gets the flight matrix for contest.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        public async Task<Result<FlightMatrix>> GetFlightMatrixForContest(string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<FlightMatrix>(null, $"The {nameof(contestId)} parameter is not valid.");
            }

            try
            {
                var result = await this.flightMatrixRepository.ReadAsync(contestId);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<FlightMatrix>(null, $"An error occured while retrieveing a flight matrix.");
                }

                return Success(result.Value, nameof(GetFlightMatrixForContest));
            }
            catch (Exception ex)
            {
                return Error<FlightMatrix>(null, ex);
            }
        }

        /// <summary>
        /// Gets the pilot sorted flight matrix.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<PilotFlightSchedule>>> GetPilotSortedFlightMatrix(string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<IEnumerable<PilotFlightSchedule>>(null, $"{nameof(FlightMatrixQueryInteractor)}:{nameof(GetPilotSortedFlightMatrix)} - {nameof(contestId)} cannot be null.");
            }
            
            var flightMatrixToConvertResult = await GetFlightMatrixForContest(contestId);
            if (flightMatrixToConvertResult.IsFaulted)
            {
                return Error<IEnumerable<PilotFlightSchedule>>(null, $"{nameof(FlightMatrixQueryInteractor)}:{nameof(GetPilotSortedFlightMatrix)} - The contest with the id:{nameof(contestId)} could not be found.");
            }

            try
            {
                var result = new List<PilotFlightSchedule>();

                foreach (var round in flightMatrixToConvertResult.Value.Matrix)
                {
                    var roundOrdinal = round.RoundOrdinal;

                    foreach (var pilot in round.PilotSlots)
                    {
                        // If we already have one, add to it, otherwise
                        // create and populate.
                        if (result.Any(p => p.PilotId == pilot.PilotId))
                        {
                            result.Where(p => p.PilotId == pilot.PilotId)
                                .First().FlightGroupDraw.Add(pilot.FlightGroup);
                        }
                        else
                        {
                            result.Add(new PilotFlightSchedule
                            {
                                PilotId = pilot.PilotId,
                                FlightGroupDraw = new List<FlightGroup> { pilot.FlightGroup }
                            });
                        }
                    }
                }

                return Success<IEnumerable<PilotFlightSchedule>>(result, nameof(GetPilotSortedFlightMatrix));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<PilotFlightSchedule>>(null, ex);
            }
        }
    }
}