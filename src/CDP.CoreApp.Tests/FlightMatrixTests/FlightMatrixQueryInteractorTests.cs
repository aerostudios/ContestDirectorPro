using CDP.AppDomain;
using CDP.AppDomain.FlightMatrices;
using CDP.Common.Logging;
using CDP.CoreApp.Features.FlightMatrices.Queries;
using CDP.CoreApp.Interfaces.FlightMatrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.FlightMatrixTests
{
    [TestClass]
    public class FlightMatrixQueryInteractorTests
    {
        private Mock<IFlightMatrixRepository> mockFlightMatrixRepository;
        private Mock<ILoggingService> mockLogger;

        [TestInitialize]
        public void Setup()
        {
            mockFlightMatrixRepository = new Mock<IFlightMatrixRepository>();
            mockLogger = new Mock<ILoggingService>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FlightMatrixQueryInteractor_Ctor_NullRepositoryParameter()
        {
            var fmqInteractor = new FlightMatrixQueryInteractor(null, mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FlightMatrixQueryInteractor_Ctor_NullLoggerParameter()
        {
            var fmqInteractor = new FlightMatrixQueryInteractor(mockFlightMatrixRepository.Object, null);
        }

        [TestMethod]
        public void FlightMatrixQueryInteractor_Ctor_HappyPath()
        {
            var fmqInteractor = new FlightMatrixQueryInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
        }

        [TestMethod]
        public async Task FlightMatrixQueryInteractor_GetFlightMatrixForContest_NullParameters()
        {
            var fmqInteractor = new FlightMatrixQueryInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
            var result = await fmqInteractor.GetFlightMatrixForContest(null);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task FlightMatrixQueryInteractor_GetFlightMatrixForContest_BadContestParameter()
        {
            var fmqInteractor = new FlightMatrixQueryInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
            var result = await fmqInteractor.GetFlightMatrixForContest(string.Empty);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task FlightMatrixQueryInteractor_GetFlightMatrixForContest_HappyPath()
        {
            var contestId = "dfklewrowidf";

            var flightMatrix = new FlightMatrix
            {
                ContestId = contestId,
                Matrix = new List<FlightMatrixRound>
                {
                    new FlightMatrixRound
                    {
                        RoundOrdinal = 0,
                        PilotSlots = new List<FlightMatrixPilotSlot>
                        {
                            new FlightMatrixPilotSlot
                            {
                                PilotId = "sdfsf",
                                FlightGroup = FlightGroup.A
                            }
                        }
                    }
                }
            };

            mockFlightMatrixRepository.Setup(fmqr => fmqr.ReadAsync(contestId)).Returns(Task.FromResult(new Result<FlightMatrix>(flightMatrix)));
            var fmqInteractor = new FlightMatrixQueryInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
            var result = await fmqInteractor.GetFlightMatrixForContest(contestId);

            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(contestId, result.Value.ContestId);
            Assert.IsNotNull(result.Value.Matrix);
            Assert.AreEqual(1, result.Value.Matrix.Count);
        }
    }
}