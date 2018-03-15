//---------------------------------------------------------------

// Date: 7/7/2014
// Rights: 
// FileName: TaskBase.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Scoring.Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Base class defining a Task.
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Tasks.ITaskValidator" />
    /// <seealso cref="CDP.AppDomain.Scoring.Interfaces.ITaskCalculator" />
    public abstract class TaskBase : ITaskValidator, ITaskCalculator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; protected set; }

        /// <summary>Gets the time gates.
        /// </summary>
        /// <value>
        /// The time gates.
        /// </value>
        public virtual IList<TimeGate> ScoredTimeGates { get; protected set; } = new List<TimeGate>();

        /// <summary>
        /// Gets or sets the task flight windows.
        /// </summary>
        /// <value>
        /// The task flight windows.
        /// </value>
        public virtual IList<TimeWindow> TaskFlightWindows { get; protected set; } = new List<TimeWindow>();

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public virtual string Description { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is landing scored.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is landing scored; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsLandingScored { get; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name { get; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public virtual ContestType Type { get; }

        /// <summary>
        /// Gets or sets the number of time gates allowed.
        /// </summary>
        /// <value>
        /// The number of time gates allowed.
        /// </value>
        public virtual int NumberOfTimeGatesAllowed { get; }
        
        #endregion

        #region Overrides

        /// <summary>
        /// Scores the task.
        /// </summary>
        /// <param name="contestTaskToScore">The contest task to score.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual double ScoreTask(RoundScoreBase contestTaskToScore) { throw new NotImplementedException(); }

        /// <summary>
        /// Validates the specified gates.
        /// </summary>
        /// <param name="contestTaskToValidate">The contest task to validate.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool ValidateTask(RoundScoreBase contestTaskToValidate) { throw new NotImplementedException(); }

        #endregion
    }
}
