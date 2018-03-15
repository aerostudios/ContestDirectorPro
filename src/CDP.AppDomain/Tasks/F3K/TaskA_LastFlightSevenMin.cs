//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: ITaskValidator.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks.F3K
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Timer;
    using System;
    using System.Linq;

    /// <summary>
    /// Defines TaskA in F3K
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Tasks.TaskBase" />
    public sealed class TaskA_LastFlightSevenMin : TaskBase
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
        public override string Description => "Last flight, 5 minute max in a 7 minute window.";

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Task A - last flight 5 min max";

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
        /// Initializes a new instance of the <see cref="LastFlight7Min" /> class.
        /// </summary>
        public TaskA_LastFlightSevenMin()
        {
            Id = "cf5227e2-fc4b-401e-bd98-24b25d9846bf";
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
            if (contestTaskToValidate.TimeGates == null)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Count != NumberOfTimeGatesAllowed)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Any(x => x.Time > TimeSpan.FromMinutes(5)))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Inits the gates.
        /// </summary>
        private void InitGates()
        {
            ScoredTimeGates.Add(new TimeGate() { Ordinal = 0, Time = TimeSpan.FromMinutes(5) });
            TaskFlightWindows.Add(
                new TimeWindow()
                {
                    Ordinal = 0,
                    DirectionOfCount = TimerDirection.CountDown,
                    GateType = TimeGateType.Task,
                    Time = TimeSpan.FromMinutes(7)
                });
        }

        #endregion
    }
}
