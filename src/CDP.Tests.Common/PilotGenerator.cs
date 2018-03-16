using CDP.AppDomain.Pilots;
using System;
using System.Collections.Generic;

namespace CDP.Tests.Common
{
    public static class PilotGenerator
    {
        /// <summary>
        /// Gets the basic pilot list.
        /// </summary>
        /// <param name="numberOfPilotsToCreate">The number of pilots to create.</param>
        /// <returns></returns>
        public static List<Pilot> GetBasicPilotList(int numberOfPilotsToCreate)
        {
            var result = new List<Pilot>();

            for(var i = 0; i < numberOfPilotsToCreate; ++i)
            {
                result.Add(new Pilot($"FirstName{i}", $"LastName{i}", $"{i}", $"ama2342{i}", string.Empty));
            }

            return result;
        }
    }
}
