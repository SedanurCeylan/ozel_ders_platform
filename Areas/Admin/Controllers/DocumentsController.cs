using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Data;
using OzelDersYonetim.Models.ViewModels;
using OzelDersYonetim.Services.Documents;
using OzelDersYonetim.Services.Auditing;

namespace OzelDersYonetim.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class DocumentsController(ApplicationDbContext dbContext, IStudentDocumentService documentService, IWebHostEnvironment environment, IAuditService audit) : Controller
{
    public async Task<IActionResult> Index(string? category)
    {
        var query = dbContext.CourseDocuments.AsNoTracking().Include(x => x.StudentDocuments).OrderByDescending(x => x.CreatedAt).AsQueryable();
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category);
        ViewBag.Category = category; ViewBag.Categories = await dbContext.CourseDocuments.Select(x => x.Category).Distinct().OrderBy(x => x).ToListAsync();
        return View(await query.ToListAsync());
    }
    public async Task<IActionResult> Create() => View(await FormAsync(new DocumentFormViewModel()));
    public async Task<IActionResult> Edit(int id){var document=await dbContext.CourseDocuments.Include(x=>x.StudentDocuments).SingleOrDefaultAsync(x=>x.Id==id);if(document is null)return NotFound();return View(await FormAsync(new DocumentFormViewModel{Document=document,SelectedStudentIds=document.StudentDocuments.Select(x=>x.StudentProfileId).ToList()}));}

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> Create(DocumentFormViewModel model)
    {
        if (model.File is null) ModelState.AddModelError(nameof(model.File), "Yüklenecek dosyayı seçin.");
        if (!ModelState.IsValid) return View(await FormAsync(model));
        try { await documentService.CreateAsync(model.Document, model.File!, model.SelectedStudentIds); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(await FormAsync(model)); }
        await audit.LogAsync("Doküman yükleme", "Doküman", model.Document.Id, model.Document.Title);
        TempData["Success"] = "Doküman başarıyla paylaşıldı."; return RedirectToAction(nameof(Index));
    }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,DocumentFormViewModel model)
    {
        if(id!=model.Document.Id)return BadRequest();if(model.Document.AccessType==Models.Documents.DocumentAccessType.SelectedStudents&&model.SelectedStudentIds.Count==0)ModelState.AddModelError(nameof(model.SelectedStudentIds),"Özel paylaşım için en az bir öğrenci seçin.");if(!ModelState.IsValid)return View(await FormAsync(model));
        var document=await dbContext.CourseDocuments.Include(x=>x.StudentDocuments).SingleOrDefaultAsync(x=>x.Id==id);if(document is null)return NotFound();document.Title=model.Document.Title;document.Description=model.Document.Description;document.Category=model.Document.Category;document.AccessType=model.Document.AccessType;document.IsActive=model.Document.IsActive;document.UpdatedAt=DateTime.UtcNow;
        var selected=model.Document.AccessType==Models.Documents.DocumentAccessType.SelectedStudents?model.SelectedStudentIds.Distinct().ToHashSet():new HashSet<int>();var existing=document.StudentDocuments.Select(x=>x.StudentProfileId).ToHashSet();dbContext.StudentDocuments.RemoveRange(document.StudentDocuments.Where(x=>!selected.Contains(x.StudentProfileId)&&!x.IsViewed&&!x.IsDownloaded));foreach(var studentId in selected.Except(existing))document.StudentDocuments.Add(new Models.Documents.StudentDocument{StudentProfileId=studentId});await dbContext.SaveChangesAsync();await audit.LogAsync("Doküman güncelleme","Doküman",id,document.Title);TempData["Success"]="Doküman bilgileri ve erişim izinleri güncellendi.";return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download(int id)
    {
        var document = await dbContext.CourseDocuments.FindAsync(id); if (document is null) return NotFound();
        var path = Path.Combine(environment.ContentRootPath, "App_Data", "uploads", "documents", document.StoredFilePath); if (!System.IO.File.Exists(path)) return NotFound();
        return File(System.IO.File.OpenRead(path), document.ContentType, document.OriginalFileName);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id) { var document = await dbContext.CourseDocuments.FindAsync(id); if (document is null) return NotFound(); document.IsActive = !document.IsActive; document.UpdatedAt = DateTime.UtcNow; await dbContext.SaveChangesAsync(); TempData["Success"] = document.IsActive ? "Doküman etkinleştirildi." : "Doküman pasifleştirildi."; return RedirectToAction(nameof(Index)); }

    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id){var document=await dbContext.CourseDocuments.FindAsync(id);if(document is null)return NotFound();document.IsActive=false;document.UpdatedAt=DateTime.UtcNow;await dbContext.SaveChangesAsync();await audit.LogAsync("Doküman kaldırma","Doküman",id,document.Title);TempData["Success"]="Doküman erişime kapatıldı; indirme geçmişi korundu.";return RedirectToAction(nameof(Index));}

    private async Task<DocumentFormViewModel> FormAsync(DocumentFormViewModel model) { model.Students = await dbContext.StudentProfiles.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.FirstName).Select(x => new SelectListItem(x.FullName + " · " + x.GradeLevel, x.Id.ToString())).ToListAsync(); return model; }
}
