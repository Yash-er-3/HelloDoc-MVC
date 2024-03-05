using HelloDoc.DataContext;
using HelloDoc.DataModels;
using HelloDoc.Views.Shared;
using Services.Viewmodels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class RequestDataRepository : Repository<Request> , IRequestDataRepository
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
                                               RequestId = req.Requestid
                                           };
            return allRequestDataViewModels.ToList();
        }
    }
}
