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
    public class ConciergeRequestController:Controller
    {
        private readonly HelloDocDbContext _log;

        public ConciergeRequestController(HelloDocDbContext log)
        {
            _log = log;
        }

        [HttpPost]

        public async Task<IActionResult> Concierge(concierge c)
        {
            var aspnetuser = await _log.Aspnetusers.FirstOrDefaultAsync(m => m.Email == c.PEmail);
            var user = await _log.Users.FirstOrDefaultAsync(m => m.Email == c.PEmail);


            if (aspnetuser != null)
            {
                Request request = new Request
                {
                    Requesttypeid = 2,
                    Userid = user.Userid,
                    Firstname = c.PFirstName,
                    Lastname = c.PLastName,
                    Phonenumber = c.PPhoneNumber,
                    Email = c.PEmail,
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
                r.Firstname = c.FirstName;
                r.Lastname = c.LastName;
                r.Email = c.Email;
                r.Phonenumber = c.PhoneNumber;
                r.State = c.State;
                r.City = c.City;
                r.Zipcode = c.ZipCode;
                r.Address = c.Room + " , " + c.Street + " , " + c.City + " , " + c.State;
                r.Intdate = c.PDOB.Day;
                r.Intyear = c.PDOB.Year;
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

                var requestdata = await _log.Requests.FirstOrDefaultAsync(m => m.Email == c.PEmail);
                var condata = await _log.Concierges.FirstOrDefaultAsync(m => m.Conciergename == c.FirstName + c.LastName);

                Requestconcierge reqcon = new Requestconcierge();

                reqcon.Requestid = requestdata.Requestid;
                reqcon.Conciergeid = condata.Conciergeid;

                _log.Add(reqcon);
                _log.SaveChanges();

                return RedirectToAction("Index","Home");
            }
            else
            {
                return RedirectToAction("submitrequest","Home");
            }
        }
    }
}
