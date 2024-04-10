using HelloDoc.DataModels;
using Services.Viewmodels;

namespace Services.Contracts
{
    public interface IVendorRepository
    {
        public List<Region> getRegionList();
        public VendorViewModel getVendorData();
        public VendorViewModel EditVendorData(int vendorid);
        public VendorViewModel getFilteredVendorData(int professionid,string search,int vendorid);

    }
}
