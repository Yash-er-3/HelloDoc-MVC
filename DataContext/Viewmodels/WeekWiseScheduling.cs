using HelloDoc;

namespace Services.Viewmodels
{
    public class WeekWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }

        public List<Shiftdetail> shiftdetails { get; set; }

    }
}
