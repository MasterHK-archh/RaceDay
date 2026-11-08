using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;

namespace RaceDayMVC.Controllers;

[RequireAuth(Role = "Organiser")]
public class DashboardController : Controller
{
    public IActionResult Index() => View("~/Views/Shared/_ComingSoon.cshtml", "Organiser Dashboard");
}