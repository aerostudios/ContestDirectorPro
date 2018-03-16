
export interface ITimeGate {
    time: string;
    ordinal: number;
    gateType: string;
}

export class TimeGate implements ITimeGate {
    time: string;
    ordinal: number;
    gateType: string;

    constructor(timeToRecord: string, ordinal: number) {
        this.time = timeToRecord;
        this.ordinal = ordinal;
    }
}