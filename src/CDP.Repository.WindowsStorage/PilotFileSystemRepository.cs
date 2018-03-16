//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: PilotFileSystemRepository.cs
//---------------------------------------------------------------

namespace CDP.Repository.WindowsStorage
{
    using CDP.AppDomain;
    using CDP.AppDomain.Pilots;
    using CDP.Common.Caching;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Pilots;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Data storage manager for pilots.  Stores data in the file system.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.Features.Pilots.Interfaces.IPilotRepository" />
    public sealed class PilotFileSystemRepository : RepositoryBase, IPilotRepository
    {
        /// <summary>
        /// The pilots cache key
        /// </summary>
        private const string PILOTS_CACHE_KEY = "pilots";

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotFileSystemRepository" /> class.
        /// </summary>
        /// <param name="fileName">The file path.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">filePath - filePath</exception>
        public PilotFileSystemRepository(string fileName, ICache cache, ILoggingService logger) : base(fileName, cache, logger) { }

        /// <summary>
        /// Creates a pilot.
        /// </summary>
        /// <param name="pilotToCreate">The pilot.</param>
        /// <returns></returns>
        public async Task<Result<Pilot>> CreateAsync(Pilot pilotToCreate)
        {
            if(pilotToCreate == null)
            {
                return Error<Pilot>(null, "The Pilot cannot be null");
            }

            if (!string.IsNullOrEmpty(pilotToCreate.Id))
            {
                return Error<Pilot>(null, "The Id already has a value");
            }

            var allPilots = new List<Pilot>();

            try
            {
                allPilots = this.cache.Get<List<Pilot>>(PILOTS_CACHE_KEY);
                if (allPilots == null) allPilots = (await GetAll<Pilot>()) ?? new List<Pilot>();

                if (allPilots == null)
                {
                    allPilots = new List<Pilot>();
                }
            }
            catch(Exception ex)
            {
                this.logger.LogException(ex);
                throw;
            }

            // Assign a new GUID for the ID
            pilotToCreate.Id = base.GenerateId();

            allPilots.Add(pilotToCreate);

            try
            {
                var success = await WriteAll(allPilots);
                this.cache.Put(PILOTS_CACHE_KEY, allPilots);
            }
            catch (Exception ex)
            {
                return Error<Pilot>(null, ex);
            }

            return Success((allPilots.Where(p => p.Id == pilotToCreate.Id)).First(), nameof(CreateAsync));
        }

        /// <summary>
        /// Deletes a pilot.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Error(false, new ArgumentNullException(nameof(id), "Id cannot be null or empty."));
            }

            var allPilots = this.cache.Get<List<Pilot>>(PILOTS_CACHE_KEY);
            if (allPilots == null) allPilots = (await GetAll<Pilot>()) ?? new List<Pilot>();

            if (allPilots == null)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }
            else if (allPilots.Count(p => p.Id == id) == 0)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }

            // Grab all of the Pilots w/o the id passed in and save.
            var pilotsToSave = allPilots.Where(p => p.Id != id);

            var result = false;

            try
            {
                var pilots = pilotsToSave.ToList();
                result = await WriteAll(pilots);
                this.cache.Put(PILOTS_CACHE_KEY, pilots);
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }

            return Success(result, nameof(DeleteAsync));
        }

        /// <summary>
        /// Gets the requested pilot.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Result<Pilot>> ReadAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Error<Pilot>(null, new ArgumentNullException(nameof(id), "Id cannot be null or empty."));
            }

            var allPilots = this.cache.Get<List<Pilot>>(PILOTS_CACHE_KEY);
            if (allPilots != null) allPilots = (await GetAll<Pilot>()) ?? new List<Pilot>();

            if (allPilots == null)
            {
                allPilots = new List<Pilot>();
            }

            var pilotToReturn = allPilots.FirstOrDefault(p => p.Id == id);

            if (pilotToReturn == null)
            {
                return Error<Pilot>(null, new Exception($"A pilot with the id {id} could not be found"));
            }

            return Success(pilotToReturn, nameof(ReadAsync));
        }

        /// <summary>
        /// Gets all pilots.
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IEnumerable<Pilot>>> ReadAsync()
        {
            var allPilots = this.cache.Get<List<Pilot>>(PILOTS_CACHE_KEY);
            if (allPilots != null) return Success<IEnumerable<Pilot>>(allPilots, nameof(ReadAsync));
            allPilots = (await GetAll<Pilot>()) ?? new List<Pilot>();
            
            if (allPilots == null)
            {
                allPilots = new List<Pilot>();
            }

            return Success<IEnumerable<Pilot>>(allPilots, nameof(ReadAsync));
        }

        /// <summary>
        /// Updates a pilot.
        /// </summary>
        /// <param name="pilot">The pilot.</param>
        /// <returns></returns>
        public async Task<Result<Pilot>> UpdateAsync(Pilot pilotToUpdate)
        {
            if (pilotToUpdate == null)
            {
                return Error<Pilot>(null, new ArgumentNullException(nameof(pilotToUpdate), "Pilot cannot be null."));
            }

            if (string.IsNullOrEmpty(pilotToUpdate.Id))
            {
                return Error<Pilot>(null, new ArgumentOutOfRangeException(nameof(pilotToUpdate), "The Pilot passed in does not have a valid id."));
            }

            var allPilots = this.cache.Get<List<Pilot>>(PILOTS_CACHE_KEY);
            if (allPilots == null) allPilots = (await GetAll<Pilot>()) ?? new List<Pilot>();

            if (allPilots == null ||
                allPilots.Count < 1 ||
                allPilots.Where(c => c.Id != pilotToUpdate.Id).FirstOrDefault() == null)
            {
                return Error<Pilot>(null, new Exception("A pilot with that name and id could not be found."));
            }

            // Grab all of the pilots w/o the id passed in and save.
            var newPilotList = allPilots.Where(c => c.Id != pilotToUpdate.Id).ToList();
            newPilotList.Add(pilotToUpdate);

            var result = false;

            try
            {
                // Write the new pilot list.
                result = await WriteAll(newPilotList);
                this.cache.Put(PILOTS_CACHE_KEY, newPilotList);  
            }
            catch (Exception ex)
            {
                return Error<Pilot>(null, ex);
            }

            if (!result)
            {
                return Error<Pilot>(null, $"An error occured with updating the pilot with id {pilotToUpdate.Id}.");
            }
            else
            {
                return Success(pilotToUpdate, nameof(UpdateAsync));
            }
        }
    }
}