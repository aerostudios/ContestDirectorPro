//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: IPilotRepository.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Pilots
{
    using CDP.AppDomain;
    using CDP.AppDomain.Pilots;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a storage interface for pilots
    /// </summary>
    public interface IPilotRepository
    {
        /// <summary>
        /// Gets the requested pilot.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Result<Pilot>> ReadAsync(string id);

        /// <summary>
        /// Gets all pilots.
        /// </summary>
        /// <returns></returns>
        Task<Result<IEnumerable<Pilot>>> ReadAsync();

        /// <summary>
        /// Updates a pilot.
        /// </summary>
        /// <param name="pilotToUpdate">The pilot.</param>
        /// <returns></returns>
        Task<Result<Pilot>> UpdateAsync(Pilot pilotToUpdate);

        /// <summary>
        /// Deletes a pilot.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Result<bool>> DeleteAsync(string id);

        /// <summary>
        /// Creates a pilot.
        /// </summary>
        /// <param name="pilotToCreate">The pilot.</param>
        /// <returns></returns>
        Task<Result<Pilot>> CreateAsync(Pilot pilotToCreate);
    }
}
