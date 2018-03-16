using CDP.AppDomain.Tasks;
using CDP.AppDomain.Tasks.F3K;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDP.Tests.Common
{
    public class TaskGenerator
    {
        private Dictionary<int, TaskBase> tasks = new Dictionary<int, TaskBase>();
        private Random randomNumberGen = new Random();
        
        public TaskGenerator()
        {
            tasks = new Dictionary<int, TaskBase>
            {
                { 0, new TaskA_LastFlightSevenMin() },
                { 1, new TaskB_LastTwoFlights4MinMax() },
                { 2, new TaskC_AllUpLastDown(3) },
                { 3, new TaskC_AllUpLastDown(4) },
                { 4, new TaskD_Ladder() },
                { 5, new TaskE_Poker() },
                { 6, new TaskF_BestThreeOutOfSix() },
                { 7, new TaskG_FiveTwos() },
                { 8, new TaskH_FourThreeTwoOne() },
                { 9, new TaskI_ThreeThreeTwenties() },
                { 10, new TaskJ_LastThree() },
            };
        }

        public List<TaskBase> GetAllTasksAsList() => tasks.Select(t => t.Value).ToList();

        public Dictionary<int, TaskBase> GetAllTasks() => tasks;

        public TaskBase GetRandomTask() => tasks[randomNumberGen.Next(0, 10)];
    }
}
