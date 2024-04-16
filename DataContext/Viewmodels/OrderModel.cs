using HelloDoc;

namespace Services.Viewmodels
{
    public class OrderModel
    {
        public List<Healthprofessionaltype> Healthprofessionaltypes { get; set; }

        public List<Healthprofessional> Healthprofessionals { get; set; }

        public int requestid { get; set; }

        public string BusinessContact { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string OrderDetails { get; set; }
    }
}
