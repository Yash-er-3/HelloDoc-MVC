using Services.Viewmodels;

namespace HelloDoc.Views.Shared
{
    public interface IRequestDataRepository
    {
        List<allrequestdataViewModel> GetAllRequestData(int status);
    }
}
