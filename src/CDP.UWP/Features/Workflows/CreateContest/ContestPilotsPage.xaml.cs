//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: ContestPilotsPage.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.CreateContest
{
    using CDP.UWP.Features.Workflows.CreateContest.AdditionalViewModels;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContestPilotsPage : Page
    {
        public ContestPilotsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the ItemClick event of the AllPilotsList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ItemClickEventArgs"/> instance containing the event data.</param>
        private void AllPilotsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.RemoveAvailablePilot(e.ClickedItem as PilotListItemViewModel);
            ViewModel.AddSelectedPilot(e.ClickedItem as PilotListItemViewModel);
        }

        /// <summary>
        /// Handles the ItemClick event of the SelectedPilotsList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ItemClickEventArgs"/> instance containing the event data.</param>
        private void SelectedPilotsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.RemoveSelectedPilot(e.ClickedItem as PilotListItemViewModel);
            ViewModel.AddAvailablePilot(e.ClickedItem as PilotListItemViewModel);
        }
    }
}
