//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: PilotRegistrationListViewItemViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Template10.Mvvm;

    /// <summary>
    /// View model for a row in the pilots registration page
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class PilotRegistrationListViewItemViewModel : ViewModelBase
    {
        #region Instance Members

        private string registrationId = string.Empty;
        private string pilotid = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string amaNumber = string.Empty;
        private ObservableCollection<string> airframes = new ObservableCollection<string>();
        private bool isPaid = false;
        private bool isAirframeRegistered = false;
        private ObservableCollection<string> airframeNumbers = new ObservableCollection<string>();
        private bool isNewPilot = false;
        private string contestId;
        private bool isMarkedForRemoval = false;
        private bool isInEditMode = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the registration identifier.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        public string RegistrationId { get { return registrationId; } set { Set(ref registrationId, value); } }

        /// <summary>
        /// Gets or sets the pilot identifier.
        /// </summary>
        /// <value>
        /// The pilot identifier.
        /// </value>
        public string PilotId { get { return pilotid; } set { Set(ref pilotid, value); } }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get { return firstName; } set { Set(ref firstName, value); } }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get { return lastName; } set { Set(ref lastName, value); } }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName => string.Format("{0} {1}", firstName, lastName);

        /// <summary>
        /// Gets or sets the ama number.
        /// </summary>
        /// <value>
        /// The ama number.
        /// </value>
        public string AmaNumber { get { return amaNumber; } set { Set(ref amaNumber, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is paid; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaid { get { return isPaid; } set { Set(ref isPaid, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is air frame registered.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is air frame registered; otherwise, <c>false</c>.
        /// </value>
        public bool AirframesSignedOff { get { return isAirframeRegistered; } set { Set(ref isAirframeRegistered, value); } }

        /// <summary>
        /// Gets a value indicating whether this instance is registration complete.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is registration complete; otherwise, <c>false</c>.
        /// </value>
        public bool IsRegistrationComplete { get { return isPaid && isAirframeRegistered; } }

        /// <summary>
        /// Gets or sets the airframes.
        /// </summary>
        /// <value>
        /// The airframes.
        /// </value>
        public ObservableCollection<string> Airframes { get { return airframes; } set { Set(ref airframes, value); } }

        /// <summary>
        /// Gets or sets the airframe numbers.
        /// </summary>
        /// <value>
        /// The airframe numbers.
        /// </value>
        public ObservableCollection<string> AirframeNumbers { get { return airframeNumbers; } set { Set(ref airframeNumbers, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new pilot.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is new pilot; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewPilot { get { return isNewPilot; } set { Set(ref isNewPilot, value); } }  // This property is a hack...

        /// <summary>
        /// Gets or sets the contest identifier.
        /// </summary>
        /// <value>
        /// The contest identifier.
        /// </value>
        public string ContestId { get { return contestId; } set { Set(ref contestId, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is marked for removal.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is marked for removal; otherwise, <c>false</c>.
        /// </value>
        public bool IsMarkedForRemoval { get { return isMarkedForRemoval; } set { Set(ref isMarkedForRemoval, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in edit mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsInEditMode { get { return isInEditMode; } set { Set(ref isInEditMode, value); } }

        #endregion

        /// <summary>
        /// Edits the mode clicked.
        /// </summary>
        public void EditModeClicked()
        {
            App.Current.NavigationService.Dispatcher.DispatchAsync(() => this.IsInEditMode = isInEditMode ? false : true);
        }
    }
}
