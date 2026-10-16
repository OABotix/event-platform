using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts.Dtos.Events;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class EventTests
    {
        private RaceDayDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new RaceDayDbContext(options);
        }

        private EventsController GetController(RaceDayDbContext context, string role, int userId)
        {
            var controller = new EventsController(context);
            var httpContext = new DefaultHttpContext { Session = new TestSession() };
            httpContext.Session.SetInt32("UserId", userId);
            httpContext.Session.SetString("Role", role);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            return controller;
        }

        [Fact]
        public async Task CreateEvent_AsParticipant_ReturnsForbidden()
        {
            var context = GetContext();
            var controller = GetController(context, "Participant", 1);

            var result = await controller.CreateEvent(new CreateEventRequest
            {
                EventName = "T",
                Description = "T",
                EventDate = DateTime.Now.AddDays(7),
                Location = "T",
                Distance = 10,
                EventTypeId = 1
            });

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, obj.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_AsOrganiser_ReturnsCreated()
        {
            var context = GetContext();
            context.EventTypes.Add(new EventType { TypeName = "Run" });
            await context.SaveChangesAsync();

            var controller = GetController(context, "Organiser", 1);

            var result = await controller.CreateEvent(new CreateEventRequest
            {
                EventName = "T",
                Description = "T",
                EventDate = DateTime.Now.AddDays(7),
                Location = "T",
                Distance = 10,
                EventTypeId = 1
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task GetEvents_Unauthenticated_ReturnsUnauthorized()
        {
            var context = GetContext();
            var controller = new EventsController(context);
            var httpContext = new DefaultHttpContext { Session = new TestSession() };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = await controller.GetAllEvents();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}