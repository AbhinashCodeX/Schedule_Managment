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
        public async Task<IActionResult> Index(
         string? search,
         string? status,
         string sortOrder = "name_asc",
         int page = 1)
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

            const int pageSize = 5;

            var query = _context.ActivityTypes
                .AsNoTracking()
                .AsQueryable();

            // -------------------------
            // SEARCH
            // -------------------------
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(a =>
                    a.ActivityName.Contains(search));
            }

            // -------------------------
            // FILTER
            // -------------------------
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Active")
                {
                    query = query.Where(a => a.IsActive);
                }
                else if (status == "Inactive")
                {
                    query = query.Where(a => !a.IsActive);
                }
            }

            // -------------------------
            // SORTING
            // -------------------------
            query = sortOrder switch
            {
                "name_desc" =>
                    query.OrderByDescending(a => a.ActivityName),

                "date_asc" =>
                    query.OrderBy(a => a.CreatedOn),

                "date_desc" =>
                    query.OrderByDescending(a => a.CreatedOn),

                _ =>
                    query.OrderBy(a => a.ActivityName)
            };

            // -------------------------
            // PAGINATION
            // -------------------------
            int totalRecords = await query.CountAsync();

            int totalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize
                );

            if (page < 1)
            {
                page = 1;
            }

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var activities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ActivityTypeViewModel
                {
                    ActivityTypeId = a.ActivityTypeId,
                    ActivityName = a.ActivityName,
                    IsActive = a.IsActive,
                    CreatedOn = a.CreatedOn
                })
                .ToListAsync();

            // -------------------------
            // VIEW DATA
            // -------------------------
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.SortOrder = sortOrder;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            return View(activities);
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
                TempData["ErrorMessage"] =
                  "Activity type was not found.";

                return RedirectToAction(nameof(Index));
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
                TempData["ErrorMessage"] =
               "Activity type was not found.";

                return RedirectToAction(nameof(Index));
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