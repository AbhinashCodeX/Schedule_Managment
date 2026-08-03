using Microsoft.AspNetCore.Mvc;

namespace Schedule_Management.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            string? role =
                HttpContext.Session.GetString("RoleName");

            if (role != "User")
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