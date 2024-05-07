using BuscoAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscoAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options){}


        public DbSet<User> Users { get; set; }
    }
}
