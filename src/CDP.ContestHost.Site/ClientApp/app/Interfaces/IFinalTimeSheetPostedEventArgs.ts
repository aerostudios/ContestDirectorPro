import { ITimeSheet } from "../Interfaces/ITimeSheet"

export interface IFinalTimeSheetPostedEventArgs {
    finalTimeSheet: ITimeSheet;
    pilotId: string;
    timingDeviceId: string;
}

export class FinalTimeSheetPostedEventArgs implements IFinalTimeSheetPostedEventArgs {
    finalTimeSheet: ITimeSheet;
    pilotId: string;
    timingDeviceId: string;
}