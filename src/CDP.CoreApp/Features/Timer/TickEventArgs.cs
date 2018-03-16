//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: TickEventArgs.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Timer
{
    using System;

    /// <summary>
    /// Event args for a Tick event for the Basic Timer class.
    /// </summary>
    public class TickEventArgs
    {
        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public TimeSpan CurrentTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TickEventArgs"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        public TickEventArgs(TimeSpan time)
        {
            this.CurrentTime = time;
        }
    }
}
