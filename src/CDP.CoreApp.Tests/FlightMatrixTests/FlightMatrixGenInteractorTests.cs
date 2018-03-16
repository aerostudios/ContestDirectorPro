using CDP.AppDomain.FlightMatrices;
using CDP.AppDomain.Registration;
using CDP.Common.Logging;
using CDP.CoreApp.Features.FlightMatrices.Commands;
using CDP.CoreApp.Interfaces.FlightMatrices.SortingAlgos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace CDP.CoreApp.Tests.FlightMatrixTests
{
    [TestClass]
    public class FlightMatrixGenInteractorTests
    {
        private Mock<ISortingAlgo> mockSortingAlgo;
        private Mock<ILoggingService> mockLogger;

        [TestInitialize]
        public void Setup()
        {
            this.mockSortingAlgo = new Mock<ISortingAlgo>();
            this.mockLogger = new Mock<ILoggingService>();
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_NullPilotRegistrationParameter()
        {
            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(null, 1, 7);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_EmptyPilotRegistrationParameter()
        {
            var pilotRegistrations = new List<PilotRegistration>();
            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, 1, 7);
             
            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_NegativeRoundsParameter()
        {
            var pilotRegistrations = GenerateValidPilotRegistration(10);
            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, -1, 7);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_ZeroRoundsParameter()
        {
            var pilotRegistrations = GenerateValidPilotRegistration(10);
            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, 0, 7);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_NegativeSuggestionParameter()
        {
            var pilotRegistrations = GenerateValidPilotRegistration(10);
            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, 1, -7);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_ZeroSuggestionParameter()
        {
            var pilotRegistrations = GenerateValidPilotRegistration(10);
            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, 1, 0);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_SortingAlgo_Exception()
        {
            var pilotRegistrations = GenerateValidPilotRegistration(10);

            this.mockSortingAlgo.Setup(sa => sa.GenerateInitialMatrix(It.IsAny<IEnumerable<PilotRegistration>>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new ArgumentException("test"));

            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, 1, 7);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsInstanceOfType(result.Error.Exception, typeof(ArgumentException));
            Assert.IsNotNull(result.Error.ErrorMessage);
            Assert.AreEqual("test", result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_SortingAlgo_ReturnsNull()
        {
            var pilotRegistrations = GenerateValidPilotRegistration(10);

            this.mockSortingAlgo.Setup(sa => sa.GenerateInitialMatrix(It.IsAny<IEnumerable<PilotRegistration>>(), It.IsAny<int>(), It.IsAny<int>())).Returns<FlightMatrix>(null);

            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, 1, 7);

            Assert.IsTrue(result.IsFaulted);
            Assert.IsNotNull(result.Error);
            Assert.IsNotNull(result.Error.ErrorMessage);
        }

        [TestMethod]
        public void FlightMatrixGenerationInteractor_CreateSortedFlightMatrix_SortingAlgo_HappyPath()
        {
            var pilotRegistrations = GenerateValidPilotRegistration(10);

            var flightMatrix = new FlightMatrix
            {
                ContestId = "sdfasdf"
            };

            flightMatrix.Matrix.Add(new FlightMatrixRound
            {
                RoundOrdinal = 0,
                PilotSlots = new List<FlightMatrixPilotSlot>
                {
                    new FlightMatrixPilotSlot
                    {
                        PilotId = "sadfxcvcxasdf",
                        FlightGroup = FlightGroup.A
                    }
                }
            });

            this.mockSortingAlgo.Setup(sa => sa.GenerateInitialMatrix(It.IsAny<IEnumerable<PilotRegistration>>(), It.IsAny<int>(), It.IsAny<int>())).Returns(flightMatrix);

            var fmgi = new FlightMatrixGenInteractor(mockSortingAlgo.Object, mockLogger.Object);
            var result = fmgi.CreateSortedFlightMatrix(pilotRegistrations, 1, 7);

            Assert.IsFalse(result.IsFaulted);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Value);
        }

        private IEnumerable<PilotRegistration> GenerateValidPilotRegistration(int numberToCreate)
        {
            var listToCreate = new List<PilotRegistration>();
            var rnd = new Random();

            for (var i = 0; i < numberToCreate; ++i)
            {
                listToCreate.Add(new PilotRegistration
                {
                    AirframeRegistrationNumbers = new List<string> { rnd.Next(1, 100).ToString(), rnd.Next(1, 100).ToString() },
                    AirframesSignedOff = true,
                    ContestId = "34234",
                    Id = string.Empty,
                    IsPaid = true,
                    PilotId = rnd.Next(1, 100).ToString()
                });
            }

            return listToCreate;
        }
    }
}
