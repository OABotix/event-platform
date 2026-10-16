using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts.Dtos.Enrolments;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class EnrolmentTests
    {
        private RaceDayDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new RaceDayDbContext(options);
        }

        private EnrolmentsController GetController(RaceDayDbContext context, string role, int userId)
        {
            var controller = new EnrolmentsController(context);
            var httpContext = new DefaultHttpContext { Session = new TestSession() };
            httpContext.Session.SetInt32("UserId", userId);
            httpContext.Session.SetString("Role", role);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        [Fact]
        public async Task CreateEnrolment_AsParticipant_ReturnsCreated()
        {
            var context = GetContext();
            context.EventTypes.Add(new EventType { TypeName = "Run" });
            context.Users.Add(new User { RoleId = 1, FullName = "Org", Email = "o@o.com", PasswordHash = "x" });
            context.Users.Add(new User { RoleId = 2, FullName = "Part", Email = "p@p.com", PasswordHash = "x" });
            await context.SaveChangesAsync();

            var ev = new Event
            {
                OrganiserId = 1,
                EventTypeId = 1,
                EventName = "Test",
                Description = "Test",
                EventDate = DateTime.Now.AddDays(7),
                Location = "CPT",
                Distance = 10
            };
            context.Events.Add(ev);
            await context.SaveChangesAsync();

            var category = new Category { EventId = ev.EventId, CategoryName = "10km", EntryFee = 100 };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var controller = GetController(context, "Participant", 2);

            var result = await controller.CreateEnrolment(new CreateEnrolmentRequest
            {
                EventId = ev.EventId,
                CategoryId = category.CategoryId
            });

            Assert.IsType<CreatedAtActionResult>(result);
            var saved = await context.Enrolments.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal(2, saved!.ParticipantId);
            Assert.Equal("Pending", saved.Status);
        }

        [Fact]
        public async Task CreateEnrolment_AsOrganiser_ReturnsForbidden()
        {
            var context = GetContext();
            var controller = GetController(context, "Organiser", 1);

            var result = await controller.CreateEnrolment(new CreateEnrolmentRequest
            {
                EventId = 1,
                CategoryId = 1
            });

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, obj.StatusCode);
        }

        [Fact]
        public async Task CreateEnrolment_Duplicate_ReturnsConflict()
        {
            var context = GetContext();
            context.Users.Add(new User { RoleId = 2, FullName = "P", Email = "p@p.com", PasswordHash = "x" });
            context.EventTypes.Add(new EventType { TypeName = "Run" });
            await context.SaveChangesAsync();

            var ev = new Event
            {
                OrganiserId = 1,
                EventTypeId = 1,
                EventName = "T",
                Description = "T",
                EventDate = DateTime.Now.AddDays(7),
                Location = "L",
                Distance = 5
            };
            context.Events.Add(ev);
            await context.SaveChangesAsync();

            var cat = new Category { EventId = ev.EventId, CategoryName = "5km", EntryFee = 50 };
            context.Categories.Add(cat);
            await context.SaveChangesAsync();

            var controller = GetController(context, "Participant", 1);
            var request = new CreateEnrolmentRequest { EventId = ev.EventId, CategoryId = cat.CategoryId };

            await controller.CreateEnrolment(request);
            var result = await controller.CreateEnrolment(request);

            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}