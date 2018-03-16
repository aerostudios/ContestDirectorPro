//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: TimeGate.cs
//---------------------------------------------------------------

namespace CDP.AppDomain.Tasks
{
    using System;

    /// <summary>
    /// Defines a time measurement. Can be used for recording times or defining time slots for a task.
    /// </summary>
    public class TimeGate
    {
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Gets or sets the ordinal.
        /// </summary>
        /// <value>
        /// The ordinal.
        /// </value>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the type of the gate.
        /// </summary>
        /// <value>
        /// The type of the gate.
        /// </value>
        public TimeGateType GateType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGate" /> class.
        /// </summary>
        public TimeGate()
            : this(TimeSpan.FromSeconds(0), 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGate" /> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="gateType">Type of the gate.</param>
        public TimeGate(TimeSpan time, int ordinal, TimeGateType gateType = TimeGateType.Task)
        {
            this.Time = time;
            this.Ordinal = ordinal;
            this.GateType = gateType;
        }
    }
}
