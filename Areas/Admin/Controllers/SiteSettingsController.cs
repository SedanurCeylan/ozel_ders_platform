using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Content;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class SiteSettingsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var setting = await dbContext.SiteSettings.SingleAsync();
        return View(setting);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SiteSetting model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var setting = await dbContext.SiteSettings.SingleAsync();
        setting.SiteName = model.SiteName;
        setting.TeacherName = model.TeacherName;
        setting.Email = model.Email?.Trim();
        setting.Phone = model.Phone?.Trim();
        setting.InstagramUrl = model.InstagramUrl?.Trim();
        setting.EmailJsServiceId = model.EmailJsServiceId;
        setting.EmailJsTemplateId = model.EmailJsTemplateId;
        setting.EmailJsPublicKey = model.EmailJsPublicKey;
        setting.HeroTitle = model.HeroTitle;
        setting.HeroDescription = model.HeroDescription;
        setting.AboutTitle = model.AboutTitle;
        setting.AboutDescription = model.AboutDescription;
        setting.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "Site ayarları başarıyla kaydedildi.";
        return RedirectToAction(nameof(Index));
    }
}
