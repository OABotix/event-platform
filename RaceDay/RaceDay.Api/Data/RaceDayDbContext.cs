using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
            : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasIndex(r => r.RoleName).IsUnique();
            modelBuilder.Entity<EventType>().HasIndex(e => e.TypeName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Enrolment>().HasIndex(e => new { e.ParticipantId, e.EventId }).IsUnique();
            modelBuilder.Entity<Result>().HasIndex(r => r.EnrolmentId).IsUnique();

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrolments)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Participant)
                .WithMany(u => u.Enrolments)
                .HasForeignKey(e => e.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}