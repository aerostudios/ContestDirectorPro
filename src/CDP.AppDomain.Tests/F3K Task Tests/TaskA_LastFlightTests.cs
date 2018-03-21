using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Scoring;
using CDP.AppDomain.Tasks;
using CDP.AppDomain.Tasks.F3K;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CDP.AppDomain.Tests.F3K_Task_Tests
{
    [TestClass]
    public class TaskA_LastFlightTests
    {
        private TaskA_LastFlightSevenMin taskA = new TaskA_LastFlightSevenMin();

        [TestMethod]
        public void VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskA.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskA.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskA.Id));
            Assert.IsFalse(taskA.IsLandingScored);
            Assert.IsTrue(taskA.NumberOfTimeGatesAllowed == 1);
            Assert.IsTrue(taskA.ScoredTimeGates.Count == 1);
            Assert.IsTrue(taskA.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskA.Type == Contests.ContestType.F3K);
        }

        [TestMethod]
        public void VerifyTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskA.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.Add(
                new TimeGate
                {
                    Ordinal = 0,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 300)
                });

            Assert.IsTrue(taskA.ValidateTask(roundScore));
        }

        [TestMethod]
        public void VerifyTask_TwoGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskA.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 15)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 300)
                    }
                });

            Assert.IsFalse(taskA.ValidateTask(roundScore));
        }

        [TestMethod]
        public void VerifyTask_OverTime()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskA.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.Add(
                new TimeGate
                {
                    Ordinal = 0,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 301)
                });

            Assert.IsFalse(taskA.ValidateTask(roundScore));
        }

        [TestMethod]
        public void ScoreTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskA.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.Add(
                new TimeGate
                {
                    Ordinal = 0,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 145)
                });

            Assert.IsTrue(taskA.ScoreTask(roundScore) == 145);
        }
    }
}
