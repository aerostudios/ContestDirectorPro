using CDP.CoreApp.Interfaces.Contests;
using CDP.ScoringAndSortingImpl.F3K.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDP.ScoringAndSortingImpl.F3K.Tests
{
    [TestClass]
    public class Top10PilotsFlyoffsSortTests
    {
        private IFlyOffSelectionAlgo topTenAlgo = new Top10PilotsFlyoffsSort();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SortSingleRound_BadCurrentScores()
        {
            topTenAlgo.SortSingleRound(null, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SortSingleRound_BadSuggestedPilots()
        {
            topTenAlgo.SortSingleRound(new Dictionary<string, AppDomain.Scoring.PilotContestScoreCollection>(), 14);
        }

        [TestMethod]
        public void SortSingleRound_ValidateProperties()
        {
            Assert.IsNotNull(topTenAlgo.GetUniqueId());
            Assert.IsFalse(string.IsNullOrEmpty(topTenAlgo.GetDescription()));
            Assert.IsFalse(string.IsNullOrEmpty(topTenAlgo.GetSuggestedDisplayName()));
            Assert.IsTrue(topTenAlgo.IsSingleRoundSort());
        }
    }
}
