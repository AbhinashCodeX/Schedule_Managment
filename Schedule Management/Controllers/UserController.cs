using Microsoft.AspNetCore.Mvc;
using Schedule_Management.Models;
using Microsoft.EntityFrameworkCore;
using Schedule_Management.Filters;

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
    }
}