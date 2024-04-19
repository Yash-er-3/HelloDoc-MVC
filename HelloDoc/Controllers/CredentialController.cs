using DataAccess.ServiceRepository.IServiceRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HelloDoc.Controllers
{
    public class CredentialController : Controller
    {
        private readonly HelloDocDbContext _context;
        private IAuthorizatoinRepository _authorizatoinRepository;

        public CredentialController(HelloDocDbContext context, IAuthorizatoinRepository authorizatoinRepository)
        {
            _context = context;
            _authorizatoinRepository = authorizatoinRepository;
        }

        [HttpPost]

        public async Task<IActionResult> Login(Aspnetuser user)
        {
            try
            {
                var match = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == user.Email);
                var users = await _context.Users.FirstOrDefaultAsync(m => m.Email == user.Email);

                if(users == null)
                {
                    TempData["email"] = "you are not an user!";
                    return RedirectToAction("registeredpatient", "Home");
                }

                if (match == null)
                {
                    TempData["email"] = "email doesn't exist";
                    return RedirectToAction("registeredpatient", "Home");

                }

                if (match.Passwordhash == user.Passwordhash)
                {
                    @TempData["msg"] = "<script>alert('Change succesfully');</script>";
                    TempData["success"] = "Login Successfull";
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
                

                return RedirectToAction("registeredpatient", "Home");
            }
        }
    }
}
