//---------------------------------------------------------------
// Date: 2/3/2018
// Rights: 
// FileName: ContestEngineBase.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.ContestEngine
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Tasks;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.Scoring.Commands;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the behaviors (and a base implementation) of a Contest engine.  
    /// The contest engine controls the flow of the contest, when to move to the next
    /// flight group, when to move to the next round, how to set up the clock, etc...
    /// </summary>
    public abstract class ContestEngineBase
    {
        /// <summary>
        /// The contest storage interactor
        /// </summary>
        protected Lazy<ContestStorageCmdInteractor> contestStorageIntr;

        /// <summary>
        /// The scoring command interactor
        /// </summary>
        protected Lazy<ScoringStorageCmdInteractor> scoringCmdIntr;

        /// <summary>
        /// Gets the current round ordinal.
        /// </summary>
        /// <value>
        /// The current round ordinal.
        /// </value>
        public virtual int CurrentRoundOrdinal
        {
            get { return this.Contest?.State.CurrentRoundOrdinal ?? 0; }
            protected set { this.Contest.State.CurrentRoundOrdinal = value; }
        }

        /// <summary>
        /// Gets the current flight group.
        /// </summary>
        /// <value>
        /// The current flight group.
        /// </value>
        public virtual FlightGroup CurrentFlightGroup
        {
            get { return this.Contest.State.CurrentFlightGroup; }
            protected set { this.Contest.State.CurrentFlightGroup = value; }
        }

        /// <summary>
        /// Gets the current task.
        /// </summary>
        /// <value>
        /// The current task.
        /// </value>
        public virtual TaskBase CurrentTask { get; protected set; }

        /// <summary>
        /// Gets the current time window.
        /// </summary>
        /// <value>
        /// The current time window.
        /// </value>
        public virtual TimeWindow CurrentTimeWindow { get; protected set; }
        
        /// <summary>
        /// Gets or sets the current contest.
        /// </summary>
        /// <value>
        /// The current contest.
        /// </value>
        public virtual Contest Contest { get; protected set; }

        /// <summary>
        /// Gets or sets the flight matrix.
        /// </summary>
        /// <value>
        /// The flight matrix.
        /// </value>
        public virtual FlightMatrix FlightMatrix { get; protected set; }

        /// <summary>
        /// Gets all pilots in contest.
        /// </summary>
        /// <value>
        /// All pilots in contest.
        /// </value>
        public List<Pilot> PilotsInContest { get; protected set; } = new List<Pilot>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContestEngineBase"/> class.
        /// </summary>
        /// <param name="contestStorageCmdInteractor">The contest storage command interactor.</param>
        /// <param name="scoringStorageCmdInteractor">The scoring storage command interactor.</param>
        public ContestEngineBase(ContestStorageCmdInteractor contestStorageCmdInteractor,
            ScoringStorageCmdInteractor scoringStorageCmdInteractor)
        {
            this.contestStorageIntr = new Lazy<ContestStorageCmdInteractor>(() => contestStorageCmdInteractor);
            this.scoringCmdIntr = new Lazy<ScoringStorageCmdInteractor>(() => scoringStorageCmdInteractor);
        }

        /// <summary>
        /// Initializes the specified contest.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        public virtual async Task Initialize(Contest contest)
        {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Moves to next contest stage.
        /// </summary>
        /// <returns></returns>
        public virtual async Task MoveToNextContestStage()
        {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Gets the next time window.
        /// </summary>
        /// <returns></returns>
        public virtual void SetTimeWindow()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scores the flight group.
        /// </summary>
        /// <param name="timeSheets">The time sheets.</param>
        /// <returns></returns>
        public virtual async Task ScoreFlightGroup(IEnumerable<TimeSheet> timeSheets)
        {
            await Task.FromException(new NotImplementedException());
        }

        /// <summary>
        /// Resets the flight windows.
        /// </summary>
        /// <returns></returns>
        public virtual void ResetFlightWindows()
        {
            throw new NotImplementedException();
        }
    }
}
