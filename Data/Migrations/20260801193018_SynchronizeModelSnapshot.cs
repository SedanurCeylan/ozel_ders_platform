using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzelDersYonetim.Data.Migrations;

/// <summary>
/// Synchronizes the EF model snapshot with the schema already created by the
/// hand-authored migrations that precede this migration. No database operation
/// is required because those migrations are already part of the migration chain.
/// </summary>
public partial class SynchronizeModelSnapshot : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
