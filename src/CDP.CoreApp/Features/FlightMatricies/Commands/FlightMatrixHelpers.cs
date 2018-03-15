//---------------------------------------------------------------
// Date: 12/10/2017
// Rights: 
// FileName: FlightMatrixHelpers.cs
//---------------------------------------------------------------

namespace CDP.CoreApp.Features.FlightMatrices.Commands
{
    using CDP.AppDomain.Registration;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines some validation helpers for the FlightMatrix interactors
    /// </summary>
    internal static class FlightMatrixHelpers
    {
        /// <summary>
        /// Validates the pilot registration list.
        /// </summary>
        /// <param name="pilots">The pilots.</param>
        /// <returns></returns>
        public static ValidationResult ValidatePilotRegistrations(IEnumerable<PilotRegistration> pilots)
        {
            if (pilots == null)
            {
                return new ValidationResult(false, "The pilot registrations collection submitted cannot be null.");
            }

            if(pilots.Count() <= 0)
            {
                return new ValidationResult(false, "The pilot registrations submitted are out of range.  Must be greater than 0.");
            }
            
            // Make sure all of the contest Id's are the same
            if (pilots.Any(p => p.ContestId != pilots.First().ContestId))
            {
                return new ValidationResult(false, "One of the pilot registrations submitted has a different contest id.");
            }
            
            return new ValidationResult(true);
        }

        /// <summary>
        /// Validates the pilot registration.
        /// </summary>
        /// <param name="pilotRegistration">The pilot registration.</param>
        /// <returns></returns>
        public static ValidationResult ValidatePilotRegistration(PilotRegistration pilotRegistration)
        {
            if (pilotRegistration == null)
            {
                return new ValidationResult(false, "The pilot registration submitted cannot be null");
            }

            if (string.IsNullOrEmpty(pilotRegistration.PilotId))
            {
                return new ValidationResult(false, "The pilot registration's pilot id submitted cannot be null or empty");
            }

            if (string.IsNullOrEmpty(pilotRegistration.ContestId))
            {
                return new ValidationResult(false, "The pilot registration's contest id submitted cannot be null or empty");
            }

            return new ValidationResult(true);
        }
    }
}
