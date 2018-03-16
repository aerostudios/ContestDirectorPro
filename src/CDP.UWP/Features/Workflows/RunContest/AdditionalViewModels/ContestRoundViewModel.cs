//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: ContestRoundViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using CDP.AppDomain.Tasks;
    using System.Collections.ObjectModel;
    using Template10.Mvvm;

    /// <summary>
    /// View model for a contest round on the Contest Rounds Page.
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class ContestRoundViewModel : ViewModelBase
    {
        private string displayNumber;
        private int ordinal;
        private ObservableCollection<FlightGroupViewModel> _flightGroups = new ObservableCollection<FlightGroupViewModel>();
        private TaskBase task;

        /// <summary>
        /// Gets or sets the display number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public string DisplayNumber { get { return displayNumber; } set { Set(ref displayNumber, value); } }

        /// <summary>
        /// Gets or sets the ordinal.
        /// </summary>
        /// <value>
        /// The ordinal.
        /// </value>
        public int Ordinal { get { return ordinal; } set { Set(ref ordinal, value); } }

        /// <summary>
        /// Gets or sets the flight groups.
        /// </summary>
        /// <value>
        /// The flight groups.
        /// </value>
        public ObservableCollection<FlightGroupViewModel> FlightGroups { get { return _flightGroups; } set { Set(ref _flightGroups, value); } }

        /// <summary>
        /// Gets or sets the task.
        /// </summary>
        /// <value>
        /// The task.
        /// </value>
        public TaskBase Task { get { return task; } set { Set(ref task, value); } }
    }
}
