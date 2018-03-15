//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: FlightTimePostedEventArgs.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.ApplicationEvents
{
    using CDP.AppDomain.Scoring;
    using Newtonsoft.Json;

    /// <summary>
    /// Event arguments for a FinalTimeSheetPosted event
    /// </summary>
    public sealed class FinalTimeSheetPostedEventArgs
    {
        /// <summary>
        /// Gets or sets the final time sheet.
        /// </summary>
        /// <value>
        /// The final time sheet.
        /// </value>
        public TimeSheet FinalTimeSheet { get; set; } = new TimeSheet();

        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        public string PilotId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timing device identifier.
        /// </summary>
        /// <value>
        /// The timing device identifier.
        /// </value>
        public string TimingDeviceId { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalTimeSheetPostedEventArgs"/> class.
        /// </summary>
        [JsonConstructor]
        public FinalTimeSheetPostedEventArgs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalTimeSheetPostedEventArgs"/> class.
        /// </summary>
        /// <param name="timeSheet">The time sheet.</param>
        public FinalTimeSheetPostedEventArgs(TimeSheet timeSheet, string pilotId, string timingDeviceId)
        {
            this.FinalTimeSheet = timeSheet;
        }
    }
}
