using DataAccess.ServiceRepository;
using DataAccess.ServiceRepository.IServiceRepository;
using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using Services.Viewmodels;
using System.Net;

namespace HelloDoc.Controllers.Admin
{

    public class CredentialAdminController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IAdminCredential adminCredential;
        private readonly IJwtRepository _jwtRepository;
        private IAuthorizatoinRepository _authorizatoinRepository;

        public CredentialAdminController(HelloDocDbContext context, IAdminCredential adminCredential, IJwtRepository jwtRepository, IAuthorizatoinRepository authorizatoinRepository)
        {
            _context = context;
            this.adminCredential = adminCredential;
            _jwtRepository = jwtRepository;
            _authorizatoinRepository = authorizatoinRepository;
            
        }

        public IActionResult Logout() {

            //Response.Cookies.Delete("jwt");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Aspnetuser user)
        {
            int valid =  adminCredential.Login(user);
            var correct = _context.Aspnetusers.FirstOrDefault(m => m.Email == user.Email);
            LoggedInPersonViewModel loggedInPersonViewModel = new LoggedInPersonViewModel();
            loggedInPersonViewModel.AspnetId = correct.Id;
            loggedInPersonViewModel.UserName = correct.Username;
            var Roleid = _context.Aspnetuserroles.FirstOrDefault(x => x.Userid == correct.Id).Roleid.ToString();
            loggedInPersonViewModel.Role = _context.Aspnetroles.FirstOrDefault(x => x.Id == Roleid).Name;
            Response.Cookies.Append("jwt", _jwtRepository.GenerateJwtToken(loggedInPersonViewModel));

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
