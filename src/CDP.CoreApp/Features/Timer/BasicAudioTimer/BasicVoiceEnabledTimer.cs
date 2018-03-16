//---------------------------------------------------------------

// Date: 11/17/2014
// Rights: 
// FileName: BasicVoiceEnabledTimer.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.Timer.BasicAudioTimer
{
    using CDP.AppDomain.Timer;
    using CDP.CoreApp.Interfaces.Announcer;
    using CDP.CoreApp.Interfaces.Timer;
    using System;
    using System.Threading.Tasks;

    public sealed class BasicVoiceEnabledTimer
    {
        #region Instance Members

        /// <summary>
        /// The timer step in seconds
        /// </summary>
        private const double TIMER_STEP = 100;

        /// <summary>
        /// The number of seconds in a minute
        /// </summary>
        private const short NUMBER_OF_SECONDS_IN_A_MIN = 60;

        /// <summary>
        /// The number of minutes in an hour
        /// </summary>
        private const short NUMBER_OF_MINUTES_IN_AN_HOUR = 60;

        /// <summary>
        /// Represents one second
        /// </summary>
        private static readonly TimeSpan ONE_SECOND_INTERVAL = new TimeSpan(0, 0, 1);

        /// <summary>
        /// Represents the Zero position on the clock
        /// </summary>
        private static readonly TimeSpan ZERO_VALUE = new TimeSpan(0, 0, 0);

        /// <summary>
        /// The direction of the timer (count up, count down)
        /// </summary>
        private TimerDirection direction;

        /// <summary>
        /// Reference to the External (physical) clock
        /// </summary>
        private ITimer externalClock;

        /// <summary>
        /// The internal clock (software) clock
        /// </summary>
        private System.Timers.Timer internalClock;

        /// <summary>
        /// The current time value
        /// </summary>
        private TimeSpan currentTime;

        /// <summary>
        /// The start time
        /// </summary>
        private TimeSpan startTime;

        /// <summary>
        /// The clock configuration
        /// </summary>
        private TimerConfig clockConfig;
        
        /// <summary>
        /// The announcer
        /// </summary>
        private readonly AnnouncerBase announcer;

        #endregion

        #region Properties

        /// <summary>Gets the seconds.
        /// </summary>
        /// <value>
        /// The seconds.
        /// </value>
        public int Seconds { get { return currentTime.Seconds; } }

        /// <summary>Gets the minutes.
        /// </summary>
        /// <value>
        /// The minutes.
        /// </value>
        public int Minutes { get { return currentTime.Minutes; } }

        /// <summary>Gets the hours.
        /// </summary>
        /// <value>
        /// The hours.
        /// </value>
        public int Hours { get { return currentTime.Hours; } }

        /// <summary>
        /// Gets the total seconds.
        /// </summary>
        /// <value>
        /// The total seconds.
        /// </value>
        public double TotalSeconds { get { return currentTime.TotalSeconds; } }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingClock" /> class.
        /// </summary>
        /// <param name="externalClock">The clock.</param>
        /// <param name="announcer">The announcer.</param>
        public BasicVoiceEnabledTimer(ITimer externalClock, AnnouncerBase announcer)
        {
            this.externalClock = externalClock;
            this.announcer = announcer;
            this.internalClock = new System.Timers.Timer
            {
                Interval = TIMER_STEP
            };

            this.internalClock.Elapsed += InternalClock_Tick;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the Timing Clock.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="startTime">The start time.</param>
        /// <returns></returns>
        public void Start(TimerDirection direction, TimeSpan startTime)
        {
            if (internalClock.Enabled)
            {
                internalClock.Stop();
            }

            this.currentTime = this.startTime = startTime;
            this.direction = direction;

            // Reset the clock
            ReSetCurrentTime();

            this.internalClock.Start();
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task Resume()
        {
            if (internalClock.Enabled)
            {
                return;
            }

            await Task.Run(() => { internalClock.Start(); });
        }

        /// <summary>
        /// Stops the Timing Clock.
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            await Task.Run(() => { internalClock.Stop(); });
        }

        /// <summary>
        /// Resets the Timing Clock.
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            await Task.Run(() => { internalClock.Stop(); });
            ReSetCurrentTime();
        }

        /// <summary>
        /// Initializes the specified clock configuration.
        /// </summary>
        /// <param name="clockConfig">The clock configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// clockConfig - Config cannot be null
        /// or
        /// voiceConfig - Voice Config cannot be null
        /// </exception>
        public void Initialize(TimerConfig clockConfig)
        {
            this.clockConfig = clockConfig ?? throw new ArgumentNullException("clockConfig", "Config cannot be null");
            this.currentTime = clockConfig.StartTime;
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the internal clock tick event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void InternalClock_Tick(object sender, object e)
        {
            bool fireEndEvent = false;

            switch (direction)
            {
                case TimerDirection.CountDown:
                    currentTime = currentTime.Subtract(new TimeSpan(0,0,1));
                    fireEndEvent = currentTime.TotalSeconds == 0;
                    break;
                case TimerDirection.CountUp:
                    currentTime = currentTime.Add(ONE_SECOND_INTERVAL);
                    fireEndEvent = currentTime.TotalSeconds == startTime.TotalSeconds;
                    break;
            }

            // Fire the end event if necessary
            if (fireEndEvent) { OnClockEnd(sender, null); }

            // Fire our own tick event
            OnTick(sender, null);
        }

        /// <summary>
        /// Event handler delegate for a Tick event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public delegate void TickHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when [tick].
        /// </summary>
        public event TickHandler Tick;

        /// <summary>
        /// Called when [tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        public void OnTick(object sender, EventArgs e)
        {
            Tick?.Invoke(this, e);
        }

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
        public async void OnClockEnd(object sender, EventArgs e)
        {
            // Stop the clock as it has finished.
            await this.Stop();

            // Fire the event handlers for this event.
            ClockEnd?.Invoke(this, e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the current time.
        /// </summary>
        private void ReSetCurrentTime()
        {
            switch (direction)
            {
                case TimerDirection.CountDown:
                    currentTime = new TimeSpan(0, 0, (int)this.startTime.TotalSeconds);
                    break;
                case TimerDirection.CountUp:
                    currentTime = new TimeSpan(0, 0, 0);
                    break;
            }
        }

        /// <summary>
        /// Creates the remaining time announcement.
        /// </summary>
        /// <param name="timeRemaining">The time remaining.</param>
        /// <returns>A string of words to represent the time remaining.</returns>
        private string CreateRemainingTimeAnnouncement(TimeSpan timeRemaining)
        {
            string returnAnnouncement = string.Empty;

            returnAnnouncement += (timeRemaining.Minutes > 0) ? timeRemaining.Minutes.ToString() : string.Empty;
            returnAnnouncement += (timeRemaining.Minutes > 1) ? " minutes" : (timeRemaining.Minutes == 1) ? " minute" : string.Empty;
            returnAnnouncement += (timeRemaining.Seconds > 0) ? timeRemaining.Seconds.ToString() : string.Empty;
            returnAnnouncement += (timeRemaining.Seconds > 1 && timeRemaining.Seconds != 0) ? " seconds" : (timeRemaining.Seconds == 1) ? " second" : string.Empty;
            returnAnnouncement += " remaining";

            return returnAnnouncement;
        }

        #endregion

        #region DTors

        ~BasicVoiceEnabledTimer()
        {
            // Remove any event bindings
            if (this.internalClock != null)
            {
                this.internalClock.Elapsed -= InternalClock_Tick;
            }
        }

        #endregion
    }
}
