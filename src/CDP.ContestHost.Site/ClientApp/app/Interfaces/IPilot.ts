export interface IPilot {
    FirstName: string;
    LastName: string;
    Airframe: string;
    AmaNumber: string;
    Id: string;
}

export class Pilot implements IPilot {
    FirstName: string;
    LastName: string;
    Airframe: string;
    AmaNumber: string;
    Id: string;
}