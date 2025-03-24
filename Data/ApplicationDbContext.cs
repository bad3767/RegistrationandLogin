using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<JobApplications> JobApplications {get; set;}
        public DbSet<ResumeUpload> ResumeUpload {get; set;}
        public DbSet<JobStatus>JobStatus {get; set;}

    }
}
