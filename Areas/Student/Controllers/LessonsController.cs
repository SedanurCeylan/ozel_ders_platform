using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Identity;
namespace OzelDersYonetim.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles=IdentityDataSeeder.StudentRole)]
public class LessonsController(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager):Controller
{
    public async Task<IActionResult> Index(string scope="upcoming")
    {
        var userId=userManager.GetUserId(User);var now=DateTime.Now;
        var query=dbContext.StudentLessons.AsNoTracking().Include(x=>x.Lesson).Where(x=>x.StudentProfile.ApplicationUserId==userId&&x.StudentProfile.IsActive);
        query=scope=="past"?query.Where(x=>x.Lesson.EndDate<now):query.Where(x=>x.Lesson.EndDate>=now);ViewBag.Scope=scope;
        return View(await query.OrderBy(x=>x.Lesson.StartDate).ToListAsync());
    }
    public async Task<IActionResult> Details(int id)
    {
        var userId=userManager.GetUserId(User);var item=await dbContext.StudentLessons.AsNoTracking().Include(x=>x.Lesson).SingleOrDefaultAsync(x=>x.Id==id&&x.StudentProfile.ApplicationUserId==userId&&x.StudentProfile.IsActive);
        return item is null?NotFound():View(item);
    }
}
