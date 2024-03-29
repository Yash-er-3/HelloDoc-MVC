using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Viewmodels;
using System;
using System.Collections;
using System.IO;

namespace HelloDoc.Controllers.Provider
{
    public class ProviderController : Controller
    {

        private readonly HelloDocDbContext _context;
        private readonly IWebHostEnvironment _env;


        public ProviderController(HelloDocDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult ProviderMenu()
        {
            ProviderData data = new ProviderData();
            data.regions = _context.Regions.ToList();
            data.physicians = _context.Physicians.ToList();
            return View(data);
        }

        [HttpPost]
        public IActionResult ContactProviderSubmit(string email, string message, string selectedoption)
        {

            if (selectedoption == "SMS")
            {
                SendEmailAndSMS.SendSMS();
            }
            else if (selectedoption == "Email")
            {
                SendEmailAndSMS.Sendemail(email, "Order for you", message);
            }
            else
            {
                SendEmailAndSMS.Sendemail(email, "Order for you", message);
                SendEmailAndSMS.SendSMS();
            }

            return RedirectToAction("Admindashboard", "Admin");
        }

        [HttpGet]
        public IActionResult EditProviderAccount(int providerid)
        {
            var adminid = HttpContext.Session.GetInt32("AdminId");
            var admin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(m => m.Id == admin.Aspnetuserid);
            var rolelist = _context.Aspnetroles.ToList();
            var regionlist = _context.Regions.ToList();
            var selectedregionlist = _context.Physicianregions.ToList().Where(a => a.Physicianid == providerid).ToList();
            var physiciandata = _context.Physicians.FirstOrDefault(p => p.Physicianid == providerid);
            var model = new ProviderData
            {
                providerid = physiciandata.Physicianid,
                UserName = physiciandata.Firstname + " " + physiciandata.Lastname,
                password = aspnetuser.Passwordhash,
                role = rolelist,
                regions = _context.Regions.ToList(),
                firstname = physiciandata.Firstname,
                lastname = physiciandata.Lastname,
                email = physiciandata.Email,
                phonenumber = physiciandata.Mobile,
                address1 = physiciandata.Address1,
                address2 = physiciandata.Address2,
                city = physiciandata.City,
                zip = physiciandata.Zip,
                alterphonenumber = physiciandata.Altphone,
                phyregionlist = selectedregionlist,
                BusinessName = physiciandata.Businessname,
                BusinessWebsite = physiciandata.Businesswebsite,
                UploadPhoto = physiciandata.Photo,
                UploadSign = physiciandata.Signature,
                AdminNotes = physiciandata.Adminnotes,
                NPINumber = physiciandata.Npinumber,
                MedicalLicence = physiciandata.Medicallicense,
                SynchronizeEmail = physiciandata.Syncemailaddress,
                IsAgreementDoc = physiciandata.Isagreementdoc,
                IsCredentialDoc = physiciandata.Iscredentialdoc,
                IsBackgroundDoc = physiciandata.Isbackgrounddoc,
                IsLicenseDoc = physiciandata.Islicensedoc,
                IsNonDisclosureDoc = physiciandata.Isnondisclosuredoc
            };
            return View(model);
        }

        [HttpPost]

        public IActionResult EditProviderPhoto(int providerid, string base64String)
        {
            var physiciandata = _context.Physicians.FirstOrDefault(p => p.Physicianid == providerid);
            physiciandata.Photo = base64String;

            _context.Physicians.Update(physiciandata);
            _context.SaveChanges();

            return RedirectToAction("EditProviderAccount", new { providerid = providerid });
        }

        [HttpPost]

        public IActionResult EditProviderSign(int providerid, string base64String)
        {
            var physiciandata = _context.Physicians.FirstOrDefault(p => p.Physicianid == providerid);
            physiciandata.Signature = base64String;

            _context.Physicians.Update(physiciandata);
            _context.SaveChanges();

            return RedirectToAction("EditProviderAccount", new { providerid = providerid });

        }

        [HttpPost]
        public IActionResult UpdatePhysicianInfo(ProviderData p)
        {

            var updatephyinfo = _context.Physicians.Include(r => r.Physicianregions).FirstOrDefault(m => m.Physicianid == p.providerid);
            var aspnetuser = _context.Aspnetusers.FirstOrDefault(m => m.Id == updatephyinfo.Aspnetuserid);

            //for account information edit
            if (p.UserName != null)
            {
                aspnetuser.Username = p.UserName;
                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
            }

            //for physician info edit
            if (p.firstname != null)
            {
                var addphyregion = new Physicianregion();
                List<int> phyRegion = updatephyinfo.Physicianregions.Select(m => m.Regionid).ToList();
                var RegionToDelete = phyRegion.Except(p.selectedregion);
                foreach (var item in RegionToDelete)
                {
                    Physicianregion? phyRegionToDelete = _context.Physicianregions.FirstOrDefault(ar => ar.Physicianid == p.providerid && ar.Regionid == item);

                    if (phyRegionToDelete != null)
                    {
                        _context.Physicianregions.Remove(phyRegionToDelete);
                    }
                }
                IEnumerable<int> regionsToAdd = p.selectedregion.Except(phyRegion);

                foreach (int item in regionsToAdd)
                {
                    Physicianregion newPhyRegion = new Physicianregion
                    {
                        Physicianid = p.providerid,
                        Regionid = item,
                    };
                    _context.Physicianregions.Add(newPhyRegion);
                }

                if (updatephyinfo != null)
                {
                    updatephyinfo.Firstname = p.firstname;
                    updatephyinfo.Lastname = p.lastname;
                    updatephyinfo.Email = p.email;
                    updatephyinfo.Mobile = p.phonenumber;
                    updatephyinfo.Medicallicense = p.MedicalLicence;
                    updatephyinfo.Npinumber = p.NPINumber;
                    updatephyinfo.Syncemailaddress = p.SynchronizeEmail;
                }

                _context.Physicians.Update(updatephyinfo);
                _context.SaveChanges();

            }


            //for mailing billing information edit

            if (p.address1 != null && updatephyinfo != null)
            {

                updatephyinfo.Address1 = p.address1;
                updatephyinfo.Address2 = p.address2;
                updatephyinfo.City = p.city;
                updatephyinfo.Altphone = p.alterphonenumber;
                updatephyinfo.Zip = p.zip;

                if (updatephyinfo != null)
                {

                    _context.Physicians.Update(updatephyinfo);
                    _context.SaveChanges();
                }

            }

            //for provider profile edit

            if (p.BusinessName != null && updatephyinfo != null)
            {
                updatephyinfo.Businessname = p.BusinessName;
                updatephyinfo.Businesswebsite = p.BusinessWebsite;
                updatephyinfo.Adminnotes = p.AdminNotes;

                if (updatephyinfo != null)
                {

                    _context.Physicians.Update(updatephyinfo);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("EditProviderAccount", new { providerid = p.providerid });
        }

        public IActionResult uploadFile(IFormFile file, int providerid, string onboardinguploadvalue)
        {
            if (file != null && file.Length > 0)
            {

                string extension = Path.GetExtension(file.FileName);
                string filename = onboardinguploadvalue + extension;

                string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "onboarding", providerid.ToString());

                if (!Directory.Exists(folderpath))
                    Directory.CreateDirectory(folderpath);

                string uploadFile = Path.Combine(folderpath, filename);

                using (var fileStream = new FileStream(uploadFile, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }



                Physician physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == providerid);
                

                BitArray bitset = new BitArray(1);

                // Set some bits
                bitset[0] = true; // Set the first bit to 1

                if (onboardinguploadvalue == "IndependentContractorAgreement")
                {
                    physician.Isagreementdoc = bitset;
                }
                else if (onboardinguploadvalue == "BackgroundCheck")
                {
                    physician.Isbackgrounddoc = bitset;

                }
                else if (onboardinguploadvalue == "HIPAACompliance")
                {
                    physician.Iscredentialdoc = bitset;

                }
                else if (onboardinguploadvalue == "Non-DisclosureAgreement")
                {
                    physician.Isnondisclosuredoc = bitset;

                }
                else if (onboardinguploadvalue == "LicenseDocument")
                {
                    physician.Islicensedoc = bitset;

                }

                _context.Update(physician);
                _context.SaveChanges();
            }

            return RedirectToAction("EditProviderAccount",new {providerid = providerid});
        }
    }
}
