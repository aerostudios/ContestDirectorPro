//---------------------------------------------------------------
// Date: 12/17/2017
// Rights:
// FileName: ContestRoundListItemViewModel.cs
//---------------------------------------------------------------

namespace CDP.UWP.Features.Workflows.CreateContest.AdditionalViewModels
{
    using CDP.AppDomain.Tasks;
    using System.Collections.Generic;
    using Template10.Mvvm;

    /// <summary>
    /// View model for a contest round item.
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class ContestRoundListItemViewModel : ViewModelBase
    {
        private int roundNumber = 0;
        private TaskBase selectedTask = null;

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>
        /// The round number.
        /// </value>
        public int RoundNumber { get { return roundNumber; } set { Set(ref roundNumber, value); } }

        /// <summary>
        /// Gets or sets the selected task.
        /// </summary>
        /// <value>
        /// The selected task.
        /// </value>
        public TaskBase SelectedTask { get { return selectedTask; } set { Set(ref selectedTask, value); } }

        /// <summary>
        /// Gets or sets the available tasks.
        /// </summary>
        /// <value>
        /// The available tasks.
        /// </value>
        public List<TaskBase> AvailableTasks { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestRoundListItemViewModel"/> class.
        /// </summary>
        /// <param name="round">The round.</param>
        public ContestRoundListItemViewModel() { }
    }
}
