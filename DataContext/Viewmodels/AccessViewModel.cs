
using HelloDoc;

namespace Services.ViewModels
{
    public class AccessViewModel
    {
        public string Phone { get; set; }
        public short? Status { get; set; }
        public string OpenRequest { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Accounttype { get; set; }

        public int physicianId { get; set; }
        public int AdminId { get; set; }
        public List<Physician> physicianlist { get; set; }
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
            return By;
        }
        public enum status
        {
            Active,
            InActive,

        }

        public string StatusName(int by)
        {
            string By = ((status)by).ToString();
            return By;
        }
    }
}


