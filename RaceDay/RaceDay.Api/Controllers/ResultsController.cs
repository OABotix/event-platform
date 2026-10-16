using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;
using RaceDay.Contracts.Dtos.Results;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public ResultsController(RaceDayDbContext context) => _context = context;

        private IActionResult Forbidden(string message) => StatusCode(403, new { message });

        [HttpPost]
        public async Task<IActionResult> CreateResult([FromBody] CreateResultRequest request)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can capture results");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var enrolment = await _context.Enrolments.Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == request.EnrolmentId);

            if (enrolment == null) return NotFound(new { message = "Enrolment not found" });
            if (enrolment.Event!.OrganiserId != userId)
                return Forbidden("You can only capture results for your own events");

            var existing = await _context.Results
                .FirstOrDefaultAsync(r => r.EnrolmentId == request.EnrolmentId);
            if (existing != null)
                return Conflict(new { message = "Result already exists for this enrolment" });

            var result = new Result
            {
                EnrolmentId = request.EnrolmentId,
                FinishTime = request.FinishTime,
                FinishPosition = request.FinishPosition,
                PublishedAt = DateTime.Now
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyResults), null, result.ResultId);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyResults()
        {
            if (!SessionHelper.IsParticipant(HttpContext))
                return Forbidden("Only Participants can view their results");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;

            var results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e!.Event)
                .Include(r => r.Enrolment).ThenInclude(e => e!.Category)
                .Include(r => r.Enrolment).ThenInclude(e => e!.Participant)
                .Where(r => r.Enrolment!.ParticipantId == userId)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    ParticipantName = r.Enrolment!.Participant!.FullName,
                    EventName = r.Enrolment.Event!.EventName,
                    CategoryName = r.Enrolment.Category!.CategoryName,
                    EventDate = r.Enrolment.Event.EventDate,
                    FinishTime = r.FinishTime,
                    FinishPosition = r.FinishPosition,
                    PublishedAt = r.PublishedAt
                })
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can view event results");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var ev = await _context.Events.FindAsync(eventId);

            if (ev == null) return NotFound();
            if (ev.OrganiserId != userId)
                return Forbidden("You can only view results for your own events");

            var results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e!.Event)
                .Include(r => r.Enrolment).ThenInclude(e => e!.Category)
                .Include(r => r.Enrolment).ThenInclude(e => e!.Participant)
                .Where(r => r.Enrolment!.EventId == eventId)
                .Select(r => new ResultDto
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    ParticipantName = r.Enrolment!.Participant!.FullName,
                    EventName = r.Enrolment.Event!.EventName,
                    CategoryName = r.Enrolment.Category!.CategoryName,
                    EventDate = r.Enrolment.Event.EventDate,
                    FinishTime = r.FinishTime,
                    FinishPosition = r.FinishPosition,
                    PublishedAt = r.PublishedAt
                })
                .ToListAsync();

            return Ok(results);
        }
    }
}