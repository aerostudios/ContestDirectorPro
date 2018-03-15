//------------------------------------------------------------
// Origin: AeroStudios
// Author: Rick Rogahn
// FileName: PilotRoundMatrixListItemViewModel.cs
//------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using System.Collections.ObjectModel;
    using Template10.Mvvm;

    /// <summary>
    /// Defines a list item for the flight matrix page
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class PilotRoundMatrixListItemViewModel : ViewModelBase
    {
        private string pilotName = string.Empty;
        private ObservableCollection<string> flightGroups = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the name of the pilot.
        /// </summary>
        /// <value>
        /// The name of the pilot.
        /// </value>
        public string PilotName { get { return pilotName; } set { Set(ref pilotName, value); } }

        /// <summary>
        /// Gets or sets the flight groups.
        /// </summary>
        /// <value>
        /// The flight groups.
        /// </value>
        public ObservableCollection<string> FlightGroups { get { return flightGroups; } set { Set(ref flightGroups, value); } }
    }
}
