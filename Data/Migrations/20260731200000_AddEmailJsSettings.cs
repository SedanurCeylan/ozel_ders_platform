using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OzelDersYonetim.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260731200000_AddEmailJsSettings")]
public partial class AddEmailJsSettings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name:"EmailJsPublicKey",table:"SiteSettings",type:"TEXT",maxLength:180,nullable:true);
        migrationBuilder.AddColumn<string>(name:"EmailJsServiceId",table:"SiteSettings",type:"TEXT",maxLength:120,nullable:true);
        migrationBuilder.AddColumn<string>(name:"EmailJsTemplateId",table:"SiteSettings",type:"TEXT",maxLength:120,nullable:true);
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name:"EmailJsPublicKey",table:"SiteSettings");
        migrationBuilder.DropColumn(name:"EmailJsServiceId",table:"SiteSettings");
        migrationBuilder.DropColumn(name:"EmailJsTemplateId",table:"SiteSettings");
    }
}
