//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: TaskC_AllUpLastDown.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks.F3K
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Timer;
    using System;
    using System.Linq;

    /// <summary>
    /// Defines TaskC in F3K
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Tasks.TaskBase" />
    public sealed class TaskC_AllUpLastDown : TaskBase
    {
        #region Instance Members

        private int _numberOfFlights = 0;

        #endregion

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
        public override string Description => string.Format("All up last down, 3 min max.  {0} flights.", _numberOfFlights);

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => string.Format("Task C - {0} flights", _numberOfFlights);

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
        /// Initializes a new instance of the <see cref="TaskA_LastFlight7Min"/> class.
        /// </summary>
        public TaskC_AllUpLastDown(int numberOfFlights = 3)
        {
            Id = "169b456c-9002-4c11-a86e-42c31fbe013" + numberOfFlights;
            _numberOfFlights = numberOfFlights;
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

            if (contestTaskToValidate.TimeGates.Count() != _numberOfFlights)
            {
                return false;
            }

            if (contestTaskToValidate.TimeGates.Any(x => x.Time > TimeSpan.FromMinutes(3)))
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
            for (var i = 0; i < _numberOfFlights; i++)
            {
                ScoredTimeGates.Add(new TimeGate() { Ordinal = i, Time = TimeSpan.FromMinutes(3) });

                TaskFlightWindows.Add(
                    new TimeWindow()
                    {
                        Ordinal = 0,
                        DirectionOfCount = TimerDirection.CountDown,
                        GateType = TimeGateType.Task,
                        Time = TimeSpan.FromMinutes(3)
                    });

                if (i < _numberOfFlights - 1)
                {
                    TaskFlightWindows.Add(
                        new TimeWindow()
                        {
                            Ordinal = 0,
                            DirectionOfCount = TimerDirection.CountDown,
                            GateType = TimeGateType.Landing,
                            Time = TimeSpan.FromMinutes(1)
                        });

                    TaskFlightWindows.Add(
                        new TimeWindow()
                        {
                            Ordinal = 0,
                            DirectionOfCount = TimerDirection.CountDown,
                            GateType = TimeGateType.Break,
                            Time = TimeSpan.FromMinutes(1)
                        });
                }
            }

            for (var j = 0; j < (_numberOfFlights - 1); ++j)
            {

            }
        }

        #endregion
    }
}

