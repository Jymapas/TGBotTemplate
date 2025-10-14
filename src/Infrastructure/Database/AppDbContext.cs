using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext : DbContext
{
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var session = modelBuilder.Entity<UserSession>();

        session.ToTable("UserSessions");
        session.HasKey(x => x.Id);

        session.HasIndex(x => x.UserId).IsUnique();

        session.Property(x => x.State)
            .IsRequired()
            .HasMaxLength(64);

        session.Property(x => x.PayloadJson)
            .HasColumnType("TEXT");

        session.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}
