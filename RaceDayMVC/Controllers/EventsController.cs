using Microsoft.AspNetCore.Mvc;

namespace RaceDayMVC.Controllers;

public class EventsController : Controller
{
    public IActionResult Index() => View("~/Views/Shared/_ComingSoon.cshtml", "Events listing");
    public IActionResult Details(int id) => View("~/Views/Shared/_ComingSoon.cshtml", "Event details");
}