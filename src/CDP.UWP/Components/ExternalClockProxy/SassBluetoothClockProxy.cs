//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: SassBluetoothClockProxy.cs
//---------------------------------------------------------------

namespace CDP.UWP.Components.ExternalClockProxy
{
    using CDP.CoreApp.Interfaces.Timer;
    using System.Threading.Tasks;

    public sealed class SassBluetoothClockProxy : ITimer
    {
        /// <summary>
        /// Handles the tick event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public void HandleTick(object sender, object e)
        {
            // Update the clock UI
        }

        /// <summary>
        /// Resets the display.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task Reset()
        {
            // Update the UI
            return Task.CompletedTask;
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task Resume()
        {
            // Nothing to do for this implementation.
            return Task.CompletedTask;
        }

        public Task Start(AppDomain.Timer.TimerDirection direction)
        {
            // Nothing to do for this implementation.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the clock.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task Stop()
        {
            // Nothing to do for this implementation
            return Task.CompletedTask;
        }
    }
}
