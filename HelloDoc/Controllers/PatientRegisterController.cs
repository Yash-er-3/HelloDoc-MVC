using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.Controllers
{
    public class PatientRegisterController : Controller
    {

        private readonly HelloDocDbContext _log;

        public PatientRegisterController(HelloDocDbContext log)
        {
            _log = log;
        }

        public async Task<IActionResult> CreatePatient(Aspnetuser user)
        {
            
        }
    }
}
