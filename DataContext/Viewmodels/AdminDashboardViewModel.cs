using HelloDoc.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class AdminDashboardViewModel
    {
        public List<Request> requests { get; set; }
        public List<Casetag> casetags{ get; set; }
        public List<Physician> physicians{ get; set; }
        public List<Region> regions{ get; set; }

        [MaybeNull]
        public string blocknotes { get; set; }
        public string physicianname { get; set; }
        public string adminname{ get; set; }
        public int requestid { get; set; }
        public DateTime assignTime { get; set; }
    }
}
