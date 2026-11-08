using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;

namespace RaceDayMVC.Controllers;

[RequireAuth(Role = "Participant")]
public class EnrolmentsController : Controller
{
    public IActionResult Index() => View("~/Views/Shared/_ComingSoon.cshtml", "My Enrolments");
}