using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;
using RaceDay.Contracts.Dtos.Categories;
using RaceDay.Contracts.Dtos.Events;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context) => _context = context;

        private IActionResult Forbidden(string message) => StatusCode(403, new { message });

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return Unauthorized(new { message = "Not logged in" });

            var events = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Organiser)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventTypeName = e.EventType!.TypeName,
                    OrganiserId = e.OrganiserId,
                    OrganiserName = e.Organiser!.FullName
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return Unauthorized(new { message = "Not logged in" });

            var ev = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Organiser)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();

            return Ok(new EventDto
            {
                EventId = ev.EventId,
                EventName = ev.EventName,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Distance = ev.Distance,
                EventTypeName = ev.EventType!.TypeName,
                OrganiserId = ev.OrganiserId,
                OrganiserName = ev.Organiser!.FullName,
                Categories = ev.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    EntryFee = c.EntryFee
                }).ToList()
            });
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyEvents()
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can view their events");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;

            var events = await _context.Events
                .Include(e => e.EventType)
                .Where(e => e.OrganiserId == userId)
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventTypeName = e.EventType!.TypeName,
                    OrganiserId = e.OrganiserId
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can create events");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;

            var ev = new Event
            {
                OrganiserId = userId,
                EventTypeId = request.EventTypeId,
                EventName = request.EventName,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                Distance = request.Distance,
                CreatedAt = DateTime.Now
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = ev.EventId }, ev.EventId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can update events");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();
            if (ev.OrganiserId != userId) return Forbidden("You can only edit your own events");

            ev.EventName = request.EventName;
            ev.Description = request.Description;
            ev.EventDate = request.EventDate;
            ev.Location = request.Location;
            ev.Distance = request.Distance;
            ev.EventTypeId = request.EventTypeId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can delete events");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();
            if (ev.OrganiserId != userId) return Forbidden("You can only delete your own events");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}