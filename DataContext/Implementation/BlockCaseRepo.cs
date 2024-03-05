using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Services.Contracts;

namespace Services.Implementation
{
    public class BlockCaseRepo : Repository<Blockrequest>, IBlockCaseRepo
    {
        private readonly HelloDocDbContext _context;

        public BlockCaseRepo(HelloDocDbContext context):base(context)
        {
            _context = context;
        }

        public void BlockCaseData(int id, string blocknotes)
        {
            var request = _context.Requests.FirstOrDefault(m => m.Requestid == id);

            if (request != null)
            {
                var blockmodaldata = new Blockrequest
                {
                    Requestid = request.Requestid,
                    Phonenumber = request.Phonenumber,
                    Email = request.Email,
                    Reason = blocknotes,
                    Createddate = DateTime.Now,
                    
                };

                request.Status = 11;
                if(blockmodaldata != null)
                {
                    _context.Blockrequests.Add(blockmodaldata);
                    _context.SaveChanges();
                    _context.Requests.Update(request);
                    _context.SaveChanges();
                }
            }

        }
    }
}
