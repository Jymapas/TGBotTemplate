using System;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
public class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

        modelBuilder.Entity("Domain.Entities.UserSession", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER");

            b.Property<string>("PayloadJson")
                .HasColumnType("TEXT");

            b.Property<string>("State")
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnType("TEXT");

            b.Property<DateTime>("UpdatedAt")
                .HasColumnType("TEXT");

            b.Property<long>("UserId")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.HasIndex("UserId")
                .IsUnique();

            b.ToTable("UserSessions", (string?)null);
        });
#pragma warning restore 612, 618
    }
}
