using CDP.AppDomain;
using CDP.AppDomain.Pilots;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Pilots.Queries;
using CDP.CoreApp.Interfaces.Pilots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.ContestTests
{
    [TestClass]
    public class PilotQueryTests
    {
        Mock<ILoggingService> mockLogger;
        Mock<IPilotRepository> mockPilotRepository;

        [TestInitialize]
        public void Setup()
        {
            mockPilotRepository = new Mock<IPilotRepository>();
            mockLogger = new Mock<ILoggingService>();
        }

        [TestMethod]
        public async Task PilotQueryInteractor_GetPilotAsync_NullParameters()
        {
            var pilotQueryInteractor = new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object);
            var result = await pilotQueryInteractor.GetPilotAsync(string.Empty);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task PilotQueryInteractor_GetPilotAsync_RepositoryFailure_ReturnsNull()
        {
            var id = "dlfw23efsdfl";
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "23423940";
            var airFrame = "Snipe 2";

            var pilot = new Pilot(fName, lName, id, amaNumber, airFrame);

            mockPilotRepository.Setup(p => p.ReadAsync(It.IsAny<string>())).Returns(Task.FromResult(new Result<Pilot>(null)));
            var pilotQueryInteractor = new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object);

            var result = await pilotQueryInteractor.GetPilotAsync(id);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task PilotQueryInteractor_GetPilotAsync_RepositoryFailure_Exception()
        {
            var id = "234sdfsjf";
            var exceptionMessage = "Derp";

            mockPilotRepository.Setup(p => p.ReadAsync(It.IsAny<string>())).Throws(new Exception(exceptionMessage));
            var pilotQueryInteractor = new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object);

            var result = await pilotQueryInteractor.GetPilotAsync(id);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
            Assert.IsNotNull(result.Error.Exception);
            Assert.AreEqual(exceptionMessage, result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task PilotQueryInteractor_GetPilotAsync_HappyPath()
        {
            var id = "sdflkwjrw";
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "23423940";
            var airFrame = "Snipe 2";

            var pilot = new Pilot(fName, lName, id, amaNumber, airFrame);

            mockPilotRepository.Setup(p => p.ReadAsync(It.IsAny<string>())).Returns(Task.FromResult(new Result<Pilot>(pilot)));
            var pilotQueryInteractor = new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object);

            var result = await pilotQueryInteractor.GetPilotAsync(id);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(id, result.Value.Id);
            Assert.AreEqual(fName, result.Value.FirstName);
            Assert.AreEqual(lName, result.Value.LastName);
            Assert.AreEqual(amaNumber, result.Value.StandardsBodyId);
            Assert.AreEqual(airFrame, result.Value.Airframe);
        }

        [TestMethod]
        public async Task PilotQueryInteractor_GetAllPilotsAsync_RepositoryFailure_ReturnsNull()
        {
            mockPilotRepository.Setup(p => p.ReadAsync()).Returns(Task.FromResult(new Result<IEnumerable<Pilot>>(null)));

            var pilotQueryInteractor = new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object);
            var result = await pilotQueryInteractor.GetAllPilotsAsync();

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task PilotQueryInteractor_GetAllPilotsAsync_RepositoryFailure_Exception()
        {
            var exceptionMessage = "Derp";

            mockPilotRepository.Setup(p => p.ReadAsync()).Throws(new Exception(exceptionMessage));
            var pilotQueryInteractor = new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object);

            var result = await pilotQueryInteractor.GetAllPilotsAsync();

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
            Assert.IsNotNull(result.Error.Exception);
            Assert.AreEqual(exceptionMessage, result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task PilotQueryInteractor_GetAllPilotsAsync_HappyPath()
        {
            var id = "235dsfkjsdfwi";
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "23423940";
            var airFrame = "Snipe 2";

            var pilot = new Pilot(fName, lName, id, amaNumber, airFrame);
            var pilot2 = new Pilot("Sam", "Snead", "23dsfsdf", "1234", "Gambler");
            var pilots = new List<Pilot> { pilot, pilot2 };

            mockPilotRepository.Setup(c => c.ReadAsync()).Returns(Task.FromResult(new Result<IEnumerable<Pilot>>(pilots)));

            var pilotQueryInteractor = new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object);
            var result = await pilotQueryInteractor.GetAllPilotsAsync();

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
