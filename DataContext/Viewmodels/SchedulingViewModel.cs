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
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int repeatcount { get; set; }
        public int shiftid { get; set; }

    }
}
