//---------------------------------------------------------------
// Created By: Rick Rogahn
// Date: 12/7/2017
// Rights: 
// FileName: FlightMatrixRepository.cs
//---------------------------------------------------------------

namespace CDP.Repository.FileSystem
{
    using CDP.AppDomain;
    using CDP.AppDomain.FlightMatrices;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.FlightMatrices;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles flight matrix data persitance.  This implementation writes all data to the file system.
    /// </summary>
    /// <seealso cref="CDP.Repository.FileSystem.RepositoryBase" />
    /// <seealso cref="CDP.CoreApp.Interfaces.FlightMatrices.IFlightMatrixRepository" />
    public sealed class FlightMatrixRepository : RepositoryBase, IFlightMatrixRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixRepository"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="logger">The logger.</param>
        public FlightMatrixRepository(string filePath, ILogger logger) : base(filePath, logger) { }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="flightMatrixToCreate">The flight matrix to create.</param>
        /// <returns></returns>
        public async Task<Result<FlightMatrix>> CreateAsync(FlightMatrix flightMatrixToCreate)
        {
            if (flightMatrixToCreate == null)
            {
                return Error<FlightMatrix>(null, "The Pilot cannot be null");
            }

            if (!string.IsNullOrEmpty(flightMatrixToCreate.Id))
            {
                return Error<FlightMatrix>(null, "The Id already has a value");
            }

            var allFlightMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

            if (allFlightMatricies == null)
            {
                allFlightMatricies = new List<FlightMatrix>();
            }

            // Assign a new GUID for the ID
            flightMatrixToCreate.Id = base.GenerateId();

            allFlightMatricies.Add(flightMatrixToCreate);

            try
            {
                var success = await WriteAll<FlightMatrix>(allFlightMatricies);
            }
            catch (Exception ex)
            {
                return Error<FlightMatrix>(null, ex);
            }

            return Success((allFlightMatricies.Where(p => p.Id == flightMatrixToCreate.Id)).First(), nameof(CreateAsync));
        }

        /// <summary>
        /// Deletes the flight matrix.
        /// </summary>
        /// <param name="flightMatrixId">The contest identifier.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAsync(string flightMatrixId)
        {
            if (string.IsNullOrEmpty(flightMatrixId))
            {
                return Error(false, new ArgumentNullException(nameof(flightMatrixId), "Id cannot be null or empty."));
            }

            var allFlightMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

            if (allFlightMatricies == null)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }
            else if (allFlightMatricies.Count(fmx => fmx.Id == flightMatrixId) == 0)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }

            // Grab all of the Pilots w/o the id passed in and save.
            var pilotsToSave = allFlightMatricies.Where(fmx => fmx.Id != flightMatrixId);

            var result = false;

            try
            {
                result = await WriteAll<FlightMatrix>(pilotsToSave.ToList());
            }
            catch (Exception ex)
            {
                return Error(false, ex);
            }

            return Success(result, nameof(DeleteAsync));
        }

        /// <summary>
        /// Reads the asynchronous.
        /// </summary>
        /// <param name="flightMatrixId">The flight matrix identifier.</param>
        /// <returns></returns>
        public async Task<Result<FlightMatrix>> ReadAsync(string flightMatrixId)
        {
            if (string.IsNullOrEmpty(flightMatrixId))
            {
                return Error<FlightMatrix>(null, new ArgumentNullException(nameof(flightMatrixId), "Id cannot be null or empty."));
            }

            var allMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

            if (allMatricies == null)
            {
                allMatricies = new List<FlightMatrix>();
            }

            var pilotToReturn = allMatricies.FirstOrDefault(p => p.Id == flightMatrixId);

            if (pilotToReturn == null)
            {
                return Error<FlightMatrix>(null, new Exception($"A flight matrix with the id {flightMatrixId} could not be found"));
            }

            return Success(pilotToReturn, nameof(ReadAsync));
        }

        /// <summary>
        /// Updates the specified flight matrix.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <returns></returns>
        public async Task<Result<FlightMatrix>> UpdateAsync(FlightMatrix flightMatrix)
        {
            if (flightMatrix == null)
            {
                return Error<FlightMatrix>(null, new ArgumentNullException(nameof(flightMatrix), "Pilot cannot be null."));
            }

            if (!string.IsNullOrEmpty(flightMatrix.Id))
            {
                return Error<FlightMatrix>(null, new ArgumentOutOfRangeException(nameof(flightMatrix), "The Pilot passed in does not have a valid id."));
            }

            var allMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

            if (allMatricies == null ||
                allMatricies.Count < 1 ||
                allMatricies.Where(c => c.Id != flightMatrix.Id).FirstOrDefault() == null)
            {
                return Error<FlightMatrix>(null, new Exception("A flight matrix with that name and id could not be found."));
            }

            // Grab all of the pilots w/o the id passed in and save.
            var newFlightMatrixList = allMatricies.Where(c => c.Id != flightMatrix.Id).ToList();
            newFlightMatrixList.Add(flightMatrix);

            var result = false;

            try
            {
                // Write the new pilot list.
                result = await WriteAll<FlightMatrix>(newFlightMatrixList);
            }
            catch (Exception ex)
            {
                return Error<FlightMatrix>(null, ex);
            }

            if (!result)
            {
                return Error<FlightMatrix>(null, $"An error occured with updating the flight matrix with id {flightMatrix.Id}.");
            }
            else
            {
                return Success(flightMatrix, nameof(UpdateAsync));
            }
        }
    }
}