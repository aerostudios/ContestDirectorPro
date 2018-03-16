﻿//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: IFlyOffSelectionAlgo.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Contests
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Scoring;
    using System.Collections.Generic;

    public interface IFlyOffSelectionAlgo
    {
        /// <summary>
        /// Sorts the single round.
        /// </summary>
        /// <param name="currentScores">The current scores.</param>
        /// <param name="suggestedNumberOfPilotsInFlightGroup">The suggested number of pilots in flight group.</param>
        /// <returns>
        /// Dictionary of Pilot ID keys w/ FlightGroup Values.
        /// </returns>
        Dictionary<string, FlightGroup> SortSingleRound(Dictionary<string, PilotContestScoreCollection> currentScores, int suggestedNumberOfPilotsInFlightGroup);

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <returns></returns>
        string GetUniqueId();

        /// <summary>
        /// Gets the display name of the suggested.
        /// </summary>
        /// <returns></returns>
        string GetSuggestedDisplayName();

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        string GetDescription();

        /// <summary>
        /// Determines whether [is single round sort].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is single round sort]; otherwise, <c>false</c>.
        /// </returns>
        bool IsSingleRoundSort();
    }
}