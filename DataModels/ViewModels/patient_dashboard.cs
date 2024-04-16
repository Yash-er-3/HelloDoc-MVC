using Microsoft.AspNetCore.Http;

namespace HelloDoc.ViewModels
{
    public class patient_dashboard
    {
        public User user { get; set; }

        public List<Request> requests { get; set; }

        public List<Requestwisefile> requestwisefile { get; set; }

        public List<IFormFile> fileName { get; set; }

        public int requestid { get; set; }

        public DateTime DOB { get; set; }
        enum statusName
        {
            january,
            Unassigned,
            Accepted,
            MdEnRoute,
            MdOnSite,
            Conclude,
            Cancelled,
            CancelledByPatient,
            Closed,
            Unpaid,
            Clear
        }

        public string Confirmationnumber { get; set; }

        public string StatusFind(int id)
        {
            string sName = ((statusName)id).ToString();
            return sName;
        }

    }
}
