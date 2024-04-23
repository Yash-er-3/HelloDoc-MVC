
using HelloDoc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Viewmodels;
using Services.ViewModels;
using System.Collections;

namespace HalloDoc.Controllers.Access
{
    public class AccessController : Controller
    {
        private readonly HelloDocDbContext _context;
        public AccessController(HelloDocDbContext context)
        {
            _context = context;
        }
        public IActionResult AccessRole()
        {
            var rolelist = _context.Roles.ToList();
            var model = new AccessRoleViewModel
            {
                rolelist = rolelist
            };
            return View(model);
        }

        public IActionResult CreateRole(int accounttype, int roleid)
        {
            var rolelist = _context.Roles.ToList();
            var model = new AccessRoleViewModel();
            model.rolelist = rolelist;
            if (accounttype != 0)
            {
                var accounttypemenulist = _context.Menus.Where(m => m.Accounttype == accounttype).ToList();
                model.menulist = accounttypemenulist;
            }
            else
            {
                var menulist = _context.Menus.ToList();
                model.menulist = menulist;
            }
            if (roleid != 0)
            {
                var role = _context.Roles.FirstOrDefault(m => m.Roleid == roleid);
                var rolemenu = _context.Rolemenus.Where(m => m.Roleid == role.Roleid).ToList();
                model.RoleName = role.Name;
                model.selectedrolemenulist = rolemenu;
            }
            return View(model);
        }

        public IActionResult MenuFilterCheck(int accounttype)
        {
            var model = new AccessRoleViewModel();
            if (accounttype != 0)
            {
                var accounttypemenulist = _context.Menus.Where(m => m.Accounttype == accounttype).ToList();
                model.menulist = accounttypemenulist;
            }
            else
            {
                var menulist = _context.Menus.ToList();
                model.menulist = menulist;
            }
            return PartialView("MenuFilterCheckbox", model);
        }



        [HttpPost]
        public IActionResult CreateRole(string rolename, int accounttype, int[] selectedmenu, int roleid)
        {
            var adminname = HttpContext.Session.GetString("AdminName");
            if (roleid == 0)
            {
                var role = new Role();
                var bit = new BitArray(1);
                bit[0] = false;
                role.Name = rolename;
                role.Accounttype = (short)accounttype;
                role.Createdby = adminname;
                role.Createddate = DateTime.Now;
                role.Isdeleted = bit;

                _context.Roles.Add(role);
                _context.SaveChanges();

                foreach (var item in selectedmenu)
                {
                    var rolemenu = new Rolemenu
                    {
                        Menuid = item
                    };
                    rolemenu.Roleid = role.Roleid;

                    _context.Rolemenus.Add(rolemenu);
                }
                _context.SaveChanges();
                TempData["success"] = "Role Created Successfully!";
            }
            else
            {

                int[]? roleMenus = _context.Rolemenus.Where(r => r.Roleid == roleid).Select(s => s.Menuid).ToArray();

                IEnumerable<int> menusToDelete = roleMenus.Except(selectedmenu);

                foreach (var menuToDelete in menusToDelete)
                {
                    Rolemenu? roleMenu = _context.Rolemenus.Where(r => r.Roleid == roleid && r.Menuid == menuToDelete).FirstOrDefault();

                    if (roleMenu != null)
                    {
                        _context.Remove(roleMenu);
                    }

                }

                IEnumerable<int> menusToAdd = selectedmenu.Except(roleMenus);

                foreach (var menuToAdd in menusToAdd)
                {
                    Rolemenu roleMenu = new Rolemenu
                    {
                        Roleid = roleid,
                        Menuid = menuToAdd,
                    };
                    _context.Add(roleMenu);
                }

                _context.SaveChanges();


                TempData["success"] = "Role Updated Successfully!";
            }


            return RedirectToAction("AccessRole");
        }
        public IActionResult DeleteRole(int roleid)
        {
            if (roleid != 0)
            {
                Role role = _context.Roles.FirstOrDefault(m => m.Roleid == roleid);
                List<Rolemenu> roleMenu = _context.Rolemenus.Where(m => m.Roleid == roleid).ToList();
                foreach (var item in roleMenu)
                {
                    _context.Remove(item);
                }
                _context.Remove(role);
                _context.SaveChanges();


            }
            TempData["success"] = "Role Deleted Successfully!";
            return RedirectToAction("AccessRole");
        }

        public IActionResult UserAccess()
        {
            var rolelist = _context.Roles.ToList();
            List<AccessViewModel> model = new List<AccessViewModel>();
            var aspuser = _context.Aspnetusers.Include(m => m.Aspnetuserroles).Where(m => m.Aspnetuserroles.FirstOrDefault().Roleid == "1" || m.Aspnetuserroles.FirstOrDefault().Roleid == "2").ToList();
            foreach (var user in aspuser)
            {
                var access = new AccessViewModel();
                access.Phone = user.Phonenumber;
                if (user.Aspnetuserroles.Count() > 0)
                {
                    if (user.Aspnetuserroles.FirstOrDefault(m => m.Userid == user.Id).Roleid == 1.ToString())
                    {
                        var admin = _context.Admins.FirstOrDefault(m => m.Aspnetuserid == user.Id);
                        access.Accounttype = 1.ToString();
                        access.Status = admin.Status;
                        access.Name = admin.Firstname + admin.Lastname;
                        access.OpenRequest = _context.Requests.ToList().Count().ToString();
                        access.AdminId = admin.Adminid;
                    }
                    if (user.Aspnetuserroles.FirstOrDefault(m => m.Userid == user.Id).Roleid == 2.ToString())
                    {
                        var physician = _context.Physicians.FirstOrDefault(m => m.Aspnetuserid == user.Id);
                        access.Status = physician.Status;
                        access.Accounttype = 2.ToString();
                        access.Name = physician.Firstname + physician.Lastname;
                        access.OpenRequest = _context.Requests.ToList().Where(m => m.Physicianid == physician.Physicianid).ToList().Count().ToString();
                        access.physicianId = physician.Physicianid;
                    }
                }
                model.Add(access);
            }
            return View(model);
        }
        public IActionResult AdminProfileFromUserAccess(int adminid)        {            var admin = _context.Admins.FirstOrDefault(m => m.Adminid == adminid);            var aspnetuser = _context.Aspnetusers.FirstOrDefault(m => m.Id == admin.Aspnetuserid);            var rolelist = _context.Aspnetroles.ToList();            var regionlist = _context.Regions.ToList();            var adminregionlist = _context.Adminregions.Where(m => m.Adminid == adminid).ToList();            var model = new UserAllDataViewModel            {                UserName = aspnetuser.Username,                password = aspnetuser.Passwordhash,                status = admin.Status,                role = rolelist,                firstname = admin.Firstname,                lastname = admin.Lastname,                email = admin.Email,                confirmationemail = admin.Email,                phonenumber = admin.Mobile,                regionlist = regionlist,                address1 = admin.Address1,                address2 = admin.Address2,                zip = admin.Zip,                alterphonenumber = admin.Altphone,                adminregionlist = adminregionlist,                check = false,            };            return PartialView("../Admin/AdminProfile", model);        }
    }
}
