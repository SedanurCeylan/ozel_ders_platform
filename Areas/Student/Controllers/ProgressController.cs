using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Models.ViewModels;

namespace OzelDersYonetim.Areas.Student.Controllers;

[Area("Student"), Authorize(Roles = IdentityDataSeeder.StudentRole)]
public class ProgressController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User); var student = await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.ApplicationUserId == userId && x.IsActive); if (student is null) return NotFound();
        return View(new StudentProgressViewModel { Student = student, ExamResults = await dbContext.ExamResults.AsNoTracking().Where(x => x.StudentProfileId == student.Id).OrderBy(x => x.ExamDate).ToListAsync(), ProgressRecords = await dbContext.StudentProgressRecords.AsNoTracking().Where(x => x.StudentProfileId == student.Id).OrderByDescending(x => x.EvaluatedAt).ToListAsync() });
    }
}
