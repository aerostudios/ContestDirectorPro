import { ITimeGate } from "../Interfaces/ITimeGate"

export interface ITask {
    Id: string;
    TimeGates: ITimeGate[];
    Description: string;
    IsLandingScored: boolean;
    Name: string;
    Type: string;
    NumberOfTimeGatesAllowed: number;
}