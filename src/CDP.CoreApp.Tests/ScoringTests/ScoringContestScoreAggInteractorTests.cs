using CDP.AppDomain.Scoring;
using CDP.AppDomain.Tasks;
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
    public class ScoringContestScoreAggInteractorTests
    {
        Mock<ILoggingService> mockLogger;
        Mock<IContestScoreAggregator> mockContestScoreAggregator;
        Mock<TaskBase> mockTaskBase;

        [TestInitialize]
        public void Setup()
        {
            mockLogger = new Mock<ILoggingService>();
            mockContestScoreAggregator = new Mock<IContestScoreAggregator>();
            mockTaskBase = new Mock<TaskBase>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScoringContestScoreAggInteractor_Ctor_NullParam()
        {
            var sci = new ScoringContestScoreAggInteractor(null, mockLogger.Object);
        }

        [TestMethod]
        public void ScoringContestScoreAggInteractor_GenerateContestScores_NullParam()
        {
            var sci = new ScoringContestScoreAggInteractor(mockContestScoreAggregator.Object, mockLogger.Object);
            var result = sci.GetAggregateRoundScoresForPilots(null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void ScoringContestScoreAggInteractor_GenerateContestScores_EmptyParam()
        {
            var sci = new ScoringContestScoreAggInteractor(mockContestScoreAggregator.Object, mockLogger.Object);
            var result = sci.GetAggregateRoundScoresForPilots(new ContestScoresCollection());

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void ScoringContestScoreAggInteractor_GetAggregateRoundScoresForPilots_HappyPath()
        {
            var numberOfPilots = 10;
            var numberOfRounds = 10;
            var contestScores = ScoringTestHelpers.GenerateValidContestRoundScores(numberOfPilots, "SASS October 2017", numberOfRounds, 120, true, true);
            var fakeScores = GeneratePilotScores(numberOfPilots, numberOfRounds, contestScores);

            mockContestScoreAggregator.Setup(csa => csa.GenerateContestScores(It.IsAny<ContestScoresCollection>())).Returns(fakeScores);

            var sci = new ScoringContestScoreAggInteractor(mockContestScoreAggregator.Object, mockLogger.Object);
            var result = sci.GetAggregateRoundScoresForPilots(contestScores);

            // Verify all the pilots come back.
            Assert.AreEqual(10, result.Value.Keys.Count);
        }

        /// <summary>
        /// Generates the pilot scores.
        /// </summary>
        /// <param name="numberOfPilots">The number of pilots.</param>
        /// <param name="numberOfRounds">The number of rounds.</param>
        /// <param name="timeSheets">The time sheets.</param>
        /// <returns></returns>
        private Dictionary<string, PilotContestScoreCollection> GeneratePilotScores(int numberOfPilots, int numberOfRounds, ContestScoresCollection scores)
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
