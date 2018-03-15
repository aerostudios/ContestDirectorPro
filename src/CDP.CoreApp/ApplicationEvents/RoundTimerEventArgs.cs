//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: RoundTimerEventArgs.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.ApplicationEvents
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Event arguments passed when a RoundTimer event is fired.
    /// </summary>
    [JsonObject]
    public sealed class RoundTimerEventArgs
    {
        /// <summary>
        /// Gets or sets the clock time.
        /// </summary>
        /// <value>
        /// The clock time.
        /// </value>
        [JsonProperty("clockTime")]
        public TimeSpan ClockTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundTimerEventArgs"/> class.
        /// </summary>
        /// <param name="clockTime">The clock time.</param>
        public RoundTimerEventArgs(TimeSpan clockTime)
        {
            this.ClockTime = clockTime;
        }
    }
}
