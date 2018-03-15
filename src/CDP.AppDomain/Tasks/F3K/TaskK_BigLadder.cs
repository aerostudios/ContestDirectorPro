//---------------------------------------------------------------
// Date: 3/4/2018
// Rights: 
// FileName: TaskI_ThreeThreeTwenties.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks.F3K
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Timer;
    using System;
    using System.Linq;

    public class TaskK_BigLadder : TaskBase
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
        public override string Description => "Task K - Big Ladder; 5 flights; First target 1:00; 0:30 added after each flight.";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Task K - Big Ladder";

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
        /// Initializes a new instance of the <see cref="TaskI_ThreeThreeTwenties"/> class.
        /// </summary>
        public TaskK_BigLadder()
        {
            base.Id = "486b476e-4f43-42b8-b81f-26cc2ac2da51";
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

            if (contestTaskToValidate.TimeGates.Count > 5) { return false; }

            if (contestTaskToValidate.TimeGates.Sum(x => x.Time.TotalSeconds) > 600) { return false; }

            for (var i = 0; i < 5; ++i)
            {
                var time = contestTaskToValidate.TimeGates[i]?.Time.TotalSeconds ?? 0;
                if (time > 60 + (i * 30))
                {
                    return false;
                }
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
            for (var i = 0; i < 5; ++i)
            {
                ScoredTimeGates.Add(new TimeGate() { Ordinal = i, Time = TimeSpan.FromSeconds(60 + (i*30)) });
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
