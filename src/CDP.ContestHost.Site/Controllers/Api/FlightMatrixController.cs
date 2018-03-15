//---------------------------------------------------------------
// Date: 3/1/2018
// Rights: 
// FileName: FlightMatrixController.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.Site.Controllers.Api
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.CoreApp.Features.FlightMatrices.Commands;
    using CDP.CoreApp.Features.FlightMatrices.Queries;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/FlightMatrix")]
    public class FlightMatrixController : Controller
    {
        private readonly FlightMatrixStorageCmdInteractor flightMatrixStorageCmdInteractor;
        private readonly FlightMatrixQueryInteractor flightMatrixQueryInteractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixController"/> class.
        /// </summary>
        /// <param name="flightMatrixStorageCmdInteractor">The flight matrix storage command interactor.</param>
        /// <param name="flightMatrixQueryInteractor">The flight matrix query interactor.</param>
        public FlightMatrixController(FlightMatrixStorageCmdInteractor flightMatrixStorageCmdInteractor, 
            FlightMatrixQueryInteractor flightMatrixQueryInteracto)
        {
            this.flightMatrixStorageCmdInteractor = flightMatrixStorageCmdInteractor;
            this.flightMatrixQueryInteractor = flightMatrixQueryInteractor;
        }

        /// <summary>
        /// Adds the flight matrix to storage
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddFlightMatrix([FromBody] FlightMatrix flightMatrix)
        {
            var result = await this.flightMatrixStorageCmdInteractor.SaveFlightMatrixForContestAsync(flightMatrix);
            if (result.IsFaulted)
            {
                return BadRequest(result.Error.ErrorMessage);
            }

            return Ok();
        }

        /// <summary>
        /// Gets the flight matrix for contest from storage.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlightMatrixForContest(string contestId)
        {
            var pilotViewResult = await this.flightMatrixQueryInteractor.GetPilotSortedFlightMatrix(contestId);
            if (pilotViewResult.IsFaulted)
            {
                return BadRequest(pilotViewResult.Error.ErrorMessage);
            }
            return Ok(pilotViewResult.Value);
        }
    }
}