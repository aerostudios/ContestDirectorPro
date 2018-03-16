//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: FlightGroupViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Tasks;
    using System.Collections.ObjectModel;
    using Template10.Mvvm;

    /// <summary>
    /// View model for a flight group on the Contest Rounds page
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class FlightGroupViewModel : ViewModelBase
    {
        private string flightGroupName;
        private FlightGroup flightGroup;
        private TaskBase roundTask;
        private ObservableCollection<PilotScoreViewModel> pilots;
        private string roundNumber;
        public bool hasBeenScored = false;
        public bool isEditScoresEnabled = false;

        /// <summary>
        /// Gets or sets the round task.
        /// </summary>
        /// <value>
        /// The round task.
        /// </value>
        public TaskBase RoundTask { get { return roundTask; } set { Set(ref roundTask, value); } }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public string FlightGroupName { get { return flightGroupName; } set { Set(ref flightGroupName, value); } }

        /// <summary>
        /// Gets or sets the flight group.
        /// </summary>
        /// <value>
        /// The flight group.
        /// </value>
        public FlightGroup FlightGroup { get { return flightGroup; } set { Set(ref flightGroup, value); } }

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>
        /// The round number.
        /// </value>
        public string DisplayRoundNumber { get { return roundNumber; } set { Set(ref roundNumber, value); } }

        /// <summary>
        /// Gets or sets the pilots.
        /// </summary>
        /// <value>
        /// The pilots.
        /// </value>
        public ObservableCollection<PilotScoreViewModel> Pilots { get { return pilots; } set { Set(ref pilots, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has been scored.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has been scored; otherwise, <c>false</c>.
        /// </value>
        public bool HasBeenScored { get { return hasBeenScored; } set { Set(ref hasBeenScored, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is edit scores enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is edit scores enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditScoresEnabled { get { return isEditScoresEnabled; } set { Set(ref isEditScoresEnabled, value); } }

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightGroupViewModel"/> class.
        /// </summary>
        public FlightGroupViewModel() { }

        #endregion

        #region Public Methods

        #region Event Handlers


        #endregion

        #endregion
    }
}
