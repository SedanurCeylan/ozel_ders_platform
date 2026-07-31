using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Identity;
namespace OzelDersYonetim.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles=IdentityDataSeeder.StudentRole)]
public class AnnouncementsController(ApplicationDbContext dbContext,UserManager<ApplicationUser> users,OzelDersYonetim.Services.Notifications.AnnouncementFileService files):Controller
{
    public async Task<IActionResult> Index(){var uid=users.GetUserId(User);var sid=await dbContext.StudentProfiles.Where(x=>x.ApplicationUserId==uid).Select(x=>(int?)x.Id).SingleOrDefaultAsync();if(!sid.HasValue)return NotFound();var now=DateTime.Now;return View(await dbContext.Announcements.AsNoTracking().Where(x=>x.IsActive&&x.PublishDate<=now&&(!x.ExpiryDate.HasValue||x.ExpiryDate>=now)&&x.AnnouncementStudents.Any(s=>s.StudentProfileId==sid)).OrderByDescending(x=>x.PublishDate).ToListAsync());}
    public async Task<IActionResult> Details(int id){var uid=users.GetUserId(User);var link=await dbContext.AnnouncementStudents.Include(x=>x.Announcement).SingleOrDefaultAsync(x=>x.AnnouncementId==id&&x.StudentProfile.ApplicationUserId==uid);if(link is null)return NotFound();link.IsViewed=true;link.ViewedAt??=DateTime.UtcNow;await dbContext.SaveChangesAsync();return View(link.Announcement);}
    public async Task<IActionResult> Download(int id){var uid=users.GetUserId(User);var item=await dbContext.AnnouncementStudents.AsNoTracking().Include(x=>x.Announcement).SingleOrDefaultAsync(x=>x.AnnouncementId==id&&x.StudentProfile.ApplicationUserId==uid&&x.Announcement.IsActive);if(item?.Announcement.AttachmentPath is null)return NotFound();var file=files.Open(item.Announcement.AttachmentPath);return file is null?NotFound():File(file.Value.Stream,file.Value.ContentType,file.Value.FileName);}
}
