using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Content;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Auditing;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles=IdentityDataSeeder.AdminRole)]
public class ClassContentsController(ApplicationDbContext db, IAuditService audit) : Controller
{
    private const string PageKey = "SecondaryEducation";

    public async Task<IActionResult> Index() => View(await db.ContentSections.AsNoTracking().Where(x=>x.PageKey==PageKey).OrderBy(x=>x.DisplayOrder).ToListAsync());
    public IActionResult Create() => View("Form",new ClassContentViewModel{DisplayOrder=10});

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassContentViewModel model)
    {
        if(!ModelState.IsValid)return View("Form",model);
        var item=new ContentSection{PageKey=PageKey,SectionKey=await UniqueKeyAsync(model.GradeLabel),Title=model.Title,Subtitle=model.GradeLabel,Content=NormalizeTopics(model.Topics),DisplayOrder=model.DisplayOrder,IsActive=model.IsActive};
        db.ContentSections.Add(item);await db.SaveChangesAsync();await audit.LogAsync("Sınıf içeriği ekleme","Sınıf içeriği",item.Id,item.Title);TempData["Success"]="Sınıf içeriği eklendi.";return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id){var item=await FindAsync(id);return item is null?NotFound():View("Form",ToModel(item));}

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,ClassContentViewModel model)
    {
        if(id!=model.Id)return BadRequest();if(!ModelState.IsValid)return View("Form",model);var item=await FindAsync(id);if(item is null)return NotFound();item.Title=model.Title;item.Subtitle=model.GradeLabel;item.Content=NormalizeTopics(model.Topics);item.DisplayOrder=model.DisplayOrder;item.IsActive=model.IsActive;item.UpdatedAt=DateTime.UtcNow;await db.SaveChangesAsync();await audit.LogAsync("Sınıf içeriği güncelleme","Sınıf içeriği",item.Id,item.Title);TempData["Success"]="Sınıf içeriği güncellendi.";return RedirectToAction(nameof(Index));
    }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id){var item=await FindAsync(id);if(item is null)return NotFound();item.IsActive=!item.IsActive;item.UpdatedAt=DateTime.UtcNow;await db.SaveChangesAsync();TempData["Success"]=item.IsActive?"Sınıf içeriği yayına alındı.":"Sınıf içeriği gizlendi.";return RedirectToAction(nameof(Index));}

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id){var item=await FindAsync(id);if(item is null)return NotFound();db.ContentSections.Remove(item);await db.SaveChangesAsync();await audit.LogAsync("Sınıf içeriği silme","Sınıf içeriği",id,item.Title);TempData["Success"]="Sınıf içeriği tamamen silindi.";return RedirectToAction(nameof(Index));}

    private Task<ContentSection?> FindAsync(int id)=>db.ContentSections.SingleOrDefaultAsync(x=>x.Id==id&&x.PageKey==PageKey);
    private static ClassContentViewModel ToModel(ContentSection x)=>new(){Id=x.Id,GradeLabel=x.Subtitle??x.Title,Title=x.Title,Topics=x.Content.Replace('|','\n'),DisplayOrder=x.DisplayOrder,IsActive=x.IsActive};
    private static string NormalizeTopics(string value)=>string.Join('|',value.Split(new[]{'\r','\n','|',','},StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Distinct(StringComparer.CurrentCultureIgnoreCase));
    private async Task<string> UniqueKeyAsync(string label){var baseKey="class-"+string.Concat(label.ToLowerInvariant().Select(c=>char.IsLetterOrDigit(c)?c:'-')).Trim('-');if(string.IsNullOrWhiteSpace(baseKey))baseKey="class";var key=baseKey;var n=2;while(await db.ContentSections.AnyAsync(x=>x.PageKey==PageKey&&x.SectionKey==key))key=$"{baseKey}-{n++}";return key;}
}
