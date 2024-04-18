using DataAccess.ServiceRepository.IServiceRepository;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Viewmodels;

namespace HelloDoc.Controllers.Admin
{

    public class CredentialAdminController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IAdminCredential adminCredential;
        private readonly IJwtRepository _jwtRepository;
        private IAuthorizatoinRepository _authorizatoinRepository;
        private ISendEmailAndSMS sendEmailAndSMS;

        public CredentialAdminController(HelloDocDbContext context, IAdminCredential adminCredential, IJwtRepository jwtRepository, 
            IAuthorizatoinRepository authorizatoinRepository, ISendEmailAndSMS sendEmailAndSMS)
        {
            _context = context;
            this.adminCredential = adminCredential;
            _jwtRepository = jwtRepository;
            _authorizatoinRepository = authorizatoinRepository;
            this.sendEmailAndSMS = sendEmailAndSMS;
        }

        public ActionResult Admin()
        {
            return View();
        }

        public IActionResult AdminForgotPassword(Aspnetuser user)
        {
            return View(user);
        }

        //when mailed link opened
        public IActionResult ResetPassword(string id)
        {
            var aspuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == id);
            return View(aspuser);
        }

        public IActionResult ResetPasswordEmail(Aspnetuser user)
        {
            string Id = (_context.Aspnetusers.FirstOrDefault(x => x.Email == user.Email)).Id;
            string resetPasswordUrl = GenerateResetPasswordUrl(Id);

            sendEmailAndSMS.Sendemail(user.Email, "Reset Your Password", $"Hello, reset your password using this link: {resetPasswordUrl}");

            return RedirectToAction("Admin", "CredentialAdmin");
        }

        private string GenerateResetPasswordUrl(string userId)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string resetPasswordPath = Url.Action("ResetPassword", "CredentialAdmin", new { id = userId });
            return baseUrl + resetPasswordPath;
        }
        //when mailed link opened and submit for reset password
        [HttpPost]
        public IActionResult ResetPasswordBtn(Aspnetuser aspnetuser)
        {
            var aspuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspnetuser.Id);
            aspuser.Passwordhash = aspnetuser.Passwordhash;
            _context.Aspnetusers.Update(aspuser);
            _context.SaveChanges();
            return RedirectToAction("Admin", "CredentialAdmin");
        }


        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            HttpContext.Session.Clear();
            return RedirectToAction("Admin", "CredentialAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> Login(Aspnetuser user)
        {

            int valid = adminCredential.Login(user);

            if (user.Email == null)
            {
                TempData["Email"] = "Please Enter email!";
                TempData["EmailStyle"] = "border-danger";
                return RedirectToAction("Admin", "CredentialAdmin");
            }
            else if (valid == 2)
            {
                TempData["WrongPassword"] = "Enter Correct Password";
                TempData["PassStyle"] = "border-danger";
                return RedirectToAction("Admin", "CredentialAdmin");
            }
            else if (valid == 4)
            {
                TempData["Email"] = "Enter correct email!";
                TempData["EmailStyle"] = "border-danger";
                return RedirectToAction("Admin", "CredentialAdmin");
            }
            else if (valid == 5)
            {
                var physician = _context.Physicians.FirstOrDefault(m => m.Email == user.Email);
                if (physician != null)
                {
                    HttpContext.Session.SetString("AdminName", $"{physician.Firstname}{physician.Lastname}");
                    HttpContext.Session.SetInt32("PhysicianId", physician.Physicianid);
                }

                var correct = _context.Aspnetusers.FirstOrDefault(m => m.Email == user.Email);
                LoggedInPersonViewModel loggedInPersonViewModel = new LoggedInPersonViewModel();
                loggedInPersonViewModel.AspnetId = correct.Id;
                loggedInPersonViewModel.UserName = correct.Username;
                var Roleid = _context.Aspnetuserroles.FirstOrDefault(x => x.Userid == correct.Id).Roleid.ToString();
                loggedInPersonViewModel.Role = _context.Aspnetroles.FirstOrDefault(x => x.Id == Roleid).Name;
                Response.Cookies.Append("jwt", _jwtRepository.GenerateJwtToken(loggedInPersonViewModel));
                TempData["success"] = "Login SuccessFull...";
                return RedirectToAction("ProviderDashboard", "ProviderSide");
            }
            else
            {
                var admin = _context.Admins.FirstOrDefault(m => m.Email == user.Email);
                if (admin != null)
                {
                    HttpContext.Session.SetInt32("AdminId", admin.Adminid);

                    HttpContext.Session.SetString("AdminName", $"{admin.Firstname}{admin.Lastname}");
                }

                var correct = _context.Aspnetusers.FirstOrDefault(m => m.Email == user.Email);
                LoggedInPersonViewModel loggedInPersonViewModel = new LoggedInPersonViewModel();
                loggedInPersonViewModel.AspnetId = correct.Id;
                loggedInPersonViewModel.UserName = correct.Username;
                var Roleid = _context.Aspnetuserroles.FirstOrDefault(x => x.Userid == correct.Id).Roleid.ToString();
                loggedInPersonViewModel.Role = _context.Aspnetroles.FirstOrDefault(x => x.Id == Roleid).Name;


                Response.Cookies.Append("jwt", _jwtRepository.GenerateJwtToken(loggedInPersonViewModel));
                TempData["success"] = "Login SuccessFull...";
                return RedirectToAction("Admindashboard", "Admin");
            }
        }

        [HttpPost]
        public IActionResult ResetPasswordProfileAdmin(string password, string email)
        {
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Email == email);

            if (aspnetuser != null)
            {
                aspnetuser.Passwordhash = password;
                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
            }
            TempData["success"] = "Password changed successfully";
            return RedirectToAction("AdminProfile", "Admin");
        }
    }
}
