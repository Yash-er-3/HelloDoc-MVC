using HelloDoc;
using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;

namespace HalloDoc.Controllers.ProviderLocation
{
    public class ProviderLocationController : Controller
    {
        private readonly HelloDocDbContext _context;
        public ProviderLocationController()
        {
            _context = new HelloDocDbContext();
        }
        public IActionResult ProviderLocation()
        {
            return View();
        }
        public List<ProviderLocationViewModel> GetLocations()
        {
            var physicianlocation = _context.Physicianlocations.ToList();
            var physician = _context.Physicians.ToList();
            List<ProviderLocationViewModel> locations = new List<ProviderLocationViewModel>();
            foreach (var physicianLocation in physicianlocation)
            {
                locations.Add(new ProviderLocationViewModel
                {
                    Photo = physician.FirstOrDefault(x => x.Physicianid == physicianLocation.Physicianid).Photo,
                    Lat = physicianLocation.Latitude,
                    Long = physicianLocation.Longitude,
                    Physicianid = physicianLocation.Physicianid,
                    Name = physician.FirstOrDefault(m => m.Physicianid == physicianLocation.Physicianid).Firstname + physician.FirstOrDefault(m => m.Physicianid == physicianLocation.Physicianid).Lastname,
                });
            }

            return locations.ToList();
        }
    }
}
