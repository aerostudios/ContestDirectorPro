//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: IContestRepository.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Contests
{
    using CDP.AppDomain;
    using CDP.AppDomain.Contests;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a storage interface for contests.
    /// </summary>
    public interface IContestRepository
    {
        /// <summary>
        /// Gets the name of the contest by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<Result<Contest>> ReadAsync(string name);

        /// <summary>
        /// Saves the contest.
        /// </summary>
        /// <param name="contestToSave">The contest to save.</param>
        /// <returns></returns>
        Task<Result<Contest>> UpdateAsync(Contest contestToSave);

        /// <summary>
        /// Deletes the contest.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Result<bool>> DeleteAsync(string id);

        /// <summary>
        /// Creates the contest.
        /// </summary>
        /// <param name="contestToCreate">The contest to create.</param>
        /// <returns></returns>
        Task<Result<Contest>> CreateAsync(Contest contestToCreate);

        /// <summary>
        /// Gets all contests.
        /// </summary>
        /// <returns></returns>
        Task<Result<IEnumerable<Contest>>> ReadAsync();
    }
}
