using Services.Viewmodels;

namespace HelloDoc.Views.Shared
{
    public interface IRequestDataRepository
    {
         List<allrequestdataViewModel> GetAllRequestData(int status);
        List<allrequestdataViewModel> GetAllExportData();
        List<allrequestdataViewModel> GetAllRequestProviderData(int status,int physicianid);

        public void TransferCase(int requestid, AdminDashboardViewModel note, int physicianid);
    }
}
