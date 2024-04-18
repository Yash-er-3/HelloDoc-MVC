using HelloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Implementation;
using Services.Viewmodels;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace HelloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HelloDocDbContext _context;
        private readonly IAddOrUpdateRequestStatusLog _addOrUpdateRequestStatusLog;
        public HomeController(ILogger<HomeController> logger, HelloDocDbContext context, IAddOrUpdateRequestStatusLog addOrUpdateRequestStatusLog)
        {
            _logger = logger;
            _context = context;
            _addOrUpdateRequestStatusLog = addOrUpdateRequestStatusLog;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult registeredpatient()
        {
            return View();
        }

        public IActionResult submitrequest()
        {
            return View();
        }

        public IActionResult forgotpassword(Aspnetuser user)
        {
            return View(user);
        }

        public IActionResult PatientResetPasswordEmail(Aspnetuser user)
        {
            string Id = (_context.Aspnetusers.FirstOrDefault(x => x.Email == user.Email)).Id;
            string resetPasswordUrl = GenerateResetPasswordUrl(Id);

            SendEmail(user.Email, "Reset Your Password", $"Hello, reset your password using this link: {resetPasswordUrl}");

            return RedirectToAction("registeredpatient", "Home");
        }

        private string GenerateResetPasswordUrl(string userId)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string resetPasswordPath = Url.Action("resetpassword", "Home", new { id = userId });
            return baseUrl + resetPasswordPath;
        }


        private Task SendEmail(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.yashsarvaiya@outlook.com";
            var password = "Yash@1234";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }

        public IActionResult resetpassword(string id)
        {
            var aspuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == id);
            return View(aspuser);
        }

        [HttpPost]
        public IActionResult resetpassword(Aspnetuser aspnetuser)
        {
            var aspuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == aspnetuser.Id);
            aspuser.Passwordhash = aspnetuser.Passwordhash;
            _context.Aspnetusers.Update(aspuser);
            _context.SaveChanges();
            return RedirectToAction("registeredpatient", "Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //review agrreement
        public IActionResult ReviewAgreement(string id)
        {
            var requestid = int.Parse(EncryptDecrypt.Decrypt(id));
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            var model = new AdminDashboardViewModel
            {
                requestid = requestid,
                PatientNameForAgreement = request.Firstname + " " + request.Lastname
            };
            return View(model);
        }

        public IActionResult IAgreeSendAgreement(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            request.Status = 4;
            _context.Requests.Update(request);
            _context.SaveChanges();
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(requestid);
            return RedirectToAction("PatientDashboard", "Patient");
        }

        [HttpPost]
        public IActionResult cancelCaseModal(int id, AdminDashboardViewModel note)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == id);
            request.Status = 3;
            _context.Requests.Update(request);
            _context.SaveChanges();
            var adminid = HttpContext.Session.GetInt32("AdminId");
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(id, adminid, note.blocknotes);
            return RedirectToAction("Admindashboard");
        }
    }
}