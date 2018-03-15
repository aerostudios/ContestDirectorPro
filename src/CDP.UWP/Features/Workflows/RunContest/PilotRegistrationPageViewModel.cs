//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: PilotRegistrationPageViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Registration;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.Contests.Queries;
    using CDP.CoreApp.Features.Pilots.Commands;
    using CDP.CoreApp.Features.Pilots.Queries;
    using CDP.CoreApp.Features.Registration.Commands;
    using CDP.CoreApp.Features.Registration.Queries;
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Features.Workflows.CreateContest;
    using CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels;
    using CDP.UWP.Helpers;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// View model for the pilot registration page.
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class PilotRegistrationPageViewModel : CdproViewModelBase
    {
        #region Instance Members

        private ObservableCollection<PilotRegistrationListViewItemViewModel> pilotsToRegister =
            new ObservableCollection<PilotRegistrationListViewItemViewModel>();
        private double totalMoneyCollected;
        private Contest contest;
        private List<Pilot> pilotsInContest = new List<Pilot>();

        /// <summary>
        /// The contest query interactor
        /// </summary>
        private Lazy<ContestQueryInteractor> contestQueryIntr = new Lazy<ContestQueryInteractor>(
            () => new ContestQueryInteractor(new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The contest command interactor
        /// </summary>
        private Lazy<ContestStorageCmdInteractor> contestCmdIntr = new Lazy<ContestStorageCmdInteractor>(
            () => new ContestStorageCmdInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The registration command interactor
        /// </summary>
        private Lazy<PilotRegistrationStorageCmdInteractor> registrationCmdIntr = new Lazy<PilotRegistrationStorageCmdInteractor>(
            () => new PilotRegistrationStorageCmdInteractor(
                    new RegistrationRepository(StorageFileConfig.RegistrationFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The registration query interactor
        /// </summary>
        private Lazy<PilotRegistrationQueryInteractor> registrationQueryIntr = new Lazy<PilotRegistrationQueryInteractor>(
            () => new PilotRegistrationQueryInteractor(
                    new RegistrationRepository(StorageFileConfig.RegistrationFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The pilot query interactor
        /// </summary>
        private Lazy<PilotQueryInteractor> pilotQueryInteractor = new Lazy<PilotQueryInteractor>(
            () => new PilotQueryInteractor(
                    new PilotFileSystemRepository(StorageFileConfig.PilotsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The pilot command interactor
        /// </summary>
        private Lazy<PilotStorageCmdInteractor> pilotCmdInteractor = new Lazy<PilotStorageCmdInteractor>(
            () => new PilotStorageCmdInteractor(
                new PilotFileSystemRepository(StorageFileConfig.PilotsFileName, App.Cache, App.Logger), App.Logger));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pilots to register.
        /// </summary>
        /// <value>
        /// The pilots to register.
        /// </value>
        public ObservableCollection<PilotRegistrationListViewItemViewModel> PilotsToRegister
        {
            get { return pilotsToRegister; }
            set { Set(ref pilotsToRegister, value); }
        }

        /// <summary>
        /// Gets or sets the total money to be collected.
        /// </summary>
        /// <value>
        /// The total money to be collected.
        /// </value>
        public double TotalMoneyCollected { get { return totalMoneyCollected; } set { Set(ref totalMoneyCollected, value); } }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotRegistrationViewModel"/> class.
        /// </summary>
        public PilotRegistrationPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                for (int i = 0; i < 12; i++)
                {
                    this.PilotsToRegister.Add(new PilotRegistrationListViewItemViewModel
                    {
                        FirstName = "Rick",
                        LastName = "Rogahn",
                        AmaNumber = "w3r23r08efasdfsa",
                        IsPaid = i % 2 == 0,
                        ContestId = this.contest.Id
                    });
                }
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
                // restore state
                state.Clear();
            }
            else
            {
                // If a contest was passed to this model, load it up for registration
                this.contest = parameter as Contest;

                if (this.contest == null)
                {
                    base.Throw(new Exception($"{nameof(PilotRegistrationPageViewModel)}:{nameof(OnNavigatedToAsync)} - Invalid contest parameter passed to the page."));
                }

                // If the contest has already started, nav to the contest rounds page.
                if (contest.State.HasStarted)
                {
                    this.NavigationService.Navigate(typeof(ContestRoundsPage), new ContestOpenParameters(contest, true));
                }

                await LoadFromContest(contest);
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
        /// Adds the new pilot.
        /// </summary>
        /// <returns></returns>
        public async Task AddNewPilot()
        {
            var success = await SaveRegistrationData();

            if (success)
            {
                NavigationService.Navigate(typeof(ContestPilotsPage), this.contest);
                //NavigationService.Navigate(typeof(EditPilotRegistrationPage),
                //    new PilotRegistrationListViewItemViewModel { ContestId = this.contest.Id, IsNewPilot = true });
            }
            else
            {
                base.Alert("An error occured saving pilot registrations.");
            }
        }

        /// <summary>
        /// Updates the moneys collected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateMoneyCollected(bool isOn)
        {
            this.Dispatcher.DispatchAsync(() => 
            { 
                if (isOn)
                {
                    this.TotalMoneyCollected += this.contest.ContestRegistrationFee;
                }
                else
                {
                    this.TotalMoneyCollected -= this.contest.ContestRegistrationFee;
                }
            });
        }
        
        /// <summary>
        /// Saves the page data and moves to next page.
        /// </summary>
        public async Task SaveAndGoToNextPage()
        {
            if (await SaveRegistrationData())
            {
                this.NavigationService.Navigate(typeof(FlightMatrixPage), this.contest);
            }
            else
            {
                base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(SaveAndGoToNextPage)} - Failed to save the pilot registrations.");
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads from contest object.
        /// </summary>
        /// <param name="contestToLoad">The contest to load.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Could not find previously registered pilot...</exception>
        private async Task LoadFromContest(Contest contestToLoad)
        {
            // The contest pilot roster must match the registrations, so loop through the contest pilot
            // roster and look up the registration, creating a new one if one is not found.
            if (contest.PilotRoster.Count < 1) { return; }

            var contestRegistrationsResult = await this.registrationQueryIntr.Value.GetPilotRegistrationsForContest(contest.Id);
            if (contestRegistrationsResult.IsFaulted)
            {
                base.Alert($"{nameof(PilotRegistrationPageViewModel)}:{nameof(LoadFromContest)} - Failed to get the registrations for contest {contestToLoad.Id}.");
                return;
            }

            var allPilotsResult = await this.pilotQueryInteractor.Value.GetAllPilotsAsync();
            if (contestRegistrationsResult.IsFaulted)
            {
                base.Alert($"{nameof(PilotRegistrationPageViewModel)}:{nameof(LoadFromContest)} - Failed to get the all pilots.");
                return;
            }

            this.pilotsInContest = allPilotsResult.Value.ToList();

            foreach (var pilotId in contest.PilotRoster)
            {
                var pilot = this.pilotsInContest.Where(p => p.Id == pilotId).FirstOrDefault();
                if (pilot == null) { return; }

                var pilotRegistration = contestRegistrationsResult.Value.Where(r => r.PilotId == pilotId).FirstOrDefault();
                PilotsToRegister.Add(new PilotRegistrationListViewItemViewModel
                {
                    FirstName = pilot.FirstName,
                    LastName = pilot.LastName,
                    PilotId = pilot.Id,
                    AmaNumber = pilot.StandardsBodyId,
                    AirframeNumbers = pilotRegistration == null ? new ObservableCollection<string>() : new ObservableCollection<string>(pilotRegistration.AirframeRegistrationNumbers),
                    AirframesSignedOff = pilotRegistration?.AirframesSignedOff ?? false,
                    IsPaid = pilotRegistration?.IsPaid ?? false,
                    Airframes = pilotRegistration == null ? new ObservableCollection<string>() : new ObservableCollection<string>(new List<string> { pilot.Airframe }),
                    ContestId = this.contest.Id
                });
            }
        }

        /// <summary>
        /// Saves the registration data.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private async Task<bool> SaveRegistrationData()
        {
            var pilotRegistrations = new List<PilotRegistration>();
            var changesToContest = false;

            foreach (var registration in this.PilotsToRegister)
            {
                // Don't add a pilot that is marked for removal
                if (registration.IsMarkedForRemoval)
                {
                    this.contest.PilotRoster.Remove(registration.PilotId);
                    continue;
                }

                var savePilotResult = await pilotCmdInteractor.Value.UpdatePilotAsync(this.pilotsInContest.Where(p => p.Id == registration.PilotId).Single());
                if (savePilotResult.IsFaulted)
                {
                    base.Alert($"{nameof(PilotRegistrationPageViewModel)}:{nameof(SaveRegistrationData)} - Could not save the pilot with id {registration.PilotId}.");
                    return false;
                }

                pilotRegistrations.Add(new PilotRegistration
                {
                    AirframeRegistrationNumbers = new List<string>(),
                    AirframesSignedOff = registration.AirframesSignedOff,
                    ContestId = contest.Id,
                    Id = registration.RegistrationId,
                    IsPaid = registration.IsPaid,
                    PilotId = registration.PilotId
                });
            }

            if (changesToContest)
            {
                var contestUpdateResult = await contestCmdIntr.Value.UpdateContestAsync(this.contest);
                if (contestUpdateResult.IsFaulted)
                {
                    base.Alert($"{nameof(PilotRegistrationPageViewModel)}:{nameof(SaveRegistrationData)} - Could not save the contest.");
                }
            }

            var saveRegistrationsResult = await this.registrationCmdIntr.Value.RegisterPilotsForContestAsync(pilotRegistrations);

            return !saveRegistrationsResult.IsFaulted;
        }

        #endregion
    }
}
