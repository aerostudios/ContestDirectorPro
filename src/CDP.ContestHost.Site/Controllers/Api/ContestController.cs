//---------------------------------------------------------------
// Date: 1/1/2018
// Rights: 
// FileName: ContestController.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.Site.Controllers.Api
{
    using CDP.AppDomain.Contests;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.Contests.Queries;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// API controller for contests
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Produces("application/json")]
    [Route("api/Contest")]
    public class ContestController : Controller
    {
        /// <summary>
        /// The contest storage command interactor
        /// </summary>
        private readonly ContestStorageCmdInteractor contestStorageCmdInteractor;

        /// <summary>
        /// The contest query interactor
        /// </summary>
        private readonly ContestQueryInteractor contestQueryInteractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestController"/> class.
        /// </summary>
        /// <param name="contestStorageCmdInteractor">The contest storage command interactor.</param>
        /// <param name="contestQueryInteractor">The contest query interactor.</param>
        public ContestController(ContestStorageCmdInteractor contestStorageCmdInteractor, ContestQueryInteractor contestQueryInteractor)
        {
            this.contestStorageCmdInteractor = contestStorageCmdInteractor;
            this.contestQueryInteractor = contestQueryInteractor;
        }

        /// <summary>
        /// Adds the contest to storage.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddContest([FromBody] Contest contest)
        {
            var result = await this.contestStorageCmdInteractor.CreateContestAsync(contest);
            if (result.IsFaulted)
            {
                return StatusCode(500);
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Gets the current contest from storage.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCurrentContest()
        {
            var result = await this.contestQueryInteractor.GetAllContestsAsync();
            if (result.IsFaulted)
            {
                return BadRequest(result.Error.ErrorMessage);
            }

            return Ok(result.Value.FirstOrDefault());
        }
    }
}