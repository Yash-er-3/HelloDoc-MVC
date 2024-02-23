using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        public  HelloDocDbContext Dbcontext { get; }
        public RequestRepository(HelloDocDbContext dbContext) : base(dbContext)
        {
        }

        
    }
}
