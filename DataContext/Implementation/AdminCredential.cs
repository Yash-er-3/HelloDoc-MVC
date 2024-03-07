﻿using DataAccess.ServiceRepository;
using DataAccess.ServiceRepository.IServiceRepository;
using HelloDoc.DataContext;
using HelloDoc.DataModels;
using Microsoft.AspNetCore.Http;
using Services.Contracts;
using Services.Viewmodels;

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
