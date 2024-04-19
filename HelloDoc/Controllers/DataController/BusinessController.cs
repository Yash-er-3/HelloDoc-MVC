using HelloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.Controllers.DataController
{
    public class BusinessController : Controller
    {
        private readonly HelloDocDbContext _log;

        public BusinessController(HelloDocDbContext log)
        {
            _log = log;
        }

        [HttpPost]

        public IActionResult BusinessRequest(business b)
        {
            var aspnetuser =  _log.Aspnetusers.FirstOrDefault(m => m.Email == b.PEmail);
            var user =  _log.Users.FirstOrDefault(m => m.Email == b.PEmail);
            if (aspnetuser != null)
            {
                aspnetuser.Phonenumber = b.PhoneNumber;
                user.Mobile = b.PhoneNumber;
                user.Street = b.Street;
                user.City = b.City;
                user.State = b.State;
                user.Zip = b.ZipCode;
                user.Intyear = int.Parse(b.PDOB.ToString("yyyy"));
                user.Intdate = int.Parse(b.PDOB.ToString("dd"));
                user.Strmonth = b.PDOB.ToString("MMM");
                _log.Aspnetusers.Update(aspnetuser);
                _log.Users.Update(user);
                _log.SaveChanges();

                Request request = new Request
                {
                    Requesttypeid = 4,
                    Userid = user.Userid,
                    Firstname = b.FirstName,
                    Lastname = b.LastName,
                    Email = b.Email,
                    Phonenumber = b.PhoneNumber,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Casenumber = b.CaseNumber,
                    User = user,
                };
                _log.Add(request);
                _log.SaveChanges();
                Requestclient requestclient = new Requestclient
                {
                    Notes = b.Symptoms,
                    Requestid = request.Requestid,
                    Firstname = b.PFirstName,
                    Lastname = b.PLastName,
                    Email = b.PEmail,
                    Phonenumber = b.PPhoneNumber,
                    Street = b.Street,
                    City = b.City,
                    State = b.State,
                    Zipcode = b.ZipCode,
                    Address = b.Room + " , " + b.Street + " , " + b.City + " , " + b.State,
                    Intyear = int.Parse(b.PDOB.ToString("yyyy")),
                    Intdate = int.Parse(b.PDOB.ToString("dd")),
                    Strmonth = b.PDOB.ToString("MMM"),
                    Regionid = 1
                };
                _log.Add(requestclient);
                _log.SaveChanges();
                Business business = new Business
                {
                    Name = b.FirstName + b.LastName,
                    Address1 = b.Room + " , " + b.Street + " , " + b.City + " , " + b.State,
                    City = b.City,
                    Zipcode = b.ZipCode,
                    Createddate = DateTime.Now,
                    Phonenumber = b.PhoneNumber,
                    Regionid = 1,
                    Createdby = aspnetuser.Id,
                    Modifiedby = aspnetuser.Id
                };
                _log.Businesses.Add(business);
                _log.SaveChanges();
               
                Requestbusiness requestbusiness = new Requestbusiness
                {
                    Requestid = requestclient.Requestid,
                    Businessid = business.Businessid,

                };
                _log.Add(requestbusiness);
                _log.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("submitrequest", "Home");
            }
        }

    }
}
