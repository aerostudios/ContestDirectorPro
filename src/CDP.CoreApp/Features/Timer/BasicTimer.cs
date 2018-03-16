//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: BasicTimer.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Timer
{
    using CDP.AppDomain.Timer;
    using System;
    using System.Timers;

    /// <summary>
    /// Basic timer implementation.
    /// </summary>
    public sealed class BasicTimer
    {
        private TimeSpan currentTime;

        private System.Timers.Timer internalTimer = new System.Timers.Timer();
        private int timerIntervalInMilliseconds;
        private TimerDirection direction;

        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public TimeSpan CurrentTime { get { return currentTime; } }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public TimerDirection Direction { get { return direction; } private set { direction = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicTimer"/> class.
        /// </summary>
        /// <param name="updateIntervalInMilliseconds">The update interval in milliseconds.</param>
        public BasicTimer(int updateIntervalInMilliseconds)
        {
            this.timerIntervalInMilliseconds = updateIntervalInMilliseconds;
            this.internalTimer.Elapsed += InternalTimer_Elapsed;
        }

        /// <summary>
        /// Handles the Elapsed event of the InternalTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void InternalTimer_Elapsed(object sender, ElapsedEventArgs e)
        { 
            if (this.direction == TimerDirection.CountUp)
            {
                this.currentTime = CurrentTime.Add(new TimeSpan(0,0,0,0,this.timerIntervalInMilliseconds));
            }
            else
            {
                this.currentTime = CurrentTime.Subtract(new TimeSpan(0, 0, 0, 0, this.timerIntervalInMilliseconds));
            }
            
            if (currentTime.TotalMilliseconds < 1)
            {
                OnClockEnd(this, null);
            }

            Tick?.Invoke(this, new TickEventArgs(this.currentTime));
        }

        /// <summary>
        /// Starts the specified start time.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="callerSyncObject">The caller synchronize object.</param>
        public void Start(TimeSpan startTime, TimeSpan endTime, TimerDirection direction, System.ComponentModel.ISynchronizeInvoke callerSyncObject = null)
        {
            this.direction = direction;
            this.internalTimer.SynchronizingObject = callerSyncObject;
            this.internalTimer.Interval = this.timerIntervalInMilliseconds;

            currentTime = new TimeSpan(startTime.Ticks);

            this.internalTimer.Start();
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            this.internalTimer.Stop();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.internalTimer.Stop();
            this.currentTime = new TimeSpan();
        }

        /// <summary>
        /// Event handler delegate for a Tick event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public delegate void TickHandler(object sender, TickEventArgs e);

        /// <summary>
        /// Occurs when [tick].
        /// </summary>
        public event TickHandler Tick;

        /// <summary>
        /// Event handler delegate for an End of the Clock timer event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public delegate void ClockEndHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when [clock end].
        /// </summary>
        public event ClockEndHandler ClockEnd;

        /// <summary>
        /// Called when [clock end].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnClockEnd(object sender, EventArgs e)
        {
            // Stop the clock as it has finished.
            this.Stop();

            // Fire the event handlers for this event.
            ClockEnd?.Invoke(this, e);
        }
    }
}
