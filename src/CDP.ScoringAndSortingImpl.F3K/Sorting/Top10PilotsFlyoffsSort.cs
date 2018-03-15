//---------------------------------------------------------------
// Date: 3/9/2018
// Rights: 
// FileName: Top10PilotsFlyoffsSort.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Sorting
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Registration;
    using CDP.AppDomain.Scoring;
    using CDP.CoreApp.Interfaces.Contests;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Top10PilotsFlyoffsSort : IFlyOffSelectionAlgo
    {
        /// <summary>
        /// Sorts the specified pilots and generates a flight matrix.
        /// </summary>
        /// <param name="pilots">The pilots.</param>
        /// <param name="numberOfRounds">The number of rounds.</param>
        /// <param name="suggestedNumberofPilotsInFlightGroup">The suggested numberof pilots in flight group.</param>
        /// <returns>
        /// Flight Matrix for Complete Contest.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public FlightMatrix GenerateInitialMatrix(IEnumerable<PilotRegistration> pilots, int numberOfRounds, int suggestedNumberofPilotsInFlightGroup)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        public string GetDescription() => "This sorting method for flyoffs selects the top 10 pilots.";

        /// <summary>
        /// Gets the display name of the suggested.
        /// </summary>
        /// <returns></returns>
        public string GetSuggestedDisplayName() => "Top 10 Pilots";

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <returns></returns>
        public string GetUniqueId() => "579f6752-d620-4ebf-ad85-5d6f17fc1f60";

        /// <summary>
        /// Determines whether [is single round sort].
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is single round sort]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSingleRoundSort() => true;

        /// <summary>
        /// Sorts the single round.
        /// </summary>
        /// <param name="currentScores">The current scores.</param>
        /// <param name="suggestedNumberOfPilotsInFlightGroup">The suggested number of pilots in flight group.</param>
        /// <returns>
        /// Dictionary of Pilot ID keys w/ FlightGroup Values.
        /// </returns>
        /// <exception cref="ArgumentNullException">currentScores</exception>
        /// <exception cref="ArgumentOutOfRangeException">suggestedNumberOfPilotsInFlightGroup</exception>
        public Dictionary<string, FlightGroup> SortSingleRound(Dictionary<string, PilotContestScoreCollection> currentScores, int suggestedNumberOfPilotsInFlightGroup = 10)
        {
            if (currentScores == null)
            {
                throw new ArgumentNullException($"{nameof(currentScores)} cannot be null or empty.");
            }

            if (suggestedNumberOfPilotsInFlightGroup != 10)
            {
                throw new ArgumentOutOfRangeException($"{nameof(suggestedNumberOfPilotsInFlightGroup)} is out of range.  Must be 10.");
            }

            var sortedPilots = new Dictionary<string, FlightGroup>();
            var sortedPilotIds = currentScores.OrderByDescending(pilotScoreSet => pilotScoreSet.Value.TotalScore).Select(r => r.Key).ToList();
            var cnt = 0;

            foreach (var pilotId in sortedPilotIds)
            {
                if (++cnt == 10) break;
                sortedPilots.Add(pilotId, FlightGroup.A);
            }

            return sortedPilots;
        }
    }
}
