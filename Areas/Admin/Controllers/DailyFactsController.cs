using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Content;
namespace OzelDersYonetim.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles=IdentityDataSeeder.AdminRole)]
public class DailyFactsController(ApplicationDbContext db):Controller
{
 public async Task<IActionResult> Index()=>View(await db.DailyFacts.AsNoTracking().OrderBy(x=>x.DisplayOrder).ThenBy(x=>x.Id).ToListAsync());
 public IActionResult Create()=>View(new DailyFact{DisplayOrder=1});
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Create(DailyFact model){if(!ModelState.IsValid)return View(model);model.Title=model.Title.Trim();model.Content=model.Content.Trim();model.Category=model.Category.Trim();model.CreatedAt=model.UpdatedAt=DateTime.UtcNow;db.DailyFacts.Add(model);await db.SaveChangesAsync();TempData["Success"]="Günün bilgisi havuzuna eklendi.";return RedirectToAction(nameof(Index));}
 public async Task<IActionResult>Edit(int id){var item=await db.DailyFacts.FindAsync(id);return item is null?NotFound():View(item);}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Edit(int id,DailyFact model){if(id!=model.Id)return BadRequest();if(!ModelState.IsValid)return View(model);var item=await db.DailyFacts.FindAsync(id);if(item is null)return NotFound();item.Title=model.Title.Trim();item.Content=model.Content.Trim();item.Category=model.Category.Trim();item.DisplayOrder=model.DisplayOrder;item.IsActive=model.IsActive;item.UpdatedAt=DateTime.UtcNow;await db.SaveChangesAsync();TempData["Success"]="Günün bilgisi güncellendi.";return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Toggle(int id){var item=await db.DailyFacts.FindAsync(id);if(item is null)return NotFound();item.IsActive=!item.IsActive;item.UpdatedAt=DateTime.UtcNow;await db.SaveChangesAsync();TempData["Success"]=item.IsActive?"Bilgi yayın sırasına alındı.":"Bilgi yayından kaldırıldı.";return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Delete(int id){var item=await db.DailyFacts.FindAsync(id);if(item is null)return NotFound();db.DailyFacts.Remove(item);await db.SaveChangesAsync();TempData["Success"]="Bilgi silindi.";return RedirectToAction(nameof(Index));}
}
