using HelloDoc.DataModels;

namespace Services.Viewmodels
{
    public class VendorViewModel
    {
        public string profession { get; set; }
        public string businessName { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string businesscontact { get; set; }
        public string phone { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string Zip { get; set; }


        public List<Region> regionlist { get; set; }
        public List<Healthprofessional> healthProfessionallist { get; set; }
        public List<Healthprofessionaltype> healthProfessionalTypelist { get; set; }
    }
}
