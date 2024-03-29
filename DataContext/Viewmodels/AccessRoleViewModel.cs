using HelloDoc.DataModels;

namespace Services.ViewModels
{
    public class AccessRoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<Role> rolelist { get; set; }
        public List<Rolemenu> rolemenulist { get; set; }
        public List<Rolemenu> selectedrolemenulist { get; set; }
        public List<Menu> menulist { get; set; }

        public enum accounttype
        {
            All,
            Admin,
            Physician,
            Patient
        }

        public string Accounttypename(int by)
        {
            string By = ((accounttype)by).ToString();
            //string By = ((Requestby)by).ToString();
            return By;
        }

    }
}
