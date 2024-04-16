using HelloDoc;
using Services.Viewmodels;

namespace Services.Contracts
{
    public interface IVendorRepository
    {
        public List<Region> getRegionList();
        public VendorViewModel getVendorData();
        public VendorViewModel GetEditVendorData(int vendorid);
        public VendorViewModel getFilteredVendorData(int professionid, string search, int vendorid);

        public int EditVendorData(VendorViewModel formdata);
        public int AddVendorData(VendorViewModel formdata);


    }
}
