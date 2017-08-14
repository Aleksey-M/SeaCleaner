using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SeaCleaner.Domain;

namespace SeaCleaner.Persistance
{
    public class GameDataContext : IdentityDbContext<Player>
    {
        public DbSet<GameResults> GameResults { get; set; }
        public GameDataContext(DbContextOptions<GameDataContext> options) : base(options){}
    }
}
