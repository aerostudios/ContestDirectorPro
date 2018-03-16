//------------------------------------------------------------
// Origin: AeroStudios
// Author: Rick Rogahn
// FileName: Shell.cs
//------------------------------------------------------------

namespace CDP.UWP.Features.Shell
{
    using CDP.UWP.Features.Settings.SettingsServices;
    using Template10.Controls;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Shell Instance { get; set; }

        /// <summary>
        /// Gets the hamburger menu.
        /// </summary>
        /// <value>
        /// The hamburger menu.
        /// </value>
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;

        /// <summary>
        /// The settings
        /// </summary>
        SettingsService _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            Instance = this;
            InitializeComponent();
            _settings = SettingsService.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        /// <summary>
        /// Sets the navigation service.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
            HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
            HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
            HamburgerMenu.HamburgerButtonVisibility = Visibility.Visible;
        }
    }
}
