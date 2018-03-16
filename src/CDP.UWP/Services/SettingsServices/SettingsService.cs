//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: CdproViewModelBase.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Settings.SettingsServices
{
    using System;
    using Template10.Common;
    using Template10.Utils;
    using Windows.UI.Xaml;
    
    /// <summary>
    /// Service for Application settings.
    /// </summary>
    public class SettingsService
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SettingsService Instance { get; } = new SettingsService();

        /// <summary>
        /// The helper
        /// </summary>
        Template10.Services.SettingsService.ISettingsHelper _helper;

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingsService"/> class from being created.
        /// </summary>
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use shell back button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use shell back button]; otherwise, <c>false</c>.
        /// </value>
        public bool UseShellBackButton
        {
            get { return true; }//return _helper.Read<bool>(nameof(UseShellBackButton), true); }
            set
            {
                _helper.Write(nameof(UseShellBackButton), value);
                BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
                {
                    BootStrapper.Current.ShowShellBackButton = value;
                    BootStrapper.Current.UpdateShellBackButton();
                });
            }
        }

        /// <summary>
        /// Gets or sets the application theme.
        /// </summary>
        /// <value>
        /// The application theme.
        /// </value>
        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Light;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                (Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
                Shell.Shell.HamburgerMenu.RefreshStyles(value, true);
            }
        }

        /// <summary>
        /// Gets or sets the maximum duration of the cache.
        /// </summary>
        /// <value>
        /// The maximum duration of the cache.
        /// </value>
        public TimeSpan CacheMaxDuration
        {
            get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
            set
            {
                _helper.Write(nameof(CacheMaxDuration), value);
                BootStrapper.Current.CacheMaxDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show hamburger button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show hamburger button]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowHamburgerButton
        {
            get { return _helper.Read<bool>(nameof(ShowHamburgerButton), true); }
            set
            {
                _helper.Write(nameof(ShowHamburgerButton), value);
                Shell.Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is full screen.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is full screen; otherwise, <c>false</c>.
        /// </value>
        public bool IsFullScreen
        {
            get { return _helper.Read<bool>(nameof(IsFullScreen), false); }
            set
            {
                _helper.Write(nameof(IsFullScreen), value);
                Shell.Shell.HamburgerMenu.IsFullScreen = value;
            }
        }
    }
}
