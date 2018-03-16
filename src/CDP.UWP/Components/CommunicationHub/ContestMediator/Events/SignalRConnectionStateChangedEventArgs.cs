//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: SignalRConnectionStateChangedEventArgs.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.CommunicationHub.ContestMediator.Events
{
    /// <summary>
    /// Defines the event arguments for a connnection state change in SignalR.
    /// </summary>
    public sealed class SignalRConnectionStateChangedEventArgs
    {
        public SignalRConnectionState State { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRConnectionStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="stateChange">The state change.</param>
        public SignalRConnectionStateChangedEventArgs(SignalRConnectionState stateChange)
        {
            this.State = stateChange;
        }
    }
}
