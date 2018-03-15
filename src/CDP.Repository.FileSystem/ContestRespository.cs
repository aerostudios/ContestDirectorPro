//---------------------------------------------------------------
// Created By: Rick Rogahn
// Date: 12/7/2017
// Rights: 
// FileName: ContestRespository.cs
//---------------------------------------------------------------

namespace CDP.Repository.FileSystem
{
    using CDP.AppDomain;
    using CDP.AppDomain.Contests;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Contests;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Data storage manager for Contests.  Stores data in the file system.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.Features.Contests.Commands.IContestRepository" />
    public sealed class ContestRespository : RepositoryBase, IContestRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContestRespository" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="logger">The logger.</param>
        public ContestRespository(string filePath, ILogger logger) : base (filePath, logger) { }

        /// <summary>
        /// Creates a contest.
        /// </summary>
        /// <param name="contestToCreate">The contest to create.</param>
        /// <returns></returns>
        public async Task<Result<Contest>> CreateAsync(Contest contestToCreate)
        {
            if(contestToCreate == null)
            {
                return Error<Contest>(null, new ArgumentNullException(nameof(contestToCreate), "Contest cannot be null."));
            }

            if (!string.IsNullOrEmpty(contestToCreate.Id))
            {
                return Error<Contest>(null, new ArgumentOutOfRangeException(nameof(contestToCreate), "Contest to create already has an id."));
            }
            
            var allContests = (await GetAll<Contest>()) ?? new List<Contest>();
            
            if(allContests == null)
            {
                allContests = new List<Contest>();
            }
            else if (allContests.Count(c => c.Name == contestToCreate.Name) > 0)
            {
                return Error<Contest>(null, new Exception("A contest with this name already exists."));
            }

            // Assign a new GUID for the ID
            contestToCreate.Id = base.GenerateId();
            
            allContests.Add(contestToCreate);

            try
            {
                var success = await WriteAll(allContests);
            }
            catch (Exception ex)
            {
                return Error<Contest>(null, ex);
            }

            return Success((allContests.Where(c => c.Name == contestToCreate.Name)).First(), nameof(CreateAsync));
        }
          
        /// <summary>
        /// Deletes a contest.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Error(false, new ArgumentNullException(nameof(id), "Id cannot be null or empty."));
            }

            var allContests = (await GetAll<Contest>()) ?? new List<Contest>();

            if (allContests == null)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }
            else if (allContests.Count(c => c.Id == id) == 0)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }

            // Grab all of the contests w/o the id passed in and save.
            var contestsToSave = allContests.Where(c => c.Id != id);

            var result = false;

            try
            {
                result = await WriteAll(contestsToSave.ToList());
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }

            return Success(result, nameof(DeleteAsync));
        }

        /// <summary>
        /// Gets a contest by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<Contest>> ReadAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Error<Contest>(null, new ArgumentNullException(nameof(name), "Name cannot be null or empty."));
            }

            var allContests = (await GetAll<Contest>()) ?? new List<Contest>();

            if (allContests == null)
            {
                allContests = new List<Contest>();
            }

            var contestToReturn = allContests.FirstOrDefault(c => c.Name == name);

            if (contestToReturn == null)
            {
                return Error<Contest>(null, new Exception($"A contest with the name {name} could not be found"));
            }

            return Success(contestToReturn, nameof(ReadAsync));
        }

        /// <summary>
        /// Gets all contests.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<IEnumerable<Contest>>> ReadAsync()
        {
            var allContests = (await GetAll<Contest>()) ?? new List<Contest>();

            if (allContests == null)
            {
                allContests = new List<Contest>();
            }
            
            return Success<IEnumerable<Contest>>(allContests, nameof(ReadAsync));
        }

        /// <summary>
        /// Saves the contest.
        /// </summary>
        /// <param name="contestToUpdate">The contest to save.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<Contest>> UpdateAsync(Contest contestToUpdate)
        {
            if (contestToUpdate == null)
            {
                return Error<Contest>(null, new ArgumentNullException(nameof(contestToUpdate), "Contest cannot be null."));
            }

            if (!string.IsNullOrEmpty(contestToUpdate.Id))
            {
                return Error<Contest>(null, new ArgumentOutOfRangeException(nameof(contestToUpdate), "Contest passed in does not have a valid id."));
            }

            var allContests = (await GetAll<Contest>()) ?? new List<Contest>();

            if (allContests == null ||
                allContests.Count < 1 ||
                allContests.Where(c => c.Id != contestToUpdate.Id).FirstOrDefault() == null)
            {
                return Error<Contest>(null, new Exception("A contest with that name and id could not be found."));
            }

            // Grab all of the contests w/o the id passed in and save.
            var newContestList = allContests.Where(c => c.Id != contestToUpdate.Id).ToList();
            newContestList.Add(contestToUpdate);

            var result = false;

            try
            {
                // Write the new contest list.
                result = await WriteAll(newContestList);
            }
            catch (Exception ex)
            {
                return Error<Contest>(null, ex);
            }

            if (!result)
            {
                return Error<Contest>(null, $"An error occured with updating the contest with id {contestToUpdate.Id}.");
            }
            else
            {
                return Success(contestToUpdate, nameof(UpdateAsync));
            }
        }
    }
}