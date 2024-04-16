using HelloDoc;

namespace Services.Viewmodels
{
    public class MonthWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Shiftdetail> shiftdetails { get; set; }
        public List<Physician> physicians { get; set; }

    }
}
