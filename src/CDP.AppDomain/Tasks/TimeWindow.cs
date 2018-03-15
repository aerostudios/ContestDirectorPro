//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: TimeWindow.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks
{
    using CDP.AppDomain.Timer;

    /// <summary>
    /// Defines a window of time for the app.  Used for Flight test, task and landing windows currently in the app.
    /// </summary>
    /// <seealso cref="CDP.AppDomain.Tasks.TimeGate" />
    public class TimeWindow : TimeGate
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [count down last10 seconds].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [count down last10 seconds]; otherwise, <c>false</c>.
        /// </value>
        public bool CountDownLast10Seconds { get; set; }

        /// <summary>
        /// Gets or sets the direction of count.
        /// </summary>
        /// <value>
        /// The direction of count.
        /// </value>
        public TimerDirection DirectionOfCount { get; set; }
    }
}
