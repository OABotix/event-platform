using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;
using RaceDay.Contracts.Dtos.Enrolments;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public EnrolmentsController(RaceDayDbContext context) => _context = context;

        private IActionResult Forbidden(string message) => StatusCode(403, new { message });

        [HttpPost]
        public async Task<IActionResult> CreateEnrolment([FromBody] CreateEnrolmentRequest request)
        {
            if (!SessionHelper.IsParticipant(HttpContext))
                return Forbidden("Only Participants can enrol");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;

            var ev = await _context.Events.FindAsync(request.EventId);
            if (ev == null) return NotFound(new { message = "Event not found" });

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId && c.EventId == request.EventId);
            if (category == null)
                return BadRequest(new { message = "Category not found for this event" });

            var existing = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.ParticipantId == userId && e.EventId == request.EventId);
            if (existing != null)
                return Conflict(new { message = "Already enrolled in this event" });

            var enrolment = new Enrolment
            {
                ParticipantId = userId,
                EventId = request.EventId,
                CategoryId = request.CategoryId,
                EnrolmentDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyEnrolments), null, enrolment.EnrolmentId);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            if (!SessionHelper.IsParticipant(HttpContext))
                return Forbidden("Only Participants can view their enrolments");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;

            var enrolments = await _context.Enrolments
                .Include(e => e.Event).Include(e => e.Category).Include(e => e.Participant)
                .Where(e => e.ParticipantId == userId)
                .Select(e => new EnrolmentDto
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    ParticipantName = e.Participant!.FullName,
                    EventId = e.EventId,
                    EventName = e.Event!.EventName,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category!.CategoryName,
                    EnrolmentDate = e.EnrolmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can view event enrolments");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var ev = await _context.Events.FindAsync(eventId);

            if (ev == null) return NotFound();
            if (ev.OrganiserId != userId)
                return Forbidden("You can only view enrolments for your own events");

            var enrolments = await _context.Enrolments
                .Include(e => e.Event).Include(e => e.Category).Include(e => e.Participant)
                .Where(e => e.EventId == eventId)
                .Select(e => new EnrolmentDto
                {
                    EnrolmentId = e.EnrolmentId,
                    ParticipantId = e.ParticipantId,
                    ParticipantName = e.Participant!.FullName,
                    EventId = e.EventId,
                    EventName = e.Event!.EventName,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category!.CategoryName,
                    EnrolmentDate = e.EnrolmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateEnrolmentStatus(int id, [FromBody] UpdateEnrolmentStatusRequest request)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can update enrolment status");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var enrolment = await _context.Enrolments.Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == id);

            if (enrolment == null) return NotFound();
            if (enrolment.Event!.OrganiserId != userId)
                return Forbidden("You can only manage enrolments for your own events");

            enrolment.Status = request.Status;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}