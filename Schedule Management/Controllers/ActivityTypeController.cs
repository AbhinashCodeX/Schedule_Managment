using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Models;
using Schedule_Management.ViewModels;

namespace Schedule_Management.Controllers
{
    public class ActivityTypeController : Controller
    {
        private readonly ScheduleManagementDbContext _context;

        public ActivityTypeController(
            ScheduleManagementDbContext context)
        {
            _context = context;
        }

        // Activity Type List
        [HttpGet]
        public async Task<IActionResult> Index()
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

            var activities = await _context.ActivityTypes
                .AsNoTracking()   //make the query read-only and improve performance
                .OrderByDescending(a => a.CreatedOn) // order by CreatedOn in descending order
                .Select(a => new ActivityTypeViewModel //project to the view model
                {
                    ActivityTypeId = a.ActivityTypeId,//map the ActivityTypeId property
                    ActivityName = a.ActivityName,//map the ActivityName property
                    IsActive = a.IsActive,//map the IsActive property
                    CreatedOn = a.CreatedOn//map the CreatedOn property
                })
                .ToListAsync();//execute the query and return the results as a list

            return View(activities);//pass the list of activities to the view
        }

        // Create Page
        [HttpGet]
        public IActionResult Create()
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

            var model = new ActivityTypeViewModel
            {
                IsActive = true
            };

            return View(model);
        }

        // Create Activity Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityTypeViewModel model)
        {
            string? roleName =
                HttpContext.Session.GetString("RoleName");

            int? adminUserId =
                HttpContext.Session.GetInt32("UserId");

            if (roleName != "Admin" ||
                !adminUserId.HasValue)
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string activityName =
                model.ActivityName.Trim();

            bool activityExists =
                await _context.ActivityTypes.AnyAsync(a =>
                    a.ActivityName.ToLower() ==
                    activityName.ToLower()
                );

            if (activityExists)
            {
                ModelState.AddModelError(
                    nameof(model.ActivityName),
                    "This activity type already exists."
                );

                return View(model);
            }

            var activityType = new ActivityType
            {
                ActivityName = activityName,
                IsActive = model.IsActive,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = adminUserId.Value
            };

            _context.ActivityTypes.Add(activityType);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Activity type created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

            var activity = await _context.ActivityTypes
                .AsNoTracking()
                .Where(a => a.ActivityTypeId == id)
                .Select(a => new ActivityTypeViewModel
                {
                    ActivityTypeId = a.ActivityTypeId,
                    ActivityName = a.ActivityName,
                    IsActive = a.IsActive,
                    CreatedOn = a.CreatedOn
                })
                .FirstOrDefaultAsync();

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityTypeViewModel model)
        {
            string? roleName =
                HttpContext.Session.GetString("RoleName");

            int? adminUserId =
                HttpContext.Session.GetInt32("UserId");

            if (roleName != "Admin" ||
                !adminUserId.HasValue)
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var activity = await _context.ActivityTypes
                .FirstOrDefaultAsync(a =>
                    a.ActivityTypeId == model.ActivityTypeId
                );

            if (activity == null)
            {
                return NotFound();
            }

            string activityName =
                model.ActivityName.Trim();

            bool duplicateExists =
                await _context.ActivityTypes.AnyAsync(a =>
                    a.ActivityTypeId != model.ActivityTypeId &&
                    a.ActivityName.ToLower() ==
                    activityName.ToLower()
                );

            if (duplicateExists)
            {
                ModelState.AddModelError(
                    nameof(model.ActivityName),
                    "This activity type already exists."
                );

                return View(model);
            }

            activity.ActivityName = activityName;
            activity.IsActive = model.IsActive;
            activity.ModifiedOn = DateTime.UtcNow;
            activity.ModifiedBy = adminUserId.Value;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Activity type updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            string? roleName =
                HttpContext.Session.GetString("RoleName");

            int? adminUserId =
                HttpContext.Session.GetInt32("UserId");

            if (roleName != "Admin" ||
                !adminUserId.HasValue)
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            var activity = await _context.ActivityTypes
                .FirstOrDefaultAsync(a =>
                    a.ActivityTypeId == id
                );

            if (activity == null)
            {
                return NotFound();
            }

            activity.IsActive = !activity.IsActive;
            activity.ModifiedOn = DateTime.UtcNow;
            activity.ModifiedBy = adminUserId.Value;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                activity.IsActive
                    ? "Activity type activated successfully."
                    : "Activity type deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}