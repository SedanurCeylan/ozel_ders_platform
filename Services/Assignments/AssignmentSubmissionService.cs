using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Assignments;

namespace OzelDersYonetim.Services.Assignments;

public class AssignmentSubmissionService(ApplicationDbContext dbContext, AssignmentFileService fileService) : IAssignmentSubmissionService
{
    public async Task SubmitAsync(StudentAssignment studentAssignment, string? description, IFormFile? file)
    {
        if (!studentAssignment.Assignment.AllowStudentComment && !string.IsNullOrWhiteSpace(description)) throw new InvalidOperationException("Bu ödev için açıklama eklenemez.");
        if (!studentAssignment.Assignment.AllowFileUpload && file is not null) throw new InvalidOperationException("Bu ödev için dosya yüklenemez.");
        if (file is null && string.IsNullOrWhiteSpace(description)) throw new InvalidOperationException("Bir açıklama yazın veya dosya yükleyin.");
        if (DateTime.Now > studentAssignment.DueDate && !studentAssignment.Assignment.AllowLateSubmission) throw new InvalidOperationException("Bu ödev için teslim süresi sona erdi.");
        foreach (var old in studentAssignment.Submissions.Where(x => x.IsActive)) old.IsActive = false;
        var submission = new AssignmentSubmission { StudentAssignmentId = studentAssignment.Id, StudentDescription = description?.Trim(), SubmissionNumber = await dbContext.AssignmentSubmissions.CountAsync(x => x.StudentAssignmentId == studentAssignment.Id) + 1 };
        if (file is not null) { submission.FileName = Path.GetFileName(file.FileName); submission.FileSize = file.Length; submission.FilePath = await fileService.SaveAsync(file, studentAssignment.AssignmentId, studentAssignment.StudentProfileId); }
        var isLate = DateTime.Now > studentAssignment.DueDate;
        studentAssignment.IsLate = isLate;
        studentAssignment.SubmittedAt = DateTime.UtcNow;
        studentAssignment.Status = isLate ? StudentAssignmentStatus.LateSubmitted : StudentAssignmentStatus.Submitted;
        studentAssignment.UpdatedAt = DateTime.UtcNow;
        dbContext.AssignmentSubmissions.Add(submission);
        await dbContext.SaveChangesAsync();
    }
}
