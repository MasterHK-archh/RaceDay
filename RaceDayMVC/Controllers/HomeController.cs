using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class HomeController : Controller
{
    private readonly CurrentUser _user;
    public HomeController(CurrentUser user) => _user = user;

    public IActionResult Index()
    {
        if (!_user.IsAuthenticated) return View();
        if (_user.IsOrganiser) return RedirectToAction("Index", "Dashboard");
        if (_user.IsParticipant) return RedirectToAction("Index", "Events");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Forbidden() => View();

    public IActionResult Error() => View();
}