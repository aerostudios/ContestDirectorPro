//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: SettingsPageViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Settings
{
    using CDP.UWP.Components;
    using CDP.UWP.Features.Settings.SettingsServices;
    using System;
    using System.Threading.Tasks;
    using Template10.Mvvm;
    using Windows.UI.Xaml;

    // FYI - This is provided by the Template 10 app template
    internal sealed class SettingsPageViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the settings part view model.
        /// </summary>
        /// <value>
        /// The settings part view model.
        /// </value>
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();

        /// <summary>
        /// Gets the about part view model.
        /// </summary>
        /// <value>
        /// The about part view model.
        /// </value>
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        /// <summary>
        /// The settings
        /// </summary>
        SettingsService _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPartViewModel"/> class.
        /// </summary>
        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = SettingsService.Instance;
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
            get { return _settings.ShowHamburgerButton; }
            set { _settings.ShowHamburgerButton = value; base.RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is full screen.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is full screen; otherwise, <c>false</c>.
        /// </value>
        public bool IsFullScreen
        {
            get { return _settings.IsFullScreen; }
            set
            {
                _settings.IsFullScreen = value;
                base.RaisePropertyChanged();
                if (value)
                {
                    ShowHamburgerButton = false;
                }
                else
                {
                    ShowHamburgerButton = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use shell back button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use shell back button]; otherwise, <c>false</c>.
        /// </value>
        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use light theme button].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use light theme button]; otherwise, <c>false</c>.
        /// </value>
        public bool UseLightThemeButton
        {
            get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
            set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
        }

        /// <summary>
        /// The busy text
        /// </summary>
        private string _BusyText = "Please wait...";

        /// <summary>
        /// Gets or sets the busy text.
        /// </summary>
        /// <value>
        /// The busy text.
        /// </value>
        public string BusyText
        {
            get { return _BusyText; }
            set
            {
                Set(ref _BusyText, value);
                _ShowBusyCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The show busy command
        /// </summary>
        DelegateCommand _ShowBusyCommand;

        /// <summary>
        /// Gets the show busy command.
        /// </summary>
        /// <value>
        /// The show busy command.
        /// </value>
        public DelegateCommand ShowBusyCommand
            => _ShowBusyCommand ?? (_ShowBusyCommand = new DelegateCommand(async () =>
            {
                Busy.SetBusy(true, _BusyText);
                await Task.Delay(5000);
                Busy.SetBusy(false);
            }, () => !string.IsNullOrEmpty(BusyText)));
    }

    public class AboutPartViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        /// <summary>
        /// Gets the publisher.
        /// </summary>
        /// <value>
        /// The publisher.
        /// </value>
        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        /// <summary>
        /// Gets the rate me.
        /// </summary>
        /// <value>
        /// The rate me.
        /// </value>
        public Uri RateMe => new Uri("http://aka.ms/template10");
    }
}