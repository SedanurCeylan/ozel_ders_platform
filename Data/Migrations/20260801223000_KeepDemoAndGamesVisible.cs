using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzelDersYonetim.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260801223000_KeepDemoAndGamesVisible")]
public partial class KeepDemoAndGamesVisible : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE ContentSections
            SET IsActive = 1, UpdatedAt = CURRENT_TIMESTAMP
            WHERE PageKey = 'Home'
              AND SectionKey IN (
                  'section-demo', 'demo-featured', 'section-games',
                  'homeui-game-arena', 'homeui-game-geometry'
              );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
