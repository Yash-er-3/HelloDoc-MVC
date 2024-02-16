using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HelloDoc.Controllers
{
    public class CredentialController : Controller
    {
        private readonly HelloDocDbContext _context;

        public CredentialController(HelloDocDbContext context)
        {
            _context = context;
        }

        [HttpPost]

        public async Task<IActionResult> Login(Aspnetuser user)
        {
            try
            {
                var match = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == user.Email);
                var users = await _context.Users.FirstOrDefaultAsync(m => m.Email == user.Email);
                if(match.Passwordhash == user.Passwordhash) {
                    @TempData["msg"] = "<script>alert('Change succesfully');</script>";
                    TempData["info"] = "Login Successfull";
                    HttpContext.Session.SetInt32("UserId", users.Userid);
                    return RedirectToAction("PatientDashboard", "Patient");
                }
                else
                {
                    TempData["style"] = "text-danger";
                    TempData["password"] = "Enter valid password";
                    return RedirectToAction("registeredpatient", "Home");
                }


            }
            catch (Exception e)
            {
                TempData["style"] = "text-danger";
                TempData["email"] = "Enter valid email";
                //TempData["error"] = "Enter Valid Email and Password";

                return RedirectToAction("registeredpatient", "Home");
            }
        }
    }
}
