using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Polling.Model;

namespace Polling.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Choices> Choices { get; set; }
        public DbSet<Vote> Votes { get; set; }

    }
}
