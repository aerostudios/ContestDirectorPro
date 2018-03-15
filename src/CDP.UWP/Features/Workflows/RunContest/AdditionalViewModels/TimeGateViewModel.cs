//---------------------------------------------------------------
// Date: 12/16/2017
// Rights: 
// FileName: PilotScoreViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using CDP.AppDomain.Tasks;
    using System;
    using Template10.Mvvm;

    /// <summary>
    /// View model for a time gate, for a pilot, in a flight group on the Contest Rounds page.
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class TimeGateViewModel : ViewModelBase
    {
        /// <summary>
        /// The time
        /// </summary>
        private string time;

        /// <summary>
        /// The gate type
        /// </summary>
        private TimeGateType gateType;

        /// <summary>
        /// The ordinal
        /// </summary>
        private int ordinal = 0;

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public string Time { get { return time; } set { Set(ref time, value); } }

        /// <summary>
        /// Gets or sets the type of the gate.
        /// </summary>
        /// <value>
        /// The type of the gate.
        /// </value>
        public TimeGateType GateType { get { return gateType; } set { Set(ref gateType, value); } }

        /// <summary>
        /// Gets or sets the ordinal.
        /// </summary>
        /// <value>
        /// The ordinal.
        /// </value>
        public int Ordinal { get { return ordinal; } set { Set(ref ordinal, value); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGateViewModel"/> class.
        /// </summary>
        public TimeGateViewModel()
        {
            this.Time = "0:00";
        }

        /// <summary>
        /// Parses the time.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TimeSpan ParseTime()
        {
            var timeSplit = time.Split(":", StringSplitOptions.None);

            var success = int.TryParse(timeSplit[0], out var minutes);
            if (!success) { throw new Exception("Error parsing time."); }
            success = int.TryParse(timeSplit[1], out var seconds);
            if (!success) { throw new Exception("Error parsing time."); }

            return new TimeSpan(0, minutes, seconds);
        }

        /// <summary>
        /// To the time gate.
        /// </summary>
        /// <returns></returns>
        public TimeGate ToTimeGate()
        {
            return new TimeGate
            {
                GateType = TimeGateType.Task,
                Ordinal = this.ordinal,
                Time = this.ParseTime()
            };
        }
    }
}