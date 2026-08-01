using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OzelDersYonetim.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260801030000_AddInstagramUrlToSiteSettings")]
public partial class AddInstagramUrlToSiteSettings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "InstagramUrl",
            table: "SiteSettings",
            type: "TEXT",
            maxLength: 300,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "InstagramUrl",
            table: "SiteSettings");
    }
}
