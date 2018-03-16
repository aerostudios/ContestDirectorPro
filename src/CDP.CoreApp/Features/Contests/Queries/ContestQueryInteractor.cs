//---------------------------------------------------------------
// Date: 12/7/2017
// Rights: 
// FileName: ContestQueryInteractor.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Contests.Queries
{
    using AppDomain.Contests;
    using CDP.AppDomain;
    using CDP.Common;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Contests;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles queries for contests
    /// </summary>
    /// <seealso cref="CDP.CoreApp.InteractorBase" />
    public sealed class ContestQueryInteractor : InteractorBase
    {
        /// <summary>
        /// The contest repository
        /// </summary>
        private readonly IContestRepository contestRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContestQueryInteractor"/> class.
        /// </summary>
        /// <param name="contestRepository">The contest repository.</param>
        /// <param name="logger">The logger.</param>
        public ContestQueryInteractor(IContestRepository contestRepository,
            ILoggingService logger) : base(logger)
        {
            Validate.IsNotNull(contestRepository, nameof(contestRepository));
            this.contestRepository = contestRepository;
        }

        /// <summary>
        /// Gets the contest by name.
        /// </summary>
        /// <param name="contestName">Name of the contest.</param>
        /// <returns></returns>
        public async Task<Result<Contest>> GetContestByNameAsync(string contestName)
        {
            Validate.IsNotNullOrEmpty(contestName, nameof(contestName));

            try
            {
                var result = await this.contestRepository.ReadAsync(contestName);

                if (result.IsFaulted)
                {
                    return Error<Contest>(null, "No contest was found with that name.");
                }

                return Success(result.Value, nameof(GetContestByNameAsync));
            }
            catch (Exception ex)
            {
                return Error<Contest>(null, ex);
            }
        }

        /// <summary>
        /// Gets all contests.
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IEnumerable<Contest>>> GetAllContestsAsync()
        {
            try
            {
                var result = await this.contestRepository.ReadAsync();

                if (result.IsFaulted || result.Value.Count() < 0)
                {
                    return Error<IEnumerable<Contest>>(null, "No contests were found.");
                }

                // Order most recent to oldest
                var orderedResult = result.Value;
                orderedResult = result.Value.OrderByDescending(c => c.StartDate); 
                
                return Success(orderedResult, nameof(GetAllContestsAsync));
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<Contest>>(null, ex);
            }
        }
    }
}
