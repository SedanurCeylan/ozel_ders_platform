using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzelDersYonetim.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationUserId = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GradeLevel = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    SchoolName = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    ParentFirstName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    ParentLastName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    ParentPhone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    ParentEmail = table.Column<string>(type: "TEXT", maxLength: 180, nullable: true),
                    LessonType = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    LessonPreference = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    TeacherNote = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ProfileImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_ApplicationUserId",
                table: "StudentProfiles",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_Email",
                table: "StudentProfiles",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentProfiles");
        }
    }
}
