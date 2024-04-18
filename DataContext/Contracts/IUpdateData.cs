namespace Services.Contracts
{
    public interface IUpdateData
    {
        public int UpdateRequestTable(int requestid, short status);

        public int DeclineRequestTable(int requestid, int physicianid);


    }
}
