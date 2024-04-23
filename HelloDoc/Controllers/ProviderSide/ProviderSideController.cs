using DataAccess.ServiceRepository;
using DataAccess.ServiceRepository.IServiceRepository;
using HelloDoc.ViewModels;
using HelloDoc.Views.Shared;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Services.Contracts;
using Services.Implementation;
using Services.Viewmodels;
using System.Collections;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Vonage.Users;
using static NPOI.HSSF.Util.HSSFColor;

namespace HelloDoc.Controllers.ProviderSide
{
    public class ProviderSideController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IRequestDataRepository _data;
        private readonly IRequestRepository _request;
        private readonly IunitOfWork _unit;
        private readonly IAddOrUpdateRequestNotes _addOrUpdateRequestNotes;
        private readonly ISendEmailAndSMS sendEmailAndSMS;

        public ProviderSideController(HelloDocDbContext context, IRequestRepository request, IRequestDataRepository requestDataRepository, IunitOfWork unit, IAddOrUpdateRequestNotes addOrUpdateRequestNotes, ISendEmailAndSMS sendEmailAndSMS)
        {
            _context = context;
            _request = request;
            _data = requestDataRepository;
            _unit = unit;
            _addOrUpdateRequestNotes = addOrUpdateRequestNotes;
            this.sendEmailAndSMS = sendEmailAndSMS;
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
                return RedirectToAction("Encounter", new { requestid = requestid });
            }
            else if (encountervalue == "Housecall" && requestdata != null)
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
                return Ok();
            }
            return Ok();

        }

        public bool CheckFinalize(int requestid)
        {
            BitArray fortrue = new BitArray(1);
            fortrue[0] = true;

            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == requestid);

            if(encounter == null)
            {
                return false;
            }

            if (encounter.IsFinalized[0])
            {
                return true;
            }
            else
            {
                return false;
            }
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
            try
            {
                int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");
            }
            catch
            {

            }

            try
            {
                int adminid = (int)HttpContext.Session.GetInt32("AdminId");

            }
            catch
            {

            }

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
            return Ok();
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

        private PdfPCell CreateCell(string content)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content));
            cell.Padding = 5;
            return cell;
        }

        [HttpGet]
        public IActionResult DownloadEncounterAsAdmin(int id)
        {
            var request = _context.Requests.Include(x => x.Requestclients).Include(x => x.User).FirstOrDefault(x => x.Requestid == id);
            var encounter = _context.Encounters.FirstOrDefault(x => x.RequestId == request.Requestid);
            EncounterFormViewModel model = new EncounterFormViewModel();
            model.RequestId = request.Requestid;
            model.Firstname = request.Requestclients.First().Firstname;
            model.Lastname = request.Requestclients.First().Lastname;
            model.DOB = new DateTime(Convert.ToInt32(request.User.Intyear), DateTime.ParseExact(request.User.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(request.User.Intdate)).ToString("yyyy-MM-dd");
            model.Mobile = request.Requestclients.FirstOrDefault().Phonenumber;
            model.Email = request.Requestclients.FirstOrDefault().Email;
            model.Location = request.Requestclients.FirstOrDefault().Address;
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
            var pdf = new iTextSharp.text.Document();
            using (var memoryStream = new MemoryStream())
            {
                var writer = PdfWriter.GetInstance(pdf, memoryStream);
                pdf.Open();

                // Create a table with two columns
                PdfPTable table = new PdfPTable(2);


                // Add cells to the table here:
                table.AddCell(CreateCell("First Name"));
                table.AddCell(CreateCell(model.Firstname ?? "N/A"));
                table.AddCell(CreateCell("Last Name"));
                table.AddCell(CreateCell(model.Lastname ?? "N/A"));
                table.AddCell(CreateCell("DOB"));
                table.AddCell(CreateCell(model.DOB?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("Mobile"));
                table.AddCell(CreateCell(model.Mobile ?? "N/A"));
                table.AddCell(CreateCell("Email"));
                table.AddCell(CreateCell(model.Email ?? "N/A"));
                table.AddCell(CreateCell("Location"));
                table.AddCell(CreateCell(model.Location ?? "N/A"));
                table.AddCell(CreateCell("History Of Illness"));
                table.AddCell(CreateCell(model.HistoryOfIllness ?? "N/A"));
                table.AddCell(CreateCell("Medical History"));
                table.AddCell(CreateCell(model.MedicalHistory ?? "N/A"));
                table.AddCell(CreateCell("Medication"));
                table.AddCell(CreateCell(model.Medication ?? "N/A"));
                table.AddCell(CreateCell("Allergies"));
                table.AddCell(CreateCell(model.Allergies ?? "N/A"));
                table.AddCell(CreateCell("Temp"));
                table.AddCell(CreateCell(model.Temp?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("HR"));
                table.AddCell(CreateCell(model.HR?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("RR"));
                table.AddCell(CreateCell(model.RR?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("Blood pressure(Systolic)"));
                table.AddCell(CreateCell(model.BPs?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("Blood pressure(Diastolic)"));
                table.AddCell(CreateCell(model.BPd?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("O2"));
                table.AddCell(CreateCell(model.O2?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("Pain"));
                table.AddCell(CreateCell(model.Pain?.ToString() ?? "N/A"));
                table.AddCell(CreateCell("Heent"));
                table.AddCell(CreateCell(model.Heent ?? "N/A"));
                table.AddCell(CreateCell("CV"));
                table.AddCell(CreateCell(model.CV ?? "N/A"));
                table.AddCell(CreateCell("Chest"));
                table.AddCell(CreateCell(model.Chest ?? "N/A"));
                table.AddCell(CreateCell("ABD"));
                table.AddCell(CreateCell(model.ABD ?? "N/A"));
                table.AddCell(CreateCell("Extr"));
                table.AddCell(CreateCell(model.Extr ?? "N/A"));
                table.AddCell(CreateCell("Skin"));
                table.AddCell(CreateCell(model.Skin ?? "N/A"));
                table.AddCell(CreateCell("Neuro"));
                table.AddCell(CreateCell(model.Neuro ?? "N/A"));
                table.AddCell(CreateCell("Other"));
                table.AddCell(CreateCell(model.Other ?? "N/A"));
                table.AddCell(CreateCell("Diagnosis"));
                table.AddCell(CreateCell(model.Diagnosis ?? "N/A"));
                table.AddCell(CreateCell("Treatment Plan"));
                table.AddCell(CreateCell(model.TreatmentPlan ?? "N/A"));
                table.AddCell(CreateCell("Medications Dispended"));
                table.AddCell(CreateCell(model.MedicationsDispended ?? "N/A"));
                table.AddCell(CreateCell("Procedure"));
                table.AddCell(CreateCell(model.Procedure ?? "N/A"));
                table.AddCell(CreateCell("Followup"));
                table.AddCell(CreateCell(model.Followup ?? "N/A"));
                table.AddCell(CreateCell("Is Finaled"));
                table.AddCell(CreateCell(model.isFinaled.ToString() ?? "N/A"));

                // Add the table to the PDF
                pdf.Add(table);

                pdf.Close();
                writer.Close();

                var bytes = memoryStream.ToArray();
                var result = new FileContentResult(bytes, "application/pdf");
                result.FileDownloadName = "Encounter_" + model.RequestId + ".pdf";
                return result;
            }
        }
        public IActionResult MyProfile()
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            var physiciandata = _context.Physicians.FirstOrDefault(p => p.Physicianid == physicianid);

            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == physiciandata.Aspnetuserid);
            var rolelist = _context.Aspnetroles.ToList();
            var regionlist = _context.Regions.ToList();
            var selectedregionlist = _context.Physicianregions.ToList().Where(a => a.Physicianid == physicianid).ToList();
            var model = new ProviderData
            {
                providerid = physiciandata.Physicianid,
                UserName = physiciandata.Firstname + " " + physiciandata.Lastname,
                password = aspnetuser.Passwordhash,
                role = rolelist,
                regions = _context.Regions.ToList(),
                firstname = physiciandata.Firstname,
                lastname = physiciandata.Lastname,
                email = physiciandata.Email,
                phonenumber = physiciandata.Mobile,
                address1 = physiciandata.Address1,
                address2 = physiciandata.Address2,
                city = physiciandata.City,
                zip = physiciandata.Zip,
                alterphonenumber = physiciandata.Altphone,
                phyregionlist = selectedregionlist,
                BusinessName = physiciandata.Businessname,
                BusinessWebsite = physiciandata.Businesswebsite,
                UploadPhoto = physiciandata.Photo,
                UploadSign = physiciandata.Signature,
                AdminNotes = physiciandata.Adminnotes,
                NPINumber = physiciandata.Npinumber,
                MedicalLicence = physiciandata.Medicallicense,
                SynchronizeEmail = physiciandata.Syncemailaddress,
                IsAgreementDoc = physiciandata.Isagreementdoc,
                IsCredentialDoc = physiciandata.Iscredentialdoc,
                IsBackgroundDoc = physiciandata.Isbackgrounddoc,
                IsLicenseDoc = physiciandata.Islicensedoc,
                IsNonDisclosureDoc = physiciandata.Isnondisclosuredoc
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePhysicianInfo(ProviderData p)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            var phydata = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianid);

            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == phydata.Aspnetuserid);

            aspnetuser.Passwordhash = p.password;

            TempData["success"] = "Password Updated Successfully";
            _context.Aspnetusers.Update(aspnetuser);
            _context.SaveChanges();

            return RedirectToAction("MyProfile");
        }

        public IActionResult CreateRequestPhysician()
        {
            return View();
        }

        public void Add(int id, List<IFormFile> formFiles)
        {
            foreach (var file in formFiles)
            {
                string filename = file.FileName;
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
                string extension = Path.GetExtension(filename);
                string filewith = filenameWithoutExtension + "_" + DateTime.Now.ToString("dd`MM`yyyy`HH`mm`ss") + extension;

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", file.FileName);


                string filePath = path;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                Requestwisefile requestwisefile = new Requestwisefile()
                {
                    Requestid = id,
                    Filename = filePath,
                    Createddate = DateTime.Now,
                };

                _context.Requestwisefiles.Add(requestwisefile);

            }
            _context.SaveChanges();
        }

        public IActionResult CreateRequestPhysicianSubmit(PatientInfo p)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            var phydata = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianid);
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(x => x.Id == phydata.Aspnetuserid);
            var user = _context.Users.FirstOrDefault(n => n.Email == p.Email);

            var region = _context.Regions.FirstOrDefault(x => x.Regionid == phydata.Regionid);
            var requestcount = (from m in _context.Requests where m.Createddate.Date == DateTime.Now.Date select m).ToList();
            string regiondata = _context.Regions.FirstOrDefault(a => a.Regionid == phydata.Regionid).Abbreviation;

            if (aspnetuser != null)
            {
                Request requests = new Request
                {
                    Firstname = phydata.Firstname,
                    Lastname = phydata.Lastname,
                    Email = phydata.Email,
                    Createddate = DateTime.Now,
                    Requesttypeid = 5,
                    Status = 1,
                    Phonenumber = phydata.Mobile,
                    Modifieddate = DateTime.Now,
                    Confirmationnumber = "NY000027RY7877"
                };
                if (user != null)
                {
                    requests.Userid = user.Userid;
                }
                _context.Requests.Add(requests);
                _context.SaveChanges();
                Requestclient requestclients = new Requestclient
                {
                    Firstname = p.FirstName,
                    Lastname = p.LastName,
                    Email = p.Email,
                    Phonenumber = p.PhoneNumber,
                    Street = p.Street,
                    City = p.City,
                    State = p.State,
                    Zipcode = p.ZipCode,
                    Requestid = requests.Requestid,
                    Regionid = (int)phydata.Regionid,
                    Notes = p.Symptoms,
                    Address = p.Street + " , " + p.City + " , " + p.State + " , " + p.ZipCode,
                    Intdate = int.Parse(p.DOB.ToString("dd")),
                    Intyear = int.Parse(p.DOB.ToString("yyyy")),
                    Strmonth = p.DOB.ToString("MMM"),
                };
                _context.Requestclients.Add(requestclients);
                _context.SaveChanges();

                if (p.Upload != null)
                {
                    Add(requests.Requestid, p.Upload);
                }
            }

            return RedirectToAction("ProviderDashboard");
        }

        public IActionResult ConcludeCare(int requestid)
        {
            var wisefileslist = _context.Requestwisefiles.ToList().Where(m => m.Isdeleted == null && m.Requestid == requestid).ToList();
            var requestclient = _context.Requestclients.FirstOrDefault(m => m.Requestid == requestid);
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            var requestnote = _context.Requestnotes.FirstOrDefault(m => m.Requestid == requestid);
            var model = new ViewUploadModel
            {
                totalFiles = wisefileslist,
                requestId = requestid,
                FirstName = requestclient.Firstname,
                LastName = requestclient.Lastname,
                ConfirmationNumber = request.Confirmationnumber,
            };
            if (requestnote != null)
            {
                model.Notes = requestnote.Physiciannotes;

            }
            var encounter = _context.Encounters.FirstOrDefault(m => m.RequestId == requestid);
            if (encounter != null)
            {
                model.isfinalize = encounter.IsFinalized.ToString();
            }
            return View(model);
        }
        public IActionResult ConcludeCareSubmit(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            request.Status = 8;
            _context.Requests.Update(request);
            _context.SaveChanges();
            return RedirectToAction("ProviderDashboard");
        }
        [HttpPost]
        public IActionResult PhysicianNotes(AdminDashboardViewModel obj)
        {
            _addOrUpdateRequestNotes.PhysicianRequestNotes(obj);
            return RedirectToAction("ProviderDashboard");
        }




        public IActionResult MyScheduling()
        {
            int physicianid = (int)HttpContext.Session.GetInt32("PhysicianId");

            SchedulingViewModel modal = new SchedulingViewModel();
            var phyregion = _context.Physicianregions.Where(m => m.Physicianid == physicianid).ToList();
            var list = new List<Region>();
            var repolist = _context.Regions.ToList();
            foreach (var item in phyregion)
            {
                var listin = repolist.Where(m => m.Regionid == item.Regionid).First();
                list.Add(listin);
            }
            modal.regions = list;
            return View(modal);
        }


        public IActionResult DeclineRequest(int requestid)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");

            var result = _unit._updateData.DeclineRequestTable(requestid, (int)physicianid);
            return RedirectToAction("ProviderDashboard");
        }

        [HttpPost]
        public IActionResult RequestAdminForEdit(ProviderData p)
        {
            sendEmailAndSMS.Sendemail(p.email, "request for edit profile", p.AdminNotes);
            sendEmailAndSMS.SendSMS();
            return RedirectToAction("MyProfile");
        }
    }
}
