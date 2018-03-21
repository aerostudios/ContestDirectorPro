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
    public class TaskB_LastTwoFlights4MinMaxTests
    {
        private TaskB_LastTwoFlights4MinMax taskB = new TaskB_LastTwoFlights4MinMax();

        [TestMethod]
        public void VerifyBasicProperties()
        {
            Assert.IsFalse(string.IsNullOrEmpty(taskB.Description));
            Assert.IsFalse(string.IsNullOrEmpty(taskB.Name));
            Assert.IsFalse(string.IsNullOrEmpty(taskB.Id));
            Assert.IsFalse(taskB.IsLandingScored);
            Assert.IsTrue(taskB.NumberOfTimeGatesAllowed == 2);
            Assert.IsTrue(taskB.ScoredTimeGates.Count == 2);
            Assert.IsTrue(taskB.TaskFlightWindows.Count == 1);
            Assert.IsTrue(taskB.Type == Contests.ContestType.F3K);
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
                TaskId = taskB.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 240)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 240)
                    }
                });

            Assert.IsTrue(taskB.ValidateTask(roundScore));
        }

        [TestMethod]
        public void VerifyTask_ThreeGates()
        {
            var roundScore = new TimeSheet
            {
                ContestId = "asdfasf",
                FlightGroup = FlightGroup.A,
                Id = "2342342",
                PilotId = "23423423",
                RoundOrdinal = 0,
                Score = 0,
                TaskId = taskB.Id,
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
                    },
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 15)
                    },
                });

            Assert.IsFalse(taskB.ValidateTask(roundScore));
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
                TaskId = taskB.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 240)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 241)
                    }
                });

            Assert.IsFalse(taskB.ValidateTask(roundScore));
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
                TaskId = taskB.Id,
                TotalPenalties = 0
            };

            roundScore.TimeGates.AddRange(new List<TimeGate>
                {
                    new TimeGate
                    {
                        Ordinal = 0,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 240)
                    },
                    new TimeGate
                    {
                        Ordinal = 1,
                        GateType = TimeGateType.Task,
                        Time = new TimeSpan(0, 0, 240)
                    }
                });

            Assert.IsTrue(taskB.ScoreTask(roundScore) == 480);
        }
    }
}
