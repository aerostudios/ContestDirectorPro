//---------------------------------------------------------------
// Date: 12/10/2017
// Rights: 
// FileName: FlightMatrixGenInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.FlightMatrices.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Registration;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handles Matrix sorting requests.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class FlightMatrixGenInteractor : InteractorBase
    {
        /// <summary>
        /// The sorting algo
        /// </summary>
        private ISortingAlgo sortingAlgo;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixGenInteractor"/> class.
        /// </summary>
        /// <param name="sortingAlgo">The sorting algo.</param>
        /// <param name="logger">The logger.</param>
        public FlightMatrixGenInteractor(ISortingAlgo sortingAlgo, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(sortingAlgo, nameof(sortingAlgo));
            this.sortingAlgo = sortingAlgo;
        }

        /// <summary>
        /// Creates the sorted flight matrix.
        /// </summary>
        /// <param name="pilotRegistrations">The pilot registrations.</param>
        /// <param name="numberOfRounds">The number of rounds.</param>
        /// <param name="suggestionNumberOfPilotsPerRound">The suggestion number of pilots per round.</param>
        /// <returns></returns>
        public Result<FlightMatrix> CreateSortedFlightMatrix(IEnumerable<PilotRegistration> pilotRegistrations, int numberOfRounds, int suggestionNumberOfPilotsPerRound)
        {
            var validationResult = FlightMatrixHelpers.ValidatePilotRegistrations(pilotRegistrations);

            if (!validationResult.IsValid)
            {
                return Error<FlightMatrix>(null, validationResult.Message);
            }

            try
            {
                var result = this.sortingAlgo.GenerateInitialMatrix(pilotRegistrations, numberOfRounds, suggestionNumberOfPilotsPerRound);

                if (result == null)
                {
                    return Error<FlightMatrix>(null, $"An error occured while sorting pilots.");
                }  

                // Sort the rounds for a cleaner output just in case something got screwed up in the 
                // generation algo.
                result.Matrix.Sort((roundFirst, roundSecond) => roundFirst.RoundOrdinal.CompareTo(roundSecond.RoundOrdinal));

                return Success(result, nameof(CreateSortedFlightMatrix));
            }
            catch (Exception ex)
            {
                return Error<FlightMatrix>(null, ex);
            }
        }
    }
}