//---------------------------------------------------------------
// Date: 2/19/2017
// Rights: 
// FileName: RandomSortNoTeamProtection.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Sorting
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Scoring;
    using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a class that will randomly sort the pilots into flight groups for the 
    /// entire contest.  NO TEAM PROTECTION!!! For those who feel that F3K should be 
    /// about the pilot's own skills and should have to fly against everyone.
    /// </summary>
    public sealed class RandomSortNoTeamProtection : SortingAlgoBase, ISortingAlgo
    {
        #region Public Methods

        #region IMatrixSortingAlgo Implementation

        /// <summary>
        /// Sorts the single round.
        /// </summary>
        /// <param name="currentScores">The current scores.</param>
        /// <param name="suggestedNumberOfPilotsInFlightGroup">The suggested number of pilots in flight group.</param>
        /// <returns>
        /// Dictionary of Pilot ID keys w/ FlightGroup Values.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public Dictionary<string, FlightGroup> SortSingleRound(Dictionary<string, PilotContestScoreCollection> currentScores, int suggestedNumberOfPilotsInFlightGroup)
        {
            // Not an option for this type of Algo.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the suggested display name.
        /// </summary>
        /// <returns></returns>
        public string GetSuggestedDisplayName() => "Random Sort, no team protection.";

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        public string GetDescription() => "Pilots are randomly sorted for the entire contest before the contest starts.  No team protection, pilots cannot pick who they get to fly against.";

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <returns></returns>
        public string GetUniqueId() => "798743a0-7d7d-4f8c-9cee-9fa74c7c9972";

        /// <summary>
        /// Determines whether [is single round sort].
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is single round sort]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSingleRoundSort() => false;

        #endregion

        #endregion

        #region Private Methods

        #endregion
    }
}
