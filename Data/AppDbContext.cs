using JobManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace JobManagement.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Department> Departments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Location)
                .WithMany(l => l.Jobs)
                .HasForeignKey(j => j.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Department)
                .WithMany(d => d.Jobs)
                .HasForeignKey(j => j.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
