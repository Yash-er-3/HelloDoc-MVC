using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.Controllers.Admin
{
    public class CredentialAdminController : Controller
    {
        public readonly HelloDocDbContext _context;

        public CredentialAdminController(HelloDocDbContext context) { 
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Login(Aspnetuser user)
        {
            var correct = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == user.Email);

            if(correct != null)
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Aspnetuserid == correct.Id);

                if(admin != null)
                {
                    if(user.Passwordhash == correct.Passwordhash)
                    {
                        int id = admin.Adminid;
                        HttpContext.Session.SetInt32("AdminId", id);
                        HttpContext.Session.SetString("UserName", admin.Firstname + admin.Lastname);
                        return RedirectToAction("Admindashboard", "Admin");
                    }
                    TempData["WrongPassword"] = "Enter Correct Password";
                    TempData["PassStyle"] = "border-danger";
                    return RedirectToAction("Admin", "Admin");
                }
                else
                {
                    TempData["WrongPassword"] = "You're not Admin!";
                    TempData["PassStyle"] = "border-danger";
                    TempData["EmailStyle"] = "border-danger";
                    return RedirectToAction("Admin", "Admin");

                }
            }
            else
            {
                TempData["Email"] = "Enter correct email!";
                TempData["EmailStyle"] = "border-danger";
                return RedirectToAction("Admin", "Admin");
            }
        }
    }
}
