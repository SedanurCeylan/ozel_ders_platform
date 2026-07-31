using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.Lessons;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Notifications;
using OzelDersYonetim.Services.Auditing;
namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles=IdentityDataSeeder.AdminRole)]
public class LessonsController(ApplicationDbContext dbContext, INotificationService notifications, IEmailService emails, IEmailTemplateService templates, IAuditService audit) : Controller
{
    public async Task<IActionResult> Index(string scope="upcoming")
    {
        var now=DateTime.Now;
        var query=dbContext.Lessons.AsNoTracking().Include(x=>x.StudentLessons).ThenInclude(x=>x.StudentProfile).AsQueryable();
        query=scope switch { "past"=>query.Where(x=>x.EndDate<now), "today"=>query.Where(x=>x.StartDate.Date==now.Date), _=>query.Where(x=>x.EndDate>=now) };
        ViewBag.Scope=scope;
        return View(await query.OrderBy(x=>x.StartDate).ToListAsync());
    }
    public async Task<IActionResult> Details(int id)
    {
        var lesson=await dbContext.Lessons.AsNoTracking().Include(x=>x.StudentLessons).ThenInclude(x=>x.StudentProfile).SingleOrDefaultAsync(x=>x.Id==id);
        return lesson is null?NotFound():View(lesson);
    }
    public async Task<IActionResult> Create()=>View(await PrepareAsync(new LessonFormViewModel()));
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LessonFormViewModel model)
    {
        Validate(model); if(!ModelState.IsValid)return View(await PrepareAsync(model));
        model.Lesson.StudentLessons=model.SelectedStudentIds.Distinct().Select(id=>new StudentLesson{StudentProfileId=id}).ToList();
        dbContext.Lessons.Add(model.Lesson); await dbContext.SaveChangesAsync();
        await SendLessonNoticeAsync(model.Lesson, "Yeni ders planlandı", "Yeni ders");
        await audit.LogAsync("Ders oluşturma", "Ders", model.Lesson.Id, model.Lesson.Title);
        TempData["Success"]="Ders ve öğrenci atamaları oluşturuldu."; return RedirectToAction(nameof(Details),new{id=model.Lesson.Id});
    }
    public async Task<IActionResult> Edit(int id)
    {
        var lesson=await dbContext.Lessons.Include(x=>x.StudentLessons).SingleOrDefaultAsync(x=>x.Id==id); if(lesson is null)return NotFound();
        return View(await PrepareAsync(new LessonFormViewModel{Lesson=lesson,SelectedStudentIds=lesson.StudentLessons.Select(x=>x.StudentProfileId).ToList()}));
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,LessonFormViewModel model)
    {
        if(id!=model.Lesson.Id)return BadRequest(); Validate(model); if(!ModelState.IsValid)return View(await PrepareAsync(model));
        var lesson=await dbContext.Lessons.Include(x=>x.StudentLessons).SingleOrDefaultAsync(x=>x.Id==id); if(lesson is null)return NotFound();
        var dateChanged=lesson.StartDate!=model.Lesson.StartDate; dbContext.Entry(lesson).CurrentValues.SetValues(model.Lesson); lesson.UpdatedAt=DateTime.UtcNow;
        var selected=model.SelectedStudentIds.Distinct().ToHashSet(); dbContext.StudentLessons.RemoveRange(lesson.StudentLessons.Where(x=>!selected.Contains(x.StudentProfileId)));
        foreach(var studentId in selected.Except(lesson.StudentLessons.Select(x=>x.StudentProfileId)))lesson.StudentLessons.Add(new StudentLesson{StudentProfileId=studentId});
        await dbContext.SaveChangesAsync(); if(dateChanged)await SendLessonNoticeAsync(lesson,"Ders saati değişti","Ders saati değişikliği"); await audit.LogAsync("Ders güncelleme","Ders",id,lesson.Title); TempData["Success"]="Ders bilgileri güncellendi."; return RedirectToAction(nameof(Details),new{id});
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAttendance(int id,int studentLessonId,AttendanceStatus attendanceStatus,int? questionCount,string? performanceNote)
    {
        var item=await dbContext.StudentLessons.SingleOrDefaultAsync(x=>x.Id==studentLessonId&&x.LessonId==id); if(item is null)return NotFound();
        item.AttendanceStatus=attendanceStatus; item.QuestionCount=questionCount; item.PerformanceNote=performanceNote; item.UpdatedAt=DateTime.UtcNow;
        await dbContext.SaveChangesAsync(); TempData["Success"]="Katılım ve performans bilgisi kaydedildi."; return RedirectToAction(nameof(Details),new{id});
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson=await dbContext.Lessons.Include(x=>x.StudentLessons).SingleOrDefaultAsync(x=>x.Id==id); if(lesson is null)return NotFound();
        if(lesson.StudentLessons.Count>0){lesson.Status=LessonStatus.TeacherCancelled;lesson.UpdatedAt=DateTime.UtcNow;}else dbContext.Lessons.Remove(lesson);
        await dbContext.SaveChangesAsync(); if(lesson.StudentLessons.Count>0)await SendLessonNoticeAsync(lesson,"Ders iptal edildi","Ders iptali"); await audit.LogAsync("Ders iptal etme","Ders",id,lesson.Title); TempData["Success"]="Ders iptal edildi."; return RedirectToAction(nameof(Index));
    }
    private async Task<LessonFormViewModel> PrepareAsync(LessonFormViewModel model){model.Students=await dbContext.StudentProfiles.AsNoTracking().Where(x=>x.IsActive).OrderBy(x=>x.FirstName).Select(x=>new SelectListItem(x.FirstName+" "+x.LastName+" · "+x.GradeLevel,x.Id.ToString())).ToListAsync();return model;}
    private void Validate(LessonFormViewModel model){if(model.Lesson.EndDate<=model.Lesson.StartDate)ModelState.AddModelError("Lesson.EndDate","Bitiş zamanı başlangıçtan sonra olmalıdır.");if(model.SelectedStudentIds.Count==0)ModelState.AddModelError(nameof(model.SelectedStudentIds),"En az bir öğrenci seçin.");if(model.Lesson.LessonMode==LessonMode.Online&&string.IsNullOrWhiteSpace(model.Lesson.OnlineMeetingUrl))ModelState.AddModelError("Lesson.OnlineMeetingUrl","Online ders bağlantısı zorunludur.");}
    private async Task SendLessonNoticeAsync(Lesson lesson,string title,string type){var students=await dbContext.StudentLessons.Where(x=>x.LessonId==lesson.Id).Select(x=>x.StudentProfile).Where(x=>x.ApplicationUserId!=null).ToListAsync();var message=$"{lesson.Title} · {lesson.StartDate:dd.MM.yyyy HH:mm}";await notifications.CreateAsync(students.Select(x=>x.ApplicationUserId!),title,message,type,lesson.Id,"/Student/Lessons");if(lesson.SendEmailNotification)foreach(var student in students)await emails.SendEmailAsync(student.Email,student.FullName,title,templates.Build(title,message,lesson.StartDate),type,lesson.Id);}
}
