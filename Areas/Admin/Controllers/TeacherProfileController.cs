using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Content;
using OzelDersYonetim.Models.ViewModels;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class TeacherProfileController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var settings = await db.SiteSettings.AsNoTracking().SingleAsync();
        var sections = await db.ContentSections.AsNoTracking().Where(x => x.PageKey == "About").ToDictionaryAsync(x => x.SectionKey);
        return View(new TeacherProfileViewModel
        {
            TeacherName = settings.TeacherName,
            ProfessionalTitle = Value(sections, "teacher-title", x => x.Content, "Matematik Öğretmeni"),
            Education = Value(sections, "teacher-education", x => x.Content, "Mezuniyet ve eğitim bilgilerinizi buraya ekleyin."),
            Experience = Value(sections, "teacher-experience", x => x.Content, "Deneyim sürenizi ve uzman olduğunuz sınıf düzeylerini belirtin."),
            LessonFormat = Value(sections, "teacher-format", x => x.Content, "Çevrim içi ve yüz yüze birebir matematik dersleri"),
            AboutTitle = settings.AboutTitle,
            AboutDescription = settings.AboutDescription,
            Quote = Value(sections, "quote", x => x.Content, "Her öğrenci doğru yöntem ve düzenli çalışmayla matematikte ilerleyebilir."),
            ApproachTitle = Value(sections, "approach", x => x.Title, "Öğrencinin sesini duyan ders deneyimi"),
            ApproachSubtitle = Value(sections, "approach", x => x.Subtitle, "Eğitim yaklaşımım"),
            ApproachContent = Value(sections, "approach", x => x.Content, "Her öğrencinin ihtiyacına göre anlaşılır ve planlı bir öğrenme süreci oluştururum."),
            FirstValueTitle = Value(sections, "patience", x => x.Title, "Sabır"),
            FirstValueContent = Value(sections, "patience", x => x.Content, "Her öğrencinin öğrenme hızına saygı duyarım."),
            SecondValueTitle = Value(sections, "clarity", x => x.Title, "Netlik"),
            SecondValueContent = Value(sections, "clarity", x => x.Content, "Karmaşık konuları anlaşılır adımlara bölerim."),
            ThirdValueTitle = Value(sections, "continuity", x => x.Title, "Süreklilik"),
            ThirdValueContent = Value(sections, "continuity", x => x.Content, "Gelişimi düzenli takip ve geri bildirimle desteklerim.")
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(TeacherProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var settings = await db.SiteSettings.SingleAsync();
        settings.TeacherName = model.TeacherName.Trim();
        settings.AboutTitle = model.AboutTitle.Trim();
        settings.AboutDescription = model.AboutDescription.Trim();
        settings.UpdatedAt = DateTime.UtcNow;

        await SaveSection("quote", "Eğitim anlayışı", null, model.Quote, 1);
        await SaveSection("teacher-title", "Mesleki unvan", null, model.ProfessionalTitle, 2);
        await SaveSection("teacher-education", "Eğitim", null, model.Education, 3);
        await SaveSection("teacher-experience", "Deneyim ve uzmanlık", null, model.Experience, 4);
        await SaveSection("teacher-format", "Ders biçimi", null, model.LessonFormat, 5);
        await SaveSection("approach", model.ApproachTitle, model.ApproachSubtitle, model.ApproachContent, 6);
        await SaveSection("patience", model.FirstValueTitle, null, model.FirstValueContent, 7);
        await SaveSection("clarity", model.SecondValueTitle, null, model.SecondValueContent, 8);
        await SaveSection("continuity", model.ThirdValueTitle, null, model.ThirdValueContent, 9);
        await db.SaveChangesAsync();

        TempData["Success"] = "Öğretmen ve Hakkımda bilgileri güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task SaveSection(string key, string title, string? subtitle, string content, int order)
    {
        var section = await db.ContentSections.SingleOrDefaultAsync(x => x.PageKey == "About" && x.SectionKey == key);
        if (section is null)
        {
            section = new ContentSection { PageKey = "About", SectionKey = key, CreatedAt = DateTime.UtcNow };
            db.ContentSections.Add(section);
        }
        section.Title = title.Trim();
        section.Subtitle = string.IsNullOrWhiteSpace(subtitle) ? null : subtitle.Trim();
        section.Content = content.Trim();
        section.DisplayOrder = order;
        section.IsActive = true;
        section.UpdatedAt = DateTime.UtcNow;
    }

    private static string Value(IReadOnlyDictionary<string, ContentSection> sections, string key, Func<ContentSection, string?> selector, string fallback)
        => sections.TryGetValue(key, out var section) && !string.IsNullOrWhiteSpace(selector(section)) ? selector(section)! : fallback;
}
