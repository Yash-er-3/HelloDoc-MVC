using System.Linq.Expressions;

namespace Services.Contracts
{
    public interface IRepository<T> where T : class
    {

        //CommonAce method implementation
        IEnumerable<T> GetAll();

        //to get matching record from table only 1 row
        T GetFirstOrDefault(Expression<Func<T, bool>> filter);

        //for admin login
    }
}
