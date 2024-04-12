//using HalloDoc.DataContext;
//using HalloDoc.DataModels;
//using Services.Contracts;
//using Services.ViewModels;

//namespace Services.Implementation
//{
//    public class Add : Repository<Admin>, IAdd
//    {
//        private readonly ApplicationDbContext _context;
//        public Add(ApplicationDbContext context) : base(context)
//        {
//            _context = context;
//        }

//        public void AddAdmin(user obj, int adminid)
//        {
//            if (obj != null)
//            {
//                Guid aspnetid = Guid.NewGuid();
//                AspNetUser aspnetuser = new AspNetUser
//                {
//                    Id = aspnetid.ToString(),
//                    UserName = obj.UserName,
//                    Email = obj.email,
//                    PasswordHash = obj.password,
//                    CreatedDate = DateTime.Now,
//                    PhoneNumber = obj.phonenumber,
//                };
//                _context.AspNetUsers.Add(aspnetuser);
//                _context.SaveChanges();
//                var createdbyadmin = _context.Admins.FirstOrDefault(m => m.AdminId == adminid);
//                var createdbyaspnet = _context.AspNetUsers.FirstOrDefault(m => m.Id == createdbyadmin.AspNetUserId);
//                var regionidbyname = _context.Regions.FirstOrDefault(m => m.Name == obj.selectedstate.ToString());
//                Admin admin = new Admin
//                {
//                    AspNetUserId = aspnetid.ToString(),
//                    FirstName = obj.firstname,
//                    LastName = obj.lastname,
//                    Email = obj.email,
//                    Mobile = obj.phonenumber,
//                    Address1 = obj.address1,
//                    Address2 = obj.address2,
//                    City = obj.city,
//                    Zip = obj.zip,
//                    RegionId = regionidbyname.RegionId,
//                    CreatedBy = createdbyaspnet.Id,
//                    CreatedDate = DateTime.Now,
//                };
//                _context.Add(admin);
//                _context.SaveChanges();
//                foreach (var item in obj.selectedregion)
//                {
//                    AdminRegion adminregion = new AdminRegion();
//                    adminregion.AdminId = admin.AdminId;
//                    adminregion.RegionId = item;
//                    _context.Add(adminregion);
//                    _context.SaveChanges();
//                }

//                var roleidbyname = _context.Roles.FirstOrDefault(m => m.Name == obj.selectedrole);
//                AspNetUserRole aspnetuserrole = new AspNetUserRole
//                {
//                    UserId = aspnetid.ToString(),
//                    RoleId = 1.ToString(),
//                };
//                _context.Add(aspnetuserrole);
//                _context.SaveChanges();
//            }
//        }
//    }
//}