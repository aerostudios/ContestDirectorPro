//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: SignalRConnectionState.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.CommunicationHub.ContestMediator
{
    /// <summary>
    /// Defines the SignalR connection states
    /// </summary>
    public enum SignalRConnectionState
    {
        /// <summary>
        /// The default state (null essentially).
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The connected state.
        /// </summary>
        Connected = 1,

        /// <summary>
        /// The disconnected state.
        /// </summary>
        Disconnected = 2
    }
}
