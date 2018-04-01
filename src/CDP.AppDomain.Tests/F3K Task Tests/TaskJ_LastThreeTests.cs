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
    public class TaskJ_LastThreeTests
    {
        private TaskJ_LastThree taskJ = new TaskJ_LastThree();

        [TestMethod]
        public void TaskJ_LastThreeTests_VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskJ.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskJ.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskJ.Id));
            Assert.IsFalse(taskJ.IsLandingScored);
            Assert.IsTrue(taskJ.NumberOfTimeGatesAllowed == 3);
            Assert.IsTrue(taskJ.ScoredTimeGates.Count == 3);
            Assert.IsTrue(taskJ.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskJ.Type == Contests.ContestType.F3K);
        }

        [TestMethod]
        public void TaskJ_LastThreeTests_VerifyTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskJ.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    }
                });

            Assert.IsTrue(taskJ.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskJ_LastThreeTests_VerifyTask_FourGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskJ.Id,
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
                        Time = new TimeSpan(0, 0, 200)
                    },
                });

            Assert.IsFalse(taskJ.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskJ_LastThreeTests_VerifyTask_OverTime()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskJ.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 181)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    }
                });

            Assert.IsFalse(taskJ.ValidateTask(roundScore));
        }

        [TestMethod]
        public void TaskJ_LastThreeTests_ScoreTask_HappyPath()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskJ.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    },
                    new TimeGate
                    {
                        Ordinal = 2,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 180)
                    }
                });

            Assert.IsTrue(taskJ.ScoreTask(roundScore) == 540);
        }
    }
}
