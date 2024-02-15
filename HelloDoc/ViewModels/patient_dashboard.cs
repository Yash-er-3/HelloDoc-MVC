
using HelloDoc.DataModels;

namespace HelloDoc.ViewModels
{
    public class patient_dashboard
    {
        public User user { get; set; }

        public List<Request> requests { get; set; }

        public List<Requestwisefile> requestwisefile { get; set; }

        public List<IFormFile> fileName { get; set; }

        public int requestid { get; set; }

        enum statusName
        {
            january,
            Unassigned,
            Cancelled,
            MdEnRoute,
            MdOnSite,
            Closed,
            Clear,
            Unpaid
        }

        public string StatusFind(int id)
        {
            string sName = ((statusName)id).ToString();
            return sName;
        }

    }
}
