using DataAccess.ServiceRepository;
using DataAccess.ServiceRepository.IServiceRepository;
using HelloDoc.Views.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using Services.Viewmodels;
using System.Collections;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace HelloDoc.Controllers.ProviderSide
{
    public class ProviderSideController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IRequestDataRepository _data;
        private readonly IRequestRepository _request;
        private readonly IunitOfWork _unit;

        public ProviderSideController(HelloDocDbContext context, IRequestRepository request, IRequestDataRepository requestDataRepository, IunitOfWork unit)
        {
            _context = context;
            _request = request;
            _data = requestDataRepository;
            _unit = unit;
        }
        public IActionResult ProviderDashboard()
        {
            var requests = _request.GetAll().ToList();
            var region = _context.Regions.ToList();
            var casetag = _context.Casetags.ToList();
            var physician = _context.Physicians.ToList();
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
            adminDashboardViewModel.requests = requests.Where(m => m.Physicianid == physicianid).ToList();
            adminDashboardViewModel.regions = region;
            adminDashboardViewModel.casetags = casetag;
            return View(adminDashboardViewModel);

        }

        public IActionResult NewState()
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");
            var model = _data.GetAllRequestProviderData(1, physicianid);
            return View(model);
        }

        //accept case
        public IActionResult AcceptRequest(int requestid)
        {
            var result = _unit._updateData.UpdateRequestTable(requestid, 2);
            return RedirectToAction("ProviderDashboard");
        }

        public IActionResult PendingState()
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            var model = _data.GetAllRequestProviderData(2, physicianid);
            foreach (var item in model)
            {
                var physician = _context.Physicians.FirstOrDefault(m => m.Physicianid == item.PhysicianId);
                item.PhysicianName = physician.Firstname;
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult TransferModal(int requestid, AdminDashboardViewModel note)
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            _data.TransferCase(requestid, note, physicianid);

            return RedirectToAction("ProviderDashboard");
        }

        public IActionResult ActiveState()
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            var model = _data.GetAllRequestProviderData(4, physicianid).Concat(_data.GetAllRequestProviderData(5, physicianid)).ToList();

            return View(model);
        }
        public IActionResult ConcludeState()
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            var model = _data.GetAllRequestProviderData(6, physicianid);

            return View(model);
        }

        //encounter

        public IActionResult EncounterSubmit(int requestid, string encountervalue)
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            var requestdata = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            if (encountervalue == "Consult" && requestdata != null)
            {
                requestdata.Status = 6;
                requestdata.Calltype = 2;
                Requeststatuslog requeststatuslog = new Requeststatuslog();
                requeststatuslog.Status = requestdata.Status;
                requeststatuslog.Requestid = requestdata.Requestid;
                requeststatuslog.Notes = "Provider choose for consultunt";
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Physicianid = physicianid;
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(requestdata);
                _context.SaveChanges();
            }
            if (encountervalue == "Housecall" && requestdata != null)
            {
                requestdata.Status = 5;
                Requeststatuslog requeststatuslog = new Requeststatuslog();
                requeststatuslog.Status = requestdata.Status;
                requeststatuslog.Requestid = requestdata.Requestid;
                requeststatuslog.Notes = "Provider choose for housecall";
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Physicianid = physicianid;
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(requestdata);
                _context.SaveChanges();
            }
            return RedirectToAction("Encounter", new { requestid = requestid });
        }

        public IActionResult Encounter(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            var requestclients = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);

            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == requestid);
            var jwtservice = HttpContext.RequestServices.GetService<IJwtRepository>();
            var requestcookie = HttpContext.Request;
            var token = requestcookie.Cookies["jwt"];
            jwtservice.ValidateToken(token, out JwtSecurityToken jwttoken);
            var roleClaim = jwttoken.Claims.FirstOrDefault(x => x.Type == "Role");
            var role = "";
            if (roleClaim != null)
            {
                role = roleClaim.Value;
            }
            BitArray fortrue = new BitArray(1);
            fortrue[0] = true;
            BitArray forfalse = new BitArray(1);
            forfalse[0] = false;
            if (request.Status == 4 || (request.Status == 5 && request.Calltype == 1))
            {
                return PartialView("_SelectCallType", request);
            }
            if (request.Status == 6 && encounter == null)
            {
                EncounterFormViewModel model = new EncounterFormViewModel();
                model.Firstname = requestclients.Firstname;
                model.Lastname = requestclients.Lastname;
                model.DOB = new DateTime(Convert.ToInt32(requestclients.Intyear), DateTime.ParseExact(requestclients.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, (int)requestclients.Intdate).ToString("yyyy-MM-dd");
                model.Mobile = requestclients.Phonenumber;
                model.Email = requestclients.Email;
                model.Location = requestclients.Address;
                model.isFinaled = !fortrue[0];
                model.RequestId = request.Requestid;
                return View(model);
            }
            else if (request.Status == 6 && encounter.IsFinalized[0] != true)
            {
                EncounterFormViewModel model = new EncounterFormViewModel();
                model.RequestId = request.Requestid;
                model.Firstname = requestclients.Firstname;
                model.Lastname = requestclients.Lastname;
                model.DOB = new DateTime(Convert.ToInt32(requestclients.Intyear), DateTime.ParseExact(requestclients.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, (int)requestclients.Intdate).ToString("yyyy-MM-dd");
                model.Mobile = requestclients.Phonenumber;
                model.Email = requestclients.Email;
                model.Location = requestclients.Address;
                model.isFinaled = !fortrue[0];
                model.HistoryOfIllness = encounter.HistoryIllness;
                model.MedicalHistory = encounter.MedicalHistory;
                model.Medication = encounter.Medications;
                model.Allergies = encounter.Allergies;
                model.Temp = encounter.Temp;
                model.HR = encounter.Hr;
                model.RR = encounter.Rr;
                model.BPs = encounter.BpS;
                model.BPd = encounter.BpD;
                model.O2 = encounter.O2;
                model.Pain = encounter.Pain;
                model.Heent = encounter.Heent;
                model.CV = encounter.Cv;
                model.Chest = encounter.Chest;
                model.ABD = encounter.Abd;
                model.Extr = encounter.Extr;
                model.Skin = encounter.Skin;
                model.Neuro = encounter.Neuro;
                model.Other = encounter.Other;
                model.Diagnosis = encounter.Diagnosis;
                model.TreatmentPlan = encounter.TreatmentPlan;
                model.MedicationsDispended = encounter.MedicationDispensed;
                model.Procedure = encounter.Procedures;
                model.Followup = encounter.FollowUp;
                model.role = role;
                return View(model);
            }
            //else if(request.Status == 6 && encounter.IsFinalized[0] == true)
            //{
            //    return BadRequest("Already Finalized");
            //}
            else if ((request.Status == 6 || request.Status == 7 || request.Status == 8 || request.Status == 3) && encounter.IsFinalized != fortrue)
            {
                return PartialView("_DownLoadEncounter", new { requestid = request.Requestid, role = role });
            }
            else
            {
                return PartialView("_SelectCallType", request);
            }
        }

        [HttpPost]
        public IActionResult OnHouseOpenEncounter(int requestid)
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            request.Status = 6;
            _context.Requests.Update(request);
            Requeststatuslog requeststatuslog = new Requeststatuslog();
            requeststatuslog.Status = request.Status;
            requeststatuslog.Requestid = requestid;
            requeststatuslog.Notes = "Provider click on housecall";
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Physicianid = physicianid;
            _context.Requeststatuslogs.Add(requeststatuslog);
            _context.SaveChanges();
            return RedirectToAction("Encounter", new { requestid = requestid });
        }

        [HttpPost]
        public IActionResult EncounterFormSubmit(EncounterFormViewModel model)
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");
            int adminid = (int)HttpContext.Session.GetInt32("AdminId");

            var requestclient = _context.Requestclients.FirstOrDefault(x => x.Requestid == model.RequestId);

            BitArray fortrue = new BitArray(1);
            fortrue[0] = true;
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == model.RequestId);

            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == request.Requestid);
            if (encounter == null)
            {
                encounter = new Encounter();
            }
            requestclient.Firstname = model.Firstname;
            requestclient.Lastname = model.Lastname;
            requestclient.Phonenumber = model.Mobile;
            requestclient.Email = model.Email;
            requestclient.Address = model.Location;
            encounter.HistoryIllness = model.HistoryOfIllness;
            encounter.MedicalHistory = model.MedicalHistory;
            encounter.Medications = model.Medication;
            encounter.Allergies = model.Allergies;
            encounter.Temp = model.Temp;
            encounter.Hr = model.HR;
            encounter.Rr = model.RR;
            encounter.BpS = model.BPs;
            encounter.BpD = model.BPd;
            encounter.O2 = model.O2;
            encounter.Pain = model.Pain;
            encounter.Heent = model.Heent;
            encounter.Cv = model.CV;
            encounter.Chest = model.Chest;
            encounter.Abd = model.ABD;
            encounter.Extr = model.Extr;
            encounter.Skin = model.Skin;
            encounter.Neuro = model.Neuro;
            encounter.Other = model.Other;
            encounter.Diagnosis = model.Diagnosis;
            encounter.TreatmentPlan = model.TreatmentPlan;
            encounter.MedicationDispensed = model.MedicationsDispended;
            encounter.Procedures = model.Procedure;
            encounter.FollowUp = model.Followup;
            _context.Requests.Update(request);
            if (encounter.RequestId == 0)
            {
                encounter.RequestId = requestclient.Requestid;
                encounter.Date = DateTime.Now;
                //encounter. = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianid).Aspnetuserid;
                _context.Encounters.Add(encounter);
            }
            else
            {
                _context.Encounters.Update(encounter);
                //encounter.Modifieddate = DateTime.Now;
                //if (physicianid != -1)
                //{
                //    encounter.Modifiedby = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianid).Aspnetuserid;
                //}
                //if (adminid != -1)
                //{
                //    encounter.Modifiedby = _context.Admins.FirstOrDefault(x => x.Adminid == adminid).Aspnetuserid;
                //}
            }
            _context.SaveChanges();
            var jwtservice = HttpContext.RequestServices.GetService<IJwtRepository>();
            var requestcookie = HttpContext.Request;
            var token = requestcookie.Cookies["jwt"];
            jwtservice.ValidateToken(token, out JwtSecurityToken jwttoken);
            var roleClaim = jwttoken.Claims.FirstOrDefault(x => x.Type == "Role");
            var role = "";
            if (roleClaim != null)
            {
                role = roleClaim.Value;
            }
            if (role == "Admin")
            {
                return RedirectToAction("Admindashboard", "Admin");
            }
            else
            {
                return RedirectToAction("ProviderDashboard", "ProviderSide");
            }
        }

        [HttpPost]
        public IActionResult FinalizeEncounter(int requestid)
        {
            BitArray fortrue = new BitArray(1);
            fortrue[0] = true;
            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == requestid);
            if (encounter == null)
            {
                var enounternew = new Encounter();
                enounternew.RequestId = requestid;
                enounternew.IsFinalized = fortrue;
                enounternew.Date = DateTime.Now;
             
                _context.Encounters.Add(enounternew);
                _context.SaveChanges();
            }
            else
            {
                encounter.IsFinalized = fortrue;
                _context.Encounters.Update(encounter);
                _context.SaveChanges();
            }
            return RedirectToAction("ProviderDashboard", "ProviderSide");
        }

        public IActionResult EditEncounterAsAdmin(int requestid)
        {
            var requestclient = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);

            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == requestclient.Requestid);
            EncounterFormViewModel model = new EncounterFormViewModel();
            model.RequestId = requestclient.Requestid;
            model.Firstname = requestclient.Firstname;
            model.Lastname = requestclient.Lastname;
            model.DOB = new DateTime(Convert.ToInt32(requestclient.Intyear), DateTime.ParseExact(requestclient.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, (int)requestclient.Intdate).ToString("yyyy-MM-dd");
            model.Mobile = requestclient.Phonenumber;
            model.Email = requestclient.Email;
            model.Location = requestclient.Address;
            model.HistoryOfIllness = encounter.HistoryIllness;
            model.MedicalHistory = encounter.MedicalHistory;
            model.Medication = encounter.Medications;
            model.Allergies = encounter.Allergies;
            model.Temp = encounter.Temp;
            model.HR = encounter.Hr;
            model.RR = encounter.Rr;
            model.BPs = encounter.BpS;
            model.BPd = encounter.BpD;
            model.O2 = encounter.O2;
            model.Pain = encounter.Pain;
            model.Heent = encounter.Heent;
            model.CV = encounter.Cv;
            model.Chest = encounter.Chest;
            model.ABD = encounter.Abd;
            model.Extr = encounter.Extr;
            model.Skin = encounter.Skin;
            model.Neuro = encounter.Neuro;
            model.Other = encounter.Other;
            model.Diagnosis = encounter.Diagnosis;
            model.TreatmentPlan = encounter.TreatmentPlan;
            model.MedicationsDispended = encounter.MedicationDispensed;
            model.Procedure = encounter.Procedures;
            model.Followup = encounter.FollowUp;
            model.isFinaled = encounter.IsFinalized[0];
            model.role = "Admin";
            return PartialView("Encounter", model);
        }

        public IActionResult MyScheduling()
        {
            SchedulingViewModel modal = new SchedulingViewModel();
            modal.regions = _context.Regions.ToList();
            return View(modal);
        }

        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid, int status)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _context.Physicianregions.Include(u => u.Physician).Where(u => u.Regionid == regionid).Select(u => u.Physician).ToList();
            if (regionid == 0)
            {
                physician = _context.Physicians.ToList();
            }

            switch (PartialName)
            {

                case "_DayWise":
                    DayWiseScheduling day = new DayWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,

                    };
                    if (regionid != 0 && status != 0)
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid && m.Status == status).ToList();
                    }
                    else if (regionid != 0)
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid).ToList();

                    }
                    else if (status != 0)
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Status == status).ToList();

                    }
                    else
                    {
                        day.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).ToList();
                    }
                    return PartialView("_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new WeekWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,

                    };

                    if (regionid != 0 && status != 0)
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid && m.Status == status).ToList();
                    }
                    else if (regionid != 0)
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid).ToList();

                    }
                    else if (status != 0)
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Status == status).ToList();

                    }
                    else
                    {
                        week.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).ToList();
                    }
                    return PartialView("_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new MonthWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                    };
                    if (regionid != 0 && status != 0)
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid && m.Status == status).ToList();
                    }
                    else if (regionid != 0)
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Regionid == regionid).ToList();

                    }
                    else if (status != 0)
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).Where(m => m.Status == status).ToList();

                    }
                    else
                    {
                        month.shiftdetails = _context.Shiftdetails.Include(u => u.Shift).ToList();
                    }
                    return PartialView("_MonthWise", month);

                default:
                    return PartialView("_DayWise");
            }
        }

        public List<Physician> filterregion(string regionid)
        {
            List<Physician> physicians = _context.Physicianregions.Where(u => u.Regionid.ToString() == regionid).Select(y => y.Physician).ToList();
            return physicians;
        }

        public IActionResult AddShift(SchedulingViewModel model)
        {
            int adminid = (int)HttpContext.Session.GetInt32("AdminId");
            var admin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);
            Aspnetuser aspnetadmin = _context.Aspnetusers.FirstOrDefault(m => m.Id == admin.Aspnetuserid);
            var chk = Request.Form["repeatdays"].ToList();
            var shiftid = _context.Shifts.Where(u => u.Physicianid == model.providerid).Select(u => u.Shiftid).ToList();
            if (shiftid.Count() > 0)
            {
                foreach (var obj in shiftid)
                {
                    var shiftdetailchk = _context.Shiftdetails.Where(u => u.Shiftid == obj && u.Shiftdate == DateOnly.FromDateTime(model.shiftdate)).ToList();
                    if (shiftdetailchk.Count() > 0)
                    {
                        foreach (var item in shiftdetailchk)
                        {
                            if ((model.starttime >= item.Starttime && model.starttime <= item.Endtime) || (model.endtime >= item.Starttime && model.endtime <= item.Endtime))
                            {
                                TempData["error"] = "Shift is already assigned in this time";
                                return RedirectToAction("Scheduling");
                            }
                        }
                    }
                }
            }
            Shift shift = new Shift
            {
                Physicianid = model.providerid,
                Startdate = DateOnly.FromDateTime(model.shiftdate),
                Repeatupto = model.repeatcount,
                Createddate = DateTime.Now,
                Createdby = aspnetadmin.Id
            };
            foreach (var obj in chk)
            {
                shift.Weekdays += obj;
            }
            if (model.repeatcount > 0)
            {
                shift.Isrepeat = new BitArray(new[] { true });
            }
            else
            {
                shift.Isrepeat = new BitArray(new[] { false });
            }
            _context.Shifts.Add(shift);
            _context.SaveChanges();
            DateTime curdate = model.shiftdate;
            Shiftdetail shiftdetail = new Shiftdetail();
            shiftdetail.Shiftid = shift.Shiftid;
            shiftdetail.Shiftdate = DateOnly.FromDateTime(curdate);
            shiftdetail.Regionid = model.regionid;
            shiftdetail.Starttime = model.starttime;
            shiftdetail.Endtime = model.endtime;
            shiftdetail.Isdeleted = new BitArray(new[] { false });
            shiftdetail.Status = 1;
            _context.Shiftdetails.Add(shiftdetail);
            _context.SaveChanges();

            var dayofweek = model.shiftdate.DayOfWeek.ToString();
            int valueforweek;
            if (dayofweek == "Sunday")
            {
                valueforweek = 0;
            }
            else if (dayofweek == "Monday")
            {
                valueforweek = 1;
            }
            else if (dayofweek == "Tuesday")
            {
                valueforweek = 2;
            }
            else if (dayofweek == "Wednesday")
            {
                valueforweek = 3;
            }
            else if (dayofweek == "Thursday")
            {
                valueforweek = 4;
            }
            else if (dayofweek == "Friday")
            {
                valueforweek = 5;
            }
            else
            {
                valueforweek = 6;
            }
            if (shift.Isrepeat[0] == true)
            {
                for (int j = 0; j < shift.Weekdays.Count(); j++)
                {
                    var z = shift.Weekdays;
                    var p = shift.Weekdays.ElementAt(j).ToString();
                    int ele = Int32.Parse(p);
                    int x;
                    if (valueforweek > ele)
                    {
                        x = 6 - valueforweek + 1 + ele;
                    }
                    else
                    {
                        x = ele - valueforweek;
                    }
                    if (x == 0)
                    {
                        x = 7;
                    }
                    DateTime newcurdate = model.shiftdate.AddDays(x);
                    for (int i = 0; i < model.repeatcount; i++)
                    {
                        Shiftdetail shiftdetailnew = new Shiftdetail
                        {
                            Shiftid = shift.Shiftid,
                            Shiftdate = DateOnly.FromDateTime(newcurdate),
                            Regionid = model.regionid,
                            Starttime = new DateTime(newcurdate.Year, newcurdate.Month, newcurdate.Day, model.starttime.Hour, model.starttime.Minute, model.starttime.Second),
                            Endtime = new DateTime(newcurdate.Year, newcurdate.Month, newcurdate.Day, model.endtime.Hour, model.endtime.Minute, model.endtime.Second),
                            Isdeleted = new BitArray(new[] { false }),
                            Status = 1
                        };
                        _context.Shiftdetails.Add(shiftdetailnew);
                        _context.SaveChanges();
                        newcurdate = newcurdate.AddDays(7);
                    }
                }
            }
            return RedirectToAction("Scheduling");
        }
        public SchedulingViewModel ViewShiftOpen(int shiftdetailid)
        {

            Shiftdetail shiftdata = _context.Shiftdetails.Include(x => x.Shift).FirstOrDefault(s => s.Shiftdetailid == shiftdetailid);

            SchedulingViewModel model = new SchedulingViewModel
            {
                regionname = _context.Regions.FirstOrDefault(r => r.Regionid == shiftdata.Regionid).Regionid.ToString(),
                physicianname = _context.Physicians.FirstOrDefault(p => p.Physicianid == shiftdata.Shift.Physicianid).Firstname + " "
                                + _context.Physicians.FirstOrDefault(p => p.Physicianid == shiftdata.Shift.Physicianid).Lastname,
                shiftdateviewshift = shiftdata.Shiftdate,
                starttime = shiftdata.Starttime,
                endtime = shiftdata.Endtime,
            };

            return model;

        }

        public IActionResult MyProfile()
        {
            return View();
        }
    }
}
