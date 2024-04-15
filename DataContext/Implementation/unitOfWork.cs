using Services.Contracts;

namespace Services.Implementation
{
    public class unitOfWork : IunitOfWork
    {
        public IVendorRepository vendor { get; private set; }
        public IRecordRepository records { get; private set; }
        public IRequestRepository _request { get; private set; }


        public unitOfWork(IVendorRepository Vendor,IRecordRepository Record, IRequestRepository request)
        {
            vendor = Vendor;
            records = Record;
            _request = request;
        }
    }
}
