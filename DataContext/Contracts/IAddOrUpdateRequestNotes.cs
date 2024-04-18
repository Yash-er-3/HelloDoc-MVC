using Services.Viewmodels;

namespace Services.Contracts
{
    public interface IAddOrUpdateRequestNotes
    {
        public void addOrUpdateRequestNotes(AdminDashboardViewModel obj);
        public void PhysicianRequestNotes(AdminDashboardViewModel obj);
    }
}
