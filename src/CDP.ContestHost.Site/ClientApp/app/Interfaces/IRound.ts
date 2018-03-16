import { IPilot } from "../Interfaces/IPilot"

export interface IRound {
    assignedTask: string;
    flightGroups: { [flightGroup: string]: IPilot };
}