using HelloDoc.DataModels;

namespace Services.Contracts
{
    public interface IAdminCredential : IRepository<Admin>
    {
        int Login(Aspnetuser user);

        void Save();
    }
}