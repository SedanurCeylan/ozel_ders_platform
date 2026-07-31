using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
namespace OzelDersYonetim.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles=IdentityDataSeeder.AdminRole)]
public class TestimonialsController(ApplicationDbContext db):Controller
{
 public async Task<IActionResult> Index()=>View(await db.StudentTestimonials.AsNoTracking().Include(x=>x.StudentProfile).OrderByDescending(x=>x.UpdatedAt).ToListAsync());
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Toggle(int id){var item=await db.StudentTestimonials.FindAsync(id);if(item is null)return NotFound();item.IsActive=!item.IsActive;item.UpdatedAt=DateTime.UtcNow;if(item.IsActive){var section=await db.ContentSections.SingleOrDefaultAsync(x=>x.PageKey=="Home"&&x.SectionKey=="section-testimonials");if(section is not null){section.IsActive=true;section.UpdatedAt=DateTime.UtcNow;}}await db.SaveChangesAsync();TempData["Success"]=item.IsActive?"Öğrenci yorumu yayınlandı.":"Öğrenci yorumu pasif yapıldı.";return RedirectToAction(nameof(Index));}
}
