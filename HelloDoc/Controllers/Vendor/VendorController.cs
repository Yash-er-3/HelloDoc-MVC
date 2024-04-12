using HelloDoc.DataModels;
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

        public IActionResult Vendor(VendorViewModel formdata)
        {
            VendorViewModel modal = new VendorViewModel();

            if (formdata.vendorid != 0)
            {
                var editdata = _unit.vendor.EditVendorData(formdata);

                if (editdata == 1)
                {
                    TempData["success"] = "Business Updated Successfully";
                }
                else
                {
                    TempData["error"] = "Error!";

                }
            }
            else if(formdata.businessName != null)
            {
                var adddata = _unit.vendor.AddVendorData(formdata);
                if (adddata == 1)
                {
                    TempData["success"] = "Business Added Successfully";
                }
                else
                {
                    TempData["error"] = "Error!";

                }
            }

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
                modal = _unit.vendor.GetEditVendorData(vendorid);
                return View(modal);
            }
            else
            {
                modal = _unit.vendor.GetEditVendorData(vendorid);
            }

            return View(modal);
        }

      
    }
}
