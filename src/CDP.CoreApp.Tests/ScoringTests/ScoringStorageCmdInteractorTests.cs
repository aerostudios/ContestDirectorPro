using CDP.AppDomain;
using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Scoring;
using CDP.AppDomain.Tasks;
using CDP.AppDomain.Tasks.F3K;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Scoring.Commands;
using CDP.CoreApp.Interfaces.Scoring;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.ScoringTests
{
    [TestClass]
    public class ScoringStorageCmdInteractorTests
    {
        private Mock<IScoringRepository> mockScoringRepository;
        private Mock<ILoggingService> mockLogger;

        [TestInitialize]
        public void Setup()
        {
            mockScoringRepository = new Mock<IScoringRepository>();
            mockLogger = new Mock<ILoggingService>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScoringStorageCmdInteractor_Ctor_NullRespositoryParam()
        {
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(null, mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScoringStorageCmdInteractor_Ctor_NullLoggerParam()
        {
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, null);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_SaveRoundScores_NullTimeSheet()
        {
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.SaveRoundScoresAsync(null, "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_SaveRoundScores_EmptyTimeSheet()
        {
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.SaveRoundScoresAsync(new List<TimeSheet>(), "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_SaveRoundScores_NullContestId()
        {
            List<TimeSheet> pilotScores = GenerateValidTimeSheets(10);
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.SaveRoundScoresAsync(pilotScores, null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_SaveRoundScores_EmptyContestId()
        {
            List<TimeSheet> pilotScores = GenerateValidTimeSheets(10);
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.SaveRoundScoresAsync(pilotScores, string.Empty);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_SaveRoundScores_BadScores_BadPilotId()
        {
            List<TimeSheet> pilotScores = GenerateValidTimeSheets(10);

            var timeGates = new List<TimeGate> { new TimeGate(TimeSpan.MinValue, 0, TimeGateType.Task) };
            var taskB = new TaskB_LastTwoFlights4MinMax();

            var roundScore = new TimeSheet
            {
                RoundOrdinal = 8,
                TimeGates = timeGates,
                PilotId = null,
                ContestId = "asdfawewrs",
                TaskId = taskB.Id,
                FlightGroup = FlightGroup.A
            };

            pilotScores[7] = roundScore;

            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.SaveRoundScoresAsync(pilotScores, "234234");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_SaveRoundScores_HappyPath()
        {
            List<TimeSheet> pilotScores = GenerateValidTimeSheets(10);

            var timeGates = new List<TimeGate> { new TimeGate(TimeSpan.MinValue, 0, TimeGateType.Task) };
            var taskB = new TaskB_LastTwoFlights4MinMax();

            mockScoringRepository.Setup(sr => sr.CreateAsync(It.IsAny<IEnumerable<TimeSheet>>())).Returns(Task.FromResult(new Result<IEnumerable<TimeSheet>>(pilotScores)));

            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.SaveRoundScoresAsync(pilotScores, "234234");

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(10, result.Value.Count());
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_UpdatePilotsScoreAsync_NullTimeSheetParam()
        {
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.UpdatePilotsScoreAsync(null, "asdfsa");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_UpdatePilotScoreAsync_EmptyContestParam()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet.First(), string.Empty);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_UpdatePilotScoreAsync_RepositoryException()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            mockScoringRepository.Setup(sr => sr.UpdateAsync(It.IsAny<TimeSheet>())).ThrowsAsync(new Exception());
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet.First(), "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_UpdatePilotScoreAsync_RepositoryReturnsNull()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            mockScoringRepository.Setup(sr => sr.UpdateAsync(It.IsAny<TimeSheet>())).Returns(Task.FromResult(new Result<TimeSheet>(null)));
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet.First(), "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_UpdatePilotScoreAsync_BadTimeSheetParam()
        {
            var timeSheet = GenerateValidTimeSheets(1).First();

            timeSheet.ContestId = null;

            mockScoringRepository.Setup(sr => sr.UpdateAsync(It.IsAny<TimeSheet>())).Returns(Task.FromResult(new Result<TimeSheet>(timeSheet)));
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet, "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);

            timeSheet.ContestId = "234234";
            timeSheet.FlightGroup = FlightGroup.Unknown;

            scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet, "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);

            timeSheet.FlightGroup = FlightGroup.A;
            timeSheet.PilotId = null;

            scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet, "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);

            timeSheet.PilotId = "234234";
            timeSheet.RoundOrdinal = -1;

            scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet, "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);

            timeSheet.RoundOrdinal = 2;
            timeSheet.TaskId = null;

            scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet, "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);

            timeSheet.TaskId = "234234";
            timeSheet.TimeGates = null;

            scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet, "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_UpdatePilotScoreAsync_HappyPath()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            mockScoringRepository.Setup(sr => sr.UpdateAsync(It.IsAny<TimeSheet>())).Returns(Task.FromResult(new Result<TimeSheet>(timeSheet.First())));
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.UpdatePilotsScoreAsync(timeSheet.First(), "sadfasd");

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Value);
        }
        
        [TestMethod]
        public async Task ScoringStorageCmdInteractor_DeletePilotScoreAsync_NullTimeSheetParam()
        {
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.DeletePilotScoreAsync(null, "asdfsa");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(false, result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_DeletePilotScoreAsync_EmptyContestId()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.DeletePilotScoreAsync(timeSheet.First(), string.Empty);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(false, result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_DeletePilotScoreAsync_RepositoryException()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            mockScoringRepository.Setup(sr => sr.UpdateAsync(It.IsAny<TimeSheet>())).ThrowsAsync(new Exception());
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.DeletePilotScoreAsync(timeSheet.First(), "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(false, result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_DeletePilotScoreAsync_RepositoryReturnsFalse()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            mockScoringRepository.Setup(sr => sr.DeleteAsync(It.IsAny<TimeSheet>())).Returns(Task.FromResult(new Result<bool>(false)));
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.DeletePilotScoreAsync(timeSheet.First(), "sadfasd");

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(false, result.Value);
        }

        [TestMethod]
        public async Task ScoringStorageCmdInteractor_DeletePilotScoreAsync_HappyPath()
        {
            var timeSheet = GenerateValidTimeSheets(1);
            mockScoringRepository.Setup(sr => sr.DeleteAsync(It.IsAny<TimeSheet>())).Returns(Task.FromResult(new Result<bool>(true)));
            var scoringCmdInteractor = new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object);
            var result = await scoringCmdInteractor.DeletePilotScoreAsync(timeSheet.First(), "sadfasd");

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNull(result.Error);
            Assert.AreEqual(true, result.Value);
        }

        private List<TimeSheet> GenerateValidTimeSheets(int numberToCreate)
        {
            var validScores = new List<TimeSheet>();
            var TaskA = new TaskA_LastFlightSevenMin();

            if (numberToCreate == 0) return validScores;
            var rnd = new Random();

            for (var i = 0; i < numberToCreate; ++i)
            {
                var timeGates = new List<TimeGate> { new TimeGate(TimeSpan.MinValue, i, TimeGateType.Task) };
                var roundScore = new TimeSheet
                {
                    RoundOrdinal = 1,
                    TimeGates = timeGates,
                    PilotId = $"pilot{rnd.Next(1, 100)}",
                    ContestId = "234234",
                    TaskId = TaskA.Id,
                    FlightGroup = FlightGroup.A
                };

                validScores.Add(roundScore);
            }

            return validScores;
        }
    }
}