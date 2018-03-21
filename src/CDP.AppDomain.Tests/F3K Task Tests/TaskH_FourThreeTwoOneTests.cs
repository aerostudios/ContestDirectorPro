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
    public class TaskH_FourThreeTwoOneTests
    {
        private TaskH_FourThreeTwoOne taskH = new TaskH_FourThreeTwoOne();
         
        [TestMethod]
        public void TaskH_FourThreeTwoOneTests_VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskH.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskH.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskH.Id));
            Assert.IsFalse(taskH.IsLandingScored);
            Assert.IsTrue(taskH.NumberOfTimeGatesAllowed == 4);
            Assert.IsTrue(taskH.ScoredTimeGates.Count == 4);
            Assert.IsTrue(taskH.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskH.Type == Contests.ContestType.F3K);
        }

        [TestMethod]
        public void TaskH_FourThreeTwoOneTests_VerifyTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskH.Id,
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
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 233)
                    }
                });

            Assert.IsTrue(taskH.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskH_FourThreeTwoOneTests_VerifyTask_FiveGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskH.Id,
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
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 233)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 23)
                    }
                });

            Assert.IsFalse(taskH.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskH_FourThreeTwoOneTests_VerifyTask_OverTime()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskH.Id,
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
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 183)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 233)
                    }
                });

            Assert.IsFalse(taskH.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskH_FourThreeTwoOneTests_ScoreTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskH.Id,
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
                        Time = new TimeSpan(0, 0, 120)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 3,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 233)
                    }
                });

            Assert.IsTrue(taskH.ScoreTask(roundScore) == 593);
        }
    }
}
