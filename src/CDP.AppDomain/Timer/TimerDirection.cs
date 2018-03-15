//---------------------------------------------------------------

// Date: 11/17/2014
// Rights: 
// FileName: TimerDirection.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Timer
{
    /// <summary>
    /// Defines the types of directions a timer can go.
    /// </summary>
    public enum TimerDirection
    {
        /// <summary>
        /// Sets the clock to count up from a determined time
        /// </summary>
        CountUp = 0,

        /// <summary>
        /// Sets the clock to count down from a determined time
        /// </summary>
        CountDown = 1,
    }
}
