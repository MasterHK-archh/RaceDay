using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;

namespace RaceDayMVC.Controllers;

[RequireAuth]
public class ProfileController : Controller
{
    public IActionResult Index() => View("~/Views/Shared/_ComingSoon.cshtml", "Profile");
}