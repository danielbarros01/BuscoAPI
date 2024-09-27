using BuscoAPI.DTOS.Worker;
using BuscoAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<ProfessionCategory> Categories { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<WorkersProfessions> WorkersProfessions { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Qualification> WorkerQualifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //La tabla en BD es 
            //La llave compuesta
            modelBuilder.Entity<WorkersProfessions>()
                .ToTable("Workers_has_Professions")
                .HasKey(x => new {x.WorkerId, x.ProfessionId});

            //La llave es
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
