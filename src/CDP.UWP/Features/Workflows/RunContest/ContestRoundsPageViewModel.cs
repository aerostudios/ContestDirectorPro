//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: ContestRoundsPageViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Tasks;
    using CDP.AppDomain.Tasks.F3K;
    using CDP.CoreApp.ApplicationEvents;
    using CDP.CoreApp.Features.ContestEngine;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.FlightMatrices.Commands;
    using CDP.CoreApp.Features.FlightMatrices.Queries;
    using CDP.CoreApp.Features.Pilots.Queries;
    using CDP.CoreApp.Features.Scoring.Commands;
    using CDP.CoreApp.Features.Scoring.Queries;
    using CDP.CoreApp.Features.Tasks.Queries;
    using CDP.CoreApp.Features.Timer;
    using CDP.CoreApp.Interfaces.Contests;
    using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
    using CDP.CoreApp.Interfaces.Scoring;
    using CDP.Repository.WindowsStorage;
    using CDP.ScoringAndSortingImpl.F3K.Scoring;
    using CDP.ScoringAndSortingImpl.F3K.Sorting;
    using CDP.UWP.Components.Speech;
    using CDP.UWP.Config;
    using CDP.UWP.Features.CommunicationHub.ContestMediator;
    using CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels;
    using CDP.UWP.Helpers;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Defines a view model for the main page of the application, the contest rounds page.
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class ContestRoundsPageViewModel : CdproViewModelBase
    {
        #region Instance Members
        // Contest
        private string contestName = string.Empty;

        // Rounds
        private List<ContestRoundViewModel> contestRoundsViewModels = new List<ContestRoundViewModel>();
        private ContestRoundViewModel currentRoundViewModel = new ContestRoundViewModel();
        private string currentRoundNumberText = string.Empty;
        private int currentUIRoundOrdinal = 0;

        // Tasks
        private string currentTaskName = string.Empty;

        // Flight Groups
        private FlightGroup currentUIFlightGroup = FlightGroup.A;
        private ObservableCollection<FlightGroupViewModel> flightGroupsForCurrentRound = new ObservableCollection<FlightGroupViewModel>();
        private bool showFlightGroups = false;

        // Round Timer
        private string currentTime = string.Empty;
        private WindowsVoiceBox voiceBox;
        private Queue<string> voiceQueue = new Queue<string>();
        private string currentflightWindow;
        private bool isStartRoundButtonVisible = true;
        private bool isMoveNextButtonVisible = false;
        private bool isRoundInProgressButtonGroupVisible = false;
        private bool isResumeRoundButtonVisible = false;

        // Hub connection and Scores
        private string hostServerUrl;
        private bool isConnected;
        private ObservableCollection<LiveScoreViewModel> liveScoresForCurrentFlightGroup = new ObservableCollection<LiveScoreViewModel>();
        private ObservableCollection<SubmittedScoreViewModel> submittedScoresForFlightGroup = new ObservableCollection<SubmittedScoreViewModel>();

        // Flight Matrix
        private ObservableCollection<PilotRoundMatrixListItemViewModel> flightMatrixPilots = new ObservableCollection<PilotRoundMatrixListItemViewModel>();
        private bool showFlightMatrix = false;

        // Contest Totals
        private ObservableCollection<PilotContestScoreViewModel> pilotTotalScores = new ObservableCollection<PilotContestScoreViewModel>();
        private bool showContestTotals = false;

        // Scoring 
        private ISortingAlgo sortingAlgo = null;

        // Flyoffs
        private IFlyOffSelectionAlgo flyOffSelectionAlgo = null;

        /// <summary>
        /// The pilot query interactor
        /// </summary>
        private Lazy<PilotQueryInteractor> pilotQueryIntr = new Lazy<PilotQueryInteractor>(
            () => new PilotQueryInteractor(
                    new PilotFileSystemRepository(StorageFileConfig.PilotsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The flight matrix query interactor
        /// </summary>
        private Lazy<FlightMatrixQueryInteractor> flightMatrixQueryIntr = new Lazy<FlightMatrixQueryInteractor>(
            () => new FlightMatrixQueryInteractor(
                    new FlightMatrixRepository(StorageFileConfig.FlightMatrixFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The flight matrix generation interactor
        /// </summary>
        private Lazy<FlightMatrixGenInteractor> flightMatrixGenerationIntr = new Lazy<FlightMatrixGenInteractor>(
            () => new FlightMatrixGenInteractor(new RandomSortNoTeamProtection(), App.Logger));

        /// <summary>
        /// The scoring query interactor
        /// </summary>
        private Lazy<ScoringQueryInteractor> scoringQueryIntr = new Lazy<ScoringQueryInteractor>(
            () => new ScoringQueryInteractor(
                    new ScoringRepository(StorageFileConfig.ScoresFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The scoring Round Score interactor
        /// </summary>
        private Lazy<ScoringRoundScoreInteractor> scoringRoundScoreIntr = new Lazy<ScoringRoundScoreInteractor>(
            () => new ScoringRoundScoreInteractor(new RoundScoringAlgo(), App.Logger));

        /// <summary>
        /// The scoring contest interactor (no drop)
        /// </summary>
        private Lazy<ScoringContestScoreAggInteractor> scoringNoDropContestScoreIntr = new Lazy<ScoringContestScoreAggInteractor>(
            () => new ScoringContestScoreAggInteractor(new ContestScoreAggregatorNoDrop(), App.Logger));

        /// <summary>
        /// The scoring with dropped round contest score intr
        /// </summary>
        private Lazy<ScoringContestScoreAggInteractor> scoringWithDroppedRoundContestScoreIntr = new Lazy<ScoringContestScoreAggInteractor>(
            () => new ScoringContestScoreAggInteractor(new ContestScoreAggregatorWithDrop(), App.Logger));

        /// <summary>
        /// The task query intr
        /// </summary>
        private Lazy<TaskQueryInteractor> taskQueryIntr = new Lazy<TaskQueryInteractor>(
            () => new TaskQueryInteractor(
                new TaskRepository(), App.Logger));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get { return isConnected; } set { Set(ref isConnected, value); } }

        /// <summary>
        /// Gets or sets the host server URL.
        /// </summary>
        /// <value>
        /// The host server URL.
        /// </value>
        public string HostServerUrl { get { return hostServerUrl; } set { Set(ref hostServerUrl, value); } }

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public string CurrentTime { get { return currentTime; } set { Set(ref currentTime, value); } }

        /// <summary>
        /// Gets or sets the name of the contest.
        /// </summary>
        /// <value>
        /// The name of the contest.
        /// </value>
        public string ContestName { get { return contestName; } set { Set(ref contestName, value); } }

        /// <summary>
        /// Gets or sets the current round.
        /// </summary>
        /// <value>
        /// The current round.
        /// </value>
        public ContestRoundViewModel CurrentRound { get { return currentRoundViewModel; } set { Set(ref currentRoundViewModel, value); } }

        /// <summary>
        /// Gets or sets the name of the current task.
        /// </summary>
        /// <value>
        /// The name of the current task.
        /// </value>
        public string CurrentTaskName { get { return currentTaskName; } set { Set(ref currentTaskName, value); } }

        /// <summary>
        /// Gets or sets the current round number.
        /// </summary>
        /// <value>
        /// The current round number.
        /// </value>
        public string CurrentRoundNumberText { get { return currentRoundNumberText; } set { Set(ref currentRoundNumberText, value); } }

        /// <summary>
        /// Gets or sets the current flight group.
        /// </summary>
        /// <value>
        /// The current flight group.
        /// </value>
        public FlightGroup RoundTimerCurrentFlightGroup { get { return currentUIFlightGroup; } set { Set(ref currentUIFlightGroup, value); } }

        /// <summary>
        /// Gets or sets the current flight groups.
        /// </summary>
        /// <value>
        /// The current flight groups.
        /// </value>
        public ObservableCollection<FlightGroupViewModel> FlightGroupsForCurrentRoundViewModels { get { return flightGroupsForCurrentRound; } set { Set(ref flightGroupsForCurrentRound, value); } }

        /// <summary>
        /// Gets or sets the contest rounds.
        /// </summary>
        /// <value>
        /// The contest rounds.
        /// </value>
        public List<ContestRoundViewModel> ContestRoundsViewModels { get { return contestRoundsViewModels; } set { Set(ref contestRoundsViewModels, value); } }

        /// <summary>
        /// Gets or sets the pilot total scores.
        /// </summary>
        /// <value>
        /// The pilot total scores.
        /// </value>
        public ObservableCollection<PilotContestScoreViewModel> PilotTotalScores { get { return pilotTotalScores; } set { Set(ref pilotTotalScores, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is start round button enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is start round button enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsStartRoundButtonVisible { get { return isStartRoundButtonVisible; } set { Set(ref isStartRoundButtonVisible, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is move next button visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is move next button visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsMoveNextButtonVisible { get { return isMoveNextButtonVisible; } set { Set(ref isMoveNextButtonVisible, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is round in progress button group visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is round in progress button group visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsRoundInProgressButtonGroupVisible { get { return isRoundInProgressButtonGroupVisible; } set { Set(ref isRoundInProgressButtonGroupVisible, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is resume round button visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is resume round button visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsResumeRoundButtonVisible { get { return isResumeRoundButtonVisible; } set { Set(ref isResumeRoundButtonVisible, value); } }

        /// <summary>
        /// Gets or sets the current flight window.
        /// </summary>
        /// <value>
        /// The current flight window.
        /// </value>
        public string RoundTimerCurrentFlightWindow { get { return currentflightWindow; } set { Set(ref currentflightWindow, value); } }

        /// <summary>
        /// Gets or sets the live scores for current flight group.
        /// </summary>
        /// <value>
        /// The live scores for current flight group.
        /// </value>
        public ObservableCollection<LiveScoreViewModel> LiveScoresForCurrentFlightGroupViewModels { get { return liveScoresForCurrentFlightGroup; } set { Set(ref liveScoresForCurrentFlightGroup, value); } }

        /// <summary>
        /// Gets or sets the submitted scores for flight group.
        /// </summary>
        /// <value>
        /// The submitted scores for flight group.
        /// </value>
        public ObservableCollection<SubmittedScoreViewModel> SubmittedScoresForFlightGroup { get { return submittedScoresForFlightGroup; } set { Set(ref submittedScoresForFlightGroup, value); } }

        /// <summary>
        /// Gets or sets the flight matrix pilots.
        /// </summary>
        /// <value>
        /// The flight matrix pilots.
        /// </value>
        public ObservableCollection<PilotRoundMatrixListItemViewModel> FlightMatrixPilots { get { return flightMatrixPilots; } set { Set(ref flightMatrixPilots, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether [show flight matrix].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show flight matrix]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowFlightMatrix { get { return showFlightMatrix; } set { Set(ref showFlightMatrix, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether [show flight groups].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show flight groups]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowFlightGroups { get { return showFlightGroups; } set { Set(ref showFlightGroups, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether [show contest totals].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show contest totals]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowContestTotals { get { return showContestTotals; } set { Set(ref showContestTotals, value); } }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestRoundsPageViewModel"/> class.
        /// </summary>
        public ContestRoundsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                PopulateWithTestData();
            }
            else
            {
                // runtime experience
                this.voiceBox = new WindowsVoiceBox(this.Dispatcher);

                RegisterForEvents();
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
                var contestOpenParameters = parameter as ContestOpenParameters;
                if (contestOpenParameters == null || contestOpenParameters.Contest == null)
                {
                    base.Throw(new ArgumentNullException(
                        $"{nameof(ContestRoundsPageViewModel)}:{nameof(OnNavigatedToAsync)} - Contest parameter to this page was null, app is in a bad state."));
                }

                this.sortingAlgo = App.SortingAlgos.Where(algo => algo.GetUniqueId() == contestOpenParameters.Contest.SortingAlgoId).Single();
                this.flyOffSelectionAlgo = App.FlyOffSelectionAlgos.Where(algo => algo.GetUniqueId() == contestOpenParameters.Contest.FlyOffSelectionAlgoId).Single();

                if (contestOpenParameters.OverrideExistingContest)
                {
                    await InitializeContestEngine(contestOpenParameters.Contest);
                }

                if (App.ContestEngine.Contest == null) { base.Throw(new Exception("Contest could not be found, we are in a bad state...")); }

                await this.Dispatcher.DispatchAsync(new Action(async () =>
                {
                    this.ShowFlightMatrix = false;
                    this.showFlightGroups = true;
                    await InitializeState();
                }));
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
            UnRegisterEvents();
            await Task.CompletedTask;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Nexts the round click.
        /// </summary>
        public void NextRound_Click()
        {
            PageRound(RoundPagingDirection.Forward);
        }

        /// <summary>
        /// Previouses the round click.
        /// </summary>
        public void PreviousRound_Click()
        {
            PageRound(RoundPagingDirection.Back);
        }

        /// <summary>
        /// Totalses the click.
        /// </summary>
        public void Totals_Click()
        {
            this.Dispatcher.DispatchAsync(async () =>
            {
                var allScoresResult = await this.scoringQueryIntr.Value.GetScoresForContest(App.ContestEngine.Contest.Id);

                if (allScoresResult.IsFaulted)
                {
                   base.Alert($"{nameof(ContestRoundsPageViewModel)}:{nameof(Totals_Click)} - Failed to get the scores for contest: {App.ContestEngine.Contest?.Name}.");
                }

                this.pilotTotalScores.Clear();

                foreach (var vm in await InflateContestScores(allScoresResult.Value))
                {
                    this.pilotTotalScores.Add(vm);
                }

                ShowContestTotals = true;
                ShowFlightGroups = false;
                ShowFlightMatrix = false;
            });
        }

        /// <summary>
        /// Flights the matrix click.
        /// </summary>
        public void FlightMatrix_Click()
        {
            this.Dispatcher.DispatchAsync(async () =>
            {
                await InflateMatrix();
            });

            this.Dispatcher.Dispatch(() =>
            {
                this.ShowFlightGroups = false;
                this.ShowContestTotals = false;
                this.ShowFlightMatrix = true;
            });
        }

        /// <summary>
        /// Handles the 'back to rounds' button click.
        /// </summary>
        public void RoundsButton_Click()
        {
            this.Dispatcher.Dispatch(() =>
            {
                this.ShowFlightGroups = true;
                this.ShowFlightMatrix = false;
                this.ShowContestTotals = false;
            });
        }

        /// <summary>
        /// Handles the Click event of the StartRoundClock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void StartRoundClock_Click(object sender, RoutedEventArgs e)
        {
            var currentTask = this.contestRoundsViewModels[App.ContestEngine.CurrentRoundOrdinal].Task;
            
            this.Dispatcher.DispatchAsync(async () =>
            {
                // Get the first time window
                App.ContestEngine.SetTimeWindow();

                // Build the announcement and announce
                var pilotsInFlightGroup = App.ContestEngine.Contest
                                            .Rounds[App.ContestEngine.CurrentRoundOrdinal].FlightGroups
                                            .Where(fg => fg.Key == App.ContestEngine.CurrentFlightGroup).First().Value;

                var pilotsInFlightGroupAnnouncement = BuildPilotsInFlightGroupAnnouncement(pilotsInFlightGroup);
                await this.voiceBox.SayItAsync($"{pilotsInFlightGroupAnnouncement}; {currentTask.Name}; {App.ContestEngine.CurrentTimeWindow.Name}");

                // Set up the UI
                this.IsStartRoundButtonVisible = false;
                this.IsRoundInProgressButtonGroupVisible = true;
                this.RoundTimerCurrentFlightWindow = App.ContestEngine.CurrentTimeWindow.Name;
                this.CurrentTime = App.ContestEngine.CurrentTimeWindow.Time.ToString("c");

                await App.ContestCommunicationsHub.PostRoundTimerStarted(App.ContestEngine.CurrentTimeWindow.Time);
                await App.ContestCommunicationsHub.PostNewRoundAvailable(App.ContestEngine.CurrentFlightGroup, App.ContestEngine.CurrentTask, pilotsInFlightGroup);
            });

            // Start the timer
            App.ContestTimer.Start(App.ContestEngine.CurrentTimeWindow.Time, new TimeSpan(), App.ContestEngine.CurrentTimeWindow.DirectionOfCount, sender as System.ComponentModel.ISynchronizeInvoke);
        }

        /// <summary>
        /// Handles the Click event of the PauseRoundClock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void PauseRoundClock_Click(object sender, RoutedEventArgs e)
        {
            App.ContestTimer.Pause();
            this.Dispatcher.Dispatch(async () =>
            {
                this.IsRoundInProgressButtonGroupVisible = false;
                this.IsResumeRoundButtonVisible = true;
                await App.ContestCommunicationsHub.PostRoundTimerStopped(App.ContestTimer.CurrentTime);
            });
        }

        /// <summary>
        /// Handles the Click event of the StopRoundClock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void StopRoundClock_Click(object sender, RoutedEventArgs e)
        {
            App.ContestTimer.Stop();

            this.Dispatcher.Dispatch(async () =>
            {
                this.IsRoundInProgressButtonGroupVisible = false;
                this.IsStartRoundButtonVisible = true;
                this.CurrentTime = "00:00:00";

                App.ContestEngine.ResetFlightWindows();

                await App.ContestCommunicationsHub.PostRoundTimerStopped(App.ContestTimer.CurrentTime);
            });
        }

        /// <summary>
        /// Handles the Click event of the ContinueRound control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ContinueRound_Click(object sender, RoutedEventArgs e)
        {
            App.ContestTimer.Start(App.ContestTimer.CurrentTime, new TimeSpan(), App.ContestTimer.Direction);

            this.Dispatcher.Dispatch(() =>
            {
                this.IsStartRoundButtonVisible = false;
                this.IsMoveNextButtonVisible = false;
                this.IsResumeRoundButtonVisible = false;
                this.IsRoundInProgressButtonGroupVisible = true;
            });
        }

        /// <summary>
        /// Handles the Click event of the MoveToNextFlightGroup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void MoveToNextFlightGroup_Click(object sender, RoutedEventArgs e)
        {
            // Set the next flight group
            this.Dispatcher.Dispatch(() =>
            {
                this.LiveScoresForCurrentFlightGroupViewModels.Clear();
                this.IsRoundInProgressButtonGroupVisible = false;
                this.IsStartRoundButtonVisible = true;
                this.IsMoveNextButtonVisible = false;
            });
        }

        /// <summary>
        /// Announces the pilots in flight group.
        /// </summary>
        /// <returns></returns>
        private string BuildPilotsInFlightGroupAnnouncement(List<Pilot> pilots)
        {
            var pilotsListBuilder = new StringBuilder();
            pilotsListBuilder.Append("Pilots in flight group; ");

            foreach (var pilot in pilots)
            {
                pilotsListBuilder.Append($"{pilot.FirstName} {pilot.LastName};");
            }

            return pilotsListBuilder.ToString();
        }

        /// <summary>
        /// Handles the FlightTimerStopped event of the ContestCommunicationsHub control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CoreApp.ApplicationEvents.FlightTimerEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ContestCommunicationsHub_FlightTimerStopped(object sender, FlightTimerEventArgs e)
        {
            var pilotsInFlightGroup = App.ContestEngine.Contest
                                            .Rounds[App.ContestEngine.CurrentRoundOrdinal].FlightGroups
                                            .Where(fg => fg.Key == App.ContestEngine.CurrentFlightGroup).First().Value;

            var pilot = pilotsInFlightGroup.Where(p => p.Id == e.PilotId).First();

            this.Dispatcher.DispatchAsync(() =>
            {
                this.LiveScoresForCurrentFlightGroupViewModels.Add(new LiveScoreViewModel
                {
                    PilotName = $"{pilot.FirstName} {pilot.LastName}",
                    FlightTime = new TimeSpan(0, e.Minutes, e.Seconds).ToString("c")
                });
            });
        }

        /// <summary>
        /// Handles the FlightTimerStarted event of the ContestCommunicationsHub control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CoreApp.ApplicationEvents.FlightTimerEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ContestCommunicationsHub_FlightTimerStarted(object sender, FlightTimerEventArgs e)
        {
            base.Throw(new NotImplementedException(), "The program has tried to execute code that is not intended to be run.  Call your local programmer and fix the bug :).");
        }

        /// <summary>
        /// Handles the FinalTimeSheetPosted event of the ContestCommunicationsHub control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CoreApp.ApplicationEvents.FinalTimeSheetPostedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ContestCommunicationsHub_FinalTimeSheetPosted(object sender, FinalTimeSheetPostedEventArgs e)
        {
            this.Dispatcher.DispatchAsync(() =>
            {
                // TODO: Clean up the current flight group pilots story.
                var pilot = this.flightGroupsForCurrentRound.Where(fg => fg.FlightGroup == App.ContestEngine.CurrentFlightGroup).FirstOrDefault().Pilots.Where(p => p.PilotId == e.PilotId).Single();
                this.SubmittedScoresForFlightGroup.Add(new SubmittedScoreViewModel(new Pilot(pilot.Name, string.Empty, pilot.PilotId), e.FinalTimeSheet));

                InsertScoreIntoFlightGroup(e);
            });
        }

        /// <summary>
        /// Handles the ClockEnd event of the ContestTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ContestTimer_ClockEnd(object sender, EventArgs e)
        {
            this.Dispatcher.DispatchAsync(() => this.CurrentTime = "00:00:00");
            App.ContestEngine.SetTimeWindow();

            if (App.ContestEngine.CurrentTimeWindow != null)
            {
                this.Dispatcher.DispatchAsync(async () =>
                {
                    // Update the UI
                    this.RoundTimerCurrentFlightWindow = App.ContestEngine.CurrentTimeWindow.Name;
                    this.CurrentTime = App.ContestEngine.CurrentTimeWindow.Time.ToString("c");
                    this.IsStartRoundButtonVisible = false;

                    // Announce the window if it isn't the task window
                    if (!App.ContestEngine.CurrentTimeWindow.Name.Contains("task", StringComparison.OrdinalIgnoreCase))
                    {
                        await this.voiceBox.SayItAsync(App.ContestEngine.CurrentTimeWindow.Name);
                    }
                });

                // Let everyone else know
                this.Dispatcher.DispatchAsync(() => App.ContestCommunicationsHub.PostRoundTimerStarted(App.ContestEngine.CurrentTimeWindow.Time));

                // Start the timer
                App.ContestTimer.Start(App.ContestEngine.CurrentTimeWindow.Time, new TimeSpan(), App.ContestEngine.CurrentTimeWindow.DirectionOfCount, sender as System.ComponentModel.ISynchronizeInvoke);
            }
            else
            {
                this.Dispatcher.DispatchAsync(async () =>
                {
                    this.IsRoundInProgressButtonGroupVisible = false;
                    this.IsMoveNextButtonVisible = true;
                    this.RoundTimerCurrentFlightWindow = string.Empty;
                    await App.ContestCommunicationsHub.PostRoundTimerStopped(new TimeSpan());
                });
            }
        }

        /// <summary>
        /// Handles the Tick event of the ContestTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ContestTimer_Tick(object sender, TickEventArgs e)
        {
            this.Dispatcher.Dispatch(() => { this.CurrentTime = e.CurrentTime.ToString("c"); });

            // Announce the last 10 seconds
            if (e.CurrentTime.TotalSeconds <= 10
                && e.CurrentTime.Seconds != 0)
            {
                // We can't queue these, doesn't execute in time.
                var voiceTask = Task.Run(() => this.voiceBox.SayItAsync(e.CurrentTime.Seconds.ToString()));
                voiceTask.Wait();
                return;
            }

            // Check to see if we have hit any of the checkpoints set
            //foreach (var time in this.voiceConfig.CheckpointsToAnnounce)
            //{
            //    if (time.TotalSeconds == e.CurrentTime.TotalSeconds)
            //    {
            //        // We will not await on this as we don't want it to block
            //        // the event loop. 
            //        //await this.announcer.SayIt(CreateRemainingTimeAnnouncement(_currentTimeValue));
            //        break;
            //    }
            //}
        }

        /// <summary>
        /// Handles the SpeechEnded event of the VoiceBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VoiceBox_SpeechEnded(object sender, EventArgs e)
        {
            if (this.voiceQueue.Count > 0)
            {
                this.Dispatcher.DispatchAsync(() => this.voiceBox.SayItAsync(this.voiceQueue.Dequeue()));
            }
        }

        /// <summary>
        /// Handles the connection button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.DispatchAsync(async () =>
            {
                await App.ContestCommunicationsHub.ConnectToSignalRHost(new Uri(this.HostServerUrl));
                this.IsConnected = true;
            });
        }

        /// <summary>
        /// Handles the SignalRConnectionStateChanged event of the ContestCommunicationsHub control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CommunicationHub.ContestMediator.Events.SignalRConnectionStateChangedEventArgs"/> instance containing the event data.</param>
        private void ContestCommunicationsHub_SignalRConnectionStateChanged(object sender, CommunicationHub.ContestMediator.Events.SignalRConnectionStateChangedEventArgs e)
        {
            this.IsConnected = e.State == SignalRConnectionState.Connected;

            if(this.IsConnected == false)
            {
                base.Alert("The app has disconnected from the host server.  Please re-connect.");
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the contest engine.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        private async Task InitializeContestEngine(Contest contest)
        {
            if (App.ContestEngine == null)
            {
                IContestScoreAggregator aggrigator;

                if (contest.AllowDroppedRound == true)
                {
                    aggrigator = new ContestScoreAggregatorWithDrop();
                }
                else
                {
                    aggrigator = new ContestScoreAggregatorNoDrop();
                }

                switch (contest.Type)
                {
                    case ContestType.F3K:
                        // Yes, I realize this constructor is a bit out of control...
                        App.ContestEngine = new F3KContestEngine(
                            new TaskQueryInteractor(new TaskRepository(), App.Logger),
                            new ContestStorageCmdInteractor(new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger),
                            new ScoringStorageCmdInteractor(new ScoringRepository(StorageFileConfig.ScoresFileName, App.Cache, App.Logger), App.Logger),
                            new ScoringQueryInteractor(new ScoringRepository(StorageFileConfig.ScoresFileName, App.Cache, App.Logger), App.Logger),
                            new ScoringContestScoreAggInteractor(aggrigator, App.Logger),
                            new FlightMatrixStorageCmdInteractor(new FlightMatrixRepository(StorageFileConfig.FlightMatrixFileName, App.Cache, App.Logger), App.Logger),
                            new FlightMatrixQueryInteractor(new FlightMatrixRepository(StorageFileConfig.FlightMatrixFileName, App.Cache, App.Logger), App.Logger),
                            new PilotQueryInteractor(new PilotFileSystemRepository(StorageFileConfig.PilotsFileName, App.Cache, App.Logger), App.Logger),
                            this.sortingAlgo,
                            this.flyOffSelectionAlgo,
                            App.Logger);
                        break;
                }
            }

            await App.ContestEngine.Initialize(contest);
        }

        /// <summary>
        /// Initializes the state.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeState()
        {
            // Verify that the contest engine has been initialized
            if (App.ContestEngine.Contest == null)
            {
                base.Throw(new Exception("Contest Engine is in an invalid state."));
            }

            try
            {
                // Build up the rounds and flight group view models
                var flightMatrixResult = await this.flightMatrixQueryIntr.Value.GetFlightMatrixForContest(App.ContestEngine.Contest.Id);
                if (flightMatrixResult.IsFaulted)
                {
                    base.Alert($"{nameof(ContestRoundsPageViewModel)}:{nameof(InitializeState)} - Failed to get the flight matric for contest: {App.ContestEngine.Contest?.Name}.");
                    return;
                }

                var allPilotsResult = await pilotQueryIntr.Value.GetAllPilotsAsync();
                if (allPilotsResult.IsFaulted)
                {
                    base.Alert($"{nameof(ContestRoundsPageViewModel)}:{nameof(InitializeState)} - Failed to get all pilots.");
                    return;
                }

                // Initialize the Rounds, Flight Groups and Scores
                var newViewModels = await CreateContestRoundViewModelsFromMatrix(flightMatrixResult.Value, App.ContestEngine.Contest, allPilotsResult.Value);
                ResetRoundViewModels(newViewModels);

                this.Dispatcher.Dispatch(() => 
                { 
                    this.ContestName = App.ContestEngine.Contest.Name;
                    this.currentUIRoundOrdinal = App.ContestEngine.Contest.State.CurrentRoundOrdinal;
                    this.RoundTimerCurrentFlightGroup = App.ContestEngine.Contest.State.CurrentFlightGroup;
                    this.CurrentTime = App.ContestTimer.CurrentTime.ToString("c");

                    // Set up the UI
                    SetCurrentUIRound(App.ContestEngine.CurrentRoundOrdinal);
                });
            }
            catch (Exception)
            {
                // Just eat it for now...
            }
        }

        /// <summary>
        /// Registers for events.
        /// </summary>
        private void RegisterForEvents()
        {
            this.voiceBox.SpeechEnded += VoiceBox_SpeechEnded;

            App.ContestTimer.Tick += ContestTimer_Tick;
            App.ContestTimer.ClockEnd += ContestTimer_ClockEnd;
            App.ContestCommunicationsHub.FinalTimeSheetPosted += ContestCommunicationsHub_FinalTimeSheetPosted;
            App.ContestCommunicationsHub.FlightTimerStarted += ContestCommunicationsHub_FlightTimerStarted;
            App.ContestCommunicationsHub.FlightTimerStopped += ContestCommunicationsHub_FlightTimerStopped;
            App.ContestCommunicationsHub.SignalRConnectionStateChanged += ContestCommunicationsHub_SignalRConnectionStateChanged;
        }

        /// <summary>
        /// Uns the register events.
        /// </summary>
        private void UnRegisterEvents()
        {
            App.ContestTimer.Tick -= ContestTimer_Tick;
            App.ContestTimer.ClockEnd -= ContestTimer_ClockEnd;
            App.ContestCommunicationsHub.FinalTimeSheetPosted -= ContestCommunicationsHub_FinalTimeSheetPosted;
            App.ContestCommunicationsHub.FlightTimerStarted -= ContestCommunicationsHub_FlightTimerStarted;
            App.ContestCommunicationsHub.FlightTimerStopped -= ContestCommunicationsHub_FlightTimerStopped;
        }

        /// <summary>
        /// Builds the contest rounds from matrix.
        /// </summary>
        /// <param name="flightMatrix">The flight matrix.</param>
        /// <param name="contest">The contest.</param>
        /// <param name="allPilots">All pilots.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Bad flight matrix.</exception>
        private async Task<List<ContestRoundViewModel>> CreateContestRoundViewModelsFromMatrix(FlightMatrix flightMatrix, 
            Contest contest, 
            IEnumerable<Pilot> allPilots)
        {
            var contestRounds = new List<ContestRoundViewModel>();
            var roundCntr = 0;

            // Get any scores that have been entered for the contest
            var contestScores = await this.scoringQueryIntr.Value.GetScoresForContest(contest.Id);

            // Create the rounds
            foreach (var round in contest.Rounds)
            {
                var taskResult = await this.taskQueryIntr.Value.GetTask(round.Value.AssignedTaskId);
                if (taskResult.IsFaulted)
                {
                    base.Alert($"{nameof(ContestRoundsPageViewModel)}:{nameof(CreateContestRoundViewModelsFromMatrix)} - Could not find task {round.Value.AssignedTaskId}.");
                }

                var roundTask = taskResult.Value;

                // Create the flight groups
                var flightMatrixRound = flightMatrix.Matrix.Where(fmRounds => fmRounds.RoundOrdinal == roundCntr).FirstOrDefault();
                if (flightMatrixRound == null)
                {
                    // If this is a fly-off round, just break; we aren't there yet.
                    if (round.Value.IsFlyOffRound) break;

                    // Otherwise we are in a bad state.
                    base.Throw(new Exception("Bad flight matrix."));
                }

                var fmFlightGroups = flightMatrixRound.PilotSlots.GroupBy(ps => ps.FlightGroup);
                var contestFlightGroupsForRound = new ObservableCollection<FlightGroupViewModel>();

                foreach (var flightGroup in fmFlightGroups)
                {
                    var fg = new FlightGroupViewModel
                    {
                        FlightGroupName = flightGroup.Key.ToString(),
                        FlightGroup = flightGroup.Key,
                        RoundTask = roundTask,
                        DisplayRoundNumber = (roundCntr + 1).ToString(),
                        HasBeenScored = contestScores.Value.Any(roundScoreCollection => roundScoreCollection.Any(ts => ts.RoundOrdinal == round.Key && ts.FlightGroup == flightGroup.Key))
                    };

                    // Set up the pilot view models
                    var pilotsCollection = new ObservableCollection<PilotScoreViewModel>();
                    await CreatePilotViewsForFlightGroup(contest.Id, flightMatrixRound, allPilots, roundTask, flightGroup, pilotsCollection);

                    fg.Pilots = new ObservableCollection<PilotScoreViewModel>(pilotsCollection);
                    contestFlightGroupsForRound.Add(fg);
                }

                contestRounds.Add(new ContestRoundViewModel
                {
                    FlightGroups = contestFlightGroupsForRound,
                    DisplayNumber = (roundCntr + 1).ToString(),
                    Ordinal = roundCntr,
                    Task = roundTask
                });

                // Don't create flight groups for rounds that have not been calculated yet.
                // Seeded MoM for instance doesn't have a flight matrix past the current round.
                if (this.sortingAlgo.IsSingleRoundSort() && round.Key >= App.ContestEngine.Contest.State.CurrentRoundOrdinal)
                {
                    break;
                }

                roundCntr++;
            }

            return contestRounds;
        }

        /// <summary>
        /// Resets the round view models.
        /// </summary>
        /// <param name="newViewModels">The new view models.</param>
        private void ResetRoundViewModels(IEnumerable<ContestRoundViewModel> newViewModels)
        {
            this.Dispatcher.Dispatch(() =>
            {
                this.ContestRoundsViewModels.Clear();
                foreach (var viewModel in newViewModels)
                {
                    this.ContestRoundsViewModels.Add(viewModel);
                }
            });
        }

        /// <summary>
        /// Creates the pilot views for flight group.
        /// </summary>
        /// <param name="contestId">The contest identifier.</param>
        /// <param name="flightMatrixRound">The flight matrix round.</param>
        /// <param name="allPilots">All pilots.</param>
        /// <param name="roundTask">The round task.</param>
        /// <param name="flightGroup">The flight group.</param>
        /// <param name="pilotsCollection">The pilots collection.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task CreatePilotViewsForFlightGroup(string contestId,
            FlightMatrixRound flightMatrixRound,
            IEnumerable<Pilot> allPilots,
            TaskBase roundTask,
            IGrouping<FlightGroup, FlightMatrixPilotSlot> flightGroup,
            ObservableCollection<PilotScoreViewModel> pilotsCollection)
        {

            // Get the scores for the contest
            var contestScoresResult = await this.scoringQueryIntr.Value.GetScoresForContest(contestId);
            if (contestScoresResult.IsFaulted)
            {
                //TODO - Something...
                return;
            }

            var contestScores = contestScoresResult.Value;

            foreach (var pilotSlot in flightGroup)
            {
                // Get the pilot
                var pilot = allPilots.Where(p => p.Id == pilotSlot.PilotId).FirstOrDefault();
                if (pilot == null) { base.Throw(new Exception($"Could not find pilotId {pilotSlot.PilotId}")); }

                // Get their score
                TimeSheet pilotScore = null;
                if (contestScores.Any(rs => rs.RoundOrdinal == flightMatrixRound.RoundOrdinal))
                {
                    pilotScore = contestScores[flightMatrixRound.RoundOrdinal]?.Where(p => p.PilotId == pilot.Id).FirstOrDefault();
                }

                // Create the gates for this round
                var gates = new List<TimeGateViewModel>();
                for (int i = 0; i < roundTask.NumberOfTimeGatesAllowed; ++i)
                {
                    var parsedTime = pilotScore == null ? "0:00" : ParseTime(pilotScore.TimeGates[i].Time);

                    gates.Add(new TimeGateViewModel
                    {
                        GateType = TimeGateType.Task,
                        Ordinal = i,
                        Time = parsedTime,
                    });
                }

                pilotsCollection.Add(new PilotScoreViewModel
                {
                    PilotId = pilotSlot.PilotId,
                    Score = pilotScore?.Score ?? 0,
                    Penalty = pilotScore?.TotalPenalties ?? 0,
                    Name = $"{pilot.FirstName} {pilot.LastName}",
                    Gates = new ObservableCollection<TimeGateViewModel>(gates)
                });
            }
        }

        /// <summary>
        /// Inflates the matrix.
        /// </summary>
        /// <param name="pilotMatrix">The pilot matrix.</param>
        private async Task InflateMatrix()
        {
            // Sort by pilot
            var pilotSortedMatrixResult = await this.flightMatrixQueryIntr.Value.GetPilotSortedFlightMatrix(App.ContestEngine.Contest.Id);

            if (pilotSortedMatrixResult.IsFaulted)
            {
                base.Alert($"{nameof(ContestRoundsPageViewModel)}:{nameof(InflateMatrix)} - Failed to pilot sort the matrix.");
                return;
            }
            
            // Get all of the pilots
            var allPilotsResult = await this.pilotQueryIntr.Value.GetAllPilotsAsync();
            if (allPilotsResult.IsFaulted)
            {
                base.Alert($"{nameof(ContestRoundsPageViewModel)}:{nameof(InflateMatrix)} - Failed to get all pilots.");
                return;
            }

            // Clear the flight matrix pilots collection on the UI thread...
            await this.Dispatcher.DispatchAsync(new Action(() => this.FlightMatrixPilots.Clear()));

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
                    this.FlightMatrixPilots.Add(new PilotRoundMatrixListItemViewModel
                    {
                        PilotName = $"{fullPilotObj.FirstName} {fullPilotObj.LastName}",
                        FlightGroups = new ObservableCollection<string>(pilotSchedule.FlightGroupDraw.Select(flightGroup => flightGroup.ToString()))
                    });
                }));
            }
        }

        /// <summary>
        /// Inflates the contest scores.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private async Task<List<PilotContestScoreViewModel>> InflateContestScores(ContestScoresCollection value)
        {
            var allPilots = await this.pilotQueryIntr.Value.GetAllPilotsAsync();
            var pilotScoreLookup = new Dictionary<string, PilotContestScoreViewModel>();

            foreach (var roundNumber in value.Rounds)
            {
                foreach (var timesheet in value[roundNumber])
                {
                    if (pilotScoreLookup.ContainsKey(timesheet.PilotId))
                    {
                        pilotScoreLookup[timesheet.PilotId].Scores.Add(timesheet.RoundOrdinal,
                            new FinalRoundScoreViewModel
                            {
                                Score = timesheet.Score,
                                IncludesPenalty = false,
                                IsDropped = false, 
                                RoundOrdinal = timesheet.RoundOrdinal
                            });
                    }
                    else
                    {
                        var pilot = allPilots.Value.Where(p => p.Id == timesheet.PilotId).SingleOrDefault();
                        pilotScoreLookup.Add(timesheet.PilotId, new PilotContestScoreViewModel
                        {
                            PilotName = $"{pilot.FirstName} {pilot.LastName}",
                            Scores = new Dictionary<int, FinalRoundScoreViewModel>
                            {
                                { timesheet.RoundOrdinal,
                                    new FinalRoundScoreViewModel
                                    {
                                        Score = timesheet.Score,
                                        IncludesPenalty = false,
                                        IsDropped = false,
                                        RoundOrdinal = timesheet.RoundOrdinal
                                    }
                                }
                            }
                        });
                    }
                }
            }

            // Mark the dropped rounds if set
            if (App.ContestEngine.Contest.AllowDroppedRound)
            {
                MarkDroppedRounds(pilotScoreLookup);
            }

            return pilotScoreLookup.Select(kvp => kvp.Value).OrderByDescending(pvm => pvm.TotalScore).ToList();
        }

        /// <summary>
        /// Removes the dropped rounds.
        /// </summary>
        /// <param name="pilotScoreLookup">The pilot score lookup.</param>
        private void MarkDroppedRounds(Dictionary<string, PilotContestScoreViewModel> pilotScoreLookup)
        {
            // Go through the scores for each pilot and mark the dropped round
            foreach(var pilotScores in pilotScoreLookup)
            {
                // Only drop after 4 rounds
                if(pilotScores.Value.Scores.Count < 4)
                {
                    continue;
                }

                var scores = pilotScores.Value.Scores;
                if(scores == null)
                {
                    continue;
                }

                var scoreToDrop = scores.Min(s => s.Value.Score);
                var viewModelWithLowestScore = scores.Where(s => s.Value.Score == scoreToDrop).First();

                pilotScores.Value.Scores.Where(s => s.Value.RoundOrdinal == viewModelWithLowestScore.Value.RoundOrdinal).Single().Value.IsDropped = true;
            }
        }

        /// <summary>
        /// Sets the current round.
        /// </summary>
        /// <param name="direction">The direction.</param>
        private void PageRound(RoundPagingDirection direction)
        {
            if (direction == RoundPagingDirection.Forward &&
                this.ContestRoundsViewModels.Count > this.currentUIRoundOrdinal + 1)
            {
                this.currentUIRoundOrdinal++;
                this.FlightGroupsForCurrentRoundViewModels = this.contestRoundsViewModels[currentUIRoundOrdinal].FlightGroups;

            }
            else if (direction == RoundPagingDirection.Back &&
                this.currentUIRoundOrdinal != 0)
            {
                this.currentUIRoundOrdinal--;
            }
            else
            {
                return;
            }

            SetCurrentUIRound(this.currentUIRoundOrdinal);
        }

        /// <summary>
        /// Sets the current round.
        /// </summary>
        /// <param name="roundNumber">The round number.</param>
        private void SetCurrentUIRound(int roundNumber)
        {
            this.currentUIRoundOrdinal = roundNumber;
            this.FlightGroupsForCurrentRoundViewModels = this.contestRoundsViewModels[currentUIRoundOrdinal].FlightGroups;
            this.CurrentTaskName = this.contestRoundsViewModels[currentUIRoundOrdinal].Task.Name;
            this.CurrentRoundNumberText = this.contestRoundsViewModels[currentUIRoundOrdinal].DisplayNumber;
        }

        /// <summary>
        /// Inserts the score into flight group.
        /// </summary>
        /// <param name="e">The <see cref="FinalTimeSheetPostedEventArgs"/> instance containing the event data.</param>
        private void InsertScoreIntoFlightGroup(FinalTimeSheetPostedEventArgs e)
        {
            // Get the appropriate view model
            var pilotViewModel = this.flightGroupsForCurrentRound
                .Where(fg => fg.FlightGroup == App.ContestEngine.CurrentFlightGroup)
                .SingleOrDefault()
                ?.Pilots
                .Where(p => p.PilotId == e.PilotId)
                .SingleOrDefault();
            
            if (pilotViewModel == default(PilotScoreViewModel))
            {
                base.Alert($"{nameof(ContestRoundsPageViewModel)}:{nameof(InsertScoreIntoFlightGroup)} - Could not find the current flight group view model...");
            }

            // Make sure the scores passed are legit
            var isValidTimeSheet = App.ContestEngine.CurrentTask.ValidateTask(e.FinalTimeSheet);

            // Invalidate the model in the UI.
            if (!isValidTimeSheet)
            {
                this.Dispatcher.Dispatch(() => pilotViewModel.IsValid = false);

                // Bad stuff, bail and let the CD handle it.
                return;
            }

            // Push the data from the timer to the fligth group scoring UI.
            this.Dispatcher.DispatchAsync(() =>
            {
                for(var i = 0; i < e.FinalTimeSheet.TimeGates.Count; ++i)
                {
                    if (pilotViewModel.Gates.Count > i)
                    {
                        // Parse the time into a string for the view model - needs to be in the format "0:00".  TimeSpan("c") is "00:00:00"
                        // Yes, this is hacky; but I'm working with a mask control from the UWP toolkit :/ 
                        var splitTimes = e.FinalTimeSheet.TimeGates[i].Time.ToString("c").Split(":");
                        var minutes = splitTimes[1];
                        minutes = minutes[1].ToString();
                        pilotViewModel.Gates[i].Time = $"{minutes}:{splitTimes[2]}";
                    }
                }
            });
        }

        /// <summary>
        /// Scores the flight group.
        /// </summary>
        /// <param name="name">The name.</param>
        internal void ScoreFlightGroup()
        {
            this.Dispatcher.DispatchAsync(async () =>
            {
                try
                {
                    // Find the flight group to score
                    var flightGroupViewModelToScore = FlightGroupsForCurrentRoundViewModels.Where(fgvm => fgvm.FlightGroup == currentUIFlightGroup).FirstOrDefault();

                    // Build the time sheets
                    var timeSheets = flightGroupViewModelToScore.Pilots.Select(p => p.ToTimeSheet(
                        App.ContestEngine.Contest.Id,
                        App.ContestEngine.CurrentFlightGroup,
                        App.ContestEngine.CurrentRoundOrdinal,
                        flightGroupViewModelToScore.RoundTask.Id)).ToList();

                    // Score them using the Algo
                    var scoreFlightGroupResult = this.scoringRoundScoreIntr.Value.ScoreRound(timeSheets as IEnumerable<TimeSheet>, flightGroupViewModelToScore.RoundTask);
                    if (scoreFlightGroupResult.IsFaulted)
                    {
                        // Bail out.
                        base.Alert(scoreFlightGroupResult.Error.ErrorMessage);
                        return;
                    }

                    // Save the scores and move to next stage
                    var tempCurrentRound = App.ContestEngine.CurrentRoundOrdinal;
                    await App.ContestEngine.ScoreFlightGroup(timeSheets as IEnumerable<TimeSheet>);
                    var roundNumberBeforeMove = App.ContestEngine.CurrentRoundOrdinal;
                    await App.ContestEngine.MoveToNextContestStage();

                    // If this is a single round scroing contest (Seeded MoM)
                    // build out the new viewmodels for the flight groups
                    if ((this.sortingAlgo.IsSingleRoundSort() && roundNumberBeforeMove < App.ContestEngine.CurrentRoundOrdinal) ||
                        App.ContestEngine.Contest.Rounds[App.ContestEngine.CurrentRoundOrdinal].IsFlyOffRound)
                    {
                        var newViewModels = await CreateContestRoundViewModelsFromMatrix(App.ContestEngine.FlightMatrix, App.ContestEngine.Contest, App.ContestEngine.PilotsInContest);
                        ResetRoundViewModels(newViewModels);
                    }

                    // Update the UI state
                    SetCurrentUIRound(App.ContestEngine.CurrentRoundOrdinal);

                    this.RoundTimerCurrentFlightGroup = App.ContestEngine.CurrentFlightGroup;
                    this.CurrentRound = this.contestRoundsViewModels.Where(crvm => crvm.Ordinal == App.ContestEngine.CurrentRoundOrdinal).Single();
                    this.FlightGroupsForCurrentRoundViewModels = this.CurrentRound.FlightGroups;

                    foreach (var timeSheet in timeSheets)
                    {
                        var pilotScoreSheet = flightGroupViewModelToScore.Pilots.Where(p => p.PilotId == timeSheet.PilotId).FirstOrDefault();
                        pilotScoreSheet.Score = timeSheet.Score;
                    }

                    flightGroupViewModelToScore.HasBeenScored = true;
                    flightGroupViewModelToScore.IsEditScoresEnabled = true;
                }
                catch(Exception ex)
                {
                    base.Throw(ex);
                }
            });
        }

        /// <summary>
        /// Parses the time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        private string ParseTime(TimeSpan time)
        {
            return $"{String.Format("{0:0}", time.Minutes)}:{String.Format("{0:00}", time.Seconds)}";
        }

        /// <summary>
        /// Populates the with test data.
        /// </summary>
        private void PopulateWithTestData()
        {
            // Set up the tasks
            var taskCollection = new List<TaskBase>
                {
                    new TaskF_BestThreeOutOfSix(),
                    new TaskA_LastFlightSevenMin(),
                    new TaskE_Poker(),
                    new TaskB_LastTwoFlights4MinMax(),
                    new TaskG_FiveTwos(),
                    new TaskJ_LastThree(),
                    new TaskC_AllUpLastDown()
                };

            // Create the rounds
            for (var i = 0; i < 7; ++i)
            {
                var pilotsCollection = new ObservableCollection<PilotScoreViewModel>();

                var gates = new List<TimeGateViewModel>();

                // Create the gates for this round
                for (int j = 0; j < taskCollection[i].NumberOfTimeGatesAllowed; ++j)
                {
                    gates.Add(new TimeGateViewModel
                    {
                        GateType = TimeGateType.Task,
                        Ordinal = j,
                        Time = "0:00"
                    });
                }

                // Create the pilot view models
                for (int k = 0; k < 10; ++k)
                {
                    pilotsCollection.Add(new PilotScoreViewModel
                    {
                        Name = $"Pilot{k}",
                        Score = 0,
                        Gates = new ObservableCollection<TimeGateViewModel>(gates)
                    });
                }

                var flightGroups = new List<FlightGroupViewModel>();

                // Create the Flight groups
                for (int l = 0; l < 4; ++l)
                {
                    flightGroups.Add(new FlightGroupViewModel
                    {
                        DisplayRoundNumber = (i + 1).ToString(),
                        FlightGroupName = ((FlightGroup)(l + 1)).ToString(),
                        FlightGroup = (FlightGroup)1,
                        Pilots = pilotsCollection,
                        RoundTask = taskCollection[i]
                    });
                }

                ContestRoundsViewModels.Add(new ContestRoundViewModel
                {
                    DisplayNumber = (i + 1).ToString(),
                    Ordinal = i,
                    Task = taskCollection[i],
                    FlightGroups = new ObservableCollection<FlightGroupViewModel>(flightGroups)
                });
            }

            SetCurrentUIRound(0);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ContestRoundsPageViewModel"/> class.
        /// </summary>
        ~ContestRoundsPageViewModel()
        {
            UnRegisterEvents();
        }

        #endregion
    }

    /// <summary>
    /// Directional enum for paging
    /// </summary>
    internal enum RoundPagingDirection
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Move Forward
        /// </summary>
        Forward = 1,

        /// <summary>
        /// Move Back
        /// </summary>
        Back = 2
    }
}