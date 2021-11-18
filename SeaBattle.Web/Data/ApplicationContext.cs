using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SeaBattle.Web.Models;

namespace SeaBattle.Web.Data
{
    public sealed class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        
            builder.Entity<SeaBattleCell>().HasKey("FieldId", "PosX", "PosY");
        
            builder.Entity<SeaBattleField>().HasKey(field => field.FieldId);
        
            builder.Entity<SeaBattleGame>().HasKey(game => game.GameId);
        
            builder.Entity<SeaBattlePlayer>().HasKey(player => player.PlayerId);
        
            builder.Entity<SeaBattleShip>().HasKey("FieldId", "ShipId", "PosX", "PosY", "Type");
        
            builder.Entity<SeaBattlePosition>().HasKey("FieldId", "PosX", "PosY", "Type");
        }
        
        public DbSet<SeaBattleGame> Games { get; set; }
        public DbSet<SeaBattlePlayer> Players { get; set; }
        public DbSet<SeaBattleField> Fields { get; set; }
        public DbSet<SeaBattleCell> Cells { get; set; }
        public DbSet<SeaBattleShip> Ships { get; set; }
        public DbSet<SeaBattlePosition> Positions { get; set; }
    }
}