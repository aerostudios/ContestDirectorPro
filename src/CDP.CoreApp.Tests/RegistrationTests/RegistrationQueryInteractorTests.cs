using CDP.AppDomain;
using CDP.AppDomain.Registration;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Registration.Queries;
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
    public class RegistrationQueryInteractorTests
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
        public void RegistrationQueryInteractor_NullParameter()
        {
            var regInteractor = new PilotRegistrationQueryInteractor(null, mockLogger.Object);
        }

        [TestMethod]
        public async Task RegistrationQueryInteractor_GetPilotRegistrationsForContest_NullContestParam()
        {
            var regInteractor = new PilotRegistrationQueryInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await regInteractor.GetPilotRegistrationsForContest(null);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task RegistrationQueryInteractor_GetPilotRegistrationsForContest_BadContestParam_Empty()
        {
            var contestId = string.Empty;

            var regInteractor = new PilotRegistrationQueryInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await regInteractor.GetPilotRegistrationsForContest(contestId);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task RegistrationQueryInteractor_GetPilotRegistrationsForContest_BadContestParam_Null()
        {
            // Make a bad contest id.
            string contestId = null;

            var regInteractor = new PilotRegistrationQueryInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await regInteractor.GetPilotRegistrationsForContest(contestId);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task RegistrationQueryInteractor_GetPilotRegistrationsForContest_RepositoryFailure_Null()
        {
            // Make a valid contest id.
            string contestId = "234werwefsd";

            mockPilotRegistrationRepository.Setup(prr => prr.ReadAsync(It.IsAny<string>())).Returns<IEnumerable<PilotRegistration>>(null);

            var regInteractor = new PilotRegistrationQueryInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await regInteractor.GetPilotRegistrationsForContest(contestId);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task RegistrationQueryInteractor_GetPilotRegistrationsForContest_RepositoryFailure_Empty()
        {
            // Make a valid contest id.
            string contestId = "234we8776refsd";
            var pilotRegistrations = new List<PilotRegistration>();

            mockPilotRegistrationRepository.Setup(prr => prr.ReadAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Result<IEnumerable<PilotRegistration>>(pilotRegistrations)));

            var regInteractor = new PilotRegistrationQueryInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await regInteractor.GetPilotRegistrationsForContest(contestId);

            // Empty results should be fine.
            Assert.IsFalse(result.IsFaulted);
            Assert.IsNull(result.Error);
        }

        [TestMethod]
        public async Task RegistrationQueryInteractor_GetPilotRegistrationsForContest_HappyPath()
        {
            // Make a valid contest id.
            string contestId = "234werwefsd";
            var pilotRegistrations = new List<PilotRegistration>
            {
                new PilotRegistration
                {
                    AirframeRegistrationNumbers = new List<string>{ "1", "2"},
                    AirframesSignedOff = true,
                    ContestId = contestId,
                    Id = "23wdafsdf",
                    IsPaid = true,
                    PilotId = "234234"
                }
            };

            mockPilotRegistrationRepository.Setup(prr => prr.ReadAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new Result<IEnumerable<PilotRegistration>>(pilotRegistrations)));

            var regInteractor = new PilotRegistrationQueryInteractor(mockPilotRegistrationRepository.Object, mockLogger.Object);
            var result = await regInteractor.GetPilotRegistrationsForContest(contestId);
            
            Assert.IsFalse(result.IsFaulted);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Value);
            Assert.IsFalse(string.IsNullOrEmpty(result.Value.First().PilotId));
            Assert.IsFalse(string.IsNullOrEmpty(result.Value.First().Id));
            Assert.AreEqual(2, result.Value.First().AirframeRegistrationNumbers.Count());
            Assert.IsTrue(result.Value.First().AirframesSignedOff);
            Assert.IsTrue(result.Value.First().IsPaid);
            Assert.AreEqual(contestId, result.Value.First().ContestId);
        }
    }
}
