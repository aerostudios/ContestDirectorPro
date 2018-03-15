//---------------------------------------------------------------
// Date: 2/12/2018
// Rights: 
// FileName: TaskRepository.cs
//---------------------------------------------------------------

namespace CDP.Repository.WindowsStorage
{
    using CDP.AppDomain;
    using CDP.AppDomain.Contests;
    using CDP.AppDomain.Tasks;
    using CDP.AppDomain.Tasks.F3K;
    using CDP.CoreApp.Interfaces.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository that returns contest tasks
    /// </summary>
    /// <seealso cref="CDP.CoreApp.Interfaces.Tasks.ITaskRepository" />
    public class TaskRepository : ITaskRepository
    {
        /// <summary>
        /// The internal cache
        /// </summary>
        private Dictionary<string, TaskBase> internalCache = new Dictionary<string, TaskBase>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRepository" /> class.
        /// </summary>
        public TaskRepository()
        {
            var taskA = new TaskA_LastFlightSevenMin();
            var taskB = new TaskB_LastTwoFlights4MinMax();
            var taskC = new TaskC_AllUpLastDown(3);
            var taskC4 = new TaskC_AllUpLastDown(4);
            var taskC5 = new TaskC_AllUpLastDown(5);
            var taskD = new TaskD_Ladder();
            var taskE = new TaskE_Poker();
            var taskF = new TaskF_BestThreeOutOfSix();
            var taskG = new TaskG_FiveTwos();
            var taskH = new TaskH_FourThreeTwoOne();
            var taskI = new TaskI_ThreeThreeTwenties();
            var taskJ = new TaskJ_LastThree();
            var taskK = new TaskK_BigLadder();

            this.internalCache.Add(taskA.Id, taskA);
            this.internalCache.Add(taskB.Id, taskB);
            this.internalCache.Add(taskC.Id, taskC);
            this.internalCache.Add(taskC4.Id, taskC4);
            this.internalCache.Add(taskC5.Id, taskC5);
            this.internalCache.Add(taskD.Id, taskD);
            this.internalCache.Add(taskE.Id, taskE);
            this.internalCache.Add(taskF.Id, taskF);
            this.internalCache.Add(taskG.Id, taskG);
            this.internalCache.Add(taskH.Id, taskH);
            this.internalCache.Add(taskI.Id, taskI);
            this.internalCache.Add(taskJ.Id, taskJ);
            this.internalCache.Add(taskK.Id, taskK);
        }

        /// <summary>
        /// Gets all contests.
        /// </summary>
        /// <param name="contestType"></param>
        /// <returns></returns>
        public Task<Result<IEnumerable<TaskBase>>> ReadAsync(ContestType contestType)
        {
            var allTasks = this.internalCache.Values.Where(value => value.Type == contestType);
            return Task.FromResult(new Result<IEnumerable<TaskBase>>(allTasks));
        }

        /// <summary>
        /// Reads the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task<Result<TaskBase>> ReadAsync(string id)
        {
            var task = this.internalCache[id];
            return Task.FromResult(new Result<TaskBase>(task));
        }
    }
}
