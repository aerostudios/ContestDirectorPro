
export interface ITimeGate {
    time: string;
    ordinal: number;
    gateType: number;
}

export class TimeGate implements ITimeGate {
    time: string;
    ordinal: number;
    gateType: number;

    constructor(timeToRecord: string, ordinal: number, gateType: number) {
        this.time = timeToRecord;
        this.ordinal = ordinal;
        this.gateType = gateType;
    }
}