using HelloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HelloDoc.Controllers
{
    public class submitrequestforms : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public submitrequestforms(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult patient()
        {
            return View();
        }

        public IActionResult familyfriend()
        {
            return View();
        }

        public IActionResult concierge()
        {
            return View();
        }

        public IActionResult businesspartner()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
