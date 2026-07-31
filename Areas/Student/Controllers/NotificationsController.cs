using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Identity;
namespace OzelDersYonetim.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles=IdentityDataSeeder.StudentRole)]
public class NotificationsController(ApplicationDbContext dbContext,UserManager<ApplicationUser> users):Controller
{
    public async Task<IActionResult> Index(){var uid=users.GetUserId(User);return View(await dbContext.UserNotifications.Where(x=>x.ApplicationUserId==uid).OrderByDescending(x=>x.CreatedAt).ToListAsync());}
    public async Task<IActionResult> Open(int id){var uid=users.GetUserId(User);var item=await dbContext.UserNotifications.SingleOrDefaultAsync(x=>x.Id==id&&x.ApplicationUserId==uid);if(item is null)return NotFound();item.IsRead=true;item.ReadAt=DateTime.UtcNow;await dbContext.SaveChangesAsync();return !string.IsNullOrWhiteSpace(item.TargetUrl)?LocalRedirect(item.TargetUrl):RedirectToAction(nameof(Index));}
    [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> MarkAllRead(){var uid=users.GetUserId(User);var items=await dbContext.UserNotifications.Where(x=>x.ApplicationUserId==uid&&!x.IsRead).ToListAsync();foreach(var item in items){item.IsRead=true;item.ReadAt=DateTime.UtcNow;}await dbContext.SaveChangesAsync();return RedirectToAction(nameof(Index));}
}
