using DevTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DevTask.DataAccess
{
    public class DevTaskContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<GitHubRepository> GitHubRepositories { get; set; }

        public DevTaskContext(DbContextOptions<DevTaskContext> options) : base(options)
        { 

        }
    }
}
