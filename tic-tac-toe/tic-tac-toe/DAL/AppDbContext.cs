using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<SavedGame> SavedGames { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
         
    }
}