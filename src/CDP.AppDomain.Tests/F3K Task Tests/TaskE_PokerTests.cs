﻿using CDP.AppDomain.FlightMatrices;
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
    public class TaskE_PokerTests
    {
        private TaskE_Poker taskE = new TaskE_Poker();
        
        [TestMethod]
        public void TaskE_PokerTests_VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskE.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskE.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskE.Id));
            Assert.IsFalse(taskE.IsLandingScored);
            Assert.IsTrue(taskE.NumberOfTimeGatesAllowed == 5);
            Assert.IsTrue(taskE.ScoredTimeGates.Count == 5);
            Assert.IsTrue(taskE.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskE.Type == Contests.ContestType.F3K);
        }

        [TestMethod]
        public void TaskE_PokerTests_VerifyTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskE.Id,
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
                        Time = new TimeSpan(0, 0, 110)
                    }
                });

            Assert.IsTrue(taskE.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskE_PokerTests_VerifyTask_SixGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskE.Id,
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
                        Time = new TimeSpan(0, 0, 110)
                    },
                    new TimeGate
                    {
                        Ordinal = 5,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 110)
                    }
                });

            Assert.IsFalse(taskE.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskE_PokerTests_VerifyTask_TwoGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskE.Id,
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
                        Time = new TimeSpan(0, 0, 478)
                    }
                });

            Assert.IsTrue(taskE.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskE_PokerTests_VerifyTask_OneGate()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskE.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 558)
                    }
                });

            Assert.IsTrue(taskE.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskE_PokerTests_VerifyTask_OverTime()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskE.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 601)
                    }
                });

            Assert.IsFalse(taskE.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskE_PokerTests_ScoreTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskE.Id,
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
                        Time = new TimeSpan(0, 0, 110)
                    }
                });

            Assert.IsTrue(taskE.ScoreTask(roundScore) == 590);
        }
    }
}
