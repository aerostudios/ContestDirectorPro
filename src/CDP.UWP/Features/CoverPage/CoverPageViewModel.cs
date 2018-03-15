//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: CoverPageViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Template10.Mvvm;
    using Template10.Services.NavigationService;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// View model for the cover page.
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class CoverPageViewModel : ViewModelBase
    {
        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverPageViewModel"/> class.
        /// </summary>
        public CoverPageViewModel()
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
                // use parameter
            }
            await Task.CompletedTask;
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

        #endregion
    }
}
