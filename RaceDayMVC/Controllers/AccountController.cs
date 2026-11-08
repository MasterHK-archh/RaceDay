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
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest req, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(req);

        var (ok, error, data) = await _api.LoginAsync(req);
        if (!ok || data is null)
        {
            ModelState.AddModelError("", ApiErrorHelper.Prettify(error));
            return View(req);
        }

        _user.SignIn(data);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (!ModelState.IsValid) return View(req);

        var (ok, error, data) = await _api.RegisterAsync(req);
        if (!ok || data is null)
        {
            ModelState.AddModelError("", ApiErrorHelper.Prettify(error));
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