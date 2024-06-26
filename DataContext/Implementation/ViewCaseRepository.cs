﻿using HelloDoc;
using Services.Contracts;
using Services.Viewmodels;
using System.Globalization;

namespace Services.Implementation
{
    public class ViewCaseRepository : Repository<Requestclient>, IViewCaseRepository
    {
        private readonly HelloDocDbContext _view;

        public ViewCaseRepository(HelloDocDbContext view) : base(view)
        {
            _view = view;
        }

        public void EditViewCaseData(ViewCaseView view)
        {
            var request = _view.Requests.FirstOrDefault(m => m.Confirmationnumber == view.ConfirmationNumber);


            request.Phonenumber = view.PhoneNumber;
            request.Modifieddate = DateTime.Now;
            request.Email = view.Email;

            var updatedata = _view.Requestclients.FirstOrDefault(m => m.Requestid == request.Requestid);

            updatedata.Phonenumber = view.PhoneNumber;
            updatedata.Email = view.Email;

            if (request != null && updatedata != null)
            {
                _view.Requests.Update(request);
                _view.SaveChanges();

                _view.Requestclients.Update(updatedata);
                _view.SaveChanges();
            }
        }

        public ViewCaseView GetViewCaseData(int reqid)
        {

            var model = _view.Requestclients.FirstOrDefault(m => m.Requestid == reqid);
            var request = _view.Requests.FirstOrDefault(m => m.Requestid == reqid);
            var regiondata = _view.Regions.FirstOrDefault(m => m.Regionid == model.Regionid);
            var data = new ViewCaseView
            {
                ConfirmationNumber = request.Confirmationnumber,
                PatientNotes = model.Notes,
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Email = model.Email,
                DOB = new DateTime(Convert.ToInt32(model.Intyear), DateTime.ParseExact(model.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(model.Intdate)),
                PhoneNumber = model.Phonenumber,
                Region = regiondata.Name,
                Address = model.Address,
                requestid = reqid,
                status = request.Status
            };

            return data;
        }

    }
}
