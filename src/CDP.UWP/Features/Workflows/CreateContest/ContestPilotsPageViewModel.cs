//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: ContestPilotsPageViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.CreateContest
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Pilots;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.Pilots.Queries;
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Features.Workflows.CreateContest.AdditionalViewModels;
    using CDP.UWP.Helpers;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Defines a view model for the Contest pilots page in the create contest workflow.
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class ContestPilotsPageViewModel : CdproViewModelBase
    {
        #region Instance Methods
        
        private IEnumerable<Pilot> pilots;
        private Contest contest;

        /// <summary>
        /// The pilot query interactor
        /// </summary>
        private Lazy<PilotQueryInteractor> pilotQueryIntr = new Lazy<PilotQueryInteractor>(
            () => new PilotQueryInteractor(
                    new PilotFileSystemRepository(StorageFileConfig.PilotsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The contest storage interactor
        /// </summary>
        private Lazy<ContestStorageCmdInteractor> contestStorageInteractor = new Lazy<ContestStorageCmdInteractor>(
            () => new ContestStorageCmdInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));
        
        #endregion

        #region Properties

        /// <summary>
        /// The selected contest pilots
        /// </summary>
        private ObservableCollection<PilotListItemViewModel> availablePilots = new ObservableCollection<PilotListItemViewModel>();

        /// <summary>
        /// The selected contest pilots
        /// </summary>
        private ObservableCollection<PilotListItemViewModel> selectedContestPilots = new ObservableCollection<PilotListItemViewModel>();

        /// <summary>
        /// Gets or sets the selected contest pilots.
        /// </summary>
        /// <value>
        /// The selected contest pilots.
        /// </value>
        public ObservableCollection<PilotListItemViewModel> SelectedContestPilots { get { return selectedContestPilots; } set { Set(ref selectedContestPilots, value); } }

        /// <summary>
        /// Gets or sets the available pilots.
        /// </summary>
        /// <value>
        /// The available pilots.
        /// </value>
        public ObservableCollection<PilotListItemViewModel> AvailablePilots { get { return availablePilots; } set { Set(ref availablePilots, value); } }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestPilotsViewModel"/> class.
        /// </summary>
        public ContestPilotsPageViewModel()
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

        #region Public Methods

        #region Navigation

        /// <summary>
        /// Called when [navigated to asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.Any())
            {
                if (state[nameof(ContestPilotsPageViewModel)] is ContestPilotsPageViewModel previousState)
                {
                    this.AvailablePilots = previousState.AvailablePilots;
                }
                state.Clear();
                return;
            }

            state.Clear();

            this.contest = parameter as Contest;

            if (this.contest == null)
            {
                var ex = new Exception($"{nameof(ContestPilotsPageViewModel)}:{nameof(OnNavigatedToAsync)}: Contest object was not passed in.");
                base.Throw(ex, "Could not load the contest.");
            }

            // Grab all of the pilots that we have currently
            var result = await this.pilotQueryIntr.Value.GetAllPilotsAsync();

            if (result.IsFaulted)
            {
                base.Alert($"{nameof(ContestPilotsPageViewModel)}:{nameof(OnNavigatedToAsync)} - Failed to get pilots.");
            }

            this.pilots = result.Value;

            if (contest == null) { throw new Exception("Invaid page context."); }

            // Populate the Available pilots
            foreach (var p in pilots)
            {
                // Don't add to the available list if they are already selected to fly in the contest.
                if (!contest.PilotRoster.Any(contestPilot => contestPilot == p.Id))
                {
                    this.NavigationService.Dispatcher.Dispatch(new Action(() => { this.AvailablePilots.Add(new PilotListItemViewModel(p)); }));
                }
            }

            // Clear out the list, seems like there are items in there already...
            this.NavigationService.Dispatcher.Dispatch(new Action(() => { this.selectedContestPilots.Clear(); }));

            // Populate the Selected Pilots
            foreach (var pId in contest.PilotRoster)
            {
                this.NavigationService.Dispatcher.Dispatch(new Action(() => { this.SelectedContestPilots.Add(new PilotListItemViewModel(pilots.Where(p => p.Id == pId).FirstOrDefault())); }));
            }
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
                state[nameof(ContestPilotsPageViewModel)] = this;
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

        #endregion

        #region Event Handlers

        /// <summary>
        /// Adds the available pilot.
        /// </summary>
        /// <param name="pilotListItemViewModel">The pilot list item view model.</param>
        public void AddAvailablePilot(PilotListItemViewModel pilotListItemViewModel)
        {
            this.Dispatcher.Dispatch(new Action(() => { this.AvailablePilots.Add(pilotListItemViewModel); }));
        }

        /// <summary>
        /// Adds the selected pilot.
        /// </summary>
        /// <param name="pilotListItemViewModel">The pilot list item view model.</param>
        public void AddSelectedPilot(PilotListItemViewModel pilotListItemViewModel)
        {
            this.Dispatcher.Dispatch(new Action(() => { this.SelectedContestPilots.Add(pilotListItemViewModel); }));
        }

        /// <summary>
        /// Removes the available pilot.
        /// </summary>
        /// <param name="pilotListItemViewModel">The pilot list item view model.</param>
        public void RemoveAvailablePilot(PilotListItemViewModel pilotListItemViewModel)
        {
            this.Dispatcher.Dispatch(new Action(() => { this.AvailablePilots.Remove(pilotListItemViewModel); }));
        }

        /// <summary>
        /// Removes the selected pilot.
        /// </summary>
        /// <param name="pilotListItemViewModel">The pilot list item view model.</param>
        internal void RemoveSelectedPilot(PilotListItemViewModel pilotListItemViewModel)
        {
            this.Dispatcher.Dispatch(new Action(() => { this.SelectedContestPilots.Remove(pilotListItemViewModel); }));
        }

        /// <summary>
        /// Saves the and go to next page.
        /// </summary>
        public async Task SaveAndGoToNextPage()
        {
            var success = await SavePilotsList(this);
            if (success) { NavigationService.Navigate(typeof(ContestTasksPage), this.contest); }
        }

        /// <summary>
        /// Adds the new pilot.
        /// </summary>
        /// <returns></returns>
        public async Task AddNewPilot()
        {
            var success = await SavePilotsList(this);
            if (success) { NavigationService.Navigate(typeof(AddPilotPage), new AddPilotParameters(this.contest, typeof(ContestPilotsPage))); }
        }
        
        #endregion 

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves the pilots list.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        private async Task<bool> SavePilotsList(ContestPilotsPageViewModel viewModel)
        {
            if (contest == null) { throw new Exception("page context is corrupt."); }

            contest.PilotRoster.Clear();
            contest.PilotRoster.AddRange(SelectedContestPilots.Select(scp => scp.Id));

            var contestSaveResult = await this.contestStorageInteractor.Value.UpdateContestAsync(contest);

            if (contestSaveResult.IsFaulted)
            {
                App.Logger.LogTrace($"{nameof(ContestPilotsPageViewModel)}:{nameof(SavePilotsList)} - Failed to update contest.");
            }

            return !contestSaveResult.IsFaulted;
        }

        #endregion
    }
}
