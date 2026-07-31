using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Progress;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Auditing;
using OzelDersYonetim.Services.Notifications;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class ProgressController(ApplicationDbContext dbContext, IEmailService emailService, IEmailTemplateService emailTemplates, IAuditService audit) : Controller
{
    public async Task<IActionResult> Index(int? studentId)
    {
        ViewBag.Students = await dbContext.StudentProfiles.AsNoTracking().OrderBy(x => x.FirstName).ToListAsync();
        if (!studentId.HasValue) return View(new StudentProgressViewModel());
        var student = await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == studentId); if (student is null) return NotFound();
        return View(new StudentProgressViewModel { Student = student, ExamResults = await dbContext.ExamResults.Where(x => x.StudentProfileId == studentId).OrderByDescending(x => x.ExamDate).ToListAsync(), ProgressRecords = await dbContext.StudentProgressRecords.Where(x => x.StudentProfileId == studentId).OrderByDescending(x => x.EvaluatedAt).ToListAsync(), TeacherNotes = await dbContext.TeacherStudentNotes.Where(x => x.StudentProfileId == studentId).OrderByDescending(x => x.IsImportant).ThenByDescending(x => x.CreatedAt).ToListAsync() });
    }

    public async Task<IActionResult> CreateExam(int studentId) { var student = await dbContext.StudentProfiles.FindAsync(studentId); return student is null ? NotFound() : View(new ExamResult { StudentProfileId = studentId }); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExam(ExamResult model)
    {
        var student = await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.StudentProfileId);
        if (student is null) return NotFound();
        if (model.CorrectCount + model.WrongCount + model.EmptyCount > model.TotalQuestions) ModelState.AddModelError(string.Empty, "Doğru, yanlış ve boş toplamı toplam soru sayısını aşamaz.");
        if (model.NotifyParent && string.IsNullOrWhiteSpace(student.ParentEmail)) ModelState.AddModelError(nameof(model.NotifyParent), "Veli bildirimi için öğrencinin veli e-posta adresini kaydedin.");
        if (!ModelState.IsValid) return View(model);
        model.CalculateNet(); dbContext.ExamResults.Add(model); await dbContext.SaveChangesAsync();
        var emailSent = true;
        if (model.NotifyParent)
        {
            var parentName = ($"{student.ParentFirstName} {student.ParentLastName}").Trim();
            var message = $"{student.FullName} öğrencisinin {model.ExamDate:dd.MM.yyyy} tarihli <strong>{model.ExamName}</strong> sonucu sisteme eklendi.<br><br>Doğru: <strong>{model.CorrectCount}</strong> · Yanlış: <strong>{model.WrongCount}</strong> · Boş: <strong>{model.EmptyCount}</strong> · Net: <strong>{model.NetScore:0.##}</strong>{(model.MathematicsScore.HasValue ? $" · Puan: <strong>{model.MathematicsScore:0.##}</strong>" : string.Empty)}";
            emailSent = await emailService.SendEmailAsync(student.ParentEmail!, parentName, $"{student.FullName} · {model.ExamName} sonucu", emailTemplates.Build("Deneme sınavı sonucu", message), "Veli deneme sonucu", model.Id);
        }
        await audit.LogAsync("Deneme sonucu ekleme", "Deneme sonucu", model.Id, $"{student.FullName} · {model.ExamName}");
        TempData["Success"] = model.NotifyParent ? (emailSent ? "Deneme sonucu eklendi ve veliye bildirildi." : "Deneme sonucu eklendi; veli e-postası gönderilemedi. E-posta geçmişini inceleyin.") : "Deneme sonucu eklendi.";
        return RedirectToAction(nameof(Index), new { studentId = model.StudentProfileId });
    }

    public async Task<IActionResult> CreateProgress(int studentId) { var student = await dbContext.StudentProfiles.FindAsync(studentId); return student is null ? NotFound() : View(new StudentProgress { StudentProfileId = studentId }); }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProgress(StudentProgress model)
    {
        var student = await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.StudentProfileId);
        if (student is null) return NotFound();
        if (model.NotifyParent && string.IsNullOrWhiteSpace(student.ParentEmail)) ModelState.AddModelError(nameof(model.NotifyParent), "Veli bildirimi için öğrencinin veli e-posta adresini kaydedin.");
        if (!ModelState.IsValid) return View(model);
        dbContext.StudentProgressRecords.Add(model); await dbContext.SaveChangesAsync();
        var emailSent = true;
        if (model.NotifyParent)
        {
            var parentName = ($"{student.ParentFirstName} {student.ParentLastName}").Trim();
            var comment = string.IsNullOrWhiteSpace(model.TeacherComment) ? string.Empty : $"<br><br>Öğretmen değerlendirmesi: {System.Net.WebUtility.HtmlEncode(model.TeacherComment)}";
            var message = $"{student.FullName} öğrencisinin <strong>{System.Net.WebUtility.HtmlEncode(model.Topic)}</strong> konusu için gelişim kaydı güncellendi.<br><br>Durum: <strong>{model.StatusName}</strong> · Başarı: <strong>%{model.SuccessPercentage:0.##}</strong>{comment}";
            emailSent = await emailService.SendEmailAsync(student.ParentEmail!, parentName, $"{student.FullName} · gelişim bilgilendirmesi", emailTemplates.Build("Öğrenci gelişim bilgilendirmesi", message), "Veli gelişim raporu", model.Id);
        }
        await audit.LogAsync("Gelişim kaydı ekleme", "Gelişim kaydı", model.Id, $"{student.FullName} · {model.Topic}");
        TempData["Success"] = model.NotifyParent ? (emailSent ? "Gelişim kaydı eklendi ve veliye bildirildi." : "Gelişim kaydı eklendi; veli e-postası gönderilemedi. E-posta geçmişini inceleyin.") : "Gelişim kaydı eklendi.";
        return RedirectToAction(nameof(Index), new { studentId = model.StudentProfileId });
    }

    public async Task<IActionResult> CreateNote(int studentId) { var student = await dbContext.StudentProfiles.FindAsync(studentId); return student is null ? NotFound() : View(new TeacherStudentNote { StudentProfileId = studentId }); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CreateNote(TeacherStudentNote model) { if (!ModelState.IsValid) return View(model); dbContext.TeacherStudentNotes.Add(model); await dbContext.SaveChangesAsync(); TempData["Success"] = "Özel öğretmen notu eklendi."; return RedirectToAction(nameof(Index), new { studentId = model.StudentProfileId }); }

    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> DeleteExam(int id) { var item = await dbContext.ExamResults.FindAsync(id); if (item is null) return NotFound(); var studentId = item.StudentProfileId; dbContext.Remove(item); await dbContext.SaveChangesAsync(); return RedirectToAction(nameof(Index), new { studentId }); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> DeleteProgress(int id) { var item = await dbContext.StudentProgressRecords.FindAsync(id); if (item is null) return NotFound(); var studentId = item.StudentProfileId; dbContext.Remove(item); await dbContext.SaveChangesAsync(); return RedirectToAction(nameof(Index), new { studentId }); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> DeleteNote(int id) { var item = await dbContext.TeacherStudentNotes.FindAsync(id); if (item is null) return NotFound(); var studentId = item.StudentProfileId; dbContext.Remove(item); await dbContext.SaveChangesAsync(); return RedirectToAction(nameof(Index), new { studentId }); }
}
