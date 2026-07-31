using Microsoft.AspNetCore.Http;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Assignments;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Services.Notifications;

namespace OzelDersYonetim.Services.Assignments;

public class AssignmentService(ApplicationDbContext dbContext, AssignmentFileService fileService, INotificationService notificationService, IEmailService emailService, IEmailTemplateService emailTemplates) : IAssignmentService
{
    public async Task CreateAndAssignAsync(Assignment assignment, IReadOnlyCollection<int> studentIds, IFormFile? attachment)
    {
        if (assignment.DueDate <= assignment.StartDate) throw new InvalidOperationException("Son teslim tarihi başlangıç tarihinden sonra olmalıdır.");
        if (studentIds.Count == 0) throw new InvalidOperationException("En az bir öğrenci seçin.");
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        assignment.CreatedAt = DateTime.UtcNow;
        dbContext.Assignments.Add(assignment);
        await dbContext.SaveChangesAsync();
        if (attachment is not null) assignment.AttachmentPath = await fileService.SaveAsync(attachment, assignment.Id);
        foreach (var id in studentIds.Distinct()) dbContext.StudentAssignments.Add(new StudentAssignment { AssignmentId = assignment.Id, StudentProfileId = id, DueDate = assignment.DueDate });
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        var recipients = await dbContext.StudentProfiles.AsNoTracking().Where(x => studentIds.Contains(x.Id) && x.ApplicationUserId != null).ToListAsync();
        await notificationService.CreateAsync(recipients.Select(x => x.ApplicationUserId!), "Yeni ödev atandı", assignment.Title, "Yeni ödev", assignment.Id, $"/Student/Assignments");
        if (assignment.SendEmailNotification)
            foreach (var student in recipients)
                await emailService.SendEmailAsync(student.Email, student.FullName, "Yeni ödev: " + assignment.Title, emailTemplates.Build("Yeni ödev atandı", assignment.ShortDescription, assignment.DueDate), "Yeni ödev", assignment.Id);
    }
}
