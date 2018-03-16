//---------------------------------------------------------------
// Date: 1/30/2018
// Rights: 
// FileName: ContestScoringHub.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.Site.Hubs
{
    using CDP.CoreApp.ApplicationEvents;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles all of the SignalR requests and routing.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.SignalR.Hub" />
    public class ContestScoringHub : Hub
    {
        /// <summary>
        /// Called when [connected asynchronous].
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            Debug.WriteLine("Client Connected...");
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Handles the FinalTimeSheetPosted event / message.
        /// </summary>
        /// <param name="args">The <see cref="FinalTimeSheetPostedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task FinalTimeSheetPosted(FinalTimeSheetPostedEventArgs args) => await Clients.AllExcept(GetCallerId()).InvokeAsync("FinalTimeSheetPosted", args);

        /// <summary>
        /// Handles the FlightTimerStopped event / message.
        /// </summary>
        /// <param name="args">The <see cref="FlightTimerEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task FlightTimerStopped(FlightTimerEventArgs args)
        {
            try
            {
                await Clients.AllExcept(GetCallerId()).InvokeAsync("FlightTimerStopped", args);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"SignalR Error: {ex.Message}");
            }

        }

        /// <summary>
        /// Handles the FlightTimerStarted event / message.
        /// </summary>
        /// <param name="args">The <see cref="FlightTimerEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task FlightTimerStarted(FlightTimerEventArgs args) => await Clients.AllExcept(GetCallerId()).InvokeAsync("FlightTimerStarted", args);

        /// <summary>
        /// Handles the NewRoundAvailable event / message.
        /// </summary>
        /// <param name="args">The <see cref="NewRoundAvailableEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task NewRoundAvailable(NewRoundAvailableEventArgs args) => await Clients.AllExcept(GetCallerId()).InvokeAsync("NewRoundAvailable", args);

        /// <summary>
        /// Handles the RoundTimerStarted event / message.
        /// </summary>
        /// <param name="args">The <see cref="RoundTimerEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task RoundTimerStarted(RoundTimerEventArgs args) => await Clients.AllExcept(GetCallerId()).InvokeAsync("RoundTimerStarted", args);

        /// <summary>
        /// Handles the RoundTimerStopped event / message.
        /// </summary>
        /// <param name="args">The <see cref="RoundTimerEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task RoundTimerStopped(RoundTimerEventArgs args) => await Clients.AllExcept(GetCallerId()).InvokeAsync("RoundTimerStopped", args);

        /// <summary>
        /// Handles the RoundTimerClockPing event / message.
        /// </summary>
        /// <param name="args">The <see cref="RoundTimerEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task RoundTimerClockPing(RoundTimerEventArgs args) => await Clients.AllExcept(GetCallerId()).InvokeAsync("RoundTimerClockPing", args);
        
        /// <summary>
        /// Gets the caller id.  Helper method.
        /// </summary>
        /// <returns></returns>
        private IReadOnlyList<string> GetCallerId()
        {
            return new List<string> { Context.ConnectionId };
        }
    }
}
