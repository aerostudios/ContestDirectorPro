using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Scoring;
using CDP.AppDomain.Tasks;
using CDP.AppDomain.Tasks.F3K;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDP.AppDomain.Tests.F3K_Task_Tests
{
    [TestClass]
    public class TaskK_BigLadderTests
    {
        private TaskK_BigLadder taskK = new TaskK_BigLadder();

        [TestMethod]
        public void TaskK_BigLadderTests_VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskK.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskK.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskK.Id));
            Assert.IsFalse(taskK.IsLandingScored);
            Assert.IsTrue(taskK.NumberOfTimeGatesAllowed == 5);
            Assert.IsTrue(taskK.ScoredTimeGates.Count == 5);
            Assert.IsTrue(taskK.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskK.Type == Contests.ContestType.F3K);
        }

        [TestMethod]
        public void TaskK_BigLadderTests_VerifyTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskK.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 60)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 90)
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
                        Time = new TimeSpan(0, 0, 150)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    }
                });

            Assert.IsTrue(taskK.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskK_BigLadderTests_VerifyTask_SixGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskK.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 60)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 90)
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
                        Time = new TimeSpan(0, 0, 150)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 5,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 1)
                    }
                });

            Assert.IsFalse(taskK.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskK_BigLadderTests_VerifyTask_OverTime()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskK.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 60)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 90)
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
                        Time = new TimeSpan(0, 0, 150)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 181)
                    }
                });

            Assert.IsFalse(taskK.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskK_BigLadderTests_ScoreTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskK.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 60)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 90)
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
                        Time = new TimeSpan(0, 0, 150)
                    },
                    new TimeGate
                    {
                        Ordinal = 4,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    }
                });

            Assert.IsTrue(taskK.ScoreTask(roundScore) == 600);
        }
    }
}
