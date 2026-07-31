using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace OzelDersYonetim.Data.Migrations;
[DbContext(typeof(ApplicationDbContext))]
[Migration("20260731174000_AddAuditLogs")]
public class AddAuditLogs : Migration
{
    protected override void Up(MigrationBuilder m){m.CreateTable(name:"AuditLogs",columns:t=>new{Id=t.Column<int>("INTEGER",nullable:false).Annotation("Sqlite:Autoincrement",true),ApplicationUserId=t.Column<string>("TEXT",maxLength:450,nullable:true),ActionType=t.Column<string>("TEXT",maxLength:100,nullable:false),EntityType=t.Column<string>("TEXT",maxLength:100,nullable:false),EntityId=t.Column<int>("INTEGER",nullable:true),Description=t.Column<string>("TEXT",maxLength:2000,nullable:false),IpAddress=t.Column<string>("TEXT",maxLength:80,nullable:true),CreatedAt=t.Column<DateTime>("TEXT",nullable:false)},constraints:t=>t.PrimaryKey("PK_AuditLogs",x=>x.Id));m.CreateIndex(name:"IX_AuditLogs_CreatedAt",table:"AuditLogs",column:"CreatedAt");}
    protected override void Down(MigrationBuilder m)=>m.DropTable("AuditLogs");
}
