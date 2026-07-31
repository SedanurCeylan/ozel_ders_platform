using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OzelDersYonetim.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260801010000_AddDailyFacts")]
public partial class AddDailyFacts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DailyFacts",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                Content = table.Column<string>(type: "TEXT", maxLength: 1200, nullable: false),
                Category = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_DailyFacts", x => x.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "DailyFacts");
}
