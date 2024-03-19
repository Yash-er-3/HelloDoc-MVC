using HelloDoc.DataContext;
using HelloDoc.DataModels;
using HelloDoc.ViewModels;
using HelloDoc.Views.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Viewmodels;
using System.Collections;
using System.Net.Mail;
using System.Net;
using static Services.Viewmodels.allrequestdataViewModel;
using DataAccess.ServiceRepository;
using Microsoft.EntityFrameworkCore;
using Services.Implementation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using Twilio.Http;

namespace HelloDoc.Controllers.Admin
{
    [AuthorizationRepository("Admin")]

    public class AdminController : Controller
    {
        private readonly IRequestRepository _request;
        private readonly IRequestDataRepository _data;
        private readonly IViewCaseRepository _view;
        private readonly HelloDocDbContext _context;
        private readonly IBlockCaseRepo _blockCaseRepo;
        private readonly IAddOrUpdateRequestStatusLog _addOrUpdateRequestStatusLog;
        private readonly IAddOrUpdateRequestNotes _addOrUpdateRequestNotes;

        byte[] key = { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
        byte[] iv = { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };


        public AdminController(IRequestRepository requestRepository, IRequestDataRepository requestDataRepository, IViewCaseRepository view, HelloDocDbContext context,
            IBlockCaseRepo blockCaseRepo, IAddOrUpdateRequestStatusLog addOrUpdateRequestStatusLog, IAddOrUpdateRequestNotes addOrUpdateRequestNotes)
        {
            _request = requestRepository;
            _data = requestDataRepository;
            _view = view;
            _context = context;
            _blockCaseRepo = blockCaseRepo;
            _addOrUpdateRequestStatusLog = addOrUpdateRequestStatusLog;
            _addOrUpdateRequestNotes = addOrUpdateRequestNotes;
        }
        // GET: AdminController



        public IActionResult Admindashboard()
        {
            if (HttpContext.Session.GetInt32("AdminId") != null)
            {

                var requests = _request.GetAll().ToList();
                var region = _context.Regions.ToList();
                var casetag = _context.Casetags.ToList();
                var physician = _context.Physicians.ToList();

                AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
                adminDashboardViewModel.requests = requests;
                adminDashboardViewModel.regions = region;
                adminDashboardViewModel.casetags = casetag;
                return View(adminDashboardViewModel);
            }
            return RedirectToAction("Admin", "CredentialAdmin");
        }

        [HttpGet]
        public List<allrequestdataViewModel> ExportAllDownload()
        {
            var alldata = _data.GetAllExportData();
            return alldata;
        }



        public IActionResult NewState()
        {
            var model = _data.GetAllRequestData(1);
            return View(model);
        }


        public IActionResult PendingState()
        {
            var model = _data.GetAllRequestData(2);
            foreach (var item in model)
            {
                var physician = _context.Physicians.FirstOrDefault(m => m.Physicianid == item.PhysicianId);
                item.PhysicianName = physician.Firstname;
            };
            return View(model);
        }
        public IActionResult ActiveState()
        {
            var model = _data.GetAllRequestData(4).Concat(_data.GetAllRequestData(5)).ToList();

            return View(model);
        }
        public IActionResult ConcludeState()
        {
            var model = _data.GetAllRequestData(6);

            return View(model);
        }
        public IActionResult ToCloseState()
        {
            var model = _data.GetAllRequestData(3).Concat(_data.GetAllRequestData(7)).Concat(_data.GetAllRequestData(8)).ToList();

            return View(model);
        }
        public IActionResult UnpaidState()
        {
            var model = _data.GetAllRequestData(9);

            return View(model);
        }

        [HttpGet]
        public IActionResult ViewCase(int id)
        {
            var request = _view.GetViewCaseData(id);
            return View(request);
        }

        public IActionResult Edit(ViewCaseView viewcasedata)
        {
            _view.EditViewCaseData(viewcasedata);

            var request = _context.Requests.FirstOrDefault(m => m.Confirmationnumber == viewcasedata.ConfirmationNumber);

            return RedirectToAction("ViewCase", new { id = request.Requestid });
        }

        public IActionResult cancelCase(String number)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Confirmationnumber == number);
            request.Status = 3;
            _context.Requests.Update(request);
            _context.SaveChanges();
            return RedirectToAction("Admindashboard");
        }

        public IActionResult ViewNotes(int id)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == id);
            var requestnote = _context.Requestnotes.FirstOrDefault(m => m.Requestid == id);

            var viewnote = new AdminDashboardViewModel();
            if (requestnote != null)
            {

                viewnote.blocknotes = requestnote.Adminnotes;
            }

            var adminname = HttpContext.Session.GetString("UserName");
            viewnote.requestid = id;
            var transfernotedetail = _context.Requeststatuslogs.FirstOrDefault(m => m.Requestid == id && m.Status == 2);
            if (transfernotedetail != null)
            {
                var physicianname = _context.Physicians.FirstOrDefault(m => m.Physicianid == transfernotedetail.Transtophysicianid);
                viewnote.physicianname = physicianname.Firstname;
                viewnote.adminname = adminname;
                viewnote.assignTime = transfernotedetail.Createddate;

            }
            return View(viewnote);
        }
        [HttpPost]
        public IActionResult ViewNotes(AdminDashboardViewModel item)
        {
            _addOrUpdateRequestNotes.addOrUpdateRequestNotes(item);
            return RedirectToAction("Admindashboard");

        }
        public IActionResult BlockCaseModal(int requestid, string blocknotes)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            _blockCaseRepo.BlockCaseData(requestid, blocknotes);
            return RedirectToAction("Admindashboard");
        }
        [HttpGet]
        public List<Physician> GetPhysicianByRegionId(int regionid)
        {
            var physician = _context.Physicians.ToList().Where(m => m.Regionid == regionid).ToList();
            return physician;
        }

        [HttpPost]
        public IActionResult cancelCaseModal(int id, AdminDashboardViewModel note)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == id);
            request.Status = 3;
            _context.Requests.Update(request);
            _context.SaveChanges();
            var adminid = HttpContext.Session.GetInt32("AdminId");
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(id, adminid, note.blocknotes);
            return RedirectToAction("Admindashboard");
        }

        [HttpPost]
        public IActionResult assignCaseModal(int requestid, AdminDashboardViewModel note, string physicianname)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            var physician = _context.Physicians.FirstOrDefault(m => m.Firstname + m.Lastname == physicianname);
            request.Status = 2;
            request.Physicianid = physician.Physicianid;
            _context.Requests.Update(request);
            _context.SaveChanges();
            var adminid = HttpContext.Session.GetInt32("AdminId");
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(requestid, adminid, note.blocknotes, physician.Physicianid);
            return RedirectToAction("Admindashboard");
        }



        public IActionResult ViewUpload(int requestid)
        {
            var filelist = _context.Requestwisefiles.ToList().Where(m => m.Requestid == requestid && m.Isdeleted == null).ToList();
            var requestclientdata = _context.Requestclients.FirstOrDefault(m => m.Requestid == requestid);
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);

            var modeldata = new ViewUploadModel
            {
                totalFiles = filelist,
                requestId = requestid,
                FirstName = requestclientdata.Firstname,
                LastName = requestclientdata.Lastname,
                ConfirmationNumber = request.Confirmationnumber
            };
            return View(modeldata);
        }

        [HttpPost]
        public IActionResult UploadFiles(List<IFormFile> files, int RequestsId)
        {
            Add(RequestsId, files);
            return RedirectToAction("ViewUpload", "Admin", new { requestid = RequestsId });
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

        [HttpPost]
        public IActionResult DeleteDoc(int id, int reqid)
        {
            var file = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == id);
            BitArray r = new BitArray(1);
            r[0] = true;
            file.Isdeleted = r;
            _context.Requestwisefiles.Update(file);
            _context.SaveChanges();

            return RedirectToAction("ViewUpload", new { requestid = reqid });
        }


        [HttpPost]
        public IActionResult SendMail(List<int> wiseFileId, int reqid)
        {
            List<string> filenames = new List<string>();
            foreach (var item in wiseFileId)
            {
                var s = (item);
                var file = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == s).Filename;
                filenames.Add(file);
            }

            Sendemail("yashsarvaiya40@gmail.com", "Your Attachments", "Please Find Your Attachments Here", filenames);

            TempData["success"] = "Email sent successfully!";
            return RedirectToAction("ViewUpload", new { requestid = reqid });

        }
        public async Task Sendemail(string email, string subject, string message, List<string> attachmentPaths)
        {
            try
            {
                var mail = "tatva.dotnet.yashsarvaiya@outlook.com";
                var password = "Yash@1234";

                var client = new SmtpClient("smtp.office365.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mail, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(mail),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true // Set to true if your message contains HTML
                };

                mailMessage.To.Add(email);

                foreach (var attachmentPath in attachmentPaths)
                {
                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        var attachment = new Attachment(attachmentPath);
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                await client.SendMailAsync(mailMessage);
                TempData["success"] = "Email sent successfully!";

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        public IActionResult Orders(int requestid)
        {
            var professiontypeList = _context.Healthprofessionaltypes.ToList();
            OrderModel orderdata = new OrderModel();
            orderdata.requestid = requestid;
            orderdata.Healthprofessionaltypes = professiontypeList;
            return View(orderdata);
        }

        [HttpPost]
        public IActionResult Orders(int requestid, int vendorid, int RefillNumber, string presription)
        {
            var venderDetails = _context.Healthprofessionals.FirstOrDefault(m => m.Vendorid == vendorid);
            var order = new Orderdetail
            {
                Vendorid = vendorid,
                Requestid = requestid,
                Faxnumber = venderDetails.Faxnumber,
                Email = venderDetails.Email,
                Businesscontact = venderDetails.Businesscontact,
                Createddate = DateTime.Now,
                Prescription = presription,
                Noofrefill = RefillNumber,
                Createdby = HttpContext.Session.GetString("AdminName"),
            };
            if (order != null)
            {
                _context.Orderdetails.Add(order);
                _context.SaveChanges();
                TempData["success"] = "Order Placed Successfully";
            }
            return RedirectToAction("NewState", "Admin");
        }

        [HttpGet]
        public List<Healthprofessional> GetBusiness(int healthprofessionId)
        {
            var businessList = _context.Healthprofessionals.ToList().Where(m => m.Profession == healthprofessionId).ToList();

            return businessList;
        }

        public OrderModel GetVendorDetail(int vendorid)
        {
            var vendordetails = _context.Healthprofessionals.FirstOrDefault(m => m.Vendorid == vendorid);
            var orderdata = new OrderModel();
            orderdata.FaxNumber = vendordetails.Faxnumber;
            orderdata.Email = vendordetails.Email;
            orderdata.BusinessContact = vendordetails.Businesscontact;
            return orderdata;
        }

        public IActionResult ClearModal(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);

            if (request != null)
            {
                request.Status = 10;
                _context.Requests.Update(request);
                _context.SaveChanges();
            }

            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(requestid, HttpContext.Session.GetInt32("AdminId"));

            return RedirectToAction("Admindashboard");
        }

        [HttpGet]
        public JsonResult GetAgreementData(int requestid)
        {
            var requestclient = _context.Requests.Include(r => r.Requestclients).FirstOrDefault(x => x.Requestid == requestid);
            var phonenumber = requestclient.Requestclients.ElementAt(0).Phonenumber;
            var email = requestclient.Requestclients.ElementAt(0).Email;
            var requesttype = requestclient.Requesttypeid;
            var result = new
            {
                phonenumber = phonenumber,
                email = email,
                requesttype = requesttype
            };
            return Json(result);
        }




        [HttpPost]
        public IActionResult SendAgreementModal(int requestid, string email)
        {

            string AgreementUrl = GenerateAgreementUrl(requestid);
            SendEmail(email, "Confirm Your Agreement", $"Hello, Click On below Link for COnfirm Agreement: {AgreementUrl}");
            SendEmailAndSMS.SendSMS();

            TempData["success"] = "Agreement sent in Email..!";
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult ReviewAgreement(string id)
        {
            var requestid = int.Parse(EncryptDecrypt.Decrypt(id));
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            var model = new AdminDashboardViewModel
            {
                requestid = requestid,
                PatientNameForAgreement = request.Firstname + " " + request.Lastname
            };
            return View(model);
        }



        private string GenerateAgreementUrl(int reqid)
        {
            var link = "https://localhost:44300/Admin/ReviewAgreement/?id=" + EncryptDecrypt.Encrypt(reqid.ToString());
            return link;
        }
        public IActionResult SendAgreement(string id)
        {
            var viewModel = new AdminDashboardViewModel();
            var requestid = EncryptDecrypt.Encrypt(id);
            viewModel.requestid = int.Parse(requestid);
            return View(viewModel);
        }

        private Task SendEmail(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.yashsarvaiya@outlook.com";
            var password = "Yash@1234";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }

        public IActionResult IAgreeSendAgreement(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            request.Status = 4;
            _context.Requests.Update(request);
            _context.SaveChanges();
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(requestid);
            return RedirectToAction("PatientDashboard", "Patient");
        }


        [HttpGet]

        public IActionResult CloseCase(int requestid)
        {
            var filelist = _context.Requestwisefiles.ToList().Where(m => m.Requestid == requestid && m.Isdeleted == null).ToList();
            var requestclientdata = _context.Requestclients.FirstOrDefault(m => m.Requestid == requestid);
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);

            var modeldata = new ViewUploadModel
            {
                totalFiles = filelist,
                requestId = requestid,
                FirstName = requestclientdata.Firstname,
                LastName = requestclientdata.Lastname,
                ConfirmationNumber = request.Confirmationnumber,
                PatientDOB = new DateTime(Convert.ToInt32(requestclientdata.Intyear), DateTime.ParseExact(requestclientdata.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(requestclientdata.Intdate)),
                Email = requestclientdata.Email,
                PhoneNumber = requestclientdata.Phonenumber
            };
            return View(modeldata);
        }

        public IActionResult EditCloseCase(ViewUploadModel obj, int requestid)
        {
            if (obj.Email != null)
            {
                var request = _context.Requestclients.FirstOrDefault(m => m.Requestid == obj.requestId);
                request.Email = obj.Email;
                request.Phonenumber = obj.PhoneNumber;
                _context.Requestclients.Update(request);
                _context.SaveChanges();
                return RedirectToAction("CloseCase", new { requestid = obj.requestId });
            }
            return RedirectToAction("CloseCase", new { requestid = requestid });

        }
        [HttpPost]
        public IActionResult CloseCase(ViewUploadModel obj)
        {

            var request = _context.Requests.FirstOrDefault(m => m.Requestid == obj.requestId);
            request.Status = 9;
            var adminid = HttpContext.Session.GetInt32("UserId");
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(obj.requestId, adminid);
            _context.Requests.Update(request);
            _context.SaveChanges();
            return RedirectToAction("Admindashboard");
        }
        public void EncounterSubmit(int requestid, string encountervalue)
        {
            var requestdata = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            if (encountervalue == "Housecall" && requestdata != null)
            {
                requestdata.Status = 6;
                _context.Requests.Update(requestdata);
                _context.SaveChanges();
            }

        }


        public IActionResult AdminProfile()
        {


            var adminid = HttpContext.Session.GetInt32("AdminId");
            var admin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(m => m.Id == admin.Aspnetuserid);
            var rolelist = _context.Aspnetroles.ToList();
            var regionlist = _context.Regions.ToList();
            var selectedregionlist = _context.Adminregions.ToList().Where(a => a.Adminid == adminid).ToList();
            var model = new UserAllDataViewModel
            {
                UserName = aspnetuser.Username,
                password = aspnetuser.Passwordhash,
                status = admin.Status,
                role = rolelist,
                firstname = admin.Firstname,
                lastname = admin.Lastname,
                email = admin.Email,
                confirmationemail = admin.Email,
                phonenumber = admin.Mobile,
                regionlist = regionlist,
                address1 = admin.Address1,
                address2 = admin.Address2,
                zip = admin.Zip,
                alterphonenumber = admin.Altphone,
                adminregionlist = selectedregionlist,
            };
            return View(model);
        }

        public IActionResult MailingBillingEditProfileAdmin(UserAllDataViewModel u)
        {
            var adminid = HttpContext.Session.GetInt32("AdminId");
            var admin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);

            admin.Address1 = u.address1;
            admin.Address2 = u.address2;
            admin.Altphone = u.alterphonenumber;
            admin.Zip = u.zip;

            _context.Admins.Update(admin);
            _context.SaveChanges();
            return RedirectToAction("AdminProfile");
        }

        [HttpPost]
        public IActionResult UpdateAdministrationInfoAdminProfile(UserAllDataViewModel model)
        {
            var adminid = HttpContext.Session.GetInt32("AdminId");

            var admin = _context.Admins.Include(r => r.Adminregions).FirstOrDefault(m => m.Adminid == adminid);
            var addadminregion = new Adminregion();
            List<int> adminRegion = admin.Adminregions.Select(m => m.Regionid).ToList();
            var RegionToDelete = adminRegion.Except(model.selectedregion);
            foreach (var item in RegionToDelete)
            {
                Adminregion? adminRegionToDelete = _context.Adminregions
            .FirstOrDefault(ar => ar.Adminid == adminid && ar.Regionid == item);

                if (adminRegionToDelete != null)
                {
                    _context.Adminregions.Remove(adminRegionToDelete);
                }
            }
            IEnumerable<int> regionsToAdd = model.selectedregion.Except(adminRegion);

            foreach (int item in regionsToAdd)
            {
                Adminregion newAdminRegion = new Adminregion
                {
                    Adminid = (int)adminid,
                    Regionid = item,
                };
                _context.Adminregions.Add(newAdminRegion);
            }
            _context.SaveChanges();

            if (admin != null)
            {
                admin.Firstname = model.firstname;
                admin.Lastname = model.lastname;
                admin.Email = model.email;
                admin.Mobile = model.phonenumber;
            }
            _context.Admins.Update(admin);
            _context.SaveChanges();
            return RedirectToAction("AdminProfile");
        }


        //send link button
        private string GenerateSendLinkUrl(string name, string email, string phonenumber)
        {

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string AgreementPath = Url.Action("patient", "submitrequestforms", new { name = name, phonenumber = phonenumber, email = email });
            return baseUrl + AgreementPath;

        }
        public IActionResult SendLinkAdminModal(string name, string email, string phonenumber)
        {
            string SendLinkAdminUrl = GenerateSendLinkUrl(name, email, phonenumber);
            SendEmail(email, "CREATE REQUEST", $"Hello, Click On below Link for Creating Request: {SendLinkAdminUrl}");

            TempData["success"] = "Create Request Link Sent In Email!";
            return RedirectToAction("Admindashboard");
        }

        public IActionResult CreateRequestAdmin()
        {
            return View();
        }
    }
}
