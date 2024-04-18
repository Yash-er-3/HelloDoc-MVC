using DataAccess.ServiceRepository;
using HelloDoc;
using Microsoft.AspNetCore.Http;
using Services.Contracts;

namespace Services.Implementation
{
    [AuthorizationRepository("Admin")]
    public class AdminCredential : Repository<Admin>, IAdminCredential
    {
        private HelloDocDbContext _context;
        private IHttpContextAccessor _httpcontext;


        public AdminCredential(HelloDocDbContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _httpcontext = httpContextAccessor;

        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public int Login(Aspnetuser user)
        {
            var correct = _context.Aspnetusers.FirstOrDefault(m => m.Email == user.Email);

            if (_context.Physicians.Any(x => x.Email == user.Email))
            {
                var x = _context.Physicians.FirstOrDefault(x => x.Email == user.Email);

                if (x != null)
                {
                    if (correct.Passwordhash != user.Passwordhash)
                    {

                        return 2;
                    }
                }
                int id = x.Physicianid;

                return 5;

            }

            if (correct != null)
            {
                var admin = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == correct.Id.ToString());

                if (admin != null)
                {
                    if (user.Passwordhash == correct.Passwordhash)
                    {
                        int id = admin.Adminid;
                        _httpcontext.HttpContext.Session.SetString("UserName", admin.Firstname + admin.Lastname);
                        return 1;
                    }
                    return 2;
                }
            }
            else
            {
                return 4;
            }
            return 0;
        }


    }
}

