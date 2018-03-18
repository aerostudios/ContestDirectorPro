//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: ContestMediator.cs
//---------------------------------------------------------------

namespace CDP.UWP.Components.ContestMediator
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Tasks;
    using CDP.Common;
    using CDP.CoreApp.ApplicationEvents;
    using CDP.UWP.Config;
    using CDP.UWP.Features.CommunicationHub.ContestMediator;
    using CDP.UWP.Features.CommunicationHub.ContestMediator.Events;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Mediates communications between the contest app and the host site / timers / clocks.
    /// </summary>
    public class ContestMediator
    {
        /// <summary>
        /// The logger
        /// </summary>
        private Common.Logging.ILoggingService logger;

        /// <summary>
        /// The hub connection
        /// </summary>
        private HubConnection hubConnection;

        /// <summary>
        /// Occurs when [signal r connection state changed].
        /// </summary>
        internal event EventHandler<SignalRConnectionStateChangedEventArgs> SignalRConnectionStateChanged;

        /// <summary>
        /// Occurs when [final time sheet posted].
        /// </summary>
        internal event EventHandler<FinalTimeSheetPostedEventArgs> FinalTimeSheetPosted;

        /// <summary>
        /// Occurs when [flight timer stopped].
        /// </summary>
        internal event EventHandler<FlightTimerEventArgs> FlightTimerStopped;

        /// <summary>
        /// Occurs when [flight timer started].
        /// </summary>
        internal event EventHandler<FlightTimerEventArgs> FlightTimerStarted;

        /// <summary>
        /// Occurs when [new round available].
        /// </summary>
        internal event EventHandler<NewRoundAvailableEventArgs> NewRoundAvailable;

        /// <summary>
        /// Occurs when [round timer started].
        /// </summary>
        internal event EventHandler<RoundTimerEventArgs> RoundTimerStarted;

        /// <summary>
        /// Occurs when [round timer stopped].
        /// </summary>
        internal event EventHandler<RoundTimerEventArgs> RoundTimerStopped;

        /// <summary>
        /// Occurs when [round timer clock ping].
        /// </summary>
        internal event EventHandler<RoundTimerEventArgs> RoundTimerClockPing;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContestMediator"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ContestMediator(Common.Logging.ILoggingService logger)
        {
            this.logger = logger;
        }

        private Uri serverUrl;

        /// <summary>
        /// Initializes the signalR connection and listeners.
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        public async Task ConnectToSignalRHost(Uri serverAddress)
        {
            Validate.IsNotNull(serverAddress, nameof(serverAddress));

            this.serverUrl = serverAddress;

            try
            {
                this.hubConnection = new HubConnectionBuilder()
                .WithUrl(serverAddress.AbsoluteUri + "scoring")
                .WithConsoleLogger(LogLevel.Debug)
                .WithJsonProtocol(new Newtonsoft.Json.JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto
                })
                .Build();

                // Wire up the connection listeners
                this.hubConnection.Closed += HubConnection_Closed;
                this.hubConnection.Connected += HubConnection_Connected;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return;
            }

            // Initialize the inbound server comms (what we are listening for).
            hubConnection.On<FlightTimerEventArgs>("FlightTimerStopped", HandleFlightTimerStopped);
            hubConnection.On<FlightTimerEventArgs>("FlightTimerStarted", HandleFlightTimerStarted);
            hubConnection.On<FinalTimeSheetPostedEventArgs>("FinalTimeSheetPosted", HandleFinalTimeSheetPosted);

            await hubConnection.StartAsync();
        }

        /// <summary>
        /// Disconnects from signal r host.
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectFromSignalRHost()
        {
            await this.hubConnection.DisposeAsync();
        }

        /// <summary>
        /// Handles the "connected" state change.
        /// </summary>
        /// <returns></returns>
        private Task HubConnection_Connected()
        {
            return Task.Run(async () =>
            {
                SignalRConnectionStateChanged?.Invoke(this, new SignalRConnectionStateChangedEventArgs(SignalRConnectionState.Connected));
                this.logger.LogTrace("Connected to host server.");

                try
                {
                    using (var webClient = new HttpClient())
                    {
                        // Send the contest data to the server to store
                        webClient.BaseAddress = serverUrl;

                        var content = JsonConvert.SerializeObject(App.ContestEngine.Contest);
                        var byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await webClient.PostAsync($"{serverUrl.AbsoluteUri}{ContestHostApis.ContestsRoute}", byteContent);

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            logger.LogTrace("bad stuff man.");
                        }

                        // Send the Flight Matrix for the contest as well
                        var flightMatrix = JsonConvert.SerializeObject(App.ContestEngine.FlightMatrix);
                        byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(flightMatrix));
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        response = await webClient.PostAsync($"{serverUrl.AbsoluteUri}{ContestHostApis.FlightMatrixRoute}", byteContent);

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            logger.LogTrace("bad stuff man.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogException(ex);
                }
            });
        }

        /// <summary>
        /// Handles the "closed" state change.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private async Task HubConnection_Closed(Exception ex)
        {
            SignalRConnectionStateChanged?.Invoke(this, new SignalRConnectionStateChangedEventArgs(SignalRConnectionState.Disconnected));
            await this.hubConnection.DisposeAsync();

            this.logger.LogTrace("SignalR connection to host server has closed." + ex?.Message);
        }

        /// <summary>
        /// Posts a message with a final timesheet.
        /// </summary>
        /// <param name="timeSheet">The time sheet.</param>
        internal void PostTimeSheetFinal(TimeSheet timeSheet, string pilotId, string timingDeviceId)
        {
            FinalTimeSheetPosted?.Invoke(this, new FinalTimeSheetPostedEventArgs(timeSheet, pilotId, timingDeviceId));
        }

        /// <summary>
        /// Posts a message for a new round that is available.
        /// </summary>
        /// <param name="round">The round.</param>
        internal async Task PostNewRoundAvailable(FlightGroup flightGroup, TaskBase task, IEnumerable<Pilot> pilots)
        {
            var args = new NewRoundAvailableEventArgs(flightGroup, task, pilots);
            NewRoundAvailable?.Invoke(this, args);

            try
            {
                if (this.hubConnection != null) await this.hubConnection.InvokeAsync<object>("NewRoundAvailable", args);
                this.logger.LogTrace($"{nameof(PostNewRoundAvailable)}: Sent new round available command to host server.");

                using (var webClient = new HttpClient())
                {
                    // Send the contest data to the server to store
                    webClient.BaseAddress = serverUrl;
                    var content = JsonConvert.SerializeObject(App.ContestEngine.Contest);
                    var byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await webClient.PostAsync($"{serverUrl.AbsoluteUri}{ContestHostApis.ContestsRoute}", byteContent);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        logger.LogTrace("bad stuff man.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Posts the round timer started event.
        /// </summary>
        /// <param name="currentClockTime">The current clock time.</param>
        internal async Task PostRoundTimerStarted(TimeSpan currentClockTime)
        {
            var args = new RoundTimerEventArgs(currentClockTime);
            RoundTimerStarted?.Invoke(this, args);

            try
            {
                if (this.hubConnection != null) await this.hubConnection.InvokeAsync<RoundTimerEventArgs>("RoundTimerStarted", args);
                this.logger.LogTrace($"{nameof(PostRoundTimerStarted)}: Sent round timer start command to host server.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Posts the round timer stopped event.
        /// </summary>
        /// <param name="currentClockTime">The current clock time.</param>
        internal async Task PostRoundTimerStopped(TimeSpan currentClockTime)
        {
            var args = new RoundTimerEventArgs(currentClockTime);
            RoundTimerStopped?.Invoke(this, args);

            try
            {
                if (this.hubConnection != null) await this.hubConnection.InvokeAsync<object>("RoundTimerStopped", args);
                this.logger.LogTrace($"{nameof(PostRoundTimerStopped)}: Sent round timer stop command to host server.");
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);
            }
        }

        /// <summary>
        /// Posts the round timer current time event.
        /// </summary>
        /// <param name="currentClickTime">The current click time.</param>
        internal async Task PostRoundTimerCurrentTime(TimeSpan currentClockTime)
        {
            RoundTimerClockPing?.Invoke(this, new RoundTimerEventArgs(currentClockTime));

            try
            {
                if (this.hubConnection != null) await this.hubConnection.InvokeAsync<object>("RoundTimerCurrentTime", currentClockTime);
                this.logger.LogTrace($"{nameof(PostRoundTimerCurrentTime)}: Sent round timer current time update command to host server.");
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);
            }
        }

        /// <summary>
        /// Handles the final time sheet posted event.
        /// </summary>
        /// <param name="args">The <see cref="FinalTimeSheetPostedEventArgs"/> instance containing the event data.</param>
        private void HandleFinalTimeSheetPosted(FinalTimeSheetPostedEventArgs args)
        {
            FinalTimeSheetPosted?.Invoke(this, args);

            this.logger.LogTrace($"{nameof(ContestMediator)}:{nameof(HandleFinalTimeSheetPosted)} " +
                $" - Pilot:{args.PilotId}, TimingDeviceId:{args.TimingDeviceId}");
        }

        /// <summary>
        /// Handles the flight timer started event.
        /// </summary>
        /// <param name="args">The <see cref="FlightTimerEventArgs"/> instance containing the event data.</param>
        private void HandleFlightTimerStarted(FlightTimerEventArgs args)
        {
            this.FlightTimerStarted?.Invoke(this, args);

            this.logger.LogTrace($"{nameof(ContestMediator)}:{nameof(HandleFlightTimerStopped)} " +
                $" - Pilot:{args.PilotId}, TimingDeviceId:{args.TimingDeviceId}, Minutes:{args.Minutes}, Seconds:{args.Seconds}");
        }

        /// <summary>
        /// Handles the flight timer stopped event.
        /// </summary>
        /// <param name="args">The <see cref="FlightTimerEventArgs"/> instance containing the event data.</param>
        private void HandleFlightTimerStopped(FlightTimerEventArgs args)
        {
            this.FlightTimerStopped?.Invoke(this, args);

            this.logger.LogTrace($"{nameof(ContestMediator)}:{nameof(HandleFlightTimerStopped)} " +
                $" - Pilot:{args.PilotId}, TimingDeviceId:{args.TimingDeviceId}, Minutes:{args.PilotId}, Seconds:{args.Seconds}");
        }
    }
}