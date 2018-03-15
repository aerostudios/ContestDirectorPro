//---------------------------------------------------------------
// Date: 03/01/2018
// Rights: 
// FileName: ContestHostApis.cs
//---------------------------------------------------------------

namespace CDP.UWP.Config
{
    /// <summary>
    /// Route segements for the server API's
    /// </summary>
    internal static class ContestHostApis
    {
        /// <summary>
        /// Gets the contests route.
        /// </summary>
        /// <value>
        /// The contests route.
        /// </value>
        public static string ContestsRoute { get; } = "api/contest";

        /// <summary>
        /// Gets the flight matrix route.
        /// </summary>
        /// <value>
        /// The flight matrix route.
        /// </value>
        public static string FlightMatrixRoute { get; } = "api/FlightMatrix";
    }
}
