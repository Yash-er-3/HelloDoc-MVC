using HelloDoc.Models;
using HelloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Implementation;
using Services.Viewmodels;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Transactions;

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

            var usercheck = _context.Users.FirstOrDefault(x => x.Email == user.Email);

            if (usercheck != null)
            {
                string Id = (_context.Aspnetusers.FirstOrDefault(x => x.Email == user.Email)).Id;

                string resetPasswordUrl = GenerateResetPasswordUrl(Id);

                SendEmail(user.Email, "Reset Your Password", $"Hello, reset your password using this link: {resetPasswordUrl}");
                return RedirectToAction("registeredpatient", "Home");
            }
            else
            {
                TempData["error"] = "Email not exist";
            }
            return RedirectToAction("forgotpassword", "Home");

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

            if(request.Status == 4)
            {
                return BadRequest("Already agreed!");
            }
            else
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();
                _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(requestid);
                return RedirectToAction("PatientDashboard", "Patient");
            }

          
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

        public IActionResult CreateUser(string email)
        {
            PatientInfo p = new PatientInfo();
            p.Email = email;
            return View(p);
        }

        [HttpPost]
        public IActionResult CreateUser(PatientInfo p)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    var usercheck = _context.Users.FirstOrDefault(x => x.Email == p.Email);
                    if (usercheck != null)
                    {
                        TempData["error"] = "User Already Exist";
                    }
                    else
                    {
                        Aspnetuser aspnetuer = new Aspnetuser
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = p.Email,
                            Username = p.FirstName + " " + p.LastName,
                            Passwordhash = p.Password,
                            Createddate = DateTime.Now,
                        };

                        _context.Add(aspnetuer);
                        _context.SaveChanges();

                        Aspnetuserrole aspnetuserrole = new Aspnetuserrole
                        {
                            Userid = aspnetuer.Id,
                            Roleid = "1"
                        };

                        _context.Add(aspnetuserrole);
                        _context.SaveChanges();

                        User user = new User
                        {
                            Aspnetuserid = aspnetuer.Id,
                            Firstname = p.FirstName,
                            Lastname = p.LastName,
                            Email = p.Email,
                            Intyear = int.Parse(aspnetuer.Createddate.ToString("yyyy")),
                            Intdate = int.Parse(aspnetuer.Createddate.ToString("dd")),
                            Strmonth = aspnetuer.Createddate.ToString("MMM"),
                            Createdby = p.FirstName + " " + p.LastName,
                            Createddate = DateTime.Now,
                            Regionid = 1
                        };
                        _context.Add(user);
                        _context.SaveChanges();
                        transaction.Commit();
                        TempData["success"] = "User Created Successfully";
                    }


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }


            return RedirectToAction("registeredpatient", "Home");
        }

    }
}