import { ITimeGate } from "../Interfaces/ITimeGate"

export interface ITimeSheet {
    id: string;
    contestId: string;
    taskId: string;
    pilotId: string;
    roundOrdinal: number;
    flightGroup: string;
    timeGates: ITimeGate[];
    score: number;
    totalPenalties: number;
}

export class TimeSheet implements ITimeSheet {
    id: string;
    contestId: string;
    taskId: string;
    pilotId: string;
    roundOrdinal: number;
    flightGroup: string;
    timeGates: ITimeGate[] = [];
    score: number;
    totalPenalties: number;

    constructor() {
        this.id = "";
        this.contestId = "";
        this.taskId = "";
        this.pilotId = "";
        this.roundOrdinal = 0;
        this.flightGroup = "";
        this.score = 0;
        this.totalPenalties = 0;
    }
}