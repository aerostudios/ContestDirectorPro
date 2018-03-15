using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Registration;
using CDP.ScoringAndSortingImpl.F3K.Sorting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDP.ScoringAndSortingImpl.F3K.Tests
{
    [TestClass]
    public class RandomSortNoTeamProtectionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RandomSortNTP_Sort_NullPilotListParameter()
        {
            var randSort = new RandomSortNoTeamProtection();
            randSort.GenerateInitialMatrix(null, 10, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RandomSortNTP_Sort_BadPilotRegistrationParameter()
        {
            var randSort = new RandomSortNoTeamProtection();
            randSort.GenerateInitialMatrix(new List<PilotRegistration>(), 0, 5);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RandomSortNTP_Sort_BadRoundsParameter()
        {
            var randSort = new RandomSortNoTeamProtection();
            randSort.GenerateInitialMatrix(GeneratePilotsList(1), 0, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RandomSortNTP_Sort_NegativeRoundsParameter()
        {
            var randSort = new RandomSortNoTeamProtection();
            randSort.GenerateInitialMatrix(GeneratePilotsList(1), -1, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RandomSortNTP_Sort_BadSuggestedPilotsPerRoundParameter()
        {
            var randSort = new RandomSortNoTeamProtection();
            randSort.GenerateInitialMatrix(GeneratePilotsList(1), 10, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RandomSortNTP_Sort_NegativeSuggestedPilotsPerRoundParameter()
        {
            var randSort = new RandomSortNoTeamProtection();
            randSort.GenerateInitialMatrix(GeneratePilotsList(1), 10, -1);
        }

        [TestMethod]
        public void RandomSortNTP_Sort_HappyPath()
        {
            var pilots = GeneratePilotsList(10);
            var sortAlgo = new RandomSortNoTeamProtection();
            var sorted = sortAlgo.GenerateInitialMatrix(pilots, 10, 10);

            Assert.AreEqual(10, sorted.Matrix.Count);
            Assert.AreEqual(10, sorted.Matrix[0].PilotSlots.Count);
        }

        [TestMethod]
        public void RandomSortNTP_Sort_PilotsAndSuggestedPilotsPerRoundOutOfSync()
        {
            var pilots = GeneratePilotsList(7);
            var sortAlgo = new RandomSortNoTeamProtection();
            var numberOfRounds = 1;
            var suggestedPilotsPerGroup = 3;

            var sorted = sortAlgo.GenerateInitialMatrix(pilots, numberOfRounds, suggestedPilotsPerGroup);

            Assert.AreEqual(1, sorted.Matrix.Count);
            Assert.AreEqual(7, sorted.Matrix[0].PilotSlots.Count);

            var pilotsInFirstGroup = sorted.Matrix[0].PilotSlots.Count(p => p.FlightGroup == FlightGroup.A);
            var pilotsInSecondGroup = sorted.Matrix[0].PilotSlots.Count(p => p.FlightGroup == FlightGroup.B);

            // There should be two flight groups.
            // The first should have the suggested number of pilots.
            Assert.AreEqual(suggestedPilotsPerGroup, pilotsInFirstGroup);

            // Since there are seven pilots, there is 1 remaining pilot.  The rule for this
            // sorting algo states that when generating a matrix, if the remainder is less than 
            // half of the number of pilots suggested per group, they should be added to the 
            // final group and a new group should not be created.
            Assert.AreEqual(suggestedPilotsPerGroup + 1, pilotsInSecondGroup);
        }

        [TestMethod]
        public void RandomSortNTP_Sort_PilotsAndSuggestedPilotsPerRoundOutOfSyncCreateNewGroup()
        {
            var pilots = GeneratePilotsList(19);
            var sortAlgo = new RandomSortNoTeamProtection();
            var numberOfRounds = 1;
            var suggestedPilotsPerGroup = 5;

            var sorted = sortAlgo.GenerateInitialMatrix(pilots, numberOfRounds, suggestedPilotsPerGroup);

            // One Round
            Assert.AreEqual(1, sorted.Matrix.Count);
            // 19 Pilots
            Assert.AreEqual(19, sorted.Matrix[0].PilotSlots.Count);

            var pilotsInFirstGroup = sorted.Matrix[0].PilotSlots.Count(p => p.FlightGroup == FlightGroup.A);
            var pilotsInSecondGroup = sorted.Matrix[0].PilotSlots.Count(p => p.FlightGroup == FlightGroup.B);
            var pilotsInThirdGroup = sorted.Matrix[0].PilotSlots.Count(p => p.FlightGroup == FlightGroup.C);
            var pilotsInFourthGroup = sorted.Matrix[0].PilotSlots.Count(p => p.FlightGroup == FlightGroup.D);

            // There should be four flight groups, 5 in 3 (15 pilots) and the remaining 4 in a final group (19 total).
            // The first should have the suggested number of pilots.
            Assert.AreEqual(suggestedPilotsPerGroup, pilotsInFirstGroup);
            Assert.AreEqual(suggestedPilotsPerGroup, pilotsInSecondGroup);
            Assert.AreEqual(suggestedPilotsPerGroup, pilotsInThirdGroup);
            // Since there are 19, the remainder should be added to a new group and not included in the previous.
            Assert.AreEqual(suggestedPilotsPerGroup - 1, pilotsInFourthGroup);
        }

        /// <summary>
        /// Generates a pilots list with randomish values.
        /// </summary>
        /// <param name="numberOfPilotsToCreate">The number of pilots to create.</param>
        /// <returns></returns>
        private List<PilotRegistration> GeneratePilotsList(int numberOfPilotsToCreate)
        {
            var pilotsList = new List<PilotRegistration>();
            var rndNumber = new Random();
            var contestId = "TestcontestId1";

            for(int i = 0; i < numberOfPilotsToCreate; ++i)
            {
                pilotsList.Add(new PilotRegistration
                {
                    AirframeRegistrationNumbers = new List<string>
                    {
                        $"{rndNumber.Next().ToString()}test"
                    },
                    AirframesSignedOff = true,
                    ContestId = contestId,
                    IsPaid = false,
                    Id = rndNumber.Next().ToString(),
                    PilotId = rndNumber.Next().ToString()
                });
            }

            return pilotsList;
        }
    }
}
