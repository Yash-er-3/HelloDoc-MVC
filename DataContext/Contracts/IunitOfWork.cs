namespace Services.Contracts
{
    public interface IunitOfWork
    {
        public IVendorRepository vendor { get; }
        public IRecordRepository records { get; }
    }
}
