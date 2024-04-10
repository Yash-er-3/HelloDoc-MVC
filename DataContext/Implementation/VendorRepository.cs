using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using Services.Viewmodels;
using System.Collections;

namespace Services.Implementation
{
    public class VendorRepository : IVendorRepository
    {
        private readonly HelloDocDbContext _context;
        public VendorRepository()
        {
            _context = new HelloDocDbContext();
        }
        public List<Region> getRegionList()
        {
            var regions = _context.Regions.ToList();
            return regions;
        }
        public VendorViewModel getVendorData()
        {
            VendorViewModel model = new VendorViewModel();
            model.healthProfessionallist = _context.Healthprofessionals.Where(h => h.Isdeleted == new BitArray(new[] {false})).ToList();
            model.healthProfessionalTypelist = _context.Healthprofessionaltypes.ToList();
            model.regionlist = _context.Regions.ToList();
            return model;
        }

        public VendorViewModel getFilteredVendorData(int professionid, string search, int vendorid)
        {


            VendorViewModel model = new VendorViewModel();

            if (professionid != 0 && search != null)
            {
                model.healthProfessionallist = _context.Healthprofessionals.Where(h => h.Profession == professionid && h.Isdeleted == new BitArray(new[] { false }) && h.Vendorname.ToLower().Contains(search.ToLower())).ToList();

            }
            else if(professionid != 0)
            {
                model.healthProfessionallist = _context.Healthprofessionals.Where(h => h.Profession == professionid && h.Isdeleted == new BitArray(new[] { false })).ToList();

            }
            else if(search != null)
            {
                List<Healthprofessional> searchdata = _context.Healthprofessionals.Where(h => h.Vendorname.ToLower().Contains(search.ToLower()) &&  h.Isdeleted == new BitArray(new[] { false })).ToList();
                model.healthProfessionallist = searchdata;
            }
            

            if(vendorid != 0)
            {
                var x = _context.Healthprofessionals.FirstOrDefault(h => h.Vendorid == vendorid);
                x.Isdeleted = new BitArray(new[] { true });
                _context.Healthprofessionals.Update(x);
                _context.SaveChanges();

                model.healthProfessionallist = _context.Healthprofessionals.Where(h => h.Isdeleted == new BitArray(new[] { false })).ToList();

            }

            model.healthProfessionalTypelist = _context.Healthprofessionaltypes.ToList();
            return model;

        }

        public VendorViewModel EditVendorData(int vendorid)
        {

            if (vendorid != 0)
            {
                var rowdata = _context.Healthprofessionals.FirstOrDefault(h => h.Vendorid == vendorid);//for healthprofessional row data 
                var typeprofession = _context.Healthprofessionaltypes.FirstOrDefault(h => h.Healthprofessionalid == rowdata.Profession);

                VendorViewModel model = new VendorViewModel
                {
                    businessName = rowdata.Vendorname,
                    profession = typeprofession.Professionname,
                    fax = rowdata.Faxnumber,
                    email = rowdata.Email,
                    phone = rowdata.Phonenumber,
                    businesscontact = rowdata.Businesscontact,
                    street = rowdata.Address,
                    city = rowdata.City,
                    state = rowdata.State,
                    Zip = rowdata.Zip
                };

                model.healthProfessionalTypelist = _context.Healthprofessionaltypes.ToList();

                return model;

            }


            VendorViewModel mode = new VendorViewModel();
            if (vendorid == 0)
            {
                mode.healthProfessionalTypelist = _context.Healthprofessionaltypes.ToList();

                return mode;

            }
            return mode;
        }
    }
}
