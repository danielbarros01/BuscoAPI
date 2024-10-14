using BuscoAPI.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace BuscoAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ProfessionCategory> Categories { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<WorkersProfessions> WorkersProfessions { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Qualification> WorkerQualification { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkersProfessions>()
                .ToTable("Workers_has_Professions")
                .HasKey(x => new { x.WorkerId, x.ProfessionId });

            modelBuilder.Entity<Worker>()
                .HasKey(x => new { x.UserId });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.UserSender)
                    .WithMany()
                    .HasForeignKey(n => n.UserSenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

