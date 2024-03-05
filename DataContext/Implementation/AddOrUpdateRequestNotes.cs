using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Services.Contracts;
using Services.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class AddOrUpdateRequestNotes : Repository<Requestnote>, IAddOrUpdateRequestNotes
    {
        private readonly HelloDocDbContext _context;
        public AddOrUpdateRequestNotes(HelloDocDbContext context) : base(context)
        {
            _context = context;
        }

        public void addOrUpdateRequestNotes(AdminDashboardViewModel obj)
        {
            var requestnote = new Requestnote();
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == obj.requestid);
            var existrequestnote = _context.Requestnotes.FirstOrDefault(m => m.Requestid == obj.requestid);
            if (existrequestnote != null)
            {

                var requestnoteupdate = _context.Requestnotes.FirstOrDefault(m => m.Requestid == obj.requestid);
                requestnoteupdate.Modifieddate = DateTime.Now;
                requestnoteupdate.Adminnotes = obj.blocknotes;
                _context.Requestnotes.Update(requestnoteupdate);
                _context.SaveChanges();

            }
            else
            {
                requestnote.Requestid = obj.requestid;
                requestnote.Createddate = DateTime.Now;
                requestnote.Createdby = "1";
                requestnote.Adminnotes = obj.blocknotes;
                _context.Requestnotes.Add(requestnote);
                _context.SaveChanges();
            }
        }

       
    }
}
