//---------------------------------------------------------------
// Date: 12/11/2017
// Rights: 
// FileName: IScoringRepository.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Scoring
{
    using CDP.AppDomain;
    using CDP.AppDomain.Scoring;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a data storage interface for scores.
    /// </summary>
    public interface IScoringRepository
    {
        /// <summary>
        /// Gets the specified contest scores for all rounds.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns></returns>
        Task<Result<ContestScoresCollection>> ReadAsync(string contestId);

        /// <summary>
        /// Gets the specified round.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="roundOrdinal">The round ordinal.</param>
        /// <returns></returns>
        Task<Result<IEnumerable<TimeSheet>>> ReadAsync(string contestId, int roundOrdinal);

        /// <summary>
        /// Gets the specified pilot.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="roundOrdinal">The round ordinal.</param>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <returns></returns>
        Task<Result<TimeSheet>> ReadAsync(string contestId, int roundOrdinal, string pilotId);

        /// <summary>
        /// Writes the specified pilot round scores.
        /// </summary>
        /// <param name="timeSheets">The pilot round scores.</param>
        /// <returns></returns>
        Task<Result<IEnumerable<TimeSheet>>> CreateAsync(IEnumerable<TimeSheet> timeSheets);

        /// <summary>
        /// Updates the specified pilot round score.
        /// </summary>
        /// <param name="timeSheet">The pilot round score.</param>
        /// <returns></returns>
        Task<Result<TimeSheet>> UpdateAsync(TimeSheet timeSheet);

        /// <summary>
        /// Deletes the specified pilot round score.
        /// </summary>
        /// <param name="timeSheet">The pilot round score.</param>
        /// <returns></returns>
        Task<Result<bool>> DeleteAsync(TimeSheet timeSheet);
    }
}
