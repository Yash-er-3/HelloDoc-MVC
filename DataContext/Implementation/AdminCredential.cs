using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class AdminCredential : IAdminCredential
    {
        private readonly HelloDocDbContext _context;
        private readonly IHttpContextAccessor _httpcontext;

        public AdminCredential(HelloDocDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpcontext = httpContextAccessor;
        }
        public int Login(Aspnetuser user)
        {
            var correct = _context.Aspnetusers.FirstOrDefault(m => m.Email == user.Email);

            if (correct != null)
            {
                var admin = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == correct.Id.ToString());

                if (admin != null)
                {
                    if (user.Passwordhash == correct.Passwordhash)
                    {
                        int id = admin.Adminid;
                        _httpcontext.HttpContext.Session.SetInt32("AdminId", id);
                        _httpcontext.HttpContext.Session.SetString("UserName", admin.Firstname + admin.Lastname);
                        return 1;
                    }
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
               
                return 4;
            }
        }
    }
}

