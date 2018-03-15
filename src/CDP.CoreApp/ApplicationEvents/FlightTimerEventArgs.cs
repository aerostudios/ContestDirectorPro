//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: FlightTimerEventArgs.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.ApplicationEvents
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Event arguments passed when a FlightTimerStopped event is fired.
    /// </summary>
    [JsonObject]
    public sealed class FlightTimerEventArgs
    {
        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        /// <value>
        /// The minutes.
        /// </value>
        [JsonProperty("minutes")]
        public int Minutes { get; set; }

        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        /// <value>
        /// The seconds.
        /// </value>
        [JsonProperty("seconds")]
        public int Seconds { get; set; }

        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        [JsonProperty("pilotId")]
        public string PilotId { get; set; }

        /// <summary>
        /// Gets or sets the timing device identifier.
        /// </summary>
        /// <value>
        /// The timing device identifier.
        /// </value>
        [JsonProperty("TimingDeviceId")]
        public string TimingDeviceId {get;set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightTimerEventArgs"/> class.
        /// </summary>
        /// <param name="clockValue">The clock value.</param>
        /// <param name="pilotId">The pilot identifier.</param>
        /// <param name="timingDeviceId">The timing device identifier.</param>
        public FlightTimerEventArgs(TimeSpan clockValue, string pilotId, string timingDeviceId)
        {
            this.Minutes = clockValue.Minutes;
            this.Seconds = clockValue.Seconds;
            this.PilotId = pilotId;
            this.TimingDeviceId = timingDeviceId;
        }
    }
}
