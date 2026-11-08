using Microsoft.AspNetCore.Mvc;
using RaceDayMVC.Models;
using RaceDayMVC.Services;

namespace RaceDayMVC.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;
    private readonly CurrentUser _user;

    public AccountController(ApiClient api, CurrentUser user)
    {
        _api = api;
        _user = user;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var (ok, error, data) = await _api.LoginAsync(req);
        if (!ok || data is null)
        {
            ModelState.AddModelError("", error ?? "Login failed.");
            return View(req);
        }
        _user.SignIn(data);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var (ok, error, data) = await _api.RegisterAsync(req);
        if (!ok || data is null)
        {
            ModelState.AddModelError("", error ?? "Registration failed.");
            return View(req);
        }
        _user.SignIn(data);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        _user.SignOut();
        return RedirectToAction("Login");
    }
}