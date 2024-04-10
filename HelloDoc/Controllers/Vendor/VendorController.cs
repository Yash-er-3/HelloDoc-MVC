using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Services.Viewmodels;
using Services.ViewModels;

namespace HalloDoc.Controllers.Vendor
{
    public class VendorController : Controller
    {
        private readonly IunitOfWork _unit;
        public VendorController(IunitOfWork unit, IVendorRepository Vendor)
        {
            _unit = unit;
        }

        public IActionResult Vendor()
        {
            VendorViewModel modal = new VendorViewModel();
            modal = _unit.vendor.getVendorData();

            return View(modal);
        }

        public IActionResult VendorFilter(int professionid,string search,int vendorid)
        {

            VendorViewModel modal = new VendorViewModel();

            if (professionid == 0 && search == null && vendorid == 0 )
            {
                modal = _unit.vendor.getVendorData();

            }
            else if (professionid != 0 || search != null || vendorid!=0 )
            {
                modal = _unit.vendor.getFilteredVendorData(professionid,search, vendorid);
            }
           

            return PartialView("_VendorTable", modal);

        }
        public IActionResult AddVendorAccount(int vendorid)
        {
            VendorViewModel modal = new VendorViewModel();

            if (vendorid == 0)
            {
                modal = _unit.vendor.EditVendorData(vendorid);
                return View(modal);
            }
            else
            {
                modal = _unit.vendor.EditVendorData(vendorid);
            }

            return View(modal);
        }

      
    }
}
