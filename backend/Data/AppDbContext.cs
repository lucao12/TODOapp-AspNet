using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Teste.Models;

namespace Teste.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
    }
}
