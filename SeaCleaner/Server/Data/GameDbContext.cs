using Microsoft.EntityFrameworkCore;
using SeaCleaner.Server.Model;

namespace SeaCleaner.Server.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }
        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<GameResult> GameResults { get; set; }
    }
}
