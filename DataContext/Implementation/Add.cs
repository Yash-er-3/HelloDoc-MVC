using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Services.Contracts;
using Services.Viewmodels;

namespace Services.Implementation
{
    public class Add : Repository<Admin>, IAdd
    {
        private readonly HelloDocDbContext _context;
        public Add(HelloDocDbContext context) : base(context)
        {
            _context = context;
        }

        public void AddAdmin(UserAllDataViewModel obj, int adminid)
        {
            if (obj != null)
            {
                Guid aspnetid = Guid.NewGuid();
                Aspnetuser aspnetuser = new Aspnetuser
                {
                    Id = aspnetid.ToString(),
                    Username = obj.firstname,
                    Email = obj.email,
                    Passwordhash = obj.password,
                    Createddate = DateTime.Now,
                    Phonenumber = obj.phonenumber,
                };
                _context.Aspnetusers.Add(aspnetuser);
                _context.SaveChanges();
                var createdbyadmin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);
                var createdbyaspnet = _context.Aspnetusers.FirstOrDefault(m => m.Id == createdbyadmin.Aspnetuserid);
                var regionidbyname = _context.Regions.FirstOrDefault(m => m.Name == obj.selectedstate.ToString());
                Admin admin = new Admin
                {
                    Aspnetuserid = aspnetid.ToString(),
                    Firstname = obj.firstname,
                    Lastname = obj.lastname,
                    Email = obj.email,
                    Mobile = obj.phonenumber,
                    Address1 = obj.address1,
                    Address2 = obj.address2,
                    Zip = obj.zip,
                    Regionid = regionidbyname.Regionid,
                    Createdby = createdbyaspnet.Id,
                    Createddate = DateTime.Now,
                };
                _context.Add(admin);
                _context.SaveChanges();
                foreach (var item in obj.selectedregion)
                {
                    Adminregion adminregion = new Adminregion();
                    adminregion.Adminid = admin.Adminid;
                    adminregion.Regionid = item;
                    _context.Add(adminregion);
                    _context.SaveChanges();
                }

                var roleidbyname = _context.Roles.FirstOrDefault(m => m.Name == obj.selectedrole);
                Aspnetuserrole aspnetuserrole = new Aspnetuserrole
                {
                    Userid = aspnetid.ToString(),
                    Roleid = 1.ToString(),
                };
                _context.Add(aspnetuserrole);
                _context.SaveChanges();
            }
        }
    }
}