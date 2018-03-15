import * as signalR from '@aspnet/signalr-client'
import { SignalRConnector } from '../../../components/SignalRConnector/SignalRConnector'
import { inject } from 'aurelia-framework';

@inject(SignalRConnector)
export class HomePage {
    private logger: any = new signalR.ConsoleLogger(signalR.LogLevel.Information);
    private scoringConnection: signalR.HubConnection;

    public displayRecordedTimes: string[] = [];

    constructor(signalr: SignalRConnector) {

        this.scoringConnection = signalr.scoringConnection;

        this.scoringConnection.on("roundTimerStarted", data => {
            this.displayRecordedTimes.push("Recieved new time");
        });

        this.scoringConnection.on("flightTimerStopped", (args) => {
            this.displayRecordedTimes.push(`PilotId:${args.pilotId}  ${args.minutes}:${args.seconds}`);
        });
    }
}

