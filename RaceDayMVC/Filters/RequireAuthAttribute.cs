using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace RaceDayMVC.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAuthAttribute : ActionFilterAttribute
{
    /// If set, the signed-in user's role must match this value.
    public string? Role { get; set; }

    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        var session = ctx.HttpContext.Session;
        var token = session.GetString("JwtToken");

        // Not logged in → bounce to login with return URL
        if (string.IsNullOrEmpty(token))
        {
            ctx.Result = new RedirectToActionResult(
                "Login", "Account",
                new { returnUrl = ctx.HttpContext.Request.Path.ToString() });
            return;
        }

        // Logged in but wrong role → forbidden
        if (!string.IsNullOrEmpty(Role))
        {
            var userRole = session.GetString("RoleName");
            if (!string.Equals(userRole, Role, StringComparison.OrdinalIgnoreCase))
            {
                ctx.Result = new RedirectToActionResult("Forbidden", "Home", null);
            }
        }
    }
}