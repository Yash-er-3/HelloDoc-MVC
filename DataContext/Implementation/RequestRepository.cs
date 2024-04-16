using HelloDoc;
using Services.Contracts;

namespace Services.Implementation
{
    public class RequestRepository : Repository<Request>, IRequestRepository
    {
        public HelloDocDbContext Dbcontext { get; }
        public RequestRepository(HelloDocDbContext dbContext) : base(dbContext)
        {
        }


    }
}
