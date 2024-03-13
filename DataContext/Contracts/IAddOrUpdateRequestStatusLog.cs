namespace Services.Contracts
{
    public interface IAddOrUpdateRequestStatusLog
    {
        void AddOrUpdateRequestStatusLog(int requestid, int? APId=null, string? cancelnote=null, int? transtophyid=null);
    }
}