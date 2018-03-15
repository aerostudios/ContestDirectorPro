
export class Timer {

    public displayHours: string;
    public displayMinutes: string;
    public displaySeconds: string;
    public displayMilliseconds: string;

    private internalTimer: any;
    private direction: boolean; // true == forward, false == backward
    private timerIntervalInMilliseconds: number;
    private totalTimeInMilliseconds: number;
    private callerCallBack: () => any;

    constructor(direction: boolean) {
        this.direction = direction;
    }

    set = (clockTime: string): void => {

        clearInterval(this.internalTimer);

        let time: string[] = clockTime.split(":");

        if (time.length < 3) {
            console.log("Bad clock time sent to the timer.");
            return;
        }

        let hours: number = parseInt(time[0]);
        let minutes: number = parseInt(time[1]);
        let seconds: number = parseInt(time[2]);

        this.totalTimeInMilliseconds = (((hours * 60) * 60) * 1000) + ((minutes * 60) * 1000) + (seconds * 1000);
    }

    start = (intervalInMilliseconds: number, callback: () => any): void => {
        this.callerCallBack = callback;
        this.timerIntervalInMilliseconds = intervalInMilliseconds;
        clearInterval(this.internalTimer);
        this.internalTimer = setInterval(this.onInterval, intervalInMilliseconds);
    }

    stop = (): void => {
        clearInterval(this.internalTimer);
    }

    private onInterval = (): void => {
        if (this.direction) {
            this.totalTimeInMilliseconds = this.totalTimeInMilliseconds + this.timerIntervalInMilliseconds;
        } else {
            this.totalTimeInMilliseconds = this.totalTimeInMilliseconds - this.timerIntervalInMilliseconds;
        }

        this.displayHours = `${this.padNumbersWithZeros(Math.floor(((this.totalTimeInMilliseconds / 1000) / 60) / 60))}`;
        this.displayMinutes = `${this.padNumbersWithZeros(Math.floor(((this.totalTimeInMilliseconds / 1000) / 60) % 60))}`;
        this.displaySeconds = `${this.padNumbersWithZeros(Math.floor((this.totalTimeInMilliseconds / 1000) % 60))}`;
        this.displayMilliseconds = `${this.padNumbersWithZeros(Math.floor(this.totalTimeInMilliseconds % 1000))}`;

        this.callerCallBack();
    }

    // Adds some zeros to the time to format it properly
    private padNumbersWithZeros = (value: number): string => {
        let valueToReturn: string = value + "";
        if (valueToReturn.length < 2) {
            return "0" + valueToReturn;
        } else {
            return valueToReturn;
        }
    }
}