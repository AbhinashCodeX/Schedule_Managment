using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Models;
using Schedule_Management.ViewModels;
using Schedule_Management.Filters;

namespace Schedule_Management.Controllers
{
    [RoleAuthorize("Coach")]
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
        public async Task<IActionResult> MyAvailability(int? activityTypeId,DateOnly? date,string? status, string? bookingStatus, string? sortOrder, int page = 1)
        {
            int coachId =
                HttpContext.Session.GetInt32("UserId")!.Value;

            // Base Query
            var query = _context.CoachAvailabilities
                .Where(x => x.CoachId == coachId)
                .Include(x => x.ActivityType)
                .AsQueryable();


            // Activity Type Filter
            if (activityTypeId.HasValue)
            {
                query = query.Where(x =>
                    x.ActivityTypeId == activityTypeId.Value);
            }


            // Date Filter
            if (date.HasValue)
            {
                query = query.Where(x =>
                    x.AvailableDate == date.Value);
            }


            // Status Filter
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                {
                    query = query.Where(x => x.IsActive);
                }
                else if (status == "inactive")
                {
                    query = query.Where(x => !x.IsActive);
                }
            }
            //Booking status filter 
            if (!string.IsNullOrWhiteSpace(bookingStatus))
            {
                if (bookingStatus == "Booked")
                {
                    query = query.Where(x => x.IsBooked);
                }
                else if (bookingStatus == "Available")
                {
                    query = query.Where(x =>
                        !x.IsBooked &&
                        x.IsActive);
                }
                else if (bookingStatus == "Unavailable")
                {
                    query = query.Where(x =>
                        !x.IsBooked &&
                        !x.IsActive);
                }
            }
            ViewBag.BookingStatus = bookingStatus;
            ViewBag.SortOrder = sortOrder;
            // Sorting
            query = sortOrder switch
            {
                "date_desc" => query
                    .OrderByDescending(x => x.AvailableDate)
                    .ThenByDescending(x => x.StartTime),

                "activity_asc" => query
                    .OrderBy(x => x.ActivityType.ActivityName)
                    .ThenBy(x => x.AvailableDate)
                    .ThenBy(x => x.StartTime),

                "activity_desc" => query
                    .OrderByDescending(x => x.ActivityType.ActivityName)
                    .ThenBy(x => x.AvailableDate)
                    .ThenBy(x => x.StartTime),

                _ => query
                    .OrderBy(x => x.AvailableDate)
                    .ThenBy(x => x.StartTime)
            };
            //Pagination Data 
            int pageSize = 5;

            int totalRecords = await query.CountAsync();

            int totalPages = (int)Math.Ceiling(
                totalRecords / (double)pageSize
            );

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }

            // Final Query Execute
              var availability = await query
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
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

            ViewBag.SelectedActivityTypeId = activityTypeId;
            ViewBag.SelectedDate = date;
            ViewBag.Status = status;
            ViewBag.BookingStatus = bookingStatus;
            ViewBag.SortOrder = sortOrder;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            return View(availability);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAvailability(
        CreateCoachAvailabilityViewModel model)
        {
            int coachId =
                HttpContext.Session.GetInt32("UserId")!.Value;

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

            var skippedDates = new List<string>();

            var currentDate = model.FromDate;

            while (currentDate <= model.ToDate)
            {
                bool slotExists = await _context.CoachAvailabilities
                    .AnyAsync(x =>
                        x.CoachId == coachId &&
                        x.AvailableDate == currentDate &&
                        x.IsActive &&
                        x.StartTime < model.EndTime &&
                        x.EndTime > model.StartTime);

                if (slotExists)
                {
                    skippedDates.Add(
                        currentDate.ToString("dd MMM yyyy")
                    );

                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                var availability = new CoachAvailability
                {
                    CoachId = coachId,

                    ActivityTypeId =
                        model.ActivityTypeId,

                    AvailableDate =
                        currentDate,

                    StartTime =
                        model.StartTime,

                    EndTime =
                        model.EndTime,

                    IsBooked = false,
                    IsActive = true,

                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = coachId
                };

                availabilityList.Add(availability);

                currentDate = currentDate.AddDays(1);
            }
            if (!availabilityList.Any())
            {
                return Json(new
                {
                    success = false,
                    message =
                        "All selected dates already contain overlapping time slots."
                });
            }

            await _context.CoachAvailabilities
            .AddRangeAsync(availabilityList);

            await _context.SaveChangesAsync();

            string message =
                $"{availabilityList.Count} availability slot(s) added successfully.";

            if (skippedDates.Any())
            {
                message +=
                    $" {skippedDates.Count} overlapping date(s) skipped.";
            }

            return Json(new
            {
                success = true,
                message = message
            });

        }

        [HttpGet]
        public async Task<IActionResult> GetAvailabilityById(int id)
        {
            int coachId =
                HttpContext.Session.GetInt32("UserId")!.Value;

            var availability = await _context.CoachAvailabilities
                .Where(x =>
                    x.AvailabilityId == id &&
                    x.CoachId == coachId)
                .Select(x => new
                {
                    x.AvailabilityId,
                    x.ActivityTypeId,
                    x.AvailableDate,
                    x.StartTime,
                    x.EndTime,
                    x.IsBooked
                })
                .FirstOrDefaultAsync();

            if (availability == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Availability not found."
                });
            }

            return Json(new
            {
                success = true,
                data = availability
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvailability(
        EditCoachAvailabilityViewModel model)
        {
            int coachId =
                HttpContext.Session.GetInt32("UserId")!.Value;

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter all required details."
                });
            }

            var availability = await _context.CoachAvailabilities
                .FirstOrDefaultAsync(x =>
                    x.AvailabilityId == model.AvailabilityId &&
                    x.CoachId == coachId);

            if (availability == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Availability not found."
                });
            }

            // Booked slot protection
            if (availability.IsBooked)
            {
                return Json(new
                {
                    success = false,
                    message = "Booked availability cannot be edited."
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (model.AvailableDate < today)
            {
                return Json(new
                {
                    success = false,
                    message = "Past dates are not allowed."
                });
            }

            if (model.EndTime <= model.StartTime)
            {
                return Json(new
                {
                    success = false,
                    message = "End Time must be greater than Start Time."
                });
            }

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

            // Duplicate / overlap check
            bool overlapExists = await _context.CoachAvailabilities
                .AnyAsync(x =>
                    x.CoachId == coachId &&
                    x.AvailabilityId != model.AvailabilityId &&
                    x.AvailableDate == model.AvailableDate &&
                    x.IsActive &&
                    x.StartTime < model.EndTime &&
                    x.EndTime > model.StartTime);

            if (overlapExists)
            {
                return Json(new
                {
                    success = false,
                    message = "Another availability overlaps with this time slot."
                });
            }

            availability.ActivityTypeId = model.ActivityTypeId;
            availability.AvailableDate = model.AvailableDate;
            availability.StartTime = model.StartTime;
            availability.EndTime = model.EndTime;

            availability.ModifiedOn = DateTime.UtcNow;
            availability.ModifiedBy = coachId;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Availability updated successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeAvailabilityStatus(int id)
        {
            int coachId =
                HttpContext.Session.GetInt32("UserId")!.Value;

            var availability = await _context.CoachAvailabilities
                .FirstOrDefaultAsync(x =>
                    x.AvailabilityId == id &&
                    x.CoachId == coachId);

            if (availability == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Availability not found."
                });
            }

            if (availability.IsBooked)
            {
                return Json(new
                {
                    success = false,
                    message = "Booked availability status cannot be changed."
                });
            }

            availability.IsActive = !availability.IsActive;
            availability.ModifiedOn = DateTime.UtcNow;
            availability.ModifiedBy = coachId;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = availability.IsActive
                    ? "Availability activated successfully."
                    : "Availability deactivated successfully.",
                isActive = availability.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            int coachId =
                HttpContext.Session.GetInt32("UserId")!.Value;

            var availability = await _context.CoachAvailabilities
                .FirstOrDefaultAsync(x =>
                    x.AvailabilityId == id &&
                    x.CoachId == coachId);

            if (availability == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Availability not found."
                });
            }

            if (availability.IsBooked)
            {
                return Json(new
                {
                    success = false,
                    message = "Booked availability cannot be deleted."
                });
            }

            _context.CoachAvailabilities.Remove(availability);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Availability deleted successfully."
            });
        }
    }
}