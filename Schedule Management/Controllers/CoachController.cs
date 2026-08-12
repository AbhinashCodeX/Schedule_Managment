using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Models;
using Schedule_Management.ViewModels;

namespace Schedule_Management.Controllers
{
    public class CoachController : Controller
    {
        private readonly ScheduleManagementDbContext _context;

        public CoachController(ScheduleManagementDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            string? role =
                HttpContext.Session.GetString("RoleName");

            if (role != "Coach")
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyAvailability()
        {
            string? role =
                HttpContext.Session.GetString("RoleName");

            int? coachId =
                HttpContext.Session.GetInt32("UserId");

            if (role != "Coach" || !coachId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var availability = await _context.CoachAvailabilities
                .Where(x => x.CoachId == coachId.Value)
                .Include(x => x.ActivityType)
                .OrderBy(x => x.AvailableDate)
                .ThenBy(x => x.StartTime)
                .Select(x => new CoachAvailabilityViewModel
                {
                    AvailabilityId = x.AvailabilityId,
                    ActivityTypeId = x.ActivityTypeId,
                    ActivityName = x.ActivityType.ActivityName,
                    AvailableDate = x.AvailableDate,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    IsBooked = x.IsBooked,
                    IsActive = x.IsActive
                })
                .ToListAsync();
                 ViewBag.ActivityTypes = await _context.ActivityTypes
                 .Where(x => x.IsActive)
                 .OrderBy(x => x.ActivityName)
                .ToListAsync();

            return View(availability);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAvailability(
        CreateCoachAvailabilityViewModel model)
        {
            string? role =
                HttpContext.Session.GetString("RoleName");

            int? coachId =
                HttpContext.Session.GetInt32("UserId");

            if (role != "Coach" || !coachId.HasValue)
            {
                return Json(new
                {
                    success = false,
                    message = "Unauthorized access."
                });
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter all required details."
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Past date validation
            if (model.FromDate < today)
            {
                return Json(new
                {
                    success = false,
                    message = "Past dates are not allowed."
                });
            }

            // Date range validation
            if (model.ToDate < model.FromDate)
            {
                return Json(new
                {
                    success = false,
                    message = "To Date cannot be before From Date."
                });
            }

            // Time validation
            if (model.EndTime <= model.StartTime)
            {
                return Json(new
                {
                    success = false,
                    message = "End Time must be greater than Start Time."
                });
            }

            // Activity validation
            bool activityExists = await _context.ActivityTypes
                .AnyAsync(x =>
                    x.ActivityTypeId == model.ActivityTypeId &&
                    x.IsActive);

            if (!activityExists)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Activity Type."
                });
            }

            var availabilityList = new List<CoachAvailability>();

            var currentDate = model.FromDate;

            while (currentDate <= model.ToDate)
            {
                var availability = new CoachAvailability
                {
                    CoachId = coachId.Value,
                    ActivityTypeId = model.ActivityTypeId,

                    AvailableDate = currentDate,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,

                    IsBooked = false,
                    IsActive = true,

                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = coachId.Value
                };

                availabilityList.Add(availability);

                currentDate = currentDate.AddDays(1);
            }

            await _context.CoachAvailabilities
                .AddRangeAsync(availabilityList);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"{availabilityList.Count} availability slot(s) added successfully."
            });
        }
    }
}