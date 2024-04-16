using HelloDoc;
using Services.Viewmodels;

namespace Services.Contracts
{
    public interface IRecordRepository
    {
        public List<RecordViewModel> GetSearchRecordData(int reqstatus, string patientname, int requesttype, string fromdateofservice,
        string todateofservice, string physicianname, string email, string phonenumber);
        public List<RecordViewModel> GetBlockHistoryFilterData(string name, string date, string email, string phonenumber);


        public int DeleteSearchRecord(int requestid);
        public int UnblockBlockHistory(int requestid, int adminid);
        public List<User> GetUserList(string firstname, string lastname, string email, int phone);
        public List<PatientRecordViewModel> GetPatientRecordData(int userid);
        public List<EmailLogViewModel> GetEmailLogs(int Role, string ReceiverName, string Email, DateTime CreatedDate, DateTime SentDate);
        public List<EmailLogViewModel> GetSMSLogs(int Role, string ReceiverName, string Email, DateTime CreatedDate, DateTime SentDate);
        public EmailLogViewModel GetAspNetRoleList();

    }
}
