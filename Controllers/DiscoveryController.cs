using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Documents;
using OzelDersYonetim.Models.ViewModels;

namespace OzelDersYonetim.Controllers;

public class DiscoveryController(ApplicationDbContext db,IWebHostEnvironment environment):Controller
{
    [Route("demo-dersler")]
    public async Task<IActionResult> DemoLessons()
    {
        if (!await SectionIsActive("section-demo")) return NotFound();
        return View("DemoLessons",await Collection("demo-","Demo Dersler","Öğretmenin anlatım tarzını ve soru çözme yaklaşımını ücretsiz demo derslerle keşfedin."));
    }

    [Route("matematik-icerikleri")]
    public async Task<IActionResult> Content()
    {
        if (!await SectionIsActive("section-content")) return NotFound();
        return View("Content",await Collection("content-","Matematik İçerikleri","Matematik ipuçları, kısa konu anlatımları ve çalışma önerileri."));
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
        var path=Path.Combine(environment.ContentRootPath,"App_Data","uploads","documents",item.StoredFilePath);if(!System.IO.File.Exists(path))return NotFound();return File(System.IO.File.OpenRead(path),item.ContentType,item.OriginalFileName);
    }

    private async Task<PublicCollectionViewModel> Collection(string prefix,string title,string description)=>new(){Title=title,Description=description,Items=await db.ContentSections.AsNoTracking().Where(x=>x.PageKey=="Home"&&x.IsActive&&x.SectionKey.StartsWith(prefix)).OrderBy(x=>x.DisplayOrder).ToListAsync()};
    private Task<bool> SectionIsActive(string key)=>db.ContentSections.AsNoTracking().AnyAsync(x=>x.PageKey=="Home"&&x.SectionKey==key&&x.IsActive);
}
