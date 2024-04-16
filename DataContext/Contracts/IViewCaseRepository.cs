using Services.Viewmodels;

namespace Services.Contracts
{
    public interface IViewCaseRepository
    {
        public ViewCaseView GetViewCaseData(int reqid);

        public void EditViewCaseData(ViewCaseView view);
    }
}