using Assignment.Models;
using DataModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentDataRepo _studentDataRepo;

        public HomeController(ILogger<HomeController> logger, IStudentDataRepo studentDataRepo)
        {
            _logger = logger;
            _studentDataRepo = studentDataRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchRecordsFilter()
        {
            var data = _studentDataRepo.GetStudentData();

            return PartialView("_StudentDetailTable", data);
        }

        public IActionResult openModal()
        {
            return PartialView("_EditAndAddStudentForm");
        }

        [HttpPost]
        public IActionResult AddStudent(string FirstName, string LastName, string Email, string DOB, string Gender, string Grade, string Course)
        {
            var check = _studentDataRepo.AddStudentData(FirstName, LastName, Email, DOB, Gender, Grade, Course);
            return RedirectToAction("SearchRecordsFilter");
        }

        public void AddCourseNotExist(string Course)
        {
            bool check = _studentDataRepo.CheckForExist(Course);
        }

        public IActionResult EditStudent(string id)
        {
            var getdatafromid = _studentDataRepo.GetStudentDataForEdit(id);

            return PartialView("_EditStuentPartial", getdatafromid);
        }

        [HttpPost]
        public IActionResult EditStudentSubmit(string FirstName, string LastName, string Email, string DOB, string Gender, string Grade, string Course,string id)
        {
            var check = _studentDataRepo.EditStudentData(FirstName, LastName, Email, DOB, Gender, Grade, Course,id);
            return RedirectToAction("SearchRecordsFilter");
        }

        public void DeleteStudent(string id)
        {
            var deletedata = _studentDataRepo.deleteStudent(id);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}