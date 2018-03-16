import { IPilot } from "../Interfaces/IPilot"
import { ITask } from "../Interfaces/ITask"

export interface INewRoundAvailableEventArgs {

    pilots: IPilot[];
    flightGroup: string;
    task: ITask;
}