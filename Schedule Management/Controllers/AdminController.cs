using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Models;
using Schedule_Management.ViewModels;

namespace Schedule_Management.Controllers
{
    public class AdminController : Controller
    {   
        private readonly ScheduleManagementDbContext _context;
        public AdminController(
          ScheduleManagementDbContext context)
        {
            _context = context;
        }

        //Once the user is logged in if by mistakely the user hits the back button the browser will take the user to the previous page which is the login page.
        //To avoid this we will use the ResponseCache attribute to prevent caching of the login page. This will ensure that when the user hits the back button, they will be redirected to the dashboard page instead of the login page.
        [ResponseCache(
         Duration = 0,
         Location = ResponseCacheLocation.None,
         NoStore = true
         )]
        public IActionResult Dashboard()
        {
            string? role =
                HttpContext.Session.GetString("RoleName");

            if (role != "Admin")
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            return View();
        }

        //Users List
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            string? roleName =
                HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            var users = await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Role.RoleName == "User" ||
                    u.Role.RoleName == "Coach")
                .OrderByDescending(u => u.CreatedOn)
                .Select(u => new UserListViewModel
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    RoleName = u.Role.RoleName,

                    DistrictName = u.District != null
                        ? u.District.DistrictName
                        : "Not Available",

                    IsActive = u.IsActive,
                    CreatedOn = u.CreatedOn
                })
                .ToListAsync();

            return View(users);
        }
    }
}