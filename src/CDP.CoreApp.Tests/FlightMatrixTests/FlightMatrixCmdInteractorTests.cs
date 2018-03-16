using CDP.AppDomain;
using CDP.AppDomain.FlightMatrices;
using CDP.Common.Logging;
using CDP.CoreApp.Features.FlightMatrices.Commands;
using CDP.CoreApp.Interfaces.FlightMatrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.FlightMatrixTests
{
    [TestClass]
    public class FlightMatrixCmdInteractorTests
    {
        Mock<IFlightMatrixRepository> mockFlightMatrixRepository;
        Mock<ILoggingService> mockLogger;

        [TestInitialize]
        public void Setup()
        {
            this.mockFlightMatrixRepository = new Mock<IFlightMatrixRepository>();
            this.mockLogger = new Mock<ILoggingService>();
        }

        [TestMethod]
        public async Task FlightMatrixCommandInteractor_SaveFlightMatrixForContestAsync_NullParameters()
        {
            var flightMatrixCmdInteractor = new FlightMatrixStorageCmdInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
            var result = await flightMatrixCmdInteractor.SaveFlightMatrixForContestAsync(null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task FlightMatrixCommandInteractor_SaveFlightMatrixForContestAsync_BadFlightMatrixParameter()
        {
            var flightMatrix = new FlightMatrix
            {
                Matrix = null
            };

            var flightMatrixCmdInteractor = new FlightMatrixStorageCmdInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
            var result = await flightMatrixCmdInteractor.SaveFlightMatrixForContestAsync(flightMatrix);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task FlightMatrixCommandInteractor_SaveFlightMatrixForContestAsync_NoPilotsInMatrix()
        {
            var flightMatrix = new FlightMatrix
            {
                ContestId = "234234"
            };

            var flightMatrixCmdInteractor = new FlightMatrixStorageCmdInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
            var result = await flightMatrixCmdInteractor.SaveFlightMatrixForContestAsync(flightMatrix);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task FlightMatrixCommandInteractor_SaveFlightMatrixForContestAsync_HappyPath()
        {
            var flightMatrix = new FlightMatrix
            {
                ContestId = "sdfasdf"
            };

            flightMatrix.Matrix.Add(new FlightMatrixRound
            {
                RoundOrdinal = 0,
                PilotSlots = new List<FlightMatrixPilotSlot>
                {
                    new FlightMatrixPilotSlot
                    {
                        PilotId = "sadfxcvcxasdf",
                        FlightGroup = FlightGroup.A
                    }
                }
            });

            mockFlightMatrixRepository.Setup(f => f.CreateAsync(It.IsAny<FlightMatrix>())).Returns<FlightMatrix>(x => Task.FromResult(new Result<FlightMatrix>(flightMatrix)));

            var flightMatrixCmdInteractor = new FlightMatrixStorageCmdInteractor(mockFlightMatrixRepository.Object, mockLogger.Object);
            var result = await flightMatrixCmdInteractor.SaveFlightMatrixForContestAsync(flightMatrix);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
        }
    }
}
