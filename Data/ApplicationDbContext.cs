using Microsoft.EntityFrameworkCore;
using TradeWave.Models;

namespace TradeWave.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Veritabanı tablolarını temsil eden DbSet'ler
        public DbSet<User> User { get; set; }
        public DbSet<Watchlistuser> Watchlistuser { get; set; }

    }
}
