//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: ContestInfoViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.CreateContest
{
    using CDP.AppDomain.Contests;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.Contests.Queries;
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// View Model for the Contest Info page.
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class ContestInfoViewModel : CdproViewModelBase
    {
        private Contest contest;
        private string contestId = string.Empty;
        private string contestName = string.Empty;
        private DateTimeOffset startDate = DateTimeOffset.Now;
        private DateTimeOffset endDate = DateTimeOffset.Now;
        private int suggestedNumberOfPilotsPerRound = 0;
        private int selectedNumberOfFlyoffRounds = 0;
        private double contestRegistrationFee = 0.0;
        private bool allowDrop = true;
        private int selectedContestType = 1;
        private string selectedPilotSortType;
        private string selectedFlyoffSelectionType;

        /// <summary>
        /// The contest storage intr
        /// </summary>
        private Lazy<ContestStorageCmdInteractor> contestStorageIntr = new Lazy<ContestStorageCmdInteractor>(
            () => new ContestStorageCmdInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The contest query interactor
        /// </summary>
        private Lazy<ContestQueryInteractor> contestQueryIntr = new Lazy<ContestQueryInteractor>(
            () => new ContestQueryInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string ContestId { get { return contestId; } set { Set(ref contestId, value); } }

        /// <summary>
        /// Gets or sets the name of the contest.
        /// </summary>
        /// <value>
        /// The name of the contest.
        /// </value>
        public string ContestName { get { return contestName; } set { Set(ref contestName, value); } }

        /// <summary>
        /// Gets or sets the contest registration fee.
        /// </summary>
        /// <value>
        /// The contest registration fee.
        /// </value>
        public double ContestRegistrationFee { get { return contestRegistrationFee; } set { Set(ref contestRegistrationFee, value); } }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTimeOffset StartDate { get { return startDate; } set { Set(ref startDate, value); } }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTimeOffset EndDate { get { return endDate; } set { Set(ref endDate, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether [allow drop].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow drop]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowDrop { get { return allowDrop; } set { Set(ref allowDrop, value); } }

        /// <summary>
        /// Gets or sets the selected number of pilots per group.
        /// </summary>
        /// <value>
        /// The selected number of pilots per group.
        /// </value>
        public int SelectedNumberOfPilotsPerGroup { get { return suggestedNumberOfPilotsPerRound; } set { Set(ref suggestedNumberOfPilotsPerRound, value); } }

        /// <summary>
        /// Gets or sets the number of pilots per round.
        /// </summary>
        /// <value>
        /// The number of pilots per round.
        /// </value>
        public List<short> NumberOfPilotsPerRoundData { get; set; } = new List<short>();

        /// <summary>
        /// Gets or sets the selected number of flyoff rounds.
        /// </summary>
        /// <value>
        /// The selected number of flyoff rounds.
        /// </value>
        public int SelectedNumberOfFlyoffRounds { get { return selectedNumberOfFlyoffRounds; } set { Set(ref selectedNumberOfFlyoffRounds, value); } }

        /// <summary>
        /// Gets the number of flyoff rounds.
        /// </summary>
        /// <value>
        /// The number of flyoff rounds.
        /// </value>
        public List<short> NumberOfFlyoffRoundsData { get; private set; } = new List<short>();

        /// <summary>
        /// Gets the contest type data.
        /// </summary>
        /// <value>
        /// The contest type data.
        /// </value>
        public List<KeyValuePair<string, int>> ContestTypeData { get; private set; } = new List<KeyValuePair<string, int>>();

        /// <summary>
        /// Gets the pilot sorting type data.
        /// </summary>
        /// <value>
        /// The pilot sorting type data.
        /// </value>
        public List<SortingAlgoViewModel> PilotSortingTypeData { get; private set; } = new List<SortingAlgoViewModel>();

        /// <summary>
        /// Gets the fly off selection type data.
        /// </summary>
        /// <value>
        /// The fly off selection type data.
        /// </value>
        public List<SortingAlgoViewModel> FlyOffSelectionTypeData { get; private set; } = new List<SortingAlgoViewModel>();

        /// <summary>
        /// Gets or sets the type of the selected contest.
        /// </summary>
        /// <value>
        /// The type of the selected contest.
        /// </value>
        public int SelectedContestType { get { return selectedContestType; } set { Set(ref selectedContestType, value); } }

        /// <summary>
        /// Gets or sets the type of the selected pilot sort.
        /// </summary>
        /// <value>
        /// The type of the selected pilot sort.
        /// </value>
        public string SelectedPilotSortType { get { return selectedPilotSortType; } set { Set(ref selectedPilotSortType, value); } }

        /// <summary>
        /// Gets or sets the type of the selected fly off selection.
        /// </summary>
        /// <value>
        /// The type of the selected fly off selection.
        /// </value>
        public string SelectedFlyOffSelectionType { get { return selectedFlyoffSelectionType; } set { Set(ref selectedFlyoffSelectionType, value); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestInfoViewModel" /> class.
        /// </summary>
        public ContestInfoViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // design-time experience
                return;
            }

            PopulateRoundsData();
            PopulateSuggestedPilotsPerRoundData();
            PopulateContestTypesData();
            PopulateSortingTypesData();
            PopulateFlyOffSelectionTypesData();
        }

        /// <summary>
        /// Populates the sorting types data.
        /// </summary>
        private void PopulateSortingTypesData()
        {
            foreach (var sortingAlgo in App.SortingAlgos) {
                this.PilotSortingTypeData.Add(new SortingAlgoViewModel
                {
                    DisplayName = sortingAlgo.GetSuggestedDisplayName(),
                    Id = sortingAlgo.GetUniqueId(),
                    Description = sortingAlgo.GetDescription()
                });
            }
        }

        /// <summary>
        /// Populates the fly off selection types data.
        /// </summary>
        private void PopulateFlyOffSelectionTypesData()
        {
            foreach(var flyOffSelectionAlgo in App.FlyOffSelectionAlgos)
            {
                this.FlyOffSelectionTypeData.Add(new SortingAlgoViewModel
                {
                    DisplayName = flyOffSelectionAlgo.GetSuggestedDisplayName(),
                    Id = flyOffSelectionAlgo.GetUniqueId(),
                    Description = flyOffSelectionAlgo.GetDescription()
                });
            }
        }

        /// <summary>
        /// Populates the contest types data.
        /// </summary>
        private void PopulateContestTypesData()
        {
            this.ContestTypeData.Add(new KeyValuePair<string, int>("F3K", 1));
        }

        #region Public Methods

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
                var previousState = state[nameof(ContestInfoViewModel)] as ContestInfoViewModel;

                // Set any previous contest in progress (if a contest was sent in as a parameter 
                // before the app was suspended for instance)
                contest = previousState.contest;

                if (previousState != null)
                {
                    this.NumberOfPilotsPerRoundData = previousState.NumberOfPilotsPerRoundData;
                    this.AllowDrop = previousState.AllowDrop;
                    this.ContestName = previousState.ContestName;
                    this.EndDate = previousState.EndDate;
                    this.ContestId = previousState.contestId;
                    this.SelectedNumberOfFlyoffRounds = previousState.SelectedNumberOfFlyoffRounds;
                    this.SelectedNumberOfPilotsPerGroup = previousState.SelectedNumberOfPilotsPerGroup;
                    this.StartDate = previousState.StartDate;
                    this.ContestRegistrationFee = previousState.ContestRegistrationFee;
                }
                state.Clear();
            }
            else
            {
                // If a contest was passed to this model, load it up for editing
                if (parameter is Contest passedContest)
                {
                    this.contest = passedContest;
                    LoadUI();
                }
                else
                {
                    this.contest = new Contest();
                    this.ContestId = string.Empty;
                    this.SelectedNumberOfPilotsPerGroup = 7;
                    this.SelectedNumberOfFlyoffRounds = 0;
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
                state.Add(nameof(ContestInfoViewModel), this);
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
        /// Saves and goes to the next page.
        /// </summary>
        /// <returns></returns>
        public async Task SaveAndGoToNextPage()
        {
            var result = await SaveContestInfo();

            if (result)
            {
                NavigationService.Navigate(typeof(ContestPilotsPage), this.contest);
            }
        }

        #endregion

        /// <summary>
        /// Populates the rounds data.
        /// </summary>
        private void PopulateRoundsData()
        {
            for (short i = 0; i < 30; i++)
            {
                NumberOfFlyoffRoundsData.Add(i);
            }
        }

        /// <summary>
        /// Populates the suggested pilots per round data.
        /// </summary>
        private void PopulateSuggestedPilotsPerRoundData()
        {
            for (short i = 0; i < 15; ++i)
            {
                NumberOfPilotsPerRoundData.Add(i);
            }
        }

        /// <summary>
        /// Saves the contest information asynchronously.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveContestInfo()
        {
            this.contest.StartDate = this.StartDate;
            this.contest.EndDate = this.EndDate;
            this.contest.Name = this.ContestName;
            this.contest.SuggestedNumberOfPilotsPerGroup = this.SelectedNumberOfPilotsPerGroup;
            this.contest.NumberOfFlyoffRounds = this.SelectedNumberOfFlyoffRounds;
            this.contest.AllowDroppedRound = this.AllowDrop;
            this.contest.ContestRegistrationFee = this.ContestRegistrationFee;
            this.contest.SortingAlgoId = this.SelectedPilotSortType;
            // TODO: Figure out why the UI doesn't like my combo box for this...
            this.contest.FlyOffSelectionAlgoId = App.FlyOffSelectionAlgos.First().GetUniqueId();
            this.contest.Type = (ContestType)this.SelectedContestType;

            // Create or update, depending on situation
            if (string.IsNullOrEmpty(this.contestId))
            {
                this.contestId = Guid.NewGuid().ToString();
                var createContestResult = await contestStorageIntr.Value.CreateContestAsync(this.contest);
                // TODO - Better error handling here...
                if (createContestResult.IsFaulted)
                {
                    base.Alert($"An error occured trying to save the contest. {createContestResult.Error.ErrorMessage}");
                    return false;
                }

                return true;
            }
            else
            {
                var updateResult = await contestStorageIntr.Value.UpdateContestAsync(this.contest);
                // TODO - Better error handling here...
                return !updateResult.IsFaulted ? true : false;
            }
        }

        /// <summary>
        /// Loads from contest object.
        /// </summary>
        /// <param name="contest">The contest.</param>
        private void LoadUI()
        {
            this.ContestId = this.contest.Id;
            this.ContestName = this.contest.Name;
            this.SelectedNumberOfFlyoffRounds = this.contest.NumberOfFlyoffRounds;
            this.SelectedNumberOfPilotsPerGroup = this.contest.SuggestedNumberOfPilotsPerGroup;
            this.SelectedPilotSortType = this.contest.SortingAlgoId;
            this.SelectedFlyOffSelectionType = this.contest.FlyOffSelectionAlgoId;
            this.SelectedContestType = (int)this.contest.Type;
            this.StartDate = this.contest.StartDate;
            this.EndDate = this.contest.EndDate;
            this.AllowDrop = this.contest.AllowDroppedRound;
            this.ContestRegistrationFee = this.contest.ContestRegistrationFee;
        }
    }

    /// <summary>
    /// Display element for the types of sorting algos
    /// </summary>
    public class SortingAlgoViewModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }
}
