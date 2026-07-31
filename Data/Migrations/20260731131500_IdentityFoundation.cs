using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OzelDersYonetim.Data;

namespace OzelDersYonetim.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260731131500_IdentityFoundation")]
public partial class IdentityFoundation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: false,
            defaultValueSql: "CURRENT_TIMESTAMP");

        migrationBuilder.AddColumn<string>(
            name: "FirstName",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsActive",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<string>(
            name: "LastName",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "MustChangePassword",
            table: "AspNetUsers",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "AspNetUsers",
            type: "TEXT",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "CreatedAt", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "FirstName", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "IsActive", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "LastName", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "MustChangePassword", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "UpdatedAt", table: "AspNetUsers");
    }
}
