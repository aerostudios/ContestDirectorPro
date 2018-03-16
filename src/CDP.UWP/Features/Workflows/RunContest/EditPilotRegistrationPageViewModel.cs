//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: EditPilotRegistrationPageViewModel.cs
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
    using CDP.Repository.WindowsStorage;
    using CDP.UWP.Config;
    using CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels;
    using CDP.UWP.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Defines a view model for the edit pilot page of the run contest workflow
    /// </summary>
    /// <seealso cref="CDP.UWP.Models.CdproViewModelBase" />
    public class EditPilotRegistrationPageViewModel : CdproViewModelBase
    {
        #region Instance Members

        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string amaNumber = string.Empty;
        private string airframe = string.Empty;
        private bool isPaid = false;
        private bool isAirframeRegistered = false;
        private string pilotId = string.Empty;
        private string contestId = string.Empty;
        private string registrationId = string.Empty;
        private bool isNewPilot = false;
        private Contest contest;

        /// <summary>
        /// The contest query interactor
        /// </summary>
        private Lazy<ContestQueryInteractor> contestQueryInteractor = new Lazy<ContestQueryInteractor>(
            () => new ContestQueryInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The pilot command interactor
        /// </summary>
        private Lazy<PilotRegistrationStorageCmdInteractor> registrationCmdInteractor = new Lazy<PilotRegistrationStorageCmdInteractor>(
            () => new PilotRegistrationStorageCmdInteractor(
                    new RegistrationRepository(StorageFileConfig.RegistrationFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The pilot command interactor
        /// </summary>
        private Lazy<PilotStorageCmdInteractor> pilotCmdInteractor = new Lazy<PilotStorageCmdInteractor>(
            () => new PilotStorageCmdInteractor(
                    new PilotFileSystemRepository(StorageFileConfig.RegistrationFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The pilot query interactor
        /// </summary>
        private Lazy<PilotQueryInteractor> pilotQueryInteractor = new Lazy<PilotQueryInteractor>(
            () => new PilotQueryInteractor(
                    new PilotFileSystemRepository(StorageFileConfig.RegistrationFileName, App.Cache, App.Logger), App.Logger));

        /// <summary>
        /// The contest command intr
        /// </summary>
        private Lazy<ContestStorageCmdInteractor> contestCmdIntr = new Lazy<ContestStorageCmdInteractor>(
            () => new ContestStorageCmdInteractor(
                    new ContestRespository(StorageFileConfig.ContestsFileName, App.Cache, App.Logger), App.Logger));

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
        public string Airframe { get { return airframe; } set { Set(ref airframe, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is paid; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaid { get { return isPaid; } set { Set(ref isPaid, value); } }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is airframe registered.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is airframe registered; otherwise, <c>false</c>.
        /// </value>
        public bool AirframesSignedOff { get { return isAirframeRegistered; } set { Set(ref isAirframeRegistered, value); } }

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditPilotRegistrationViewModel"/> class.
        /// </summary>
        public EditPilotRegistrationPageViewModel()
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
                // There should be a registration ID that is passed to this page
                // Otherwise, we need to throw an exception
                // If a contest was passed to this model, load it up for registration
                var passedRegistration = parameter as PilotRegistrationListViewItemViewModel;

                if (passedRegistration == null)
                {
                    base.Throw(new Exception($"This page, {nameof(EditPilotRegistrationPageViewModel)} requires a registration view model parameter."));
                }

                this.Airframe = passedRegistration.Airframes.FirstOrDefault();
                this.AmaNumber = passedRegistration.AmaNumber;
                this.FirstName = passedRegistration.FirstName;
                this.LastName = passedRegistration.LastName;
                this.IsPaid = passedRegistration.IsPaid;
                this.AirframesSignedOff = passedRegistration.AirframesSignedOff;
                this.pilotId = passedRegistration.PilotId;
                this.registrationId = passedRegistration.RegistrationId;
                this.isNewPilot = passedRegistration.IsNewPilot;

                var contestResult = await this.contestQueryInteractor.Value.GetAllContestsAsync();
                if (contestResult.IsFaulted)
                {
                    base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(OnNavigatedToAsync)} - Could not get contests.");
                }

                this.contest = contestResult.Value.Where(c => c.Id == passedRegistration.ContestId).SingleOrDefault();

                if (contest == null)
                {
                    base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(OnNavigatedToAsync)} - Could not find the contest for this registration.");
                }
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
        /// Saves the and continue.
        /// </summary>
        /// <returns></returns>
        public async Task SaveAndContinue()
        {
            // This might be a new pilot registration, handle it.
            if (isNewPilot)
            {
                var pilotToAdd = new Pilot(this.FirstName, this.LastName, string.Empty, this.AmaNumber, this.Airframe);

                try
                {
                    // Create the pilot
                    var createPilotResult = await this.pilotCmdInteractor.Value.CreateNewPilotAsync(pilotToAdd);
                    if (createPilotResult.IsFaulted)
                    {
                        base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(SaveAndContinue)} - Failed to add new pilot.");
                        return;
                    }

                    // Update the contest to make sure the pilot roster is update w/ the new pilot...
                    this.contest.PilotRoster.Add(createPilotResult.Value.Id);
                    // Be sure to add the new pilot id back into the viewmodel
                    pilotId = createPilotResult.Value.Id;
                }
                catch(Exception ex)
                {
                    App.Logger.LogException(ex);
                    base.Alert($"An error occured creating a new pilot for the registration.");
                    return;
                }

                var updateContestResult = await this.contestCmdIntr.Value.UpdateContestAsync(contest);
                if (updateContestResult.IsFaulted)
                {
                    base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(SaveAndContinue)} - Failed to update the contest.");
                    return;
                }
            }
            else
            {
                // Save any changes to the pilot object
                var storedPilotsResult = await pilotQueryInteractor.Value.GetAllPilotsAsync();
                if (storedPilotsResult.IsFaulted)
                {
                    base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(SaveAndContinue)} - Failed to save changes to the pilot.");
                    return;
                }

                var storedPilotToUpdate = storedPilotsResult.Value.Where(p => p.Id == pilotId).FirstOrDefault();
                if (storedPilotToUpdate == null) { return; } //TODO: Throw an error message here...

                storedPilotToUpdate.FirstName = this.FirstName;
                storedPilotToUpdate.LastName = this.LastName;
                storedPilotToUpdate.StandardsBodyId = this.AmaNumber;
                storedPilotToUpdate.Airframe = this.Airframe;

                var updatePilotResult = await pilotCmdInteractor.Value.UpdatePilotAsync(storedPilotToUpdate);
                if (updatePilotResult.IsFaulted)
                {
                    base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(SaveAndContinue)} - Failed to update the pilot.");
                    return;
                }
            }

            // Save the registration
            var registrationToSave = new PilotRegistration
            {
                ContestId = this.contest.Id,
                Id = registrationId,
                IsPaid = IsPaid,
                AirframesSignedOff = AirframesSignedOff,
                PilotId = pilotId
            };

            try
            {
                var registerPilotResult = await registrationCmdInteractor.Value.RegisterSinglePilotForContestAsync(registrationToSave);
                if (registerPilotResult.IsFaulted)
                {
                    base.Alert($"{nameof(EditPilotRegistrationPageViewModel)}:{nameof(SaveAndContinue)} - Failed to save the registration.");
                    return;
                }
            }
            catch
            {
                base.Alert("An error occured creating a new pilot registration.");
            }

            this.NavigationService.Navigate(typeof(PilotRegistrationPage), this.contest);
        }

        /// <summary>
        /// Cancels the changes.
        /// </summary>
        /// <returns></returns>
        public void CancelChanges()
        {
            this.NavigationService.Navigate(typeof(PilotRegistrationPage), this.contest);
        }

        #endregion

        #endregion
    }
}
