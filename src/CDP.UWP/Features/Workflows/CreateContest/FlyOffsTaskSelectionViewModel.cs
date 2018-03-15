﻿//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: FlyOffsTaskSelectionViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.CreateContest
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Tasks;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.Tasks.Queries;
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Features.Workflows.CreateContest.AdditionalViewModels;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// View model for the fly-off round task selection page
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class FlyOffsTaskSelectionViewModel : CdproViewModelBase
    {
        #region Instance Members

        private Contest contest = null;
        private Dictionary<int, int> selectedContestTasks = new Dictionary<int, int>();
        private ObservableCollection<ContestRoundListItemViewModel> rounds = new ObservableCollection<ContestRoundListItemViewModel>();
        private TaskBase selectedTaskItem;
        private ContestRoundListItemViewModel selectedRoundItem;
        private bool removeButtonEnabled = true;
        private List<TaskBase> allTasks = null;
        private bool isAddRoundButtonEnabled = true;

        /// <summary>
        /// The contest storage interactor
        /// </summary>
        private Lazy<ContestStorageCmdInteractor> contestStorageInteractor = new Lazy<ContestStorageCmdInteractor>(
            () => new ContestStorageCmdInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The task query interactor
        /// </summary>
        private Lazy<TaskQueryInteractor> taskQueryIntr = new Lazy<TaskQueryInteractor>(
            () => new TaskQueryInteractor(
                new TaskRepository(), App.Logger));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected contest tasks.
        /// </summary>
        /// <value>
        /// The selected contest tasks.
        /// </value>
        public Dictionary<int, int> SelectedContestTasks { get { return selectedContestTasks; } set { Set(ref selectedContestTasks, value); } }

        /// <summary>
        /// Gets or sets the selected task item.
        /// </summary>
        /// <value>
        /// The selected task item.
        /// </value>
        public TaskBase SelectedTaskItem { get { return selectedTaskItem; } set { Set(ref selectedTaskItem, value); } }

        /// <summary>
        /// Gets or sets the selected round item.
        /// </summary>
        /// <value>
        /// The selected round item.
        /// </value>
        public ContestRoundListItemViewModel SelectedRoundItem { get { return selectedRoundItem; } set { Set(ref selectedRoundItem, value); } }

        /// <summary>
        /// Gets or sets the available tasks.
        /// </summary>
        /// <value>
        /// The available tasks.
        /// </value>
        public ObservableCollection<TaskBase> AvailableTasks { get; set; } = new ObservableCollection<TaskBase>();

        /// <summary>
        /// Gets or sets the rounds.
        /// </summary>
        /// <value>
        /// The rounds.
        /// </value>
        public ObservableCollection<ContestRoundListItemViewModel> Rounds { get { return rounds; } set { Set(ref rounds, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is add round button enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is add round button enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsAddRoundButtonEnabled { get { return isAddRoundButtonEnabled; } set { Set(ref isAddRoundButtonEnabled, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether [remove button enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [remove button enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveButtonEnabled
        {
            get { return removeButtonEnabled; }
            set
            {
                Set(ref removeButtonEnabled, value);
            }
        }

        #endregion

        #region Ctors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FlyOffsTaskSelectionViewModel"/> class.
        /// </summary>
        public FlyOffsTaskSelectionViewModel()
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
            try
            {
                if (state.Any())
                {
                    if (state[nameof(FlyOffsTaskSelectionViewModel)] is FlyOffsTaskSelectionViewModel previousState)
                    {
                        this.AvailableTasks = previousState.AvailableTasks;
                        this.RemoveButtonEnabled = previousState.RemoveButtonEnabled;
                        this.Rounds = previousState.Rounds;
                        this.SelectedContestTasks = previousState.SelectedContestTasks;
                        this.contest = previousState.contest;
                        state.Clear();
                        return;
                    }
                }
                else
                {
                    // use parameter
                    this.contest = parameter as Contest;
                }

                if (this.contest == null)
                {
                    var ex = new Exception($"{nameof(ContestTasksPageViewModel)}:{nameof(OnNavigatedToAsync)} - No contest available for this page.");
                    base.Throw(ex, "Could not load the contest.");
                }

                var allTasksResult = await this.taskQueryIntr.Value.GetAllTasksAsync(ContestType.F3K);
                if (allTasksResult.IsFaulted)
                {
                    base.Alert($"{nameof(ContestTasksPageViewModel)}:{nameof(OnNavigatedToAsync)} - Could not load the tasks.");
                }

                this.allTasks = allTasksResult.Value.ToList();

                // Init the UI
                await this.Dispatcher.DispatchAsync(new Action(() =>
                {
                    foreach (var t in allTasks) { this.AvailableTasks.Add(t); }
                    this.SelectedTaskItem = this.AvailableTasks.First();
                }));

                // Build up the view for each of the fly-off rounds.
                
                if (this.contest.NumberOfFlyoffRounds <= 0)
                {
                    base.Alert("We could not find any flyoff rounds for this contest.  Contest creation is complete.");
                    return;
                }
                
                foreach (var flyOffRound in this.contest.Rounds.Where(r => r.Value.IsFlyOffRound))
                {                    
                    this.Rounds.Add(new ContestRoundListItemViewModel
                    {
                        RoundNumber = flyOffRound.Key,
                        AvailableTasks = this.AvailableTasks.ToList(),
                        SelectedTask = allTasks.Where(t => t.Id == flyOffRound.Value.AssignedTaskId).Single()
                    });
                }

                // Disable the 'add' button if we are at capacity for flyoff rounds.
                if(this.Rounds.Count >= this.contest.NumberOfFlyoffRounds)
                {
                    this.IsAddRoundButtonEnabled = false;
                }
            }
            catch (Exception ex)
            {
                base.Throw(ex);
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
                state[nameof(ContestTasksPageViewModel)] = this;
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
        /// Saves changes and goes to next page.
        /// </summary>
        public async Task SaveAndGoToNextPage()
        {
            await SaveRoundTasks(this);
            NavigationService.Navigate(typeof(CoverPage));
        }

        /// <summary>
        /// Event handler, when the selection of the round changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task RoundSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Run(() => {
                this.Dispatcher.DispatchAsync(new Action(() => { RenameRoundNumbers(); }));
                RemoveButtonEnabled = true;
            });

        }

        /// <summary>
        /// Event hanlder for when a Round is added.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        public async Task AddRound(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.DispatchAsync(() =>
            {
                this.Rounds.Add(new ContestRoundListItemViewModel
                {
                    RoundNumber = Rounds.Count + 1,
                    SelectedTask = SelectedTaskItem
                });

                if(this.Rounds.Count >= this.contest.NumberOfFlyoffRounds)
                {
                    this.IsAddRoundButtonEnabled = false;
                }
            });
        }

        /// <summary>
        /// Event handler for when a Round is removed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        public async Task RemoveRound(object sender, RoutedEventArgs e)
        {
            await this.Dispatcher.DispatchAsync(() =>
            {
                this.Rounds.Remove(SelectedRoundItem);
                RenameRoundNumbers();

                this.IsAddRoundButtonEnabled = true;
            });
        }

        /// <summary>
        /// Event handler for whne the contest tasks container contents are changed.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The <see cref="ContainerContentChangingEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public async Task ContestTaskContainerItemsChanging(object sender, ContainerContentChangingEventArgs args)
        {
            await Task.FromResult(((ListViewItem)args.ItemContainer).IsSelected = true);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Renames the round numbers.
        /// </summary>
        private void RenameRoundNumbers()
        {
            // Get the last contest round and start from there
            var regulationRounds = this.contest.Rounds.Where(r => !r.Value.IsFlyOffRound);
            var lastRegulationRound = 0;

            if(regulationRounds.Count() > 0)
            {
                lastRegulationRound = regulationRounds.Select(result => result.Key).Max() + 1;
            }

            for (int i = 0; i < this.Rounds.Count; i++)
            {
                this.Rounds[i].RoundNumber = lastRegulationRound + i + 1;
            }
        }

        /// <summary>
        /// Saves the round tasks.
        /// </summary>
        /// <param name="contestTasksViewModel">The contest tasks view model.</param>
        /// <returns></returns>
        private async Task<bool> SaveRoundTasks(FlyOffsTaskSelectionViewModel contestTasksViewModel)
        {
            // Make a copy of the regulation rounds.
            var regulationRoundsToAddBackIn = new Dictionary<int, Round>();
            var cntr = 0;
            foreach (var round in this.contest.Rounds.Where(r => !r.Value.IsFlyOffRound))
            {
                regulationRoundsToAddBackIn.Add(cntr, round.Value);
                ++cntr;
            }

            // Need to clear in case the previous screen
            // Added or removed an item, need to redo the round
            // indexes.
            this.contest.Rounds.Clear();

            // Add in the regulation rounds
            for(var i = 0; i < regulationRoundsToAddBackIn.Count; ++i)
            {
                contest.Rounds.Add(i, regulationRoundsToAddBackIn[i]);
            }

            var lastRegulationRound = regulationRoundsToAddBackIn.Count;

            // Add in the new Rounds
            for (var i = 0; i < this.Rounds.Count; ++i)
            {
                contest.Rounds.Add(lastRegulationRound,
                    new Round
                    {
                        FlightGroups = new Dictionary<FlightGroup, List<Pilot>>(),
                        AssignedTaskId = this.Rounds[i].SelectedTask.Id,
                        IsFlyOffRound = true
                    });

                lastRegulationRound++;
            }

            // Update the contest.
            var contestUpdateResult = await this.contestStorageInteractor.Value.UpdateContestAsync(contest);

            if (contestUpdateResult.IsFaulted)
            {
                base.Alert($"{nameof(ContestTasksPageViewModel)}:{nameof(SaveRoundTasks)} - Failed to update the contest.");
            }

            return !contestUpdateResult.IsFaulted;
        }

        #endregion
    }
}
