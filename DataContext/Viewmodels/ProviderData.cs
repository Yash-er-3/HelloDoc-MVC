using HelloDoc.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Viewmodels
{
    public class ProviderData
    {

        public int providerid { get; set; }
        public List<Region> regions { get; set; }
        public List<Physician> physicians { get; set; }

        public string ContactMessage { get; set; }

        public string UserName { get; set; }
        public string password { get; set; }
        public short? status { get; set; }
        public List<Aspnetrole> role { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string SynchronizeEmail { get; set; }
        public string confirmationemail { get; set; }
        public string MedicalLicence { get; set; }
        public string phonenumber { get; set; }
        public string NPINumber { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string alterphonenumber { get; set; }
        public string BusinessWebsite { get; set; }
        public string BusinessName { get; set; }
        public string UploadSign { get; set; }
        public string UploadPhoto { get; set; }
        public string AdminNotes { get; set; }
        public List<Physicianregion> phyregionlist { get; set; }
        public List<Role> rolelist { get; set; }


        public BitArray IsAgreementDoc { get; set; }
        public BitArray IsBackgroundDoc { get; set; }
        public BitArray IsCredentialDoc { get; set; }
        public BitArray IsNonDisclosureDoc { get; set; }
        public BitArray IsLicenseDoc { get; set; }

        public int[] selectedregion { get; set; }
    }
}
