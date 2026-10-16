using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;
using RaceDay.Contracts.Dtos.Categories;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public CategoriesController(RaceDayDbContext context) => _context = context;

        private IActionResult Forbidden(string message) => StatusCode(403, new { message });

        [HttpGet("events/{eventId}/categories")]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext))
                return Unauthorized(new { message = "Not logged in" });

            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    EntryFee = c.EntryFee
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost("events/{eventId}/categories")]
        public async Task<IActionResult> AddCategory(int eventId, [FromBody] CreateCategoryRequest request)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can add categories");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev == null) return NotFound();
            if (ev.OrganiserId != userId) return Forbidden("You can only manage your own events");

            var category = new Category
            {
                EventId = eventId,
                CategoryName = request.CategoryName,
                Description = request.Description,
                EntryFee = request.EntryFee
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { eventId }, category.CategoryId);
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can update categories");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var category = await _context.Categories.Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();
            if (category.Event!.OrganiserId != userId)
                return Forbidden("You can only manage categories for your own events");

            category.CategoryName = request.CategoryName;
            category.Description = request.Description;
            category.EntryFee = request.EntryFee;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!SessionHelper.IsOrganiser(HttpContext))
                return Forbidden("Only Organisers can delete categories");

            var userId = SessionHelper.GetUserId(HttpContext)!.Value;
            var category = await _context.Categories.Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();
            if (category.Event!.OrganiserId != userId)
                return Forbidden("You can only delete categories for your own events");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}