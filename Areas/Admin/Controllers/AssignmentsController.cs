using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Assignments;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Assignments;
using OzelDersYonetim.Services.Auditing;
using OzelDersYonetim.Services.Notifications;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class AssignmentsController(ApplicationDbContext dbContext, IAssignmentService assignmentService, AssignmentFileService assignmentFiles, IAuditService audit, INotificationService notifications, IEmailService emails, IEmailTemplateService templates) : Controller
{
    public async Task<IActionResult> Index(string? scope)
    {
        var query = dbContext.Assignments.AsNoTracking().Include(x => x.StudentAssignments).OrderByDescending(x => x.CreatedAt).AsQueryable();
        query = scope switch { "waiting" => query.Where(x => x.StudentAssignments.Any(a => a.Status == StudentAssignmentStatus.Submitted || a.Status == StudentAssignmentStatus.LateSubmitted)), "late" => query.Where(x => x.StudentAssignments.Any(a => a.DueDate < DateTime.Now && a.Status < StudentAssignmentStatus.Submitted)), "evaluated" => query.Where(x => x.StudentAssignments.Any(a => a.Status == StudentAssignmentStatus.Evaluated)), _ => query };
        ViewBag.Scope = scope;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await dbContext.Assignments.AsNoTracking().Include(x => x.StudentAssignments).ThenInclude(x => x.StudentProfile).Include(x => x.StudentAssignments).ThenInclude(x => x.Submissions).SingleOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : View(item);
    }

    public async Task<IActionResult> DownloadAttachment(int id)
    {
        var item = await dbContext.Assignments.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
        if (item?.AttachmentPath is null) return NotFound();
        var file = assignmentFiles.Open(item.AttachmentPath, item.Id);
        return file is null ? NotFound() : File(file.Value.Stream, file.Value.ContentType, "odev-eki" + Path.GetExtension(item.AttachmentPath));
    }

    public async Task<IActionResult> DownloadSubmission(int id)
    {
        var item = await dbContext.AssignmentSubmissions.AsNoTracking()
            .Include(x => x.StudentAssignment)
            .SingleOrDefaultAsync(x => x.Id == id);
        if (item?.FilePath is null) return NotFound();
        var file = assignmentFiles.Open(item.FilePath, item.StudentAssignment.AssignmentId, item.StudentAssignment.StudentProfileId);
        return file is null ? NotFound() : File(file.Value.Stream, file.Value.ContentType, item.FileName ?? "ogrenci-teslimi" + Path.GetExtension(item.FilePath));
    }

    public async Task<IActionResult> Create() => View(await FormAsync(new AssignmentFormViewModel()));

    public async Task<IActionResult> Edit(int id)
    {
        var assignment=await dbContext.Assignments.Include(x=>x.StudentAssignments).SingleOrDefaultAsync(x=>x.Id==id);if(assignment is null)return NotFound();return View(await FormAsync(new AssignmentFormViewModel{Assignment=assignment,SelectedStudentIds=assignment.StudentAssignments.Select(x=>x.StudentProfileId).ToList()}));
    }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,AssignmentFormViewModel model)
    {
        if(id!=model.Assignment.Id)return BadRequest();if(model.Assignment.DueDate<=model.Assignment.StartDate)ModelState.AddModelError("Assignment.DueDate","Son teslim tarihi başlangıç tarihinden sonra olmalıdır.");if(model.SelectedStudentIds.Count==0)ModelState.AddModelError(nameof(model.SelectedStudentIds),"En az bir öğrenci seçin.");if(!ModelState.IsValid)return View(await FormAsync(model));
        var assignment=await dbContext.Assignments.Include(x=>x.StudentAssignments).ThenInclude(x=>x.Submissions).SingleOrDefaultAsync(x=>x.Id==id);if(assignment is null)return NotFound();var oldDueDate=assignment.DueDate;var attachment=assignment.AttachmentPath;var created=assignment.CreatedAt;dbContext.Entry(assignment).CurrentValues.SetValues(model.Assignment);assignment.AttachmentPath=attachment;assignment.CreatedAt=created;assignment.UpdatedAt=DateTime.UtcNow;
        var selected=model.SelectedStudentIds.Distinct().ToHashSet();var removable=assignment.StudentAssignments.Where(x=>!selected.Contains(x.StudentProfileId)&&x.Submissions.Count==0).ToList();dbContext.StudentAssignments.RemoveRange(removable);foreach(var existing in assignment.StudentAssignments.Where(x=>selected.Contains(x.StudentProfileId)))existing.DueDate=assignment.DueDate;foreach(var studentId in selected.Except(assignment.StudentAssignments.Select(x=>x.StudentProfileId)))assignment.StudentAssignments.Add(new StudentAssignment{StudentProfileId=studentId,DueDate=assignment.DueDate});await dbContext.SaveChangesAsync();
        if(oldDueDate!=assignment.DueDate){var recipients=await dbContext.StudentAssignments.Where(x=>x.AssignmentId==id&&x.StudentProfile.ApplicationUserId!=null).Select(x=>x.StudentProfile).ToListAsync();var message=$"{assignment.Title} ödevinin yeni son teslim tarihi: {assignment.DueDate:dd.MM.yyyy HH:mm}";await notifications.CreateAsync(recipients.Select(x=>x.ApplicationUserId!),"Ödev teslim tarihi değişti",message,"Ödev tarihi değişikliği",assignment.Id,$"/Student/Assignments");if(assignment.SendEmailNotification)foreach(var student in recipients)await emails.SendEmailAsync(student.Email,student.FullName,"Ödev teslim tarihi değişti",templates.Build("Ödev teslim tarihi değişti",message,assignment.DueDate),"Ödev tarihi değişikliği",assignment.Id);}
        await audit.LogAsync("Ödev güncelleme","Ödev",id,assignment.Title);TempData["Success"]="Ödev ve öğrenci atamaları güncellendi.";return RedirectToAction(nameof(Details),new{id});
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> Create(AssignmentFormViewModel model)
    {
        if (!ModelState.IsValid) return View(await FormAsync(model));
        try { await assignmentService.CreateAndAssignAsync(model.Assignment, model.SelectedStudentIds, model.Attachment); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(await FormAsync(model)); }
        await audit.LogAsync("Ödev oluşturma ve atama", "Ödev", model.Assignment.Id, model.Assignment.Title);
        TempData["Success"] = "Ödev oluşturuldu ve öğrencilere atandı.";
        return RedirectToAction(nameof(Details), new { id = model.Assignment.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Evaluate(int id, decimal? score, string? feedback, bool requestResubmission)
    {
        var item = await dbContext.StudentAssignments.Include(x => x.Assignment).Include(x=>x.StudentProfile).SingleOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();
        if (score is < 0 || score > item.Assignment.MaximumScore) { TempData["Success"] = $"Puan 0 ile {item.Assignment.MaximumScore} arasında olmalıdır."; return RedirectToAction(nameof(Details), new { id = item.AssignmentId }); }
        item.Score = score; item.TeacherFeedback = feedback; item.EvaluatedAt = DateTime.UtcNow; item.UpdatedAt = DateTime.UtcNow;
        item.Status = requestResubmission ? StudentAssignmentStatus.ResubmissionRequested : StudentAssignmentStatus.Evaluated;
        await dbContext.SaveChangesAsync();
        await audit.LogAsync("Ödev değerlendirme", "Öğrenci ödevi", item.Id, $"{item.Assignment.Title} · Puan: {score?.ToString() ?? "Yok"}");
        if(item.StudentProfile.ApplicationUserId is not null){var title=requestResubmission?"Ödev yeniden teslim istendi":"Ödeviniz değerlendirildi";var message=requestResubmission?$"{item.Assignment.Title} ödevi için yeniden teslim istendi.":$"{item.Assignment.Title} ödeviniz değerlendirildi. Puan: {score?.ToString()??"—"}/{item.Assignment.MaximumScore}.";await notifications.CreateAsync(new[]{item.StudentProfile.ApplicationUserId},title,message,title,item.Id,$"/Student/Assignments/Details/{item.Id}");await emails.SendEmailAsync(item.StudentProfile.Email,item.StudentProfile.FullName,title,templates.Build(title,message),title,item.Id);}
        TempData["Success"] = requestResubmission ? "Ödev yeniden teslim için öğrenciye gönderildi." : "Ödev değerlendirildi.";
        return RedirectToAction(nameof(Details), new { id = item.AssignmentId });
    }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var assignment=await dbContext.Assignments.Include(x=>x.StudentAssignments).ThenInclude(x=>x.Submissions).SingleOrDefaultAsync(x=>x.Id==id);if(assignment is null)return NotFound();if(assignment.StudentAssignments.Any(x=>x.Submissions.Count>0||x.SubmittedAt.HasValue)){assignment.Status=AssignmentStatus.Cancelled;assignment.UpdatedAt=DateTime.UtcNow;}else{dbContext.StudentAssignments.RemoveRange(assignment.StudentAssignments);dbContext.Assignments.Remove(assignment);}await dbContext.SaveChangesAsync();await audit.LogAsync("Ödev iptal etme","Ödev",id,assignment.Title);TempData["Success"]="Ödev iptal edildi; mevcut teslim geçmişi korundu.";return RedirectToAction(nameof(Index));
    }

    private async Task<AssignmentFormViewModel> FormAsync(AssignmentFormViewModel model)
    {
        model.Students = await dbContext.StudentProfiles.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.FirstName).Select(x => new SelectListItem(x.FullName + " · " + x.GradeLevel, x.Id.ToString())).ToListAsync();
        return model;
    }
}
