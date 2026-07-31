using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzelDersYonetim.Data.Migrations;

public partial class AddLessonsAndAttendance : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Lessons",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Title = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                Subject = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                LessonMode = table.Column<int>(type: "INTEGER", nullable: false),
                OnlineMeetingUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                Status = table.Column<int>(type: "INTEGER", nullable: false),
                TeacherNote = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                NextLessonTopic = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Lessons", x => x.Id));

        migrationBuilder.CreateTable(
            name: "StudentLessons",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                LessonId = table.Column<int>(type: "INTEGER", nullable: false),
                StudentProfileId = table.Column<int>(type: "INTEGER", nullable: false),
                AttendanceStatus = table.Column<int>(type: "INTEGER", nullable: false),
                PerformanceNote = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                QuestionCount = table.Column<int>(type: "INTEGER", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StudentLessons", x => x.Id);
                table.ForeignKey("FK_StudentLessons_Lessons_LessonId", x => x.LessonId, "Lessons", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_StudentLessons_StudentProfiles_StudentProfileId", x => x.StudentProfileId, "StudentProfiles", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_StudentLessons_StudentProfileId", "StudentLessons", "StudentProfileId");
        migrationBuilder.CreateIndex("IX_StudentLessons_LessonId_StudentProfileId", "StudentLessons", new[] { "LessonId", "StudentProfileId" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "StudentLessons");
        migrationBuilder.DropTable(name: "Lessons");
    }
}
