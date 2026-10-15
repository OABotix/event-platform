using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
      public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options)
      {
      }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<EventType>()
                .HasIndex(et => et.TypeName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Composite unique constraint on Enrolment
            modelBuilder.Entity<Enrolment>()
                .HasIndex(e => new { e.ParticipantID, e.EventID })
                .IsUnique();

            // One-to-one: Enrolment -> Result
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID);
        }
    }
}
