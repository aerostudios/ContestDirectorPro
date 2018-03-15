//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: FlightMatrixRepository.cs
//---------------------------------------------------------------

namespace CDP.Repository.WindowsStorage
{
    using CDP.AppDomain;
    using CDP.AppDomain.FlightMatrices;
    using CDP.Common.Caching;
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
        /// The flight matrix cache key
        /// </summary>
        private const string FLIGHT_MATRIX_CACHE_KEY = "flightmatrix";

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixRepository"/> class.
        /// </summary>
        /// <param name="fileName">The file path.</param>
        /// <param name="logger">The logger.</param>
        public FlightMatrixRepository(string fileName, ICache cache, ILoggingService logger) : base(fileName, cache, logger) { }

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

            // Check cache
            var allFlightMatricies = this.cache.Get<List<FlightMatrix>>(FLIGHT_MATRIX_CACHE_KEY);
            if (allFlightMatricies == null) allFlightMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

            if (allFlightMatricies == null)
            {
                allFlightMatricies = new List<FlightMatrix>();
            }

            // Assign a new GUID for the ID
            flightMatrixToCreate.Id = base.GenerateId();

            // Remove any that exist for the contest this is intended for
            allFlightMatricies = allFlightMatricies.Where(matrix => matrix.ContestId != flightMatrixToCreate.ContestId).ToList();

            allFlightMatricies.Add(flightMatrixToCreate);

            try
            {
                var success = await WriteAll(allFlightMatricies);
                this.cache.Put(FLIGHT_MATRIX_CACHE_KEY, allFlightMatricies);
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

            // Check cache
            var allFlightMatricies = this.cache.Get<List<FlightMatrix>>(FLIGHT_MATRIX_CACHE_KEY);
            if(allFlightMatricies == null) allFlightMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

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
            var allMatrices = allFlightMatricies.Where(fmx => fmx.Id != flightMatrixId);

            var result = false;

            try
            {
                var matrixList = allMatrices.ToList();
                result = await WriteAll(matrixList);
                this.cache.Put(FLIGHT_MATRIX_CACHE_KEY, matrixList);
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
        /// <param name="contestId">The flight matrix identifier.</param>
        /// <returns></returns>
        public async Task<Result<FlightMatrix>> ReadAsync(string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<FlightMatrix>(null, new ArgumentNullException(nameof(contestId), "Id cannot be null or empty."));
            }

            // Check cache
            var allMatricies = this.cache.Get<List<FlightMatrix>>(FLIGHT_MATRIX_CACHE_KEY);
            if (allMatricies == null) allMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

            if (allMatricies == null)
            {
                allMatricies = new List<FlightMatrix>();
            }

            var matrixToReturn = allMatricies.FirstOrDefault(m => m.ContestId == contestId);

            if (matrixToReturn == null)
            {
                return Error<FlightMatrix>(null, new Exception($"A flight matrix with the id {contestId} could not be found"));
            }

            return Success(matrixToReturn, nameof(ReadAsync));
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

            if (string.IsNullOrEmpty(flightMatrix.Id))
            {
                return Error<FlightMatrix>(null, new ArgumentOutOfRangeException(nameof(flightMatrix), "The Pilot passed in does not have a valid id."));
            }

            // Check cache
            var allMatricies = this.cache.Get<List<FlightMatrix>>(FLIGHT_MATRIX_CACHE_KEY);
            if (allMatricies == null) allMatricies = (await GetAll<FlightMatrix>()) ?? new List<FlightMatrix>();

            if (allMatricies == null ||
                allMatricies.Count < 1 ||
                allMatricies.Where(matrix => matrix.Id != flightMatrix.Id).FirstOrDefault() == null)
            {
                return Error<FlightMatrix>(null, new Exception("A flight matrix with that name and id could not be found."));
            }
            
            // Clear out any existing matrices with the same id or contest id.
            var newFlightMatrixList = allMatricies.Where(
                matrix => matrix.Id != flightMatrix.Id && 
                matrix.ContestId != flightMatrix.ContestId).ToList();

            newFlightMatrixList.Add(flightMatrix);

            var result = false;

            try
            {
                // Write the new pilot list.
                result = await WriteAll<FlightMatrix>(newFlightMatrixList);
                this.cache.Put(FLIGHT_MATRIX_CACHE_KEY, newFlightMatrixList);
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