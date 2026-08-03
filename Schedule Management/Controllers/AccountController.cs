using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Models;
using Schedule_Management.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace Schedule_Management.Controllers
{
    public class AccountController : Controller
    {
        private readonly ScheduleManagementDbContext _context;

        public AccountController(ScheduleManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            await LoadCountries();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCountries();

                return View(model);
            }

            string email = model.Email.Trim().ToLower();

            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Email already registered."
                );

                await LoadCountries();

                return View(model);
            }

            if (model.RegisterAs != "Coach" &&
                model.RegisterAs != "User")
            {
                ModelState.AddModelError(
                    nameof(model.RegisterAs),
                    "Invalid role selected."
                );

                await LoadCountries();

                return View(model);
            }

            int? roleId = await _context.Roles
                .Where(r =>
                    r.RoleName == model.RegisterAs &&
                    r.IsActive)
                .Select(r => (int?)r.RoleId)
                .SingleOrDefaultAsync();

            if (roleId == null)
            {
                ModelState.AddModelError(
                    nameof(model.RegisterAs),
                    "Selected role is not available."
                );

                await LoadCountries();

                return View(model);
            }

            var user = new User
            {
                RoleId = roleId.Value,
                DistrictId = model.DistrictId,
                FullName = model.FullName.Trim(),
                Email = email,
                PhoneNumber = model.PhoneNumber.Trim(),
                FullAddress = model.FullAddress.Trim(),

                PasswordHash = HashPassword(model.Password),

                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Registration completed successfully.";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        private async Task LoadCountries()
        {
            ViewBag.Countries = await _context.Countries
                .Where(c => c.IsActive)
                .OrderBy(c => c.CountryName)
                .ToListAsync();
        }

        private static string HashPassword(string password)
        {
            using SHA256 sha256 = SHA256.Create();

            byte[] passwordBytes =
                Encoding.UTF8.GetBytes(password);

            byte[] hashBytes =
                sha256.ComputeHash(passwordBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}