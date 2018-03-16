//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: OpenExistingContestPageViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.OpenExistingContestPage
{
    using CDP.AppDomain.Contests;
    using CDP.CoreApp.Features.Contests.Queries;
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Features.Workflows.CreateContest;
    using CDP.UWP.Features.Workflows.RunContest;
    using CDP.UWP.Helpers;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Defines a view model for the Open contest page.
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class OpenExistingContestPageViewModel : CdproViewModelBase
    {
        #region Instance Methods

        private IEnumerable<Contest> allContests;
        private Contest selectedContest = null;
        private Type pageToContinueOnTo = null;

        /// <summary>
        /// The contest storage interactor
        /// </summary>
        private Lazy<ContestQueryInteractor> contestStorageInteractor = new Lazy<ContestQueryInteractor>(
            () => new ContestQueryInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets all contests.
        /// </summary>
        /// <value>
        /// All contests.
        /// </value>
        public IEnumerable<Contest> AllContests { get { return allContests; } set { Set(ref allContests, value); } }
        
        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenExistingContestViewModel"/> class.
        /// </summary>
        public OpenExistingContestPageViewModel()
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
                // restore state
                state.Clear();
            }
            else
            {
                // Users of this page need to pass a valid type of page we would
                // like to continue on to (looked up by a string value, which sucks (sorry).
                var suggestedType = parameter as string;

                if (!string.IsNullOrEmpty(suggestedType))
                {
                    pageToContinueOnTo = SelectNextPageType(suggestedType);
                    if (pageToContinueOnTo == null)
                    {
                        base.Throw(new Exception("Page cannt load.  Make sure you pass the page type that you would like to continue on to."));
                    }
                }
                else
                {
                    base.Throw(new Exception("Page cannot load.  Make sure you pass the page type that you would like to continue on to."));
                }
            }
            var allContestsQueryResult = await this.contestStorageInteractor.Value.GetAllContestsAsync();

            if (allContestsQueryResult.IsFaulted)
            {
                base.Alert($"{nameof(OpenExistingContestPageViewModel)}:{nameof(OnNavigatedToAsync)} - Failed to get the list of contests.");
            }

            this.AllContests = allContestsQueryResult.Value;
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
        /// Event handler for the "open" button.
        /// </summary>
        /// <returns></returns>
        public async Task OpenContest()
        {
            if (selectedContest != null)
            {
                var allContestsQueryResult = await this.contestStorageInteractor.Value.GetAllContestsAsync();

                if (allContestsQueryResult.IsFaulted)
                {
                    base.Alert($"{nameof(OpenExistingContestPageViewModel)}:{nameof(OnNavigatedToAsync)} - Failed to get the contests.");
                }

                this.AllContests = allContestsQueryResult.Value as IEnumerable<Contest>;
                
                // Navigate to the appropriate page.
                var contest = allContests.Where(c => c.Id == selectedContest.Id).FirstOrDefault();

                // Short curcuit to contest control if the contest has already started...
                if (contest.State.HasStarted)
                {
                    NavigationService.Navigate(typeof(ContestRoundsPage), new ContestOpenParameters(contest, true));
                }
                
                NavigationService.Navigate(pageToContinueOnTo, contest);
            }

            return;
        }

        /// <summary>
        /// Contests the selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public void ContestSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems?.Count > 0 && e?.AddedItems?.First() is Contest temp)
            {
                selectedContest = temp;
                this.Dispatcher.DispatchAsync(async () => await OpenContest());
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Selects the type of the next page.
        /// </summary>
        /// <param name="suggestedType">Type of the suggested.</param>
        /// <returns></returns>
        private Type SelectNextPageType(string suggestedType)
        {
            Type returnType = null;

            switch (suggestedType)
            {
                case "ContestInfoPage":
                    returnType = typeof(ContestInfoPage);
                    break;
                case "PilotRegistrationPage":
                    returnType = typeof(PilotRegistrationPage);
                    break;
                default:
                    break;
            }

            return returnType;
        }

        #endregion
    }
}
