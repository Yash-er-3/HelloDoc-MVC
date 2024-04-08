using HelloDoc.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class SchedulingViewModel
    {

        public List<Region> regions { get; set; }
        public int regionid { get; set; }
        public int providerid { get; set; }
        public DateTime shiftdate { get; set; }
        public DateOnly shiftdateviewshift { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
       
        public int repeatcount { get; set; }
        public int shiftdetailsid { get; set; }

        public string regionname { get; set; }
        public string physicianname { get; set; }
        public string eventvalue { get; set; }


    }

    public class ShiftForReviewModel
    {
        public List<Region> regions { get; set; }

        public List <Shiftdetail> shiftreviewlist { get; set; }
        public List<Physician> physicians { get; set; }

        public int totalCount { get; set; }
    }
}
