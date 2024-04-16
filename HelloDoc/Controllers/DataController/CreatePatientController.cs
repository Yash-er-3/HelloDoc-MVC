using HelloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.Controllers.DataController
{
    public class CreatePatientController : Controller
    {

        private readonly IWebHostEnvironment _env;
        private readonly HelloDocDbContext _log;

        public CreatePatientController(HelloDocDbContext log, IWebHostEnvironment env)
        {
            _log = log;
            _env = env;
        }
        public IActionResult patient()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> patient(PatientInfo r)
        {

            var Aspnetuser = await _log.Aspnetusers.FirstOrDefaultAsync(m => m.Email == r.Email);

            if (Aspnetuser == null)
            {
                Aspnetuser aspnetuser = new Aspnetuser();
                aspnetuser.Id = Guid.NewGuid().ToString();
                aspnetuser.Passwordhash = r.Password;
                aspnetuser.Email = r.Email;
                string username = r.FirstName + r.LastName;
                aspnetuser.Username = username;
                aspnetuser.Phonenumber = r.PhoneNumber;
                aspnetuser.Createddate = DateTime.Now;
                _log.Aspnetusers.Add(aspnetuser);
                Aspnetuser = aspnetuser;

                User user = new User();
                user.Aspnetuserid = Aspnetuser.Id;
                user.Firstname = r.FirstName;
                user.Lastname = r.LastName;
                user.Email = r.Email;
                user.Mobile = r.PhoneNumber;
                user.Street = r.Street;
                user.City = r.City;
                user.State = r.State;
                user.Zip = r.ZipCode;
                user.Createdby = r.FirstName + r.LastName;
                user.Createddate = DateTime.Now;
                user.Intyear = int.Parse(r.DOB.ToString("yyyy"));
                user.Intdate = int.Parse(r.DOB.ToString("dd"));
                user.Strmonth = r.DOB.ToString("MMM");
                user.Status = 1;
                user.Regionid = 1;

                _log.Users.Add(user);
                _log.SaveChanges();
            }

            var user1 = await _log.Users.FirstOrDefaultAsync(m => m.Email == r.Email);
            var region = await _log.Regions.FirstOrDefaultAsync(x => x.Regionid == user1.Regionid);
            var requestcount = (from m in _log.Requests where m.Createddate.Date == DateTime.Now.Date select m).ToList();
            Request request = new Request
            {
                Requesttypeid = 1,
                Firstname = r.FirstName,
                Lastname = r.LastName,
                Phonenumber = r.PhoneNumber,
                Email = r.Email,
                Status = 1,
                Createddate = DateTime.Now,
                Modifieddate = DateTime.Now,
                Userid = user1.Userid,
                Confirmationnumber = (region.Abbreviation.Substring(0, 2) + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + r.LastName.Substring(0, 2) + r.FirstName.Substring(0, 2) + requestcount.Count().ToString().PadLeft(4, '0')).ToUpper(),
            };

            _log.Requests.Add(request);
            _log.SaveChanges();
            //var requestdata = await _log.Requests.FirstOrDefaultAsync(m => m.Email == user.Email);
            Requestclient requestclient = new Requestclient
            {
                Requestid = request.Requestid,
                Firstname = r.FirstName,
                Lastname = r.LastName,
                Phonenumber = r.PhoneNumber,
                Notes = r.Symptoms,
                Email = r.Email,
                Street = r.Street,
                City = r.City,
                State = r.State,
                Zipcode = r.ZipCode,
                Regionid = 1,
                Intyear = int.Parse(r.DOB.ToString("yyyy")),
                Intdate = int.Parse(r.DOB.ToString("dd")),
                Strmonth = r.DOB.ToString("MMM")
            };

            _log.Requestclients.Add(requestclient);
            _log.SaveChanges();


            if (r.Upload != null)
            {
                uploadFile(r.Upload, request.Requestid);
            }

            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("PatientDashboard", "Patient");

            }
            return RedirectToAction("registeredpatient", "Home");

        }

        public void uploadFile(List<IFormFile> upload, int id)
        {

            foreach (var item in upload)
            {
                String path = _env.WebRootPath + "/upload/" + id + " " + item.FileName;
                FileStream stream = new FileStream(path, FileMode.Create);

                item.CopyTo(stream);

                Requestwisefile requestwisefile = new Requestwisefile();

                requestwisefile.Requestid = id;
                requestwisefile.Filename = path;
                requestwisefile.Createddate = DateTime.Now;

                _log.Add(requestwisefile);
                _log.SaveChanges();
            }


        }

        [Route("/CreatePatient/patient/checkmail/{email}")]
        [HttpGet]
        public IActionResult CheckEmail(string email)
        {
            var emailExists = _log.Aspnetusers.Any(u => u.Email == email);
            return Json(new { exists = emailExists });
        }



    }
}
