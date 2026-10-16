using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(RaceDayDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Organiser" },
                    new Role { RoleName = "Participant" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.EventTypes.AnyAsync())
            {
                context.EventTypes.AddRange(
                    new EventType { TypeName = "Run" },
                    new EventType { TypeName = "Walk" },
                    new EventType { TypeName = "Cycle" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}