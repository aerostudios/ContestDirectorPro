//---------------------------------------------------------------
// Created By: Rick Rogahn
// Date: 12/23/2017
// Rights: 
// FileName: RegistrationRepository.cs
//---------------------------------------------------------------

namespace CDP.Repository.FileSystem
{
    using CDP.AppDomain;
    using CDP.AppDomain.Registration;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Registration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an implemntation for the Registration Repository.  Handles saving registration data to the file system.
    /// </summary>
    /// <seealso cref="CDP.Repository.FileSystem.RepositoryBase" />
    /// <seealso cref="CDP.CoreApp.Interfaces.Registration.IPilotRegistrationRepository" />
    public sealed class RegistrationRepository : RepositoryBase, IPilotRegistrationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationRepository" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">filePath - filePath</exception>
        public RegistrationRepository(string filePath, ILogger logger) : base(filePath, logger) { }

        /// <summary>
        /// Registers the pilots for contest.
        /// </summary>
        /// <param name="pilotsToRegister">The pilots.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<PilotRegistration>>> CreateAsync(IEnumerable<PilotRegistration> pilotsToRegister)
        {
            if (pilotsToRegister?.Count() < 1)
            {
                return Error<IEnumerable<PilotRegistration>>(null, $"{nameof(pilotsToRegister)} cannot be null.");
            }

            if (pilotsToRegister.Any(x => !string.IsNullOrEmpty(x.Id)))
            {
                return Error<IEnumerable<PilotRegistration>>(null, "One of the registration Id's already has a value.");
            }

            var allPilotRegistrations = (await GetAll<PilotRegistration>())?.ToList() ?? new List<PilotRegistration>();

            if (allPilotRegistrations == null)
            {
                allPilotRegistrations = new List<PilotRegistration>();
            }

            // Assign a new GUID to the ID of each registration.
            foreach (var pr in pilotsToRegister)
            {
                pr.Id = base.GenerateId();
            }

            allPilotRegistrations.AddRange(pilotsToRegister);

            try
            {
                var success = await WriteAll(allPilotRegistrations);
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<PilotRegistration>>(null, ex);
            }

            return Success(allPilotRegistrations as IEnumerable<PilotRegistration>, nameof(CreateAsync));
        }

        /// <summary>
        /// Registers the single pilot for a contest.
        /// </summary>
        /// <param name="pilotToRegister">The pilot reg.</param>
        /// <returns></returns>
        public async Task<Result<PilotRegistration>> CreateAsync(PilotRegistration pilotToRegister)
        {
            if (pilotToRegister == null)
            {
                return Error<PilotRegistration>(null, $"{nameof(pilotToRegister)} cannot be null");
            }

            if (!string.IsNullOrEmpty(pilotToRegister.Id))
            {
                return Error<PilotRegistration>(null, "The Id already has a value");
            }

            var allPilotRegistrations = (await GetAll<PilotRegistration>())?.ToList() ?? new List<PilotRegistration>();

            if (allPilotRegistrations == null)
            {
                allPilotRegistrations = new List<PilotRegistration>();
            }

            // Assign a new GUID for the ID
            pilotToRegister.Id = Guid.NewGuid().ToString();

            allPilotRegistrations.Add(pilotToRegister);

            try
            {
                var success = await WriteAll(allPilotRegistrations);
            }
            catch (Exception ex)
            {
                return Error<PilotRegistration>(null, ex);
            }

            return Success((allPilotRegistrations.Where(p => p.Id == pilotToRegister.Id)).First(), nameof(CreateAsync));
        } 

        /// <summary>
        /// Unregisters pilots for a contest.
        /// </summary>
        /// <param name="pilotsToUnregister"></param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAsync(IEnumerable<PilotRegistration> pilotsToUnregister)
        {
            if (pilotsToUnregister?.Count() < 1)
            {
                return Error(false, $"{nameof(pilotsToUnregister)} cannot be null.");
            }

            var allPilotRegistrations = (await GetAll<PilotRegistration>()) ?? new List<PilotRegistration>();
            var registrationsToSave = new List<PilotRegistration>();

            foreach (var pr in pilotsToUnregister)
            {
                if (allPilotRegistrations == null)
                {
                    // It's not there, so no-op and we'll call it a success.
                    break;
                }

                // Grab all of the pilot registrations w/o the id to unregister.
                registrationsToSave = allPilotRegistrations.Where(p => p.Id != pr.Id).ToList();
            }

            var result = false;

            try
            {
                result = await WriteAll(registrationsToSave.ToList());
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }

            return Success(result, nameof(DeleteAsync));
        }

        /// <summary>
        /// Unregisters pilot for a contest.
        /// </summary>
        /// <param name="id">The pilot to unregister.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Error(false, new ArgumentNullException(nameof(id), "Id cannot be null or empty."));
            }

            var allPilotRegistrations = (await GetAll<PilotRegistration>()) ?? new List<PilotRegistration>();

            if (allPilotRegistrations == null)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }
            else if (allPilotRegistrations.Count(p => p.Id == id) == 0)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }

            // Grab all of the Pilots w/o the id passed in and save.
            var pilotsToSave = allPilotRegistrations.Where(p => p.Id != id);

            var result = false;

            try
            {
                result = await WriteAll(pilotsToSave.ToList());
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }

            return Success(result, nameof(DeleteAsync));
        }

        /// <summary>
        /// Gets the pilot registrations for a contest.
        /// </summary>
        /// <param name="contestId"></param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<PilotRegistration>>> ReadAsync(string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<IEnumerable<PilotRegistration>>(null, new ArgumentNullException(nameof(contestId), $"{nameof(contestId)} cannot be null or empty."));
            }

            var allPilotRegistrations = (await GetAll<PilotRegistration>()) ?? new List<PilotRegistration>();

            if (allPilotRegistrations == null)
            {
                allPilotRegistrations = new List<PilotRegistration>();
            }

            var registrationsToReturn = allPilotRegistrations.Where(p => p.ContestId == contestId);

            if (registrationsToReturn == null)
            {
                return Error<IEnumerable<PilotRegistration>>(null, new Exception($"A pilot with the id {contestId} could not be found"));
            }

            return Success(registrationsToReturn, nameof(ReadAsync));
        }
    }
}