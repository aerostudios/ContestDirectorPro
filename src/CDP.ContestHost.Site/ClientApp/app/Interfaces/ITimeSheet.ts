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
}