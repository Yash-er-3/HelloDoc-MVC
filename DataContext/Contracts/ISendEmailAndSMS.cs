namespace Services.Contracts
{
    public interface ISendEmailAndSMS
    {
        Task Sendemail(string email, string subject, string message);
        void SendSMS();
    }
}