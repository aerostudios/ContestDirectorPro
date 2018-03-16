//---------------------------------------------------------------
// Date: 11/17/2014
// Rights: 
// FileName: TaskClockConfig.cs
//---------------------------------------------------------------

namespace CDP.UWP.Components.Clock
{
    using CDP.AppDomain.Timer;
    using System;

    /// <summary>Holds config values to set up the clock during a task
    /// </summary>
    public sealed class TaskClockConfig
    {
        /// <summary>Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public TimeSpan StartTime { get; set; }

        /// <summary>Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        public TimeSpan EndTime { get; set; }

        /// <summary>Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public TimerDirection Direction { get; set; }
    }
}
