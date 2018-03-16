//---------------------------------------------------------------
// Date: 12/10/2017
// Rights: 
// FileName: IFlightMatrixRepository.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.FlightMatrices
{
    using CDP.AppDomain;
    using CDP.AppDomain.FlightMatrices;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a storage interface for flight matrices
    /// </summary>
    public interface IFlightMatrixRepository
    {
        /// <summary>
        /// Gets a flight matrix by Contest Id.
        /// </summary>
        /// <param name="contestId">The identifier.</param>
        /// <returns></returns>
        Task<Result<FlightMatrix>> ReadAsync(string contestId);
        
        /// <summary>
        /// Updates the specified flight matrix.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        Task<Result<FlightMatrix>> UpdateAsync(FlightMatrix flightMatrix);

        /// <summary>
        /// Deletes the flight matrix.
        /// </summary>
        /// <param name="flightMatrixId">The contest identifier.</param>
        /// <returns></returns>
        Task<Result<bool>> DeleteAsync(string flightMatrixId);

        /// <summary>
        /// Creates the specified flight matrix.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        Task<Result<FlightMatrix>> CreateAsync(FlightMatrix flightMatrix);
    }
}
