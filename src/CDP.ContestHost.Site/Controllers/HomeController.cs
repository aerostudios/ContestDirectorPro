//---------------------------------------------------------------
// Date: 1/1/2018
// Rights: 
// FileName: HomeController.cs
//---------------------------------------------------------------

namespace CDP.ContestHost.Site.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    /// <summary>
    /// The controller for the home actions.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class HomeController : Controller
    {
        /// <summary>
        /// Main entry point for the SPA.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Error handler for the site.
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
