//---------------------------------------------------------------
// Date: 12/9/2017
// Rights: 
// FileName: IPrintFormatter.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Printing.Interfaces
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using System.IO;
    using System.Threading.Tasks;

    public interface IPrintFormatter
    {
        /// <summary>
        /// Creates a contest results document.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        Task<Stream> CreateContestResultsDocumentAsync(Contest contest);

        /// <summary>
        /// Creates a round results document.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        Task<Stream> CreateRoundResultsDocumentAsync(Contest contest);

        /// <summary>
        /// Creates a flight matrix document.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        Task<Stream> CreateFlightMatrixDocumentAsync(FlightMatrix flightMatrix);
    }
}
