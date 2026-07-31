using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Notifications;
namespace OzelDersYonetim.Services.Notifications;
public class AnnouncementService(ApplicationDbContext dbContext, INotificationService notifications, IEmailService email, IEmailTemplateService templates) : IAnnouncementService
{
    public async Task PublishAsync(Announcement announcement, IReadOnlyCollection<int> selectedStudentIds)
    {
        var students = dbContext.StudentProfiles.Include(x=>x.ApplicationUser).Where(x=>x.IsActive);
        students = announcement.TargetType switch { AnnouncementTargetType.GradeLevel => students.Where(x=>x.GradeLevel==announcement.GradeLevel), AnnouncementTargetType.SelectedStudents => students.Where(x=>selectedStudentIds.Contains(x.Id)), _ => students };
        var recipients = await students.ToListAsync(); if(announcement.TargetType==AnnouncementTargetType.SelectedStudents && recipients.Count==0) throw new InvalidOperationException("En az bir öğrenci seçin.");
        dbContext.Announcements.Add(announcement); await dbContext.SaveChangesAsync(); foreach(var student in recipients) dbContext.AnnouncementStudents.Add(new AnnouncementStudent { AnnouncementId=announcement.Id, StudentProfileId=student.Id }); await dbContext.SaveChangesAsync();
        await notifications.CreateAsync(recipients.Where(x=>x.ApplicationUserId!=null).Select(x=>x.ApplicationUserId!), announcement.Title, announcement.Content.Length>180?announcement.Content[..180]+"…":announcement.Content, "Yeni duyuru", announcement.Id, "/Student/Announcements/Details/"+announcement.Id);
        if(announcement.SendEmail) foreach(var student in recipients) await email.SendEmailAsync(student.Email, student.FullName, announcement.Title, templates.Build(announcement.Title, announcement.Content, announcement.PublishDate), "Duyuru", announcement.Id);
    }
}
