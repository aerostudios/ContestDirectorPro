//---------------------------------------------------------------
// Date: 12/21/2017
// Rights: 
// FileName: ScoringRepository.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.FileStoreRepository
{
    using CDP.AppDomain;
    using CDP.AppDomain.Scoring;
    using CDP.Common.Caching;
    using CDP.Common.Logging;
    using CDP.CoreApp.Interfaces.Scoring;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles data persistance for scoring.  This implementation uses the file system for storage.
    /// </summary>
    /// <seealso cref="CDP.Repository.FileSystem.RepositoryBase" />
    /// <seealso cref="CDP.CoreApp.Interfaces.Scoring.IScoringRepository" />
    public sealed class ScoringRepository : RepositoryBase, IScoringRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScoringRepository"/> class.
        /// </summary>
        /// <param name="fileName">The file path.</param>
        /// <param name="logger">The logger.</param>
        public ScoringRepository(string fileName, ICache cache, ILoggingService logger) : base(fileName, cache, logger) { }

        /// <summary>
        /// Writes the specified pilot round scores to the file.
        /// </summary>
        /// <param name="timeSheets">The pilot round scores.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<TimeSheet>>> CreateAsync(IEnumerable<TimeSheet> timeSheets)
        {
            if (timeSheets?.Count() < 1)
            {
                return Error<IEnumerable<TimeSheet>>(null, $"{nameof(timeSheets)} cannot be null.");
            }

            if (timeSheets.Any(x => !string.IsNullOrEmpty(x.Id)))
            {
                return Error<IEnumerable<TimeSheet>>(null, "One of the TimeSheet Id's already has a value.");
            }

            var allScores = (await GetAll<TimeSheet>())?.ToList() ?? new List<TimeSheet>();

            if (allScores == null)
            {
                allScores = new List<TimeSheet>();
            }

            // Assign a new GUID to the ID of each registration.
            foreach (var pr in timeSheets)
            {
                pr.Id = base.GenerateId();
            }

            allScores.AddRange(timeSheets);

            try
            {
                var success = await WriteAll(allScores);
            }
            catch (Exception ex)
            {
                return Error<IEnumerable<TimeSheet>>(null, ex);
            }

            return Success(allScores as IEnumerable<TimeSheet>, nameof(CreateAsync));
        }

        /// <summary>
        /// Deletes the pilotRoundScore from the file.
        /// </summary>
        /// <param name="pilotRoundScore">The pilot round score.</param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAsync(TimeSheet pilotRoundScore)
        {
            if (pilotRoundScore == null)
            {
                return Error(false, $"{nameof(pilotRoundScore)} cannot be null.");
            }

            if (string.IsNullOrEmpty(pilotRoundScore.ContestId))
            {
                return Error(false, $"{nameof(pilotRoundScore)} {nameof(pilotRoundScore.ContestId)} cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(pilotRoundScore.PilotId))
            {
                return Error(false, $"{nameof(pilotRoundScore)} {nameof(pilotRoundScore.PilotId)} cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(pilotRoundScore.Id))
            {
                return Error(false, $"{nameof(pilotRoundScore)}'s Id cannot be null or empty");
            }

            var allTimeSheets = (await GetAll<TimeSheet>()) ?? new List<TimeSheet>();
            var registrationsToSave = new List<TimeSheet>();


            if (allTimeSheets == null)
            {
                // It's not there, so no-op and we'll call it a success.
                return Success(true, nameof(DeleteAsync));
            }

            // Grab the timesheet that matches the id to delete.
            registrationsToSave = allTimeSheets.Where(ts => ts.Id != pilotRoundScore.Id).ToList();


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
        /// Gets the specified contest scores for all rounds from the file.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <returns>Dictionary of timesheets.  Key is the round ordinal, value is an enumeration of timesheets for that round.</returns>
        public async Task<Result<ContestScoresCollection>> ReadAsync(string contestId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<ContestScoresCollection>(null, $"{nameof(contestId)} cannot be null or empty.");
            }

            var allScores = (await GetAll<TimeSheet>()) ?? new List<TimeSheet>();

            if (allScores == null)
            {
                allScores = new List<TimeSheet>();
            }

            var scoresToReturn = allScores.Where(p => p.ContestId == contestId).GroupBy(score => score.RoundOrdinal);

            if (scoresToReturn == null)
            {
                return Error<ContestScoresCollection>(null, new Exception($"A pilot with the id {contestId} could not be found"));
            }

            var returnObj = new ContestScoresCollection();

            foreach (var scoreGroup in scoresToReturn)
            {
                var pilotTimeSheets = new List<TimeSheet>();

                foreach (var score in scoreGroup)
                {
                    pilotTimeSheets.Add(score);
                }

                var roundOrdinal = scoreGroup.First()?.RoundOrdinal ?? 0;

                if (!returnObj.Rounds.Contains(roundOrdinal))
                {
                    returnObj.Add(new ContestRoundScoresCollection(pilotTimeSheets, roundOrdinal));
                }
            }

            return Success(returnObj, nameof(ReadAsync));
        }

        /// <summary>
        /// Gets the specified round for a contest from the file. 
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="roundOrdinal">The round ordinal.</param>
        /// <returns></returns>
        public async Task<Result<IEnumerable<TimeSheet>>> ReadAsync(string contestId, int roundOrdinal)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<IEnumerable<TimeSheet>>(null, $"{nameof(contestId)} cannot be null or empty.");
            }

            if (roundOrdinal < 0)
            {
                return Error<IEnumerable<TimeSheet>>(null, $"{nameof(roundOrdinal)} cannot be less than 0.");
            }

            var allScores = (await GetAll<TimeSheet>()) ?? new List<TimeSheet>();

            if (allScores == null)
            {
                allScores = new List<TimeSheet>();
            }

            var scoresToReturn = allScores.Where(s => s.ContestId == contestId && s.RoundOrdinal == roundOrdinal);

            if (scoresToReturn == null)
            {
                return Error<IEnumerable<TimeSheet>>(null, new Exception($"A collection of timesheets could not be found for {contestId}."));
            }

            return Success(scoresToReturn, nameof(ReadAsync));
        }

        /// <summary>
        /// Gets the specified pilot score for a round in a contest from the file.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="roundOrdinal">The round ordinal.</param>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <returns></returns>
        public async Task<Result<TimeSheet>> ReadAsync(string contestId, int roundOrdinal, string pilotId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Error<TimeSheet>(null, $"{nameof(contestId)} cannot be null or empty.");
            }

            if (roundOrdinal < 0)
            {
                return Error<TimeSheet>(null, $"{nameof(roundOrdinal)} cannot be less than 0.");
            }

            if (string.IsNullOrEmpty(pilotId))
            {
                return Error<TimeSheet>(null, $"{nameof(pilotId)} cannot be null or empty.");
            }

            var allScores = (await GetAll<TimeSheet>()) ?? new List<TimeSheet>();

            if (allScores == null)
            {
                allScores = new List<TimeSheet>();
            }

            var scoreToReturn = allScores.Where(
                s => s.ContestId == contestId
                && s.RoundOrdinal == roundOrdinal
                && s.PilotId == pilotId).FirstOrDefault();

            if (scoreToReturn == null)
            {
                return Error<TimeSheet>(null, new Exception($"A timesheet could not be found."));
            }

            return Success(scoreToReturn, nameof(ReadAsync));
        }

        /// <summary>
        /// Updates a score in the file.
        /// </summary>
        /// <param name="timeSheetToUpdate">The time sheet to update.</param>
        /// <returns></returns>
        public async Task<Result<TimeSheet>> UpdateAsync(TimeSheet timeSheetToUpdate)
        {
            if (timeSheetToUpdate == null)
            {
                return Error<TimeSheet>(null, new ArgumentNullException(nameof(timeSheetToUpdate), $"{nameof(timeSheetToUpdate)} cannot be null."));
            }

            if (!string.IsNullOrEmpty(timeSheetToUpdate.Id))
            {
                return Error<TimeSheet>(null, new ArgumentOutOfRangeException(nameof(timeSheetToUpdate), "The time sheet passed in does not have a valid id."));
            }

            var allTimeSheets = (await GetAll<TimeSheet>()) ?? new List<TimeSheet>();

            if (allTimeSheets == null ||
                allTimeSheets.Count < 1 ||
                allTimeSheets.Where(c => c.Id != timeSheetToUpdate.Id).FirstOrDefault() == null)
            {
                return Error<TimeSheet>(null, new Exception("A time sheet with that name and id could not be found."));
            }

            // Grab all of the time sheets w/o the id passed in.
            var newPilotList = allTimeSheets.Where(c => c.Id != timeSheetToUpdate.Id).ToList();
            newPilotList.Add(timeSheetToUpdate);

            var result = false;

            try
            {
                // Write the new pilot list.
                result = await WriteAll(newPilotList);
            }
            catch (Exception ex)
            {
                return Error<TimeSheet>(null, ex);
            }

            if (!result)
            {
                return Error<TimeSheet>(null, $"An error occured when updating the time sheet with id {timeSheetToUpdate.Id}.");
            }
            else
            {
                return Success(timeSheetToUpdate, nameof(UpdateAsync));
            }
        }
    }
}