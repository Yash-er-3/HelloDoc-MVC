using Services.Contracts;

namespace Services.Implementation
{
    public class unitOfWork : IunitOfWork
    {
        public IVendorRepository vendor { get; private set; }
        public IRecordRepository records { get; private set; }


        public unitOfWork(IVendorRepository Vendor,IRecordRepository Record)
        {
            vendor = Vendor;
            records = Record;
        }
    }
}
