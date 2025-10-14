using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserSessions",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                UserId = table.Column<long>(type: "INTEGER", nullable: false),
                State = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                PayloadJson = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserSessions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserSessions_UserId",
            table: "UserSessions",
            column: "UserId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserSessions");
    }
}
