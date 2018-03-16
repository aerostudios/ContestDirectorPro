//---------------------------------------------------------------
// Date: 1/17/2018
// Rights: 
// FileName: SubmittedScoreViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.RunContest.AdditionalViewModels
{
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Scoring;
    using CDP.UWP.Models;
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// View model for a Timesheet that was submitted by a timer, in the Round Timer section of the Contest Rounds page
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class SubmittedScoreViewModel : CdproViewModelBase
    {
        TimeSheet timeSheet = new TimeSheet();
        Pilot pilot;
        string timingDeviceId = string.Empty;

        /// <summary>
        /// Gets the pilots times.
        /// </summary>
        /// <value>
        /// The pilots times.
        /// </value>
        public ObservableCollection<string> PilotsTimes { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets the name of the pilot.
        /// </summary>
        /// <value>
        /// The name of the pilot.
        /// </value>
        public string PilotName { get; private set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmittedScoreViewModel"/> class.
        /// </summary>
        /// <param name="pilot">The pilot.</param>
        /// <param name="timeSheet">The time sheet.</param>
        public SubmittedScoreViewModel(Pilot pilot, TimeSheet timeSheet)
        {
            if (pilot == null) base.Throw(new ArgumentNullException(nameof(pilot)));
            if (timeSheet == null) base.Throw(new ArgumentNullException(nameof(timeSheet)));

            this.pilot = pilot;
            this.timeSheet = timeSheet;

            this.PilotName = $"{this.pilot.FirstName} {this.pilot.LastName}";

            foreach(var timegate in timeSheet.TimeGates)
            {
                this.PilotsTimes.Add(timegate.Time.ToString("c"));
            }
        }
    }
}
