//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: PilotRegistrationPage.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PilotRegistrationPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PilotRegistrationPage"/> class.
        /// </summary>
        public PilotRegistrationPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Toggled event of the ToggleSwitch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void IsPaid_Toggled(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateMoneyCollected(((ToggleSwitch)e.OriginalSource).IsOn);
        }
    }
}
