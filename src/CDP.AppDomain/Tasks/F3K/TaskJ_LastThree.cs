﻿//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: TaskJ_LastThree.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks.F3K
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Timer;
    using System;
    using System.Linq;

    /// <summary>
    /// Defines Task J in F3K, Last 3 flights
    /// </summary>
    public sealed class TaskJ_LastThree : TaskBase
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
        public override string Description => "Last 3 flights in a 10 minute window.  Max flight time of 180 seconds (three minutes).";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Task J - Three 3:20's";

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
        /// Initializes a new instance of the <see cref="TaskJ_LastThree"/> class.
        /// </summary>
        public TaskJ_LastThree()
        {
            base.Id = "f5c32338-00ea-4115-a18c-3612b691bf3a";
            InitGates();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Scores the task.
        /// </summary>
        /// <param name="contestTaskToScore">The contest task to score.</param>
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

            if (contestTaskToValidate.TimeGates.Count > 3) { return false; }

            if (contestTaskToValidate.TimeGates.Sum(x => x.Time.TotalSeconds) > 600) { return false; }

            if (contestTaskToValidate.TimeGates.Any(x => x.Time > TimeSpan.FromSeconds(180))) { return false; }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the gates.
        /// </summary>
        private void InitGates()
        {
            for (var i = 0; i < 3; ++i)
            {
                ScoredTimeGates.Add(new TimeGate() { Ordinal = i, Time = TimeSpan.FromSeconds(180) });
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