using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Content;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services;

namespace OzelDersYonetim.Controllers;

public class DiscoveryController(ApplicationDbContext db,StoragePathResolver storage):Controller
{
    [Route("demo-dersler")]
    public async Task<IActionResult> DemoLessons()
    {
        return View("DemoLessons",await Collection("demo-","Demo Dersler","Öğretmenin anlatım tarzını ve soru çözme yaklaşımını ücretsiz demo derslerle keşfedin."));
    }

    [Route("matematik-icerikleri")]
    public async Task<IActionResult> Content()
    {
        if (!await SectionIsActive("section-content")) return NotFound();
        return View("Content",await Collection("content-","Matematik İçerikleri","Matematik ipuçları, kısa konu anlatımları ve çalışma önerileri."));
    }

    [Route("matematik-icerikleri/{key}")]
    public async Task<IActionResult> Article(string key)
    {
        var article = await db.ContentSections.AsNoTracking().SingleOrDefaultAsync(x => x.PageKey == "Home" && x.SectionKey == key && x.SectionKey.StartsWith("content-") && x.IsActive);
        if (article is null) return NotFound();
        var related = await db.ContentSections.AsNoTracking().Where(x => x.PageKey == "Home" && x.SectionKey.StartsWith("content-") && x.SectionKey != key && x.IsActive).OrderBy(x => x.DisplayOrder).Take(3).ToListAsync();
        return View(new PublicArticleViewModel { Article = article, Sections = ArticleSections(article), RelatedArticles = related });
    }

    [Route("ucretsiz-dokumanlar")]
    public async Task<IActionResult> Documents()
    {
        var model=new PublicCollectionViewModel{Title="Ücretsiz Dokümanlar",Description="Herkese açık çalışma dosyalarını ücretsiz görüntüleyin ve indirin.",Documents=await db.CourseDocuments.AsNoTracking().Where(x=>x.IsActive&&x.AccessType==DocumentAccessType.Public).OrderByDescending(x=>x.CreatedAt).ToListAsync()};
        return View(model);
    }

    [Route("ucretsiz-dokumanlar/indir/{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        var item=await db.CourseDocuments.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id&&x.IsActive&&x.AccessType==DocumentAccessType.Public);if(item is null)return NotFound();
        var path=storage.ResolveStoredFile(item.StoredFilePath,"documents");if(path is null||!System.IO.File.Exists(path))return NotFound();return File(System.IO.File.OpenRead(path),item.ContentType,item.OriginalFileName);
    }

    private async Task<PublicCollectionViewModel> Collection(string prefix,string title,string description)=>new(){Title=title,Description=description,Items=await db.ContentSections.AsNoTracking().Where(x=>x.PageKey=="Home"&&x.IsActive&&x.SectionKey.StartsWith(prefix)).OrderBy(x=>x.DisplayOrder).ToListAsync()};
    private Task<bool> SectionIsActive(string key)=>db.ContentSections.AsNoTracking().AnyAsync(x=>x.PageKey=="Home"&&x.SectionKey==key&&x.IsActive);

    private static IReadOnlyList<(string Heading, string Body)> ArticleSections(ContentSection article) => article.SectionKey switch
    {
        "content-fractions" => new[] { ("Önce bütünü gör", "Kesirlerde en önemli adım, parçaların hangi bütüne ait olduğunu anlamaktır. Bir pizzayı sekiz eş parçaya ayırdığımızda her parça 1/8 olur. Üç parça seçersek 3/8 elde ederiz. Pay seçilen parça sayısını, payda ise bütünün kaç eş parçaya ayrıldığını gösterir."), ("Şekil çizerek karşılaştır", "Paydaları farklı iki kesri karşılaştırırken hemen işlem yapmak yerine aynı büyüklükte iki dikdörtgen çiz. Dikdörtgenleri paydalar kadar eş parçaya ayır ve pay kadarını boya. Görsel, hangi kesrin daha büyük olduğunu çoğu zaman işlemsiz gösterir."), ("Günlük hayatla pekiştir", "Tarif ölçüleri, saat dilimleri ve para paylaşımı kesir çalışmak için iyi örneklerdir. Bir tarifte yarım bardak ile çeyrek bardağın toplamını düşünmek, sembollerin gerçek miktarları temsil ettiğini fark ettirir.") },
        "content-time" => new[] { ("İlk turda ritmini bul", "Sınav başladığında bütün sorulara aynı süreyi ayırmaya çalışma. İlk turda çözüm yolunu hemen gördüğün soruları tamamla. Uzun süreceğini düşündüğün soruların yanına küçük bir işaret koyarak ikinci tura bırak."), ("Bir soruda takılı kalma", "Yaklaşık iki dakika boyunca ilerleyemediğin soruyu geçmek başarısızlık değildir; süreyi yönetmektir. Diğer soruları tamamladıktan sonra geri döndüğünde zihnin farklı bir çözüm yolu görebilir."), ("Kontrol süresi bırak", "Son beş ila on dakikayı kodlama, işlem işaretleri ve soru kökleri için ayır. Özellikle 'değildir', 'en az' ve 'en çok' gibi ifadeleri yeniden kontrol etmek küçük dikkatsizlikleri azaltır.") },
        "content-mistakes" => new[] { ("Soru kökünü erken okumak", "Öğrenciler bazen metindeki sayıları görür görmez işleme başlar. Önce sorunun ne istediğini tek cümleyle ifade etmek, gereksiz işlem yapmayı ve yanlış sonuca gitmeyi önler."), ("İşlem işaretlerini karıştırmak", "Eksi işareti, parantez ve bölme çizgisi küçük görünse de sonucu tamamen değiştirir. Her işlem satırında yalnızca bir dönüşüm yapmak ve işaretleri belirgin yazmak hata olasılığını azaltır."), ("Cevabı kontrol etmemek", "Bulduğun sonucu sorudaki koşullara geri yerleştir. Sonuç günlük hayata uygun mu, yaklaşık değerle tutarlı mı ve sorulan birimle yazılmış mı? Bu üç kısa kontrol güçlü bir alışkanlığa dönüşür.") },
        _ => new[] { ("Konuyu küçük adımlara ayır", article.Content + " Konuyu tek seferde bitirmeye çalışmak yerine temel kavramı öğren, örneği incele ve ardından benzer bir soruyu kendi başına çöz."), ("Aktif çalış", "Sadece okumak yerine önemli bilgileri kendi cümlelerinle özetle. Çözüm sırasında neden o işlemi seçtiğini sesli anlatmak, ezber yerine anlamayı güçlendirir."), ("Kısa tekrar yap", "Çalışmadan bir gün sonra beş dakikalık tekrar yap ve zorlandığın bir soruyu yeniden çöz. Düzenli kısa tekrarlar, uzun fakat seyrek çalışmalardan daha kalıcı olabilir.") }
    };
}
