//---------------------------------------------------------------
// Date: 12/10/2017
// Rights: 
// FileName: FlightMatrixStorageCmdInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.FlightMatrices.Commands
{
    using CDP.AppDomain;
    using CDP.AppDomain.FlightMatrices;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.FlightMatrices;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles storage commands for Flight Matrices
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class FlightMatrixStorageCmdInteractor : InteractorBase
    {
        /// <summary>
        /// The flight matrix
        /// </summary>
        private readonly IFlightMatrixRepository flightMatrixRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixStorageCmdInteractor"/> class.
        /// </summary>
        /// <param name="flightMatrixRepository">The flight matrix repository.</param>
        /// <param name="logger">The logger.</param>
        public FlightMatrixStorageCmdInteractor(IFlightMatrixRepository flightMatrixRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(flightMatrixRepository, nameof(flightMatrixRepository));
            this.flightMatrixRepository = flightMatrixRepository;
        }

        /// <summary>
        /// Saves the flight matrix for contest asynchronous.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        public async Task<Result<FlightMatrix>> SaveFlightMatrixForContestAsync(FlightMatrix flightMatrix)
        {
            if (!ValidateFlightMatrix(flightMatrix))
            {
                return Error<FlightMatrix>(null, $"The {nameof(flightMatrix)} is not a valid flight matrix.");
            }

            try
            {
                var result = await this.flightMatrixRepository.CreateAsync(flightMatrix);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<FlightMatrix>(null, "An error occured while creating the flight matrix.");
                }

                return Success(result.Value, nameof(SaveFlightMatrixForContestAsync));
            }
            catch (Exception ex)
            {
                return Error<FlightMatrix>(null, ex);
            }
        }

        /// <summary>
        /// Deletes the flight matrix for contest asynchronous.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteFlightMatrixForContestAsync(FlightMatrix flightMatrix)
        {
            if (!ValidateFlightMatrix(flightMatrix))
            {
                return Error<bool>(false, $"The {nameof(flightMatrix)} is not a valid flight matrix.");
            }

            try
            {
                var result = await this.flightMatrixRepository.DeleteAsync(flightMatrix.ContestId);

                if (result.IsFaulted || result.Value)
                {
                    return Error(false, "An error occured while deleting the flight matrix.");
                }

                return Success(true, nameof(DeleteFlightMatrixForContestAsync));
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }
        }

        /// <summary>
        /// Updates the flight matrix for contest asynchronous.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        public async Task<Result<FlightMatrix>> UpdateFlightMatrixForContestAsync(FlightMatrix flightMatrix)
        {
            if (!ValidateFlightMatrix(flightMatrix))
            {
                return Error<FlightMatrix>(null, $"The {nameof(flightMatrix)} is not a valid flight matrix.");
            }

            try
            {
                var result = await this.flightMatrixRepository.UpdateAsync(flightMatrix);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<FlightMatrix>(null, "An error occured while updating the flight matrix.");
                }

                return Success(result.Value, nameof(UpdateFlightMatrixForContestAsync));
            }
            catch (Exception ex)
            {
                return Error<FlightMatrix>(null, ex);
            }
        }

        /// <summary>
        /// Validates the flight matrix.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool ValidateFlightMatrix(FlightMatrix flightMatrix)
        {
            if (flightMatrix == null)
            {
                return false;
            }

            if (flightMatrix.Matrix == null ||
                flightMatrix.Matrix.Count < 1)
            {
                return false;
            }

            if (string.IsNullOrEmpty(flightMatrix.ContestId))
            {
                return false;
            }

            foreach (var flightMatrixRounds in flightMatrix.Matrix)
            {
                if (flightMatrixRounds.PilotSlots.Count < 1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}