import * as signalR from '@aspnet/signalr-client'

export class SignalRConnector {

    private logger: any = new signalR.ConsoleLogger(signalR.LogLevel.Information);

    public scoringConnection: signalR.HubConnection;

    constructor() {

        this.scoringConnection = new signalR.HubConnection('/scoring');

        // Bind to the appropriate events 
        this.scoringConnection.onclose = e => {
            console.log('connection closed' + e);
        };

        // Start up the connection to the server
        this.scoringConnection.start()
            .then(() => {
                console.log('Hub connection started')
            })
            .catch(err => {
                console.log('Error while establishing connection')
            });
    }
}