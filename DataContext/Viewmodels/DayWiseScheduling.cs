using HelloDoc;

namespace Services.Viewmodels
{
    public class DayWiseScheduling
    {
        public int shiftid { get; set; }
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }
        public List<Shiftdetail> shiftdetails { get; set; }
    }
}
