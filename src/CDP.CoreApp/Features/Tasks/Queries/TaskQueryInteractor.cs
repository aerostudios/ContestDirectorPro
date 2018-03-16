//---------------------------------------------------------------
// Date: 2/3/2018
// Rights: 
// FileName: TaskQueryInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Tasks.Queries
{
    using CDP.AppDomain;
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Tasks;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles queries for contest tasks
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public class TaskQueryInteractor : InteractorBase
    {
        /// <summary>
        /// The task repository
        /// </summary>
        private readonly ITaskRepository taskRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskQueryInteractor"/> class.
        /// </summary>
        /// <param name="taskRepository">The task repository.</param>
        /// <param name="logger">The logger.</param>
        public TaskQueryInteractor(ITaskRepository taskRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(taskRepository, nameof(taskRepository));
            this.taskRepository = taskRepository;
        }

        /// <summary>
        /// Gets all tasks asynchronously.
        /// </summary>
        /// <param name="contestType">Type of the contest.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">contestType - Invalid contest type.</exception>
        public async Task<Result<IEnumerable<TaskBase>>> GetAllTasksAsync(ContestType contestType)
        {
            if(contestType == ContestType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(contestType), "Invalid contest type.");
            }

            try
            {
                var result = await this.taskRepository.ReadAsync(contestType);

                if (result.IsFaulted)
                {
                    return Error<IEnumerable<TaskBase>>(null, "No tasks were found for that contest type.");
                }

                return Success(result.Value, nameof(GetAllTasksAsync));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<TaskBase>>(null, ex);
            }
        }

        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Result<TaskBase>> GetTask(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Error<TaskBase>(null, new ArgumentNullException(id));
            }

            try
            {
                var result = await this.taskRepository.ReadAsync(id);

                if (result.IsFaulted)
                {
                    return Error<TaskBase>(null, $"No task was found for id {id}.");
                }

                return Success(result.Value, nameof(GetTask));
            }
            catch (Exception ex)
            {
                return Error<TaskBase>(null, ex);
            }
        }
    }
}
