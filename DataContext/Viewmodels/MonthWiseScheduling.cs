﻿using HelloDoc.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class MonthWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Shiftdetail> shiftdetails { get; set; }
        public List<Physician> physicians { get; set; }

    }
}
