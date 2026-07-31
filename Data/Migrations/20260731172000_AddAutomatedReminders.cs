using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace OzelDersYonetim.Data.Migrations;
[DbContext(typeof(ApplicationDbContext))]
[Migration("20260731172000_AddAutomatedReminders")]
public class AddAutomatedReminders : Migration
{
    protected override void Up(MigrationBuilder m)
    {
        m.AddColumn<bool>(name:"SendEmailNotification",table:"Lessons",type:"INTEGER",nullable:false,defaultValue:false);
        m.CreateTable(name:"ReminderDispatches",columns:t=>new{Id=t.Column<int>("INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),ReminderType=t.Column<string>("TEXT",maxLength:80,nullable:false),EntityType=t.Column<string>("TEXT",maxLength:80,nullable:false),EntityId=t.Column<int>("INTEGER",nullable:false),ApplicationUserId=t.Column<string>("TEXT",nullable:false),CreatedAt=t.Column<DateTime>("TEXT",nullable:false)},constraints:t=>t.PrimaryKey("PK_ReminderDispatches",x=>x.Id));
        m.CreateIndex(name:"IX_ReminderDispatches_ReminderType_EntityType_EntityId_ApplicationUserId",table:"ReminderDispatches",columns:new[]{"ReminderType","EntityType","EntityId","ApplicationUserId"},unique:true);
    }
    protected override void Down(MigrationBuilder m){m.DropTable("ReminderDispatches");m.DropColumn(name:"SendEmailNotification",table:"Lessons");}
}
