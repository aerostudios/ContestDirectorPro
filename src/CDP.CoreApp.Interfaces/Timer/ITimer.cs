//---------------------------------------------------------------
// Date: 11/17/2014
// Rights: 
// FileName: ITimer.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Interfaces.Timer
{
    using CDP.AppDomain.Timer;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a timer for the application.
    /// </summary>
    public interface ITimer
    {
        /// <summary>Starts the clock w/ the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns></returns>
        Task Start(TimerDirection direction);

        /// <summary>Stops the clock.
        /// </summary>
        /// <returns></returns>
        Task Stop();

        /// <summary>Resumes this instance.
        /// </summary>
        /// <returns></returns>
        Task Resume();

        /// <summary>Resets the display.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns></returns>
        Task Reset();
    }
}
