using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzelDersYonetim.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260731162000_AddDocumentsAndDownloadTracking")]
public class AddDocumentsAndDownloadTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "CourseDocuments", columns: table => new
        {
            Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
            Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
            Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
            Category = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
            AccessType = table.Column<int>(type: "INTEGER", nullable: false),
            OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
            StoredFilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
            ContentType = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
            FileSize = table.Column<long>(type: "INTEGER", nullable: false),
            IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_CourseDocuments", x => x.Id));

        migrationBuilder.CreateTable(name: "StudentDocuments", columns: table => new
        {
            Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
            StudentProfileId = table.Column<int>(type: "INTEGER", nullable: false),
            CourseDocumentId = table.Column<int>(type: "INTEGER", nullable: false),
            AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
            IsViewed = table.Column<bool>(type: "INTEGER", nullable: false),
            ViewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
            IsDownloaded = table.Column<bool>(type: "INTEGER", nullable: false),
            DownloadedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_StudentDocuments", x => x.Id);
            table.ForeignKey("FK_StudentDocuments_CourseDocuments_CourseDocumentId", x => x.CourseDocumentId, "CourseDocuments", "Id", onDelete: ReferentialAction.Restrict);
            table.ForeignKey("FK_StudentDocuments_StudentProfiles_StudentProfileId", x => x.StudentProfileId, "StudentProfiles", "Id", onDelete: ReferentialAction.Restrict);
        });
        migrationBuilder.CreateIndex("IX_StudentDocuments_CourseDocumentId_StudentProfileId", "StudentDocuments", new[] { "CourseDocumentId", "StudentProfileId" }, unique: true);
        migrationBuilder.CreateIndex("IX_StudentDocuments_StudentProfileId", "StudentDocuments", "StudentProfileId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) { migrationBuilder.DropTable("StudentDocuments"); migrationBuilder.DropTable("CourseDocuments"); }
}
