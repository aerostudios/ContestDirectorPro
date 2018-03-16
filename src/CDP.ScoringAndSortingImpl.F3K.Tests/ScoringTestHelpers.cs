//---------------------------------------------------------------
// Date: 12/18/2017
// Rights: 
// FileName: ScoringTestHelpers.cs
//---------------------------------------------------------------

namespace CDP.ScoringAndSortingImpl.F3K.Tests
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Tasks;
    using CDP.AppDomain.Tasks.F3K;
    using CDP.CoreApp.Features.Scoring;
    using CDP.ScoringAndSortingImpl.F3K.Scoring;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper class for scoring tests
    /// </summary>
    public static class ScoringTestHelpers
    {
        /// <summary>
        /// Generates valid time sheets.
        /// </summary>
        /// <param name="numberOfPilots">The number to create.</param>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="numberOfRounds">The number of rounds.</param>
        /// <param name="maxTimeGateTimeInSeconds">The maximum time gate time in seconds.</param>
        /// <param name="multipleFlightGroups">if set to <c>true</c> [multiple flight groups].</param>
        /// <param name="generateScores">if set to <c>true</c> [generate scores].</param>
        /// <returns></returns>
        public static List<TimeSheet> GenerateValidTimeSheets(int numberOfPilots, 
            string contestId = "SASS August 2017", 
            int numberOfRounds = 1,
            int maxTimeGateTimeInSeconds = 120,
            bool multipleFlightGroups = false, 
            bool generateScores = false)
        {
            var validScoresToReturn = new List<TimeSheet>();
            var validScoresForRound = new List<TimeSheet>();
            var TaskA = new TaskA_LastFlightSevenMin();

            if (numberOfPilots == 0) return validScoresToReturn;
            var rnd = new Random();

            // Set up a mock task to validate and score the rounds if required.
            var mockTask = new Mock<TaskBase>();
            mockTask.Setup(x => x.ValidateTask(It.IsAny<RoundScoreBase>())).Returns(true);
            mockTask.Setup(x => x.ScoreTask(It.IsAny<RoundScoreBase>())).Returns<RoundScoreBase>(y => y.TimeGates.Sum(x => x.Time.TotalSeconds));
            var f3kRoundScoreAlgo = new RoundScoringAlgo();

            for (var i = 0; i < numberOfRounds; ++i)
            {
                // Reset
                validScoresForRound = new List<TimeSheet>();

                for (var j = 0; j < numberOfPilots; ++j)
                {
                    var flightGroup = FlightGroup.A;

                    // Generate 2 flight groups
                    if (multipleFlightGroups)
                    {
                        flightGroup = (j % 2) != 0 ? FlightGroup.A : FlightGroup.B;
                    }

                    var timeGates = new List<TimeGate> { new TimeGate(TimeSpan.FromSeconds(rnd.Next(45, maxTimeGateTimeInSeconds)), j, TimeGateType.Task) };
                    var roundScore = new TimeSheet
                    {
                        RoundOrdinal = i,
                        TimeGates = timeGates,
                        PilotId = $"pilot{j}",
                        ContestId = contestId,
                        TaskId = TaskA.Id,
                        FlightGroup = FlightGroup.A
                    };

                    // Generate the scores if needed.
                    if (generateScores)
                    {
                    }

                    validScoresForRound.Add(roundScore);
                }

                if (generateScores) { f3kRoundScoreAlgo.ScoreRound(validScoresForRound, mockTask.Object); }
                
                validScoresToReturn.AddRange(validScoresForRound);
            }

            return validScoresToReturn;
        }
    }
}
