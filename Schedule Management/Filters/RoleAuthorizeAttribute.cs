using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Schedule_Management.Filters
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _role;

        public RoleAuthorizeAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? roleName =
                context.HttpContext.Session.GetString("RoleName");

            int? userId =
                context.HttpContext.Session.GetInt32("UserId");

            bool unauthorized =
                string.IsNullOrEmpty(roleName) ||
                !userId.HasValue ||
                roleName != _role;

            if (!unauthorized)
            {
                return;
            }

            bool isAjax =
                context.HttpContext.Request.Headers["X-Requested-With"]
                    .ToString()
                    .Equals(
                        "XMLHttpRequest",
                        StringComparison.OrdinalIgnoreCase
                    );

            if (isAjax)
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    message = "Unauthorized access."
                });

                return;
            }

            context.Result = new RedirectToActionResult(
                "Login",
                "Account",
                null
            );
        }
    }
}