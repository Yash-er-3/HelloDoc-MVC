using DataAccess.ServiceRepository.IServiceRepository;
using DataAccess.ServiceRepository;
using HelloDoc.DataContext;
using HelloDoc.DataModels;
using HelloDoc.Views.Shared;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Services.Contracts;
using Services.Implementation;
using Services.Viewmodels;
using System.Collections;
using System.Diagnostics.Metrics;
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
            return RedirectToAction("Encounter", "ProviderSide");
        }

        [AuthorizationRepository("Admin,Physician")]
        [HttpGet]
        public IActionResult Encounter(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);

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
                model.Firstname = request.Requestclients.First().Firstname;
                model.Lastname = request.Requestclients.First().Lastname;
                model.DOB = new DateTime(Convert.ToInt32(request.User.Intyear), DateTime.ParseExact(request.User.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(request.User.Intdate)).ToString("yyyy-MM-dd");
                model.Mobile = request.Requestclients.FirstOrDefault().Phonenumber;
                model.Email = request.Requestclients.FirstOrDefault().Email;
                model.Location = request.Requestclients.FirstOrDefault().Address;
                model.isFinaled = !fortrue[0];
                model.RequestId = request.Requestid;
                return View(model);
            }
            else if (request.Status == 6 && encounter.IsFinalized[0] != true)
            {
                EncounterFormViewModel model = new EncounterFormViewModel();
                model.RequestId = request.Requestid;
                model.Firstname = request.Requestclients.First().Firstname;
                model.Lastname = request.Requestclients.First().Lastname;
                model.DOB = new DateTime(Convert.ToInt32(request.User.Intyear), DateTime.ParseExact(request.User.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(request.User.Intdate)).ToString("yyyy-MM-dd");
                model.Mobile = request.Requestclients.FirstOrDefault().Phonenumber;
                model.Email = request.Requestclients.FirstOrDefault().Email;
                model.Location = request.Requestclients.FirstOrDefault().Address;
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
            else if ((request.Status == 6 || request.Status == 7 || request.Status == 8 || request.Status == 3) && encounter.IsFinalized != fortrue)
            {
                return PartialView("_DownLoadEncounter", new { requestid = request.Requestid, role = role });
            }
            else
            {
                return PartialView("_SelectCallType", request);
            }
        }

        [AuthorizationRepository("Admin,Physician")]
        [HttpPost]
        public IActionResult EncounterFormSubmit(EncounterFormViewModel model)
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");
            int adminid = (int)HttpContext.Session.GetInt32("AdminId");

            BitArray fortrue = new BitArray(1);
            fortrue[0] = true;
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == model.RequestId);

            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == request.Requestid);
            if (encounter == null)
            {
                encounter = new Encounter();
            }
            request.Requestclients.First().Firstname = model.Firstname;
            request.Requestclients.First().Lastname = model.Lastname;
            request.Requestclients.FirstOrDefault().Phonenumber = model.Mobile;
            request.Requestclients.FirstOrDefault().Email = model.Email;
            request.Requestclients.FirstOrDefault().Address = model.Location;
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
                encounter.RequestId = request.Requestid;
                encounter.Createddate = DateTime.Now;
                encounter.Createdby = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianid).Aspnetuserid;
                _context.Encounters.Add(encounter);
            }
            else
            {
                _context.Encounters.Update(encounter);
                encounter.Modifieddate = DateTime.Now;
                if (physicianid != -1)
                {
                    encounter.Modifiedby = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianid).Aspnetuserid;
                }
                if (adminid != -1)
                {
                    encounter.Modifiedby = _context.Admins.FirstOrDefault(x => x.Adminid == adminid).Aspnetuserid;
                }
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
    }
}
