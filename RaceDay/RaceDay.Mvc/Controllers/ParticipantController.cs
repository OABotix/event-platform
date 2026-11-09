using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Dtos.Enrolments;
using RaceDay.Contracts.Dtos.Events;
using RaceDay.Contracts.Dtos.Profile;
using RaceDay.Contracts.Dtos.Results;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class ParticipantController : Controller
{
    private readonly RaceDayApiClient _api;

    public ParticipantController(RaceDayApiClient api) => _api = api;

    private bool IsParticipant() => HttpContext.Session.GetString("Role") == "Participant";

    public async Task<IActionResult> Index(string? type, string? search)
    {
        if (!IsParticipant()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, "api/events");
        var events = await _api.ReadAsync<List<EventDto>>(response) ?? new();

        events = events.Where(e => e.EventDate >= DateTime.Today).ToList();

        if (!string.IsNullOrWhiteSpace(type))
            events = events.Where(e => e.EventTypeName == type).ToList();

        if (!string.IsNullOrWhiteSpace(search))
            events = events.Where(e =>
                e.EventName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                e.Location.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        ViewBag.TypeFilter = type;
        ViewBag.SearchFilter = search;
        return View(events);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!IsParticipant()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}");
        var ev = await _api.ReadAsync<EventDto>(response);
        if (ev == null) return NotFound();
        return View(ev);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnterEvent(int eventId, int categoryId)
    {
        if (!IsParticipant()) return RedirectToAction("Login", "Auth");

        var request = new CreateEnrolmentRequest { EventId = eventId, CategoryId = categoryId };
        var response = await _api.SendAsync(HttpMethod.Post, "api/enrolments", request);

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        TempData["Success"] = "You have entered the event successfully.";
        return RedirectToAction(nameof(MyEvents));
    }

    public async Task<IActionResult> MyEvents()
    {
        if (!IsParticipant()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, "api/enrolments/mine");
        var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(response) ?? new();
        return View(enrolments);
    }

    public async Task<IActionResult> MyResults()
    {
        if (!IsParticipant()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, "api/results/mine");
        var results = await _api.ReadAsync<List<ResultDto>>(response) ?? new();
        return View(results);
    }

    public async Task<IActionResult> Profile()
    {
        if (!IsParticipant()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, "api/profile");
        var profile = await _api.ReadAsync<ProfileDto>(response);
        if (profile == null) return NotFound();
        return View(profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UpdateProfileRequest model)
    {
        if (!IsParticipant()) return RedirectToAction("Login", "Auth");

        await _api.SendAsync(HttpMethod.Put, "api/profile", model);
        TempData["Success"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }
}