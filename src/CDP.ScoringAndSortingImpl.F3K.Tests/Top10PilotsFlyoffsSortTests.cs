using CDP.AppDomain.Scoring;
using CDP.Common.Logging;
using CDP.CoreApp.Interfaces.Contests;
using CDP.CoreApp.Interfaces.Scoring;
using CDP.ScoringAndSortingImpl.F3K.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CDP.ScoringAndSortingImpl.F3K.Tests
{
    [TestClass]
    public class Top10PilotsFlyoffsSortTests
    {
        private Mock<IScoringRepository> mockScoringRepository = new Mock<IScoringRepository>();
        private Mock<ILoggingService> mockLogger = new Mock<ILoggingService>();
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

        [TestMethod]
        public void SortSingleRound_HappyPath()
        {
            // GetScores returns a set of scores for 13 pilots, should only get 10 back.
            var flyoffs = topTenAlgo.SortSingleRound(GetScores(), 10);
            Assert.AreEqual(10, flyoffs.Count);
        }

        /// <summary>
        /// Gets the scores.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, PilotContestScoreCollection> GetScores()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, PilotContestScoreCollection>>("{\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\":{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"TotalScore\":1000.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[1000.0]},\"1e5a2df0-1b6a-4db6-813b-bab26817054d\":{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"TotalScore\":877.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[877.0]},\"74cb37e4-b607-4217-aaec-d8d56c11242e\":{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"TotalScore\":767.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[767.0]},\"55205178-c4d4-4f1d-8969-bb72beabb805\":{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"TotalScore\":1000.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[1000.0]},\"e73f8d6d-515a-4b98-bef7-672c5db99de1\":{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"TotalScore\":1000.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[1000.0]},\"066ce68c-a1b3-4dea-b445-125d965f6e84\":{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"TotalScore\":305.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[305.0]},\"de346d10-4529-430c-bf9c-97dc214070e2\":{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"TotalScore\":816.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[816.0]},\"a86117e2-0eac-4f38-97f8-60f39d4df565\":{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"TotalScore\":1000.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[1000.0]},\"9e34fe28-157e-45ab-8995-3f3f41d19891\":{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"TotalScore\":1000.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[1000.0]},\"0801c060-0af4-4225-abba-d8bf952194d2\":{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"TotalScore\":1000.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[1000.0]},\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\":{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"TotalScore\":940.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[940.0]},\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\":{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"TotalScore\":113.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[113.0]},\"330e2e97-0fdd-493b-a703-6774a5a735e7\":{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"TotalScore\":500.0,\"DroppedRoundOrdinal\":-1,\"Comparer\":{},\"Count\":1,\"Keys\":[0],\"Values\":[500.0]}}");
            
        }
    }
}
