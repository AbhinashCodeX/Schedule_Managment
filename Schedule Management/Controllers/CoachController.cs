using Microsoft.AspNetCore.Mvc;

namespace Schedule_Management.Controllers
{
    public class CoachController : Controller
    {
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
    }
}