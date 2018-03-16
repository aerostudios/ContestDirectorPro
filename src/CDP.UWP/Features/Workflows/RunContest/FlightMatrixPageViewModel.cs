//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: EditPilotRegistrationPageViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.FlightMatrices.Commands;
    using CDP.CoreApp.Features.FlightMatrices.Queries;
    using CDP.CoreApp.Features.Pilots.Queries;
    using CDP.CoreApp.Features.Registration.Queries;
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels;
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
    /// Defines a view model for the flight matrix creation page of the run contest workflow.
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class FlightMatrixPageViewModel : CdproViewModelBase
    {
        #region Instance Members
        
        private ObservableCollection<PilotRoundMatrixListItemViewModel> pilots = new ObservableCollection<PilotRoundMatrixListItemViewModel>();
        private FlightMatrix matrix;
        private Contest contest;

        /// <summary>
        /// The flight matrix query interactor
        /// </summary>
        private Lazy<FlightMatrixQueryInteractor> flightMatrixQueryIntr = new Lazy<FlightMatrixQueryInteractor>(
            () => new FlightMatrixQueryInteractor(
                    new FlightMatrixRepository(StorageFileConfig.FlightMatrixFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The flight matrix command interactor
        /// </summary>
        private Lazy<FlightMatrixStorageCmdInteractor> flightMatrixCmdIntr = new Lazy<FlightMatrixStorageCmdInteractor>(
            () => new FlightMatrixStorageCmdInteractor(
                    new FlightMatrixRepository(StorageFileConfig.FlightMatrixFileName, App.Cache, App.Logger), App.Logger));
        
        /// <summary>
        /// The pilot query interactor
        /// </summary>
        private Lazy<PilotQueryInteractor> pilotQueryIntr = new Lazy<PilotQueryInteractor>(
            () => new PilotQueryInteractor(
                    new PilotFileSystemRepository(StorageFileConfig.PilotsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The registration query interactor
        /// </summary>
        private Lazy<PilotRegistrationQueryInteractor> registrationQueryIntr = new Lazy<PilotRegistrationQueryInteractor>(
            () => new PilotRegistrationQueryInteractor(
                    new RegistrationRepository(StorageFileConfig.RegistrationFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The contest command interactor
        /// </summary>
        private Lazy<ContestStorageCmdInteractor> contestCmdIntr = new Lazy<ContestStorageCmdInteractor>(
            () => new ContestStorageCmdInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pilots.
        /// </summary>
        /// <value>
        /// The pilots.
        /// </value>
        public ObservableCollection<PilotRoundMatrixListItemViewModel> Pilots { get { return pilots; } set { Set(ref pilots, value); } }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixPageViewModel"/> class.
        /// </summary>
        public FlightMatrixPageViewModel()
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
                this.contest = parameter as Contest;
                if (this.contest == null)
                {
                    base.Throw(new ArgumentNullException($"{nameof(FlightMatrixPageViewModel)}:{nameof(OnNavigatedToAsync)} - Contest parameter to this page was null, app is in a bad state."));
                }
                
                // If the contest has started, get the current flight matrix
                if (this.contest.State.HasStarted)
                {
                    var pilotMatrixResult = await this.flightMatrixQueryIntr.Value.GetFlightMatrixForContest(this.contest.Id);
                    if (pilotMatrixResult.IsFaulted)
                    {
                        base.Alert($"{nameof(FlightMatrixPageViewModel)}:{nameof(OnNavigatedToAsync)} - Failed to get the flight matrix for contest: {this.contest.Name}.");
                    }

                    if (pilotMatrixResult.Value != null)
                    {
                        await InflateMatrix(pilotMatrixResult.Value);
                    }
                }
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

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles a click on the Generate matrix button.
        /// </summary>
        /// <returns></returns>
        public async Task GenerateMatrix()
        {
            var contestRegistrationsResult = await this.registrationQueryIntr.Value.GetPilotRegistrationsForContest(this.contest.Id);
            if (contestRegistrationsResult.IsFaulted || contestRegistrationsResult.Value == null)
            {
                return;
            }

            try
            {
                var flightMatrixGenerationIntr = new FlightMatrixGenInteractor(
                    App.SortingAlgos.Where(algo => algo.GetUniqueId() == this.contest.SortingAlgoId).Single(), 
                    App.Logger);

                var sortedMatrixResult = flightMatrixGenerationIntr.CreateSortedFlightMatrix(
                    contestRegistrationsResult.Value,
                    // Don't include the fly-off rounds
                    this.contest.Rounds.Where(r => !r.Value.IsFlyOffRound).Count(),
                    this.contest.SuggestedNumberOfPilotsPerGroup);

                if (sortedMatrixResult.IsFaulted || sortedMatrixResult.Value == null)
                {
                    return;
                }

                this.matrix = sortedMatrixResult.Value;

                // Assign it to the current contest.
                this.matrix.ContestId = this.contest.Id;

                // Save it
                var flightMatrixSaveResult = await this.flightMatrixCmdIntr.Value.SaveFlightMatrixForContestAsync(this.matrix);
                if (flightMatrixSaveResult.IsFaulted)
                {
                    base.Alert($"{nameof(FlightMatrixPageViewModel)}:{nameof(GenerateMatrix)} - Failed to save the flight matrix.");
                    return;
                }

                await InflateMatrix(sortedMatrixResult.Value);
            }
            catch (Exception ex)
            {
                App.Logger.LogException(ex);
            }
        }

        /// <summary>
        /// Saves the matrix and continues.
        /// </summary>
        public void SaveAndContinue()
        {
            this.Dispatcher.DispatchAsync(async () => 
            {
                if (this.matrix == null)
                {
                    base.Alert("The internal flight matrix is null, cannot save.");
                }

                this.contest.State.HasStarted = true;

                if (string.IsNullOrEmpty(this.matrix.Id))
                {
                    var saveFlightMatrixResult = await flightMatrixCmdIntr.Value.SaveFlightMatrixForContestAsync(this.matrix);
                    if (saveFlightMatrixResult.IsFaulted)
                    {
                        base.Alert("An error occured saving the flight matrix.");
                    }
                }
                else
                {
                    var updateFlightMatrixResult = await flightMatrixCmdIntr.Value.UpdateFlightMatrixForContestAsync(this.matrix);
                    if (updateFlightMatrixResult.IsFaulted)
                    {
                        base.Alert("An error occured updating the flight matrix.");
                    }
                }

                var allPilotsResult = await pilotQueryIntr.Value.GetAllPilotsAsync();
                if (allPilotsResult.IsFaulted)
                {
                    base.Alert("An error occured getting a list of contest pilots.");
                }

                // Populate the rounds for the contest (ugh)
                foreach (var flightMatrixRound in this.matrix.Matrix)
                {
                    var pilotSlotsByFlightGroup = flightMatrixRound.PilotSlots.GroupBy(ps => ps.FlightGroup);

                    foreach (var flightGroup in pilotSlotsByFlightGroup)
                    {
                        foreach(var pilot in flightGroup)
                        {
                            if (!this.contest.Rounds[flightMatrixRound.RoundOrdinal].FlightGroups.ContainsKey(flightGroup.Key))
                            {
                                this.contest.Rounds[flightMatrixRound.RoundOrdinal].FlightGroups.Add(flightGroup.Key, new List<Pilot>());
                            }

                            this.contest.Rounds[flightMatrixRound.RoundOrdinal].FlightGroups[flightGroup.Key].Add(allPilotsResult.Value.Where(p => p.Id == pilot.PilotId).First());
                        }
                    }
                }

                // Save the contest
                var saveContestResult = Task.Run(async () => { await contestCmdIntr.Value.UpdateContestAsync(this.contest); });
                if (saveContestResult.IsFaulted)
                {
                    base.Alert("An error occured saving the contest.");
                }

                this.NavigationService.Navigate(typeof(ContestRoundsPage), new ContestOpenParameters(this.contest, true));
            });
            
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Inflates the matrix.
        /// </summary>
        /// <param name="pilotMatrix">The pilot matrix.</param>
        private async Task InflateMatrix(FlightMatrix pilotMatrix)
        {
            var flightMatrixGenerationIntr = new FlightMatrixGenInteractor(App.SortingAlgos.Where(algo => algo.GetUniqueId() == this.contest.SortingAlgoId).Single(), App.Logger);

            // Sort by pilot
            var pilotSortedMatrixResult = await this.flightMatrixQueryIntr.Value.GetPilotSortedFlightMatrix(this.contest.Id);

            if (pilotSortedMatrixResult.IsFaulted)
            {
                base.Alert($"{nameof(FlightMatrixPageViewModel)}:{nameof(InflateMatrix)} - Failed to pilot sort the matrix.");
                return;
            }

            // Get all of the pilots
            var allPilotsResult = await this.pilotQueryIntr.Value.GetAllPilotsAsync();
            if (allPilotsResult.IsFaulted)
            {
                base.Alert($"{nameof(FlightMatrixPageViewModel)}:{nameof(InflateMatrix)} - Failed to get all pilots.");
                return;
            }

            // Clear the Pilots collection on the UI thread...
            await this.Dispatcher.DispatchAsync(new Action(() => this.Pilots.Clear()));
            
            // Populate the View models
            foreach (var pilotSchedule in pilotSortedMatrixResult.Value)
            {
                var fullPilotObj = allPilotsResult.Value.Where(p => p.Id == pilotSchedule.PilotId).FirstOrDefault();
                if (fullPilotObj == null)
                {
                    base.Throw(new Exception($"Could not find pilotId:{pilotSchedule.PilotId} in the system."));
                }

                // Update the pilots collection on the UI thread.
                await this.Dispatcher.DispatchAsync(new Action(() => 
                {
                    // Create a view model for each pilot schedule
                    this.Pilots.Add(new PilotRoundMatrixListItemViewModel
                    {
                        PilotName = $"{fullPilotObj.FirstName} {fullPilotObj.LastName}",
                        FlightGroups = new ObservableCollection<string>(pilotSchedule.FlightGroupDraw.Select(flightGroup => flightGroup.ToString()))
                    });
                }));
            }

            this.matrix = pilotMatrix;
        }

        #endregion
    }
}
