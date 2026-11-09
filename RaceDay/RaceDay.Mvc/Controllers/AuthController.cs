using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Dtos.Auth;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly RaceDayApiClient _api;

    public AuthController(RaceDayApiClient api) => _api = api;

    // GET /Auth/RegisterOrganiser
    [HttpGet]
    public IActionResult RegisterOrganiser() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterOrganiser(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var response = await _api.SendAsync(HttpMethod.Post, "api/auth/register/organiser",
            new RegisterRequest
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Password = vm.Password,
                PhoneNumber = vm.PhoneNumber
            });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Registration failed. Email may already exist.");
            return View(vm);
        }

        TempData["Success"] = "Organiser account created. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    // GET /Auth/RegisterParticipant
    [HttpGet]
    public IActionResult RegisterParticipant() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterParticipant(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var response = await _api.SendAsync(HttpMethod.Post, "api/auth/register",
            new RegisterRequest
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Password = vm.Password,
                PhoneNumber = vm.PhoneNumber
            });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Registration failed. Email may already exist.");
            return View(vm);
        }

        TempData["Success"] = "Participant account created. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var response = await _api.SendAsync(HttpMethod.Post, "api/auth/login",
            new LoginRequest { Email = vm.Email, Password = vm.Password });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        var auth = await _api.ReadAsync<AuthResponse>(response);
        if (auth == null)
        {
            ModelState.AddModelError("", "Login response could not be read.");
            return View(vm);
        }

        var apiCookie = RaceDayApiClient.ExtractSessionCookie(response);
        if (!string.IsNullOrWhiteSpace(apiCookie))
            HttpContext.Session.SetString("ApiSessionCookie", apiCookie);

        HttpContext.Session.SetInt32("UserId", auth.UserId);
        HttpContext.Session.SetString("Role", auth.Role);
        HttpContext.Session.SetString("DisplayName", auth.FullName);

        return auth.Role == "Organiser"
            ? RedirectToAction("Index", "Organiser")
            : RedirectToAction("Index", "Participant");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _api.SendAsync(HttpMethod.Post, "api/auth/logout");
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}