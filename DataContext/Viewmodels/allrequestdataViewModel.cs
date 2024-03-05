using HelloDoc.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class allrequestdataViewModel
    {
        public string PatientName { get; set; }
        public DateOnly PatientDOB { get; set; }
        public string RequestorName { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PatientPhone { get; set; }
        public string RequestorPhone { get; set; }
        public string RequestorEmail { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public string ProviderEmail { get; set; }
        public string PatientEmail { get; set; }
        public int RequestType { get; set; }

        public int RequestId { get; set; }

        public enum Requestby
        {
            first,
            Patient,
            Friend_Family,
            Concierge,
            Business_Partner
        }
        public string RequestTypeName(int by)
        {
            string By = ((Requestby)by).ToString();
            return By;
        }

        public int status { get; set; }
    }
}
