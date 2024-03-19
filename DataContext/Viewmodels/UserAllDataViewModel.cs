using HelloDoc.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class UserAllDataViewModel
    {
        public string UserName { get; set; }
        public string password { get; set; }
        public short? status { get; set; }
        public List<Aspnetrole> role { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string confirmationemail { get; set; }
        public List<Region> regionlist { get; set; }
        public string phonenumber { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string alterphonenumber { get; set; }
        public List<Adminregion> adminregionlist { get; set; }

        public int[] selectedregion { get; set; }
    }
}
