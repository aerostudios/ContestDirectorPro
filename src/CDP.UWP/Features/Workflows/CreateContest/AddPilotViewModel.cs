//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: AddPilotViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.CreateContest
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Pilots;
    using CDP.CoreApp.Features.Pilots.Commands;
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Helpers;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Defines a view model for the Add pilot page of the create contest workflow.
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class AddPilotViewModel : CdproViewModelBase
    {
        #region Private members

        /// <summary>
        /// The previous page
        /// </summary>
        private Type previousPage = null;

        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string amaNumber = string.Empty;
        private string airFrame = string.Empty;
        private Contest contest;

        /// <summary>
        /// The pilot query interactor
        /// </summary>
        private Lazy<PilotStorageCmdInteractor> pilotCmdIntr = new Lazy<PilotStorageCmdInteractor>(
            () => new PilotStorageCmdInteractor(
                    new PilotFileSystemRepository(StorageFileConfig.PilotsFileName, App.Cache, App.Logger), App.Logger));

        #endregion

        #region Properties

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
        /// Gets or sets the ama number.
        /// </summary>
        /// <value>
        /// The ama number.
        /// </value>
        public string AmaNumber { get { return amaNumber; } set { Set(ref amaNumber, value); } }

        /// <summary>
        /// Gets or sets the airframe.
        /// </summary>
        /// <value>
        /// The airframe.
        /// </value>
        public string Airframe { get { return airFrame; } set { Set(ref airFrame, value); } }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPilotViewModel"/> class.
        /// </summary>
        public AddPilotViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // design-time experience
            }
            else
            {
                // runtime experience
            }
        }

        #endregion

        #region public Methods

        /// <summary>
        /// Called when [navigated to asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            // Check to see where we came from
            var previousPage = parameter as Type;
            if (previousPage != null)
            {
                this.previousPage = previousPage;
            }

            if (state.Any())
            {
                // restore state
                state.Clear();
            }
            else
            {
                var parameters = parameter as AddPilotParameters;

                if (parameters == null)
                {
                    var ex = new Exception($"{nameof(AddPilotViewModel)}:{nameof(OnNavigatedToAsync)}: Parameters object was not passed in.");
                    base.Throw(ex, "Could not load the contest.");
                }

                this.contest = parameters.Contest;
                this.previousPage = parameters.OriginatingPage;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Called when [navigated from asynchronous].
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="suspending">if set to <c>true</c> [suspending].</param>
        /// <returns></returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {
                // save state
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatingFromAsync" /> event.
        /// </summary>
        /// <param name="args">The <see cref="NavigatingEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Saves the and return.
        /// </summary>
        public async Task SaveAndReturn()
        {
            var sucess = await SaveNewPilot(this);
            if (sucess)
            {
                NavigationService.Navigate(previousPage, this.contest);
            }
        }

        /// <summary>
        /// Saves the and reload.
        /// </summary>
        public async Task SaveAndReload()
        {
            await SaveNewPilot(this);
            ClearFields();
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        /// <returns></returns>
        public void Cancel()
        {
            NavigationService.Navigate(previousPage, this.contest);
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Saves the new pilot.
        /// </summary>
        /// <param name="addPilotViewModel">The add pilot view model.</param>
        /// <returns></returns>
        private async Task<bool> SaveNewPilot(AddPilotViewModel addPilotViewModel)
        {
            var pilotToAdd = new Pilot(addPilotViewModel.firstName,
                addPilotViewModel.LastName,
                string.Empty,
                addPilotViewModel.amaNumber,
                addPilotViewModel.Airframe);

            var savePilotResult = await this.pilotCmdIntr.Value.CreateNewPilotAsync(pilotToAdd);

            if (savePilotResult.IsFaulted)
            {
                base.Alert($"{nameof(AddPilotViewModel)}:{nameof(SaveNewPilot)} - Failed to add new pilot.");
            }
            
            return !savePilotResult.IsFaulted;
        }
        
        /// <summary>
        /// Clears the fields.
        /// </summary>
        private void ClearFields()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            AmaNumber = string.Empty;
            Airframe = string.Empty;
        }

        #endregion
    }
}
