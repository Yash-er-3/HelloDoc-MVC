using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelloDoc.DataModels;
using HelloDoc.ViewModels;
using System.Collections;
using System.Globalization;

namespace HelloDoc.Controllers.DataController
{
    public class CreateFamilyFriendController : Controller
    {
        private readonly HelloDocDbContext _log;

        public CreateFamilyFriendController(HelloDocDbContext log)
        {
            _log = log;
        }

        [HttpPost]
        public async Task<IActionResult> Family(family f)
        {
            var aspnetuser = await _log.Aspnetusers.FirstOrDefaultAsync(m => m.Email == f.PEmail);
            var user = await _log.Users.FirstOrDefaultAsync(m => m.Email == f.PEmail);

            if (aspnetuser != null)
            {
                Request request = new Request
                {
                    Requesttypeid = 2,
                    Userid = user.Userid,
                    Firstname = f.FirstName,
                    Lastname = f.LastName,
                    Phonenumber = f.PhoneNumber,
                    Email = f.Email,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Modifieddate = DateTime.Now,
                    User = user
                };

                _log.Requests.Add(request);
                _log.SaveChanges();


                Requestclient r = new Requestclient();
                ;
                r.Notes = f.Symptoms;
                r.Requestid = request.Requestid;
                r.Firstname = f.FirstName;
                r.Lastname = f.LastName;
                r.Email = f.Email;
                r.Phonenumber = f.PhoneNumber;
                r.State = f.State;
                r.City = f.City;
                r.Zipcode = f.ZipCode;
                r.Address = f.Room + " , " + f.Street + " , " + f.City + " , " + f.State;
                r.Intdate = f.PDOB.Day;
                r.Intyear = f.PDOB.Year;
                r.Strmonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(f.PDOB.Month);
                r.Regionid = 1;


                _log.Requestclients.Add(r);
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
