//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: TaskH_FourThreeTwoOne.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks.F3K
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Timer;
    using System;
    using System.Linq;

    /// <summary>Describes a Four, Three, Two, One Task.
    /// </summary>
    public sealed class TaskH_FourThreeTwoOne : TaskBase
    {
        #region Properites

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
        public override string Description => "Best 4 flights in a 1, 2, 3 and 4 minute window.";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Task H - One, Two, Three, Four";

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

        /// <summary>Initializes a new instance of the <see cref="TaskH_FourThreeTwoOne"/> class.
        /// </summary>
        public TaskH_FourThreeTwoOne()
        {
            Id = "a04578b3-21d0-455c-a8c0-4337b71448dc";
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

            if (contestTaskToValidate.TimeGates.Sum(g => g.Time.TotalSeconds) > TimeSpan.FromMinutes(10).TotalSeconds)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Count() > 4)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Any(g => g.Time > TimeSpan.FromMinutes(4)))
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Where(g => g.Time > TimeSpan.FromMinutes(3)).Count() > 1)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Where(g => g.Time > TimeSpan.FromMinutes(2)).Count() > 2)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Where(g => g.Time > TimeSpan.FromMinutes(1)).Count() > 3)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>Inits the gates.
        /// </summary>
        private void InitGates()
        {
            for (var i = 0; i < 4; ++i)
            {
                ScoredTimeGates.Add(new TimeGate() { Ordinal = i, Time = TimeSpan.FromMinutes(i + 1) });
            }

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
