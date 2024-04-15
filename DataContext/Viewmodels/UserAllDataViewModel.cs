using HelloDoc.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class UserAllDataViewModel
    {

        public int adminid { get; set; }
        [Required]
        [RegularExpression(@"[a-zA-Z0-9]+", ErrorMessage = "Username is not valid")]
        public string UserName { get; set; }
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&/=?_.]).{7,15}$", ErrorMessage = "password should contain uppercase,lowercase,digit,special symbol and at least 8 character")]
        public string password { get; set; }

        public short? status { get; set; }

        public List<Aspnetrole> role { get; set; }

        [RegularExpression(@"[a-zA-Z]+", ErrorMessage = "firastname is not valid")]
        public string firstname { get; set; }

        [RegularExpression(@"[a-zA-Z]+", ErrorMessage = "lastname is not valid")]
        public string lastname { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string email { get; set; }
        [Compare("email")]
        public string confirmationemail { get; set; }

        public List<Region> regionlist { get; set; }

        [RegularExpression(@"[0-9]{10}", ErrorMessage = "please enter 10 digit number")]
        public string phonenumber { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        [RegularExpression(@"[0-9]{6}", ErrorMessage = "please enter 6 ZIP")]
        public string zip { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{10}", ErrorMessage = "please enter 10 digit number")]
        public string alterphonenumber { get; set; }

        public List<Adminregion> adminregionlist { get; set; }

        public int[] selectedregion { get; set; }
        public List<Role> rolelist { get; set; }
        public string selectedrole { get; set; }
        public string selectedstate { get; set; }


        //public string UserName { get; set; }
        //    public string password { get; set; }
        //    public short? status { get; set; }
        //    public List<Aspnetrole> role { get; set; }
        //    public string firstname { get; set; }
        //    public string lastname { get; set; }
        //    public string email { get; set; }
        //    public string confirmationemail { get; set; }
        //    public List<Region> regionlist { get; set; }
        //    public string phonenumber { get; set; }
        //    public string address1 { get; set; }
        //    public string address2 { get; set; }
        //    public string city { get; set; }
        //    public string state { get; set; }
        //    public string zip { get; set; }
        //    public string alterphonenumber { get; set; }
        //    public List<Adminregion> adminregionlist { get; set; }

        //    public int[] selectedregion { get; set; }
        //    public string selectedstate { get; set; }
        //    public string selectedrole { get; set; }

        //    public List<Role> rolelist { get; set; }
    }
}
