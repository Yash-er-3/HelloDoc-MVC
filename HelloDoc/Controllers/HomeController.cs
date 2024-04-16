using HelloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace HelloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HelloDocDbContext _context;


        public HomeController(ILogger<HomeController> logger, HelloDocDbContext context)
        {
            _logger = logger;
            _context = context;
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

        // Handle the reset password URL in the same controller or in a separate one
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
    }
}