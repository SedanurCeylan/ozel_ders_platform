using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OzelDersYonetim.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260801210000_RestoreHomeDemoAndGameCards")]
public partial class RestoreHomeDemoAndGameCards : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE ContentSections
            SET IsActive = 1, UpdatedAt = CURRENT_TIMESTAMP
            WHERE PageKey = 'Home' AND SectionKey IN ('section-demo', 'demo-featured');

            INSERT OR IGNORE INTO ContentSections
                (PageKey, SectionKey, Title, Subtitle, Content, DisplayOrder, IsActive, CreatedAt, UpdatedAt)
            VALUES
                ('Home', 'homeui-game-arena', 'İşlem Arenası', 'AKTİF OYUN', 'Süreye karşı işlemleri çöz, hızlı cevaplarla bonus puan kazan ve kendi rekorunu kır.', 23, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                ('Home', 'homeui-game-geometry', 'Geometri Kaşifi', 'YAKINDA', 'Şekilleri incele, ölçümleri hesapla ve görsel keşif görevlerini tamamla.', 24, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM ContentSections
            WHERE PageKey = 'Home' AND SectionKey IN ('homeui-game-arena', 'homeui-game-geometry');
            """);
    }
}
