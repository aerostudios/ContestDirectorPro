//---------------------------------------------------------------
// Date: 7/7/2014
// Rights: 
// FileName: FlightMatrixPage.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest
{
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FlightMatrixPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlightMatrixPage"/> class.
        /// </summary>
        public FlightMatrixPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the GenerateMatrix control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void GenerateMatrix_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () => { await ViewModel.GenerateMatrix(); });
        }

        /// <summary>
        /// Handles the Click event of the SaveAndContinue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveAndContinue_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveAndContinue();
        }
    }
}
