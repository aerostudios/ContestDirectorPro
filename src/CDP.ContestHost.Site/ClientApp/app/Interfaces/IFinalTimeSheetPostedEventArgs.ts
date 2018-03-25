import { ITimeSheet, TimeSheet } from "../Interfaces/ITimeSheet"

export interface IFinalTimeSheetPostedEventArgs {
    finalTimeSheet: ITimeSheet;
    pilotId: string;
    timingDeviceId: string;
}

export class FinalTimeSheetPostedEventArgs implements IFinalTimeSheetPostedEventArgs {
    finalTimeSheet: ITimeSheet;
    pilotId: string;
    timingDeviceId: string;

    constructor() {
        this.finalTimeSheet = new TimeSheet();
        this.pilotId = "";
        this.timingDeviceId = "";
    }
}