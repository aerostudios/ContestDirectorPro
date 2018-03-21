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
    public class TaskG_FiveTwosTests
    {
        private TaskG_FiveTwos taskG = new TaskG_FiveTwos();

        [TestMethod]
        public void TaskG_FiveTwosTests_VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskG.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskG.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskG.Id));
            Assert.IsFalse(taskG.IsLandingScored);
            Assert.IsTrue(taskG.NumberOfTimeGatesAllowed == 5);
            Assert.IsTrue(taskG.ScoredTimeGates.Count == 5);
            Assert.IsTrue(taskG.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskG.Type == Contests.ContestType.F3K);
        }

        [TestMethod]
        public void TaskG_FiveTwosTests_VerifyTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskG.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 112)
                    }
                });

            Assert.IsTrue(taskG.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskG_FiveTwosTests_VerifyTask_SixGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskG.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 112)
                    },
                    new TimeGate
                    {
                        Ordinal = 5,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 112)
                    }
                });

            Assert.IsFalse(taskG.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskG_FiveTwosTests_VerifyTask_OverTime()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskG.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 121)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 112)
                    }
                });

            Assert.IsFalse(taskG.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskG_FiveTwosTests_ScoreTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskG.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 112)
                    }
                });

            Assert.IsTrue(taskG.ScoreTask(roundScore) == 592);
        }
    }
}
