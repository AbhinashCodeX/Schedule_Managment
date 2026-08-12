using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Models;
using Schedule_Management.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        //[HttpGet]
        //public async Task<IActionResult> Users(string? search, string? role, string? status, string? sortOrder, int page = 1)
        //{
        //    string? roleName =
        //        HttpContext.Session.GetString("RoleName");

        //    if (roleName != "Admin")
        //    {
        //        return RedirectToAction(
        //            "Login",
        //            "Account"
        //        );
        //    }
        //    // STEP 1: Query start
        //    var query = _context.Users
        //     .Include(x => x.Role)
        //     .AsNoTracking()
        //     .AsQueryable();

        //    // STEP 2: Search filter    
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        search = search.Trim();

        //        query = query.Where(x =>
        //            x.FullName.Contains(search) ||
        //            x.Email.Contains(search) ||
        //              (x.PhoneNumber != null &&
        //                 x.PhoneNumber.Contains(search)));
        //    }

        //    // STEP 3: Role filter
        //    if (!string.IsNullOrWhiteSpace(role))
        //    {
        //        query = query.Where(x =>
        //            x.Role.RoleName == role);
        //    }

        //    if (!string.IsNullOrWhiteSpace(status))
        //    {
        //        if (status == "Active")
        //        {
        //            query = query.Where(x => x.IsActive == true);
        //        }
        //        else if (status == "Inactive")
        //        {
        //            query = query.Where(x => x.IsActive == false);
        //        }
        //    }

        //    query = sortOrder switch
        //    {
        //        "name_desc" => query.OrderByDescending(x => x.FullName),

        //        "oldest" => query.OrderBy(x => x.CreatedOn),

        //        "newest" => query.OrderByDescending(x => x.CreatedOn),

        //        _ => query.OrderBy(x => x.FullName)
        //    };


        //    // STEP 6: Pagination information
        //    int pageSize = 5;

        //    int totalRecords = await query.CountAsync();

        //    int totalPages = (int)Math.Ceiling(
        //        totalRecords / (double)pageSize
        //    );

        //    if (page < 1)
        //    {
        //        page = 1;
        //    }

        //    if (page > totalPages && totalPages > 0)
        //    {
        //        page = totalPages;
        //    }


        //    // STEP 4: Final query
        //    var users = await query
        //        .AsNoTracking()
        //        .Where(u =>
        //            u.Role.RoleName == "User" ||
        //            u.Role.RoleName == "Coach")
        //        .OrderByDescending(u => u.CreatedOn)
        //        .Select(u => new UserListViewModel
        //        {
        //            UserId = u.UserId,
        //            FullName = u.FullName,
        //            Email = u.Email,
        //            PhoneNumber = u.PhoneNumber,
        //            RoleName = u.Role.RoleName,

        //            DistrictName = u.District != null
        //                ? u.District.DistrictName
        //                : "Not Available",

        //            IsActive = u.IsActive,
        //            CreatedOn = u.CreatedOn
        //        })
        //        .ToListAsync();

        //    ViewBag.Search = search;
        //    ViewBag.Role = role;
        //    ViewBag.Status = status;
        //    ViewBag.SortOrder = sortOrder;
        //    return View(users);
        //}

        [HttpGet]
        public async Task<IActionResult> Users(
        string? search,
        string? role,
        string? status,
        string? sortOrder,
         int page = 1)
        {
            string? roleName =
                HttpContext.Session.GetString("RoleName");

            if (roleName != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }


            // STEP 1: Query start
            var query = _context.Users
                .Include(x => x.Role)
                .AsNoTracking()
                .AsQueryable();


            // Only User and Coach
            query = query.Where(x =>
                x.Role.RoleName == "User" ||
                x.Role.RoleName == "Coach");


            // STEP 2: Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.FullName.Contains(search) ||
                    x.Email.Contains(search) ||
                    (x.PhoneNumber != null &&
                     x.PhoneNumber.Contains(search)));
            }


            // STEP 3: Role Filter
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(x =>
                    x.Role.RoleName == role);
            }


            // STEP 4: Status Filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "Active")
                {
                    query = query.Where(x => x.IsActive);
                }
                else if (status == "Inactive")
                {
                    query = query.Where(x => !x.IsActive);
                }
            }


            // STEP 5: Sorting
            query = sortOrder switch
            {
                "name_desc" =>
                    query.OrderByDescending(x => x.FullName),

                "oldest" =>
                    query.OrderBy(x => x.CreatedOn),

                "newest" =>
                    query.OrderByDescending(x => x.CreatedOn),

                _ =>
                    query.OrderBy(x => x.FullName)
            };


            // STEP 6: Pagination information
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


            // STEP 7: Current page ka data
            var users = await query

                .Skip((page - 1) * pageSize)

                .Take(pageSize)

                .Select(x => new UserListViewModel
                {
                    UserId = x.UserId,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    RoleName = x.Role.RoleName,

                    DistrictName = x.District != null
                        ? x.District.DistrictName
                        : "Not Available",

                    IsActive = x.IsActive,
                    CreatedOn = x.CreatedOn
                })

                .ToListAsync();


            // STEP 8: ViewBag
            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.Status = status;
            ViewBag.SortOrder = sortOrder;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;


            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
          
            var user = await _context.Users
                .Where(x => x.UserId == id)
                .Select(x => new
                {
                    x.UserId,
                    x.FullName,
                    x.Email,
                    x.PhoneNumber,
                    x.FullAddress,
                    x.RoleId
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            return Json(new
            {
                success = true,
                data = user
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(EditUserViewModel model)
        {
            if (!IsAdminLoggedIn(out _))
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
                    message = "Please enter valid details."
                });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == model.UserId);

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            bool emailExists = await _context.Users
                .AnyAsync(x =>
                    x.Email == model.Email &&
                    x.UserId != model.UserId);

            if (emailExists)
            {
                return Json(new
                {
                    success = false,
                    message = "Email already exists."
                });
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.FullAddress = model.FullAddress;
            user.ModifiedOn = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "User updated successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserStatus(int id)
        {
            if (!IsAdminLoggedIn(out int adminId))
            {
                return Json(new
                {
                    success = false,
                    message = "Unauthorized access."
                });
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (user == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            user.IsActive = !user.IsActive;
            user.ModifiedOn = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = user.IsActive
                    ? "User activated successfully."
                    : "User deactivated successfully.",
                isActive = user.IsActive
            });
        }

        private bool IsAdminLoggedIn(out int adminId)
        {
            adminId = 0;

            string? roleName =
                HttpContext.Session.GetString("RoleName");

            int? userId =
                HttpContext.Session.GetInt32("UserId");

            if (roleName != "Admin" || !userId.HasValue)
            {
                return false;
            }

            adminId = userId.Value;
            return true;
        }
    }
}