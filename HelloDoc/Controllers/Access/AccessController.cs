
using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Mvc;
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

        //public IActionResult MenuFilterCheckbox(AccessRoleViewModel model)
        //{
        //    return View(model);
        //}

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
    }
}
