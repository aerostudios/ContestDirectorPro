//---------------------------------------------------------------
// Date: 12/20/2017
// Rights: 
// FileName: MoMSingleRoundSort.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Sorting
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Registration;
    using CDP.AppDomain.Scoring;
    using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class MoMSingleRoundSort : SortingAlgoBase, ISortingAlgo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoMSingleRoundSort" /> class.
        /// </summary>
        public MoMSingleRoundSort() { }

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
        public override FlightMatrix GenerateInitialMatrix(IEnumerable<PilotRegistration> pilots, int numberOfRounds, int suggestedNumberofPilotsInFlightGroup)
        {
            // Override to only generate a single round
            return base.GenerateInitialMatrix(pilots, 1, suggestedNumberofPilotsInFlightGroup);
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <returns></returns>
        public string GetUniqueId() => "48687d77-fdd7-42af-bb54-a9938b0b3ddc";

        /// <summary>
        /// Gets the suggested display name.
        /// </summary>
        /// <returns></returns>
        public string GetSuggestedDisplayName() => "Seeded man on man.";

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        public string GetDescription() => "Pilots are randomly sorted for the first round.  After that, they are sorted based on their current contest total score.";

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
        /// <param name="suggestedNumberOfPilotsInFlightGroup">The number of groups.</param>
        /// <returns>
        /// Dictionary of Pilot ID keys w/ FlightGroup Values.
        /// </returns>
        /// <exception cref="ArgumentNullException">numberOfGroups</exception>
        /// <exception cref="ArgumentOutOfRangeException">numberOfGroups</exception>
        public Dictionary<string, FlightGroup> SortSingleRound(Dictionary<string, PilotContestScoreCollection> currentScores, int suggestedNumberOfPilotsInFlightGroup)
        {
            if (currentScores == null)
            {
                throw new ArgumentNullException($"{nameof(currentScores)} cannot be null or empty.");
            }

            if (suggestedNumberOfPilotsInFlightGroup < 1)
            {
                throw new ArgumentOutOfRangeException($"{nameof(suggestedNumberOfPilotsInFlightGroup)} is out of range.  Must be greater than Zero.");
            }

            var sortedPilots = new Dictionary<string, FlightGroup>();
            var sortedPilotIds = currentScores.OrderByDescending(pilotScoreSet => pilotScoreSet.Value.TotalScore).Select(r => r.Key).ToList();

            var remainder = sortedPilotIds.Count % suggestedNumberOfPilotsInFlightGroup;
            var numberOfGroups = sortedPilotIds.Count / suggestedNumberOfPilotsInFlightGroup;

            // If we have a remainder and that number is less than half of the size of the flight group
            // size, just add the remainder to the last group.  Otherwise, create a new group for the remainder.
            // Also, if there is only 1 left over, don't create an extra group. (Ex: suggested ppg is 2)
            if (remainder > 1 &&
                (((double)suggestedNumberOfPilotsInFlightGroup / 2) <= remainder))
            {
                ++numberOfGroups;
            }

            var numberInGroup = 0;
            var flightGroup = FlightGroup.A;
            
            foreach (var pilotId in sortedPilotIds)
            {
                // Increment the groups, but if there are leftover pilots that do not divide
                // evently, add them to the last group..
                if (numberInGroup != 0 &&
                    (numberInGroup % suggestedNumberOfPilotsInFlightGroup == 0 &&
                    (int)flightGroup < numberOfGroups))
                {
                    ++flightGroup;
                }

                sortedPilots.Add(pilotId, flightGroup);
                ++numberInGroup;
            }

            return sortedPilots;
        }
    }
}
