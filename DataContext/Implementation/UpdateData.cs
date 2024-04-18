
using HelloDoc;
using Services.Contracts;

namespace Services.Implementation
{
    public class UpdateData : IUpdateData
    {
        private readonly HelloDocDbContext _context;
        public UpdateData(HelloDocDbContext context)
        {
            _context = context;
        }
        public int UpdateRequestTable(int requestid, short status)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            if (request != null)
            {

                request.Status = status;
                _context.Requests.Update(request);
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        public int DeclineRequestTable(int requestid, int physicianid)
        {
            var request = _context.Requests.FirstOrDefault(x => x.Requestid == requestid);
            if (request != null)
            {
                request.Physicianid = null;
                request.Declinedby = physicianid.ToString();
                _context.Requests.Update(request);
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }
    }
}
