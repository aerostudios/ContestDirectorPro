//---------------------------------------------------------------
// Date: 2/19/2017
// Rights: 
// FileName: SortingAlgoBase.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Sorting
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Registration;
    using CDP.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines behaviors (and base implementation) for a sorting algorhithm. 
    /// </summary>
    public abstract class SortingAlgoBase
    {
        /// <summary>
        /// Sorts the specified pilots.
        /// </summary>
        /// <param name="pilots">The pilots.</param>
        /// <param name="numberOfRounds">The number of rounds.</param>
        /// <param name="suggestedNumberofPilotsInFlightGroup">The suggested numberof pilots in flight group.</param>
        /// <returns>
        /// Flight Matrix for Complete Contest.
        /// </returns>
        public virtual FlightMatrix GenerateInitialMatrix(IEnumerable<PilotRegistration> pilots,
            int numberOfRounds,
            int suggestedNumberofPilotsInFlightGroup)
        {
            Validate.IsEnumerableSizeInRange(pilots, nameof(pilots), 1, int.MaxValue);
            Validate.IsInRange(numberOfRounds, nameof(numberOfRounds), 1, 100);
            Validate.IsInRange(suggestedNumberofPilotsInFlightGroup, nameof(suggestedNumberofPilotsInFlightGroup), 1, 100);

            var pilotsList = new List<PilotRegistration>(pilots);
            var flightMatrixToReturn = new FlightMatrix();
            var remainder = pilotsList.Count % suggestedNumberofPilotsInFlightGroup;
            var numberOfGroups = pilotsList.Count / suggestedNumberofPilotsInFlightGroup;

            // If we have a remainder and that number is less than half of the size of the flight group
            // size, just add the remainder to the last group.  Otherwise, create a new group for the remainder.
            // Also, if there is only 1 left over, don't create an extra group. (Ex: suggested ppg is 2)
            if (remainder > 1 &&
                (((double)suggestedNumberofPilotsInFlightGroup / 2) <= remainder))
            {
                ++numberOfGroups;
            }

            // Loop through each round and build the flight groups
            for (var i = 0; i < numberOfRounds; ++i)
            {
                var group = FlightGroup.A;
                var numberInGroup = 0;

                // Shuffle the pilots each round
                pilotsList = Shuffle(pilotsList).ToList();

                // Create the groups
                foreach (var p in pilotsList)
                {
                    // Increment the groups, but if there are leftover pilots that do not divide
                    // evently, add them to the last group..
                    if (numberInGroup != 0 &&
                        (numberInGroup % suggestedNumberofPilotsInFlightGroup == 0 &&
                        (int)group < numberOfGroups))
                    {
                        group++;
                    }

                    if (!flightMatrixToReturn.Matrix.Any(round => round.RoundOrdinal == i))
                    {
                        flightMatrixToReturn.Matrix.Add(
                            new FlightMatrixRound
                            {
                                RoundOrdinal = i,
                                PilotSlots = new List<FlightMatrixPilotSlot>()
                            });
                    }
                    flightMatrixToReturn.Matrix[i].PilotSlots.Add(
                        new FlightMatrixPilotSlot { PilotId = p.PilotId, FlightGroup = group });
                    ++numberInGroup;
                }
            }

            return flightMatrixToReturn;
        }

        /// <summary>
        /// Shuffles the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        protected static IList<T> Shuffle<T>(IList<T> list)
        {
            Random provider = new Random(DateTime.Now.Millisecond);
            return new List<T>(list.OrderBy(i => provider.Next()));
        }
    }
}
