using CDP.AppDomain.Contests;
using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Pilots;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDP.Tests.Common
{
    public static class RoundsGenerator
    {
        private static TaskGenerator taskGenerator = new TaskGenerator();

        public static Dictionary<int, Round> GenerateRounds_NoFlightGroups(int numberOfRoundsToCreate)
        {
            var result = new Dictionary<int, Round>();

            for(int i = 0; i < numberOfRoundsToCreate; ++i)
            {
                result.Add(i, new Round
                {
                    AssignedTaskId = taskGenerator.GetRandomTask().Id,
                    IsFlyOffRound = false
                });
            }

            return result;
        }
    }
}
