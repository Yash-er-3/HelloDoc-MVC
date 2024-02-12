using HelloDoc.DataContext;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoc.Controllers
{
    public class PatientController:Controller
    {
      


        public IActionResult PatientDashboard()
        {
            return View();
        }
    }
}
