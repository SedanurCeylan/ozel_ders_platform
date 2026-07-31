using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace OzelDersYonetim.Data.Migrations;
[DbContext(typeof(ApplicationDbContext))]
[Migration("20260801020000_AddStudentTestimonials")]
public partial class AddStudentTestimonials:Migration
{
 protected override void Up(MigrationBuilder m){m.CreateTable(name:"StudentTestimonials",columns:t=>new{Id=t.Column<int>("INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),StudentProfileId=t.Column<int>("INTEGER",nullable:false),Comment=t.Column<string>("TEXT",maxLength:1000,nullable:false),Rating=t.Column<int>("INTEGER",nullable:false),IsActive=t.Column<bool>("INTEGER",nullable:false),CreatedAt=t.Column<DateTime>("TEXT",nullable:false),UpdatedAt=t.Column<DateTime>("TEXT",nullable:false)},constraints:t=>{t.PrimaryKey("PK_StudentTestimonials",x=>x.Id);t.ForeignKey("FK_StudentTestimonials_StudentProfiles_StudentProfileId",x=>x.StudentProfileId,"StudentProfiles","Id",onDelete:ReferentialAction.Restrict);});m.CreateIndex("IX_StudentTestimonials_StudentProfileId","StudentTestimonials","StudentProfileId",unique:true);}
 protected override void Down(MigrationBuilder m)=>m.DropTable("StudentTestimonials");
}
