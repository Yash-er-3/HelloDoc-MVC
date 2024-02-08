﻿using HelloDoc.DataContext;
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

        public async Task<IActionResult> Login(Aspnetuser user)
        {
            try
            {
                var match = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == user.Email);

                if(match.Passwordhash == user.Passwordhash) {
                    @TempData["msg"] = "<script>alert('Change succesfully');</script>";
                    return RedirectToAction("Index", "Home");
                }
                TempData["style"] = "text-danger";
                TempData["password"] = "Enter valid password";
                return RedirectToAction("registeredpatient", "Home");

            }
            catch (Exception e)
            {
                TempData["style"] = "text-danger";
                TempData["email"] = "Enter valid email";
                return RedirectToAction("registeredpatient", "Home");
            }
        }
    }
}
