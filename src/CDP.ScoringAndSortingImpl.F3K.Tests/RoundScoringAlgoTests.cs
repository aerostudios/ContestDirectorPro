using CDP.AppDomain.Scoring;
using CDP.AppDomain.Scoring.Exceptions;
using CDP.AppDomain.Tasks;
using CDP.AppDomain.Tasks.F3K;
using CDP.ScoringAndSortingImpl.F3K.Scoring;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDP.ScoringAndSortingImpl.F3K.Tests
{
    [TestClass]
    public class RoundScoringAlgoTests
    {
        Mock<TaskBase> mockTaskBase;

        [TestInitialize]
        public void Setup()
        {
            mockTaskBase = new Mock<TaskBase>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRoundScoreException))]
        public void F3KRoundScoringAlgo_ScoreRound_BadPilotIdParam()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            timeSheets[3].PilotId = null;

            var taskG = new TaskG_FiveTwos();
            var f3KRoundScoringAlgo = new RoundScoringAlgo();
            f3KRoundScoringAlgo.ScoreRound(timeSheets, taskG);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRoundScoreException))]
        public void F3KRoundScoringAlgo_ScoreRound_BadTimeGateParam_InvalidTimespan()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            timeSheets[4].TimeGates = new List<TimeGate> { new TimeGate(new TimeSpan(0, 0, -34), timeSheets[3].RoundOrdinal, TimeGateType.Task) };

            mockTaskBase.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));
            var f3KRoundScoringAlgo = new RoundScoringAlgo();

            f3KRoundScoringAlgo.ScoreRound(timeSheets, mockTaskBase.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRoundScoreException))]
        public void F3KRoundScoringAlgo_ScoreRound_BadTimeGateParam_NullItem()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            timeSheets[4] = null;

            mockTaskBase.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));
            var f3KRoundScoringAlgo = new RoundScoringAlgo();

            f3KRoundScoringAlgo.ScoreRound(timeSheets, mockTaskBase.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRoundScoreException))]
        public void F3KRoundScoringAlgo_ScoreRound_BadTimeGateParam_EmptyItem()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            timeSheets[7].TimeGates = new List<TimeGate>();

            mockTaskBase.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));
            var f3KRoundScoringAlgo = new RoundScoringAlgo();

            f3KRoundScoringAlgo.ScoreRound(timeSheets, mockTaskBase.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRoundScoreException))]
        public void F3KRoundScoringAlgo_ScoreRound_BadTimeGateParam_NegativePenalty()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            timeSheets[4].TotalPenalties = -100;

            mockTaskBase.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));
            var f3KRoundScoringAlgo = new RoundScoringAlgo();

            f3KRoundScoringAlgo.ScoreRound(timeSheets, mockTaskBase.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRoundScoreException))]
        public void F3KRoundScoringAlgo_ScoreRound_BadTimeGateParam_PracticeTimeIncluded()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            timeSheets[2].TimeGates = new List<TimeGate> { new TimeGate(new TimeSpan(0, 0, -34), timeSheets[3].RoundOrdinal, TimeGateType.Practice) };

            mockTaskBase.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));
            var f3KRoundScoringAlgo = new RoundScoringAlgo();

            f3KRoundScoringAlgo.ScoreRound(timeSheets, mockTaskBase.Object);
        }

        [TestMethod]
        public void F3KRoundScoringAlgo_ScoreRound_HappyPath()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            
            // Mock out the validation to return true every time.
            mockTaskBase.Setup(x => x.ValidateTask(It.IsAny<RoundScoreBase>())).Returns(true);

            // Just mock a task scorer that adds up all the seconds to get a total score (essentially what every F3K task does).
            mockTaskBase.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));

            var f3KRoundScoringAlgo = new RoundScoringAlgo();
            f3KRoundScoringAlgo.ScoreRound(timeSheets, mockTaskBase.Object);
            timeSheets.Sort();

            // Verify the all come back.
            Assert.AreEqual(10, timeSheets.Count());

            // Verify the first is the granny.
            Assert.AreEqual(1000, timeSheets.First().Score);

            // Verify sort order is descending.
            // Loop through each and make sure each is less that the previous (they can be the same though).
            var previousScore = timeSheets.First().Score;
            foreach (var score in timeSheets)
            {
                Assert.IsFalse(score.Score > previousScore);
            }
        }

        [TestMethod]
        public void F3KRoundScoringAlgo_ScoreRound_VerifyPenaltyIsApplied()
        {
            // Create a single round.
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(2, "Test Contest", 1, 120);
            
            // Set the first the be the highest score so we can calculate. 
            timeSheets[0].TimeGates = new List<TimeGate> { new TimeGate(TimeSpan.FromSeconds(121), 0, TimeGateType.Task) };

            // Set a known time on the #2 and add a penalty of 100pts
            var totalPenalty = 100;
            timeSheets[1].TimeGates = new List<TimeGate> { new TimeGate(TimeSpan.FromSeconds(110), 0, TimeGateType.Task) };
            timeSheets[1].TotalPenalties = totalPenalty;
            var expectedEndScoreWithPenaltyApplied = 809;
            
            mockTaskBase.Setup(x => x.ValidateTask(It.IsAny<RoundScoreBase>())).Returns(true);
            mockTaskBase.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));

            var f3KRoundScoringAlgo = new RoundScoringAlgo();
            f3KRoundScoringAlgo.ScoreRound(timeSheets, mockTaskBase.Object);
            timeSheets.Sort();
            
            // Verify the penalty is applied
            Assert.AreEqual(expectedEndScoreWithPenaltyApplied, timeSheets[1].Score);
            Assert.AreEqual(totalPenalty, timeSheets[1].TotalPenalties);
        }
    }
}
