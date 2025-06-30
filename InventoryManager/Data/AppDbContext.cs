using Microsoft.EntityFrameworkCore;
using InventoryManager.Models;

namespace InventoryManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //
        }
    }
}
