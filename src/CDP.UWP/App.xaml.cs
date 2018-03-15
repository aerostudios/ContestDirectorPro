//---------------------------------------------------------------
// Date: 12/1/2017
// Rights: 
// FileName: App.cs
//---------------------------------------------------------------

namespace CDP.UWP
{
    using CDP.Common.Caching;
    using CDP.Common.Logging;
    using CDP.CoreApp.Features.ContestEngine;
    using CDP.CoreApp.Features.Timer;
    using CDP.CoreApp.Interfaces.Contests;
    using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
    using CDP.ScoringAndSortingImpl.F3K.Sorting;
    using CDP.UWP.Components;
    using CDP.UWP.Components.ContestMediator;
    using CDP.UWP.Features;
    using CDP.UWP.Features.Settings.SettingsServices;
    using CDP.UWP.Features.Shell;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Template10.Common;
    using Template10.Controls;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Main entry point of the Application.
    /// </summary>
    /// <seealso cref="Template10.Common.BootStrapper" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IXamlMetadataProvider" />
    [Bindable]
    sealed partial class App : BootStrapper
    {
        /// <summary>
        /// The logger
        /// </summary>
        internal static ILoggingService Logger = new DebugLogger();
        
        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        internal static ICache Cache = new InMemoryCache();

        /// <summary>
        /// The contest engine
        /// </summary>
        internal static ContestEngineBase ContestEngine;

        /// <summary>
        /// The contest timer
        /// </summary>
        internal static BasicTimer ContestTimer = new BasicTimer(1000);

        /// <summary>
        /// The contest communications hub
        /// </summary>
        internal static ContestMediator ContestCommunicationsHub = new ContestMediator(Logger);

        /// <summary>
        /// The sorting algos
        /// </summary>
        internal static List<ISortingAlgo> SortingAlgos = new List<ISortingAlgo>();

        /// <summary>
        /// The fly off selection algos
        /// </summary>
        internal static List<IFlyOffSelectionAlgo> FlyOffSelectionAlgos = new List<IFlyOffSelectionAlgo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();
            this.UnhandledException += App_UnhandledException;

            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsService.Instance;
            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;
            AutoSuspendAllFrames = true;
            AutoRestoreAfterTerminated = true;
            AutoExtendExecutionSession = true;

            #endregion
        }

        /// <summary>
        /// Initializes the sorting algos.
        /// </summary>
        private void InitSortingAlgos()
        {
            SortingAlgos.Add(new MoMSingleRoundSort());
            SortingAlgos.Add(new RandomSortNoTeamProtection());
        }

        /// <summary>
        /// Initializes the fly off algos.
        /// </summary>
        private void InitFlyOffAlgos()
        {
            FlyOffSelectionAlgos.Add(new Top10PilotsFlyoffsSort());
        }

        /// <summary>
        /// Creates the root element.  Loads the Shell for the application.
        /// </summary>
        /// <param name="e">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Shell(service),
                ModalContent = new Busy(),
            };
        }

        /// <summary>
        /// Called when [start asynchronous].
        /// </summary>
        /// <param name="startKind">The start kind.</param>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // "long running" tasks here.
            InitSortingAlgos();
            InitFlyOffAlgos();

            await NavigationService.NavigateAsync(typeof(CoverPage));
        }

        /// <summary>
        /// Handles the UnhandledException event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Logger.LogException(e.Exception);
            var messageDialog = new MessageDialog($"An unhandled exception occured: {e.Message}")
            {
                Title = "Unhandled Error"
            };
            var result = messageDialog.ShowAsync();
            result.AsTask().Wait();
            Debug.WriteLine("Ouch: " + e.Message + "\r\n" + e.Exception.StackTrace);
            e.Handled = true;
        }
    }
}
