using HelloDoc.Views.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Viewmodels;

namespace HelloDoc.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IRequestRepository _request;
        private readonly IRequestDataRepository _data;

        public AdminController(IRequestRepository requestRepository , IRequestDataRepository requestDataRepository) {
            _request = requestRepository;
            _data = requestDataRepository;
        }
        // GET: AdminController
        public ActionResult Admin()
        {
            return View();
        }

        public IActionResult Admindashboard()
        {
            var requests = _request.GetAll().ToList();
            AdminDashboardViewModel adminDashboardViewModel = new AdminDashboardViewModel();
            adminDashboardViewModel.requests = requests;
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
    }
}
