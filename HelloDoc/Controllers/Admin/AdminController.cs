using HelloDoc.DataContext;
using HelloDoc.DataModels;
using HelloDoc.ViewModels;
using HelloDoc.Views.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Viewmodels;
using static Services.Viewmodels.allrequestdataViewModel;

namespace HelloDoc.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IRequestRepository _request;
        private readonly IRequestDataRepository _data;
        private readonly IViewCaseRepository _view;
        private readonly HelloDocDbContext _context;
        private readonly IBlockCaseRepo _blockCaseRepo;
        private readonly IAddOrUpdateRequestStatusLog _addOrUpdateRequestStatusLog;
        private readonly IAddOrUpdateRequestNotes _addOrUpdateRequestNotes;


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
        public ActionResult Admin()
        {
            return View();
        }

        public IActionResult Admindashboard()
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

        public IActionResult NewState()
        {
            var model = _data.GetAllRequestData(1);
            return View(model);
        }
        public IActionResult PendingState()
        {
            var model = _data.GetAllRequestData(2);
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

            return RedirectToAction("ViewCase", request);
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
        public IActionResult cancelCaseModal(int requestid, AdminDashboardViewModel note)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            request.Status = 3;
            _context.Requests.Update(request);
            _context.SaveChanges();
            var adminid = HttpContext.Session.GetInt32("AdminId");
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(requestid, adminid, note.blocknotes);
            return RedirectToAction("Admindashboard");
        }

        [HttpPost]
        public IActionResult assignCaseModal(int requestid, AdminDashboardViewModel note, string physicianname)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == requestid);
            var physician = _context.Physicians.FirstOrDefault(m => m.Firstname + m.Lastname == physicianname);
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();
            var adminid = HttpContext.Session.GetInt32("AdminId");
            _addOrUpdateRequestStatusLog.AddOrUpdateRequestStatusLog(requestid, adminid, note.blocknotes, physician.Physicianid);
            return RedirectToAction("Admindashboard");
        }

        public IActionResult ViewUpload(int requestid)
        {
            var filelist = _context.Requestwisefiles.ToList().Where(m => m.Requestid == requestid).ToList();
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
        public IActionResult UploadButton(ViewUploadModel document)
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                patient_dashboard model = new patient_dashboard();
                var users = _context.Users.FirstOrDefault(m => m.Userid == id);
                model.requests = (from m in _context.Requests where m.Userid == id select m).ToList();
              
                //var req = Context.Requests.FirstOrDefault(m => m.UserId == id);
                model.requestwisefile = (from m in _context.Requestwisefiles where m.Requestid == document.requestId select m).ToList();
                //var reqe = Context.Requests.FirstOrDefault(m => m.UserId == id)

                model.requestid = document.requestId;

                if (document.upload != null)
                {
                    uploadFile(document);
                }
            }
                return RedirectToAction("ViewUpload" , new { requestid  = document.requestId});
          
        }
        [HttpPost]
        public void uploadFile(ViewUploadModel document)
        {

            foreach (var item in document.upload)

            {

                //string path = _environment.WebRootPath + "/UploadDocument/" + item.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", item.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    item.CopyTo(fileStream);
                }

                Requestwisefile requestWiseFiles = new Requestwisefile
                {
                    Requestid = document.requestId,
                    Filename = path,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(requestWiseFiles);
                _context.SaveChanges();

            }
        }


    }
}
