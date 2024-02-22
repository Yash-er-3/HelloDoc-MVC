using HelloDoc.DataModels;

namespace Services.Contracts
{
    public interface IAdminCredential
    {
        int Login(Aspnetuser user);

    }
}