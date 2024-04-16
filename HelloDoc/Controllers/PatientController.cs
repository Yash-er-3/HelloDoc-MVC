using HelloDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace HelloDoc.Controllers
{
    public class PatientController : Controller
    {
        public readonly HelloDocDbContext Context;

        public PatientController(HelloDocDbContext context)
        {
            Context = context;
        }
        public IActionResult PatientDashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                patient_dashboard dash = new patient_dashboard();
                var userdata = Context.Users.FirstOrDefault(m => m.Userid == id);
                TempData["user"] = userdata.Firstname;
                dash.user = userdata;
                var request = from m in Context.Requests
                              where m.Userid == id
                              select m;
                dash.requests = request.ToList();
                dash.DOB = new DateTime(Convert.ToInt32(userdata.Intyear), DateTime.ParseExact(userdata.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(userdata.Intdate));
                dash.requestwisefile = Context.Requestwisefiles.ToList();
                return View(dash);
            }
            else
            {
                return RedirectToAction("registeredpatient", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ydit(patient_dashboard dash)
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var userdata = Context.Users.FirstOrDefault(m => m.Userid == id);
            userdata.Firstname = dash.user.Firstname;
            userdata.Lastname = dash.user.Lastname;
            userdata.Mobile = dash.user.Mobile;
            userdata.City = dash.user.City;
            userdata.State = dash.user.State;
            userdata.Street = dash.user.Street;
            userdata.Zip = dash.user.Zip;
            userdata.Modifieddate = DateTime.Now;
            userdata.Strmonth = dash.DOB.ToString("MMM");
            userdata.Intdate = int.Parse(dash.DOB.ToString("dd"));
            userdata.Intyear = int.Parse(dash.DOB.ToString("yyyy"));

            Context.Users.Update(userdata);
            Context.SaveChanges();

            return RedirectToAction("PatientDashboard", "Patient");
        }

        //for view document

        public IActionResult viewdoc(int requestid)
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            patient_dashboard patient_Dashboard = new patient_dashboard();
            var userdata = Context.Users.FirstOrDefault(m => m.Userid == id);
            var confirmnumber = Context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            TempData["user"] = userdata.Firstname;
            patient_Dashboard.user = userdata;
            var req = from m in Context.Requestwisefiles
                      where m.Requestid == requestid
                      select m;
            patient_Dashboard.requestwisefile = req.ToList();
            patient_Dashboard.requestid = requestid;
            patient_Dashboard.DOB = new DateTime(Convert.ToInt32(userdata.Intyear), DateTime.ParseExact(userdata.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(userdata.Intdate));
            patient_Dashboard.Confirmationnumber = confirmnumber.Confirmationnumber;
            return View(patient_Dashboard);
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
                Context.Requestwisefiles.Add(requestWiseFiles);
                Context.SaveChanges();

            }
        }

        [HttpPost]
        public IActionResult UploadButton(patient_dashboard req, int requestid)
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                patient_dashboard model = new patient_dashboard();
                var users = Context.Users.FirstOrDefault(m => m.Userid == id);
                model.user = Context.Users.FirstOrDefault(m => m.Userid == id);
                model.requests = (from m in Context.Requests where m.Userid == id select m).ToList();
                TempData["user"] = users.Firstname;
                TempData["RequestId"] = requestid;
                //var req = Context.Requests.FirstOrDefault(m => m.UserId == id);
                model.requestwisefile = (from m in Context.Requestwisefiles where m.Requestid == requestid select m).ToList();
                //var reqe = Context.Requests.FirstOrDefault(m => m.UserId == id)

                model.requestid = requestid;

                if (req.fileName != null)
                {
                    UploadTable(requestid, req.fileName);
                }

                return RedirectToAction("viewdoc", model);
            }
            else
            {
                return RedirectToAction("viewdoc", requestid);

            }
        }

        [HttpPost]
        public ActionResult redirect()
        {
            var radio = Request.Form["options-outlined"];
            if (radio == "me")
            {
                return RedirectToAction("me", "Patient");
            }
            else
            {
                return RedirectToAction("someone", "Patient");
            }
        }

        public IActionResult me()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var users = Context.Users.FirstOrDefault(m => m.Userid == id);
            PatientInfo details = new PatientInfo();
            details.FirstName = users.Firstname;
            details.LastName = users.Lastname;
            details.Email = users.Email;
            details.DOB = new DateTime(Convert.ToInt32(users.Intyear), DateTime.ParseExact(users.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(users.Intdate));
            return View(details);
        }

        public IActionResult someone()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var users = Context.Users.FirstOrDefault(m => m.Userid == id);
            family f = new family();
            f.FirstName = users.Firstname;
            f.LastName = users.Lastname;
            return View(f);
        }

    }
}
