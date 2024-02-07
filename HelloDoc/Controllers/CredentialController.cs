using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.Controllers
{
    public class CredentialController : Controller
    {
        private readonly HelloDocDbContext _context;

        public CredentialController(HelloDocDbContext context)
        {
            _context = context;
        }

        [HttpPost]

        public async Task<IActionResult> Login(User user)
        {
            try
            {
                var match = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == user.Email);

                if(match.Passwordhash == user.Aspnetuser.Passwordhash) {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Privacy", "Home");
            }
            catch (Exception e)
            {
                return RedirectToAction("registeredpatient", "Home");
            }
        }
    }
}
