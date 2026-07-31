using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Services.Auditing;
using OzelDersYonetim.Services.Notifications;
namespace OzelDersYonetim.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles=IdentityDataSeeder.AdminRole)]
public class EmailLogsController(ApplicationDbContext dbContext,IEmailService emailService,IAuditService audit):Controller
{
    public async Task<IActionResult> Index(bool? successful,string? search){var q=dbContext.EmailLogs.AsNoTracking().OrderByDescending(x=>x.CreatedAt).AsQueryable();if(successful.HasValue)q=q.Where(x=>x.IsSuccessful==successful);if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.RecipientEmail.Contains(search)||x.Subject.Contains(search));ViewBag.Successful=successful;ViewBag.Search=search;return View(await q.Take(300).ToListAsync());}
    public async Task<IActionResult> Details(int id){var item=await dbContext.EmailLogs.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id);return item is null?NotFound():View(item);}
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Retry(int id)
    {
        var item=await dbContext.EmailLogs.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id);if(item is null)return NotFound();if(item.IsSuccessful){TempData["Success"]="Başarılı e-postanın yeniden gönderilmesine gerek yok.";return RedirectToAction(nameof(Details),new{id});}
        if(item.EmailType is "Yeni öğrenci hesabı" or "Şifre sıfırlama"){TempData["Success"]="Güvenlik nedeniyle geçici şifre e-postası geçmişten yeniden gönderilemez. Öğrenci sayfasından yeni bir şifre sıfırlama işlemi başlatın.";return RedirectToAction(nameof(Details),new{id});}
        var success=await emailService.SendEmailAsync(item.RecipientEmail,item.RecipientName??item.RecipientEmail,item.Subject,item.Body,item.EmailType+" · Yeniden deneme",item.RelatedEntityId);await audit.LogAsync("E-posta yeniden gönderme","E-posta",item.Id,$"{item.RecipientEmail} · {(success?"Başarılı":"Başarısız")}");TempData["Success"]=success?"E-posta yeniden gönderildi.":"E-posta tekrar gönderilemedi. Yeni hata kaydını inceleyin.";return RedirectToAction(nameof(Index),new{successful=false});
    }
}
