//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: PilotScoreViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Scoring;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Template10.Mvvm;

    /// <summary>
    /// View model for a row in a flight group, on the Contest Rounds page.
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class PilotScoreViewModel : ViewModelBase
    {
        private string name;
        private string pilotId;
        private ObservableCollection<TimeGateViewModel> gates = new ObservableCollection<TimeGateViewModel>();
        private double score;
        private double penalty;
        private bool isValid = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotScoreViewModel"/> class.
        /// </summary>
        public PilotScoreViewModel() { }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get { return name; } set { Set(ref name, value); } }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string PilotId { get { return pilotId; } set { Set(ref pilotId, value); } }

        /// <summary>
        /// Gets or sets the penalty.
        /// </summary>
        /// <value>
        /// The penalty.
        /// </value>
        public double Penalty { get { return penalty; } set { Set(ref penalty, value); } }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public double Score { get { return score; } set { Set(ref score, value); } }

        /// <summary>
        /// Returns true if the times presented to this model were valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get { return isValid; } set { Set(ref isValid, value); } }

        /// <summary>
        /// Gets or sets the gates.
        /// </summary>
        /// <value>
        /// The gates.
        /// </value>
        public ObservableCollection<TimeGateViewModel> Gates { get { return gates; } set { Set(ref gates, value); } }

        /// <summary>
        /// To the time sheet.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal TimeSheet ToTimeSheet(string contestId, FlightGroup flightGroup, int roundOrdinal, string taskId)
        {
            return new TimeSheet
            {
                ContestId = contestId,
                FlightGroup = flightGroup,
                PilotId = this.pilotId,
                RoundOrdinal = roundOrdinal,
                Score = this.score,
                TaskId = taskId,
                TimeGates = this.Gates.Select(gate => gate.ToTimeGate()).ToList()
            };
        }
    }
}