using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoc.Controllers.Admin
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public ActionResult Admin()
        {
            return View();
        }

        public IActionResult Admindashboard()
        {
            return View();
        }

        public IActionResult header_dashboard()
        {
            return View();
        }
    }
}
