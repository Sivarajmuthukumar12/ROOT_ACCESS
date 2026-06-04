using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Services;
using DoctorAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentSystem.Controllers.UI;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetString("Token") != null)
            return RedirectToAction("Index", "Home");

        return View("~/Views/Account/Login.cshtml", new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Account/Login.cshtml", model);

        var result = await _authService.LoginAsync(new LoginRequest(model.Email, model.Password));
        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View("~/Views/Account/Login.cshtml", model);
        }

        HttpContext.Session.SetString("Token", result.Token);
        HttpContext.Session.SetString("UserId", result.UserId.ToString());
        HttpContext.Session.SetString("UserName", result.FullName);
        HttpContext.Session.SetString("Email", result.Email);
        HttpContext.Session.SetString("Role", result.Role.ToString());

        TempData["Success"] = $"Welcome back, {result.FullName}!";

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return result.Role.ToString() == "Admin"
            ? RedirectToAction("Dashboard", "Admin")
            : RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetString("Token") != null)
            return RedirectToAction("Index", "Home");

        return View("~/Views/Account/Register.cshtml", new RegisterViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Account/Register.cshtml", model);

        var result = await _authService.RegisterAsync(new RegisterRequest(
            model.FullName, model.Email, model.Password, model.Phone));

        if (result == null)
        {
            ModelState.AddModelError("Email", "This email is already registered.");
            return View("~/Views/Account/Register.cshtml", model);
        }

        HttpContext.Session.SetString("Token", result.Token);
        HttpContext.Session.SetString("UserId", result.UserId.ToString());
        HttpContext.Session.SetString("UserName", result.FullName);
        HttpContext.Session.SetString("Email", result.Email);
        HttpContext.Session.SetString("Role", result.Role.ToString());

        TempData["Success"] = $"Account created! Welcome, {result.FullName}!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }
}
