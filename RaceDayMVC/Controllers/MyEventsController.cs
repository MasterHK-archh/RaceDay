using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Filters;

namespace RaceDayMVC.Controllers;

[RequireAuth(Role = "Organiser")]
public class MyEventsController : Controller
{
    public IActionResult Index() => View("~/Views/Shared/_ComingSoon.cshtml", "My Events");
    public IActionResult Create() => View("~/Views/Shared/_ComingSoon.cshtml", "Create Event");
    public IActionResult Edit(int id) => View("~/Views/Shared/_ComingSoon.cshtml", "Edit Event");
    public IActionResult Categories(int eventId) => View("~/Views/Shared/_ComingSoon.cshtml", "Event Categories");
    public IActionResult Enrolments(int eventId) => View("~/Views/Shared/_ComingSoon.cshtml", "Event Enrolments");
    public IActionResult Results(int eventId) => View("~/Views/Shared/_ComingSoon.cshtml", "Event Results");
}