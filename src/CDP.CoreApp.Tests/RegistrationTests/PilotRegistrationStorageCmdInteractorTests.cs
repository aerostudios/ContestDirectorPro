using CDP.AppDomain;
using CDP.AppDomain.Registration;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Registration.Commands;
using CDP.CoreApp.Interfaces.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.RegistrationTests
{
    [TestClass]
    public class PilotRegistrationStorageCmdInteractorTests
    {
        private Mock<IPilotRegistrationRepository> mockPilotRegistrationRepository;
        private Mock<ILoggingService> mockLogger;

        [TestInitialize]
        public void Setup()
        {
            mockPilotRegistrationRepository = new Mock<IPilotRegistrationRepository>();
            mockLogger = new Mock<ILoggingService>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PilotRegistrationStorageCmdInteractor_Ctor_NullRepositoryParameter()
        {
            var prci = new PilotRegistrationStorageCmdInteractor(null, mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PilotRegistrationStorageCmdInteractor_Ctor_NullLoggerParameter()
        {
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, null);
        }

        [TestMethod]
        public void PilotRegistrationStorageCmdInteractor_Ctor_HappyPath()
        {
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterSinglePilotForContestAsync_NullParameter()
        {
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterSinglePilotForContestAsync(null);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterSinglePilotForContestAsync_BadRegistrationParameter()
        {
            PilotRegistration pilotRegistration = GenerateValidPilotRegistration(1).First();
            
            pilotRegistration.ContestId = string.Empty;
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            // Invalid Contest Id should fail
            Assert.IsTrue(result.IsFaulted);

            pilotRegistration.ContestId = null;
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));
            prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            // Null Contest Id should fail
            Assert.IsTrue(result.IsFaulted);

            pilotRegistration.ContestId = "1";
            pilotRegistration.AirframesSignedOff = false;
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));
            result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            // It should be ok to register a pilot w/ a failed airframe sign-off
            Assert.IsFalse(result.IsFaulted);

            pilotRegistration.AirframesSignedOff = true;
            pilotRegistration.AirframeRegistrationNumbers = null;
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));
            result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            // It should be ok to not have any airframes registered
            Assert.IsFalse(result.IsFaulted);

            pilotRegistration.AirframeRegistrationNumbers = new List<string> { "1", "2" };
            pilotRegistration.IsPaid = false;
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));
            result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            // It should be ok to have a pilot register that is not paid
            Assert.IsFalse(result.IsFaulted);

            pilotRegistration.PilotId = string.Empty;
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));
            result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);
            
            // Empty Pilot ID should fail.
            Assert.IsTrue(result.IsFaulted);

            pilotRegistration.PilotId = null;
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));
            result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            // Null Pilot ID should fail.
            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterSinglePilotForContestAsync_RepositoryException()
        {
            PilotRegistration pilotRegistration = GenerateValidPilotRegistration(1).First();
            
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>())).ThrowsAsync(new Exception());
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterSinglePilotForContestAsync_HappyPath()
        {
            var pilotRegistration = GenerateValidPilotRegistration(1).First();
            
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<PilotRegistration>()))
                .Returns(Task.FromResult(new Result<PilotRegistration>(pilotRegistration)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterSinglePilotForContestAsync(pilotRegistration);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNull(result.Error);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterPilotsForContestAsync_NullParameter()
        {
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterPilotsForContestAsync(null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterPilotsForContestAsync_EmptyPilotsToRegisterParameter()
        {
            var pilotRegistrations = new List<PilotRegistration>();
            
            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<IEnumerable<PilotRegistration>>()))
                .Returns(Task.FromResult(new Result<IEnumerable<PilotRegistration>>(pilotRegistrations)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterPilotsForContestAsync(pilotRegistrations);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterPilotsForContestAsync_BadPilotsToRegisterParameter()
        {
            var pilotRegistrations = new List<PilotRegistration>(GenerateValidPilotRegistration(10));
            pilotRegistrations[4].ContestId = string.Empty;

            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<IEnumerable<PilotRegistration>>()))
                .Returns(Task.FromResult(new Result<IEnumerable<PilotRegistration>>(pilotRegistrations)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterPilotsForContestAsync(pilotRegistrations);

            // One bad contest id should fail the set.
            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
            Assert.IsTrue(result.Error.ErrorMessage.Contains("4"));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_RegisterPilotsForContestAsync_HappyPath()
        {
            var pilotRegistrations = new List<PilotRegistration>(GenerateValidPilotRegistration(10));

            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<IEnumerable<PilotRegistration>>()))
                .Returns(Task.FromResult(new Result<IEnumerable<PilotRegistration>>(pilotRegistrations)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.RegisterPilotsForContestAsync(pilotRegistrations);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotForContestAsync_NullParameter()
        {
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotForContestAsync(null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotForContestAsync_BadPilotToUnregisterParam()
        {
            var pilotRegistration = GenerateValidPilotRegistration(1).First();
            pilotRegistration.PilotId = string.Empty;

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotForContestAsync(pilotRegistration);

            // Bad pilot Id 
            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));

            pilotRegistration.PilotId = "21341";
            pilotRegistration.ContestId = string.Empty;

            prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            result = await prci.UnRegisterPilotForContestAsync(pilotRegistration);

            // Bad Contest id
            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotForContestAsync_RepositoryFailure()
        {
            var pilotRegistration = GenerateValidPilotRegistration(1).First();
            pilotRegistration.PilotId = "2342342";

            mockPilotRegistrationRepository.Setup(prr => prr.DeleteAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotForContestAsync(pilotRegistration);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error.Exception);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotForContestAsync_HappyPath()
        {
            var pilotRegistration = GenerateValidPilotRegistration(1).First();
            pilotRegistration.PilotId = "23092wefsdf";

            mockPilotRegistrationRepository.Setup(prr => prr.DeleteAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Result<bool>(true)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotForContestAsync(pilotRegistration);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotsForContestAsync_NullParameter()
        {
            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotsForContestAsync(null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotsForContestAsync_EmptyPilotsToRegisterParameter()
        {
            var pilotRegistrations = new List<PilotRegistration>();

            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<IEnumerable<PilotRegistration>>()))
                .Returns(Task.FromResult(new Result<IEnumerable<PilotRegistration>>(pilotRegistrations)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotsForContestAsync(pilotRegistrations);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotsForContestAsync_BadPilotsToRegisterParameter()
        {
            var pilotRegistrations = new List<PilotRegistration>(GenerateValidPilotRegistration(10));
            pilotRegistrations[4].ContestId = string.Empty;

            mockPilotRegistrationRepository.Setup(prr => prr.CreateAsync(It.IsAny<IEnumerable<PilotRegistration>>()))
                .Returns(Task.FromResult(new Result<IEnumerable<PilotRegistration>>(pilotRegistrations)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotsForContestAsync(pilotRegistrations);

            // One bad contest id should fail the set.
            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsFalse(string.IsNullOrEmpty(result.Error.ErrorMessage));
            Assert.IsTrue(result.Error.ErrorMessage.Contains("4"));
        }

        [TestMethod]
        public async Task PilotRegistrationStorageCmdInteractor_UnRegisterPilotsForContestAsync_HappyPath()
        {
            var pilotRegistrations = new List<PilotRegistration>(GenerateValidPilotRegistration(10));

            mockPilotRegistrationRepository.Setup(prr => prr.DeleteAsync(It.IsAny<IEnumerable<PilotRegistration>>()))
                .Returns(Task.FromResult(new Result<bool>(true)));

            var prci = new PilotRegistrationStorageCmdInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await prci.UnRegisterPilotsForContestAsync(pilotRegistrations);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
        }

        private IEnumerable<PilotRegistration> GenerateValidPilotRegistration(int numberToCreate)
        {
            var listToCreate = new List<PilotRegistration>();
            var rnd = new Random();

            for (var i = 0; i < numberToCreate; ++i)
            {
                listToCreate.Add(new PilotRegistration
                {
                    AirframeRegistrationNumbers = new List<string> { rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString() },
                    AirframesSignedOff = true,
                    ContestId = rnd.Next(1, 100).ToString(),
                    Id = string.Empty,
                    IsPaid = true,
                    PilotId = rnd.Next(1, 100).ToString()
                });
            }

            return listToCreate;
        }
    }
}
