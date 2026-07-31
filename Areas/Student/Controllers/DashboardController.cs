using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Students;

namespace OzelDersYonetim.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = IdentityDataSeeder.StudentRole)]
public class DashboardController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,ProfileImageService profileImages) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User);
        var student = await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(profile => profile.ApplicationUserId == userId && profile.IsActive);
        if (student is null) return NotFound("Aktif öğrenci profili bulunamadı.");
        var upcoming = await dbContext.StudentLessons.AsNoTracking().Include(item => item.Lesson).Where(item => item.StudentProfileId == student.Id && item.Lesson.EndDate >= DateTime.Now).OrderBy(item => item.Lesson.StartDate).Take(3).ToListAsync();
        var assignments = await dbContext.StudentAssignments.AsNoTracking().Include(item => item.Assignment).Where(item => item.StudentProfileId == student.Id && item.Status != Models.Assignments.StudentAssignmentStatus.Completed && item.Status != Models.Assignments.StudentAssignmentStatus.Evaluated).OrderBy(item => item.DueDate).Take(3).ToListAsync();
        var lateCount = await dbContext.StudentAssignments.CountAsync(item => item.StudentProfileId == student.Id && item.DueDate < DateTime.Now && item.Status < Models.Assignments.StudentAssignmentStatus.Submitted);
        var unreadCount = await dbContext.UserNotifications.CountAsync(item => item.ApplicationUserId == userId && !item.IsRead);
        var now=DateTime.Now;
        var documentsQuery=dbContext.CourseDocuments.AsNoTracking().Where(x=>x.IsActive&&(x.AccessType!=Models.Documents.DocumentAccessType.SelectedStudents||x.StudentDocuments.Any(s=>s.StudentProfileId==student.Id)));
        var announcementsQuery=dbContext.Announcements.AsNoTracking().Where(x=>x.IsActive&&x.PublishDate<=now&&(!x.ExpiryDate.HasValue||x.ExpiryDate>=now)&&x.AnnouncementStudents.Any(s=>s.StudentProfileId==student.Id));
        return View(new StudentDashboardViewModel{Student=student,UpcomingLessons=upcoming,UpcomingAssignments=assignments,LateAssignmentCount=lateCount,UnreadNotificationCount=unreadCount,EvaluatedAssignmentCount=await dbContext.StudentAssignments.CountAsync(x=>x.StudentProfileId==student.Id&&(x.Status==Models.Assignments.StudentAssignmentStatus.Evaluated||x.Status==Models.Assignments.StudentAssignmentStatus.Completed)),DocumentCount=await documentsQuery.CountAsync(),RecentAnnouncements=await announcementsQuery.OrderByDescending(x=>x.PublishDate).Take(3).ToListAsync(),RecentDocuments=await documentsQuery.OrderByDescending(x=>x.CreatedAt).Take(3).ToListAsync(),RecentFeedback=await dbContext.StudentAssignments.AsNoTracking().Include(x=>x.Assignment).Where(x=>x.StudentProfileId==student.Id&&x.TeacherFeedback!=null).OrderByDescending(x=>x.EvaluatedAt).Take(3).ToListAsync()});
    }

    public async Task<IActionResult> Profile()
    {
        var userId = userManager.GetUserId(User);
        var student = await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(profile => profile.ApplicationUserId == userId && profile.IsActive);
        return student is null ? NotFound() : View(student);
    }
    public async Task<IActionResult> Photo(){var userId=userManager.GetUserId(User);var stored=await dbContext.StudentProfiles.Where(x=>x.ApplicationUserId==userId&&x.IsActive).Select(x=>x.ProfileImagePath).SingleOrDefaultAsync();if(stored is null)return NotFound();var file=profileImages.Open(stored);return file is null?NotFound():File(file.Value.Stream,file.Value.ContentType);}
    [HttpPost,ValidateAntiForgeryToken,RequestSizeLimit(6*1024*1024)]public async Task<IActionResult> UploadPhoto(IFormFile? profileImage){var userId=userManager.GetUserId(User);var student=await dbContext.StudentProfiles.SingleOrDefaultAsync(x=>x.ApplicationUserId==userId&&x.IsActive);if(student is null)return NotFound();if(profileImage is null){TempData["Success"]="Yüklenecek profil fotoğrafını seçin.";return RedirectToAction(nameof(Profile));}try{student.ProfileImagePath=await profileImages.SaveAsync(profileImage);}catch(InvalidOperationException ex){TempData["Success"]=ex.Message;return RedirectToAction(nameof(Profile));}student.UpdatedAt=DateTime.UtcNow;await dbContext.SaveChangesAsync();TempData["Success"]="Profil fotoğrafınız güncellendi.";return RedirectToAction(nameof(Profile));}
}
