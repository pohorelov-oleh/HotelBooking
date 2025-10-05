using HotelBooking.Application.Services;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _auth;

    public AccountController(IAuthService auth) => _auth = auth;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null) => View(model: returnUrl);

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl, CancellationToken ct)
    {
        var user = await _auth.LoginAsync(email, password, ct);
        if (user is null)
        {
            ModelState.AddModelError("", "Incorrect login or password");
            return View(model: returnUrl);
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "app-cookie"));
        await HttpContext.SignInAsync("app-cookie", principal);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string email, string password, string? fullName, CancellationToken ct)
    {
        var id = await _auth.RegisterAsync(email, password, fullName, makeAdmin: false, ct);
        if (id <= 0)
        {
            ModelState.AddModelError("", "Failed to register user");
            return View();
        }
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("app-cookie");
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult Denied() => View();
}