using Microsoft.AspNetCore.Mvc;

namespace Schedule_Management.Controllers
{
    public class AdminController : Controller
    {
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
    }
}