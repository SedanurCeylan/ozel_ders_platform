using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.ViewModels;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class DashboardController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;
        var todayStart = now.Date;
        var tomorrow = todayStart.AddDays(1);
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var nextMonth = monthStart.AddMonths(1);
        var submittedStatuses = new[] { StudentAssignmentStatus.Submitted, StudentAssignmentStatus.LateSubmitted };

        var model = new AdminDashboardViewModel
        {
            TotalStudents = await dbContext.StudentProfiles.CountAsync(),
            ActiveStudents = await dbContext.StudentProfiles.CountAsync(x => x.IsActive),
            PassiveStudents = await dbContext.StudentProfiles.CountAsync(x => !x.IsActive),
            TodayLessons = await dbContext.Lessons.CountAsync(x => x.StartDate >= todayStart && x.StartDate < tomorrow),
            UpcomingLessons = await dbContext.Lessons.CountAsync(x => x.StartDate >= now && x.Status != Models.Lessons.LessonStatus.TeacherCancelled && x.Status != Models.Lessons.LessonStatus.StudentCancelled),
            PendingAssignments = await dbContext.StudentAssignments.CountAsync(x => x.DueDate >= now && x.Status < StudentAssignmentStatus.Submitted),
            LateAssignments = await dbContext.StudentAssignments.CountAsync(x => x.DueDate < now && x.Status < StudentAssignmentStatus.Submitted),
            AwaitingEvaluation = await dbContext.StudentAssignments.CountAsync(x => submittedStatuses.Contains(x.Status)),
            FailedEmails = await dbContext.EmailLogs.CountAsync(x => !x.IsSuccessful),
            TotalDocuments = await dbContext.CourseDocuments.CountAsync(x => x.IsActive),
            MonthlyLessons = await dbContext.Lessons.CountAsync(x => x.StartDate >= monthStart && x.StartDate < nextMonth),
            UnreadAnnouncements = await dbContext.AnnouncementStudents.CountAsync(x => !x.IsViewed && x.Announcement.IsActive),
            TodaySchedule = await dbContext.Lessons.AsNoTracking().Include(x => x.StudentLessons).ThenInclude(x => x.StudentProfile).Where(x => x.StartDate >= todayStart && x.StartDate < tomorrow).OrderBy(x => x.StartDate).ToListAsync(),
            RecentStudents = await dbContext.StudentProfiles.AsNoTracking().OrderByDescending(x => x.CreatedAt).Take(5).ToListAsync(),
            UpcomingAssignmentDeadlines = await dbContext.StudentAssignments.AsNoTracking().Include(x => x.Assignment).Include(x => x.StudentProfile).Where(x => x.DueDate >= now && x.Status < StudentAssignmentStatus.Submitted).OrderBy(x => x.DueDate).Take(5).ToListAsync(),
            RecentSubmissions = await dbContext.AssignmentSubmissions.AsNoTracking().Include(x => x.StudentAssignment).ThenInclude(x => x.Assignment).Include(x => x.StudentAssignment).ThenInclude(x => x.StudentProfile).OrderByDescending(x => x.SubmittedAt).Take(5).ToListAsync(),
            RecentExamResults = await dbContext.ExamResults.AsNoTracking().Include(x => x.StudentProfile).OrderByDescending(x => x.ExamDate).Take(5).ToListAsync(),
            RecentEmailErrors = await dbContext.EmailLogs.AsNoTracking().Where(x => !x.IsSuccessful).OrderByDescending(x => x.CreatedAt).Take(5).ToListAsync()
        };
        return View(model);
    }
}
