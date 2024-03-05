namespace Services.Contracts
{
    public interface IAddOrUpdateRequestStatusLog
    {
        void AddOrUpdateRequestStatusLog(int requestid, int? APId, string cancelnote, int? transtophyid=null);
    }
}