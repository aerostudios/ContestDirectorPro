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
    public class TaskD_LadderTests
    {
        private TaskD_Ladder taskD = new TaskD_Ladder();

        [TestMethod]
        public void VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskD.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskD.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskD.Id));
            Assert.IsFalse(taskD.IsLandingScored);
            Assert.IsTrue(taskD.NumberOfTimeGatesAllowed == 7);
            Assert.IsTrue(taskD.ScoredTimeGates.Count == 7);
            Assert.IsTrue(taskD.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskD.Type == Contests.ContestType.F3K);
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
                TaskId = taskD.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
            {
                new TimeGate
                {
                    Ordinal = 0,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 30)
                },
                new TimeGate
                {
                    Ordinal = 1,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 45)
                },
                new TimeGate
                {
                    Ordinal = 2,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 60)
                },
                new TimeGate
                {
                    Ordinal = 3,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 15)
                },
                new TimeGate
                {
                    Ordinal = 4,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 30)
                },
                new TimeGate
                {
                    Ordinal = 5,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 45)
                },
                new TimeGate
                {
                    Ordinal = 6,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 2, 0)
                },
            });

            Assert.IsTrue(taskD.ValidateTask(roundScore));
        }

        [TestMethod]
        public void VerifyTask_TwoInSameWindow()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskD.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
            {
                new TimeGate
                {
                    Ordinal = 0,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 30)
                },
                new TimeGate
                {
                    Ordinal = 1,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 45)
                },
                new TimeGate
                {
                    Ordinal = 2,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 45)
                },
                new TimeGate
                {
                    Ordinal = 3,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 15)
                },
                new TimeGate
                {
                    Ordinal = 4,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 30)
                },
                new TimeGate
                {
                    Ordinal = 5,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 45)
                },
                new TimeGate
                {
                    Ordinal = 6,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 2, 0)
                },
            });

            Assert.IsFalse(taskD.ValidateTask(roundScore));
        }

        [TestMethod]
        public void VerifyTask_CanUpShortOnOne()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskD.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
            {
                new TimeGate
                {
                    Ordinal = 0,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 30)
                },
                new TimeGate
                {
                    Ordinal = 1,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 45)
                },
                new TimeGate
                {
                    Ordinal = 2,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 60)
                },
                new TimeGate
                {
                    Ordinal = 3,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 15)
                },
                new TimeGate
                {
                    Ordinal = 4,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 29)  // This one.
                },
                new TimeGate
                {
                    Ordinal = 5,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 45)
                },
                new TimeGate
                {
                    Ordinal = 6,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 2, 0)
                },
            });

            Assert.IsFalse(taskD.ValidateTask(roundScore));
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
                TaskId = taskD.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
            {
                new TimeGate
                {
                    Ordinal = 0,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 30)
                },
                new TimeGate
                {
                    Ordinal = 1,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 45)
                },
                new TimeGate
                {
                    Ordinal = 2,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 0, 45)
                },
                new TimeGate
                {
                    Ordinal = 3,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 15)
                },
                new TimeGate
                {
                    Ordinal = 4,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 30)
                },
                new TimeGate
                {
                    Ordinal = 5,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 1, 45)
                },
                new TimeGate
                {
                    Ordinal = 6,
                    GateType = TimeGateType.Task,
                    Time = new TimeSpan(0, 2, 0)
                },
            });

            Assert.IsFalse(taskD.ScoreTask(roundScore) == 420);
        }
    }
}
