//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: TaskG_FiveTwos.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks.F3K
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Timer;
    using System;
    using System.Linq;

    /// <summary>
    /// Defines Task G in F3K, Five 2's
    /// </summary>
    public sealed class TaskG_FiveTwos : TaskBase
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
        public override string Description => "Best 5 flights in a 10 minute window.  Max flight time of 120 seconds (two minutes).";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Task G - Five 2's";

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
        /// Initializes a new instance of the <see cref="TaskG_FiveTwos"/> class.
        /// </summary>
        public TaskG_FiveTwos()
        {
            base.Id = "e030c71a-5b54-47ed-b4a5-f0dcdb03efff";
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
            if (contestTaskToValidate.TimeGates == null) { return false; }

            if (contestTaskToValidate.TimeGates.Count > 5) { return false; }

            if (contestTaskToValidate.TimeGates.Any(x => x.Time.TotalSeconds < 0)) { return false; }

            if (contestTaskToValidate.TimeGates.Sum(x => x.Time.TotalSeconds) > 600) { return false; }

            if (contestTaskToValidate.TimeGates.Any(x => x.Time > TimeSpan.FromSeconds(120))) { return false; }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the gates.
        /// </summary>
        private void InitGates()
        {
            for (var i = 0; i < 5; ++i)
            {
                ScoredTimeGates.Add(new TimeGate() { Ordinal = i, Time = TimeSpan.FromSeconds(120) });
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