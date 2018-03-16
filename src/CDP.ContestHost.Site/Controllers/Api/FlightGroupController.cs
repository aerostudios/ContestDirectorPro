//---------------------------------------------------------------
// Date: 3/2/2018
// Rights: 
// FileName: FlightGroupController.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.Site.Controllers.Api
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.CoreApp.Features.Contests.Queries;
    using CDP.CoreApp.Features.Pilots.Queries;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    [Produces("application/json")]
    [Route("api/FlightGroup")]
    public class FlightGroupController : Controller
    {
        private readonly ContestQueryInteractor contestQueryInteractor;
        private readonly PilotQueryInteractor pilotQueryInteractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightGroupController"/> class.
        /// </summary>
        /// <param name="contestStorageCmdInteractor">The contest storage command interactor.</param>
        /// <param name="pilotStorageCmdInteractor">The pilot storage command interactor.</param>
        public FlightGroupController(ContestQueryInteractor contestQueryInteractor, PilotQueryInteractor pilotQueryInteractor)
        {
            this.contestQueryInteractor = contestQueryInteractor;
            this.pilotQueryInteractor = pilotQueryInteractor;
        }

        /// <summary>
        /// Gets the flight group for a specific contest and round.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="roundOrdinal">The round ordinal.</param>
        /// <param name="flightGroup">The flight group.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFlightGroup(string contestId, int roundOrdinal, FlightGroup flightGroup)
        {
            var contestResult = await this.contestQueryInteractor.GetAllContestsAsync();
            if (contestResult.IsFaulted)
            {
                return BadRequest();
            }

            var contest = contestResult.Value.Where(c => c.Id == contestId).Single();
            if (contest == null)
            {
                return BadRequest();
            }

            var allPilotsResult = await this.pilotQueryInteractor.GetAllPilotsAsync();
            if (contestResult.IsFaulted)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (contest.Rounds.ContainsKey(roundOrdinal) &&
                contest.Rounds[roundOrdinal].FlightGroups.ContainsKey(flightGroup))
            {
                return Ok(contest.Rounds[roundOrdinal]?.FlightGroups[flightGroup]);
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}