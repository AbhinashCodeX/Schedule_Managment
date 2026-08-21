using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Filters;
using Schedule_Management.Models;
using Schedule_Management.ViewModels;

namespace Schedule_Management.Controllers
{
    [RoleAuthorize("User")]
    public class UserController : Controller
    {
        private readonly ScheduleManagementDbContext _context;
        public UserController(ScheduleManagementDbContext context)
        {
            _context = context;
        }
        //Dashboard View Return 
        public IActionResult Dashboard()
        {
            return View();
        }


        //BookActivity GET Method 
        [HttpGet]
        public async Task<IActionResult> BookActivity()
       {
            ViewBag.ActivityTypes = await _context.ActivityTypes
                .Where(x => x.IsActive)
                .OrderBy(x => x.ActivityName)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCoachesByActivity(int activityTypeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var coaches = await _context.CoachAvailabilities
                .Where(x =>
                    x.ActivityTypeId == activityTypeId &&
                    x.IsActive &&
                    !x.IsBooked &&
                    x.AvailableDate >= today)
                .Select(x => new
                {
                    CoachId = x.CoachId,
                    CoachName = x.Coach.FullName
                })
                .Distinct()
                .OrderBy(x => x.CoachName)
                .ToListAsync();

            return Json(coaches);
        }
        [HttpGet]
        public async Task<IActionResult> GetAvailableDates(
           int activityTypeId,
           int coachId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var dates = await _context.CoachAvailabilities
                .Where(x =>
                    x.ActivityTypeId == activityTypeId &&
                    x.CoachId == coachId &&
                    x.IsActive &&
                    !x.IsBooked &&
                    x.AvailableDate >= today)
                .Select(x => x.AvailableDate)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var result = dates.Select(x => new
            {
                value = x.ToString("yyyy-MM-dd"),
                text = x.ToString("dd MMM yyyy")
            });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(int activityTypeId,int coachId,DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (date < today)
            {
                return Json(new List<object>());
            }

            var slots = await _context.CoachAvailabilities
                .Where(x =>
                    x.ActivityTypeId == activityTypeId &&
                    x.CoachId == coachId &&
                    x.AvailableDate == date &&
                    x.IsActive &&
                    !x.IsBooked)
                .OrderBy(x => x.StartTime)
                .Select(x => new
                {
                    availabilityId = x.AvailabilityId,
                    startTime = x.StartTime.ToString("HH:mm"),
                    endTime = x.EndTime.ToString("HH:mm")
                })
                .ToListAsync();

            return Json(slots);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookActivity(int availabilityId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return Json(new
                {
                    success = false,
                    message = "Session expired. Please login again."
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var availability = await _context.CoachAvailabilities
                    .FirstOrDefaultAsync(x =>
                        x.AvailabilityId == availabilityId);

                if (availability == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Selected availability was not found."
                    });
                }

                if (!availability.IsActive)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This slot is no longer available."
                    });
                }

                if (availability.IsBooked)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This slot has already been booked."
                    });
                }

                if (availability.AvailableDate < today)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Past slots cannot be booked."
                    });
                }

                bool alreadyBooked = await _context.Bookings    
                 .AnyAsync(x =>
                     x.UserId == userId.Value &&
                     x.AvailabilityId == availabilityId &&
                     x.IsActive &&
                     x.BookingStatus == "Confirmed");

                if (alreadyBooked)
                {
                    return Json(new
                    {
                        success = false,
                        message = "You have already booked this slot."
                    });
                }

                var booking = new Booking
                {
                    UserId = userId.Value,
                    AvailabilityId = availability.AvailabilityId,

                    BookingStatus = "Confirmed",
                    BookedOn = DateTime.UtcNow,

                    IsActive = true,

                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userId.Value
                };

                await _context.Bookings.AddAsync(booking);

                availability.IsBooked = true;
                availability.ModifiedOn = DateTime.UtcNow;
                availability.ModifiedBy = userId.Value;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Activity booked successfully."
                });
            }
            catch
            {
                await transaction.RollbackAsync();

                return Json(new
                {
                    success = false,
                    message = "Something went wrong while booking the activity."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return Json(new
                {
                    success = false,
                    message = "Session expired. Please login again."
                });
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(x => x.Availability)
                    .FirstOrDefaultAsync(x =>
                        x.BookingId == id &&
                        x.UserId == userId.Value);

                if (booking == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Booking not found."
                    });
                }

                if (booking.BookingStatus == "Cancelled")
                {
                    return Json(new
                    {
                        success = false,
                        message = "This booking is already cancelled."
                    });
                }

                if (booking.BookingStatus == "Completed")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Completed booking cannot be cancelled."
                    });
                }

                var today = DateOnly.FromDateTime(DateTime.Today);

                if (booking.Availability.AvailableDate < today)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Past bookings cannot be cancelled."
                    });
                }

                booking.BookingStatus = "Cancelled";
                booking.IsActive = false;
                booking.ModifiedOn = DateTime.UtcNow;
                booking.ModifiedBy = userId.Value;

                booking.Availability.IsBooked = false;
                booking.Availability.ModifiedOn = DateTime.UtcNow;
                booking.Availability.ModifiedBy = userId.Value;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Booking cancelled successfully."
                });
            }
            catch
            {
                await transaction.RollbackAsync();

                return Json(new
                {
                    success = false,
                    message = "Something went wrong while cancelling the booking."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings(string? status)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Bookings
                .Where(x => x.UserId == userId.Value)
                .Include(x => x.Availability)
                    .ThenInclude(x => x.ActivityType)
                .Include(x => x.Availability)
                    .ThenInclude(x => x.Coach)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x =>
                    x.BookingStatus == status);
            }

            var bookings = await query
                .OrderByDescending(x => x.BookedOn)
                .Select(x => new MyBookingViewModel
                {
                    BookingId = x.BookingId,

                    ActivityName =
                        x.Availability.ActivityType.ActivityName,

                    CoachName =
                        x.Availability.Coach.FullName,

                    BookingDate =
                        x.Availability.AvailableDate,

                    StartTime =
                        x.Availability.StartTime,

                    EndTime =
                        x.Availability.EndTime,

                    BookingStatus =
                        x.BookingStatus,

                    IsActive =
                        x.IsActive
                })
                .ToListAsync();

            ViewBag.Status = status;

            return View(bookings);
        }
    }
}