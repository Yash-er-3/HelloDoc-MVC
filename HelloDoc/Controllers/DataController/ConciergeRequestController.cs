using HelloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HelloDoc.Controllers.DataController
{
    public class ConciergeRequestController : Controller
    {
        private readonly HelloDocDbContext _log;

        public ConciergeRequestController(HelloDocDbContext log)
        {
            _log = log;
        }

        [HttpPost]
        public IActionResult Concierge(concierge c)
        {
            var aspnetuser =  _log.Aspnetusers.FirstOrDefault(m => m.Email == c.PEmail);
            var user =  _log.Users.FirstOrDefault(m => m.Email == c.PEmail);


            if (aspnetuser != null)
            {
                aspnetuser.Phonenumber = c.PhoneNumber;
                user.Mobile = c.PhoneNumber;
                user.Street = c.Street;
                user.City = c.City;
                user.State = c.State;
                user.Zip = c.ZipCode;
                user.Intyear = int.Parse(c.PDOB.ToString("yyyy"));
                user.Intdate = int.Parse(c.PDOB.ToString("dd"));
                user.Strmonth = c.PDOB.ToString("MMM");
                _log.Aspnetusers.Update(aspnetuser);
                _log.Users.Update(user);
                _log.SaveChanges();

                Request request = new Request
                {
                    Requesttypeid = 3,
                    Userid = user.Userid,
                    Firstname = c.FirstName,
                    Lastname = c.LastName,
                    Phonenumber = c.PhoneNumber,
                    Email = c.Email,
                    Status = 1,
                    Createddate = DateTime.Now,
                    Modifieddate = DateTime.Now,
                    User = user
                };

                _log.Add(request);
                _log.SaveChanges();


                Requestclient r = new Requestclient();

                r.Notes = c.Symptoms;
                r.Requestid = request.Requestid;
                r.Firstname = c.PFirstName;
                r.Lastname = c.PLastName;
                r.Email = c.PEmail;
                r.Phonenumber = c.PPhoneNumber;
                r.State = c.State;
                r.City = c.City;
                r.Zipcode = c.ZipCode;
                r.Address = c.Room + " , " + c.Street + " , " + c.City + " , " + c.State;
                user.Intyear = int.Parse(c.PDOB.ToString("yyyy"));
                user.Intdate = int.Parse(c.PDOB.ToString("dd"));
                user.Strmonth = c.PDOB.ToString("MMM");
                r.Strmonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(c.PDOB.Month);
                r.Regionid = 1;

                _log.Add(r);
                _log.SaveChanges();

                Concierge con = new Concierge();

                con.Conciergename = c.FirstName + c.LastName;
                con.Address = c.Room + " , " + c.Street + " , " + c.City + " , " + c.State;
                con.State = c.State;
                con.Street = c.Street;
                con.City = c.City;
                con.Zipcode = c.ZipCode;
                con.Createddate = DateTime.Now;
                con.Regionid = 1;


                _log.Add(con);
                _log.SaveChanges();

             

                Requestconcierge reqcon = new Requestconcierge();

                reqcon.Requestid = r.Requestid;
                reqcon.Conciergeid = con.Conciergeid;

                _log.Add(reqcon);
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
