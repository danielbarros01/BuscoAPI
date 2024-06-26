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

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.User)  // Worker tiene una relación con User
                .WithOne(u => u.Worker)  // User tiene una relación con Worker
                .HasForeignKey<Worker>(w => w.UserId)  // Clave externa en Worker
                .IsRequired();  // Es requerido, si es el caso


            base.OnModelCreating(modelBuilder);
        }
    }
}
