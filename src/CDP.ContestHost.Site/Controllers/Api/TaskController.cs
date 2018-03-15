//---------------------------------------------------------------
// Date: 3/1/2018
// Rights: 
// FileName: TaskController.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.Site.Controllers.Api
{
    using CDP.CoreApp.Features.Tasks.Queries;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    /// <summary>
    /// API controller for Contest Tasks
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Produces("application/json")]
    [Route("api/Task")]
    public class TaskController : Controller
    {
        /// <summary>
        /// The task query interactor
        /// </summary>
        private readonly TaskQueryInteractor taskQueryInteractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskController"/> class.
        /// </summary>
        /// <param name="taskQueryInteractor">The task query interactor.</param>
        public TaskController(TaskQueryInteractor taskQueryInteractor)
        {
            this.taskQueryInteractor = taskQueryInteractor;
        }

        /// <summary>
        /// Gets the task by id from storage.
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTaskById(string taskId)
        {
            var getTaskResult = await this.taskQueryInteractor.GetTask(taskId);
            if (getTaskResult.IsFaulted)
            {
                return BadRequest(getTaskResult.Error.ErrorMessage);
            }

            return Ok(getTaskResult.Value);
        }
    }
}