using CDP.AppDomain;
using CDP.AppDomain.Contests;
using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Pilots;
using CDP.AppDomain.Scoring;
using CDP.AppDomain.Tasks;
using CDP.AppDomain.Tasks.F3K;
using CDP.Common.Logging;
using CDP.CoreApp.Features.ContestEngine;
using CDP.CoreApp.Features.Contests.Commands;
using CDP.CoreApp.Features.FlightMatrices.Commands;
using CDP.CoreApp.Features.FlightMatrices.Queries;
using CDP.CoreApp.Features.Pilots.Queries;
using CDP.CoreApp.Features.Scoring.Commands;
using CDP.CoreApp.Features.Scoring.Queries;
using CDP.CoreApp.Features.Tasks.Queries;
using CDP.CoreApp.Interfaces.Contests;
using CDP.CoreApp.Interfaces.FlightMatrices;
using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
using CDP.CoreApp.Interfaces.Pilots;
using CDP.CoreApp.Interfaces.Scoring;
using CDP.CoreApp.Interfaces.Tasks;
using CDP.ScoringAndSortingImpl.F3K.Scoring;
using CDP.ScoringAndSortingImpl.F3K.Sorting;
using CDP.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDP.CoreApp.Tests.ContestEngineTests
{
    [TestClass]
    public class F3KContestEngineTests
    {
        private Mock<ITaskRepository> mockTaskRepository = new Mock<ITaskRepository>();
        private Mock<IScoringRepository> mockScoringRepository = new Mock<IScoringRepository>();
        private Mock<IContestRepository> mockContestRepository = new Mock<IContestRepository>();
        private Mock<IFlightMatrixRepository> mockFlightMatrixRepository = new Mock<IFlightMatrixRepository>();
        private Mock<IPilotRepository> mockPilotRepository = new Mock<IPilotRepository>();
        private Mock<IContestScoreAggregator> mockContestScoreAggrigator = new Mock<IContestScoreAggregator>();
        private Mock<ILoggingService> mockLogger = new Mock<ILoggingService>();
        private Mock<ISortingAlgo> mockAlgo = new Mock<ISortingAlgo>();

        [TestMethod]
        public async Task MoveNext_HappyPath_RandomSort()
        {
            var taskGenerator = new TaskGenerator();
            mockTaskRepository.Setup(tr => tr.ReadAsync(It.IsAny<ContestType>())).Returns(Task.FromResult(new Result<IEnumerable<TaskBase>>(taskGenerator.GetAllTasksAsList())));
            mockTaskRepository.Setup(tr => tr.ReadAsync(It.IsAny<string>()))
                .Returns<string>((id) => Task.FromResult(new Result<TaskBase>(taskGenerator.GetAllTasksAsList().Where(t => t.Id == id).Single())));
            
            var allPilots = ContestThatHasStartedData_RandomSort.GetAllPilots();
            mockPilotRepository.Setup(p => p.ReadAsync()).Returns(Task.FromResult(new Result<IEnumerable<Pilot>>(allPilots)));

            mockFlightMatrixRepository.Setup(fmr => fmr.ReadAsync(It.IsAny<string>())).Returns(Task.FromResult(ContestThatHasStartedData_RandomSort.GetFlightMatrix()));

            // We are testing random sort
            mockAlgo.Setup(algo => algo.IsSingleRoundSort()).Returns(false);

            var contestEngine = new F3KContestEngine(
                new TaskQueryInteractor(mockTaskRepository.Object, mockLogger.Object),
                new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object),
                new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object),
                new ScoringQueryInteractor(mockScoringRepository.Object, mockLogger.Object),
                new ScoringContestScoreAggInteractor(mockContestScoreAggrigator.Object, mockLogger.Object),
                new FlightMatrixStorageCmdInteractor(mockFlightMatrixRepository.Object, mockLogger.Object),
                new FlightMatrixQueryInteractor(mockFlightMatrixRepository.Object, mockLogger.Object),
                new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object),
                mockAlgo.Object,
                new Top10PilotsFlyoffsSort(),
                mockLogger.Object);

            var contest = ContestThatHasStartedData_RandomSort.GetContest();

            await contestEngine.Initialize(ContestThatHasStartedData_RandomSort.GetContest());

            Assert.AreEqual(0, contestEngine.CurrentRoundOrdinal);
            Assert.AreEqual(FlightGroup.B, contestEngine.CurrentFlightGroup);
            Assert.IsNotNull(contestEngine.CurrentTask as TaskA_LastFlightSevenMin);

            await contestEngine.MoveToNextContestStage();

            Assert.AreEqual(0, contestEngine.CurrentRoundOrdinal);
            Assert.AreEqual(FlightGroup.C, contestEngine.CurrentFlightGroup);

            // Set up the repository to return an appropriate updated contest for this test.
            var updatedContest = ContestThatHasStartedData_RandomSort.GetContest();
            updatedContest.State.CurrentFlightGroup = FlightGroup.A;
            updatedContest.State.CurrentRoundOrdinal = 1;
            mockContestRepository.Setup(cr => cr.UpdateAsync(It.IsAny<Contest>())).Returns(Task.FromResult<Result<Contest>>(new Result<Contest>(updatedContest)));

            await contestEngine.MoveToNextContestStage();

            Assert.AreEqual(1, contestEngine.CurrentRoundOrdinal);
            Assert.AreEqual(FlightGroup.A, contestEngine.CurrentFlightGroup);
            Assert.IsNotNull(contestEngine.CurrentTask as TaskB_LastTwoFlights4MinMax);
        }

        /// <summary>
        /// Moves the next happy path mo m sort.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task MoveNext_HappyPath_MoMSort()
        {
            var taskGenerator = new TaskGenerator();
            mockTaskRepository.Setup(tr => tr.ReadAsync(It.IsAny<string>()))
                .Returns<string>((id) => Task.FromResult(new Result<TaskBase>(taskGenerator.GetAllTasksAsList().Where(t => t.Id == id).Single())));
            mockTaskRepository.Setup(tr => tr.ReadAsync(It.IsAny<ContestType>()))
                .Returns(Task.FromResult(new Result<IEnumerable<TaskBase>>(taskGenerator.GetAllTasksAsList())));

            var allPilots = ContestThatHasStarted_MoMSort_FirstRound.GetPilots();
            mockPilotRepository.Setup(p => p.ReadAsync()).Returns(Task.FromResult(new Result<IEnumerable<Pilot>>(allPilots)));

            mockScoringRepository.Setup(s => s.ReadAsync(It.IsAny<string>())).Returns(Task.FromResult(new Result<ContestScoresCollection>(ContestThatHasStarted_MoMSort_FirstRound.GetScores())));

            var algo = new MoMSingleRoundSort();
            var contestScoreAggrigator = new ContestScoreAggregatorWithDrop();

            var contestEngine = new F3KContestEngine(
                new TaskQueryInteractor(mockTaskRepository.Object, mockLogger.Object),
                new ContestStorageCmdInteractor(mockContestRepository.Object, mockLogger.Object),
                new ScoringStorageCmdInteractor(mockScoringRepository.Object, mockLogger.Object),
                new ScoringQueryInteractor(mockScoringRepository.Object, mockLogger.Object),
                new ScoringContestScoreAggInteractor(contestScoreAggrigator, mockLogger.Object),
                new FlightMatrixStorageCmdInteractor(mockFlightMatrixRepository.Object, mockLogger.Object),
                new FlightMatrixQueryInteractor(mockFlightMatrixRepository.Object, mockLogger.Object),
                new PilotQueryInteractor(mockPilotRepository.Object, mockLogger.Object),
                algo,
                new Top10PilotsFlyoffsSort(),
                mockLogger.Object);

            await contestEngine.Initialize(ContestThatHasStarted_MoMSort_FirstRound.GetContest());

            Assert.AreEqual(0, contestEngine.CurrentRoundOrdinal);
            Assert.AreEqual(FlightGroup.C, contestEngine.CurrentFlightGroup);

            await contestEngine.MoveToNextContestStage();

            Assert.AreEqual(1, contestEngine.CurrentRoundOrdinal);
            Assert.AreEqual(FlightGroup.A, contestEngine.CurrentFlightGroup);
            //Assert.IsNotNull(contestEngine.Contest.);
        }
    }

    static class ContestThatHasStartedData_RandomSort
    {
        internal static Contest GetContest()
        {
            var contests = JsonConvert.DeserializeObject<List<Contest>>("[{\"Id\":\"44b80ec3-7737-44ab-8f95-5ce329c886a8\",\"State\":{\"HasStarted\":true,\"CurrentRoundOrdinal\":0,\"CurrentFlightGroup\":2},\"SortingAlgoId\":\"798743a0-7d7d-4f8c-9cee-9fa74c7c9972\",\"SuggestedNumberOfPilotsPerGroup\":4,\"NumberOfFlyoffRounds\":2,\"FlyOffSelectionAlgoId\":\"579f6752-d620-4ebf-ad85-5d6f17fc1f60\",\"Name\":\"SASS March 2018\",\"StartDate\":\"2018-03-24T10:02:25.4315585-07:00\",\"EndDate\":\"2018-03-24T10:02:25.431856-07:00\",\"PilotRoster\":[\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"0801c060-0af4-4225-abba-d8bf952194d2\",\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"de346d10-4529-430c-bf9c-97dc214070e2\",\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"330e2e97-0fdd-493b-a703-6774a5a735e7\"],\"Rounds\":{\"0\":{\"AssignedTaskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"},{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"}],\"B\":[{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"},{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"}],\"C\":[{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"},{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"},{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]},\"IsFlyOffRound\":false},\"1\":{\"AssignedTaskId\":\"7ee26489-a227-4bb0-9bf1-08cef9c72064\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"},{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"}],\"B\":[{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"},{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"}],\"C\":[{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"},{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"},{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]},\"IsFlyOffRound\":false},\"2\":{\"AssignedTaskId\":\"da5f5ac0-92df-404d-9a4a-bdc0d678cc7c\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"},{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"}],\"B\":[{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"},{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"}],\"C\":[{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"},{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"},{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]},\"IsFlyOffRound\":false},\"3\":{\"AssignedTaskId\":\"5d1f2fe5-58de-4f3e-98f3-49d86b2e17f7\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"},{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"}],\"B\":[{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"},{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"}],\"C\":[{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"},{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"},{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]},\"IsFlyOffRound\":false},\"4\":{\"AssignedTaskId\":\"a3403f43-b6b2-4fa2-a981-dd4b9f5baaef\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"},{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"}],\"B\":[{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"},{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"}],\"C\":[{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"},{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"},{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]},\"IsFlyOffRound\":false},\"5\":{\"AssignedTaskId\":\"e030c71a-5b54-47ed-b4a5-f0dcdb03efff\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"},{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"}],\"B\":[{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"},{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"}],\"C\":[{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"},{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"},{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]},\"IsFlyOffRound\":false},\"6\":{\"AssignedTaskId\":\"169b456c-9002-4c11-a86e-42c31fbe0133\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"},{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"}],\"B\":[{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"},{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"}],\"C\":[{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"},{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"},{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]},\"IsFlyOffRound\":false},\"7\":{\"AssignedTaskId\":\"5d1f2fe5-58de-4f3e-98f3-49d86b2e17f7\",\"FlightGroups\":{},\"IsFlyOffRound\":true},\"8\":{\"AssignedTaskId\":\"622c9966-fbae-4d86-8389-ad9cccc9397d\",\"FlightGroups\":{},\"IsFlyOffRound\":true}},\"AllowDroppedRound\":true,\"Type\":1,\"ContestRegistrationFee\":10.0}]");
            return contests.FirstOrDefault();
        }

        internal static List<Pilot> GetAllPilots()
        {
            return JsonConvert.DeserializeObject<List<Pilot>>("[{\"FirstName\":\"Rick\",\"LastName\":\"James\",\"Airframe\":\"Snipe\",\"StandardsBodyId\":\"234234\",\"Id\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\"},{\"FirstName\":\"Albert\",\"LastName\":\"Uno\",\"Airframe\":\"V-2 Rocket\",\"StandardsBodyId\":\"234223\",\"Id\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\"},{\"FirstName\":\"Yorick\",\"LastName\":\"VI\",\"Airframe\":\"Aerobee Rocket\",\"StandardsBodyId\":\"23093\",\"Id\":\"0801c060-0af4-4225-abba-d8bf952194d2\"},{\"FirstName\":\"Able\",\"LastName\":\"Mitchell\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"09823\",\"Id\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\"},{\"FirstName\":\"Gordo\",\"LastName\":\"Reliable\",\"Airframe\":\"Jupiter AM-13\",\"StandardsBodyId\":\"43523\",\"Id\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\"},{\"FirstName\":\"Miss\",\"LastName\":\"Baker\",\"Airframe\":\"Jupiter AM-18\",\"StandardsBodyId\":\"63452\",\"Id\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\"},{\"FirstName\":\"Enos\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"8324\",\"Id\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\"},{\"FirstName\":\"Bonny\",\"LastName\":\"Macaque\",\"Airframe\":\"Biosatellite 3\",\"StandardsBodyId\":\"83242\",\"Id\":\"de346d10-4529-430c-bf9c-97dc214070e2\"},{\"FirstName\":\"Goliath\",\"LastName\":\"Boom\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"8324\",\"Id\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\"},{\"FirstName\":\"Sam\",\"LastName\":\"Joe\",\"Airframe\":\"Little Joe 2\",\"StandardsBodyId\":\"98234\",\"Id\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\"},{\"FirstName\":\"Ham\",\"LastName\":\"Chimp\",\"Airframe\":\"Mercury\",\"StandardsBodyId\":\"09324\",\"Id\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\"},{\"FirstName\":\"Scat\",\"LastName\":\"Back\",\"Airframe\":\"Atlas Rocket\",\"StandardsBodyId\":\"09823\",\"Id\":\"55205178-c4d4-4f1d-8969-bb72beabb805\"},{\"FirstName\":\"Albert\",\"LastName\":\"Dos\",\"Airframe\":\"V-2\",\"StandardsBodyId\":\"234234\",\"Id\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\"}]");
        }

        internal static Result<ContestScoresCollection> GetContestScores()
        {
            return new Result<ContestScoresCollection>(JsonConvert.DeserializeObject<ContestScoresCollection>("[{\"id\":\"a1dcac8e-6cf8-4746-a65d-614d7fa06282\",\"contestId\":\"44b80ec3-7737-44ab-8f95-5ce329c886a8\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:05:00\",\"Ordinal\":0,\"GateType\":0}],\"score\":1000.0,\"totalPenalties\":0.0},{\"id\":\"b7db9e18-a2e9-448c-8539-979a30cf9e5b\",\"contestId\":\"44b80ec3-7737-44ab-8f95-5ce329c886a8\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:04:23\",\"Ordinal\":0,\"GateType\":0}],\"score\":877.0,\"totalPenalties\":0.0},{\"id\":\"596f5d5c-1edb-434a-8a26-adf2d099b516\",\"contestId\":\"44b80ec3-7737-44ab-8f95-5ce329c886a8\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:03:50\",\"Ordinal\":0,\"GateType\":0}],\"score\":767.0,\"totalPenalties\":0.0},{\"id\":\"cd7202ad-09fe-4682-943a-60c30748dec1\",\"contestId\":\"44b80ec3-7737-44ab-8f95-5ce329c886a8\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:05:00\",\"Ordinal\":0,\"GateType\":0}],\"score\":1000.0,\"totalPenalties\":0.0}]"));
        }

        /// <summary>
        /// Gets the flight matrix.
        /// </summary>
        /// <returns></returns>
        internal static Result<FlightMatrix> GetFlightMatrix()
        {
            return new Result<FlightMatrix>(JsonConvert.DeserializeObject<FlightMatrix>("{\"ContestId\":\"44b80ec3-7737-44ab-8f95-5ce329c886a8\",\"Id\":\"00b67f5e-dbdb-4895-8429-24a285fc0136\",\"Matrix\":[{\"RoundOrdinal\":0,\"PilotSlots\":[{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"FlightGroup\":1},{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"FlightGroup\":1},{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"FlightGroup\":1},{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"FlightGroup\":1},{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"FlightGroup\":2},{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"FlightGroup\":2},{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"FlightGroup\":2},{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"FlightGroup\":2},{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"FlightGroup\":3},{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"FlightGroup\":3},{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"FlightGroup\":3},{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"FlightGroup\":3},{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"FlightGroup\":3}]},{\"RoundOrdinal\":1,\"PilotSlots\":[{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"FlightGroup\":1},{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"FlightGroup\":1},{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"FlightGroup\":1},{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"FlightGroup\":1},{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"FlightGroup\":2},{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"FlightGroup\":2},{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"FlightGroup\":2},{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"FlightGroup\":2},{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"FlightGroup\":3},{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"FlightGroup\":3},{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"FlightGroup\":3},{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"FlightGroup\":3},{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"FlightGroup\":3}]},{\"RoundOrdinal\":2,\"PilotSlots\":[{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"FlightGroup\":1},{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"FlightGroup\":1},{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"FlightGroup\":1},{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"FlightGroup\":1},{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"FlightGroup\":2},{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"FlightGroup\":2},{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"FlightGroup\":2},{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"FlightGroup\":2},{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"FlightGroup\":3},{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"FlightGroup\":3},{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"FlightGroup\":3},{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"FlightGroup\":3},{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"FlightGroup\":3}]},{\"RoundOrdinal\":3,\"PilotSlots\":[{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"FlightGroup\":1},{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"FlightGroup\":1},{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"FlightGroup\":1},{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"FlightGroup\":1},{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"FlightGroup\":2},{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"FlightGroup\":2},{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"FlightGroup\":2},{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"FlightGroup\":2},{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"FlightGroup\":3},{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"FlightGroup\":3},{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"FlightGroup\":3},{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"FlightGroup\":3},{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"FlightGroup\":3}]},{\"RoundOrdinal\":4,\"PilotSlots\":[{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"FlightGroup\":1},{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"FlightGroup\":1},{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"FlightGroup\":1},{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"FlightGroup\":1},{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"FlightGroup\":2},{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"FlightGroup\":2},{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"FlightGroup\":2},{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"FlightGroup\":2},{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"FlightGroup\":3},{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"FlightGroup\":3},{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"FlightGroup\":3},{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"FlightGroup\":3},{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"FlightGroup\":3}]},{\"RoundOrdinal\":5,\"PilotSlots\":[{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"FlightGroup\":1},{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"FlightGroup\":1},{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"FlightGroup\":1},{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"FlightGroup\":1},{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"FlightGroup\":2},{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"FlightGroup\":2},{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"FlightGroup\":2},{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"FlightGroup\":2},{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"FlightGroup\":3},{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"FlightGroup\":3},{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"FlightGroup\":3},{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"FlightGroup\":3},{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"FlightGroup\":3}]},{\"RoundOrdinal\":6,\"PilotSlots\":[{\"PilotId\":\"5adb60aa-6599-4292-8cb9-eeffbf2b8416\",\"FlightGroup\":1},{\"PilotId\":\"55205178-c4d4-4f1d-8969-bb72beabb805\",\"FlightGroup\":1},{\"PilotId\":\"74cb37e4-b607-4217-aaec-d8d56c11242e\",\"FlightGroup\":1},{\"PilotId\":\"1e5a2df0-1b6a-4db6-813b-bab26817054d\",\"FlightGroup\":1},{\"PilotId\":\"ae33b5f5-f5f4-4fd3-9873-8f250a9ba182\",\"FlightGroup\":2},{\"PilotId\":\"066ce68c-a1b3-4dea-b445-125d965f6e84\",\"FlightGroup\":2},{\"PilotId\":\"9e34fe28-157e-45ab-8995-3f3f41d19891\",\"FlightGroup\":2},{\"PilotId\":\"606f4ae4-a1ae-44bb-9faa-6aed2dabe55f\",\"FlightGroup\":2},{\"PilotId\":\"de346d10-4529-430c-bf9c-97dc214070e2\",\"FlightGroup\":3},{\"PilotId\":\"0801c060-0af4-4225-abba-d8bf952194d2\",\"FlightGroup\":3},{\"PilotId\":\"a86117e2-0eac-4f38-97f8-60f39d4df565\",\"FlightGroup\":3},{\"PilotId\":\"e73f8d6d-515a-4b98-bef7-672c5db99de1\",\"FlightGroup\":3},{\"PilotId\":\"330e2e97-0fdd-493b-a703-6774a5a735e7\",\"FlightGroup\":3}]}]}"));
        }
    }


    static class ContestThatHasStarted_MoMSort_FirstRound
    {
        private static ISortingAlgo sortingAlgo = new MoMSingleRoundSort();
        private static List<Pilot> pilotsInContest = PilotGenerator.GetBasicPilotList(9);
        private static string contestId = Guid.NewGuid().ToString();

        internal static Contest GetContest()
        {
            var returnVal = new Contest
            {
                AllowDroppedRound = false,
                ContestRegistrationFee = 10.00,
                EndDate = DateTimeOffset.Now.AddDays(1),
                Id = contestId,
                Name = "SASS March 2018",
                NumberOfFlyoffRounds = 0,
                SortingAlgoId = sortingAlgo.GetUniqueId(),
                StartDate = DateTimeOffset.Now,
                State = new ContestState
                {
                    CurrentFlightGroup = FlightGroup.C,
                    CurrentRoundOrdinal = 0,
                    HasStarted = true
                },
                SuggestedNumberOfPilotsPerGroup = 3,
                Type = ContestType.F3K
            };

            returnVal.PilotRoster = pilotsInContest.Select(p => p.Id).ToList();
            returnVal.Rounds = RoundsGenerator.GenerateRounds_NoFlightGroups(7);

            returnVal.Rounds[0].FlightGroups = new Dictionary<FlightGroup, List<Pilot>>
                {
                    { FlightGroup.A, pilotsInContest.Where(p => Convert.ToInt32(p.Id) < 3).ToList() },
                    { FlightGroup.B, pilotsInContest.Where(p => Convert.ToInt32(p.Id) >= 3 && Convert.ToInt32(p.Id) < 6).ToList() },
                    { FlightGroup.C, pilotsInContest.Where(p => Convert.ToInt32(p.Id) >= 6).ToList() }
                };

            return returnVal;
        }

        internal static List<Pilot> GetPilots() => pilotsInContest;

        internal static ContestScoresCollection GetScores()
        {
            return new ContestScoresCollection
                {
                    new ContestRoundScoresCollection(new List<TimeSheet>
                    {
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.A,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[0].Id,
                            RoundOrdinal = 0,
                            Score = 1000,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.A,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[1].Id,
                            RoundOrdinal = 0,
                            Score = 789,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.A,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[2].Id,
                            RoundOrdinal = 0,
                            Score = 321,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.B,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[3].Id,
                            RoundOrdinal = 0,
                            Score = 333,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.B,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[4].Id,
                            RoundOrdinal = 0,
                            Score = 900,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.B,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[5].Id,
                            RoundOrdinal = 0,
                            Score = 1000,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.C,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[6].Id,
                            RoundOrdinal = 0,
                            Score = 1000,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.C,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[7].Id,
                            RoundOrdinal = 0,
                            Score = 932,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        },
                        new TimeSheet
                        {
                            ContestId = contestId,
                            FlightGroup = FlightGroup.C,
                            Id = Guid.NewGuid().ToString(),
                            PilotId = pilotsInContest[8].Id,
                            RoundOrdinal = 0,
                            Score = 532,
                            TaskId = Guid.NewGuid().ToString(),
                            TimeGates = new List<TimeGate>(),
                            TotalPenalties = 0
                        }
                    }, 0)
                };
        }
    }
}