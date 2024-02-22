using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace HelloDoc.Controllers.Admin
{
    public class CredentialAdminController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IAdminCredential adminCredential;

        public CredentialAdminController(HelloDocDbContext context, IAdminCredential adminCredential) { 
            _context = context;
            this.adminCredential = adminCredential;
        }
        [HttpPost]
        public async Task<IActionResult> Login(Aspnetuser user)
        {
            int valid =  adminCredential.Login(user);

            if (valid == 2)
            {
                TempData["WrongPassword"] = "Enter Correct Password";
                TempData["PassStyle"] = "border-danger";
                return RedirectToAction("Admin", "Admin");
            }
            else if (valid == 3)
            {
                TempData["WrongPassword"] = "You're not Admin!";
                TempData["PassStyle"] = "border-danger";
                TempData["EmailStyle"] = "border-danger";
                return RedirectToAction("Admin", "Admin");

            }
            else if (valid == 4)
            {
                TempData["Email"] = "Enter correct email!";
                TempData["EmailStyle"] = "border-danger";
                return RedirectToAction("Admin", "Admin");
            }
            else
            {
                TempData["success"] = "Login SuccessFull...";
               return RedirectToAction("Admindashboard", "Admin");
            }
        }
    }
}
