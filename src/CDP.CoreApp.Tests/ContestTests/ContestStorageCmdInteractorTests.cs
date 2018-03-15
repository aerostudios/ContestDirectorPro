using CDP.AppDomain;
using CDP.AppDomain.Contests;
using CDP.Common.Logging;
using CDP.CoreApp.Features.Contests.Commands;
using CDP.CoreApp.Interfaces.Contests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.ContestTests
{
    [TestClass]
    public class ContestStorageCmdInteractorTests
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
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ContestStorageCmdInteractor_CreateContestAsync_NullParameters()
        {
            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            await contestCmdInteractor.CreateContestAsync(null);
        }

        [TestMethod]
        public async Task ContestStorageCmdInteractor_CreateContestAsync_BadContestObjectParameter()
        {
            var contest = new Contest
            {
                Name = string.Empty,
                NumberOfFlyoffRounds = 0,
                Rounds = new Dictionary<int, Round>(),
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(1),
                Type = ContestType.F3K,
                Id = string.Empty
            };

            mockContestRepository.Setup(c => c.CreateAsync(It.IsAny<Contest>())).Returns<Contest>(null);

            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.CreateContestAsync(contest);

            // Name is empty
            Assert.IsTrue(result.IsFaulted);

            contest.Name = "foo";
            contest.NumberOfFlyoffRounds = -1;
            result = await contestCmdInteractor.CreateContestAsync(contest);

            // Contest Flyoff Rounds is out of range
            Assert.IsTrue(result.IsFaulted);

            contest.NumberOfFlyoffRounds = 0;
            contest.Rounds = null;
            result = await contestCmdInteractor.CreateContestAsync(contest);

            // Contest Rounds is out of range
            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task ContestStorageCmdInteractor_CreateContestAsync_RepositoryFailure()
        {
            var contestName = "Foo";
            var date = DateTimeOffset.Now;
            var contest = new Contest
            {
                Name = contestName,
                NumberOfFlyoffRounds = 0,
                Rounds = new Dictionary<int, Round>(),
                StartDate = date,
                EndDate = date.AddDays(1),
                Type = ContestType.F3K
            };

            mockContestRepository.Setup(c => c.CreateAsync(It.IsAny<Contest>())).Returns<Contest>(x => Task.FromResult(new Result<Contest>(null)));
            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.CreateContestAsync(contest);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public async Task ContestStorageCmdInteractor_CreateContestAsync_HappyPath()
        {
            var contestName = "Foo";
            var date = DateTimeOffset.Now;
            var contest = new Contest
            {
                Name = contestName,
                NumberOfFlyoffRounds = 0,
                Rounds = new Dictionary<int, Round>(),
                StartDate = date,
                EndDate = date.AddDays(1),
                Type = ContestType.F3K
            };

            mockContestRepository.Setup(c => c.CreateAsync(It.IsAny<Contest>())).Returns<Contest>(x => Task.FromResult(new Result<Contest>(contest)));
            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.CreateContestAsync(contest);

            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(contestName, result.Value.Name);
            Assert.AreEqual(contest.NumberOfFlyoffRounds, result.Value.NumberOfFlyoffRounds);
            Assert.AreEqual(date, result.Value.StartDate);
            Assert.AreEqual(contest.Rounds.Count, result.Value.Rounds.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ContestStorageCmdInteractor_UpdateContestAsync_NullParameters()
        {
            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            await contestCmdInteractor.UpdateContestAsync(null);
        }

        [TestMethod]
        public async Task ContestStorageCmdInteractor_UpdateContestAsync_BadContestObjectParameter()
        {
            var contest = new Contest
            {
                Name = string.Empty,
                NumberOfFlyoffRounds = 0,
                Rounds = new Dictionary<int, Round>(),
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(1),
                Type = ContestType.F3K,
                Id = "2323rsdfdfs"
            };

            mockContestRepository.Setup(c => c.UpdateAsync(It.IsAny<Contest>())).Returns<Contest>(null);

            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.UpdateContestAsync(contest);

            // Name is empty
            Assert.IsTrue(result.IsFaulted);

            contest.Name = "foo";
            contest.NumberOfFlyoffRounds = -1;
            result = await contestCmdInteractor.UpdateContestAsync(contest);

            // Contest Flyoff Rounds is out of range
            Assert.IsTrue(result.IsFaulted);

            contest.NumberOfFlyoffRounds = 0;
            contest.Rounds = null;
            result = await contestCmdInteractor.UpdateContestAsync(contest);

            // Contest Rounds is out of range
            Assert.IsTrue(result.IsFaulted);

            contest.Id = string.Empty;
            contest.Rounds = new Dictionary<int, Round>();
            result = await contestCmdInteractor.UpdateContestAsync(contest);

            // Contest ID is invalid
            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task ContestStorageCmdInteractor_UpdateContestAsync_HappyPath()
        {
            var id = "l23l4sdf";
            var contestName = "Foo";
            var date = DateTimeOffset.Now;
            var contest = new Contest
            {
                Name = contestName,
                NumberOfFlyoffRounds = 0,
                Rounds = new Dictionary<int, Round>(),
                StartDate = date,
                EndDate = date.AddDays(1),
                Type = ContestType.F3K,
                Id = id
            };

            mockContestRepository.Setup(c => c.UpdateAsync(It.IsAny<Contest>())).Returns<Contest>(x => Task.FromResult(new Result<Contest>(contest)));
            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.UpdateContestAsync(contest);

            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(id, result.Value.Id);
            Assert.AreEqual(contestName, result.Value.Name);
            Assert.AreEqual(contest.NumberOfFlyoffRounds, result.Value.NumberOfFlyoffRounds);
            Assert.AreEqual(date, result.Value.StartDate);
            Assert.AreEqual(contest.Rounds.Count, result.Value.Rounds.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ContestStorageCmdInteractor_DeleteContestAsync_NullParameters()
        {
            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            await contestCmdInteractor.DeleteContestAsync(null);
        }

        [TestMethod]
        public async Task ContestStorageCmdInteractor_DeleteContestAsync_BadContestObjectParameter()
        {
            var contest = new Contest
            {
                Name = string.Empty,
                NumberOfFlyoffRounds = 0,
                Rounds = new Dictionary<int, Round>(),
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(1),
                Type = ContestType.F3K,
                Id = "234wsdfs"
            };

            mockContestRepository.Setup(c => c.DeleteAsync(It.IsAny<string>())).Returns<bool>(x => Task.FromResult(new Result<bool>(false)));

            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.DeleteContestAsync(contest);

            // Name is empty
            Assert.IsTrue(result.IsFaulted);

            // Contest Flyoff Rounds is out of range
            contest.Name = "foo";
            contest.NumberOfFlyoffRounds = -1;
            result = await contestCmdInteractor.DeleteContestAsync(contest);

            Assert.IsTrue(result.IsFaulted);

            // Contest Rounds is out of range
            contest.NumberOfFlyoffRounds = 0;
            contest.Rounds = null;
            result = await contestCmdInteractor.DeleteContestAsync(contest);

            // Contest Rounds is out of range
            Assert.IsTrue(result.IsFaulted);

            contest.Id = string.Empty;
            contest.Rounds = new Dictionary<int, Round>();
            result = await contestCmdInteractor.DeleteContestAsync(contest);

            // Contest ID is invalid
            Assert.IsTrue(result.IsFaulted);
        }

        [TestMethod]
        public async Task ContestStorageCmdInteractor_DeleteContestAsync_HappyPath()
        {
            var id = "4354rs";
            var contestName = "Foo";
            var date = DateTimeOffset.Now;

            var contest = new Contest
            {
                Name = contestName,
                NumberOfFlyoffRounds = 0,
                Rounds = new Dictionary<int, Round>(),
                StartDate = date,
                EndDate = date.AddDays(1),
                Type = ContestType.F3K,
                Id = id
            };

            mockContestRepository.Setup(c => c.DeleteAsync(It.IsAny<string>())).Returns<string>(x => Task.FromResult(new Result<bool>(true)));
            var contestCmdInteractor = new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object);
            var result = await contestCmdInteractor.DeleteContestAsync(contest);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsTrue(result.Value);
        }
    }
}
