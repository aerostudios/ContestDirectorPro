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
            var contests = JsonConvert.DeserializeObject<List<Contest>>("[{\"Id\":\"4dd104ea-05e5-4cbf-93ae-026b2392da0d\",\"State\":{\"HasStarted\":true,\"CurrentRoundOrdinal\":0,\"CurrentFlightGroup\":2},\"SortingAlgoId\":null,\"SuggestedNumberOfPilotsPerGroup\":5,\"NumberOfFlyoffRounds\":0,\"Name\":\"SASS March 2018\",\"StartDate\":\"2018-03-11T20:57:25.3184212-07:00\",\"EndDate\":\"2018-03-11T20:57:25.3184295-07:00\",\"PilotRoster\":[\"fb191740-1608-4348-aa9f-59d7bdd75d22\",\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\",\"55213d52-9793-417d-8ceb-ed4b8d39bca4\",\"0868a622-8496-4726-baf7-f5aa97317e67\",\"58decef1-6f71-4153-abb2-1541145de16f\",\"27bf2738-72c2-4e68-ba30-459afb0db4b7\",\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\",\"01e91a11-3894-4d67-9700-04b9c94efa96\",\"b8303837-c425-407f-8d3a-d3a2a8596184\",\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\",\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\",\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\",\"173d8822-e311-4533-84ac-3c6d6d885cde\",\"24990720-d88b-496b-aa7a-152223c22c98\",\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\",\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\",\"1ca06e15-ed22-478f-9e50-04f58f39befe\"],\"Rounds\":{\"0\":{\"AssignedTaskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"},{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"},{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"},{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"}],\"B\":[{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"},{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"},{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"}],\"C\":[{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"},{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"},{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"},{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"},{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"}]},\"IsFlyOffRound\":false},\"1\":{\"AssignedTaskId\":\"7ee26489-a227-4bb0-9bf1-08cef9c72064\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"},{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"},{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"},{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"}],\"B\":[{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"},{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"},{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"}],\"C\":[{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"},{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"},{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"}]},\"IsFlyOffRound\":false},\"2\":{\"AssignedTaskId\":\"169b456c-9002-4c11-a86e-42c31fbe0133\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"},{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"},{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"}],\"B\":[{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"},{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"},{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"},{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"}],\"C\":[{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"},{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"},{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"}]},\"IsFlyOffRound\":false},\"3\":{\"AssignedTaskId\":\"da5f5ac0-92df-404d-9a4a-bdc0d678cc7c\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"},{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"},{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"}],\"B\":[{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"},{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"},{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"},{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"}],\"C\":[{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"},{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"},{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"},{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"}]},\"IsFlyOffRound\":false},\"4\":{\"AssignedTaskId\":\"5d1f2fe5-58de-4f3e-98f3-49d86b2e17f7\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"},{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"},{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"},{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"}],\"B\":[{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"},{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"},{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"},{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"}],\"C\":[{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"},{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"},{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"}]},\"IsFlyOffRound\":false},\"5\":{\"AssignedTaskId\":\"5d1f2fe5-58de-4f3e-98f3-49d86b2e17f7\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"},{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"},{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"},{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"}],\"B\":[{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"},{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"}],\"C\":[{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"},{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"},{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"},{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"},{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"}]},\"IsFlyOffRound\":false},\"6\":{\"AssignedTaskId\":\"a3403f43-b6b2-4fa2-a981-dd4b9f5baaef\",\"FlightGroups\":{\"A\":[{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"},{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"},{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"},{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"},{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"}],\"B\":[{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"},{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"},{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"}],\"C\":[{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"},{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"},{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"},{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"}]},\"IsFlyOffRound\":false}},\"AllowDroppedRound\":true,\"Type\":1,\"ContestRegistrationFee\":10.0}]");
            return contests.FirstOrDefault();
        }

        internal static List<Pilot> GetAllPilots()
        {
            return JsonConvert.DeserializeObject<List<Pilot>>("[{\"FirstName\":\"Rick\",\"LastName\":\"Rogahn\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"2342342\",\"Id\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\"},{\"FirstName\":\"Rick\",\"LastName\":\"Jay\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"0934234\",\"Id\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\"},{\"FirstName\":\"Dave\",\"LastName\":\"Banks\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"0934243\",\"Id\":\"55213d52-9793-417d-8ceb-ed4b8d39bca4\"},{\"FirstName\":\"Dan\",\"LastName\":\"Wright\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"3242923\",\"Id\":\"0868a622-8496-4726-baf7-f5aa97317e67\"},{\"FirstName\":\"Barry\",\"LastName\":\"Deviny\",\"Airframe\":\"Aerial\",\"AmaNumber\":\"2340923\",\"Id\":\"58decef1-6f71-4153-abb2-1541145de16f\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Jr\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\"},{\"FirstName\":\"Lee\",\"LastName\":\"Westerlund\",\"Airframe\":\"Flitz 3\",\"AmaNumber\":\"092342\",\"Id\":\"740e47c7-c9e6-4cb0-9d35-a33ec07e3a03\"},{\"FirstName\":\"Mike\",\"LastName\":\"Seid\",\"Airframe\":\"BAMF\",\"AmaNumber\":\"324234\",\"Id\":\"01e91a11-3894-4d67-9700-04b9c94efa96\"},{\"FirstName\":\"Jerry\",\"LastName\":\"Garcia\",\"Airframe\":\"LSD\",\"AmaNumber\":\"203942\",\"Id\":\"b8303837-c425-407f-8d3a-d3a2a8596184\"},{\"FirstName\":\"Bill\",\"LastName\":\"Gowan\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"093423\",\"Id\":\"7f36b84d-17c1-42d3-9a7b-fa19a414be7e\"},{\"FirstName\":\"Ray\",\"LastName\":\"Ernst\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"092342\",\"Id\":\"6082c90c-075a-43e2-bd04-5aa9ff685b4a\"},{\"FirstName\":\"Hedgehogs\",\"LastName\":\"Sr\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"342034\",\"Id\":\"ed0f43a2-0e1f-4eba-a384-ca26557a40dd\"},{\"FirstName\":\"Tim\",\"LastName\":\"Johnson\",\"Airframe\":\"Snipe\",\"AmaNumber\":\"23423\",\"Id\":\"173d8822-e311-4533-84ac-3c6d6d885cde\"},{\"FirstName\":\"Laurence\",\"LastName\":\"Doan\",\"Airframe\":\"CX3\",\"AmaNumber\":\"0923423\",\"Id\":\"24990720-d88b-496b-aa7a-152223c22c98\"},{\"FirstName\":\"Chris\",\"LastName\":\"Bloom\",\"Airframe\":\"Flitz\",\"AmaNumber\":\"234234\",\"Id\":\"89fb8bec-60ea-4b8a-9578-5610d692b2bf\"},{\"FirstName\":\"Matt\",\"LastName\":\"James\",\"Airframe\":\"Own Design\",\"AmaNumber\":\"234234\",\"Id\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\"},{\"FirstName\":\"Adam\",\"LastName\":\"Mathes\",\"Airframe\":\"Banjo\",\"AmaNumber\":\"234234\",\"Id\":\"1ca06e15-ed22-478f-9e50-04f58f39befe\"}]");
        }

        internal static Result<ContestScoresCollection> GetContestScores()
        {
            return new Result<ContestScoresCollection>(JsonConvert.DeserializeObject<ContestScoresCollection>("[{\"id\":\"7fd15c03-f551-4f74-972e-c53c2a0b4330\",\"contestId\":\"4dd104ea-05e5-4cbf-93ae-026b2392da0d\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"27bf2738-72c2-4e68-ba30-459afb0db4b7\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:04:40\",\"Ordinal\":0,\"GateType\":0}],\"score\":933.0,\"totalPenalties\":0.0},{\"id\":\"5c44653c-dfeb-497a-b6ff-4afa137a327e\",\"contestId\":\"4dd104ea-05e5-4cbf-93ae-026b2392da0d\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"58decef1-6f71-4153-abb2-1541145de16f\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:05:00\",\"Ordinal\":0,\"GateType\":0}],\"score\":1000.0,\"totalPenalties\":0.0},{\"id\":\"b858289d-44c6-46d8-8c1a-4a6b639002a7\",\"contestId\":\"4dd104ea-05e5-4cbf-93ae-026b2392da0d\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"b7a1cd19-8b45-4da6-aa64-6c4e237544db\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:03:23\",\"Ordinal\":0,\"GateType\":0}],\"score\":677.0,\"totalPenalties\":0.0},{\"id\":\"adbec24a-2745-4c2a-bb87-987eb8afc3bc\",\"contestId\":\"4dd104ea-05e5-4cbf-93ae-026b2392da0d\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"d9ee19bc-9ca9-48e4-ae71-0186dcf7c181\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:05:00\",\"Ordinal\":0,\"GateType\":0}],\"score\":1000.0,\"totalPenalties\":0.0},{\"id\":\"9721b5f9-3f78-49a1-b47c-fb87b30d099f\",\"contestId\":\"4dd104ea-05e5-4cbf-93ae-026b2392da0d\",\"taskId\":\"cf5227e2-fc4b-401e-bd98-24b25d9846bf\",\"pilotId\":\"fb191740-1608-4348-aa9f-59d7bdd75d22\",\"roundOrdinal\":0,\"flightGroup\":1,\"timeGates\":[{\"Time\":\"00:05:00\",\"Ordinal\":0,\"GateType\":0}],\"score\":1000.0,\"totalPenalties\":0.0}]"));
        }

        /// <summary>
        /// Gets the flight matrix.
        /// </summary>
        /// <returns></returns>
        internal static Result<FlightMatrix> GetFlightMatrix()
        {
            return new Result<FlightMatrix>(new FlightMatrix());
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