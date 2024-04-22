using HelloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Services.Contracts;
using Services.Implementation;
using System.Globalization;

namespace HelloDoc.Controllers.DataController
{
    public class CreateFamilyFriendController : Controller
    {
        private readonly HelloDocDbContext _log;
        private readonly ISendEmailAndSMS sendEmailAndSMS;

        public CreateFamilyFriendController(HelloDocDbContext log, ISendEmailAndSMS senEmailAndSMS)
        {
            _log = log;
            sendEmailAndSMS = senEmailAndSMS;
        }
        [HttpPost]
        public void UploadTable(int id, List<IFormFile> file)
        {
            foreach (var item in file)

            {

                //string path = _environment.WebRootPath + "/UploadDocument/" + item.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", item.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    item.CopyTo(fileStream);
                }

                Requestwisefile requestWiseFiles = new Requestwisefile
                {
                    Requestid = id,
                    Filename = path,
                    Createddate = DateTime.Now,
                };
                _log.Requestwisefiles.Add(requestWiseFiles);
                _log.SaveChanges();

            }
        }

        [HttpPost]
        public async Task<IActionResult> Family(family f, string someone)
        {
            var aspnetuser = await _log.Aspnetusers.FirstOrDefaultAsync(m => m.Email == f.PEmail);
            var user = await _log.Users.FirstOrDefaultAsync(m => m.Email == f.PEmail);

            if (someone == "someone")
            {
                var userid = HttpContext.Session.GetInt32("UserId");
                var userloggedin = _log.Users.FirstOrDefault(x => x.Userid == userid);

                if (userloggedin != null)
                {
                    Request req = new Request
                    {
                        Requesttypeid = 5,
                        Userid = userloggedin.Userid,
                        Firstname = userloggedin.Firstname,
                        Lastname = userloggedin.Lastname,
                        Email = userloggedin.Email,
                        Phonenumber = userloggedin.Mobile,
                        Status = 1,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now,
                    };

                    _log.Add(req);
                    _log.SaveChanges();

                    Requestclient r = new Requestclient();

                    r.Notes = f.Symptoms;
                    r.Requestid = req.Requestid;
                    r.Firstname = f.PFirstName;
                    r.Lastname = f.PLastName;
                    r.Phonenumber = f.PPhoneNumber;
                    r.Email = f.PEmail;
                    r.State = f.State;
                    r.City = f.City;
                    r.Zipcode = f.ZipCode;
                    r.Address = f.Room + " , " + f.Street + " , " + f.City + " , " + f.State;
                    user.Intyear = int.Parse(f.PDOB.ToString("yyyy"));
                    user.Intdate = int.Parse(f.PDOB.ToString("dd"));
                    user.Strmonth = f.PDOB.ToString("MMM");
                    r.Strmonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(f.PDOB.Month);
                    r.Regionid = 1;

                    _log.Requestclients.Add(r);
                    _log.SaveChanges();

                    if (f.FileName != null)
                    {
                        UploadTable(req.Requestid, f.FileName);
                    }
                }

               

                return RedirectToAction("submitrequest", "Home");
            }

            if (aspnetuser != null)
            {
                aspnetuser.Phonenumber = f.PhoneNumber;
                user.Mobile = f.PhoneNumber;
                user.Street = f.Street;
                user.City = f.City;
                user.State = f.State;
                user.Zip = f.ZipCode;
                user.Intyear = int.Parse(f.PDOB.ToString("yyyy"));
                user.Intdate = int.Parse(f.PDOB.ToString("dd"));
                user.Strmonth = f.PDOB.ToString("MMM");
                _log.Aspnetusers.Update(aspnetuser);
                _log.Users.Update(user);
                _log.SaveChanges();

                Request request = new Request
                {
                    Requesttypeid = 2,
                    Userid = user.Userid,
                    Firstname = f.FirstName,
                    Lastname = f.LastName,
                    Email = f.Email,
                    Phonenumber = f.PhoneNumber,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Modifieddate = DateTime.Now,
                    User = user
                };

                _log.Requests.Add(request);
                _log.SaveChanges();


                Requestclient r = new Requestclient();

                r.Notes = f.Symptoms;
                r.Requestid = request.Requestid;
                r.Firstname = f.PFirstName;
                r.Lastname = f.PLastName;
                r.Phonenumber = f.PPhoneNumber;
                r.Email = f.PEmail;
                r.State = f.State;
                r.City = f.City;
                r.Zipcode = f.ZipCode;
                r.Address = f.Room + " , " + f.Street + " , " + f.City + " , " + f.State;
                user.Intyear = int.Parse(f.PDOB.ToString("yyyy"));
                user.Intdate = int.Parse(f.PDOB.ToString("dd"));
                user.Strmonth = f.PDOB.ToString("MMM");
                r.Strmonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(f.PDOB.Month);
                r.Regionid = 1;


                _log.Requestclients.Add(r);
                _log.SaveChanges();


                if (f.FileName != null)
                {
                    UploadTable(request.Requestid, f.FileName);
                }

                return RedirectToAction("submitrequest", "Home");
            }
            else
            {
                return RedirectToAction("submitrequest", "Home");
            }
        }

        public bool EmailValidate(family f)
        {
            var aspnetuser = _log.Aspnetusers.FirstOrDefault(x => x.Email == f.Email);

            if (aspnetuser != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string GenerateAgreementUrl(string email)
        {
            //var encryptemail = EncryptDecrypt.Encrypt(email);
            var link = "https://localhost:44300/Home/CreateUser?email=" + email;
            return link;
        }

        public void CreateUserSendMail(string email)
        {
            string AgreementUrl = GenerateAgreementUrl(email);
            sendEmailAndSMS.Sendemail(email, "Create Your Account", $"Hello, Click On below Link to create account: {AgreementUrl}");
        }
    }
}
