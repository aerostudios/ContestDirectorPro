import { HttpClient } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class ContestPage {
    public name: string;
    public startDate: string;
    public endDate: string;
    public currentRoundNumber: number;
    public currentFlightGroup: number;
    public hasStarted: boolean;
    public numberOfRounds: number;
    public numberOfPilots: number;
    public registrationFee: number;
    public numberOfFlyoffRounds: number;
    public allowDroppedRound: boolean;
    public pilotRoster: string[];
    public rounds: { [id: string]: any };
    public flightMatrix: any;

    private httpClient: HttpClient;

    /**
     * The class ctor
     * @param {HttpClient} http The injected http fetch client  TODO: include polyfill for older browsers
     */
    constructor(http: HttpClient) {
        this.httpClient = http;

        http.fetch('api/Contest')
            .then(result => result.json() as Promise<any>)
            .then(data => {
                console.log(data);
                console.log(this);
                this.name = data.name;
                this.allowDroppedRound = data.allowDroppedRound;
                this.endDate = data.endDate;
                this.startDate = data.startDate;
                this.currentRoundNumber = data.state.currentRoundOrdinal;
                this.currentFlightGroup = data.state.currentFlightGroup;
                this.hasStarted = data.state.hasStarted;
                this.numberOfFlyoffRounds = data.numberOfFlyoffRounds;
                this.pilotRoster = data.pilotRoster;
                this.registrationFee = data.contestRegistrationFee;
                this.rounds = data.rounds;
                this.numberOfPilots = data.pilotRoster.length;
                this.numberOfRounds = Object.keys(data.rounds).length;

                return data.id;
            })
            .then(contestId => {
                http.fetch(`api/FlightMatrix/${contestId}`)
                    .then(result => result.json() as Promise<any>)
                    .then(data => {
                        console.log(data);
                        this.flightMatrix = data;
                    });
            });
    }

    /**
     * Aurelia construct.  Handles a page refresh (TODO: Do we need this?)
     */
    Refresh(): void {
        this.httpClient.fetch('api/Contest')
            .then(result => result.json() as Promise<any>)
            .then(data => {
                this.name = data.name;
                this.allowDroppedRound = data.allowDroppedRound;
                this.endDate = data.endDate;
                this.startDate = data.startDate;
                this.currentRoundNumber = data.state.currentRoundOrdinal;
                this.currentFlightGroup = data.state.currentFlightGroup;
                this.hasStarted = data.state.hasStarted;
                this.numberOfFlyoffRounds = data.numberOfFlyoffRounds;
                this.pilotRoster = data.pilotRoster;
                this.registrationFee = data.contestRegistrationFee;
                this.rounds = data.rounds;
                this.numberOfPilots = data.pilotRoster.length;
                this.numberOfRounds = Object.keys(data.rounds).length;
                return data.id;
            })
            .then(contestId => {
                this.httpClient.fetch(`api/FlightMatrix/${contestId}`)
                    .then(result => result.json() as Promise<any>)
                    .then(data => {
                        console.log(data);
                        this.flightMatrix = data;
                    });
            });
    }
}

// TODO: Move this
interface Contest {
    name: string;
    startDate: string;
    endDate: string;
    registrationFee: number;
    numberOfFlyoffRounds: number;
    allowDroppedRound: boolean;
    rounds: { [id: string]: any };
    pilotRoster: string[];

    GetNumberOfRounds(): number;
}