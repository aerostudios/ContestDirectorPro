using CDP.AppDomain;
using CDP.AppDomain.Contests;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Contests.Queries;
using CDP.CoreApp.Interfaces.Contests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.ContestTests
{
    [TestClass]
    public class ContestQueryTests
    {
        Mock<ILoggingService> mockLogger;
        Mock<IContestRepository> mockContestRepository;

        [TestInitialize]
        public void Setup()
        {
            mockContestRepository = new Mock<IContestRepository>();
            mockLogger = new Mock<ILoggingService>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ContestQueryInteractor_GetContestByNameAsync_NullParameters()
        {
            var ContestQueryInteractor = new ContestQueryInteractor(mockContestRepository.Object, mockLogger.Object);
            await ContestQueryInteractor.GetContestByNameAsync(null);
        }

        [TestMethod]
        public async Task ContestQueryInteractor_GetContestByNameAsync_RepositoryFailure_NullReturned()
        {
            var id = "234wdfsdfsa";
            var date = DateTimeOffset.Now;
            var name = "2017 SASS April";
            var numOfFlyoffRounds = 0;

            var contest = new Contest
            {
                Id = id,
                StartDate = date,
                Name = name,
                NumberOfFlyoffRounds = numOfFlyoffRounds,
                Rounds = new Dictionary<int, Round>()
            };

            mockContestRepository.Setup(c => c.ReadAsync(It.IsAny<string>())).Returns<Contest>(x => Task.FromResult(new Result<Contest>(null)));
            var contestQueryInteractor = new ContestQueryInteractor(mockContestRepository.Object, mockLogger.Object);

            var result = await contestQueryInteractor.GetContestByNameAsync(contest.Name);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ContestQueryInteractor_GetContestByNameAsync_RepositoryFailure_Exception()
        {
            var id = "345fswejsdf";
            var date = DateTimeOffset.Now;
            var name = "2017 SASS April";
            var rounds = new Dictionary<int, Round>();
            var numOfFlyoffRounds = 0;

            var contest = new Contest
            {
                Id = id,
                StartDate = date,
                Name = name,
                NumberOfFlyoffRounds = numOfFlyoffRounds,
                Rounds = rounds
            };
            
            var exceptionMessage = "Derp";

            mockContestRepository.Setup(c => c.ReadAsync(It.IsAny<string>())).Throws(new Exception(exceptionMessage));
            var contestQueryInteractor = new ContestQueryInteractor(mockContestRepository.Object, mockLogger.Object);

            var result = await contestQueryInteractor.GetContestByNameAsync(contest.Name);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
            Assert.IsNotNull(result.Error.Exception);
            Assert.AreEqual(exceptionMessage, result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task ContestQueryInteractor_GetContestByNameAsync_HappyPath()
        {
            var id = "sdlfkj32rsdf";
            var date = DateTimeOffset.Now;
            var name = "2017 SASS April";
            var rounds = new Dictionary<int, Round>();
            var numOfFlyoffRounds = 0;

            var contest = new Contest
            {
                Id = id,
                StartDate = date,
                Name = name,
                NumberOfFlyoffRounds = numOfFlyoffRounds,
                Rounds = rounds
            };


            mockContestRepository.Setup(c => c.ReadAsync(It.IsAny<string>())).Returns(Task.FromResult(new Result<Contest>(contest)));
            var ContestQueryInteractor = new ContestQueryInteractor(mockContestRepository.Object, mockLogger.Object);

            var result = await ContestQueryInteractor.GetContestByNameAsync(contest.Name);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(id, result.Value.Id);
            Assert.AreEqual(name, result.Value.Name);
            Assert.AreEqual(date, result.Value.StartDate);
            Assert.AreEqual(numOfFlyoffRounds, result.Value.NumberOfFlyoffRounds);
            Assert.AreEqual(rounds.Count, result.Value.Rounds.Count);
            Assert.AreEqual(0, result.Value.NumberOfPilots);
            Assert.AreEqual(1, result.Value.SuggestedNumberOfPilotsPerGroup);
        }

        [TestMethod]
        public async Task ContestQueryInteractor_GetAllContestsAsync_RepositoryFailure_ReturnsNull()
        {
            mockContestRepository.Setup(c => c.ReadAsync()).Returns(Task.FromResult(new Result<IEnumerable<Contest>>(null)));

            var contestQueryInteractor = new ContestQueryInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestQueryInteractor.GetAllContestsAsync();

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ContestQueryInteractor_GetAllContestsAsync_RepositoryFailure_Exception()
        {
            var exceptionMessage = "Derp";

            mockContestRepository.Setup(p => p.ReadAsync()).Throws(new Exception(exceptionMessage));
            var contestQueryInteractor = new ContestQueryInteractor(mockContestRepository.Object, mockLogger.Object);

            var result = await contestQueryInteractor.GetAllContestsAsync();

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
            Assert.IsNotNull(result.Error.Exception);
            Assert.AreEqual(exceptionMessage, result.Error.ErrorMessage);
        }

        [TestMethod]
        public async Task ContestQueryInteractor_GetAllContestsAsync_HappyPath()
        {
            var id = "dfasdjfasldf";
            var date = DateTimeOffset.Now;
            var name = "2017 SASS April";
            var rounds = new Dictionary<int, Round>();
            var numOfFlyoffRounds = 0;

            var contest = new Contest
            {
                Id = id,
                StartDate = date,
                Name = name,
                NumberOfFlyoffRounds = numOfFlyoffRounds,
                Rounds = rounds
            };

            var contest2 = new Contest
            {
                Id = "dfksjdwr",
                StartDate = DateTimeOffset.Now.AddDays(-1),
                Name = "2017 SASS May",
                Rounds = rounds,
                NumberOfFlyoffRounds = 1
            };

            var contests = new List<Contest> { contest, contest2 };

            mockContestRepository.Setup(c => c.ReadAsync()).Returns(Task.FromResult(new Result<IEnumerable<Contest>>(contests)));

            var contestQueryInteractor = new ContestQueryInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestQueryInteractor.GetAllContestsAsync();

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
