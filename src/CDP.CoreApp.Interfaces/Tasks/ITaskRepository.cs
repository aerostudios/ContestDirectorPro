//---------------------------------------------------------------
// Date: 2/3/2018
// Rights: 
// FileName: ITaskRepository.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Tasks
{
    using CDP.AppDomain;
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Tasks;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a data storage interface for Contest Tasks
    /// </summary>
    public interface ITaskRepository
    {
        /// <summary>
        /// Gets all contests.
        /// </summary>
        /// <returns></returns>
        Task<Result<IEnumerable<TaskBase>>> ReadAsync(ContestType contestType);

        /// <summary>
        /// Reads the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Result<TaskBase>> ReadAsync(string id);
    }
}
