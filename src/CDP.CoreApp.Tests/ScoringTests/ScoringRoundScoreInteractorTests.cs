using CDP.AppDomain.Scoring;
using CDP.AppDomain.Tasks;
using CDP.AppDomain.Tasks.F3K;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Scoring;
using CDP.CoreApp.Features.Scoring.Commands;
using CDP.CoreApp.Interfaces.Scoring;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace CDP.CoreApp.Tests.ScoringTests
{
    [TestClass]
    public class ScoringRoundScoreInteractorTests
    {
        Mock<ILoggingService> mockLogger;
        Mock<IRoundScoringAlgo> mockRoundScoringAlgo;
        Mock<TaskBase> mockTaskBase;

        [TestInitialize]
        public void Setup()
        {
            mockLogger = new Mock<ILoggingService>();
            mockRoundScoringAlgo = new Mock<IRoundScoringAlgo>();
            mockTaskBase = new Mock<TaskBase>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScoringRoundScoreInteractor_Ctor_NullParam()
        {
            var sci = new ScoringRoundScoreInteractor(null, mockLogger.Object);
        }

        [TestMethod]
        public void ScoringRoundScoreInteractor_ScoreRound_AllNullParams()
        {
            var sci = new ScoringRoundScoreInteractor(mockRoundScoringAlgo.Object, mockLogger.Object);
            var result = sci.ScoreRound(null, null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void ScoringRoundScoreInteractor_ScoreRound_NullTimeCardsParam()
        {
            var taskG = new TaskG_FiveTwos();
            var sci = new ScoringRoundScoreInteractor(mockRoundScoringAlgo.Object, mockLogger.Object);
            var result = sci.ScoreRound(null, taskG);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void ScoringRoundScoreInteractor_ScoreRound_NullTaskParam()
        {
            var timeSheets = ScoringTestHelpers.GenerateValidTimeSheets(10);
            var sci = new ScoringRoundScoreInteractor(mockRoundScoringAlgo.Object, mockLogger.Object);
            var result = sci.ScoreRound(timeSheets, null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        /// <summary>
        /// Generates the pilot scores.
        /// </summary>
        /// <param name="numberOfPilots">The number of pilots.</param>
        /// <param name="numberOfRounds">The number of rounds.</param>
        /// <param name="timeSheets">The time sheets.</param>
        /// <returns></returns>
        private Dictionary<string, PilotContestScoreCollection> GeneratePilotScores(int numberOfPilots, int numberOfRounds, IEnumerable<TimeSheet> timeSheets)
        {
            // This is really basic, we aren't testing the algo, we are testing the validation and handling of the non-algo code in this test class
            var returnCollection = new Dictionary<string, PilotContestScoreCollection>();
            var rnd = new Random();

            for (var i = 0; i < numberOfPilots; ++i)
            {
                returnCollection.Add($"pilot{i}", new PilotContestScoreCollection($"pilot{i}", rnd.Next(0, 1000)));
                for (var j = 0; j < numberOfRounds; ++j)
                {
                    returnCollection[$"pilot{i}"].Add(j, 10);
                }
            }

            return returnCollection;
        }
    }
}
