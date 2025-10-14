using Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        var env = EnvLoader.Load(envPath);
        var dbPath = env.GetOptional("DB_PATH", "data/bot.db")!;

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new AppDbContext(optionsBuilder.Options);
    }
}
