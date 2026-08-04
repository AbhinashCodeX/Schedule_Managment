using Microsoft.AspNetCore.Mvc;

namespace Schedule_Management.Controllers
{
    public class AdminController : Controller
    {
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
    }
}