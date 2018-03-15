//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: PilotQueryInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Pilots.Queries
{
    using CDP.AppDomain;
    using CDP.AppDomain.Pilots;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Pilots;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles all pilot queries for the application
    /// </summary>
    public sealed class PilotQueryInteractor : InteractorBase
    {
        /// <summary>
        /// The pilot repository
        /// </summary>
        private readonly IPilotRepository pilotRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotQueryInteractor"/> class.
        /// </summary>
        /// <param name="pilotRepository">The pilot repository.</param>
        /// <param name="logger">The logger.</param>
        public PilotQueryInteractor(IPilotRepository pilotRepository, ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(pilotRepository, nameof(pilotRepository));
            this.pilotRepository = pilotRepository;
        }

        /// <summary>
        /// Gets all pilots.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<IEnumerable<Pilot>>> GetAllPilotsAsync()
        {
            try
            {
                var result = await this.pilotRepository.ReadAsync();

                if (result.IsFaulted || result.Value.Count() < 0)
                {
                    return Error<IEnumerable<Pilot>>(null, "No Pilots were found.");
                }

                return Success(result.Value, nameof(GetAllPilotsAsync));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<Pilot>>(null, ex);
            }
        }

        /// <summary>
        /// Gets a pilot.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Result<Pilot>> GetPilotAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Error<Pilot>(null, $"{nameof(id)} cannot be null or empty.");
            }

            try
            {
                var result = await this.pilotRepository.ReadAsync(id);

                if (result.IsFaulted || result.Value == null)
                {
                    return Error<Pilot>(null, "No Pilot was found with that id.");
                }

                return Success(result.Value, nameof(GetPilotAsync));
            }
            catch (Exception ex)
            {
                return Error<Pilot>(null, ex);
            }
        }
    }
}