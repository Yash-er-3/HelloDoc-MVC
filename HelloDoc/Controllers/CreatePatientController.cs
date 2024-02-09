using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloDoc.DataModels;
using HelloDoc.ViewModels;
using System.Collections;

namespace HelloDoc.Controllers
{
    public class CreatePatientController : Controller
    {

        private readonly HelloDocDbContext _log;

        public CreatePatientController(HelloDocDbContext log)
        {
            _log = log;
        }

        [HttpPost]
        public async Task<IActionResult> patient(PatientInfo r)
        {
            var Aspnetuser = await _log.Aspnetusers.FirstOrDefaultAsync(m => m.Email == r.Email);

            if(Aspnetuser == null)
            {

                Aspnetuser aspnetuser = new Aspnetuser();
                aspnetuser.Passwordhash = r.Password;
                aspnetuser.Email = r.Email;
                String username = r.FirstName + r.LastName;
                aspnetuser.Username = username;
                aspnetuser.Phonenumber = r.PhoneNumber;
                aspnetuser.Modifieddate = DateTime.Now;
                _log.Aspnetusers.Add(aspnetuser);
                Aspnetuser = aspnetuser;
            }

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
            user.Modifieddate = DateTime.Now;
            user.Status = 1;
            user.Regionid = 1;

            _log.Users.Add(user);
            _log.SaveChanges();

            Request request = new Request {
                Requesttypeid = 2,
                Firstname = r.FirstName,
                Lastname = r.LastName,
                Phonenumber = r.PhoneNumber,
                Email = r.Email,
                Status = 1,
                Createddate = DateTime.Now,
                Modifieddate = DateTime.Now,
            Userid = user.Userid
            };

            _log.Requests.Add(request);
            _log.SaveChanges();
            var requestdata = await _log.Requests.FirstOrDefaultAsync(m => m.Email == user.Email);
            Requestclient requestclient = new Requestclient
            {
                Requestid = requestdata.Requestid,
                Firstname = r.FirstName,
                Lastname = r.LastName,
                Phonenumber = r.PhoneNumber,
                Notes = r.Symptoms,
                Email = r.Email, 
                Street = r.Street,
                City = r.City,
                State = r.State,   
                Zipcode = r.ZipCode,
                Regionid = 1
            };

            _log.Requestclients.Add(requestclient);
            _log.SaveChanges();

            return RedirectToAction("registeredpatient","Home");
            
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
