import * as signalR from '@aspnet/signalr-client';
import { SignalRConnector } from '../../../components/SignalRConnector/SignalRConnector';
import { inject } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';
import { Timer } from '../../../components/Timer/timer';
import { INewRoundAvailableEventArgs } from '../../../Interfaces/INewRoundAvailableEventArgs';
import { FinalTimeSheetPostedEventArgs } from '../../../Interfaces/IFinalTimeSheetPostedEventArgs';
import { Pilot } from '../../../Interfaces/IPilot';
import { TimeSheet } from '../../../Interfaces/ITimeSheet';
import { TimeGate } from '../../../Interfaces/ITimeGate';
import { FlightGroup } from '../../../Interfaces/FlightGroup';

@inject(SignalRConnector, HttpClient)
export class TimerPage {

    // Page properties
    public taskName: string;
    public taskId: string;
    public timeRemaining: string;
    public currentTime: string;
    public displayRecordedTimes: string[] = [];
    public pilotsInRound: Pilot[] = [];
    private pilotIdAssignedToTimer: string;
    private pilotNameAssignedToTimer: string;
    public showPilotList: boolean = false;
    public showTimeGates: boolean = false;
    public fetchClient: HttpClient;

    // State
    private isRoundActive: boolean = false;
    public startStopButtonState: string;

    // SignalR Connection
    private scoringConnection: signalR.HubConnection;

    // Timers
    private stopwatchTimer: Timer;
    private roundCountDownTimer: Timer;
    private startTimer: any;
    private isRunning: boolean = false;
    private roundTimer: any;
    private startTime: number;
    private stopTime: number;
    private scoredTimeGates: any[] = [];

    // More Properties

    // Delegate that Handles the 'tick' event on the main timer
    timerOnInterval = (): void => {
        this.currentTime =
            this.formatTime(this.startTime ? (new Date()).getTime() - this.startTime : 0);
    }

    /**
     * Updates the UI when a new round is posted to SignalR
     * @param args
     */
    newRoundPosted = (args: INewRoundAvailableEventArgs): void => {
        this.showPilotList = true;
        this.pilotsInRound = args.pilots;
        this.taskName = args.task.Name;
        this.isRoundActive = false;
    }

    /**
     * Sets the round timer from a SignalR event
     * @param {any} args Event Args
     */
    setRoundTimer = (args: any): void => {

        let time: string[] = args.clockTime.split(":");
        let hours: number = parseInt(time[0]);
        let minutes: number = parseInt(time[1]);
        let seconds: number = parseInt(time[2]);

        this.roundCountDownTimer.set(`${this.padNumbersWithZeros(hours)}:${this.padNumbersWithZeros(minutes)}:${this.padNumbersWithZeros(seconds)}`);
    }

    /**
     *  Starts the round timer
     * @param {any} args Event Args
     */
    startRoundTimer = (args: any): void => {

        this.setRoundTimer(args);
        this.roundCountDownTimer.start(1000, this.roundTimerOnInterval);
    }

    /**
     * Handles the 'tick' event for the round timer clock
     */
    roundTimerOnInterval = (): void => {
        this.timeRemaining = `00:${this.roundCountDownTimer.displayMinutes}:${this.roundCountDownTimer.displaySeconds}`;
    }

    /**
     * The Constructor
     * @param {SignalRConnector} signalr Instance of the SingalR connection object.
     */
    constructor(signalr: SignalRConnector, fetchClient: HttpClient) {

        this.taskName = "";
        this.timeRemaining = "00:00:00";
        this.currentTime = "00:00:00";
        this.startStopButtonState = "START";
        this.stopwatchTimer = new Timer(true);
        this.roundCountDownTimer = new Timer(false);
        this.scoringConnection = signalr.scoringConnection;

        this.fetchClient = fetchClient;
        this.fetchClient.configure(config => {
            config
                .withDefaults({
                    headers: {
                        'Accept': 'application/json'
                    }
                });
        });

        // Bind to the round start event
        this.scoringConnection.on("RoundTimerStarted", (args) => { this.startRoundTimer(args); })

        // Bind to the round end event
        this.scoringConnection.on("RoundTimerStopped", (args) => { this.setRoundTimer(args); })

        // Bind to the new round posted event
        this.scoringConnection.on("NewRoundAvailable", (args) => { this.newRoundPosted(args); })

        // Bind to the timer ping event
        this.scoringConnection.on("RoundTimerClockPing", (args) => { this.startRoundTimer(args); })
    }

    /**
     * Aurelia trigger, handles 'page load'.
     */
    activate() {
        var result = this.fetchClient.fetch('api/Contest',
            {
                method: "GET"
            })
            .then(response => response.json())
            .then(data => {
                this.showPilotList = true;
                const currentFlightGroup: string = FlightGroup[<number>data.state.currentFlightGroup];
                for (let pilot of data.rounds[data.state.currentRoundOrdinal].flightGroups[currentFlightGroup]) {
                    let newPilot = new Pilot();
                    newPilot.FirstName = pilot.firstName;
                    newPilot.LastName = pilot.lastName;
                    newPilot.Id = pilot.id;
                    this.pilotsInRound.push(newPilot);
                }
                this.taskId = data.rounds[data.state.currentRoundOrdinal].assignedTaskId;
                this.isRoundActive = false;

                return data.rounds[data.state.currentRoundOrdinal].assignedTaskId;
            })
            .then(taskId => {
                this.fetchClient.fetch(`api/Task?taskId=${taskId}`)
                    .then(response => response.json())
                    .then(data =>
                    {
                        this.taskName = data.name;
                    });
            });
    }

    /**
     * Handles a pilot selection for a timer
     * @param {any} e Event args
     */
    selectPilot = (e: any): void => {
        this.pilotIdAssignedToTimer = e.srcElement.id;
        this.pilotNameAssignedToTimer = e.srcElement.innerHTML;
        this.showPilotList = false;
    }

    /**
     * Adds a score to the list of scored time gates.
     * @param {any} e Event args
     */
    addToScore(e: any): void {
        e.srcElement.style = "backgroundColor:lightgreen";
        this.scoredTimeGates.push(e.srcElement.id);
    }
    
    /**
     * Handles a click on the start / stop button of the timer
     * @param {any} e Event Args
     */
    toggleStopWatch(e: any): void {

        this.isRoundActive = true;

        if (this.isRunning) {
            if (this.startTimer) {
                // Get the time.
                var stopTime = (new Date()).getTime() - this.startTime;
                // Reset.
                this.startTime = 0;
                let formattedTime = this.formatTime(stopTime)
                this.displayRecordedTimes.push(formattedTime);

                this.scoringConnection.invoke('flightTimerStopped', { pilotId: this.pilotIdAssignedToTimer, minutes: formattedTime.split(':')[0], seconds: formattedTime.split(':')[1], timingDeviceId: 1 })
                    .catch(e => {
                        console.log("****" + e)
                });

                // Update the UI.
                this.currentTime = this.formatTime(0);
                // Clear the internal timer.
                clearInterval(this.startTimer);
                this.startStopButtonState = "START";
                this.isRunning = false;
            } else {
                this.startTimer = undefined;
            }
        } else {
            this.startTime = (new Date()).getTime();
            this.startTimer = setInterval(this.timerOnInterval, 10)
            this.startStopButtonState = "STOP";
            this.isRunning = true;
        }

        e.stopPropagation(); e.preventDefault();
    }

    /**
     * Shows the pilot list to select a pilot to time for
     * @param {any} e Event Args
     */
    showPilotsListClick(e: any): void {
        if (this.pilotsInRound !== null && this.pilotsInRound.length > 0) {
            this.showPilotList = this.showPilotList === true ? false : true;
            this.showTimeGates = false;
        }
    }

    /**
     * Shows the score view to allow the user to select the times to submit
     * @param {any} e Event Args
     */
    showScoreViewClick(e: any): void {
        this.showTimeGates = this.showTimeGates === true ? false : true;
        this.showPilotList = this.showTimeGates === true ? true : false;
    }

    /**
     * Event handler, when a user select a time gate to submit
     * @param {any} e Event args
     */
    selectTimeGate(e: any): void {
        if (e.srcElement.id === "") return;

        let index = this.scoredTimeGates.indexOf(e.srcElement.id);

        if (index == -1) {
            // Add the item
            this.scoredTimeGates.push(e.srcElement.id);
            e.srcElement.style = "background-color:green";
        } else {
            // Remove the item
            this.scoredTimeGates.splice(index, 1);
            e.srcElement.style = "";
        }
    }

    /**
     * Event handler, when a user clicks submit on their times.
     * @param {any} e Event args
     */
    sendScore(e: any): void {
        let pilotScore = new FinalTimeSheetPostedEventArgs();
        pilotScore.pilotId = this.pilotIdAssignedToTimer;
        pilotScore.finalTimeSheet = new TimeSheet();
        pilotScore.finalTimeSheet.pilotId = this.pilotIdAssignedToTimer;

        let cntr = 0;
        for (let score of this.scoredTimeGates) {
            pilotScore.finalTimeSheet.timeGates.push(new TimeGate(this.removeMilliseconds(this.displayRecordedTimes[score]), cntr));    
            ++cntr;
        }

        // Post it to the hub
        this.scoringConnection.invoke("FinalTimeSheetPosted", pilotScore);
        this.resetEntireTimer();
    }

    /**
     * Removes milliseconds from a precision time string
     * @param {string} time The time to parse
     */
    removeMilliseconds(time: string): string {
        let split = time.split(':');
        return `00:${split[0]}:${split[1]}`;
    }

    /**
     * Resets everything
     */
    resetEntireTimer(): void {
        this.showPilotList = false;
        this.showTimeGates = false;
        this.pilotIdAssignedToTimer = "";
        this.currentTime = "00:00:00";
        this.displayRecordedTimes = [];
        this.isRunning = false;
        this.pilotsInRound = [];
        this.scoredTimeGates = [];
        this.startTime = 0;
        this.stopTime = 0;
        this.taskName = "";
    }

    /**
     * Adds some zeros to the time to format it properly
     * @param {number} value Time value to pad
     * Ex: 1:09 -> 01:09
     */
    padNumbersWithZeros(value: number): string {
        let valueToReturn: string = value + "";
        if (valueToReturn.length < 2) {
            return "0" + valueToReturn;
        } else {
            return valueToReturn;
        }
    }

    /**
     * Formats the time / interval into a string
     * @param {number} time The time to format.
     */
    formatTime(time:number): string {

        let m: number = 0;
        let s: number = 0;
        let ms: number = 0;
        let newTime: string = '';

        time = time % (60 * 60 * 1000);
        m = Math.floor(time / (60 * 1000));
        time = time % (60 * 1000);
        s = Math.floor(time / 1000);
        ms = time % 1000;
        
        newTime = this.padNumbersWithZeros(m) + ':' + this.padNumbersWithZeros(s) + ':' + this.padNumbersWithZeros(ms);

        return newTime;
    }
}