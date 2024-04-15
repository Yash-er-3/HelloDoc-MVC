using HelloDoc.DataContext;
using HelloDoc.Views.Shared;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Viewmodels;

namespace HelloDoc.Controllers.ProviderSide
{
    public class ProviderSideController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IRequestDataRepository _data;

        private readonly IRequestRepository _request;

        public ProviderSideController(HelloDocDbContext context, IRequestRepository request, IRequestDataRepository requestDataRepository)
        {
            _context = context;
            _request = request;
            _data = requestDataRepository;
        }
        public IActionResult ProviderDashboard()
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


    }
}
