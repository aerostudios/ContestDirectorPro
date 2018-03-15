//---------------------------------------------------------------
// Date: 12/17/2017
// Rights: 
// FileName: PilotListItemViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.CreateContest.AdditionalViewModels
{
    using CDP.AppDomain.Pilots;
    using CDP.Common;
    using Template10.Mvvm;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Defines a Pilot List item
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class PilotListItemViewModel : ViewModelBase
    {
        #region Internal Members

        private Pilot internalPilot = new Pilot(string.Empty, string.Empty, string.Empty);
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id => internalPilot?.Id ?? string.Empty;

        /// <summary>
        /// Gets the airframe.
        /// </summary>
        /// <value>
        /// The airframe.
        /// </value>
        public string Airframe => internalPilot?.Airframe ?? string.Empty;

        /// <summary>
        /// Gets the ama number.
        /// </summary>
        /// <value>
        /// The ama number.
        /// </value>
        public string AmaNumber => internalPilot?.StandardsBodyId ?? string.Empty;

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName => internalPilot?.FirstName ?? string.Empty;

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName => internalPilot?.LastName ?? string.Empty;

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName => string.Format("{0} {1}", internalPilot?.FirstName ?? string.Empty, internalPilot?.LastName ?? string.Empty);

        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotListItemViewModel"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        public PilotListItemViewModel(Pilot p)
        {
            Validate.IsNotNull(p, nameof(p));

            internalPilot.Id = p.Id;
            internalPilot.Airframe = p.Airframe;
            internalPilot.StandardsBodyId = p.StandardsBodyId;
            internalPilot.FirstName = p.FirstName;
            internalPilot.LastName = p.LastName;
        }

        #endregion

        /// <summary>
        /// Data context change event handler.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="DataContextChangedEventArgs"/> instance containing the event data.</param>
        public void DataContextChanged(object o, DataContextChangedEventArgs e)
        {
            var sdf = o as StackPanel;
        }
    }
}
