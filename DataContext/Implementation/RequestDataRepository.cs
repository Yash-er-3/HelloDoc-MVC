using HelloDoc.DataContext;
using HelloDoc.DataModels;
using HelloDoc.Views.Shared;
using Microsoft.EntityFrameworkCore;
using Services.Viewmodels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class RequestDataRepository : Repository<Request>, IRequestDataRepository
    {
        private readonly HelloDocDbContext _db;

        public RequestDataRepository(HelloDocDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }

        public List<allrequestdataViewModel> GetAllRequestData(int status)
        {
            var allRequestDataViewModels = from user in _db.Users
                                           join req in _db.Requests on user.Userid equals req.Userid
                                           join reqclient in _db.Requestclients on req.Requestid equals reqclient.Requestid
                                           where req.Status == status
                                           orderby req.Createddate descending
                                           select new allrequestdataViewModel
                                           {
                                               PatientName = reqclient.Firstname + " " + reqclient.Lastname,
                                               PatientDOB = new DateOnly(Convert.ToInt32(user.Intyear), DateTime.ParseExact(user.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(user.Intdate)),
                                               RequestorName = req.Firstname + " " + req.Lastname,
                                               RequestedDate = req.Createddate,
                                               PatientPhone = user.Mobile,
                                               RequestorPhone = req.Phonenumber,
                                               Address = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Address,
                                               Notes = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Notes,
                                               ProviderEmail = _db.Physicians.FirstOrDefault(x => x.Physicianid == req.Physicianid).Email,
                                               PatientEmail = user.Email,
                                               RequestorEmail = req.Email,
                                               RequestType = req.Requesttypeid,
                                               RequestId = req.Requestid,
                                               RegionId = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Regionid,
                                               PhysicianId = req.Physicianid,
                                           };
            return allRequestDataViewModels.ToList();
        }



        public List<allrequestdataViewModel> GetAllExportData()
        {
            var allRequestDataViewModels = from user in _db.Users
                                           join req in _db.Requests on user.Userid equals req.Userid
                                           join reqclient in _db.Requestclients on req.Requestid equals reqclient.Requestid
                                           orderby req.Createddate descending
                                           select new allrequestdataViewModel
                                           {
                                               PatientName = reqclient.Firstname + " " + reqclient.Lastname,
                                               PatientDOB = new DateOnly(Convert.ToInt32(user.Intyear), DateTime.ParseExact(user.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(user.Intdate)),
                                               RequestorName = req.Firstname + " " + req.Lastname,
                                               RequestedDate = req.Createddate,
                                               PatientPhone = user.Mobile,
                                               RequestorPhone = req.Phonenumber,
                                               Address = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Address,
                                               Notes = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Notes,
                                               ProviderEmail = _db.Physicians.FirstOrDefault(x => x.Physicianid == req.Physicianid).Email,
                                               PatientEmail = user.Email,
                                               RequestorEmail = req.Email,
                                               RequestType = req.Requesttypeid,
                                               RequestId = req.Requestid,
                                               RegionId = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Regionid,
                                               PhysicianId = req.Physicianid,
                                           };
            return allRequestDataViewModels.ToList();
        }


        public  List<allrequestdataViewModel> GetAllRequestProviderData(int status, int physicianid)
        {

            var allRequestDataViewModels = from user in _db.Users
                                           join req in _db.Requests on user.Userid equals req.Userid
                                           join reqclient in _db.Requestclients on req.Requestid equals reqclient.Requestid
                                           where req.Physicianid == physicianid && req.Status == status
                                           orderby req.Createddate descending
                                           select new allrequestdataViewModel
                                           {
                                               PatientName = reqclient.Firstname + " " + reqclient.Lastname,
                                               PatientDOB = new DateOnly(Convert.ToInt32(user.Intyear), DateTime.ParseExact(user.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(user.Intdate)),
                                               RequestorName = req.Firstname + " " + req.Lastname,
                                               RequestedDate = req.Createddate,
                                               PatientPhone = user.Mobile,
                                               RequestorPhone = req.Phonenumber,
                                               Address = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Address,
                                               Notes = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Notes,
                                               ProviderEmail = _db.Physicians.FirstOrDefault(x => x.Physicianid == req.Physicianid).Email,
                                               PatientEmail = user.Email,
                                               RequestorEmail = req.Email,
                                               RequestType = req.Requesttypeid,
                                               RequestId = req.Requestid,
                                               RegionId = req.Requestclients.FirstOrDefault(x => x.Requestid == req.Requestid).Regionid,
                                               PhysicianId = req.Physicianid,
                                           };
            return allRequestDataViewModels.ToList();
        }

        public void TransferCase(int requestid, AdminDashboardViewModel note,int physicianid)
        {
            var request = _db.Requests.FirstOrDefault(m => m.Requestid == requestid);
            request.Status = 1;
            request.Physicianid = null;

            var statuslog = new Requeststatuslog();

            statuslog.Status = 1;
            statuslog.Requestid = requestid;
            statuslog.Physicianid = physicianid;
            statuslog.Notes = note.blocknotes;
            statuslog.Createddate = DateTime.Now;
            statuslog.Transtoadmin = new BitArray(new[] { true });

            _db.Requests.Update(request);
            _db.Requeststatuslogs.Add(statuslog);
            _db.SaveChanges();
        }
     

    }
}
