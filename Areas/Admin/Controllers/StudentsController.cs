using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Identity;
using OzelDersYonetim.Models.Students;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Notifications;
using OzelDersYonetim.Services.Auditing;
using OzelDersYonetim.Services.Students;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class StudentsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IEmailService emailService, IEmailTemplateService emailTemplates, IAuditService audit, ProfileImageService profileImages) : Controller
{
    public async Task<IActionResult> Index(string? search, string? grade, bool? active, string? lessonPreference, string? assignmentStatus, string? sort)
    {
        var query = dbContext.StudentProfiles.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(student => student.FirstName.Contains(search) || student.LastName.Contains(search) || student.Email.Contains(search));
        }
        if (!string.IsNullOrWhiteSpace(grade)) query = query.Where(student => student.GradeLevel == grade);
        if (active.HasValue) query = query.Where(student => student.IsActive == active.Value);
        if (!string.IsNullOrWhiteSpace(lessonPreference)) query = query.Where(student => student.LessonPreference == lessonPreference);
        if (assignmentStatus == "pending") query = query.Where(student => student.StudentAssignments.Any(x => x.Status < Models.Assignments.StudentAssignmentStatus.Submitted));
        if (assignmentStatus == "submitted") query = query.Where(student => student.StudentAssignments.Any(x => x.Status >= Models.Assignments.StudentAssignmentStatus.Submitted));

        ViewBag.Search = search;
        ViewBag.Grade = grade;
        ViewBag.Active = active;
        ViewBag.LessonPreference = lessonPreference; ViewBag.AssignmentStatus = assignmentStatus; ViewBag.Sort = sort;
        query = sort switch { "oldest" => query.OrderBy(x => x.RegistrationDate), "newest" => query.OrderByDescending(x => x.RegistrationDate), _ => query.OrderBy(x => x.FirstName).ThenBy(x => x.LastName) };
        return View(await query.Select(student => new StudentListItemViewModel { Student = student, LastLessonDate = student.StudentLessons.Where(x => x.Lesson.EndDate < DateTime.Now).OrderByDescending(x => x.Lesson.EndDate).Select(x => (DateTime?)x.Lesson.EndDate).FirstOrDefault(), PendingAssignmentCount = student.StudentAssignments.Count(x => x.Status < Models.Assignments.StudentAssignmentStatus.Submitted) }).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var student = await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
        if(student is null)return NotFound();
        var lessons=await dbContext.StudentLessons.AsNoTracking().Include(x=>x.Lesson).Where(x=>x.StudentProfileId==id).OrderByDescending(x=>x.Lesson.StartDate).ToListAsync();
        var assignments=await dbContext.StudentAssignments.AsNoTracking().Include(x=>x.Assignment).Include(x=>x.Submissions).Where(x=>x.StudentProfileId==id).OrderByDescending(x=>x.AssignedAt).ToListAsync();
        var documents=await dbContext.StudentDocuments.AsNoTracking().Include(x=>x.CourseDocument).Where(x=>x.StudentProfileId==id).OrderByDescending(x=>x.AssignedAt).ToListAsync();
        var concluded=lessons.Where(x=>x.AttendanceStatus!=Models.Lessons.AttendanceStatus.Pending).ToList();
        var evaluated=assignments.Where(x=>x.Score.HasValue&&x.Assignment.MaximumScore>0).ToList();
        return View(new StudentDetailViewModel{Student=student,Lessons=lessons,Assignments=assignments,Documents=documents,ExamResults=await dbContext.ExamResults.AsNoTracking().Where(x=>x.StudentProfileId==id).OrderByDescending(x=>x.ExamDate).ToListAsync(),ProgressRecords=await dbContext.StudentProgressRecords.AsNoTracking().Where(x=>x.StudentProfileId==id).OrderByDescending(x=>x.EvaluatedAt).ToListAsync(),TeacherNotes=await dbContext.TeacherStudentNotes.AsNoTracking().Where(x=>x.StudentProfileId==id).OrderByDescending(x=>x.IsImportant).ThenByDescending(x=>x.CreatedAt).ToListAsync(),EmailLogs=await dbContext.EmailLogs.AsNoTracking().Where(x=>x.RecipientEmail==student.Email).OrderByDescending(x=>x.CreatedAt).Take(50).ToListAsync(),AttendanceRate=concluded.Count==0?0:Math.Round(concluded.Count(x=>x.AttendanceStatus==Models.Lessons.AttendanceStatus.Attended)*100m/concluded.Count,1),AssignmentSuccessRate=evaluated.Count==0?0:Math.Round(evaluated.Average(x=>x.Score!.Value/x.Assignment.MaximumScore*100m),1)});
    }

    public IActionResult Create() => View(new StudentCreateViewModel());

    [HttpPost, ValidateAntiForgeryToken,RequestSizeLimit(6*1024*1024)]
    public async Task<IActionResult> Create(StudentCreateViewModel model)
    {
        if (await dbContext.StudentProfiles.IgnoreQueryFilters().AnyAsync(student => student.Email == model.Student.Email))
            ModelState.AddModelError("Student.Email", "Bu e-posta adresi başka bir öğrenci tarafından kullanılıyor.");
        if (model.CreateAccount && string.IsNullOrWhiteSpace(model.TemporaryPassword))
            ModelState.AddModelError(nameof(model.TemporaryPassword), "Hesap oluşturmak için geçici şifre zorunludur.");
        if (!ModelState.IsValid) return View(model);
        if(model.ProfileImage is not null){try{model.Student.ProfileImagePath=await profileImages.SaveAsync(model.ProfileImage);}catch(InvalidOperationException ex){ModelState.AddModelError(nameof(model.ProfileImage),ex.Message);return View(model);}}

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        ApplicationUser? account = null;
        if (model.CreateAccount)
        {
            account = new ApplicationUser { UserName = model.Student.Email, Email = model.Student.Email, EmailConfirmed = true, FirstName = model.Student.FirstName, LastName = model.Student.LastName, IsActive = model.Student.IsActive, MustChangePassword = true };
            var createResult = await userManager.CreateAsync(account, model.TemporaryPassword!);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors) ModelState.AddModelError(nameof(model.TemporaryPassword), error.ToTurkish());
                return View(model);
            }
            var roleResult = await userManager.AddToRoleAsync(account, IdentityDataSeeder.StudentRole);
            if (!roleResult.Succeeded) throw new InvalidOperationException("Öğrenci rolü atanamadı.");
            model.Student.ApplicationUserId = account.Id;
        }

        model.Student.CreatedAt = DateTime.UtcNow;
        dbContext.StudentProfiles.Add(model.Student);
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        if (account is not null)
        {
            var message = $"Sisteme öğrenci olarak kaydınız oluşturuldu. Kullanıcı adınız: {model.Student.Email}. Geçici şifreniz: {model.TemporaryPassword}. İlk girişinizden sonra şifrenizi değiştirin.";
            var safeLogBody = emailTemplates.Build("Öğrenci hesabınız hazır", "Öğrenci hesabı oluşturuldu. Güvenlik nedeniyle geçici şifre gönderim geçmişinde saklanmamıştır.");
            await emailService.SendEmailAsync(model.Student.Email, model.Student.FullName, "Matematik Atölyesi öğrenci hesabınız", emailTemplates.Build("Öğrenci hesabınız hazır", message), "Yeni öğrenci hesabı", model.Student.Id, safeLogBody);
        }
        await audit.LogAsync("Öğrenci oluşturma", "Öğrenci", model.Student.Id, model.Student.FullName);
        TempData["Success"] = "Öğrenci kaydı başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Details), new { id = model.Student.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var student = await dbContext.StudentProfiles.FindAsync(id);
        return student is null ? NotFound() : View(student);
    }

    public async Task<IActionResult> Photo(int id){var student=await dbContext.StudentProfiles.AsNoTracking().IgnoreQueryFilters().SingleOrDefaultAsync(x=>x.Id==id);if(student?.ProfileImagePath is null)return NotFound();var file=profileImages.Open(student.ProfileImagePath);return file is null?NotFound():File(file.Value.Stream,file.Value.ContentType);}

    [HttpPost,ValidateAntiForgeryToken,RequestSizeLimit(6*1024*1024)]
    public async Task<IActionResult> UploadPhoto(int id,IFormFile? profileImage){var student=await dbContext.StudentProfiles.FindAsync(id);if(student is null)return NotFound();if(profileImage is null){TempData["Success"]="Yüklenecek fotoğrafı seçin.";return RedirectToAction(nameof(Details),new{id});}try{student.ProfileImagePath=await profileImages.SaveAsync(profileImage);}catch(InvalidOperationException ex){TempData["Success"]=ex.Message;return RedirectToAction(nameof(Details),new{id});}student.UpdatedAt=DateTime.UtcNow;await dbContext.SaveChangesAsync();await audit.LogAsync("Profil fotoğrafı güncelleme","Öğrenci",id,student.FullName);TempData["Success"]="Profil fotoğrafı güncellendi.";return RedirectToAction(nameof(Details),new{id});}

    public async Task<IActionResult> ResetPassword(int id)
    {
        var student=await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id);if(student is null)return NotFound();if(string.IsNullOrWhiteSpace(student.ApplicationUserId)){TempData["Success"]="Bu öğrenci için giriş hesabı bulunmuyor.";return RedirectToAction(nameof(Details),new{id});}return View(new StudentPasswordResetViewModel{StudentId=id,StudentName=student.FullName});
    }

    public async Task<IActionResult> CreateAccount(int id)
    {
        var student=await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id);if(student is null)return NotFound();if(!string.IsNullOrWhiteSpace(student.ApplicationUserId)){TempData["Success"]="Bu öğrencinin giriş hesabı zaten var.";return RedirectToAction(nameof(Details),new{id});}return View(new StudentAccountCreateViewModel{StudentId=id,StudentName=student.FullName,Email=student.Email});
    }

    public async Task<IActionResult> SendEmail(int id)
    {
        var student=await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==id);if(student is null)return NotFound();return View(ToEmailModel(student));
    }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> SendEmail(StudentEmailViewModel model)
    {
        var student=await dbContext.StudentProfiles.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==model.StudentId);if(student is null)return NotFound();var display=ToEmailModel(student);model.StudentName=display.StudentName;model.StudentEmail=display.StudentEmail;model.ParentName=display.ParentName;model.ParentEmail=display.ParentEmail;if(model.RecipientType is "Parent" or "Both"&&string.IsNullOrWhiteSpace(student.ParentEmail))ModelState.AddModelError(nameof(model.RecipientType),"Bu öğrenci için veli e-posta adresi kayıtlı değil.");if(model.RecipientType is not("Student" or "Parent" or "Both"))ModelState.AddModelError(nameof(model.RecipientType),"Geçerli bir alıcı seçin.");if(!ModelState.IsValid)return View(model);
        var body=emailTemplates.Build(model.Subject,model.Message);var results=new List<bool>();if(model.RecipientType is "Student" or "Both")results.Add(await emailService.SendEmailAsync(student.Email,student.FullName,model.Subject,body,"Özel öğrenci mesajı",student.Id));if(model.RecipientType is "Parent" or "Both")results.Add(await emailService.SendEmailAsync(student.ParentEmail!,($"{student.ParentFirstName} {student.ParentLastName}").Trim(),model.Subject,body,"Veli bilgilendirmesi",student.Id));await audit.LogAsync("Özel e-posta gönderme","Öğrenci",student.Id,$"{student.FullName} · {model.RecipientType} · {model.Subject}");TempData["Success"]=results.All(x=>x)?"E-posta başarıyla gönderildi.":"E-posta gönderimi tamamlanamadı. E-posta geçmişindeki hata kaydını inceleyin.";return RedirectToAction(nameof(Details),new{id=student.Id});
    }

    private static StudentEmailViewModel ToEmailModel(StudentProfile student)=>new(){StudentId=student.Id,StudentName=student.FullName,StudentEmail=student.Email,ParentName=($"{student.ParentFirstName} {student.ParentLastName}").Trim(),ParentEmail=student.ParentEmail};

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount(StudentAccountCreateViewModel model)
    {
        var student=await dbContext.StudentProfiles.SingleOrDefaultAsync(x=>x.Id==model.StudentId);if(student is null)return NotFound();model.StudentName=student.FullName;model.Email=student.Email;if(!string.IsNullOrWhiteSpace(student.ApplicationUserId)){ModelState.AddModelError(string.Empty,"Bu öğrencinin giriş hesabı zaten var.");return View(model);}if(await userManager.FindByEmailAsync(student.Email) is not null)ModelState.AddModelError(string.Empty,"Bu e-posta adresine ait başka bir kullanıcı hesabı bulunuyor.");if(!ModelState.IsValid)return View(model);
        await using var transaction=await dbContext.Database.BeginTransactionAsync();var account=new ApplicationUser{UserName=student.Email,Email=student.Email,EmailConfirmed=true,FirstName=student.FirstName,LastName=student.LastName,IsActive=student.IsActive,MustChangePassword=true};var result=await userManager.CreateAsync(account,model.TemporaryPassword);if(!result.Succeeded){foreach(var error in result.Errors)ModelState.AddModelError(nameof(model.TemporaryPassword),error.ToTurkish());return View(model);}var roleResult=await userManager.AddToRoleAsync(account,IdentityDataSeeder.StudentRole);if(!roleResult.Succeeded){ModelState.AddModelError(string.Empty,"Öğrenci rolü atanamadı.");return View(model);}student.ApplicationUserId=account.Id;student.UpdatedAt=DateTime.UtcNow;await dbContext.SaveChangesAsync();await transaction.CommitAsync();
        var message=$"Öğrenci hesabınız oluşturuldu. Kullanıcı adınız: {student.Email}. Geçici şifreniz: {model.TemporaryPassword}. İlk girişte şifrenizi değiştirin.";var safe=emailTemplates.Build("Öğrenci hesabınız hazır","Öğrenci hesabı oluşturuldu. Güvenlik nedeniyle geçici şifre gönderim geçmişinde saklanmamıştır.");await emailService.SendEmailAsync(student.Email,student.FullName,"Matematik Atölyesi öğrenci hesabınız",emailTemplates.Build("Öğrenci hesabınız hazır",message),"Yeni öğrenci hesabı",student.Id,safe);await audit.LogAsync("Öğrenci hesabı oluşturma","Öğrenci",student.Id,student.FullName);TempData["Success"]="Öğrenci giriş hesabı oluşturuldu.";return RedirectToAction(nameof(Details),new{id=student.Id});
    }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(StudentPasswordResetViewModel model)
    {
        var student=await dbContext.StudentProfiles.Include(x=>x.ApplicationUser).SingleOrDefaultAsync(x=>x.Id==model.StudentId);if(student is null||student.ApplicationUser is null)return NotFound();model.StudentName=student.FullName;if(!ModelState.IsValid)return View(model);
        var token=await userManager.GeneratePasswordResetTokenAsync(student.ApplicationUser);var result=await userManager.ResetPasswordAsync(student.ApplicationUser,token,model.TemporaryPassword);if(!result.Succeeded){foreach(var error in result.Errors)ModelState.AddModelError(nameof(model.TemporaryPassword),error.ToTurkish());return View(model);}student.ApplicationUser.MustChangePassword=true;student.ApplicationUser.UpdatedAt=DateTime.UtcNow;await userManager.UpdateAsync(student.ApplicationUser);
        var body=emailTemplates.Build("Şifreniz sıfırlandı",$"Yeni geçici şifreniz: {model.TemporaryPassword}. İlk girişten sonra şifrenizi değiştirin.");var safe=emailTemplates.Build("Şifreniz sıfırlandı","Güvenlik nedeniyle geçici şifre gönderim geçmişinde saklanmamıştır.");await emailService.SendEmailAsync(student.Email,student.FullName,"Matematik Atölyesi şifre sıfırlama",body,"Şifre sıfırlama",student.Id,safe);await audit.LogAsync("Şifre sıfırlama","Öğrenci",student.Id,student.FullName);TempData["Success"]="Öğrenci şifresi sıfırlandı.";return RedirectToAction(nameof(Details),new{id=student.Id});
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudentProfile model)
    {
        if (id != model.Id) return BadRequest();
        if (await dbContext.StudentProfiles.AnyAsync(student => student.Id != id && student.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "Bu e-posta adresi başka bir öğrenci tarafından kullanılıyor.");
        if (!ModelState.IsValid) return View(model);

        var student = await dbContext.StudentProfiles.Include(item => item.ApplicationUser).SingleOrDefaultAsync(item => item.Id == id);
        if (student is null) return NotFound();
        dbContext.Entry(student).CurrentValues.SetValues(model);
        student.UpdatedAt = DateTime.UtcNow;
        if (student.ApplicationUser is not null)
        {
            student.ApplicationUser.FirstName = model.FirstName;
            student.ApplicationUser.LastName = model.LastName;
            student.ApplicationUser.IsActive = model.IsActive;
            student.ApplicationUser.Email = model.Email;
            student.ApplicationUser.UserName = model.Email;
            student.ApplicationUser.UpdatedAt = DateTime.UtcNow;
            var result = await userManager.UpdateAsync(student.ApplicationUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.ToTurkish());
                return View(model);
            }
        }
        await dbContext.SaveChangesAsync();
        await audit.LogAsync("Öğrenci güncelleme", "Öğrenci", id, student.FullName);
        TempData["Success"] = "Öğrenci bilgileri güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var student = await dbContext.StudentProfiles.Include(item => item.ApplicationUser).SingleOrDefaultAsync(item => item.Id == id);
        if (student is null) return NotFound();
        student.IsActive = !student.IsActive;
        student.UpdatedAt = DateTime.UtcNow;
        if (student.ApplicationUser is not null) student.ApplicationUser.IsActive = student.IsActive;
        await dbContext.SaveChangesAsync();
        await audit.LogAsync(student.IsActive ? "Öğrenci aktifleştirme" : "Öğrenci pasifleştirme", "Öğrenci", id, student.FullName);
        TempData["Success"] = student.IsActive ? "Öğrenci aktifleştirildi." : "Öğrenci pasifleştirildi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await dbContext.StudentProfiles.Include(item => item.ApplicationUser).SingleOrDefaultAsync(item => item.Id == id);
        if (student is null) return NotFound();
        student.IsDeleted = true;
        student.IsActive = false;
        student.UpdatedAt = DateTime.UtcNow;
        if (student.ApplicationUser is not null) student.ApplicationUser.IsActive = false;
        await dbContext.SaveChangesAsync();
        await audit.LogAsync("Öğrenci arşivleme", "Öğrenci", id, student.FullName);
        TempData["Success"] = "Öğrenci akademik geçmişi korunarak arşivlendi.";
        return RedirectToAction(nameof(Index));
    }
}
