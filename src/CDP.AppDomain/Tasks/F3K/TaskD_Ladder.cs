//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: TaskD_Ladder.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks.F3K
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Timer;
    using System;
    using System.Linq;

    /// <summary>
    /// Describes the Ladder task in F3K
    /// </summary>
    /// <seealso cref="ContestDirectorPro.Tasks.F3K.TaskBase" />
    public sealed class TaskD_Ladder : TaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public override ContestType Type => ContestType.F3K;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public override string Description => "Increasing time by 15 seconds.";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Task D - Ladder";

        /// <summary>
        /// Gets or sets a value indicating whether this instance is landing scored.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is landing scored; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLandingScored => false;

        /// <summary>
        /// Gets or sets the number of time gates allowed.
        /// </summary>
        /// <value>
        /// The number of time gates allowed.
        /// </value>
        public override int NumberOfTimeGatesAllowed => ScoredTimeGates.Count;

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskD_Ladder"/> class.
        /// </summary>
        public TaskD_Ladder()
        {
            Id = "da5f5ac0-92df-404d-9a4a-bdc0d678cc7c";
            InitGates();
        }

        #endregion

        #region Public Methods

        /// <summary>Scores the task.
        /// </summary>
        /// <param name="gates">The gates.</param>
        /// <returns></returns>
        public override double ScoreTask(RoundScoreBase contestTaskToScore) => contestTaskToScore.TimeGates.Sum(x => x.Time.TotalSeconds);

        /// <summary>
        /// Validates the specified gates.
        /// </summary>
        /// <param name="contestTaskToValidate">The contest task to validate.</param>
        /// <returns></returns>
        public override bool ValidateTask(RoundScoreBase contestTaskToValidate)
        {
            if (contestTaskToValidate.TimeGates == null)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Count() != 7)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Any(x => x.Time > TimeSpan.FromSeconds(120)))
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Select(x => x.Time > TimeSpan.FromSeconds(105)).Count() > 2)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Select(x => x.Time > TimeSpan.FromMinutes(90)).Count() > 3)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Select(x => x.Time > TimeSpan.FromMinutes(75)).Count() > 4)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Select(x => x.Time > TimeSpan.FromMinutes(60)).Count() > 4)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Select(x => x.Time > TimeSpan.FromMinutes(45)).Count() > 5)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Select(x => x.Time > TimeSpan.FromMinutes(30)).Count() > 6)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the gates.
        /// </summary>
        private void InitGates()
        {
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 6, Time = TimeSpan.FromSeconds(120) });
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 5, Time = TimeSpan.FromSeconds(105) });
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 4, Time = TimeSpan.FromSeconds(90) });
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 3, Time = TimeSpan.FromSeconds(75) });
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 2, Time = TimeSpan.FromSeconds(60) });
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 1, Time = TimeSpan.FromSeconds(45) });
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 0, Time = TimeSpan.FromSeconds(30) });

            TaskFlightWindows.Add(
                new TimeWindow()
                {
                    Ordinal = 0,
                    DirectionOfCount = TimerDirection.CountDown,
                    GateType = TimeGateType.Task,
                    Time = TimeSpan.FromMinutes(10)
                });
        }

        #endregion
    }
}



