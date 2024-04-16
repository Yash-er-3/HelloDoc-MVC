using HelloDoc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using Services.Viewmodels;
using System.Collections;
using System.Globalization;

namespace Services.Implementation
{
    public class RecordRepository : IRecordRepository
    {
        private readonly HelloDocDbContext _context;

        public RecordRepository(HelloDocDbContext context)
        {
            _context = context;
        }

        //get and filter search record data
        public List<RecordViewModel> GetSearchRecordData(int reqstatus, string patientname, int requesttype, string fromdateofservice,
            string todateofservice, string physicianname, string email, string phonenumber)
        {
            var recorddata = _context.Requests.Where(x => x.Isdeleted == null).Include(r => r.Requestclients).Include(x => x.Physician).Include(r => r.Requeststatuslogs).Include(r => r.Requestnotes).ToList();

            //for request status search
            if (reqstatus != 0)
            {
                if (reqstatus == 45)
                {
                    recorddata = recorddata.Where(x => x.Status == 4 || x.Status == 5).ToList();

                }
                else if (reqstatus == 378)
                {
                    recorddata = recorddata.Where(x => x.Status == 3 || x.Status == 7 || x.Status == 8).ToList();

                }
                else
                {
                    recorddata = recorddata.Where(x => x.Status == reqstatus).ToList();
                }
            }

            //for patientname search
            if (patientname != null)
            {
                recorddata = recorddata.Where(x => (x.Requestclients.FirstOrDefault().Firstname +
                x.Requestclients.FirstOrDefault().Firstname)
                .ToLower()
                .Contains(patientname.ToLower()))
                .ToList();
            }

            //for requesttype search
            if (requesttype != 0)
            {
                recorddata = recorddata.Where(x => x.Requesttypeid == requesttype).ToList();
            }
            //for fromdateofservice search

            if (fromdateofservice != null)
            {
                DateTime dt = DateTime.ParseExact(fromdateofservice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                // Extract the day, month, and year
                int day = dt.Day;
                int month = dt.Month;
                int year = dt.Year;
                recorddata = recorddata
                .Where(x => x.Requestclients.FirstOrDefault().Intyear >= year
                && x.Requestclients.FirstOrDefault().Intdate >= day
                && DateTime.ParseExact(x.Requestclients.FirstOrDefault().Strmonth, "MMM", CultureInfo.InvariantCulture).Month >= month)
                .ToList();
            }

            //for todateofservice search

            if (todateofservice != null)
            {
                DateTime dt = DateTime.ParseExact(todateofservice, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                // Extract the day, month, and year
                int day = dt.Day;
                int month = dt.Month;
                int year = dt.Year;
                recorddata = recorddata
                .Where(x => x.Requestclients.FirstOrDefault().Intyear <= year
                && x.Requestclients.FirstOrDefault().Intdate <= day
                && DateTime.ParseExact(x.Requestclients.FirstOrDefault().Strmonth, "MMM", CultureInfo.InvariantCulture).Month <= month)
                .ToList();
            }

            //for physicianname search

            if (physicianname != null)
            {
                recorddata = recorddata.Where(x => x.Physician != null).ToList();
                recorddata = recorddata.Where(x => (x.Physician.Firstname + x.Physician.Lastname).ToLower().Contains(physicianname.ToLower())).ToList();
            }

            //for email search

            if (email != null)
            {
                recorddata = recorddata.Where(x => x.Requestclients.FirstOrDefault().Email.ToLower().Contains(email.ToLower())).ToList();
            }

            //for phonenumber search

            if (phonenumber != null)
            {
                recorddata = recorddata.Where(x => x.Requestclients.FirstOrDefault().Phonenumber.ToLower().Contains(phonenumber)).ToList();

            }

            List<RecordViewModel> records = new List<RecordViewModel>();


            foreach (var request in recorddata)
            {
                var model = new RecordViewModel();
                model.PatientName = request.Requestclients.FirstOrDefault().Firstname + " " + request.Requestclients.FirstOrDefault().Lastname;
                model.Requestor = model.RequestTypeName(request.Requesttypeid);
                model.DateOfService = request.Requestclients.FirstOrDefault().Strmonth
                    + " " + request.Requestclients.FirstOrDefault().Intdate + "," +
                    request.Requestclients.FirstOrDefault().Intyear;

                model.Physician = "-";
                if (request.Physicianid != null)
                {
                    model.Physician = _context.Physicians.FirstOrDefault(x => x.Physicianid == request.Physicianid).Firstname.ToString();

                }

                model.CloseCaseDate = "-";
                model.CancelProviderNote = "-";
                if (request.Requeststatuslogs.Count() > 0)
                {

                    var noteobj = request.Requeststatuslogs.Where(m => m.Status == 3 && m.Physicianid == request.Physicianid).FirstOrDefault();
                    model.CancelProviderNote = noteobj != null ? noteobj.Notes : "-";
                    model.CloseCaseDate = request.Requeststatuslogs.FirstOrDefault().Createddate.ToString("MMM dd,yyyy") ?? "-";

                }
                model.PhysicianNote = "-";
                model.AdminNote = "-";
                if (request.Requestnotes.Count() > 0)
                {
                    model.PhysicianNote = request.Requestnotes.FirstOrDefault().Physiciannotes ?? "-";
                    model.AdminNote = request.Requestnotes.FirstOrDefault().Adminnotes ?? "-";

                }


                model.Email = request.Requestclients.FirstOrDefault().Email;
                model.PhoneNumber = request.Requestclients.FirstOrDefault().Phonenumber.ToString();
                model.Address = request.Requestclients.FirstOrDefault().Address;
                model.Zip = request.Requestclients.FirstOrDefault().Zipcode;
                model.RequestStatus = request.Status;
                model.PatientNote = request.Requestclients.FirstOrDefault().Notes;
                model.requestid = request.Requestid;
                records.Add(model);
            }


            return records;
        }

        //for deleting search record
        public int DeleteSearchRecord(int requestid)
        {
            var x = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);


            if (x != null)
            {
                x.Isdeleted = new BitArray(new[] { true });
                _context.Requests.Update(x);
            }
            _context.SaveChanges();

            return 1;
        }




        //get block history data

        public List<RecordViewModel> GetBlockHistoryFilterData(string name, string date, string email, string phonenumber)
        {

            var blockdata = _context.Blockrequests.ToList();


            if (date != null)
            {

                blockdata = blockdata.Where(x => DateOnly.FromDateTime(x.Createddate).ToString("yyyy-MM-dd") == date).ToList();

            }


            //for email search

            if (email != null)
            {
                blockdata = blockdata.Where(x => x.Email.Contains(email.ToLower())).ToList();
            }

            //for phonenumber search

            if (phonenumber != null)
            {
                blockdata = blockdata.Where(x => x.Phonenumber.Contains(phonenumber)).ToList();
            }

            List<RecordViewModel> blockhistorydata = new List<RecordViewModel>();
            foreach (var x in blockdata)
            {


                var model = new RecordViewModel();

                model.PatientName = _context.Requestclients.FirstOrDefault(m => m.Requestid == x.Requestid).Firstname +
                " " + _context.Requestclients.FirstOrDefault(m => m.Requestid == x.Requestid).Lastname;

                model.PhoneNumber = x.Phonenumber;
                model.Email = x.Email;
                model.DateOfService = x.Createddate.ToString("MMM dd,yyyy");
                model.PatientNote = x.Reason;
                model.requestid = x.Requestid;//for unblock
                blockhistorydata.Add(model);


            }

            //for name search
            if (name != null)
            {
                blockhistorydata = blockhistorydata.Where(x => x.PatientName.ToLower().Contains(name.ToLower())).ToList();
            }

            return blockhistorydata;
        }

        public int UnblockBlockHistory(int requestid, int adminid)
        {
            var requestdata = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            var blockhistory = _context.Blockrequests.FirstOrDefault(x => x.Requestid == requestid);
            var requeststatuslog = new Requeststatuslog();

            if (requestid != 0)
            {
                requestdata.Status = 1;
                requeststatuslog.Requestid = requestid;
                requeststatuslog.Status = 1;
                requeststatuslog.Adminid = adminid;
                requeststatuslog.Createddate = DateTime.Now;
            }

            if (requeststatuslog != null && requestdata != null && blockhistory != null)
            {
                _context.Requests.Update(requestdata);
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Blockrequests.Remove(blockhistory);
                _context.SaveChanges();
                return 1;

            }

            return 0;
        }




        //patient history methods
        public List<User> GetUserList(string firstname, string lastname, string email, int phone)
        {
            List<User> list = _context.Users.ToList();
            if (firstname != null)
            {
                list = list.Where(m => m.Firstname.ToLower().Contains(firstname.ToLower())).ToList();
            }
            if (lastname != null)
            {
                list = list.Where(m => m.Lastname.ToLower().Contains(lastname.ToLower())).ToList();
            }
            if (email != null)
            {
                list = list.Where(m => m.Email.ToLower().Contains(email.ToLower())).ToList();
            }
            if (phone != 0)
            {
                list = list.Where(m => m.Mobile.ToLower().Contains(phone.ToString())).ToList();
            }
            return list;
        }

        //Patient Record method
        public List<PatientRecordViewModel> GetPatientRecordData(int userid)
        {
            List<PatientRecordViewModel> patientRecordViewModels = new List<PatientRecordViewModel>();
            List<Request> request = _context.Requests.Include(m => m.Requestclients).Include(m => m.Requeststatuslogs).Where(m => m.Userid == userid).ToList();
            foreach (var item in request)
            {
                PatientRecordViewModel model = new PatientRecordViewModel();
                model.ClientName = item.Requestclients.FirstOrDefault().Firstname + item.Requestclients.FirstOrDefault().Lastname;
                model.CreatedDate = item.Createddate.ToString("MMM dd,yyyy");
                model.Confirmation = item.Confirmationnumber;
                var physician = _context.Physicians.FirstOrDefault(m => m.Physicianid == item.Physicianid);
                model.ProviderName = physician != null ? physician.Firstname + physician.Lastname : "-";
                model.ConcludedDate = "-";
                if (item.Requeststatuslogs.Count > 0)
                {
                    //var ConcludedDateobj = item.Requeststatuslogs.FirstOrDefault(m => m.Status == 6).CreatedDate.ToString("MMM dd,yyyy");
                    var ConcludedDateobj = _context.Requeststatuslogs.FirstOrDefault(m => m.Status == 6 && m.Requestid == item.Requestid);

                    model.ConcludedDate = ConcludedDateobj != null ? ConcludedDateobj.Createddate.ToString() : "-";
                }
                model.RequestId = item.Requestid;
                patientRecordViewModels.Add(model);
            }
            return patientRecordViewModels;
        }

        //Email Log Methods
        public List<EmailLogViewModel> GetEmailLogs(int Role, string ReceiverName, string Email, DateTime CreatedDate, DateTime SentDate)
        {
            var emailLogs = _context.Emaillogs.ToList();
            var aspnetrole = _context.Aspnetroles.ToList();
            var model = new List<EmailLogViewModel>();
            foreach (var item in emailLogs)
            {
                var viewmodel = new EmailLogViewModel();
                viewmodel.ConfirmationNumber = item.Confirmationnumber != null ? item.Confirmationnumber : "-";
                viewmodel.CreatedDate = item.Createdate.ToString("MMM dd, yyyy");
                var sentdateobj = item.Sentdate;
                viewmodel.SentDate = sentdateobj != null ? sentdateobj?.ToString("MMM dd, yyyy") : "-";
                viewmodel.Email = item.Emailid;
                viewmodel.SentTries = item.Senttries.ToString();
                viewmodel.Sent = item.Isemailsent[0] == true ? item.Isemailsent : new BitArray(new[] { false });
                viewmodel.Action = item.Subjectname;
                viewmodel.Recipint = item.Emailid.Split("@")[0];
                viewmodel.RoleName = aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()) != null ? aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()).Name : "-";
                viewmodel.RoleId = aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()) != null ? aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()).Id : "-";
                model.Add(viewmodel);
            }
            if (Role != 0)
            {
                model = model.Where(m => m.RoleId == Role.ToString()).ToList();
            }
            if (ReceiverName != null)
            {
                model = model.Where(m => m.Recipint.ToLower().Contains(ReceiverName.ToLower())).ToList();

            }
            if (Email != null)
            {
                model = model.Where(m => m.Email.ToLower().Contains(Email.ToLower())).ToList();

            }
            if (CreatedDate.ToString() != "01-01-0001 00:00:00")
            {
                model = model.Where(m => m.CreatedDate == CreatedDate.ToString("MMM dd, yyyy")).ToList();
            }
            if (SentDate.ToString() != "01-01-0001 00:00:00")
            {
                model = model.Where(m => m.SentDate == SentDate.ToString("MMM dd, yyyy")).ToList();
            }
            return model;
        }
        public EmailLogViewModel GetAspNetRoleList()
        {
            var model = new EmailLogViewModel();
            model.AspNetRoleList = _context.Aspnetroles.ToList();
            return model;
        }

        // sms log methods
        public List<EmailLogViewModel> GetSMSLogs(int Role, string ReceiverName, string Email, DateTime CreatedDate, DateTime SentDate)
        {
            var SMSLogs = _context.Smslogs.ToList();
            var aspnetrole = _context.Aspnetroles.ToList();
            var model = new List<EmailLogViewModel>();
            foreach (var item in SMSLogs)
            {
                var viewmodel = new EmailLogViewModel();
                viewmodel.ConfirmationNumber = item.Confirmationnumber != null ? item.Confirmationnumber : "-";
                viewmodel.CreatedDate = item.Createdate.ToString("MMM dd, yyyy");
                var sentdateobj = item.Sentdate;
                viewmodel.SentDate = sentdateobj != null ? sentdateobj?.ToString("MMM dd, yyyy") : "-";
                viewmodel.Email = item.Mobilenumber.ToString();
                viewmodel.SentTries = item.Senttries.ToString();
                viewmodel.Sent = item.Issmssent[0] == true ? item.Issmssent : new BitArray(new[] { false });
                viewmodel.Action = "-";
                viewmodel.Recipint = item.Smslogid.ToString().Split("@")[0];
                viewmodel.RoleName = aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()) != null ? aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()).Name : "-";
                viewmodel.RoleId = aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()) != null ? aspnetrole.FirstOrDefault(x => x.Id == item.Roleid.ToString()).Id : "-";
                model.Add(viewmodel);
            }
            if (Role != 0)
            {
                model = model.Where(m => m.RoleId == Role.ToString()).ToList();
            }
            if (ReceiverName != null)
            {
                model = model.Where(m => m.Recipint.ToLower().Contains(ReceiverName.ToLower())).ToList();

            }
            if (Email != null)
            {
                model = model.Where(m => m.Email.ToLower().Contains(Email.ToLower())).ToList();

            }
            if (CreatedDate.ToString() != "01-01-0001 00:00:00")
            {
                model = model.Where(m => m.CreatedDate == CreatedDate.ToString("MMM dd, yyyy")).ToList();
            }
            if (SentDate.ToString() != "01-01-0001 00:00:00")
            {
                model = model.Where(m => m.SentDate == SentDate.ToString("MMM dd, yyyy")).ToList();
            }
            return model;
        }


    }
}
