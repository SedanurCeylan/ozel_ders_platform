using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Content;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class ContentSectionsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(string? page, string? contentType, string? category)
    {
        if (category == "testimonials") return RedirectToAction("Index", "Testimonials");
        var query = dbContext.ContentSections.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(page))
        {
            query = query.Where(section => section.PageKey == page);
        }

        if (page == "Home" && !string.IsNullOrWhiteSpace(category))
        {
            var prefix = CategoryPrefix(category);
            if (prefix is null) return BadRequest();
            query = query.Where(section => section.SectionKey.StartsWith(prefix));
            ViewBag.Category = category;
            ViewBag.CategoryName = CategoryName(category);
            ViewBag.CategorySection = await dbContext.ContentSections.AsNoTracking()
                .SingleOrDefaultAsync(section => section.PageKey == "Home" && section.SectionKey == CategorySectionKey(category));
        }
        else if (page == "Home")
        {
            contentType = contentType == "items" ? "items" : "sections";
            query = contentType == "items"
                ? query.Where(section => !section.SectionKey.StartsWith("section-") && section.SectionKey != "cta" && section.SectionKey != "contact")
                : query.Where(section => section.SectionKey.StartsWith("section-") || section.SectionKey == "cta" || section.SectionKey == "contact");
        }

        ViewBag.Page = page;
        ViewBag.ContentType = contentType;
        return View(await query.OrderBy(section => section.PageKey).ThenBy(section => section.DisplayOrder).ToListAsync());
    }

    public IActionResult Create(string? category)
    {
        var prefix = CategoryPrefix(category);
        ViewBag.Category = category;
        return View(new ContentSection { PageKey = prefix is null ? "Home" : "Home", SectionKey = prefix ?? string.Empty });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContentSection model)
    {
        if (await dbContext.ContentSections.AnyAsync(section => section.PageKey == model.PageKey && section.SectionKey == model.SectionKey))
        {
            ModelState.AddModelError(nameof(model.SectionKey), "Bu sayfada aynı bölüm anahtarı zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.CreatedAt = model.UpdatedAt = DateTime.UtcNow;
        dbContext.ContentSections.Add(model);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "İçerik bölümü oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var section = await dbContext.ContentSections.FindAsync(id);
        return section is null ? NotFound() : View(section);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContentSection model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (await dbContext.ContentSections.AnyAsync(section => section.Id != id && section.PageKey == model.PageKey && section.SectionKey == model.SectionKey))
        {
            ModelState.AddModelError(nameof(model.SectionKey), "Bu sayfada aynı bölüm anahtarı zaten kullanılıyor.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.UpdatedAt = DateTime.UtcNow;
        dbContext.Update(model);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "İçerik bölümü güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id, string? page, string? contentType, string? category)
    {
        var section = await dbContext.ContentSections.FindAsync(id);
        if (section is null) return NotFound();
        section.IsActive = !section.IsActive;
        section.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        TempData["Success"] = section.IsActive ? "Bölüm ana sayfada yayınlandı." : "Bölüm ana sayfada gizlendi.";
        return RedirectToAction(nameof(Index), new { page, contentType, category });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var section = await dbContext.ContentSections.FindAsync(id);
        if (section is null)
        {
            return NotFound();
        }

        dbContext.ContentSections.Remove(section);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "İçerik bölümü silindi.";
        return RedirectToAction(nameof(Index));
    }

    private static string? CategoryPrefix(string? category) => category switch
    {
        "demo" => "demo-", "articles" => "content-", "faq" => "faq-", "programs" => "program-",
        "process" => "process-", "tracking" => "tracking-", "statistics" => "stat-", "testimonials" => "testimonial-", _ => null
    };

    private static string CategoryName(string category) => category switch
    {
        "demo" => "Demo Dersler", "articles" => "Matematik İçerikleri", "faq" => "Sık Sorulan Sorular", "programs" => "Eğitim Programları",
        "process" => "Ders Süreci", "tracking" => "Takip Sistemi Özellikleri", "statistics" => "İstatistikler", "testimonials" => "Öğrenci ve Veli Yorumları", _ => "İçerikler"
    };

    private static string CategorySectionKey(string category) => category switch
    {
        "demo" => "section-demo", "articles" => "section-content", "faq" => "section-faq", "programs" => "section-programs",
        "process" => "section-process", "tracking" => "section-tracking", "statistics" => "section-stats", "testimonials" => "section-testimonials", _ => string.Empty
    };
}
