//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: NewRoundAvailableEventArgs.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.ApplicationEvents
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Tasks;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Event arguments passed when a NewRoundAvailable event is fired.
    /// </summary>
    [JsonObject]
    public sealed class NewRoundAvailableEventArgs
    {
        /// <summary>
        /// Gets or sets the pilots.
        /// </summary>
        /// <value>
        /// The pilots.
        /// </value>
        [JsonProperty("pilots")]
        public IEnumerable<Pilot> Pilots { get; set; } = new List<Pilot>();

        /// <summary>
        /// Gets or sets the flight group.
        /// </summary>
        /// <value>
        /// The flight group.
        /// </value>
        [JsonProperty("flightGroup")]
        public FlightGroup FlightGroup { get; set; }

        /// <summary>
        /// Gets or sets the task.
        /// </summary>
        /// <value>
        /// The task.
        /// </value>
        [JsonProperty("task")]
        public TaskBase Task { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewRoundAvailableEventArgs"/> class.
        /// </summary>
        /// <param name="flightGroup">The flight group.</param>
        /// <param name="task">The task.</param>
        /// <param name="pilots">The pilots.</param>
        public NewRoundAvailableEventArgs(FlightGroup flightGroup, TaskBase task, IEnumerable<Pilot> pilots)
        {
            this.FlightGroup = flightGroup;
            this.Task = task;
            this.Pilots = pilots;
        }
    }
}
