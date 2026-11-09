using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts.Dtos.Categories;
using RaceDay.Contracts.Dtos.Enrolments;
using RaceDay.Contracts.Dtos.Events;
using RaceDay.Contracts.Dtos.Results;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers;

public class OrganiserController : Controller
{
    private readonly RaceDayApiClient _api;

    public OrganiserController(RaceDayApiClient api) => _api = api;

    private bool IsOrganiser() => HttpContext.Session.GetString("Role") == "Organiser";

    public async Task<IActionResult> Index()
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

        var vm = new OrganiserDashboardVm();

        var eventsResponse = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
        var events = await _api.ReadAsync<List<EventDto>>(eventsResponse) ?? new();

        foreach (var ev in events)
        {
            var enrolResponse = await _api.SendAsync(HttpMethod.Get, $"api/enrolments/event/{ev.EventId}");
            var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(enrolResponse) ?? new();

            vm.Events.Add(new OrganiserEventSummaryVm
            {
                EventId = ev.EventId,
                EventName = ev.EventName,
                EventDate = ev.EventDate,
                EnrolmentCount = enrolments.Count
            });
        }

        return View(vm);
    }

    public async Task<IActionResult> Events()
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
        var events = await _api.ReadAsync<List<EventDto>>(response) ?? new();
        return View(events);
    }

    [HttpGet]
    public IActionResult CreateEvent()
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(CreateEventRequest model)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
        if (!ModelState.IsValid) return View(model);

        // 1. Send the create request
        var response = await _api.SendAsync(HttpMethod.Post, "api/events", model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
            return View(model);
        }

        // 2. Read the new Event ID from the API response
        // The API returns the raw integer ID (e.g., 5) in the response body
        var responseContent = await response.Content.ReadAsStringAsync();
        if (int.TryParse(responseContent, out int newEventId))
        {
            TempData["Success"] = "Event created! Now add categories for this event.";
            // 3. Redirect straight to the Categories page for this event
            return RedirectToAction(nameof(Categories), new { eventId = newEventId });
        }

        // Fallback if parsing fails (just in case)
        return RedirectToAction(nameof(Events));
    }

    [HttpGet]
    public async Task<IActionResult> EditEvent(int id)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}");
        var ev = await _api.ReadAsync<EventDto>(response);
        if (ev == null) return NotFound();

        return View(new UpdateEventRequest
        {
            EventName = ev.EventName,
            Description = ev.Description,
            EventDate = ev.EventDate,
            Location = ev.Location,
            Distance = ev.Distance,
            EventTypeId = 1
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(int id, UpdateEventRequest model)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
        if (!ModelState.IsValid) return View(model);

        await _api.SendAsync(HttpMethod.Put, $"api/events/{id}", model);
        return RedirectToAction(nameof(Events));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
        await _api.SendAsync(HttpMethod.Delete, $"api/events/{id}");
        return RedirectToAction(nameof(Events));
    }

    public async Task<IActionResult> Categories(int eventId)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, $"api/events/{eventId}/categories");
        var categories = await _api.ReadAsync<List<CategoryDto>>(response) ?? new();
        ViewBag.EventId = eventId;
        return View(categories);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(int eventId, CreateCategoryRequest model)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
        await _api.SendAsync(HttpMethod.Post, $"api/events/{eventId}/categories", model);
        return RedirectToAction(nameof(Categories), new { eventId });
    }

    public async Task<IActionResult> Enrolments(int eventId)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, $"api/enrolments/event/{eventId}");
        var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(response) ?? new();
        ViewBag.EventId = eventId;
        return View(enrolments);
    }

    public async Task<IActionResult> Results(int eventId)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");

        var response = await _api.SendAsync(HttpMethod.Get, $"api/results/event/{eventId}");
        var results = await _api.ReadAsync<List<ResultDto>>(response) ?? new();
        ViewBag.EventId = eventId;
        return View(results);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CaptureResult(int eventId, CreateResultRequest model)
    {
        if (!IsOrganiser()) return RedirectToAction("Login", "Auth");
        await _api.SendAsync(HttpMethod.Post, "api/results", model);
        return RedirectToAction(nameof(Results), new { eventId });
    }
}