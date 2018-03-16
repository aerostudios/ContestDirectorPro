using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Scoring;
using CDP.CoreApp.Features.Scoring;
using CDP.CoreApp.Interfaces.Scoring;
using CDP.ScoringAndSortingImpl.F3K.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace CDP.ScoringAndSortingImpl.F3K.Tests
{
    [TestClass]
    public class MoMSingleRoundSortTests
    {
        Mock<IContestScoreAggregator> mockContestAgg = new Mock<IContestScoreAggregator>();

        [TestMethod]
        public void MoMSingleRoundSort_TwoPilotsPerGrounp_SevenPilots()
        {
            var momSort = new MoMSingleRoundSort();

            //// Holds all of the contest scores by round
            //var contestScores = new ContestScoresCollection(
            //    // Create the container for all the Rounds
            //    new List<ContestRoundScoresCollection>
            //    {
            //        // Create the First round.
            //        new ContestRoundScoresCollection(
            //            new List<TimeSheet>
            //            {
            //                new TimeSheet
            //                {
            //                    ContestId = "1",
            //                    PilotId = "Pilot1",
            //                    FlightGroup = FlightGroup.A,
            //                    Score = 893,
            //                    RoundOrdinal = 0
            //                },

            //                new TimeSheet
            //                {
            //                    ContestId = "1",
            //                    PilotId = "Pilot2",
            //                    FlightGroup = FlightGroup.A,
            //                    Score = 762,
            //                    RoundOrdinal = 0
            //                },

            //                new TimeSheet
            //                {
            //                    ContestId = "1",
            //                    PilotId = "Pilot3",
            //                    FlightGroup = FlightGroup.A,
            //                    Score = 934,
            //                    RoundOrdinal = 0
            //                },

            //                new TimeSheet
            //                {
            //                    ContestId = "1",
            //                    PilotId = "Pilot4",
            //                    FlightGroup = FlightGroup.B,
            //                    Score = 553,
            //                    RoundOrdinal = 0
            //                },

            //                new TimeSheet
            //                {
            //                    ContestId = "1",
            //                    PilotId = "Pilot5",
            //                    FlightGroup = FlightGroup.B,
            //                    Score = 893,
            //                    RoundOrdinal = 0
            //                },

            //                new TimeSheet
            //                {
            //                    ContestId = "1",
            //                    PilotId = "Pilot6",
            //                    FlightGroup = FlightGroup.B,
            //                    Score = 300,
            //                    RoundOrdinal = 0
            //                },

            //                new TimeSheet
            //                {
            //                    ContestId = "1",
            //                    PilotId = "Pilot7",
            //                    FlightGroup = FlightGroup.B,
            //                    Score = 999,
            //                    RoundOrdinal = 0
            //                }
            //            }
            //            , 0)
            //    });

            //mockContestAgg.Setup(ca => ca.GenerateContestScores(It.IsAny<ContestScoresCollection>())).Returns();

            var scores = new Dictionary<string, PilotContestScoreCollection>
            {
                {
                    "Pilot1",
                    new PilotContestScoreCollection("Pilot1", 893, new Dictionary<int, double>())
                },
                {
                    "Pilot2",
                    new PilotContestScoreCollection("Pilot2", 762, new Dictionary<int, double>())
                },
                {
                    "Pilot3",
                    new PilotContestScoreCollection("Pilot3", 934, new Dictionary<int, double>())
                },
                {
                    "Pilot4",
                    new PilotContestScoreCollection("Pilot4", 553, new Dictionary<int, double>())
                },
                {
                    "Pilot5",
                    new PilotContestScoreCollection("Pilot5", 893, new Dictionary<int, double>())
                },
                {
                    "Pilot6",
                    new PilotContestScoreCollection("Pilot6", 300, new Dictionary<int, double>())
                },
                {
                    "Pilot7",
                    new PilotContestScoreCollection("Pilot7", 999, new Dictionary<int, double>())
                }
            };

            var result = momSort.SortSingleRound(scores, 2);

            Assert.IsNotNull(result);
             Assert.AreEqual("Pilot7", result.First().Key);
            Assert.AreEqual(2, result.Count(x => x.Value == FlightGroup.A));
            Assert.AreEqual(2, result.Count(x => x.Value == FlightGroup.B));
            Assert.AreEqual(3, result.Count(x => x.Value == FlightGroup.C));
        }

        [TestMethod]
        public void MoMSingleRoundSort_ThreePilotsPerGrounp_SevenPilots()
        {
            var momSort = new MoMSingleRoundSort();

            //var scoresList = new List<RoundScoreBase>
            //{
            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot1",
            //        FlightGroup = FlightGroup.A,
            //        Score = 893,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot2",
            //        FlightGroup = FlightGroup.A,
            //        Score = 762,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot3",
            //        FlightGroup = FlightGroup.A,
            //        Score = 934,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot4",
            //        FlightGroup = FlightGroup.B,
            //        Score = 553,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot5",
            //        FlightGroup = FlightGroup.B,
            //        Score = 893,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot6",
            //        FlightGroup = FlightGroup.B,
            //        Score = 300,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot7",
            //        FlightGroup = FlightGroup.B,
            //        Score = 1000,
            //        RoundOrdinal = 0
            //    }
            //};

            var scores = new Dictionary<string, PilotContestScoreCollection>
            {
                {
                    "Pilot1",
                    new PilotContestScoreCollection("Pilot1", 893, new Dictionary<int, double>())
                },
                {
                    "Pilot2",
                    new PilotContestScoreCollection("Pilot2", 762, new Dictionary<int, double>())
                },
                {
                    "Pilot3",
                    new PilotContestScoreCollection("Pilot3", 934, new Dictionary<int, double>())
                },
                {
                    "Pilot4",
                    new PilotContestScoreCollection("Pilot4", 553, new Dictionary<int, double>())
                },
                {
                    "Pilot5",
                    new PilotContestScoreCollection("Pilot5", 893, new Dictionary<int, double>())
                },
                {
                    "Pilot6",
                    new PilotContestScoreCollection("Pilot6", 300, new Dictionary<int, double>())
                },
                {
                    "Pilot7",
                    new PilotContestScoreCollection("Pilot7", 1000, new Dictionary<int, double>())
                }
            };

            var result = momSort.SortSingleRound(scores, 3);

            Assert.IsNotNull(result);
            Assert.AreEqual("Pilot7", result.First().Key);
            Assert.AreEqual(3, result.Count(x => x.Value == FlightGroup.A));
            Assert.AreEqual(4, result.Count(x => x.Value == FlightGroup.B));
            // No flight group C
            Assert.AreEqual(0, result.Count(x => x.Value == FlightGroup.C));
        }

        [TestMethod]
        public void MoMSingleRoundSort_FourPilotsPerGrounp_SevenPilots()
        {
            var momSort = new MoMSingleRoundSort();

            //var scoresList = new List<RoundScoreBase>
            //{
            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot1",
            //        FlightGroup = FlightGroup.A,
            //        Score = 893,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot2",
            //        FlightGroup = FlightGroup.A,
            //        Score = 762,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot3",
            //        FlightGroup = FlightGroup.A,
            //        Score = 934,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot4",
            //        FlightGroup = FlightGroup.B,
            //        Score = 553,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot5",
            //        FlightGroup = FlightGroup.B,
            //        Score = 893,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot6",
            //        FlightGroup = FlightGroup.B,
            //        Score = 300,
            //        RoundOrdinal = 0
            //    },

            //    new TimeSheet
            //    {
            //        ContestId = "1",
            //        PilotId = "Pilot7",
            //        FlightGroup = FlightGroup.B,
            //        Score = 1000,
            //        RoundOrdinal = 0
            //    }
            //};
            var scores = new Dictionary<string, PilotContestScoreCollection>
            {
                {
                    "Pilot1",
                    new PilotContestScoreCollection("Pilot1", 893, new Dictionary<int, double>())
                },
                {
                    "Pilot2",
                    new PilotContestScoreCollection("Pilot2", 762, new Dictionary<int, double>())
                },
                {
                    "Pilot3",
                    new PilotContestScoreCollection("Pilot3", 934, new Dictionary<int, double>())
                },
                {
                    "Pilot4",
                    new PilotContestScoreCollection("Pilot4", 553, new Dictionary<int, double>())
                },
                {
                    "Pilot5",
                    new PilotContestScoreCollection("Pilot5", 893, new Dictionary<int, double>())
                },
                {
                    "Pilot6",
                    new PilotContestScoreCollection("Pilot6", 300, new Dictionary<int, double>())
                },
                {
                    "Pilot7",
                    new PilotContestScoreCollection("Pilot7", 999, new Dictionary<int, double>())
                }
            };

            var result = momSort.SortSingleRound(scores, 4);

            Assert.IsNotNull(result);
            Assert.AreEqual("Pilot7", result.First().Key);
            Assert.AreEqual(4, result.Count(x => x.Value == FlightGroup.A));
            Assert.AreEqual(3, result.Count(x => x.Value == FlightGroup.B));
            // No flight group C
            Assert.AreEqual(0, result.Count(x => x.Value == FlightGroup.C));
        }
    }
}
