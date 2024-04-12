using HelloDoc.DataModels;
using System.Collections;

namespace Services.Viewmodels
{
    public class RecordViewModel
    {
        public int requestid { get; set; }
        public string PatientName { get; set; }

        public string Requestor { get; set; }

        public string DateOfService { get; set; }

        public string CloseCaseDate { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public int RequestStatus { get; set; }
        public string Physician { get; set; }
        public string PhysicianNote { get; set; }
        public string CancelProviderNote { get; set; }
        public string AdminNote { get; set; }
        public string PatientNote { get; set; }

       
        public enum Requestby
        {
            first,
            Patient,
            Friend_Family,
            Concierge,
            Business_Partner,
            VIP
        }

       

        public string RequestTypeName(int by)
        {
            string By = ((Requestby)by).ToString();
            return By;
        } 
        
       
    }
    public class PatientHistoryViewModel
    {
        public List<User> UserList { get; set; }
    }
    public class PatientRecordViewModel
    {
        public string ClientName { get; set; }
        public string CreatedDate { get; set; }
        public string Confirmation { get; set; }
        public string ProviderName { get; set; }
        public string ConcludedDate { get; set; }
        public string Status { get; set; }
        public int RequestId { get; set; }
    }

    public class EmailLogViewModel
    {
        public string Recipint { get; set; }
        public string Action { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string Email { get; set; }
        public string CreatedDate { get; set; }
        public string? SentDate { get; set; }
        public BitArray Sent { get; set; }
        public string SentTries { get; set; }
        public string ConfirmationNumber { get; set; }
        public List<Emaillog> EmailLogList { get; set; }
        public List<Aspnetrole> AspNetRoleList { get; set; }
    }
}
