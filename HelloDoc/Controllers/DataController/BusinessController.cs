using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using HelloDoc.ViewModels;

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

        public async Task<IActionResult> BusinessRequest(business b)
        {
            var aspnetuser = await _log.Aspnetusers.FirstOrDefaultAsync(m => m.Email == b.PEmail);
            var user = await _log.Users.FirstOrDefaultAsync(m => m.Email == b.PEmail);
            if (aspnetuser != null)
            {

                Request request = new Request
                {
                    Requesttypeid = 2,
                    Userid = user.Userid,
                    Firstname = b.PFirstName,
                    Lastname = b.PLastName,
                    Email = b.PEmail,
                    Phonenumber = b.PPhoneNumber,
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
                    Firstname = b.FirstName,
                    Lastname = b.LastName,
                    Email = b.Email,
                    Phonenumber = b.PhoneNumber,
                    State = b.State,
                    Street = b.Street,
                    City = b.City,
                    Zipcode = b.ZipCode,
                    Address = b.Room + " , " + b.Street + " , " + b.City + " , " + b.State,
                    Intyear = int.Parse(b.PDOB.ToString("yyyy")),
                    Intdate = int.Parse(b.PDOB.ToString("dd")),
                    Strmonth = b.PDOB.ToString("MMM"),
                    Regionid = (int)user.Regionid
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
                var requestdata = await _log.Requests.FirstOrDefaultAsync(m => m.Email == b.Email);
                var buisnessdata = await _log.Businesses.FirstOrDefaultAsync(m => m.Name == b.FirstName + b.LastName);
                Requestbusiness requestbusiness = new Requestbusiness
                {
                    Requestid = requestdata.Requestid,
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
