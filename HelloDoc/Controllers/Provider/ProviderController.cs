using Microsoft.AspNetCore.Mvc;

namespace HelloDoc.Controllers.Provider
{
    public class ProviderController : Controller
    {
        [HttpGet]
        public IActionResult ProviderMenu()
        {
            return View();
        }
    }
}
