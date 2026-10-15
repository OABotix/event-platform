using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts.Dtos.Auth;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class AuthTests
    {
        private RaceDayDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new RaceDayDbContext(options);
        }

        private AuthController GetController(RaceDayDbContext context)
        {
            return new AuthController(context, new PasswordHasher<User>());
        }

        [Fact]
        public async Task RegisterOrganiser_ValidData_ReturnsCreated()
        {
            var context = GetContext();
            context.Roles.Add(new Role { RoleName = "Organiser" });
            await context.SaveChangesAsync();

            var controller = GetController(context);
            var result = await controller.RegisterOrganiser(new RegisterRequest
            {
                FullName = "Test",
                Email = "t@t.com",
                Password = "Pass123!"
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task RegisterParticipant_ValidData_ReturnsCreated()
        {
            var context = GetContext();
            context.Roles.Add(new Role { RoleName = "Participant" });
            await context.SaveChangesAsync();

            var controller = GetController(context);
            var result = await controller.RegisterParticipant(new RegisterRequest
            {
                FullName = "Test",
                Email = "t@t.com",
                Password = "Pass123!"
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            var context = GetContext();
            context.Roles.Add(new Role { RoleName = "Organiser" });
            await context.SaveChangesAsync();

            var controller = GetController(context);
            var request = new RegisterRequest
            {
                FullName = "Test",
                Email = "t@t.com",
                Password = "Pass123!"
            };

            await controller.RegisterOrganiser(request);
            var result = await controller.RegisterOrganiser(request);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var context = GetContext();
            context.Roles.Add(new Role { RoleName = "Organiser" });
            await context.SaveChangesAsync();

            var controller = GetController(context);
            await controller.RegisterOrganiser(new RegisterRequest
            {
                FullName = "Test",
                Email = "t@t.com",
                Password = "Pass123!"
            });

            var result = await controller.Login(new LoginRequest
            {
                Email = "t@t.com",
                Password = "Pass123!"
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            var context = GetContext();
            context.Roles.Add(new Role { RoleName = "Organiser" });
            await context.SaveChangesAsync();

            var controller = GetController(context);
            await controller.RegisterOrganiser(new RegisterRequest
            {
                FullName = "Test",
                Email = "t@t.com",
                Password = "Pass123!"
            });

            var result = await controller.Login(new LoginRequest
            {
                Email = "t@t.com",
                Password = "WrongPass"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}