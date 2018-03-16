//---------------------------------------------------------------
// Date: 12/16/2017
// Rights: 
// FileName: ContestRoundsPage.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest
{
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContestRoundsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContestRoundsPage"/> class.
        /// </summary>
        public ContestRoundsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the ScoreFlightGroup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private void ScoreFlightGroup_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var buttonSrc = ((Button)e.OriginalSource);
            ViewModel.ScoreFlightGroup();
        }
    }
}
