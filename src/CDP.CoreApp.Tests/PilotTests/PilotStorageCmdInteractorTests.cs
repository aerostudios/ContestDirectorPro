using CDP.AppDomain;
using CDP.AppDomain.Pilots;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Pilots.Commands;
using CDP.CoreApp.Interfaces.Pilots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.PilotTests
{
    [TestClass]
    public class PilotStorageCmdInteractorTests
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
        public async Task PilotStorageCmdInteractor_CreatePilotAsync_NullParameters()
        {

            var contestCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.CreateNewPilotAsync(null);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_CreatePilotAsync_BadPilotObjectParameter()
        {
            var pilot = new Pilot(string.Empty, "foo", string.Empty);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            var result = await pilotCmdInteractor.CreateNewPilotAsync(pilot);

            // Check empty first name
            Assert.IsTrue(result.IsFaulted);

            pilot.FirstName = "bar";
            pilot.LastName = string.Empty;

            result = await pilotCmdInteractor.CreateNewPilotAsync(pilot);

            // Check empty last name
            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_CreatePilotAsync_RepositoryFailure()
        {
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "234235045";
            var airframe = "snipe";

            var pilot = new Pilot(fName, lName, string.Empty, amaNumber, airframe);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            mockPilotRepository.Setup(p => p.CreateAsync(It.IsAny<Pilot>())).Returns<Pilot>(x => Task.FromResult(new Result<Pilot>(null)));

            var result = await pilotCmdInteractor.CreateNewPilotAsync(pilot);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_CreatePilotAsync_HappyPath()
        {
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "234235045";
            var airframe = "snipe";

            var pilot = new Pilot(fName, lName, string.Empty, amaNumber, airframe);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            mockPilotRepository.Setup(p => p.CreateAsync(It.IsAny<Pilot>())).Returns<Pilot>(x => Task.FromResult(new Result<Pilot>(pilot)));

            var result = await pilotCmdInteractor.CreateNewPilotAsync(pilot);

            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(fName, result.Value.FirstName);
            Assert.AreEqual(lName, result.Value.LastName);
            Assert.AreEqual(amaNumber, result.Value.StandardsBodyId);
            Assert.AreEqual(airframe, result.Value.Airframe);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_UpdatePilotAsync_NullParameters()
        {
            var contestCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.UpdatePilotAsync(null);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_UpdatePilotAsync_BadPilotParameter()
        {
            var pilot = new Pilot(string.Empty, "foo", string.Empty);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            var result = await pilotCmdInteractor.CreateNewPilotAsync(pilot);

            // Check empty first name
            Assert.IsTrue(result.IsFaulted);

            pilot.FirstName = "bar";
            pilot.LastName = string.Empty;

            result = await pilotCmdInteractor.UpdatePilotAsync(pilot);

            // Check empty last name
            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_UpdatePilotAsync_RepositoryFailure()
        {
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "234235045";
            var airframe = "snipe";

            var pilot = new Pilot(fName, lName, string.Empty, amaNumber, airframe);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            mockPilotRepository.Setup(p => p.UpdateAsync(It.IsAny<Pilot>())).Returns<Pilot>(x => Task.FromResult(new Result<Pilot>(null)));

            var result = await pilotCmdInteractor.UpdatePilotAsync(pilot);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_UpdatePilotAsync_HappyPath()
        {
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "234235045";
            var airframe = "snipe";

            var pilot = new Pilot(fName, lName, string.Empty, amaNumber, airframe);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            mockPilotRepository.Setup(p => p.UpdateAsync(It.IsAny<Pilot>())).Returns<Pilot>(x => Task.FromResult(new Result<Pilot>(pilot)));

            var result = await pilotCmdInteractor.UpdatePilotAsync(pilot);

            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(fName, result.Value.FirstName);
            Assert.AreEqual(lName, result.Value.LastName);
            Assert.AreEqual(amaNumber, result.Value.StandardsBodyId);
            Assert.AreEqual(airframe, result.Value.Airframe);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_DeletePilotAsync_NullParameters()
        {
            var contestCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.DeletePilotAsync(null);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_DeletePilotAsync_RepositoryFailure()
        {
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "234235045";
            var airframe = "snipe";
            var id = "adla;sdkjfas";

            var pilot = new Pilot(fName, lName, id, amaNumber, airframe);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            mockPilotRepository.Setup(p => p.DeleteAsync(It.IsAny<string>())).Returns<Pilot>(x => Task.FromResult(new Result<bool>(false)));

            var result = await pilotCmdInteractor.DeletePilotAsync(pilot);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsFalse(result.Value);
        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_DeletePilotAsync_BadPilotParameter()
        {
            var pilot = new Pilot("bar", "foo", string.Empty);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            var result = await pilotCmdInteractor.DeletePilotAsync(pilot);

            // Check empty id GUID
            Assert.IsTrue(result.IsFaulted);
            Assert.IsFalse(result.Value);

        }

        [TestMethod]
        public async Task PilotStorageCmdInteractor_DeletePilotAsync_HappyPath()
        {
            var fName = "Rick";
            var lName = "R";
            var amaNumber = "234235045";
            var airframe = "snipe";
            var id = "dlsfjasdf";

            var pilot = new Pilot(fName, lName, id, amaNumber, airframe);
            var pilotCmdInteractor = new PilotStorageCmdInteractor(mockPilotRepository.Object, mockLogger.Object);

            mockPilotRepository.Setup(p => p.DeleteAsync(It.IsAny<string>())).Returns<string>(x => Task.FromResult(new Result<bool>(true)));

            var result = await pilotCmdInteractor.DeletePilotAsync(pilot);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsTrue(result.Value);
        }
    }
}
