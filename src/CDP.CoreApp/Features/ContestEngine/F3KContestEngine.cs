//---------------------------------------------------------------
// Date: 2/3/2018
// Rights: 
// FileName: F3KContestEngine.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.ContestEngine
{
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.FlightMatrices;
    using CDP.AppDomain.Pilots;
    using CDP.AppDomain.Scoring;
    using CDP.AppDomain.Tasks;
    using CDP.AppDomain.Timer;
    using CDP.Common.Logging;
    using CDP.CoreApp.Features.Contests.Commands;
    using CDP.CoreApp.Features.FlightMatrices.Commands;
    using CDP.CoreApp.Features.FlightMatrices.Queries;
    using CDP.CoreApp.Features.Pilots.Queries;
    using CDP.CoreApp.Features.Scoring.Commands;
    using CDP.CoreApp.Features.Scoring.Queries;
    using CDP.CoreApp.Features.Tasks.Queries;
    using CDP.CoreApp.Interfaces.Contests;
    using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Main controller for an F3K contest.  Keeps state and moves the 
    /// contest through it's stages.
    /// </summary>
    /// <seealso cref="CDP.CoreApp.Features.ContestEngine.ContestEngineBase" />
    public class F3KContestEngine : ContestEngineBase
    {
        private Queue<FlightGroup> flightGroupQueue = new Queue<FlightGroup>();
        private Queue<TimeWindow> timeWindowsQueue = new Queue<TimeWindow>();
        private ILoggingService logger;
        private TaskQueryInteractor taskQueryInteractor;
        private PilotQueryInteractor pilotQueryInteractor;
        private ScoringQueryInteractor scoringQueryInteractor;
        private ScoringContestScoreAggInteractor scoringContestScoreAggIntr;
        private FlightMatrixQueryInteractor flightMatrixQueryIntr;
        private FlightMatrixStorageCmdInteractor flightMatrixStorageCmdIntr;
        private ISortingAlgo sortingAlgo;
        private IFlyOffSelectionAlgo flyOffPilotPicker;

        /// <summary>
        /// Initializes a new instance of the <see cref="F3KContestEngine" /> class.
        /// </summary>
        /// <param name="taskQueryInteractor">The task query interactor.</param>
        /// <param name="contestStorageCmdIntr">The contest storage command intr.</param>
        /// <param name="scoringStorageCmdIntr">The scoring storage command intr.</param>
        /// <param name="logger">The logger.</param>
        public F3KContestEngine(TaskQueryInteractor taskQueryInteractor,
            ContestStorageCmdInteractor contestStorageCmdIntr,
            ScoringStorageCmdInteractor scoringStorageCmdIntr,
            ScoringQueryInteractor scoringQueryInteractor,
            ScoringContestScoreAggInteractor scoringContestScoreAggIntr,
            FlightMatrixStorageCmdInteractor flightMatrixStorageCmdIntr,
            FlightMatrixQueryInteractor flightMatrixQueryIntr,
            PilotQueryInteractor pilotQueryInteractor,
            ISortingAlgo sortingAlgo,
            IFlyOffSelectionAlgo flyOffAlgo,
            ILoggingService logger) : base(contestStorageCmdIntr, scoringStorageCmdIntr)
        {
            this.taskQueryInteractor = taskQueryInteractor ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(taskQueryInteractor)} cannot be null");
            this.scoringQueryInteractor = scoringQueryInteractor ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(scoringQueryInteractor)} cannot be null");
            this.scoringContestScoreAggIntr = scoringContestScoreAggIntr ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(scoringContestScoreAggIntr)} cannot be null");
            this.flightMatrixQueryIntr = flightMatrixQueryIntr ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(flightMatrixQueryIntr)} cannot be null");
            this.flightMatrixStorageCmdIntr = flightMatrixStorageCmdIntr ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(flightMatrixStorageCmdIntr)} cannot be null");
            this.pilotQueryInteractor = pilotQueryInteractor ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(pilotQueryInteractor)} cannot be null");
            this.sortingAlgo = sortingAlgo ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(sortingAlgo)} cannot be null");
            this.flyOffPilotPicker = flyOffAlgo ?? throw new ArgumentException($"{nameof(F3KContestEngine)}:Ctor - {nameof(flyOffAlgo)} cannot be null");
            this.logger = logger ?? throw new ArgumentNullException($"{nameof(F3KContestEngine)}:Ctor - {nameof(logger)} cannot be null");
        }

        /// <summary>
        /// Initializes the engine using a specified contest.
        /// </summary>
        /// <param name="contest">The contest.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">contest</exception>
        public override async Task Initialize(Contest contest)
        {
            // Set the contest and initialize it's state if needed
            this.Contest = contest ?? throw new ArgumentNullException($"The {nameof(contest)} parameter cannot be null.");
            if (this.Contest.State == null) this.Contest.State = new ContestState();

            // Set the current round
            this.CurrentRoundOrdinal = this.Contest.State.CurrentRoundOrdinal;

            // Get all of the pilots for the loaded contest.
            var allPilotsResult = await this.pilotQueryInteractor.GetAllPilotsAsync();
            if (allPilotsResult.IsFaulted)
            {
                this.logger.LogTrace($"{nameof(F3KContestEngine)}:{nameof(Initialize)} - An error occured getting all the pilots.");
                return;
            }

            base.PilotsInContest = allPilotsResult.Value.Where(p => this.Contest.PilotRoster.Contains(p.Id)).ToList();

            // If this contest has not started, we need to load up the contest rounds with the appropriate pilots 
            // based on the flight matrix that was generated.
            if (!this.Contest.State.HasStarted)
            {
                this.InitializeContestRoundsFromFlightMatrix();
            }

            // Get the current task
            var taskResult = await this.taskQueryInteractor.GetTask(this.Contest.Rounds[this.CurrentRoundOrdinal].AssignedTaskId);
            if (taskResult.IsFaulted)
            {
                this.logger.LogTrace($"{nameof(F3KContestEngine)}:{nameof(Initialize)} - An error occured getting the task.");
                return;
            }

            this.CurrentTask = taskResult.Value;

            var flightMatrixResult = await this.flightMatrixQueryIntr.GetFlightMatrixForContest(this.Contest.Id);
            if (flightMatrixResult.IsFaulted)
            {
                this.logger.LogTrace($"{nameof(F3KContestEngine)}:{nameof(Initialize)} - An error occured getting the flight matrix.");
                return;
            }

            this.FlightMatrix = flightMatrixResult.Value;

            // Initialize the flight groups
            InitializeFlightGroupQueue(this.Contest.Rounds[this.CurrentRoundOrdinal], this.CurrentFlightGroup);
            ResetFlightWindows();
            this.GetNextTimeWindow();
        }

        /// <summary>
        /// Moves to next contest stage.
        /// </summary>
        /// <returns></returns>
        public override async Task MoveToNextContestStage()
        {
            if (this.Contest == null)
            {
                var ex = new Exception("The current App state is bad.  We need a current contest to perform these actions.");
                this.logger.LogException(ex);
                throw ex;
            }

            // If this is the last flight group, load the next round
            if (this.flightGroupQueue.Count < 1)
            {
                // Init the next round
                await InitializeNextRound();
                return;
            }

            // Set the flight group
            this.CurrentFlightGroup = this.flightGroupQueue.Dequeue();

            // Set the current Task
            var allTasksResult = await this.taskQueryInteractor.GetAllTasksAsync(ContestType.F3K);
            if (allTasksResult.IsFaulted)
            {
                this.logger.LogTrace($"{nameof(F3KContestEngine)}:{nameof(MoveToNextContestStage)} - An error occured getting all the tasks from the repository.");
                return;
            }

            this.CurrentTask = allTasksResult.Value
                .Where(t => t.Id == this.Contest.Rounds[this.CurrentRoundOrdinal].AssignedTaskId)
                .FirstOrDefault();

            // Build the flight windows
            ResetFlightWindows();

            // Update the contest object
            await this.contestStorageIntr.Value.UpdateContestAsync(this.Contest);
        }

        /// <summary>
        /// Gets the next time window.
        /// </summary>
        /// <returns></returns>
        public override TimeWindow GetNextTimeWindow()
        {
            this.CurrentTimeWindow = timeWindowsQueue.Count > 0
                ? timeWindowsQueue.Dequeue()
                : null;

            return this.CurrentTimeWindow;
        }

        /// <summary>
        /// Resets the flight windows.
        /// </summary>
        public override void ResetFlightWindows()
        {
            this.timeWindowsQueue = this.InitializeTimeWindowsQueue(this.CurrentTask);
        }

        /// <summary>
        /// Scores the flight group.
        /// </summary>
        /// <param name="timeSheets">The time sheets.</param>
        /// <returns></returns>
        public override async Task ScoreFlightGroup(IEnumerable<TimeSheet> timeSheets)
        {
            if (timeSheets == null)
            {
                logger.LogException(new ArgumentNullException($"The {nameof(timeSheets)} parameter cannot be null."));
            }

            // Saves the scores.  The interactor handles the validation as the Core App needs to own that business
            // logic.  For the UI to be responsive to user errors, it should probably handled higher up the stack...  TODO.
            var saveScoresResult = await this.scoringCmdIntr.Value.SaveRoundScoresAsync(timeSheets, this.Contest.Id);
            if (saveScoresResult.IsFaulted)
            {
                logger.LogException(new Exception($"Failed to same timesheets for round {timeSheets.First().RoundOrdinal}, flight group {timeSheets.First().FlightGroup}"));
            }
        }

        /// <summary>
        /// Initializes the next round.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InitializeNextRound()
        {
            // Check if we are finished w/ the contest
            if (!this.sortingAlgo.IsSingleRoundSort() && this.Contest.State.CurrentRoundOrdinal + 1 >= this.Contest.Rounds.Count)
            {
                return false;
            }

            // Get the next round, init the flight group queue.
            ++this.Contest.State.CurrentRoundOrdinal;

            

            var allTasksResult = await this.taskQueryInteractor.GetAllTasksAsync(ContestType.F3K);
            if (allTasksResult.IsFaulted)
            {
                this.logger.LogTrace($"{nameof(F3KContestEngine)}:{nameof(MoveToNextContestStage)} - An error occured getting all the tasks from the repository.");
                return false;
            }

            // If this is a fly-off round, handle it
            if (this.Contest.Rounds[this.Contest.State.CurrentRoundOrdinal].IsFlyOffRound)
            {
                await InitializeFlyOffRound();
            }
            // If this is a single round sort, we need to generate the next round for the application.
            else if (this.sortingAlgo.IsSingleRoundSort())
            {
                await PopulateTheNextContestRoundWithPilots();
            }

            this.CurrentTask = allTasksResult.Value
                .Where(t => t.Id == this.Contest.Rounds[this.CurrentRoundOrdinal].AssignedTaskId)
                .FirstOrDefault();

            // Initialize to the first flight group since we are starting a new round.
            InitializeFlightGroupQueue(this.Contest.Rounds[this.CurrentRoundOrdinal], FlightGroup.A);

            // Init the time windows for the contest clock.
            ResetFlightWindows();
            this.GetNextTimeWindow();

            // Update the contest object to save state.
            await this.contestStorageIntr.Value.UpdateContestAsync(this.Contest);
            return true;
        }

        /// <summary>
        /// Initializes the fly off round.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> InitializeFlyOffRound()
        {
            // Get all of the round scores
            var scoresResult = await this.scoringQueryInteractor.GetScoresForContest(this.Contest.Id);
            if (scoresResult.IsFaulted)
            {
                var ex = new Exception($"{nameof(F3KContestEngine)}:{nameof(MoveToNextContestStage)} - An error occured getting all the Tasks from the repository.");
                this.logger.LogException(ex);
                return false;
            }

            // Calculate the totals
            var scoredRoundsResult = this.scoringContestScoreAggIntr.GetAggregateRoundScoresForPilots(scoresResult.Value);
            if (scoredRoundsResult.IsFaulted)
            {
                var ex = new Exception($"{nameof(F3KContestEngine)}:{nameof(MoveToNextContestStage)} - An error occured calculating the contest scores.");
                this.logger.LogException(ex);
                return false;
            }

            // Generate the sequence for the next round.
            var newPilotSequence = this.flyOffPilotPicker.SortSingleRound(scoredRoundsResult.Value, 10);

            // Update the contest
            CreateNewRound(newPilotSequence);

            // Update the Flight Matrix, the UI is built partly on the flight 
            // Matrix, so it needs to be up to date
            await UpdateFlightMatrixWithNewRound(newPilotSequence);

            // Update the contest object
            await this.contestStorageIntr.Value.UpdateContestAsync(this.Contest);

            return true;
        }

        /// <summary>
        /// Initializes the contest rounds from flight matrix.
        /// </summary>
        /// <returns></returns>
        private bool InitializeContestRoundsFromFlightMatrix()
        {
            try
            {
                // Populate the rounds for the contest (ugh)
                foreach (var flightMatrixRound in base.FlightMatrix.Matrix)
                {
                    var pilotSlotsByFlightGroup = flightMatrixRound.PilotSlots.GroupBy(ps => ps.FlightGroup);

                    foreach (var flightGroup in pilotSlotsByFlightGroup)
                    {
                        foreach (var pilot in flightGroup)
                        {
                            if (!this.Contest.Rounds[flightMatrixRound.RoundOrdinal].FlightGroups.ContainsKey(flightGroup.Key))
                            {
                                this.Contest.Rounds[flightMatrixRound.RoundOrdinal].FlightGroups.Add(flightGroup.Key, new List<Pilot>());
                            }

                            this.Contest.Rounds[flightMatrixRound.RoundOrdinal].FlightGroups[flightGroup.Key].Add(PilotsInContest.Where(p => p.Id == pilot.PilotId).First());
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Populates the next contest round with pilots.
        /// </summary> 
        /// <returns></returns>
        private async Task<bool> PopulateTheNextContestRoundWithPilots()
        {
            // Get all of the round scores
            var scoresResult = await this.scoringQueryInteractor.GetScoresForContest(this.Contest.Id);
            if (scoresResult.IsFaulted)
            {
                var ex = new Exception($"{nameof(F3KContestEngine)}:{nameof(MoveToNextContestStage)} - An error occured getting all the tasks from the repository.");
                this.logger.LogException(ex);
                return false;
            }

            // Calculate the totals
            var scoredRoundsResult = this.scoringContestScoreAggIntr.GetAggregateRoundScoresForPilots(scoresResult.Value);
            if (scoredRoundsResult.IsFaulted)
            {
                var ex = new Exception($"{nameof(F3KContestEngine)}:{nameof(MoveToNextContestStage)} - An error occured calculating the contest scores.");
                this.logger.LogException(ex);
                return false;
            }

            // Generate the sequence for the next round.
            var newPilotSequence = this.sortingAlgo.SortSingleRound(scoredRoundsResult.Value, this.Contest.SuggestedNumberOfPilotsPerGroup);

            // Update the contest
            CreateNewRound(newPilotSequence);

            // Update the Flight Matrix, the UI is built partly on the flight 
            // Matrix, so it needs to be up to date
            await UpdateFlightMatrixWithNewRound(newPilotSequence);

            // Update the contest object
            await this.contestStorageIntr.Value.UpdateContestAsync(this.Contest);

            return true;
        }

        /// <summary>
        /// Updates the flight matrix.
        /// </summary>
        /// <param name="newPilotSequence">The new pilot sequence.</param>
        /// <returns></returns>
        private async Task UpdateFlightMatrixWithNewRound(Dictionary<string, FlightGroup> newPilotSequence)
        {
            // Get the flight matrix for this round.
            var flightMatrixResult = await this.flightMatrixQueryIntr.GetFlightMatrixForContest(this.Contest.Id);
            if (flightMatrixResult.IsFaulted)
            {
                this.logger.LogTrace($"{nameof(F3KContestEngine)}:{nameof(UpdateFlightMatrixWithNewRound)} - An error occured retrieving the flight matrix.");
                return;
            }

            // Insert the new round in the flight matrix.
            base.FlightMatrix = flightMatrixResult.Value;
            base.FlightMatrix.Matrix.Add(new FlightMatrixRound
            {
                RoundOrdinal = this.Contest.State.CurrentRoundOrdinal,
                PilotSlots = newPilotSequence.Select(nps => new FlightMatrixPilotSlot { FlightGroup = nps.Value, PilotId = nps.Key }).ToList()
            });

            // Save.
            var saveFlightMatrixResult = await this.flightMatrixStorageCmdIntr.UpdateFlightMatrixForContestAsync(base.FlightMatrix);
            if (flightMatrixResult.IsFaulted)
            {
                this.logger.LogTrace($"{nameof(F3KContestEngine)}:{nameof(UpdateFlightMatrixWithNewRound)} - An error occured updating the flight matrix.");
                return;
            }
        }

        /// <summary>
        /// Creates the new round.
        /// </summary>
        /// <param name="newPilotSequence">The new pilot sequence.</param>
        private void CreateNewRound(Dictionary<string, FlightGroup> newPilotSequence)
        {
            // Group the pilots by flight group
            var flightGroupings = newPilotSequence.GroupBy(nps => nps.Value);

            // Populate the round with them.
            foreach (var flightGroup in flightGroupings)
            {
                var pilots = flightGroup.Select(p => this.PilotsInContest.Where(pilot => pilot.Id == p.Key).Single()).ToList();
                this.Contest.Rounds[this.CurrentRoundOrdinal].FlightGroups.Add(flightGroup.Key, pilots);
            }
        }

        /// <summary>
        /// Initializes the flight group queue.
        /// </summary>
        /// <param name="currentRound">The current round.</param>
        /// <returns></returns>
        private void InitializeFlightGroupQueue(Round currentRound, FlightGroup currentFlightGroup)
        {
            if (currentRound == null) { return; }

            this.flightGroupQueue.Clear();

            // Get the remaining flight groups (any that haven't been scored) and sort them
            var fgroupsInRound = currentRound?.FlightGroups?.Where(fg => fg.Key >= currentFlightGroup);
            fgroupsInRound = fgroupsInRound.OrderBy(fg => fg.Key);

            // Populate the queue
            foreach (var flightGroup in fgroupsInRound)
            {
                this.flightGroupQueue.Enqueue(flightGroup.Key);
            }

            // Set the current flight group
            this.CurrentFlightGroup = this.flightGroupQueue.Dequeue();
        }

        /// <summary>
        /// Builds the time windows for flight group.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns> 
        private Queue<TimeWindow> InitializeTimeWindowsQueue(TaskBase task)
        {
            var returnVal = new Queue<TimeWindow>();
            //Practice Window
            returnVal.Enqueue(new TimeWindow
            {
                Name = "Flight Test Window",
                GateType = TimeGateType.Practice,
                DirectionOfCount = TimerDirection.CountDown,
                CountDownLast10Seconds = true,
                Time = new TimeSpan(0, 3, 0)
            });

            //Task Window (build from the task object)
            foreach(var timeWindow in task.TaskFlightWindows)
            {
                var name = string.Empty;
                switch (timeWindow.GateType)
                {
                    case TimeGateType.Break:
                        name = $"{timeWindow.Time.TotalMinutes} before a 3 minute task.";
                        break;
                    case TimeGateType.Landing:
                        name = $"{timeWindow.Time.TotalMinutes} minute landing Window.  All aircraft must be on the ground in {timeWindow.Time.TotalMinutes} minute";
                        break;
                    case TimeGateType.Task:
                        name = $"Task Window";
                        break;
                }

                returnVal.Enqueue(new TimeWindow
                {
                    Name =  name,
                    GateType = timeWindow.GateType,
                    DirectionOfCount = timeWindow.DirectionOfCount,
                    CountDownLast10Seconds = true,
                    Time = timeWindow.Time
                });
            }

            //Landing Window
            returnVal.Enqueue(new TimeWindow
            {
                Name = "Landing Window",
                GateType = TimeGateType.Landing,
                DirectionOfCount = TimerDirection.CountDown,
                CountDownLast10Seconds = true,
                Time = new TimeSpan(0, 1, 0)
            });

            return returnVal;
        }
    }
}